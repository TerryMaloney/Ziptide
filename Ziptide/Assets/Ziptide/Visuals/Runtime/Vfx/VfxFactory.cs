using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.5 commit 2 — the bounded runtime half of the VFX vocabulary.
    ///
    /// The factory is scene-local by design: it is created lazily in the active world, then normal
    /// scene unload destroys its pool and shared resources. It is not a new persistent gameplay owner.
    /// At most six ParticleSystems are ever allocated or active; inactive systems are pooled by kind
    /// and may be reconfigured across kinds instead of growing the pool.
    /// </summary>
    public sealed class VfxFactory : MonoBehaviour
    {
        private const string FactoryObjectName = "__VFX_FACTORY";
        private const string SharedMaterialName = "Ziptide_VFX_Shared";
        private const string SharedTextureName = "Ziptide_VFX_SoftParticle";
        private const int SoftTextureSize = 32;

        private static VfxFactory _instance;

        private readonly Dictionary<VfxKind, List<VfxPooledInstance>> _byKind =
            new Dictionary<VfxKind, List<VfxPooledInstance>>();
        private readonly List<VfxPooledInstance> _all = new List<VfxPooledInstance>();

        private Material _sharedMaterial;
        private Texture2D _sharedTexture;
        private int _activeCount;

        /// <summary>Current live effect count. Diagnostic/test seam; always within the six-system rail.</summary>
        public static int ActiveCount => _instance != null ? _instance._activeCount : 0;

        /// <summary>Total allocated systems across all kind buckets. Never exceeds MaxLiveSystems.</summary>
        public static int PooledCount => _instance != null ? _instance._all.Count : 0;

        /// <summary>
        /// Spawn one effect from the closed VfxLibrary vocabulary. Unknown ids and exhausted budgets
        /// are safe no-ops. The returned ParticleSystem can be passed to <see cref="Stop"/> for looping
        /// effects; one-shots return themselves to the pool through ParticleSystemStopAction.Callback.
        /// </summary>
        public static ParticleSystem Spawn(string id, Vector3 position, Vector3 normal)
        {
            VfxRecipeDefinition recipe = VfxLibrary.Get(id);
            if (recipe == null)
            {
                Debug.LogWarning("ZIPTIDE: VFX_MISSING id=" + (id ?? "<null>"));
                return null;
            }

            return EnsureInstance().SpawnInternal(recipe, position, normal);
        }

        /// <summary>Stop and return a factory-owned looping or one-shot effect to the bounded pool.</summary>
        public static bool Stop(ParticleSystem system)
        {
            if (system == null) return false;
            var pooled = system.GetComponent<VfxPooledInstance>();
            return pooled != null && pooled.Owner != null && pooled.Owner.Release(pooled);
        }

        private static VfxFactory EnsureInstance()
        {
            if (_instance != null) return _instance;

            var root = new GameObject(FactoryObjectName)
            {
                hideFlags = HideFlags.HideInHierarchy
            };
            _instance = root.AddComponent<VfxFactory>();
            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                DestroyObject(gameObject);
                return;
            }
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance != this) return;

            for (int i = 0; i < _all.Count; i++)
            {
                VfxPooledInstance pooled = _all[i];
                if (pooled == null || pooled.System == null) continue;
                pooled.MarkInactive();
                pooled.System.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            _activeCount = 0;
            _all.Clear();
            _byKind.Clear();

            DestroyObject(_sharedMaterial);
            DestroyObject(_sharedTexture);
            _sharedMaterial = null;
            _sharedTexture = null;
            _instance = null;
        }

        private ParticleSystem SpawnInternal(
            VfxRecipeDefinition recipe,
            Vector3 position,
            Vector3 normal)
        {
            if (_activeCount >= VfxRecipeDefinition.MaxLiveSystems)
            {
                Debug.LogWarning("ZIPTIDE: VFX_DROPPED reason=live_cap id=" + recipe.id);
                return null;
            }

            bool reused;
            VfxPooledInstance pooled = Acquire(recipe.kind, out reused);
            if (pooled == null)
            {
                Debug.LogWarning("ZIPTIDE: VFX_DROPPED reason=pool_unavailable id=" + recipe.id);
                return null;
            }

            Transform effectTransform = pooled.transform;
            effectTransform.SetPositionAndRotation(position, RotationFromNormal(normal));
            pooled.gameObject.name = "VFX_" + recipe.id;
            pooled.gameObject.SetActive(true);
            pooled.MarkActive(this, recipe.kind, recipe.id, recipe.oneShot);

            Configure(pooled.System, recipe);
            _activeCount++;
            pooled.System.Play(true);

            Debug.Log("ZIPTIDE: VFX_SPAWN id=" + recipe.id +
                      " kind=" + recipe.kind +
                      " pooled=" + reused +
                      " active=" + _activeCount);
            return pooled.System;
        }

        private VfxPooledInstance Acquire(VfxKind kind, out bool reused)
        {
            reused = false;
            List<VfxPooledInstance> bucket = Bucket(kind);
            for (int i = 0; i < bucket.Count; i++)
            {
                VfxPooledInstance pooled = bucket[i];
                if (pooled != null && !pooled.IsActive)
                {
                    reused = true;
                    return pooled;
                }
            }

            if (_all.Count < VfxRecipeDefinition.MaxLiveSystems)
                return CreatePooled(kind);

            // The total allocation rail is already full, but the live rail is not: recycle any
            // inactive system across kinds rather than allowing a long session to grow the pool.
            for (int i = 0; i < _all.Count; i++)
            {
                VfxPooledInstance pooled = _all[i];
                if (pooled == null || pooled.IsActive) continue;

                Bucket(pooled.Kind).Remove(pooled);
                pooled.SetKind(kind);
                bucket.Add(pooled);
                reused = true;
                return pooled;
            }

            return null;
        }

        private VfxPooledInstance CreatePooled(VfxKind kind)
        {
            var go = new GameObject("VFX_POOL_" + kind + "_" + _all.Count);
            go.SetActive(false);
            go.transform.SetParent(transform, false);

            ParticleSystem system = go.AddComponent<ParticleSystem>();
            var main = system.main;
            main.playOnAwake = false;

            var pooled = go.AddComponent<VfxPooledInstance>();
            pooled.Initialize(this, system, kind);

            _all.Add(pooled);
            Bucket(kind).Add(pooled);
            return pooled;
        }

        private List<VfxPooledInstance> Bucket(VfxKind kind)
        {
            if (!_byKind.TryGetValue(kind, out List<VfxPooledInstance> bucket))
            {
                bucket = new List<VfxPooledInstance>();
                _byKind.Add(kind, bucket);
            }
            return bucket;
        }

        internal bool Release(VfxPooledInstance pooled)
        {
            if (pooled == null || !pooled.IsActive) return false;

            pooled.MarkInactive();
            _activeCount = Mathf.Max(0, _activeCount - 1);
            if (pooled.System != null)
            {
                pooled.System.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                pooled.System.Clear(true);
            }
            pooled.gameObject.SetActive(false);
            return true;
        }

        private void Configure(ParticleSystem system, VfxRecipeDefinition recipe)
        {
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            system.Clear(true);

            var main = system.main;
            main.playOnAwake = false;
            main.loop = !recipe.oneShot;
            main.prewarm = false;
            main.duration = Mathf.Max(0.05f, recipe.lifetimeMax);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.maxParticles = Mathf.Clamp(
                recipe.PeakParticles(),
                1,
                VfxRecipeDefinition.MaxParticles);
            main.startLifetime = new ParticleSystem.MinMaxCurve(recipe.lifetimeMin, recipe.lifetimeMax);
            main.startSpeed = new ParticleSystem.MinMaxCurve(recipe.speedMin, recipe.speedMax);
            main.startSize = new ParticleSystem.MinMaxCurve(recipe.sizeMin, recipe.sizeMax);
            main.startColor = Color.white;
            main.gravityModifier = recipe.gravity;
            main.stopAction = recipe.oneShot
                ? ParticleSystemStopAction.Callback
                : ParticleSystemStopAction.None;

            var emission = system.emission;
            emission.enabled = true;
            emission.rateOverTime = recipe.oneShot ? 0f : recipe.rateOverTime;
            emission.SetBursts(Array.Empty<ParticleSystem.Burst>());
            if (recipe.burstCount > 0)
            {
                var burst = new ParticleSystem.Burst(0f, (short)recipe.burstCount);
                emission.SetBursts(new[] { burst });
            }

            var color = system.colorOverLifetime;
            color.enabled = true;
            color.color = new ParticleSystem.MinMaxGradient(ColorGradient(recipe));

            var size = system.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, SizeCurve(recipe.kind));

            ConfigureShape(system, recipe.kind);
            ConfigureRenderer(system, recipe.kind);
        }

        private static void ConfigureShape(ParticleSystem system, VfxKind kind)
        {
            var shape = system.shape;
            shape.enabled = true;
            shape.scale = Vector3.one;
            shape.radiusThickness = 1f;

            switch (kind)
            {
                case VfxKind.Muzzle:
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 9f;
                    shape.radius = 0.008f;
                    break;
                case VfxKind.SteamVent:
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 12f;
                    shape.radius = 0.05f;
                    break;
                case VfxKind.Motes:
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 1.25f;
                    shape.radiusThickness = 1f;
                    break;
                case VfxKind.Sparks:
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 24f;
                    shape.radius = 0.015f;
                    break;
                case VfxKind.Drips:
                    shape.shapeType = ParticleSystemShapeType.Box;
                    shape.scale = new Vector3(0.18f, 0.02f, 0.18f);
                    break;
                default:
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 34f;
                    shape.radius = 0.025f;
                    break;
            }
        }

        private void ConfigureRenderer(ParticleSystem system, VfxKind kind)
        {
            ParticleSystemRenderer renderer = system.GetComponent<ParticleSystemRenderer>();
            if (renderer == null) return;

            renderer.sharedMaterial = SharedMaterial();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            renderer.sortMode = ParticleSystemSortMode.Distance;
            renderer.minParticleSize = 0f;
            renderer.maxParticleSize = 0.08f;

            bool streak = kind == VfxKind.Muzzle || kind == VfxKind.Sparks;
            renderer.renderMode = streak
                ? ParticleSystemRenderMode.Stretch
                : ParticleSystemRenderMode.Billboard;
            renderer.alignment = ParticleSystemRenderSpace.View;
            renderer.velocityScale = streak ? 0.08f : 0f;
            renderer.lengthScale = streak ? 0.35f : 1f;
        }

        private Material SharedMaterial()
        {
            if (_sharedMaterial != null) return _sharedMaterial;

            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                Debug.LogWarning("ZIPTIDE: VFX_MATERIAL_MISSING reason=no_supported_shader");
                return null;
            }

            _sharedTexture = SoftParticleTexture();
            _sharedMaterial = new Material(shader)
            {
                name = SharedMaterialName,
                hideFlags = HideFlags.HideAndDontSave,
                enableInstancing = true,
                renderQueue = (int)RenderQueue.Transparent
            };

            if (_sharedMaterial.HasProperty("_BaseMap"))
                _sharedMaterial.SetTexture("_BaseMap", _sharedTexture);
            if (_sharedMaterial.HasProperty("_MainTex"))
                _sharedMaterial.SetTexture("_MainTex", _sharedTexture);
            if (_sharedMaterial.HasProperty("_BaseColor"))
                _sharedMaterial.SetColor("_BaseColor", Color.white);
            if (_sharedMaterial.HasProperty("_Color"))
                _sharedMaterial.SetColor("_Color", Color.white);
            return _sharedMaterial;
        }

        private static Texture2D SoftParticleTexture()
        {
            var texture = new Texture2D(
                SoftTextureSize,
                SoftTextureSize,
                TextureFormat.RGBA32,
                false,
                true)
            {
                name = SharedTextureName,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[SoftTextureSize * SoftTextureSize];
            for (int y = 0; y < SoftTextureSize; y++)
            {
                for (int x = 0; x < SoftTextureSize; x++)
                {
                    float u = ((x + 0.5f) / SoftTextureSize) * 2f - 1f;
                    float v = ((y + 0.5f) / SoftTextureSize) * 2f - 1f;
                    float distance = Mathf.Sqrt(u * u + v * v);
                    float alpha = Mathf.Clamp01(1f - distance);
                    alpha = alpha * alpha * (3f - 2f * alpha);
                    pixels[y * SoftTextureSize + x] = new Color32(
                        255,
                        255,
                        255,
                        (byte)Mathf.RoundToInt(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return texture;
        }

        private static Gradient ColorGradient(VfxRecipeDefinition recipe)
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(recipe.colorStart, 0f),
                    new GradientColorKey(recipe.colorEnd, 1f),
                },
                new[]
                {
                    new GradientAlphaKey(recipe.colorStart.a, 0f),
                    new GradientAlphaKey(recipe.colorEnd.a, 1f),
                });
            return gradient;
        }

        private static AnimationCurve SizeCurve(VfxKind kind)
        {
            switch (kind)
            {
                case VfxKind.SteamVent:
                    return AnimationCurve.Linear(0f, 0.55f, 1f, 1.45f);
                case VfxKind.Motes:
                    return AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1f);
                case VfxKind.Sparks:
                case VfxKind.Muzzle:
                    return AnimationCurve.Linear(0f, 1f, 1f, 0.05f);
                default:
                    return AnimationCurve.EaseInOut(0f, 0.45f, 1f, 1.1f);
            }
        }

        private static Quaternion RotationFromNormal(Vector3 normal)
        {
            Vector3 direction = normal.sqrMagnitude > 0.000001f
                ? normal.normalized
                : Vector3.up;
            Vector3 up = Mathf.Abs(Vector3.Dot(direction, Vector3.up)) > 0.98f
                ? Vector3.forward
                : Vector3.up;
            return Quaternion.LookRotation(direction, up);
        }

        private static void DestroyObject(UnityEngine.Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) Destroy(value);
            else DestroyImmediate(value);
        }
    }

    /// <summary>Bookkeeping component attached to each pooled ParticleSystem.</summary>
    internal sealed class VfxPooledInstance : MonoBehaviour
    {
        public VfxFactory Owner { get; private set; }
        public ParticleSystem System { get; private set; }
        public VfxKind Kind { get; private set; }
        public string RecipeId { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsOneShot { get; private set; }

        public void Initialize(VfxFactory owner, ParticleSystem system, VfxKind kind)
        {
            Owner = owner;
            System = system;
            Kind = kind;
        }

        public void SetKind(VfxKind kind)
        {
            Kind = kind;
        }

        public void MarkActive(
            VfxFactory owner,
            VfxKind kind,
            string recipeId,
            bool oneShot)
        {
            Owner = owner;
            Kind = kind;
            RecipeId = recipeId;
            IsOneShot = oneShot;
            IsActive = true;
        }

        public void MarkInactive()
        {
            IsActive = false;
            RecipeId = null;
            IsOneShot = false;
        }

        private void OnParticleSystemStopped()
        {
            if (IsActive && IsOneShot && Owner != null)
                Owner.Release(this);
        }
    }
}

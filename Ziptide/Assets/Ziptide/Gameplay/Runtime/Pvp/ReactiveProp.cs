using System;
using UnityEngine;
using Ziptide.Multiplayer;
using Ziptide.Visuals;

namespace Ziptide.Gameplay
{
    /// <summary>The four closed environmental reactions authorized by FORGE III F3.6.</summary>
    public enum ReactionKind
    {
        LightFlickerOut,
        SteamBurst,
        SparkShower,
        Shatter
    }

    /// <summary>Pure reaction vocabulary and timing law shared by runtime and EditMode tests.</summary>
    public static class ReactivePropRules
    {
        public const double DefaultCooldownSeconds = 30.0;
        public const float SteamBurstSeconds = 0.85f;
        public const int ShatterChunkCount = 3;

        public static string VfxId(ReactionKind kind)
        {
            switch (kind)
            {
                case ReactionKind.LightFlickerOut: return "sparks_short";
                case ReactionKind.SteamBurst: return "steam_vent";
                case ReactionKind.SparkShower: return "sparks_short";
                case ReactionKind.Shatter: return "impact_stone";
                default: return null;
            }
        }

        public static bool IsOneShot(ReactionKind kind)
        {
            return kind != ReactionKind.SteamBurst;
        }
    }

    /// <summary>
    /// Pure injected-clock state: one-shot props react once; repeatable props reopen exactly after
    /// their cooldown. No Unity, VFX, save state, loot or navigation behavior lives here.
    /// </summary>
    public sealed class ReactivePropState
    {
        private readonly bool _oneShot;
        private readonly double _cooldownSeconds;
        private bool _spent;
        private double _readyAt = double.NegativeInfinity;

        public int ReactionCount { get; private set; }
        public bool IsSpent => _spent;
        public double ReadyAt => _readyAt;

        public ReactivePropState(bool oneShot, double cooldownSeconds)
        {
            _oneShot = oneShot;
            _cooldownSeconds = Math.Max(0.0, cooldownSeconds);
        }

        public bool CanReact(double now)
        {
            return !_spent && now >= _readyAt;
        }

        public bool TryReact(double now)
        {
            if (!CanReact(now)) return false;

            ReactionCount++;
            if (_oneShot) _spent = true;
            else _readyAt = now + _cooldownSeconds;
            return true;
        }

        public void Reset()
        {
            ReactionCount = 0;
            _spent = false;
            _readyAt = double.NegativeInfinity;
        }
    }

    /// <summary>
    /// Small environmental damage target. Existing PvP weapons discover it through IPvpDamageable;
    /// the prop never joins scoring (PlayerIndex=-1), never drops rewards, and never changes authored
    /// structural/navigation colliders. A dedicated author-owned projectile proxy may be disabled when
    /// a one-shot visual disappears.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ReactiveProp : MonoBehaviour, IPvpDamageable
    {
        [SerializeField] private ReactionKind reactionKind;
        [SerializeField] private float cooldownSeconds = (float)ReactivePropRules.DefaultCooldownSeconds;
        [SerializeField] private Collider projectileHitProxy;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private ReactivePropState _state;
        private ParticleSystem _timedVfx;
        private float _timedVfxStopAt = -1f;

        public ReactionKind Kind => reactionKind;
        public float CooldownSeconds => cooldownSeconds;
        public Collider ProjectileHitProxy => projectileHitProxy;
        public int PlayerIndex => -1;
        public bool IsAlive => State.CanReact(Time.time);
        public int ReactionCount => State.ReactionCount;

        private ReactivePropState State
        {
            get
            {
                if (_state == null)
                {
                    _state = new ReactivePropState(
                        ReactivePropRules.IsOneShot(reactionKind),
                        cooldownSeconds);
                }
                return _state;
            }
        }

        /// <summary>Editor-authoring seam. Reconfiguration resets only the pure reaction clock.</summary>
        public void Configure(ReactionKind kind, Collider hitProxy = null, float cooldown = -1f)
        {
            reactionKind = kind;
            projectileHitProxy = hitProxy;
            cooldownSeconds = cooldown >= 0f
                ? cooldown
                : (float)ReactivePropRules.DefaultCooldownSeconds;
            _state = new ReactivePropState(
                ReactivePropRules.IsOneShot(reactionKind),
                cooldownSeconds);
        }

        public void ReceiveHit(PvpWeapon weapon, Vector3 point, Vector3 dir)
        {
            TryReact(Time.time, point, dir);
        }

        /// <summary>Injected-clock seam used by headless tests and the existing weapon interface.</summary>
        public bool TryReact(double now, Vector3 point, Vector3 direction)
        {
            if (!State.TryReact(now)) return false;

            Vector3 normal = direction.sqrMagnitude > 0.000001f
                ? -direction.normalized
                : transform.forward;
            string vfxId = ReactivePropRules.VfxId(reactionKind);
            ParticleSystem spawned = string.IsNullOrEmpty(vfxId)
                ? null
                : VfxFactory.Spawn(vfxId, point, normal);

            switch (reactionKind)
            {
                case ReactionKind.LightFlickerOut:
                    ApplyLightShutdown();
                    break;
                case ReactionKind.SteamBurst:
                    StartTimedVfx(spawned, ReactivePropRules.SteamBurstSeconds);
                    break;
                case ReactionKind.SparkShower:
                    DimRenderers();
                    break;
                case ReactionKind.Shatter:
                    ApplyShatter(point, normal);
                    break;
            }

            Debug.Log("ZIPTIDE: REACTIVE_PROP kind=" + reactionKind +
                      " count=" + State.ReactionCount +
                      " vfx=" + (vfxId ?? "none") +
                      " name=" + gameObject.name);
            return true;
        }

        public void ResetReactionState()
        {
            State.Reset();
        }

        public bool StopTimedVfx()
        {
            if (_timedVfx == null) return false;
            ParticleSystem value = _timedVfx;
            _timedVfx = null;
            _timedVfxStopAt = -1f;
            return VfxFactory.Stop(value);
        }

        private void Update()
        {
            if (_timedVfx != null && Time.time >= _timedVfxStopAt)
                StopTimedVfx();
        }

        private void OnDisable()
        {
            StopTimedVfx();
        }

        private void OnDestroy()
        {
            StopTimedVfx();
        }

        private void ApplyLightShutdown()
        {
            PracticalLight practical = GetComponent<PracticalLight>();
            if (practical == null) practical = GetComponentInChildren<PracticalLight>(true);
            if (practical != null) practical.SetLit(false);

            // PracticalAuthor's <=2 hero point lights are authored children/components of the same prop.
            foreach (Light light in GetComponentsInChildren<Light>(true))
                light.enabled = false;
        }

        private void StartTimedVfx(ParticleSystem system, float seconds)
        {
            StopTimedVfx();
            if (system == null) return;
            _timedVfx = system;
            _timedVfxStopAt = Time.time + Mathf.Max(0.05f, seconds);
        }

        private void DimRenderers()
        {
            var block = new MaterialPropertyBlock();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null) continue;
                renderer.GetPropertyBlock(block);
                Color dim = new Color(0.28f, 0.28f, 0.28f, 1f);
                block.SetColor(BaseColorId, dim);
                block.SetColor(ColorId, dim);
                block.SetColor(EmissionColorId, Color.black);
                renderer.SetPropertyBlock(block);
                block.Clear();
            }
        }

        private void ApplyShatter(Vector3 point, Vector3 normal)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            Bounds bounds = new Bounds(transform.position, Vector3.one * 0.25f);
            Material material = null;
            bool found = false;
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || !renderer.enabled) continue;
                if (!found) { bounds = renderer.bounds; found = true; }
                else bounds.Encapsulate(renderer.bounds);
                if (material == null) material = renderer.sharedMaterial;
            }

            Vector3 size = bounds.size;
            size.x = Mathf.Clamp(size.x * 0.34f, 0.08f, 0.45f);
            size.y = Mathf.Clamp(size.y * 0.36f, 0.08f, 0.45f);
            size.z = Mathf.Clamp(size.z * 0.70f, 0.06f, 0.35f);
            Vector3 away = normal.sqrMagnitude > 0.000001f ? normal.normalized : transform.forward;

            for (int i = 0; i < ReactivePropRules.ShatterChunkCount; i++)
            {
                float side = i - 1f;
                Vector3 offset = transform.right * side * Mathf.Max(0.08f, bounds.extents.x * 0.35f)
                               + Vector3.up * (0.04f + i * 0.035f);
                Vector3 velocity = away * (0.65f + Hash01(i + 17) * 0.45f)
                                 + Vector3.up * (0.55f + Hash01(i + 31) * 0.45f)
                                 + transform.right * side * 0.35f;
                Vector3 angular = new Vector3(
                    Hash01(i + 43) - 0.5f,
                    Hash01(i + 59) - 0.5f,
                    Hash01(i + 71) - 0.5f) * 4f;
                WorldDebrisBudget.SpawnChunk(
                    bounds.center + offset,
                    transform.rotation,
                    size,
                    velocity,
                    angular,
                    material);
            }

            foreach (Renderer renderer in renderers)
                if (renderer != null) renderer.enabled = false;

            // Only the dedicated projectile proxy is retired. Structural/navigation colliders are not
            // discovered or modified by this component.
            if (projectileHitProxy != null) projectileHitProxy.enabled = false;
        }

        private static float Hash01(int value)
        {
            unchecked
            {
                int hash = value * 374761393 + 1013904223;
                hash = (hash ^ (hash >> 13)) * 1103515245;
                hash ^= hash >> 16;
                return (hash & 0x7FFFFFFF) / 2147483647f;
            }
        }
    }
}

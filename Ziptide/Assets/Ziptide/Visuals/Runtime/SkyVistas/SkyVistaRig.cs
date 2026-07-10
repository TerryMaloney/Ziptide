using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Runtime renderer for a <see cref="SkyVistaDefinition"/>: one inverted dome sphere carrying a
    /// single composited 1024×512 bake (gradient + stars + nebula + Shell grid + shimmer — the grid is
    /// a texture layer, never a second dome) plus up to three body spheres. URP/Unlit, no shadows, no
    /// colliders; ≤4 draw calls / ≤5 materials / ~3k tris — far inside the Quest budget. Textures bake
    /// on world entry (~tens of ms, inside the travel transition). Created and driven by
    /// <see cref="SkyPlanetRig"/> when the active theme carries a vista; Quest-safe like its sibling.
    /// </summary>
    public class SkyVistaRig : MonoBehaviour
    {
        private const string URPUnlitShaderName = "Universal Render Pipeline/Unlit";
        private const string ShaderBaseMap = "_BaseMap";
        private const int DomeTextureWidth = 1024;
        private const int DomeTextureHeight = 512;
        private const int BodyTextureWidth = 512;
        private const int BodyTextureHeight = 256;
        private const float DomeRadius = 500f;
        private const float BodyDistance = 420f; // inside the dome

        private GameObject _domeRoot;
        private Material _domeMaterial;
        private Texture2D _domeTexture;
        private readonly List<BodyInstance> _bodies = new List<BodyInstance>();
        private Transform _playerTransform;
        private SkyVistaDefinition _vista;

        private class BodyInstance
        {
            public GameObject go;
            public Material material;
            public Texture2D texture;
            public SkyVistaDefinition.CelestialBodyDef def;
        }

        /// <summary>Build (or rebuild) the full skyscape from the vista. Idempotent per vista.</summary>
        public void ApplyVista(SkyVistaDefinition vista, Transform playerTransform)
        {
            if (vista == null) return;
            _playerTransform = playerTransform != null ? playerTransform : transform;

            bool sameVista = _vista == vista && _domeRoot != null;
            _vista = vista;

            EnsureDome();
            if (!sameVista)
            {
                BakeDome(vista);
                RebuildBodies(vista);
                ApplyAtmosphere(vista);
            }
            PositionAll();
            ApplySceneTieIns(vista);
        }

        /// <summary>SKYSCAPE layers (haze/motes/glow) ride a child rig; it drives itself per-frame.</summary>
        private SkyAtmosphereRig _atmosphere;

        private void ApplyAtmosphere(SkyVistaDefinition vista)
        {
            if (_atmosphere == null)
            {
                var go = new GameObject("SkyAtmosphere");
                go.transform.SetParent(transform, false);
                _atmosphere = go.AddComponent<SkyAtmosphereRig>();
            }
            _atmosphere.Apply(vista, _playerTransform);
        }

        private void Update()
        {
            if (_vista == null) return;
            PositionAll();
            foreach (var b in _bodies)
                if (b.go != null && b.def != null && b.def.rotationSpeedDeg != 0f)
                    b.go.transform.Rotate(Vector3.up, b.def.rotationSpeedDeg * Time.deltaTime);
        }

        // ── Dome ─────────────────────────────────────────────────────────────

        private void EnsureDome()
        {
            if (_domeRoot != null) return;

            _domeRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _domeRoot.name = "SkyVistaDome";
            _domeRoot.transform.SetParent(transform);
            _domeRoot.transform.localPosition = Vector3.zero;
            _domeRoot.transform.localRotation = Quaternion.identity;
            // Negative scale flips the sphere inside-out, same trick as SkyPlanetRig.
            _domeRoot.transform.localScale = new Vector3(-DomeRadius * 2f, -DomeRadius * 2f, -DomeRadius * 2f);
            Destroy(_domeRoot.GetComponent<Collider>());

            Shader unlit = Shader.Find(URPUnlitShaderName);
            if (unlit == null) return;
            _domeMaterial = new Material(unlit)
            {
                name = "SkyVistaRig_Dome",
                hideFlags = HideFlags.HideAndDontSave,
                renderQueue = 1000
            };

            var r = _domeRoot.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = _domeMaterial;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
            }
        }

        private void BakeDome(SkyVistaDefinition vista)
        {
            if (_domeMaterial == null) return;

            if (_domeTexture == null)
            {
                _domeTexture = new Texture2D(DomeTextureWidth, DomeTextureHeight, TextureFormat.RGBA32, false)
                {
                    wrapModeU = TextureWrapMode.Repeat,  // azimuth wraps
                    wrapModeV = TextureWrapMode.Clamp    // horizon→zenith clamps
                };
            }

            var pixels = new Color32[DomeTextureWidth * DomeTextureHeight];
            SkyVistaTexture.BakeDome(vista, DomeTextureWidth, DomeTextureHeight, pixels);
            _domeTexture.SetPixels32(pixels);
            _domeTexture.Apply(false, false);

            if (_domeMaterial.HasProperty(Shader.PropertyToID(ShaderBaseMap)))
                _domeMaterial.SetTexture(ShaderBaseMap, _domeTexture);
        }

        // ── Bodies ───────────────────────────────────────────────────────────

        private void RebuildBodies(SkyVistaDefinition vista)
        {
            foreach (var b in _bodies)
            {
                if (b.go != null) Destroy(b.go);
                if (b.material != null) Destroy(b.material);
                if (b.texture != null) Destroy(b.texture);
            }
            _bodies.Clear();

            if (vista.bodies == null) return;
            Shader unlit = Shader.Find(URPUnlitShaderName);
            if (unlit == null) return;

            int count = Mathf.Min(vista.bodies.Count, SkyVistaDefinition.MaxBodies);
            for (int i = 0; i < count; i++)
            {
                var def = vista.bodies[i];
                if (def == null) continue;

                var inst = new BodyInstance { def = def };
                inst.go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                inst.go.name = "SkyVistaBody_" + def.type + "_" + i;
                inst.go.transform.SetParent(transform);
                Destroy(inst.go.GetComponent<Collider>());

                inst.texture = new Texture2D(BodyTextureWidth, BodyTextureHeight, TextureFormat.RGBA32, false)
                {
                    wrapModeU = TextureWrapMode.Repeat,
                    wrapModeV = TextureWrapMode.Clamp
                };
                var pixels = new Color32[BodyTextureWidth * BodyTextureHeight];
                SkyVistaTexture.BakeBody(def, BodyTextureWidth, BodyTextureHeight, pixels);
                inst.texture.SetPixels32(pixels);
                inst.texture.Apply(false, false);

                inst.material = new Material(unlit)
                {
                    name = "SkyVistaRig_Body" + i,
                    hideFlags = HideFlags.HideAndDontSave,
                    renderQueue = 1001 // over the dome, under everything else
                };
                if (inst.material.HasProperty(Shader.PropertyToID(ShaderBaseMap)))
                    inst.material.SetTexture(ShaderBaseMap, inst.texture);

                var r = inst.go.GetComponent<Renderer>();
                if (r != null)
                {
                    r.sharedMaterial = inst.material;
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    r.receiveShadows = false;
                }

                _bodies.Add(inst);
            }
        }

        private void PositionAll()
        {
            Vector3 center = _playerTransform != null ? _playerTransform.position : transform.position;
            if (_domeRoot != null) _domeRoot.transform.position = center;

            foreach (var b in _bodies)
            {
                if (b.go == null || b.def == null) continue;
                Vector3 dir = b.def.direction.sqrMagnitude > 0.01f
                    ? b.def.direction.normalized
                    : new Vector3(0f, 0.45f, 0.89f);
                b.go.transform.position = center + dir * BodyDistance;
                // World diameter that subtends angularSizeDeg at BodyDistance.
                float diameter = Mathf.Tan(b.def.angularSizeDeg * Mathf.Deg2Rad * 0.5f) * BodyDistance * 2f;
                b.go.transform.localScale = Vector3.one * diameter;
            }
        }

        // ── Scene tie-ins (after the theme's base pass) ──────────────────────

        private void ApplySceneTieIns(SkyVistaDefinition vista)
        {
            if (vista.directionalLightIntensity > 0f)
            {
                Light sun = FindDirectionalLight();
                if (sun != null)
                {
                    sun.color = vista.directionalLightColor;
                    sun.intensity = vista.directionalLightIntensity;
                }
            }

            if (vista.overrideAmbient)
            {
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                RenderSettings.ambientLight = vista.ambientColor;
            }

            if (vista.overrideFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogColor = vista.fogColor;
                RenderSettings.fogDensity = vista.fogDensity;
            }
        }

        private static Light FindDirectionalLight()
        {
            foreach (var l in FindObjectsOfType<Light>())
                if (l != null && l.type == LightType.Directional) return l;
            return null;
        }

        private void OnDestroy()
        {
            if (_domeMaterial != null) Destroy(_domeMaterial);
            if (_domeTexture != null) Destroy(_domeTexture);
            foreach (var b in _bodies)
            {
                if (b.material != null) Destroy(b.material);
                if (b.texture != null) Destroy(b.texture);
            }
        }
    }
}

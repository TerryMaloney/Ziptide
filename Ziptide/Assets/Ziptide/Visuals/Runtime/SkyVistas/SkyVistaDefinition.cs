using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// A world's complete skyscape as data: layered dome (gradient + stars + nebula + the Shell grid +
    /// zenith shimmer) plus up to three celestial bodies, with optional light/ambient/fog tie-ins.
    /// Referenced by <see cref="VisualThemeProfile.skyVista"/>; rendered by <c>SkyVistaRig</c>;
    /// authored per world by the editor-side SkyVistaLibrary (create-only — the asset is the live truth).
    /// The story-canon progression (banded giant grows, Shell grid 0→1 across W001→W012) lives in the
    /// authored assets, never in code.
    /// </summary>
    [CreateAssetMenu(fileName = "SkyVista", menuName = "Ziptide/Sky Vista Definition")]
    public class SkyVistaDefinition : ScriptableObject
    {
        public const int MaxBodies = 3;

        [Tooltip("Stable id (e.g. 'w003_glass_shelf'). Unique across the registry.")]
        public string vistaId;

        [Header("Dome — base gradient")]
        [Tooltip("Sky color bottom(=horizon, t=0) to top(=zenith, t=1). Baked with dithering to avoid VR banding.")]
        public Gradient skyGradient;

        [Header("Dome — stars")]
        public StarsLayer stars = new StarsLayer();

        [Header("Dome — nebula")]
        public NebulaLayer nebula = new NebulaLayer();

        [Header("Dome — the Shell grid (story canon: 0 at W001 → 1.0 at W012)")]
        [Range(0f, 1f)] public float shellGridIntensity = 0f;
        [Tooltip("Hex cells across the dome width.")]
        [Range(2f, 40f)] public float shellGridScale = 14f;
        public Color shellGridColor = new Color(0.55f, 0.75f, 0.85f, 1f);

        [Header("Dome — zenith shimmer (the Pattern seed, W003+)")]
        public ZenithShimmerLayer zenithShimmer = new ZenithShimmerLayer();

        [Header("Atmosphere — SKYSCAPE_DESIGN.md layers (haze card + particulate drift + body glow)")]
        public AtmosphereLayer atmosphere = new AtmosphereLayer();

        [Header("Celestial bodies (max 3)")]
        public List<CelestialBodyDef> bodies = new List<CelestialBodyDef>();

        [Header("Scene tie-ins (applied after the theme's base pass)")]
        [Tooltip("If intensity > 0, the scene's single directional light gets this color/intensity.")]
        public Color directionalLightColor = Color.white;
        [Tooltip("0 = leave the scene light alone.")]
        [Range(0f, 2f)] public float directionalLightIntensity = 0f;
        [Tooltip("If enabled, RenderSettings.ambientLight is set to ambientColor.")]
        public bool overrideAmbient = false;
        public Color ambientColor = new Color(0.35f, 0.37f, 0.42f, 1f);
        [Tooltip("Worlds keep fog layout-owned (leave OFF); arenas may override.")]
        public bool overrideFog = false;
        public Color fogColor = Color.gray;
        [Range(0f, 0.2f)] public float fogDensity = 0.01f;

        public enum BodyType { BandedPlanet, Moon, SunDisc, BlackHole }

        [System.Serializable]
        public class StarsLayer
        {
            [Range(0f, 1f)] public float density = 0f;
            public int seed = 1;
            public Color tint = Color.white;
            [Tooltip("Stars fade out below this dome height (0=horizon..1=zenith) so they don't pierce the skyline.")]
            [Range(0f, 1f)] public float horizonFade = 0.15f;
        }

        [System.Serializable]
        public class NebulaLayer
        {
            public bool enabled = false;
            public int seed = 7;
            public Color colorA = new Color(0.25f, 0.20f, 0.45f, 1f);
            public Color colorB = new Color(0.10f, 0.35f, 0.45f, 1f);
            [Tooltip("How much of the sky the nebula covers.")]
            [Range(0f, 1f)] public float coverage = 0.35f;
            [Tooltip(">0 pushes the nebula toward the zenith, <0 toward the horizon.")]
            [Range(-1f, 1f)] public float altitudeBias = 0.3f;
        }

        [System.Serializable]
        public class ZenithShimmerLayer
        {
            public bool enabled = false;
            public Color color = new Color(0.75f, 0.9f, 1f, 1f);
            [Range(0f, 1f)] public float intensity = 0.25f;
        }

        /// <summary>The "air is real" layers (SKYSCAPE_DESIGN.md): authored as a hazard tag +
        /// intensity — SkyAtmosphere.ForHazard derives the actual spec, so the §4.1 defaults table
        /// stays the single source of truth. Optional tint override for worlds that subvert their
        /// hazard's expected palette.</summary>
        [System.Serializable]
        public class AtmosphereLayer
        {
            public bool enabled = false;
            [Tooltip("Hazard tag from the chapter docs (Bloom/fire/acid/radiation/flood/wind/…). Drives every default.")]
            public string hazardTag = "";
            [Range(0f, 1f)] public float intensity = 0.7f;
            [Tooltip("If alpha > 0, replaces the hazard default haze tint (for palette-subverting worlds).")]
            public Color hazeTintOverride = new Color(0f, 0f, 0f, 0f);
            [Tooltip("Signature-tier only: soft scatter glow behind body 0 so its edge reads as lit air, not cardboard.")]
            public bool bodyGlow = false;
        }

        [System.Serializable]
        public class CelestialBodyDef
        {
            public BodyType type = BodyType.Moon;
            [Tooltip("Apparent size in degrees (the banded giant GROWS across early worlds).")]
            [Range(1f, 60f)] public float angularSizeDeg = 10f;
            [Tooltip("Direction from the player (normalized on validate).")]
            public Vector3 direction = new Vector3(0f, 0.45f, 0.89f);
            public Color baseColor = new Color(0.6f, 0.55f, 0.5f, 1f);
            public Color accentColor = new Color(0.4f, 0.35f, 0.3f, 1f);
            [Tooltip("Band pairs for BandedPlanet; crater budget scaler for Moon.")]
            [Range(1, 16)] public int bandCount = 6;
            public int seed = 3;
            [Tooltip("Terminator: 0 = fully lit, 0.5 = half dark, 1 = fully dark side toward player.")]
            [Range(0f, 1f)] public float phase = 0.15f;
            [Tooltip("Slow rotation around local up, deg/s. Keep subtle.")]
            public float rotationSpeedDeg = 0.3f;
        }

        /// <summary>Pure validation — returns an empty list when the vista is well-formed.</summary>
        public List<string> Validate()
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(vistaId)) issues.Add("empty vistaId");
            if (skyGradient == null || skyGradient.colorKeys == null || skyGradient.colorKeys.Length < 1)
                issues.Add("skyGradient missing");
            if (bodies != null && bodies.Count > MaxBodies)
                issues.Add("more than " + MaxBodies + " celestial bodies");
            if (bodies != null)
                for (int i = 0; i < bodies.Count; i++)
                {
                    var b = bodies[i];
                    if (b == null) { issues.Add("body " + i + " is null"); continue; }
                    if (b.angularSizeDeg < 1f || b.angularSizeDeg > 60f)
                        issues.Add("body " + i + " angularSizeDeg out of range [1,60]");
                    if (b.direction.sqrMagnitude < 0.01f)
                        issues.Add("body " + i + " direction ~zero");
                }
            if (shellGridIntensity < 0f || shellGridIntensity > 1f)
                issues.Add("shellGridIntensity out of range [0,1]");
            if (stars != null && (stars.density < 0f || stars.density > 1f))
                issues.Add("stars.density out of range [0,1]");
            return issues;
        }

        private void OnValidate()
        {
            if (bodies == null) return;
            foreach (var b in bodies)
                if (b != null && b.direction.sqrMagnitude > 0.01f)
                    b.direction = b.direction.normalized;
        }
    }
}

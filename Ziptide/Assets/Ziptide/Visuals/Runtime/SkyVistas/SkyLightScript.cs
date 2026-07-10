using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.1 — THE LIGHT SCRIPT. Derives a world's whole light rig (key light, trilight
    /// ambient, fog) FROM its SkyVistaDefinition by formula, so the scene's atmosphere can never
    /// disagree with its sky: fog IS the horizon color, ambient IS the sky gradient, the key light
    /// comes FROM the brightest celestial body. Pure and deterministic — same vista, same values.
    ///
    /// THE DERIVATION LAW (FORGE_III_PLAN §0): these values are computed, never authored twice.
    /// A vista's explicit tie-in fields (directionalLightIntensity &gt; 0, overrideAmbient,
    /// overrideFog) remain the authored OVERRIDE — SkyVistaRig applies this derivation only where
    /// the vista is silent, and never touches layout-baked fog.
    /// </summary>
    public static class SkyLightScript
    {
        // Key-light elevation stays inside the "always sculpts, never flattens" band.
        private const float MinElevationSin = 0.342f; // sin 20°
        private const float MaxElevationSin = 0.819f; // sin 55°

        public struct SkyLightValues
        {
            public Vector3 keyDirection;   // direction the light TRAVELS (rotation = LookRotation(this))
            public Color keyColor;
            public float keyIntensity;     // clamped 0.7..1.4
            public Color ambientSky, ambientEquator, ambientGround;
            public Color fogColor;
            public float fogDensity;       // exponential; clamped ≤ 0.02
        }

        public static SkyLightValues Derive(SkyVistaDefinition v)
        {
            Color horizon = new Color(0.5f, 0.5f, 0.55f);
            Color zenith = new Color(0.2f, 0.25f, 0.4f);
            if (v != null && v.skyGradient != null && v.skyGradient.colorKeys != null
                && v.skyGradient.colorKeys.Length > 0)
            {
                horizon = v.skyGradient.Evaluate(0f);
                zenith = v.skyGradient.Evaluate(1f);
            }
            horizon.a = 1f; zenith.a = 1f;

            var r = new SkyLightValues
            {
                ambientSky = zenith,
                ambientEquator = horizon,
                ambientGround = new Color(horizon.r * 0.55f, horizon.g * 0.55f, horizon.b * 0.55f, 1f),
                fogColor = horizon,
            };

            // ── Key light: from the brightest body (SunDisc wins; else the largest) ──
            var key = KeySourceBody(v);
            Vector3 from;
            if (key != null)
            {
                from = key.direction.sqrMagnitude > 0.01f ? key.direction.normalized : Vector3.up;
                r.keyColor = Color.Lerp(key.baseColor, Color.white, 0.4f);
            }
            else
            {
                // Bodiless sky: deterministic azimuth from the vista id so sibling worlds differ.
                float az = Hash01(v != null ? v.vistaId : "") * 2f * Mathf.PI;
                from = new Vector3(Mathf.Cos(az) * 0.766f, 0.643f, Mathf.Sin(az) * 0.766f); // 40° up
                r.keyColor = Color.Lerp(zenith, Color.white, 0.5f);
            }
            from = ClampElevation(from);
            r.keyDirection = -from;
            r.keyColor.a = 1f;

            // Brighter skies light harder — but always inside the sculpting band.
            r.keyIntensity = Mathf.Clamp(0.9f + 0.4f * Luminance(zenith), 0.7f, 1.4f);

            // ── Fog: horizon-colored; hazard atmosphere thickens the air ──
            float atmos = v != null && v.atmosphere != null && v.atmosphere.enabled
                ? Mathf.Clamp01(v.atmosphere.intensity) : 0f;
            r.fogDensity = Mathf.Min(0.006f + atmos * 0.012f, 0.02f);

            return r;
        }

        private static SkyVistaDefinition.CelestialBodyDef KeySourceBody(SkyVistaDefinition v)
        {
            if (v == null || v.bodies == null) return null;
            SkyVistaDefinition.CelestialBodyDef best = null;
            foreach (var b in v.bodies)
            {
                if (b == null || b.type == SkyVistaDefinition.BodyType.BlackHole) continue; // holes don't light
                if (b.type == SkyVistaDefinition.BodyType.SunDisc) return b;
                if (best == null || b.angularSizeDeg > best.angularSizeDeg) best = b;
            }
            return best;
        }

        /// <summary>Clamp the FROM direction's elevation into [20°, 55°], keeping its azimuth.</summary>
        public static Vector3 ClampElevation(Vector3 from)
        {
            from = from.sqrMagnitude > 1e-6f ? from.normalized : Vector3.up;
            float y = Mathf.Clamp(from.y, MinElevationSin, MaxElevationSin);
            Vector2 flat = new Vector2(from.x, from.z);
            float flatLen = Mathf.Sqrt(Mathf.Max(0.0001f, 1f - y * y));
            flat = flat.sqrMagnitude > 1e-6f ? flat.normalized * flatLen : new Vector2(0f, flatLen);
            return new Vector3(flat.x, y, flat.y);
        }

        private static float Luminance(Color c) => 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;

        private static float Hash01(string s)
        {
            unchecked
            {
                uint h = 2166136261u;
                if (s != null) foreach (char c in s) h = (h ^ c) * 16777619u;
                return (h & 0xFFFFFF) / (float)0x1000000;
            }
        }
    }
}

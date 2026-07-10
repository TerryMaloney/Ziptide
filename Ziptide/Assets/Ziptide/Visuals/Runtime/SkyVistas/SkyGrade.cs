using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.2 — THE GRADE, pure math half. Derives a world's color-grade values from its
    /// SkyVistaDefinition under hard clamps (FORGE_III_PLAN §F3.2): post-exposure ±0.3, saturation
    /// −10..+15, color filter ≤8% toward the desaturated horizon, white balance ±15. The clamps ARE
    /// the contract — an operator cannot push a wild grade through this path. SkyVistaRig builds
    /// the URP Volume from these values; this file stays URP-free so the tests stay pure.
    /// </summary>
    public static class SkyGrade
    {
        public struct GradeValues
        {
            public float postExposure;   // clamped ±0.3 (dark skies get a small lift)
            public float saturation;     // clamped -10..+15 (hazard haze desaturates)
            public Color colorFilter;    // ≤8% from white toward the desaturated horizon
            public float temperature;    // clamped ±15 (follows the horizon's warmth)
        }

        public static GradeValues Derive(SkyVistaDefinition v)
        {
            Color horizon = new Color(0.5f, 0.5f, 0.55f);
            Color zenith = new Color(0.2f, 0.25f, 0.4f);
            if (v != null && v.skyGradient != null && v.skyGradient.colorKeys != null
                && v.skyGradient.colorKeys.Length > 0)
            {
                horizon = v.skyGradient.Evaluate(0f);
                zenith = v.skyGradient.Evaluate(1f);
            }

            var g = new GradeValues();

            // Dark skies get a small exposure lift so interiors of dim worlds stay readable;
            // bright skies get pulled slightly down. Never past ±0.3.
            g.postExposure = Mathf.Clamp(0.15f - 0.5f * Lum(zenith), -0.3f, 0.3f);

            // A hazed world reads through murk — desaturate with the hazard intensity.
            float atmos = v != null && v.atmosphere != null && v.atmosphere.enabled
                ? Mathf.Clamp01(v.atmosphere.intensity) : 0f;
            g.saturation = Mathf.Clamp(5f - 15f * atmos, -10f, 15f);

            // The filter: 8% toward the horizon color at half saturation — enough to unify a
            // frame, never enough to stain it.
            float hl = Lum(horizon);
            Color grey = new Color(hl, hl, hl, 1f);
            Color tint = Color.Lerp(horizon, grey, 0.5f);
            g.colorFilter = Color.Lerp(Color.white, tint, 0.08f);
            g.colorFilter.a = 1f;

            // Warmth follows the horizon: warm skies grade warm, cold skies cold. ±15 max.
            g.temperature = Mathf.Clamp((horizon.r - horizon.b) * 40f, -15f, 15f);

            return g;
        }

        private static float Lum(Color c) => 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
    }
}

using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// FORGE III F3.3 — WATER (the namesake). The pure ripple field: a small sum of DIRECTIONAL
    /// sine waves with INTEGER wave numbers, so the height (and therefore its normal map) tiles
    /// seamlessly over the [0,1] UV square — no fBm seam, no depth texture, Quest-cheap. The
    /// runtime scrolls two samples of the baked normal (commit 2) and bobs the mesh; this file is
    /// just the deterministic, EditMode-testable source of the surface shape.
    /// </summary>
    public static class WaterSurface
    {
        // (kx, ky, amplitude, phase) — kx/ky are integer cycles per tile → perfectly periodic.
        private static readonly float[,] Waves =
        {
            { 3f, 1f, 0.50f, 0.0f },
            { 1f, 3f, 0.32f, 1.7f },
            { 4f, 2f, 0.20f, 3.1f },
            { 2f, 5f, 0.13f, 0.5f },
            { 6f, 3f, 0.08f, 2.2f },
        };

        /// <summary>Tileable surface height at UV, roughly [-1.23, 1.23]. Height(0,v)==Height(1,v).</summary>
        public static float Height(float u, float v)
        {
            float h = 0f;
            for (int i = 0; i < Waves.GetLength(0); i++)
                h += Waves[i, 2] * Mathf.Sin(2f * Mathf.PI * (Waves[i, 0] * u + Waves[i, 1] * v) + Waves[i, 3]);
            return h;
        }

        /// <summary>Tangent-space normal from the height field (central difference, wrap-safe so it
        /// tiles). <paramref name="strength"/> scales the bump slope; higher = choppier.</summary>
        public static Vector3 Normal(float u, float v, float strength)
        {
            const float e = 1f / 256f;
            float hL = Height(Wrap01(u - e), v), hR = Height(Wrap01(u + e), v);
            float hD = Height(u, Wrap01(v - e)), hU = Height(u, Wrap01(v + e));
            // Gradient → normal (x,z from slope, y up). 2e is the world-space run of the difference.
            var n = new Vector3(-(hR - hL) * strength, 2f * e * 4f, -(hU - hD) * strength);
            return n.sqrMagnitude > 1e-9f ? n.normalized : Vector3.up;
        }

        /// <summary>Canonical RGB normal map (R=x, G=y, B=z, each *0.5+0.5), tileable. Consumers
        /// swizzle to their platform's _BumpMap convention (the booth/baker do this, as for Forge
        /// creature normals).</summary>
        public static Color32[] BakeNormalMap(int size, float strength)
        {
            var px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = (x + 0.5f) / size, v = (y + 0.5f) / size;
                    Vector3 n = Normal(u, v, strength);
                    px[y * size + x] = new Color32(
                        (byte)Mathf.Clamp(Mathf.RoundToInt((n.x * 0.5f + 0.5f) * 255f), 0, 255),
                        (byte)Mathf.Clamp(Mathf.RoundToInt((n.y * 0.5f + 0.5f) * 255f), 0, 255),
                        (byte)Mathf.Clamp(Mathf.RoundToInt((n.z * 0.5f + 0.5f) * 255f), 0, 255),
                        255);
                }
            return px;
        }

        private static float Wrap01(float t) => t - Mathf.Floor(t);
    }
}

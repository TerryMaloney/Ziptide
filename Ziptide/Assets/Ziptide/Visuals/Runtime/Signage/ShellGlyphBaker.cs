using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>Closed wayfinding classes. They control hue and a small terminal mark, never gameplay.</summary>
    public enum SignDestinationClass
    {
        Travel,
        Job,
        Vendor
    }

    /// <summary>
    /// FORGE III F3.7 — pure procedural Shell-script glyph strips. The result is abstract angular
    /// writing, not a font and not localized text. The same seed/class always produces the same mask.
    /// Texture/material creation belongs to the cleanup-owning sign surface, not this pure baker.
    /// </summary>
    public static class ShellGlyphBaker
    {
        public const int DefaultSize = 64;
        public const int MinimumSize = 16;
        public const int MaximumSize = 256;
        public const int GlyphCount = 5;

        /// <summary>RGBA alpha mask with white glyphs and a transparent background.</summary>
        public static Color32[] Bake(int size, int seed, SignDestinationClass destination)
        {
            size = Mathf.Clamp(size, MinimumSize, MaximumSize);
            var alpha = BakeAlpha(size, seed, destination);
            var pixels = new Color32[alpha.Length];
            for (int i = 0; i < alpha.Length; i++)
                pixels[i] = new Color32(255, 255, 255, alpha[i]);
            return pixels;
        }

        /// <summary>Pure alpha channel used by tests and the cleanup-owning runtime sign surface.</summary>
        public static byte[] BakeAlpha(int size, int seed, SignDestinationClass destination)
        {
            size = Mathf.Clamp(size, MinimumSize, MaximumSize);
            var alpha = new byte[size * size];

            float margin = size * 0.08f;
            float baseline = size * 0.24f;
            float top = size * 0.78f;
            float usable = size - margin * 2f;
            float cell = usable / (GlyphCount + 1.1f); // final cell is the destination mark
            float width = Mathf.Max(1.2f, size * 0.025f);

            for (int glyph = 0; glyph < GlyphCount; glyph++)
            {
                uint h = Hash(seed, glyph, (int)destination + 17);
                float x0 = margin + glyph * cell;
                float x1 = x0 + cell * 0.72f;
                float xm = (x0 + x1) * 0.5f;
                float low = baseline + Hash01(h, 1) * size * 0.07f;
                float high = top - Hash01(h, 2) * size * 0.08f;

                // Every glyph has a deliberate vertical/diagonal spine and two angular branches.
                bool leanRight = (h & 1u) == 0u;
                Vector2 spineA = new Vector2(leanRight ? x0 : x1, low);
                Vector2 spineB = new Vector2(leanRight ? x1 : x0, high);
                DrawLine(alpha, size, spineA, spineB, width);

                float branchY = Mathf.Lerp(low, high, 0.42f + Hash01(h, 3) * 0.18f);
                DrawLine(alpha, size,
                    new Vector2(xm, branchY),
                    new Vector2((h & 2u) == 0u ? x0 : x1, high),
                    width);

                float footY = low + size * (0.02f + Hash01(h, 4) * 0.05f);
                DrawLine(alpha, size,
                    new Vector2(leanRight ? x0 : x1, footY),
                    new Vector2(xm, footY + size * 0.05f),
                    width);
            }

            DrawDestinationMark(alpha, size, destination, margin + GlyphCount * cell, baseline, top, width);
            return alpha;
        }

        public static float Coverage01(byte[] alpha)
        {
            if (alpha == null || alpha.Length == 0) return 0f;
            int lit = 0;
            for (int i = 0; i < alpha.Length; i++)
                if (alpha[i] >= 32) lit++;
            return lit / (float)alpha.Length;
        }

        private static void DrawDestinationMark(
            byte[] alpha,
            int size,
            SignDestinationClass destination,
            float x,
            float low,
            float high,
            float width)
        {
            float w = size * 0.10f;
            float mid = (low + high) * 0.5f;
            switch (destination)
            {
                case SignDestinationClass.Travel:
                    // Open chevron: movement / berth.
                    DrawLine(alpha, size, new Vector2(x, low), new Vector2(x + w, mid), width * 1.15f);
                    DrawLine(alpha, size, new Vector2(x + w, mid), new Vector2(x, high), width * 1.15f);
                    break;
                case SignDestinationClass.Job:
                    // Angular diamond: task / objective.
                    DrawLine(alpha, size, new Vector2(x + w * 0.5f, low), new Vector2(x + w, mid), width);
                    DrawLine(alpha, size, new Vector2(x + w, mid), new Vector2(x + w * 0.5f, high), width);
                    DrawLine(alpha, size, new Vector2(x + w * 0.5f, high), new Vector2(x, mid), width);
                    DrawLine(alpha, size, new Vector2(x, mid), new Vector2(x + w * 0.5f, low), width);
                    break;
                default:
                    // Forked vessel: cache / resource / trade.
                    DrawLine(alpha, size, new Vector2(x + w * 0.5f, low), new Vector2(x + w * 0.5f, high), width);
                    DrawLine(alpha, size, new Vector2(x + w * 0.5f, mid), new Vector2(x, high), width);
                    DrawLine(alpha, size, new Vector2(x + w * 0.5f, mid), new Vector2(x + w, high), width);
                    break;
            }
        }

        private static void DrawLine(
            byte[] alpha,
            int size,
            Vector2 a,
            Vector2 b,
            float halfWidth)
        {
            float minX = Mathf.Min(a.x, b.x) - halfWidth - 1f;
            float maxX = Mathf.Max(a.x, b.x) + halfWidth + 1f;
            float minY = Mathf.Min(a.y, b.y) - halfWidth - 1f;
            float maxY = Mathf.Max(a.y, b.y) + halfWidth + 1f;
            int x0 = Mathf.Max(0, Mathf.FloorToInt(minX));
            int x1 = Mathf.Min(size - 1, Mathf.CeilToInt(maxX));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(minY));
            int y1 = Mathf.Min(size - 1, Mathf.CeilToInt(maxY));

            Vector2 ab = b - a;
            float denom = Mathf.Max(0.0001f, ab.sqrMagnitude);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / denom);
                    float distance = Vector2.Distance(p, a + ab * t);
                    float coverage = Mathf.Clamp01(halfWidth + 1f - distance);
                    byte value = (byte)Mathf.RoundToInt(coverage * 255f);
                    int index = y * size + x;
                    if (value > alpha[index]) alpha[index] = value;
                }
            }
        }

        private static uint Hash(int seed, int index, int salt)
        {
            unchecked
            {
                uint h = (uint)(seed * 374761393 + index * 668265263 + salt * 1274126177);
                h = (h ^ (h >> 13)) * 1274126177u;
                return h ^ (h >> 16);
            }
        }

        private static float Hash01(uint h, int shift)
        {
            uint mixed = h ^ ((uint)shift * 2246822519u);
            mixed ^= mixed >> 15;
            return (mixed & 0x00FFFFFFu) / 16777216f;
        }
    }
}

using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Pure texel math for the SkyVista system — every layer (gradient, stars, nebula, Shell hex grid,
    /// zenith shimmer, celestial body surfaces) composited into caller-provided Color32 buffers.
    /// Deterministic per seed, no scene or GPU dependency, so EditMode tests can assert pixels.
    /// A 4×4 ordered Bayer dither is applied to the dome gradient — an 8-bit vertical gradient on a
    /// 500 m dome shows hard banding in a VR headset without it.
    /// </summary>
    public static class SkyVistaTexture
    {
        // 4×4 Bayer matrix, values 0..15 (threshold map for ±half-LSB ordered dithering).
        private static readonly byte[] Bayer4 =
        {
             0,  8,  2, 10,
            12,  4, 14,  6,
             3, 11,  1,  9,
            15,  7, 13,  5
        };

        // ── Dome ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Bake the full dome texture (u = azimuth, v = 0 horizon → 1 zenith) into
        /// <paramref name="pixels"/> (length must be width*height, row 0 = bottom/horizon).
        /// </summary>
        public static void BakeDome(SkyVistaDefinition def, int width, int height, Color32[] pixels)
        {
            if (def == null || pixels == null || pixels.Length != width * height) return;

            Gradient grad = def.skyGradient ?? DefaultGradient();
            var stars = def.stars;
            var nebula = def.nebula;
            var shimmer = def.zenithShimmer;

            for (int y = 0; y < height; y++)
            {
                float v = height > 1 ? y / (float)(height - 1) : 0f;
                Color rowBase = grad.Evaluate(v);

                for (int x = 0; x < width; x++)
                {
                    float u = width > 1 ? x / (float)(width - 1) : 0f;
                    Color c = rowBase;

                    if (nebula != null && nebula.enabled && nebula.coverage > 0f)
                        c = ApplyNebula(c, nebula, u, v);

                    if (stars != null && stars.density > 0f)
                        c = ApplyStars(c, stars, x, y, v);

                    if (def.shellGridIntensity > 0f)
                        c = ApplyShellGrid(c, def, u, v);

                    if (shimmer != null && shimmer.enabled && shimmer.intensity > 0f)
                        c = ApplyZenithShimmer(c, shimmer, u, v);

                    pixels[y * width + x] = DitherTo32(c, x, y);
                }
            }
        }

        // ── Bodies ───────────────────────────────────────────────────────────

        /// <summary>Bake a celestial body's surface (wrapped equirect: u = longitude, v = latitude).</summary>
        public static void BakeBody(SkyVistaDefinition.CelestialBodyDef body, int width, int height, Color32[] pixels)
        {
            if (body == null || pixels == null || pixels.Length != width * height) return;

            for (int y = 0; y < height; y++)
            {
                float v = height > 1 ? y / (float)(height - 1) : 0f;
                for (int x = 0; x < width; x++)
                {
                    float u = width > 1 ? x / (float)(width - 1) : 0f;
                    Color c;
                    switch (body.type)
                    {
                        case SkyVistaDefinition.BodyType.BandedPlanet: c = BandedPlanetTexel(body, u, v); break;
                        case SkyVistaDefinition.BodyType.Moon: c = MoonTexel(body, u, v); break;
                        case SkyVistaDefinition.BodyType.SunDisc: c = SunDiscTexel(body, u, v); break;
                        case SkyVistaDefinition.BodyType.BlackHole: c = BlackHoleTexel(body, u, v); break;
                        default: c = body.baseColor; break;
                    }

                    // Terminator: darken longitudes past the phase point (reads as a lit/dark side).
                    if (body.phase > 0f && body.type != SkyVistaDefinition.BodyType.SunDisc
                        && body.type != SkyVistaDefinition.BodyType.BlackHole)
                    {
                        float dark = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((u - (1f - body.phase)) * 6f));
                        c = Color.Lerp(c, c * 0.12f, dark);
                    }

                    pixels[y * width + x] = DitherTo32(c, x, y);
                }
            }
        }

        // ── Layers ───────────────────────────────────────────────────────────

        private static Color ApplyNebula(Color c, SkyVistaDefinition.NebulaLayer n, float u, float v)
        {
            // Two-octave tileable-enough value noise; mask thresholded by coverage.
            float noise = Fbm(u * 6f, v * 4f, n.seed, 3);
            float bias = n.altitudeBias >= 0f
                ? Mathf.Lerp(1f, v, n.altitudeBias)
                : Mathf.Lerp(1f, 1f - v, -n.altitudeBias);
            float threshold = 1f - n.coverage;
            float mask = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((noise - threshold) / Mathf.Max(0.0001f, 1f - threshold))) * bias;
            if (mask <= 0f) return c;

            float mix = Fbm(u * 11f + 31f, v * 7f + 17f, n.seed + 1, 2);
            Color neb = Color.Lerp(n.colorA, n.colorB, mix);
            // Additive-ish screen blend keeps the gradient underneath readable.
            return Color.Lerp(c, c + neb * 0.8f, mask);
        }

        private static Color ApplyStars(Color c, SkyVistaDefinition.StarsLayer s, int x, int y, float v)
        {
            if (v < s.horizonFade * 0.5f) return c;

            const int cell = 8;
            int cx = x / cell, cy = y / cell;
            float presence = Hash01(cx, cy, s.seed);
            if (presence > s.density) return c;

            // Star position within the cell + brightness, all from hashes (deterministic).
            int sx = cx * cell + (int)(Hash01(cx, cy, s.seed + 1) * (cell - 1));
            int sy = cy * cell + (int)(Hash01(cx, cy, s.seed + 2) * (cell - 1));
            int dx = x - sx, dy = y - sy;
            int d2 = dx * dx + dy * dy;
            if (d2 > 1) return c;

            float brightness = 0.35f + 0.65f * Hash01(cx, cy, s.seed + 3);
            if (d2 == 1) brightness *= 0.35f; // soft 1px halo
            float fade = Mathf.Clamp01((v - s.horizonFade * 0.5f) / Mathf.Max(0.0001f, s.horizonFade));
            return Color.Lerp(c, c + s.tint * brightness, fade);
        }

        /// <summary>
        /// The Shell: a hexagonal containment lattice over the whole sky. Distance-to-hex-edge in UV
        /// space; at intensity 0 this is exactly a no-op (tested), at 1.0 it reads as a solid wall-grid.
        /// </summary>
        private static Color ApplyShellGrid(Color c, SkyVistaDefinition def, float u, float v)
        {
            float e = HexEdge(u * def.shellGridScale, v * def.shellGridScale * 0.6f);
            // e: 0 at cell center → 1 at edge. Thin bright line near the edge.
            float line = Smooth01(0.92f - 0.10f * def.shellGridIntensity, 0.98f, e);
            float a = line * def.shellGridIntensity;
            if (a <= 0f) return c;
            return Color.Lerp(c, def.shellGridColor, a * 0.85f);
        }

        private static Color ApplyZenithShimmer(Color c, SkyVistaDefinition.ZenithShimmerLayer z, float u, float v)
        {
            if (v < 0.7f) return c;
            float alt = (v - 0.7f) / 0.3f;
            // Interfering angular + ring waves — reads as faint geometry that shouldn't be there.
            float w = Mathf.Sin(u * Mathf.PI * 24f) * Mathf.Sin(alt * Mathf.PI * 9f);
            float m = Mathf.Max(0f, w) * alt * z.intensity;
            return Color.Lerp(c, z.color, m * 0.6f);
        }

        // ── Body surfaces ────────────────────────────────────────────────────

        private static Color BandedPlanetTexel(SkyVistaDefinition.CelestialBodyDef b, float u, float v)
        {
            // Latitude bands warped by noise — the classic gas-giant read.
            float warp = (Fbm(u * 5f, v * 3f, b.seed, 3) - 0.5f) * 0.18f;
            float band = Mathf.Sin((v + warp) * Mathf.PI * b.bandCount) * 0.5f + 0.5f;
            band = Smooth01(0.15f, 0.85f, band);
            Color c = Color.Lerp(b.baseColor, b.accentColor, band);
            float storm = Fbm(u * 9f + 53f, v * 9f + 29f, b.seed + 2, 2);
            if (storm > 0.82f) c = Color.Lerp(c, b.accentColor * 1.25f, (storm - 0.82f) / 0.18f * 0.5f);
            return c;
        }

        private static Color MoonTexel(SkyVistaDefinition.CelestialBodyDef b, float u, float v)
        {
            Color c = Color.Lerp(b.baseColor, b.accentColor, Fbm(u * 7f, v * 7f, b.seed, 3) * 0.5f);
            // Craters: dark cells where the noise dips hard.
            float crater = Fbm(u * 14f + 7f, v * 14f + 3f, b.seed + 1, 2);
            float dip = Mathf.Clamp01((0.34f - crater) * (2.4f + b.bandCount * 0.2f));
            return Color.Lerp(c, c * 0.55f, dip);
        }

        private static Color SunDiscTexel(SkyVistaDefinition.CelestialBodyDef b, float u, float v)
        {
            // Bright core with limb color shift; slight granulation.
            float grain = Fbm(u * 16f, v * 16f, b.seed, 2) * 0.12f;
            float limb = Mathf.Abs(v - 0.5f) * 2f;
            Color c = Color.Lerp(b.baseColor * 1.35f, b.accentColor, limb * 0.6f);
            return c * (1f - grain);
        }

        private static Color BlackHoleTexel(SkyVistaDefinition.CelestialBodyDef b, float u, float v)
        {
            // Near-black body with a blazing equatorial accretion band + faint lensed haze above/below.
            float d = Mathf.Abs(v - 0.5f) * 2f;
            float ring = Smooth01(0.16f, 0.02f, d);
            float haze = Smooth01(0.55f, 0.18f, d) * 0.22f;
            float flow = 0.85f + 0.15f * Mathf.Sin(u * Mathf.PI * 8f + Fbm(u * 4f, v * 4f, b.seed, 2) * 4f);
            Color black = b.baseColor * 0.04f;
            return black + b.accentColor * (ring * 1.6f * flow + haze);
        }

        // ── Noise / hashing (deterministic, allocation-free) ─────────────────

        /// <summary>Integer lattice hash → [0,1). Stable across platforms (pure int math).</summary>
        public static float Hash01(int x, int y, int seed)
        {
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + seed * 1274126177;
                h = (h ^ (h >> 13)) * 1103515245;
                h ^= h >> 16;
                return (h & 0x7FFFFFFF) / 2147483647f;
            }
        }

        /// <summary>Bilinear value noise over the integer lattice.</summary>
        public static float ValueNoise(float x, float y, int seed)
        {
            int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y);
            float fx = x - x0, fy = y - y0;
            float sx = fx * fx * (3f - 2f * fx), sy = fy * fy * (3f - 2f * fy);
            float a = Hash01(x0, y0, seed), b = Hash01(x0 + 1, y0, seed);
            float c = Hash01(x0, y0 + 1, seed), d = Hash01(x0 + 1, y0 + 1, seed);
            return Mathf.Lerp(Mathf.Lerp(a, b, sx), Mathf.Lerp(c, d, sx), sy);
        }

        /// <summary>Fractal value noise, <paramref name="octaves"/> capped small for bake speed.</summary>
        public static float Fbm(float x, float y, int seed, int octaves)
        {
            float sum = 0f, amp = 0.5f, freq = 1f, norm = 0f;
            for (int i = 0; i < octaves; i++)
            {
                sum += ValueNoise(x * freq, y * freq, seed + i * 101) * amp;
                norm += amp;
                amp *= 0.5f;
                freq *= 2.1f;
            }
            return norm > 0f ? sum / norm : 0f;
        }

        /// <summary>Distance-to-edge in a pointy-top hex tiling; 0 at cell center → 1 at the edge.</summary>
        public static float HexEdge(float x, float y)
        {
            // Axial-ish fold: work in a skewed space where hex cells become comparable triangles.
            float qx = Mathf.Abs(Frac(x) - 0.5f);
            float qy = Mathf.Abs(Frac(y + ((int)Mathf.Floor(x) % 2 == 0 ? 0f : 0.5f)) - 0.5f);
            // Hexagon edge function (flat approximation good enough for a sky lattice).
            return Mathf.Clamp01(Mathf.Max(qx * 1.7320508f + qy, qy * 2f));
        }

        private static float Frac(float f) => f - Mathf.Floor(f);

        /// <summary>GLSL-style smoothstep between edges (either order) — NOT Mathf.SmoothStep, which
        /// interpolates values rather than remapping an input across an edge band.</summary>
        public static float Smooth01(float edge0, float edge1, float x)
        {
            float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        // ── Dither / quantize ────────────────────────────────────────────────

        /// <summary>Quantize a Color to Color32 with 4×4 ordered Bayer dithering (±half LSB).</summary>
        public static Color32 DitherTo32(Color c, int x, int y)
        {
            float t = (Bayer4[(y & 3) * 4 + (x & 3)] / 16f - 0.46875f) / 255f; // centered threshold
            return new Color32(
                QuantizeChannel(c.r + t), QuantizeChannel(c.g + t), QuantizeChannel(c.b + t), 255);
        }

        private static byte QuantizeChannel(float v) => (byte)Mathf.Clamp(Mathf.RoundToInt(v * 255f), 0, 255);

        private static Gradient DefaultGradient()
        {
            var g = new Gradient();
            g.SetKeys(
                new[] { new GradientColorKey(new Color(0.5f, 0.5f, 0.55f), 0f), new GradientColorKey(new Color(0.2f, 0.25f, 0.4f), 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return g;
        }
    }
}

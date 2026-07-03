using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// ARCHITECTURE V2 H3/Q3 (PDF §Macro-Scale Procedural Generation) — the PURE terrain math.
    /// Fractal Brownian Motion (per-biome octave stacks over the project's seed-hash idiom) with
    /// DOMAIN WARPING (coordinates are offset by a secondary noise field before sampling, which turns
    /// synthetic noise-sheets into organic folds), plus a low-frequency CLIMATE field
    /// (temperature × moisture) that dressing/scatter density channels read (H5).
    ///
    /// Contracts (pinned by TerrainFieldTests — the audit gate TERRAIN_SLOPE_UNWALKABLE builds on #3):
    ///  1. Deterministic: same (biome, x, z, amplitude, seed) → bit-identical height. No
    ///     UnityEngine.Random, no wall-clock.
    ///  2. Bounded: |Height| ≤ amplitude * 1.5 for every biome (shaping may overshoot base noise,
    ///     never past 1.5×).
    ///  3. Walkable: ≥ <see cref="MinWalkableFraction"/> of any sampled area stays under
    ///     <see cref="MaxWalkableSlope"/> at mesh resolution, regardless of the amplitude a spec
    ///     asks for (the internal clamp enforces it). Mesas/Canyon keep their DELIBERATE cliff
    ///     steps — you walk around a mesa, not up its face — so the contract is a fraction, not a
    ///     worst case; smooth biomes (Dunes/CavernFloor/TideFlats) additionally pin the worst case.
    /// Consumed by WorldExperienceBuilder.RawHeight — scene code translates, never re-implements.
    /// </summary>
    public static class TerrainField
    {
        /// <summary>Max terrain rise/run counted as walkable (≈40°). CharacterController-comfortable.</summary>
        public const float MaxWalkableSlope = 0.85f;

        /// <summary>The TERRAIN_SLOPE_UNWALKABLE bar: at least this fraction of a world's sampled
        /// cells must be under MaxWalkableSlope (cliff-stepped biomes stay legal; a world that is
        /// MOSTLY unclimbable fails the build). Empirically: Mesas ≥ 0.847 across seeds (its terrace
        /// edges ARE the character), every other biome ≥ 0.98 — 0.80 is the bar with margin.</summary>
        public const float MinWalkableFraction = 0.80f;

        // Amplitude cap per meter of wavelength. Worst-case fBM gradient scales ~ amp/wavelength ×
        // octave gain stack × warp amplification; 0.22 keeps the sampled worst case under the
        // walkable bound with margin (TerrainFieldTests sweeps it per biome).
        private const float SlopeSafetyRatio = 0.22f;

        /// <summary>Per-biome fBM parameter set — THE biome table (WORLD_RECIPE documents the feel).</summary>
        public struct BiomeParams
        {
            public int octaves;         // noise layers (more = busier)
            public float wavelength;    // dominant feature size (m)
            public float warpStrength;  // domain-warp offset magnitude (m)
        }

        public static BiomeParams ParamsFor(BiomePreset biome)
        {
            switch (biome)
            {
                case BiomePreset.Mesas: return new BiomeParams { octaves = 4, wavelength = 70f, warpStrength = 8f };
                case BiomePreset.Canyon: return new BiomeParams { octaves = 4, wavelength = 60f, warpStrength = 14f };
                case BiomePreset.CavernFloor: return new BiomeParams { octaves = 3, wavelength = 55f, warpStrength = 6f };
                case BiomePreset.TideFlats: return new BiomeParams { octaves = 2, wavelength = 110f, warpStrength = 12f };
                default: return new BiomeParams { octaves = 3, wavelength = 85f, warpStrength = 10f }; // Dunes
            }
        }

        // ── Deterministic noise primitives (the project's seed-hash idiom — no Random) ────────────

        private static float Hash01(int x, int z, int seed)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + z * 668265263 + seed * 1274126177);
                h = (h ^ (h >> 13)) * 1274126177u;
                return ((h ^ (h >> 16)) & 0xFFFFFF) / (float)0x1000000;
            }
        }

        private static float ValueNoise(float x, float z, int seed)
        {
            int x0 = Mathf.FloorToInt(x), z0 = Mathf.FloorToInt(z);
            float fx = x - x0, fz = z - z0;
            fx = fx * fx * (3f - 2f * fx);
            fz = fz * fz * (3f - 2f * fz);
            float a = Hash01(x0, z0, seed), b = Hash01(x0 + 1, z0, seed);
            float c = Hash01(x0, z0 + 1, seed), d = Hash01(x0 + 1, z0 + 1, seed);
            return Mathf.Lerp(Mathf.Lerp(a, b, fx), Mathf.Lerp(c, d, fx), fz); // 0..1
        }

        /// <summary>fBM in [-1, 1]: octave stack with gain 0.45, lacunarity 2.1.</summary>
        public static float Fbm(float x, float z, int seed, int octaves, float wavelength)
        {
            if (octaves < 1) octaves = 1;
            if (wavelength < 1f) wavelength = 1f;
            float n = 0f, amp = 0.55f, freq = 1f / wavelength;
            for (int o = 0; o < octaves; o++)
            {
                n += (ValueNoise(x * freq, z * freq, seed + o * 101) * 2f - 1f) * amp;
                amp *= 0.45f;
                freq *= 2.1f;
            }
            return Mathf.Clamp(n, -1f, 1f);
        }

        // ── The height field ───────────────────────────────────────────────────────────────────────

        /// <summary>Terrain height (m, relative) at an XZ point. Pure and deterministic.</summary>
        public static float Height(BiomePreset biome, float x, float z, float amplitude, int seed)
        {
            if (biome == BiomePreset.None || amplitude <= 0f) return 0f;
            var p = ParamsFor(biome);

            // Slope-safe amplitude: proportional to the biome's wavelength, never what was asked
            // for if that would break walkability (contract #3).
            float amp = Mathf.Min(amplitude, p.wavelength * SlopeSafetyRatio);

            // Domain warp: sample the elevation at coordinates twisted by a secondary field.
            float wx = x + Fbm(x + 37.2f, z - 11.7f, seed + 9001, 2, p.wavelength * 1.7f) * p.warpStrength;
            float wz = z + Fbm(x - 63.9f, z + 24.3f, seed + 9313, 2, p.wavelength * 1.7f) * p.warpStrength;

            float n = Fbm(wx, wz, seed, p.octaves, p.wavelength);
            return Shape(biome, n, amp);
        }

        /// <summary>Biome shaping — the landform recipes (kept from the P1a engine, warp-fed now).</summary>
        private static float Shape(BiomePreset biome, float n, float amp)
        {
            switch (biome)
            {
                case BiomePreset.Mesas:
                    float terraced = Mathf.Floor((n * 0.5f + 0.5f) * 4f) / 4f;
                    return (Mathf.Lerp(terraced, n * 0.5f + 0.5f, 0.2f) * 2f - 1f) * amp * 1.15f;
                case BiomePreset.Canyon:
                    return (Mathf.Abs(n) * 1.7f - 0.45f) * amp;
                case BiomePreset.CavernFloor:
                    return n * amp * 0.45f;
                case BiomePreset.TideFlats:
                    return Mathf.Min(0f, n) * amp * 0.5f + n * amp * 0.1f;
                default: // Dunes
                    return n * amp;
            }
        }

        // ── Climate (temp × moisture) — the biome matrix the dressing channels read ───────────────

        /// <summary>Low-frequency climate at a point: x = temperature 0..1, y = moisture 0..1.
        /// Scatter density channels (H5) and future sub-biome tinting read this — height does NOT
        /// (a world's landform stays its authored BiomePreset; climate varies texture, not terrain).</summary>
        public static Vector2 Climate(float x, float z, int seed)
        {
            float t = (Fbm(x, z, seed + 555, 2, 160f) + 1f) * 0.5f;
            float m = (Fbm(x, z, seed + 777, 2, 130f) + 1f) * 0.5f;
            return new Vector2(Mathf.Clamp01(t), Mathf.Clamp01(m));
        }

        // ── Slope estimation (the TERRAIN_SLOPE_UNWALKABLE gate's measurement) ─────────────────────

        /// <summary>Sampled worst-case slope (rise/run) over a square extent. Deterministic.
        /// Step defaults to the terrain-mesh cell size — slope at the resolution players walk on.</summary>
        public static float EstimateMaxSlope(BiomePreset biome, float amplitude, int seed,
            float extent, float step = 5f)
        {
            if (step < 0.5f) step = 0.5f;
            float worst = 0f;
            for (float z = -extent; z <= extent; z += step)
            {
                for (float x = -extent; x <= extent; x += step)
                {
                    float g = SlopeAt(biome, amplitude, seed, x, z, step);
                    if (g > worst) worst = g;
                }
            }
            return worst;
        }

        /// <summary>Fraction of sampled cells whose slope is walkable — the gate's measurement.</summary>
        public static float WalkableFraction(BiomePreset biome, float amplitude, int seed,
            float extent, float step = 5f)
        {
            if (step < 0.5f) step = 0.5f;
            int total = 0, walkable = 0;
            for (float z = -extent; z <= extent; z += step)
            {
                for (float x = -extent; x <= extent; x += step)
                {
                    total++;
                    if (SlopeAt(biome, amplitude, seed, x, z, step) <= MaxWalkableSlope) walkable++;
                }
            }
            return total == 0 ? 1f : walkable / (float)total;
        }

        private static float SlopeAt(BiomePreset biome, float amplitude, int seed, float x, float z, float step)
        {
            float h = Height(biome, x, z, amplitude, seed);
            float gx = Mathf.Abs(Height(biome, x + step, z, amplitude, seed) - h) / step;
            float gz = Mathf.Abs(Height(biome, x, z + step, amplitude, seed) - h) / step;
            return Mathf.Sqrt(gx * gx + gz * gz);
        }
    }
}

using System;

namespace Ziptide.Gameplay
{
    /// <summary>Everything a biome's air sounds like, as plain data. Levels are 0..1 mix weights;
    /// densities are events-per-minute; 0 disables a layer.</summary>
    [Serializable]
    public struct AmbienceSpec
    {
        public float WindLevel;      // broadband air movement
        public float WindGustiness;  // 0 = steady breath, 1 = strong slow surges
        public float HumHz;          // tonal bed root (power grids, station drives, the Hum itself)
        public float HumLevel;
        public float RumbleLevel;    // sub-slow pulsing (surf, machinery, tide mass)
        public float DripDensity;    // water/creak one-shots per minute (caves, cisterns)
        public float ChirpDensity;   // insect/bird-ish blips per minute (living exteriors)
        public float ChirpPitchHz;   // chirp base pitch

        public bool Silent => WindLevel <= 0f && HumLevel <= 0f && RumbleLevel <= 0f
                           && DripDensity <= 0f && ChirpDensity <= 0f;
    }

    /// <summary>
    /// ADAPTIVE_AUDIO 5.5 / EXCELLENCE_MAP "ambient audio" — the missing half of presence
    /// (SKYSCAPE_DESIGN §7: "Prospect's alien-place feeling is at least half wind, insects, and
    /// atmospheric hum"). Scene → biome → spec, the same registry idiom as the conquest seed list
    /// and the sky hazard table: one table, offline-reasoned, gate-tested for coverage.
    /// </summary>
    public static class BiomeAmbience
    {
        /// <summary>Scene name → biome key. Story worlds match the conquest seed biomes; labs,
        /// arenas, caves and space get their own. Unknown scenes get the neutral default —
        /// the standard says a world is never DEAD silent unless it means to be.</summary>
        public static string BiomeForScene(string sceneName)
        {
            switch (sceneName)
            {
                case "ToxicCity": case "W009_Chitinwall": case "D0_City": return "city";
                case "W002_DryCistern": case "W011_TheHum": return "underground";
                case "W003_GlassShelf": case "W006_MirrorFlats": return "exterior";
                case "W004_BroadcastTomb": case "W008_SealedArchive": case "MilestoneA_GrabCube": return "interior";
                case "W005_OxidizedCanopy": return "forest";
                case "W007_SableStation": case "W000_DriftIn": case "SandboxTestLab": return "station";
                case "W010_TidalArray": return "coastal";
                case "W012_MarasLastJump": case "SpaceLane_Trial": return "void";
                case "Cavern_TestLab": case "W011_Undercroft": return "cave";
                default:
                    if (!string.IsNullOrEmpty(sceneName) && sceneName.StartsWith("PvP_", StringComparison.Ordinal))
                        return "arena";
                    return "default";
            }
        }

        public static AmbienceSpec ForScene(string sceneName) => ForBiome(BiomeForScene(sceneName));

        public static AmbienceSpec ForBiome(string biome)
        {
            switch (biome)
            {
                case "city":        // dead city: wind through structures + a live grid somewhere
                    return new AmbienceSpec { WindLevel = 0.30f, WindGustiness = 0.45f, HumHz = 55f, HumLevel = 0.14f, RumbleLevel = 0.08f };
                case "underground": // moving air in rock, the deep grid, water finding its way down
                    return new AmbienceSpec { WindLevel = 0.12f, WindGustiness = 0.2f, HumHz = 41f, HumLevel = 0.12f, RumbleLevel = 0.14f, DripDensity = 5f };
                case "cave":        // wetter, closer, more alive in the dark
                    return new AmbienceSpec { WindLevel = 0.10f, WindGustiness = 0.15f, HumHz = 36f, HumLevel = 0.08f, RumbleLevel = 0.12f, DripDensity = 9f };
                case "exterior":    // open ground under weather; sparse alien insect life
                    return new AmbienceSpec { WindLevel = 0.48f, WindGustiness = 0.6f, ChirpDensity = 4f, ChirpPitchHz = 950f };
                case "forest":      // the canopy breathes; the Bloom sings
                    return new AmbienceSpec { WindLevel = 0.34f, WindGustiness = 0.5f, ChirpDensity = 11f, ChirpPitchHz = 1250f, HumHz = 62f, HumLevel = 0.05f };
                case "interior":    // sealed rooms: faint ventilation and a live wall socket
                    return new AmbienceSpec { WindLevel = 0.06f, WindGustiness = 0.1f, HumHz = 60f, HumLevel = 0.11f };
                case "station":     // the drive is always on somewhere below deck
                    return new AmbienceSpec { WindLevel = 0.05f, WindGustiness = 0.1f, HumHz = 92f, HumLevel = 0.18f, RumbleLevel = 0.12f };
                case "coastal":     // surf mass + driven air + something crying far off
                    return new AmbienceSpec { WindLevel = 0.52f, WindGustiness = 0.7f, RumbleLevel = 0.26f, ChirpDensity = 2f, ChirpPitchHz = 700f };
                case "void":        // the thin dread: almost nothing, and the nothing has a pitch
                    return new AmbienceSpec { HumHz = 30f, HumLevel = 0.07f };
                case "arena":       // pre-fight tension: taut air, live systems
                    return new AmbienceSpec { WindLevel = 0.14f, WindGustiness = 0.25f, HumHz = 70f, HumLevel = 0.13f, RumbleLevel = 0.16f };
                default:            // never dead silence by accident
                    return new AmbienceSpec { WindLevel = 0.15f, WindGustiness = 0.3f };
            }
        }
    }

    /// <summary>
    /// Deterministic, LOOP-EXACT procedural synthesis (the ZiptideGateEffect/ship-hum lineage).
    /// The trick that makes loops seamless by construction: every component is a sine at an INTEGER
    /// number of cycles over the buffer, so sample[N] == sample[0] mathematically — wind is ~48
    /// integer-harmonic partials with 1/f weighting and seeded phases (perfectly loopable colored
    /// noise), gusts/rumble are integer-cycle LFOs, hums snap their root to the nearest integer
    /// cycle count. Pure float[] math — CI pins determinism, loop seams, and headroom.
    /// </summary>
    public static class AmbienceSynth
    {
        /// <summary>Looping wind bed. level 0 → silence.</summary>
        public static void FillWindLoop(float[] buf, int seed, float level, float gustiness, int sampleRate)
        {
            Array.Clear(buf, 0, buf.Length);
            if (level <= 0f || buf.Length == 0) return;
            int n = buf.Length;
            double twoPiOverN = Math.PI * 2.0 / n;
            float seconds = n / (float)sampleRate;

            // Colored noise: partials at integer cycle counts spanning ~30–700 Hz.
            uint h = Hash((uint)(seed * 2654435761u + 12345u));
            int minCycles = Math.Max(1, (int)(30f * seconds));
            int maxCycles = Math.Max(minCycles + 8, (int)(700f * seconds));
            const int partials = 48;
            for (int p = 0; p < partials; p++)
            {
                h = Hash(h);
                int cycles = minCycles + (int)(h % (uint)(maxCycles - minCycles));
                h = Hash(h);
                double phase = (h & 0xFFFF) / 65536.0 * Math.PI * 2.0;
                float amp = 1f / (1f + cycles * 0.04f);   // 1/f-ish: low whoosh dominates
                double w = cycles * twoPiOverN;
                for (int i = 0; i < n; i++)
                    buf[i] += (float)(Math.Sin(w * i + phase)) * amp;
            }

            // Normalize the raw partial sum to unit-ish, then breathe with integer-cycle gust LFOs.
            float peak = 0f;
            for (int i = 0; i < n; i++) { float a = Math.Abs(buf[i]); if (a > peak) peak = a; }
            if (peak < 1e-6f) return;
            int gustCycles = Math.Max(1, (int)(0.35f * seconds));       // slow surges (~0.35 Hz)
            int breathCycles = Math.Max(1, (int)(0.13f * seconds));     // slower under-swell
            double wg = gustCycles * twoPiOverN, wb = breathCycles * twoPiOverN;
            float norm = level / peak;
            for (int i = 0; i < n; i++)
            {
                float gust = 1f - gustiness * 0.5f * (1f + (float)Math.Sin(wg * i))
                           * (0.6f + 0.4f * (float)Math.Sin(wb * i + 1.7));
                buf[i] *= norm * Clamp01(gust);
            }
        }

        /// <summary>Looping tonal hum: root (snapped to an integer cycle count) + 2 harmonics +
        /// a slow integer-cycle tremble so it reads as machinery, not a test tone.</summary>
        public static void FillHumLoop(float[] buf, float hz, float level, int sampleRate)
        {
            Array.Clear(buf, 0, buf.Length);
            if (level <= 0f || hz <= 0f || buf.Length == 0) return;
            int n = buf.Length;
            double twoPiOverN = Math.PI * 2.0 / n;
            float seconds = n / (float)sampleRate;
            int rootCycles = Math.Max(1, (int)Math.Round(hz * seconds));
            int trembleCycles = Math.Max(1, (int)(0.8f * seconds));
            double w = rootCycles * twoPiOverN, wt = trembleCycles * twoPiOverN;
            for (int i = 0; i < n; i++)
            {
                float s = (float)(Math.Sin(w * i) * 0.62
                                + Math.Sin(w * 2 * i + 0.9) * 0.27
                                + Math.Sin(w * 3 * i + 2.1) * 0.11);
                float tremble = 0.9f + 0.1f * (float)Math.Sin(wt * i);
                buf[i] = s * level * tremble;
            }
        }

        /// <summary>Looping sub-rumble: two very low integer-cycle sines beating against each
        /// other — surf mass / deep machinery. Loop-exact like everything else.</summary>
        public static void FillRumbleLoop(float[] buf, int seed, float level, int sampleRate)
        {
            Array.Clear(buf, 0, buf.Length);
            if (level <= 0f || buf.Length == 0) return;
            int n = buf.Length;
            double twoPiOverN = Math.PI * 2.0 / n;
            float seconds = n / (float)sampleRate;
            uint h = Hash((uint)seed + 77u);
            int c1 = Math.Max(1, (int)(23f * seconds) + (int)(h % 5));   // ~23 Hz
            int c2 = c1 + 1 + (int)(Hash(h) % 3);                        // near neighbor → slow beat
            int swellCycles = Math.Max(1, (int)(0.18f * seconds));
            double w1 = c1 * twoPiOverN, w2 = c2 * twoPiOverN, ws = swellCycles * twoPiOverN;
            for (int i = 0; i < n; i++)
            {
                float s = (float)(Math.Sin(w1 * i) * 0.6 + Math.Sin(w2 * i + 1.3) * 0.4);
                float swell = 0.55f + 0.45f * (float)Math.Sin(ws * i + 0.4);
                buf[i] = s * level * swell;
            }
        }

        /// <summary>One-shot chirp (alien insect/bird blip): a short pitched trill with decay.
        /// Seed varies contour so no two chirp variants are identical.</summary>
        public static void FillChirp(float[] buf, int seed, float baseHz, int sampleRate)
        {
            Array.Clear(buf, 0, buf.Length);
            if (buf.Length == 0 || baseHz <= 0f) return;
            uint h = Hash((uint)(seed * 31 + 7));
            float slide = 0.6f + (h % 100) / 100f;            // pitch slide factor
            float trillHz = 18f + (Hash(h) % 20);             // warble speed
            int n = buf.Length;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)sampleRate;
                float env = (float)(Math.Exp(-t * 14f)) * Math.Min(1f, t * 60f);   // fast in, breathy out
                float hz = baseHz * (1f + 0.25f * (float)Math.Sin(2 * Math.PI * trillHz * t)) * (1f + slide * t);
                buf[i] = (float)Math.Sin(2 * Math.PI * hz * t) * env * 0.8f;
            }
        }

        /// <summary>One-shot drip/creak: a damped low blip with a pitch fall — water in the dark.</summary>
        public static void FillDrip(float[] buf, int seed, int sampleRate)
        {
            Array.Clear(buf, 0, buf.Length);
            if (buf.Length == 0) return;
            uint h = Hash((uint)(seed * 131 + 3));
            float startHz = 900f + (h % 500);
            int n = buf.Length;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)sampleRate;
                float env = (float)Math.Exp(-t * 22f) * Math.Min(1f, t * 200f);
                float hz = startHz * (float)Math.Exp(-t * 6f) + 90f;
                buf[i] = (float)Math.Sin(2 * Math.PI * hz * t) * env * 0.7f;
            }
        }

        private static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;

        private static uint Hash(uint x)
        {
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            return x == 0 ? 2463534242u : x;
        }
    }
}

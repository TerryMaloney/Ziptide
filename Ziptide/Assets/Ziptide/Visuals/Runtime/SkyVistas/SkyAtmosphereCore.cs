using UnityEngine;

namespace Ziptide.Visuals
{
    public enum DriftStyle
    {
        Steady,   // slow settle-and-drift (spores, dust motes)
        Wind,     // fast lateral streaks (sand, driven ash)
        Puff,     // short irregular bursts (shaken dust, cave-in)
        Glitch,   // stepped, non-natural motion (the Pattern — motes that feel WRONG)
        Sparse    // steady but few and faint (radiation's "nothing to see is scarier")
    }

    /// <summary>Everything the atmosphere layers need, as plain data (derived, never authored raw —
    /// authoring picks a hazard tag + intensity and this is computed).</summary>
    public struct AtmosphereSpec
    {
        public bool HazeEnabled;
        public Color HazeTint;          // alpha = max opacity of the haze band
        public float HazeDistance;      // metres — REAL world-space distance (the stereo-depth law)
        public float HazeHeight;        // metres of band above the horizon line

        public int MoteCount;           // 0 = no particulate layer
        public Color MoteTint;
        public float MoteSize;          // metres (billboard edge)
        public DriftStyle Drift;
        public float DriftSpeed;        // style-relative speed scale
        public float ShellNear, ShellFar; // motes live between these radii from the player
    }

    /// <summary>
    /// SKYSCAPE_DESIGN.md made code — §4.1's hazard→atmosphere defaults table plus the drift math.
    /// The thesis: the sky should read as WEATHER, not a backdrop. These layers sit at REAL distances
    /// (haze ~40m, motes 8–15m) so stereo VR separates them into actual depth — the one effect a flat
    /// screen cannot copy. All pure and deterministic: mote positions are a function of (seed, index,
    /// time) — the LiftCycle idiom — so CI can pin every behavior headlessly.
    /// </summary>
    public static class SkyAtmosphere
    {
        /// <summary>§4.1 verbatim: every hazard tag in the chapter catalog gets a first-guess
        /// atmosphere. Unknown/empty/"none" returns a disabled spec (defer to the vista).</summary>
        public static AtmosphereSpec ForHazard(string hazardTag, float intensity)
        {
            intensity = Mathf.Clamp01(intensity);
            var s = new AtmosphereSpec
            {
                HazeEnabled = true,
                HazeDistance = 40f,
                HazeHeight = 16f,
                MoteSize = 0.045f,
                ShellNear = 8f,
                ShellFar = 15f,
                Drift = DriftStyle.Steady,
                DriftSpeed = 1f,
            };
            string tag = string.IsNullOrEmpty(hazardTag) ? "none" : hazardTag.ToLowerInvariant();
            switch (tag)
            {
                case "bloom":
                case "spore":
                    s.HazeTint = new Color(0.95f, 0.65f, 0.30f, 0.35f * intensity);   // Prospect's pollen-lit air
                    s.MoteCount = Mathf.RoundToInt(120 * intensity);
                    s.MoteTint = new Color(1.0f, 0.82f, 0.45f, 0.85f);
                    s.MoteSize = 0.06f;
                    s.DriftSpeed = 0.6f;
                    break;
                case "fire":
                    s.HazeTint = new Color(0.35f, 0.22f, 0.16f, 0.45f * intensity);    // smoke: opaque, not misty
                    s.MoteCount = Mathf.RoundToInt(90 * intensity);
                    s.MoteTint = new Color(1.0f, 0.45f, 0.15f, 0.9f);                  // embers
                    s.DriftSpeed = 1.4f;                                                // rising, restless
                    break;
                case "acid":
                    s.HazeTint = new Color(0.60f, 0.75f, 0.25f, 0.40f * intensity);
                    s.MoteCount = Mathf.RoundToInt(80 * intensity);
                    s.MoteTint = new Color(0.75f, 0.90f, 0.30f, 0.8f);
                    break;
                case "radiation":
                    s.HazeTint = new Color(0.80f, 0.85f, 0.75f, 0.10f * intensity);    // thin — LESS reads as MORE dangerous
                    s.MoteCount = Mathf.RoundToInt(18 * intensity);                    // a few hot glints
                    s.MoteTint = new Color(1.0f, 0.95f, 0.55f, 1f);
                    s.MoteSize = 0.03f;
                    s.Drift = DriftStyle.Sparse;
                    break;
                case "flood":
                    s.HazeTint = new Color(0.55f, 0.70f, 0.75f, 0.45f * intensity);    // mist off the water
                    s.HazeHeight = 8f;                                                 // ground-hugging
                    s.MoteCount = Mathf.RoundToInt(60 * intensity);
                    s.MoteTint = new Color(0.85f, 0.95f, 1.0f, 0.5f);
                    s.DriftSpeed = 0.4f;
                    break;
                case "vibration":
                case "cave-in":
                    s.HazeEnabled = false;                                             // interior dust, not sky
                    s.MoteCount = Mathf.RoundToInt(70 * intensity);
                    s.MoteTint = new Color(0.75f, 0.68f, 0.60f, 0.7f);
                    s.Drift = DriftStyle.Puff;                                         // shaken loose, not drifting
                    break;
                case "static":
                    s.HazeTint = new Color(0.70f, 0.75f, 0.85f, 0.30f * intensity);
                    s.MoteCount = Mathf.RoundToInt(70 * intensity);
                    s.MoteTint = new Color(0.80f, 0.88f, 1.0f, 0.8f);
                    s.Drift = DriftStyle.Puff;                                         // flickering density
                    s.DriftSpeed = 2.2f;
                    break;
                case "pattern":
                    s.HazeTint = new Color(0.60f, 0.55f, 0.85f, 0.30f * intensity);
                    s.MoteCount = Mathf.RoundToInt(60 * intensity);
                    s.MoteTint = new Color(0.75f, 0.70f, 1.0f, 0.9f);
                    s.Drift = DriftStyle.Glitch;                                       // motion that feels WRONG
                    break;
                case "reflection":
                    s.HazeTint = new Color(0.80f, 0.85f, 0.90f, 0.28f * intensity);    // the doubled horizon
                    s.HazeHeight = 24f;                                                // taller band = mirrored read
                    s.MoteCount = Mathf.RoundToInt(40 * intensity);
                    s.MoteTint = new Color(0.95f, 0.97f, 1.0f, 0.6f);
                    break;
                case "wind":
                    s.HazeTint = new Color(0.80f, 0.70f, 0.50f, 0.35f * intensity);
                    s.MoteCount = Mathf.RoundToInt(110 * intensity);
                    s.MoteTint = new Color(0.90f, 0.80f, 0.60f, 0.75f);
                    s.Drift = DriftStyle.Wind;                                         // dragged, not drifting
                    s.DriftSpeed = 3.0f;
                    break;
                case "swarm":
                    // The swarm itself is the particulate (distant silhouettes) — no separate layer.
                    s.HazeTint = new Color(0.45f, 0.40f, 0.35f, 0.25f * intensity);
                    s.MoteCount = 0;
                    break;
                case "void":
                    s.HazeTint = new Color(0.30f, 0.35f, 0.50f, 0.08f * intensity);    // raw space: thin to none
                    s.MoteCount = 0;
                    break;
                case "pressure":
                    s.HazeTint = new Color(0.20f, 0.40f, 0.50f, 0.55f * intensity);    // water, not air — same pillar
                    s.MoteCount = Mathf.RoundToInt(80 * intensity);
                    s.MoteTint = new Color(0.70f, 0.85f, 0.85f, 0.55f);                // sediment
                    s.DriftSpeed = 0.3f;
                    break;
                case "all":
                    s.HazeTint = new Color(0.75f, 0.60f, 0.55f, 0.50f * intensity);    // convergence: overload is correct
                    s.MoteCount = Mathf.RoundToInt(150 * intensity);
                    s.MoteTint = new Color(0.95f, 0.80f, 0.70f, 0.85f);
                    s.DriftSpeed = 1.6f;
                    break;
                default: // "none" or unknown — defer entirely to the vista
                    s.HazeEnabled = false;
                    s.MoteCount = 0;
                    break;
            }
            if (intensity <= 0f) { s.HazeEnabled = false; s.MoteCount = 0; }
            return s;
        }

        // ── Drift math: mote position is a PURE function of (seed, index, time) ──

        /// <summary>Local-space mote position inside the shell. Deterministic and bounded — the rig
        /// just evaluates it every frame; there is no simulation state to save or desync.</summary>
        public static Vector3 MotePosition(int seed, int index, float time, in AtmosphereSpec spec)
        {
            // Stable per-mote basis from a xorshift hash (the project's seeded idiom).
            uint h = Hash((uint)(seed * 7919 + index * 31 + 1));
            float u0 = Frac01(h);          // azimuth
            float u1 = Frac01(h = Hash(h)); // radius blend
            float u2 = Frac01(h = Hash(h)); // height
            float u3 = Frac01(h = Hash(h)); // phase

            float radius = Mathf.Lerp(spec.ShellNear, spec.ShellFar, u1);
            float angle = u0 * Mathf.PI * 2f;
            float baseY = Mathf.Lerp(-1.5f, 6f, u2);
            float phase = u3 * Mathf.PI * 2f;
            float t = time * spec.DriftSpeed;

            switch (spec.Drift)
            {
                case DriftStyle.Wind:
                {
                    // Dragged sideways through the shell; wraps around. Mostly lateral, tiny bob.
                    float sweep = angle + t * 0.55f;
                    return new Vector3(Mathf.Cos(sweep) * radius,
                                       baseY + Mathf.Sin(t * 2f + phase) * 0.15f,
                                       Mathf.Sin(sweep) * radius);
                }
                case DriftStyle.Puff:
                {
                    // Irregular bursts: motion happens in short windows, then settles.
                    float cycle = Mathf.Repeat(t * 0.5f + u3 * 3f, 3f);          // 3s cycles, offset per mote
                    float burst = Mathf.Clamp01(1f - cycle);                      // active in the first second
                    float kick = burst * Mathf.Sin(t * 9f + phase) * 0.8f;
                    return new Vector3(Mathf.Cos(angle) * radius + kick,
                                       baseY + burst * 1.2f,
                                       Mathf.Sin(angle) * radius + kick * 0.6f);
                }
                case DriftStyle.Glitch:
                {
                    // Position STEPS between poses instead of flowing — deliberately unnatural.
                    float step = Mathf.Floor(t * 1.5f + u3 * 7f);
                    uint g = Hash((uint)(seed * 131 + index * 17) + (uint)step);
                    float ga = Frac01(g) * Mathf.PI * 2f;
                    float gy = Mathf.Lerp(-1f, 5f, Frac01(Hash(g)));
                    return new Vector3(Mathf.Cos(ga) * radius, gy, Mathf.Sin(ga) * radius);
                }
                default: // Steady / Sparse — slow orbital drift with a gentle sink-and-rise.
                {
                    float sweep = angle + t * 0.045f;
                    float bob = Mathf.Sin(t * 0.35f + phase) * 0.9f;
                    return new Vector3(Mathf.Cos(sweep) * radius,
                                       baseY + bob,
                                       Mathf.Sin(sweep) * radius);
                }
            }
        }

        private static uint Hash(uint x)
        {
            x ^= x << 13; x ^= x >> 17; x ^= x << 5;
            return x == 0 ? 2463534242u : x;
        }

        private static float Frac01(uint h) => (h & 0xFFFFFF) / (float)0x1000000;
    }
}

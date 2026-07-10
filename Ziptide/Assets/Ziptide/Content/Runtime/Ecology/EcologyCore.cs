using System;
using System.Collections.Generic;

namespace Ziptide.Content.Ecology
{
    /// <summary>Per-species ecology parameters — pure presets keyed by creatureId (the
    /// BotProfileData idiom: code presets are the fallback truth; SO authoring can mirror later).
    /// Every axis is a mechanic, not a label (LAW 6): capacity shapes density, growth shapes
    /// recovery, diet couples species, activity shapes WHEN a world feels alive, pack size shapes
    /// encounters.</summary>
    public sealed class EcologySpecies
    {
        public string CreatureId;
        public float CarryingCapacity;  // K — the zone's sustainable population
        public float GrowthPerHour;     // r — logistic growth rate (also the recovery speed)
        public int PackSize;            // typical band; 1 = solitary
        public string PreyId;           // hunts this species (null = grazes flora)
        public float PredationRate;     // how hard a full predator pop suppresses its prey
        public float DayActivity01;     // fraction of the population abroad by day…
        public float NightActivity01;   // …and by night (the rest are denned up)
        public int NestsPerZone;        // physical homes the director will stamp

        /// <summary>The authored roster as an ecosystem — grazers, swarms, hunters, an apex, and
        /// the Warden (a lawful construct OUTSIDE the food web: fixed population, no growth).</summary>
        public static readonly EcologySpecies[] All =
        {
            new EcologySpecies { CreatureId = "swarm_bug", CarryingCapacity = 24f, GrowthPerHour = 0.35f,
                PackSize = 6, PreyId = null, PredationRate = 0f,
                DayActivity01 = 0.7f, NightActivity01 = 0.9f, NestsPerZone = 3 },
            new EcologySpecies { CreatureId = "light_grazer", CarryingCapacity = 12f, GrowthPerHour = 0.15f,
                PackSize = 4, PreyId = null, PredationRate = 0f,
                DayActivity01 = 0.9f, NightActivity01 = 0.2f, NestsPerZone = 2 },
            new EcologySpecies { CreatureId = "tether_swarm", CarryingCapacity = 10f, GrowthPerHour = 0.2f,
                PackSize = 5, PreyId = "swarm_bug", PredationRate = 0.10f,
                DayActivity01 = 0.3f, NightActivity01 = 0.9f, NestsPerZone = 2 },
            new EcologySpecies { CreatureId = "witness_mite", CarryingCapacity = 6f, GrowthPerHour = 0.12f,
                PackSize = 1, PreyId = "swarm_bug", PredationRate = 0.05f,
                DayActivity01 = 0.2f, NightActivity01 = 0.8f, NestsPerZone = 1 },
            new EcologySpecies { CreatureId = "husk_molter", CarryingCapacity = 5f, GrowthPerHour = 0.08f,
                PackSize = 1, PreyId = null, PredationRate = 0f,
                DayActivity01 = 0.5f, NightActivity01 = 0.6f, NestsPerZone = 1 },
            new EcologySpecies { CreatureId = "stalker", CarryingCapacity = 3f, GrowthPerHour = 0.06f,
                PackSize = 1, PreyId = "light_grazer", PredationRate = 0.15f,
                DayActivity01 = 0.1f, NightActivity01 = 0.9f, NestsPerZone = 1 },
            new EcologySpecies { CreatureId = "warden", CarryingCapacity = 1f, GrowthPerHour = 0f,
                PackSize = 1, PreyId = null, PredationRate = 0f,
                DayActivity01 = 1f, NightActivity01 = 1f, NestsPerZone = 0 },
        };

        public static EcologySpecies Find(string creatureId)
        {
            foreach (var s in All) if (s.CreatureId == creatureId) return s;
            return null;
        }
    }

    /// <summary>A recorded player disturbance: creatures DISABLED (never killed — non-lethal canon)
    /// suppress the local population until it recovers.</summary>
    [Serializable]
    public struct EcologyPressure
    {
        public string creatureId;
        public int disabled;
        public long atUnix;
    }

    /// <summary>
    /// CREATURE ECOLOGY 4.3 — the PURE population engine. Deterministic per (seed, elapsed,
    /// pressures): the same world resolves the same living state everywhere, forever — the save/
    /// offline law the whole project runs on. The model, hourly-stepped and capped: logistic growth
    /// toward carrying capacity · predator/prey coupling (a fed hunter population suppresses its
    /// prey; starvation suppresses the hunter) · player disable-pressure that DECAYS (populations
    /// always recover — the non-lethal canon made math) · a floor of one (nothing ever goes
    /// extinct; a world never empties permanently) · day/night activity (who is abroad WHEN) ·
    /// pack composition · and a hard active-spawn budget (the perf law). Pinned by EcologyCoreTests.
    /// </summary>
    public static class EcologyCore
    {
        public const int MaxResolveHours = 24 * 14;    // offline resolve window (mirrors idle economy)
        public const float PressureHalfLifeHours = 6f; // disables fade — the wild heals
        public const float ExtinctionFloor = 1f;       // non-lethal canon: never zero, always recovery
        public const int DefaultActiveBudget = 14;     // max live creatures per world (perf cap)

        /// <summary>Resolve every species' population after <paramref name="elapsedHours"/>,
        /// starting from a seeded initial state and applying recorded disturbances.</summary>
        public static Dictionary<string, float> PopulationsAt(int worldSeed, float elapsedHours,
            IList<EcologyPressure> pressures = null, long nowUnix = 0)
        {
            var pops = new Dictionary<string, float>();
            var rng = new Rng(worldSeed);
            foreach (var s in EcologySpecies.All)
            {
                // Seeded start: settled worlds sit near capacity with local character.
                float jitter = 0.6f + rng.Next01() * 0.35f;
                pops[s.CreatureId] = Math.Max(ExtinctionFloor, s.CarryingCapacity * jitter);
            }

            float hours = Math.Min(Math.Max(0f, elapsedHours), MaxResolveHours);
            int steps = (int)Math.Ceiling(hours);
            for (int h = 0; h < steps; h++)
            {
                float dt = Math.Min(1f, hours - h);
                foreach (var s in EcologySpecies.All)
                {
                    if (s.GrowthPerHour <= 0f) continue; // constructs sit outside the food web
                    float p = pops[s.CreatureId];

                    // Logistic growth toward K — also THE recovery mechanic after disturbance.
                    float growth = s.GrowthPerHour * p * (1f - p / s.CarryingCapacity);

                    // Predation coupling: hunters suppress prey; a fed hunter grows a little faster.
                    float predation = 0f;
                    foreach (var hunter in EcologySpecies.All)
                        if (hunter.PreyId == s.CreatureId)
                            predation += hunter.PredationRate * pops[hunter.CreatureId]
                                         * (p / s.CarryingCapacity);
                    float feeding = 0f;
                    if (s.PreyId != null && pops.TryGetValue(s.PreyId, out float preyPop))
                    {
                        var preySpec = EcologySpecies.Find(s.PreyId);
                        float preyAbundance = preySpec != null ? preyPop / preySpec.CarryingCapacity : 0f;
                        feeding = s.GrowthPerHour * 0.5f * p * (preyAbundance - 0.5f); // scarce prey starves the hunter
                    }

                    p += (growth - predation + feeding) * dt;
                    pops[s.CreatureId] = Math.Max(ExtinctionFloor, Math.Min(p, s.CarryingCapacity * 1.25f));
                }
            }

            // Player disturbance: each recorded disable suppresses its species, decaying with a
            // half-life — the wild always heals (non-lethal canon).
            if (pressures != null)
            {
                foreach (var pr in pressures)
                {
                    if (pr.disabled <= 0 || string.IsNullOrEmpty(pr.creatureId)) continue;
                    if (!pops.ContainsKey(pr.creatureId)) continue;
                    float ageHours = nowUnix > pr.atUnix ? (nowUnix - pr.atUnix) / 3600f : 0f;
                    float remaining = pr.disabled * (float)Math.Pow(0.5, ageHours / PressureHalfLifeHours);
                    pops[pr.creatureId] = Math.Max(ExtinctionFloor, pops[pr.creatureId] - remaining);
                }
            }

            return pops;
        }

        /// <summary>Fraction of a species abroad at a time of day (hour01: 0 = midnight, 0.5 = noon).
        /// Smooth dawn/dusk blend — a world's cast changes with its clock.</summary>
        public static float ActivityAt(EcologySpecies s, float hour01)
        {
            if (s == null) return 0f;
            // Day weight peaks at noon, night weight at midnight, cosine-smooth between.
            float day = 0.5f - 0.5f * (float)Math.Cos((hour01 - 0.0f) * 2.0 * Math.PI); // 0 at midnight → 1 at noon
            return s.NightActivity01 + (s.DayActivity01 - s.NightActivity01) * day;
        }

        /// <summary>Split an abroad population into packs (deterministic): full bands first, a
        /// remainder band last, solitary species one by one. The encounter grammar of the world.</summary>
        public static List<int> PacksFor(EcologySpecies s, float population, int seed)
        {
            var packs = new List<int>();
            if (s == null) return packs;
            int abroad = (int)Math.Floor(population);
            int size = Math.Max(1, s.PackSize);
            var rng = new Rng(seed ^ s.CreatureId.GetHashCode());
            while (abroad > 0)
            {
                int band = Math.Min(abroad, size <= 1 ? 1 : size - 1 + (int)(rng.Next01() * 3f)); // 6→5..7
                packs.Add(band);
                abroad -= band;
            }
            return packs;
        }

        /// <summary>Cap the live-spawn total at the perf budget: larger packs first (they ARE the
        /// encounter), then fill with smaller ones; a pack never partially spawns.</summary>
        public static List<int> CapToBudget(List<int> packs, int budget = DefaultActiveBudget)
        {
            var chosen = new List<int>();
            if (packs == null) return chosen;
            var sorted = new List<int>(packs);
            sorted.Sort((a, b) => b.CompareTo(a));
            int used = 0;
            foreach (var pack in sorted)
                if (used + pack <= budget) { chosen.Add(pack); used += pack; }
            return chosen;
        }

        // The project's xorshift idiom — deterministic, allocation-free.
        private struct Rng
        {
            private uint _s;
            public Rng(int seed) { _s = seed == 0 ? 2463534242u : (uint)seed; }
            public float Next01() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return (_s & 0xFFFFFF) / (float)0x1000000; }
        }
    }
}

using System;

namespace Ziptide.Core
{
    /// <summary>Heritable plant genes (GARDEN AAA §genetics — the mutation chase). Three axes:
    /// growth speed, harvest yield, and size (past the giant threshold = a GIANT crop, the
    /// screenshot moment). Serializable into PlotState; neutral baseline reproduces pre-genetics
    /// behavior exactly, so old saves are untouched.</summary>
    [Serializable]
    public struct PlantGenes
    {
        public float speed;      // growth-rate multiplier (divides grow time at plant time)
        public float yield;      // harvest multiplier (joins tend × timing in GardenService)
        public float size;       // 0..1 — at/after GiantThreshold the crop is a GIANT
        public int generation;   // breeding lineage depth (0 = wild)

        public static PlantGenes Baseline => new PlantGenes { speed = 1f, yield = 1f, size = 0.35f };
    }

    public enum PlantRarity { Common, Uncommon, Rare, Legendary }

    /// <summary>
    /// GARDEN AAA — the PURE genetics engine. Deterministic per seed (same cross replays
    /// identically — the save/async law), and ECONOMY-SAFE BY CLAMP: no breeding line, however
    /// long, can escape the stat ceilings (pinned by a 200-generation test). Roll wild seeds,
    /// cross two mature plants, let world hazards kick mutations — the chase Roblox's garden runs
    /// on, but deterministic and headless-testable.
    /// </summary>
    public static class PlantGenetics
    {
        public const float SpeedMin = 0.5f, SpeedMax = 2.2f;
        public const float YieldMin = 0.5f, YieldMax = 2.5f;
        public const float GiantThreshold = 0.85f;
        public const float GiantYieldBonus = 3f;

        /// <summary>A wild seed: near baseline with a little variance — store-bought is never the chase.</summary>
        public static PlantGenes RollWild(int seed)
        {
            var rng = new Rng(seed);
            return Clamp(new PlantGenes
            {
                speed = 1f + (rng.Next01() - 0.5f) * 0.3f,
                yield = 1f + (rng.Next01() - 0.5f) * 0.3f,
                size = 0.2f + rng.Next01() * 0.3f,
                generation = 0,
            });
        }

        /// <summary>Cross two mature plants: each axis inherits around the parents' midpoint with
        /// spread, and ~12% of axes take a MUTATION KICK (the jackpot roll). Deterministic per seed.</summary>
        public static PlantGenes Cross(PlantGenes a, PlantGenes b, int seed)
        {
            var rng = new Rng(seed);
            return Clamp(new PlantGenes
            {
                speed = Inherit(a.speed, b.speed, ref rng),
                yield = Inherit(a.yield, b.yield, ref rng),
                size = Inherit(a.size, b.size, ref rng),
                generation = Math.Max(a.generation, b.generation) + 1,
            });
        }

        /// <summary>A world hazard (radiation, static bloom) kicks the genes — variance scales with
        /// strength, biased UP on size (hazard gardens are where giants come from). Deterministic.</summary>
        public static PlantGenes HazardKick(PlantGenes g, float strength01, int seed)
        {
            strength01 = strength01 < 0f ? 0f : (strength01 > 1f ? 1f : strength01);
            if (strength01 <= 0f) return g;
            var rng = new Rng(seed);
            g.speed += (rng.Next01() - 0.5f) * 0.5f * strength01;
            g.yield += (rng.Next01() - 0.5f) * 0.6f * strength01;
            g.size += (rng.Next01() - 0.25f) * 0.5f * strength01; // biased upward — the giant chase
            return Clamp(g);
        }

        /// <summary>Rarity from combined gene score — the almanac/trading tier. Monotonic: better
        /// genes never rank lower.</summary>
        public static PlantRarity Rarity(PlantGenes g)
        {
            float score = (g.speed - 1f) + (g.yield - 1f) + g.size;
            if (score >= 1.5f) return PlantRarity.Legendary;
            if (score >= 0.9f) return PlantRarity.Rare;
            if (score >= 0.4f) return PlantRarity.Uncommon;
            return PlantRarity.Common;
        }

        public static bool IsGiant(PlantGenes g) => g.size >= GiantThreshold;

        /// <summary>The yield factor genetics contributes at harvest (gene yield × giant bonus).</summary>
        public static double HarvestFactor(PlantGenes g)
        {
            double f = g.yield > 0f ? g.yield : 1.0;
            if (IsGiant(g)) f *= GiantYieldBonus;
            return f;
        }

        private static float Inherit(float pa, float pb, ref Rng rng)
        {
            float mid = (pa + pb) * 0.5f;
            float spread = Math.Abs(pa - pb) * 0.5f + 0.05f;
            float v = mid + (rng.Next01() - 0.5f) * 2f * spread;
            if (rng.Next01() < 0.12f)                      // the mutation kick — the jackpot roll
                v += (rng.Next01() - 0.35f) * 0.5f;        // biased slightly upward: chases feel winnable
            return v;
        }

        private static PlantGenes Clamp(PlantGenes g)
        {
            g.speed = g.speed < SpeedMin ? SpeedMin : (g.speed > SpeedMax ? SpeedMax : g.speed);
            g.yield = g.yield < YieldMin ? YieldMin : (g.yield > YieldMax ? YieldMax : g.yield);
            g.size = g.size < 0f ? 0f : (g.size > 1f ? 1f : g.size);
            return g;
        }

        // The project's xorshift idiom (ScatterField) — deterministic, allocation-free.
        private struct Rng
        {
            private uint _s;
            public Rng(int seed) { _s = seed == 0 ? 2463534242u : (uint)seed; }
            public float Next01() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return (_s & 0xFFFFFF) / (float)0x1000000; }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>A planned cave chamber: a position (Y is depth — caves stack), a radius, and a role
    /// derived from its connectivity (dead-ends become secret/loot spots; junctions become fights).</summary>
    public sealed class CaveChamber
    {
        public int Id;
        public float X, Y, Z;
        public float Radius;
        public int Links;               // filled by the planner
        public bool IsDeadEnd => Links <= 1;
        public bool IsJunction => Links >= 3;
    }

    /// <summary>A planned tunnel between two chambers. Steep tunnels are vertical SHAFTS — the
    /// traversal hooks (climb studs / lift) attach there instead of a walkable ramp.</summary>
    public sealed class CaveTunnel
    {
        public int FromId, ToId;
        public bool IsShaft;            // |ΔY| dominates the run — needs climb/lift, not walking
    }

    /// <summary>The full plan: chambers + tunnels, guaranteed connected, deterministic per seed.</summary>
    public sealed class CavePlan
    {
        public readonly List<CaveChamber> Chambers = new List<CaveChamber>();
        public readonly List<CaveTunnel> Tunnels = new List<CaveTunnel>();
    }

    /// <summary>
    /// PURE seeded cave-network planning (Hardwiring 1.4 — the brain behind the locked "modular-kit
    /// caverns" decision). A heightmap can't make caves, so an underground layer is a GRAPH first:
    /// scatter chambers with minimum spacing inside a bounded volume (depth included — caves stack),
    /// connect them into a guaranteed-connected tree (Prim's MST by distance), then add a seeded
    /// fraction of loop tunnels (pure trees are boring to explore; loops give route choice). Steep
    /// tunnels classify as SHAFTS so the builder attaches climb/lift traversal instead of a ramp, and
    /// dead-end chambers are the natural secret/loot spots ("Room to expand": hidden cave secrets).
    /// No UnityEngine → EditMode-testable; the CavernKit builder (next) consumes the plan and strings
    /// registry modules along it; MultiLevelReachability can audit the result.
    /// xorshift RNG inline — the deterministic-seed idiom this codebase already uses (BotRng/LotPartitioner).
    /// </summary>
    public static class CaveNetworkPlanner
    {
        /// <summary>Plan a network. All parameters have safe clamps; same inputs → identical plan.</summary>
        /// <param name="seed">Deterministic seed.</param>
        /// <param name="extentX">Half-extent of the volume on X (chambers in [-extentX, extentX]).</param>
        /// <param name="extentZ">Half-extent on Z.</param>
        /// <param name="depth">Vertical range: chamber Y in [-depth, 0].</param>
        /// <param name="chamberTarget">How many chambers to try for (placement may yield fewer).</param>
        /// <param name="minSpacing">Minimum center-to-center distance between chambers.</param>
        /// <param name="loopChance">0..1 — chance each nearby non-tree pair gains a loop tunnel.</param>
        public static CavePlan Plan(int seed, float extentX = 40f, float extentZ = 40f,
            float depth = 18f, int chamberTarget = 10, float minSpacing = 10f, float loopChance = 0.35f)
        {
            extentX = Math.Max(5f, extentX);
            extentZ = Math.Max(5f, extentZ);
            depth = Math.Max(0f, depth);
            chamberTarget = Math.Max(2, chamberTarget);
            minSpacing = Math.Max(1f, minSpacing);
            loopChance = Math.Max(0f, Math.Min(1f, loopChance));

            var plan = new CavePlan();
            uint rng = (uint)(seed == 0 ? 1 : seed);

            // 1. Chamber scatter: dart-throwing with min spacing (Poisson-flavored, bounded attempts).
            int attempts = chamberTarget * 12;
            while (plan.Chambers.Count < chamberTarget && attempts-- > 0)
            {
                float x = Lerp(-extentX, extentX, Next01(ref rng));
                float z = Lerp(-extentZ, extentZ, Next01(ref rng));
                float y = -depth * Next01(ref rng);
                bool clear = true;
                foreach (var c in plan.Chambers)
                    if (Dist(c.X, c.Y, c.Z, x, y, z) < minSpacing) { clear = false; break; }
                if (!clear) continue;
                plan.Chambers.Add(new CaveChamber
                {
                    Id = plan.Chambers.Count,
                    X = x, Y = y, Z = z,
                    Radius = Lerp(3f, 6.5f, Next01(ref rng)),
                });
            }
            if (plan.Chambers.Count < 2) // degenerate volume: force a second chamber so the plan is a cave
                plan.Chambers.Add(new CaveChamber { Id = plan.Chambers.Count, X = extentX * 0.5f, Y = 0f, Z = 0f, Radius = 4f });

            // 2. Prim's MST by distance — GUARANTEED connected before any decoration.
            int n = plan.Chambers.Count;
            var inTree = new bool[n];
            inTree[0] = true;
            var linked = new HashSet<long>();
            for (int added = 1; added < n; added++)
            {
                float best = float.MaxValue; int bi = -1, bj = -1;
                for (int i = 0; i < n; i++)
                {
                    if (!inTree[i]) continue;
                    for (int j = 0; j < n; j++)
                    {
                        if (inTree[j]) continue;
                        float d = Dist(plan.Chambers[i], plan.Chambers[j]);
                        if (d < best) { best = d; bi = i; bj = j; }
                    }
                }
                if (bj < 0) break;
                inTree[bj] = true;
                AddTunnel(plan, linked, bi, bj);
            }

            // 3. Seeded loops between NEAR pairs the tree didn't join — route choice, not spaghetti.
            float loopRadius = minSpacing * 2.2f;
            for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                if (linked.Contains(Key(i, j))) continue;
                if (Dist(plan.Chambers[i], plan.Chambers[j]) > loopRadius) continue;
                if (Next01(ref rng) < loopChance) AddTunnel(plan, linked, i, j);
            }

            return plan;
        }

        /// <summary>BFS connectivity check — the audit's (and tests') ground truth.</summary>
        public static bool IsConnected(CavePlan plan)
        {
            if (plan == null || plan.Chambers.Count == 0) return false;
            var adj = new List<int>[plan.Chambers.Count];
            foreach (var t in plan.Tunnels)
            {
                (adj[t.FromId] ??= new List<int>()).Add(t.ToId);
                (adj[t.ToId] ??= new List<int>()).Add(t.FromId);
            }
            var seen = new bool[plan.Chambers.Count];
            var q = new Queue<int>();
            seen[0] = true; q.Enqueue(0);
            int count = 1;
            while (q.Count > 0)
            {
                int c = q.Dequeue();
                if (adj[c] == null) continue;
                foreach (int nxt in adj[c])
                    if (!seen[nxt]) { seen[nxt] = true; count++; q.Enqueue(nxt); }
            }
            return count == plan.Chambers.Count;
        }

        private const float ShaftSlope = 1.0f; // |ΔY| >= horizontal run → a shaft, not a ramp

        private static void AddTunnel(CavePlan plan, HashSet<long> linked, int i, int j)
        {
            var a = plan.Chambers[i]; var b = plan.Chambers[j];
            float dy = Math.Abs(a.Y - b.Y);
            float run = (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z));
            plan.Tunnels.Add(new CaveTunnel { FromId = i, ToId = j, IsShaft = dy >= run * ShaftSlope });
            linked.Add(Key(i, j));
            a.Links++; b.Links++;
        }

        private static long Key(int i, int j) => i < j ? ((long)i << 32) | (uint)j : ((long)j << 32) | (uint)i;

        private static float Dist(CaveChamber a, CaveChamber b) => Dist(a.X, a.Y, a.Z, b.X, b.Y, b.Z);
        private static float Dist(float ax, float ay, float az, float bx, float by, float bz)
        {
            float dx = ax - bx, dy = ay - by, dz = az - bz;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;

        // xorshift32 — the codebase's deterministic-seed idiom, inlined (no shared RNG dependency).
        private static float Next01(ref uint state)
        {
            state ^= state << 13; state ^= state >> 17; state ^= state << 5;
            return (state & 0xFFFFFF) / (float)0x1000000;
        }
    }
}

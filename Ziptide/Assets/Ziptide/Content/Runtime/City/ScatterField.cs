using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>A scatter placement: XZ position, prop kind, and a 0..1 size roll.</summary>
    public struct ScatterPoint
    {
        public Vector2 Position;
        public int Kind;
        public float Size01;
    }

    /// <summary>An exclusion capsule in XZ: segment A→B swollen by Radius (A==B makes a disc).
    /// Pads, POI pockets and route legs all express as these.</summary>
    public struct ScatterMask
    {
        public Vector2 A, B;
        public float Radius;
        public static ScatterMask Disc(Vector2 center, float radius) => new ScatterMask { A = center, B = center, Radius = radius };
        public static ScatterMask Capsule(Vector2 a, Vector2 b, float radius) => new ScatterMask { A = a, B = b, Radius = radius };
    }

    /// <summary>
    /// ARCHITECTURE V2 H5 (PDF §dressing) — PURE seeded Poisson-disk scatter (Bridson's algorithm)
    /// replacing hand-rolled polar scatter: blue-noise spacing (no clumps, no bald patches), a 0..1
    /// DENSITY CHANNEL (probabilistic thinning — feed it TerrainField.Climate moisture and vegetation
    /// follows water), exclusion masks (nothing spawns on pads/routes/POIs), and per-kind minimum
    /// spacing (two bone-arcs shouldn't share a dune; tufts may).
    ///
    /// Contracts (pinned by ScatterFieldTests): deterministic from seed · every point inside the
    /// radius, outside every mask, ≥ minSpacing from every other point · same-kind pairs additionally
    /// honor kindSpacings · density 0 regions stay empty. Consumed by WorldDressingBuilder.
    /// </summary>
    public static class ScatterField
    {
        private const int Attempts = 20; // Bridson's k

        private struct Rng
        {
            private uint _s;
            public Rng(int seed) { _s = seed == 0 ? 2463534242u : (uint)seed; }
            public float Next01() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return (_s & 0xFFFFFF) / (float)0x1000000; }
        }

        /// <summary>Generate scatter points inside a disc of <paramref name="radius"/> around the
        /// origin. <paramref name="density01"/> may be null (uniform). <paramref name="kindSpacings"/>
        /// may be null (no same-kind constraint); entries ≤ minSpacing are free.</summary>
        public static List<ScatterPoint> Generate(float radius, float minSpacing, int kinds, int seed,
            List<ScatterMask> exclusions, System.Func<float, float, float> density01,
            float[] kindSpacings = null)
        {
            var result = new List<ScatterPoint>();
            if (radius <= 0f || minSpacing <= 0.1f) return result;
            if (kinds < 1) kinds = 1;

            var rng = new Rng(seed);

            // Bridson: background grid at cellSize = r/√2 so each cell holds at most one point.
            float cell = minSpacing / 1.41421356f;
            int gridExtent = Mathf.CeilToInt(radius * 2f / cell) + 4;
            var grid = new Dictionary<(int, int), Vector2>();
            var active = new List<Vector2>();
            var accepted = new List<Vector2>();

            (int, int) CellOf(Vector2 p) => (Mathf.FloorToInt((p.x + radius) / cell), Mathf.FloorToInt((p.y + radius) / cell));

            bool FarEnough(Vector2 p)
            {
                var (cx, cy) = CellOf(p);
                for (int dy = -2; dy <= 2; dy++)
                    for (int dx = -2; dx <= 2; dx++)
                        if (grid.TryGetValue((cx + dx, cy + dy), out var q)
                            && (q - p).sqrMagnitude < minSpacing * minSpacing)
                            return false;
                return true;
            }

            void Accept(Vector2 p)
            {
                grid[CellOf(p)] = p;
                active.Add(p);
                accepted.Add(p);
            }

            // Seed point: deterministic, near the middle but off-center.
            var first = new Vector2((rng.Next01() - 0.5f) * radius * 0.5f, (rng.Next01() - 0.5f) * radius * 0.5f);
            Accept(first);

            while (active.Count > 0 && accepted.Count < 20000)
            {
                int idx = (int)(rng.Next01() * active.Count);
                if (idx >= active.Count) idx = active.Count - 1;
                Vector2 basePt = active[idx];
                bool spawned = false;
                for (int a = 0; a < Attempts; a++)
                {
                    float ang = rng.Next01() * Mathf.PI * 2f;
                    float dist = minSpacing * (1f + rng.Next01());
                    Vector2 p = basePt + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * dist;
                    if (p.sqrMagnitude > radius * radius) continue;
                    if (!FarEnough(p)) continue;
                    Accept(p);
                    spawned = true;
                }
                if (!spawned) active.RemoveAt(idx);
            }

            // Thin by density channel + masks, then roll kind/size — one rng stream, still deterministic.
            var kept = new List<ScatterPoint>();
            foreach (var p in accepted)
            {
                float keepRoll = rng.Next01();
                float kindRoll = rng.Next01();
                float sizeRoll = rng.Next01();
                if (Excluded(p, exclusions)) continue;
                float d = density01 != null ? Mathf.Clamp01(density01(p.x, p.y)) : 1f;
                if (keepRoll >= d) continue;
                kept.Add(new ScatterPoint
                {
                    Position = p,
                    Kind = Mathf.Min(kinds - 1, (int)(kindRoll * kinds)),
                    Size01 = sizeRoll,
                });
            }

            // Per-kind spacing: greedy deterministic pass — later same-kind violators drop.
            if (kindSpacings != null)
            {
                for (int i = 0; i < kept.Count; i++)
                {
                    var pi = kept[i];
                    if (pi.Kind >= kindSpacings.Length) { result.Add(pi); continue; }
                    float spacing = kindSpacings[pi.Kind];
                    if (spacing <= minSpacing) { result.Add(pi); continue; }
                    bool ok = true;
                    for (int j = 0; j < result.Count; j++)
                    {
                        if (result[j].Kind != pi.Kind) continue;
                        if ((result[j].Position - pi.Position).sqrMagnitude < spacing * spacing) { ok = false; break; }
                    }
                    if (ok) result.Add(pi);
                }
                return result;
            }

            result.AddRange(kept);
            return result;
        }

        /// <summary>True if the point lies inside any exclusion capsule.</summary>
        public static bool Excluded(Vector2 p, List<ScatterMask> masks)
        {
            if (masks == null) return false;
            for (int i = 0; i < masks.Count; i++)
            {
                Vector2 ab = masks[i].B - masks[i].A;
                float len2 = ab.sqrMagnitude;
                float t = len2 < 0.0001f ? 0f : Mathf.Clamp01(Vector2.Dot(p - masks[i].A, ab) / len2);
                if ((masks[i].A + ab * t - p).sqrMagnitude < masks[i].Radius * masks[i].Radius)
                    return true;
            }
            return false;
        }
    }
}

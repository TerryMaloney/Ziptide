using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>An interior layout produced by <see cref="RoomPartitioner"/>: inset rooms (the gaps
    /// between them are walls) plus corridor strips guaranteed to connect every room.</summary>
    public struct InteriorPlan
    {
        public List<Rect> Rooms;
        public List<Rect> Corridors;
        public bool IsEmpty => Rooms == null || Rooms.Count == 0;
    }

    /// <summary>
    /// ARCHITECTURE V2 H2 (PDF §Binary Space Partitioning) — PURE seeded interior partitioning for
    /// ship decks, hero-building interiors and hive worlds. A footprint rectangle is BSP-subdivided
    /// into leaves (LotPartitioner's recipe: same Rng, min-area / MinSide / aspect base cases); each
    /// leaf's ROOM is the leaf inset by a wall margin; then, walking back UP the tree, every internal
    /// node carves an L-shaped corridor between a representative room of each child subtree.
    ///
    /// THE ACCESS RULE (provable, pinned by RoomPartitionerTests): a corridor's endpoints are always
    /// ROOM CENTERS, so by induction every subtree's rooms+corridors form one connected component —
    /// the finished plan is fully reachable, no orphan rooms, ever. Deterministic from seed.
    /// </summary>
    public static class RoomPartitioner
    {
        public const int MaxDepth = 6;
        /// <summary>Minimum room side (m) — VR-walkable with a CharacterController.</summary>
        public const float MinSide = 2.2f;
        /// <summary>Wall gap between a leaf's edge and its room (each side).</summary>
        public const float WallInset = 0.35f;

        /// <summary>Seeded xorshift (the LotPartitioner/BotRng recipe) — no UnityEngine.Random.</summary>
        private struct Rng
        {
            private uint _s;
            public Rng(int seed) { _s = seed == 0 ? 2463534242u : (uint)seed; }
            public float Next01() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return (_s & 0xFFFFFF) / (float)0x1000000; }
        }

        /// <summary>Partition a footprint into a connected interior. Footprints too small for one
        /// room return a single full-footprint room (a cabin) or an empty plan when degenerate.</summary>
        public static InteriorPlan Partition(Rect footprint, float corridorWidth, float minRoomArea,
            float maxAspect, int seed)
        {
            var plan = new InteriorPlan { Rooms = new List<Rect>(), Corridors = new List<Rect>() };
            if (corridorWidth < 0.8f) corridorWidth = 0.8f;
            if (minRoomArea < MinSide * MinSide) minRoomArea = MinSide * MinSide;
            if (maxAspect < 1.2f) maxAspect = 1.2f;
            if (footprint.width < MinSide || footprint.height < MinSide) return plan;

            var rng = new Rng(seed);
            Split(footprint, corridorWidth, minRoomArea, maxAspect, ref rng, plan, 0);
            return plan;
        }

        /// <summary>Recursive BSP. Returns the index (into plan.Rooms) of this subtree's
        /// representative room — the anchor its parent's corridor connects to.</summary>
        private static int Split(Rect r, float corridorWidth, float minRoomArea, float maxAspect,
            ref Rng rng, InteriorPlan plan, int depth)
        {
            float area = r.width * r.height;
            float aspect = r.width > r.height
                ? r.width / Mathf.Max(r.height, 0.001f)
                : r.height / Mathf.Max(r.width, 0.001f);

            bool canSplit = area >= 2f * minRoomArea && depth < MaxDepth
                && Mathf.Max(r.width, r.height) >= 2f * MinSide;
            // Organic early stop — but never while the aspect law is violated and fixable.
            if (canSplit && aspect <= maxAspect && area < 4f * minRoomArea && rng.Next01() < 0.25f)
                canSplit = false;

            if (canSplit)
            {
                bool vertical = r.width >= r.height; // cut across the longer axis
                float length = vertical ? r.width : r.height;
                float shortSide = vertical ? r.height : r.width;
                float req = Mathf.Max(MinSide, minRoomArea / Mathf.Max(shortSide, 0.001f));

                float lo = Mathf.Max(0.35f, req / length);
                float hi = Mathf.Min(0.65f, 1f - req / length);
                if (lo > hi) { lo = req / length; hi = 1f - req / length; }
                if (lo <= hi)
                {
                    float cut = length * (lo + (hi - lo) * rng.Next01());
                    Rect a, b;
                    if (vertical)
                    {
                        a = new Rect(r.x, r.y, cut, r.height);
                        b = new Rect(r.x + cut, r.y, r.width - cut, r.height);
                    }
                    else
                    {
                        a = new Rect(r.x, r.y, r.width, cut);
                        b = new Rect(r.x, r.y + cut, r.width, r.height - cut);
                    }

                    int repA = Split(a, corridorWidth, minRoomArea, maxAspect, ref rng, plan, depth + 1);
                    int repB = Split(b, corridorWidth, minRoomArea, maxAspect, ref rng, plan, depth + 1);

                    // THE ACCESS RULE: connect the two subtrees room-center to room-center. Endpoints
                    // inside rooms ⇒ each corridor overlaps a room of each side ⇒ connected by induction.
                    CarveCorridor(plan, plan.Rooms[repA].center, plan.Rooms[repB].center, corridorWidth);
                    return repA;
                }
                // No legal cut — fall through to leaf.
            }

            // Leaf: the room is the leaf inset by the wall margin (never below MinSide).
            float insetX = Mathf.Min(WallInset, (r.width - MinSide) * 0.5f);
            float insetY = Mathf.Min(WallInset, (r.height - MinSide) * 0.5f);
            if (insetX < 0f) insetX = 0f;
            if (insetY < 0f) insetY = 0f;
            plan.Rooms.Add(new Rect(r.x + insetX, r.y + insetY, r.width - 2f * insetX, r.height - 2f * insetY));
            return plan.Rooms.Count - 1;
        }

        /// <summary>An L-shaped (or straight) corridor between two points: horizontal leg first,
        /// then vertical — both strips centered on the walking line.</summary>
        private static void CarveCorridor(InteriorPlan plan, Vector2 from, Vector2 to, float width)
        {
            float half = width * 0.5f;
            if (Mathf.Abs(from.x - to.x) > 0.001f)
            {
                float x0 = Mathf.Min(from.x, to.x) - half;
                plan.Corridors.Add(new Rect(x0, from.y - half, Mathf.Abs(to.x - from.x) + width, width));
            }
            if (Mathf.Abs(from.y - to.y) > 0.001f)
            {
                float y0 = Mathf.Min(from.y, to.y) - half;
                plan.Corridors.Add(new Rect(to.x - half, y0, width, Mathf.Abs(to.y - from.y) + width));
            }
            if (Mathf.Abs(from.x - to.x) <= 0.001f && Mathf.Abs(from.y - to.y) <= 0.001f)
            {
                // Same point (single-room subtree pair after inset) — a stub keeps the invariant.
                plan.Corridors.Add(new Rect(from.x - half, from.y - half, width, width));
            }
        }

        /// <summary>True if every room is reachable from every other through touching
        /// rooms/corridors — THE contract (used by tests and any future interior gate).</summary>
        public static bool IsFullyConnected(InteriorPlan plan)
        {
            if (plan.IsEmpty) return true;
            var rects = new List<Rect>(plan.Rooms);
            if (plan.Corridors != null) rects.AddRange(plan.Corridors);

            var parent = new int[rects.Count];
            for (int i = 0; i < parent.Length; i++) parent[i] = i;
            int Find(int i) { while (parent[i] != i) { parent[i] = parent[parent[i]]; i = parent[i]; } return i; }
            void Union(int i, int j) { int a = Find(i), b = Find(j); if (a != b) parent[a] = b; }

            for (int i = 0; i < rects.Count; i++)
                for (int j = i + 1; j < rects.Count; j++)
                    if (rects[i].Overlaps(rects[j])) Union(i, j);

            int root = Find(0);
            for (int i = 1; i < plan.Rooms.Count; i++)
                if (Find(i) != root) return false;
            return true;
        }
    }
}

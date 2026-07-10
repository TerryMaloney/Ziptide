using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>What a room is FOR — assigned from the plan's own shape (nearest-entry = foyer,
    /// largest = common), it drives which furniture catalog fills it. Rooms with jobs read as
    /// lived-in; rooms without jobs read as debug geometry (LAW 6).</summary>
    public enum RoomRole { Foyer, Common, Quarters, Workshop, Storage }

    /// <summary>One placed furnishing: a catalog kind at a footprint-space position, backed against
    /// a wall (RotationDeg ∈ {0,90,180,270}, the direction it FACES). Size is the unrotated
    /// catalog footprint; <see cref="RoomFurnishCore.AabbOf"/> gives the placed rect.</summary>
    public struct FurnishItem
    {
        public string Kind;
        public Vector2 Pos;
        public float RotationDeg;
        public Vector2 Size;
        /// <summary>Flat floor dressing (paint, a drain grate) — walkable, exempt from every
        /// blocking rule because it blocks nothing.</summary>
        public bool Flat;
    }

    /// <summary>A room's finished furnish order: its role and every item that fits.</summary>
    public struct RoomFurnishPlan
    {
        public int RoomIndex;
        public RoomRole Role;
        public List<FurnishItem> Items;
    }

    /// <summary>
    /// HARDWIRING 1.3e / BUILDING_INTERIORS Tier B — the PURE furnish planner: the step between
    /// InteriorMeshCore's walls and rooms worth entering. Deterministic per (plan, entry, seed):
    /// roles from the plan's own shape, then wall-hugging placement that provably never blocks a
    /// corridor mouth, never overlaps another item, never crowds the walk lane, and never exceeds
    /// the room's floor budget. Every room is guaranteed at least one furnishing — a bare room is
    /// a bug (the EXCELLENCE_MAP interior gate: furnished ≥N props per room). Pinned by
    /// InteriorFurnishCoreTests.
    /// </summary>
    public static class RoomFurnishCore
    {
        public const float WallClearance = 0.06f;     // air gap between an item's back and its wall
        public const float CorridorClearance = 0.30f; // keep-out around every corridor/door mouth
        public const float ItemGap = 0.25f;           // min air between items
        public const float WalkLaneRadius = 0.55f;    // free disc at room center (centerpieces exempt)
        public const float FloorBudget01 = 0.35f;     // max fraction of floor area under furniture
        public const float SlotStep = 0.45f;          // perimeter candidate spacing

        private struct CatalogEntry
        {
            public string Kind;
            public Vector2 Size;   // width (along wall) × depth (out from wall)
            public int Min, Max;
            public bool Centerpiece;
        }

        private static readonly Dictionary<RoomRole, CatalogEntry[]> Catalog =
            new Dictionary<RoomRole, CatalogEntry[]>
        {
            { RoomRole.Foyer, new[]
                {
                    new CatalogEntry { Kind = "bench",      Size = new Vector2(1.4f, 0.45f), Min = 1, Max = 1 },
                    new CatalogEntry { Kind = "coat_rack",  Size = new Vector2(0.4f, 0.4f),  Min = 0, Max = 1 },
                    new CatalogEntry { Kind = "sign_board", Size = new Vector2(0.9f, 0.15f), Min = 1, Max = 1 },
                } },
            { RoomRole.Common, new[]
                {
                    new CatalogEntry { Kind = "table",  Size = new Vector2(1.6f, 0.9f),  Min = 1, Max = 1, Centerpiece = true },
                    new CatalogEntry { Kind = "stool",  Size = new Vector2(0.4f, 0.4f),  Min = 2, Max = 4 },
                    new CatalogEntry { Kind = "shelf",  Size = new Vector2(1.5f, 0.35f), Min = 1, Max = 2 },
                    new CatalogEntry { Kind = "lamp",   Size = new Vector2(0.35f, 0.35f), Min = 1, Max = 1 },
                } },
            { RoomRole.Quarters, new[]
                {
                    new CatalogEntry { Kind = "cot",        Size = new Vector2(2.0f, 0.95f), Min = 1, Max = 1 },
                    new CatalogEntry { Kind = "footlocker", Size = new Vector2(0.9f, 0.45f), Min = 1, Max = 1 },
                    new CatalogEntry { Kind = "side_table", Size = new Vector2(0.5f, 0.5f),  Min = 0, Max = 1 },
                    new CatalogEntry { Kind = "lamp",       Size = new Vector2(0.35f, 0.35f), Min = 1, Max = 1 },
                } },
            { RoomRole.Workshop, new[]
                {
                    new CatalogEntry { Kind = "workbench", Size = new Vector2(1.8f, 0.8f),  Min = 1, Max = 1 },
                    new CatalogEntry { Kind = "tool_rack", Size = new Vector2(1.4f, 0.3f),  Min = 1, Max = 1 },
                    new CatalogEntry { Kind = "parts_bin", Size = new Vector2(0.6f, 0.6f),  Min = 1, Max = 3 },
                    new CatalogEntry { Kind = "stool",     Size = new Vector2(0.4f, 0.4f),  Min = 0, Max = 1 },
                } },
            { RoomRole.Storage, new[]
                {
                    new CatalogEntry { Kind = "crate",  Size = new Vector2(0.55f, 0.55f), Min = 2, Max = 5 },
                    new CatalogEntry { Kind = "barrel", Size = new Vector2(0.6f, 0.6f),   Min = 1, Max = 3 },
                    new CatalogEntry { Kind = "shelf",  Size = new Vector2(1.5f, 0.35f),  Min = 1, Max = 2 },
                } },
        };

        /// <summary>Role per room, from the plan's own shape: nearest room to the entry is the
        /// Foyer, the largest remaining is the Common room, the rest cycle Quarters → Workshop →
        /// Storage in order of distance from the entry. No randomness — same plan, same roles.</summary>
        public static RoomRole[] AssignRoles(InteriorPlan plan, Vector2 entry)
        {
            if (plan.IsEmpty) return new RoomRole[0];
            int n = plan.Rooms.Count;
            var roles = new RoomRole[n];
            if (n == 1) { roles[0] = RoomRole.Common; return roles; }

            int foyer = 0;
            float best = float.MaxValue;
            for (int i = 0; i < n; i++)
            {
                float d = (plan.Rooms[i].center - entry).sqrMagnitude;
                if (d < best) { best = d; foyer = i; }
            }
            roles[foyer] = RoomRole.Foyer;

            int common = -1;
            float bigArea = -1f;
            for (int i = 0; i < n; i++)
            {
                if (i == foyer) continue;
                float a = plan.Rooms[i].width * plan.Rooms[i].height;
                if (a > bigArea) { bigArea = a; common = i; }
            }
            if (common >= 0) roles[common] = RoomRole.Common;

            // The rest: cycle by distance from the entry — quarters near, storage deep.
            var rest = new List<int>();
            for (int i = 0; i < n; i++) if (i != foyer && i != common) rest.Add(i);
            rest.Sort((a, b) =>
                ((plan.Rooms[a].center - entry).sqrMagnitude)
                .CompareTo((plan.Rooms[b].center - entry).sqrMagnitude));
            var cycle = new[] { RoomRole.Quarters, RoomRole.Workshop, RoomRole.Storage };
            for (int i = 0; i < rest.Count; i++) roles[rest[i]] = cycle[i % cycle.Length];
            return roles;
        }

        /// <summary>The full furnish pass: every room gets a role and at least one furnishing.
        /// Deterministic per seed; all invariants (in-room, corridor-clear, non-overlapping,
        /// walk-lane-clear, floor budget) hold by construction and are pinned by tests.</summary>
        public static List<RoomFurnishPlan> Furnish(InteriorPlan plan, Vector2 entry, int seed)
        {
            var result = new List<RoomFurnishPlan>();
            if (plan.IsEmpty) return result;
            var roles = AssignRoles(plan, entry);
            var rng = new Rng(seed);

            for (int ri = 0; ri < plan.Rooms.Count; ri++)
            {
                var room = plan.Rooms[ri];
                var placed = new List<FurnishItem>();
                float floorUsed = 0f;
                float floorMax = room.width * room.height * FloorBudget01;

                foreach (var entryDef in Catalog[roles[ri]])
                {
                    int count = entryDef.Min + (int)(rng.Next01() * (entryDef.Max - entryDef.Min + 0.999f));
                    for (int c = 0; c < count; c++)
                    {
                        float itemArea = entryDef.Size.x * entryDef.Size.y;
                        if (floorUsed + itemArea > floorMax) break;
                        FurnishItem item;
                        bool ok = entryDef.Centerpiece
                            ? TryPlaceCenter(room, entryDef, plan, placed, out item)
                            : TryPlaceOnWall(room, entryDef, plan, placed, ref rng, out item);
                        if (!ok) continue;
                        placed.Add(item);
                        floorUsed += itemArea;
                    }
                }

                // The bare-room guarantee: a room with a job always shows at least one furnishing.
                // Try a crate on a wall; a room too corridor-cut even for that gets a flat floor
                // drain at its center — walkable paint that blocks nothing, so it needs no checks.
                if (placed.Count == 0)
                {
                    var crate = new CatalogEntry { Kind = "crate", Size = new Vector2(0.55f, 0.55f) };
                    if (TryPlaceOnWall(room, crate, plan, placed, ref rng, out var item))
                        placed.Add(item);
                    else
                        placed.Add(new FurnishItem
                        {
                            Kind = "floor_drain", Pos = room.center, RotationDeg = 0f,
                            Size = new Vector2(0.3f, 0.3f), Flat = true,
                        });
                }

                result.Add(new RoomFurnishPlan { RoomIndex = ri, Role = roles[ri], Items = placed });
            }
            return result;
        }

        /// <summary>The placed footprint rect of an item (rotation collapses to an AABB swap).</summary>
        public static Rect AabbOf(FurnishItem item)
        {
            bool swapped = Mathf.Abs(Mathf.DeltaAngle(item.RotationDeg, 90f)) < 1f
                        || Mathf.Abs(Mathf.DeltaAngle(item.RotationDeg, 270f)) < 1f;
            float w = swapped ? item.Size.y : item.Size.x;
            float h = swapped ? item.Size.x : item.Size.y;
            return new Rect(item.Pos.x - w * 0.5f, item.Pos.y - h * 0.5f, w, h);
        }

        // ── placement ────────────────────────────────────────────────────────

        private static bool TryPlaceCenter(Rect room, CatalogEntry def, InteriorPlan plan,
            List<FurnishItem> placed, out FurnishItem item)
        {
            item = new FurnishItem { Kind = def.Kind, Pos = room.center, RotationDeg = 0f, Size = def.Size };
            // A centerpiece needs walking room on every side and a corridor-clear footprint.
            if (def.Size.x + 1.2f > room.width || def.Size.y + 1.2f > room.height) return false;
            return Fits(item, room, plan, placed, ignoreWalkLane: true);
        }

        private static bool TryPlaceOnWall(Rect room, CatalogEntry def, InteriorPlan plan,
            List<FurnishItem> placed, ref Rng rng, out FurnishItem item)
        {
            item = default;
            var slots = new List<FurnishItem>();

            // Perimeter candidates: back to each of the four walls, stepping along it.
            // RotationDeg is the direction the item FACES (into the room).
            void WallRun(bool horizontalWall, bool minSide)
            {
                float wallLen = horizontalWall ? room.width : room.height;
                float along = def.Size.x;
                if (wallLen < along + 2f * WallClearance) return;
                float depthOff = def.Size.y * 0.5f + WallClearance;
                int steps = Mathf.Max(1, Mathf.FloorToInt((wallLen - along) / SlotStep));
                for (int k = 0; k <= steps; k++)
                {
                    float t = along * 0.5f + k * SlotStep;
                    if (t > wallLen - along * 0.5f) break;
                    Vector2 pos;
                    float rot;
                    if (horizontalWall)
                    {
                        float y = minSide ? room.yMin + depthOff : room.yMax - depthOff;
                        pos = new Vector2(room.xMin + t, y);
                        rot = minSide ? 0f : 180f;   // faces +Y from the south wall, −Y from the north
                    }
                    else
                    {
                        float x = minSide ? room.xMin + depthOff : room.xMax - depthOff;
                        pos = new Vector2(x, room.yMin + t);
                        rot = minSide ? 90f : 270f;  // faces +X from the west wall, −X from the east
                    }
                    slots.Add(new FurnishItem { Kind = def.Kind, Pos = pos, RotationDeg = rot, Size = def.Size });
                }
            }
            WallRun(true, true); WallRun(true, false); WallRun(false, true); WallRun(false, false);
            if (slots.Count == 0) return false;

            // Seeded start + prime stride so furniture scatters instead of clumping at a corner.
            int start = (int)(rng.Next01() * slots.Count);
            for (int k = 0; k < slots.Count; k++)
            {
                var candidate = slots[(start + k * 7) % slots.Count];
                if (Fits(candidate, room, plan, placed, ignoreWalkLane: false))
                {
                    item = candidate;
                    return true;
                }
            }
            return false;
        }

        private static bool Fits(FurnishItem item, Rect room, InteriorPlan plan,
            List<FurnishItem> placed, bool ignoreWalkLane)
        {
            Rect aabb = AabbOf(item);

            // Fully inside the room.
            if (aabb.xMin < room.xMin - 0.001f || aabb.xMax > room.xMax + 0.001f ||
                aabb.yMin < room.yMin - 0.001f || aabb.yMax > room.yMax + 0.001f) return false;

            // Never in a corridor mouth: corridors overlap the room where doors open — an item
            // there is a blocked doorway (the access rule extends to furniture).
            var swollen = new Rect(aabb.xMin - CorridorClearance, aabb.yMin - CorridorClearance,
                aabb.width + 2f * CorridorClearance, aabb.height + 2f * CorridorClearance);
            if (plan.Corridors != null)
                foreach (var c in plan.Corridors)
                    if (swollen.Overlaps(c)) return false;

            // The walk lane: a free disc at the room's center keeps every room crossable.
            // Scaled down in small rooms — a 2.2 m cabin still deserves its crate.
            if (!ignoreWalkLane)
            {
                float lane = Mathf.Min(WalkLaneRadius, 0.25f * Mathf.Min(room.width, room.height));
                Vector2 nearest = new Vector2(
                    Mathf.Clamp(room.center.x, aabb.xMin, aabb.xMax),
                    Mathf.Clamp(room.center.y, aabb.yMin, aabb.yMax));
                if ((nearest - room.center).sqrMagnitude < lane * lane) return false;
            }

            // Air between items.
            foreach (var other in placed)
            {
                Rect o = AabbOf(other);
                var grown = new Rect(o.xMin - ItemGap, o.yMin - ItemGap,
                    o.width + 2f * ItemGap, o.height + 2f * ItemGap);
                if (grown.Overlaps(aabb)) return false;
            }
            return true;
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

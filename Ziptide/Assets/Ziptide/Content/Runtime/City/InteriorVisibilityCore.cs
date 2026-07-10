using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// HARDWIRING 1.3e / BUILDING_INTERIORS Tier B — the PURE room-visibility step under portal
    /// culling: from a plan and a standing point, which rooms can possibly be seen? The honest
    /// Quest v1 (VR_TECHNIQUE_RESEARCH §2: per-frame occlusion is the CPU cost to avoid — this is
    /// rect math on a slow cadence): standing INSIDE a room, you can see that room and every room
    /// a shared corridor opens into; standing in a corridor (or outside), everything stays visible
    /// — corridors are the sight-lines. Deterministic; pinned by InteriorFurnishCoreTests.
    /// </summary>
    public static class InteriorVisibilityCore
    {
        /// <summary>Index of the room containing <paramref name="p"/>, or -1 (corridor/outside).</summary>
        public static int RoomIndexAt(InteriorPlan plan, Vector2 p)
        {
            if (plan.IsEmpty) return -1;
            for (int i = 0; i < plan.Rooms.Count; i++)
                if (plan.Rooms[i].Contains(p)) return i;
            return -1;
        }

        /// <summary>
        /// The set of room indices possibly visible from room <paramref name="currentRoom"/>:
        /// itself plus every room sharing a corridor with it (a corridor overlapping both rooms is
        /// an open sight-line). Pass -1 (in a corridor / outside) to get EVERY room — cull only
        /// when the walls actually enclose you. Result is cleared and filled, sorted ascending.
        /// </summary>
        public static void VisibleRooms(InteriorPlan plan, int currentRoom, List<int> result)
        {
            result.Clear();
            if (plan.IsEmpty) return;

            if (currentRoom < 0 || currentRoom >= plan.Rooms.Count)
            {
                for (int i = 0; i < plan.Rooms.Count; i++) result.Add(i);
                return;
            }

            result.Add(currentRoom);
            Rect here = plan.Rooms[currentRoom];
            if (plan.Corridors == null) return;

            foreach (var corridor in plan.Corridors)
            {
                if (!corridor.Overlaps(here)) continue;
                for (int i = 0; i < plan.Rooms.Count; i++)
                {
                    if (i == currentRoom || result.Contains(i)) continue;
                    if (corridor.Overlaps(plan.Rooms[i])) result.Add(i);
                }
            }
            result.Sort();
        }
    }
}

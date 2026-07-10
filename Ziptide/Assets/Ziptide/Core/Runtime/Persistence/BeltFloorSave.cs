using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>One saved lattice cell. Enum-free on purpose: Core cannot see
    /// Content's CellKind/BeltDir, so kind/dir are their int values (stable — the lattice's
    /// serialized vocabulary, pinned by tests on the Content side).</summary>
    [Serializable]
    public class BeltCellRecord
    {
        public int x, z;
        public int kind;
        public int dir;
        public string resourceId = "";
    }

    /// <summary>
    /// HARDWIRING 4.1f — a belt floor's player edits, the DYNAMIC OVERLAY (the ConquestSave idiom):
    /// the patcher-authored layout stays the canonical truth and is never stored; only what the
    /// player changed is. An empty overlay reproduces pre-persistence behavior exactly (the
    /// neutral-defaults law), and authored-layout updates from newer patchers still show through
    /// old saves because the canonical side always rebuilds fresh.
    /// </summary>
    [Serializable]
    public class BeltFloorState
    {
        public string floorId = "";
        /// <summary>Cells the player placed by hand (full records).</summary>
        public List<BeltCellRecord> placed = new List<BeltCellRecord>();
        /// <summary>Coords of AUTHORED cells the player picked back up (x/z only matter).</summary>
        public List<BeltCellRecord> removedAuthored = new List<BeltCellRecord>();
    }

    /// <summary>
    /// Pure overlay logic for <see cref="BeltFloorState"/> — fully EditMode-testable, no Unity, no
    /// clock, no randomness. The scene translator (BeltFloorRuntime) converts its specs to records,
    /// applies, and converts back.
    /// </summary>
    public static class BeltFloorSave
    {
        /// <summary>Find (or create) the overlay for one floor in a world's save state.</summary>
        public static BeltFloorState GetFloor(WorldState world, string floorId, bool createIfMissing)
        {
            if (world == null || string.IsNullOrEmpty(floorId)) return null;
            for (int i = 0; i < world.beltFloors.Count; i++)
                if (world.beltFloors[i] != null && world.beltFloors[i].floorId == floorId)
                    return world.beltFloors[i];
            if (!createIfMissing) return null;
            var f = new BeltFloorState { floorId = floorId };
            world.beltFloors.Add(f);
            return f;
        }

        /// <summary>Record a hand placement: upsert by (x,z) — re-placing a cell replaces it.</summary>
        public static void RecordPlace(BeltFloorState floor, BeltCellRecord cell)
        {
            if (floor == null || cell == null) return;
            RemoveAt(floor.placed, cell.x, cell.z);
            floor.placed.Add(cell);
        }

        /// <summary>Record a pick-up. A player-placed cell just leaves the overlay; an AUTHORED cell
        /// is remembered as removed so the canonical layout stops producing it.</summary>
        public static void RecordRemove(BeltFloorState floor, int x, int z, bool wasAuthored)
        {
            if (floor == null) return;
            RemoveAt(floor.placed, x, z);
            if (wasAuthored && FindAt(floor.removedAuthored, x, z) == null)
                floor.removedAuthored.Add(new BeltCellRecord { x = x, z = z });
        }

        /// <summary>
        /// Canonical + overlay → the effective layout, deterministically: authored cells in authored
        /// order (minus the removed ones), then player-placed cells in placement order. When a NEWER
        /// authored layout occupies a cell the player once placed on, authored truth wins and the
        /// stale placed cell is skipped.
        /// </summary>
        public static List<BeltCellRecord> Apply(IList<BeltCellRecord> authored, BeltFloorState floor)
        {
            var effective = new List<BeltCellRecord>();
            if (authored != null)
                for (int i = 0; i < authored.Count; i++)
                {
                    var a = authored[i];
                    if (a == null) continue;
                    if (floor != null && FindAt(floor.removedAuthored, a.x, a.z) != null) continue;
                    effective.Add(a);
                }
            if (floor != null)
                for (int i = 0; i < floor.placed.Count; i++)
                {
                    var p = floor.placed[i];
                    if (p == null || FindAt(effective, p.x, p.z) != null) continue;
                    effective.Add(p);
                }
            return effective;
        }

        private static BeltCellRecord FindAt(List<BeltCellRecord> list, int x, int z)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] != null && list[i].x == x && list[i].z == z) return list[i];
            return null;
        }

        private static void RemoveAt(List<BeltCellRecord> list, int x, int z)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] != null && list[i].x == x && list[i].z == z) list.RemoveAt(i);
        }
    }
}

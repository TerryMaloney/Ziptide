#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// HARDWIRING 4.1i — authors belt pads into world packs at build time, derived (spec-is-truth,
    /// regenerated every run) from the pack's OWN mines: every world with an extractor gets one
    /// "connect the mine to the depot" pad beside its first mine — port bound to that mine's hopper,
    /// sink paying the same resource, the player hand-builds the line between. No hand table, so
    /// new mine-bearing worlds pick the pad up automatically; economy-safe because the belt only
    /// moves ore the mine already produced (shared MineState, no double-pay).
    /// </summary>
    public static class BeltPadLibrary
    {
        public static void EnsurePadsFor(WorldPackDefinition pack)
        {
            if (pack == null) return;
            pack.beltFloors.Clear();
            UnityEditor.EditorUtility.SetDirty(pack); // own our mutation — don't lean on the caller's
            if (pack.mines == null || pack.mines.Count == 0) return;

            var mine = pack.mines[0];
            if (mine == null || string.IsNullOrEmpty(mine.id)) return;
            pack.beltFloors.Add(new BeltFloorSpawnDefinition
            {
                id = "belt_pad_" + mine.id,
                // Beside the rig, running east — the intake sits between the mine and the pad.
                localPosition = mine.localPosition + new Vector3(2.5f, 0.02f, -1.6f),
                width = 8, depth = 4, cellSize = 0.8f,
                feedMineId = mine.id,
                payoutResourceId = mine.resourceId,
                dispenser = true,
                conductor = true
            });
        }
    }
}
#endif

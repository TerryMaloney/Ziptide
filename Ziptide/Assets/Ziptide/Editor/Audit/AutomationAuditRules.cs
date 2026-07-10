#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// HARDWIRING 4.1g — the belt gate (a budget + an audit rule per new content type, board rule
    /// 0.6; AUTOMATION_CONVEYORS "PerfBudget cap on active belt-item count"). The runtime item count
    /// is structurally ≤ the lattice AREA (belts are single-file — pinned by BeltLatticeTests), and
    /// the player can carpet the whole grid via the dispenser, so the area IS the perf budget:
    /// each filled cell ≈ 2 renderers (tile + chevron) + at most 1 pooled puck. Caps are exact data
    /// counts (no scene measurement), so over-cap BLOCKS outright — no baselining window needed.
    /// Also gates save-identity integrity (4.1f): duplicate floorIds would share one overlay.
    /// </summary>
    public static class AutomationAuditRules
    {
        private const int FloorAreaTarget = 64, FloorAreaCap = 256;     // cells per floor grid
        private const int SceneCellsTarget = 96, SceneCellsCap = 256;   // authored cells per scene

        public static void Run(SceneAuditReport report)
        {
            var floors = Object.FindObjectsOfType<BeltFloorRuntime>(true);
            if (floors.Length == 0) return;

            int sceneCells = 0;
            var idOwner = new Dictionary<string, string>();

            foreach (var floor in floors)
            {
                if (floor == null) continue;
                string path = floor.gameObject.name;

                int area = floor.width * floor.depth;
                if (area > FloorAreaCap)
                    report.Blocker("BELT_AREA_OVER_CAP",
                        "belt floor grid " + floor.width + "x" + floor.depth + " = " + area +
                        " cells exceeds the cap " + FloorAreaCap +
                        " (the grid bounds the player-buildable item count on Quest).", path);
                else if (area > FloorAreaTarget)
                    report.Warning("BELT_AREA_OVER_TARGET",
                        "belt floor grid " + floor.width + "x" + floor.depth + " = " + area +
                        " cells over the target " + FloorAreaTarget + " (cap " + FloorAreaCap + ").",
                        path);

                var seen = new HashSet<long>();
                foreach (var c in floor.cells)
                {
                    sceneCells++;
                    if (c.x < 0 || c.x >= floor.width || c.z < 0 || c.z >= floor.depth)
                        report.Blocker("BELT_CELL_OOB",
                            "authored belt cell (" + c.x + "," + c.z + ") is outside the " +
                            floor.width + "x" + floor.depth + " grid — it would silently no-op at " +
                            "runtime (a patcher typo).", path);
                    long key = ((long)c.z << 32) | (uint)c.x;
                    if (!seen.Add(key))
                        report.Warning("BELT_CELL_DUP",
                            "belt cell (" + c.x + "," + c.z + ") is authored twice — the later " +
                            "Author* call overwrites the earlier at runtime.", path);
                }

                if (floor.cells.Count > 0 && string.IsNullOrEmpty(floor.floorId))
                    report.Warning("BELT_NO_FLOORID",
                        "belt floor has authored cells but no floorId — player edits to it will " +
                        "not persist (fine for throwaway rigs, a bug for real worlds).", path);

                if (!string.IsNullOrEmpty(floor.floorId))
                {
                    if (idOwner.TryGetValue(floor.floorId, out string other))
                        report.Blocker("BELT_FLOORID_DUP",
                            "floorId '" + floor.floorId + "' is used by both '" + other + "' and '" +
                            path + "' — two floors sharing one save overlay corrupts both.", path);
                    else
                        idOwner[floor.floorId] = path;
                }
            }

            if (sceneCells > SceneCellsCap)
                report.Blocker("BELT_CELLS_OVER_CAP",
                    "scene authors " + sceneCells + " belt cells, over the cap " + SceneCellsCap +
                    " (each cell ≈ 2 renderers + a possible riding puck).");
            else if (sceneCells > SceneCellsTarget)
                report.Warning("BELT_CELLS_OVER_TARGET",
                    "scene authors " + sceneCells + " belt cells, over the target " +
                    SceneCellsTarget + " (cap " + SceneCellsCap + ").");
        }
    }
}
#endif

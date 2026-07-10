using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// HARDWIRING 4.1i — a buildable belt pad authored as PACK DATA (the MineSpawnDefinition
    /// pattern): BeltPadSpawner materializes a BeltFloorRuntime per entry at scene start. The pad's
    /// line begins at the WEST-MIDDLE cell: when <see cref="feedMineId"/> names a pack mine, a port
    /// is authored there and an intake adapter binds that mine's SAME hopper (shared machineId —
    /// ore the belt moves is ore the hopper loses, economy-safe by construction); when
    /// <see cref="payoutResourceId"/> is set, a sink is authored at the east-middle cell. The player
    /// hand-builds the line between them — connect the mine to the depot — and 4.1f persists their
    /// work under <see cref="id"/>.
    /// </summary>
    [Serializable]
    public class BeltFloorSpawnDefinition
    {
        [Tooltip("Stable id — the floorId that keys this pad's save overlay (4.1f).")]
        public string id = "belt_pad";

        [Tooltip("Local position relative to world origin (same space as mines/spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;

        [Tooltip("Grid size in cells (clamped to the 4.1g budget at spawn).")]
        public int width = 8, depth = 4;

        [Tooltip("World meters per cell.")]
        public float cellSize = 0.8f;

        [Tooltip("Optional: a pack mine id — authors a port at the west-middle cell and binds an " +
                 "intake to that mine's hopper (shared MineState).")]
        public string feedMineId = "";

        [Tooltip("Optional: authors a sink at the east-middle cell paying this resource.")]
        public string payoutResourceId = "";

        [Tooltip("Spawn a belt-tile dispenser beside the pad.")]
        public bool dispenser = true;

        [Tooltip("Spawn a conductor post at the line's start (4.1h — ride your line).")]
        public bool conductor = true;
    }
}

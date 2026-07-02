using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A repairable machine authored as PACK DATA (GAME_PLAN M2): JobDirector spawns a RepairableMachine
    /// per entry at scene start (the Marker_&lt;id&gt; pattern). The repair is HANDS-ON — pull the access
    /// panel off, seat the replacement part (a grabbable that spawns at partLocalPosition, so fetching it
    /// is part of the job), flip the power switch — and credits RepairMachineCount job steps. This is the
    /// contract-tech fantasy the jobs were missing.
    /// </summary>
    [Serializable]
    public class MachineSpawnDefinition
    {
        [Tooltip("Machine id — matched by RepairMachineCountStepDefinition.machineId.")]
        public string machineId = "machine";

        [Tooltip("Label shown over the machine (blank = derived from machineId).")]
        public string displayName = "";

        [Tooltip("Local position relative to world origin (same space as spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;

        [Tooltip("Item id of the replacement part (labels the part pickup).")]
        public string partItemId = "part";

        [Tooltip("Where the replacement part spawns — placing it away from the machine makes the fetch " +
                 "part of the job. Zero = beside the machine.")]
        public Vector3 partLocalPosition = Vector3.zero;
    }
}

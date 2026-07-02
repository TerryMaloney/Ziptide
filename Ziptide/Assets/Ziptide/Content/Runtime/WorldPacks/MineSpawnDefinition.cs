using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A placed extractor authored as PACK DATA (GAME_PLAN M2): JobDirector spawns a MiningRigRuntime
    /// per entry. The rig binds a MineState in THIS world's WorldState (the existing idle-economy
    /// backend), so it produces while you're away (ProfileEconomy resolves on entry — ECON_RESOLVE) and
    /// you collect the stored yield by hand. The economy stops being invisible.
    /// </summary>
    [Serializable]
    public class MineSpawnDefinition
    {
        [Tooltip("Stable id — keys the MineState in the world's save data (machineId).")]
        public string id = "mine";

        [Tooltip("Resource produced (resourceId in the profile balance, e.g. 'mineral').")]
        public string resourceId = "mineral";

        [Tooltip("Units produced per second (idle + live).")]
        public double ratePerSecond = 0.05;

        [Tooltip("Storage cap before production pauses (0 = uncapped).")]
        public double storageCap = 50.0;

        [Tooltip("Local position relative to world origin (same space as spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;
    }
}

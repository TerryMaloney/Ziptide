using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A garden planter authored as PACK DATA (Quality Bar P3): JobDirector spawns a GardenPlotRuntime
    /// per entry at the HarvestGrove POI's planter pads. The runtime binds a PlotState in THIS world's
    /// WorldState, so growth is pure elapsed time (offline-friendly — ProfileEconomy already flags
    /// ready plots on entry) and harvesting credits the plant's yield by hand.
    /// </summary>
    [Serializable]
    public class GardenSpawnDefinition
    {
        [Tooltip("Stable id — keys the PlotState in the world's save data (plotId).")]
        public string id = "garden";

        [Tooltip("PlantDefinition id this planter grows (asset in Resources/Garden).")]
        public string plantId = "dew_bulb";

        [Tooltip("Local position relative to world origin (same space as spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;

        [Tooltip("GARDEN AAA 4.2d: world-hazard exposure 0..1 (radiation, static bloom). Non-zero " +
                 "planters mutation-kick every seed at plant time — hazard gardens are where giants " +
                 "come from. 0 (default) = a safe garden, genes untouched.")]
        [Range(0f, 1f)] public float hazardStrength01 = 0f;
    }
}

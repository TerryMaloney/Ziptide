using System;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// A machine BUILD SOCKET authored as PACK DATA (Quality Bar P3 — mechanical building v1):
    /// JobDirector spawns a BuildSocketRuntime per entry at the MachineSite POI's socket plinth. Pay
    /// the cost, and a REAL extractor rig is built there — its MineState persists in this world's
    /// WorldState (the same save path as authored mines), so what you build is still running, and
    /// still yours, every time you come back.
    /// </summary>
    [Serializable]
    public class BuildSocketSpawnDefinition
    {
        [Tooltip("Stable id — keys the built machine's MineState in the world's save data.")]
        public string id = "socket";

        [Tooltip("Resource the buildable extractor produces.")]
        public string resourceId = "mineral";

        [Tooltip("Build cost in credits.")]
        public double buildCost = 40.0;

        [Tooltip("Production per second once built.")]
        public double ratePerSecond = 0.05;

        [Tooltip("Storage cap before production pauses (0 = uncapped).")]
        public double storageCap = 40.0;

        [Tooltip("Local position relative to world origin (same space as spawnMarkers).")]
        public Vector3 localPosition = Vector3.zero;
    }
}

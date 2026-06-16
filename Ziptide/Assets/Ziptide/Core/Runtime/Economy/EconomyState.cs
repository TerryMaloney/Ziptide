using System;

namespace Ziptide.Core
{
    /// <summary>A placed extractor/mine's saved state. Idle production accrues into <see cref="stored"/>
    /// (capped); collecting moves it into the profile's resource balance.</summary>
    [Serializable]
    public class MineState
    {
        public string machineId;        // MachineDefinition id that placed this
        public string resourceId;       // resource produced
        public double ratePerSecond;    // resolved rate (after tools/balance)
        public double stored;           // accumulated, awaiting collection
        public double storageCap;       // 0 = uncapped
        public long lastResolvedAtUnix; // idle anchor for this mine
    }

    /// <summary>A garden plot's saved state. Growth is pure elapsed time since <see cref="plantedAtUnix"/>
    /// (no per-tick simulation needed — offline-friendly).</summary>
    [Serializable]
    public class PlotState
    {
        public string plantId;
        public long plantedAtUnix;
        public double growSeconds;
        public bool harvested;

        public bool IsReady(long nowUnix)
            => !harvested && growSeconds >= 0 && (nowUnix - plantedAtUnix) >= (long)growSeconds;

        /// <summary>Growth fraction in [0, 1].</summary>
        public double GrowthProgress(long nowUnix)
        {
            if (growSeconds <= 0) return 1.0;
            double e = nowUnix - plantedAtUnix;
            if (e < 0) e = 0;
            double p = e / growSeconds;
            return p > 1.0 ? 1.0 : p;
        }
    }
}

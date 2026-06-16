namespace Ziptide.Core
{
    /// <summary>Result of an idle/offline accrual window. All amounts are doubles (idle games grow big).</summary>
    public struct IdleAccrual
    {
        public double produced;     // gross produced over the window (before storage cap)
        public double stored;       // new stored total (after cap)
        public double added;        // actually added to storage (stored - previousStored)
        public double overflow;     // production lost to a full storage cap (produced - added)
        public long secondsApplied; // window actually used (after the maxSeconds clamp)
    }

    /// <summary>
    /// Offline/idle progression — pure math, no Unity, fully EditMode-testable. The economy advances
    /// away-worlds by computing elapsed time × rate (capped), instead of simulating live objects
    /// (Quest perf). On load / world entry: dt = now − WorldState.lastResolvedAtUnix → Accrue → show a
    /// "welcome back" summary → set lastResolvedAtUnix = now. Server time resolves this when online.
    /// </summary>
    public static class IdleEngine
    {
        /// <summary>Whole seconds between two unix timestamps, never negative.</summary>
        public static long SecondsBetween(long fromUnix, long toUnix)
            => toUnix > fromUnix ? toUnix - fromUnix : 0;

        /// <summary>
        /// Accrue idle production into storage.
        /// <paramref name="storageCap"/> &lt;= 0 means uncapped; <paramref name="maxSeconds"/> &lt;= 0
        /// means the window is unlimited (otherwise offline gains are clamped to that many seconds,
        /// e.g. 8h, so leaving the game a month doesn't break the economy).
        /// </summary>
        public static IdleAccrual Accrue(double ratePerSecond, long secondsElapsed,
                                         double currentStored, double storageCap, long maxSeconds = 0)
        {
            if (secondsElapsed < 0) secondsElapsed = 0;
            if (ratePerSecond < 0) ratePerSecond = 0;
            if (currentStored < 0) currentStored = 0;

            long window = secondsElapsed;
            if (maxSeconds > 0 && window > maxSeconds) window = maxSeconds;

            double produced = ratePerSecond * window;
            double stored = storageCap > 0
                ? System.Math.Min(storageCap, currentStored + produced)
                : currentStored + produced;

            double added = stored - currentStored;
            if (added < 0) added = 0;
            double overflow = produced - added;
            if (overflow < 0) overflow = 0;

            return new IdleAccrual
            {
                produced = produced,
                stored = stored,
                added = added,
                overflow = overflow,
                secondsApplied = window,
            };
        }
    }
}

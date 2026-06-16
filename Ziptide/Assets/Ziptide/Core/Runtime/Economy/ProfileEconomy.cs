namespace Ziptide.Core
{
    /// <summary>Summary of an offline resolution for one world — feeds a "welcome back" message.</summary>
    public struct WorldResolveResult
    {
        public int minesResolved;
        public double totalProduced;
        public int plotsReady;
    }

    /// <summary>
    /// Applies idle/offline progression to saved profile data — pure (no Unity), fully testable.
    /// "Idle is math, not objects": away-worlds advance by computing elapsed time; only the active
    /// world spawns live machines (Quest perf). Call <see cref="ResolveWorld"/> on load / world entry,
    /// then set anchors to now. Server time resolves this once online.
    /// </summary>
    public static class ProfileEconomy
    {
        /// <summary>Advance every mine in a world by elapsed time (capped by <paramref name="maxOfflineSeconds"/>)
        /// and flag grown plots. Updates each mine's stored + lastResolvedAtUnix and the world's anchor.</summary>
        public static WorldResolveResult ResolveWorld(WorldState world, long nowUnix, long maxOfflineSeconds = 0)
        {
            var result = new WorldResolveResult();
            if (world == null) return result;

            if (world.mines != null)
            {
                for (int i = 0; i < world.mines.Count; i++)
                {
                    var m = world.mines[i];
                    if (m == null) continue;
                    long elapsed = IdleEngine.SecondsBetween(m.lastResolvedAtUnix, nowUnix);
                    if (elapsed <= 0) continue;
                    var acc = IdleEngine.Accrue(m.ratePerSecond, elapsed, m.stored, m.storageCap, maxOfflineSeconds);
                    m.stored = acc.stored;
                    m.lastResolvedAtUnix = nowUnix;
                    result.totalProduced += acc.added;
                    result.minesResolved++;
                }
            }

            if (world.plots != null)
            {
                for (int i = 0; i < world.plots.Count; i++)
                {
                    var p = world.plots[i];
                    if (p != null && p.IsReady(nowUnix)) result.plotsReady++;
                }
            }

            world.lastResolvedAtUnix = nowUnix;
            return result;
        }

        /// <summary>Move a mine's stored output into the profile's resource balance. Returns amount collected.</summary>
        public static double CollectMine(PlayerProfile profile, MineState mine)
        {
            if (profile == null || mine == null || string.IsNullOrEmpty(mine.resourceId)) return 0;
            double amt = mine.stored;
            if (amt <= 0) return 0;
            mine.stored = 0;
            profile.AddResource(mine.resourceId, amt);
            return amt;
        }
    }
}

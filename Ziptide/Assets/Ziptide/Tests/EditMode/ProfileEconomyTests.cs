using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public class ProfileEconomyTests
    {
        private static WorldState WorldWithMine(double rate, long lastResolved, double stored = 0, double cap = 0)
        {
            var w = new WorldState { worldId = "d0_city", lastResolvedAtUnix = lastResolved };
            w.mines.Add(new MineState
            {
                machineId = "driller", resourceId = "scrap",
                ratePerSecond = rate, stored = stored, storageCap = cap,
                lastResolvedAtUnix = lastResolved
            });
            return w;
        }

        [Test]
        public void ResolveWorld_AdvancesMine()
        {
            var w = WorldWithMine(rate: 1.0, lastResolved: 1000);
            var r = ProfileEconomy.ResolveWorld(w, nowUnix: 1100);
            Assert.AreEqual(1, r.minesResolved);
            Assert.AreEqual(100.0, r.totalProduced, 1e-9);
            Assert.AreEqual(100.0, w.mines[0].stored, 1e-9);
            Assert.AreEqual(1100L, w.mines[0].lastResolvedAtUnix);
            Assert.AreEqual(1100L, w.lastResolvedAtUnix);
        }

        [Test]
        public void ResolveWorld_RespectsCapAndMaxOffline()
        {
            var capped = WorldWithMine(rate: 1.0, lastResolved: 0, stored: 0, cap: 50);
            var r = ProfileEconomy.ResolveWorld(capped, nowUnix: 1000);
            Assert.AreEqual(50.0, capped.mines[0].stored, 1e-9);
            Assert.AreEqual(50.0, r.totalProduced, 1e-9);

            var windowed = WorldWithMine(rate: 1.0, lastResolved: 0, stored: 0, cap: 0);
            ProfileEconomy.ResolveWorld(windowed, nowUnix: 100000, maxOfflineSeconds: 60);
            Assert.AreEqual(60.0, windowed.mines[0].stored, 1e-9);
        }

        [Test]
        public void ResolveWorld_NoElapsed_SkipsMine()
        {
            var w = WorldWithMine(rate: 5.0, lastResolved: 1000);
            var r = ProfileEconomy.ResolveWorld(w, nowUnix: 1000);
            Assert.AreEqual(0, r.minesResolved);
            Assert.AreEqual(0.0, w.mines[0].stored, 1e-9);
        }

        [Test]
        public void Plot_ReadyByElapsedTime()
        {
            var w = new WorldState { worldId = "garden" };
            w.plots.Add(new PlotState { plantId = "glowfruit", plantedAtUnix = 0, growSeconds = 600 });
            Assert.IsFalse(w.plots[0].IsReady(500));
            Assert.IsTrue(w.plots[0].IsReady(600));
            Assert.AreEqual(0.5, w.plots[0].GrowthProgress(300), 1e-9);

            var r = ProfileEconomy.ResolveWorld(w, 700);
            Assert.AreEqual(1, r.plotsReady);
        }

        [Test]
        public void EnterWorld_ResolvesAndMarksDiscovered()
        {
            var profile = new PlayerProfile();
            var w = profile.GetWorld("ToxicCity", createIfMissing: true);
            w.lastResolvedAtUnix = 1000;
            w.mines.Add(new MineState { resourceId = "scrap", ratePerSecond = 2.0, lastResolvedAtUnix = 1000 });

            var r = ProfileEconomy.EnterWorld(profile, "ToxicCity", nowUnix: 1100);

            Assert.AreEqual(1, r.minesResolved);
            Assert.AreEqual(200.0, r.totalProduced, 1e-9);          // 2/s * 100s
            Assert.IsTrue(profile.GetWorld("ToxicCity").discovered); // entry marks discovery
            Assert.AreEqual(1100L, profile.GetWorld("ToxicCity").lastResolvedAtUnix);
        }

        [Test]
        public void EnterWorld_FirstVisit_CreatesWorldNoProduction()
        {
            var profile = new PlayerProfile();
            Assert.IsNull(profile.GetWorld("NewWorld")); // not present yet

            var r = ProfileEconomy.EnterWorld(profile, "NewWorld", nowUnix: 5000);

            Assert.AreEqual(0, r.minesResolved);                  // nothing to accrue on first visit
            var ws = profile.GetWorld("NewWorld");
            Assert.IsNotNull(ws);                                 // created
            Assert.IsTrue(ws.discovered);
            Assert.AreEqual(5000L, ws.lastResolvedAtUnix);        // anchor set to entry time
        }

        [Test]
        public void EnterWorld_NullProfileOrId_IsSafe()
        {
            Assert.AreEqual(0, ProfileEconomy.EnterWorld(null, "x", 1).minesResolved);
            var profile = new PlayerProfile();
            Assert.AreEqual(0, ProfileEconomy.EnterWorld(profile, "", 1).minesResolved);
            Assert.AreEqual(0, ProfileEconomy.EnterWorld(profile, null, 1).minesResolved);
        }

        [Test]
        public void CollectMine_MovesStoredToProfile()
        {
            var profile = new PlayerProfile();
            var mine = new MineState { resourceId = "scrap", stored = 42.0 };
            Assert.AreEqual(42.0, ProfileEconomy.CollectMine(profile, mine), 1e-9);
            Assert.AreEqual(0.0, mine.stored, 1e-9);
            Assert.AreEqual(42.0, profile.GetResource("scrap"), 1e-9);
            Assert.AreEqual(0.0, ProfileEconomy.CollectMine(profile, mine), 1e-9); // nothing left
        }

        [Test]
        public void MinesAndPlots_SurviveRoundTrip()
        {
            var p = ProfileSerializer.NewProfile();
            var w = p.GetWorld("d0_city", createIfMissing: true);
            w.mines.Add(new MineState { machineId = "driller", resourceId = "scrap", ratePerSecond = 2, stored = 10, storageCap = 100, lastResolvedAtUnix = 5 });
            w.plots.Add(new PlotState { plantId = "glowfruit", plantedAtUnix = 1, growSeconds = 300 });

            var r = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(p));
            var rw = r.GetWorld("d0_city");
            Assert.IsNotNull(rw);
            Assert.AreEqual(1, rw.mines.Count);
            Assert.AreEqual("scrap", rw.mines[0].resourceId);
            Assert.AreEqual(10.0, rw.mines[0].stored, 1e-9);
            Assert.AreEqual(1, rw.plots.Count);
            Assert.AreEqual("glowfruit", rw.plots[0].plantId);
        }
    }
}

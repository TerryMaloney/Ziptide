using NUnit.Framework;
using Ziptide.Core;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// MP100 §F / A5 — pins the progression contracts: stat/streak math, payout determinism with
    /// the economy-safe cap and never-reward-losing-more-than-winning shape, the unlock ladder's
    /// exact thresholds, the daily pick's same-day-same-combo / different-days-vary guarantees,
    /// day-stamp bookkeeping, and the career's JSON round-trip (the save story ships WITH it).
    /// </summary>
    public class PvpProgressionTests
    {
        [Test]
        public void Stats_TrackKillsDownsAndStreaks()
        {
            var s = new MatchStatsCore(4);
            s.RecordKill(0, 1);
            s.RecordKill(0, 2);
            s.RecordKill(0, 3);
            s.RecordKill(2, 0); // the player goes down — streak resets
            s.RecordKill(0, 2);
            Assert.AreEqual(4, s.Kills(0));
            Assert.AreEqual(1, s.Downs(0));
            Assert.AreEqual(3, s.BestStreak(0), "best streak survives the reset");
            Assert.AreEqual(2, s.Downs(2));
            s.RecordKill(1, 1); // self-kill guard: no credit
            Assert.AreEqual(0, s.Kills(1));
            Assert.AreEqual(1, s.Downs(1), "the down still counts");
            s.RecordKill(-1, 9); // out of range never throws
        }

        [Test]
        public void Payout_IsDeterministic_CappedAndKillMonotonic()
        {
            var a = PvpProgression.ComputePayout(5, 3, won: true, PvpProgression.Veteran, false, false);
            var b = PvpProgression.ComputePayout(5, 3, won: true, PvpProgression.Veteran, false, false);
            Assert.AreEqual(a.Total, b.Total, "same inputs, same credits, every device");

            int prev = -1;
            for (int kills = 0; kills <= 30; kills++)
            {
                var p = PvpProgression.ComputePayout(kills, 0, true, PvpProgression.Nightmare, true, true);
                Assert.GreaterOrEqual(p.Total, prev, "more kills never pays less");
                Assert.LessOrEqual(p.Total, PvpProgression.PayoutCap, "economy-safe by clamp");
                prev = p.Total;
            }

            var loss = PvpProgression.ComputePayout(2, 2, won: false, PvpProgression.Regular, true, true);
            Assert.AreEqual(0, loss.WinBonus + loss.DailyBonus + loss.FirstWinBonus,
                "challenge bonuses reward the WIN — no idle daily farming");
            Assert.Greater(loss.Total, 0, "a played match always pays something");

            var win = PvpProgression.ComputePayout(2, 2, won: true, PvpProgression.Regular, false, false);
            Assert.Greater(win.Total, loss.Total, "winning beats losing, always");
        }

        [Test]
        public void Payout_UnknownDifficulty_NeverJackpots()
        {
            var unknown = PvpProgression.ComputePayout(3, 0, true, "typo_difficulty", false, false);
            var rookie = PvpProgression.ComputePayout(3, 0, true, PvpProgression.Rookie, false, false);
            Assert.AreEqual(rookie.Total, unknown.Total, "a typo'd id pays the floor, not the ceiling");
        }

        [Test]
        public void UnlockLadder_ExactThresholds()
        {
            Assert.AreEqual(0, PvpProgression.EarnedUnlocks(2, 0, 0).Count, "2 wins: nothing yet");
            var atVeteran = PvpProgression.EarnedUnlocks(3, 0, 0);
            Assert.Contains(PvpProgression.FlagVeteranUnlocked, atVeteran);
            Assert.AreEqual(1, atVeteran.Count, "veteran only — nightmare needs VETERAN wins");

            var atNightmare = PvpProgression.EarnedUnlocks(0, 3, 0);
            Assert.Contains(PvpProgression.FlagVeteranUnlocked, atNightmare, "harder wins ladder down");
            Assert.Contains(PvpProgression.FlagNightmareUnlocked, atNightmare);

            var atMutators = PvpProgression.EarnedUnlocks(0, 2, 1);
            Assert.Contains(PvpProgression.FlagMutatorsUnlocked, atMutators);
            Assert.Contains(PvpProgression.FlagNightmareUnlocked, atMutators);
        }

        [Test]
        public void Daily_SameDaySameCombo_DifferentDaysVary()
        {
            PvpProgression.PickDaily(20278, 5, 5, out int a1, out int m1, out string d1);
            PvpProgression.PickDaily(20278, 5, 5, out int a2, out int m2, out string d2);
            Assert.AreEqual(a1, a2); Assert.AreEqual(m1, m2); Assert.AreEqual(d1, d2);
            Assert.That(a1, Is.InRange(0, 4));
            Assert.That(m1, Is.InRange(0, 4));
            Assert.AreNotEqual(PvpProgression.Rookie, d1, "a daily is a challenge, never a stroll");

            var combos = new System.Collections.Generic.HashSet<string>();
            for (int day = 20278; day < 20278 + 14; day++)
            {
                PvpProgression.PickDaily(day, 5, 5, out int a, out int m, out string d);
                combos.Add(a + "|" + m + "|" + d);
            }
            Assert.Greater(combos.Count, 5, "two weeks of dailies must not repeat one combo");
            PvpProgression.PickDaily(1, 0, 0, out int za, out int zm, out _);
            Assert.AreEqual(0, za); Assert.AreEqual(0, zm); // empty catalogs never divide by zero
        }

        [Test]
        public void DayNumber_IsStableBookkeeping()
        {
            Assert.AreEqual(PvpProgression.UtcDayNumber(86400L * 100 + 5),
                            PvpProgression.UtcDayNumber(86400L * 100 + 86000),
                "any second of the same UTC day is the same day");
            Assert.AreEqual(100, PvpProgression.UtcDayNumber(86400L * 100));
            Assert.Less(PvpProgression.UtcDayNumber(86400L * 99), PvpProgression.UtcDayNumber(86400L * 100));
        }

        [Test]
        public void Career_RoundTripsThroughProfileJson_AndOldSavesDefaultToZero()
        {
            var p = ProfileSerializer.NewProfile();
            p.pvpCareer.matches = 7;
            p.pvpCareer.winsVeteran = 3;
            p.pvpCareer.bestStreakEver = 6;
            p.pvpCareer.lastFirstWinDay = 20278;
            var back = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(p));
            Assert.AreEqual(7, back.pvpCareer.matches);
            Assert.AreEqual(3, back.pvpCareer.winsVeteran);
            Assert.AreEqual(6, back.pvpCareer.bestStreakEver);
            Assert.AreEqual(20278, back.pvpCareer.lastFirstWinDay);

            var old = ProfileSerializer.Deserialize("{\"schemaVersion\":2,\"playerId\":\"p\"}");
            Assert.IsNotNull(old.pvpCareer, "pre-A5 saves get a zero career, never null");
            Assert.AreEqual(0, old.pvpCareer.matches);
        }
    }
}

using NUnit.Framework;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// PvP match-rule contract tests (Phase 1 backbone). Pure, headless — guards the best-of-10 win
    /// condition, score tracking, and phase gating. No scene, no netcode.
    /// </summary>
    public class PvpMatchTests
    {
        [Test]
        public void NewMatch_StartsInLobby_ZeroScores()
        {
            var m = new PvpMatch();
            Assert.AreEqual(PvpPhase.Lobby, m.Phase);
            Assert.AreEqual(0, m.Score(0));
            Assert.AreEqual(0, m.Score(1));
            Assert.IsFalse(m.IsOver);
            Assert.AreEqual(-1, m.WinnerIndex);
        }

        [Test]
        public void RegisterKill_BeforeBegin_IsIgnored()
        {
            var m = new PvpMatch();
            Assert.IsFalse(m.RegisterKill(0));
            Assert.AreEqual(0, m.Score(0));
        }

        [Test]
        public void Begin_ActivatesAndCountsKills()
        {
            var m = new PvpMatch();
            m.Begin();
            Assert.AreEqual(PvpPhase.Active, m.Phase);
            Assert.IsFalse(m.RegisterKill(0));
            Assert.AreEqual(1, m.Score(0));
            Assert.AreEqual(0, m.Score(1));
        }

        [Test]
        public void TenthKill_EndsMatch_AndSetsWinner()
        {
            var m = new PvpMatch(); // KillsToWin = 10
            m.Begin();
            for (int i = 0; i < 9; i++)
                Assert.IsFalse(m.RegisterKill(1), "match should not end before the 10th kill");
            Assert.IsTrue(m.RegisterKill(1), "the 10th kill ends the match");
            Assert.IsTrue(m.IsOver);
            Assert.AreEqual(PvpPhase.Ended, m.Phase);
            Assert.AreEqual(1, m.WinnerIndex);
            Assert.AreEqual(10, m.Score(1));
        }

        [Test]
        public void KillsAfterEnd_AreIgnored()
        {
            var m = new PvpMatch(3);
            m.Begin();
            m.RegisterKill(0);
            m.RegisterKill(0);
            Assert.IsTrue(m.RegisterKill(0)); // 3rd kill ends it
            Assert.IsFalse(m.RegisterKill(1), "no scoring after the match ends");
            Assert.AreEqual(0, m.Score(1));
            Assert.AreEqual(0, m.WinnerIndex);
        }

        [Test]
        public void CustomKillsToWin_IsRespected()
        {
            var m = new PvpMatch(2);
            Assert.AreEqual(2, m.KillsToWin);
            m.Begin();
            Assert.IsFalse(m.RegisterKill(0));
            Assert.IsTrue(m.RegisterKill(0));
        }
    }
}

using NUnit.Framework;
using Ziptide.Gameplay;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    public class PvpRoundLogicTests
    {
        [Test]
        public void KillerOf_IsTheOtherPlayer()
        {
            Assert.AreEqual(1, PvpRoundLogic.KillerOf(0));
            Assert.AreEqual(0, PvpRoundLogic.KillerOf(1));
        }

        [Test]
        public void ResolveDeath_CreditsTheKiller()
        {
            var m = new PvpMatch();
            m.Begin();
            var (killer, ended) = PvpRoundLogic.ResolveDeath(m, killedIndex: 0); // player died -> bot scores
            Assert.AreEqual(1, killer);
            Assert.IsFalse(ended);
            Assert.AreEqual(1, m.Score(1));
            Assert.AreEqual(0, m.Score(0));
        }

        [Test]
        public void ResolveDeath_EndsMatchAtKillsToWin()
        {
            var m = new PvpMatch(killsToWin: 2);
            m.Begin();
            PvpRoundLogic.ResolveDeath(m, 0);            // bot -> 1
            var (killer, ended) = PvpRoundLogic.ResolveDeath(m, 0); // bot -> 2, ends
            Assert.AreEqual(1, killer);
            Assert.IsTrue(ended);
            Assert.IsTrue(m.IsOver);
            Assert.AreEqual(1, m.WinnerIndex);
        }

        [Test]
        public void ResolveDeath_BeforeBegin_DoesNotScore()
        {
            var m = new PvpMatch(); // still in Lobby
            var (_, ended) = PvpRoundLogic.ResolveDeath(m, 1);
            Assert.IsFalse(ended);
            Assert.AreEqual(0, m.Score(0));
            Assert.AreEqual(0, m.Score(1));
        }
    }
}

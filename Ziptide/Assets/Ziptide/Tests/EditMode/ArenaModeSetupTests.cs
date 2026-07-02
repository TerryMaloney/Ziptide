using NUnit.Framework;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Modes;

namespace Ziptide.Tests.EditMode
{
    /// <summary>A3-scene: the pure match-setup rules the mode director translates from.</summary>
    public class ArenaModeSetupTests
    {
        [Test]
        public void CombatantCount_IsPlayerPlusBots_Clamped()
        {
            Assert.AreEqual(2, ArenaModeSetup.CombatantCount(1));
            Assert.AreEqual(3, ArenaModeSetup.CombatantCount(2));
            Assert.AreEqual(4, ArenaModeSetup.CombatantCount(3));
            Assert.AreEqual(2, ArenaModeSetup.CombatantCount(0));   // at least one opponent
            Assert.AreEqual(4, ArenaModeSetup.CombatantCount(99));  // PvpMatch caps at 4
        }

        [Test]
        public void Deathmatch_UsesTheClassicKillLimit()
        {
            Assert.AreEqual(PvpRules.KillsToWin, ArenaModeSetup.KillsToWinFor(PvpModeKind.Deathmatch));
        }

        [Test]
        public void ObjectiveModes_ParkTheKillLimitOutOfReach()
        {
            foreach (var mode in new[] { PvpModeKind.GunGame, PvpModeKind.KingOfTheHill,
                                         PvpModeKind.FragmentRush, PvpModeKind.Horde })
                Assert.AreEqual(ArenaModeSetup.ObjectiveModeKillLimit, ArenaModeSetup.KillsToWinFor(mode), mode.ToString());
        }

        [Test]
        public void ObjectiveKillLimit_NeverEndsARealisticRound()
        {
            var m = new PvpMatch(4, ArenaModeSetup.ObjectiveModeKillLimit);
            m.Begin();
            for (int i = 0; i < 500; i++) m.RegisterKill(0);
            Assert.IsFalse(m.IsOver);
        }

        [Test]
        public void Horde_OwnsItsOwnBotPool()
        {
            Assert.IsFalse(ArenaModeSetup.UsesLobbyBots(PvpModeKind.Horde));
            Assert.IsTrue(ArenaModeSetup.UsesLobbyBots(PvpModeKind.Deathmatch));
            Assert.IsTrue(ArenaModeSetup.UsesLobbyBots(PvpModeKind.KingOfTheHill));
        }
    }
}

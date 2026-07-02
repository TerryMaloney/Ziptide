using NUnit.Framework;
using System.Collections.Generic;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Modes;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The A3 mode engines' contracts (PVP_ARENA_AAA §A3) + the N-player/team PvpMatch generalization.
    /// Pure, clock-injected, headless.
    /// </summary>
    public class PvpModesTests
    {
        // ── PvpMatch: N players + teams (the 1v1 default stays untouched — its own tests cover it) ──
        [Test]
        public void Match_FourPlayerFFA_HighestScorerWins()
        {
            var m = new PvpMatch(playerCount: 4, killsToWin: 3);
            m.Begin();
            m.RegisterKill(2); m.RegisterKill(2);
            Assert.IsFalse(m.IsOver);
            Assert.IsTrue(m.RegisterKill(2), "third kill ends it");
            Assert.AreEqual(2, m.WinnerIndex);
        }

        [Test]
        public void Match_Teams_SumScores_AndWinTogether()
        {
            var m = new PvpMatch(playerCount: 4, killsToWin: 4, teams: new[] { 0, 0, 1, 1 });
            m.Begin();
            m.RegisterKill(0); m.RegisterKill(0);
            m.RegisterKill(1);                       // teammate contributes
            Assert.IsFalse(m.IsOver);
            Assert.IsTrue(m.RegisterKill(1), "team total 4 ends it");
            Assert.AreEqual(0, m.WinnerTeam);
            Assert.AreEqual(4, m.TeamScore(0));
        }

        [Test]
        public void Match_EndByRule_ClosesFromModes()
        {
            var m = new PvpMatch(playerCount: 2, killsToWin: 10);
            m.Begin();
            m.RegisterKill(1);
            m.EndByRule();
            Assert.IsTrue(m.IsOver);
            Assert.AreEqual(1, m.WinnerIndex, "highest scorer at the whistle");
        }

        // ── Gun Game ─────────────────────────────────────────────────────────
        [Test]
        public void GunGame_KillsClimbTheLadder_FinishWins()
        {
            var g = new GunGameState(playerCount: 2, ladder: new[] { "taser_dart_gun", "pistol", "gravity_gun" });
            Assert.AreEqual("taser_dart_gun", g.CurrentWeapon(0));
            Assert.IsFalse(g.OnKill(0));
            Assert.AreEqual("pistol", g.CurrentWeapon(0));
            Assert.IsFalse(g.OnKill(0));
            Assert.AreEqual("gravity_gun", g.CurrentWeapon(0));
            Assert.IsTrue(g.OnKill(0), "cleared the last rung → win");
            Assert.AreEqual(0, g.Rung(1), "opponent unaffected");
        }

        [Test]
        public void GunGame_DefaultLadder_RunsAllSixWeapons()
        {
            var g = new GunGameState(playerCount: 2);
            Assert.AreEqual(6, g.Ladder.Count);
            Assert.AreEqual("taser_dart_gun", g.Ladder[0]);
            Assert.AreEqual("prism_beam", g.Ladder[5]);
        }

        // ── King of the Hill ─────────────────────────────────────────────────
        [Test]
        public void Koth_SoleKingAccrues_ContestedDoesNot()
        {
            var k = new KothState(playerCount: 2, zoneCount: 2, rotateEverySeconds: 100f, targetHoldSeconds: 5f);
            k.Tick(0f, null);                                    // clock priming tick
            k.Tick(2f, new List<int> { 0 });                     // 2s as sole king
            Assert.AreEqual(2f, k.Hold(0), 0.001f);
            k.Tick(4f, new List<int> { 0, 1 });                  // contested — nobody accrues
            Assert.AreEqual(2f, k.Hold(0), 0.001f);
            int winner = k.Tick(8f, new List<int> { 0 });        // +4s → 6s ≥ 5 target
            Assert.AreEqual(0, winner);
        }

        [Test]
        public void Koth_ZoneRotates_OnTheTimer()
        {
            var k = new KothState(playerCount: 2, zoneCount: 3, rotateEverySeconds: 10f, targetHoldSeconds: 99f);
            k.Tick(0f, null);
            Assert.AreEqual(0, k.ActiveZone);
            k.Tick(11f, null);
            Assert.AreEqual(1, k.ActiveZone);
            k.Tick(22f, null);
            Assert.AreEqual(2, k.ActiveZone);
            k.Tick(33f, null);
            Assert.AreEqual(0, k.ActiveZone, "wraps");
        }

        // ── Fragment Rush ────────────────────────────────────────────────────
        [Test]
        public void FragmentRush_CarryAndBank_ToTheTarget()
        {
            var f = new FragmentRushState(playerCount: 2, targetBanks: 2);
            Assert.IsTrue(f.PickUp(0));
            Assert.AreEqual(0, f.CarrierIndex);
            Assert.IsFalse(f.PickUp(1), "only one carrier");
            Assert.IsFalse(f.Bank(1), "you can't bank what you don't carry");
            Assert.IsFalse(f.Bank(0), "first bank doesn't win yet");
            Assert.IsTrue(f.FragmentAtHome, "fragment resets to mid after a bank");
            f.PickUp(0);
            Assert.IsTrue(f.Bank(0), "second bank wins");
        }

        [Test]
        public void FragmentRush_DropResets()
        {
            var f = new FragmentRushState(playerCount: 2);
            f.PickUp(1);
            f.Drop();                                            // carrier died
            Assert.AreEqual(-1, f.CarrierIndex);
            Assert.IsTrue(f.FragmentAtHome);
            Assert.IsTrue(f.PickUp(0), "anyone can grab it again");
        }

        // ── Horde ────────────────────────────────────────────────────────────
        [Test]
        public void Horde_WavesEscalate_AndClear()
        {
            var h = new HordeState();
            int w1 = h.NextWave();
            Assert.AreEqual(1, w1);
            int wave1Size = HordeState.WaveBots(1) + HordeState.WaveCreatures(1).Count;
            Assert.AreEqual(wave1Size, h.Alive);

            for (int i = 0; i < wave1Size - 1; i++)
                Assert.IsFalse(h.RegisterDown());
            Assert.IsTrue(h.RegisterDown(), "last enemy down → wave clear");
            Assert.AreEqual(wave1Size + HordeState.WaveClearBonus, h.Score, "kills + clear bonus");

            h.NextWave();
            Assert.Greater(h.Alive, wave1Size - 1, "wave 2 is bigger or equal");
        }

        [Test]
        public void Horde_CompositionIsDeterministic_AndCapped()
        {
            var a = HordeState.WaveCreatures(5);
            var b = HordeState.WaveCreatures(5);
            CollectionAssert.AreEqual(a, b, "same wave = same monsters");
            Assert.LessOrEqual(HordeState.WaveCreatures(50).Count, 6, "creature cap (Quest perf)");
            Assert.LessOrEqual(HordeState.WaveBots(50), 3, "bot cap");
        }
    }
}

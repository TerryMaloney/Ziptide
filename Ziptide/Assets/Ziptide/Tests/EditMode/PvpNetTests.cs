using NUnit.Framework;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// PvP network-contract tests (Phase 3 prep). Guards the transport seam via the loopback double:
    /// Send* must deliver the exact payload through On*. Pure/headless — no Photon, no scene.
    /// </summary>
    public class PvpNetTests
    {
        [Test]
        public void Loopback_IsHost_LocalPlayerZero()
        {
            var t = new LoopbackPvpTransport();
            Assert.IsTrue(t.IsHost);
            Assert.AreEqual(PvpNetRole.Host, t.Role);
            Assert.AreEqual(0, t.LocalPlayerId);
        }

        [Test]
        public void Loopback_Fire_DeliversPayload()
        {
            var t = new LoopbackPvpTransport();
            FireMsg got = default;
            int calls = 0;
            t.OnFire += m => { got = m; calls++; };

            t.SendFire(new FireMsg { shooterId = 1, weapon = PvpWeapon.Taser });

            Assert.AreEqual(1, calls);
            Assert.AreEqual(1, got.shooterId);
            Assert.AreEqual(PvpWeapon.Taser, got.weapon);
        }

        [Test]
        public void Loopback_Hit_DeliversKillFlagAndAmount()
        {
            var t = new LoopbackPvpTransport();
            HitMsg got = default;
            t.OnHit += m => got = m;

            t.SendHit(new HitMsg { attackerId = 0, targetId = 1, weapon = PvpWeapon.Gravity, amount = 1, killed = true });

            Assert.AreEqual(1, got.targetId);
            Assert.AreEqual(PvpWeapon.Gravity, got.weapon);
            Assert.AreEqual(1, got.amount);
            Assert.IsTrue(got.killed);
        }

        [Test]
        public void Loopback_Score_DeliversPhaseAndWinner()
        {
            var t = new LoopbackPvpTransport();
            ScoreMsg got = default;
            t.OnScore += m => got = m;

            t.SendScore(new ScoreMsg { score0 = 10, score1 = 7, phase = PvpPhase.Ended, winnerIndex = 0 });

            Assert.AreEqual(PvpPhase.Ended, got.phase);
            Assert.AreEqual(0, got.winnerIndex);
            Assert.AreEqual(10, got.score0);
        }

        [Test]
        public void Loopback_Wall_DeliversPanelState()
        {
            var t = new LoopbackPvpTransport();
            WallMsg got = default;
            t.OnWall += m => got = m;

            t.SendWall(new WallMsg { panelId = 42, state = 2 });

            Assert.AreEqual(42, got.panelId);
            Assert.AreEqual(2, got.state);
        }

        [Test]
        public void Loopback_NoSubscriber_DoesNotThrow()
        {
            var t = new LoopbackPvpTransport();
            Assert.DoesNotThrow(() => t.SendPose(new PlayerPoseMsg { playerId = 1, heldWeapon = -1 }));
        }
    }
}

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

        [Test]
        public void Loopback_Pose_RoundTripsHeadAndHands()
        {
            var t = new LoopbackPvpTransport();
            PlayerPoseMsg got = default;
            t.OnPose += m => got = m;

            t.SendPose(new PlayerPoseMsg
            {
                playerId = 3,
                headPos = new UnityEngine.Vector3(1, 2, 3),
                rHandPos = new UnityEngine.Vector3(4, 5, 6),
                heldWeapon = 2,
            });

            Assert.AreEqual(3, got.playerId);
            Assert.AreEqual(new UnityEngine.Vector3(1, 2, 3), got.headPos);
            Assert.AreEqual(new UnityEngine.Vector3(4, 5, 6), got.rHandPos);
            Assert.AreEqual(2, got.heldWeapon);
        }

        [Test]
        public void Hub_StartOnline_NoNetcode_StaysOnLoopback_AndReportsUnavailable()
        {
            // No adapter installed (headless test = no ZIPTIDE_PHOTON): StartOnline must not throw,
            // must return false, and must leave gameplay on the loopback transport.
            PvpNetHub.OnlineStarter = null;
            bool ok = PvpNetHub.StartOnline("ZIP-777");

            Assert.IsFalse(ok, "with no netcode compiled in, going online must fail gracefully");
            Assert.IsFalse(PvpNetHub.IsOnline);
            Assert.AreEqual("ZIP-777", PvpNetHub.RoomCode, "the requested room code is still recorded");
            Assert.IsInstanceOf<LoopbackPvpTransport>(PvpNetHub.Active);
        }

        [Test]
        public void Hub_StartOnline_InvokesInstalledStarterWithRoomCode()
        {
            string seen = null;
            PvpNetHub.OnlineStarter = code => { seen = code; return true; };
            try
            {
                bool ok = PvpNetHub.StartOnline("ZIP-042");
                Assert.IsTrue(ok);
                Assert.AreEqual("ZIP-042", seen, "the starter receives the room code");
            }
            finally { PvpNetHub.OnlineStarter = null; }
        }

        [Test]
        public void Hub_IsOnline_FollowsTransportSwap()
        {
            PvpNetHub.SetTransport(null);
            Assert.IsFalse(PvpNetHub.IsOnline, "loopback default is not online");

            PvpNetHub.SetTransport(new LoopbackPvpTransport());
            Assert.IsFalse(PvpNetHub.IsOnline, "an explicit loopback is still not online");

            PvpNetHub.SetTransport(new FakeOnlineTransport());
            Assert.IsTrue(PvpNetHub.IsOnline, "a non-loopback transport reads as online");

            PvpNetHub.SetTransport(null); // leave the static hub clean for other tests
        }

        /// <summary>A minimal non-loopback transport so IsOnline flips true without Photon.</summary>
        private sealed class FakeOnlineTransport : IPvpTransport
        {
            public PvpNetRole Role => PvpNetRole.Client;
            public bool IsHost => false;
            public int LocalPlayerId => 1;
            public event System.Action<PlayerPoseMsg> OnPose;
            public event System.Action<FireMsg> OnFire;
            public event System.Action<HitMsg> OnHit;
            public event System.Action<ScoreMsg> OnScore;
            public event System.Action<WallMsg> OnWall;
            public void SendPose(PlayerPoseMsg m) => OnPose?.Invoke(m);
            public void SendFire(FireMsg m) => OnFire?.Invoke(m);
            public void SendHit(HitMsg m) => OnHit?.Invoke(m);
            public void SendScore(ScoreMsg m) => OnScore?.Invoke(m);
            public void SendWall(WallMsg m) => OnWall?.Invoke(m);
        }
    }
}

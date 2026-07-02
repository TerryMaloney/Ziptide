using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Bruiser charge FSM contract (GAME_PLAN M3): the telegraph always precedes the charge (fair in
    /// VR), a wall slam skips to the vulnerable Recover window, and a stun interrupts anything.
    /// Pure, headless.
    /// </summary>
    public class ChargeStateTests
    {
        [Test]
        public void Windup_AlwaysPrecedesTheCharge()
        {
            var s = new ChargeState { TriggerRange = 7f, WindupSeconds = 1f, ChargeSeconds = 1f };

            s.Tick(0.1f, 5f, active: true);
            Assert.AreEqual(ChargePhase.Windup, s.Phase, "entering range starts the TELEGRAPH, never a charge");
            Assert.IsFalse(s.ChargeStarted);

            s.Tick(0.5f, 5f, true);
            Assert.AreEqual(ChargePhase.Windup, s.Phase);
            Assert.Greater(s.WindupProgress, 0.4f);

            s.Tick(0.6f, 5f, true);
            Assert.AreEqual(ChargePhase.Charging, s.Phase);
            Assert.IsTrue(s.ChargeStarted, "the launch tick fires exactly once");

            s.Tick(0.1f, 5f, true);
            Assert.IsFalse(s.ChargeStarted, "not re-fired mid-charge");
        }

        [Test]
        public void Charge_EndsInRecover_ThenReturnsToIdle()
        {
            var s = new ChargeState { TriggerRange = 7f, WindupSeconds = 0.1f, ChargeSeconds = 0.5f, RecoverSeconds = 0.5f };
            s.Tick(0.2f, 3f, true);              // Idle → Windup (the transition consumes a tick)
            s.Tick(0.2f, 3f, true);              // windup complete → charging
            Assert.AreEqual(ChargePhase.Charging, s.Phase);
            s.Tick(0.6f, 3f, true);              // charge time elapses
            Assert.AreEqual(ChargePhase.Recover, s.Phase);
            s.Tick(0.6f, 30f, true);             // recover elapses, player far
            Assert.AreEqual(ChargePhase.Idle, s.Phase);
        }

        [Test]
        public void WallSlam_SkipsToRecover()
        {
            var s = new ChargeState { TriggerRange = 7f, WindupSeconds = 0.1f, ChargeSeconds = 5f };
            s.Tick(0.2f, 3f, true);
            s.Tick(0.2f, 3f, true);
            Assert.AreEqual(ChargePhase.Charging, s.Phase);
            s.HitWall();
            Assert.AreEqual(ChargePhase.Recover, s.Phase, "slamming a wall ends the charge early");
        }

        [Test]
        public void Stun_InterruptsAnyPhase()
        {
            var s = new ChargeState { TriggerRange = 7f, WindupSeconds = 0.1f, ChargeSeconds = 5f };
            s.Tick(0.2f, 3f, true);
            s.Tick(0.2f, 3f, true);
            Assert.AreEqual(ChargePhase.Charging, s.Phase);
            s.Tick(0.1f, 3f, active: false);     // tased mid-charge
            Assert.AreEqual(ChargePhase.Idle, s.Phase, "the taser counter stops a charge cold");
        }

        [Test]
        public void OutOfRange_StaysIdle()
        {
            var s = new ChargeState { TriggerRange = 7f };
            s.Tick(0.5f, 20f, true);
            Assert.AreEqual(ChargePhase.Idle, s.Phase);
        }
    }
}

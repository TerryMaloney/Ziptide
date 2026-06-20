using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class DroneCombatStateTests
    {
        private static DroneCombatState Make()
        {
            return new DroneCombatState
            {
                DetectRange = 10f,
                LoseRange = 14f,
                TelegraphSeconds = 0.9f,
                BoltCooldown = 2.5f,
            };
        }

        [Test]
        public void Patrol_To_Engage_OnInRangeWithLoS()
        {
            var s = Make();
            Assert.AreEqual(DroneCombatPhase.Patrol, s.Phase);
            s.Tick(0.1f, distance: 5f, hasLoS: true, isActive: true);
            Assert.AreEqual(DroneCombatPhase.Engage, s.Phase);
        }

        [Test]
        public void StaysPatrol_WhenOutOfRange_OrNoLoS()
        {
            var s = Make();
            s.Tick(0.1f, 20f, true, true);
            Assert.AreEqual(DroneCombatPhase.Patrol, s.Phase);
            s.Tick(0.1f, 5f, false, true);
            Assert.AreEqual(DroneCombatPhase.Patrol, s.Phase);
        }

        [Test]
        public void TelegraphPrecedesFire_AndFireIsOneTick()
        {
            var s = Make();
            s.Tick(0.1f, 5f, true, true); // -> Engage
            s.Tick(0.1f, 5f, true, true); // -> Telegraph (timer 0.9)
            Assert.AreEqual(DroneCombatPhase.Telegraph, s.Phase);
            Assert.IsFalse(s.FireRequested);
            s.Tick(0.5f, 5f, true, true); // telegraph 0.4
            Assert.IsFalse(s.FireRequested);
            s.Tick(0.5f, 5f, true, true); // telegraph elapses -> fire
            Assert.IsTrue(s.FireRequested);
            Assert.AreEqual(DroneCombatPhase.Cooldown, s.Phase);
            s.Tick(0.1f, 5f, true, true); // fire is a single tick
            Assert.IsFalse(s.FireRequested);
        }

        [Test]
        public void CooldownEnforced_BeforeNextTelegraph()
        {
            var s = Make();
            s.Tick(0.1f, 5f, true, true); // Engage
            s.Tick(0.1f, 5f, true, true); // Telegraph
            s.Tick(0.9f, 5f, true, true); // fire -> Cooldown(2.5)
            Assert.AreEqual(DroneCombatPhase.Cooldown, s.Phase);
            s.Tick(1.0f, 5f, true, true);
            Assert.AreEqual(DroneCombatPhase.Cooldown, s.Phase); // still cooling
            s.Tick(2.0f, 5f, true, true); // cooldown elapsed, re-telegraph
            Assert.AreEqual(DroneCombatPhase.Telegraph, s.Phase);
        }

        [Test]
        public void NoFire_WhenInactive_ResetsToPatrol()
        {
            var s = Make();
            s.Tick(0.1f, 5f, true, true); // Engage
            s.Tick(0.1f, 5f, true, true); // Telegraph
            s.Tick(0.9f, 5f, true, isActive: false); // dead/shocked mid-telegraph
            Assert.IsFalse(s.FireRequested);
            Assert.AreEqual(DroneCombatPhase.Patrol, s.Phase);
        }

        [Test]
        public void LosesTarget_DuringTelegraph_ReturnsToPatrol()
        {
            var s = Make();
            s.Tick(0.1f, 5f, true, true); // Engage
            s.Tick(0.1f, 5f, true, true); // Telegraph
            s.Tick(0.1f, 30f, true, true); // target fled beyond loseRange
            Assert.AreEqual(DroneCombatPhase.Patrol, s.Phase);
            Assert.IsFalse(s.FireRequested);
        }
    }
}

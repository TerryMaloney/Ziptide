using NUnit.Framework;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Contract tests for the campaign player's armor-only defense (COMBAT_HEALTH_PLAN, decided
    /// 2026-07-07). Pure/headless from a passed-in clock, like PvpCombatTests. Guards the rule:
    /// a hit while armored just DRAINS the meter (overkill only breaks it), and a hit at 0 armor kills.
    /// </summary>
    public class ArmorMeterTests
    {
        [Test]
        public void StartsFull()
        {
            var a = new ArmorMeter(maxCharge: 4);
            Assert.AreEqual(4, a.MaxCharge);
            Assert.AreEqual(4, a.Charge);
            Assert.IsFalse(a.IsBroken);
        }

        [Test]
        public void HitWhileArmored_Absorbs_NeverKills()
        {
            var a = new ArmorMeter(maxCharge: 4);
            Assert.AreEqual(ArmorHit.Absorbed, a.ApplyDamage(2, 0.0));
            Assert.AreEqual(2, a.Charge);
            Assert.IsFalse(a.IsBroken);
        }

        [Test]
        public void OverkillHit_Breaks_ButDoesNotKill()
        {
            var a = new ArmorMeter(maxCharge: 4);
            a.ApplyDamage(2, 0.0);                                   // -> 2
            Assert.AreEqual(ArmorHit.Broke, a.ApplyDamage(5, 0.0));  // overkill: empties, does not kill
            Assert.AreEqual(0, a.Charge);
            Assert.IsTrue(a.IsBroken);
        }

        [Test]
        public void HitAtZeroArmor_Kills()
        {
            var a = new ArmorMeter(maxCharge: 4);
            Assert.AreEqual(ArmorHit.Broke, a.ApplyDamage(4, 0.0)); // breaks
            Assert.AreEqual(ArmorHit.Killed, a.ApplyDamage(1, 0.0), "a hit at 0 armor is death");
        }

        [Test]
        public void LethalOnBreak_KillsOnTheEmptyingHit()
        {
            var a = new ArmorMeter(maxCharge: 4, lethalOnBreak: true);
            Assert.AreEqual(ArmorHit.Killed, a.ApplyDamage(4, 0.0), "with the flag, emptying the meter kills");
        }

        [Test]
        public void ZeroDamage_IsNotARealHit()
        {
            var a = new ArmorMeter(maxCharge: 4);
            Assert.AreEqual(ArmorHit.Absorbed, a.ApplyDamage(0, 0.0));
            Assert.AreEqual(4, a.Charge);
        }

        [Test]
        public void Regen_HoldsForTheDelay_ThenRefillsGradually()
        {
            var a = new ArmorMeter(maxCharge: 4, regenPerSec: 1.0, regenDelaySec: 3.0);
            a.ApplyDamage(3, 0.0);                 // -> 1, regen blocked until t=3
            a.Tick(2.0, 1.0);                      // still inside the delay
            Assert.AreEqual(1, a.Charge, "no regen before the out-of-combat delay elapses");
            a.Tick(3.0, 1.0);
            Assert.AreEqual(2, a.Charge, "one charge back at the delay");
            a.Tick(4.0, 1.0);
            a.Tick(5.0, 1.0);
            Assert.AreEqual(4, a.Charge);
            a.Tick(6.0, 1.0);
            Assert.AreEqual(4, a.Charge, "never overfills past max");
        }

        [Test]
        public void AnyHit_RestartsTheRegenDelay()
        {
            var a = new ArmorMeter(maxCharge: 4, regenPerSec: 1.0, regenDelaySec: 3.0);
            a.ApplyDamage(2, 0.0);   // -> 2, ready at t=3
            a.Tick(3.0, 1.0);        // -> 3
            a.ApplyDamage(1, 3.0);   // -> 2, delay restarts, ready at t=6
            a.Tick(5.0, 1.0);
            Assert.AreEqual(2, a.Charge, "the fresh hit pushed regen back out");
            a.Tick(6.0, 1.0);
            Assert.AreEqual(3, a.Charge);
        }

        [Test]
        public void Reset_RefillsForRespawn()
        {
            var a = new ArmorMeter(maxCharge: 4);
            a.ApplyDamage(4, 0.0);
            Assert.IsTrue(a.IsBroken);
            a.Reset();
            Assert.AreEqual(4, a.Charge);
            Assert.IsFalse(a.IsBroken);
        }

        [Test]
        public void Fraction_TracksCharge()
        {
            var a = new ArmorMeter(maxCharge: 4);
            Assert.AreEqual(1.0, a.Fraction, 1e-9);
            a.ApplyDamage(2, 0.0);
            Assert.AreEqual(0.5, a.Fraction, 1e-9);
        }
    }
}

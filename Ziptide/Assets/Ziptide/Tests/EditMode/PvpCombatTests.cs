using NUnit.Framework;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// PvP combat-rule contract tests (Phase 1 backbone). Pure, headless — guards the health/damage
    /// model (3 taser hits or 6 gravity hits = kill), respawn, and the "fire 2 then recharge" weapon
    /// charge model (deterministic from a clock). No scene, no netcode.
    /// </summary>
    public class PvpCombatTests
    {
        [Test]
        public void Combatant_StartsAtMaxHealth_Alive()
        {
            var c = new PvpCombatant();
            Assert.AreEqual(PvpRules.MaxHealth, c.MaxHealth);
            Assert.AreEqual(PvpRules.MaxHealth, c.Health);
            Assert.IsTrue(c.IsAlive);
        }

        [Test]
        public void ThreeTaserHits_Kill()
        {
            var c = new PvpCombatant();
            Assert.IsFalse(c.ApplyHit(PvpWeapon.Taser));
            Assert.IsFalse(c.ApplyHit(PvpWeapon.Taser));
            Assert.IsTrue(c.ApplyHit(PvpWeapon.Taser), "3rd taser hit kills");
            Assert.IsFalse(c.IsAlive);
            Assert.AreEqual(0, c.Health);
            Assert.AreEqual(1, c.Deaths);
        }

        [Test]
        public void SixGravityHits_Kill_HalfTaserDamage()
        {
            Assert.AreEqual(PvpRules.TaserDamage, 2 * PvpRules.GravityDamage, "gravity = half taser");
            var c = new PvpCombatant();
            for (int i = 0; i < 5; i++)
                Assert.IsFalse(c.ApplyHit(PvpWeapon.Gravity));
            Assert.IsTrue(c.ApplyHit(PvpWeapon.Gravity), "6th gravity hit kills");
            Assert.IsFalse(c.IsAlive);
        }

        [Test]
        public void DamageOnDeadCombatant_IsIgnored()
        {
            var c = new PvpCombatant();
            c.ApplyDamage(PvpRules.MaxHealth);
            Assert.IsFalse(c.IsAlive);
            Assert.IsFalse(c.ApplyHit(PvpWeapon.Taser));
            Assert.AreEqual(1, c.Deaths, "no extra death from hitting a corpse");
        }

        [Test]
        public void Respawn_RestoresFullHealth()
        {
            var c = new PvpCombatant();
            c.ApplyDamage(PvpRules.MaxHealth);
            c.Respawn();
            Assert.IsTrue(c.IsAlive);
            Assert.AreEqual(c.MaxHealth, c.Health);
        }

        [Test]
        public void WeaponCharge_FiresTwiceThenMustRecharge()
        {
            var w = new WeaponCharge(2, 1.5);
            Assert.IsTrue(w.TryFire(0.0));
            Assert.IsTrue(w.TryFire(0.0));
            Assert.IsFalse(w.TryFire(0.0), "empty after 2 shots");
            Assert.IsFalse(w.CanFire(1.0), "still recharging before the window elapses");
        }

        [Test]
        public void WeaponCharge_RefillsAfterRechargeWindow()
        {
            var w = new WeaponCharge(2, 1.5);
            w.TryFire(0.0);
            w.TryFire(0.0); // empties at t=0 -> ready at t=1.5
            Assert.IsFalse(w.CanFire(1.4));
            Assert.IsTrue(w.CanFire(1.5), "refilled exactly at the recharge window");
            Assert.AreEqual(2, w.Charges);
            Assert.IsTrue(w.TryFire(1.6));
            Assert.AreEqual(1, w.Charges);
        }

        [Test]
        public void WeaponCharge_RechargeProgress_RisesToOne()
        {
            var w = new WeaponCharge(1, 2.0);
            w.TryFire(0.0); // empties, ready at 2.0
            Assert.AreEqual(0.0, w.RechargeProgress(0.0), 1e-6);
            Assert.AreEqual(0.5, w.RechargeProgress(1.0), 1e-6);
            Assert.AreEqual(1.0, w.RechargeProgress(2.0), 1e-6);
        }
    }
}

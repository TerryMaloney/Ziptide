using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Space combat 3.1 laws (SPACE_COMBAT.md, non-lethal disable + salvage): armor floors at zero
    /// and DISABLES, a disabled ship never recharges (the wreck stays a wreck — the salvage loop
    /// depends on it), live armor recharges only after the quiet delay, fire respects the cooldown,
    /// the aim cone is bounded by angle AND range, and salvage demands proximity. Pure, headless.
    /// </summary>
    public class SpaceCombatCoreTests
    {
        private static ShipArmorState Fresh(float armor = 6f)
            => new ShipArmorState { Armor = armor, LastHitTime = float.NegativeInfinity };

        [Test]
        public void Armor_FloorsAtZero_AndDisables()
        {
            var s = SpaceCombatCore.Hit(Fresh(3f), 2f, now: 10f);
            Assert.IsFalse(s.Disabled);
            Assert.AreEqual(1f, s.Armor);

            s = SpaceCombatCore.Hit(s, 99f, now: 11f);
            Assert.IsTrue(s.Disabled, "crossing zero disables");
            Assert.AreEqual(0f, s.Armor, "armor never goes negative");

            var again = SpaceCombatCore.Hit(s, 5f, now: 12f);
            Assert.AreEqual(s.Armor, again.Armor, "bolts pass a powered-down hull — no overkill state");
        }

        [Test]
        public void DisabledShips_NeverRecharge_SalvageStaysSalvage()
        {
            var s = SpaceCombatCore.Hit(Fresh(1f), 5f, now: 10f);
            Assert.IsTrue(s.Disabled);
            for (float t = 10f; t < 120f; t += 0.5f)
                s = SpaceCombatCore.Recharge(s, maxArmor: 6f, now: t, dt: 0.5f);
            Assert.AreEqual(0f, s.Armor, "a wreck must STAY a wreck — the salvage loop depends on it");
            Assert.IsTrue(s.Disabled);
        }

        [Test]
        public void LiveArmor_RechargesToMax_OnlyAfterTheQuietDelay()
        {
            var s = SpaceCombatCore.Hit(Fresh(6f), 4f, now: 100f); // armor 2, hit at t=100
            s = SpaceCombatCore.Recharge(s, 6f, now: 100f + SpaceCombatCore.RechargeDelay - 0.1f, dt: 1f);
            Assert.AreEqual(2f, s.Armor, "no recharge inside the quiet delay");

            for (float t = 100f + SpaceCombatCore.RechargeDelay; t < 130f; t += 0.25f)
                s = SpaceCombatCore.Recharge(s, 6f, now: t, dt: 0.25f);
            Assert.AreEqual(6f, s.Armor, 0.01f, "after the delay armor climbs back to max, capped");
        }

        [Test]
        public void Fire_RespectsTheCooldown()
        {
            Assert.IsTrue(SpaceCombatCore.CanFire(float.NegativeInfinity, now: 0f), "first shot is free");
            Assert.IsFalse(SpaceCombatCore.CanFire(10f, now: 10f + SpaceCombatCore.FireCooldown - 0.05f));
            Assert.IsTrue(SpaceCombatCore.CanFire(10f, now: 10f + SpaceCombatCore.FireCooldown));
        }

        [Test]
        public void AimCone_IsBoundedByAngleAndRange()
        {
            Vector3 pos = Vector3.zero, fwd = Vector3.forward;
            Assert.IsTrue(SpaceCombatCore.InAimCone(pos, fwd, new Vector3(0f, 0f, 100f)), "dead ahead hits");
            Assert.IsTrue(SpaceCombatCore.InAimCone(pos, fwd, new Vector3(5f, 0f, 100f)),
                "inside the comfort cone (~2.9 degrees) hits — fly to aim, don't pixel-hunt");
            Assert.IsFalse(SpaceCombatCore.InAimCone(pos, fwd, new Vector3(30f, 0f, 100f)),
                "well outside the cone misses");
            Assert.IsFalse(SpaceCombatCore.InAimCone(pos, fwd, new Vector3(0f, 0f, SpaceCombatCore.BoltRange + 10f)),
                "beyond range misses even dead ahead");
            Assert.IsFalse(SpaceCombatCore.InAimCone(pos, fwd, pos), "a degenerate zero-distance target is a miss");
        }

        [Test]
        public void Salvage_DemandsProximity()
        {
            Assert.IsTrue(SpaceCombatCore.InSalvageRange(Vector3.zero, new Vector3(0f, 0f, SpaceCombatCore.SalvageRange - 1f)));
            Assert.IsFalse(SpaceCombatCore.InSalvageRange(Vector3.zero, new Vector3(0f, 0f, SpaceCombatCore.SalvageRange + 1f)));
        }
    }
}

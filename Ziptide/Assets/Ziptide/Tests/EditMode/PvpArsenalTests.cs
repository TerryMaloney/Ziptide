using NUnit.Framework;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Modes;

namespace Ziptide.Tests.EditMode
{
    /// <summary>A4: the arsenal's damage table + the balance shapes that keep each weapon honest.</summary>
    public class PvpArsenalTests
    {
        [Test]
        public void DamageTable_MatchesTheDesign()
        {
            Assert.AreEqual(PvpRules.TaserDamage, PvpCombatant.DamageFor(PvpWeapon.Taser));
            Assert.AreEqual(PvpRules.GravityDamage, PvpCombatant.DamageFor(PvpWeapon.Gravity));
            Assert.AreEqual(PvpRules.StaticNetDamage, PvpCombatant.DamageFor(PvpWeapon.StaticNet));
            Assert.AreEqual(PvpRules.SonicThumperDamage, PvpCombatant.DamageFor(PvpWeapon.SonicThumper));
            Assert.AreEqual(PvpRules.PrismBeamDamage, PvpCombatant.DamageFor(PvpWeapon.PrismBeam));
        }

        [Test]
        public void EveryWeapon_KillsFromFull_InFiniteHits()
        {
            foreach (PvpWeapon w in System.Enum.GetValues(typeof(PvpWeapon)))
            {
                var c = new PvpCombatant();
                int hits = 0;
                while (c.IsAlive && hits < 100) { c.ApplyHit(w); hits++; }
                Assert.IsFalse(c.IsAlive, w + " must be lethal eventually");
                Assert.LessOrEqual(hits, PvpRules.MaxHealth, w + " can't need more hits than health points");
            }
        }

        [Test]
        public void PrismHitsHardest_ButNeverOneShots()
        {
            Assert.Greater(PvpRules.PrismBeamDamage, PvpRules.TaserDamage, "the charge-up must be worth it");
            Assert.Less(PvpRules.PrismBeamDamage, PvpRules.MaxHealth, "no one-shot kills — non-lethal canon");
        }

        [Test]
        public void StaticNet_IsUtilityFirst()
        {
            Assert.LessOrEqual(PvpRules.StaticNetDamage, PvpRules.GravityDamage, "the slow is the payload");
            Assert.Less(PvpRules.StaticNetSlowFactor, 1.0, "the zone must actually slow");
            Assert.Greater(PvpRules.StaticNetSlowFactor, 0.0);
        }

        [Test]
        public void GunGameDefaultLadder_UsesOnlyKnownWeaponIds()
        {
            // Every rung must be an itemId the factory can resolve once ArenaWeaponAuthor runs.
            var known = new[] { "taser_dart_gun", "pistol", "gravity_gun", "static_net", "sonic_thumper", "prism_beam" };
            foreach (var rung in GunGameState.DefaultLadder)
                Assert.Contains(rung, known, "unknown ladder weapon: " + rung);
            Assert.AreEqual(6, GunGameState.DefaultLadder.Length);
        }
    }
}

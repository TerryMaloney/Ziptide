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
            Assert.AreEqual(PvpRules.BreakerBladeDamage, PvpCombatant.DamageFor(PvpWeapon.BreakerBlade));
            Assert.AreEqual(PvpRules.TidePikeDamage, PvpCombatant.DamageFor(PvpWeapon.TidePike));
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
            var known = new[] { "taser_dart_gun", "pistol", "gravity_gun", "static_net", "sonic_thumper", "prism_beam",
                                "breaker_blade", "tide_pike" };
            foreach (var rung in GunGameState.DefaultLadder)
                Assert.Contains(rung, known, "unknown ladder weapon: " + rung);
            Assert.AreEqual(7, GunGameState.DefaultLadder.Length);
        }

        // ── The melee pair (MP100 wave 1) — balance shapes that keep contact weapons honest ──────
        [Test]
        public void Melee_BladeIsFastAndLight_PikeIsSlowAndHeavy()
        {
            // The blade wins through sustained contact (low per-hit, short debounce); the pike wins
            // through committed thrusts (taser-tier per-hit, long recovery). Neither may one-shot.
            Assert.Less(PvpRules.BreakerBladeDamage, PvpRules.TidePikeDamage, "blade must hit lighter than the pike");
            Assert.Less(PvpRules.BladeContactDebounce, PvpRules.PikeThrustDebounce, "blade must swing faster than the pike recovers");
            Assert.Less(PvpRules.BladeReach, PvpRules.PikeReach, "the pike's whole identity is reach");
            Assert.Less(PvpRules.TidePikeDamage, PvpRules.MaxHealth, "no one-shot kills — non-lethal canon");
        }

        [Test]
        public void Melee_DpsCeiling_NeverBeatsThePrismPerHit()
        {
            // Melee's advantage is uptime inside reach, not raw per-hit numbers — the prism stays the
            // heavy-hit king so its long telegraph keeps being worth the risk.
            Assert.LessOrEqual(PvpRules.BreakerBladeDamage, PvpRules.PrismBeamDamage);
            Assert.LessOrEqual(PvpRules.TidePikeDamage, PvpRules.PrismBeamDamage);
        }

        [Test]
        public void Melee_SwingThresholds_DemandARealSwing()
        {
            // Above the thumper's 1.6 m/s: contact melee must not trigger from idle hand drift, or
            // walking past an opponent becomes a hit — the threshold IS the input.
            Assert.Greater(PvpRules.MeleeSwingSpeed, 1.6, "a blade swing must be more deliberate than a thumper shake");
            Assert.Greater(PvpRules.PikeThrustSpeed, PvpRules.MeleeSwingSpeed, "a thrust is a bigger commitment than a swing");
        }
    }
}

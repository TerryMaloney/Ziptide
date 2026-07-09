using NUnit.Framework;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Pins the whole campaign-player combat rule (COMBAT_HEALTH_PLAN Phase B core) so the device shell
    /// (`PlayerArmor`) can stay a decision-free translator. Deterministic clock, pure/headless.
    /// </summary>
    public class PlayerCombatStateTests
    {
        private static PlayerCombatState Fresh(int armor = 4, double protection = 2.0)
            => new PlayerCombatState(new ArmorMeter(maxCharge: armor, regenPerSec: 1.0, regenDelaySec: 3.0),
                                     spawnProtectionSec: protection);

        [Test]
        public void StartsAlive_Unprotected_FullArmor()
        {
            var p = Fresh();
            Assert.IsTrue(p.IsAlive);
            Assert.IsFalse(p.IsProtected(0.0));
            Assert.AreEqual(4, p.Armor.Charge);
        }

        [Test]
        public void ArmoredHit_Absorbs_ThenBreaks_ThenKills()
        {
            var p = Fresh();
            Assert.AreEqual(PlayerHitOutcome.Absorbed, p.ApplyDamage(2, 0.0));
            Assert.AreEqual(PlayerHitOutcome.Broke, p.ApplyDamage(5, 0.0), "overkill only breaks");
            Assert.IsTrue(p.IsBroken);
            Assert.IsTrue(p.IsAlive, "the emptying hit never kills");
            Assert.AreEqual(PlayerHitOutcome.Killed, p.ApplyDamage(1, 0.0), "a hit at 0 armor kills");
            Assert.IsFalse(p.IsAlive);
            Assert.AreEqual(1, p.Deaths);
        }

        [Test]
        public void HitsWhileDead_AreIgnored_NoExtraDeaths()
        {
            var p = Fresh(armor: 1);
            p.ApplyDamage(1, 0.0); // breaks
            p.ApplyDamage(1, 0.0); // kills
            Assert.AreEqual(PlayerHitOutcome.Ignored, p.ApplyDamage(9, 0.0));
            Assert.AreEqual(1, p.Deaths, "no corpse-kicking deaths");
        }

        [Test]
        public void SpawnProtection_IgnoresHits_ThenExpires()
        {
            var p = Fresh(protection: 2.0);
            p.GrantProtection(0.0);
            Assert.AreEqual(PlayerHitOutcome.Ignored, p.ApplyDamage(3, 1.9));
            Assert.AreEqual(4, p.Armor.Charge, "a protected hit doesn't even scratch armor");
            Assert.AreEqual(PlayerHitOutcome.Absorbed, p.ApplyDamage(3, 2.0), "window over — hits land");
        }

        [Test]
        public void Respawn_RestoresEverything_AndArmsProtection()
        {
            var p = Fresh(protection: 2.0);
            p.ApplyDamage(4, 0.0);
            p.ApplyDamage(1, 0.0);
            Assert.IsFalse(p.IsAlive);

            p.Respawn(10.0);
            Assert.IsTrue(p.IsAlive);
            Assert.AreEqual(4, p.Armor.Charge, "full armor on respawn");
            Assert.IsTrue(p.IsProtected(11.9));
            Assert.IsFalse(p.IsProtected(12.0));
        }

        [Test]
        public void DeadPlayers_DoNotRegen()
        {
            var p = Fresh(armor: 2);
            p.ApplyDamage(2, 0.0); // breaks at t=0
            p.ApplyDamage(1, 0.5); // kills
            p.Tick(10.0, 5.0);     // way past the regen delay
            Assert.AreEqual(0, p.Armor.Charge, "respawn is the only way back");
        }

        [Test]
        public void AliveBrokenPlayer_RegensBackFromTheEdge()
        {
            var p = Fresh(armor: 2);
            p.ApplyDamage(2, 0.0);           // broken at t=0; regen ready at t=3
            Assert.IsTrue(p.IsBroken);
            p.Tick(3.0, 1.0);
            Assert.AreEqual(1, p.Armor.Charge, "escaped the one-hit-from-death state");
            Assert.IsFalse(p.IsBroken);
        }

        [Test]
        public void Defaults_ComeFromPvpRules()
        {
            var p = new PlayerCombatState();
            Assert.AreEqual(PvpRules.PlayerArmor, p.Armor.MaxCharge);
        }
    }
}

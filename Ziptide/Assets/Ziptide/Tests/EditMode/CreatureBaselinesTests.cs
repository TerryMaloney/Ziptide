using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Multiplayer;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Pins the unified combat scale (COMBAT_HEALTH_PLAN A3): creature health lives on the same integer
    /// scale as PvP weapon damage, so time-to-kill is a handful of hits, not fifteen. Pure/headless.
    /// </summary>
    public class CreatureBaselinesTests
    {
        private static readonly string[] Known =
            { "swarm_bug", "witness_mite", "light_grazer", "tether_swarm", "tendril", "husk_molter", "warden" };

        [Test]
        public void EveryKnownCreature_IsOnTheUnifiedIntegerScale()
        {
            foreach (var id in Known)
            {
                float hp = CreatureBaselines.HealthFor(id);
                Assert.Greater(hp, 0f, id + " must have positive health");
                // Taser deals 2; on-scale means even the tankiest body dies in a sane number of hits.
                int taserHits = Mathf.CeilToInt(hp / PvpRules.TaserDamage);
                Assert.LessOrEqual(taserHits, 12, id + " should not be an absurd bullet-sponge on the unified scale");
                Assert.GreaterOrEqual(taserHits, 1, id);
            }
        }

        [Test]
        public void Baselines_MatchTheDesignedTimeToKill()
        {
            // A fragile swarmer dies in ~2 taser hits; the warden is a ~10-hit mini-boss.
            Assert.AreEqual(4f, CreatureBaselines.HealthFor("swarm_bug"));
            Assert.AreEqual(20f, CreatureBaselines.HealthFor("warden"));
            Assert.Greater(CreatureBaselines.HealthFor("warden"), CreatureBaselines.HealthFor("tendril"),
                "the warden is tankier than a wall-crawler");
        }

        [Test]
        public void UnknownCreature_FallsBackToTheDefault()
        {
            Assert.AreEqual(CreatureBaselines.DefaultHealth, CreatureBaselines.HealthFor("not_a_real_id"));
        }

        [Test]
        public void ScaleVersion_IsCurrent()
        {
            Assert.AreEqual(1, CreatureBaselines.StatScaleVersion,
                "bump this + the migration together when the scale changes");
        }
    }
}

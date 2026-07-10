using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// GARDEN AAA 4.2b catalog laws, audited on the pure spec table (never the AssetDatabase):
    /// the board's ≥20-species bar, unique ids, every yield a REAL economy resource (the typo
    /// catcher — a misspelled resourceId would grant nothing silently), sane timing overrides,
    /// and the retention ladder — enough one-visit learners AND enough overnight prizes.
    /// </summary>
    public class GardenCatalogTests
    {
        // The authored economy resources (Resources/Economy) — extend when EconomyAuthor grows.
        private static readonly HashSet<string> KnownResources = new HashSet<string>
        {
            "credits", "spore", "mineral", "crystal", "salt", "carapace", "prism",
            "resonator", "data_chip", "memory_shard", "fuel_cell", "jump_core",
        };

        [Test]
        public void Catalog_MeetsTheTwentySpeciesBar_WithUniqueIds()
        {
            var specs = GardenAuthor.PlantSpecs();
            Assert.GreaterOrEqual(specs.Length, 20, "GARDEN_AAA.md: ≥20 plants across biomes");
            var ids = new HashSet<string>();
            foreach (var s in specs)
                Assert.IsTrue(ids.Add(s.Id), "duplicate plant id: " + s.Id);
        }

        [Test]
        public void EveryPlant_YieldsRealResources_AndPositiveAmounts()
        {
            foreach (var s in GardenAuthor.PlantSpecs())
            {
                Assert.IsTrue(s.Yield != null && s.Yield.Length > 0, s.Id + " yields nothing");
                foreach (var (resourceId, amount) in s.Yield)
                {
                    Assert.IsTrue(KnownResources.Contains(resourceId),
                        s.Id + " yields unknown resource '" + resourceId + "' — a typo would grant nothing silently");
                    Assert.Greater(amount, 0.0, s.Id + " has a non-positive yield amount");
                }
            }
        }

        [Test]
        public void TimingOverrides_AreSane()
        {
            foreach (var s in GardenAuthor.PlantSpecs())
            {
                Assert.Greater(s.GrowSeconds, 0.0, s.Id + " must actually grow");
                if (s.FreshOverride > 0 && s.OverripeOverride > 0)
                    Assert.Less(s.FreshOverride, s.OverripeOverride,
                        s.Id + ": the fresh window must close before overripe opens");
                if (s.OverripeOverride > 0)
                    Assert.GreaterOrEqual(s.OverripeOverride, 300.0,
                        s.Id + ": overripe under 5 minutes would punish normal play (all-ages law)");
            }
        }

        [Test]
        public void RetentionLadder_HasLearnersAndOvernightPrizes()
        {
            int learners = 0, sessions = 0, prizes = 0;
            foreach (var s in GardenAuthor.PlantSpecs())
            {
                if (s.GrowSeconds <= 300) learners++;
                else if (s.GrowSeconds <= 1800) sessions++;
                else prizes++;
            }
            Assert.GreaterOrEqual(learners, 4, "need one-visit learner crops (≤5 min)");
            Assert.GreaterOrEqual(sessions, 4, "need session-length crops (5–30 min)");
            Assert.GreaterOrEqual(prizes, 4, "need idle/overnight prizes (>30 min) — the come-back-tomorrow hook");
        }

        [Test]
        public void EveryPlant_AnswersToTheWateringCan()
        {
            foreach (var s in GardenAuthor.PlantSpecs())
            {
                Assert.IsTrue(s.TendTools != null && System.Array.IndexOf(s.TendTools, "watering_can") >= 0,
                    s.Id + " must be tendable by the watering can — the hands-on loop covers the whole catalog");
            }
        }

        [Test]
        public void Biomes_AreActuallyCovered()
        {
            var biomes = new HashSet<string>();
            foreach (var s in GardenAuthor.PlantSpecs())
                if (!string.IsNullOrEmpty(s.Biome)) biomes.Add(s.Biome);
            Assert.GreaterOrEqual(biomes.Count, 4,
                "plants should spread across ≥4 named biomes so rare-seed hunts span worlds");
        }
    }
}

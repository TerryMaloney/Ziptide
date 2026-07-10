using NUnit.Framework;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// GARDEN AAA genetics laws: everything is deterministic per seed (same cross replays
    /// identically — the save law), every result lands inside the stat clamps, a 200-generation
    /// greedy breeding line can NEVER escape the economy ceiling, hazard kicks are bounded,
    /// rarity is monotonic, and BASELINE GENES REPRODUCE PRE-GENETICS BEHAVIOR EXACTLY — old
    /// saves and classic Plant calls yield precisely what they did before this system existed.
    /// </summary>
    public class PlantGeneticsTests
    {
        [Test]
        public void RollAndCross_AreDeterministicPerSeed()
        {
            Assert.AreEqual(PlantGenetics.RollWild(42), PlantGenetics.RollWild(42));
            Assert.AreNotEqual(PlantGenetics.RollWild(42), PlantGenetics.RollWild(43));

            var a = PlantGenetics.RollWild(7);
            var b = PlantGenetics.RollWild(11);
            Assert.AreEqual(PlantGenetics.Cross(a, b, 99), PlantGenetics.Cross(a, b, 99),
                "the same cross must replay identically — saves and async depend on it");
        }

        [Test]
        public void Cross_IncrementsGeneration_AndStaysClamped()
        {
            var a = PlantGenetics.RollWild(1);
            var b = PlantGenetics.RollWild(2);
            for (int seed = 0; seed < 200; seed++)
            {
                var c = PlantGenetics.Cross(a, b, seed);
                Assert.AreEqual(1, c.generation);
                Assert.That(c.speed, Is.InRange(PlantGenetics.SpeedMin, PlantGenetics.SpeedMax));
                Assert.That(c.yield, Is.InRange(PlantGenetics.YieldMin, PlantGenetics.YieldMax));
                Assert.That(c.size, Is.InRange(0f, 1f));
            }
        }

        [Test]
        public void TwoHundredGenerations_OfGreedyBreeding_NeverEscapeTheEconomyCeiling()
        {
            // Breed the best child from 8 rolls, every generation, for 200 generations — the most
            // abusive line a player could run. The ceiling must hold: max harvest factor is
            // YieldMax × GiantYieldBonus, forever.
            var best = PlantGenetics.RollWild(5);
            var mate = PlantGenetics.RollWild(6);
            for (int gen = 0; gen < 200; gen++)
            {
                var top = PlantGenetics.Cross(best, mate, gen * 8);
                for (int s = 1; s < 8; s++)
                {
                    var child = PlantGenetics.Cross(best, mate, gen * 8 + s);
                    if (child.yield + child.size > top.yield + top.size) top = child;
                }
                mate = best;
                best = top;
            }
            Assert.AreEqual(200, best.generation);
            Assert.LessOrEqual(best.yield, PlantGenetics.YieldMax);
            Assert.LessOrEqual(PlantGenetics.HarvestFactor(best),
                PlantGenetics.YieldMax * PlantGenetics.GiantYieldBonus + 0.0001,
                "no breeding line may ever out-earn the ceiling");
        }

        [Test]
        public void HazardKick_IsBounded_Deterministic_AndZeroStrengthIsANoOp()
        {
            var g = PlantGenetics.RollWild(3);
            Assert.AreEqual(g, PlantGenetics.HazardKick(g, 0f, 55), "no hazard, no kick");
            var kicked = PlantGenetics.HazardKick(g, 1f, 55);
            Assert.AreEqual(kicked, PlantGenetics.HazardKick(g, 1f, 55));
            Assert.That(kicked.speed, Is.InRange(PlantGenetics.SpeedMin, PlantGenetics.SpeedMax));
            Assert.That(kicked.yield, Is.InRange(PlantGenetics.YieldMin, PlantGenetics.YieldMax));
            Assert.That(kicked.size, Is.InRange(0f, 1f));
        }

        [Test]
        public void Rarity_IsMonotonic_AndBaselineIsCommon()
        {
            Assert.AreEqual(PlantRarity.Common, PlantGenetics.Rarity(PlantGenes.Baseline));
            var legendary = new PlantGenes { speed = 2.2f, yield = 2.5f, size = 1f };
            Assert.AreEqual(PlantRarity.Legendary, PlantGenetics.Rarity(legendary));
            var mid = new PlantGenes { speed = 1.2f, yield = 1.4f, size = 0.5f };
            Assert.That((int)PlantGenetics.Rarity(mid),
                Is.GreaterThan((int)PlantRarity.Common).And.LessThanOrEqualTo((int)PlantRarity.Rare));
        }

        [Test]
        public void GiantCrops_TripleTheGeneFactor()
        {
            var giant = new PlantGenes { speed = 1f, yield = 2f, size = PlantGenetics.GiantThreshold };
            Assert.AreEqual(2.0 * PlantGenetics.GiantYieldBonus, PlantGenetics.HarvestFactor(giant), 1e-6);
            var normal = new PlantGenes { speed = 1f, yield = 2f, size = PlantGenetics.GiantThreshold - 0.01f };
            Assert.AreEqual(2.0, PlantGenetics.HarvestFactor(normal), 1e-6);
        }

        [Test]
        public void BaselineGenes_ReproducePreGeneticsBehaviorExactly()
        {
            Assert.AreEqual(1.0, PlantGenetics.HarvestFactor(PlantGenes.Baseline), 1e-9,
                "an old save's plot must harvest exactly what it did before genetics existed");
            Assert.IsFalse(PlantGenetics.IsGiant(PlantGenes.Baseline));

            // And the classic Plant() call plants at the plant's own grow time, untouched.
            var world = new WorldState { worldId = "w" };
            var plant = UnityEngine.ScriptableObject.CreateInstance<PlantDefinition>();
            try
            {
                plant.id = "test_plant";
                plant.growSeconds = 600;
                var plot = GardenService.Plant(world, plant, nowUnix: 1000);
                Assert.AreEqual(600.0, plot.growSeconds, 1e-9);
                Assert.AreEqual(PlantGenes.Baseline, plot.genes);
            }
            finally { UnityEngine.Object.DestroyImmediate(plant); }
        }

        [Test]
        public void GeneSpeed_ShortensGrowTime_AtPlantTime()
        {
            var world = new WorldState { worldId = "w" };
            var plant = UnityEngine.ScriptableObject.CreateInstance<PlantDefinition>();
            try
            {
                plant.id = "fast_plant";
                plant.growSeconds = 600;
                var fast = new PlantGenes { speed = 2f, yield = 1f, size = 0.3f };
                var plot = GardenService.Plant(world, plant, fast, nowUnix: 1000);
                Assert.AreEqual(300.0, plot.growSeconds, 1e-9, "speed 2 halves the grow time");
                Assert.IsTrue(plot.IsReady(1000 + 300));
                Assert.IsFalse(plot.IsReady(1000 + 299));
            }
            finally { UnityEngine.Object.DestroyImmediate(plant); }
        }

        [Test]
        public void CrossPlots_DemandsTwoMatureParents()
        {
            var world = new WorldState { worldId = "w" };
            var plant = UnityEngine.ScriptableObject.CreateInstance<PlantDefinition>();
            try
            {
                plant.id = "p";
                plant.growSeconds = 100;
                var a = GardenService.Plant(world, plant, 0);
                var b = GardenService.Plant(world, plant, 0);

                Assert.IsNull(GardenService.CrossPlots(a, b, 1, nowUnix: 50), "unripe parents can't breed");
                Assert.IsNull(GardenService.CrossPlots(a, a, 1, nowUnix: 200), "a plant can't breed with itself");
                var child = GardenService.CrossPlots(a, b, 1, nowUnix: 200);
                Assert.IsNotNull(child, "two ripe parents breed");
                Assert.AreEqual(1, child.Value.generation);
            }
            finally { UnityEngine.Object.DestroyImmediate(plant); }
        }
    }
}

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Garden v1 contract tests — plant -> (tend) -> grow over time -> harvest -> profile inventory.
    /// Pure backend, headless: no scene, no device. Covers planting, the tend gate + speed/yield effect,
    /// the harvest tool gate + readiness, and ties to Core idle growth (ProfileEconomy.ResolveWorld).
    /// </summary>
    public class GardenServiceTests
    {
        private static PlantDefinition Plant(
            string id = "glowfruit", double growSeconds = 600,
            ToolFunction harvestWith = ToolFunction.Harvest,
            string biomeId = "", (string id, double amt)[] yield = null, string[] tendToolIds = null)
        {
            var p = ScriptableObject.CreateInstance<PlantDefinition>();
            p.id = id;
            p.growSeconds = growSeconds;
            p.harvestWith = harvestWith;
            p.biomeId = biomeId;
            p.harvestYield = new List<ResourceCost>();
            foreach (var y in (yield ?? new[] { ("glowfruit_fruit", 10.0) }))
                p.harvestYield.Add(new ResourceCost { resourceId = y.id, amount = y.amt });
            p.tendToolIds = new List<string>(tendToolIds ?? new[] { "watering_can" });
            return p;
        }

        private static ToolDefinition Tool(
            string id = "harvest_sickle", ToolFunction fn = ToolFunction.Harvest, float power = 1f, params string[] worksOn)
        {
            var t = ScriptableObject.CreateInstance<ToolDefinition>();
            t.id = id;
            t.function = fn;
            t.power = power;
            t.tier = 1;
            t.worksOn = new List<string>(worksOn);
            return t;
        }

        private static void Destroy(params Object[] objs)
        {
            foreach (var o in objs) if (o != null) Object.DestroyImmediate(o);
        }

        [Test]
        public void Plant_CreatesPlot_OnWorld()
        {
            var plant = Plant(growSeconds: 600);
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("garden_world", createIfMissing: true);

                var plot = GardenService.Plant(world, plant, nowUnix: 1000);
                Assert.IsNotNull(plot);
                Assert.AreEqual(1, world.plots.Count);
                Assert.AreEqual("glowfruit", plot.plantId);
                Assert.AreEqual(1000L, plot.plantedAtUnix);
                Assert.AreEqual(600.0, plot.growSeconds, 1e-9);
                Assert.AreEqual(1.0, plot.yieldMultiplier, 1e-9);
                Assert.IsFalse(plot.IsReady(1599));
                Assert.IsTrue(plot.IsReady(1600));
            }
            finally { Destroy(plant); }
        }

        [Test]
        public void Harvest_WhenReady_CreditsYieldToProfile_AndMarksHarvested()
        {
            var plant = Plant(growSeconds: 600, yield: new[] { ("glowfruit_fruit", 10.0) });
            var sickle = Tool(fn: ToolFunction.Harvest);
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("garden_world", createIfMissing: true);
                var plot = GardenService.Plant(world, plant, nowUnix: 0);

                Assert.AreEqual(HarvestPlantStatus.NotReady, GardenService.CanHarvest(plot, plant, sickle, 500));

                var r = GardenService.Harvest(profile, plot, plant, sickle, nowUnix: 600);
                Assert.IsTrue(r.Success);
                Assert.AreEqual(1, r.yieldEntries);
                Assert.AreEqual(10.0, profile.GetResource("glowfruit_fruit"), 1e-9);
                Assert.IsTrue(plot.harvested);

                // Second harvest does nothing.
                var again = GardenService.Harvest(profile, plot, plant, sickle, nowUnix: 700);
                Assert.AreEqual(HarvestPlantStatus.AlreadyHarvested, again.status);
                Assert.AreEqual(10.0, profile.GetResource("glowfruit_fruit"), 1e-9);
            }
            finally { Destroy(plant, sickle); }
        }

        [Test]
        public void Harvest_WrongToolFunction_Fails()
        {
            var plant = Plant(growSeconds: 0);             // instantly ready
            var pick = Tool(fn: ToolFunction.Mine);
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("w", createIfMissing: true);
                var plot = GardenService.Plant(world, plant, nowUnix: 0);
                Assert.AreEqual(HarvestPlantStatus.WrongToolFunction, GardenService.CanHarvest(plot, plant, pick, 10));
            }
            finally { Destroy(plant, pick); }
        }

        [Test]
        public void Tend_SpeedsGrowth_AndBoostsYield_OncePerTool()
        {
            var plant = Plant(growSeconds: 600, yield: new[] { ("glowfruit_fruit", 10.0) },
                              tendToolIds: new[] { "watering_can" });
            var can = Tool(id: "watering_can", fn: ToolFunction.Water, power: 1f);
            var sickle = Tool(fn: ToolFunction.Harvest);
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("garden_world", createIfMissing: true);
                var plot = GardenService.Plant(world, plant, nowUnix: 0);

                Assert.AreEqual(TendStatus.Success, GardenService.Tend(plot, plant, can, 10));
                Assert.AreEqual(1.25, plot.yieldMultiplier, 1e-9);     // +0.25 * power
                Assert.AreEqual(-60L, plot.plantedAtUnix);             // 60s growth credit (speed)

                // Same tool again = no double-dipping.
                Assert.AreEqual(TendStatus.AlreadyTended, GardenService.Tend(plot, plant, can, 20));
                Assert.AreEqual(1.25, plot.yieldMultiplier, 1e-9);

                // Sped up: ready at 540 instead of 600 (540 - (-60) = 600).
                Assert.IsTrue(plot.IsReady(540));

                var r = GardenService.Harvest(profile, plot, plant, sickle, nowUnix: 540);
                Assert.IsTrue(r.Success);
                Assert.AreEqual(12.5, profile.GetResource("glowfruit_fruit"), 1e-9); // 10 * 1.25
            }
            finally { Destroy(plant, can, sickle); }
        }

        [Test]
        public void Tend_ToolNotInPlantList_Rejected()
        {
            var plant = Plant(tendToolIds: new[] { "watering_can" });
            var wrong = Tool(id: "fertilizer", fn: ToolFunction.Fertilize);
            try
            {
                var world = new WorldState { worldId = "w" };
                var plot = GardenService.Plant(world, plant, 0);
                Assert.AreEqual(TendStatus.ToolNotApplicable, GardenService.CanTend(plot, plant, wrong));
            }
            finally { Destroy(plant, wrong); }
        }

        [Test]
        public void Garden_TiesToIdleResolve_PlotReadyCounted()
        {
            var plant = Plant(growSeconds: 300);
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("garden_world", createIfMissing: true);
                GardenService.Plant(world, plant, nowUnix: 0);

                var res = ProfileEconomy.ResolveWorld(world, nowUnix: 400);
                Assert.AreEqual(1, res.plotsReady);
            }
            finally { Destroy(plant); }
        }
    }
}

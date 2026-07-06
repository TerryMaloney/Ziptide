using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Additions Bank GARDEN #4/#5 — harvest timing: a fresh pull pays a bonus, an ignored crop decays
    /// toward a floor (never dies). Pure GardenService logic, headless. readyAt = 1000 + 600 = 1600.
    /// </summary>
    public class GardenTimingTests
    {
        private static PlantDefinition Plant(double growSeconds = 600, double freshOverride = 0, double overripeOverride = 0)
        {
            var p = ScriptableObject.CreateInstance<PlantDefinition>();
            p.id = "glowfruit";
            p.growSeconds = growSeconds;
            p.harvestWith = ToolFunction.Harvest;
            p.harvestYield = new List<ResourceCost> { new ResourceCost { resourceId = "glowfruit_fruit", amount = 10.0 } };
            p.tendToolIds = new List<string> { "watering_can" };
            p.freshWindowSecondsOverride = freshOverride;
            p.overripeAfterSecondsOverride = overripeOverride;
            return p;
        }

        private static ToolDefinition Sickle()
        {
            var t = ScriptableObject.CreateInstance<ToolDefinition>();
            t.id = "harvest_sickle";
            t.function = ToolFunction.Harvest;
            t.power = 1f;
            t.tier = 1;
            t.worksOn = new List<string>();
            return t;
        }

        private static PlotState PlotAt(PlantDefinition plant, long planted = 1000)
        {
            var profile = new PlayerProfile();
            var world = profile.GetWorld("garden_world", createIfMissing: true);
            return GardenService.Plant(world, plant, planted);
        }

        private static void Destroy(params Object[] objs)
        {
            foreach (var o in objs) if (o != null) Object.DestroyImmediate(o);
        }

        [Test]
        public void Fresh_WithinWindow_PaysBonus()
        {
            var p = Plant();
            try
            {
                var plot = PlotAt(p);
                Assert.AreEqual(HarvestTiming.Fresh, GardenService.TimingOf(plot, p, 1600 + 60));
                Assert.AreEqual(1.0 + GardenService.FreshBonus, GardenService.TimingMultiplier(plot, p, 1600 + 60), 1e-9);
            }
            finally { Destroy(p); }
        }

        [Test]
        public void Prime_AfterFresh_BeforeOverripe_IsFlat()
        {
            var p = Plant();
            try
            {
                var plot = PlotAt(p);
                long t = 1600 + 300; // past the 120s fresh window, before the 900s overripe onset
                Assert.AreEqual(HarvestTiming.Prime, GardenService.TimingOf(plot, p, t));
                Assert.AreEqual(1.0, GardenService.TimingMultiplier(plot, p, t), 1e-9);
            }
            finally { Destroy(p); }
        }

        [Test]
        public void Overripe_StartsAtFull_ThenDecaysLinearly()
        {
            var p = Plant();
            try
            {
                var plot = PlotAt(p);
                long onset = 1600 + 900;
                Assert.AreEqual(HarvestTiming.Overripe, GardenService.TimingOf(plot, p, onset));
                Assert.AreEqual(1.0, GardenService.TimingMultiplier(plot, p, onset), 1e-9); // decay t=0
                double mid = GardenService.TimingMultiplier(plot, p, onset + 450); // half the 900s decay
                Assert.AreEqual(1.0 - (1.0 - GardenService.OverripeFloor) * 0.5, mid, 1e-9);
            }
            finally { Destroy(p); }
        }

        [Test]
        public void Overripe_NeverBelowFloor()
        {
            var p = Plant();
            try
            {
                var plot = PlotAt(p);
                double m = GardenService.TimingMultiplier(plot, p, 1600 + 900 + 100000);
                Assert.AreEqual(GardenService.OverripeFloor, m, 1e-9);
                Assert.GreaterOrEqual(m, GardenService.OverripeFloor);
            }
            finally { Destroy(p); }
        }

        [Test]
        public void BeforeReady_IsPrimeAndFlat()
        {
            var p = Plant();
            try
            {
                var plot = PlotAt(p);
                Assert.AreEqual(HarvestTiming.Prime, GardenService.TimingOf(plot, p, 1500));
                Assert.AreEqual(1.0, GardenService.TimingMultiplier(plot, p, 1500), 1e-9);
            }
            finally { Destroy(p); }
        }

        [Test]
        public void PerPlantOverride_WidensTheFreshWindow()
        {
            var p = Plant(freshOverride: 300);
            try
            {
                var plot = PlotAt(p);
                // At +250s the DEFAULT would already be Prime (past 120s); a 300s override keeps it Fresh.
                Assert.AreEqual(HarvestTiming.Fresh, GardenService.TimingOf(plot, p, 1600 + 250));
            }
            finally { Destroy(p); }
        }

        [Test]
        public void Harvest_FoldsTiming_IntoTheCreditedYield()
        {
            var p = Plant();
            var tool = Sickle();
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("garden_world", createIfMissing: true);
                var plot = GardenService.Plant(world, p, 1000); // readyAt 1600, tend multiplier 1.0
                var res = GardenService.Harvest(profile, plot, p, tool, 1650); // fresh
                Assert.IsTrue(res.Success);
                Assert.AreEqual(HarvestTiming.Fresh, res.timing);
                Assert.AreEqual(1.0 + GardenService.FreshBonus, res.yieldMultiplier, 1e-9, "tend 1.0 × fresh 1.25");
            }
            finally { Destroy(p, tool); }
        }

        [Test]
        public void Harvest_Overripe_PaysLessButNeverZero()
        {
            var p = Plant();
            var tool = Sickle();
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("garden_world", createIfMissing: true);
                var plot = GardenService.Plant(world, p, 1000);
                var res = GardenService.Harvest(profile, plot, p, tool, 1600 + 900 + 100000); // long overripe
                Assert.IsTrue(res.Success);
                Assert.AreEqual(HarvestTiming.Overripe, res.timing);
                Assert.AreEqual(GardenService.OverripeFloor, res.yieldMultiplier, 1e-9);
                Assert.Greater(res.yieldMultiplier, 0.0, "all-ages: a neglected crop still pays out");
            }
            finally { Destroy(p, tool); }
        }
    }
}

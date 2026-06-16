using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Mining v1 contract tests — build an extractor (spend recipe) then accrue + collect its resource.
    /// Pure backend, headless: no scene, no device. Covers RecipeService (afford/spend, all-or-nothing),
    /// MiningService.TryBuildMine, and the full loop tied to Core's idle accrual (ProfileEconomy).
    /// </summary>
    public class MiningServiceTests
    {
        private static RecipeDefinition Recipe(params (string id, double amt)[] costs)
        {
            var r = ScriptableObject.CreateInstance<RecipeDefinition>();
            r.id = "recipe";
            r.costs = new List<ResourceCost>();
            foreach (var c in costs) r.costs.Add(new ResourceCost { resourceId = c.id, amount = c.amt });
            return r;
        }

        private static MachineDefinition Machine(double rate, double storageCap, RecipeDefinition build)
        {
            var m = ScriptableObject.CreateInstance<MachineDefinition>();
            m.id = "driller_mk1";
            m.ratePerSecond = rate;
            m.storageCap = storageCap;
            m.buildRecipe = build;
            return m;
        }

        private static ResourceNodeDefinition Node(string resourceId)
        {
            var n = ScriptableObject.CreateInstance<ResourceNodeDefinition>();
            n.id = "node_" + resourceId;
            n.resourceId = resourceId;
            return n;
        }

        private static void Destroy(params Object[] objs)
        {
            foreach (var o in objs) if (o != null) Object.DestroyImmediate(o);
        }

        [Test]
        public void RecipeService_CanAfford_RespectsBalanceAndFreeRecipes()
        {
            var recipe = Recipe(("scrap", 4), ("gear", 2));
            try
            {
                var profile = new PlayerProfile();
                profile.AddResource("scrap", 10);
                Assert.IsFalse(RecipeService.CanAfford(profile, recipe)); // no gear yet
                profile.AddResource("gear", 2);
                Assert.IsTrue(RecipeService.CanAfford(profile, recipe));
                Assert.IsTrue(RecipeService.CanAfford(profile, null)); // null recipe is free
            }
            finally { Destroy(recipe); }
        }

        [Test]
        public void RecipeService_TrySpend_IsAllOrNothing()
        {
            var recipe = Recipe(("scrap", 4), ("gear", 2));
            try
            {
                var profile = new PlayerProfile();
                profile.AddResource("scrap", 10); // gear missing

                Assert.IsFalse(RecipeService.TrySpend(profile, recipe));
                Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9); // nothing deducted on failure

                profile.AddResource("gear", 5);
                Assert.IsTrue(RecipeService.TrySpend(profile, recipe));
                Assert.AreEqual(6.0, profile.GetResource("scrap"), 1e-9);
                Assert.AreEqual(3.0, profile.GetResource("gear"), 1e-9);
            }
            finally { Destroy(recipe); }
        }

        [Test]
        public void TryBuildMine_Success_PlacesMine_AndSpendsBuildCost()
        {
            var build = Recipe(("scrap", 5));
            var machine = Machine(rate: 2.0, storageCap: 100.0, build: build);
            var node = Node("ore");
            try
            {
                var profile = new PlayerProfile();
                profile.AddResource("scrap", 8);
                var world = profile.GetWorld("d0_city", createIfMissing: true);

                var r = MiningService.TryBuildMine(profile, world, machine, node, nowUnix: 1000);

                Assert.IsTrue(r.Success);
                Assert.AreEqual(1, world.mines.Count);
                Assert.AreSame(r.mine, world.mines[0]);
                Assert.AreEqual("ore", r.mine.resourceId);
                Assert.AreEqual("driller_mk1", r.mine.machineId);
                Assert.AreEqual(2.0, r.mine.ratePerSecond, 1e-9);
                Assert.AreEqual(100.0, r.mine.storageCap, 1e-9);
                Assert.AreEqual(1000L, r.mine.lastResolvedAtUnix);
                Assert.AreEqual(3.0, profile.GetResource("scrap"), 1e-9); // 8 - 5 build cost
            }
            finally { Destroy(build, machine, node); }
        }

        [Test]
        public void TryBuildMine_CannotAfford_PlacesNothing_AndSpendsNothing()
        {
            var build = Recipe(("scrap", 5));
            var machine = Machine(rate: 2.0, storageCap: 100.0, build: build);
            var node = Node("ore");
            try
            {
                var profile = new PlayerProfile();
                profile.AddResource("scrap", 2); // not enough
                var world = profile.GetWorld("d0_city", createIfMissing: true);

                var r = MiningService.TryBuildMine(profile, world, machine, node, nowUnix: 1000);

                Assert.AreEqual(BuildMineStatus.CannotAfford, r.status);
                Assert.IsNull(r.mine);
                Assert.AreEqual(0, world.mines.Count);
                Assert.AreEqual(2.0, profile.GetResource("scrap"), 1e-9); // untouched
            }
            finally { Destroy(build, machine, node); }
        }

        [Test]
        public void TryBuildMine_InvalidNode_Fails()
        {
            var machine = Machine(rate: 1.0, storageCap: 0.0, build: null);
            var noResource = Node("");
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("w", createIfMissing: true);
                Assert.AreEqual(BuildMineStatus.InvalidNode, MiningService.TryBuildMine(profile, world, machine, noResource, 0).status);
                Assert.AreEqual(BuildMineStatus.InvalidNode, MiningService.TryBuildMine(profile, world, machine, null, 0).status);
                Assert.AreEqual(BuildMineStatus.InvalidArgs, MiningService.TryBuildMine(profile, null, machine, Node("ore"), 0).status);
            }
            finally { Destroy(machine, noResource); }
        }

        [Test]
        public void BuiltMine_AccruesIdle_AndCollectsToProfile()
        {
            // Full loop: build extractor (free recipe) -> idle accrue (capped) -> collect into inventory.
            var machine = Machine(rate: 2.0, storageCap: 100.0, build: null); // free to build
            var node = Node("ore");
            try
            {
                var profile = new PlayerProfile();
                var world = profile.GetWorld("d0_city", createIfMissing: true);

                var built = MiningService.TryBuildMine(profile, world, machine, node, nowUnix: 1000);
                Assert.IsTrue(built.Success);

                // 200s elapsed at 2/s = 400 produced, capped at 100.
                var res = ProfileEconomy.ResolveWorld(world, nowUnix: 1200);
                Assert.AreEqual(1, res.minesResolved);
                Assert.AreEqual(100.0, built.mine.stored, 1e-9);

                double collected = ProfileEconomy.CollectMine(profile, built.mine);
                Assert.AreEqual(100.0, collected, 1e-9);
                Assert.AreEqual(100.0, profile.GetResource("ore"), 1e-9);
                Assert.AreEqual(0.0, built.mine.stored, 1e-9);
            }
            finally { Destroy(machine, node); }
        }
    }
}

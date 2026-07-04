using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// THE GOLDEN META-LOOP (Terry's acceptance test for the whole architecture, GPT addendum §1):
    /// one deterministic chain — campaign reward → ledger → garden plant → offline growth → harvest
    /// → factory recipe → crafted component → upgrade/defense spend → async conquest income →
    /// ledger explanation → staleness lookup — proving the modes share ONE economy spine. Pure data,
    /// no scenes, no visuals. If this test passes, a lesser model can safely expand content.
    /// </summary>
    public class GoldenMetaLoopTests
    {
        // Mirrors EconomyAuthor's registry — the test asserts every id the loop touches is here.
        private static readonly HashSet<string> Registered = new HashSet<string>
        {
            "credits", "mineral", "spore", "memory_shard", "stun_charge_cell",
        };

        private static PlantDefinition Plant()
        {
            var p = ScriptableObject.CreateInstance<PlantDefinition>();
            p.id = "dew_bulb";
            p.growSeconds = 120;
            p.harvestWith = ToolFunction.Harvest;
            p.harvestYield.Add(new ResourceCost { resourceId = "spore", amount = 2 });
            p.harvestYield.Add(new ResourceCost { resourceId = "credits", amount = 10 });
            return p;
        }

        private static ToolDefinition Gloves()
        {
            var t = ScriptableObject.CreateInstance<ToolDefinition>();
            t.id = "field_gloves"; t.function = ToolFunction.Harvest; t.power = 1f;
            return t;
        }

        private static RecipeDefinition StunCell()
        {
            var r = ScriptableObject.CreateInstance<RecipeDefinition>();
            r.id = "recipe_stun_charge_cell";
            r.costs.Add(new ResourceCost { resourceId = "spore", amount = 2 });
            r.costs.Add(new ResourceCost { resourceId = "mineral", amount = 2 });
            r.producesId = "stun_charge_cell";
            r.producesAmount = 1;
            r.requiredMachineType = MachineType.BioRefiner;
            r.durationTicks = 12;
            r.sourceWorlds.Add("W002_DryCistern");
            r.storyTags.Add("guild_work");
            return r;
        }

        private static List<MachineNodeState> Factory() => new List<MachineNodeState>
        {
            new MachineNodeState { nodeId = "bin", type = MachineType.InputBin },
            new MachineNodeState { nodeId = "refiner", type = MachineType.BioRefiner,
                recipeId = "recipe_stun_charge_cell", inputNodeIds = { "bin" } },
            new MachineNodeState { nodeId = "outstation", type = MachineType.OutputStation,
                inputNodeIds = { "refiner" } },
        };

        [Test]
        public void GoldenMetaLoop_CampaignGardenFactoryConquest_RoundTripsThroughLedger()
        {
            var profile = ProfileSerializer.NewProfile();
            var plant = Plant();
            var recipe = StunCell();
            long t0 = 1_000_000;

            // 1. CAMPAIGN — a contract pays out (JobRewards routes through RewardRouter).
            var job = ScriptableObject.CreateInstance<JobDefinition>();
            job.jobId = "w002_pumps";
            job.reward.Add(new ResourceCost { resourceId = "mineral", amount = 3 });
            job.reward.Add(new ResourceCost { resourceId = "credits", amount = 20 });
            JobRewards.Grant(job, profile);
            Assert.AreEqual(3, profile.GetResource("mineral"));

            // 2. GARDEN — plant, grow OFFLINE (pure elapsed time), harvest.
            var world = profile.GetWorld("W002_DryCistern", createIfMissing: true);
            var plot = GardenService.Plant(world, plant, t0);
            plot.plotId = "garden_grove_0";
            long later = t0 + 200; // past growSeconds — offline growth needs no scene, no ticks
            Assert.IsTrue(plot.IsReady(later));
            var harvest = GardenService.Harvest(profile, plot, plant, Gloves(), later);
            Assert.IsTrue(harvest.Success);
            Assert.AreEqual(2, profile.GetResource("spore"));

            // 3. FACTORY — a valid graph consumes spore+mineral and crafts the component offline.
            var factory = Factory();
            RecipeDefinition Lookup(string id) => id == recipe.id ? recipe : null;
            CollectionAssert.IsEmpty(ProductionGraph.Validate(factory, Lookup, Registered),
                "golden factory layout must validate clean");
            int made = ProductionGraph.CatchUp(factory, profile, "W002_DryCistern", Lookup,
                elapsedSeconds: 65); // 13 ticks > durationTicks
            Assert.AreEqual(1, made, "one stun charge cell batch should complete");
            Assert.AreEqual(1, profile.GetResource("stun_charge_cell"));
            Assert.AreEqual(0, profile.GetResource("spore"), "factory consumed the bio input");
            Assert.AreEqual(1, profile.GetResource("mineral"), "3 - 2 consumed");

            // 4. UPGRADE / CONQUEST DEFENSE — spend the component; async income flows back.
            Assert.IsTrue(RewardRouter.TrySpend(profile, LedgerSource.UpgradeCost, "stun_charge_cell", 1,
                reason: "defense_module_w002", worldId: "W002_DryCistern"));
            RewardRouter.Grant(profile, LedgerSource.Multiplayer, "mineral", 2, reason: "territory_income");
            RewardRouter.Grant(profile, LedgerSource.Multiplayer, "memory_shard", 1, reason: "conquest_reward");

            // 5. THE LEDGER EXPLAINS EVERYTHING — every mode's movement is recorded and queryable.
            var sourcesSeen = new HashSet<string>();
            foreach (var e in profile.ledger) sourcesSeen.Add(e.source);
            CollectionAssert.IsSubsetOf(new[]
            {
                LedgerSource.Campaign, LedgerSource.Garden, LedgerSource.RecipeCost,
                LedgerSource.Factory, LedgerSource.UpgradeCost, LedgerSource.Multiplayer,
            }, sourcesSeen, "a mode moved resources without leaving a ledger trail");
            Assert.AreEqual(0, ResourceLedger.SumFor(profile, "stun_charge_cell"), "crafted then spent");
            StringAssert.Contains("factory_output", ResourceLedger.Explain(profile, "stun_charge_cell"));

            // 6. EVERY id the loop touched is registered (the one-economy law).
            foreach (var e in profile.ledger)
                Assert.IsTrue(Registered.Contains(e.resourceId), "unregistered id in loop: " + e.resourceId);

            // 7. SAVE/LOAD preserves the loop state (ledger, plots, factory nodes, versions).
            world.factory = factory;
            string json = ProfileSerializer.Serialize(profile);
            var loaded = ProfileSerializer.Deserialize(json);
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, loaded.schemaVersion);
            Assert.AreEqual(profile.ledger.Count, loaded.ledger.Count);
            Assert.AreEqual(1, loaded.GetWorld("W002_DryCistern").factory.Count == 3 ? 1 : 0,
                "factory layout survived the round trip");

            // 8. STALENESS — "what does changing W002 touch?" is answerable from the flow model.
            var flow = new EconomyFlowModel();
            flow.AddSource("mineral", "job:w002_pumps@W002_DryCistern");
            flow.AddSink("spore", "recipe:recipe_stun_charge_cell@W002_DryCistern");
            flow.AddSource("stun_charge_cell", "recipe:recipe_stun_charge_cell@W002_DryCistern");
            var affected = flow.AffectedBy("W002");
            Assert.IsTrue(affected.ContainsKey("mineral") && affected.ContainsKey("stun_charge_cell"),
                "a W002 story change must surface its dependent resources");
        }

        [Test]
        public void OfflineCatchUp_IsCappedAgainstExploits()
        {
            var profile = ProfileSerializer.NewProfile();
            RewardRouter.Grant(profile, LedgerSource.Debug, "spore", 10000);
            RewardRouter.Grant(profile, LedgerSource.Debug, "mineral", 10000);
            var recipe = StunCell();
            var factory = Factory();
            RecipeDefinition Lookup(string id) => recipe;
            int made = ProductionGraph.CatchUp(factory, profile, "w", Lookup, elapsedSeconds: 999_999_999);
            Assert.LessOrEqual(made, ProductionGraph.MaxCatchUpTicks / recipe.durationTicks + 1,
                "a year offline must not out-produce the catch-up cap");
        }

        [Test]
        public void OldSave_MigratesForwardClean()
        {
            string v0 = "{\"schemaVersion\":0,\"playerId\":\"old\",\"displayName\":\"Cal\"," +
                        "\"flags\":[\"TUTORIAL_COMPLETE\"],\"resources\":[{\"id\":\"credits\",\"amount\":50}]}";
            var p = ProfileSerializer.Deserialize(v0);
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, p.schemaVersion);
            Assert.IsNotNull(p.ledger, "v0 saves gain an empty ledger");
            Assert.AreEqual(50, p.GetResource("credits"), "old data survives migration");
        }

        [Test]
        public void Ledger_CapsAndSpendGuardsHold()
        {
            var profile = ProfileSerializer.NewProfile();
            Assert.IsFalse(RewardRouter.TrySpend(profile, LedgerSource.RecipeCost, "mineral", 5),
                "spending what you don't have must fail AND leave no ledger entry");
            Assert.AreEqual(0, profile.ledger.Count);
            for (int i = 0; i < ResourceLedger.MaxEntries + 50; i++)
                RewardRouter.Grant(profile, LedgerSource.Debug, "credits", 1);
            Assert.AreEqual(ResourceLedger.MaxEntries, profile.ledger.Count, "ring cap holds");
        }

        [Test]
        public void ProductionGraph_ValidatorCatchesBrokenLayouts()
        {
            var recipe = StunCell();
            RecipeDefinition Lookup(string id) => id == recipe.id ? recipe : null;
            // A refiner floating with no input path and no output path, plus a bogus recipe id.
            var broken = new List<MachineNodeState>
            {
                new MachineNodeState { nodeId = "refiner", type = MachineType.BioRefiner, recipeId = "nope" },
                new MachineNodeState { nodeId = "lone", type = MachineType.Assembler,
                    recipeId = recipe.id, inputNodeIds = { "ghost" } },
            };
            var issues = ProductionGraph.Validate(broken, Lookup, Registered);
            Assert.IsTrue(issues.Exists(i => i.StartsWith("RECIPE_UNKNOWN")), string.Join("|", issues));
            Assert.IsTrue(issues.Exists(i => i.StartsWith("NODE_INPUT_UNKNOWN")));
            Assert.IsTrue(issues.Exists(i => i.StartsWith("MACHINE_NO_INPUT_PATH")));
            Assert.IsTrue(issues.Exists(i => i.StartsWith("MACHINE_NO_OUTPUT_PATH")));
            // Wrong machine type for the recipe:
            var wrong = new List<MachineNodeState>
            {
                new MachineNodeState { nodeId = "bin", type = MachineType.InputBin },
                new MachineNodeState { nodeId = "asm", type = MachineType.Assembler,
                    recipeId = recipe.id, inputNodeIds = { "bin" } },
                new MachineNodeState { nodeId = "outstation", type = MachineType.OutputStation,
                    inputNodeIds = { "asm" } },
            };
            Assert.IsTrue(ProductionGraph.Validate(wrong, Lookup, Registered)
                .Exists(i => i.StartsWith("RECIPE_WRONG_MACHINE")));
        }

        [Test]
        public void FlowModel_FlagsOrphansDeadEndsAndUnused()
        {
            var flow = new EconomyFlowModel();
            flow.AddSource("mineral", "job:x");
            flow.AddSink("mineral", "recipe:y");
            flow.AddSource("orphan_out", "recipe:y"); // produced, never consumed
            flow.AddSink("ghost_in", "recipe:z");     // consumed, never produced
            var registered = new[] { "mineral", "orphan_out", "ghost_in", "never_used" };
            CollectionAssert.Contains(flow.NoSink(registered), "orphan_out");
            CollectionAssert.Contains(flow.NoSource(registered), "ghost_in");
            CollectionAssert.Contains(flow.Unused(registered), "never_used");
            CollectionAssert.DoesNotContain(flow.NoSource(registered), "mineral");
        }
    }
}

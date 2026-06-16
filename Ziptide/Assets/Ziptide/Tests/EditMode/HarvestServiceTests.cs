using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Harvest v1 contract tests — the simplest economy loop (node + tool -> profile inventory).
    /// Pure backend, headless: no scene, no device. Guards the data-driven gate (function/tier/worksOn),
    /// the yield math (power + reserve clamp), and reserve depletion/exhaustion.
    /// </summary>
    public class HarvestServiceTests
    {
        private static ResourceNodeDefinition Node(
            string resourceId = "scrap", double yield = 1.0, double reserve = 0.0,
            ToolFunction fn = ToolFunction.Mine, int reqTier = 1, string biomeId = "", string nodeId = "node_a")
        {
            var n = ScriptableObject.CreateInstance<ResourceNodeDefinition>();
            n.id = nodeId;
            n.resourceId = resourceId;
            n.yieldPerHarvest = yield;
            n.reserve = reserve;
            n.requiredFunction = fn;
            n.requiredToolTier = reqTier;
            n.biomeId = biomeId;
            return n;
        }

        private static ToolDefinition Tool(
            ToolFunction fn = ToolFunction.Mine, int tier = 1, float power = 1f, params string[] worksOn)
        {
            var t = ScriptableObject.CreateInstance<ToolDefinition>();
            t.id = "tool_a";
            t.function = fn;
            t.tier = tier;
            t.power = power;
            t.worksOn = new List<string>(worksOn);
            return t;
        }

        private static void Destroy(params Object[] objs)
        {
            foreach (var o in objs) if (o != null) Object.DestroyImmediate(o);
        }

        [Test]
        public void Harvest_Success_CreditsProfileInventory()
        {
            var node = Node(resourceId: "scrap", yield: 5.0, reserve: 0.0); // inexhaustible
            var tool = Tool();
            try
            {
                var profile = new PlayerProfile();
                var rn = new ResourceNode();
                rn.Init(node);

                var r1 = rn.Harvest(tool, profile);
                Assert.IsTrue(r1.Success);
                Assert.AreEqual("scrap", r1.resourceId);
                Assert.AreEqual(5.0, r1.amount, 1e-9);
                Assert.AreEqual(5.0, profile.GetResource("scrap"), 1e-9);

                rn.Harvest(tool, profile); // inexhaustible: keeps producing
                Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9);
                Assert.IsFalse(rn.IsExhausted);
            }
            finally { Destroy(node, tool); }
        }

        [Test]
        public void Harvest_ToolPower_ScalesYield()
        {
            var node = Node(yield: 2.0);
            var tool = Tool(power: 3f);
            try
            {
                var r = HarvestService.Evaluate(node, remaining: 0, tool: tool);
                Assert.IsTrue(r.Success);
                Assert.AreEqual(6.0, r.amount, 1e-9);
            }
            finally { Destroy(node, tool); }
        }

        [Test]
        public void Harvest_WrongFunction_Fails_AndLeavesProfileUntouched()
        {
            var node = Node(fn: ToolFunction.Mine);
            var tool = Tool(fn: ToolFunction.Harvest);
            try
            {
                var profile = new PlayerProfile();
                var rn = new ResourceNode();
                rn.Init(node);

                var r = rn.Harvest(tool, profile);
                Assert.AreEqual(HarvestStatus.WrongToolFunction, r.status);
                Assert.IsFalse(r.Success);
                Assert.AreEqual(0.0, profile.GetResource("scrap"), 1e-9);
            }
            finally { Destroy(node, tool); }
        }

        [Test]
        public void Harvest_ToolTierTooLow_Fails()
        {
            var node = Node(reqTier: 2);
            var tool = Tool(tier: 1);
            try
            {
                Assert.AreEqual(HarvestStatus.ToolTierTooLow, HarvestService.CheckTool(node, tool));
            }
            finally { Destroy(node, tool); }
        }

        [Test]
        public void Harvest_WorksOnGate_RestrictsAndMatches()
        {
            var node = Node(resourceId: "scrap", biomeId: "toxic_venice", nodeId: "node_scrap");
            var wrong = Tool(worksOn: new[] { "ore" });
            var byResource = Tool(worksOn: new[] { "scrap" });
            var byNodeId = Tool(worksOn: new[] { "node_scrap" });
            var byBiome = Tool(worksOn: new[] { "toxic_venice" });
            try
            {
                Assert.AreEqual(HarvestStatus.ToolCannotWorkNode, HarvestService.CheckTool(node, wrong));
                Assert.AreEqual(HarvestStatus.Success, HarvestService.CheckTool(node, byResource));
                Assert.AreEqual(HarvestStatus.Success, HarvestService.CheckTool(node, byNodeId));
                Assert.AreEqual(HarvestStatus.Success, HarvestService.CheckTool(node, byBiome));
            }
            finally { Destroy(node, wrong, byResource, byNodeId, byBiome); }
        }

        [Test]
        public void Harvest_FiniteReserve_DepletesClampsAndExhausts()
        {
            var node = Node(resourceId: "scrap", yield: 4.0, reserve: 10.0);
            var tool = Tool();
            try
            {
                var profile = new PlayerProfile();
                var rn = new ResourceNode();
                rn.Init(node);
                Assert.AreEqual(10.0, rn.Remaining, 1e-9);

                Assert.AreEqual(4.0, rn.Harvest(tool, profile).amount, 1e-9); // 10 -> 6
                Assert.AreEqual(4.0, rn.Harvest(tool, profile).amount, 1e-9); // 6 -> 2
                Assert.AreEqual(2.0, rn.Harvest(tool, profile).amount, 1e-9); // clamped 2 -> 0

                Assert.IsTrue(rn.IsExhausted);
                Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9);

                var spent = rn.Harvest(tool, profile);
                Assert.AreEqual(HarvestStatus.NodeExhausted, spent.status);
                Assert.AreEqual(10.0, profile.GetResource("scrap"), 1e-9); // no further credit
            }
            finally { Destroy(node, tool); }
        }

        [Test]
        public void Evaluate_IsPure_NoNodeMutation()
        {
            var node = Node(yield: 3.0, reserve: 9.0);
            var tool = Tool();
            try
            {
                var rn = new ResourceNode();
                rn.Init(node);

                var a = HarvestService.Evaluate(node, rn.Remaining, tool);
                var b = HarvestService.Evaluate(node, rn.Remaining, tool);
                Assert.AreEqual(a.amount, b.amount, 1e-9);   // repeatable
                Assert.AreEqual(9.0, rn.Remaining, 1e-9);     // preview did not deplete
            }
            finally { Destroy(node, tool); }
        }

        [Test]
        public void Harvest_InvalidNodeOrTool_Fails()
        {
            var node = Node();
            try
            {
                Assert.AreEqual(HarvestStatus.InvalidTool, HarvestService.CheckTool(node, null));
                Assert.AreEqual(HarvestStatus.InvalidNode, HarvestService.CheckTool(null, Tool()));
            }
            finally { Destroy(node); }
        }
    }
}

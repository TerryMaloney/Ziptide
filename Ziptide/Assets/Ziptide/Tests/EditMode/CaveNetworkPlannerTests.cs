using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Phase 1.4: the cave network planner's contracts — deterministic, connected, spaced,
    /// bounded, and correctly classified. Pure, headless.</summary>
    public class CaveNetworkPlannerTests
    {
        [Test]
        public void SameSeed_SamePlan()
        {
            var a = CaveNetworkPlanner.Plan(seed: 42);
            var b = CaveNetworkPlanner.Plan(seed: 42);
            Assert.AreEqual(a.Chambers.Count, b.Chambers.Count);
            Assert.AreEqual(a.Tunnels.Count, b.Tunnels.Count);
            for (int i = 0; i < a.Chambers.Count; i++)
            {
                Assert.AreEqual(a.Chambers[i].X, b.Chambers[i].X, 1e-6f);
                Assert.AreEqual(a.Chambers[i].Y, b.Chambers[i].Y, 1e-6f);
                Assert.AreEqual(a.Chambers[i].Z, b.Chambers[i].Z, 1e-6f);
            }
        }

        [Test]
        public void DifferentSeeds_DifferentPlans()
        {
            var a = CaveNetworkPlanner.Plan(seed: 1);
            var b = CaveNetworkPlanner.Plan(seed: 2);
            bool anyDiff = a.Chambers.Count != b.Chambers.Count;
            for (int i = 0; !anyDiff && i < a.Chambers.Count; i++)
                anyDiff = System.Math.Abs(a.Chambers[i].X - b.Chambers[i].X) > 1e-4f;
            Assert.IsTrue(anyDiff, "two seeds should not produce the same cave");
        }

        [Test]
        public void EveryChamber_IsReachable_AcrossManySeeds()
        {
            for (int seed = 1; seed <= 25; seed++)
                Assert.IsTrue(CaveNetworkPlanner.IsConnected(CaveNetworkPlanner.Plan(seed)),
                    "seed " + seed + " produced a disconnected cave — the MST contract broke");
        }

        [Test]
        public void MinSpacing_And_Bounds_AreRespected()
        {
            var p = CaveNetworkPlanner.Plan(seed: 7, extentX: 30f, extentZ: 25f, depth: 12f,
                                            chamberTarget: 8, minSpacing: 9f);
            for (int i = 0; i < p.Chambers.Count; i++)
            {
                var c = p.Chambers[i];
                Assert.LessOrEqual(System.Math.Abs(c.X), 30f);
                Assert.LessOrEqual(System.Math.Abs(c.Z), 25f);
                Assert.LessOrEqual(c.Y, 0f); Assert.GreaterOrEqual(c.Y, -12f);
                for (int j = i + 1; j < p.Chambers.Count; j++)
                {
                    var d = p.Chambers[j];
                    float dist = (float)System.Math.Sqrt(
                        (c.X - d.X) * (c.X - d.X) + (c.Y - d.Y) * (c.Y - d.Y) + (c.Z - d.Z) * (c.Z - d.Z));
                    Assert.GreaterOrEqual(dist, 9f - 1e-3f, "chambers too close");
                }
            }
        }

        [Test]
        public void Loops_AddRouteChoice_BeyondTheTree()
        {
            // A tree has exactly n-1 edges; with loopChance forced high and a dense scatter,
            // the plan should carry extra loop tunnels (route choice is the fun).
            var p = CaveNetworkPlanner.Plan(seed: 11, extentX: 25f, extentZ: 25f, depth: 8f,
                                            chamberTarget: 12, minSpacing: 7f, loopChance: 1f);
            Assert.Greater(p.Tunnels.Count, p.Chambers.Count - 1, "no loops formed — pure tree");
            Assert.IsTrue(CaveNetworkPlanner.IsConnected(p));
        }

        [Test]
        public void Classification_DeadEndsAndJunctions_MatchLinkCounts()
        {
            var p = CaveNetworkPlanner.Plan(seed: 3, chamberTarget: 10, loopChance: 0f); // pure tree
            int deadEnds = 0;
            foreach (var c in p.Chambers)
            {
                if (c.IsDeadEnd) deadEnds++;
                if (c.Links >= 3) Assert.IsTrue(c.IsJunction);
            }
            Assert.Greater(deadEnds, 0, "a tree always has leaves — the secret/loot spots");
        }

        [Test]
        public void SteepTunnels_ClassifyAsShafts()
        {
            // Deep volume + tight XZ forces steep connections somewhere across seeds.
            bool sawShaft = false, sawWalk = false;
            for (int seed = 1; seed <= 10 && !(sawShaft && sawWalk); seed++)
            {
                var p = CaveNetworkPlanner.Plan(seed, extentX: 12f, extentZ: 12f, depth: 30f,
                                                chamberTarget: 8, minSpacing: 8f);
                foreach (var t in p.Tunnels) { if (t.IsShaft) sawShaft = true; else sawWalk = true; }
            }
            Assert.IsTrue(sawShaft, "a 30m-deep, 12m-wide cave must produce vertical shafts");
            Assert.IsTrue(sawWalk, "and still some walkable tunnels");
        }
    }
}

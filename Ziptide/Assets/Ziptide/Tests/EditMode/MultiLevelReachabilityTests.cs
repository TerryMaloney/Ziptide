using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Phase 1.4: stacked-world reachability — a heightmap can't overhang, so vertical worlds
    /// need cross-layer traversal edges to be provably reachable. Pure, headless.</summary>
    public class MultiLevelReachabilityTests
    {
        private static bool[] AllWalkable(int n)
        {
            var w = new bool[n];
            for (int i = 0; i < n; i++) w[i] = true;
            return w;
        }

        [Test]
        public void UpperLayer_IsUnreachable_WithoutAVerticalEdge()
        {
            // 2x2 x 2 layers. Both layers fully walkable, but nothing links them.
            var g = new MultiLevelReachability(2, 2, 2);
            g.SetLayer(0, AllWalkable(4), null, 1f);
            g.SetLayer(1, AllWalkable(4), null, 1f);

            var reached = g.Flood(g.NodeId(0, 0, 0));
            // The whole lower layer is reachable; none of the upper layer is.
            Assert.IsTrue(reached[g.NodeId(0, 1, 1)], "lower layer connected");
            Assert.IsFalse(reached[g.NodeId(1, 0, 0)], "upper layer stranded — a heightmap can't reach it");
            Assert.AreEqual(4, MultiLevelReachability.Count(reached));
        }

        [Test]
        public void AZipline_ConnectsTheLayers()
        {
            var g = new MultiLevelReachability(2, 2, 2);
            g.SetLayer(0, AllWalkable(4), null, 1f);
            g.SetLayer(1, AllWalkable(4), null, 1f);
            g.AddEdge(new TraversalEdge(g.NodeId(0, 1, 1), g.NodeId(1, 0, 0), TraversalKind.Zipline, bidirectional: true));

            var reached = g.Flood(g.NodeId(0, 0, 0));
            Assert.IsTrue(reached[g.NodeId(1, 1, 1)], "the whole upper layer is now reachable via the zip");
            Assert.AreEqual(8, MultiLevelReachability.Count(reached), "both layers fully connected");
        }

        [Test]
        public void OneWayZipline_DoesNotLetYouRideBackUp()
        {
            // A downhill zip from the top layer to the bottom — one-way by design.
            var g = new MultiLevelReachability(2, 2, 2);
            g.SetLayer(0, AllWalkable(4), null, 1f);
            g.SetLayer(1, AllWalkable(4), null, 1f);
            g.AddEdge(new TraversalEdge(g.NodeId(1, 0, 0), g.NodeId(0, 0, 0), TraversalKind.Zipline, bidirectional: false));

            // From the TOP, you can reach the bottom.
            Assert.IsTrue(g.Flood(g.NodeId(1, 0, 0))[g.NodeId(0, 1, 1)], "top reaches bottom via the one-way zip");
            // From the BOTTOM, you cannot climb back up the one-way zip.
            Assert.IsFalse(g.Flood(g.NodeId(0, 0, 0))[g.NodeId(1, 0, 0)], "one-way — no riding back up");
        }

        [Test]
        public void MaxStep_BreaksSameLayerConnectivity_LikeACliff()
        {
            // One layer, a 3-wide strip; the middle cell is a cliff (big height jump) → right cell stranded.
            var g = new MultiLevelReachability(3, 1, 1);
            var walk = AllWalkable(3);
            var heights = new float[] { 0f, 0f, 5f }; // cell 2 is 5m up
            g.SetLayer(0, walk, heights, maxStep: 1f);

            var reached = g.Flood(g.NodeId(0, 0, 0));
            Assert.IsTrue(reached[g.NodeId(0, 1, 0)], "the gentle step is walkable");
            Assert.IsFalse(reached[g.NodeId(0, 2, 0)], "the cliff breaks connectivity");
        }

        [Test]
        public void Unreached_ListsStrandedNodes_ForTheAudit()
        {
            var g = new MultiLevelReachability(2, 2, 2);
            g.SetLayer(0, AllWalkable(4), null, 1f);
            g.SetLayer(1, AllWalkable(4), null, 1f);

            var stranded = g.Unreached(g.NodeId(0, 0, 0));
            Assert.AreEqual(4, stranded.Count, "the whole upper layer is flagged unreachable");
            CollectionAssert.Contains(stranded, g.NodeId(1, 0, 0));
        }
    }
}

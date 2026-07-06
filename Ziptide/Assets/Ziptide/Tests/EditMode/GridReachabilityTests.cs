using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>WORLDS_50 #13 — the pure reachability flood-fill's contract.</summary>
    public class GridReachabilityTests
    {
        // Build a w*h all-walkable grid.
        private static bool[] AllWalkable(int w, int h)
        {
            var g = new bool[w * h];
            for (int i = 0; i < g.Length; i++) g[i] = true;
            return g;
        }

        [Test]
        public void OpenGrid_EverythingReachable()
        {
            int w = 5, h = 4;
            var reached = GridReachability.Flood(w, h, AllWalkable(w, h), null, 0, 0, 100f);
            Assert.AreEqual(w * h, GridReachability.Count(reached));
            Assert.IsTrue(GridReachability.IsReached(reached, w, 4, 3));
        }

        [Test]
        public void WallColumn_SplitsTheGrid()
        {
            int w = 5, h = 3;
            var g = AllWalkable(w, h);
            for (int y = 0; y < h; y++) g[y * w + 2] = false; // full wall at column x=2
            var reached = GridReachability.Flood(w, h, g, null, 0, 0, 100f);
            // Left of the wall reachable, right of it not.
            Assert.IsTrue(GridReachability.IsReached(reached, w, 1, 0));
            Assert.IsFalse(GridReachability.IsReached(reached, w, 3, 0), "the far side must be walled off");
            Assert.IsFalse(GridReachability.IsReached(reached, w, 4, 2));
        }

        [Test]
        public void HeightStep_BreaksConnectivity_WhenOverMaxStep()
        {
            int w = 3, h = 1;
            var g = AllWalkable(w, h);
            var heights = new float[] { 0f, 0f, 10f }; // a 10m cliff into cell (2,0)
            var reached = GridReachability.Flood(w, h, g, heights, 0, 0, maxStep: 3f);
            Assert.IsTrue(GridReachability.IsReached(reached, w, 1, 0));
            Assert.IsFalse(GridReachability.IsReached(reached, w, 2, 0), "a 10m step > 3m maxStep is impassable");
        }

        [Test]
        public void GentleGrade_StaysConnected_UnderMaxStep()
        {
            int w = 4, h = 1;
            var g = AllWalkable(w, h);
            var heights = new float[] { 0f, 1f, 2f, 3f }; // 1m steps
            var reached = GridReachability.Flood(w, h, g, heights, 0, 0, maxStep: 2f);
            Assert.AreEqual(4, GridReachability.Count(reached), "1m grade steps are all traversable");
        }

        [Test]
        public void StartOnWall_NothingReachable()
        {
            int w = 3, h = 3;
            var g = AllWalkable(w, h);
            g[0] = false; // start cell is a wall
            var reached = GridReachability.Flood(w, h, g, null, 0, 0, 100f);
            Assert.AreEqual(0, GridReachability.Count(reached));
        }

        [Test]
        public void OutOfBoundsStart_ReturnsEmpty_NotCrash()
        {
            int w = 3, h = 3;
            var reached = GridReachability.Flood(w, h, AllWalkable(w, h), null, 9, 9, 100f);
            Assert.AreEqual(0, GridReachability.Count(reached));
        }

        [Test]
        public void MalformedInput_IsSafe()
        {
            Assert.AreEqual(0, GridReachability.Count(GridReachability.Flood(0, 0, null, null, 0, 0, 1f)));
            Assert.AreEqual(0, GridReachability.Count(GridReachability.Flood(3, 3, new bool[2], null, 0, 0, 1f)));
            Assert.IsFalse(GridReachability.IsReached(null, 3, 0, 0));
        }

        [Test]
        public void Deterministic_SamePocketEveryRun()
        {
            // An L-shaped corridor: reachable count is fixed and repeatable.
            int w = 4, h = 4;
            var g = new bool[w * h]; // start all false
            // carve an L: row 0 across, then column 0 down
            for (int x = 0; x < w; x++) g[0 * w + x] = true;
            for (int y = 0; y < h; y++) g[y * w + 0] = true;
            var a = GridReachability.Flood(w, h, g, null, 0, 0, 100f);
            var b = GridReachability.Flood(w, h, g, null, 0, 0, 100f);
            Assert.AreEqual(GridReachability.Count(a), GridReachability.Count(b));
            Assert.AreEqual(w + h - 1, GridReachability.Count(a), "L corridor = w + h - 1 cells (corner shared)");
        }
    }
}

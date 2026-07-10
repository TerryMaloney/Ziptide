using NUnit.Framework;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Destruction v2's structural rule (Terry: "chunks that break off in a way that makes sense"):
    /// support = a 4-connected path of intact bricks to the bottom row. Arches hold, severed slabs
    /// avalanche, hanging islands fall, and collapsed bricks regen with the wall. Pure/headless.
    /// </summary>
    public class WallCollapseTests
    {
        private static WallState Wall(int cols, int rows) => new WallState(cols, rows, brickHits: 1);

        [Test]
        public void ArchOverAHole_Holds()
        {
            var w = Wall(6, 5);
            w.HitBrick(2, 1, 0.0);
            w.HitBrick(3, 1, 0.0);
            var fell = w.CollapseUnsupported(0.0);
            Assert.IsEmpty(fell, "bricks above a hole still reach the floor around its sides — the arch holds");
            Assert.AreEqual(2, w.BrokenCount);
        }

        [Test]
        public void SeveringAFullBand_DropsTheWholeSlabAbove()
        {
            var w = Wall(6, 5);
            for (int c = 0; c < 6; c++) w.HitBrick(c, 1, 0.0);
            var fell = w.CollapseUnsupported(0.0);
            Assert.AreEqual(18, fell.Count, "rows 2..4 (6x3) lost their path to the floor");
            foreach (var (col, row) in fell)
                Assert.GreaterOrEqual(row, 2, "only bricks ABOVE the cut fall");
            Assert.AreEqual(6 + 18, w.BrokenCount);
        }

        [Test]
        public void HangingIsland_Falls()
        {
            var w = Wall(4, 4);
            w.HitBrick(1, 3, 0.0);
            w.HitBrick(0, 2, 0.0);
            var fell = w.CollapseUnsupported(0.0);
            Assert.AreEqual(1, fell.Count);
            Assert.AreEqual((0, 3), fell[0], "the isolated corner brick has nothing holding it");
        }

        [Test]
        public void CuttingTheEntireBottomRow_BringsTheWallDown()
        {
            var w = Wall(4, 4);
            for (int c = 0; c < 4; c++) w.HitBrick(c, 0, 0.0);
            var fell = w.CollapseUnsupported(0.0);
            Assert.AreEqual(12, fell.Count, "no anchor left — everything above the floor lets go");
        }

        [Test]
        public void CollapsedBricks_RegenerateWithTheWall()
        {
            var w = new WallState(6, 5, brickHits: 1, regenSeconds: 10.0);
            for (int c = 0; c < 6; c++) w.HitBrick(c, 1, 0.0);
            w.CollapseUnsupported(0.0);
            Assert.Greater(w.BrokenCount, 6);

            w.Tick(9.9);
            Assert.Greater(w.BrokenCount, 0, "not yet — the collapse refreshed the regen clock");
            w.Tick(10.0);
            Assert.AreEqual(0, w.BrokenCount, "the whole wall heals, collapsed bricks included");
            Assert.IsFalse(w.AnyDamaged);
        }

        [Test]
        public void IntactWall_NeverCollapses()
        {
            var w = Wall(5, 4);
            Assert.IsEmpty(w.CollapseUnsupported(0.0));
            Assert.AreEqual(0, w.BrokenCount);
        }
    }
}

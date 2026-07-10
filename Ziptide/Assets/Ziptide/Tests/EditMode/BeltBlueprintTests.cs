using NUnit.Framework;
using Ziptide.Content.Automation;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1k — pins the blueprint contracts: capture takes the whole connected
    /// player-buildable line (and ONLY that — ports/sinks never clone), normalization + seed
    /// anchoring are exact, captures are deterministic, oversized components refuse rather than
    /// half-capture, and stamping is all-or-nothing onto empty ground with directions preserved
    /// (a stamped copy FLOWS like the original).
    /// </summary>
    public class BeltBlueprintTests
    {
        [Test]
        public void Capture_TakesTheConnectedLine_NotTheMachines()
        {
            var b = new BeltLattice(6, 2);
            b.PlacePort(0, 0, BeltDir.East);       // machine-bound — never cloned
            b.PlaceBelt(1, 0, BeltDir.East);
            b.PlaceBelt(2, 0, BeltDir.East);
            b.PlaceSplitter(3, 0, BeltDir.East);
            b.PlaceSink(4, 0);                     // machine-bound — never cloned
            b.PlaceBelt(0, 1, BeltDir.North);      // NOT 4-connected to the line — separate island

            var bp = BeltBlueprint.Capture(b, 2, 0);
            Assert.IsNotNull(bp);
            Assert.AreEqual(3, bp.Count, "2 belts + 1 splitter; port/sink/island excluded");
            Assert.AreEqual(1, bp.SeedDx, "seed (2,0) sits 1 cell into the normalized footprint");
            Assert.AreEqual(0, bp.SeedDz);
        }

        [Test]
        public void Capture_FromEmptyOrMachine_ReturnsNull()
        {
            var b = new BeltLattice(3, 1);
            b.PlaceSink(1, 0);
            Assert.IsNull(BeltBlueprint.Capture(b, 0, 0), "empty ground captures nothing");
            Assert.IsNull(BeltBlueprint.Capture(b, 1, 0), "a sink is not a line");
        }

        [Test]
        public void Capture_IsDeterministic()
        {
            var b = new BeltLattice(4, 4);
            b.PlaceBelt(1, 1, BeltDir.East);
            b.PlaceBelt(2, 1, BeltDir.North);
            b.PlaceBelt(2, 2, BeltDir.West);
            var p = BeltBlueprint.Capture(b, 1, 1);
            var q = BeltBlueprint.Capture(b, 1, 1);
            Assert.AreEqual(p.Count, q.Count);
            for (int i = 0; i < p.Count; i++)
            {
                Assert.AreEqual(p.Cells[i].Dx, q.Cells[i].Dx);
                Assert.AreEqual(p.Cells[i].Dz, q.Cells[i].Dz);
                Assert.AreEqual(p.Cells[i].Dir, q.Cells[i].Dir);
            }
        }

        [Test]
        public void Capture_OversizedComponent_RefusesWholesale()
        {
            var b = new BeltLattice(10, 1);
            for (int x = 0; x < 10; x++) b.PlaceBelt(x, 0, BeltDir.East);
            Assert.IsNull(BeltBlueprint.Capture(b, 0, 0, maxCells: 5),
                "never a partial capture — the player must trust what the wand holds");
            Assert.IsNotNull(BeltBlueprint.Capture(b, 0, 0, maxCells: 10));
        }

        [Test]
        public void Stamp_IsAllOrNothing_AndPreservesFlow()
        {
            var src = new BeltLattice(4, 1);
            b4(src);
            var bp = BeltBlueprint.Capture(src, 0, 0);

            var dst = new BeltLattice(8, 3);
            dst.PlaceSink(6, 1); // occupies the last cell of a (3,1)-anchored footprint

            // Blocked footprint refuses entirely.
            Assert.IsFalse(bp.CanStampAt(dst, 3, 1), "cell (6,1) is occupied — the whole stamp refuses");
            Assert.AreEqual(0, bp.StampInto(dst, 3, 1).Count);
            Assert.AreEqual(CellKind.Empty, dst.KindAt(3, 1), "nothing half-landed");

            // Clear ground stamps whole, directions preserved, and the copy FLOWS.
            var placed = bp.StampInto(dst, 1, 1);
            Assert.AreEqual(4, placed.Count);
            Assert.AreEqual(CellKind.Belt, dst.KindAt(1, 1));
            Assert.AreEqual(BeltDir.East, dst.DirAt(4, 1));
            Assert.IsTrue(dst.HandPlaceItem(1, 1, "scrap"));
            for (float t = 0f; t < 5f; t += 0.05f) dst.Tick(0.05f);
            Assert.AreEqual(4, dst.Items[0].X, "the item rode the stamped copy to its end");
        }

        private static void b4(BeltLattice b)
        {
            for (int x = 0; x < 4; x++) b.PlaceBelt(x, 0, BeltDir.East);
        }

        [Test]
        public void Stamp_SeedAnchoring_LandsWhereTheHandPoints()
        {
            var src = new BeltLattice(5, 1);
            for (int x = 0; x < 5; x++) src.PlaceBelt(x, 0, BeltDir.East);
            var bp = BeltBlueprint.Capture(src, 3, 0); // seed 3 cells into the line
            Assert.AreEqual(3, bp.SeedDx);

            var dst = new BeltLattice(10, 2);
            // Anchor the SEED at (5,1): the footprint origin must land at (5-3, 1) = (2,1).
            Assert.IsTrue(bp.CanStampAt(dst, 5 - bp.SeedDx, 1 - bp.SeedDz));
            var placed = bp.StampInto(dst, 5 - bp.SeedDx, 1 - bp.SeedDz);
            Assert.AreEqual(5, placed.Count);
            Assert.AreEqual(CellKind.Belt, dst.KindAt(2, 1), "west end");
            Assert.AreEqual(CellKind.Belt, dst.KindAt(5, 1), "the seed cell is under the hand");
            Assert.AreEqual(CellKind.Belt, dst.KindAt(6, 1), "east end");
            Assert.AreEqual(CellKind.Empty, dst.KindAt(7, 1));
        }
    }
}

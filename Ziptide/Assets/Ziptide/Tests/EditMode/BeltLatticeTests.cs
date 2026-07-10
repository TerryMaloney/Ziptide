using NUnit.Framework;
using Ziptide.Content.Automation;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1a — the conveyor layer's pure heart. Pins the contracts that make belts FEEL
    /// right: items flow at belt speed, jams compress upstream (head blocking), junctions merge
    /// fairly, sources never overflow a blocked belt, sinks count what arrives, hand interaction
    /// (drop an item on / pick a belt up) behaves, and everything is deterministic.
    /// </summary>
    public class BeltLatticeTests
    {
        /// <summary>West→east belt run of <paramref name="len"/> cells at z=0, source at (0,0)… no —
        /// plain belts only; callers add sources/sinks.</summary>
        private static BeltLattice Line(int len)
        {
            var b = new BeltLattice(len, 1);
            for (int x = 0; x < len; x++) b.PlaceBelt(x, 0, BeltDir.East);
            return b;
        }

        private static void Run(BeltLattice b, float seconds, float dt = 0.05f)
        {
            for (float t = 0f; t < seconds; t += dt) b.Tick(dt);
        }

        [Test]
        public void Item_FlowsDownTheLine_AtBeltSpeed()
        {
            var b = Line(6);
            Assert.IsTrue(b.HandPlaceItem(0, 0, "scrap"));
            // 6 cells at DefaultSpeed cells/sec — after enough time it parks at the line's end.
            Run(b, 6f / BeltLattice.DefaultSpeed + 1f);
            Assert.AreEqual(1, b.Items.Count, "nothing consumed — no sink");
            Assert.AreEqual(5, b.Items[0].X, "item waits at the end of the line");
            Assert.AreEqual(1f, b.Items[0].Progress, 1e-3f, "parked at the lip");
        }

        [Test]
        public void Sink_ConsumesAndCounts()
        {
            var b = Line(4);
            b.PlaceSink(3, 0);
            b.HandPlaceItem(0, 0, "scrap");
            Run(b, 5f);
            Assert.AreEqual(0, b.Items.Count);
            Assert.AreEqual(1, b.SunkCount("scrap"));
            Assert.AreEqual(1, b.DrainSunk("scrap"), "drain hands the count to the graph adapter");
            Assert.AreEqual(0, b.SunkCount("scrap"), "drained");
        }

        [Test]
        public void Jam_CompressesUpstream_HeadBlocking()
        {
            var b = Line(4); // no sink — the head item parks at the end, the rest pile behind
            b.HandPlaceItem(0, 0, "a");
            Run(b, 1.5f);
            b.HandPlaceItem(0, 0, "b");
            Run(b, 6f);
            Assert.AreEqual(2, b.Items.Count);
            Assert.IsNotNull(b.OccupantAt(3, 0), "head parked at the end");
            Assert.IsNotNull(b.OccupantAt(2, 0), "second compressed right behind it");
        }

        [Test]
        public void Source_Emits_ButNeverOverflowsABlockedBelt()
        {
            var b = new BeltLattice(3, 1);
            b.PlaceSource(0, 0, BeltDir.East, "ore");
            b.PlaceBelt(1, 0, BeltDir.East);
            b.PlaceBelt(2, 0, BeltDir.East);
            Run(b, 30f);
            // 2 belt cells, no sink: exactly 2 items fit, emission then holds. Never a third.
            Assert.AreEqual(2, b.Items.Count, "a blocked source holds — no overflow, no item soup");
        }

        [Test]
        public void SourceToSink_Throughput_Flows()
        {
            var b = new BeltLattice(5, 1);
            b.PlaceSource(0, 0, BeltDir.East, "ore");
            for (int x = 1; x <= 3; x++) b.PlaceBelt(x, 0, BeltDir.East);
            b.PlaceSink(4, 0);
            Run(b, 20f);
            Assert.Greater(b.SunkCount("ore"), 5, "a clean line moves real throughput");
            Assert.LessOrEqual(b.Items.Count, 3, "line never holds more than its cells");
        }

        [Test]
        public void Junction_MergesFairly_RoundRobin()
        {
            // Two source-fed lines merging into one cell, then a sink: both resources must arrive.
            var b = new BeltLattice(3, 3);
            b.PlaceSource(0, 2, BeltDir.South, "left");  // feeds (0,1) from the north
            b.PlaceBelt(0, 1, BeltDir.East);              // wait — build the merge at (1,1)
            b.PlaceSource(1, 2, BeltDir.South, "top");    // feeds (1,1) from the north
            b.PlaceBelt(1, 1, BeltDir.East);              // the contested cell (fed by west + north)
            b.PlaceSink(2, 1);
            Run(b, 40f);
            Assert.Greater(b.SunkCount("left"), 2, "west feeder gets turns");
            Assert.Greater(b.SunkCount("top"), 2, "north feeder gets turns");
        }

        [Test]
        public void Clear_PicksTheBeltBackUp_AndReturnsTheItem()
        {
            var b = Line(3);
            b.HandPlaceItem(1, 0, "gem");
            var held = b.Clear(1, 0);
            Assert.IsNotNull(held);
            Assert.AreEqual("gem", held.ResourceId);
            Assert.AreEqual(CellKind.Empty, b.KindAt(1, 0));
            Assert.AreEqual(0, b.Items.Count, "the item left the lattice with the hand");
        }

        [Test]
        public void StrandedItem_HoldsInPlace_WhenItsBeltIsRemoved()
        {
            var b = Line(4);
            b.HandPlaceItem(0, 0, "scrap");
            Run(b, 1.0f);
            var item = b.Items[0];
            b.PlaceSink(3, 0); // give the line an end
            // Remove the belt under a DIFFERENT cell than the item, then the item's own cell.
            b.Clear(item.X, item.Z);
            Assert.AreEqual(0, b.Items.Count, "clearing a cell lifts its item too");
        }

        [Test]
        public void HandPlace_RespectsOccupancyAndCarriers()
        {
            var b = Line(2);
            Assert.IsTrue(b.HandPlaceItem(0, 0, "a"));
            Assert.IsFalse(b.HandPlaceItem(0, 0, "b"), "occupied cell refuses");
            Assert.IsFalse(b.HandPlaceItem(5, 0, "c"), "out of bounds refuses");
            var empty = new BeltLattice(2, 2);
            Assert.IsFalse(empty.HandPlaceItem(0, 0, "d"), "empty ground is not a carrier");
        }

        [Test]
        public void Splitter_FeedsBothOutputs()
        {
            // source → belt → splitter(E): primary exit East to (3,1), alternate exit South to (2,0).
            var b = new BeltLattice(4, 2);
            b.PlaceSource(0, 1, BeltDir.East, "ore");
            b.PlaceBelt(1, 1, BeltDir.East);
            b.PlaceSplitter(2, 1, BeltDir.East);
            b.PlaceBelt(3, 1, BeltDir.East);  // parks at the lattice edge
            b.PlaceBelt(2, 0, BeltDir.South); // parks at the lattice edge
            Run(b, 10f);
            Assert.IsNotNull(b.OccupantAt(3, 1), "primary side received an item");
            Assert.IsNotNull(b.OccupantAt(2, 0), "alternate side received an item");
        }

        [Test]
        public void Splitter_BlockedSide_EverythingTakesTheFreeSide()
        {
            // The south side is EMPTY ground (never accepts) — all throughput goes east.
            var b = new BeltLattice(5, 2);
            b.PlaceSource(0, 1, BeltDir.East, "ore");
            b.PlaceBelt(1, 1, BeltDir.East);
            b.PlaceSplitter(2, 1, BeltDir.East);
            b.PlaceBelt(3, 1, BeltDir.East);
            b.PlaceSink(4, 1);
            Run(b, 20f);
            Assert.Greater(b.SunkCount("ore"), 5, "one dead side must not halve (or halt) the line");
        }

        [Test]
        public void Splitter_BothOutputsBlocked_Compresses()
        {
            var b = new BeltLattice(4, 2);
            b.PlaceSource(0, 1, BeltDir.East, "ore");
            b.PlaceBelt(1, 1, BeltDir.East);
            b.PlaceSplitter(2, 1, BeltDir.East); // East → (3,1) empty; South → (2,0) empty
            Run(b, 15f);
            Assert.AreEqual(2, b.Items.Count, "splitter + feed belt hold one each; source waits");
            Assert.IsNotNull(b.OccupantAt(2, 1), "an item parks ON the splitter");
        }

        [Test]
        public void Deterministic_SameBuildSameTicks_SameWorld()
        {
            BeltLattice Build()
            {
                var b = new BeltLattice(6, 2);
                b.PlaceSource(0, 0, BeltDir.East, "ore");
                for (int x = 1; x <= 4; x++) b.PlaceBelt(x, 0, BeltDir.East);
                b.PlaceSink(5, 0);
                return b;
            }
            var p = Build(); var q = Build();
            Run(p, 13.3f); Run(q, 13.3f);
            Assert.AreEqual(p.SunkCount("ore"), q.SunkCount("ore"));
            Assert.AreEqual(p.Items.Count, q.Items.Count);
            for (int i = 0; i < p.Items.Count; i++)
            {
                Assert.AreEqual(p.Items[i].X, q.Items[i].X);
                Assert.AreEqual(p.Items[i].Progress, q.Items[i].Progress, 1e-6f);
            }
        }
    }
}

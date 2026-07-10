using NUnit.Framework;
using Ziptide.Content.Automation;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 4.1h — pins conductor mode's pure contracts: a route traces exactly the way an
    /// item rides (corners followed, splitter takes the primary line, sink is the terminus, lattice
    /// edge parks, loops terminate), and the glide is deterministic at the ore's-eye pace with one
    /// boundary crossing per cell lip (the haptic-tick contract).
    /// </summary>
    public class BeltRouteTests
    {
        [Test]
        public void Trace_StraightLine_SourceToSink_IncludesTerminus()
        {
            var b = new BeltLattice(5, 1);
            b.PlaceSource(0, 0, BeltDir.East, "ore");
            for (int x = 1; x <= 3; x++) b.PlaceBelt(x, 0, BeltDir.East);
            b.PlaceSink(4, 0);
            var path = BeltRoute.Trace(b, 0, 0);
            Assert.AreEqual(5, path.Count, "source + 3 belts + the sink terminus");
            Assert.AreEqual(4, path[4].X);
        }

        [Test]
        public void Trace_FollowsCorners()
        {
            // East 2 cells, then a corner north, then east again — the sandbox upper path shape.
            var b = new BeltLattice(4, 3);
            b.PlaceBelt(0, 0, BeltDir.East);
            b.PlaceBelt(1, 0, BeltDir.North);
            b.PlaceBelt(1, 1, BeltDir.East);
            b.PlaceSink(2, 1);
            var path = BeltRoute.Trace(b, 0, 0);
            Assert.AreEqual(4, path.Count);
            Assert.AreEqual(1, path[1].X);
            Assert.AreEqual(1, path[2].Z, "turned the corner");
            Assert.AreEqual(2, path[3].X);
        }

        [Test]
        public void Trace_Splitter_FollowsThePrimaryLine()
        {
            var b = new BeltLattice(4, 2);
            b.PlaceBelt(0, 1, BeltDir.East);
            b.PlaceSplitter(1, 1, BeltDir.East);  // primary east; alternate south
            b.PlaceBelt(2, 1, BeltDir.East);
            b.PlaceBelt(1, 0, BeltDir.East);      // the fork — NOT the conductor's route
            var path = BeltRoute.Trace(b, 0, 1);
            Assert.AreEqual(3, path.Count);
            foreach (var c in path) Assert.AreEqual(1, c.Z, "the ride stays on the primary line");
        }

        [Test]
        public void Trace_EndsAtLatticeEdge_AndAtEmptyGround()
        {
            var b = new BeltLattice(3, 1);
            b.PlaceBelt(1, 0, BeltDir.East);
            b.PlaceBelt(2, 0, BeltDir.East); // exits the lattice — park at the lip
            Assert.AreEqual(2, BeltRoute.Trace(b, 1, 0).Count);

            var c = new BeltLattice(4, 1);
            c.PlaceBelt(0, 0, BeltDir.East); // (1,0) is empty ground
            Assert.AreEqual(1, BeltRoute.Trace(c, 0, 0).Count);
            Assert.AreEqual(0, BeltRoute.Trace(c, 3, 0).Count, "starting on empty ground = no route");
        }

        [Test]
        public void Trace_Loop_TerminatesWithEachCellOnce()
        {
            var b = new BeltLattice(2, 2);
            b.PlaceBelt(0, 0, BeltDir.East);
            b.PlaceBelt(1, 0, BeltDir.North);
            b.PlaceBelt(1, 1, BeltDir.West);
            b.PlaceBelt(0, 1, BeltDir.South);
            var path = BeltRoute.Trace(b, 0, 0);
            Assert.AreEqual(4, path.Count, "one full lap, no infinite ride");
        }

        [Test]
        public void Ride_GlidesAtTheOresPace_AndArrives()
        {
            var b = new BeltLattice(5, 1);
            for (int x = 0; x < 5; x++) b.PlaceBelt(x, 0, BeltDir.East);
            var ride = new ConductorRide(BeltRoute.Trace(b, 0, 0));
            Assert.AreEqual(4f, ride.TotalCells);
            Assert.IsFalse(ride.Arrived);

            // Half the distance at DefaultSpeed cells/sec.
            float half = 2f / BeltLattice.DefaultSpeed;
            for (float t = 0f; t < half; t += 0.01f) ride.Step(0.01f);
            ride.GridPosition(out float gx, out float gz);
            Assert.AreEqual(2f, gx, 0.1f, "midway down the line");
            Assert.AreEqual(0f, gz);

            for (int i = 0; i < 1000 && !ride.Arrived; i++) ride.Step(0.05f);
            Assert.IsTrue(ride.Arrived);
            ride.GridPosition(out gx, out gz);
            Assert.AreEqual(4f, gx, 1e-3f, "parked at the last cell, never past it");
        }

        [Test]
        public void Ride_CountsOneBoundaryCrossingPerCellLip()
        {
            var b = new BeltLattice(4, 1);
            for (int x = 0; x < 4; x++) b.PlaceBelt(x, 0, BeltDir.East);
            var ride = new ConductorRide(BeltRoute.Trace(b, 0, 0));
            for (int i = 0; i < 1000 && !ride.Arrived; i++) ride.Step(0.02f);
            Assert.AreEqual(3, ride.BoundaryCrossings, "3 lips on a 4-cell line — the haptic ticks");
        }

        [Test]
        public void Ride_EmptyOrSingleCellPath_IsInstantlyArrived()
        {
            Assert.IsTrue(new ConductorRide(null).Arrived);
            var b = new BeltLattice(2, 1);
            b.PlaceBelt(0, 0, BeltDir.East);
            var one = BeltRoute.Trace(b, 0, 0);
            Assert.AreEqual(1, one.Count);
            Assert.IsTrue(new ConductorRide(one).Arrived, "nowhere to go = no ride");
        }
    }
}

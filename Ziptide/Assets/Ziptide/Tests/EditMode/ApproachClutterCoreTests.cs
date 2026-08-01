using UnityEngine;
using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Junk on the quay is decoration. Junk IN THE WALKING LANE is a soft-lock on the first walk of
    /// the game, and it is the kind that hides: a VR player cannot see their own feet, so a knee-high
    /// crate on the line is invisible until it stops them. Every one of these pins that lane open.
    ///
    /// The span below is ToxicCity's real berth (centre z −48, depth 16 m): amidships to the landward
    /// edge, which is the deck the player crosses leaving the ramp. Using the SHIPPED numbers is not
    /// incidental — the first version of the author derived its span from two edges whose names both
    /// read as "the walk" and which are one metre apart, and a test on invented coordinates would have
    /// passed happily while the bake dumped nine crates in a heap on the bridge.
    /// </summary>
    public class ApproachClutterCoreTests
    {
        private const float CentreX = 0f;
        private const float FromZ = -52f;   // berth centre − depth/4, beside the hull
        private const float ToZ = -40f;     // the berth's landward edge

        private static System.Collections.Generic.List<ClutterPiece> Walk()
            => ApproachClutterCore.Place(CentreX, FromZ, ToZ);

        [Test]
        public void NothingEverStandsInTheWalkingLane()
        {
            foreach (var p in Walk())
                Assert.IsFalse(
                    ApproachClutterCore.BlocksTheWalk(p.Position, CentreX, ApproachClutterCore.Footprint(p).x),
                    p.Kind + " at x=" + p.Position.x + " (footprint "
                    + ApproachClutterCore.Footprint(p).x + ") reaches into the corridor — that is a "
                    + "crate the player walks into and cannot see.");
        }

        [Test]
        public void EverythingStaysCloseEnoughToBeForeground()
        {
            // Too far out and the piece has stopped doing its job: parallax comes from things you
            // pass CLOSE to. A crate 8 m away is midground and the walk is flat again.
            foreach (var p in Walk())
                Assert.LessOrEqual(
                    Mathf.Abs(p.Position.x - CentreX) + ApproachClutterCore.Footprint(p).x * 0.5f,
                    ApproachClutterCore.MaxOffset + 0.001f);
        }

        [Test]
        public void BothSidesOfTheWalkGetDressed()
        {
            int left = 0, right = 0;
            foreach (var p in Walk())
                if (p.Position.x < CentreX) left++; else right++;

            Assert.Greater(left, 0, "a hedge down one side is not a dressed walk");
            Assert.Greater(right, 0);
        }

        [Test]
        public void TheWalkIsLongEnoughToBeAWalk()
        {
            // The one this file exists for. Nine pieces spread over a one-metre span is a heap, and a
            // heap is what the first version built — the arithmetic was never checked against the
            // shipped numbers because both variable names sounded correct.
            Assert.Greater(Mathf.Abs(ToZ - FromZ), 6f,
                "under six metres and the 'walk' is a doorway; spreading nine props along it makes a pile");
        }

        [Test]
        public void NothingSitsWhereTheShipIs()
        {
            // The corridor is not a comfort number, it is the hull. Anything inside it is not on the
            // deck at all — it is inside your own ship.
            const float ShipHalfBeam = 2.5f;
            foreach (var p in Walk())
                Assert.Greater(Mathf.Abs(p.Position.x - CentreX) - ApproachClutterCore.Footprint(p).x * 0.5f,
                    ShipHalfBeam, p.Kind + " is inside the ship's footprint");
        }

        [Test]
        public void NothingReachesTheGantryColumnsOrTheDeckEdge()
        {
            // Columns stand at ±9.4 m and the deck ends at ±10 m. Clutter that drifts out there stops
            // being foreground and starts being something you clip through.
            const float ColumnX = 9.4f;
            foreach (var p in Walk())
                Assert.Less(Mathf.Abs(p.Position.x - CentreX) + ApproachClutterCore.Footprint(p).x * 0.5f,
                    ColumnX - 1f);
        }

        [Test]
        public void NothingIsDumpedAtEitherEndOfTheWalk()
        {
            // The ramp foot and the district mouth are exactly where a player turns, and a turn is the
            // worst possible place to put an obstacle.
            foreach (var p in Walk())
            {
                Assert.Greater(p.Position.z, FromZ + 0.5f);
                Assert.Less(p.Position.z, ToZ - 0.5f);
            }
        }

        [Test]
        public void ThePlacementIsDeterministic_SoRebakingChangesNothing()
        {
            var a = Walk();
            var b = Walk();
            Assert.AreEqual(a.Count, b.Count);
            for (int i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i].Position.x, b[i].Position.x, 0.0001f);
                Assert.AreEqual(a[i].Position.z, b[i].Position.z, 0.0001f);
                Assert.AreEqual(a[i].Size, b[i].Size, 0.0001f);
            }
        }

        [Test]
        public void AllThreeKindsShowUp_BecauseNineOfTheSameBoxIsATexture()
        {
            bool crate = false, spool = false, box = false;
            foreach (var p in Walk())
            {
                crate |= p.Kind == ClutterKind.Crate;
                spool |= p.Kind == ClutterKind.CableSpool;
                box |= p.Kind == ClutterKind.Toolbox;
            }
            Assert.IsTrue(crate && spool && box);
        }

        [Test]
        public void APieceCountThatCannotQuietlyBecomeAHundred()
        {
            Assert.AreEqual(ApproachClutterCore.PieceCount, Walk().Count);
            Assert.LessOrEqual(ApproachClutterCore.PieceCount, 16,
                "foreground dressing is cheap right up until it is not — this is a Quest");
        }

        [Test]
        public void TheTeachingCrateIsReachableWithoutBeingInTheWay()
        {
            Vector3 c = ApproachClutterCore.TeachingCratePosition(CentreX, FromZ, ToZ);
            Assert.IsFalse(ApproachClutterCore.BlocksTheWalk(c, CentreX, ApproachClutterCore.TeachingCrateSize),
                "the free lesson must not also be a trip hazard");
            Assert.LessOrEqual(Mathf.Abs(c.x - CentreX), ApproachClutterCore.MaxOffset,
                "a lesson you have to leave the walk to find is not a lesson");
            Assert.Greater(c.y, 0f, "it sits ON the deck, not in it");
        }

        [Test]
        public void TheTeachingCrateComesEarly_OrItTeachesNothing()
        {
            // A grab lesson placed after the player has already walked the whole quay has been
            // taught by the walk instead, which is to say not at all.
            Vector3 c = ApproachClutterCore.TeachingCratePosition(CentreX, FromZ, ToZ);
            float t = Mathf.InverseLerp(FromZ, ToZ, c.z);
            Assert.Less(t, 0.4f);
        }

        [Test]
        public void ADegenerateWalkPlacesNothingRatherThanThrowing()
        {
            Assert.AreEqual(0, ApproachClutterCore.Place(0f, -30f, -30f).Count);
        }

        [Test]
        public void TheFootprintIsWhatTheCorridorLawMeasures()
        {
            // The bug this pins: the author stretches a toolbox 1.5x across the walk. Measuring the
            // nominal size instead of the built shape put half of it back in the lane, and nothing
            // would have said so until somebody walked into an invisible box in a headset.
            Assert.AreEqual(0.9f, ApproachClutterCore.Footprint(ClutterKind.Toolbox, 0.6f).x, 0.001f);
            Assert.AreEqual(0.6f, ApproachClutterCore.Footprint(ClutterKind.Crate, 0.6f).x, 0.001f);
            Assert.Less(ApproachClutterCore.Footprint(ClutterKind.Toolbox, 0.6f).y,
                ApproachClutterCore.Footprint(ClutterKind.Crate, 0.6f).y,
                "a toolbox that is as tall as it is long is a crate with a different name");
        }
    }
}

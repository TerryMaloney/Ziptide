using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Pins the hangar walk's arithmetic (LEVEL1_SPATIAL_SCRIPT §3). The failure this exists to
    /// prevent is not a crash: it is five coplanar decks quietly overlapping and z-fighting the
    /// length of the quay, which reads on device as a broken harbour and costs a headset session
    /// to diagnose.
    /// </summary>
    public class QuayBerthCoreTests
    {
        // The committed ToxicCity shipyard: 20 x 16 deck centred at (0, 0, -48).
        private static readonly Vector3 Berth = new Vector3(0f, 0f, -48f);
        private const float BerthWidth = 20f;

        [Test]
        public void FiveBerths_AreAuthored_WestOfTheShip()
        {
            var pads = QuayBerthCore.PadCentres(Berth, BerthWidth);
            Assert.AreEqual(QuayBerthCore.BerthCount, pads.Count);
            foreach (var p in pads)
            {
                Assert.Less(p.x, Berth.x, "every empty berth lies west of berth six");
                Assert.AreEqual(Berth.z, p.z, 0.001f, "the quay is one straight dockfront");
            }
        }

        [Test]
        public void Spacing_IsTheScriptsFourteenMetres()
        {
            var pads = QuayBerthCore.PadCentres(Berth, BerthWidth);
            for (int i = 1; i < pads.Count; i++)
                Assert.AreEqual(QuayBerthCore.Spacing, pads[i - 1].x - pads[i].x, 0.001f);
        }

        [Test]
        public void Decks_NeverOverlapEachOther_OrTheShipsBerth()
        {
            Assert.IsTrue(QuayBerthCore.DecksAreDisjoint(Berth, BerthWidth));

            var pads = QuayBerthCore.PadCentres(Berth, BerthWidth);
            float firstEastEdge = pads[0].x + QuayBerthCore.PadWidth * 0.5f;
            Assert.LessOrEqual(firstEastEdge, Berth.x - BerthWidth * 0.5f,
                "the nearest empty deck must stop short of berth six's own deck");
        }

        [Test]
        public void TheGuard_ActuallyBites_WhenSpacingWouldOverlap()
        {
            // A berth wide enough that the 14 m rhythm would run the first deck into it. If this
            // ever returns true the invariant has become decorative.
            Assert.IsFalse(QuayBerthCore.DecksAreDisjoint(Berth, 400f));
        }

        [Test]
        public void BerthNumbers_CountDownWestward_FromFive()
        {
            Assert.AreEqual(5, QuayBerthCore.BerthNumberAt(0), "nearest berth six is berth five");
            Assert.AreEqual(1, QuayBerthCore.BerthNumberAt(QuayBerthCore.BerthCount - 1),
                "the far end of the walk is berth one");
        }

        [Test]
        public void TheWalk_FollowsTheBerth_WhenTheShipyardMoves()
        {
            // The whole reason positions are computed and not authored: move the shipyard and the
            // walk must move with it, still disjoint.
            var moved = new Vector3(35f, 0f, -70f);
            var pads = QuayBerthCore.PadCentres(moved, BerthWidth);
            Assert.AreEqual(moved.z, pads[0].z, 0.001f);
            Assert.Less(pads[0].x, moved.x);
            Assert.IsTrue(QuayBerthCore.DecksAreDisjoint(moved, BerthWidth));
        }
    }
}

using UnityEngine;
using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// A roof is how a space stops reading flat — you have to look up, and looking up is most of how a
    /// VR player feels size. But a roof over the WHOLE berth would seal off the sky, and the skyscape
    /// is the single thing this project is most committed to (`SKYSCAPE_DESIGN`: Terry named it as a
    /// reason the game exists, and W001's acid haze was built for it). A scale win that costs the
    /// horizon is not a win. That trade is what these hold.
    /// </summary>
    public class GantryRoofCoreTests
    {
        private const float BerthZ = -48f;
        private const float BerthDepth = 16f;

        [Test]
        public void TheSkyStaysOpenOverMostOfTheBerth()
        {
            GantryRoofCore.CoveredSpan(BerthZ, BerthDepth, out float from, out float to);
            float covered = to - from;

            Assert.IsTrue(GantryRoofCore.LeavesTheSkyOpen(covered, BerthDepth),
                "the gantry covers " + (covered / BerthDepth * 100f).ToString("F0")
                + "% of the berth — past the limit the horizon stops being the backdrop, and the "
                + "horizon is the point.");
            Assert.Less(covered, BerthDepth, "a roof edge to edge is a lid");
        }

        [Test]
        public void ARoofOverTheWholeBerthIsRejected()
        {
            Assert.IsFalse(GantryRoofCore.LeavesTheSkyOpen(BerthDepth, BerthDepth));
        }

        [Test]
        public void ItCoversTheLandwardEnd_WhereThePlayerWalks()
        {
            GantryRoofCore.CoveredSpan(BerthZ, BerthDepth, out float from, out float to);
            float landwardEdge = BerthZ + BerthDepth * 0.5f;
            float seawardEdge = BerthZ - BerthDepth * 0.5f;

            Assert.AreEqual(landwardEdge, to, 0.001f,
                "the covered end is the one you walk out of; the open end faces the water");
            Assert.Greater(from, seawardEdge,
                "the seaward end must be open — that is where the sky and the haze are");
        }

        [Test]
        public void TheOverheadIsWalkUnderableAndStillWorthLookingAt()
        {
            Assert.IsTrue(GantryRoofCore.ReadsAsOverhead(GantryRoofCore.TrussHeight));
            Assert.IsFalse(GantryRoofCore.ReadsAsOverhead(2.4f), "that is a ceiling you duck under");
            Assert.IsFalse(GantryRoofCore.ReadsAsOverhead(20f), "that high and it is sky, not structure");
        }

        [Test]
        public void LampsHangBelowTheTrusses_AndWellAboveARaisedHand()
        {
            Assert.Less(GantryRoofCore.LampHeight, GantryRoofCore.TrussHeight,
                "a lamp above its own truss is a lamp inside the roof");
            Assert.Greater(GantryRoofCore.LampHeight, 2.6f,
                "anything a standing player can headbutt is not decoration, it is an obstacle");
        }

        [Test]
        public void TrussesSpanTheCoveredLengthAndNoneEscapeIt()
        {
            GantryRoofCore.CoveredSpan(BerthZ, BerthDepth, out float from, out float to);

            Assert.AreEqual(from, GantryRoofCore.TrussZ(0, from, to), 0.001f);
            Assert.AreEqual(to, GantryRoofCore.TrussZ(GantryRoofCore.TrussCount - 1, from, to), 0.001f);

            for (int i = 0; i < GantryRoofCore.TrussCount; i++)
            {
                float z = GantryRoofCore.TrussZ(i, from, to);
                Assert.GreaterOrEqual(z, from - 0.001f);
                Assert.LessOrEqual(z, to + 0.001f);
            }
        }

        [Test]
        public void TrussesAreEvenlySpaced()
        {
            GantryRoofCore.CoveredSpan(BerthZ, BerthDepth, out float from, out float to);
            float a = GantryRoofCore.TrussZ(1, from, to) - GantryRoofCore.TrussZ(0, from, to);
            float b = GantryRoofCore.TrussZ(2, from, to) - GantryRoofCore.TrussZ(1, from, to);
            Assert.AreEqual(a, b, 0.001f);
        }

        [Test]
        public void ColumnsStandInsideTheBerthRatherThanOnItsEdge()
        {
            // On the edge they clip whatever the neighbouring pad builds; the inset is what keeps the
            // roof a structure on this berth rather than a fence between two.
            Assert.Greater(GantryRoofCore.EdgeInset, 0f);
            Assert.Less(GantryRoofCore.EdgeInset, 2f, "any further in and the roof no longer spans the ship");
        }

        [Test]
        public void ADegenerateBerthDoesNotDivideByZero()
        {
            Assert.IsTrue(GantryRoofCore.LeavesTheSkyOpen(0f, 0f));
        }
    }
}

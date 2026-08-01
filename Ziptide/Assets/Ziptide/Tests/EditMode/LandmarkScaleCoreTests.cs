using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ⚖ Terry, 2026-08-01: *"the hangar was pretty much just like a bunch of sort of box areas, not
    /// very good."* These pin the two numbers that decide whether a 16 m crane feels like sixteen
    /// metres, because neither one is visible in a diff and both drift the moment someone tweaks a
    /// prefab to "look better" on a monitor.
    /// </summary>
    public class LandmarkScaleCoreTests
    {
        // The crane as shipped, and as the hangar pass rebuilds it.
        private const float CraneHeight = 16f;
        private const float ShippedWidth = 2f;
        private const float FixedWidth = 4.5f;

        [Test]
        public void TheShippedCrane_IsAStick_AndTheWidenedOneIsNot()
        {
            Assert.IsTrue(LandmarkScaleCore.ReadsAsStick(CraneHeight, ShippedWidth),
                "16 m x 2 m is 8:1 — this is the defect the hangar pass exists to fix.");
            Assert.IsFalse(LandmarkScaleCore.ReadsAsStick(CraneHeight, FixedWidth),
                "16 m x 4.5 m must clear the slenderness bar, or the authored fix does not fix it.");
        }

        [Test]
        public void RungPitch_StaysInsideTheBandARealLadderUses()
        {
            // The whole trick depends on the player ALREADY knowing this distance. Outside 25-35 cm
            // the rungs stop being a known quantity and the ruler starts lying about the mast.
            Assert.GreaterOrEqual(LandmarkScaleCore.RungPitch, 0.25f);
            Assert.LessOrEqual(LandmarkScaleCore.RungPitch, 0.35f);
        }

        [Test]
        public void TheFirstRung_IsAStepNotAClimb()
        {
            Assert.LessOrEqual(LandmarkScaleCore.RungY(0), 0.5f);
        }

        [Test]
        public void RungsAreEvenlySpaced_BecauseAnUnevenLadderReadsAsNoise()
        {
            float a = LandmarkScaleCore.RungY(3) - LandmarkScaleCore.RungY(2);
            float b = LandmarkScaleCore.RungY(9) - LandmarkScaleCore.RungY(8);
            Assert.AreEqual(a, b, 0.0001f);
            Assert.AreEqual(LandmarkScaleCore.RungPitch, a, 0.0001f);
        }

        [Test]
        public void DetailStopsAtTheCeiling_SoATallTowerCostsTheSameAsAShortOne()
        {
            // The regression this prevents: rungs all the way up a 40 m tower — three pixels each and
            // a draw call each. A scale pass that blows the renderer budget is a net loss.
            int crane = LandmarkScaleCore.RungCount(CraneHeight);
            int oligarchTower = LandmarkScaleCore.RungCount(40f);
            Assert.AreEqual(crane, oligarchTower,
                "detail is capped by the ceiling, not by the object's height");
            Assert.LessOrEqual(LandmarkScaleCore.DetailPieceCount(40f, 6f), 40,
                "a single landmark must never be able to spend the district's whole budget");
        }

        [Test]
        public void NoRungRisesAboveTheDetailCeiling()
        {
            int n = LandmarkScaleCore.RungCount(CraneHeight);
            Assert.Greater(n, 0);
            Assert.LessOrEqual(LandmarkScaleCore.RungY(n - 1), LandmarkScaleCore.DetailCeiling + 0.001f);
        }

        [Test]
        public void SmallLandmarks_GetNoDetailAtAll()
        {
            // A 3 m block is furniture. Putting a ladder on it says "climb me" and nothing can.
            Assert.IsFalse(LandmarkScaleCore.ReadsAsStick(3f, 0.5f));
            Assert.AreEqual(0, LandmarkScaleCore.DetailPieceCount(3f, 0.5f));
        }

        [Test]
        public void TheWalkwayStaysWhereItCanBeSeen()
        {
            float w = LandmarkScaleCore.WalkwayY(CraneHeight);
            Assert.Greater(w, 2f, "a walkway at ankle height is not a walkway");
            Assert.LessOrEqual(w, LandmarkScaleCore.DetailCeiling);

            // On a 40 m tower the walkway must NOT ride up to 18 m, where it reads as a smudge.
            Assert.LessOrEqual(LandmarkScaleCore.WalkwayY(40f), LandmarkScaleCore.DetailCeiling);
        }

        [Test]
        public void MinimumReadableWidth_IsCapped_SoTallDoesNotMeanFat()
        {
            Assert.AreEqual(4f, LandmarkScaleCore.MinimumReadableWidth(CraneHeight), 0.001f);
            Assert.LessOrEqual(LandmarkScaleCore.MinimumReadableWidth(120f), LandmarkScaleCore.MaxUsefulWidth);
        }

        [Test]
        public void TheCabIsPersonSized_BecauseThatIsTheEntirePoint()
        {
            var cab = LandmarkScaleCore.CabSize(4.5f);
            Assert.GreaterOrEqual(cab.x, 1.2f);
            Assert.LessOrEqual(cab.x, 2.2f);
            Assert.AreEqual(2.1f, cab.y, 0.001f, "a cab a person cannot stand in tells the eye nothing");
            Assert.Greater(LandmarkScaleCore.CabY(CraneHeight), CraneHeight,
                "the cab sits on the mast, not inside it");
        }
    }
}

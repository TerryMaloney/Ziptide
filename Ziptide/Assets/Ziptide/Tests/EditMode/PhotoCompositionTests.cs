using NUnit.Framework;
using Ziptide.Content.Photo;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FIELD CAMERA — pins the composition scoring: a bare snapshot still counts, framing more of
    /// the Prospect skyscape scores strictly higher (the "teaches players to look" contract),
    /// rating thresholds are exact, scoring is deterministic, and out-of-range inputs are clamped.
    /// </summary>
    public class PhotoCompositionTests
    {
        private static PhotoSubjects Empty => new PhotoSubjects { SkyTier = 0, FramingCentered = 0f };

        [Test]
        public void BareSnapshot_StillScores_ButIsOnlyASnapshot()
        {
            var r = PhotoComposition.Evaluate(Empty);
            Assert.AreEqual(PhotoComposition.Base, r.Total, "an empty frame is just the base");
            Assert.AreEqual(PhotoRating.Snapshot, r.Rating);
            Assert.IsFalse(r.EarnsDiscovery, "a nothing shot pays nothing");
        }

        [Test]
        public void EachFramedElement_NeverLowersTheScore()
        {
            int baseline = PhotoComposition.Evaluate(Empty).Total;

            var withSky = Empty; withSky.SkyTier = 2;
            var withHorizon = Empty; withHorizon.HorizonInFrame = true;
            var withBody = Empty; withBody.OccludedBodyInFrame = true;
            var withLandmark = Empty; withLandmark.LandmarkInFrame = true;
            var withCreature = Empty; withCreature.CreatureInFrame = true;
            var withCentered = Empty; withCentered.FramingCentered = 1f;

            Assert.Greater(PhotoComposition.Evaluate(withSky).Total, baseline);
            Assert.Greater(PhotoComposition.Evaluate(withHorizon).Total, baseline);
            Assert.Greater(PhotoComposition.Evaluate(withBody).Total, baseline);
            Assert.Greater(PhotoComposition.Evaluate(withLandmark).Total, baseline);
            Assert.Greater(PhotoComposition.Evaluate(withCreature).Total, baseline);
            Assert.Greater(PhotoComposition.Evaluate(withCentered).Total, baseline);
        }

        [Test]
        public void SignatureSky_ScoresHigherThanStandard_HigherThanInterior()
        {
            var interior = Empty; interior.SkyTier = 0;
            var standard = Empty; standard.SkyTier = 1;
            var signature = Empty; signature.SkyTier = 2;
            Assert.Less(PhotoComposition.Evaluate(interior).Total, PhotoComposition.Evaluate(standard).Total);
            Assert.Less(PhotoComposition.Evaluate(standard).Total, PhotoComposition.Evaluate(signature).Total);
        }

        [Test]
        public void GreatVistaShot_RatesMasterpiece_AndEarnsDiscovery()
        {
            var hero = new PhotoSubjects
            {
                SkyTier = 2, HorizonInFrame = true, OccludedBodyInFrame = true,
                LandmarkInFrame = true, FramingCentered = 1f
            };
            var r = PhotoComposition.Evaluate(hero);
            Assert.GreaterOrEqual(r.Total, PhotoComposition.MasterpieceThreshold);
            Assert.AreEqual(PhotoRating.Masterpiece, r.Rating);
            Assert.IsTrue(r.EarnsDiscovery);
        }

        [Test]
        public void RatingBands_MatchThresholdsExactly()
        {
            // base(10) + standard sky(10) + horizon(8) + occluded body(15) = 43 → Postcard, earns discovery.
            var postcard = new PhotoSubjects { SkyTier = 1, HorizonInFrame = true, OccludedBodyInFrame = true };
            var pr = PhotoComposition.Evaluate(postcard);
            Assert.AreEqual(PhotoRating.Postcard, pr.Rating, "total=" + pr.Total);
            Assert.GreaterOrEqual(pr.Total, PhotoComposition.PostcardThreshold);
            Assert.Less(pr.Total, PhotoComposition.MasterpieceThreshold);
            Assert.IsTrue(pr.EarnsDiscovery);

            // Just under the postcard line stays a non-paying snapshot.
            var snap = new PhotoSubjects { SkyTier = 1, HorizonInFrame = true }; // 10+10+8 = 28
            var sr = PhotoComposition.Evaluate(snap);
            Assert.AreEqual(PhotoRating.Snapshot, sr.Rating, "total=" + sr.Total);
            Assert.IsFalse(sr.EarnsDiscovery);
        }

        [Test]
        public void Deterministic_AndClampsOutOfRange()
        {
            var s = new PhotoSubjects { SkyTier = 99, FramingCentered = 5f, OccludedBodyInFrame = true };
            var a = PhotoComposition.Evaluate(s);
            var b = PhotoComposition.Evaluate(s);
            Assert.AreEqual(a.Total, b.Total, "same input, same score, every device");
            // SkyTier clamps to signature (20), centered clamps to 1 (+10): 10+20+15+10 = 55.
            Assert.AreEqual(55, a.Total);

            var neg = new PhotoSubjects { SkyTier = -3, FramingCentered = -1f };
            Assert.AreEqual(PhotoComposition.Base, PhotoComposition.Evaluate(neg).Total,
                "negative tier/centered clamp to zero, never underflow");
        }
    }
}

using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    public sealed class CityBuildingPresentationCoreTests
    {
        [Test]
        public void BandForStorey_ProducesBaseMiddleCapGrammar()
        {
            Assert.That(CityBuildingPresentationCore.BandForStorey(0, 4), Is.EqualTo(CityFacadeBand.Base));
            Assert.That(CityBuildingPresentationCore.BandForStorey(1, 4), Is.EqualTo(CityFacadeBand.Middle));
            Assert.That(CityBuildingPresentationCore.BandForStorey(2, 4), Is.EqualTo(CityFacadeBand.Middle));
            Assert.That(CityBuildingPresentationCore.BandForStorey(3, 4), Is.EqualTo(CityFacadeBand.Cap));
            Assert.That(CityBuildingPresentationCore.BandForStorey(0, 1), Is.EqualTo(CityFacadeBand.Base));
        }

        [Test]
        public void WindowFor_IsDeterministicAndUsesClosedVocabulary()
        {
            var first = CityBuildingPresentationCore.WindowFor(1337, 2, 1, 4, 0.7f);
            for (int i = 0; i < 20; i++)
                Assert.That(CityBuildingPresentationCore.WindowFor(1337, 2, 1, 4, 0.7f), Is.EqualTo(first));

            for (int seed = 0; seed < 200; seed++)
            {
                var value = CityBuildingPresentationCore.WindowFor(seed, seed % 4, seed % 3, seed % 7, 0.65f);
                Assert.That(value, Is.AnyOf(
                    CityWindowLight.Dark,
                    CityWindowLight.HomeWarm,
                    CityWindowLight.ShopCool,
                    CityWindowLight.IndustrialNeutral));
            }
        }

        [Test]
        public void WindowFor_ClampsLightChance()
        {
            for (int seed = 0; seed < 64; seed++)
            {
                Assert.That(CityBuildingPresentationCore.WindowFor(seed, 0, 0, 0, -10f),
                    Is.EqualTo(CityWindowLight.Dark));
                Assert.That(CityBuildingPresentationCore.WindowFor(seed, 0, 0, 0, 10f),
                    Is.Not.EqualTo(CityWindowLight.Dark));
            }
        }

        [Test]
        public void RoofVocabulary_StaysInsideAuthoredEnvelope()
        {
            for (int seed = -100; seed <= 100; seed++)
            {
                int count = CityBuildingPresentationCore.RoofClutterCount(seed, 1, 3);
                Assert.That(count, Is.InRange(1, 3));
                Assert.That(CityBuildingPresentationCore.CrownScale(seed), Is.InRange(0.56f, 0.74f));
                Assert.That(CityBuildingPresentationCore.SignedOffset(seed, 0), Is.InRange(-1f, 1f));
            }
        }

        [Test]
        public void DifferentSeeds_ProducePresentationVariation()
        {
            int changedWindows = 0;
            int changedClutter = 0;
            var baselineWindow = CityBuildingPresentationCore.WindowFor(1, 1, 1, 1, 0.65f);
            int baselineClutter = CityBuildingPresentationCore.RoofClutterCount(1, 1, 3);
            for (int seed = 2; seed < 80; seed++)
            {
                if (CityBuildingPresentationCore.WindowFor(seed, 1, 1, 1, 0.65f) != baselineWindow)
                    changedWindows++;
                if (CityBuildingPresentationCore.RoofClutterCount(seed, 1, 3) != baselineClutter)
                    changedClutter++;
            }
            Assert.That(changedWindows, Is.GreaterThan(10));
            Assert.That(changedClutter, Is.GreaterThan(10));
        }
    }
}

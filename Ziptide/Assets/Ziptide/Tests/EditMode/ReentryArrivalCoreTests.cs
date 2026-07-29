using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The reentry act must fire on exactly ONE route — space leg → a world — and stay silent on
    /// every other way into the same scene (cold boot, gate travel between worlds, dev warp, or
    /// being inside the space leg itself). A presentation that plays on the wrong route is the
    /// "gate spectacle at minute ten" class of bug the rb121 story pass just removed; these tests
    /// pin the routing table so it cannot come back.
    /// </summary>
    public sealed class ReentryArrivalCoreTests
    {
        private const string Space = ZiptideConstants.SceneSpaceLane;
        private const string City = ZiptideConstants.SceneToxicCity;
        private const string W000 = ZiptideConstants.SceneW000;

        [Test]
        public void SpaceLegIntoAWorld_Plays()
        {
            Assert.IsTrue(ReentryArrivalCore.ShouldPlay(Space, City, Space));
        }

        [Test]
        public void ColdBoot_NoOrigin_NeverPlays()
        {
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay("", City, Space));
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(null, City, Space));
        }

        [Test]
        public void GateTravelBetweenWorlds_NeverPlays()
        {
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(W000, City, Space));
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(City, W000, Space));
        }

        [Test]
        public void ArrivingInTheSpaceLegItself_NeverPlays()
        {
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(W000, Space, Space));
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(Space, Space, Space));
        }

        [Test]
        public void MisconfiguredEmptySpaceScene_FailsClosed()
        {
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(Space, City, ""));
            Assert.IsFalse(ReentryArrivalCore.ShouldPlay(Space, "", Space));
        }
    }
}

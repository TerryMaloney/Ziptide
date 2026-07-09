using NUnit.Framework;
using Ziptide.Content.Traversal;

namespace Ziptide.Tests.EditMode
{
    /// <summary>Phase 1.4: the grapple reel's contracts — range-gated fire, monotonic capped reel,
    /// stop-margin arrival. Pure, headless.</summary>
    public class GrappleCoreTests
    {
        private static readonly TVec3 Origin = new TVec3(0, 0, 0);

        [Test]
        public void TryFire_OutOfRange_IsAMiss()
        {
            Assert.IsNull(GrappleReel.TryFire(Origin, new TVec3(0, 30, 0), maxRange: 22f),
                "past max range the hook doesn't catch");
            Assert.IsNull(GrappleReel.TryFire(Origin, new TVec3(0, 0.5f, 0)),
                "practically-touching anchors are a miss too (no zero-length reels)");
        }

        [Test]
        public void Reel_Arrives_ShortOfTheAnchor()
        {
            var reel = GrappleReel.TryFire(Origin, new TVec3(0, 12, 9), reelSpeed: 7f, stopMargin: 1.1f);
            Assert.IsNotNull(reel);
            int guard = 0;
            while (!reel.Arrived && guard++ < 10000) reel.Step(0.05f);
            Assert.IsTrue(reel.Arrived);
            float distToAnchor = (new TVec3(0, 12, 9) - reel.Position).Length;
            Assert.AreEqual(1.1f, distToAnchor, 0.02f, "you land BESIDE the ledge, not inside it");
        }

        [Test]
        public void Reel_IsMonotonic_AndSpeedCapped()
        {
            var reel = GrappleReel.TryFire(Origin, new TVec3(15, 8, 0), reelSpeed: 7f);
            float prev = 0f;
            for (int i = 0; i < 200 && !reel.Arrived; i++)
            {
                reel.Step(0.03f);
                Assert.GreaterOrEqual(reel.Progress, prev, "a grapple never stalls mid-air");
                Assert.LessOrEqual(reel.Speed, 7f + 1e-4f, "the comfort cap is a hard ceiling");
                prev = reel.Progress;
            }
        }

        [Test]
        public void Reel_EasesIn_NoYank()
        {
            var reel = GrappleReel.TryFire(Origin, new TVec3(0, 10, 10), reelSpeed: 7f);
            reel.Step(0.05f); // 50ms in — still spooling up
            Assert.Less(reel.Speed, 7f * 0.5f, "the pull starts soft (ease-in), never a yank");
        }
    }
}

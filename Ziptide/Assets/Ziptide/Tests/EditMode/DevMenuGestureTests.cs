#if UNITY_EDITOR || DEVELOPMENT_BUILD
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay.DevTools;

namespace Ziptide.Tests.EditMode
{
    public class DevMenuGestureTests
    {
        [Test]
        public void SummonPoseRequiresBothControllersAboveAndClose()
        {
            Vector3 head = Vector3.zero;
            Vector3 left = new Vector3(-0.12f, 0.18f, 0.08f);
            Vector3 right = new Vector3(0.12f, 0.18f, 0.08f);

            Assert.IsTrue(DevMenuGesture.IsSummonPose(head, left, right));
            Assert.IsFalse(DevMenuGesture.IsSummonPose(head, new Vector3(-0.12f, 0.01f, 0.08f), right));
            Assert.IsFalse(DevMenuGesture.IsSummonPose(head, new Vector3(-0.6f, 0.2f, 0f), right));
            Assert.IsFalse(DevMenuGesture.IsSummonPose(head, left, new Vector3(0.4f, 0.18f, 0.08f)));
        }

        [Test]
        public void ContinuousTwoSecondHoldFiresOnce()
        {
            var gesture = new DevMenuGesture();

            Assert.IsFalse(gesture.TickPose(true, 0.75f));
            Assert.IsFalse(gesture.TickPose(true, 0.75f));
            Assert.IsTrue(gesture.TickPose(true, 0.5f));
            Assert.AreEqual(1f, gesture.Progress01);
            Assert.IsFalse(gesture.TickPose(true, 10f), "latched pose must not retrigger");
        }

        [Test]
        public void BreakingPoseResetsProgressAndLatch()
        {
            var gesture = new DevMenuGesture();

            Assert.IsFalse(gesture.TickPose(true, 1.5f));
            Assert.Greater(gesture.Progress01, 0f);
            Assert.IsFalse(gesture.TickPose(false, 0.01f));
            Assert.AreEqual(0f, gesture.Progress01);

            Assert.IsTrue(gesture.TickPose(true, 2f));
            Assert.IsFalse(gesture.TickPose(true, 2f));
            Assert.IsFalse(gesture.TickPose(false, 0.01f));
            Assert.IsTrue(gesture.TickPose(true, 2f), "release must re-arm the gesture");
        }

        [Test]
        public void NegativeDeltaCannotAdvanceHold()
        {
            var gesture = new DevMenuGesture();
            Assert.IsFalse(gesture.TickPose(true, -100f));
            Assert.AreEqual(0f, gesture.Progress01);
        }

        [Test]
        public void ThresholdsAreInclusive()
        {
            Vector3 head = Vector3.zero;
            Vector3 left = new Vector3(-DevMenuGesture.MaximumControllerSeparation * 0.5f,
                DevMenuGesture.MinimumAboveHead, 0f);
            Vector3 right = new Vector3(DevMenuGesture.MaximumControllerSeparation * 0.5f,
                DevMenuGesture.MinimumAboveHead, 0f);

            Assert.IsTrue(DevMenuGesture.IsSummonPose(head, left, right));
        }
    }
}
#endif

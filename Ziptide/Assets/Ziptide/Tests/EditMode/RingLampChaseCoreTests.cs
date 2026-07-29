using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The lamp-chase is the space leg's whole wayfinding read ("are the rings there immediately —
    /// how do we know where we're going"): the NEXT ring must be unmistakably the bright moving
    /// one, passed rings must settle, and the classification must follow course progress exactly.
    /// These tests pin the pure math so the read can't silently invert.
    /// </summary>
    public sealed class RingLampChaseCoreTests
    {
        [Test]
        public void Classify_FollowsCourseProgress()
        {
            Assert.AreEqual(RingLampState.Next, RingLampChaseCore.Classify(0, 0));
            Assert.AreEqual(RingLampState.Future, RingLampChaseCore.Classify(3, 0));
            Assert.AreEqual(RingLampState.Passed, RingLampChaseCore.Classify(0, 1));
            Assert.AreEqual(RingLampState.Next, RingLampChaseCore.Classify(4, 4));
            // Course complete (next == count): every ring reads Passed.
            Assert.AreEqual(RingLampState.Passed, RingLampChaseCore.Classify(4, 5));
        }

        [Test]
        public void PassedAndFutureColors_AreConstantOverTime()
        {
            Assert.AreEqual(
                RingLampChaseCore.SegmentColor(RingLampState.Passed, 3, 10, 0f),
                RingLampChaseCore.SegmentColor(RingLampState.Passed, 3, 10, 7.31f));
            Assert.AreEqual(
                RingLampChaseCore.SegmentColor(RingLampState.Future, 6, 10, 0f),
                RingLampChaseCore.SegmentColor(RingLampState.Future, 6, 10, 2.5f));
        }

        [Test]
        public void ChaseWeight_StaysInUnitRange()
        {
            for (int s = 0; s < 10; s++)
                for (float t = 0f; t < 3f; t += 0.13f)
                {
                    float w = RingLampChaseCore.ChaseWeight(s, 10, t);
                    Assert.GreaterOrEqual(w, 0f, "segment " + s + " t=" + t);
                    Assert.LessOrEqual(w, 1f, "segment " + s + " t=" + t);
                }
        }

        [Test]
        public void ChaseWindow_MovesAroundTheRing()
        {
            // The brightest segment at t=0 must differ from the brightest half a cycle later —
            // a lamp that doesn't travel is a strobe, and strobes are a comfort violation.
            float half = 0.5f / RingLampChaseCore.ChaseCyclesPerSecond;
            Assert.AreNotEqual(Brightest(0f), Brightest(half));
        }

        [Test]
        public void ChaseWindow_AlwaysLightsSomething()
        {
            for (float t = 0f; t < 2f; t += 0.05f)
            {
                float max = 0f;
                for (int s = 0; s < 10; s++)
                    max = Mathf.Max(max, RingLampChaseCore.ChaseWeight(s, 10, t));
                Assert.Greater(max, 0.4f, "at t=" + t + " no segment carries the window");
            }
        }

        [Test]
        public void NextRing_IsAlwaysBrighterThanFutureRings()
        {
            for (int s = 0; s < 10; s++)
            {
                Color next = RingLampChaseCore.SegmentColor(RingLampState.Next, s, 10, 1.7f);
                Color future = RingLampChaseCore.FutureColor;
                Assert.Greater(next.r + next.g + next.b, future.r + future.g + future.b);
            }
        }

        [Test]
        public void DegenerateSegmentCount_IsSafe()
        {
            Assert.AreEqual(0f, RingLampChaseCore.ChaseWeight(0, 0, 1f));
            Assert.AreEqual(0f, RingLampChaseCore.ChaseWeight(0, -1, 1f));
        }

        private static int Brightest(float time)
        {
            int best = -1;
            float bestW = -1f;
            for (int s = 0; s < 10; s++)
            {
                float w = RingLampChaseCore.ChaseWeight(s, 10, time);
                if (w > bestW) { bestW = w; best = s; }
            }
            return best;
        }
    }
}

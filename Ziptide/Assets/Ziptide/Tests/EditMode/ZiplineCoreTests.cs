using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 1.4 / WORLDS #23 — the signature traversal's math. Pins: anchors are exact, the
    /// cable sags mid-span only, rides ease in (no jerk to cruise), progress is monotonic and always
    /// arrives, and everything is deterministic.
    /// </summary>
    public class ZiplineCoreTests
    {
        private static readonly Vector3 A = new Vector3(0f, 20f, 0f);
        private static readonly Vector3 B = new Vector3(30f, 5f, 40f);

        [Test]
        public void Sample_AnchorsAreExact_NoSagAtEnds()
        {
            float sag = ZiplineCore.SagFor(A, B);
            Assert.AreEqual(0f, Vector3.Distance(A, ZiplineCore.Sample(A, B, sag, 0f)), 1e-4f);
            Assert.AreEqual(0f, Vector3.Distance(B, ZiplineCore.Sample(A, B, sag, 1f)), 1e-4f);
        }

        [Test]
        public void Sample_MidSpan_DipsBelowTheStraightLine()
        {
            float sag = ZiplineCore.SagFor(A, B);
            Vector3 straightMid = Vector3.Lerp(A, B, 0.5f);
            Vector3 cableMid = ZiplineCore.Sample(A, B, sag, 0.5f);
            Assert.Less(cableMid.y, straightMid.y);
            Assert.AreEqual(sag, straightMid.y - cableMid.y, 1e-4f, "full sag depth lands mid-span");
        }

        [Test]
        public void Advance_EasesIn_SlowerAtStartThanAtCruise()
        {
            float len = Vector3.Distance(A, B);
            float early = ZiplineCore.Advance(0f, 0.05f, rideSeconds: 0.05f, len);
            float cruise = ZiplineCore.Advance(0.5f, 0.05f, rideSeconds: 5f, len) - 0.5f;
            Assert.Less(early, cruise, "the first moments must be gentler than cruise (no jerk)");
        }

        [Test]
        public void Advance_MonotonicAndAlwaysArrives()
        {
            float len = Vector3.Distance(A, B);
            float t = 0f, ride = 0f, prev = 0f;
            for (int i = 0; i < 2000 && !ZiplineCore.Arrived(t); i++)
            {
                ride += 0.02f;
                t = ZiplineCore.Advance(t, 0.02f, ride, len);
                Assert.GreaterOrEqual(t, prev, "progress never reverses");
                prev = t;
            }
            Assert.IsTrue(ZiplineCore.Arrived(t), "a ride always reaches the far anchor");
            Assert.AreEqual(1f, ZiplineCore.Advance(1f, 0.5f, 10f, len), 1e-5f, "clamps at the end");
        }

        [Test]
        public void Advance_DegenerateInputs_AreSafe()
        {
            Assert.AreEqual(0.3f, ZiplineCore.Advance(0.3f, 0f, 1f, 50f), 1e-6f, "zero dt no-ops");
            Assert.AreEqual(1f, ZiplineCore.Advance(2f, 0.1f, 1f, 50f), 1e-6f, "t over-range clamps");
            Assert.AreEqual(0.3f, ZiplineCore.Advance(0.3f, 0.1f, 1f, 0f), 1e-6f, "zero-length line no-ops");
        }

        [Test]
        public void SagFor_ScalesWithSpan_AndClamps()
        {
            Assert.Greater(ZiplineCore.SagFor(Vector3.zero, new Vector3(100f, 0f, 0f)),
                           ZiplineCore.SagFor(Vector3.zero, new Vector3(10f, 0f, 0f)));
            Assert.AreEqual(25f, ZiplineCore.SagFor(Vector3.zero, new Vector3(100f, 0f, 0f), 5f), 1e-4f,
                "sag fraction clamps at 0.25 — a cable, not a bungee");
        }
    }
}

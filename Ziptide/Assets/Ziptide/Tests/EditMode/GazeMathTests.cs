using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Observation-cone contract for gaze-reactive creatures (Witness-mite freezes when watched,
    /// Light-grazer shrinks when lit). Pure, headless.
    /// </summary>
    public class GazeMathTests
    {
        private const float Cos60HalfFov = 0.5f;

        [Test]
        public void DeadAhead_InRange_IsObserved()
        {
            Assert.IsTrue(GazeMath.IsObserved(Vector3.forward, new Vector3(0f, 0f, 5f), Cos60HalfFov, 10f));
        }

        [Test]
        public void BehindTheViewer_IsNotObserved()
        {
            Assert.IsFalse(GazeMath.IsObserved(Vector3.forward, new Vector3(0f, 0f, -5f), Cos60HalfFov, 10f));
        }

        [Test]
        public void OutsideTheCone_IsNotObserved()
        {
            // ~80° off-axis with a 60° half-FOV cone.
            Assert.IsFalse(GazeMath.IsObserved(Vector3.forward, new Vector3(6f, 0f, 1f), Cos60HalfFov, 10f));
        }

        [Test]
        public void InsideConeButOutOfRange_IsNotObserved()
        {
            Assert.IsFalse(GazeMath.IsObserved(Vector3.forward, new Vector3(0f, 0f, 25f), Cos60HalfFov, 10f));
        }

        [Test]
        public void EdgeOfCone_CountsAsObserved()
        {
            // Exactly 60° off with cosHalfFov = cos(60°) = 0.5 → dot == threshold.
            Vector3 to = new Vector3(Mathf.Sin(60f * Mathf.Deg2Rad), 0f, Mathf.Cos(60f * Mathf.Deg2Rad)) * 5f;
            Assert.IsTrue(GazeMath.IsObserved(Vector3.forward, to, Cos60HalfFov - 0.001f, 10f));
        }

        [Test]
        public void ZeroDistance_IsSafeAndUnobserved()
        {
            Assert.IsFalse(GazeMath.IsObserved(Vector3.forward, Vector3.zero, Cos60HalfFov, 10f));
        }
    }
}

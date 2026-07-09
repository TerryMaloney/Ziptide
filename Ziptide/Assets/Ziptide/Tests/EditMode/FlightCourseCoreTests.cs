using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// P4b ring-course contracts: rings pass strictly in order, a passed ring stays passed, flying
    /// through a LATER ring early does nothing, and an empty course is born complete (never blocks).
    /// </summary>
    public class FlightCourseCoreTests
    {
        private static readonly Vector3[] Rings = { new Vector3(0f, 0f, 50f), new Vector3(30f, 5f, 100f) };

        [Test]
        public void Rings_PassInOrder_AndCourseCompletes()
        {
            var course = new FlightCourseCore(Rings, 8f);
            Assert.AreEqual(0, course.NextRing);
            Assert.IsFalse(course.Advance(Vector3.zero), "Far from ring 0 — no progress.");

            Assert.IsTrue(course.Advance(new Vector3(2f, 1f, 52f)), "Inside ring 0 passes it.");
            Assert.AreEqual(1, course.NextRing);
            Assert.IsFalse(course.IsComplete);

            Assert.IsTrue(course.Advance(new Vector3(28f, 4f, 103f)), "Inside ring 1 passes it.");
            Assert.IsTrue(course.IsComplete);
            Assert.IsFalse(course.Advance(new Vector3(28f, 4f, 103f)), "A complete course never re-fires.");
        }

        [Test]
        public void SkippingAhead_DoesNotCount()
        {
            var course = new FlightCourseCore(Rings, 8f);
            Assert.IsFalse(course.Advance(new Vector3(30f, 5f, 100f)),
                "Sitting inside ring 1 while ring 0 is next must not advance.");
            Assert.AreEqual(0, course.NextRing);
        }

        [Test]
        public void EmptyCourse_IsBornComplete()
        {
            Assert.IsTrue(new FlightCourseCore(null, 8f).IsComplete);
            Assert.IsTrue(new FlightCourseCore(new Vector3[0], 8f).IsComplete);
        }
    }
}

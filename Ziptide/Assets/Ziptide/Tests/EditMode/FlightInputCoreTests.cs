using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// P4b stick-shaping contracts: signed symmetric throttle (forward and reverse both rescaled
    /// past the deadzone — FlightModel owns the reverse CAP), pitch deadzone, and the snap-yaw
    /// flick latch — one snap per flick, no re-fire while held, re-armed only near center. Pure,
    /// headless (the comfort companion to FlightModelTests' no-smooth-yaw law).
    /// </summary>
    public class FlightInputCoreTests
    {
        [Test]
        public void Throttle_IsSigned_AndSymmetricallyRescaledPastDeadzone()
        {
            bool armed = true;
            Assert.AreEqual(0f, FlightInputCore.Shape(new Vector2(0f, 0.1f), Vector2.zero, ref armed).Throttle,
                "Inside the deadzone the throttle must stay zero.");
            Assert.AreEqual(0f, FlightInputCore.Shape(new Vector2(0f, -0.1f), Vector2.zero, ref armed).Throttle,
                "The deadzone is symmetric — a resting stick never reverses.");
            Assert.AreEqual(1f, FlightInputCore.Shape(new Vector2(0f, 1f), Vector2.zero, ref armed).Throttle, 1e-4f,
                "Full deflection must reach full throttle despite the deadzone rescale.");
            Assert.AreEqual(-1f, FlightInputCore.Shape(new Vector2(0f, -1f), Vector2.zero, ref armed).Throttle, 1e-4f,
                "Full pull-back is full reverse intent (FlightModel caps the actual reverse speed).");
            float half = FlightInputCore.Shape(new Vector2(0f, 0.575f), Vector2.zero, ref armed).Throttle;
            Assert.AreEqual(0.5f, half, 1e-3f, "Deadzone rescale should be linear to full deflection.");
            float halfBack = FlightInputCore.Shape(new Vector2(0f, -0.575f), Vector2.zero, ref armed).Throttle;
            Assert.AreEqual(-0.5f, halfBack, 1e-3f, "…in both directions.");
        }

        [Test]
        public void Pitch_HasDeadzone_AndPassesThrough()
        {
            bool armed = true;
            Assert.AreEqual(0f, FlightInputCore.Shape(Vector2.zero, new Vector2(0f, 0.1f), ref armed).Pitch);
            Assert.AreEqual(-0.8f, FlightInputCore.Shape(Vector2.zero, new Vector2(0f, -0.8f), ref armed).Pitch, 1e-4f);
        }

        [Test]
        public void YawFlick_SnapsOnce_ThenHoldsFire_UntilRearmedNearCenter()
        {
            bool armed = true;

            var flick = FlightInputCore.Shape(Vector2.zero, new Vector2(0.9f, 0f), ref armed);
            Assert.AreEqual(1, flick.YawSnap, "Crossing the threshold while armed snaps once.");

            var held = FlightInputCore.Shape(Vector2.zero, new Vector2(0.9f, 0f), ref armed);
            Assert.AreEqual(0, held.YawSnap, "Holding the stick must NOT keep snapping (no smooth yaw).");

            var halfway = FlightInputCore.Shape(Vector2.zero, new Vector2(0.45f, 0f), ref armed);
            Assert.AreEqual(0, halfway.YawSnap, "Between rearm and snap thresholds nothing fires or rearms.");

            FlightInputCore.Shape(Vector2.zero, new Vector2(0.1f, 0f), ref armed); // return near center
            var second = FlightInputCore.Shape(Vector2.zero, new Vector2(-0.9f, 0f), ref armed);
            Assert.AreEqual(-1, second.YawSnap, "After re-arming near center a new flick snaps the other way.");
        }
    }
}

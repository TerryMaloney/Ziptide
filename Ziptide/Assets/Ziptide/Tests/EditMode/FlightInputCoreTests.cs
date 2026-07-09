using NUnit.Framework;
using UnityEngine;
using Ziptide.Ship;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// P4b stick-shaping contracts (Xbox-parity pass v1.2): signed symmetric throttle AND strafe
    /// (both rescaled past the deadzone — FlightModel owns the speed caps), pitch deadzone, and
    /// the snap-yaw HOLD-TO-REPEAT latch — the first flick snaps immediately, holding repeats on a
    /// fixed cadence (never faster), and returning near center re-arms the instant response. Snaps
    /// stay discrete: there is still no smooth-yaw path. Pure, headless.
    /// </summary>
    public class FlightInputCoreTests
    {
        [Test]
        public void Throttle_IsSigned_AndSymmetricallyRescaledPastDeadzone()
        {
            var latch = new FlightYawLatch { Armed = true };
            Assert.AreEqual(0f, FlightInputCore.Shape(new Vector2(0f, 0.1f), Vector2.zero, ref latch, 0f).Throttle,
                "Inside the deadzone the throttle must stay zero.");
            Assert.AreEqual(0f, FlightInputCore.Shape(new Vector2(0f, -0.1f), Vector2.zero, ref latch, 0f).Throttle,
                "The deadzone is symmetric — a resting stick never reverses.");
            Assert.AreEqual(1f, FlightInputCore.Shape(new Vector2(0f, 1f), Vector2.zero, ref latch, 0f).Throttle, 1e-4f,
                "Full deflection must reach full throttle despite the deadzone rescale.");
            Assert.AreEqual(-1f, FlightInputCore.Shape(new Vector2(0f, -1f), Vector2.zero, ref latch, 0f).Throttle, 1e-4f,
                "Full pull-back is full reverse intent (FlightModel caps the actual reverse speed).");
            float half = FlightInputCore.Shape(new Vector2(0f, 0.575f), Vector2.zero, ref latch, 0f).Throttle;
            Assert.AreEqual(0.5f, half, 1e-3f, "Deadzone rescale should be linear to full deflection.");
        }

        [Test]
        public void Strafe_FillsLeftStickX_WithTheSameShaping()
        {
            var latch = new FlightYawLatch { Armed = true };
            Assert.AreEqual(0f, FlightInputCore.Shape(new Vector2(0.1f, 0f), Vector2.zero, ref latch, 0f).Strafe,
                "Strafe respects the deadzone.");
            Assert.AreEqual(1f, FlightInputCore.Shape(new Vector2(1f, 0f), Vector2.zero, ref latch, 0f).Strafe, 1e-4f);
            Assert.AreEqual(-0.5f, FlightInputCore.Shape(new Vector2(-0.575f, 0f), Vector2.zero, ref latch, 0f).Strafe, 1e-3f,
                "Strafe shaping mirrors throttle shaping in both directions.");
        }

        [Test]
        public void Pitch_HasDeadzone_AndPassesThrough()
        {
            var latch = new FlightYawLatch { Armed = true };
            Assert.AreEqual(0f, FlightInputCore.Shape(Vector2.zero, new Vector2(0f, 0.1f), ref latch, 0f).Pitch);
            Assert.AreEqual(-0.8f, FlightInputCore.Shape(Vector2.zero, new Vector2(0f, -0.8f), ref latch, 0f).Pitch, 1e-4f);
        }

        [Test]
        public void YawFlick_SnapsImmediately_ThenRepeatsOnCadence_WhileHeld()
        {
            var latch = new FlightYawLatch { Armed = true };
            var held = new Vector2(0.9f, 0f);

            Assert.AreEqual(1, FlightInputCore.Shape(Vector2.zero, held, ref latch, 10.0f).YawSnap,
                "An armed flick snaps immediately.");
            Assert.AreEqual(0, FlightInputCore.Shape(Vector2.zero, held, ref latch, 10.1f).YawSnap,
                "Holding does not snap again before the repeat cadence.");
            Assert.AreEqual(0, FlightInputCore.Shape(Vector2.zero, held, ref latch, 10.3f).YawSnap);
            Assert.AreEqual(1, FlightInputCore.Shape(Vector2.zero, held, ref latch, 10.0f + FlightInputCore.RepeatSeconds).YawSnap,
                "Holding the stick repeats a snap on the cadence — Xbox players keep turning.");
            Assert.AreEqual(0, FlightInputCore.Shape(Vector2.zero, held, ref latch, 10.5f).YawSnap,
                "…and the cadence restarts after each repeat (never faster).");
        }

        [Test]
        public void YawLatch_RearmsNearCenter_ForInstantResponse()
        {
            var latch = new FlightYawLatch { Armed = true };
            FlightInputCore.Shape(Vector2.zero, new Vector2(0.9f, 0f), ref latch, 20.0f);   // snap
            FlightInputCore.Shape(Vector2.zero, new Vector2(0.45f, 0f), ref latch, 20.05f); // between zones: nothing
            Assert.IsFalse(latch.Armed, "Between rearm and snap thresholds nothing fires or rearms.");

            FlightInputCore.Shape(Vector2.zero, new Vector2(0.1f, 0f), ref latch, 20.1f);   // near center
            var second = FlightInputCore.Shape(Vector2.zero, new Vector2(-0.9f, 0f), ref latch, 20.15f);
            Assert.AreEqual(-1, second.YawSnap,
                "After re-arming near center a new flick snaps the other way immediately, no cadence wait.");
        }
    }
}

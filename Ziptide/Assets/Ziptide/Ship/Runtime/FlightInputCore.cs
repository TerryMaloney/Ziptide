using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>One shaped frame of pilot intent, ready for FlightModel.Tick / SnapYaw.</summary>
    public struct FlightInputFrame
    {
        public float Throttle;     // -1..1 — signed target speed fraction (negative = reverse)
        public float Pitch;        // -1..1 — slow world tilt
        public int YawSnap;        // -1 / 0 / +1 — at most one discrete snap per flick
    }

    /// <summary>
    /// P4b — PURE stick shaping for flight (CONTROLS_AND_FLIGHT mapping): left stick Y = throttle
    /// (push forward to fly, pull back to reverse — FlightModel caps reverse at its own fraction),
    /// right stick Y = pitch, right stick X = snap yaw with a flick LATCH — one snap per flick past
    /// the threshold, re-armed only when the stick returns near center, so holding the stick can
    /// never spin the ship (comfort law, same reason FlightModel has no smooth yaw). Boost and
    /// barrel-roll are buttons, read by the runtime directly — no shaping needed here.
    /// Pinned by FlightInputCoreTests.
    /// </summary>
    public static class FlightInputCore
    {
        public const float Deadzone = 0.15f;
        public const float SnapThreshold = 0.6f;
        public const float RearmThreshold = 0.3f;

        /// <summary>Shape raw sticks into a flight frame. <paramref name="yawArmed"/> is the flick
        /// latch — keep it across frames; true means the next threshold-crossing snaps.</summary>
        public static FlightInputFrame Shape(Vector2 leftStick, Vector2 rightStick, ref bool yawArmed)
        {
            var frame = new FlightInputFrame
            {
                Throttle = Rescale(leftStick.y),
                Pitch = Mathf.Abs(rightStick.y) > Deadzone
                    ? Mathf.Clamp(rightStick.y, -1f, 1f)
                    : 0f,
            };

            float x = rightStick.x;
            if (yawArmed && Mathf.Abs(x) >= SnapThreshold)
            {
                frame.YawSnap = x > 0f ? 1 : -1;
                yawArmed = false;
            }
            else if (!yawArmed && Mathf.Abs(x) <= RearmThreshold)
            {
                yawArmed = true;
            }
            return frame;
        }

        /// <summary>Signed throttle: symmetric deadzone, rescaled so full deflection = ±1.</summary>
        private static float Rescale(float y)
        {
            if (y > Deadzone) return Mathf.Clamp01((y - Deadzone) / (1f - Deadzone));
            if (y < -Deadzone) return -Mathf.Clamp01((-y - Deadzone) / (1f - Deadzone));
            return 0f;
        }
    }
}

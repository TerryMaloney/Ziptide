using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>One shaped frame of pilot intent, ready for FlightModel.Tick / SnapYaw.</summary>
    public struct FlightInputFrame
    {
        public float Throttle;     // -1..1 — signed target speed fraction (negative = reverse)
        public float Strafe;       // -1..1 — lateral slide intent (FlightModel caps the speed)
        public float Pitch;        // -1..1 — slow world tilt
        public int YawSnap;        // -1 / 0 / +1 — at most one discrete snap per frame
    }

    /// <summary>Cross-frame state for the snap-yaw latch. Keep one per pilot and pass by ref.</summary>
    public struct FlightYawLatch
    {
        public bool Armed;          // true = the next threshold-crossing snaps immediately
        public float NextRepeatAt;  // while held past threshold, the next auto-repeat time
    }

    /// <summary>
    /// P4b — PURE stick shaping for flight (CONTROLS_AND_FLIGHT mapping, Xbox-parity pass v1.2):
    /// left stick Y = throttle (push to fly, pull to reverse — FlightModel caps reverse), left
    /// stick X = strafe (lateral slide, translation-only), right stick Y = pitch, right stick X =
    /// snap yaw with a HOLD-TO-REPEAT latch — the first flick snaps immediately, holding the stick
    /// repeats a snap every RepeatSeconds (matching how the walking XRI snap-turn behaves), and
    /// returning near center re-arms the instant response. Snaps stay discrete: no code path can
    /// smooth-yaw (comfort law). Boost and barrel-roll are buttons, read by the runtime directly.
    /// Pinned by FlightInputCoreTests.
    /// </summary>
    public static class FlightInputCore
    {
        public const float Deadzone = 0.15f;
        public const float SnapThreshold = 0.6f;
        public const float RearmThreshold = 0.3f;
        public const float RepeatSeconds = 0.4f; // held-stick snap cadence (walking snap-turn feel)

        /// <summary>Shape raw sticks into a flight frame. <paramref name="latch"/> is the snap-yaw
        /// state — keep it across frames. <paramref name="now"/> is the pilot's clock (Time.time
        /// in the runtime; any monotonic value in tests).</summary>
        public static FlightInputFrame Shape(Vector2 leftStick, Vector2 rightStick,
            ref FlightYawLatch latch, float now)
        {
            var frame = new FlightInputFrame
            {
                Throttle = Rescale(leftStick.y),
                Strafe = Rescale(leftStick.x),
                Pitch = Mathf.Abs(rightStick.y) > Deadzone
                    ? Mathf.Clamp(rightStick.y, -1f, 1f)
                    : 0f,
            };

            float x = rightStick.x;
            if (Mathf.Abs(x) >= SnapThreshold)
            {
                if (latch.Armed || now >= latch.NextRepeatAt)
                {
                    frame.YawSnap = x > 0f ? 1 : -1;
                    latch.Armed = false;
                    latch.NextRepeatAt = now + RepeatSeconds;
                }
            }
            else if (Mathf.Abs(x) <= RearmThreshold)
            {
                latch.Armed = true;
            }
            return frame;
        }

        /// <summary>Signed axis: symmetric deadzone, rescaled so full deflection = ±1.</summary>
        private static float Rescale(float v)
        {
            if (v > Deadzone) return Mathf.Clamp01((v - Deadzone) / (1f - Deadzone));
            if (v < -Deadzone) return -Mathf.Clamp01((-v - Deadzone) / (1f - Deadzone));
            return 0f;
        }
    }
}

using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// HARDWIRING 1.4 / Additions Bank WORLDS #23 — the pure zipline math: a sagging cable between
    /// two anchors and a comfort-reviewed ride along it. The game is named ZIPTIDE — ziplines are the
    /// signature traversal, so the feel rules live HERE in tested math (the FlightModel law: comfort
    /// enforced where no translator can break it):
    ///  · the cable sags like a cable (parabolic approximation of a catenary),
    ///  · a ride EASES IN over the first moments (a hard jerk to cruise speed is the sickness spike),
    ///  · progress is monotonic and clamps at the far anchor — you always arrive, never overshoot.
    /// Deterministic, no state — scene runtimes hold their own t and call Advance each frame.
    /// </summary>
    public static class ZiplineCore
    {
        /// <summary>Default cruise speed (m/s). Brisk but readable; device feel pass tunes it.</summary>
        public const float DefaultSpeed = 8f;
        /// <summary>Seconds of ease-in from standstill to cruise.</summary>
        public const float EaseInSeconds = 0.6f;
        /// <summary>Cable sag as a fraction of span length (0.04 = 4% dip at midpoint).</summary>
        public const float DefaultSagFraction = 0.04f;

        /// <summary>Point on the cable at <paramref name="t"/> ∈ [0,1]: straight lerp plus a
        /// parabolic dip that is zero at both anchors and deepest mid-span.</summary>
        public static Vector3 Sample(Vector3 start, Vector3 end, float sagMeters, float t)
        {
            t = Mathf.Clamp01(t);
            Vector3 p = Vector3.LerpUnclamped(start, end, t);
            p.y -= Mathf.Max(0f, sagMeters) * 4f * t * (1f - t);
            return p;
        }

        /// <summary>Sag depth for a span — longer lines dip more, clamped so short hops stay taut.</summary>
        public static float SagFor(Vector3 start, Vector3 end, float sagFraction = DefaultSagFraction)
            => Vector3.Distance(start, end) * Mathf.Clamp(sagFraction, 0f, 0.25f);

        /// <summary>
        /// Advance ride progress. <paramref name="t"/> = current 0..1 progress,
        /// <paramref name="rideSeconds"/> = how long the ride has been active (drives ease-in),
        /// <paramref name="lineLength"/> = anchor distance in meters. Returns the new t, clamped to 1.
        /// </summary>
        public static float Advance(float t, float dt, float rideSeconds, float lineLength,
            float speed = DefaultSpeed)
        {
            if (dt <= 0f || lineLength <= 0.001f || speed <= 0f) return Mathf.Clamp01(t);
            float ease = EaseInSeconds <= 0f ? 1f : Mathf.Clamp01(rideSeconds / EaseInSeconds);
            // Smoothstep the ramp — no velocity discontinuity at either end of the ease.
            ease = ease * ease * (3f - 2f * ease);
            float metersPerSec = speed * ease;
            return Mathf.Clamp01(t + (metersPerSec * dt) / lineLength);
        }

        /// <summary>A ride is done once progress reaches the far anchor.</summary>
        public static bool Arrived(float t) => t >= 1f - 1e-4f;
    }
}

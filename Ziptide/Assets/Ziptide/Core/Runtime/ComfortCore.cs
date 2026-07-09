using System;

namespace Ziptide.Core
{
    /// <summary>
    /// HARDWIRING 0.4 / CONSISTENCY_SPINE §B — the ONE comfort math for every moving frame the player
    /// rides: walking locomotion today, ship flight and vehicles later. Pure C# (no UnityEngine) so
    /// the tunnel behaviour is pinned by EditMode tests and no translator can break it.
    ///
    /// Model: artificial motion (rig translation speed + rig turn rate — NEVER natural head motion)
    /// closes a peripheral iris toward a floor; stillness reopens it. Closing is FAST (protection must
    /// arrive with the motion) and opening is SLOW (a pumping iris is itself nauseating).
    /// </summary>
    public static class ComfortCore
    {
        /// <summary>Fully open aperture (no tunnel).</summary>
        public const float OpenAperture = 1f;
        /// <summary>Aperture floor at full strength + full motion — never a keyhole, always context.</summary>
        public const float MinAperture = 0.45f;
        /// <summary>Aperture closes at this fraction-per-second toward target…</summary>
        public const float CloseRate = 6f;
        /// <summary>…and reopens at this slower rate (asymmetric on purpose).</summary>
        public const float OpenRate = 1.6f;

        /// <summary>
        /// Target aperture for the current motion. <paramref name="speed01"/> = artificial move speed
        /// normalized 0..1 (1 ≈ sprint), <paramref name="turn01"/> = artificial turn rate normalized
        /// 0..1 (1 ≈ full smooth-turn). <paramref name="strength"/> = the player's comfort setting
        /// (0 = vignette off → always fully open; 1 = strongest).
        /// </summary>
        public static float TargetAperture(float speed01, float turn01, float strength)
        {
            strength = Clamp01(strength);
            if (strength <= 0f) return OpenAperture;

            // Turning is the stronger sickness driver than linear speed — weight it heavier.
            float motion = Clamp01(Clamp01(speed01) * 0.6f + Clamp01(turn01) * 0.9f);
            float depth = (OpenAperture - MinAperture) * motion * strength;
            return OpenAperture - depth;
        }

        /// <summary>Advance the live aperture toward <paramref name="target"/> over <paramref name="dt"/>
        /// seconds — fast to close, slow to open.</summary>
        public static float Step(float current, float target, float dt)
        {
            if (dt <= 0f) return current;
            float rate = target < current ? CloseRate : OpenRate;
            float max = rate * dt;
            float delta = target - current;
            if (delta > max) delta = max;
            else if (delta < -max) delta = -max;
            float next = current + delta;
            return next < MinAperture ? MinAperture : (next > OpenAperture ? OpenAperture : next);
        }

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    }
}

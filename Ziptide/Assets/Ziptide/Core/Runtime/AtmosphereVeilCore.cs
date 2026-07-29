namespace Ziptide.Core
{
    /// <summary>Which way through the atmosphere this veil is playing.</summary>
    public enum VeilLeg
    {
        /// <summary>Climbing out: the veil BUILDS into the travel cut.</summary>
        Ascent,
        /// <summary>Coming down: the veil starts hot and CLEARS into the new world.</summary>
        Reentry,
    }

    public enum VeilPhase { Build, Peak, Clear, Done }

    /// <summary>
    /// THE ATMOSPHERE VEIL's timing law (docs/design LAUNCH_AND_ATMOSPHERE_TRANSITION;
    /// LEVEL1_SPATIAL_SCRIPT §2). Leaving a planet and coming back are the two moments the
    /// space leg most needs to FEEL like distance rather than a loading screen, so the launch
    /// brackets the scene swap with a plasma build and the arrival opens with it clearing.
    ///
    /// Pure so the whole schedule is provable without a headset — and because the one rule that
    /// really matters here is a safety rule: presentation may never strand travel. Every duration
    /// sits under <see cref="HardCapSeconds"/>, the cut lead is always shorter than the veil, and
    /// intensity is clamped and forced to zero past the cap, so a stalled effect fades instead of
    /// leaving a pilot staring at orange forever.
    /// </summary>
    public static class AtmosphereVeilCore
    {
        public const float AscentBuildSeconds = 1.2f;
        public const float AscentPeakSeconds = 0.5f;
        public const float ReentryPeakSeconds = 0.6f;
        public const float ReentryClearSeconds = 1.9f;

        /// <summary>No veil may live longer than this, whatever the caller asks for.</summary>
        public const float HardCapSeconds = 4f;

        /// <summary>How long the launch waits after starting the veil before cutting the scene —
        /// inside the peak, so the load happens at the brightest moment and the arrival veil
        /// picks the moment up on the other side.</summary>
        public const float CutLeadSeconds = AscentBuildSeconds + AscentPeakSeconds * 0.6f;

        public static float TotalSeconds(VeilLeg leg) =>
            leg == VeilLeg.Ascent
                ? AscentBuildSeconds + AscentPeakSeconds
                : ReentryPeakSeconds + ReentryClearSeconds;

        public static VeilPhase PhaseAt(VeilLeg leg, float t)
        {
            if (t < 0f) t = 0f;
            if (t >= TotalSeconds(leg) || t >= HardCapSeconds) return VeilPhase.Done;
            if (leg == VeilLeg.Ascent)
                return t < AscentBuildSeconds ? VeilPhase.Build : VeilPhase.Peak;
            return t < ReentryPeakSeconds ? VeilPhase.Peak : VeilPhase.Clear;
        }

        /// <summary>Veil strength 0..1 at time t. Zero before the start and after the end, so a
        /// caller that keeps ticking can never hold the view hot.</summary>
        public static float Intensity(VeilLeg leg, float t)
        {
            if (t <= 0f) return 0f;
            if (t >= HardCapSeconds) return 0f;
            float total = TotalSeconds(leg);
            if (t >= total) return 0f;

            if (leg == VeilLeg.Ascent)
                return t < AscentBuildSeconds ? Smooth(t / AscentBuildSeconds) : 1f;

            if (t < ReentryPeakSeconds) return 1f;
            return 1f - Smooth((t - ReentryPeakSeconds) / ReentryClearSeconds);
        }

        /// <summary>Smoothstep — no linear ramps on anything the eye tracks.</summary>
        private static float Smooth(float k)
        {
            if (k <= 0f) return 0f;
            if (k >= 1f) return 1f;
            return k * k * (3f - 2f * k);
        }
    }
}

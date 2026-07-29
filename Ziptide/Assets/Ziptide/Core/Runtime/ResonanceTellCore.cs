namespace Ziptide.Core
{
    /// <summary>
    /// Implemented by anything with INSTRUMENTS the artifact can knock out — the vehicle you drove
    /// to the flats, and later anything else with a lit panel. It lives in Core so the tell (which
    /// runs in Gameplay) can reach a vehicle (which lives in Ship) without either assembly having
    /// to reference the other: the effect finds implementors by interface, not by type.
    ///
    /// Contract: <paramref name="power01"/> 0 = dark, 1 = normal, and the caller ALWAYS ends on 1.
    /// Implementations must only change how the thing looks — never whether it works.
    /// </summary>
    public interface IResonanceSensitive
    {
        void SetInstrumentPower(float power01);
    }

    /// <summary>
    /// THE RESONANCE TELL (FIRST_HOUR_DIRECTORS_CUT — "The Key That Knew You"; ⚖ Terry's expedition
    /// staging). Lifting half B off the wreck kills every instrument within reach for a couple of
    /// seconds: the vehicle's lights die, its readouts blank, then everything comes back as if
    /// nothing happened. Nobody explains it. It is the first evidence that the artifact DOES
    /// something, planted long before the join at the berth says what.
    ///
    /// Pure because the shape of the tell is the whole design: it must be short enough to read as
    /// interference rather than a breakdown, it must never leave a system dead, and it must recover
    /// smoothly rather than snapping back. A tell that strands the player's ride is a bug wearing a
    /// story's clothes.
    /// </summary>
    public static class ResonanceTellCore
    {
        /// <summary>Total length of the tell, seconds.</summary>
        public const float DurationSeconds = 2f;

        /// <summary>The dead-flat window at the start, before power crawls back.</summary>
        public const float BlackoutSeconds = 0.8f;

        /// <summary>
        /// Instrument power 0..1 at time t: 0 through the blackout, then a smooth climb back to
        /// full. Always 1 before the tell starts and after it ends — there is no path through this
        /// function that leaves anything permanently dark.
        /// </summary>
        public static float Power(float t)
        {
            if (t <= 0f) return 1f;
            if (t >= DurationSeconds) return 1f;
            if (t < BlackoutSeconds) return 0f;
            float k = (t - BlackoutSeconds) / (DurationSeconds - BlackoutSeconds);
            return k * k * (3f - 2f * k);
        }

        /// <summary>True while the tell is actively suppressing instruments.</summary>
        public static bool IsActive(float t) => t > 0f && t < DurationSeconds;
    }
}

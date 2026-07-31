namespace Ziptide.Core
{
    /// <summary>
    /// THE PLAYER HAS TO BE ABLE TO SEE IT.
    ///
    /// Combat gained stakes today: armor drains, breaking it is a warning, and the next hit kills. But
    /// the model's whole promise — *"armor up = trading, armor broken = get to cover NOW"* — is
    /// meaningless if the state is invisible. Shipping mortality without a readout is not an unfinished
    /// feature, it is an unfair one, and it is a hole this session opened.
    ///
    /// The readout is a peripheral vignette rather than a number, because:
    ///   • text at Quest resolution is unreliable and asks the player to focus on UI mid-fight;
    ///   • the project is diegetic-first, and a floating health bar is the least diegetic thing there is;
    ///   • peripheral vision is exactly where a "you are in danger" signal belongs — you feel it without
    ///     looking at it.
    ///
    /// The load-bearing rule is <see cref="Opacity01"/> returning ZERO at full armor. A permanent tint
    /// over a VR player's view is eye strain, and worse, it destroys the signal: if something is always
    /// there, its arrival means nothing.
    ///
    /// Pure so every one of those claims is a test rather than an opinion.
    /// </summary>
    public static class ArmorHudCore
    {
        /// <summary>Armor fraction at or above which nothing is drawn at all.</summary>
        public const float SilentAbove = 0.999f;

        /// <summary>Opacity of the vignette when armor is fully drained but not yet broken.</summary>
        public const float MaxWoundedOpacity = 0.42f;

        /// <summary>Opacity floor of the broken pulse — it never fully disappears.</summary>
        public const float BrokenPulseMin = 0.30f;

        /// <summary>Opacity peak of the broken pulse.</summary>
        public const float BrokenPulseMax = 0.62f;

        /// <summary>Pulses per second while broken. Urgent, not strobing — strobe is a seizure risk.</summary>
        public const float BrokenPulseHz = 1.6f;

        /// <summary>
        /// How opaque the vignette is for a given armor fraction. Zero at full armor: a healthy player
        /// sees their game, unobstructed, and the tint's ARRIVAL is the information.
        /// </summary>
        public static float Opacity01(float armor01)
        {
            if (armor01 >= SilentAbove) return 0f;
            if (armor01 <= 0f) return MaxWoundedOpacity;
            float missing = 1f - Clamp01(armor01);
            return MaxWoundedOpacity * missing;
        }

        /// <summary>
        /// Broken armor pulses instead of sitting still, because "one more hit kills you" has to read
        /// differently from "you are hurt" — a player who cannot tell those apart will trade when they
        /// should run. Deterministic in time so it is testable.
        /// </summary>
        public static float BrokenPulse01(float timeSeconds)
        {
            double phase = timeSeconds * BrokenPulseHz * 2.0 * System.Math.PI;
            float wave = (float)((System.Math.Sin(phase) + 1.0) * 0.5); // 0..1
            return BrokenPulseMin + (BrokenPulseMax - BrokenPulseMin) * wave;
        }

        /// <summary>The opacity to draw this frame, whichever state the player is in.</summary>
        public static float CurrentOpacity(float armor01, bool broken, float timeSeconds) =>
            broken ? BrokenPulse01(timeSeconds) : Opacity01(armor01);

        /// <summary>
        /// Colour ramp: amber while you still have armor, red once it is gone. Paired with the opacity
        /// and pulse so the state is carried by THREE signals at once — brightness, hue and motion —
        /// and a colour-blind player still reads it.
        /// </summary>
        public static void Tint(float armor01, bool broken,
            out float r, out float g, out float b)
        {
            if (broken) { r = 0.90f; g = 0.12f; b = 0.10f; return; }
            float t = 1f - Clamp01(armor01);              // 0 healthy → 1 drained
            r = 0.95f;
            g = 0.62f - 0.35f * t;                        // amber sliding toward red
            b = 0.20f - 0.12f * t;
        }

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    }
}

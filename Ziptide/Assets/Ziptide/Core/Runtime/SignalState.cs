namespace Ziptide.Core
{
    /// <summary>
    /// Pure derivation of the Signal tier from the SIGNAL_* flags on a <see cref="PlayerProfile"/>.
    /// The Signal is the story's rising pressure (worlds waking up as Cal travels deeper); systems that
    /// react to it — Wardens, tides, world dressing, adaptive audio — read ONE number from here instead
    /// of each re-checking flags. Thresholds are granted by world contracts (W004 → 1, W010 → 2, later
    /// chapters → 3/MAX), never authored directly at runtime. Headless + deterministic — EditMode-tested.
    /// </summary>
    public static class SignalState
    {
        /// <summary>Tier value when SIGNAL_MAX is reached (endgame band).</summary>
        public const int TierMax = 4;

        /// <summary>
        /// Current Signal tier 0–4: 0 = quiet (pre-W004) · 1 = first threshold (Ch.1 capstone) ·
        /// 2 = second (Ch.2, W010) · 3 = third (late-game) · 4 = MAX. Highest granted flag wins, so a
        /// missing intermediate flag never understates the tier.
        /// </summary>
        public static int Tier(PlayerProfile profile)
        {
            if (profile == null) return 0;
            if (profile.HasFlag(ZiptideFlags.SIGNAL_MAX)) return TierMax;
            if (profile.HasFlag(ZiptideFlags.SIGNAL_THRESHOLD_3)) return 3;
            if (profile.HasFlag(ZiptideFlags.SIGNAL_THRESHOLD_2)) return 2;
            if (profile.HasFlag(ZiptideFlags.SIGNAL_THRESHOLD_1)) return 1;
            return 0;
        }
    }
}

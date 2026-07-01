namespace Ziptide.Core
{
    /// <summary>
    /// Pure derivation of the Transmission clarity tier from the fragment flags on a
    /// <see cref="PlayerProfile"/> (spec: docs/storyboard/WORLD_DATA.md §3 / THE_TRANSMISSION.md §3).
    /// Fragments (FRAGMENT_T#_FOUND) are authored per-world; the clarity tier is NEVER authored —
    /// call <see cref="SyncClarityFlags"/> after any flag grant and the derived
    /// TRANSMISSION_CLARITY_* flags stay consistent. The de-garble playback UI reads the tier to pick
    /// how legible the assembled message sounds. Headless + deterministic — EditMode-tested.
    /// </summary>
    public static class TransmissionProgress
    {
        /// <summary>Tier value for the full assembly (MAX).</summary>
        public const int TierMax = 4;

        /// <summary>
        /// Current clarity tier 0–4 from the profile's found fragments:
        /// 0 = nothing legible · 1 = after T1 (static, professional) · 2 = after T2 (half-legible,
        /// confessional) · 3 = after T3 or T4 (clear, intimate) · 4 = MAX (T1–T5 + the RILL confession —
        /// the full message, endgame only). Tiers don't require the lower fragments — clarity comes from
        /// the strongest fragment recovered, matching the "harder to reach late fragments" design.
        /// </summary>
        public static int ComputeTier(PlayerProfile profile)
        {
            if (profile == null) return 0;

            bool t1 = profile.HasFlag(ZiptideFlags.FRAGMENT_T1_FOUND);
            bool t2 = profile.HasFlag(ZiptideFlags.FRAGMENT_T2_FOUND);
            bool t3 = profile.HasFlag(ZiptideFlags.FRAGMENT_T3_FOUND);
            bool t4 = profile.HasFlag(ZiptideFlags.FRAGMENT_T4_FOUND);
            bool t5 = profile.HasFlag(ZiptideFlags.FRAGMENT_T5_FOUND);
            bool rill = profile.HasFlag(ZiptideFlags.FRAGMENT_RILL_CONFESS);

            if (t1 && t2 && t3 && t4 && t5 && rill) return TierMax;
            if (t3 || t4) return 3;
            if (t2) return 2;
            if (t1) return 1;
            return 0;
        }

        /// <summary>
        /// Set the derived TRANSMISSION_CLARITY_* flags to match <see cref="ComputeTier"/> (cumulative:
        /// tier 3 implies clarity 1–3 are all set). Idempotent; never clears a tier (clarity can only
        /// rise — memory doesn't re-garble). Returns how many clarity flags were newly set so the caller
        /// can log/skip a no-op sync.
        /// </summary>
        public static int SyncClarityFlags(PlayerProfile profile)
        {
            if (profile == null) return 0;

            int tier = ComputeTier(profile);
            int newlySet = 0;
            if (tier >= 1) newlySet += SetIfMissing(profile, ZiptideFlags.TRANSMISSION_CLARITY_1);
            if (tier >= 2) newlySet += SetIfMissing(profile, ZiptideFlags.TRANSMISSION_CLARITY_2);
            if (tier >= 3) newlySet += SetIfMissing(profile, ZiptideFlags.TRANSMISSION_CLARITY_3);
            if (tier >= TierMax) newlySet += SetIfMissing(profile, ZiptideFlags.TRANSMISSION_CLARITY_MAX);
            return newlySet;
        }

        private static int SetIfMissing(PlayerProfile profile, string flag)
        {
            if (profile.HasFlag(flag)) return 0;
            profile.SetFlag(flag);
            return 1;
        }
    }
}

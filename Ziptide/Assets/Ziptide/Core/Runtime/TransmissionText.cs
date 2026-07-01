namespace Ziptide.Core
{
    /// <summary>
    /// The Transmission's PLAYBACK TEXT per clarity tier (GAME_PLAN M1 stub; full audio lands at the
    /// M6 pass — same tiers, recorded lines). Pure and CI-tested, like TransmissionProgress. The
    /// register arc is canon (THE_TRANSMISSION.md §3): static → professional → confessional → intimate
    /// → the name moment. Diegetically the recording never changes — Cal's ability to HEAR it does.
    /// </summary>
    public static class TransmissionText
    {
        /// <summary>Playback text for a clarity tier (from <see cref="TransmissionProgress.ComputeTier"/>).</summary>
        public static string Render(int tier)
        {
            if (tier < 0) tier = 0;
            switch (tier)
            {
                case 0:
                    return "▒▒▒▒▒ ▒▒▒ ▒▒▒▒▒▒▒ ▒▒ ▒▒▒▒▒▒▒▒\n" +
                           "▒▒▒▒ ▒▒▒▒▒▒ ▒▒▒▒▒ ▒▒▒▒▒▒ ▒▒▒\n" +
                           "( signal too degraded — recover a fragment )";
                case 1:
                    // Professional register, barely surfacing: something is addressed to someone.
                    return "…▒▒▒ addressed to ▒▒▒ watchers ▒▒▒▒▒\n" +
                           "…record ▒▒▒▒ stable ▒▒▒ hold onto ▒▒▒▒\n" +
                           "…▒▒▒▒▒ do not ▒▒▒▒▒▒ the seal ▒▒▒▒▒▒";
                case 2:
                    // The professional distance breaks — "the subject" becomes "you."
                    return "…this message is addressed to the watchers — no. No.\n" +
                           "It is addressed to you. ▒▒▒ know you're scared.\n" +
                           "Here is something stable to hold onto: the work was ▒▒▒▒▒, and you chose it…";
                case 3:
                    // Intimate — "you" becomes "I."
                    return "…I did this. I chose this. I need us to remember why.\n" +
                           "The gates were never the experiment. The forgetting was.\n" +
                           "When you find the last of these, listen for the name…";
                default:
                    // Full assembly — the name moment + the coordinate (endgame only).
                    return "…you found all of them. I wasn't sure you would. I'm glad.\n" +
                           "Your name — my name — is Cal. I recorded this so I would hear it again.\n" +
                           "Now listen carefully. The coordinate is…";
            }
        }
    }
}

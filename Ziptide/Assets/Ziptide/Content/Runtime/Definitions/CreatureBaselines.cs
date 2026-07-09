namespace Ziptide.Content
{
    /// <summary>
    /// Canonical creature max-health on the UNIFIED combat scale (COMBAT_HEALTH_PLAN A3). Before A3,
    /// creatures lived on their own scale (weapon damage hardcoded taser=10/gravity=8, hp≈8–60) while
    /// PvP used small ints (taser=2, hp=6) — the same trigger pull meant different things. A3 routes
    /// creature damage through PvpCombatant.DamageFor (the PvP table), so creature HEALTH has to move
    /// onto that same integer scale or every creature turns ~5× tanky.
    ///
    /// This is the SINGLE source for those health values: read by CreatureVariantAuthor (fresh
    /// authoring) and CreatureStatRebaseline (migrating the already-committed assets). Pure — no Unity,
    /// so it's unit-testable. Numbers are starting baselines; Terry tunes time-to-kill on device.
    /// </summary>
    public static class CreatureBaselines
    {
        /// <summary>Bump when the health scale changes. A CreatureDefinition whose statScaleVersion is
        /// below this gets migrated to <see cref="HealthFor"/> at build; one already at this version is
        /// left alone (hand-tuning is preserved — "existing assets are the editable truth").</summary>
        public const int StatScaleVersion = 1;

        /// <summary>Health for an unlisted creature — a mid-weight body, roughly as tough as a player.</summary>
        public const float DefaultHealth = 6f;

        /// <summary>Max health on the unified scale for a known creature id (else the default). Taser
        /// deals 2, so the comment on each line is its rough taser time-to-kill.</summary>
        public static float HealthFor(string id)
        {
            switch (id)
            {
                case "swarm_bug":    return 4f;   // ~2 taser hits — a fragile swarmer
                case "witness_mite": return 4f;   // ~2 hits
                case "light_grazer": return 6f;   // ~3 hits
                case "tether_swarm": return 6f;   // ~3 hits
                case "tendril":      return 8f;   // ~4 hits — a tougher wall-crawler
                case "husk_molter":  return 8f;   // ~4 hits
                case "warden":       return 20f;  // ~10 hits — a lawful bruiser / mini-boss
                default:             return DefaultHealth;
            }
        }
    }
}

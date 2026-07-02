namespace Ziptide.Multiplayer.Conquest
{
    /// <summary>
    /// Every tunable number in the war, in one place (the PvpRules pattern). Odds math per
    /// docs/10_TIDEFRONT.md: equal scores = 50%, each net point ≈ +5%, clamped 10–90% so nothing is
    /// ever safe and nothing is ever hopeless. Anti-snowball levers (upkeep, instability, dogpile)
    /// are baked in from day one per docs/design/TIDEFRONT_AAA.md.
    /// </summary>
    public static class ConquestRules
    {
        // Odds
        public const float OddsMin = 0.10f;
        public const float OddsMax = 0.90f;
        public const float OddsPerPoint = 0.05f;

        // Outcome classification (relative to the roll vs odds)
        public const float MajorVictoryFraction = 0.5f;   // roll under odds×this = major
        public const float StalemateWindow = 0.15f;       // roll within this past odds = stalemate
        public const float FailedWindow = 0.35f;          // within this past odds = failed; beyond = counterstrike

        // Anti-snowball
        public const int MaxAttacksPerTurn = 2;
        public const int VesselsPerUpkeepFlux = 4;         // 1 flux upkeep per 4 vessels per turn
        public const float CaptureInstability = 0.5f;      // fresh conquests are unstable
        public const float InstabilityDecayPerTurn = 0.1f;
        public const float InstabilityProductionPenalty = 0.5f; // at 1.0 instability, produce 50% less
        public const int DogpileBonus = 3;                  // defender bonus once dogpiled
        public const int DogpileThreshold = 2;              // attacks on the same planet in one turn

        // Battle losses
        public const float CostlyVictoryLossFraction = 0.5f;
        public const float StalemateLossFraction = 0.5f;

        public static float ComputeOdds(int attackScore, int defenseScore)
        {
            float odds = 0.5f + (attackScore - defenseScore) * OddsPerPoint;
            if (odds < OddsMin) return OddsMin;
            if (odds > OddsMax) return OddsMax;
            return odds;
        }
    }
}

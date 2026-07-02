using System;

namespace Ziptide.Multiplayer.Bots
{
    /// <summary>
    /// Difficulty as DATA — every number that separates a Rookie from a Nightmare bot
    /// (design table: docs/design/PVP_ARENA_AAA.md §A1). Pure struct so the brain is headless-testable;
    /// the Content-side <c>BotProfileDefinition</c> SO mirrors these fields for designer tuning and the
    /// scene layer copies them across at spawn (the PvpRules pattern).
    /// </summary>
    [Serializable]
    public struct BotProfileData
    {
        public float AimErrorDegrees;     // cone the shot may deviate by
        public float ReactionSeconds;     // delay between first-sight and acting on it
        public bool LeadTargets;          // project target velocity for slow projectiles
        public float DodgeChance;         // 0..1 roll per incoming threat
        public float CoverDiscipline;     // 0..1 chance to break for cover when damaged/recharging
        public float PeekSeconds;         // exposure window from cover
        public float HideSeconds;         // time behind cover between peeks
        public int RetreatBelowHP;        // 0 = never retreats
        public float FireCooldownScale;   // × PvpRules cadence (lower = faster)
        public float StrafeFlipSeconds;   // how often the strafe direction flips in Engage
        public float EngageStandoff;      // preferred fighting distance (m)
        public float EngageBand;          // acceptable ± around the standoff (m)
        public float FireRange;           // max shot distance (m)
        public float RushRange;           // closer than this → Rush aggression (m)
        public float SearchSeconds;       // how long Hunt lingers at a cold trail before Patrol
        public float RepositionEvery;     // seconds of Engage before flanking to a new angle

        public static BotProfileData Rookie => new BotProfileData
        {
            AimErrorDegrees = 8f, ReactionSeconds = 0.9f, LeadTargets = false, DodgeChance = 0f,
            CoverDiscipline = 0.2f, PeekSeconds = 1.5f, HideSeconds = 2.5f, RetreatBelowHP = 0,
            FireCooldownScale = 1.5f, StrafeFlipSeconds = 2.2f, EngageStandoff = 8f, EngageBand = 1.5f,
            FireRange = 14f, RushRange = 2.5f, SearchSeconds = 3f, RepositionEvery = 12f,
        };

        public static BotProfileData Regular => new BotProfileData
        {
            AimErrorDegrees = 5f, ReactionSeconds = 0.6f, LeadTargets = false, DodgeChance = 0.25f,
            CoverDiscipline = 0.5f, PeekSeconds = 1.2f, HideSeconds = 2f, RetreatBelowHP = 2,
            FireCooldownScale = 1.2f, StrafeFlipSeconds = 1.6f, EngageStandoff = 8f, EngageBand = 1.5f,
            FireRange = 14f, RushRange = 3f, SearchSeconds = 4f, RepositionEvery = 9f,
        };

        public static BotProfileData Veteran => new BotProfileData
        {
            AimErrorDegrees = 2.5f, ReactionSeconds = 0.35f, LeadTargets = true, DodgeChance = 0.6f,
            CoverDiscipline = 0.8f, PeekSeconds = 0.9f, HideSeconds = 1.2f, RetreatBelowHP = 2,
            FireCooldownScale = 1f, StrafeFlipSeconds = 1.1f, EngageStandoff = 9f, EngageBand = 2f,
            FireRange = 15f, RushRange = 3.5f, SearchSeconds = 5f, RepositionEvery = 7f,
        };

        public static BotProfileData Nightmare => new BotProfileData
        {
            AimErrorDegrees = 1f, ReactionSeconds = 0.2f, LeadTargets = true, DodgeChance = 0.9f,
            CoverDiscipline = 1f, PeekSeconds = 0.7f, HideSeconds = 0.8f, RetreatBelowHP = 3,
            FireCooldownScale = 0.8f, StrafeFlipSeconds = 0.8f, EngageStandoff = 10f, EngageBand = 2.5f,
            FireRange = 16f, RushRange = 4f, SearchSeconds = 6f, RepositionEvery = 5f,
        };
    }
}

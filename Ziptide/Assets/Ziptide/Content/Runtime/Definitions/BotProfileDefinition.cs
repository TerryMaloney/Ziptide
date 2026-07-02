using UnityEngine;
using Ziptide.Multiplayer.Bots;

namespace Ziptide.Content
{
    /// <summary>
    /// Bot difficulty as a designer-tunable asset (Resources/Bots/&lt;name&gt; — rookie/regular/veteran/
    /// nightmare). Mirrors the pure <see cref="BotProfileData"/> the brain runs on; PvpBot loads the
    /// asset by its difficulty string and falls back to the code presets if it's missing. Tune a fight
    /// by editing numbers here — no code.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Bot Profile", fileName = "BotProfile")]
    public class BotProfileDefinition : ScriptableObject
    {
        [Header("Perception")]
        [Tooltip("Seconds between first sighting you and acting on it.")]
        public float reactionSeconds = 0.6f;

        [Header("Aim")]
        [Tooltip("Shot deviation cone in degrees (0 = laser-perfect).")]
        public float aimErrorDegrees = 5f;
        [Tooltip("Project your movement so the slow bolt meets you (Veteran+ behavior).")]
        public bool leadTargets = false;

        [Header("Defense")]
        [Range(0f, 1f)] public float dodgeChance = 0.25f;
        [Range(0f, 1f)] [Tooltip("Chance to break for cover when damaged/recharging.")]
        public float coverDiscipline = 0.5f;
        public float peekSeconds = 1.2f;
        public float hideSeconds = 2f;
        [Tooltip("Retreat (while still fighting) at or below this HP. 0 = never.")]
        public int retreatBelowHP = 2;

        [Header("Offense")]
        [Tooltip("Multiplies the telegraph/cadence (lower = faster fights).")]
        public float fireCooldownScale = 1.2f;
        public float engageStandoff = 8f;
        public float engageBand = 1.5f;
        public float fireRange = 14f;
        public float rushRange = 3f;

        [Header("Movement")]
        public float strafeFlipSeconds = 1.6f;
        public float searchSeconds = 4f;
        [Tooltip("Seconds of face-to-face fighting before flanking to a new angle.")]
        public float repositionEvery = 9f;

        public BotProfileData ToData() => new BotProfileData
        {
            AimErrorDegrees = aimErrorDegrees,
            ReactionSeconds = reactionSeconds,
            LeadTargets = leadTargets,
            DodgeChance = dodgeChance,
            CoverDiscipline = coverDiscipline,
            PeekSeconds = peekSeconds,
            HideSeconds = hideSeconds,
            RetreatBelowHP = retreatBelowHP,
            FireCooldownScale = fireCooldownScale,
            StrafeFlipSeconds = strafeFlipSeconds,
            EngageStandoff = engageStandoff,
            EngageBand = engageBand,
            FireRange = fireRange,
            RushRange = rushRange,
            SearchSeconds = searchSeconds,
            RepositionEvery = repositionEvery,
        };
    }
}

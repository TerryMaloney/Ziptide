using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>The nine economy families (Meta-Loop A). MultiplayerToken is registered-but-reserved.</summary>
    public enum ResourceCategory
    {
        Bio, Mineral, Salvage, Energy, Artifact, Data, CraftedComponent, DefenseComponent, MultiplayerToken
    }

    /// <summary>
    /// META-LOOP A — one registered definition per resource id (assets in Resources/Economy, seeded
    /// create-only by ResourceAuthor). Raw resource strings anywhere in content that don't resolve
    /// here FAIL the build (EconomyAuditRules.RESOURCE_ID_UNREGISTERED) — the one-economy law.
    /// "Used by" is never stored: the economy flow report COMPUTES sources/sinks from recipes,
    /// plants, jobs and mines, so there is no denormalized second truth.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceDefinition", menuName = "Ziptide/Definitions/Resource")]
    public class ResourceDefinition : Definition
    {
        [Header("Identity")]
        public ResourceCategory category = ResourceCategory.Mineral;
        [Tooltip("Worlds this resource canonically comes from (staleness: a W-id change flags this).")]
        public List<string> sourceWorlds = new List<string>();
        [Tooltip("Story-bible tags (staleness: a story-direction change flags everything tagged).")]
        public List<string> storyTags = new List<string>();

        [Header("Rules")]
        [Range(0, 3)] public int rarity = 0;             // 0 common … 3 unique
        [Tooltip("0 = unstacked/uncapped.")]
        public double storageCap = 0;
        public bool multiplayerUse = false;

        [Header("Balance hooks (structure now, numbers later — Meta-Loop guardrail)")]
        public double expectedSourcePerHour = 0;
        public double expectedSinkPerHour = 0;
        [Tooltip("earliest progression phase this should appear: 0 early / 1 mid / 2 late.")]
        [Range(0, 2)] public int progressionPhase = 0;
    }
}

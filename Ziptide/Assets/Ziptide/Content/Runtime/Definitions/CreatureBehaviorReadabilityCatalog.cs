using System;
using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>
    /// Machine-readable quality metadata for one shipped creature behavior. This is not a runtime
    /// behavior owner: CityBuilder and CreatureBehaviorBase subclasses still instantiate and execute
    /// every creature. The profile exists so CI can enforce a minimum readable behavior vocabulary.
    /// </summary>
    public sealed class CreatureBehaviorReadabilityProfile
    {
        public string CreatureId { get; }
        public CreatureArchetype ExpectedArchetype { get; }
        public string BehaviorTypeName { get; }
        public IReadOnlyList<string> ActiveStates { get; }
        public string TelegraphState { get; }
        public string CounterState { get; }
        public string DisabledState { get; }

        public CreatureBehaviorReadabilityProfile(
            string creatureId,
            CreatureArchetype expectedArchetype,
            string behaviorTypeName,
            string[] activeStates,
            string telegraphState,
            string counterState,
            string disabledState)
        {
            CreatureId = creatureId ?? string.Empty;
            ExpectedArchetype = expectedArchetype;
            BehaviorTypeName = behaviorTypeName ?? string.Empty;
            ActiveStates = Array.AsReadOnly(activeStates != null
                ? (string[])activeStates.Clone()
                : Array.Empty<string>());
            TelegraphState = telegraphState ?? string.Empty;
            CounterState = counterState ?? string.Empty;
            DisabledState = disabledState ?? string.Empty;
        }
    }

    /// <summary>
    /// The shipped creature roster's readable behavior vocabulary. Adding a CreatureDefinition without
    /// adding an honest profile here fails EditMode CI. State names summarize existing M3 behavior and
    /// deliberately do not drive motion, damage, rewards, visuals or respawn.
    /// </summary>
    public static class CreatureBehaviorReadabilityCatalog
    {
        public const int MinimumActiveStates = 3;

        private static readonly CreatureBehaviorReadabilityProfile[] Profiles =
        {
            new CreatureBehaviorReadabilityProfile(
                "swarm_bug",
                CreatureArchetype.Swarmer,
                "SwarmerBehavior",
                new[] { "patrol_orbit", "gather_telegraph", "dart_attack" },
                "gather_telegraph",
                "gather_telegraph",
                "stunned_down"),

            new CreatureBehaviorReadabilityProfile(
                "tendril",
                CreatureArchetype.WallCrawler,
                "WallCrawlerBehavior",
                new[] { "surface_patrol", "ripple_telegraph", "drop_lunge" },
                "ripple_telegraph",
                "ripple_telegraph",
                "stunned_grounded"),

            new CreatureBehaviorReadabilityProfile(
                "light_grazer",
                CreatureArchetype.Swarmer,
                "LightGrazerBehavior",
                new[] { "dark_idle_grow", "dark_approach", "lit_shrink_recoil" },
                "lit_shrink_recoil",
                "lit_shrink_recoil",
                "stunned_down"),

            new CreatureBehaviorReadabilityProfile(
                "witness_mite",
                CreatureArchetype.Swarmer,
                "WitnessMiteBehavior",
                new[] { "unobserved_idle", "unobserved_stalk", "observed_freeze" },
                "observed_freeze",
                "observed_freeze",
                "stunned_down"),

            new CreatureBehaviorReadabilityProfile(
                "tether_swarm",
                CreatureArchetype.Swarmer,
                "TetherSwarmBehavior",
                new[] { "cluster_weave", "tether_node_exposed", "tether_severed" },
                "tether_node_exposed",
                "tether_node_exposed",
                "colony_disabled"),

            new CreatureBehaviorReadabilityProfile(
                "husk_molter",
                CreatureArchetype.WallCrawler,
                "HuskMolterBehavior",
                new[] { "stalk", "molt_escape", "cooldown_vulnerable" },
                "molt_escape",
                "cooldown_vulnerable",
                "stunned_down"),

            new CreatureBehaviorReadabilityProfile(
                "warden",
                CreatureArchetype.Bruiser,
                "WardenBehavior",
                new[] { "watch", "warn", "arrest_disengage", "ally_calm" },
                "warn",
                "ally_calm",
                "stunned_stand_down"),
        };

        private static readonly Dictionary<string, CreatureBehaviorReadabilityProfile> ById = BuildLookup();

        public static IReadOnlyList<CreatureBehaviorReadabilityProfile> All => Profiles;

        public static bool TryGet(string creatureId, out CreatureBehaviorReadabilityProfile profile)
        {
            if (string.IsNullOrEmpty(creatureId))
            {
                profile = null;
                return false;
            }
            return ById.TryGetValue(creatureId, out profile);
        }

        /// <summary>Returns stable error tokens; an empty list means the profile satisfies the v1 law.</summary>
        public static IReadOnlyList<string> Validate(CreatureBehaviorReadabilityProfile profile)
        {
            var errors = new List<string>();
            if (profile == null)
            {
                errors.Add("PROFILE_NULL");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(profile.CreatureId)) errors.Add("CREATURE_ID_EMPTY");
            if (string.IsNullOrWhiteSpace(profile.BehaviorTypeName)) errors.Add("BEHAVIOR_TYPE_EMPTY");

            var states = new HashSet<string>(StringComparer.Ordinal);
            foreach (string state in profile.ActiveStates)
            {
                if (string.IsNullOrWhiteSpace(state))
                {
                    errors.Add("ACTIVE_STATE_EMPTY");
                    continue;
                }
                if (!states.Add(state)) errors.Add("ACTIVE_STATE_DUPLICATE:" + state);
            }

            if (states.Count < MinimumActiveStates)
                errors.Add("ACTIVE_STATE_COUNT_LOW:" + states.Count);

            if (!states.Contains(profile.TelegraphState))
                errors.Add("TELEGRAPH_NOT_ACTIVE:" + profile.TelegraphState);
            if (!states.Contains(profile.CounterState))
                errors.Add("COUNTER_NOT_ACTIVE:" + profile.CounterState);

            if (string.IsNullOrWhiteSpace(profile.DisabledState))
                errors.Add("DISABLED_STATE_EMPTY");
            else if (states.Contains(profile.DisabledState))
                errors.Add("DISABLED_STATE_DUPLICATES_ACTIVE:" + profile.DisabledState);

            return errors;
        }

        private static Dictionary<string, CreatureBehaviorReadabilityProfile> BuildLookup()
        {
            var lookup = new Dictionary<string, CreatureBehaviorReadabilityProfile>(StringComparer.Ordinal);
            foreach (var profile in Profiles)
            {
                if (profile == null || string.IsNullOrEmpty(profile.CreatureId)) continue;
                lookup.Add(profile.CreatureId, profile);
            }
            return lookup;
        }
    }
}

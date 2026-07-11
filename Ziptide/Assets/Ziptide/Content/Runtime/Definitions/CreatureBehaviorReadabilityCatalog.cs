using System;
using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>One active readable state and the exact behavior-source token that proves it exists.</summary>
    public sealed class CreatureBehaviorStateEvidence
    {
        public string StateName { get; }
        public string EvidenceToken { get; }

        public CreatureBehaviorStateEvidence(string stateName, string evidenceToken)
        {
            StateName = stateName ?? string.Empty;
            EvidenceToken = evidenceToken ?? string.Empty;
        }
    }

    /// <summary>
    /// Machine-readable quality metadata for one shipped creature behavior. This is not a runtime
    /// behavior owner: CityBuilder and CreatureBehaviorBase subclasses still instantiate and execute
    /// every creature. The profile exists so CI/build gates can enforce a readable behavior vocabulary.
    /// </summary>
    public sealed class CreatureBehaviorReadabilityProfile
    {
        public string CreatureId { get; }
        public CreatureArchetype ExpectedArchetype { get; }
        public string BehaviorTypeName { get; }
        public string BehaviorSourceRelativePath { get; }
        public string FactoryEvidenceToken { get; }
        public IReadOnlyList<CreatureBehaviorStateEvidence> ActiveStateEvidence { get; }
        public IReadOnlyList<string> ActiveStates { get; }
        public string TelegraphState { get; }
        public string CounterState { get; }
        public string DisabledState { get; }

        /// <summary>Compatibility constructor for validation fixtures and lightweight future callers.</summary>
        public CreatureBehaviorReadabilityProfile(
            string creatureId,
            CreatureArchetype expectedArchetype,
            string behaviorTypeName,
            string[] activeStates,
            string telegraphState,
            string counterState,
            string disabledState)
            : this(
                creatureId,
                expectedArchetype,
                behaviorTypeName,
                string.Empty,
                string.Empty,
                ToEvidence(activeStates),
                telegraphState,
                counterState,
                disabledState)
        {
        }

        public CreatureBehaviorReadabilityProfile(
            string creatureId,
            CreatureArchetype expectedArchetype,
            string behaviorTypeName,
            string behaviorSourceRelativePath,
            string factoryEvidenceToken,
            CreatureBehaviorStateEvidence[] activeStateEvidence,
            string telegraphState,
            string counterState,
            string disabledState)
        {
            CreatureId = creatureId ?? string.Empty;
            ExpectedArchetype = expectedArchetype;
            BehaviorTypeName = behaviorTypeName ?? string.Empty;
            BehaviorSourceRelativePath = behaviorSourceRelativePath ?? string.Empty;
            FactoryEvidenceToken = factoryEvidenceToken ?? string.Empty;

            var evidence = activeStateEvidence != null
                ? (CreatureBehaviorStateEvidence[])activeStateEvidence.Clone()
                : Array.Empty<CreatureBehaviorStateEvidence>();
            ActiveStateEvidence = Array.AsReadOnly(evidence);

            var names = new string[evidence.Length];
            for (int i = 0; i < evidence.Length; i++)
                names[i] = evidence[i] != null ? evidence[i].StateName : string.Empty;
            ActiveStates = Array.AsReadOnly(names);

            TelegraphState = telegraphState ?? string.Empty;
            CounterState = counterState ?? string.Empty;
            DisabledState = disabledState ?? string.Empty;
        }

        private static CreatureBehaviorStateEvidence[] ToEvidence(string[] states)
        {
            if (states == null) return Array.Empty<CreatureBehaviorStateEvidence>();
            var evidence = new CreatureBehaviorStateEvidence[states.Length];
            for (int i = 0; i < states.Length; i++)
                evidence[i] = new CreatureBehaviorStateEvidence(states[i], string.Empty);
            return evidence;
        }
    }

    /// <summary>
    /// The shipped creature roster's single readable-behavior source of truth. State names summarize
    /// existing M3 behavior and do not drive motion, damage, rewards, visuals or respawn.
    /// </summary>
    public static class CreatureBehaviorReadabilityCatalog
    {
        public const int MinimumActiveStates = 3;
        private const string EnemySourceRoot = "Ziptide/Gameplay/Runtime/Enemies/";

        private static readonly CreatureBehaviorReadabilityProfile[] Profiles =
        {
            Profile(
                "swarm_bug",
                CreatureArchetype.Swarmer,
                "SwarmerBehavior",
                "go.AddComponent<SwarmerBehavior>();",
                new[]
                {
                    State("patrol_orbit", "_orbitAngle += 40f"),
                    State("gather_telegraph", "_dartUntil = Time.time + 0.7f"),
                    State("dart_attack", "if (darting)"),
                },
                "gather_telegraph",
                "gather_telegraph",
                "stunned_down"),

            Profile(
                "tendril",
                CreatureArchetype.WallCrawler,
                "WallCrawlerBehavior",
                "go.AddComponent<WallCrawlerBehavior>();",
                new[]
                {
                    State("surface_patrol", "case Mode.OnWall:"),
                    State("ripple_telegraph", "case Mode.Telegraph:"),
                    State("drop_lunge", "case Mode.Lunge:"),
                    State("return_to_wall", "case Mode.Return:"),
                },
                "ripple_telegraph",
                "ripple_telegraph",
                "stunned_grounded"),

            Profile(
                "light_grazer",
                CreatureArchetype.Swarmer,
                "LightGrazerBehavior",
                "case \"light_grazer\": go.AddComponent<LightGrazerBehavior>(); return;",
                new[]
                {
                    State("dark_idle_grow", "lit ? -shrinkPerSecond : growPerSecond"),
                    State("dark_approach", "if (!lit && Player != null && dist <= detectRange)"),
                    State("lit_shrink_recoil", "else if (lit)"),
                },
                "lit_shrink_recoil",
                "lit_shrink_recoil",
                "stunned_down"),

            Profile(
                "witness_mite",
                CreatureArchetype.Swarmer,
                "WitnessMiteBehavior",
                "case \"witness_mite\": go.AddComponent<WitnessMiteBehavior>(); return;",
                new[]
                {
                    State("unobserved_idle", "SetFrozen(false);"),
                    State("unobserved_stalk", "if (Player != null && dist <= detectRange)"),
                    State("observed_freeze", "if (observed)"),
                },
                "observed_freeze",
                "observed_freeze",
                "stunned_down"),

            Profile(
                "tether_swarm",
                CreatureArchetype.Swarmer,
                "TetherSwarmBehavior",
                "case \"tether_swarm\": go.AddComponent<TetherSwarmBehavior>(); return;",
                new[]
                {
                    State("cluster_weave", "_clusterA.localPosition = new Vector3"),
                    State("engaged_standoff_weave", "if (Player != null && dist <= detectRange)"),
                    State("tether_node_exposed", "_node.localPosition = new Vector3"),
                },
                "tether_node_exposed",
                "tether_node_exposed",
                "colony_disabled"),

            Profile(
                "husk_molter",
                CreatureArchetype.WallCrawler,
                "HuskMolterBehavior",
                "case \"husk_molter\": go.AddComponent<HuskMolterBehavior>(); return;",
                new[]
                {
                    State("stalk", "if (Player != null && dist <= detectRange)"),
                    State("molt_escape", "Debug.Log(\"ZIPTIDE: HUSK_MOLT\");"),
                    State("cooldown_vulnerable", "Time.time < _nextMoltAllowed"),
                },
                "molt_escape",
                "cooldown_vulnerable",
                "stunned_down"),

            Profile(
                "warden",
                CreatureArchetype.Bruiser,
                "WardenBehavior",
                "case \"warden\": go.AddComponent<WardenBehavior>(); return;",
                new[]
                {
                    State("watch", "case WardenMode.Watch:"),
                    State("warn", "case WardenMode.Warn:"),
                    State("arrest_disengage", "Debug.Log(\"ZIPTIDE: WARDEN_ARREST\");"),
                    State("ally_calm", "case WardenMode.Ally:"),
                },
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

        private static CreatureBehaviorReadabilityProfile Profile(
            string creatureId,
            CreatureArchetype expectedArchetype,
            string behaviorTypeName,
            string factoryEvidenceToken,
            CreatureBehaviorStateEvidence[] states,
            string telegraphState,
            string counterState,
            string disabledState)
        {
            return new CreatureBehaviorReadabilityProfile(
                creatureId,
                expectedArchetype,
                behaviorTypeName,
                EnemySourceRoot + behaviorTypeName + ".cs",
                factoryEvidenceToken,
                states,
                telegraphState,
                counterState,
                disabledState);
        }

        private static CreatureBehaviorStateEvidence State(string name, string evidenceToken)
        {
            return new CreatureBehaviorStateEvidence(name, evidenceToken);
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

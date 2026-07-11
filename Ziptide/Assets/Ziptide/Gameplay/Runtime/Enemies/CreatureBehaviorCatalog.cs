using System;
using System.Collections.Generic;

namespace Ziptide.Gameplay
{
    /// <summary>One readable species-specific mode and the exact source token that proves it exists.</summary>
    public sealed class ReadableCreatureMode
    {
        public string Name { get; }
        public string EvidenceToken { get; }

        public ReadableCreatureMode(string name, string evidenceToken)
        {
            Name = name ?? "";
            EvidenceToken = evidenceToken ?? "";
        }
    }

    /// <summary>
    /// Structural behavior contract for one shipped creature id. This metadata does not drive runtime
    /// motion; it lets editor gates prove that the authored asset, behavior source and factory wiring
    /// still agree and that the species has a readable vocabulary of at least three modes.
    /// </summary>
    public sealed class CreatureBehaviorProfile
    {
        public string CreatureId { get; }
        public Type BehaviorType { get; }
        public string BehaviorSourceRelativePath { get; }
        public string FactoryEvidenceToken { get; }
        public IReadOnlyList<ReadableCreatureMode> Modes { get; }

        public CreatureBehaviorProfile(
            string creatureId,
            Type behaviorType,
            string behaviorSourceRelativePath,
            string factoryEvidenceToken,
            params ReadableCreatureMode[] modes)
        {
            CreatureId = creatureId ?? "";
            BehaviorType = behaviorType;
            BehaviorSourceRelativePath = behaviorSourceRelativePath ?? "";
            FactoryEvidenceToken = factoryEvidenceToken ?? "";
            Modes = modes ?? Array.Empty<ReadableCreatureMode>();
        }
    }

    /// <summary>
    /// Evidence-backed vocabulary for every committed story creature. Generic stun/down/respawn states
    /// intentionally live outside this catalog because they do not make one species distinct from another.
    /// </summary>
    public static class CreatureBehaviorCatalog
    {
        private const string EnemySourceRoot = "Ziptide/Gameplay/Runtime/Enemies/";

        private static readonly CreatureBehaviorProfile[] Profiles =
        {
            new CreatureBehaviorProfile(
                "swarm_bug",
                typeof(SwarmerBehavior),
                EnemySourceRoot + "SwarmerBehavior.cs",
                "go.AddComponent<SwarmerBehavior>();",
                Mode("idle_skitter", "_orbitAngle += 25f"),
                Mode("orbit_standoff", "_orbitAngle += 40f"),
                Mode("gather_and_dart", "_dartUntil = Time.time + 0.7f")),

            new CreatureBehaviorProfile(
                "tendril",
                typeof(WallCrawlerBehavior),
                EnemySourceRoot + "WallCrawlerBehavior.cs",
                "go.AddComponent<WallCrawlerBehavior>();",
                Mode("wall_stalk", "case Mode.OnWall:"),
                Mode("ripple_telegraph", "case Mode.Telegraph:"),
                Mode("drop_lunge", "case Mode.Lunge:"),
                Mode("return_to_wall", "case Mode.Return:")),

            new CreatureBehaviorProfile(
                "light_grazer",
                typeof(LightGrazerBehavior),
                EnemySourceRoot + "LightGrazerBehavior.cs",
                "case \"light_grazer\": go.AddComponent<LightGrazerBehavior>(); return;",
                Mode("dark_growth", "lit ? -shrinkPerSecond : growPerSecond"),
                Mode("dark_advance", "if (!lit && Player != null && dist <= detectRange)"),
                Mode("lit_shrink_recoil", "else if (lit)")),

            new CreatureBehaviorProfile(
                "witness_mite",
                typeof(WitnessMiteBehavior),
                EnemySourceRoot + "WitnessMiteBehavior.cs",
                "case \"witness_mite\": go.AddComponent<WitnessMiteBehavior>(); return;",
                Mode("observed_freeze", "if (observed)"),
                Mode("unobserved_ready", "SetFrozen(false);"),
                Mode("unobserved_stalk", "if (Player != null && dist <= detectRange)")),

            new CreatureBehaviorProfile(
                "tether_swarm",
                typeof(TetherSwarmBehavior),
                EnemySourceRoot + "TetherSwarmBehavior.cs",
                "case \"tether_swarm\": go.AddComponent<TetherSwarmBehavior>(); return;",
                Mode("ambient_pair_weave", "_clusterA.localPosition = new Vector3"),
                Mode("engaged_standoff_weave", "if (Player != null && dist <= detectRange)"),
                Mode("vulnerable_tether_tell", "_node.localPosition = new Vector3")),

            new CreatureBehaviorProfile(
                "husk_molter",
                typeof(HuskMolterBehavior),
                EnemySourceRoot + "HuskMolterBehavior.cs",
                "case \"husk_molter\": go.AddComponent<HuskMolterBehavior>(); return;",
                Mode("return_home", "Runtime.HomePos + Vector3.up * 0.25f"),
                Mode("stalk", "if (Player != null && dist <= detectRange)"),
                Mode("molt_and_escape", "Debug.Log(\"ZIPTIDE: HUSK_MOLT\");")),

            new CreatureBehaviorProfile(
                "warden",
                typeof(WardenBehavior),
                EnemySourceRoot + "WardenBehavior.cs",
                "case \"warden\": go.AddComponent<WardenBehavior>(); return;",
                Mode("dormant", "case WardenMode.Dormant:"),
                Mode("watch", "case WardenMode.Watch:"),
                Mode("warn", "case WardenMode.Warn:"),
                Mode("pursue", "case WardenMode.Pursue:"),
                Mode("ally", "case WardenMode.Ally:")),
        };

        public static IReadOnlyList<CreatureBehaviorProfile> All => Profiles;

        public static bool TryGet(string creatureId, out CreatureBehaviorProfile profile)
        {
            for (int i = 0; i < Profiles.Length; i++)
            {
                if (!string.Equals(Profiles[i].CreatureId, creatureId, StringComparison.Ordinal)) continue;
                profile = Profiles[i];
                return true;
            }

            profile = null;
            return false;
        }

        public static int CountUniqueModes(CreatureBehaviorProfile profile)
        {
            if (profile == null || profile.Modes == null) return 0;
            var names = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < profile.Modes.Count; i++)
            {
                var mode = profile.Modes[i];
                if (mode != null && !string.IsNullOrWhiteSpace(mode.Name)) names.Add(mode.Name);
            }
            return names.Count;
        }

        private static ReadableCreatureMode Mode(string name, string evidenceToken)
        {
            return new ReadableCreatureMode(name, evidenceToken);
        }
    }
}

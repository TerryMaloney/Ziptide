#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Structural shipped-creature gate. It proves that every committed CreatureDefinition has one
    /// behavior profile, at least three species-specific readable modes, live source evidence and
    /// CityBuilder factory wiring. It never edits stats, behavior, art or assets.
    /// </summary>
    public static class CreatureBehaviorAuditRules
    {
        public const int MinimumReadableModes = 3;
        public const string CreatureAssetFolder = "Assets/Ziptide/Resources/Enemies";
        public const string CityBuilderRelativePath = "Ziptide/Editor/Patching/CityBuilder.cs";

        private static readonly HashSet<string> GenericRuntimeStates = new HashSet<string>(
            new[] { "stun", "stunned", "disable", "disabled", "down", "respawn", "respawning" },
            StringComparer.OrdinalIgnoreCase);

        public static void Run(SceneAuditReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var profilesById = new Dictionary<string, CreatureBehaviorProfile>(StringComparer.Ordinal);
            foreach (var profile in CreatureBehaviorCatalog.All)
            {
                ValidateProfileShape(report, profile);
                if (profile == null || string.IsNullOrWhiteSpace(profile.CreatureId)) continue;

                if (profilesById.ContainsKey(profile.CreatureId))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_PROFILE_DUPLICATE",
                        "Creature behavior catalog contains duplicate id '" + profile.CreatureId + "'.");
                }
                else
                {
                    profilesById.Add(profile.CreatureId, profile);
                }
            }

            var assetIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (string guid in AssetDatabase.FindAssets(
                "t:CreatureDefinition",
                new[] { CreatureAssetFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var definition = AssetDatabase.LoadAssetAtPath<CreatureDefinition>(path);
                if (definition == null) continue;

                string id = definition.id;
                if (string.IsNullOrWhiteSpace(id))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_ASSET_ID_EMPTY",
                        "CreatureDefinition '" + path + "' has no Definition.id; behavior coverage cannot resolve it.",
                        path);
                    continue;
                }

                if (!assetIds.Add(id))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_ASSET_ID_DUPLICATE",
                        "More than one committed CreatureDefinition uses id '" + id + "'.",
                        path);
                }

                if (!profilesById.ContainsKey(id))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_PROFILE_MISSING",
                        "Committed creature id '" + id + "' has no CreatureBehaviorCatalog profile.",
                        path);
                }
            }

            foreach (var pair in profilesById)
            {
                if (!assetIds.Contains(pair.Key))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_PROFILE_ORPHAN",
                        "Behavior profile '" + pair.Key +
                        "' has no committed CreatureDefinition under Resources/Enemies.");
                }
            }

            ValidateSourceEvidence(report, profilesById.Values);
            ValidateFactoryEvidence(report, profilesById.Values);
        }

        private static void ValidateProfileShape(SceneAuditReport report, CreatureBehaviorProfile profile)
        {
            if (profile == null)
            {
                report.Blocker("CREATURE_BEHAVIOR_STATE_INVALID", "Creature behavior catalog contains a null profile.");
                return;
            }

            if (string.IsNullOrWhiteSpace(profile.CreatureId))
            {
                report.Blocker("CREATURE_BEHAVIOR_STATE_INVALID", "Creature behavior profile has an empty creature id.");
            }

            if (profile.BehaviorType == null ||
                !typeof(CreatureBehaviorBase).IsAssignableFrom(profile.BehaviorType))
            {
                report.Blocker(
                    "CREATURE_BEHAVIOR_STATE_INVALID",
                    "Creature '" + profile.CreatureId + "' does not reference a CreatureBehaviorBase type.");
            }

            int uniqueModeCount = CreatureBehaviorCatalog.CountUniqueModes(profile);
            if (uniqueModeCount < MinimumReadableModes)
            {
                report.Blocker(
                    "CREATURE_BEHAVIOR_STATES_LOW",
                    "Creature '" + profile.CreatureId + "' exposes " + uniqueModeCount +
                    " unique readable modes; minimum is " + MinimumReadableModes + ".");
            }

            var names = new HashSet<string>(StringComparer.Ordinal);
            if (profile.Modes == null) return;
            for (int i = 0; i < profile.Modes.Count; i++)
            {
                var mode = profile.Modes[i];
                if (mode == null || string.IsNullOrWhiteSpace(mode.Name) ||
                    string.IsNullOrWhiteSpace(mode.EvidenceToken))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_STATE_INVALID",
                        "Creature '" + profile.CreatureId + "' has a null/empty mode or evidence token.");
                    continue;
                }

                if (!names.Add(mode.Name))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_STATE_INVALID",
                        "Creature '" + profile.CreatureId + "' repeats readable mode name '" + mode.Name + "'.");
                }

                if (GenericRuntimeStates.Contains(mode.Name))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_GENERIC_STATE_COUNTED",
                        "Creature '" + profile.CreatureId + "' counts generic runtime state '" + mode.Name +
                        "' toward its species vocabulary. Use species-specific behavior instead.");
                }
            }
        }

        private static void ValidateSourceEvidence(
            SceneAuditReport report,
            IEnumerable<CreatureBehaviorProfile> profiles)
        {
            foreach (var profile in profiles)
            {
                string sourcePath = ResolveAssetRelativeSource(profile.BehaviorSourceRelativePath);
                if (!File.Exists(sourcePath))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_SOURCE_MISSING",
                        "Creature '" + profile.CreatureId + "' behavior source is missing: " + sourcePath);
                    continue;
                }

                string source = File.ReadAllText(sourcePath);
                if (profile.Modes == null) continue;
                for (int i = 0; i < profile.Modes.Count; i++)
                {
                    var mode = profile.Modes[i];
                    if (mode == null || string.IsNullOrWhiteSpace(mode.EvidenceToken)) continue;
                    if (!source.Contains(mode.EvidenceToken))
                    {
                        report.Blocker(
                            "CREATURE_BEHAVIOR_EVIDENCE_DRIFT",
                            "Creature '" + profile.CreatureId + "' mode '" + mode.Name +
                            "' no longer has its evidence token in " + profile.BehaviorSourceRelativePath +
                            ": " + mode.EvidenceToken,
                            profile.BehaviorSourceRelativePath);
                    }
                }
            }
        }

        private static void ValidateFactoryEvidence(
            SceneAuditReport report,
            IEnumerable<CreatureBehaviorProfile> profiles)
        {
            string factoryPath = ResolveAssetRelativeSource(CityBuilderRelativePath);
            if (!File.Exists(factoryPath))
            {
                report.Blocker(
                    "CREATURE_BEHAVIOR_FACTORY_DRIFT",
                    "CityBuilder source is missing: " + factoryPath);
                return;
            }

            string factorySource = File.ReadAllText(factoryPath);
            foreach (var profile in profiles)
            {
                if (string.IsNullOrWhiteSpace(profile.FactoryEvidenceToken) ||
                    !factorySource.Contains(profile.FactoryEvidenceToken))
                {
                    report.Blocker(
                        "CREATURE_BEHAVIOR_FACTORY_DRIFT",
                        "Creature '" + profile.CreatureId + "' is not proven wired through CityBuilder.MakeCreature. " +
                        "Missing token: " + profile.FactoryEvidenceToken,
                        CityBuilderRelativePath);
                }
            }
        }

        public static string ResolveAssetRelativeSource(string relativePath)
        {
            return Path.Combine(
                Application.dataPath,
                (relativePath ?? "").Replace('/', Path.DirectorySeparatorChar));
        }
    }

    /// <summary>Blocks APK generation when the shipped creature catalog loses behavior vocabulary.</summary>
    public sealed class CreatureBehaviorBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => 825;

        public void OnPreprocessBuild(BuildReport buildReport)
        {
            var report = new SceneAuditReport { sceneName = "__CREATURE_BEHAVIOR__" };
            CreatureBehaviorAuditRules.Run(report);

            foreach (var finding in report.findings)
            {
                string message = "ZIPTIDE: CREATURE_BEHAVIOR_AUDIT code=" + finding.code +
                                 " path=" + finding.objectPath + " message=" + finding.message;
                if (finding.severity == AuditSeverity.Blocker) Debug.LogError(message);
                else Debug.LogWarning(message);
            }

            if (report.blockerCount > 0)
            {
                throw new BuildFailedException(
                    "Creature behavior coverage failed with " + report.blockerCount +
                    " blocker(s). See CREATURE_BEHAVIOR_AUDIT log lines.");
            }
        }
    }
}
#endif

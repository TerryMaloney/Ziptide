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
    /// canonical readability profile, at least three active states, source evidence, archetype/type
    /// agreement and CityBuilder factory wiring. It never edits behavior, stats, art or assets.
    /// </summary>
    public static class CreatureBehaviorAuditRules
    {
        public const string CreatureAssetFolder = "Assets/Ziptide/Resources/Enemies";
        public const string CityBuilderRelativePath = "Ziptide/Editor/Patching/CityBuilder.cs";

        public static void Run(SceneAuditReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            var profilesById = new Dictionary<string, CreatureBehaviorReadabilityProfile>(StringComparer.Ordinal);
            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
            {
                if (profile == null)
                {
                    report.Blocker("CREATURE_BEHAVIOR_PROFILE_INVALID", "Readability catalog contains a null profile.");
                    continue;
                }

                IReadOnlyList<string> errors = CreatureBehaviorReadabilityCatalog.Validate(profile);
                foreach (string error in errors)
                    report.Blocker("CREATURE_BEHAVIOR_PROFILE_INVALID",
                        "Creature '" + profile.CreatureId + "' profile error: " + error);

                if (string.IsNullOrWhiteSpace(profile.CreatureId)) continue;
                if (!profilesById.TryAdd(profile.CreatureId, profile))
                    report.Blocker("CREATURE_BEHAVIOR_PROFILE_DUPLICATE",
                        "Readability catalog contains duplicate id '" + profile.CreatureId + "'.");

                ValidateBehaviorType(report, profile);
                ValidateSourceEvidence(report, profile);
            }

            var assetIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (string guid in AssetDatabase.FindAssets(
                "t:CreatureDefinition",
                new[] { CreatureAssetFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var definition = AssetDatabase.LoadAssetAtPath<CreatureDefinition>(path);
                if (definition == null) continue;

                if (string.IsNullOrWhiteSpace(definition.id))
                {
                    report.Blocker("CREATURE_BEHAVIOR_ASSET_ID_EMPTY",
                        "CreatureDefinition has no Definition.id.", path);
                    continue;
                }

                if (!assetIds.Add(definition.id))
                    report.Blocker("CREATURE_BEHAVIOR_ASSET_ID_DUPLICATE",
                        "More than one CreatureDefinition uses id '" + definition.id + "'.", path);

                if (!profilesById.TryGetValue(definition.id, out var profile))
                {
                    report.Blocker("CREATURE_BEHAVIOR_PROFILE_MISSING",
                        "Committed creature id '" + definition.id + "' has no readability profile.", path);
                    continue;
                }

                if (definition.archetype != profile.ExpectedArchetype)
                    report.Blocker("CREATURE_BEHAVIOR_ARCHETYPE_DRIFT",
                        "Creature '" + definition.id + "' asset archetype " + definition.archetype +
                        " disagrees with profile " + profile.ExpectedArchetype + ".", path);
            }

            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
                if (profile != null && !assetIds.Contains(profile.CreatureId))
                    report.Blocker("CREATURE_BEHAVIOR_PROFILE_ORPHAN",
                        "Readability profile '" + profile.CreatureId +
                        "' has no committed CreatureDefinition under Resources/Enemies.");

            ValidateFactoryEvidence(report, CreatureBehaviorReadabilityCatalog.All);
        }

        private static void ValidateBehaviorType(
            SceneAuditReport report,
            CreatureBehaviorReadabilityProfile profile)
        {
            Type behaviorType = typeof(CreatureBehaviorBase).Assembly.GetType(
                "Ziptide.Gameplay." + profile.BehaviorTypeName,
                throwOnError: false,
                ignoreCase: false);
            if (behaviorType == null || !typeof(CreatureBehaviorBase).IsAssignableFrom(behaviorType))
                report.Blocker("CREATURE_BEHAVIOR_TYPE_MISSING",
                    "Creature '" + profile.CreatureId + "' references missing/invalid behavior type '" +
                    profile.BehaviorTypeName + "'.");
        }

        private static void ValidateSourceEvidence(
            SceneAuditReport report,
            CreatureBehaviorReadabilityProfile profile)
        {
            string sourcePath = ResolveAssetRelativeSource(profile.BehaviorSourceRelativePath);
            if (!File.Exists(sourcePath))
            {
                report.Blocker("CREATURE_BEHAVIOR_SOURCE_MISSING",
                    "Creature '" + profile.CreatureId + "' behavior source is missing: " + sourcePath);
                return;
            }

            string source = File.ReadAllText(sourcePath);
            foreach (var state in profile.ActiveStateEvidence)
            {
                if (state == null || string.IsNullOrWhiteSpace(state.StateName) ||
                    string.IsNullOrWhiteSpace(state.EvidenceToken))
                {
                    report.Blocker("CREATURE_BEHAVIOR_EVIDENCE_DRIFT",
                        "Creature '" + profile.CreatureId + "' has an empty state/evidence token.",
                        profile.BehaviorSourceRelativePath);
                    continue;
                }

                if (!source.Contains(state.EvidenceToken))
                    report.Blocker("CREATURE_BEHAVIOR_EVIDENCE_DRIFT",
                        "Creature '" + profile.CreatureId + "' state '" + state.StateName +
                        "' lost source token: " + state.EvidenceToken,
                        profile.BehaviorSourceRelativePath);
            }
        }

        private static void ValidateFactoryEvidence(
            SceneAuditReport report,
            IReadOnlyList<CreatureBehaviorReadabilityProfile> profiles)
        {
            string factoryPath = ResolveAssetRelativeSource(CityBuilderRelativePath);
            if (!File.Exists(factoryPath))
            {
                report.Blocker("CREATURE_BEHAVIOR_FACTORY_DRIFT",
                    "CityBuilder source is missing: " + factoryPath);
                return;
            }

            string factorySource = File.ReadAllText(factoryPath);
            foreach (var profile in profiles)
            {
                if (profile == null || string.IsNullOrWhiteSpace(profile.FactoryEvidenceToken) ||
                    !factorySource.Contains(profile.FactoryEvidenceToken))
                    report.Blocker("CREATURE_BEHAVIOR_FACTORY_DRIFT",
                        "Creature '" + (profile != null ? profile.CreatureId : "<null>") +
                        "' is not proven wired through CityBuilder.MakeCreature.",
                        CityBuilderRelativePath);
            }
        }

        public static string ResolveAssetRelativeSource(string relativePath)
        {
            return Path.Combine(
                Application.dataPath,
                (relativePath ?? string.Empty).Replace('/', Path.DirectorySeparatorChar));
        }
    }

    /// <summary>Blocks APK generation when shipped creature readability coverage drifts.</summary>
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
                throw new BuildFailedException(
                    "Creature behavior readability failed with " + report.blockerCount + " blocker(s).");
        }
    }
}
#endif

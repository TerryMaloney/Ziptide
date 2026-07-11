#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Audit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public class CreatureBehaviorAuditRulesTests
    {
        [Test]
        public void Catalog_ProfilesAreUniqueAndCarryThreeSpeciesModes()
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            Assert.AreEqual(7, CreatureBehaviorCatalog.All.Count,
                "the committed story-creature roster is deliberately seven ids at this gate revision");

            foreach (var profile in CreatureBehaviorCatalog.All)
            {
                Assert.IsNotNull(profile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(profile.CreatureId));
                Assert.IsTrue(ids.Add(profile.CreatureId), "duplicate profile id " + profile.CreatureId);
                Assert.IsNotNull(profile.BehaviorType);
                Assert.IsTrue(typeof(CreatureBehaviorBase).IsAssignableFrom(profile.BehaviorType));
                Assert.GreaterOrEqual(
                    CreatureBehaviorCatalog.CountUniqueModes(profile),
                    CreatureBehaviorAuditRules.MinimumReadableModes,
                    profile.CreatureId);

                var modeNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (var mode in profile.Modes)
                {
                    Assert.IsNotNull(mode, profile.CreatureId);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(mode.Name), profile.CreatureId);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(mode.EvidenceToken),
                        profile.CreatureId + "/" + mode.Name);
                    Assert.IsTrue(modeNames.Add(mode.Name),
                        profile.CreatureId + " repeats mode " + mode.Name);
                    Assert.IsFalse(IsGenericRuntimeState(mode.Name),
                        profile.CreatureId + " illegally counts generic state " + mode.Name);
                }
            }
        }

        [Test]
        public void CommittedCreatureDefinitions_MatchCatalogOneToOne()
        {
            var assetIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (string guid in AssetDatabase.FindAssets(
                "t:CreatureDefinition",
                new[] { CreatureBehaviorAuditRules.CreatureAssetFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var definition = AssetDatabase.LoadAssetAtPath<CreatureDefinition>(path);
                Assert.IsNotNull(definition, path);
                Assert.IsFalse(string.IsNullOrWhiteSpace(definition.id), path);
                Assert.IsTrue(assetIds.Add(definition.id), "duplicate creature id " + definition.id);
                Assert.IsTrue(CreatureBehaviorCatalog.TryGet(definition.id, out var profile),
                    definition.id + " has no behavior profile");
                Assert.AreEqual(definition.id, profile.CreatureId);
            }

            Assert.AreEqual(CreatureBehaviorCatalog.All.Count, assetIds.Count,
                "catalog and committed CreatureDefinition assets must remain one-to-one");
            foreach (var profile in CreatureBehaviorCatalog.All)
                Assert.IsTrue(assetIds.Contains(profile.CreatureId),
                    "orphan behavior profile " + profile.CreatureId);
        }

        [Test]
        public void EveryModeEvidenceToken_ExistsInItsRealBehaviorSource()
        {
            foreach (var profile in CreatureBehaviorCatalog.All)
            {
                string path = CreatureBehaviorAuditRules.ResolveAssetRelativeSource(
                    profile.BehaviorSourceRelativePath);
                Assert.IsTrue(File.Exists(path), path);
                string source = File.ReadAllText(path);
                StringAssert.Contains(profile.BehaviorType.Name, source, profile.CreatureId);

                foreach (var mode in profile.Modes)
                    StringAssert.Contains(mode.EvidenceToken, source,
                        profile.CreatureId + "/" + mode.Name + " evidence drifted");
            }
        }

        [Test]
        public void EveryProfile_IsStillWiredThroughCityBuilderFactory()
        {
            string path = CreatureBehaviorAuditRules.ResolveAssetRelativeSource(
                CreatureBehaviorAuditRules.CityBuilderRelativePath);
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);
            StringAssert.Contains("internal static void MakeCreature", source);

            foreach (var profile in CreatureBehaviorCatalog.All)
                StringAssert.Contains(profile.FactoryEvidenceToken, source,
                    profile.CreatureId + " factory wiring drifted");
        }

        [Test]
        public void ProjectAudit_HasNoBehaviorCoverageBlockers()
        {
            var report = new SceneAuditReport { sceneName = "__CREATURE_BEHAVIOR_TEST__" };
            CreatureBehaviorAuditRules.Run(report);

            if (report.blockerCount > 0)
            {
                var messages = new List<string>();
                foreach (var finding in report.findings)
                    if (finding.severity == AuditSeverity.Blocker)
                        messages.Add(finding.ToString());
                Assert.Fail(string.Join("\n", messages));
            }
        }

        [Test]
        public void Lookup_IsExactAndBuildGateOrderIsStable()
        {
            Assert.IsTrue(CreatureBehaviorCatalog.TryGet("warden", out var warden));
            Assert.AreEqual(typeof(WardenBehavior), warden.BehaviorType);
            Assert.IsFalse(CreatureBehaviorCatalog.TryGet("WARDEN", out _),
                "creature ids are exact serialized contracts");
            Assert.IsFalse(CreatureBehaviorCatalog.TryGet("drone_easy", out _),
                "drone difficulty profiles are not story-creature definitions");
            Assert.AreEqual(825, new CreatureBehaviorBuildGate().callbackOrder);
        }

        private static bool IsGenericRuntimeState(string modeName)
        {
            switch ((modeName ?? "").ToLowerInvariant())
            {
                case "stun":
                case "stunned":
                case "disable":
                case "disabled":
                case "down":
                case "respawn":
                case "respawning":
                    return true;
                default:
                    return false;
            }
        }
    }
}
#endif

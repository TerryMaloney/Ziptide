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
        public void CanonicalProfiles_CarrySourceEvidenceForEveryActiveState()
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            // 8 = the seven original creatures + tox_canal_stalker_01 (the canal stalker, added
            // with the boat leg). This number is a ratchet: it may only grow alongside a real
            // shipped CreatureDefinition, which the one-to-one test below enforces.
            Assert.AreEqual(8, CreatureBehaviorReadabilityCatalog.All.Count);

            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
            {
                Assert.IsNotNull(profile);
                Assert.IsTrue(ids.Add(profile.CreatureId), "duplicate profile " + profile.CreatureId);
                Assert.AreEqual(profile.ActiveStates.Count, profile.ActiveStateEvidence.Count);
                Assert.GreaterOrEqual(profile.ActiveStates.Count,
                    CreatureBehaviorReadabilityCatalog.MinimumActiveStates, profile.CreatureId);
                Assert.IsFalse(string.IsNullOrWhiteSpace(profile.BehaviorSourceRelativePath), profile.CreatureId);
                Assert.IsFalse(string.IsNullOrWhiteSpace(profile.FactoryEvidenceToken), profile.CreatureId);

                string sourcePath = CreatureBehaviorAuditRules.ResolveAssetRelativeSource(
                    profile.BehaviorSourceRelativePath);
                Assert.IsTrue(File.Exists(sourcePath), sourcePath);
                string source = File.ReadAllText(sourcePath);
                StringAssert.Contains(profile.BehaviorTypeName, source, profile.CreatureId);

                foreach (var state in profile.ActiveStateEvidence)
                {
                    Assert.IsNotNull(state, profile.CreatureId);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(state.StateName), profile.CreatureId);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(state.EvidenceToken),
                        profile.CreatureId + "/" + state.StateName);
                    StringAssert.Contains(state.EvidenceToken, source,
                        profile.CreatureId + "/" + state.StateName + " evidence drifted");
                }
            }
        }

        [Test]
        public void CommittedCreatureDefinitions_MatchCanonicalCatalogOneToOne()
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
                Assert.IsTrue(CreatureBehaviorReadabilityCatalog.TryGet(definition.id, out var profile),
                    definition.id + " has no readability profile");
                Assert.AreEqual(definition.archetype, profile.ExpectedArchetype, definition.id);
            }

            Assert.AreEqual(CreatureBehaviorReadabilityCatalog.All.Count, assetIds.Count);
            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
                Assert.IsTrue(assetIds.Contains(profile.CreatureId), "orphan profile " + profile.CreatureId);
        }

        [Test]
        public void FactoryEvidenceTokens_ExistInCityBuilder()
        {
            string path = CreatureBehaviorAuditRules.ResolveAssetRelativeSource(
                CreatureBehaviorAuditRules.CityBuilderRelativePath);
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);
            StringAssert.Contains("internal static void MakeCreature", source);

            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
                StringAssert.Contains(profile.FactoryEvidenceToken, source,
                    profile.CreatureId + " factory wiring drifted");
        }

        [Test]
        public void ProjectAudit_HasNoBehaviorReadabilityBlockers()
        {
            var report = new SceneAuditReport { sceneName = "__CREATURE_BEHAVIOR_TEST__" };
            CreatureBehaviorAuditRules.Run(report);

            if (report.blockerCount <= 0) return;
            var messages = new List<string>();
            foreach (var finding in report.findings)
                if (finding.severity == AuditSeverity.Blocker)
                    messages.Add(finding.ToString());
            Assert.Fail(string.Join("\n", messages));
        }

        [Test]
        public void BehaviorTypesResolveAndBuildGateOrderIsStable()
        {
            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
            {
                Type behaviorType = typeof(CreatureBehaviorBase).Assembly.GetType(
                    "Ziptide.Gameplay." + profile.BehaviorTypeName,
                    false,
                    false);
                Assert.IsNotNull(behaviorType, profile.CreatureId);
                Assert.IsTrue(typeof(CreatureBehaviorBase).IsAssignableFrom(behaviorType));
            }

            Assert.IsFalse(CreatureBehaviorReadabilityCatalog.TryGet("drone_easy", out _));
            Assert.AreEqual(825, new CreatureBehaviorBuildGate().callbackOrder);
        }
    }
}
#endif

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// EXCELLENCE_MAP creature-behavior gate: every shipped CreatureDefinition must map to an honest,
    /// machine-readable vocabulary with at least three active states, a telegraph, a counter, a separate
    /// non-lethal resolution, and an installed behavior class/factory route.
    /// </summary>
    public class CreatureBehaviorReadabilityTests
    {
        private const string CreatureAssetFolder = "Assets/Ziptide/Resources/Enemies";

        [Test]
        public void CatalogProfiles_SatisfyTheThreeStateLaw()
        {
            Assert.AreEqual(3, CreatureBehaviorReadabilityCatalog.MinimumActiveStates);
            Assert.GreaterOrEqual(CreatureBehaviorReadabilityCatalog.All.Count, 7,
                "the current shipped roster contains seven CreatureDefinition species");

            var ids = new HashSet<string>(System.StringComparer.Ordinal);
            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
            {
                Assert.IsNotNull(profile);
                Assert.IsTrue(ids.Add(profile.CreatureId),
                    "duplicate readability profile for " + profile.CreatureId);

                IReadOnlyList<string> errors = CreatureBehaviorReadabilityCatalog.Validate(profile);
                Assert.AreEqual(0, errors.Count,
                    profile.CreatureId + " readability profile invalid: " + string.Join(", ", errors));
            }
        }

        [Test]
        public void EveryShippedCreatureAsset_HasAValidProfileAndRealBehaviorType()
        {
            string[] guids = AssetDatabase.FindAssets("t:CreatureDefinition", new[] { CreatureAssetFolder });
            Assert.GreaterOrEqual(guids.Length, 7,
                "expected the seven M3 creature assets under Resources/Enemies");

            var assetIds = new HashSet<string>(System.StringComparer.Ordinal);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var definition = AssetDatabase.LoadAssetAtPath<CreatureDefinition>(path);
                Assert.IsNotNull(definition, path);
                Assert.IsFalse(string.IsNullOrWhiteSpace(definition.id), path + " has no Definition.id");
                Assert.AreEqual(Path.GetFileNameWithoutExtension(path), definition.id,
                    path + " filename and exact runtime creature id drifted");
                Assert.IsTrue(assetIds.Add(definition.id), "duplicate shipped creature id " + definition.id);

                Assert.IsTrue(CreatureBehaviorReadabilityCatalog.TryGet(definition.id, out var profile),
                    definition.id + " ships without a readability profile");
                Assert.AreEqual(definition.archetype, profile.ExpectedArchetype,
                    definition.id + " asset archetype disagrees with the behavior profile");

                IReadOnlyList<string> errors = CreatureBehaviorReadabilityCatalog.Validate(profile);
                Assert.AreEqual(0, errors.Count,
                    definition.id + " readability profile invalid: " + string.Join(", ", errors));

                System.Type behaviorType = typeof(CreatureBehaviorBase).Assembly.GetType(
                    "Ziptide.Gameplay." + profile.BehaviorTypeName,
                    throwOnError: false,
                    ignoreCase: false);
                Assert.IsNotNull(behaviorType,
                    definition.id + " points at missing behavior type " + profile.BehaviorTypeName);
                Assert.IsTrue(typeof(CreatureBehaviorBase).IsAssignableFrom(behaviorType),
                    profile.BehaviorTypeName + " must remain a CreatureBehaviorBase subclass");
            }

            foreach (var profile in CreatureBehaviorReadabilityCatalog.All)
                Assert.IsTrue(assetIds.Contains(profile.CreatureId),
                    "stale/invented readability profile has no shipped CreatureDefinition: " + profile.CreatureId);
        }

        [Test]
        public void CityBuilderFactory_StillRoutesEveryProfileToItsBehavior()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "CityBuilder.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            // Novel ids use exact factory cases.
            AssertSpecialRoute(source, "warden", "WardenBehavior");
            AssertSpecialRoute(source, "witness_mite", "WitnessMiteBehavior");
            AssertSpecialRoute(source, "light_grazer", "LightGrazerBehavior");
            AssertSpecialRoute(source, "tether_swarm", "TetherSwarmBehavior");
            AssertSpecialRoute(source, "husk_molter", "HuskMolterBehavior");

            // The two base species route through their authored archetypes.
            StringAssert.Contains("case CreatureArchetype.WallCrawler:", source);
            StringAssert.Contains("go.AddComponent<WallCrawlerBehavior>();", source);
            StringAssert.Contains("go.AddComponent<SwarmerBehavior>();", source);
        }

        [Test]
        public void ValidationRejectsThinOrContradictoryProfiles()
        {
            var thin = new CreatureBehaviorReadabilityProfile(
                "thin",
                CreatureArchetype.Swarmer,
                "SwarmerBehavior",
                new[] { "idle", "idle", "attack" },
                "missing_telegraph",
                "attack",
                "attack");

            IReadOnlyList<string> errors = CreatureBehaviorReadabilityCatalog.Validate(thin);
            CollectionAssert.Contains((System.Collections.ICollection)errors, "ACTIVE_STATE_DUPLICATE:idle");
            CollectionAssert.Contains((System.Collections.ICollection)errors, "ACTIVE_STATE_COUNT_LOW:2");
            CollectionAssert.Contains((System.Collections.ICollection)errors, "TELEGRAPH_NOT_ACTIVE:missing_telegraph");
            CollectionAssert.Contains((System.Collections.ICollection)errors, "DISABLED_STATE_DUPLICATES_ACTIVE:attack");
        }

        private static void AssertSpecialRoute(string source, string creatureId, string behaviorType)
        {
            StringAssert.Contains(
                "case \"" + creatureId + "\": go.AddComponent<" + behaviorType + ">(); return;",
                source,
                creatureId + " no longer routes to " + behaviorType + " in CityBuilder.MakeCreature");
        }
    }
}
#endif

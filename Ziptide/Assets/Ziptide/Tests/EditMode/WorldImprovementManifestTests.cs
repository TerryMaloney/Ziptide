using System.Collections.Generic;
using NUnit.Framework;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    public sealed class WorldImprovementManifestTests
    {
        [Test]
        public void ValidPortableManifest_HasNoIssues()
        {
            WorldImprovementManifest manifest = MakeManifest();
            Assert.That(manifest.Validate(), Is.Empty);
        }

        [Test]
        public void DuplicateModuleId_IsRejected()
        {
            WorldImprovementManifest manifest = MakeManifest();
            manifest.modules.Add(new WorldImprovementModuleSpec
            {
                moduleId = "arrival_identity",
                enabled = true,
                aspects = new[] { "Identity" },
            });
            Assert.That(manifest.Validate().Exists(i => i.Contains("duplicate moduleId")), Is.True);
        }

        [Test]
        public void DisabledModule_DoesNotCoverRequiredAspect()
        {
            WorldImprovementManifest manifest = MakeManifest();
            manifest.modules[0].enabled = false;
            Assert.That(manifest.Validate().Exists(i => i.Contains("Identity")), Is.True);
        }

        [Test]
        public void AppliesTo_ExactBeatsPathAndGeneratedDefaultHonorsExclusions()
        {
            WorldImprovementManifest exact = MakeManifest();
            exact.sceneName = "W100_Test";
            Assert.That(exact.AppliesTo("W100_Test", "Assets/Scenes/Somewhere.unity"), Is.True);
            Assert.That(exact.AppliesTo("W101_Other", "Assets/Scenes/Generated/W101_Other.unity"), Is.False);

            WorldImprovementManifest fallback = MakeManifest();
            fallback.sceneName = string.Empty;
            fallback.appliesToGeneratedWorlds = true;
            fallback.excludedScenes = new[] { "Arena" };
            Assert.That(fallback.AppliesTo("W101_Other", "Assets/Scenes/Generated/W101_Other.unity"), Is.True);
            Assert.That(fallback.AppliesTo("Arena", "Assets/Scenes/Generated/Arena.unity"), Is.False);
            Assert.That(fallback.AppliesTo("W101_Other", "Assets/Scenes/Handmade/W101_Other.unity"), Is.False);
        }

        [Test]
        public void Hash_IsStableButChangesWithRecipeOrCompiler()
        {
            const string json = "{\"manifestId\":\"test\",\"round\":2}";
            string a = WorldImprovementHashCore.Compute(json, 1);
            string b = WorldImprovementHashCore.Compute(json + "\n", 1);
            string c = WorldImprovementHashCore.Compute(json.Replace("2", "3"), 1);
            string d = WorldImprovementHashCore.Compute(json, 2);
            Assert.That(a, Is.EqualTo(b), "trailing whitespace must not change identity");
            Assert.That(c, Is.Not.EqualTo(a));
            Assert.That(d, Is.Not.EqualTo(a));
            Assert.That(a.Length, Is.EqualTo(64));
        }

        private static WorldImprovementManifest MakeManifest()
        {
            return new WorldImprovementManifest
            {
                gameId = "portable-test",
                manifestId = "portable-test-round1",
                sceneName = "TestScene",
                round = 1,
                recipeVersion = 1,
                requiredAspects = new[] { "Identity", "Navigation" },
                requiredEvidence = new[] { "logic", "spatial", "perceptual", "performance", "device" },
                modules = new List<WorldImprovementModuleSpec>
                {
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "arrival_identity",
                        version = 1,
                        enabled = true,
                        budget = 8,
                        aspects = new[] { "Identity" },
                    },
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "route_beacons",
                        version = 1,
                        enabled = true,
                        budget = 12,
                        aspects = new[] { "Navigation" },
                    },
                },
            };
        }
    }
}

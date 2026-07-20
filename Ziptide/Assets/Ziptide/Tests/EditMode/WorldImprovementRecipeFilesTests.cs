using System.IO;
using NUnit.Framework;
using UnityEditor;
using Ziptide.Content;
using Ziptide.Editor.WorldImprovement;

namespace Ziptide.Tests.EditMode
{
    public sealed class WorldImprovementRecipeFilesTests
    {
        [Test]
        public void CheckedInRecipes_ParseValidateAndUseKnownModuleVersions()
        {
            string folder = WorldImprovementCompiler.ManifestFolder;
            Assert.That(Directory.Exists(folder), Is.True, folder);
            string[] files = Directory.GetFiles(folder, "*.improvement.json");
            Assert.That(files.Length, Is.GreaterThanOrEqualTo(10),
                "Round history should remain checked in alongside the current recipes.");

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                WorldImprovementManifest manifest = WorldImprovementManifest.FromJson(json);
                Assert.That(manifest.Validate(), Is.Empty, Path.GetFileName(file));
                foreach (WorldImprovementModuleSpec spec in manifest.modules)
                {
                    if (spec == null || !spec.enabled) continue;
                    IWorldImprovementModule module = WorldImprovementModuleRegistry.Resolve(spec.moduleId);
                    Assert.That(module, Is.Not.Null, Path.GetFileName(file) + " unknown " + spec.moduleId);
                    Assert.That(spec.version, Is.EqualTo(module.CurrentVersion),
                        Path.GetFileName(file) + " stale module version " + spec.moduleId);
                }
                Assert.That(WorldImprovementHashCore.Compute(json, WorldImprovementCompiler.CompilerVersion).Length,
                    Is.EqualTo(64));
            }
        }

        [Test]
        public void CoveragePolicy_ValidatesAndEveryRequiredBuildSceneResolves()
        {
            WorldImprovementCoveragePolicy policy = WorldImprovementCoveragePolicyLoader.LoadRequired();
            Assert.That(policy.Validate(), Is.Empty);

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled || string.IsNullOrEmpty(scene.path)) continue;
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                if (!policy.RequiresManifest(sceneName, scene.path)) continue;
                Assert.That(WorldImprovementCompiler.TryResolveManifest(sceneName, scene.path,
                    out _, out _, out _), Is.True,
                    "Coverage policy requires a manifest for " + sceneName + " at " + scene.path);
            }
        }

        [Test]
        public void Resolver_UsesNewestExactRecipeBeforeNewestGeneratedDefault()
        {
            Assert.That(WorldImprovementCompiler.TryResolveManifest("W000_DriftIn",
                "Assets/Ziptide/Scenes/Generated/W000_DriftIn.unity", out var w000, out _, out _), Is.True);
            Assert.That(w000.manifestId, Is.EqualTo("ziptide-w000-round3"));
            Assert.That(w000.round, Is.EqualTo(3));

            Assert.That(WorldImprovementCompiler.TryResolveManifest("ToxicCity",
                "Assets/Ziptide/Scenes/ToxicCity.unity", out var toxic, out _, out _), Is.True);
            Assert.That(toxic.manifestId, Is.EqualTo("ziptide-toxiccity-round3"));

            Assert.That(WorldImprovementCompiler.TryResolveManifest("D0_City",
                "Assets/Ziptide/Scenes/D0_City.unity", out var d0, out _, out _), Is.True);
            Assert.That(d0.manifestId, Is.EqualTo("ziptide-d0-city-round3"));

            Assert.That(WorldImprovementCompiler.TryResolveManifest("StarterWorld",
                "Assets/Ziptide/Scenes/StarterWorld.unity", out var starter, out _, out _), Is.True);
            Assert.That(starter.manifestId, Is.EqualTo("ziptide-starter-world-round3"));

            Assert.That(WorldImprovementCompiler.TryResolveManifest("W002_TestWorld",
                "Assets/Ziptide/Scenes/Generated/W002_TestWorld.unity", out var generated, out _, out _), Is.True);
            Assert.That(generated.manifestId, Is.EqualTo("ziptide-generated-standard-round3"));
            Assert.That(generated.round, Is.EqualTo(3));

            Assert.That(WorldImprovementCompiler.TryResolveManifest("W011_Undercroft",
                "Assets/Ziptide/Scenes/Generated/W011_Undercroft.unity", out _, out _, out _), Is.False);
        }
    }
}

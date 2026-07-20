using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Editor.Audit;
using Ziptide.Editor.WorldImprovement;

namespace Ziptide.Tests.EditMode
{
    public sealed class WorldImprovementCompilerTests
    {
        [Test]
        public void Registry_ContainsThePortableRoundOneModules()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                "arrival_identity", "route_beacons", "ambient_motion", "horizon_frame"
            }, WorldImprovementModuleRegistry.KnownIds());
        }

        [Test]
        public void CompileTwice_IsIdempotentColliderFreeAndAuditGreen()
        {
            GameObject floor = null;
            GameObject spawn = null;
            try
            {
                DestroyOwnedRoots();
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "WIM_TestFloor";
                floor.transform.position = new Vector3(0f, -0.05f, 0f);
                floor.transform.localScale = new Vector3(100f, 0.1f, 100f);

                spawn = new GameObject("__SPAWN_PLAYER");
                spawn.transform.position = Vector3.zero;
                spawn.transform.rotation = Quaternion.identity;
                Physics.SyncTransforms();

                WorldImprovementManifest manifest = MakeFullManifest(SceneManager.GetActiveScene().name);
                string json = manifest.ToJson();
                string expectedHash = WorldImprovementHashCore.Compute(json,
                    WorldImprovementCompiler.CompilerVersion);

                WorldImprovementCompiler.BeginCompileSession();
                WorldImprovementCompiler.CompileActiveScene(manifest, json,
                    "docs/worldimprovements/test.improvement.json", SceneManager.GetActiveScene().path);
                AssertCompiled(manifest, expectedHash);

                WorldImprovementCompiler.CompileActiveScene(manifest, json,
                    "docs/worldimprovements/test.improvement.json", SceneManager.GetActiveScene().path);
                AssertCompiled(manifest, expectedHash);
                Assert.That(CountOwnedRoots(), Is.EqualTo(1), "recompile must replace, not stack, the owned root");
            }
            finally
            {
                DestroyOwnedRoots();
                if (spawn != null) Object.DestroyImmediate(spawn);
                if (floor != null) Object.DestroyImmediate(floor);
            }
        }

        [Test]
        public void Audit_DeliberatelyStaleAndIncompleteStamp_FiresBlockers()
        {
            GameObject root = null;
            try
            {
                DestroyOwnedRoots();
                string sceneName = SceneManager.GetActiveScene().name;
                WorldImprovementManifest manifest = MakeFullManifest(sceneName);
                root = new GameObject(WorldImprovementCompiler.RootName);
                root.AddComponent<WorldImprovementStamp>().Configure(manifest, sceneName,
                    WorldImprovementCompiler.CompilerVersion, "old-hash");

                var moduleRoot = new GameObject("__WIM_ARRIVAL_IDENTITY");
                moduleRoot.transform.SetParent(root.transform, false);
                moduleRoot.AddComponent<WorldImprovementModuleMarker>().Configure(manifest.modules[0], 5);

                var report = new SceneAuditReport { sceneName = sceneName };
                WorldImprovementAuditRules.AuditAgainstManifest(report, manifest, "expected-hash", sceneName);

                Assert.That(report.findings.Exists(f => f.code == WorldImprovementAuditRules.StaleRecipe), Is.True);
                Assert.That(report.findings.Exists(f => f.code == WorldImprovementAuditRules.ModuleMissing), Is.True);
                Assert.That(report.findings.Exists(f => f.code == WorldImprovementAuditRules.AspectMissing), Is.True);
            }
            finally
            {
                if (root != null) Object.DestroyImmediate(root);
                DestroyOwnedRoots();
            }
        }

        private static void AssertCompiled(WorldImprovementManifest manifest, string expectedHash)
        {
            GameObject root = FindOwnedRoot();
            Assert.That(root, Is.Not.Null);
            var stamp = root.GetComponent<WorldImprovementStamp>();
            Assert.That(stamp, Is.Not.Null);
            Assert.That(stamp.ManifestId, Is.EqualTo(manifest.manifestId));
            Assert.That(stamp.RecipeHash, Is.EqualTo(expectedHash));
            Assert.That(root.GetComponentsInChildren<WorldImprovementModuleMarker>(true).Length,
                Is.EqualTo(4));
            Assert.That(root.GetComponentsInChildren<Collider>(true), Is.Empty,
                "portable presentation modules must never create traversal collision");
            Assert.That(root.GetComponentInChildren<WorldAmbientMotionRuntime>(true), Is.Not.Null);

            var report = new SceneAuditReport { sceneName = SceneManager.GetActiveScene().name };
            WorldImprovementAuditRules.AuditAgainstManifest(report, manifest, expectedHash,
                SceneManager.GetActiveScene().name);
            Assert.That(report.blockerCount, Is.Zero,
                string.Join("\n", report.findings.ConvertAll(f => f.ToString())));
        }

        private static WorldImprovementManifest MakeFullManifest(string sceneName)
        {
            return new WorldImprovementManifest
            {
                gameId = "portable-test",
                manifestId = "portable-round2",
                sceneName = sceneName,
                displayName = "PORTABLE TEST WORLD",
                round = 2,
                recipeVersion = 1,
                seed = 12345,
                requiredAspects = new[]
                {
                    "Identity", "Perception", "Navigation", "SpatialSafety", "Ambience", "Atmosphere"
                },
                requiredEvidence = new[] { "logic", "spatial", "perceptual", "performance", "device" },
                primaryColor = new Color(0.2f, 0.3f, 0.4f, 1f),
                accentColor = new Color(0.8f, 0.4f, 0.2f, 1f),
                glowColor = new Color(0.3f, 0.8f, 1f, 1f),
                modules = new List<WorldImprovementModuleSpec>
                {
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "arrival_identity", version = 1, enabled = true,
                        intensity = 1f, budget = 8, aspects = new[] { "Identity", "Perception" }
                    },
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "route_beacons", version = 1, enabled = true,
                        intensity = 1f, budget = 12, aspects = new[] { "Navigation", "SpatialSafety", "Perception" }
                    },
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "ambient_motion", version = 1, enabled = true,
                        intensity = 1f, budget = 6, aspects = new[] { "Ambience", "Perception" }
                    },
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "horizon_frame", version = 1, enabled = true,
                        intensity = 1f, budget = 8, aspects = new[] { "Identity", "Atmosphere", "Perception" }
                    },
                },
            };
        }

        private static GameObject FindOwnedRoot()
        {
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
                if (root != null && root.name == WorldImprovementCompiler.RootName) return root;
            return null;
        }

        private static int CountOwnedRoots()
        {
            int count = 0;
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
                if (root != null && root.name == WorldImprovementCompiler.RootName) count++;
            return count;
        }

        private static void DestroyOwnedRoots()
        {
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
                if (roots[i] != null && roots[i].name == WorldImprovementCompiler.RootName)
                    Object.DestroyImmediate(roots[i]);
        }
    }
}

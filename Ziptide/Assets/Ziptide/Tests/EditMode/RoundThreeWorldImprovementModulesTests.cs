using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

using Ziptide.Content;
using Ziptide.Editor.Audit;
using Ziptide.Editor.WorldImprovement;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    public sealed class RoundThreeWorldImprovementModulesTests
    {
        [Test]
        public void CompileRoundThree_ProducesGroundedOwnedInteractiveDensityAndPassesAudit()
        {
            GameObject floor = null;
            GameObject spawn = null;
            GameObject markerA = null;
            GameObject markerB = null;
            try
            {
                DestroyOwnedRoot();
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "RoundThreeFloor";
                floor.transform.position = new Vector3(0f, -0.10f, 0f);
                floor.transform.localScale = new Vector3(80f, 0.20f, 80f);

                spawn = new GameObject("__SPAWN_PLAYER");
                markerA = new GameObject("Marker_RoundThree_A");
                markerA.transform.position = new Vector3(9f, 0f, 2f);
                markerB = new GameObject("Marker_RoundThree_B");
                markerB.transform.position = new Vector3(-4f, 0f, 10f);
                Physics.SyncTransforms();

                string actualScene = SceneManager.GetActiveScene().name;
                string manifestScene = string.IsNullOrEmpty(actualScene) ? "RoundThreeFixture" : actualScene;
                WorldImprovementManifest manifest = MakeManifest(manifestScene);
                string json = manifest.ToJson();
                string hash = WorldImprovementHashCore.Compute(json, WorldImprovementCompiler.CompilerVersion);

                WorldImprovementCompiler.BeginCompileSession();
                WorldImprovementCompiler.CompileActiveScene(manifest, json,
                    "docs/worldimprovements/round3-test.improvement.json", SceneManager.GetActiveScene().path);

                GameObject root = FindOwnedRoot();
                Assert.That(root, Is.Not.Null);
                Assert.That(root.GetComponentsInChildren<WorldImprovementModuleMarker>(true).Length,
                    Is.EqualTo(3));

                Transform route = root.transform.Find("__WIM_GROUNDED_ROUTE");
                Transform discovery = root.transform.Find("__WIM_DISCOVERY_NODES");
                Transform story = root.transform.Find("__WIM_STORY_TRACES");
                Assert.That(route, Is.Not.Null);
                Assert.That(discovery, Is.Not.Null);
                Assert.That(story, Is.Not.Null);
                Assert.That(route.GetComponentsInChildren<Collider>(true), Is.Empty,
                    "Route readability may never alter traversal collision.");
                Assert.That(story.GetComponentsInChildren<Collider>(true), Is.Empty,
                    "Environmental story traces are presentation-only.");

                WorldDiscoveryNodeRuntime[] nodes = discovery.GetComponentsInChildren<WorldDiscoveryNodeRuntime>(true);
                Assert.That(nodes.Length, Is.GreaterThanOrEqualTo(1));
                foreach (WorldDiscoveryNodeRuntime node in nodes)
                {
                    Assert.That(node.GetComponent<Collider>(), Is.Not.Null);
                    Assert.That(node.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>(), Is.Not.Null);
                    Assert.That(node.GetComponent<Rigidbody>(), Is.Null,
                        "Discovery nodes must not introduce loose physics bodies.");
                    Assert.That(node.transform.position.y, Is.InRange(0.8f, 1.3f));
                }

                var report = new SceneAuditReport { sceneName = actualScene };
                WorldImprovementAuditRules.AuditAgainstManifest(report, manifest, hash, actualScene);
                Assert.That(report.blockerCount, Is.Zero,
                    string.Join("\n", report.findings.ConvertAll(f => f.ToString())));
            }
            finally
            {
                DestroyOwnedRoot();
                if (markerB != null) Object.DestroyImmediate(markerB);
                if (markerA != null) Object.DestroyImmediate(markerA);
                if (spawn != null) Object.DestroyImmediate(spawn);
                if (floor != null) Object.DestroyImmediate(floor);
            }
        }

        private static WorldImprovementManifest MakeManifest(string sceneName)
        {
            return new WorldImprovementManifest
            {
                gameId = "round-three-test",
                manifestId = "round-three-test",
                sceneName = sceneName,
                displayName = "ROUND THREE TEST",
                round = 3,
                recipeVersion = 1,
                seed = 30003,
                requiredAspects = new[]
                {
                    "Navigation", "SpatialSafety", "Perception", "Interaction", "Discovery",
                    "Story", "Identity", "Atmosphere"
                },
                requiredEvidence = new[] { "logic", "spatial", "perceptual", "performance", "device" },
                primaryColor = new Color(0.20f, 0.28f, 0.34f, 1f),
                accentColor = new Color(0.82f, 0.48f, 0.22f, 1f),
                glowColor = new Color(0.28f, 0.82f, 0.94f, 1f),
                modules = new List<WorldImprovementModuleSpec>
                {
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "grounded_route", version = 1, enabled = true,
                        intensity = 1f, budget = 24,
                        aspects = new[] { "Navigation", "SpatialSafety", "Perception" }
                    },
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "discovery_nodes", version = 1, enabled = true,
                        intensity = 1f, budget = 12,
                        aspects = new[] { "Interaction", "Discovery", "Perception" }
                    },
                    new WorldImprovementModuleSpec
                    {
                        moduleId = "story_traces", version = 1, enabled = true,
                        intensity = 1f, budget = 12,
                        aspects = new[] { "Story", "Identity", "Atmosphere" }
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

        private static void DestroyOwnedRoot()
        {
            GameObject root = FindOwnedRoot();
            if (root != null) Object.DestroyImmediate(root);
        }
    }
}

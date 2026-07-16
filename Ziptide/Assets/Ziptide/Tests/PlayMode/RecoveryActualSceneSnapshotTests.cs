using System;
using System.Collections;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// R1.7/R1.8 actual Home Hub visual proof. A sceneLoaded hook establishes the test-owned tracked
    /// head before BootLoader/HomeHub Start so the diegetic board is authored from a realistic standing
    /// viewpoint. Content-world captures are owned by a separate test-only travel observer attached to
    /// the already-proven post-settlement round-trip route.
    /// </summary>
    public sealed class RecoveryActualSceneSnapshotTests
    {
        private const float BootLoadTimeoutSeconds = 120f;
        private const float BootReadyTimeoutSeconds = 30f;

        private RecoverySaveFileBackup _saveBackup;
        private RecoveryActualRigControllerSimulation _simulation;
        private Exception _earlySimulationFailure;
        private bool _bootReady;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);
            yield return RecoverySceneTestIsolation.PrepareFreshGoldenBoot();
            _earlySimulationFailure = null;
            _bootReady = false;
            HomeHubRuntime.BootPresentationReady += OnBootReady;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;
            HomeHubRuntime.BootPresentationReady -= OnBootReady;

            // Destroy the isolated Golden runtime before removing its test-only XR devices. Removing
            // virtual controls while live action assets and locomotion providers are enabled can stall
            // Input System and leave invalid processor state for the next PlayMode test.
            yield return RecoverySceneTestIsolation.ResetToEmptyFullDevelopment();
            _simulation?.Dispose();
            _simulation = null;

            _saveBackup?.Dispose();
            _saveBackup = null;
        }

        [UnityTest]
        public IEnumerator GoldenBoot_WritesInspectableHomeHubVisualAndUiArtifacts()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneBoot));

            SceneManager.sceneLoaded += OnBootSceneLoadedBeforeStart;
            AsyncOperation bootLoad = SceneManager.LoadSceneAsync(
                ZiptideConstants.SceneBoot,
                LoadSceneMode.Single);
            Assert.IsNotNull(bootLoad);

            float bootDeadline = Time.realtimeSinceStartup + BootLoadTimeoutSeconds;
            while (!bootLoad.isDone && Time.realtimeSinceStartup < bootDeadline)
                yield return null;
            Assert.IsTrue(bootLoad.isDone,
                "Actual _Boot scene did not finish loading within " +
                BootLoadTimeoutSeconds + " seconds. progress=" + bootLoad.progress.ToString("F3"));

            SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;
            if (_earlySimulationFailure != null)
                throw new AssertionException(
                    "Tracked-rig simulation failed before Home Hub Start: " + _earlySimulationFailure);
            Assert.IsNotNull(_simulation,
                "The actual rig was not placed into a tracked pose before Home Hub Start.");

            RecoverySceneTestIsolation.InvokeAllowedAfterSceneLoadBootstraps();
            float readyDeadline = Time.realtimeSinceStartup + BootReadyTimeoutSeconds;
            while (!_bootReady && Time.realtimeSinceStartup < readyDeadline)
                yield return null;
            Assert.IsTrue(_bootReady,
                "Actual Home Hub never reached ready state within " +
                BootReadyTimeoutSeconds + " seconds.");
            yield return null;
            yield return new WaitForEndOfFrame();

            Camera camera = _simulation.HeadCamera;
            Assert.IsNotNull(camera);
            AssertFrameRendered(RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_ACTUAL_HOME_HUB",
                "r1_7_actual_home_hub"));
            AssertUiSpatial(
                camera,
                "R1_8_ACTUAL_HOME_HUB",
                "r1_8_actual_home_hub");
        }

        private void OnBootSceneLoadedBeforeStart(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != ZiptideConstants.SceneBoot || _simulation != null) return;
            try
            {
                PlayerRigPersistence rig = UnityEngine.Object.FindObjectOfType<PlayerRigPersistence>();
                XRInteractionManager manager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();
                if (rig == null || manager == null)
                    throw new InvalidOperationException(
                        "Boot sceneLoaded callback could not resolve the persistent rig/XRI manager.");
                _simulation = RecoveryActualRigControllerSimulation.Activate(rig, manager);
            }
            catch (Exception ex)
            {
                _earlySimulationFailure = ex;
            }
        }

        internal static void AssertFrameRendered(RecoveryRenderSnapshotPaths paths)
        {
            Assert.IsTrue(File.Exists(paths.PngPath), "Actual-scene PNG was not written.");
            Assert.IsTrue(File.Exists(paths.JsonPath), "Actual-scene snapshot JSON was not written.");
            RecoveryRenderSnapshotMetrics metrics = RecoveryRenderSnapshot.ReadMetrics(paths.JsonPath);
            Assert.Greater(metrics.pngBytes, 1000,
                metrics.label + " PNG is too small to prove a rendered frame.");
            Assert.AreEqual(64, metrics.pngSha256.Length);
            Assert.Greater(metrics.quantizedColorCount, 4,
                metrics.label + " has too little color information; likely blank/unrendered.");
            Assert.Greater(metrics.dynamicRange, 0.01d,
                metrics.label + " has effectively no luminance range.");
            Assert.Less(metrics.nearBlackRatio, 0.999d,
                metrics.label + " is effectively all black.");
            Assert.Less(metrics.nearWhiteRatio, 0.999d,
                metrics.label + " is effectively all white/clipped.");
            Assert.Less(metrics.transparentRatio, 0.10d,
                metrics.label + " contains excessive transparent output.");
        }

        internal static void AssertUiSpatial(Camera camera, string label, string stem)
        {
            RecoveryUiSpatialReport report = RecoveryUiSpatialAudit.Capture(camera, label);
            RecoveryWorldCanvasTextAudit.Append(camera, report);
            RecoveryUiSpatialArtifactPaths paths = RecoveryUiSpatialAudit.WriteArtifacts(report, stem);
            Assert.IsTrue(File.Exists(paths.JsonPath), label + " UI JSON was not written.");
            Assert.IsTrue(File.Exists(paths.MarkdownPath), label + " UI Markdown was not written.");

            int blockers = 0;
            var details = new StringBuilder();
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoveryUiSpatialFinding finding = report.findings[i];
                if (!string.Equals(finding.severity, "BLOCKER", StringComparison.Ordinal)) continue;
                blockers++;
                details.AppendLine(
                    finding.code + " path=" + finding.hierarchyPath +
                    " related=" + finding.relatedPath +
                    " message=" + finding.message);
            }

            Assert.AreEqual(0, blockers,
                label + " produced camera-space UI blockers. JSON=" + paths.JsonPath +
                " Markdown=" + paths.MarkdownPath + "\n" + details);
        }

        private void OnBootReady(bool canContinue) => _bootReady = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
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
    /// R1.7 actual-scene renderer spike. A sceneLoaded hook establishes the test-owned tracked-head
    /// and bilateral-controller pose before BootLoader/HomeHub Start, so the captured Home Hub is
    /// laid out from a realistic head height. W000 and ToxicCity are reached through the production
    /// TravelCoordinator and captured from the same persistent tracked-head camera.
    /// </summary>
    public sealed class RecoveryActualSceneSnapshotTests
    {
        private const float TravelTimeoutSeconds = 120f;

        private readonly List<string> _travelCompleted = new List<string>();
        private RecoverySaveFileBackup _saveBackup;
        private RecoveryActualRigControllerSimulation _simulation;
        private Exception _earlySimulationFailure;
        private bool _bootReady;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);
            yield return RecoverySceneTestIsolation.PrepareFreshGoldenBoot();
            _travelCompleted.Clear();
            _earlySimulationFailure = null;
            _bootReady = false;
            HomeHubRuntime.BootPresentationReady += OnBootReady;
            TravelCoordinator.TravelCompleted += OnTravelCompleted;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;
            HomeHubRuntime.BootPresentationReady -= OnBootReady;
            TravelCoordinator.TravelCompleted -= OnTravelCompleted;
            _simulation?.Dispose();
            _simulation = null;
            yield return RecoverySceneTestIsolation.ResetToEmptyFullDevelopment();
            _saveBackup?.Dispose();
            _saveBackup = null;
        }

        [UnityTest]
        public IEnumerator GoldenBootW000AndToxicCity_WriteInspectablePngArtifacts()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneBoot));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneW000));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneToxicCity));

            SceneManager.sceneLoaded += OnBootSceneLoadedBeforeStart;
            AsyncOperation bootLoad = SceneManager.LoadSceneAsync(
                ZiptideConstants.SceneBoot,
                LoadSceneMode.Single);
            Assert.IsNotNull(bootLoad);
            for (int frame = 0; frame < 600 && !bootLoad.isDone; frame++) yield return null;
            Assert.IsTrue(bootLoad.isDone, "Actual _Boot scene did not finish loading.");
            SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;
            if (_earlySimulationFailure != null)
                throw new AssertionException(
                    "Tracked-rig simulation failed before Home Hub Start: " + _earlySimulationFailure);
            Assert.IsNotNull(_simulation,
                "The actual rig was not placed into a tracked pose before Home Hub Start.");

            RecoverySceneTestIsolation.InvokeAllowedAfterSceneLoadBootstraps();
            for (int frame = 0; frame < 300 && !_bootReady; frame++) yield return null;
            Assert.IsTrue(_bootReady, "Actual Home Hub never reached ready state.");
            yield return null;
            yield return new WaitForEndOfFrame();

            Camera camera = _simulation.HeadCamera;
            Assert.IsNotNull(camera);
            AssertFrameRendered(RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_ACTUAL_HOME_HUB",
                "r1_7_actual_home_hub"));

            SaveSystem save = SaveSystem.Instance;
            Assert.IsNotNull(save, "SaveSystem is missing before actual scene snapshots.");
            PlayerProfile profile = save.StartNewProfile();
            Assert.IsNotNull(profile);

            TravelCoordinator.TravelTo(ZiptideConstants.SceneW000);
            yield return WaitForDestination(1, ZiptideConstants.SceneW000);
            yield return new WaitForEndOfFrame();
            AssertFrameRendered(RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_ACTUAL_W000_SPAWN",
                "r1_7_actual_w000_spawn"));

            TravelCoordinator.TravelTo(ZiptideConstants.SceneToxicCity);
            yield return WaitForDestination(2, ZiptideConstants.SceneToxicCity);
            yield return new WaitForEndOfFrame();
            AssertFrameRendered(RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_ACTUAL_TOXIC_CITY_SPAWN",
                "r1_7_actual_toxic_city_spawn"));

            CollectionAssert.AreEqual(
                new[] { ZiptideConstants.SceneW000, ZiptideConstants.SceneToxicCity },
                _travelCompleted,
                "Snapshot route did not follow the intended Golden scene sequence.");
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

        private IEnumerator WaitForDestination(int expectedCount, string expectedScene)
        {
            float deadline = Time.realtimeSinceStartup + TravelTimeoutSeconds;
            while (_travelCompleted.Count < expectedCount && Time.realtimeSinceStartup < deadline)
                yield return null;

            Assert.GreaterOrEqual(_travelCompleted.Count, expectedCount,
                "Travel did not complete to " + expectedScene + " within " +
                TravelTimeoutSeconds + " seconds.");
            Assert.AreEqual(expectedScene, _travelCompleted[expectedCount - 1]);
            Assert.AreEqual(expectedScene, SceneManager.GetActiveScene().name);
            Assert.IsFalse(TravelCoordinator.IsTravelling);
            yield return null;
            yield return null;

            RecoveryRuntimeArtifactReport artifactReport =
                RecoveryRuntimeArtifactGuard.Capture("R1_7_" + expectedScene + "_SNAPSHOT");
            string artifactPath = RecoveryRuntimeArtifactGuard.WriteArtifact(
                artifactReport,
                "r1_7_" + expectedScene.ToLowerInvariant() + "_snapshot");
            Assert.AreEqual(0, artifactReport.findings.Count,
                "Snapshot scene has runtime/spawn findings. Artifact=" + artifactPath +
                "\n" + FormatFindings(artifactReport));
        }

        private static void AssertFrameRendered(RecoveryRenderSnapshotPaths paths)
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

        private void OnBootReady(bool canContinue) => _bootReady = true;
        private void OnTravelCompleted(string destination) => _travelCompleted.Add(destination);

        private static string FormatFindings(RecoveryRuntimeArtifactReport report)
        {
            var text = new StringBuilder();
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoveryRuntimeArtifactFinding finding = report.findings[i];
                text.AppendLine(finding.code + " path=" + finding.hierarchyPath +
                    " message=" + finding.message);
            }
            return text.ToString();
        }
    }
}

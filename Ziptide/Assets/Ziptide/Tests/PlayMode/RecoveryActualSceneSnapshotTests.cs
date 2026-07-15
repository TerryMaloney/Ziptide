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
    /// R1.7/R1.8 actual-scene renderer and UI-spatial proof. A sceneLoaded hook establishes the
    /// test-owned tracked-head and bilateral-controller pose before BootLoader/HomeHub Start, so the
    /// captured Home Hub is laid out from a realistic head height. W000 and ToxicCity are reached
    /// through the production Home Hub and TravelCoordinator path and captured/audited from the same
    /// persistent camera.
    /// </summary>
    public sealed class RecoveryActualSceneSnapshotTests
    {
        private const float TravelTimeoutSeconds = 120f;
        private const float BootReadyTimeoutSeconds = 30f;

        private readonly List<string> _travelCompleted = new List<string>();
        private RecoverySaveFileBackup _saveBackup;
        private RecoveryActualRigControllerSimulation _simulation;
        private Exception _earlySimulationFailure;
        private PlayerProfile _newGameProfile;
        private bool _bootReady;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);
            yield return RecoverySceneTestIsolation.PrepareFreshGoldenBoot();
            _travelCompleted.Clear();
            _earlySimulationFailure = null;
            _newGameProfile = null;
            _bootReady = false;
            HomeHubRuntime.BootPresentationReady += OnBootReady;
            HomeHubRuntime.NewGameProfileCreated += OnNewGameProfileCreated;
            TravelCoordinator.TravelCompleted += OnTravelCompleted;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            SceneManager.sceneLoaded -= OnBootSceneLoadedBeforeStart;
            HomeHubRuntime.BootPresentationReady -= OnBootReady;
            HomeHubRuntime.NewGameProfileCreated -= OnNewGameProfileCreated;
            TravelCoordinator.TravelCompleted -= OnTravelCompleted;
            _simulation?.Dispose();
            _simulation = null;
            yield return RecoverySceneTestIsolation.ResetToEmptyFullDevelopment();
            _saveBackup?.Dispose();
            _saveBackup = null;
        }

        [UnityTest]
        public IEnumerator GoldenBootW000AndToxicCity_WriteInspectableVisualAndUiArtifacts()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneBoot));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneW000));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneToxicCity));

            SceneManager.sceneLoaded += OnBootSceneLoadedBeforeStart;
            AsyncOperation bootLoad = SceneManager.LoadSceneAsync(
                ZiptideConstants.SceneBoot,
                LoadSceneMode.Single);
            Assert.IsNotNull(bootLoad);

            // A frame-count budget is not a time budget on the fast headless runner: 600 frames can
            // elapse in about one second while Unity is still completing a legitimate first scene load.
            // Use the same real-time budget as the production travel proof and preserve the hard fail.
            float bootDeadline = Time.realtimeSinceStartup + TravelTimeoutSeconds;
            while (!bootLoad.isDone && Time.realtimeSinceStartup < bootDeadline)
                yield return null;
            Assert.IsTrue(bootLoad.isDone,
                "Actual _Boot scene did not finish loading within " +
                TravelTimeoutSeconds + " seconds. progress=" + bootLoad.progress.ToString("F3"));

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

            // Use the real diegetic NEW GAME interactable. BootLoader owns its callback and passes
            // skipGate=true for the empty _Boot scene; bypassing this choice incorrectly enables a
            // departure-gate wait while BOOT_HOLD is still armed and does not represent production.
            HomeHubRuntime home = UnityEngine.Object.FindObjectOfType<HomeHubRuntime>();
            XRInteractionManager manager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();
            Assert.IsNotNull(home, "Actual Home Hub disappeared before NEW GAME selection.");
            Assert.IsNotNull(manager, "Canonical XRInteractionManager is missing at Home Hub selection.");
            SelectHomeHubTileThroughXri(home, "Tile_NEW_GAME", manager, _simulation.RightRay);
            Assert.IsNotNull(_newGameProfile,
                "The actual NEW GAME selection did not create a profile before travel.");

            IEnumerator w000Wait = WaitForDestination(1, ZiptideConstants.SceneW000);
            while (w000Wait.MoveNext()) yield return w000Wait.Current;
            yield return new WaitForEndOfFrame();
            AssertFrameRendered(RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_ACTUAL_W000_SPAWN",
                "r1_7_actual_w000_spawn"));
            AssertUiSpatial(
                camera,
                "R1_8_ACTUAL_W000_SPAWN",
                "r1_8_actual_w000_spawn");

            TravelCoordinator.TravelTo(ZiptideConstants.SceneToxicCity);
            IEnumerator toxicCityWait = WaitForDestination(2, ZiptideConstants.SceneToxicCity);
            while (toxicCityWait.MoveNext()) yield return toxicCityWait.Current;
            yield return new WaitForEndOfFrame();
            AssertFrameRendered(RecoveryRenderSnapshot.Capture(
                camera,
                "R1_7_ACTUAL_TOXIC_CITY_SPAWN",
                "r1_7_actual_toxic_city_spawn"));
            AssertUiSpatial(
                camera,
                "R1_8_ACTUAL_TOXIC_CITY_SPAWN",
                "r1_8_actual_toxic_city_spawn");

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

        private static void SelectHomeHubTileThroughXri(
            HomeHubRuntime home,
            string tileName,
            XRInteractionManager manager,
            XRRayInteractor ray)
        {
            Assert.IsNotNull(home);
            Assert.IsNotNull(manager);
            Assert.IsNotNull(ray);

            Transform tileTransform = home.transform.Find(tileName);
            Assert.IsNotNull(tileTransform, "Actual Home Hub tile was not found: " + tileName);
            XRSimpleInteractable tile = tileTransform.GetComponent<XRSimpleInteractable>();
            Assert.IsNotNull(tile, tileName + " is not an XRSimpleInteractable.");
            Assert.AreSame(manager, tile.interactionManager,
                tileName + " is not bound to the canonical interaction manager.");
            Assert.AreSame(manager, ray.interactionManager,
                "The actual right ray is not bound to the canonical interaction manager.");

            Vector3 approach = home.transform.forward;
            Vector3 rayPosition = tileTransform.position - approach * 1.1f;
            ray.transform.position = rayPosition;
            ray.transform.rotation = Quaternion.LookRotation(
                (tileTransform.position - rayPosition).normalized,
                Vector3.up);
            Physics.SyncTransforms();

            Assert.IsTrue(Physics.Raycast(
                ray.transform.position,
                ray.transform.forward,
                out RaycastHit hit,
                3f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide),
                "The actual rig ray did not hit " + tileName + ".");
            Assert.AreSame(tileTransform.gameObject, hit.collider.gameObject,
                "The actual rig ray hit another object before " + tileName + ": " +
                RecoveryRuntimeCensus.HierarchyPath(hit.transform));

            var hoverInteractor = (IXRHoverInteractor)ray;
            var selectInteractor = (IXRSelectInteractor)ray;
            var hoverInteractable = (IXRHoverInteractable)tile;
            var selectInteractable = (IXRSelectInteractable)tile;
            if (!tile.isHovered) manager.HoverEnter(hoverInteractor, hoverInteractable);
            Assert.IsTrue(tile.isHovered, tileName + " did not enter hover state.");
            manager.SelectEnter(selectInteractor, selectInteractable);
            Assert.IsTrue(tile.isSelected, tileName + " did not enter selected state.");
            manager.SelectExit(selectInteractor, selectInteractable);
            if (tile.isHovered) manager.HoverExit(hoverInteractor, hoverInteractable);
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
                "\n" + FormatRuntimeFindings(artifactReport));
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

        private static void AssertUiSpatial(Camera camera, string label, string stem)
        {
            RecoveryUiSpatialReport report = RecoveryUiSpatialAudit.Capture(camera, label);
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
        private void OnNewGameProfileCreated(PlayerProfile profile) => _newGameProfile = profile;
        private void OnTravelCompleted(string destination) => _travelCompleted.Add(destination);

        private static string FormatRuntimeFindings(RecoveryRuntimeArtifactReport report)
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

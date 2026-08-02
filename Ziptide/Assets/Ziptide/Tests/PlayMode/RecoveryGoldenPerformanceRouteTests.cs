using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// R1.10 reference-renderer soak. It enters the real Home Hub New Game path, travels through the
    /// canonical W000 -> ToxicCity -> W000 sequence, waits for the matching RuntimeHealthMonitor sweep
    /// at every settled destination, then samples a bounded frame window before initiating the next
    /// hop. Results are Linux/reference-renderer regression evidence, never a Quest device budget.
    /// </summary>
    public sealed class RecoveryGoldenPerformanceRouteTests
    {
        private const float BootLoadTimeoutSeconds = 120f;
        private const float BootReadyTimeoutSeconds = 30f;
        private const float TravelTimeoutSeconds = 120f;
        private const float SweepTimeoutSeconds = 45f;
        private const int FrameSampleCount = 120;

        private readonly List<string> _logs = new List<string>();
        private readonly List<string> _completedDestinations = new List<string>();
        private readonly List<RecoveryPerformanceArtifactPaths> _artifacts =
            new List<RecoveryPerformanceArtifactPaths>();

        private Application.LogCallback _logCallback;
        private RecoverySaveFileBackup _saveBackup;
        private RecoveryActualRigControllerSimulation _simulation;
        private bool _bootReady;
        private PlayerProfile _newGameProfile;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);
            yield return RecoverySceneTestIsolation.PrepareFreshGoldenBoot();

            _logs.Clear();
            _completedDestinations.Clear();
            _artifacts.Clear();
            _bootReady = false;
            _newGameProfile = null;
            _logCallback = (condition, stackTrace, type) => _logs.Add(condition);
            Application.logMessageReceived += _logCallback;
            HomeHubRuntime.BootPresentationReady += OnBootReady;
            HomeHubRuntime.NewGameProfileCreated += OnNewGameProfileCreated;
            TravelCoordinator.TravelCompleted += OnTravelCompleted;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            _simulation?.Dispose();
            _simulation = null;
            if (_logCallback != null) Application.logMessageReceived -= _logCallback;
            _logCallback = null;
            HomeHubRuntime.BootPresentationReady -= OnBootReady;
            HomeHubRuntime.NewGameProfileCreated -= OnNewGameProfileCreated;
            TravelCoordinator.TravelCompleted -= OnTravelCompleted;

            yield return RecoverySceneTestIsolation.ResetToEmptyFullDevelopment();
            _saveBackup?.Dispose();
            _saveBackup = null;
        }

        [UnityTest]
        public IEnumerator GoldenRoute_WaitsForHealthSweepsAndWritesThreePerformanceSamples()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneBoot));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneW000));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneToxicCity));

            yield return LoadActualBootAndInstallSimulator();
            Assert.IsFalse(SaveSystem.HasExistingProfile,
                "The performance soak inherited a profile instead of presenting clean New Game.");

            XRInteractionManager manager = FindRequired<XRInteractionManager>();
            Assert.IsNotNull(_simulation,
                "The actual tracked-rig simulator was not installed during cold boot.");
            Assert.IsTrue(_simulation.RightRay.isActiveAndEnabled &&
                          _simulation.RightRay.gameObject.activeInHierarchy,
                "The actual right controller ray did not become active for performance-route New Game.");

            SelectHomeHubTileThroughXri("Tile_NEW_GAME", manager, _simulation.RightRay);
            Assert.IsNotNull(_newGameProfile,
                "The actual New Game selection did not create a performance-route profile.");

            yield return WaitForCompletedDestination(1, ZiptideConstants.SceneW000);
            yield return AssertProductionInputOwnershipSurvivesTravel(manager);
            yield return WaitForSweepAndCapture(
                ZiptideConstants.SceneW000,
                sweepOccurrence: 1,
                label: "R1_10_W000_FIRST_POST_SWEEP",
                stem: "r1_10_w000_first_post_sweep");

            TravelCoordinator.TravelTo(ZiptideConstants.SceneToxicCity);
            yield return WaitForCompletedDestination(2, ZiptideConstants.SceneToxicCity);
            yield return AssertProductionInputOwnershipSurvivesTravel(manager);
            yield return WaitForSweepAndCapture(
                ZiptideConstants.SceneToxicCity,
                sweepOccurrence: 1,
                label: "R1_10_TOXIC_CITY_POST_SWEEP",
                stem: "r1_10_toxic_city_post_sweep");

            TravelCoordinator.TravelTo(ZiptideConstants.SceneW000);
            yield return WaitForCompletedDestination(3, ZiptideConstants.SceneW000);
            yield return AssertProductionInputOwnershipSurvivesTravel(manager);
            yield return WaitForSweepAndCapture(
                ZiptideConstants.SceneW000,
                sweepOccurrence: 2,
                label: "R1_10_W000_RETURN_POST_SWEEP",
                stem: "r1_10_w000_return_post_sweep");

            CollectionAssert.AreEqual(
                new[]
                {
                    ZiptideConstants.SceneW000,
                    ZiptideConstants.SceneToxicCity,
                    ZiptideConstants.SceneW000
                },
                _completedDestinations,
                "Performance soak did not follow the canonical Golden travel sequence.");
            Assert.AreEqual(3, _artifacts.Count,
                "Performance soak did not produce all three post-sweep artifacts.");
            Assert.AreEqual(0, CountLogs("ZIPTIDE: TRAVEL_FAIL"));
            Assert.AreEqual(0, CountLogs("ZIPTIDE: XRI_NOT_READY"));
            Assert.AreEqual(0, CountLogs("ZIPTIDE: RECOVERY_VIRTUAL_XR_READ_FAIL"));
            Assert.AreEqual(0, CountLogs("ZIPTIDE: INPUT_MUTATION_SETTLE_FAIL"),
                "The virtual XR harness raced a production input-mutation settle window.");
        }

        private IEnumerator LoadActualBootAndInstallSimulator()
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(
                ZiptideConstants.SceneBoot,
                LoadSceneMode.Single);
            Assert.IsNotNull(load, "Unity did not start the actual _Boot scene load.");
            float loadDeadline = Time.realtimeSinceStartup + BootLoadTimeoutSeconds;
            while (!load.isDone && Time.realtimeSinceStartup < loadDeadline) yield return null;
            Assert.IsTrue(load.isDone,
                "Actual _Boot did not load within " + BootLoadTimeoutSeconds + " seconds.");

            RecoverySceneTestIsolation.InvokeAllowedAfterSceneLoadBootstraps();

            // Headless Linux needs the full canonical action-asset refresh immediately after the scene and
            // its allowed bootstraps exist. Waiting for HomeHubRuntime readiness first consumed the
            // production two-second settle deadline; the other actual-route recovery tests already use
            // this early installation order and settle cleanly.
            PlayerRigPersistence rig = FindRequired<PlayerRigPersistence>();
            XRInteractionManager manager = FindRequired<XRInteractionManager>();
            _simulation = RecoveryActualRigControllerSimulation.Activate(rig, manager);
            yield return null;
            yield return null;

            float readyDeadline = Time.realtimeSinceStartup + BootReadyTimeoutSeconds;
            while (!_bootReady && Time.realtimeSinceStartup < readyDeadline) yield return null;
            Assert.IsTrue(_bootReady,
                "Actual Home Hub did not become ready within " + BootReadyTimeoutSeconds + " seconds.");
            Assert.AreEqual(ZiptideConstants.SceneBoot, SceneManager.GetActiveScene().name);
            Assert.IsNotNull(FindRequired<HomeHubRuntime>());
            Assert.IsNotNull(rig);
            Assert.IsNotNull(manager);
            Assert.IsNotNull(FindRequired<RuntimeHealthMonitor>());
        }

        private IEnumerator WaitForCompletedDestination(int expectedCount, string expectedScene)
        {
            float deadline = Time.realtimeSinceStartup + TravelTimeoutSeconds;
            while (_completedDestinations.Count < expectedCount && Time.realtimeSinceStartup < deadline)
            {
                if (CountLogs("ZIPTIDE: TRAVEL_FAIL") > 0) break;
                yield return null;
            }

            Assert.GreaterOrEqual(_completedDestinations.Count, expectedCount,
                "Performance travel did not complete to " + expectedScene + " within " +
                TravelTimeoutSeconds + " seconds. Recent logs:\n" + RecentLogs(30));
            Assert.AreEqual(expectedScene, _completedDestinations[expectedCount - 1]);
            Assert.AreEqual(expectedScene, SceneManager.GetActiveScene().name);
            Assert.IsFalse(TravelCoordinator.IsTravelling);
            yield return null;
            yield return null;
        }

        private IEnumerator AssertProductionInputOwnershipSurvivesTravel(
            XRInteractionManager manager)
        {
            InputActionManager inputManager = manager.GetComponent<InputActionManager>();
            Assert.IsNotNull(inputManager,
                "The canonical interaction manager lost its InputActionManager after travel.");

            for (int pass = 0; pass < 2; pass++)
            {
                int disabledAnchorActions = 0;
                int readableSnapTurnActions = 0;
                var seen = new HashSet<InputAction>();
                foreach (InputActionAsset asset in inputManager.actionAssets)
                {
                    if (asset == null) continue;
                    foreach (InputActionMap map in asset.actionMaps)
                    {
                        foreach (InputAction action in map.actions)
                        {
                            if (action == null || !seen.Add(action)) continue;
                            if (action.name == "Rotate Anchor" ||
                                action.name == "Translate Anchor")
                            {
                                disabledAnchorActions++;
                                Assert.IsFalse(action.enabled,
                                    "Production-disabled anchor action was re-enabled by the virtual XR " +
                                    "harness after travel: " + map.name + "/" + action.name +
                                    " pass=" + pass);
                            }
                            else if (action.name == "Snap Turn")
                            {
                                Assert.DoesNotThrow(
                                    () => { action.ReadValue<Vector2>(); },
                                    "Snap Turn became unreadable after production input ownership " +
                                    "settled: " + map.name + "/" + action.name + " pass=" + pass);
                                readableSnapTurnActions++;
                            }
                        }
                    }
                }

                Assert.AreEqual(4, disabledAnchorActions,
                    "The canonical input asset no longer exposes the four production anchor actions.");
                Assert.AreEqual(2, readableSnapTurnActions,
                    "The canonical input asset no longer exposes two readable Snap Turn actions.");

                if (pass == 0)
                {
                    yield return null;
                    yield return null;
                }
            }

            Debug.Log(
                "ZIPTIDE: RECOVERY_INPUT_OWNERSHIP_OK anchorsDisabled=4 snapTurnReadable=2");
        }

        private IEnumerator WaitForSweepAndCapture(
            string sceneName,
            int sweepOccurrence,
            string label,
            string stem)
        {
            string prefix = "ZIPTIDE: HEALTH_SWEEP scene=" + sceneName;
            float deadline = Time.realtimeSinceStartup + SweepTimeoutSeconds;
            string sweepLine = null;
            while (Time.realtimeSinceStartup < deadline)
            {
                sweepLine = FindOccurrence(prefix, sweepOccurrence);
                if (sweepLine != null) break;
                yield return null;
            }

            Assert.IsNotNull(sweepLine,
                "No matching post-load health sweep arrived for " + sceneName +
                " occurrence " + sweepOccurrence + " within " + SweepTimeoutSeconds +
                " seconds. Recent logs:\n" + RecentLogs(30));
            Assert.AreEqual(sceneName, SceneManager.GetActiveScene().name,
                "The performance route left " + sceneName + " before its post-sweep sample.");

            // WaitForEndOfFrame does not resume reliably in Linux batch mode and previously consumed
            // an entire PlayMode timeout after otherwise-valid evidence had been produced. The sampler
            // only requires the post-sweep runtime to settle before its 120-frame window, so two
            // ordinary frames preserve the intended semantic without the headless editor dependency.
            yield return null;
            yield return null;
            var frames = new List<float>(FrameSampleCount);
            for (int i = 0; i < FrameSampleCount; i++)
            {
                yield return null;
                frames.Add(Time.unscaledDeltaTime * 1000f);
            }

            RecoveryPerformanceSampleRecord sample = RecoveryPerformanceSample.Capture(
                label,
                sweepLine,
                frames);
            Assert.AreEqual(sceneName, sample.scene,
                "Performance artifact captured a different active scene than its health sweep.");
            Assert.AreEqual(FrameSampleCount, sample.sampledFrames);
            Assert.GreaterOrEqual(sample.averageFrameMs, 0f);
            Assert.GreaterOrEqual(sample.p95FrameMs, sample.medianFrameMs);
            RecoveryPerformanceArtifactPaths paths = RecoveryPerformanceSample.WriteArtifacts(
                sample,
                stem);
            Assert.IsTrue(File.Exists(paths.JsonPath));
            Assert.IsTrue(File.Exists(paths.MarkdownPath));
            _artifacts.Add(paths);
        }

        private void SelectHomeHubTileThroughXri(
            string tileName,
            XRInteractionManager manager,
            UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor ray)
        {
            Transform tileTransform = FindTransform(tileName);
            Assert.IsNotNull(tileTransform, "Actual Home Hub tile was not found: " + tileName);
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable tile = tileTransform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            Assert.IsNotNull(tile, tileName + " is not an XRSimpleInteractable.");
            Assert.AreSame(manager, tile.interactionManager,
                tileName + " is not bound to the canonical interaction manager.");
            Assert.AreSame(manager, ray.interactionManager,
                "The actual right ray is not bound to the canonical interaction manager.");

            HomeHubRuntime home = FindRequired<HomeHubRuntime>();
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

            var hoverInteractor = (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRHoverInteractor)ray;
            var selectInteractor = (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)ray;
            var hoverInteractable = (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable)tile;
            var selectInteractable = (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)tile;
            if (!tile.isHovered) manager.HoverEnter(hoverInteractor, hoverInteractable);
            Assert.IsTrue(tile.isHovered, tileName + " did not enter hover state.");
            manager.SelectEnter(selectInteractor, selectInteractable);
            Assert.IsTrue(tile.isSelected, tileName + " did not enter selected state.");
            manager.SelectExit(selectInteractor, selectInteractable);
            if (tile.isHovered) manager.HoverExit(hoverInteractor, hoverInteractable);
        }

        private void OnBootReady(bool canContinue) => _bootReady = true;
        private void OnNewGameProfileCreated(PlayerProfile profile) => _newGameProfile = profile;
        private void OnTravelCompleted(string destination) => _completedDestinations.Add(destination);

        private int CountLogs(string prefix)
        {
            int count = 0;
            for (int i = 0; i < _logs.Count; i++)
                if (_logs[i] != null && _logs[i].StartsWith(prefix, StringComparison.Ordinal)) count++;
            return count;
        }

        private string FindOccurrence(string prefix, int occurrence)
        {
            int count = 0;
            for (int i = 0; i < _logs.Count; i++)
            {
                string value = _logs[i];
                if (value == null || !value.StartsWith(prefix, StringComparison.Ordinal)) continue;
                count++;
                if (count == occurrence) return value;
            }
            return null;
        }

        private string RecentLogs(int maximum)
        {
            int start = Math.Max(0, _logs.Count - maximum);
            var builder = new StringBuilder();
            for (int i = start; i < _logs.Count; i++) builder.AppendLine(_logs[i]);
            return builder.ToString();
        }

        private static T FindRequired<T>() where T : UnityEngine.Object
        {
            T value = UnityEngine.Object.FindObjectOfType<T>();
            Assert.IsNotNull(value, "Required runtime owner is missing: " + typeof(T).FullName);
            return value;
        }

        private static Transform FindTransform(string name)
        {
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (go != null && go.scene.IsValid() && go.name == name) return go.transform;
            }
            return null;
        }
    }
}

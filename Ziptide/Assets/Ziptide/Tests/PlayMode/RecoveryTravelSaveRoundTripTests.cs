using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
    /// R1.6 actual-scene travel/save proof:
    /// _Boot NEW GAME (real XRI selection) -> W000 -> ToxicCity -> W000.
    /// After the initial bootstrap scene load every hop must go through TravelCoordinator. The test
    /// proves one persistent rig/input/save/travel composition, spawn settlement, autosave to disk,
    /// profile continuity, reload continuity, and zero census/artifact blockers at every destination.
    /// </summary>
    public sealed class RecoveryTravelSaveRoundTripTests
    {
        private const string ProbeFlag = "RECOVERY_R1_6_ROUNDTRIP";
        private const string ProbeResource = "recovery_r1_6_probe";
        private const double ProbeAmount = 7d;
        private const float TravelTimeoutSeconds = 120f;
        private const float SpawnHorizontalTolerance = 0.05f;

        private readonly List<string> _logs = new List<string>();
        private readonly List<string> _completedDestinations = new List<string>();
        private Application.LogCallback _logCallback;
        private RecoverySaveFileBackup _saveBackup;
        private RecoveryActualRigControllerSimulation _controllerSimulation;
        private bool _bootReady;
        private PlayerProfile _newGameProfile;

        private sealed class PersistentIdentity
        {
            public int RigId;
            public int XriManagerId;
            public int InputManagerId;
            public int SaveSystemId;
            public int TravelCoordinatorId;
            public int AudioDirectorId;
            public int[] InputAssetIds;
            public string PlayerId;
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _saveBackup = RecoverySaveFileBackup.CaptureAndClear(SaveSystem.SavePath);
            yield return RecoverySceneTestIsolation.PrepareFreshGoldenBoot();

            _logs.Clear();
            _completedDestinations.Clear();
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
            _controllerSimulation?.Dispose();
            _controllerSimulation = null;
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
        public IEnumerator NewGame_W000_ToxicCity_W000_PreservesCompositionAndDiskState()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneBoot));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneW000));
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneToxicCity));

            yield return LoadActualBoot();
            Assert.IsFalse(SaveSystem.HasExistingProfile,
                "The isolated round-trip inherited a profile instead of presenting a clean New Game.");

            PersistentIdentity identity = CapturePersistentIdentity();
            PlayerRigPersistence rig = FindRequired<PlayerRigPersistence>();
            XRInteractionManager manager = FindRequired<XRInteractionManager>();
            Assert.IsTrue(ReadBootHold(rig).Held,
                "BOOT_HOLD was not armed before NEW GAME selection.");

            RecoveryRuntimeCensusSnapshot bootCensus = RecoveryRuntimeCensus.Capture(
                "R1_6_BOOT_BEFORE_NEW_GAME");
            RecoveryRuntimeCensus.WriteArtifacts(bootCensus, "r1_6_boot_before_new_game");
            AssertNoBlockers(bootCensus, ZiptideConstants.SceneBoot);
            AssertNoForbiddenRuntimeArtifacts("r1_6_boot_before_new_game");
            Assert.AreEqual(1, ActiveManagerCount(bootCensus, "XRInteractionManager"));
            Assert.AreEqual(1, ActiveManagerCount(bootCensus, "InputActionManager"));

            _controllerSimulation = RecoveryActualRigControllerSimulation.Activate(rig, manager);
            yield return null;
            yield return null;
            Assert.IsTrue(_controllerSimulation.RightRay.isActiveAndEnabled &&
                          _controllerSimulation.RightRay.gameObject.activeInHierarchy,
                "The actual right controller ray did not become active for NEW GAME selection.");

            SelectHomeHubTileThroughXri("Tile_NEW_GAME", manager, _controllerSimulation.RightRay);
            Assert.IsNotNull(_newGameProfile,
                "The actual NEW GAME XRI selection did not create a profile before travel.");
            identity.PlayerId = _newGameProfile.playerId;
            Assert.IsFalse(string.IsNullOrEmpty(identity.PlayerId));

            yield return WaitForCompletedDestination(1, ZiptideConstants.SceneW000);
            AssertSettledWorld(identity, ZiptideConstants.SceneW000, "r1_6_w000_first_arrival");
            // Golden visual proof AFTER the arrival transition clears — capturing inside the opaque
            // crest flash is what produced the blank pale-cyan ToxicCity frame in run 29494329424.
            yield return RecoveryGoldenTravelVisualCapture.CaptureSettledPending(
                ZiptideConstants.SceneW000);

            PlayerProfile live = SaveSystem.Instance.Profile;
            Assert.AreEqual(identity.PlayerId, live.playerId);
            live.SetFlag(ProbeFlag);
            Assert.AreEqual(ProbeAmount, live.AddResource(ProbeResource, ProbeAmount));
            Assert.IsTrue(live.HasFlag(ProbeFlag));
            Assert.AreEqual(ProbeAmount, live.GetResource(ProbeResource));

            TravelCoordinator.TravelTo(ZiptideConstants.SceneToxicCity);
            yield return WaitForCompletedDestination(2, ZiptideConstants.SceneToxicCity);
            AssertSettledWorld(identity, ZiptideConstants.SceneToxicCity, "r1_6_toxic_city_arrival");
            yield return RecoveryGoldenTravelVisualCapture.CaptureSettledPending(
                ZiptideConstants.SceneToxicCity);
            AssertLiveProfile(identity.PlayerId);
            AssertDiskProfile(identity.PlayerId);

            TravelCoordinator.TravelTo(ZiptideConstants.SceneW000);
            yield return WaitForCompletedDestination(3, ZiptideConstants.SceneW000);
            AssertSettledWorld(identity, ZiptideConstants.SceneW000, "r1_6_w000_return");
            AssertLiveProfile(identity.PlayerId);
            AssertDiskProfile(identity.PlayerId);

            SaveSystem.Instance.Load();
            AssertLiveProfile(identity.PlayerId);

            CollectionAssert.AreEqual(
                new[]
                {
                    ZiptideConstants.SceneW000,
                    ZiptideConstants.SceneToxicCity,
                    ZiptideConstants.SceneW000
                },
                _completedDestinations,
                "TravelCompleted did not report the exact Golden round-trip sequence.");

            Assert.AreEqual(2, CountLogs("ZIPTIDE: TRAVEL_START dest=" + ZiptideConstants.SceneW000));
            Assert.AreEqual(2, CountLogs("ZIPTIDE: TRAVEL_OK dest=" + ZiptideConstants.SceneW000));
            Assert.AreEqual(1, CountLogs("ZIPTIDE: TRAVEL_START dest=" + ZiptideConstants.SceneToxicCity));
            Assert.AreEqual(1, CountLogs("ZIPTIDE: TRAVEL_OK dest=" + ZiptideConstants.SceneToxicCity));
            Assert.AreEqual(3, CountLogs("ZIPTIDE: SAVE_AUTOSAVE reason=travel"),
                "Each real travel hop must own exactly one travel autosave.");
            Assert.AreEqual(0, CountLogs("ZIPTIDE: TRAVEL_FAIL"),
                "The round-trip emitted a travel failure despite completion events.");
            Assert.AreEqual(0, CountLogs("ZIPTIDE: XRI_NOT_READY"),
                "A destination completed without canonical XRI readiness.");

            // Deferred golden captures may never silently skip: both destinations must have produced
            // their settled frame + UI proofs by the end of the round trip.
            Assert.AreEqual(2, RecoveryGoldenTravelVisualCapture.CapturedCount,
                "The golden visual capture set is incomplete — a deferred capture was skipped.");
        }

        private IEnumerator LoadActualBoot()
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(
                ZiptideConstants.SceneBoot,
                LoadSceneMode.Single);
            Assert.IsNotNull(load, "Unity did not start the actual _Boot scene load.");
            // REAL-TIME budgets, not frames — headless batchmode burns frame counts in fractions of
            // a second while disk-bound loads still run (the run-29496812449 boot-smoke flake class).
            float loadDeadline = Time.realtimeSinceStartup + 30f;
            while (Time.realtimeSinceStartup < loadDeadline && !load.isDone) yield return null;
            Assert.IsTrue(load.isDone, "Actual _Boot scene load did not complete within 30 s.");

            RecoverySceneTestIsolation.InvokeAllowedAfterSceneLoadBootstraps();
            float readyDeadline = Time.realtimeSinceStartup + 15f;
            while (Time.realtimeSinceStartup < readyDeadline && !_bootReady) yield return null;
            Assert.IsTrue(_bootReady, "The actual Home Hub never reached ready state.");
            Assert.AreEqual(ZiptideConstants.SceneBoot, SceneManager.GetActiveScene().name);
            Assert.IsNotNull(FindRequired<HomeHubRuntime>());
            Assert.IsNotNull(FindRequired<PlayerRigPersistence>());
            Assert.IsNotNull(FindRequired<XRInteractionManager>());
            Assert.IsNotNull(CanonicalInputManager());
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

        private IEnumerator WaitForCompletedDestination(int expectedCount, string expectedScene)
        {
            float deadline = Time.realtimeSinceStartup + TravelTimeoutSeconds;
            while (_completedDestinations.Count < expectedCount && Time.realtimeSinceStartup < deadline)
            {
                if (CountLogs("ZIPTIDE: TRAVEL_FAIL") > 0) break;
                yield return null;
            }

            Assert.GreaterOrEqual(_completedDestinations.Count, expectedCount,
                "Travel did not complete to " + expectedScene + " within " +
                TravelTimeoutSeconds + " seconds. Recent logs:\n" + RecentLogs(30));
            Assert.AreEqual(expectedScene, _completedDestinations[expectedCount - 1]);
            Assert.AreEqual(expectedScene, SceneManager.GetActiveScene().name,
                "TravelCompleted fired while another scene was active.");
            Assert.IsFalse(TravelCoordinator.IsTravelling,
                "TravelCompleted fired before TravelCoordinator released its travelling state.");
            yield return null;
            yield return null;
        }

        private PersistentIdentity CapturePersistentIdentity()
        {
            PlayerRigPersistence rig = FindRequired<PlayerRigPersistence>();
            XRInteractionManager manager = FindRequired<XRInteractionManager>();
            InputActionManager input = CanonicalInputManager();
            SaveSystem save = FindRequired<SaveSystem>();
            TravelCoordinator travel = FindRequired<TravelCoordinator>();
            AudioDirector audio = FindRequired<AudioDirector>();

            int[] assets = InputAssetIds(input, assertEnabled: true);
            Assert.Greater(assets.Length, 0,
                "The canonical InputActionManager owns no action assets at Home Hub settlement.");
            return new PersistentIdentity
            {
                RigId = rig.GetInstanceID(),
                XriManagerId = manager.GetInstanceID(),
                InputManagerId = input.GetInstanceID(),
                SaveSystemId = save.GetInstanceID(),
                TravelCoordinatorId = travel.GetInstanceID(),
                AudioDirectorId = audio.GetInstanceID(),
                InputAssetIds = assets
            };
        }

        private void AssertSettledWorld(
            PersistentIdentity identity,
            string expectedScene,
            string censusStem)
        {
            Assert.AreEqual(expectedScene, SceneManager.GetActiveScene().name);
            Assert.IsFalse(TravelCoordinator.IsTravelling);
            Assert.IsNull(UnityEngine.Object.FindObjectOfType<HomeHubRuntime>(),
                "HomeHubRuntime leaked into content scene " + expectedScene + ".");

            PlayerRigPersistence rig = FindRequired<PlayerRigPersistence>();
            XRInteractionManager manager = FindRequired<XRInteractionManager>();
            InputActionManager input = CanonicalInputManager();
            SaveSystem save = FindRequired<SaveSystem>();
            TravelCoordinator travel = FindRequired<TravelCoordinator>();
            AudioDirector audio = FindRequired<AudioDirector>();

            Assert.AreEqual(identity.RigId, rig.GetInstanceID(), "Persistent rig identity changed.");
            Assert.AreEqual(identity.XriManagerId, manager.GetInstanceID(), "XRI manager identity changed.");
            Assert.AreEqual(identity.InputManagerId, input.GetInstanceID(), "Input manager identity changed.");
            Assert.AreEqual(identity.SaveSystemId, save.GetInstanceID(), "SaveSystem identity changed.");
            Assert.AreEqual(identity.TravelCoordinatorId, travel.GetInstanceID(),
                "TravelCoordinator identity changed.");
            Assert.AreEqual(identity.AudioDirectorId, audio.GetInstanceID(), "AudioDirector identity changed.");
            CollectionAssert.AreEqual(identity.InputAssetIds, InputAssetIds(input, assertEnabled: true),
                "The canonical input asset set changed across travel.");

            BootHoldState hold = ReadBootHold(rig);
            Assert.IsNotNull(hold);
            Assert.IsFalse(hold.Held, "BOOT_HOLD remained active after content spawn settlement.");
            AssertSpawnAlignment(rig, expectedScene);

            RecoveryRuntimeCensusSnapshot census = RecoveryRuntimeCensus.Capture(
                "R1_6_" + expectedScene + "_SETTLED");
            RecoveryRuntimeCensus.WriteArtifacts(census, censusStem);
            AssertNoBlockers(census, expectedScene);
            AssertNoForbiddenRuntimeArtifacts(censusStem);
            Assert.AreEqual(1, ActiveManagerCount(census, "XRInteractionManager"));
            Assert.AreEqual(1, ActiveManagerCount(census, "InputActionManager"));
            Assert.AreEqual(1, ActiveCameraRoleCount(census, "CANONICAL_PLAYER_VIEW"));
            AssertOwnerActiveExactlyOnce(census, "SAVE_SYSTEM_BOOTSTRAP");
            AssertOwnerActiveExactlyOnce(census, "PLAYER_RIG_PERSISTENCE");
            AssertOwnerActiveExactlyOnce(census, "AUDIO_DIRECTOR");
            AssertOwnerActiveExactlyOnce(census, "TRAVEL_COORDINATOR");
        }

        private static void AssertSpawnAlignment(PlayerRigPersistence rig, string sceneName)
        {
            SpawnMarkerRuntime marker = null;
            SpawnMarkerRuntime[] markers = UnityEngine.Object.FindObjectsOfType<SpawnMarkerRuntime>();
            for (int i = 0; i < markers.Length; i++)
            {
                if (markers[i] != null && markers[i].markerId == "player")
                {
                    marker = markers[i];
                    break;
                }
            }
            Assert.IsNotNull(marker, sceneName + " has no canonical player spawn marker.");

            Camera camera = rig.GetComponentInChildren<Camera>(true);
            Assert.IsNotNull(camera, "Persistent rig has no tracked-head camera.");
            Vector2 head = new Vector2(camera.transform.position.x, camera.transform.position.z);
            Vector2 target = new Vector2(marker.transform.position.x, marker.transform.position.z);
            float distance = Vector2.Distance(head, target);
            Assert.LessOrEqual(distance, SpawnHorizontalTolerance,
                sceneName + " head XZ missed its spawn marker by " + distance.ToString("F4") + "m.");
        }

        private void AssertLiveProfile(string expectedPlayerId)
        {
            Assert.IsNotNull(SaveSystem.Instance);
            PlayerProfile profile = SaveSystem.Instance.Profile;
            Assert.IsNotNull(profile);
            Assert.AreEqual(expectedPlayerId, profile.playerId);
            Assert.IsTrue(profile.HasFlag(ProbeFlag), "Recovery probe flag was lost from live state.");
            Assert.AreEqual(ProbeAmount, profile.GetResource(ProbeResource),
                "Recovery probe resource was lost from live state.");
        }

        private static void AssertDiskProfile(string expectedPlayerId)
        {
            Assert.IsTrue(File.Exists(SaveSystem.SavePath),
                "Travel autosave did not create the main profile file.");
            string json = File.ReadAllText(SaveSystem.SavePath);
            Assert.IsTrue(ProfileSerializer.TryDeserialize(json, out PlayerProfile profile),
                "Travel autosave main profile does not deserialize.");
            Assert.AreEqual(expectedPlayerId, profile.playerId);
            Assert.IsTrue(profile.HasFlag(ProbeFlag), "Recovery probe flag was not persisted to disk.");
            Assert.AreEqual(ProbeAmount, profile.GetResource(ProbeResource),
                "Recovery probe resource was not persisted to disk.");
        }

        private static InputActionManager CanonicalInputManager()
        {
            XRInteractionManager manager = FindRequired<XRInteractionManager>();
            InputActionManager input = manager.GetComponent<InputActionManager>();
            Assert.IsNotNull(input,
                "The canonical XRInteractionManager has no InputActionManager beside it.");
            Assert.IsTrue(input.enabled, "The canonical InputActionManager is disabled.");
            return input;
        }

        private static int[] InputAssetIds(InputActionManager manager, bool assertEnabled)
        {
            var ids = new List<int>();
            if (manager != null && manager.actionAssets != null)
            {
                foreach (InputActionAsset asset in manager.actionAssets)
                {
                    if (asset == null) continue;
                    if (assertEnabled)
                        Assert.IsTrue(asset.enabled,
                            "Input action asset became disabled: " + asset.name);
                    ids.Add(asset.GetInstanceID());
                }
            }
            ids.Sort();
            return ids.ToArray();
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

        private static BootHoldState ReadBootHold(PlayerRigPersistence rig)
        {
            FieldInfo field = typeof(PlayerRigPersistence).GetField(
                "_bootHold",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return field != null ? field.GetValue(rig) as BootHoldState : null;
        }

        private static int ActiveManagerCount(
            RecoveryRuntimeCensusSnapshot snapshot,
            string category)
        {
            int count = 0;
            for (int i = 0; i < snapshot.managers.Count; i++)
            {
                RecoveryCensusComponentRecord record = snapshot.managers[i];
                if (record.category == category && record.active && record.enabled) count++;
            }
            return count;
        }

        private static int ActiveCameraRoleCount(
            RecoveryRuntimeCensusSnapshot snapshot,
            string role)
        {
            int count = 0;
            for (int i = 0; i < snapshot.cameras.Count; i++)
            {
                RecoveryCensusComponentRecord record = snapshot.cameras[i];
                if (record.role == role && record.active && record.enabled) count++;
            }
            return count;
        }

        private static void AssertOwnerActiveExactlyOnce(
            RecoveryRuntimeCensusSnapshot snapshot,
            string ownerId)
        {
            for (int i = 0; i < snapshot.automaticOwners.Count; i++)
            {
                RecoveryCensusOwnerRecord owner = snapshot.automaticOwners[i];
                if (owner.ownerId != ownerId) continue;
                Assert.AreEqual(1, owner.activeCount,
                    ownerId + " active count was " + owner.activeCount + ".");
                return;
            }
            Assert.Fail("Census omitted canonical owner " + ownerId + ".");
        }

        private static void AssertNoBlockers(
            RecoveryRuntimeCensusSnapshot snapshot,
            string sceneName)
        {
            var builder = new StringBuilder();
            int blockers = 0;
            for (int i = 0; i < snapshot.findings.Count; i++)
            {
                RecoveryCensusFinding finding = snapshot.findings[i];
                if (finding.severity != "BLOCKER") continue;
                blockers++;
                builder.AppendLine(finding.code + " owner=" + finding.owner +
                    " path=" + finding.hierarchyPath + " message=" + finding.message);
            }
            Assert.AreEqual(0, blockers,
                sceneName + " census produced blockers:\n" + builder);
        }

        private static void AssertNoForbiddenRuntimeArtifacts(string stem)
        {
            RecoveryRuntimeArtifactReport report = RecoveryRuntimeArtifactGuard.Capture(stem);
            string path = RecoveryRuntimeArtifactGuard.WriteArtifact(report, stem);
            var builder = new StringBuilder();
            for (int i = 0; i < report.findings.Count; i++)
            {
                RecoveryRuntimeArtifactFinding finding = report.findings[i];
                builder.AppendLine(finding.code + " feature=" + finding.featureId +
                    " path=" + finding.hierarchyPath + " message=" + finding.message);
            }
            Assert.AreEqual(0, report.findings.Count,
                "Forbidden runtime artifacts were active. Artifact=" + path + "\n" + builder);
        }
    }
}

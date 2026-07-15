using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// R1.5 actual-scene proof. Loads the authored/patched _Boot scene under a fresh Golden profile,
    /// verifies the real persistent composition, then selects SETTINGS through the actual rig's
    /// existing right controller ray and XRInteractionManager. CI supplies only tracked-device
    /// presence through RecoveryActualRigControllerSimulation; no private Home Hub method is invoked.
    /// </summary>
    public sealed class RecoveryBootSceneSmokeTests
    {
        private readonly List<string> _logs = new List<string>();
        private readonly List<HomeHubChoice> _choices = new List<HomeHubChoice>();
        private Application.LogCallback _logCallback;
        private RecoveryActualRigControllerSimulation _controllerSimulation;
        private bool _bootReady;
        private int _settingsRequested;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return RecoverySceneTestIsolation.PrepareFreshGoldenBoot();

            _logs.Clear();
            _choices.Clear();
            _bootReady = false;
            _settingsRequested = 0;
            _logCallback = (condition, stackTrace, type) => _logs.Add(condition);
            Application.logMessageReceived += _logCallback;
            HomeHubRuntime.BootPresentationReady += OnBootReady;
            HomeHubRuntime.ChoiceSelected += OnChoice;
            HomeHubRuntime.SettingsRequested += OnSettingsRequested;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            _controllerSimulation?.Dispose();
            _controllerSimulation = null;
            if (_logCallback != null) Application.logMessageReceived -= _logCallback;
            _logCallback = null;
            HomeHubRuntime.BootPresentationReady -= OnBootReady;
            HomeHubRuntime.ChoiceSelected -= OnChoice;
            HomeHubRuntime.SettingsRequested -= OnSettingsRequested;
            yield return RecoverySceneTestIsolation.ResetToEmptyFullDevelopment();
        }

        [UnityTest]
        public IEnumerator ActualBoot_SettingsSelectsThroughXri_WithoutTravelOrOwnerClashes()
        {
            Assert.IsTrue(Application.CanStreamedLevelBeLoaded(ZiptideConstants.SceneBoot),
                "_Boot is absent from the PlayMode test player's Build Settings.");

            AsyncOperation load = SceneManager.LoadSceneAsync(
                ZiptideConstants.SceneBoot,
                LoadSceneMode.Single);
            Assert.IsNotNull(load, "Unity did not start loading the actual _Boot scene.");
            for (int frame = 0; frame < 600 && !load.isDone; frame++) yield return null;
            Assert.IsTrue(load.isDone, "Actual _Boot scene load did not complete within 600 frames.");

            RecoverySceneTestIsolation.InvokeAllowedAfterSceneLoadBootstraps();

            HomeHubRuntime home = null;
            PlayerRigPersistence rig = null;
            XRInteractionManager manager = null;
            InputActionManager inputManager = null;
            for (int frame = 0; frame < 300; frame++)
            {
                home = UnityEngine.Object.FindObjectOfType<HomeHubRuntime>();
                rig = UnityEngine.Object.FindObjectOfType<PlayerRigPersistence>();
                manager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();
                inputManager = UnityEngine.Object.FindObjectOfType<InputActionManager>();
                if (_bootReady && home != null && rig != null && manager != null && inputManager != null)
                    break;
                yield return null;
            }

            Assert.IsTrue(_bootReady, "The actual _Boot scene never published HOME_HUB_READY.");
            Assert.IsNotNull(home, "The actual _Boot scene has no HomeHubRuntime.");
            Assert.IsNotNull(rig, "The actual _Boot scene has no PlayerRigPersistence.");
            Assert.IsNotNull(manager, "The actual _Boot scene has no XRInteractionManager.");
            Assert.IsNotNull(inputManager, "The actual _Boot scene has no InputActionManager.");
            Assert.AreEqual(ZiptideConstants.SceneBoot, SceneManager.GetActiveScene().name,
                "The Home Hub travelled before the user selected a destination.");

            BootHoldState hold = ReadBootHold(rig);
            Assert.IsNotNull(hold, "The actual persistent rig has no boot-hold state.");
            Assert.IsTrue(hold.Held, "The actual _Boot scene exposed locomotion before a content spawn.");

            RecoveryRuntimeCensusSnapshot settled = RecoveryRuntimeCensus.Capture(
                "R1_5_ACTUAL_BOOT_HOME_SETTLED");
            RecoveryRuntimeCensus.WriteArtifacts(settled, "r1_5_actual_boot_home_settled");
            AssertNoBlockers(settled);
            Assert.AreEqual(1, ActiveManagerCount(settled, "XRInteractionManager"));
            Assert.AreEqual(1, ActiveManagerCount(settled, "InputActionManager"));
            Assert.AreEqual(1, ActiveCameraRoleCount(settled, "CANONICAL_PLAYER_VIEW"));
            AssertOwnerActiveExactlyOnce(settled, "SAVE_SYSTEM_BOOTSTRAP");
            AssertOwnerActiveExactlyOnce(settled, "PLAYER_RIG_PERSISTENCE");
            AssertOwnerActiveExactlyOnce(settled, "AUDIO_DIRECTOR");
            AssertOwnerActiveExactlyOnce(settled, "TRAVEL_COORDINATOR");

            Transform settingsTile = FindTransform("Tile_SETTINGS");
            Transform newGameTile = FindTransform("Tile_NEW_GAME");
            Assert.IsNotNull(settingsTile, "The actual Home Hub has no SETTINGS tile.");
            Assert.IsNotNull(newGameTile, "The actual Home Hub has no NEW GAME tile.");

            XRSimpleInteractable settings = settingsTile.GetComponent<XRSimpleInteractable>();
            XRSimpleInteractable newGame = newGameTile.GetComponent<XRSimpleInteractable>();
            Assert.IsNotNull(settings, "The actual SETTINGS tile is not an XRI interactable.");
            Assert.IsNotNull(newGame, "The actual NEW GAME tile is not an XRI interactable.");

            for (int frame = 0; frame < 120 &&
                 (settings.interactionManager == null || newGame.interactionManager == null); frame++)
                yield return null;

            Assert.AreSame(manager, settings.interactionManager,
                "SETTINGS did not bind to the canonical XRInteractionManager.");
            Assert.AreSame(manager, newGame.interactionManager,
                "NEW GAME did not bind to the canonical XRInteractionManager.");

            // Headless CI has no tracked XR devices, so the production modality manager correctly
            // keeps controller groups inactive. Activate the actual rig's existing right ray only;
            // no alternate ray, action map, interaction manager or UI path is created.
            _controllerSimulation = RecoveryActualRigControllerSimulation.Activate(rig, manager);
            yield return null;
            yield return null;
            XRRayInteractor ray = _controllerSimulation.RightRay;
            Assert.IsNotNull(ray, "The actual persistent rig has no right controller ray component.");
            Assert.IsTrue(ray.isActiveAndEnabled && ray.gameObject.activeInHierarchy,
                "The test-owned tracked-controller simulation did not activate the actual right ray: " +
                _controllerSimulation.RightRayPath);
            StringAssert.Contains("Right", _controllerSimulation.RightRayPath);
            StringAssert.DoesNotContain("Teleport", _controllerSimulation.RightRayPath);
            Assert.AreSame(manager, ray.interactionManager,
                "The actual right ray is bound to a different interaction manager.");

            Vector3 approach = home.transform.forward;
            Vector3 rayPosition = settingsTile.position - approach * 1.1f;
            Vector3 direction = (settingsTile.position - rayPosition).normalized;
            ray.transform.position = rayPosition;
            ray.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            Physics.SyncTransforms();

            Assert.IsTrue(Physics.Raycast(
                ray.transform.position,
                ray.transform.forward,
                out RaycastHit hit,
                3f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide),
                "The actual right ray did not physically hit the SETTINGS tile.");
            Assert.AreSame(settingsTile.gameObject, hit.collider.gameObject,
                "The actual right ray hit a different object before SETTINGS: " +
                RecoveryRuntimeCensus.HierarchyPath(hit.transform));

            int travelStartsBefore = CountLogs("ZIPTIDE: TRAVEL_START");
            var hoverInteractor = (IXRHoverInteractor)ray;
            var selectInteractor = (IXRSelectInteractor)ray;
            var hoverInteractable = (IXRHoverInteractable)settings;
            var selectInteractable = (IXRSelectInteractable)settings;

            if (!settings.isHovered) manager.HoverEnter(hoverInteractor, hoverInteractable);
            Assert.IsTrue(settings.isHovered, "SETTINGS did not enter XRI hover state.");
            manager.SelectEnter(selectInteractor, selectInteractable);
            Assert.IsTrue(settings.isSelected, "SETTINGS did not enter XRI selected state.");
            yield return null;
            manager.SelectExit(selectInteractor, selectInteractable);
            if (settings.isHovered) manager.HoverExit(hoverInteractor, hoverInteractable);
            yield return null;

            Assert.AreEqual(1, _settingsRequested,
                "SETTINGS selection did not publish exactly one settings request.");
            Assert.AreEqual(1, _choices.Count, "SETTINGS selection published the wrong number of choices.");
            Assert.AreEqual(HomeHubChoice.Settings, _choices[0]);
            Assert.AreEqual(travelStartsBefore, CountLogs("ZIPTIDE: TRAVEL_START"),
                "SETTINGS incorrectly initiated scene travel.");
            Assert.IsFalse(TravelCoordinator.IsTravelling,
                "TravelCoordinator entered travelling state after SETTINGS.");
            Assert.AreEqual(ZiptideConstants.SceneBoot, SceneManager.GetActiveScene().name,
                "SETTINGS selection left the _Boot scene.");
            Assert.IsNotNull(FindTransform("__HOME_HUB_COMFORT_SETTINGS"),
                "SETTINGS did not create its bounded comfort console.");
            Assert.GreaterOrEqual(CountLogs("ZIPTIDE: BOARD_PROBE surface=HomeHub phase=select"), 1,
                "The actual Golden selection did not reach the persistent BOARD_PROBE evidence path.");

            RecoveryRuntimeCensusSnapshot afterSettings = RecoveryRuntimeCensus.Capture(
                "R1_5_ACTUAL_BOOT_AFTER_SETTINGS");
            RecoveryRuntimeCensus.WriteArtifacts(afterSettings, "r1_5_actual_boot_after_settings");
            AssertNoBlockers(afterSettings);
        }

        private void OnBootReady(bool canContinue) => _bootReady = true;
        private void OnChoice(HomeHubChoice choice) => _choices.Add(choice);
        private void OnSettingsRequested() => _settingsRequested++;

        private int CountLogs(string prefix)
        {
            int count = 0;
            for (int i = 0; i < _logs.Count; i++)
                if (_logs[i] != null && _logs[i].StartsWith(prefix, StringComparison.Ordinal)) count++;
            return count;
        }

        private static BootHoldState ReadBootHold(PlayerRigPersistence rig)
        {
            FieldInfo field = typeof(PlayerRigPersistence).GetField(
                "_bootHold",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return field != null ? field.GetValue(rig) as BootHoldState : null;
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

        private static void AssertNoBlockers(RecoveryRuntimeCensusSnapshot snapshot)
        {
            var text = new StringBuilder();
            int count = 0;
            for (int i = 0; i < snapshot.findings.Count; i++)
            {
                RecoveryCensusFinding finding = snapshot.findings[i];
                if (finding.severity != "BLOCKER") continue;
                count++;
                text.AppendLine(finding.code + " owner=" + finding.owner +
                    " path=" + finding.hierarchyPath + " message=" + finding.message);
            }
            Assert.AreEqual(0, count,
                "Actual Golden _Boot census produced blocker findings:\n" + text);
        }
    }
}

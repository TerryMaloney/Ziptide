using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// R1 checkpoint proof for the rb26 cold-boot fix. This runs the real BootLoader and persistent
    /// rig seam, then proves the observable ordering required on-device:
    /// hold armed -> Home Hub ready -> hold released at settled content spawn -> SPAWN_AT evidence.
    /// </summary>
    public sealed class RecoveryBootHoldOrderingTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();
        private readonly List<string> _logs = new List<string>();
        private Application.LogCallback _capture;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            DestroyAll<PlayerRigPersistence>();
            DestroyAll<HomeHubRuntime>();
            DestroyAll<BootLoader>();
            DestroyAll<XRInteractionManager>();
            yield return null;

            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            _logs.Clear();
            _capture = (condition, stackTrace, type) => _logs.Add(condition);
            Application.logMessageReceived += _capture;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_capture != null)
                Application.logMessageReceived -= _capture;
            _capture = null;

            for (int i = 0; i < _created.Count; i++)
                if (_created[i] != null) Object.DestroyImmediate(_created[i]);
            _created.Clear();

            DestroyAll<PlayerRigPersistence>();
            DestroyAll<HomeHubRuntime>();
            DestroyAll<BootLoader>();
            DestroyAll<XRInteractionManager>();
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BootLoader_ArmsBeforePresentation_AndSpawnReleasesBeforeSpawnEvidence()
        {
            var rigHost = new GameObject("__RECOVERY_BOOT_ORDER_RIG");
            _created.Add(rigHost);
            var rig = rigHost.AddComponent<PlayerRigPersistence>();

            var bootHost = new GameObject("__RECOVERY_BOOT_ORDER_LOADER");
            bootHost.SetActive(false);
            _created.Add(bootHost);
            bootHost.AddComponent<BootLoader>();
            bootHost.SetActive(true);

            // BootLoader.Start runs first; the HomeHubRuntime it creates receives Start next frame.
            yield return null;
            yield return null;

            var holdField = typeof(PlayerRigPersistence).GetField(
                "_bootHold", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(holdField, "PlayerRigPersistence boot-hold seam was not found.");
            var hold = holdField.GetValue(rig) as BootHoldState;
            Assert.IsNotNull(hold, "PlayerRigPersistence boot-hold state was not created.");
            Assert.IsTrue(hold.Held, "The Home Hub became ready without the cold-boot hold armed.");

            int armedIndex = FindLog("ZIPTIDE: BOOT_HOLD on");
            int readyIndex = FindLog("ZIPTIDE: HOME_HUB_READY");
            Assert.GreaterOrEqual(armedIndex, 0, "No BOOT_HOLD arm evidence was emitted.");
            Assert.Greater(readyIndex, armedIndex,
                "Home Hub readiness was emitted before BOOT_HOLD arm evidence.");

            var markerHost = new GameObject("__RECOVERY_BOOT_ORDER_MARKER");
            _created.Add(markerHost);
            markerHost.transform.position = new Vector3(0f, 1f, 0f);
            var marker = markerHost.AddComponent<SpawnMarkerRuntime>();
            marker.markerId = "player";

            rig.TeleportToMarker("player");
            Assert.IsFalse(hold.Held, "The settled content spawn did not release the boot hold.");

            int releasedIndex = FindLog("ZIPTIDE: BOOT_HOLD off");
            int spawnIndex = FindLog("ZIPTIDE: SPAWN_AT marker='player'");
            Assert.Greater(releasedIndex, readyIndex,
                "BOOT_HOLD released before the Home Hub presentation was established.");
            Assert.Greater(spawnIndex, releasedIndex,
                "SPAWN_AT evidence was emitted before locomotion/fall safety release completed.");
        }

        private int FindLog(string prefix)
        {
            for (int i = 0; i < _logs.Count; i++)
                if (_logs[i] != null && _logs[i].StartsWith(prefix)) return i;
            return -1;
        }

        private static void DestroyAll<T>() where T : Component
        {
            var all = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < all.Length; i++)
            {
                T component = all[i];
                if (component != null && component.gameObject.scene.IsValid())
                    Object.DestroyImmediate(component.gameObject);
            }
        }
    }
}

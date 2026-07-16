using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryInputSessionGuardTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private readonly List<InputActionAsset> _assets = new List<InputActionAsset>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);
            RecoverySceneTestIsolation.DestroyProductionRuntimeImmediate();
            DestroySceneManagersImmediate();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _objects.Count; i++)
                if (_objects[i] != null) Object.DestroyImmediate(_objects[i]);
            _objects.Clear();
            for (int i = 0; i < _assets.Count; i++)
                if (_assets[i] != null) Object.DestroyImmediate(_assets[i]);
            _assets.Clear();
            DestroySceneManagersImmediate();
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Consolidate_TransfersAssetsAndDisablesEveryDuplicateManager()
        {
            Assert.IsTrue(
                RecoveryExposureProfiles.GoldenSlice.Allows(RecoveryFeatureId.PlayerInputSessionGuard),
                "GoldenSlice does not enable the canonical input-session guard.");

            GameObject primaryHost = CreateHost("__RECOVERY_CANONICAL_XRI");
            primaryHost.AddComponent<XRInteractionManager>();
            InputActionManager primary = primaryHost.AddComponent<InputActionManager>();
            InputActionAsset primaryAsset = CreateAsset("PrimaryAsset", "PrimaryAction");
            InputAction primaryHeldOff = primaryAsset.FindActionMap("TestMap")
                .AddAction("Rotate Anchor", InputActionType.Value);
            primary.actionAssets = new List<InputActionAsset> { primaryAsset };
            primaryAsset.Enable();
            primaryHeldOff.Disable();

            GameObject duplicateHost = CreateHost("__RECOVERY_SCENE_INPUT_MANAGER");
            InputActionManager duplicate = duplicateHost.AddComponent<InputActionManager>();
            InputActionAsset duplicateAsset = CreateAsset("SceneAsset", "SceneAction");
            InputAction duplicateHeldOff = duplicateAsset.FindActionMap("TestMap")
                .AddAction("Translate Anchor", InputActionType.Value);
            duplicate.actionAssets = new List<InputActionAsset> { duplicateAsset };
            duplicateAsset.Enable();
            duplicateHeldOff.Disable();
            yield return null;

            int disabled = PlayerInputSessionGuard.Consolidate("controlled_test");

            Assert.AreEqual(1, disabled,
                "The known duplicate manager was not disabled exactly once.");
            Assert.IsTrue(primary.enabled, "The canonical InputActionManager was disabled.");
            Assert.IsFalse(duplicate.enabled, "The scene duplicate InputActionManager stayed enabled.");
            Assert.AreEqual(0, duplicate.actionAssets.Count,
                "The disabled duplicate retained shared action assets and can disable them on destroy.");
            CollectionAssert.AreEquivalent(
                new[] { primaryAsset, duplicateAsset },
                primary.actionAssets,
                "The canonical manager did not receive the full union of input assets.");
            Assert.IsTrue(primaryAsset.enabled, "Primary action asset was not enabled.");
            Assert.IsTrue(duplicateAsset.enabled, "Transferred scene action asset was not enabled.");
            Assert.IsFalse(primaryHeldOff.enabled,
                "Consolidation re-enabled a primary action that production intentionally disabled.");
            Assert.IsFalse(duplicateHeldOff.enabled,
                "Consolidation re-enabled a transferred action that production intentionally disabled.");

            int disabledAgain = PlayerInputSessionGuard.Consolidate("controlled_test_repeat");
            Assert.AreEqual(0, disabledAgain, "The input-session guard is not idempotent.");
            Assert.IsFalse(duplicate.enabled);
            Assert.AreEqual(0, duplicate.actionAssets.Count);
            Assert.IsFalse(primaryHeldOff.enabled,
                "Repeated consolidation changed primary per-action state.");
            Assert.IsFalse(duplicateHeldOff.enabled,
                "Repeated consolidation changed transferred per-action state.");

            MethodInfo readiness = typeof(PlayerRigPersistence).GetMethod(
                "ActionReadsSafely",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(readiness,
                "The input-mutation readiness seam is missing from PlayerRigPersistence.");

            InputAction referencedAction = primaryAsset.FindAction("PrimaryAction");
            referencedAction.Disable();
            InputActionReference referencedActionRef = InputActionReference.Create(referencedAction);
            var referencedProperty = new InputActionProperty(referencedActionRef);
            try
            {
                Assert.IsTrue((bool)readiness.Invoke(null, new object[] { referencedProperty }),
                    "A disabled externally managed reference was incorrectly treated as unstable.");
            }
            finally
            {
                if (referencedActionRef != null) Object.DestroyImmediate(referencedActionRef);
            }

            var directAction = new InputAction(
                "ControlledDirectTurn",
                InputActionType.Value,
                expectedControlType: "Vector2");
            var directProperty = new InputActionProperty(directAction);
            try
            {
                Assert.IsFalse((bool)readiness.Invoke(null, new object[] { directProperty }),
                    "A disabled direct action was accepted without being prepared first.");
                Assert.IsTrue(directAction.enabled,
                    "The provider-owned direct action was not enabled while its reader was suspended.");
                Assert.IsTrue((bool)readiness.Invoke(null, new object[] { directProperty }),
                    "The prepared direct action did not become readable on the following probe.");
            }
            finally
            {
                directAction.Dispose();
            }

            RecoveryRuntimeCensusSnapshot census = RecoveryRuntimeCensus.Capture(
                "R1_5_INPUT_SESSION_CONTROLLED");
            int activeManagers = 0;
            for (int i = 0; i < census.managers.Count; i++)
            {
                RecoveryCensusComponentRecord record = census.managers[i];
                if (record.category == "InputActionManager" && record.active && record.enabled)
                    activeManagers++;
            }
            Assert.AreEqual(1, activeManagers,
                "The runtime census still sees more than one enabled InputActionManager.");
        }

        private GameObject CreateHost(string name)
        {
            var host = new GameObject(name);
            _objects.Add(host);
            return host;
        }

        private InputActionAsset CreateAsset(string assetName, string actionName)
        {
            InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
            asset.name = assetName;
            InputActionMap map = asset.AddActionMap("TestMap");
            map.AddAction(actionName, InputActionType.Button);
            _assets.Add(asset);
            return asset;
        }

        private static void DestroySceneManagersImmediate()
        {
            var destroy = new HashSet<GameObject>();
            XRInteractionManager[] xri = Resources.FindObjectsOfTypeAll<XRInteractionManager>();
            for (int i = 0; i < xri.Length; i++)
                if (xri[i] != null && xri[i].gameObject.scene.IsValid()) destroy.Add(xri[i].gameObject);
            InputActionManager[] input = Resources.FindObjectsOfTypeAll<InputActionManager>();
            for (int i = 0; i < input.Length; i++)
                if (input[i] != null && input[i].gameObject.scene.IsValid()) destroy.Add(input[i].gameObject);
            foreach (GameObject go in destroy)
                if (go != null) Object.DestroyImmediate(go);
        }
    }
}

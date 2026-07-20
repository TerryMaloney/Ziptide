using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Subordinate enforcement for PlayerRigPersistence's canonical input-session contract.
    /// PlayerRigPersistence adopts one persistent XRInteractionManager and transfers every action
    /// asset to the InputActionManager beside it. Scene-authored InputActionManagers must then be
    /// disabled after their assets are transferred; leaving an empty manager enabled creates a
    /// second lifecycle owner whose OnEnable/OnDisable can still race the persistent session.
    ///
    /// This class owns no input meaning and creates no persistent GameObject. It runs after the
    /// initial scene and after each later scene load, then makes the persistent XRI manager's
    /// InputActionManager the only enabled manager. A scene-generation latch prevents the retained
    /// sceneLoaded subscription and AfterSceneLoad bootstrap from consolidating the same activation twice
    /// when PlayMode runs without a domain reload.
    /// </summary>
    public static class PlayerInputSessionGuard
    {
        private static InputSessionSceneLatchCore _sceneLatch;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSubscription()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _sceneLatch.Reset();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.PlayerInputSessionGuard)) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            ConsolidateSceneOnce(SceneManager.GetActiveScene(), "initial_scene");
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.PlayerInputSessionGuard)) return;
            ConsolidateSceneOnce(scene, "scene_loaded:" + scene.name);
        }

        private static void ConsolidateSceneOnce(Scene scene, string reason)
        {
            int handle = scene.IsValid() ? scene.handle : int.MinValue;
            if (!_sceneLatch.TryEnter(handle))
            {
                Debug.Log("ZIPTIDE: INPUT_SESSION_CONSOLIDATE_SKIPPED scene="
                    + (scene.IsValid() ? scene.name : "invalid") + " handle=" + handle
                    + " reason=" + reason + " cause=already_consolidated");
                return;
            }
            Consolidate(reason);
        }

        /// <summary>
        /// Transfer the union of all action assets to the InputActionManager attached to the sole
        /// active XRInteractionManager, enable that canonical session, and disable every duplicate.
        /// Returns the number of duplicate managers disabled.
        /// </summary>
        public static int Consolidate(string reason)
        {
            XRInteractionManager canonicalXri = FindCanonicalXriManager();
            if (canonicalXri == null)
            {
                Debug.LogWarning("ZIPTIDE: INPUT_SESSION_GUARD no_canonical_xri reason=" + reason);
                return 0;
            }

            InputActionManager primary = canonicalXri.GetComponent<InputActionManager>();
            if (primary == null)
            {
                Debug.LogWarning("ZIPTIDE: INPUT_SESSION_GUARD no_primary_input_manager xri="
                    + canonicalXri.name + " reason=" + reason);
                return 0;
            }

            InputActionManager[] managers = UnityEngine.Object.FindObjectsOfType<InputActionManager>(true);
            var assets = new List<InputActionAsset>();
            AddAssets(primary, assets);
            for (int i = 0; i < managers.Length; i++)
                AddAssets(managers[i], assets);

            var fullyDisabledAssets = new List<InputActionAsset>();
            var intentionallyDisabledActions = new List<InputAction>();
            for (int i = 0; i < assets.Count; i++)
            {
                InputActionAsset asset = assets[i];
                if (asset == null) continue;
                if (!asset.enabled)
                {
                    fullyDisabledAssets.Add(asset);
                    continue;
                }
                foreach (InputActionMap map in asset.actionMaps)
                    foreach (InputAction action in map.actions)
                        if (action != null && !action.enabled)
                            intentionallyDisabledActions.Add(action);
            }

            primary.actionAssets = assets;
            primary.enabled = true;

            // InputActionManager.OnEnable may enable every assigned asset. Restore the exact disabled
            // actions from assets that were already live before manager activation.
            for (int i = 0; i < intentionallyDisabledActions.Count; i++)
            {
                InputAction action = intentionallyDisabledActions[i];
                if (action != null && action.enabled) action.Disable();
            }

            int recoveredAssets = 0;
            for (int i = 0; i < fullyDisabledAssets.Count; i++)
            {
                InputActionAsset asset = fullyDisabledAssets[i];
                if (asset == null) continue;
                if (!asset.enabled) asset.Enable();
                recoveredAssets++;
            }

            int disabled = 0;
            for (int i = 0; i < managers.Length; i++)
            {
                InputActionManager manager = managers[i];
                if (manager == null || manager == primary) continue;

                int moved = manager.actionAssets != null ? manager.actionAssets.Count : 0;
                manager.actionAssets = new List<InputActionAsset>();
                if (manager.enabled)
                {
                    manager.enabled = false;
                    disabled++;
                }
                Debug.Log("ZIPTIDE: INPUT_MANAGER_DISABLED path=" + HierarchyPath(manager.transform)
                    + " movedAssets=" + moved + " reason=" + reason);
            }

            Debug.Log("ZIPTIDE: INPUT_SESSION_CANONICAL manager=" + HierarchyPath(primary.transform)
                + " assets=" + assets.Count + " duplicatesDisabled=" + disabled
                + " recoveredAssets=" + recoveredAssets + " reason=" + reason);
            return disabled;
        }

        private static XRInteractionManager FindCanonicalXriManager()
        {
            XRInteractionManager[] managers =
                UnityEngine.Object.FindObjectsOfType<XRInteractionManager>(true);
            XRInteractionManager active = null;
            for (int i = 0; i < managers.Length; i++)
            {
                XRInteractionManager manager = managers[i];
                if (manager == null) continue;
                if (manager.gameObject.scene.name == "DontDestroyOnLoad") return manager;
                if (active == null && manager.isActiveAndEnabled && manager.gameObject.activeInHierarchy)
                    active = manager;
            }
            return active;
        }

        private static void AddAssets(InputActionManager manager, List<InputActionAsset> assets)
        {
            if (manager == null || manager.actionAssets == null) return;
            foreach (InputActionAsset asset in manager.actionAssets)
                if (asset != null && !assets.Contains(asset)) assets.Add(asset);
        }

        private static string HierarchyPath(Transform value)
        {
            if (value == null) return "none";
            string path = value.name;
            Transform parent = value.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }
    }
}

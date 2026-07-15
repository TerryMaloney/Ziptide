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
    /// InputActionManager the only enabled manager.
    /// </summary>
    public static class PlayerInputSessionGuard
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSubscription()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.PlayerInputSessionGuard)) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Consolidate("initial_scene");
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.PlayerInputSessionGuard)) return;
            Consolidate("scene_loaded:" + scene.name);
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

            primary.actionAssets = assets;
            primary.enabled = true;
            for (int i = 0; i < assets.Count; i++)
                if (assets[i] != null) assets[i].Enable();

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
                + " reason=" + reason);
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

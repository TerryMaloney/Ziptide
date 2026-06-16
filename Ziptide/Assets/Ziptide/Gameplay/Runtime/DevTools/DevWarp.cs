using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Gameplay.DevTools
{
    /// <summary>
    /// Developer-only world warp. NOT a player feature — gated to the editor / development builds so
    /// it can never be triggered in a shipping build. Lets us jump straight to any world (and a named
    /// spawn marker within it) while building, instead of walking through travel doors.
    ///
    /// Routes through <see cref="TravelCoordinator"/> so the normal load + inventory-restore path is
    /// still exercised, then repositions the rig to the requested marker once travel settles. A tiny
    /// DontDestroyOnLoad runner handles the timing (it waits out any in-progress travel, including the
    /// boot scene's initial load, before warping).
    /// </summary>
    public static class DevWarp
    {
        /// <summary>True only in the editor or a development build — warps are no-ops when shipped.</summary>
        public static bool Enabled
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>Warp to a world scene, optionally landing at a named spawn marker within it.</summary>
        public static void WarpToScene(string sceneName, string markerId = null)
        {
            if (!Enabled)
            {
                Debug.LogWarning("ZIPTIDE: DEV_WARP ignored (not a dev build) scene=" + sceneName);
                return;
            }
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("ZIPTIDE: DEV_WARP no scene name");
                return;
            }

            Debug.Log("ZIPTIDE: DEV_WARP scene=" + sceneName + " marker=" + (markerId ?? "(default)"));
            DevWarpRunner.Begin(sceneName, markerId);
        }

        /// <summary>Reposition the rig to a named marker in the CURRENT scene (no scene load).</summary>
        public static void WarpToMarker(string markerId)
        {
            if (!Enabled) return;
            var rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (rig == null)
            {
                Debug.LogWarning("ZIPTIDE: DEV_WARP no PlayerRigPersistence in scene");
                return;
            }
            rig.TeleportToMarker(markerId);
        }
    }

    /// <summary>
    /// Internal coroutine host for <see cref="DevWarp"/>. Survives the scene load it triggers, waits
    /// for any in-progress travel (e.g. the boot scene's first load) to finish before and after
    /// warping, then repositions to the named marker and self-destructs.
    /// </summary>
    internal sealed class DevWarpRunner : MonoBehaviour
    {
        private const float TimeoutSeconds = 12f;

        public static void Begin(string sceneName, string markerId)
        {
            var go = new GameObject("__DevWarpRunner");
            Object.DontDestroyOnLoad(go);
            var runner = go.AddComponent<DevWarpRunner>();
            runner.StartCoroutine(runner.Run(sceneName, markerId));
        }

        private IEnumerator Run(string sceneName, string markerId)
        {
            // Wait out any travel already in flight (the boot scene kicks one off on Start).
            float t = 0f;
            while (TravelCoordinator.IsTravelling && t < TimeoutSeconds) { t += Time.unscaledDeltaTime; yield return null; }

            TravelCoordinator.TravelTo(sceneName);
            yield return null; // let the travel coroutine flip IsTravelling on

            t = 0f;
            while (TravelCoordinator.IsTravelling && t < TimeoutSeconds) { t += Time.unscaledDeltaTime; yield return null; }

            if (!string.IsNullOrEmpty(markerId))
            {
                var rig = FindObjectOfType<PlayerRigPersistence>();
                if (rig != null) rig.TeleportToMarker(markerId);
            }

            Debug.Log("ZIPTIDE: DEV_WARP_DONE scene=" + sceneName + " marker=" + (markerId ?? "(default)"));
            Destroy(gameObject);
        }
    }
}

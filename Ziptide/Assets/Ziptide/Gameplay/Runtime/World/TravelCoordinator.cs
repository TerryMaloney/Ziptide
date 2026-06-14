using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Single entry-point for all scene travel. DontDestroyOnLoad singleton.
    ///
    /// Flow:
    ///   1. SaveBeforeTravel  — snapshot + destroy current scene items
    ///   2. LoadScene         — Unity async scene load
    ///   3. WaitForXRI        — spin up to 5s until XRInteractionManager, ray interactors,
    ///                          and InputActionManager are all ready
    ///   4. RestoreAfterTravel — coroutine from InventoryState (real socket SelectEnter)
    ///   5. Log TRAVEL_OK / TRAVEL_FAIL
    /// </summary>
    public class TravelCoordinator : MonoBehaviour
    {
        private static TravelCoordinator _instance;

        private bool _travelling;

        /// <summary>True while TravelCoroutine is running. PlayerRigPersistence skips wiring/restore in OnSceneLoaded when true.</summary>
        public static bool IsTravelling => _instance != null && _instance._travelling;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("ZIPTIDE: DUP_SINGLETON TravelCoordinator – destroying extra");
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ZIPTIDE: TravelCoordinator AWAKE");
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        /// <summary>
        /// Primary travel API. Falls back to direct SceneManager.LoadScene if no coordinator.
        /// </summary>
        public static void TravelTo(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;

            if (_instance == null)
            {
                Debug.LogWarning("ZIPTIDE: TravelCoordinator not found – falling back to direct load");
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                if (rig != null) rig.PrepareForSceneTravel();
                SceneManager.LoadScene(sceneName);
                return;
            }

            _instance.StartTravelCoroutine(sceneName);
        }

        private void StartTravelCoroutine(string sceneName)
        {
            if (_travelling)
            {
                Debug.LogWarning("ZIPTIDE: TravelCoordinator already travelling – ignoring duplicate request");
                return;
            }
            StartCoroutine(TravelCoroutine(sceneName));
        }

        private IEnumerator TravelCoroutine(string sceneName)
        {
            _travelling = true;
            Debug.Log("ZIPTIDE: TRAVEL_START dest=" + sceneName);

            // 1. Save inventory before anything is destroyed by scene unload.
            var rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (rig != null)
                rig.PrepareForSceneTravel();
            else
                InventoryState.SaveBeforeTravel();

            // 2. Load destination scene.
            SceneManager.LoadScene(sceneName);

            // 3. Wait one frame for the new scene to initialise.
            yield return null;

            // 4. Single owner of post-load: teleport, wire XRI, then wait for ready, then restore.
            var playerRig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (playerRig == null)
            {
                Debug.LogWarning("ZIPTIDE: TRAVEL_FAIL dest=" + sceneName + " reason=no_PlayerRigPersistence_after_load");
                _travelling = false;
                yield break;
            }

            playerRig.TeleportToSpawnMarker();
            playerRig.EnsureXRIWiring();

            // 5. Wait for XRI to be ready (up to 5 seconds).
            float elapsed = 0f;
            const float timeout = 5f;
            bool xriReady = false;

            while (elapsed < timeout)
            {
                if (IsXRIReady())
                {
                    xriReady = true;
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!xriReady)
            {
                Debug.LogWarning("ZIPTIDE: XRI_NOT_READY after " + timeout + "s – proceeding anyway");
                Debug.LogWarning("ZIPTIDE: TRAVEL_FAIL dest=" + sceneName + " reason=XRI_NOT_READY");
            }
            else
            {
                Debug.Log("ZIPTIDE: XRI_READY elapsed=" + elapsed.ToString("F2") + "s");
            }

            // 6. Restore inventory (coroutine with frame delays for socket registration).
            yield return InventoryState.RestoreAfterTravel(playerRig.transform);

            // Runtime audit: exactly one manager after travel.
            var managers = Object.FindObjectsOfType<XRInteractionManager>();
            if (managers != null && managers.Length != 1)
                Debug.LogWarning("ZIPTIDE: AUDIT_FAIL multiple_managers_after_travel count=" + (managers?.Length ?? 0));

            if (xriReady)
                Debug.Log("ZIPTIDE: TRAVEL_OK dest=" + sceneName);

            _travelling = false;
        }

        // ── XRI readiness criteria ──────────────────────────────────────────

        private static bool IsXRIReady()
        {
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr == null) return false;

            var rays = Object.FindObjectsOfType<XRRayInteractor>();
            int enabledRays = 0;
            foreach (var ray in rays)
                if (ray.isActiveAndEnabled) enabledRays++;
            if (enabledRays < 2) return false;

            var iam = Object.FindObjectOfType<InputActionManager>();
            if (iam == null) return false;

            return true;
        }
    }
}

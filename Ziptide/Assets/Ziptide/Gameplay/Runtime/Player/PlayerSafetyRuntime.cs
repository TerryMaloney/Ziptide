using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Safety rail around the existing PlayerRigPersistence owner. It does not choose destinations,
    /// move the player during ordinary play, or own locomotion. After a scene spawn settles it validates
    /// the tracked eye height against the actual floor and corrects only impossible values. This turns
    /// the device-only "twelve feet above the floor" failure into a bounded, logged recovery instead of
    /// allowing it to contaminate every world and vehicle.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerRigPersistence))]
    public sealed class PlayerSafetyRuntime : MonoBehaviour
    {
        public const float MinimumPlausibleEyeHeight = 0.45f;
        public const float MaximumPlausibleEyeHeight = 2.20f;
        public const float RecoveryEyeHeight = 1.55f;

        private static PlayerSafetyRuntime _instance;
        private Coroutine _validation;
        private PlayerRigPersistence _rig;

        private void Awake()
        {
            _instance = this;
            _rig = GetComponent<PlayerRigPersistence>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            QueueValidation("enable");
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_validation != null) StopCoroutine(_validation);
            _validation = null;
            if (_instance == this) _instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            QueueValidation("scene_loaded:" + scene.name);
        }

        public static void RecoverNow(string reason)
        {
            if (_instance == null)
            {
                Debug.LogWarning("ZIPTIDE: PLAYER_RECOVERY_BLOCKED reason=no_safety_runtime request=" + reason);
                return;
            }
            _instance.StartRecovery(reason);
        }

        private void StartRecovery(string reason)
        {
            if (_validation != null) StopCoroutine(_validation);
            _validation = StartCoroutine(RecoverAndValidate(reason));
        }

        private void QueueValidation(string reason)
        {
            if (!isActiveAndEnabled) return;
            if (_validation != null) StopCoroutine(_validation);
            _validation = StartCoroutine(ValidateAfterSettle(reason));
        }

        private IEnumerator RecoverAndValidate(string reason)
        {
            if (_rig == null) _rig = GetComponent<PlayerRigPersistence>();
            if (_rig != null) _rig.TeleportToSpawnMarker();
            Debug.Log("ZIPTIDE: PLAYER_RECOVERY requested=" + reason);
            yield return ValidateAfterSettle("recovery:" + reason);
        }

        private IEnumerator ValidateAfterSettle(string reason)
        {
            if (SceneManager.GetActiveScene().name == Ziptide.Core.ZiptideConstants.SceneBoot)
            {
                _validation = null;
                yield break;
            }

            float deadline = Time.realtimeSinceStartup + 45f;
            while (TravelCoordinator.IsTravelling && Time.realtimeSinceStartup < deadline)
                yield return null;

            // Give tracking, XRI and CharacterControllerDriver several frames to settle after travel.
            for (int i = 0; i < 6; i++) yield return null;

            Camera cam = GetComponentInChildren<Camera>(true);
            SpawnMarkerRuntime marker = FindPlayerMarker();
            if (cam == null || marker == null)
            {
                Debug.LogWarning("ZIPTIDE: PLAYER_HEIGHT_CHECK skipped reason="
                    + (cam == null ? "no_camera" : "no_spawn") + " request=" + reason);
                _validation = null;
                yield break;
            }

            if (!TryGroundAt(marker.transform.position, out float groundY, out string groundName))
            {
                Debug.LogError("ZIPTIDE: PLAYER_HEIGHT_BLOCKER reason=no_ground marker="
                    + marker.transform.position.ToString("F2") + " request=" + reason);
                _validation = null;
                yield break;
            }

            float eyeHeight = cam.transform.position.y - groundY;
            Debug.Log("ZIPTIDE: PLAYER_HEIGHT_CHECK eye=" + eyeHeight.ToString("F2")
                + " ground=" + groundY.ToString("F2") + " collider=" + groundName
                + " request=" + reason);

            if (eyeHeight < MinimumPlausibleEyeHeight || eyeHeight > MaximumPlausibleEyeHeight)
            {
                CharacterController cc = GetComponent<CharacterController>();
                bool wasEnabled = cc != null && cc.enabled;
                if (wasEnabled) cc.enabled = false;

                Vector3 p = transform.position;
                p.y += RecoveryEyeHeight - eyeHeight;
                transform.position = p;

                if (wasEnabled) cc.enabled = true;
                Debug.LogError("ZIPTIDE: PLAYER_HEIGHT_REPAIRED from=" + eyeHeight.ToString("F2")
                    + " to=" + RecoveryEyeHeight.ToString("F2") + " rig=" + transform.position.ToString("F2")
                    + " request=" + reason);
            }

            _validation = null;
        }

        private static SpawnMarkerRuntime FindPlayerMarker()
        {
            SpawnMarkerRuntime[] markers = Object.FindObjectsOfType<SpawnMarkerRuntime>();
            if (markers == null || markers.Length == 0) return null;
            for (int i = 0; i < markers.Length; i++)
                if (markers[i] != null && markers[i].markerId == "player") return markers[i];
            return markers[0];
        }

        private static bool TryGroundAt(Vector3 marker, out float groundY, out string groundName)
        {
            RaycastHit[] hits = Physics.RaycastAll(marker + Vector3.up * 3f, Vector3.down, 15f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col == null) continue;
                if (col.GetComponentInParent<PlayerRigPersistence>() != null) continue;
                groundY = hits[i].point.y;
                groundName = col.name;
                return true;
            }
            groundY = marker.y;
            groundName = "NONE";
            return false;
        }
    }
}

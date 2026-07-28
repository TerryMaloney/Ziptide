using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Runtime turn-mode authority. Smooth and snap providers may exist for settings, but only one may
    /// be enabled at a time. The default Quest route is continuous turn at a restrained speed.
    /// </summary>
    public static class TurnModeCore
    {
        public const float DefaultSmoothTurnSpeed = 60f;

        public static void EnforceSmoothOnly(Transform rigRoot, string reason)
        {
            if (rigRoot == null) return;
            ActionBasedContinuousTurnProvider[] smooth =
                rigRoot.GetComponentsInChildren<ActionBasedContinuousTurnProvider>(true);
            ActionBasedSnapTurnProvider[] snap =
                rigRoot.GetComponentsInChildren<ActionBasedSnapTurnProvider>(true);

            int smoothEnabled = 0;
            for (int i = 0; i < smooth.Length; i++)
            {
                if (smooth[i] == null) continue;
                smooth[i].turnSpeed = DefaultSmoothTurnSpeed;
                smooth[i].enabled = true;
                smoothEnabled++;
            }
            for (int i = 0; i < snap.Length; i++)
                if (snap[i] != null) snap[i].enabled = false;

            Debug.Log("ZIPTIDE: TURN_MODE smooth=" + smoothEnabled + " snap=0 speed="
                + DefaultSmoothTurnSpeed.ToString("0") + " reason=" + reason);
        }
    }

    /// <summary>
    /// Safety rail around the existing PlayerRigPersistence owner. It does not choose destinations or
    /// own ordinary locomotion. It forces the XR runtime to Floor origin, removes synthetic camera Y,
    /// validates tracked eye height against the real walkable floor, and repairs only impossible values.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerRigPersistence))]
    public sealed class PlayerSafetyRuntime : MonoBehaviour
    {
        public const float MinimumPlausibleEyeHeight = 0.30f;
        public const float MaximumPlausibleEyeHeight = 2.20f;
        public const float RecoveryEyeHeight = 1.50f;

        private static PlayerSafetyRuntime _instance;
        private Coroutine _validation;
        private PlayerRigPersistence _rig;

        public static PlayerSafetyRuntime EnsureOnRig(GameObject rig)
        {
            if (rig == null) return null;
            PlayerSafetyRuntime safety = rig.GetComponent<PlayerSafetyRuntime>();
            if (safety == null)
            {
                safety = rig.AddComponent<PlayerSafetyRuntime>();
                Debug.Log("ZIPTIDE: PLAYER_SAFETY_ENSURED runtime=true rig=" + rig.name);
            }
            return safety;
        }

        private void Awake()
        {
            _instance = this;
            _rig = GetComponent<PlayerRigPersistence>();
            ApplyRuntimeOriginContract("awake");
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
            ApplyRuntimeOriginContract("scene_loaded:" + scene.name);
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

            ApplyRuntimeOriginContract("settle:" + reason);

            // Wait for OpenXR to provide a real tracked head pose. The previous six-frame check ran
            // before tracking settled and therefore never emitted a PLAYER_HEIGHT_CHECK on the device.
            Camera cam = GetComponentInChildren<Camera>(true);
            float trackingDeadline = Time.realtimeSinceStartup + 2.5f;
            while (cam != null && Mathf.Abs(cam.transform.localPosition.y) < 0.05f
                && Time.realtimeSinceStartup < trackingDeadline)
                yield return null;
            for (int i = 0; i < 12; i++) yield return null;

            TurnModeCore.EnforceSmoothOnly(transform, "height_settle:" + reason);

            cam = GetComponentInChildren<Camera>(true);
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
            Debug.Log("ZIPTIDE: PLAYER_HEIGHT_RUNTIME eye=" + eyeHeight.ToString("F2")
                + " ground=" + groundY.ToString("F2") + " collider=" + groundName
                + " rigY=" + transform.position.y.ToString("F2")
                + " cameraLocal=" + cam.transform.localPosition.ToString("F2")
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

        private void ApplyRuntimeOriginContract(string reason)
        {
            int running = 0;
            int floorAccepted = 0;
            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            for (int i = 0; i < subsystems.Count; i++)
            {
                XRInputSubsystem subsystem = subsystems[i];
                if (subsystem == null || !subsystem.running) continue;
                running++;
                TrackingOriginModeFlags supported = subsystem.GetSupportedTrackingOriginModes();
                if ((supported & TrackingOriginModeFlags.Floor) != 0
                    && subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor))
                    floorAccepted++;
            }

            Component origin = FindXrOrigin();
            string requested = "NONE";
            float offsetY = float.NaN;
            if (origin != null)
            {
                Type type = origin.GetType();
                PropertyInfo modeProperty = type.GetProperty("RequestedTrackingOriginMode");
                if (modeProperty != null && modeProperty.CanWrite)
                {
                    object floorValue = Enum.ToObject(modeProperty.PropertyType, 2);
                    modeProperty.SetValue(origin, floorValue, null);
                    requested = floorValue.ToString();
                }

                PropertyInfo yProperty = type.GetProperty("CameraYOffset");
                if (yProperty != null && yProperty.CanWrite)
                {
                    yProperty.SetValue(origin, 0f, null);
                    offsetY = 0f;
                }

                PropertyInfo floorObjectProperty = type.GetProperty("CameraFloorOffsetObject");
                Transform floorObject = floorObjectProperty != null
                    ? floorObjectProperty.GetValue(origin, null) as Transform : null;
                if (floorObject == null) floorObject = FindCameraOffset(origin.transform);
                if (floorObject != null)
                {
                    Vector3 local = floorObject.localPosition;
                    local.y = 0f;
                    floorObject.localPosition = local;
                    offsetY = floorObject.localPosition.y;
                }
            }

            Debug.Log("ZIPTIDE: RIG_HEIGHT_RUNTIME origin=" + requested + " offsetY="
                + (float.IsNaN(offsetY) ? "UNKNOWN" : offsetY.ToString("F3"))
                + " xrRunning=" + running + " floorAccepted=" + floorAccepted + " reason=" + reason);
        }

        private Component FindXrOrigin()
        {
            Type xrOriginType = Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            return xrOriginType != null ? GetComponent(xrOriginType) as Component : null;
        }

        private static Transform FindCameraOffset(Transform root)
        {
            if (root == null) return null;
            Transform direct = root.Find("Camera Offset");
            if (direct != null) return direct;
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
                if (all[i] != null && all[i].name == "Camera Offset") return all[i];
            return null;
        }

        private static SpawnMarkerRuntime FindPlayerMarker()
        {
            SpawnMarkerRuntime[] markers = UnityEngine.Object.FindObjectsOfType<SpawnMarkerRuntime>();
            if (markers == null || markers.Length == 0) return null;
            for (int i = 0; i < markers.Length; i++)
                if (markers[i] != null && markers[i].markerId == "player") return markers[i];
            return markers[0];
        }

        private static bool TryGroundAt(Vector3 marker, out float groundY, out string groundName)
        {
            RaycastHit[] hits = Physics.RaycastAll(marker + Vector3.up * 2.5f, Vector3.down, 15f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col == null) continue;
                if (col.GetComponentInParent<PlayerRigPersistence>() != null) continue;
                if (col.GetComponentInParent<ObjectiveBeacon>() != null) continue;
                if (col.name.IndexOf("SkySphere", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (Vector3.Dot(hits[i].normal, Vector3.up) < 0.55f) continue;
                if (hits[i].point.y > marker.y + 0.5f) continue;
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

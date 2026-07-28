using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Small device-only correction owner for the two headset failures that survived the recovery pass:
    /// stacked XR camera height and the Breaker Blade being aligned like a backwards stabbing weapon.
    /// Installed by the persistent BeltRig so it exists once across every world.
    /// </summary>
    [DefaultExecutionOrder(10000)]
    [DisallowMultipleComponent]
    public sealed class QuestDeviceCorrectionsRuntime : MonoBehaviour
    {
        // XRI aligns the item's attach transform to the controller attach transform. The controller's
        // +Z currently points back toward the player on device. 100 degrees makes the blade nearly
        // vertical with a small forward/away lean instead of an identity-rotation backwards jab.
        public static readonly Vector3 BreakerBladeGripEuler = new Vector3(100f, 0f, 0f);

        public const float MaximumAllowedEyeHeight = 1.85f;
        public const float CorrectedEyeHeight = 1.65f;
        public const float MinimumAllowedEyeHeight = 0.45f;

        private readonly HashSet<int> _correctedBlades = new HashSet<int>();
        private Coroutine _heightPass;
        private float _nextBladeScan;

        public static QuestDeviceCorrectionsRuntime EnsureOnRig(GameObject rig)
        {
            if (rig == null) return null;
            QuestDeviceCorrectionsRuntime correction = rig.GetComponent<QuestDeviceCorrectionsRuntime>();
            if (correction == null)
            {
                correction = rig.AddComponent<QuestDeviceCorrectionsRuntime>();
                Debug.Log("ZIPTIDE: QUEST_DEVICE_CORRECTIONS_ENSURED rig=" + rig.name);
            }
            return correction;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            QueueHeightPass("enable");
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_heightPass != null) StopCoroutine(_heightPass);
            _heightPass = null;
        }

        private void Update()
        {
            if (Time.unscaledTime < _nextBladeScan) return;
            _nextBladeScan = Time.unscaledTime + 0.25f;
            ApplyBreakerBladeGrip();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            QueueHeightPass("scene:" + scene.name);
        }

        private void QueueHeightPass(string reason)
        {
            if (!isActiveAndEnabled) return;
            if (_heightPass != null) StopCoroutine(_heightPass);
            _heightPass = StartCoroutine(CorrectHeightAfterTracking(reason));
        }

        private IEnumerator CorrectHeightAfterTracking(string reason)
        {
            if (SceneManager.GetActiveScene().name == ZiptideConstants.SceneBoot)
            {
                _heightPass = null;
                yield break;
            }

            float travelDeadline = Time.realtimeSinceStartup + 45f;
            while (TravelCoordinator.IsTravelling && Time.realtimeSinceStartup < travelDeadline)
                yield return null;

            float deadline = Time.realtimeSinceStartup + 3f;
            while (Time.realtimeSinceStartup < deadline)
            {
                ApplyFloorOriginContract();
                if (TryCorrectEyeHeight(reason))
                {
                    // Re-assert for several frames because XROrigin can apply its serialized offset after
                    // OpenXR reports the first tracked pose.
                    for (int i = 0; i < 12; i++)
                    {
                        yield return null;
                        ApplyFloorOriginContract();
                    }
                    break;
                }
                yield return null;
            }

            _heightPass = null;
        }

        private void ApplyBreakerBladeGrip()
        {
            MeleeWeaponRuntime[] weapons = UnityEngine.Object.FindObjectsOfType<MeleeWeaponRuntime>(true);
            for (int i = 0; i < weapons.Length; i++)
            {
                MeleeWeaponRuntime weapon = weapons[i];
                if (weapon == null) continue;

                ItemRuntime item = weapon.GetComponent<ItemRuntime>();
                ArenaWeaponDefinition definition = item != null
                    ? item.Definition as ArenaWeaponDefinition : null;
                if (definition == null || definition.kind != ArenaWeaponKind.BreakerBlade) continue;

                XRGrabInteractable grab = weapon.GetComponent<XRGrabInteractable>();
                if (grab == null) continue;

                Transform grip = weapon.transform.Find("Grip");
                if (grip == null) grip = weapon.transform.Find("HandGripAttach");
                if (grip == null) continue;

                Quaternion target = Quaternion.Euler(BreakerBladeGripEuler);
                int id = weapon.GetInstanceID();
                if (_correctedBlades.Contains(id)
                    && grab.attachTransform == grip
                    && Quaternion.Angle(grip.localRotation, target) < 0.1f)
                    continue;

                grip.localRotation = target;
                grab.attachTransform = grip;
                _correctedBlades.Add(id);
                Debug.Log("ZIPTIDE: BREAKER_BLADE_GRIP_FIXED euler="
                    + BreakerBladeGripEuler.ToString("F0") + " item=" + weapon.name);
            }
        }

        private void ApplyFloorOriginContract()
        {
            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subsystems);
            for (int i = 0; i < subsystems.Count; i++)
            {
                XRInputSubsystem subsystem = subsystems[i];
                if (subsystem == null || !subsystem.running) continue;
                TrackingOriginModeFlags supported = subsystem.GetSupportedTrackingOriginModes();
                if ((supported & TrackingOriginModeFlags.Floor) != 0)
                    subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
            }

            Type xrOriginType = Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            Component origin = xrOriginType != null ? GetComponent(xrOriginType) as Component : null;
            if (origin != null)
            {
                Type type = origin.GetType();
                PropertyInfo mode = type.GetProperty("RequestedTrackingOriginMode");
                if (mode != null && mode.CanWrite)
                    mode.SetValue(origin, Enum.ToObject(mode.PropertyType, 2), null);

                PropertyInfo yOffset = type.GetProperty("CameraYOffset");
                if (yOffset != null && yOffset.CanWrite)
                    yOffset.SetValue(origin, 0f, null);

                PropertyInfo floorObjectProperty = type.GetProperty("CameraFloorOffsetObject");
                object floorObjectValue = floorObjectProperty != null
                    ? floorObjectProperty.GetValue(origin, null) : null;
                Transform floorObject = floorObjectValue as Transform;
                if (floorObject == null && floorObjectValue is GameObject floorObjectGo)
                    floorObject = floorObjectGo.transform;
                ZeroYOffset(floorObject);
            }

            Camera camera = GetComponentInChildren<Camera>(true);
            Transform parent = camera != null ? camera.transform.parent : null;
            while (parent != null && parent != transform)
            {
                string lower = parent.name.ToLowerInvariant();
                if (lower.Contains("camera offset") || lower.Contains("floor offset"))
                    ZeroYOffset(parent);
                parent = parent.parent;
            }
        }

        private static void ZeroYOffset(Transform target)
        {
            if (target == null || Mathf.Abs(target.localPosition.y) < 0.0001f) return;
            Vector3 local = target.localPosition;
            local.y = 0f;
            target.localPosition = local;
        }

        private bool TryCorrectEyeHeight(string reason)
        {
            Camera camera = GetComponentInChildren<Camera>(true);
            if (camera == null || Mathf.Abs(camera.transform.localPosition.y) < 0.20f) return false;

            SpawnMarkerRuntime marker = FindPlayerMarker();
            Vector3 probe = marker != null ? marker.transform.position : transform.position;
            if (!TryGroundAt(probe, out float groundY, out string groundName)) return false;

            float eyeHeight = camera.transform.position.y - groundY;
            if (eyeHeight >= MinimumAllowedEyeHeight && eyeHeight <= MaximumAllowedEyeHeight)
            {
                Debug.Log("ZIPTIDE: QUEST_HEIGHT_OK eye=" + eyeHeight.ToString("F2")
                    + " ground=" + groundName + " reason=" + reason);
                return true;
            }

            CharacterController controller = GetComponent<CharacterController>();
            bool controllerWasEnabled = controller != null && controller.enabled;
            if (controllerWasEnabled) controller.enabled = false;

            Vector3 position = transform.position;
            position.y += CorrectedEyeHeight - eyeHeight;
            transform.position = position;

            if (controllerWasEnabled) controller.enabled = true;
            Debug.LogWarning("ZIPTIDE: QUEST_HEIGHT_FIXED from=" + eyeHeight.ToString("F2")
                + " to=" + CorrectedEyeHeight.ToString("F2") + " rigY="
                + transform.position.y.ToString("F2") + " reason=" + reason);
            return true;
        }

        private static SpawnMarkerRuntime FindPlayerMarker()
        {
            SpawnMarkerRuntime[] markers = UnityEngine.Object.FindObjectsOfType<SpawnMarkerRuntime>();
            for (int i = 0; i < markers.Length; i++)
                if (markers[i] != null && markers[i].markerId == "player") return markers[i];
            return markers.Length > 0 ? markers[0] : null;
        }

        private static bool TryGroundAt(Vector3 probe, out float groundY, out string groundName)
        {
            RaycastHit[] hits = Physics.RaycastAll(probe + Vector3.up * 3f, Vector3.down, 15f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null) continue;
                if (collider.GetComponentInParent<PlayerRigPersistence>() != null) continue;
                if (collider.GetComponentInParent<ObjectiveBeacon>() != null) continue;
                if (collider.name.IndexOf("SkySphere", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (Vector3.Dot(hits[i].normal, Vector3.up) < 0.55f) continue;
                if (hits[i].point.y > probe.y + 0.5f) continue;

                groundY = hits[i].point.y;
                groundName = collider.name;
                return true;
            }

            groundY = probe.y;
            groundName = "NONE";
            return false;
        }
    }
}

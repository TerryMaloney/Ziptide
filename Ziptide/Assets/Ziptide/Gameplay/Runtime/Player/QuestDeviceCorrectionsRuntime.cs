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
    /// Quest-only correction owner for two device truths that cannot be inferred from EditMode:
    /// the live controller attach basis and the live floor-to-eye measurement.
    /// Installed once on the persistent rig by BeltRig.
    /// </summary>
    [DefaultExecutionOrder(10000)]
    [DisallowMultipleComponent]
    public sealed class QuestDeviceCorrectionsRuntime : MonoBehaviour
    {
        // Cap rather than force one adult height. Children/crouched players stay unchanged; only the
        // proven "too tall" state is lowered. 1.48 m keeps all first-hour interactions reachable.
        public const float MaximumAllowedEyeHeight = 1.55f;
        public const float CorrectedEyeHeight = 1.48f;

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
            _nextBladeScan = Time.unscaledTime + 0.20f;
            EnsureBreakerBladeCalibrators();
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

            // OpenXR and XROrigin can both re-apply their tracking-origin state for several frames.
            // Reassert and remeasure for six seconds so the correction survives the complete arrival.
            float deadline = Time.realtimeSinceStartup + 6f;
            float nextProbe = 0f;
            while (Time.realtimeSinceStartup < deadline)
            {
                ApplyFloorOriginContract();
                if (Time.realtimeSinceStartup >= nextProbe)
                {
                    nextProbe = Time.realtimeSinceStartup + 0.10f;
                    TryLowerTallEyeHeight(reason);
                }
                yield return null;
            }

            // One final measurement after all arrival systems have settled.
            ApplyFloorOriginContract();
            TryLowerTallEyeHeight(reason + ":final");
            _heightPass = null;
        }

        private void EnsureBreakerBladeCalibrators()
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

                if (weapon.GetComponent<BreakerBladeHandPoseRuntime>() == null)
                    weapon.gameObject.AddComponent<BreakerBladeHandPoseRuntime>();
            }
        }

        private void ApplyFloorOriginContract()
        {
            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
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

        private bool TryLowerTallEyeHeight(string reason)
        {
            Camera camera = GetComponentInChildren<Camera>(true);
            if (camera == null) return false;

            if (!TryGroundDirectlyBelow(camera.transform.position, out float groundY, out string groundName))
            {
                SpawnMarkerRuntime marker = FindPlayerMarker();
                if (marker == null || !TryGroundDirectlyBelow(marker.transform.position + Vector3.up * 3f,
                        out groundY, out groundName))
                {
                    Debug.LogWarning("ZIPTIDE: QUEST_HEIGHT_NO_GROUND reason=" + reason);
                    return false;
                }
            }

            float eyeHeight = camera.transform.position.y - groundY;
            if (eyeHeight <= MaximumAllowedEyeHeight)
            {
                Debug.Log("ZIPTIDE: QUEST_HEIGHT_OK eye=" + eyeHeight.ToString("F2")
                    + " ground=" + groundName + " reason=" + reason);
                return true;
            }

            CharacterController controller = GetComponent<CharacterController>();
            bool controllerWasEnabled = controller != null && controller.enabled;
            if (controllerWasEnabled) controller.enabled = false;

            Vector3 position = transform.position;
            position.y -= eyeHeight - CorrectedEyeHeight;
            transform.position = position;

            if (controllerWasEnabled) controller.enabled = true;

            float corrected = camera.transform.position.y - groundY;
            Debug.LogWarning("ZIPTIDE: QUEST_HEIGHT_FIXED from=" + eyeHeight.ToString("F2")
                + " to=" + corrected.ToString("F2") + " target=" + CorrectedEyeHeight.ToString("F2")
                + " ground=" + groundName + " rigY=" + transform.position.y.ToString("F2")
                + " reason=" + reason);
            return true;
        }

        private static SpawnMarkerRuntime FindPlayerMarker()
        {
            SpawnMarkerRuntime[] markers = UnityEngine.Object.FindObjectsOfType<SpawnMarkerRuntime>();
            for (int i = 0; i < markers.Length; i++)
                if (markers[i] != null && markers[i].markerId == "player") return markers[i];
            return markers.Length > 0 ? markers[0] : null;
        }

        private static bool TryGroundDirectlyBelow(Vector3 start, out float groundY, out string groundName)
        {
            RaycastHit[] hits = Physics.RaycastAll(start + Vector3.up * 0.25f, Vector3.down, 50f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null) continue;
                if (collider.GetComponentInParent<PlayerRigPersistence>() != null) continue;
                if (collider.GetComponentInParent<ObjectiveBeacon>() != null) continue;
                if (collider.name.IndexOf("SkySphere", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (Vector3.Dot(hits[i].normal, Vector3.up) < 0.70f) continue;

                groundY = hits[i].point.y;
                groundName = collider.name;
                return true;
            }

            groundY = start.y;
            groundName = "NONE";
            return false;
        }
    }

    /// <summary>
    /// Calibrates the Breaker Blade from the ACTUAL live hand attach transform at grab time. This avoids
    /// all controller-axis and imported-model Euler guesses. On grab, the blade points mostly upward and
    /// slightly forward/away from the player's face, then follows the wrist normally.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BreakerBladeHandPoseRuntime : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private Transform _tip;

        private void Awake()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _tip = transform.Find("Muzzle");
            if (_tip == null) _tip = transform;
        }

        private void OnEnable()
        {
            if (_grab != null) _grab.selectEntered.AddListener(OnSelectEntered);
        }

        private void OnDisable()
        {
            if (_grab != null) _grab.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (_grab == null || !(args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor)) return;

            Transform handAttach = args.interactorObject.GetAttachTransform(_grab);
            if (handAttach == null) return;

            Transform grip = _grab.attachTransform;
            if (grip == null || !grip.IsChildOf(transform))
            {
                grip = transform.Find("Grip");
                if (grip == null) grip = transform.Find("HandGripAttach");
            }
            if (grip == null) return;

            Camera camera = Camera.main;
            if (camera == null) camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
            if (camera == null) return;

            Vector3 viewForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
            if (viewForward.sqrMagnitude < 0.001f) viewForward = Vector3.forward;
            viewForward.Normalize();

            // THRUST CARRY (device 2026-07-28, Terry): the tip LEADS, roughly 20 degrees above
            // horizontal — a knife held to stab, not a sword raised to salute. The previous value
            // was up*0.93 + forward*0.37, i.e. ~68 degrees above horizontal, which reads as the
            // blade sticking up out of the fist. This component runs AFTER ItemFactory's grip pose
            // and overrides it, which is why tuning gripEuler in the factory never changed anything.
            // Tuning knob: raise the up term to lift the tip, raise the forward term to level it.
            // It still cannot point back at the player because the forward component IS the
            // camera's outward view direction.
            Vector3 desiredBladeAxis = (viewForward * 0.94f + Vector3.up * 0.34f).normalized;
            Vector3 desiredBladeWidth = Vector3.ProjectOnPlane(camera.transform.right, desiredBladeAxis);
            if (desiredBladeWidth.sqrMagnitude < 0.001f) desiredBladeWidth = camera.transform.right;
            desiredBladeWidth.Normalize();

            Vector3 localGripPosition = transform.InverseTransformPoint(grip.position);
            Vector3 localBladeAxis = WeaponPoseCore.ResolveAxisLocal(transform, _tip, localGripPosition);
            Vector3 localBladeWidth = WeaponPoseCore.ResolveUpHintLocal(localBladeAxis);
            Quaternion desiredRootRotation = WeaponPoseCore.MapLocalBasisToWorld(
                localBladeAxis, localBladeWidth, desiredBladeAxis, desiredBladeWidth);

            // XRI solves itemRoot = handAttach * inverse(gripLocal). Solve the grip local rotation
            // from the desired root rotation and the real hand rotation rather than guessing Euler axes.
            grip.localRotation = Quaternion.Inverse(desiredRootRotation) * handAttach.rotation;
            _grab.attachTransform = grip;

            // Apply immediately; XRI will preserve the same relationship on subsequent frames.
            transform.rotation = desiredRootRotation;
            transform.position += handAttach.position - grip.position;

            Vector3 actualAxis = (_tip.position - grip.position).normalized;
            Debug.Log("ZIPTIDE: BREAKER_BLADE_LIVE_CALIBRATED desired="
                + desiredBladeAxis.ToString("F2") + " actual=" + actualAxis.ToString("F2")
                + " awayDot=" + Vector3.Dot(actualAxis, viewForward).ToString("F2")
                + " upDot=" + Vector3.Dot(actualAxis, Vector3.up).ToString("F2"));
        }
    }
}

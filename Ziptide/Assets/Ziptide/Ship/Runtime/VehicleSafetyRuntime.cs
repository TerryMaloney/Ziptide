using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// Safety envelope around VehicleRuntime. VehicleRuntime remains the sole movement/steering owner;
    /// this component remembers its last physically supported pose and rejects impossible one-frame
    /// vertical escapes such as the Quest failure that jumped a hoverbike from y=0.8 to y=10.7 and then
    /// emitted VEHICLE_EDGE_BLOCK forever. It also corrects an impossible mounted eye height after the
    /// vehicle has positioned the persistent rig.
    /// </summary>
    [DefaultExecutionOrder(10000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VehicleRuntime))]
    public sealed class VehicleSafetyRuntime : MonoBehaviour
    {
        public const float MaximumGroundStep = 1.50f;
        public const float MaximumVerticalEscape = 2.50f;
        public const float MaximumMountedEyeHeight = 1.95f;
        public const float MinimumMountedEyeHeight = 0.65f;
        public const float MountedRecoveryEyeHeight = 1.35f;

        private VehicleRuntime _vehicle;
        private Vector3 _lastSafePosition;
        private Quaternion _lastSafeRotation;
        private float _lastSafeGroundY;
        private bool _hasSafePose;
        private float _unsupportedSince = -1f;
        private bool _eyeRepairLogged;

        private static bool _hooked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            if (_hooked) SceneManager.sceneLoaded -= OnSceneLoadedStatic;
            _hooked = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            EnsureAll();
            if (_hooked) return;
            SceneManager.sceneLoaded += OnSceneLoadedStatic;
            _hooked = true;
        }

        private static void OnSceneLoadedStatic(Scene scene, LoadSceneMode mode)
        {
            EnsureAll();
        }

        private static void EnsureAll()
        {
            VehicleRuntime[] vehicles = Object.FindObjectsOfType<VehicleRuntime>(true);
            for (int i = 0; i < vehicles.Length; i++)
            {
                VehicleRuntime vehicle = vehicles[i];
                if (vehicle != null && vehicle.GetComponent<VehicleSafetyRuntime>() == null)
                    vehicle.gameObject.AddComponent<VehicleSafetyRuntime>();
            }
        }

        private void Awake()
        {
            _vehicle = GetComponent<VehicleRuntime>();
        }

        private void Start()
        {
            TryRecordSafePose("start");
        }

        private void LateUpdate()
        {
            if (_vehicle == null) return;

            bool supported = TryGroundY(transform.position, out float groundY);
            if (supported)
            {
                float clearance = transform.position.y - groundY;
                bool plausibleClearance = clearance >= -0.25f && clearance <= MaximumVerticalEscape;
                bool plausibleStep = !_hasSafePose || Mathf.Abs(groundY - _lastSafeGroundY) <= MaximumGroundStep;
                bool plausiblePose = !_hasSafePose || Mathf.Abs(transform.position.y - _lastSafePosition.y) <= MaximumVerticalEscape;

                if (plausibleClearance && plausibleStep && plausiblePose)
                {
                    RecordSafePose(groundY);
                    _unsupportedSince = -1f;
                }
                else
                {
                    RecoverVehicle("vertical_escape", groundY, clearance);
                }
            }
            else
            {
                if (_unsupportedSince < 0f) _unsupportedSince = Time.unscaledTime;
                if (_hasSafePose && Time.unscaledTime - _unsupportedSince >= 0.75f)
                    RecoverVehicle("unsupported", _lastSafeGroundY, float.NaN);
            }

            if (_vehicle.IsRiding) RepairMountedEyeHeight();
            else _eyeRepairLogged = false;
        }

        private void RepairMountedEyeHeight()
        {
            PlayerRigPersistence rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (rig == null) return;
            Camera cam = rig.GetComponentInChildren<Camera>(true);
            if (cam == null) return;

            float eyeHeight = cam.transform.position.y - transform.position.y;
            if (eyeHeight >= MinimumMountedEyeHeight && eyeHeight <= MaximumMountedEyeHeight) return;

            Vector3 p = rig.transform.position;
            p.y += MountedRecoveryEyeHeight - eyeHeight;
            rig.transform.position = p;
            if (!_eyeRepairLogged)
            {
                _eyeRepairLogged = true;
                Debug.LogError("ZIPTIDE: VEHICLE_EYE_HEIGHT_REPAIRED id=" + _vehicle.VehicleId
                    + " from=" + eyeHeight.ToString("F2") + " to=" + MountedRecoveryEyeHeight.ToString("F2"));
            }
        }

        private void TryRecordSafePose(string reason)
        {
            if (!TryGroundY(transform.position, out float groundY)) return;
            RecordSafePose(groundY);
            Debug.Log("ZIPTIDE: VEHICLE_SAFE_POSE id=" + (_vehicle != null ? _vehicle.VehicleId : "unknown")
                + " reason=" + reason + " position=" + transform.position.ToString("F2"));
        }

        private void RecordSafePose(float groundY)
        {
            _lastSafePosition = transform.position;
            _lastSafeRotation = transform.rotation;
            _lastSafeGroundY = groundY;
            _hasSafePose = true;
        }

        private void RecoverVehicle(string reason, float candidateGroundY, float clearance)
        {
            if (!_hasSafePose)
            {
                TryRecordSafePose("recovery_seed");
                return;
            }

            Vector3 escaped = transform.position;
            transform.SetPositionAndRotation(_lastSafePosition, _lastSafeRotation);
            _unsupportedSince = -1f;
            Debug.LogError("ZIPTIDE: VEHICLE_VERTICAL_ESCAPE_RECOVER id=" + _vehicle.VehicleId
                + " reason=" + reason + " from=" + escaped.ToString("F2")
                + " to=" + _lastSafePosition.ToString("F2")
                + " candidateGround=" + candidateGroundY.ToString("F2")
                + " clearance=" + (float.IsNaN(clearance) ? "NONE" : clearance.ToString("F2")));
        }

        private bool TryGroundY(Vector3 position, out float groundY)
        {
            RaycastHit[] hits = Physics.RaycastAll(position + Vector3.up * 4f, Vector3.down, 34f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            PlayerRigPersistence rig = Object.FindObjectOfType<PlayerRigPersistence>();
            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col == null || col.transform.IsChildOf(transform)) continue;
                if (rig != null && col.transform.IsChildOf(rig.transform)) continue;
                groundY = hits[i].point.y;
                return true;
            }
            groundY = 0f;
            return false;
        }
    }
}

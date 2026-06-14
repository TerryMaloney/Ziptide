using UnityEngine;
using UnityEngine.InputSystem;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Hold LeftGrip + RightGrip for 1 second to emergency-respawn.
    /// Prevents getting permanently stuck in broken geometry or below the city.
    /// Attaches to the persistent XR Origin (added by ScenePatcherD2).
    /// </summary>
    public class EmergencyRespawn : MonoBehaviour
    {
        [Tooltip("How long both grips must be held before respawn triggers.")]
        [SerializeField] private float holdSeconds = 1.0f;

        private InputAction _leftGrip;
        private InputAction _rightGrip;
        private float _holdTimer;
        private bool _triggered;

        private void OnEnable()
        {
            _leftGrip = new InputAction("EmergencyLeft", InputActionType.Button);
            _leftGrip.AddBinding("<XRController>{LeftHand}/grip");
            _leftGrip.Enable();

            _rightGrip = new InputAction("EmergencyRight", InputActionType.Button);
            _rightGrip.AddBinding("<XRController>{RightHand}/grip");
            _rightGrip.Enable();
        }

        private void OnDisable()
        {
            _leftGrip?.Disable();
            _rightGrip?.Disable();
        }

        private void Update()
        {
            bool leftHeld = _leftGrip != null && _leftGrip.IsPressed();
            bool rightHeld = _rightGrip != null && _rightGrip.IsPressed();

            if (leftHeld && rightHeld)
            {
                _holdTimer += Time.deltaTime;
                if (!_triggered && _holdTimer >= holdSeconds)
                {
                    _triggered = true;
                    DoRespawn();
                }
            }
            else
            {
                _holdTimer = 0f;
                _triggered = false;
            }
        }

        private void DoRespawn()
        {
            Debug.Log("ZIPTIDE: EMERGENCY_RESPAWN");

            // Prefer WorldRuntime.RespawnPlayer (uses WorldProfile.spawnPosition).
            var worldRuntime = Object.FindObjectOfType<WorldRuntime>();
            if (worldRuntime != null)
            {
                worldRuntime.RespawnPlayer(transform);
                return;
            }

            // Fallback: teleport to SpawnMarkerRuntime.
            var marker = Object.FindObjectOfType<SpawnMarkerRuntime>();
            if (marker != null)
            {
                var cc = GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                transform.position = marker.transform.position;
                transform.rotation = marker.transform.rotation;
                if (cc != null) cc.enabled = true;
                Debug.Log("ZIPTIDE: EMERGENCY_RESPAWN teleported to marker '" + marker.markerId + "'");
                return;
            }

            Debug.LogWarning("ZIPTIDE: EMERGENCY_RESPAWN no WorldRuntime or SpawnMarkerRuntime found; cannot respawn.");
        }
    }
}

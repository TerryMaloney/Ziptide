using UnityEngine;
using UnityEngine.InputSystem;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Short forward dash/hop on button press. Uses CharacterController.Move()
    /// so it respects colliders and doesn't fight XRI move providers.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class DashLocomotion : MonoBehaviour
    {
        private float _distance = 3f;
        private float _duration = 0.15f;
        private float _cooldown = 0.5f;
        private float _verticalLift = 0.1f;

        private CharacterController _cc;
        private Transform _cameraTransform;
        private InputAction _dashAction;

        private float _cooldownTimer;
        private float _dashTimer;
        private Vector3 _dashDirection;
        private bool _isDashing;

        public void Configure(float distance, float duration, float cooldown, float verticalLift)
        {
            _distance = distance;
            _duration = Mathf.Max(0.01f, duration);
            _cooldown = cooldown;
            _verticalLift = verticalLift;
        }

        private void OnEnable()
        {
            _cc = GetComponent<CharacterController>();

            var cam = GetComponentInChildren<Camera>(true);
            if (cam == null) cam = Camera.main;
            _cameraTransform = cam != null ? cam.transform : transform;

            if (_dashAction == null)
            {
                _dashAction = new InputAction("ZiptideDash", InputActionType.Button);
                _dashAction.AddBinding("<XRController>{RightHand}/primaryButton");
            }
            _dashAction.Enable();
        }

        private void OnDisable()
        {
            _dashAction?.Disable();
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            if (_isDashing)
            {
                _dashTimer -= Time.deltaTime;
                if (_dashTimer <= 0f)
                {
                    _isDashing = false;
                    return;
                }

                float speed = _distance / _duration;
                Vector3 move = _dashDirection * speed * Time.deltaTime;
                move.y += _verticalLift * (Time.deltaTime / _duration);
                if (_cc != null && _cc.enabled)
                    _cc.Move(move);
                return;
            }

            if (_dashAction != null && _dashAction.WasPressedThisFrame() && _cooldownTimer <= 0f)
                StartDash();
        }

        private void StartDash()
        {
            Vector3 forward = _cameraTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f)
                forward = transform.forward;
            forward.Normalize();

            _dashDirection = forward;
            _dashTimer = _duration;
            _cooldownTimer = _cooldown;
            _isDashing = true;
        }
    }
}

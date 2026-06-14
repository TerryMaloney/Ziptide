using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Console-style locomotion extras on the persistent XR rig:
    ///   - Jump: press A (right-hand primary button) for a vertical hop via CharacterController.
    ///   - Sprint: hold/click the LEFT thumbstick to move faster.
    /// Self-contained (own gravity, own input actions) so it works in every scene the
    /// persistent rig travels into. Class name kept as DashLocomotion to preserve existing
    /// scene component references (GUID) and the LocomotionDirector.Configure() call.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class DashLocomotion : MonoBehaviour
    {
        private float _jumpHeight = 1.1f;
        private float _gravity = 16f;
        private float _jumpCooldown = 0.2f;
        private float _sprintMultiplier = 1.9f;

        private CharacterController _cc;
        private ActionBasedContinuousMoveProvider _moveProvider;
        private float _baseMoveSpeed = -1f;

        private InputAction _jumpAction;
        private InputAction _sprintAction;

        private float _cooldownTimer;
        private float _verticalVelocity;
        private bool _jumping;
        private bool _sprinting;

        /// <summary>
        /// Kept for LocomotionDirector compatibility. The old dash distance/duration are unused;
        /// verticalLift is repurposed as an optional jump-height hint, cooldown as jump cooldown.
        /// </summary>
        public void Configure(float distance, float duration, float cooldown, float verticalLift)
        {
            if (verticalLift > 0.01f) _jumpHeight = Mathf.Clamp(verticalLift * 6f, 0.4f, 2.5f);
            _jumpCooldown = Mathf.Max(0.05f, cooldown);
        }

        private void OnEnable()
        {
            _cc = GetComponent<CharacterController>();
            _moveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);

            if (_jumpAction == null)
            {
                _jumpAction = new InputAction("ZiptideJump", InputActionType.Button);
                _jumpAction.AddBinding("<XRController>{RightHand}/primaryButton"); // A
            }
            if (_sprintAction == null)
            {
                _sprintAction = new InputAction("ZiptideSprint", InputActionType.Button);
                _sprintAction.AddBinding("<XRController>{LeftHand}/thumbstickClicked"); // L3
            }
            _jumpAction.Enable();
            _sprintAction.Enable();
        }

        private void OnDisable()
        {
            EndSprint();
            _jumpAction?.Disable();
            _sprintAction?.Disable();
        }

        private void Update()
        {
            if (_cc == null || !_cc.enabled) return;
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

            HandleSprint();
            HandleJump();
        }

        private void HandleSprint()
        {
            if (_moveProvider == null) return;

            bool wantSprint = _sprintAction != null && _sprintAction.IsPressed();
            if (wantSprint && !_sprinting)
            {
                _baseMoveSpeed = _moveProvider.moveSpeed;
                _moveProvider.moveSpeed = _baseMoveSpeed * _sprintMultiplier;
                _sprinting = true;
            }
            else if (!wantSprint && _sprinting)
            {
                EndSprint();
            }
        }

        private void EndSprint()
        {
            if (_sprinting && _moveProvider != null && _baseMoveSpeed > 0f)
                _moveProvider.moveSpeed = _baseMoveSpeed;
            _sprinting = false;
        }

        private void HandleJump()
        {
            bool grounded = _cc.isGrounded;

            if (!_jumping && grounded && _cooldownTimer <= 0f
                && _jumpAction != null && _jumpAction.WasPressedThisFrame())
            {
                _verticalVelocity = Mathf.Sqrt(2f * _gravity * _jumpHeight);
                _jumping = true;
                _cooldownTimer = _jumpCooldown;
            }

            if (_jumping)
            {
                _verticalVelocity -= _gravity * Time.deltaTime;
                _cc.Move(Vector3.up * (_verticalVelocity * Time.deltaTime));
                if (_verticalVelocity <= 0f && _cc.isGrounded)
                    _jumping = false;
            }
        }
    }
}

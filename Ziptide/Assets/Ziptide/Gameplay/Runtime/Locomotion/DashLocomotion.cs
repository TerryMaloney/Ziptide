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
        private float _diagTimer;

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

            // Guarantee a usable walk speed even if no per-scene LocomotionDirector configured it
            // (fixes "can't move in the test room"). Diagnostic logs the real rig state.
            if (_moveProvider != null && _moveProvider.moveSpeed < 0.1f)
                _moveProvider.moveSpeed = 1.75f;
            EnsureMoveActionsEnabled();
            Debug.Log("ZIPTIDE: LOCO_STATE moveProvider=" + (_moveProvider != null)
                + " moveSpeed=" + (_moveProvider != null ? _moveProvider.moveSpeed : 0f)
                + " cc=" + (_cc != null) + " ccEnabled=" + (_cc != null && _cc.enabled));

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
            // Keep the move provider + its input actions live every frame. On the FIRST scene
            // load the move action can come up disabled (it's re-armed only after a scene cycle),
            // which is why movement worked only on the 2nd entry. This fixes first-load movement.
            EnsureMoveActionsEnabled();
            MoveDiagTick();

            if (_cc == null || !_cc.enabled) return;
            if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

            // Keep walk speed alive across scene loads if something zeroed it.
            if (!_sprinting && _moveProvider != null && _moveProvider.moveSpeed < 0.1f)
                _moveProvider.moveSpeed = 1.75f;

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

        private void EnsureMoveActionsEnabled()
        {
            if (_moveProvider == null) return;
            if (!_moveProvider.enabled) _moveProvider.enabled = true;
            var la = _moveProvider.leftHandMoveAction.action;
            var ra = _moveProvider.rightHandMoveAction.action;
            if (la != null && !la.enabled) la.Enable();
            if (ra != null && !ra.enabled) ra.Enable();
        }

        private void MoveDiagTick()
        {
            _diagTimer -= Time.deltaTime;
            if (_diagTimer > 0f) return;
            _diagTimer = 1f;
            var la = _moveProvider != null ? _moveProvider.leftHandMoveAction.action : null;
            Debug.Log("ZIPTIDE: MOVE_DIAG provEnabled=" + (_moveProvider != null && _moveProvider.enabled)
                + " leftActEnabled=" + (la != null && la.enabled)
                + " leftVal=" + (la != null ? la.ReadValue<Vector2>().ToString("F2") : "null")
                + " grounded=" + (_cc != null && _cc.isGrounded));
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

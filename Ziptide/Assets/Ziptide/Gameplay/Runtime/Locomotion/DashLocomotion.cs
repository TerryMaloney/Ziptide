using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Console-style locomotion extras on the persistent XR rig (the body-verb owner per
    /// docs/design/CONTROL_SCHEME.md):
    ///   - Jump: A (right primary). Sprint: hold/click L3. Auto-run: double-click L3.
    ///   - Crouch: R3 toggle (lower CC + camera, slower). Slide: crouch while sprinting.
    /// Self-contained (own gravity, own input actions) so it works in every scene the
    /// persistent rig travels into. Class name kept as DashLocomotion to preserve existing
    /// scene component references (GUID) and the LocomotionDirector.Configure() call.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class DashLocomotion : MonoBehaviour
    {
        private const float FallbackWalkSpeed = 3f; // matches LocomotionProfile.moveSpeed default
        private const float CrouchCamDrop = 0.55f;  // how far the view lowers while crouched
        private const float CrouchCcHeight = 1.05f; // capped every frame (an HMD driver may fight it)

        private float _jumpHeight = 1.1f;
        private float _gravity = 16f;
        private float _jumpCooldown = 0.2f;
        private float _sprintMultiplier = 2f;
        private float _crouchSpeedFactor = 0.55f;
        private float _slideBoost = 1.35f;
        private float _slideSeconds = 0.8f;
        private float _autoRunTapWindow = 0.35f;

        private CharacterController _cc;
        private ActionBasedContinuousMoveProvider _moveProvider;
        private float _baseMoveSpeed = -1f;

        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _crouchAction;

        private float _cooldownTimer;
        private float _verticalVelocity;
        private bool _jumping;
        private bool _sprinting;
        private bool _crouched;
        private bool _autoRun;
        private float _slideTimer;      // > 0 while sliding
        private float _lastSprintPress; // L3 double-tap detection
        private float _ccStandHeight = -1f;
        private Transform _camOffset;   // camera's parent — lowered while crouched
        private Camera _cam;            // gaze source for auto-run heading
        private float _camOffsetStandY;
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

        /// <summary>Profile-driven sprint. Values ≤1 (including 0 from pre-sprint assets that
        /// lack the field) keep the built-in default rather than disabling sprint.</summary>
        public void ConfigureSprint(float multiplier)
        {
            if (multiplier > 1.01f) _sprintMultiplier = Mathf.Min(multiplier, 4f);
        }

        /// <summary>Profile-driven crouch/slide/auto-run (CONTROL_SCHEME.md). Zeroes from
        /// pre-field assets keep the built-in defaults.</summary>
        public void ConfigureBody(float crouchFactor, float slideBoost, float slideSeconds, float tapWindow)
        {
            if (crouchFactor > 0.05f) _crouchSpeedFactor = Mathf.Clamp(crouchFactor, 0.2f, 1f);
            if (slideBoost > 1.01f) _slideBoost = Mathf.Min(slideBoost, 2.5f);
            if (slideSeconds > 0.05f) _slideSeconds = Mathf.Min(slideSeconds, 3f);
            if (tapWindow > 0.05f) _autoRunTapWindow = Mathf.Min(tapWindow, 1f);
        }

        private void OnEnable()
        {
            _cc = GetComponent<CharacterController>();
            _moveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);

            // Guarantee a usable walk speed even if no per-scene LocomotionDirector configured it
            // (fixes "can't move in the test room"). Diagnostic logs the real rig state.
            if (_moveProvider != null && _moveProvider.moveSpeed < 0.1f)
                _moveProvider.moveSpeed = FallbackWalkSpeed;
            EnsureMoveActionsEnabled();
            Debug.Log("ZIPTIDE: LOCO_STATE moveProvider=" + (_moveProvider != null)
                + " moveSpeed=" + (_moveProvider != null ? _moveProvider.moveSpeed : 0f)
                + " cc=" + (_cc != null) + " ccEnabled=" + (_cc != null && _cc.enabled));
            Debug.Log("ZIPTIDE: CONTROLS move=left-stick turn=right-stick sprint=hold-L3 autorun=double-L3 crouch=R3 slide=crouch-while-sprinting jump=A menu=hold-Y+B");

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
            if (_crouchAction == null)
            {
                _crouchAction = new InputAction("ZiptideCrouch", InputActionType.Button);
                _crouchAction.AddBinding("<XRController>{RightHand}/thumbstickClicked"); // R3
            }
            _jumpAction.Enable();
            _sprintAction.Enable();
            _crouchAction.Enable();

            _cam = GetComponentInChildren<Camera>(true);
            _camOffset = _cam != null ? _cam.transform.parent : null;
        }

        private void OnDisable()
        {
            if (_crouched) SetCrouch(false);
            EndSprint();
            _jumpAction?.Disable();
            _sprintAction?.Disable();
            _crouchAction?.Disable();
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
                _moveProvider.moveSpeed = FallbackWalkSpeed;

            HandleBodyVerbs();
            HandleJump();
        }

        /// <summary>
        /// One resolver owns moveSpeed so sprint/crouch/slide/auto-run can't fight over it
        /// (CONTROL_SCHEME.md: slide > crouch > sprint > walk).
        /// </summary>
        private void HandleBodyVerbs()
        {
            if (_moveProvider == null) return;

            bool anyModifier = _sprinting || _crouched || _autoRun || _slideTimer > 0f;
            // Track the walk speed while NO verb owns it, so a mid-verb profile change
            // (scene travel reapplies LocomotionDirector) can't restore a stale base.
            if (!anyModifier && _moveProvider.moveSpeed >= 0.1f)
                _baseMoveSpeed = _moveProvider.moveSpeed;
            float baseSpeed = _baseMoveSpeed > 0.1f ? _baseMoveSpeed : FallbackWalkSpeed;

            // ── Inputs ──
            bool sprintHeld = _sprintAction != null && _sprintAction.IsPressed();
            bool sprintPressed = _sprintAction != null && _sprintAction.WasPressedThisFrame();
            bool crouchPressed = _crouchAction != null && _crouchAction.WasPressedThisFrame();

            // Auto-run: double-click L3 toggles; any of stick input / jump / crouch cancels.
            if (sprintPressed)
            {
                if (Time.unscaledTime - _lastSprintPress < _autoRunTapWindow)
                {
                    _autoRun = !_autoRun;
                    Debug.Log("ZIPTIDE: LOCO_STATE autorun=" + _autoRun);
                }
                _lastSprintPress = Time.unscaledTime;
            }
            if (_autoRun && (StickMagnitude() > 0.35f || _jumping || crouchPressed))
            {
                _autoRun = false;
                Debug.Log("ZIPTIDE: LOCO_STATE autorun=false");
            }

            // Crouch toggle; crouching WHILE sprinting starts a slide.
            if (crouchPressed)
            {
                if (!_crouched && (_sprinting || _autoRun))
                {
                    _slideTimer = _slideSeconds;
                    Debug.Log("ZIPTIDE: LOCO_STATE slide=true");
                }
                SetCrouch(!_crouched);
            }
            // Jumping stands you up (and Fortnite-style cancels the slide).
            if (_jumping && _crouched) { _slideTimer = 0f; SetCrouch(false); }

            bool sprintNow = sprintHeld && !_crouched;
            if (sprintNow != _sprinting)
            {
                _sprinting = sprintNow;
                Debug.Log("ZIPTIDE: LOCO_STATE sprint=" + _sprinting);
            }

            // ── Speed resolution (slide > crouch > sprint/auto-run > walk) ──
            float speed;
            if (_slideTimer > 0f)
            {
                _slideTimer -= Time.deltaTime;
                float t = Mathf.Clamp01(_slideTimer / Mathf.Max(0.05f, _slideSeconds));
                speed = Mathf.Lerp(baseSpeed * _crouchSpeedFactor, baseSpeed * _sprintMultiplier * _slideBoost, t);
                if (_slideTimer <= 0f) Debug.Log("ZIPTIDE: LOCO_STATE slide=false");
            }
            else if (_crouched) speed = baseSpeed * _crouchSpeedFactor;
            else if (_sprinting || _autoRun) speed = baseSpeed * _sprintMultiplier;
            else speed = baseSpeed;
            _moveProvider.moveSpeed = speed;

            // Auto-run pushes the body forward along the flattened gaze (additive with the
            // provider's stick movement, which is zero while auto-running by definition).
            if (_autoRun)
            {
                Vector3 fwd = _cam != null ? _cam.transform.forward : transform.forward;
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 0.001f)
                    _cc.Move(fwd.normalized * speed * Time.deltaTime);
            }

            // While crouched, keep the CC capped every frame (an HMD height driver may re-expand it).
            if (_crouched && _cc.height > CrouchCcHeight)
            {
                _cc.height = CrouchCcHeight;
                _cc.center = new Vector3(_cc.center.x, CrouchCcHeight * 0.5f, _cc.center.z);
            }
        }

        private void SetCrouch(bool crouch)
        {
            if (crouch == _crouched) return;
            _crouched = crouch;

            if (_ccStandHeight < 0f && _cc != null) _ccStandHeight = _cc.height;
            if (_camOffset != null)
            {
                if (crouch) _camOffsetStandY = _camOffset.localPosition.y;
                var lp = _camOffset.localPosition;
                lp.y = crouch ? _camOffsetStandY - CrouchCamDrop : _camOffsetStandY;
                _camOffset.localPosition = lp;
            }
            if (!crouch && _cc != null && _ccStandHeight > 0f)
            {
                _cc.height = _ccStandHeight;
                _cc.center = new Vector3(_cc.center.x, _ccStandHeight * 0.5f, _cc.center.z);
            }
            Debug.Log("ZIPTIDE: LOCO_STATE crouch=" + crouch);
        }

        private float StickMagnitude()
        {
            var la = _moveProvider != null ? _moveProvider.leftHandMoveAction.action : null;
            return la != null && la.enabled ? la.ReadValue<Vector2>().magnitude : 0f;
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

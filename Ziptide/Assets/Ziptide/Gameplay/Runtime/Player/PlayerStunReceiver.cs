using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Lives on the persistent rig. Non-lethal hit reaction for drone stun bolts (Drone Combat V1):
    /// a brief screen flash + temporary movement slow that self-clears. NO health, NO death.
    /// Ensured by <see cref="PlayerRigPersistence"/> so it's present in every world.
    /// </summary>
    public class PlayerStunReceiver : MonoBehaviour
    {
        [Tooltip("Tint of the brief screen flash on a stun hit.")]
        public Color flashColor = new Color(0.3f, 0.9f, 1f, 0.55f);

        public static event System.Action OnPlayerStunned;

        private readonly StunState _stun = new StunState();
        private Camera _cam;
        private ActionBasedContinuousMoveProvider _move;
        private float _baseMoveSpeed = -1f; // the TRUE full-speed value, captured once while NOT slowed
        private bool _haveBase;

        private GameObject _flashGo;
        private Renderer _flashRenderer;
        private float _flashTimer;
        private const float FlashDuration = 0.4f;

        /// <summary>Head (camera) transform — what stun bolts home in on.</summary>
        public Transform Head => _cam != null ? _cam.transform : transform;
        /// <summary>World point a bolt should test against (the head).</summary>
        public Vector3 HitPoint => Head.position;

        private void Awake()
        {
            _cam = GetComponentInChildren<Camera>();
            _move = GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);
        }

        /// <summary>Apply a non-lethal stun: flash + slow for <paramref name="seconds"/>. Re-stuns refresh.</summary>
        public void ApplyStun(float seconds, float slowFactor)
        {
            _stun.Apply(seconds, slowFactor);
            _flashTimer = FlashDuration;
            EnsureFlash();
            OnPlayerStunned?.Invoke();
            Debug.Log("ZIPTIDE: PLAYER_STUN sec=" + seconds.ToString("F2") + " slow=" + slowFactor.ToString("F2"));
        }

        private void Update()
        {
            _stun.Tick(Time.deltaTime);

            if (_cam == null) _cam = GetComponentInChildren<Camera>();
            if (_move == null) _move = GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);

            if (_move != null)
            {
                // Capture the TRUE base speed exactly once, and only while NOT slowed, so we can never
                // latch a reduced value as "base" (that bug left walking permanently slow after a stun that
                // straddled a scene load). Then ALWAYS drive moveSpeed = base * SlowFactor: it self-heals —
                // the instant the stun clears, SlowFactor is 1, so speed snaps back to base every frame.
                if (!_haveBase && _stun.IsClear) { _baseMoveSpeed = _move.moveSpeed; _haveBase = true; }
                if (_haveBase) _move.moveSpeed = _baseMoveSpeed * _stun.SlowFactor;
            }

            UpdateFlash();
        }

        private void EnsureFlash()
        {
            if (_flashGo != null || _cam == null) return;
            _flashGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _flashGo.name = "__StunFlash";
            var col = _flashGo.GetComponent<Collider>();
            if (col != null) Destroy(col);
            _flashGo.transform.SetParent(_cam.transform, false);
            _flashGo.transform.localPosition = new Vector3(0f, 0f, 0.35f);
            _flashGo.transform.localRotation = Quaternion.identity;
            _flashGo.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
            _flashRenderer = _flashGo.GetComponent<Renderer>();
            if (_flashRenderer != null)
            {
                var shader = Shader.Find("Sprites/Default");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
                var mat = new Material(shader);
                _flashRenderer.material = mat;
                _flashRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        private void UpdateFlash()
        {
            if (_flashRenderer == null) return;
            if (_flashTimer > 0f) _flashTimer -= Time.deltaTime;
            float a = Mathf.Clamp01(_flashTimer / FlashDuration) * flashColor.a;
            bool visible = a > 0.001f;
            if (_flashGo.activeSelf != visible) _flashGo.SetActive(visible);
            if (visible)
            {
                var c = new Color(flashColor.r, flashColor.g, flashColor.b, a);
                if (_flashRenderer.material.HasProperty("_Color")) _flashRenderer.material.color = c;
                else if (_flashRenderer.material.HasProperty("_BaseColor")) _flashRenderer.material.SetColor("_BaseColor", c);
            }
        }
    }
}

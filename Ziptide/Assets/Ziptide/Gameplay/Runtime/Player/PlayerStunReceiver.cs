using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Lives on the persistent rig. Non-lethal hit reaction for drone stun bolts (Drone Combat V1):
    /// a brief screen flash + temporary movement slow that self-clears.
    ///
    /// ⚠ This once read "NO health, NO death" — and that was the largest gameplay hole in the game:
    /// every creature's authored `damage` applied to nobody, so combat had no stakes. Phase B ended
    /// it. This receiver now hosts <see cref="PlayerArmor"/>, which owns armor, death and respawn.
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

            // COMBAT_HEALTH_PLAN Phase B: the class summary above used to end "NO health, NO death".
            // It does now. This receiver already owns the head, the screen flash and the incoming-fire
            // tracer, so it hosts the armor shell rather than a new automatic owner being registered
            // for it — the shell reuses this feedback instead of inventing a second flash.
            PlayerArmor.EnsureOn(gameObject);
        }

        /// <summary>
        /// A hit that both HURTS and stuns — the one entry point every attacker should use now that
        /// the player has armor. Damage is on the canonical integer scale (`PvpRules`); the stun
        /// values stay per-attacker because a creature's shove and a drone's bolt should not feel the
        /// same. Damage of 0 degrades to a pure stun, so a harmless contact stays harmless.
        ///
        /// Pairing them here rather than inside PlayerArmor keeps the flash and the direction tracer
        /// firing exactly once per hit.
        /// </summary>
        public void ApplyHit(int damage, float seconds, float slowFactor, Vector3 sourcePos)
        {
            if (damage > 0)
            {
                PlayerArmor armor = PlayerArmor.EnsureOn(gameObject);
                if (armor != null) armor.ApplyDamage(damage, sourcePos);
            }
            ApplyStun(seconds, slowFactor, sourcePos);
        }

        /// <summary>
        /// Drop any active stun immediately and restore full move speed. Called when a level changes:
        /// a slow applied by a drone in one world used to follow the player into the next one, which
        /// is the unexplained "walk speed is wrong after the arena" symptom on the device checklist.
        /// </summary>
        public void ClearStun()
        {
            _stun.Clear();
            _flashTimer = 0f;
            if (_move != null && _haveBase) _move.moveSpeed = _baseMoveSpeed;
        }

        /// <summary>Apply a non-lethal stun: flash + slow for <paramref name="seconds"/>. Re-stuns refresh.</summary>
        public void ApplyStun(float seconds, float slowFactor)
        {
            // Sure Step (A4.5): resistance washes the slow toward harmless — shorter AND shallower.
            float r = Ziptide.Multiplayer.Augments.AugmentEffects.SlowResistance;
            if (r > 0f)
            {
                seconds *= (1f - r);
                slowFactor = 1f - (1f - slowFactor) * (1f - r);
            }
            _stun.Apply(seconds, slowFactor);
            _flashTimer = FlashDuration;
            EnsureFlash();
            OnPlayerStunned?.Invoke();
            Debug.Log("ZIPTIDE: PLAYER_STUN sec=" + seconds.ToString("F2") + " slow=" + slowFactor.ToString("F2"));
        }

        /// <summary>
        /// Stun with a known attacker position: additionally draws the incoming-fire line toward
        /// the player's head so you can SEE what hit you (Test Day 1, W005: "getting hit by
        /// something but i cant tell what").
        /// </summary>
        public void ApplyStun(float seconds, float slowFactor, Vector3 sourcePos)
        {
            ApplyStun(seconds, slowFactor);
            Vector3 head = HitPoint;
            Vector3 toward = head - sourcePos;
            if (toward.sqrMagnitude > 0.04f)
                TracerFx.Spawn(sourcePos, head - toward.normalized * 0.4f, new Color(1f, 0.3f, 0.2f), 0.02f, 0.25f);
            Debug.Log("ZIPTIDE: PLAYER_HIT src=" + sourcePos.ToString("F1"));
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

using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Base seam for all creature motion (GAME_PLAN M3; generalizes the proven drone pattern). Owns:
    /// the COLLISION-CLEAN mover (SphereCast clamp — nothing moves through solids, LAW), the home leash,
    /// player finding, and the non-lethal contact-stun with a per-contact cooldown. Subclasses implement
    /// <see cref="Tick"/>. Gated on <see cref="CreatureRuntime.IsActive"/> so stuns/disables pause motion
    /// automatically. Visuals are built by the subclass in Awake (edit-time spawn carries only serialized
    /// fields — gotcha #7).
    /// </summary>
    [RequireComponent(typeof(CreatureRuntime))]
    public abstract class CreatureBehaviorBase : MonoBehaviour
    {
        [Tooltip("Max distance from home before the creature turns back (keeps encounters in their zone).")]
        public float leashRadius = 12f;
        [Tooltip("Player detection range.")]
        public float detectRange = 10f;
        [Tooltip("Seconds of player slow applied on contact.")]
        public float touchStunSeconds = 0.6f;
        [Range(0.1f, 1f)] public float touchSlowFactor = 0.6f;
        [Tooltip("Cooldown between contact stuns so standing in a swarm doesn't machine-gun the effect.")]
        public float touchCooldown = 1.2f;
        [Tooltip("Distance that counts as touching the player.")]
        public float touchRadius = 0.5f;

        protected CreatureRuntime Runtime { get; private set; }
        protected Transform Player { get; private set; }
        private PlayerStunReceiver _stun;
        private float _nextTouchAllowed;

        protected virtual void Awake()
        {
            Runtime = GetComponent<CreatureRuntime>();
            BuildVisuals();
            // FORGE II P3+P4: if an authored creature body exists for this id, the skinned
            // WALKING body takes over the look. The primitive parts stay alive but invisible
            // (subclass fields keep their references; Tick logic is untouched) — a look,
            // never a stat.
            if (Ziptide.Visuals.ForgeCreatureVisualApplier.TryApply(gameObject, Runtime.creatureId))
                foreach (var r in GetComponentsInChildren<MeshRenderer>(true))
                    if (r != null && !IsUnderForgeVisual(r.transform)) r.enabled = false;
        }

        private bool IsUnderForgeVisual(Transform t)
        {
            while (t != null && t != transform)
            {
                if (t.name == Ziptide.Visuals.ForgeCreatureVisualApplier.VisualChildName) return true;
                t = t.parent;
            }
            return false;
        }

        private void Update()
        {
            if (Runtime == null || !Runtime.IsActive) return;
            if (Player == null) FindPlayer();

            float dist = Player != null ? Vector3.Distance(transform.position, Player.position) : 9999f;
            Tick(Time.deltaTime, dist);
            TryTouch(dist);
        }

        /// <summary>Per-frame behavior. dist = distance to the player's head (9999 if not found).</summary>
        protected abstract void Tick(float dt, float dist);

        /// <summary>Build the creature's primitive body (runtime, in Awake).</summary>
        protected abstract void BuildVisuals();

        /// <summary>Called by CreatureRuntime when a stun lands (optional reaction).</summary>
        public virtual void OnStunned(float seconds) { }

        /// <summary>Re-apply base colors after a stun tint clears.</summary>
        public virtual void RestoreVisuals() { }

        // ── Shared machinery ─────────────────────────────────────────────────

        private void FindPlayer()
        {
            if (_stun == null) _stun = FindObjectOfType<PlayerStunReceiver>();
            if (_stun != null) { Player = _stun.Head; return; }
            if (Camera.main != null) Player = Camera.main.transform;
        }

        /// <summary>Move with the universal wall clamp — the step stops at the nearest solid.</summary>
        protected void CollideMove(Vector3 to)
        {
            Vector3 from = transform.position;
            Vector3 delta = to - from;
            float dist = delta.magnitude;
            if (dist < 0.0001f) return;
            Vector3 dir = delta / dist;
            const float r = 0.22f;
            var hits = Physics.SphereCastAll(from, r, dir, dist, ~0, QueryTriggerInteraction.Ignore);
            float nearest = dist;
            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i].collider;
                if (col == null) continue;
                if (col.GetComponentInParent<CreatureRuntime>() != null) continue; // self / other creatures
                if (col.GetComponentInParent<DroneRuntime>() != null) continue;    // drones
                if (IsPlayerRig(col.transform)) continue;                          // the player
                if (hits[i].distance < nearest) nearest = hits[i].distance;
            }
            transform.position = nearest < dist ? from + dir * Mathf.Max(0f, nearest - r) : to;
        }

        /// <summary>Clamp a desired position to the home leash.</summary>
        protected Vector3 Leashed(Vector3 desired)
        {
            Vector3 off = desired - Runtime.HomePos;
            if (off.magnitude > leashRadius) desired = Runtime.HomePos + off.normalized * leashRadius;
            return desired;
        }

        private void TryTouch(float dist)
        {
            if (dist > touchRadius || Time.time < _nextTouchAllowed) return;
            _nextTouchAllowed = Time.time + touchCooldown;
            if (_stun != null) _stun.ApplyStun(touchStunSeconds, touchSlowFactor, transform.position);
        }

        protected static bool IsPlayerRig(Transform t)
        {
            while (t != null)
            {
                if (t.name == "XR Origin" || t.GetComponent<PlayerStunReceiver>() != null) return true;
                t = t.parent;
            }
            return false;
        }

        /// <summary>Face a point smoothly on the horizontal plane.</summary>
        protected void FaceToward(Vector3 point, float dt, float speed = 5f)
        {
            Vector3 look = point - transform.position; look.y = 0f;
            if (look.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), dt * speed);
        }

        /// <summary>Shared primitive helper for subclass visuals (URP-lit, no shadows, collider kept or stripped).</summary>
        protected GameObject MakePart(string name, PrimitiveType type, Vector3 localPos, Vector3 scale, Color color, bool keepCollider = false)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            if (!keepCollider)
            {
                var c = go.GetComponent<Collider>();
                if (c != null) Destroy(c);
            }
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                    else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
                    r.sharedMaterial = mat;
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            return go;
        }
    }
}

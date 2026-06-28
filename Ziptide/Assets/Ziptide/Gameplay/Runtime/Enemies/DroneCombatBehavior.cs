using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Drone Combat V1 — optional sibling of <see cref="DroneRuntime"/> (only on combat drones).
    /// Patrols its home, detects the player, orbits/strafes at a standoff, telegraphs, then fires a
    /// slow non-lethal <see cref="StunBolt"/>. All gated on <c>DroneRuntime.IsActive</c>, so shooting
    /// the drone (taser/pistol/gravity) still downs it normally and combat simply stops. Tunables are
    /// serialized and optionally overridden by a <see cref="DroneCombatProfile"/> asset (data variants).
    /// </summary>
    [RequireComponent(typeof(DroneRuntime))]
    public class DroneCombatBehavior : MonoBehaviour
    {
        [Header("Detection")]
        public float detectRange = 10f;
        public float loseRange = 14f;
        public LayerMask lineOfSightMask = ~0;

        [Header("Movement")]
        public float standoffDistance = 5.5f;
        public float orbitSpeed = 32f;
        public float verticalBob = 0.4f;
        public float patrolRadius = 3f;
        public float patrolSpeed = 18f;
        public float moveLerp = 1.7f;
        [Tooltip("Max distance the drone will stray from its home spot — keeps it in its open zone " +
                 "instead of chasing the player through buildings.")]
        public float leashRadius = 9f;

        [Header("Attack")]
        public float telegraphSeconds = 1.0f;
        public float boltCooldown = 2.8f;
        public float boltSpeed = 6.5f;
        public float stunSeconds = 1.2f;
        [Range(0.1f, 1f)] public float slowFactor = 0.45f;
        public Color telegraphColor = new Color(0.3f, 0.9f, 1f);

        [Tooltip("Optional data variant. Overrides the serialized values above when assigned.")]
        public DroneCombatProfile profile;

        private DroneRuntime _drone;
        private readonly DroneCombatState _fsm = new DroneCombatState();
        private Transform _player;
        private PlayerStunReceiver _receiver;
        private float _orbitAngle;
        private float _phase; // per-drone offset so a pack doesn't move in lockstep
        private GameObject _telegraphFx;
        private Renderer _telegraphRenderer;

        private void Awake()
        {
            _drone = GetComponent<DroneRuntime>();
            // Per-drone phase offset so multiple drones don't bob/orbit in sync (reads as alive, not cloned).
            _phase = (GetInstanceID() & 0x3FF) * 0.123f;
            ApplyProfile();
            _fsm.DetectRange = detectRange;
            _fsm.LoseRange = loseRange;
            _fsm.TelegraphSeconds = telegraphSeconds;
            _fsm.BoltCooldown = boltCooldown;
        }

        private void OnEnable()
        {
            if (_drone != null) _drone.CombatDriven = true; // we own motion
        }

        private void ApplyProfile()
        {
            if (profile == null) return;
            detectRange = profile.detectRange;
            loseRange = profile.loseRange;
            standoffDistance = profile.standoffDistance;
            orbitSpeed = profile.orbitSpeed;
            verticalBob = profile.verticalBob;
            patrolRadius = profile.patrolRadius;
            patrolSpeed = profile.patrolSpeed;
            telegraphSeconds = profile.telegraphSeconds;
            boltCooldown = profile.boltCooldown;
            boltSpeed = profile.boltSpeed;
            stunSeconds = profile.stunSeconds;
            slowFactor = profile.slowFactor;
        }

        private void Update()
        {
            if (_drone == null) return;
            float dt = Time.deltaTime;

            if (_player == null) FindPlayer();
            bool active = _drone.IsActive;
            float dist = _player != null ? Vector3.Distance(transform.position, _player.position) : 9999f;
            bool los = _player != null && HasLineOfSight();

            _fsm.Tick(dt, dist, los, active);

            if (!active) { ClearTelegraph(); return; }

            Move(dt, dist);
            UpdateTelegraph();

            if (_fsm.FireRequested) FireBolt();
        }

        private void Move(float dt, float dist)
        {
            Vector3 home = _drone.HomePos;
            Vector3 desired;

            if (_fsm.Phase == DroneCombatPhase.Patrol || _player == null)
            {
                _orbitAngle += patrolSpeed * dt * Mathf.Deg2Rad;
                desired = home + new Vector3(Mathf.Cos(_orbitAngle), 0f, Mathf.Sin(_orbitAngle)) * patrolRadius;
                desired.y = home.y + Mathf.Sin(Time.time + _phase) * verticalBob;
            }
            else
            {
                // Lifelike engage: NON-UNIFORM orbit speed + a slow "breathe" in/out on the standoff radius
                // (darts a little closer, then backs off) + the per-drone phase so a pack isn't synchronized.
                // Still feeds the leash clamp + CollideMove below, so it never clips walls or leaves its zone.
                _orbitAngle += orbitSpeed * dt * Mathf.Deg2Rad * (1f + 0.35f * Mathf.Sin(Time.time * 0.6f + _phase));
                Vector3 center = _player.position;
                float breathe = standoffDistance + Mathf.Sin(Time.time * 0.5f + _phase) * 1.0f;
                Vector3 ring = new Vector3(Mathf.Cos(_orbitAngle), 0f, Mathf.Sin(_orbitAngle)) * breathe;
                desired = center + ring;
                desired.y = home.y + Mathf.Sin(Time.time * 1.5f + _phase) * verticalBob;
                // face the player
                Vector3 look = center - transform.position; look.y = 0f;
                if (look.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), dt * 4f);
            }

            // Leash to home so combat drones hold their open zone instead of phasing through buildings
            // to chase you ("coming out of nowhere / shooting through walls").
            Vector3 off = desired - home;
            if (off.magnitude > leashRadius) desired = home + off.normalized * leashRadius;

            // Collision-aware: a drone moves by transform (no Rigidbody), so without this it flies THROUGH
            // building walls. Clamp the step at the nearest wall so it never clips into/through geometry.
            Vector3 next = Vector3.Lerp(transform.position, desired, dt * moveLerp);
            transform.position = CollideMove(transform.position, next);
        }

        // Stop the move at the nearest solid wall (ignoring the player rig and other drones).
        private Vector3 CollideMove(Vector3 from, Vector3 to)
        {
            Vector3 delta = to - from;
            float dist = delta.magnitude;
            if (dist < 0.0001f) return to;
            Vector3 dir = delta / dist;
            const float r = 0.22f;
            var hits = Physics.SphereCastAll(from, r, dir, dist, ~0, QueryTriggerInteraction.Ignore);
            float nearest = dist;
            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i].collider;
                if (col == null) continue;
                if (col.GetComponentInParent<DroneRuntime>() != null) continue; // self / other drones
                if (IsPlayerRig(col.transform)) continue;                       // the player
                if (hits[i].distance < nearest) nearest = hits[i].distance;
            }
            if (nearest < dist) return from + dir * Mathf.Max(0f, nearest - r);
            return to;
        }

        private void FindPlayer()
        {
            if (_receiver == null) _receiver = FindObjectOfType<PlayerStunReceiver>();
            if (_receiver != null) { _player = _receiver.Head; return; }
            if (Camera.main != null) _player = Camera.main.transform;
        }

        private bool HasLineOfSight()
        {
            Vector3 a = transform.position;
            Vector3 b = _player.position;
            Vector3 dir = (b - a);
            float d = dir.magnitude;
            if (d < 0.01f) return true;
            a += dir.normalized * 0.4f; // start past our own collider
            if (Physics.Linecast(a, b, out var hit, lineOfSightMask, QueryTriggerInteraction.Ignore))
                return IsPlayerRig(hit.collider.transform);
            return true;
        }

        private static bool IsPlayerRig(Transform t)
        {
            while (t != null)
            {
                if (t.name == "XR Origin") return true;
                t = t.parent;
            }
            return false;
        }

        private void FireBolt()
        {
            if (_player == null) return;
            Vector3 origin = transform.position;
            Vector3 dir = (_player.position - origin).normalized;
            var go = new GameObject("StunBolt");
            go.transform.position = origin + dir * 0.5f;
            var bolt = go.AddComponent<StunBolt>();
            bolt.Init(dir * boltSpeed, stunSeconds, slowFactor);
        }

        private void UpdateTelegraph()
        {
            float p = _fsm.Phase == DroneCombatPhase.Telegraph ? _fsm.TelegraphProgress : 0f;
            if (p <= 0f) { ClearTelegraph(); return; }

            if (_telegraphFx == null)
            {
                _telegraphFx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _telegraphFx.name = "__Telegraph";
                var col = _telegraphFx.GetComponent<Collider>();
                if (col != null) Destroy(col);
                _telegraphFx.transform.SetParent(transform, false);
                _telegraphFx.transform.localPosition = Vector3.forward * 0.4f;
                _telegraphRenderer = _telegraphFx.GetComponent<Renderer>();
                if (_telegraphRenderer != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Unlit");
                    if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                    var mat = new Material(shader);
                    mat.color = telegraphColor;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", telegraphColor);
                    _telegraphRenderer.material = mat;
                    _telegraphRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            float s = Mathf.Lerp(0.05f, 0.35f, p);
            _telegraphFx.transform.localScale = Vector3.one * s;
        }

        private void ClearTelegraph()
        {
            if (_telegraphFx != null) Destroy(_telegraphFx);
            _telegraphFx = null;
            _telegraphRenderer = null;
        }
    }
}

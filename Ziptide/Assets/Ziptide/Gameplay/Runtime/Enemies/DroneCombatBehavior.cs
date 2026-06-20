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
        public float standoffDistance = 5f;
        public float orbitSpeed = 40f;
        public float verticalBob = 0.3f;
        public float patrolRadius = 3f;
        public float patrolSpeed = 20f;
        public float moveLerp = 2.5f;

        [Header("Attack")]
        public float telegraphSeconds = 0.9f;
        public float boltCooldown = 2.5f;
        public float boltSpeed = 6f;
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
        private GameObject _telegraphFx;
        private Renderer _telegraphRenderer;

        private void Awake()
        {
            _drone = GetComponent<DroneRuntime>();
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
                desired.y = home.y + Mathf.Sin(Time.time) * verticalBob;
            }
            else
            {
                _orbitAngle += orbitSpeed * dt * Mathf.Deg2Rad;
                Vector3 center = _player.position;
                Vector3 ring = new Vector3(Mathf.Cos(_orbitAngle), 0f, Mathf.Sin(_orbitAngle)) * standoffDistance;
                desired = center + ring;
                desired.y = home.y + Mathf.Sin(Time.time * 1.5f) * verticalBob;
                // face the player
                Vector3 look = center - transform.position; look.y = 0f;
                if (look.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), dt * 4f);
            }

            transform.position = Vector3.Lerp(transform.position, desired, dt * moveLerp);
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

using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Drone Combat V1 — existing combat FSM, movement, collision, LoS and projectile truth plus a
    /// persistent readable threat presentation. No attack timing, damage, stun or decision rule lives here.
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
        [Tooltip("Max distance the drone will stray from its home spot — keeps it in its open zone instead of chasing through buildings.")]
        public float leashRadius = 9f;

        [Header("Attack")]
        public float telegraphSeconds = 1.0f;
        public float boltCooldown = 2.8f;
        public float boltSpeed = 6.5f;
        public float stunSeconds = 1.2f;
        [Range(0.1f, 1f)] public float slowFactor = 0.45f;
        public Color telegraphColor = new Color(0.3f, 0.9f, 1f);

        [Tooltip("Optional data variant. Overrides serialized values when assigned.")]
        public DroneCombatProfile profile;

        private DroneRuntime _drone;
        private readonly DroneCombatState _fsm = new DroneCombatState();
        private Transform _player;
        private PlayerStunReceiver _receiver;
        private float _orbitAngle;
        private float _phase;
        private GameObject _telegraphFx;
        private Renderer _telegraphRenderer;
        private LineRenderer _aimLine;
        private Material _threatMaterial;
        private DroneCombatPhase _lastLoggedPhase;
        private bool _phaseLogPrimed;
        private float _shotFlashUntil;

        private void Awake()
        {
            _drone = GetComponent<DroneRuntime>();
            _phase = (GetInstanceID() & 0x3FF) * 0.123f;
            ApplyProfile();
            _fsm.DetectRange = detectRange;
            _fsm.LoseRange = loseRange;
            _fsm.TelegraphSeconds = telegraphSeconds;
            _fsm.BoltCooldown = boltCooldown;
            EnsureThreatPresentation();
            HideThreatPresentation();
        }

        private void OnEnable()
        {
            if (_drone != null) _drone.CombatDriven = true;
        }

        private void OnDisable()
        {
            HideThreatPresentation();
        }

        private void OnDestroy()
        {
            // Presentation objects and their native material are owned exclusively by this behaviour.
            // Explicit teardown keeps scene reloads and test fixture destruction from leaking resources.
            DestroyOwned(_aimLine != null ? _aimLine.gameObject : null);
            DestroyOwned(_telegraphFx);
            DestroyOwned(_threatMaterial);
            _aimLine = null;
            _telegraphFx = null;
            _telegraphRenderer = null;
            _threatMaterial = null;
        }

        private static void DestroyOwned(Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) Object.Destroy(value);
            else Object.DestroyImmediate(value);
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
            float distance = _player != null ? Vector3.Distance(transform.position, _player.position) : 9999f;
            bool lineOfSight = _player != null && HasLineOfSight();

            _fsm.Tick(dt, distance, lineOfSight, active);
            LogPhaseTransition();

            if (!active)
            {
                HideThreatPresentation();
                return;
            }

            Move(dt);
            UpdateThreatPresentation();
            if (_fsm.FireRequested) FireBolt();
        }

        private void Move(float dt)
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
                _orbitAngle += orbitSpeed * dt * Mathf.Deg2Rad
                    * (1f + 0.35f * Mathf.Sin(Time.time * 0.6f + _phase));
                Vector3 center = _player.position;
                float breathe = standoffDistance + Mathf.Sin(Time.time * 0.5f + _phase);
                desired = center + new Vector3(Mathf.Cos(_orbitAngle), 0f, Mathf.Sin(_orbitAngle)) * breathe;
                desired.y = home.y + Mathf.Sin(Time.time * 1.5f + _phase) * verticalBob;
                Vector3 look = center - transform.position;
                look.y = 0f;
                if (look.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), dt * 4f);
            }

            Vector3 offset = desired - home;
            if (offset.magnitude > leashRadius) desired = home + offset.normalized * leashRadius;
            Vector3 next = Vector3.Lerp(transform.position, desired, dt * moveLerp);
            transform.position = CollideMove(transform.position, next);
        }

        private Vector3 CollideMove(Vector3 from, Vector3 to)
        {
            Vector3 delta = to - from;
            float distance = delta.magnitude;
            if (distance < 0.0001f) return to;
            Vector3 direction = delta / distance;
            const float radius = 0.22f;
            var hits = Physics.SphereCastAll(from, radius, direction, distance, ~0, QueryTriggerInteraction.Ignore);
            float nearest = distance;
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null) continue;
                if (collider.GetComponentInParent<DroneRuntime>() != null) continue;
                if (IsPlayerRig(collider.transform)) continue;
                if (hits[i].distance < nearest) nearest = hits[i].distance;
            }
            return nearest < distance ? from + direction * Mathf.Max(0f, nearest - radius) : to;
        }

        private void FindPlayer()
        {
            if (_receiver == null) _receiver = FindObjectOfType<PlayerStunReceiver>();
            if (_receiver != null) { _player = _receiver.Head; return; }
            if (Camera.main != null) _player = Camera.main.transform;
        }

        private bool HasLineOfSight()
        {
            Vector3 from = transform.position;
            Vector3 to = _player.position;
            Vector3 direction = to - from;
            float distance = direction.magnitude;
            if (distance < 0.01f) return true;
            from += direction.normalized * 0.4f;
            if (Physics.Linecast(from, to, out var hit, lineOfSightMask, QueryTriggerInteraction.Ignore))
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
            Vector3 direction = (_player.position - origin).normalized;
            var go = new GameObject("StunBolt");
            go.transform.position = origin + direction * 0.5f;
            var bolt = go.AddComponent<StunBolt>();
            bolt.Init(direction * boltSpeed, stunSeconds, slowFactor);
            _shotFlashUntil = Time.time + 0.07f;
            Debug.Log("ZIPTIDE: DRONE_SHOT instance=" + GetInstanceID()
                + " speed=" + boltSpeed.ToString("F1")
                + " stun=" + stunSeconds.ToString("F1"));
        }

        private void UpdateThreatPresentation()
        {
            bool flash = Time.time < _shotFlashUntil;
            bool telegraphing = _fsm.Phase == DroneCombatPhase.Telegraph;
            DroneThreatPresentation presentation = DroneThreatPresentationCore.Resolve(
                _fsm.TelegraphProgress, telegraphing, flash);
            if (!presentation.Visible)
            {
                HideThreatPresentation();
                return;
            }

            EnsureThreatPresentation();
            _telegraphFx.SetActive(true);
            _telegraphFx.transform.localScale = Vector3.one * presentation.OrbScale;
            Color color = flash ? Color.white : telegraphColor;
            if (_threatMaterial != null)
            {
                if (_threatMaterial.HasProperty("_BaseColor")) _threatMaterial.SetColor("_BaseColor", color);
                else if (_threatMaterial.HasProperty("_Color")) _threatMaterial.SetColor("_Color", color);
                if (_threatMaterial.HasProperty("_EmissionColor"))
                    _threatMaterial.SetColor("_EmissionColor", color * presentation.Intensity);
            }

            bool showLine = presentation.ShowAimLine && _player != null;
            _aimLine.enabled = showLine;
            if (showLine)
            {
                Vector3 from = transform.position + transform.forward * 0.42f;
                _aimLine.startWidth = presentation.LineWidth;
                _aimLine.endWidth = presentation.LineWidth * 0.35f;
                _aimLine.SetPosition(0, from);
                _aimLine.SetPosition(1, _player.position);
            }
        }

        private void EnsureThreatPresentation()
        {
            if (_telegraphFx != null && _aimLine != null) return;
            if (_threatMaterial == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                _threatMaterial = new Material(shader) { name = "DroneThreatPresentation" };
                if (_threatMaterial.HasProperty("_BaseColor"))
                    _threatMaterial.SetColor("_BaseColor", telegraphColor);
                if (_threatMaterial.HasProperty("_EmissionColor"))
                {
                    _threatMaterial.EnableKeyword("_EMISSION");
                    _threatMaterial.SetColor("_EmissionColor", telegraphColor);
                }
            }

            if (_telegraphFx == null)
            {
                _telegraphFx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _telegraphFx.name = "__ThreatTelegraph";
                Collider col = _telegraphFx.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                _telegraphFx.transform.SetParent(transform, false);
                _telegraphFx.transform.localPosition = Vector3.forward * 0.4f;
                _telegraphRenderer = _telegraphFx.GetComponent<Renderer>();
                if (_telegraphRenderer != null)
                {
                    _telegraphRenderer.sharedMaterial = _threatMaterial;
                    _telegraphRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            if (_aimLine == null)
            {
                var line = new GameObject("__ThreatAimLine");
                line.transform.SetParent(transform, false);
                _aimLine = line.AddComponent<LineRenderer>();
                _aimLine.useWorldSpace = true;
                _aimLine.positionCount = 2;
                _aimLine.sharedMaterial = _threatMaterial;
                _aimLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        private void HideThreatPresentation()
        {
            if (_telegraphFx != null) _telegraphFx.SetActive(false);
            if (_aimLine != null) _aimLine.enabled = false;
        }

        private void LogPhaseTransition()
        {
            if (_phaseLogPrimed && _lastLoggedPhase == _fsm.Phase) return;
            DroneCombatPhase previous = _phaseLogPrimed ? _lastLoggedPhase : _fsm.Phase;
            _phaseLogPrimed = true;
            _lastLoggedPhase = _fsm.Phase;
            Debug.Log("ZIPTIDE: DRONE_PHASE instance=" + GetInstanceID()
                + " from=" + previous + " to=" + _fsm.Phase);
        }
    }
}

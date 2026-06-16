using System.Collections;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Hovering drone — enemy #1 and the reusable template (see docs/systems/CREATURE_DRONE.md).
    /// When tased, it shows a visible electric shock + seizes briefly, THEN goes down with physics
    /// that depend on WHERE it was hit (via <see cref="HitZones"/>): center = straight drop, top =
    /// nose-down plunge, sides = spin out, etc. Pistol/hitscan (no stick point) uses the center
    /// reaction. Tunable fields below let us spin up drone "subsets" (look / behavior / intensity)
    /// off the same base.
    /// </summary>
    public class DroneRuntime : MonoBehaviour, IShockable
    {
        private enum State { Active, Shocked, Dead }

        [Header("Idle motion")]
        [SerializeField] private float bobAmplitude = 0.15f;
        [SerializeField] private float bobSpeed = 1.5f;
        [SerializeField] private float spinSpeed = 30f;

        [Header("Look")]
        [SerializeField] private Color activeColor = new Color(0.8f, 0.2f, 0.1f);
        [SerializeField] private Color deadColor = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color shockColor = new Color(0.3f, 0.9f, 1f);

        [Header("Variant tuning (subsets share this base)")]
        [Tooltip("Can the taser stun/down it?")]
        [SerializeField] private bool canShock = true;
        [Tooltip("How long the visible electric seize lasts before it goes down.")]
        [SerializeField] private float shockSeconds = 0.4f;
        [Tooltip("Multiplier on death forces/torque — dial drama up or down per variant.")]
        [SerializeField] private float intensity = 1f;
        [Tooltip("Local-space radius treated as a dead-center core hit.")]
        [SerializeField] private float centerRadius = 0.08f;

        private State _state = State.Active;
        private Vector3 _homePos;
        private Renderer _renderer;
        private Material _mat;
        private TargetRuntime _target;

        public static event System.Action<DroneRuntime> OnDroneDisabled;

        private void Awake()
        {
            _homePos = transform.position;
            _renderer = GetComponentInChildren<Renderer>();
            if (_renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    _mat = new Material(shader);
                    SetColor(activeColor);
                    _renderer.material = _mat;
                    _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            // Die from the pistol/hitscan too: a TargetRuntime hit kills the drone (center reaction).
            _target = GetComponent<TargetRuntime>();
            if (_target != null)
                _target.OnHit.AddListener(Kill);
        }

        private void OnDestroy()
        {
            if (_target != null) _target.OnHit.RemoveListener(Kill);
        }

        private void Update()
        {
            if (_state != State.Active) return;
            float y = _homePos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.position = new Vector3(_homePos.x, y, _homePos.z);
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        }

        // ── Hit entry points ────────────────────────────────────────────────

        /// <summary>IShockable (generic taser). No stick point → react from the center.</summary>
        public void Shock(float seconds) => RegisterHit(transform.position, true);

        /// <summary>
        /// Primary hit handler. <paramref name="worldPoint"/> is where the dart stuck. Tased hits play
        /// the visible shock seize then a location-based go-down; non-tased go down immediately.
        /// </summary>
        public void RegisterHit(Vector3 worldPoint, bool taser)
        {
            if (_state != State.Active) return; // already shocked or dead
            HitZone zone = HitZones.Classify(transform, worldPoint, centerRadius);
            if (taser && canShock && shockSeconds > 0f)
                StartCoroutine(ShockThenDown(zone, worldPoint));
            else
                GoDown(zone);
        }

        /// <summary>Back-compat parameterless kill (pistol/OnHit path): center reaction, no shock.</summary>
        public void Kill()
        {
            if (_state == State.Dead) return;
            GoDown(HitZone.Center);
        }

        // ── Shock VFX then death ────────────────────────────────────────────

        private IEnumerator ShockThenDown(HitZone zone, Vector3 hitPoint)
        {
            _state = State.Shocked;

            var fxGo = new GameObject("ShockFX");
            fxGo.transform.SetParent(transform, false);
            var light = fxGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = shockColor;
            light.range = 2.5f;

            float t = 0f;
            while (t < shockSeconds)
            {
                t += Time.deltaTime;
                SetColor(Color.Lerp(activeColor, shockColor, Random.value));   // strobe
                light.intensity = 2f + Random.value * 5f;                      // flicker
                transform.position = _homePos + Random.insideUnitSphere * 0.03f; // seize/jitter
                if (Random.value < 0.5f) SpawnArc(hitPoint);
                yield return null;
            }

            if (fxGo != null) Destroy(fxGo);
            GoDown(zone);
        }

        /// <summary>A short emissive "arc" segment flicked out from the body — cheap code-only spark.</summary>
        private void SpawnArc(Vector3 from)
        {
            var arc = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arc.name = "Arc";
            var col = arc.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Vector3 dir = Random.onUnitSphere;
            float len = 0.15f + Random.value * 0.25f;
            arc.transform.position = transform.position + dir * (len * 0.5f);
            arc.transform.rotation = Quaternion.LookRotation(dir);
            arc.transform.localScale = new Vector3(0.015f, 0.015f, len);

            var r = arc.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    mat.color = shockColor;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", shockColor);
                    r.material = mat;
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            Destroy(arc, 0.06f);
        }

        /// <summary>Go down with physics that read as a failure from the hit zone.</summary>
        private void GoDown(HitZone zone)
        {
            if (_state == State.Dead) return;
            _state = State.Dead;
            SetColor(deadColor);
            Debug.Log("ZIPTIDE: DRONE_DOWN name=" + gameObject.name + " zone=" + zone);
            OnDroneDisabled?.Invoke(this);

            if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
                gameObject.AddComponent<BoxCollider>();

            var rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Vector3 fwd = transform.forward, right = transform.right;
            float k = Mathf.Max(0.1f, intensity);
            Vector3 force, torque;
            switch (zone)
            {
                case HitZone.Top:    force = (fwd * 1.5f + Vector3.down) * k; torque = right * 6f * k; break;   // nose-down plunge
                case HitZone.Bottom: force = Vector3.up * 2.5f * k;          torque = Random.insideUnitSphere * 3f * k; break; // pop then flop
                case HitZone.Front:  force = -fwd * 2.5f * k;                torque = right * -2f * k; break;   // recoil back
                case HitZone.Back:   force = fwd * 3f * k;                   torque = right * 4f * k; break;    // lurch forward, nose in
                case HitZone.Left:   force = -right * 2f * k;                torque = Vector3.up * 8f * k; break;  // spin out left
                case HitZone.Right:  force = right * 2f * k;                 torque = Vector3.up * -8f * k; break; // spin out right
                default:             force = Vector3.down * 2f * k;          torque = Random.insideUnitSphere * 0.5f; break; // center: clean drop
            }
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
            rb.WakeUp();
        }

        private void SetColor(Color c)
        {
            if (_mat == null) return;
            if (_mat.HasProperty("_BaseColor")) _mat.SetColor("_BaseColor", c);
            else if (_mat.HasProperty("_Color")) _mat.SetColor("_Color", c);
        }
    }
}

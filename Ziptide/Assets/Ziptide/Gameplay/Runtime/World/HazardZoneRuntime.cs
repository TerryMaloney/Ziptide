using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A biome hazard volume (GAME_PLAN M2), spawned by the world generator from the layout's
    /// <see cref="HazardZoneDef"/> data. Non-lethal per canon: Wind pushes, Static zaps (flash + brief
    /// slow ticks), Flood drags, Spore fogs (longer slow), Radiation escalates and shoves you back out.
    /// Player detection is a cheap poll against the rig position (same style as JobDirector's marker
    /// check) — no collider/rigidbody coupling with the rig. Slows reuse PlayerStunReceiver.ApplyStun
    /// (rig-ensured), so hazards stack safely with combat stuns and can never latch (the M1 self-heal).
    /// Logs ZIPTIDE: HAZARD id=… kind=… enter/exit.
    /// </summary>
    public class HazardZoneRuntime : MonoBehaviour
    {
        // SERIALIZED def — the generator assigns this at EDIT time and it must survive into the saved
        // scene (gotcha #7: only serialized state crosses the edit→runtime boundary). Visual + bounds
        // are built at RUNTIME in Awake, like DroneRuntime.
        [SerializeField] private HazardZoneDef def = new HazardZoneDef();

        private PlayerRigPersistence _rig;
        private PlayerStunReceiver _stun;
        private Bounds _bounds;
        private bool _inside;
        private float _tickTimer;
        private float _insideSeconds;
        private HazardZoneDef _def => def;

        private static Color KindColor(HazardKind k)
        {
            switch (k)
            {
                case HazardKind.Wind: return new Color(0.75f, 0.85f, 0.95f, 1f);
                case HazardKind.Static: return new Color(0.55f, 0.70f, 1.00f, 1f);
                case HazardKind.Flood: return new Color(0.20f, 0.45f, 0.60f, 1f);
                case HazardKind.Spore: return new Color(0.55f, 0.65f, 0.25f, 1f);
                default: return new Color(0.80f, 0.60f, 0.20f, 1f); // Radiation
            }
        }

        /// <summary>Assign the def (EDIT time, from the generator) — only the serialized field, no scene work.</summary>
        public void Init(HazardZoneDef d)
        {
            def = d ?? new HazardZoneDef();
        }

        private void Awake()
        {
            _bounds = new Bounds(transform.position + Vector3.up * (_def.size.y * 0.5f), _def.size);
            BuildVisual();
        }

        private void BuildVisual()
        {
            // A thin tinted floor slab marks the zone (graybox read; VFX at the M6 art pass).
            var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = "HazardSlab";
            var col = slab.GetComponent<Collider>();
            if (col != null) Destroy(col); // marker only — never blocks movement
            slab.transform.SetParent(transform, false);
            slab.transform.localPosition = new Vector3(0f, 0.03f, 0f);
            slab.transform.localScale = new Vector3(_def.size.x, 0.05f, _def.size.z);
            var r = slab.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    Color c = KindColor(_def.kind);
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                    else if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
                    r.sharedMaterial = mat;
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }

        private void Update()
        {
            if (_def == null) return;
            if (_rig == null)
            {
                _rig = FindObjectOfType<PlayerRigPersistence>();
                if (_rig == null) return;
            }
            if (_stun == null) _stun = FindObjectOfType<PlayerStunReceiver>();

            bool inside = _bounds.Contains(_rig.transform.position + Vector3.up * 0.5f);
            if (inside != _inside)
            {
                _inside = inside;
                _insideSeconds = 0f;
                _tickTimer = 0f;
                Debug.Log("ZIPTIDE: HAZARD id=" + _def.id + " kind=" + _def.kind + (inside ? " enter" : " exit"));
            }
            if (!inside) return;

            _insideSeconds += Time.deltaTime;
            switch (_def.kind)
            {
                case HazardKind.Wind:
                {
                    Vector3 dir = _def.direction.sqrMagnitude > 0.001f
                        ? new Vector3(_def.direction.x, 0f, _def.direction.z).normalized
                        : Vector3.right;
                    // Gentle, continuous shove on the rig root (transform-level, like the fall-safety
                    // teleports — the CharacterController only owns its OWN Move calls).
                    _rig.transform.position += dir * _def.strength * Time.deltaTime;
                    break;
                }
                case HazardKind.Static:
                    Tick(1.5f, () => _stun?.ApplyStun(0.3f, 0.6f));
                    break;
                case HazardKind.Flood:
                    // Refresh a heavy slow while wading — clears itself moments after you climb out.
                    Tick(0.4f, () => _stun?.ApplyStun(0.6f, 0.35f));
                    break;
                case HazardKind.Spore:
                    Tick(2.5f, () => _stun?.ApplyStun(1.0f, 0.5f));
                    break;
                case HazardKind.Radiation:
                {
                    // Flash faster the longer you linger, then shove back toward the edge.
                    float period = Mathf.Max(0.4f, 2.0f - _insideSeconds * 0.25f * _def.strength);
                    Tick(period, () => _stun?.ApplyStun(0.25f, 0.7f));
                    if (_insideSeconds > 4f)
                    {
                        Vector3 outDir = _rig.transform.position - _bounds.center;
                        outDir.y = 0f;
                        if (outDir.sqrMagnitude > 0.001f)
                            _rig.transform.position += outDir.normalized * 0.8f * Time.deltaTime;
                    }
                    break;
                }
            }
        }

        private void Tick(float period, System.Action effect)
        {
            _tickTimer -= Time.deltaTime;
            if (_tickTimer > 0f) return;
            _tickTimer = period;
            effect?.Invoke();
        }
    }
}

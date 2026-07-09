using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A4.5 — the rig-side augment brain (auto-ensured, the ClimbCoordinator idiom). Owns the pure
    /// <see cref="AugmentLoadout"/> (1 active + 1 passive) and the active's <see cref="AugmentClock"/>;
    /// publishes passive effects through the pure <see cref="AugmentEffects"/> hooks (reset + reapply
    /// on every equip — no stale hooks); triggers the ACTIVE diegetically: a belt orb floating at the
    /// left hip that you TOUCH-SELECT with either ray — no new input bindings, kid-readable, its fill
    /// ring is the cooldown. Active behaviors: SURGE DASH (comfort burst along flat gaze — the hop's
    /// big sibling), BUBBLE GUARD (a real collider shell — bolts/darts physically can't pass),
    /// OVERCLOCK (weapon cooldowns scaled for the duration via the same hook the passives use).
    /// </summary>
    public class AugmentController : MonoBehaviour
    {
        private static AugmentController _instance;

        private readonly AugmentLoadout _loadout = new AugmentLoadout();
        private AugmentDefinition _activeDef;
        private AugmentDefinition _passiveDef;
        private AugmentClock _clock;

        private Transform _rig;
        private Transform _cam;
        private Transform _orb;
        private Renderer _orbRenderer;
        private GameObject _bubble;
        private float _dashT = -1f;
        private Vector3 _dashFrom, _dashTo;
        private float _overclockUntil = -1f;

        private const float DashSeconds = 0.22f;   // the comfort-burst window (dodge-step reads)

        public static AugmentController Ensure()
        {
            if (_instance != null) return _instance;
            var go = new GameObject("__AugmentController");
            _instance = go.AddComponent<AugmentController>();
            return _instance;
        }

        /// <summary>Equip an augment definition (from a pickup/pad). Returns the displaced id.</summary>
        public string Equip(AugmentDefinition def)
        {
            if (def == null || string.IsNullOrEmpty(def.effectId)) return null;
            string displaced = _loadout.Equip(def.augmentKind, def.effectId);
            if (def.augmentKind == AugmentKind.Active)
            {
                _activeDef = def;
                _clock = new AugmentClock(def.cooldownSeconds, def.durationSeconds);
            }
            else _passiveDef = def;

            // No stale hooks: wipe, then republish whatever is CURRENTLY equipped.
            AugmentEffects.ResetAll();
            ApplyPassiveHooks(_passiveDef);
            Debug.Log("ZIPTIDE: AUGMENT_EQUIP id=" + def.effectId + " kind=" + def.augmentKind +
                      (displaced != null ? " displaced=" + displaced : ""));
            return displaced;
        }

        private static void ApplyPassiveHooks(AugmentDefinition def)
        {
            if (def == null) return;
            switch (def.effectId)
            {
                case "sure_step": AugmentEffects.SlowResistance = Mathf.Clamp01(def.magnitude); break;
                case "sixth_sense": AugmentEffects.LocatorCooldownScale = Mathf.Clamp(def.magnitude, 0.1f, 1f); break;
                case "magnet_palm": AugmentEffects.MagnetReachMeters = Mathf.Max(0f, def.magnitude); break;
            }
        }

        private void Update()
        {
            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                _rig = rig != null ? rig.transform : null;
                if (_rig == null) return;
                var cam = _rig.GetComponentInChildren<Camera>();
                _cam = cam != null ? cam.transform : _rig;
            }

            EnsureOrb();
            TickDash();
            TickOverclock();
            TickBubble();
            TickMagnet();
        }

        // ── The belt orb (the diegetic trigger + cooldown readout) ───────────
        private void EnsureOrb()
        {
            if (_orb != null || _cam == null) return;
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "__AugmentOrb";
            go.transform.localScale = Vector3.one * 0.09f;
            var col = go.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;   // keep the collider — it's the select target
            _orbRenderer = go.GetComponent<Renderer>();
            var grab = go.AddComponent<XRSimpleInteractable>();
            grab.selectEntered.AddListener(_ => TryActivate());
            _orb = go.transform;
        }

        private void LateUpdate()
        {
            if (_orb == null || _cam == null) return;
            // Right-hip station: forward-right-down of the gaze root, following lazily (belt language).
            Vector3 target = _cam.position + _cam.forward * 0.18f + _cam.right * 0.28f - _cam.up * 0.45f;
            _orb.position = Vector3.Lerp(_orb.position, target, Time.deltaTime * 8f);

            if (_orbRenderer != null)
            {
                Color baseC = _activeDef != null ? _activeDef.gemColor : new Color(0.25f, 0.25f, 0.3f);
                float fill = _clock != null ? _clock.ReadyProgress(Time.time) : 0f;
                var mat = _orbRenderer.material;
                Color c = baseC * (0.35f + 0.65f * fill); // dark while cooling, full-bright when ready
                if (_clock != null && _clock.IsActive(Time.time)) c = Color.white; // effect LIVE
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                else mat.color = c;
            }
        }

        private void TryActivate()
        {
            if (_activeDef == null || _clock == null || _rig == null) return;
            if (!_clock.TryActivate(Time.time))
            {
                Debug.Log("ZIPTIDE: AUGMENT_COOLING id=" + _activeDef.effectId);
                return;
            }
            Debug.Log("ZIPTIDE: AUGMENT_FIRE id=" + _activeDef.effectId);
            switch (_activeDef.effectId)
            {
                case "surge_dash":
                    Vector3 dir = _cam != null ? _cam.forward : _rig.forward;
                    dir.y = 0f;
                    dir = dir.sqrMagnitude > 0.01f ? dir.normalized : Vector3.forward;
                    _dashFrom = _rig.position;
                    _dashTo = _rig.position + dir * Mathf.Max(1f, _activeDef.magnitude);
                    _dashT = 0f;
                    break;
                case "bubble_guard":
                    SpawnBubble();
                    break;
                case "overclock":
                    AugmentEffects.WeaponCooldownScale = Mathf.Clamp(_activeDef.magnitude, 0.1f, 1f);
                    _overclockUntil = Time.time + _activeDef.durationSeconds;
                    break;
            }
        }

        // ── Active behaviors ─────────────────────────────────────────────────
        private void TickDash()
        {
            if (_dashT < 0f || _rig == null) return;
            _dashT += Time.deltaTime / DashSeconds;
            float k = Mathf.Clamp01(_dashT);
            // Ease-out: fast leave, soft land — the comfort-burst curve.
            float e = 1f - (1f - k) * (1f - k);
            _rig.position = Vector3.Lerp(_dashFrom, _dashTo, e);
            if (_dashT >= 1f) _dashT = -1f;
        }

        private void SpawnBubble()
        {
            if (_bubble != null) Destroy(_bubble);
            _bubble = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _bubble.name = "__BubbleGuard";
            _bubble.transform.localScale = Vector3.one * 2.4f;
            // A REAL collider shell: incoming bolts/darts physically stop on it (their wall checks
            // and collisions already respect colliders — no per-weapon code).
            var col = _bubble.GetComponent<SphereCollider>();
            if (col != null) col.isTrigger = false;
            var r = _bubble.GetComponent<Renderer>();
            if (r != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader != null)
                {
                    var mat = new Material(shader);
                    mat.color = new Color(0.5f, 0.8f, 1f, 0.28f);
                    r.material = mat;
                }
            }
        }

        private void TickBubble()
        {
            if (_bubble == null) return;
            if (_rig != null) _bubble.transform.position = _rig.position + Vector3.up * 1.2f;
            if (_clock == null || !_clock.IsActive(Time.time))
            {
                Destroy(_bubble);
                _bubble = null;
            }
        }

        private void TickOverclock()
        {
            if (_overclockUntil < 0f) return;
            if (Time.time >= _overclockUntil)
            {
                _overclockUntil = -1f;
                AugmentEffects.WeaponCooldownScale = 1f;
                ApplyPassiveHooks(_passiveDef); // in case a passive also publishes (it doesn't today — hygiene)
            }
        }

        // ── Magnet Palm (passive — pickups drift to you) ─────────────────────
        private float _nextMagnetScan;
        private void TickMagnet()
        {
            float reach = AugmentEffects.MagnetReachMeters;
            if (reach <= 0f || _rig == null || Time.time < _nextMagnetScan) return;
            _nextMagnetScan = Time.time + 0.2f; // 5Hz scan, gentle per-frame-free pull
            Vector3 hand = _cam != null
                ? _cam.position + _cam.forward * 0.3f - _cam.up * 0.2f
                : _rig.position + Vector3.up * 1.2f;
            foreach (var hit in Physics.OverlapSphere(hand, reach, ~0, QueryTriggerInteraction.Ignore))
            {
                var item = hit.GetComponentInParent<ItemRuntime>();
                if (item == null) continue;
                var rb = item.GetComponent<Rigidbody>();
                if (rb == null || rb.isKinematic) continue;
                Vector3 pull = (hand - item.transform.position);
                if (pull.magnitude < 0.4f) continue;      // close enough — let the grab take it
                rb.velocity = Vector3.Lerp(rb.velocity, pull.normalized * 2.2f, 0.5f);
            }
        }
    }
}

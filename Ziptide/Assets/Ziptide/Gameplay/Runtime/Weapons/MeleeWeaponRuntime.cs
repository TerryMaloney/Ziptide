using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE MELEE PAIR (MP100 wave 1, 2026-07-06) — the game's first TRUE contact melee. Unlike the
    /// Sonic Thumper (a swing-gated positional AoE pulse), these only hit what the swing actually
    /// reaches:
    ///  · BREAKER BLADE — fast 1H swings; while the tip moves faster than PvpRules.MeleeSwingSpeed,
    ///    a small contact sphere at the tip deals BreakerBladeDamage to each target at most once per
    ///    BladeContactDebounce. Sustained pressure, light hits, all the risk of being in reach.
    ///    Bonus: a swing that reaches a BreakableWall cracks it (the hammer synergy).
    ///  · TIDE PIKE — committed thrusts; when the shaft's velocity along its own forward axis beats
    ///    PikeThrustSpeed, a ray of PikeReach fires from the tip and the FIRST target takes
    ///    TidePikeDamage + a shove, then PikeThrustDebounce of recovery. Reach and burst, no spam.
    /// Counters (design law: every weapon has a visible counter): both need you inside their reach —
    /// range beats them; the pike's thrust is a straight line — sidestep it.
    /// Hit detection reuses HammerTool/SonicThumper's proven tracked-point velocity idiom (transform
    /// position delta, NOT Rigidbody.velocity — VelocityTracking grabs don't report it reliably).
    /// Every triggered swing reports to <see cref="PvpNoise"/> so bots can hear and investigate.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class MeleeWeaponRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Transform _tip;                 // the business end (child "Muzzle" from ItemFactory)
        private Vector3 _lastTipPos;
        private float _nextThrustAt;
        private float _nextWallHitAt;
        private readonly Dictionary<Transform, float> _nextHitAt = new Dictionary<Transform, float>();

        private ArenaWeaponDefinition Def
        {
            get
            {
                var item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as ArenaWeaponDefinition : null;
            }
        }

        private bool IsPike => Def != null && Def.kind == ArenaWeaponKind.TidePike;

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _tip = transform.Find("Muzzle");
            if (_tip == null) _tip = transform;
            _lastTipPos = _tip.position;
        }

        private void Update()
        {
            Vector3 tipPos = _tip.position;
            Vector3 tipVel = Time.deltaTime > 1e-4f ? (tipPos - _lastTipPos) / Time.deltaTime : Vector3.zero;
            _lastTipPos = tipPos;

            bool held = _grab != null && _grab.isSelected;
            if (!held) return;

            if (IsPike) TickPike(tipPos, tipVel);
            else TickBlade(tipPos, tipVel);
        }

        // ── Breaker Blade: continuous contact while genuinely swinging ────────────────────────────
        private void TickBlade(Vector3 tipPos, Vector3 tipVel)
        {
            if (tipVel.magnitude < (float)PvpRules.MeleeSwingSpeed) return;
            PvpNoise.Report(tipPos);

            var hits = Physics.OverlapSphere(tipPos, (float)PvpRules.BladeReach, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                var h = hits[i];
                if (h == null || h.transform.root == transform.root) continue;

                var wall = h.GetComponentInParent<BreakableWall>();
                if (wall != null && Time.time >= _nextWallHitAt)
                {
                    wall.HitFromHammer(h.ClosestPoint(tipPos));
                    _nextWallHitAt = Time.time + (float)PvpRules.BladeContactDebounce;
                    continue;
                }

                var pvp = h.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue;   // never cut yourself
                Transform key = h.transform.root;
                if (_nextHitAt.TryGetValue(key, out float nextAt) && Time.time < nextAt) continue;
                _nextHitAt[key] = Time.time + (float)PvpRules.BladeContactDebounce * AugmentEffects.WeaponCooldownScale;

                Vector3 dir = tipVel.sqrMagnitude > 0.01f ? tipVel.normalized : transform.forward;
                PvpHitSource.Report(0);
                pvp.ReceiveHit(PvpWeapon.BreakerBlade, h.ClosestPoint(tipPos), dir);
                HitFlash(h.ClosestPoint(tipPos), new Color(0.55f, 0.9f, 1f));
                Debug.Log("ZIPTIDE: MELEE_HIT weapon=breaker_blade");
            }
        }

        // ── Tide Pike: one committed thrust, then recovery ────────────────────────────────────────
        private void TickPike(Vector3 tipPos, Vector3 tipVel)
        {
            if (Time.time < _nextThrustAt) return;
            float forwardSpeed = Vector3.Dot(tipVel, transform.forward);
            if (forwardSpeed < (float)PvpRules.PikeThrustSpeed) return;

            _nextThrustAt = Time.time + (float)PvpRules.PikeThrustDebounce * AugmentEffects.WeaponCooldownScale;
            PvpNoise.Report(tipPos);

            // The thrust is a LINE — that's its counter. First body on the line takes the hit.
            var rays = Physics.RaycastAll(tipPos, transform.forward, (float)PvpRules.PikeReach,
                                          ~0, QueryTriggerInteraction.Ignore);
            float best = float.MaxValue; RaycastHit bestHit = default; IPvpDamageable bestPvp = null;
            for (int i = 0; i < rays.Length; i++)
            {
                if (rays[i].transform.root == transform.root) continue;
                var pvp = rays[i].collider.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue;
                if (rays[i].distance < best) { best = rays[i].distance; bestHit = rays[i]; bestPvp = pvp; }
            }
            if (bestPvp == null) return;

            PvpHitSource.Report(0);
            bestPvp.ReceiveHit(PvpWeapon.TidePike, bestHit.point, transform.forward);
            var bot = bestHit.collider.GetComponentInParent<PvpBot>();
            if (bot != null) bot.transform.position += transform.forward * 0.75f; // the poke lands
            HitFlash(bestHit.point, new Color(0.35f, 0.75f, 0.8f));
            Debug.Log("ZIPTIDE: MELEE_HIT weapon=tide_pike dist=" + best.ToString("0.0"));
        }

        /// <summary>Cheap spawn-and-die contact spark so a landed hit READS (ThumpRing idiom).</summary>
        private static void HitFlash(Vector3 at, Color color)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = "MeleeHitFlash";
            var col = s.GetComponent<Collider>();
            if (col != null) Destroy(col);
            s.transform.position = at;
            s.transform.localScale = Vector3.one * 0.12f;
            ItemFactory.ApplyURPColor(s, color);
            s.AddComponent<MeleeFlashVisual>();
        }
    }

    /// <summary>Grows briefly and dies — a contact spark, not a persistent effect.</summary>
    public class MeleeFlashVisual : MonoBehaviour
    {
        private float _t;

        private void Update()
        {
            _t += Time.deltaTime / 0.18f;
            transform.localScale = Vector3.one * Mathf.Lerp(0.12f, 0.4f, Mathf.Clamp01(_t));
            if (_t >= 1f) Destroy(gameObject);
        }
    }
}

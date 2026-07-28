using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Contact melee pair. Breaker Blade uses swing-speed contact; Tide Pike uses a committed forward
    /// thrust. Socket selection is not treated as being held in a hand, so a sheathed weapon cannot
    /// attack merely because the belt moved.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class MeleeWeaponRuntime : MonoBehaviour
    {
        // Quest evidence: 90° X gave the right broad hold but erased the asset's 180° yaw, leaving the
        // blade backwards. Preserve definition yaw/roll, apply a small forward lean from vertical.
        private static readonly Vector3 BreakerBladeDeviceGripEuler = new Vector3(82f, 180f, 0f);

        private XRGrabInteractable _grab;
        private Transform _tip;
        private Vector3 _lastTipPos;
        private float _nextThrustAt;
        private float _nextWallHitAt;
        private readonly Dictionary<Transform, float> _nextHitAt = new Dictionary<Transform, float>();

        private ArenaWeaponDefinition Def
        {
            get
            {
                ItemRuntime item = GetComponent<ItemRuntime>();
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

        private void Start()
        {
            ArenaWeaponDefinition def = Def;
            if (def == null || def.kind != ArenaWeaponKind.BreakerBlade
                || _grab == null || _grab.attachTransform == null) return;

            Vector3 pose = BreakerBladeDeviceGripEuler;
            if (def.gripLocalEuler != Vector3.zero)
            {
                pose.y = def.gripLocalEuler.y;
                pose.z = def.gripLocalEuler.z;
            }
            _grab.attachTransform.localRotation = Quaternion.Euler(pose);
            Debug.Log("ZIPTIDE: MELEE_GRIP_POSE weapon=breaker_blade euler=" + pose.ToString("F0"));
        }

        private void Update()
        {
            Vector3 tipPos = _tip.position;
            Vector3 tipVel = Time.deltaTime > 1e-4f ? (tipPos - _lastTipPos) / Time.deltaTime : Vector3.zero;
            _lastTipPos = tipPos;

            if (!IsHeldByController()) return;
            if (IsPike) TickPike(tipPos, tipVel);
            else TickBlade(tipPos, tipVel);
        }

        private bool IsHeldByController()
        {
            if (_grab == null || !_grab.isSelected) return false;
            foreach (IXRSelectInteractor interactor in _grab.interactorsSelecting)
                if (interactor is XRBaseControllerInteractor) return true;
            return false;
        }

        private void TickBlade(Vector3 tipPos, Vector3 tipVel)
        {
            if (tipVel.magnitude < (float)PvpRules.MeleeSwingSpeed) return;
            PvpNoise.Report(tipPos);

            Collider[] hits = Physics.OverlapSphere(tipPos, (float)PvpRules.BladeReach,
                ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider h = hits[i];
                if (h == null || h.transform.root == transform.root) continue;

                BreakableWall wall = h.GetComponentInParent<BreakableWall>();
                if (wall != null && Time.time >= _nextWallHitAt)
                {
                    wall.HitFromHammer(h.ClosestPoint(tipPos));
                    _nextWallHitAt = Time.time + (float)PvpRules.BladeContactDebounce;
                    continue;
                }

                IPvpDamageable pvp = h.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue;
                Transform key = h.transform.root;
                if (_nextHitAt.TryGetValue(key, out float nextAt) && Time.time < nextAt) continue;
                _nextHitAt[key] = Time.time + (float)PvpRules.BladeContactDebounce
                    * AugmentEffects.WeaponCooldownScale;

                Vector3 dir = tipVel.sqrMagnitude > 0.01f ? tipVel.normalized : transform.forward;
                PvpHitSource.Report(0);
                pvp.ReceiveHit(PvpWeapon.BreakerBlade, h.ClosestPoint(tipPos), dir);
                HitFlash(h.ClosestPoint(tipPos), new Color(0.55f, 0.9f, 1f));
                Debug.Log("ZIPTIDE: MELEE_HIT weapon=breaker_blade");
            }
        }

        private void TickPike(Vector3 tipPos, Vector3 tipVel)
        {
            if (Time.time < _nextThrustAt) return;
            float forwardSpeed = Vector3.Dot(tipVel, transform.forward);
            if (forwardSpeed < (float)PvpRules.PikeThrustSpeed) return;

            _nextThrustAt = Time.time + (float)PvpRules.PikeThrustDebounce
                * AugmentEffects.WeaponCooldownScale;
            PvpNoise.Report(tipPos);

            RaycastHit[] rays = Physics.RaycastAll(tipPos, transform.forward, (float)PvpRules.PikeReach,
                ~0, QueryTriggerInteraction.Ignore);
            float best = float.MaxValue;
            RaycastHit bestHit = default;
            IPvpDamageable bestPvp = null;
            for (int i = 0; i < rays.Length; i++)
            {
                if (rays[i].transform.root == transform.root) continue;
                IPvpDamageable pvp = rays[i].collider.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue;
                if (rays[i].distance < best)
                {
                    best = rays[i].distance;
                    bestHit = rays[i];
                    bestPvp = pvp;
                }
            }
            if (bestPvp == null) return;

            PvpHitSource.Report(0);
            bestPvp.ReceiveHit(PvpWeapon.TidePike, bestHit.point, transform.forward);
            PvpBot bot = bestHit.collider.GetComponentInParent<PvpBot>();
            if (bot != null) bot.transform.position += transform.forward * 0.75f;
            HitFlash(bestHit.point, new Color(0.35f, 0.75f, 0.8f));
            Debug.Log("ZIPTIDE: MELEE_HIT weapon=tide_pike dist=" + best.ToString("0.0"));
        }

        private static void HitFlash(Vector3 at, Color color)
        {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = "MeleeHitFlash";
            Collider col = s.GetComponent<Collider>();
            if (col != null) Destroy(col);
            s.transform.position = at;
            s.transform.localScale = Vector3.one * 0.12f;
            ItemFactory.ApplyURPColor(s, color);
            s.AddComponent<MeleeFlashVisual>();
        }
    }

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

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Multiplayer;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// SONIC THUMPER (A4 — melee area shove): swing it fast while held and it detonates a shockwave —
    /// combatants in range take a hit + a shove, and breakable walls crack (the hammer synergy, one
    /// swing instead of aimed taps). Counter: it needs melee range — keep your distance.
    /// Mirrors HammerTool's proven swing detection (head speed threshold + debounce).
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class SonicThumperRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Vector3 _lastPos;
        private float _nextThumpAt;

        private ArenaWeaponDefinition Def
        {
            get
            {
                var item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as ArenaWeaponDefinition : null;
            }
        }

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _lastPos = transform.position;
        }

        private void Update()
        {
            bool held = _grab != null && _grab.isSelected;
            float speed = (transform.position - _lastPos).magnitude / Mathf.Max(1e-4f, Time.deltaTime);
            _lastPos = transform.position;
            if (!held || Time.time < _nextThumpAt) return;

            var def = Def;
            float trigger = def != null ? def.swingSpeed : 1.6f;
            if (speed <= trigger) return;
            _nextThumpAt = Time.time + (def != null ? def.fireCooldown : 1.2f);
            Thump();
        }

        private void Thump()
        {
            PvpNoise.Report(transform.position); // bots hear the thump and investigate (MP100 wave 1)
            float r = (float)PvpRules.ThumperRadius;
            var hits = Physics.OverlapSphere(transform.position, r, ~0, QueryTriggerInteraction.Ignore);
            bool anyWall = false;
            foreach (var h in hits)
            {
                if (h == null) continue;

                var wall = h.GetComponentInParent<BreakableWall>();
                if (wall != null && !anyWall)
                {
                    wall.HitFromHammer(h.ClosestPoint(transform.position));
                    anyWall = true;
                    continue;
                }

                var pvp = h.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue; // never thump yourself
                Vector3 shoveDir = h.transform.position - transform.position;
                shoveDir.y = 0f;
                shoveDir = shoveDir.sqrMagnitude > 0.01f ? shoveDir.normalized : transform.forward;
                PvpHitSource.Report(0);
                pvp.ReceiveHit(PvpWeapon.SonicThumper, h.transform.position, shoveDir);
                var bot = h.GetComponentInParent<PvpBot>();
                if (bot != null)
                    bot.transform.position += shoveDir * (float)PvpRules.ThumperShoveMeters * 0.5f;
            }

            // The shockwave reads as a fast-expanding ring (spawn-and-die, no per-frame cost after).
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "ThumpRing";
            var col = ring.GetComponent<Collider>();
            if (col != null) Destroy(col);
            ring.transform.position = new Vector3(transform.position.x, 0.06f, transform.position.z);
            ring.transform.localScale = new Vector3(0.5f, 0.03f, 0.5f);
            ItemFactory.ApplyURPColor(ring, new Color(1f, 0.75f, 0.35f));
            ring.AddComponent<ThumpRingVisual>().targetDiameter = r * 2f;

            Debug.Log("ZIPTIDE: THUMP hits=" + hits.Length + " wall=" + anyWall);
        }
    }

    /// <summary>Expands the ring to the shockwave radius over ~0.25s, then dies.</summary>
    public class ThumpRingVisual : MonoBehaviour
    {
        public float targetDiameter = 5f;
        private float _t;

        private void Update()
        {
            _t += Time.deltaTime / 0.25f;
            float d = Mathf.Lerp(0.5f, targetDiameter, Mathf.Clamp01(_t));
            transform.localScale = new Vector3(d, 0.03f, d);
            if (_t >= 1f) Destroy(gameObject);
        }
    }
}

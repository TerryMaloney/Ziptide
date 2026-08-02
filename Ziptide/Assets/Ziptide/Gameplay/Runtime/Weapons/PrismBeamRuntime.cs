using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// PRISM BEAM (A4 — the earned lane weapon): HOLD the trigger to charge (a growing guide line shows
    /// exactly where the beam will land — the telegraph IS the counter: break line of sight); after
    /// <see cref="PvpRules.PrismChargeSeconds"/> it fires a instant lane beam for heavy damage, then a
    /// long cooldown. Release early = cancel, no cost.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class PrismBeamRuntime : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _chargeStart = -1f;
        private float _nextFireTime;
        private GameObject _guide;
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor _interactor;

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
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _muzzle = transform.Find("Muzzle");
        }

        private void OnEnable()
        {
            if (_grab == null) return;
            _grab.activated.AddListener(OnActivated);
            _grab.deactivated.AddListener(OnDeactivated);
            _grab.selectExited.AddListener(OnDropped);
        }

        private void OnDisable()
        {
            if (_grab == null) return;
            _grab.activated.RemoveListener(OnActivated);
            _grab.deactivated.RemoveListener(OnDeactivated);
            _grab.selectExited.RemoveListener(OnDropped);
        }

        private void OnActivated(ActivateEventArgs args)
        {
            if (Time.time < _nextFireTime) return;
            _chargeStart = Time.time;
            _interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
            Debug.Log("ZIPTIDE: PRISM_CHARGE_START");
        }

        private void OnDeactivated(DeactivateEventArgs args) => CancelCharge();
        private void OnDropped(SelectExitEventArgs args) => CancelCharge();

        private void CancelCharge()
        {
            _chargeStart = -1f;
            if (_guide != null) { Destroy(_guide); _guide = null; }
        }

        private void Update()
        {
            if (_chargeStart < 0f) return;

            float charge = (float)PvpRules.PrismChargeSeconds;
            float t = Mathf.Clamp01((Time.time - _chargeStart) / charge);
            UpdateGuide(t);
            if (_interactor != null) _interactor.SendHapticImpulse(0.15f + 0.35f * t, 0.05f);

            if (t >= 1f)
            {
                _chargeStart = -1f;
                _nextFireTime = Time.time + (float)PvpRules.PrismCooldownSeconds * AugmentEffects.WeaponCooldownScale;
                Fire();
            }
        }

        /// <summary>The charge telegraph: a thin guide line growing toward the target point.</summary>
        private void UpdateGuide(float t)
        {
            Vector3 origin = MuzzlePos();
            Vector3 dir = MuzzleDir();
            float len = Mathf.Lerp(0.5f, (float)PvpRules.PrismRange, t * t); // slow start, fast finish

            if (_guide == null)
            {
                _guide = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _guide.name = "PrismGuide";
                var col = _guide.GetComponent<Collider>();
                if (col != null) Destroy(col);
                ItemFactory.ApplyURPColor(_guide, new Color(0.95f, 0.55f, 0.95f));
            }
            float thick = 0.012f;
            _guide.transform.position = origin + dir * (len * 0.5f);
            _guide.transform.rotation = Quaternion.LookRotation(dir);
            _guide.transform.localScale = new Vector3(thick, thick, len);
        }

        private void Fire()
        {
            if (_guide != null) { Destroy(_guide); _guide = null; }
            Vector3 origin = MuzzlePos();
            Vector3 dir = MuzzleDir();
            float range = (float)PvpRules.PrismRange;

            Vector3 end = origin + dir * range;
            if (Physics.Raycast(origin, dir, out var hit, range, ~0, QueryTriggerInteraction.Ignore))
            {
                end = hit.point;
                var pvp = hit.collider.GetComponentInParent<IPvpDamageable>();
                if (pvp != null && pvp.PlayerIndex != 0)
                {
                    PvpHitSource.Report(0);
                    pvp.ReceiveHit(PvpWeapon.PrismBeam, hit.point, dir);
                }
            }

            // The beam flash: a fat bright lane that lives for a blink.
            var def = Def;
            float thick = def != null ? def.beamThickness : 0.05f;
            var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.name = "PrismBeam";
            var col = beam.GetComponent<Collider>();
            if (col != null) Destroy(col);
            float len = Vector3.Distance(origin, end);
            beam.transform.position = origin + dir * (len * 0.5f);
            beam.transform.rotation = Quaternion.LookRotation(dir);
            beam.transform.localScale = new Vector3(thick, thick, len);
            ItemFactory.ApplyURPColor(beam, new Color(1f, 0.35f, 1f));
            Destroy(beam, 0.15f);

            if (_interactor != null) _interactor.SendHapticImpulse(0.9f, 0.15f);
            Debug.Log("ZIPTIDE: PRISM_FIRE len=" + len.ToString("F1"));
        }

        private Vector3 MuzzlePos() =>
            _muzzle != null ? _muzzle.position : transform.position + transform.forward * 0.2f;

        private Vector3 MuzzleDir() =>
            _muzzle != null ? _muzzle.forward : transform.forward;
    }
}

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Hitscan pistol: raycast from Muzzle on trigger, hit TargetRuntime, tracer/muzzle/impact feedback,
    /// layered definition-driven haptics, visual-only Forge recoil, and authored-or-procedural audio.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(WeaponFeelRuntime))]
    public class PistolRuntime : MonoBehaviour
    {
        [SerializeField] private PistolDefinition pistolDefinition;

        /// <summary>Called by ItemFactory instead of reflection to set the definition.</summary>
        public void Init(PistolDefinition def)
        {
            pistolDefinition = def;
            EnsureFeel();
            ConfigureFeel(def);
        }

        private XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _nextFireTime;
        private AudioSource _audioSource;
        private bool _triggerWasDown;
        private WeaponFeelRuntime _feel;

        private PistolDefinition Def => pistolDefinition != null
            ? pistolDefinition
            : GetComponent<ItemRuntime>()?.Definition as PistolDefinition;

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _muzzle = transform.Find("Muzzle");
            if (_muzzle == null)
            {
                var m = new GameObject("Muzzle");
                m.transform.SetParent(transform, false);
                m.transform.localPosition = new Vector3(0f, 0f, 0.12f);
                _muzzle = m.transform;
            }
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1f;
            _audioSource.playOnAwake = false;
            _audioSource.minDistance = 0.25f;
            _audioSource.maxDistance = 8f;
            EnsureFeel();
            ConfigureFeel(Def);
        }

        private void EnsureFeel()
        {
            if (_feel == null) _feel = GetComponent<WeaponFeelRuntime>();
            if (_feel == null) _feel = gameObject.AddComponent<WeaponFeelRuntime>();
        }

        private void ConfigureFeel(PistolDefinition def)
        {
            if (def == null) return;
            EnsureFeel();
            _feel.Configure(def.itemId, WeaponFeelKind.Ballistic,
                def.hapticAmplitude, def.hapticDuration, def.recoilKick, def.fireRate);
        }

        private void OnEnable()
        {
            if (_grab != null) _grab.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (_grab != null) _grab.activated.RemoveListener(OnActivated);
        }

        private void Update()
        {
            if (_grab == null || !_grab.isSelected) { _triggerWasDown = false; return; }
            bool triggerDown = IsAnyTriggerDown();
            if (triggerDown && !_triggerWasDown) Fire(GetSelectingControllerInteractor());
            _triggerWasDown = triggerDown;
        }

        private void OnActivated(ActivateEventArgs args)
        {
            Fire(args.interactorObject as XRBaseControllerInteractor);
        }

        private void Fire(XRBaseControllerInteractor controllerInteractor)
        {
            PistolDefinition def = Def;
            if (def == null || Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + def.fireRate;
            ConfigureFeel(def);

            Vector3 origin = _muzzle != null ? _muzzle.position : transform.position;
            Vector3 dir = _muzzle != null ? _muzzle.forward : transform.forward;
            Vector3 tracerEnd = origin + dir * def.range;
            bool confirmedHit = false;
            if (Physics.Raycast(origin, dir, out RaycastHit hit, def.range))
            {
                tracerEnd = hit.point;
                TargetRuntime target = hit.collider.GetComponentInParent<TargetRuntime>();
                if (target != null)
                {
                    target.Hit(def.hitForce, hit.point);
                    confirmedHit = true;
                }
                WeaponImpactFx.Spawn(hit.point, hit.normal, confirmedHit);
            }

            TracerFx.Spawn(origin, tracerEnd, new Color(1f, 0.85f, 0.45f));
            if (_feel != null)
            {
                _feel.Fire(controllerInteractor, _audioSource, def.fireClip);
                if (confirmedHit) _feel.ConfirmHit(controllerInteractor);
            }

            if (def.muzzleFlashPrefab != null)
            {
                var flash = Instantiate(def.muzzleFlashPrefab, _muzzle.position, _muzzle.rotation);
                Destroy(flash, 0.1f);
            }
            else
            {
                var flashGo = GamePool.Get("pistol_flash", BuildFallbackFlash, _muzzle.position);
                GamePool.ReleaseAfter("pistol_flash", flashGo, 0.08f);
            }
        }

        private static GameObject BuildFallbackFlash()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "PistolFlash";
            go.transform.localScale = Vector3.one * 0.02f;
            var col = go.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            ItemFactory.ApplyURPColor(go, new Color(1f, 0.72f, 0.24f));
            return go;
        }

        private XRBaseControllerInteractor GetSelectingControllerInteractor()
        {
            if (_grab == null) return null;
            var interactors = _grab.interactorsSelecting;
            if (interactors == null || interactors.Count == 0) return null;
            return (interactors[0] as Component)?.GetComponent<XRBaseControllerInteractor>();
        }

        private static bool IsAnyTriggerDown()
        {
            bool down = false;
            var left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            if (left.isValid && left.TryGetFeatureValue(CommonUsages.triggerButton, out var l) && l) down = true;
            var right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (right.isValid && right.TryGetFeatureValue(CommonUsages.triggerButton, out var r) && r) down = true;
            return down;
        }
    }
}

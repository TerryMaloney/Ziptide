using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Hitscan pistol: raycast from Muzzle on trigger, hit TargetRuntime, haptics, muzzle flash, optional audio.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class PistolRuntime : MonoBehaviour
    {
        [SerializeField] private PistolDefinition pistolDefinition;

        /// <summary>Called by ItemFactory instead of reflection to set the definition.</summary>
        public void Init(PistolDefinition def) { pistolDefinition = def; }

        private XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _nextFireTime;
        private AudioSource _audioSource;
        private bool _triggerWasDown;
        private static AudioClip _fallbackClickClip;

        private PistolDefinition Def => pistolDefinition != null ? pistolDefinition : GetComponent<ItemRuntime>()?.Definition as PistolDefinition;

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
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1f;
            _audioSource.playOnAwake = false;
            _audioSource.minDistance = 0.25f;
            _audioSource.maxDistance = 8f;
        }

        private void OnEnable()
        {
            if (_grab != null)
                _grab.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (_grab != null)
                _grab.activated.RemoveListener(OnActivated);
        }

        private void Update()
        {
            // Fallback: some rigs don’t route Activate events; polling ensures trigger fires while held.
            if (_grab == null || !_grab.isSelected) { _triggerWasDown = false; return; }

            bool triggerDown = IsAnyTriggerDown();
            if (triggerDown && !_triggerWasDown)
            {
                Fire(GetSelectingControllerInteractor());
            }
            _triggerWasDown = triggerDown;
        }

        private void OnActivated(ActivateEventArgs args)
        {
            Fire(args.interactorObject as XRBaseControllerInteractor);
        }

        private void Fire(XRBaseControllerInteractor controllerInteractor)
        {
            var def = Def;
            if (def == null) return;
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + def.fireRate;

            var origin = _muzzle != null ? _muzzle.position : transform.position;
            var dir = _muzzle != null ? _muzzle.forward : transform.forward;
            Vector3 tracerEnd = origin + dir * def.range;
            if (Physics.Raycast(origin, dir, out var hit, def.range))
            {
                tracerEnd = hit.point;
                var target = hit.collider.GetComponentInParent<TargetRuntime>();
                if (target != null)
                    target.Hit(def.hitForce, hit.point);
            }
            // Every shot is visible (Test Day 1: "shows nothing shooting out of it").
            TracerFx.Spawn(origin, tracerEnd, new Color(1f, 0.85f, 0.45f));

            if (controllerInteractor != null)
                controllerInteractor.SendHapticImpulse(def.hapticAmplitude, def.hapticDuration);

            if (def.muzzleFlashPrefab != null)
            {
                var flash = Instantiate(def.muzzleFlashPrefab, _muzzle.position, _muzzle.rotation);
                Destroy(flash, 0.1f);
            }
            else
            {
                // Pooled (HARDWIRING 0.5): a flash per trigger pull is the hottest spawn on the belt.
                var flashGo = Ziptide.Core.GamePool.Get("pistol_flash", BuildFallbackFlash, _muzzle.position);
                Ziptide.Core.GamePool.ReleaseAfter("pistol_flash", flashGo, 0.08f);
            }

            if (_audioSource != null)
            {
                if (def.fireClip != null) _audioSource.PlayOneShot(def.fireClip);
                else _audioSource.PlayOneShot(GetFallbackClickClip());
            }
        }

        /// <summary>Pool factory — runs once; the built flash is reused for every later shot.</summary>
        private static GameObject BuildFallbackFlash()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "PistolFlash";
            go.transform.localScale = Vector3.one * 0.02f;
            var col = go.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            return go;
        }

        private static AudioClip GetFallbackClickClip()
        {
            if (_fallbackClickClip != null) return _fallbackClickClip;
            const int sampleRate = 44100;
            const float duration = 0.03f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float env = Mathf.Exp(-t * 60f);
                float n = (Random.value * 2f - 1f) * env;
                data[i] = n * 0.25f;
            }
            _fallbackClickClip = AudioClip.Create("Pistol_Click", samples, 1, sampleRate, false);
            _fallbackClickClip.SetData(data, 0);
            return _fallbackClickClip;
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

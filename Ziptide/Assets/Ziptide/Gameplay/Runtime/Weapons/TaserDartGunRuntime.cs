using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Fires sticky taser darts that shock targets implementing IShockable.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class TaserDartGunRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _nextFireTime;
        private AudioSource _audioSource;

        private TaserDartGunDefinition Def
        {
            get
            {
                var item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as TaserDartGunDefinition : null;
            }
        }

        private void Awake()
        {
            _grab = GetComponent<XRGrabInteractable>();
            _muzzle = transform.Find("Muzzle");
            if (_muzzle == null)
            {
                var m = new GameObject("Muzzle");
                m.transform.SetParent(transform, false);
                m.transform.localPosition = new Vector3(0f, 0f, 0.14f);
                _muzzle = m.transform;
            }
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1f;
            _audioSource.playOnAwake = false;
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

        private void OnActivated(ActivateEventArgs args)
        {
            Fire(args.interactorObject as XRBaseControllerInteractor);
        }

        private void Fire(XRBaseControllerInteractor controllerInteractor)
        {
            var def = Def;
            if (def == null) return;
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + def.fireCooldown;

            var origin = _muzzle.position;
            var dir = _muzzle.forward;

            var dart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dart.name = "TaserDart";
            dart.transform.position = origin + dir * 0.12f;
            dart.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f);
            dart.transform.localScale = new Vector3(0.015f, 0.06f, 0.015f);

            var dartCol = dart.GetComponent<Collider>();
            if (dartCol != null) Object.Destroy(dartCol);
            var capsule = dart.AddComponent<CapsuleCollider>();
            capsule.radius = 0.015f;
            capsule.height = 0.12f;

            var rb = dart.AddComponent<Rigidbody>();
            rb.mass = def.dartMass;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = dir * def.muzzleVelocity;

            // Don't let the dart stick to the gun itself or the player body on spawn
            // (that was the "bullet stops short / stuck in front of the gun" bug).
            foreach (var gunCol in GetComponentsInChildren<Collider>(true))
                if (gunCol != null) Physics.IgnoreCollision(capsule, gunCol);
            var playerBody = Object.FindObjectOfType<CharacterController>();
            if (playerBody != null) Physics.IgnoreCollision(capsule, playerBody);

            var proj = dart.AddComponent<TaserDartProjectile>();
            proj.Init(def.stunSeconds, def.hitImpulse, def.dartLifetime, def.impactClip);

            ItemFactory.ApplyURPColor(dart, new Color(0.1f, 0.9f, 1f));

            if (controllerInteractor != null)
                controllerInteractor.SendHapticImpulse(def.hapticAmplitude, def.hapticDuration);

            if (_audioSource != null && def.fireClip != null)
                _audioSource.PlayOneShot(def.fireClip);
        }
    }
}

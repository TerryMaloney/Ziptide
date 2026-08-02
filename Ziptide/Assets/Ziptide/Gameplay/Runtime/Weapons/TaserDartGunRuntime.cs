using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Fires sticky taser darts with definition-driven layered haptics, visual-only Forge recoil,
    /// authored-or-procedural fire audio, and a confirmed-impact return pulse from the projectile.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    [RequireComponent(typeof(WeaponFeelRuntime))]
    public class TaserDartGunRuntime : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _nextFireTime;
        private AudioSource _audioSource;
        private WeaponFeelRuntime _feel;
        private TaserDartGunDefinition _definition;

        private TaserDartGunDefinition Def
        {
            get
            {
                if (_definition != null) return _definition;
                var item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as TaserDartGunDefinition : null;
            }
        }

        public void Init(TaserDartGunDefinition def)
        {
            _definition = def;
            EnsureFeel();
            ConfigureFeel(def);
        }

        private void Awake()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _muzzle = transform.Find("Muzzle");
            if (_muzzle == null)
            {
                var m = new GameObject("Muzzle");
                m.transform.SetParent(transform, false);
                m.transform.localPosition = new Vector3(0f, 0f, 0.14f);
                _muzzle = m.transform;
            }
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1f;
            _audioSource.playOnAwake = false;
            EnsureFeel();
            ConfigureFeel(Def);
        }

        private void EnsureFeel()
        {
            if (_feel == null) _feel = GetComponent<WeaponFeelRuntime>();
            if (_feel == null) _feel = gameObject.AddComponent<WeaponFeelRuntime>();
        }

        private void ConfigureFeel(TaserDartGunDefinition def)
        {
            if (def == null) return;
            EnsureFeel();
            // Momentum-derived presentation kick: data-owned mass and velocity, bounded by WeaponFeelCore.
            float recoil = Mathf.Clamp(def.dartMass * def.muzzleVelocity * 0.01f, 0f, 0.025f);
            _feel.Configure(def.itemId, WeaponFeelKind.Electric,
                def.hapticAmplitude, def.hapticDuration, recoil, def.fireCooldown);
        }

        private void OnEnable()
        {
            if (_grab != null) _grab.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            if (_grab != null) _grab.activated.RemoveListener(OnActivated);
        }

        private void OnActivated(ActivateEventArgs args)
        {
            Fire(args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor);
        }

        private void Fire(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
        {
            TaserDartGunDefinition def = Def;
            if (def == null || Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + def.fireCooldown;
            ConfigureFeel(def);

            Vector3 origin = _muzzle.position;
            Vector3 dir = _muzzle.forward;
            var dart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dart.name = "TaserDart";
            dart.transform.position = origin + dir * 0.12f;
            dart.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f);
            dart.transform.localScale = new Vector3(0.015f, 0.06f, 0.015f);

            var dartCol = dart.GetComponent<Collider>();
            if (dartCol != null)
            {
                dartCol.enabled = false;
                Object.Destroy(dartCol);
            }
            var capsule = dart.AddComponent<CapsuleCollider>();
            capsule.radius = 0.015f;
            capsule.height = 0.12f;

            var rb = dart.AddComponent<Rigidbody>();
            rb.mass = def.dartMass;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = dir * def.muzzleVelocity;

            foreach (var gunCol in GetComponentsInChildren<Collider>(true))
                if (gunCol != null) Physics.IgnoreCollision(capsule, gunCol);
            var playerBody = Object.FindObjectOfType<CharacterController>();
            if (playerBody != null) Physics.IgnoreCollision(capsule, playerBody);

            var proj = dart.AddComponent<TaserDartProjectile>();
            proj.Init(def.stunSeconds, def.hitImpulse, def.dartLifetime, def.impactClip,
                _feel, controllerInteractor);
            ItemFactory.ApplyURPColor(dart, new Color(0.1f, 0.9f, 1f));
            if (_feel != null) _feel.Fire(controllerInteractor, _audioSource, def.fireClip);
        }
    }
}

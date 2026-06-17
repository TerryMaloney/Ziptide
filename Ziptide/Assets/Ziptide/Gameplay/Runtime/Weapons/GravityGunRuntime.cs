using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Hitscan "grav pulse" gun. On trigger it fires a beam from the muzzle; if it hits a drone it
    /// downs it (location-based reaction via <see cref="DroneRuntime.RegisterHit"/>) and LAUNCHES it
    /// with a gravity-kick impulse. Reuses the same grab/holster setup as the taser so it snaps to a
    /// forward grip and rides the belt.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GravityGunRuntime : MonoBehaviour
    {
        private XRGrabInteractable _grab;
        private Transform _muzzle;
        private float _nextFireTime;
        private AudioSource _audioSource;
        private LineRenderer _beam;
        private float _beamHideTime;

        private GravityGunDefinition Def
        {
            get
            {
                var item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as GravityGunDefinition : null;
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
                m.transform.localPosition = new Vector3(0f, 0f, 0.15f);
                _muzzle = m.transform;
            }

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1f;
            _audioSource.playOnAwake = false;

            SetupBeam();
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
            if (_beam != null && _beam.enabled && Time.time >= _beamHideTime)
                _beam.enabled = false;
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

            Vector3 origin = _muzzle.position;
            Vector3 dir = _muzzle.forward;
            Vector3 beamEnd = origin + dir * def.range;

            // Ignore the gun's own colliders by raycasting all and picking the first non-self hit.
            var hits = Physics.RaycastAll(origin, dir, def.range);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform)) continue; // skip self
                beamEnd = hit.point;

                var drone = hit.collider.GetComponentInParent<DroneRuntime>();
                if (drone != null)
                {
                    drone.RegisterHit(hit.point, false); // down it with a location reaction (no taser seize)
                    var rb = drone.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 launch = dir * def.launchForce + Vector3.up * def.upwardBias;
                        rb.AddForce(launch, ForceMode.Impulse);
                    }
                }
                break; // only the first solid thing the pulse meets
            }

            ShowBeam(origin, beamEnd);

            if (controllerInteractor != null)
                controllerInteractor.SendHapticImpulse(def.hapticAmplitude, def.hapticDuration);
            if (_audioSource != null && def.fireClip != null)
                _audioSource.PlayOneShot(def.fireClip);
        }

        private void SetupBeam()
        {
            _beam = gameObject.GetComponent<LineRenderer>();
            if (_beam == null) _beam = gameObject.AddComponent<LineRenderer>();
            _beam.positionCount = 2;
            _beam.startWidth = 0.02f;
            _beam.endWidth = 0.04f;
            _beam.useWorldSpace = true;
            _beam.numCapVertices = 2;
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                var mat = new Material(shader);
                var c = new Color(0.6f, 0.4f, 1f);
                mat.color = c;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                _beam.material = mat;
            }
            _beam.enabled = false;
        }

        private void ShowBeam(Vector3 from, Vector3 to)
        {
            if (_beam == null) return;
            _beam.SetPosition(0, from);
            _beam.SetPosition(1, to);
            _beam.enabled = true;
            _beamHideTime = Time.time + 0.08f;
        }
    }
}

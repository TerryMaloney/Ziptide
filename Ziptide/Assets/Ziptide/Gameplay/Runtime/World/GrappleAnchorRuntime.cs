using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Traversal;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.4g — the GRAPPLE's scene translator, on the ANCHOR (the ClimbableSurface pattern):
    /// a glowing ring you select with the ray from a distance; on grip the pure <see cref="GrappleReel"/>
    /// takes over — range-gated at fire (too far = the ring shakes its head with a dim pulse, a MISS,
    /// never an error), ease-in spool, comfort-capped straight reel, arrival a stop-margin short so
    /// you land beside the anchor. Locomotion suspends for the ride; releasing the grip mid-reel
    /// aborts where you are (the catch floor / fall net owns the drop). Rig delta-positioned along
    /// the reel, never parented. This is the RANGE verb: climb is slow+strong, zip is fixed-route,
    /// lift is patient, pad is fixed-arc — the grapple goes where you POINT, within its leash.
    /// </summary>
    public class GrappleAnchorRuntime : MonoBehaviour
    {
        [Tooltip("Max fire range in meters (the reel's leash).")]
        public float maxRange = (float)GrappleReel.MaxRangeDefault;

        private Transform _rig;
        private ActionBasedContinuousMoveProvider _move;
        private GrappleReel _reel;
        private Renderer _ringRenderer;
        private float _missPulseUntil;
        private bool _built;

        private static readonly Color RingColor = new Color(0.95f, 0.45f, 0.65f); // grapple magenta-rose
        private static readonly Color MissColor = new Color(0.35f, 0.2f, 0.26f);

        private void Start()
        {
            if (_built) return;
            _built = true;

            // The visible anchor: a flattened torus-read ring (cylinder shell) + a core stud.
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "GrappleRing";
            var rc = ring.GetComponent<Collider>();
            if (rc != null) Destroy(rc);
            ring.transform.SetParent(transform, false);
            ring.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // face outward
            ring.transform.localScale = new Vector3(0.55f, 0.05f, 0.55f);
            ItemFactory.ApplyURPColor(ring, RingColor);
            _ringRenderer = ring.GetComponent<Renderer>();

            // Collider BEFORE the interactable (gotcha #6) — a generous select target for the ray.
            var col = gameObject.GetComponent<Collider>();
            if (col == null)
            {
                var sc = gameObject.AddComponent<SphereCollider>();
                sc.radius = 0.45f;
                sc.isTrigger = true;
            }
            var grab = gameObject.GetComponent<XRSimpleInteractable>();
            if (grab == null) grab = gameObject.AddComponent<XRSimpleInteractable>();
            grab.selectEntered.AddListener(_ => Fire());
            grab.selectExited.AddListener(_ => Abort("released"));
            Debug.Log("ZIPTIDE: GRAPPLE_READY name=" + gameObject.name);
        }

        private void Fire()
        {
            if (_reel != null) return;
            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                _rig = rig != null ? rig.transform : null;
                if (_rig == null) return;
            }

            _reel = GrappleReel.TryFire(
                ToT(_rig.position), ToT(transform.position), maxRange);
            if (_reel == null)
            {
                // Out of the leash: the miss is VISIBLE (dim pulse), never silent and never an error.
                _missPulseUntil = Time.time + 0.5f;
                Debug.Log("ZIPTIDE: GRAPPLE_MISS dist=" +
                    Vector3.Distance(_rig.position, transform.position).ToString("F0"));
                return;
            }
            SuspendMove(true);
            Debug.Log("ZIPTIDE: GRAPPLE_FIRE dist=" +
                Vector3.Distance(_rig.position, transform.position).ToString("F0"));
        }

        private void Abort(string reason)
        {
            if (_reel == null) return;
            _reel = null;
            SuspendMove(false);
            Debug.Log("ZIPTIDE: GRAPPLE_END reason=" + reason);
        }

        private void Update()
        {
            if (_ringRenderer != null)
            {
                bool missing = Time.time < _missPulseUntil;
                var mat = _ringRenderer.material;
                Color c = missing ? MissColor
                    : RingColor * (0.8f + 0.2f * Mathf.Sin(Time.time * 2.2f)); // idle breath
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                else mat.color = c;
            }

            if (_reel == null || _rig == null) return;
            var p = _reel.Step(Time.deltaTime);
            _rig.position = new Vector3(p.X, p.Y, p.Z); // reel owns the ride — never parented
            if (_reel.Arrived) Abort("arrived");
        }

        private void SuspendMove(bool suspend)
        {
            if (_move == null && _rig != null)
                _move = _rig.GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);
            if (_move != null) _move.enabled = !suspend;
        }

        private static TVec3 ToT(Vector3 v) => new TVec3(v.x, v.y, v.z);
    }
}

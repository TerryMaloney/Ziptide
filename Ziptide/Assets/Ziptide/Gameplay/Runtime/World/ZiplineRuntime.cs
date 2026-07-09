using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.4 / WORLDS #23 — the ridable zipline, the game's namesake traversal. Place one
    /// GameObject with this component, set the two anchors (public Init or inspector), and it builds
    /// itself at Start: sagging cable visual (ZiplineCore samples), anchor posts, and a grabbable
    /// handle at the high end. Grab the handle and the RIG is delta-translated along the cable —
    /// never parented (the locked law), so the ComfortVignette engages automatically from rig motion
    /// and fall-safety sees continuous movement. Release mid-ride to drop out; the handle glides
    /// home for the next ride. Comfort (ease-in, arrive-always) lives in tested ZiplineCore math.
    /// </summary>
    public class ZiplineRuntime : MonoBehaviour
    {
        [Tooltip("Cable start (the ride begins here — usually the HIGH end).")]
        public Vector3 startAnchor;
        [Tooltip("Cable end (the ride finishes here).")]
        public Vector3 endAnchor;
        [Tooltip("Cruise speed m/s (0 = ZiplineCore.DefaultSpeed).")]
        public float speed = 0f;

        private const int CableSegments = 14;

        private Transform _handle;
        private XRSimpleInteractable _grab;
        private float _sag;
        private float _length;
        private bool _riding;
        private float _t;
        private float _rideSeconds;
        private Vector3 _lastHandlePos;
        private bool _built;

        /// <summary>Author/patcher entry point (public Init — the no-reflection law).</summary>
        public void Init(Vector3 start, Vector3 end, float cruiseSpeed = 0f)
        {
            startAnchor = start;
            endAnchor = end;
            speed = cruiseSpeed;
        }

        private void Start()
        {
            if (_built) return;
            if (Vector3.Distance(startAnchor, endAnchor) < 2f)
            {
                Debug.LogWarning("ZIPTIDE: ZIPLINE_DEGENERATE anchors too close — disabled");
                enabled = false;
                return;
            }
            Build();
        }

        private void Build()
        {
            _built = true;
            _sag = ZiplineCore.SagFor(startAnchor, endAnchor);
            _length = Vector3.Distance(startAnchor, endAnchor);

            // Cable: thin stretched cubes between successive curve samples (primitive-style, no
            // LineRenderer material dependency). No colliders — the HANDLE is the interactable.
            var dark = new Color(0.16f, 0.17f, 0.18f);
            Vector3 prev = ZiplineCore.Sample(startAnchor, endAnchor, _sag, 0f);
            for (int i = 1; i <= CableSegments; i++)
            {
                Vector3 next = ZiplineCore.Sample(startAnchor, endAnchor, _sag, i / (float)CableSegments);
                var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                seg.name = "Cable" + i;
                Object.Destroy(seg.GetComponent<Collider>());
                seg.transform.SetParent(transform, true);
                seg.transform.position = (prev + next) * 0.5f;
                seg.transform.rotation = Quaternion.LookRotation(next - prev);
                seg.transform.localScale = new Vector3(0.04f, 0.04f, Vector3.Distance(prev, next) * 1.02f);
                ItemFactory.ApplyURPColor(seg, dark);
                prev = next;
            }
            Post(startAnchor); Post(endAnchor);

            // The grab handle: a bar hanging just under the cable at the start.
            var handleGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handleGo.name = "ZipHandle";
            handleGo.transform.SetParent(transform, true);
            handleGo.transform.localScale = new Vector3(0.06f, 0.22f, 0.06f);
            var col = handleGo.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            ItemFactory.ApplyURPColor(handleGo, new Color(0.95f, 0.75f, 0.25f)); // signal yellow
            _handle = handleGo.transform;
            PlaceHandle(0f);

            _grab = handleGo.AddComponent<XRSimpleInteractable>();
            _grab.selectEntered.AddListener(_ => BeginRide());
            _grab.selectExited.AddListener(_ => EndRide("released"));
            Debug.Log("ZIPTIDE: ZIPLINE_READY len=" + _length.ToString("F0") + "m");
        }

        private void Post(Vector3 at)
        {
            var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.name = "ZipPost";
            post.transform.SetParent(transform, true);
            post.transform.position = at - new Vector3(0f, 1.1f, 0f);
            post.transform.localScale = new Vector3(0.18f, 2.2f, 0.18f);
            ItemFactory.ApplyURPColor(post, new Color(0.22f, 0.23f, 0.24f));
        }

        private void PlaceHandle(float t)
        {
            Vector3 onCable = ZiplineCore.Sample(startAnchor, endAnchor, _sag, t);
            _handle.position = onCable - new Vector3(0f, 0.28f, 0f); // hangs under the cable
            _lastHandlePos = _handle.position;
        }

        private Transform _rig; // cached for the ride — no per-frame FindObjectOfType

        private void BeginRide()
        {
            if (_riding) return;
            var rig = Object.FindObjectOfType<PlayerRigPersistence>();
            _rig = rig != null ? rig.transform : null;
            _riding = true;
            _rideSeconds = 0f;
            Debug.Log("ZIPTIDE: ZIPLINE_RIDE_START");
        }

        private void EndRide(string reason)
        {
            if (!_riding) return;
            _riding = false;
            Debug.Log("ZIPTIDE: ZIPLINE_RIDE_END reason=" + reason + " t=" + _t.ToString("F2"));
        }

        private void Update()
        {
            if (!_built) return;

            if (_riding)
            {
                _rideSeconds += Time.deltaTime;
                _t = ZiplineCore.Advance(_t, Time.deltaTime, _rideSeconds, _length,
                    speed > 0f ? speed : ZiplineCore.DefaultSpeed);
                PlaceHandleAndCarryRig(_t);
                if (ZiplineCore.Arrived(_t)) EndRide("arrived");
            }
            else if (_t > 0f)
            {
                // Unheld: the handle glides home so the line is ready for the next ride.
                _t = Mathf.Max(0f, _t - Time.deltaTime * 0.25f);
                PlaceHandle(_t);
            }
        }

        private void PlaceHandleAndCarryRig(float t)
        {
            Vector3 before = _lastHandlePos;
            PlaceHandle(t);
            Vector3 delta = _handle.position - before;
            if (_rig != null) _rig.position += delta; // delta-translate — NEVER parent the rig
        }
    }
}

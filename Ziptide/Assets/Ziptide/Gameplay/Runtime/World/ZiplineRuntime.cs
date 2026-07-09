using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Traversal;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.4 / WORLDS #23 — the ridable zipline: the SCENE TRANSLATOR for the pure
    /// <see cref="ZiplineRide"/> kinematics (along-cable gravity + drag + push-off + the hard comfort
    /// speed cap — all tested in Traversal). Place one GameObject with this component, set the two
    /// anchors (public Init or inspector), and it self-builds at Start: sagging cable visual (sag is
    /// visual-only, per the core's contract), anchor posts, and a grabbable handle at the start.
    ///
    /// Grab the handle and the RIG is delta-translated along the cable — never parented (the locked
    /// law) — so the ComfortVignette engages automatically from rig motion and fall-safety sees
    /// continuous movement. Release mid-ride to drop out; the handle glides home for the next rider.
    /// </summary>
    public class ZiplineRuntime : MonoBehaviour
    {
        [Tooltip("Cable start (the ride begins here — usually the HIGH end).")]
        public Vector3 startAnchor;
        [Tooltip("Cable end (the ride finishes here).")]
        public Vector3 endAnchor;
        [Tooltip("Comfort speed cap m/s (0 = the ride's default cap).")]
        public float maxSpeed = 0f;

        private const int CableSegments = 14;
        private const float SagFraction = 0.04f; // visual dip: 4% of span at mid-cable

        private Transform _handle;
        private Transform _rig;      // cached per ride — no per-frame finds
        private ZiplineRide _ride;   // pure kinematics; null when idle
        private float _sag;
        private float _idleT;        // handle's glide-home progress while unheld
        private Vector3 _lastHandlePos;
        private bool _built;

        /// <summary>Author/patcher entry point (public Init — the no-reflection law).</summary>
        public void Init(Vector3 start, Vector3 end, float comfortCap = 0f)
        {
            startAnchor = start;
            endAnchor = end;
            maxSpeed = comfortCap;
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

        // ── Visual-only sag (the core rides the straight chord; the cable LOOKS like a cable). ──
        private static Vector3 SagSample(Vector3 a, Vector3 b, float sagMeters, float t)
        {
            Vector3 p = Vector3.LerpUnclamped(a, b, Mathf.Clamp01(t));
            p.y -= Mathf.Max(0f, sagMeters) * 4f * t * (1f - t);
            return p;
        }

        private void Build()
        {
            _built = true;
            _sag = Vector3.Distance(startAnchor, endAnchor) * SagFraction;

            // Cable: thin stretched cubes between successive sag samples (primitive-style, no
            // LineRenderer material dependency). No colliders — the HANDLE is the interactable.
            var dark = new Color(0.16f, 0.17f, 0.18f);
            Vector3 prev = SagSample(startAnchor, endAnchor, _sag, 0f);
            for (int i = 1; i <= CableSegments; i++)
            {
                Vector3 next = SagSample(startAnchor, endAnchor, _sag, i / (float)CableSegments);
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

            var grab = handleGo.AddComponent<XRSimpleInteractable>();
            grab.selectEntered.AddListener(_ => BeginRide());
            grab.selectExited.AddListener(_ => EndRide("released"));
            Debug.Log("ZIPTIDE: ZIPLINE_READY len=" +
                Vector3.Distance(startAnchor, endAnchor).ToString("F0") + "m");
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
            Vector3 onCable = SagSample(startAnchor, endAnchor, _sag, t);
            _handle.position = onCable - new Vector3(0f, 0.28f, 0f); // hangs under the cable
            _lastHandlePos = _handle.position;
        }

        private static TVec3 ToT(Vector3 v) => new TVec3(v.x, v.y, v.z);

        private void BeginRide()
        {
            if (_ride != null) return;
            var rig = Object.FindObjectOfType<PlayerRigPersistence>();
            _rig = rig != null ? rig.transform : null;
            _ride = maxSpeed > 0f
                ? new ZiplineRide(ToT(startAnchor), ToT(endAnchor), maxSpeed)
                : new ZiplineRide(ToT(startAnchor), ToT(endAnchor));
            Debug.Log("ZIPTIDE: ZIPLINE_RIDE_START");
        }

        private void EndRide(string reason)
        {
            if (_ride == null) return;
            _idleT = _ride.Progress; // handle glides home from wherever the ride ended
            Debug.Log("ZIPTIDE: ZIPLINE_RIDE_END reason=" + reason +
                " t=" + _ride.Progress.ToString("F2"));
            _ride = null;
        }

        private void Update()
        {
            if (!_built) return;

            if (_ride != null)
            {
                _ride.Step(Time.deltaTime);
                Vector3 before = _lastHandlePos;
                PlaceHandle(_ride.Progress);
                Vector3 delta = _handle.position - before;
                if (_rig != null) _rig.position += delta; // delta-translate — NEVER parent the rig
                if (_ride.Arrived) EndRide("arrived");
            }
            else if (_idleT > 0f)
            {
                // Unheld: the handle glides home so the line is ready for the next ride.
                _idleT = Mathf.Max(0f, _idleT - Time.deltaTime * 0.25f);
                PlaceHandle(_idleT);
            }
        }
    }
}

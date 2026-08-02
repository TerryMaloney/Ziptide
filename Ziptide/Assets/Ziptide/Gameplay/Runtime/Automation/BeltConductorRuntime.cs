using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Automation;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 4.1h — CONDUCTOR MODE: grab the lantern and RIDE YOUR OWN LINE. The scene
    /// translator for the pure <see cref="BeltRoute"/> trace + <see cref="ConductorRide"/> glide:
    /// the route re-traces from the live lattice at grab time (so it always inspects the factory as
    /// built, player edits included), the handle glides along the line at the ore's-eye pace, and
    /// the RIG is delta-translated behind it — never parented (the locked law; the ZiplineRuntime
    /// idiom exactly). Release to step off anywhere; every cell lip crossed clicks in your hand
    /// like track joints. Patch-time = serialized fields only; visuals build in Start().
    /// </summary>
    public class BeltConductorRuntime : MonoBehaviour
    {
        [Tooltip("The floor whose line this conductor rides.")]
        public BeltFloorRuntime floor;
        [Tooltip("Route start cell (usually the port/source the line begins at).")]
        public int startX, startZ;

        private const float HandleHeight = 1.15f;   // hand height above the belt surface
        private const float GlideHomeSpeed = 2.5f;  // m/s — the handle's return glide when unheld

        private Transform _handle;
        private Transform _rig;
        private ConductorRide _ride;
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor _hand;
        private int _ticksSent;

        private void Start()
        {
            BuildVisual();
        }

        private void BuildVisual()
        {
            // The conductor's post: a slim pillar with a lantern-handle on top, belt-teal so it
            // reads as part of the automation family.
            var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.name = "ConductorPost";
            post.transform.SetParent(transform, false);
            post.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            post.transform.localScale = new Vector3(0.12f, 1.0f, 0.12f);
            ItemFactory.ApplyURPColor(post, new Color(0.22f, 0.24f, 0.26f));

            // 4.1j: an actual LANTERN — cage cylinder, cap, hanging loop, and a bright core that
            // reads "grab me" from across the floor. One trigger collider on the cage; the deco
            // parts are colliderless children so the whole lantern travels as one.
            var handleGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handleGo.name = "ConductorHandle";
            handleGo.transform.SetParent(transform, true);
            handleGo.transform.position = HomePosition();
            handleGo.transform.localScale = new Vector3(0.10f, 0.11f, 0.10f);
            var col = handleGo.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            ItemFactory.ApplyURPColor(handleGo, new Color(0.20f, 0.30f, 0.28f)); // dark cage
            _handle = handleGo.transform;

            var core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "LanternCore";
            Destroy(core.GetComponent<Collider>());
            core.transform.SetParent(_handle, false);
            core.transform.localScale = new Vector3(0.75f, 0.68f, 0.75f);
            ItemFactory.ApplyURPColor(core, new Color(0.45f, 1.0f, 0.85f)); // the automation accent, lit

            var cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cap.name = "LanternCap";
            Destroy(cap.GetComponent<Collider>());
            cap.transform.SetParent(_handle, false);
            cap.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            cap.transform.localScale = new Vector3(1.2f, 0.18f, 1.2f);
            ItemFactory.ApplyURPColor(cap, new Color(0.16f, 0.18f, 0.20f));

            var loop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            loop.name = "LanternLoop";
            Destroy(loop.GetComponent<Collider>());
            loop.transform.SetParent(_handle, false);
            loop.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            loop.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            loop.transform.localScale = new Vector3(0.45f, 0.06f, 0.45f);
            ItemFactory.ApplyURPColor(loop, new Color(0.35f, 0.95f, 0.75f));

            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var grab = handleGo.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            grab.selectEntered.AddListener(a => BeginRide(a));
            grab.selectExited.AddListener(_ => EndRide("released"));
        }

        private Vector3 HomePosition() => transform.position + Vector3.up * HandleHeight;

        private void BeginRide(SelectEnterEventArgs args)
        {
            if (_ride != null || floor == null || floor.Lattice == null) return;
            var path = BeltRoute.Trace(floor.Lattice, startX, startZ);
            if (path.Count < 2)
            {
                Debug.Log("ZIPTIDE: BELT_RIDE_NO_ROUTE cells=" + path.Count);
                return;
            }
            var rig = Object.FindObjectOfType<PlayerRigPersistence>();
            _rig = rig != null ? rig.transform : null;
            _hand = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
            _ride = new ConductorRide(path);
            _ticksSent = 0;
            Debug.Log("ZIPTIDE: BELT_RIDE_START cells=" + path.Count);
        }

        private void EndRide(string reason)
        {
            if (_ride == null) return;
            Debug.Log("ZIPTIDE: BELT_RIDE_END reason=" + reason +
                " t=" + _ride.ProgressCells.ToString("F1") + "/" + _ride.TotalCells.ToString("F0"));
            _ride = null;
            _hand = null;
        }

        private Vector3 RoutePosition(ConductorRide ride)
        {
            ride.GridPosition(out float gx, out float gz);
            return floor.GridToWorld(gx, gz) + Vector3.up * HandleHeight;
        }

        private void Update()
        {
            if (_handle == null || floor == null) return;

            if (_ride != null)
            {
                _ride.Step(Time.deltaTime);
                Vector3 before = _handle.position;
                _handle.position = RoutePosition(_ride);
                if (_rig != null) _rig.position += _handle.position - before; // delta — NEVER parent

                // Track-joint clicks: one gentle haptic tick per cell lip crossed.
                if (_hand != null && _ride.BoundaryCrossings > _ticksSent)
                {
                    _ticksSent = _ride.BoundaryCrossings;
                    _hand.SendHapticImpulse(0.25f, 0.04f);
                }
                if (_ride.Arrived) EndRide("arrived");
            }
            else if ((_handle.position - HomePosition()).sqrMagnitude > 1e-4f)
            {
                // Unheld: glide the handle back to its post for the next ride — from wherever the
                // rider stepped off, no snap.
                _handle.position = Vector3.MoveTowards(_handle.position, HomePosition(),
                    Time.deltaTime * GlideHomeSpeed);
            }
        }
    }
}

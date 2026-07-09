using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Traversal;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.4b — CLIMBING, the scene translator for the pure <see cref="ClimbGrip"/> math
    /// (rig moves −handDelta; two-hand handoff with no teleport; comfort-clamped release fling — all
    /// EditMode-tested in Traversal). Add this to anything with a collider and it becomes climbable:
    /// it ensures an <see cref="XRSimpleInteractable"/> (collider FIRST — VR gotcha #6) and paints
    /// handhold studs so climbability READS in the headset. Grips route to the rig-level
    /// <see cref="ClimbCoordinator"/> so both hands cooperate even across two different surfaces.
    /// Mirrors ZiplineRuntime's idiom: delta-translate the rig, never parent (the locked law); the
    /// ComfortVignette engages from rig motion automatically.
    /// </summary>
    public class ClimbableSurface : MonoBehaviour
    {
        [Tooltip("Paint small stud handholds across the surface so climbability reads (dev-visual).")]
        public bool buildHoldVisuals = true;

        private void Start()
        {
            // Gotcha #6: XRSimpleInteractable gathers colliders at Awake/enable — make sure one exists
            // BEFORE adding the interactable, or the surface is silently ungrabbable.
            if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
                gameObject.AddComponent<BoxCollider>();

            var grab = GetComponent<XRSimpleInteractable>();
            if (grab == null) grab = gameObject.AddComponent<XRSimpleInteractable>();
            grab.selectEntered.AddListener(OnGripped);
            grab.selectExited.AddListener(OnReleased);

            if (buildHoldVisuals) BuildHolds();
            Debug.Log("ZIPTIDE: CLIMBABLE_READY name=" + gameObject.name);
        }

        private const float GripReachMeters = 1.2f; // arm's length — no ray-climbing from across the plaza

        private void OnGripped(SelectEnterEventArgs args)
        {
            var interactor = args.interactorObject as XRBaseControllerInteractor;
            if (interactor == null) return;
            // The rig's ray interactors can select from range — climbing only counts when the hand is
            // actually AT the wall, or you could hoist yourself on a laser pointer.
            var col = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
            if (col != null &&
                Vector3.Distance(interactor.transform.position, col.ClosestPoint(interactor.transform.position))
                    > GripReachMeters)
                return;
            ClimbCoordinator.Ensure().BeginGrip(HandOf(interactor), interactor.transform);
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            var interactor = args.interactorObject as XRBaseControllerInteractor;
            if (interactor == null) return;
            ClimbCoordinator.Ensure().EndGrip(HandOf(interactor));
        }

        /// <summary>Handedness from the interactor's hierarchy names (the rig's controllers carry
        /// Left/Right in their names). Unknown → Right, and the coordinator still behaves.</summary>
        private static Hand HandOf(XRBaseControllerInteractor interactor)
        {
            for (Transform t = interactor.transform; t != null; t = t.parent)
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("left")) return Hand.Left;
                if (n.Contains("right")) return Hand.Right;
            }
            return Hand.Right;
        }

        // Stud handholds: a sparse grid of small bright knobs on the surface's most-vertical face —
        // the affordance is the point (an unmarked wall reads as scenery, not a route).
        private void BuildHolds()
        {
            var col = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
            if (col == null) return;
            Bounds b = col.bounds;
            var tint = new Color(0.95f, 0.62f, 0.2f); // signal orange — matches the zipline's language
            int rows = Mathf.Clamp(Mathf.RoundToInt(b.size.y / 0.55f), 2, 8);
            int cols = Mathf.Clamp(Mathf.RoundToInt(Mathf.Max(b.size.x, b.size.z) / 0.6f), 2, 6);
            bool xFace = b.size.x < b.size.z; // studs on the thinner axis' face

            for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                var stud = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stud.name = "Hold";
                Object.Destroy(stud.GetComponent<Collider>()); // the WALL is the interactable, not studs
                stud.transform.SetParent(transform, true);
                float fy = Mathf.Lerp(b.min.y + 0.3f, b.max.y - 0.2f, rows <= 1 ? 0.5f : r / (float)(rows - 1));
                float ft = Mathf.Lerp(0.15f, 0.85f, cols <= 1 ? 0.5f : c / (float)(cols - 1));
                // Offset alternate rows so the route reads as hand-over-hand, not a ladder.
                if (r % 2 == 1) ft = Mathf.Clamp01(ft + 0.5f / cols);
                Vector3 p = xFace
                    ? new Vector3(b.min.x - 0.04f, fy, Mathf.Lerp(b.min.z, b.max.z, ft))
                    : new Vector3(Mathf.Lerp(b.min.x, b.max.x, ft), fy, b.min.z - 0.04f);
                stud.transform.position = p;
                stud.transform.localScale = Vector3.one * 0.09f;
                ItemFactory.ApplyURPColor(stud, tint);
            }
        }
    }

    /// <summary>
    /// The rig-side half: ONE <see cref="ClimbGrip"/> for the whole player (so a left grip on wall A
    /// and a right grip on wall B still hand off with no teleport, exactly as the pure tests prove).
    /// While any hand grips: continuous locomotion is suspended (the PlayerStunReceiver pattern — the
    /// stick fighting the climb feels broken) and the driving hand's motion delta-translates the rig.
    /// Release of the last hand restores locomotion; the fling is computed + comfort-clamped by the
    /// core and LOGGED, but not applied in v1 (ballistic launch needs the fall-mover — queued, see
    /// VERTICAL_AND_CAVERN_WORLDS.md).
    /// </summary>
    public class ClimbCoordinator : MonoBehaviour
    {
        private static ClimbCoordinator _instance;

        private readonly ClimbGrip _grip = new ClimbGrip();
        private Transform _rig;
        private ActionBasedContinuousMoveProvider _move;
        private bool _movedSuspended;

        private Transform _leftAttach, _rightAttach;
        private Vector3 _leftLastPos, _rightLastPos;
        private Vector3 _leftVel, _rightVel;

        public static ClimbCoordinator Ensure()
        {
            if (_instance != null) return _instance;
            var go = new GameObject("__ClimbCoordinator");
            _instance = go.AddComponent<ClimbCoordinator>();
            return _instance;
        }

        public void BeginGrip(Hand hand, Transform attach)
        {
            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                _rig = rig != null ? rig.transform : null;
            }
            if (attach == null || _rig == null) return;

            if (hand == Hand.Left) { _leftAttach = attach; _leftLastPos = attach.position; }
            else { _rightAttach = attach; _rightLastPos = attach.position; }
            _grip.Grip(hand, ToT(attach.position));
            SuspendMove(true);
            Debug.Log("ZIPTIDE: CLIMB_GRIP hand=" + hand);
        }

        public void EndGrip(Hand hand)
        {
            Vector3 vel = hand == Hand.Left ? _leftVel : _rightVel;
            var fling = _grip.Release(hand, ToT(vel), maxFling: 4f);
            if (hand == Hand.Left) _leftAttach = null; else _rightAttach = null;

            if (!_grip.IsClimbing)
            {
                SuspendMove(false);
                var flingV = new Vector3(fling.X, fling.Y, fling.Z);
                Debug.Log("ZIPTIDE: CLIMB_RELEASE fling=" + flingV.magnitude.ToString("F1"));
                // A/B gate (1.4f): the comfort-clamped launch-off, OFF by default until Terry's device
                // pass says it feels good — `adb shell` or the dev menu can flip ziptide_climb_fling=1.
                if (PlayerPrefs.GetInt("ziptide_climb_fling", 0) == 1 && flingV.sqrMagnitude > 0.25f)
                {
                    _flingVel = flingV;
                    _flingT = 0f;
                }
            }
        }

        // ── The release fling (pref-gated ballistic mover) ───────────────────
        private Vector3 _flingVel;
        private float _flingT = -1f;          // <0 = idle
        private const float FlingMaxSeconds = 2.0f;

        private void TickFling(float dt)
        {
            if (_flingT < 0f || _rig == null) return;
            _flingT += dt;
            _flingVel += Vector3.down * 9.81f * dt;
            Vector3 next = _rig.position + _flingVel * dt;
            // Land when a short down-ray finds ground under the next position (or on timeout —
            // the global fall-safety net owns anything weirder).
            if (Physics.Raycast(next + Vector3.up * 0.1f, Vector3.down, out var hit, 0.4f,
                                ~0, QueryTriggerInteraction.Ignore) || _flingT > FlingMaxSeconds)
            {
                if (_flingT <= FlingMaxSeconds) next.y = hit.point.y + 0.05f;
                Debug.Log("ZIPTIDE: CLIMB_FLING_LAND t=" + _flingT.ToString("F2"));
                _flingT = -1f;
            }
            _rig.position = next;
        }

        private void Update()
        {
            float dt = Mathf.Max(1e-4f, Time.deltaTime);
            TickFling(dt);

            // Track hand velocities (for the release fling) + drive the rig from the DRIVING hand.
            if (_leftAttach != null)
            {
                _leftVel = (_leftAttach.position - _leftLastPos) / dt;
                _leftLastPos = _leftAttach.position;
            }
            if (_rightAttach != null)
            {
                _rightVel = (_rightAttach.position - _rightLastPos) / dt;
                _rightLastPos = _rightAttach.position;
            }
            if (!_grip.IsClimbing || _rig == null) return;

            Transform driver = _grip.Driver == Hand.Left ? _leftAttach : _rightAttach;
            if (driver == null) return;
            var delta = _grip.MoveGrip(_grip.Driver, ToT(driver.position));
            _rig.position += new Vector3(delta.X, delta.Y, delta.Z); // delta-translate — NEVER parent

            // The rig moved, so the driving hand moved WITH it — resync the stored grip position to
            // the hand's new world pos or next frame double-counts the rig's own motion.
            _grip.MoveGrip(_grip.Driver, ToT(driver.position));
            if (_grip.Driver == Hand.Left && _leftAttach != null) _leftLastPos = _leftAttach.position;
            if (_grip.Driver == Hand.Right && _rightAttach != null) _rightLastPos = _rightAttach.position;
        }

        private void SuspendMove(bool suspend)
        {
            if (_move == null && _rig != null)
                _move = _rig.GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);
            if (_move == null || _movedSuspended == suspend) return;
            _move.enabled = !suspend;
            _movedSuspended = suspend;
        }

        private static TVec3 ToT(Vector3 v) => new TVec3(v.x, v.y, v.z);
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Traversal;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 1.4c — the LIFT scene translator for the pure <see cref="LiftCycle"/> clock. Place
    /// one, Init with stops, and it self-builds a platform that cycles dwell→travel→dwell. The rider
    /// isn't parented (the locked law): while the player stands on the platform (XZ inside the deck,
    /// feet within a step of its top), the rig is delta-translated by the platform's own per-frame
    /// delta — step off mid-ride and you simply stop riding. Position is a pure function of
    /// Time.time, so the platform can't drift and late joiners agree (the core's contract).
    /// </summary>
    public class LiftRuntime : MonoBehaviour
    {
        [Tooltip("Ordered stops the platform cycles through (world space).")]
        public Vector3[] stops;
        [Tooltip("Travel speed m/s between stops (comfort-capped by data, 2 m/s default).")]
        public float travelSpeed = 2f;
        [Tooltip("Doors-open pause at each stop.")]
        public float dwellSeconds = 3f;
        [Tooltip("Deck size (x = width, y = depth).")]
        public Vector2 deckSize = new Vector2(1.8f, 1.8f);

        private LiftCycle _cycle;
        private Transform _deck;
        private Transform _rig;
        private Vector3 _lastPos;
        private bool _built;

        /// <summary>Author/patcher entry point (public Init — the no-reflection law).</summary>
        public void Init(Vector3[] cycleStops, float speed = 2f, float dwell = 3f)
        {
            stops = cycleStops;
            travelSpeed = speed;
            dwellSeconds = dwell;
        }

        private void Start()
        {
            if (_built) return;
            if (stops == null || stops.Length < 2)
            {
                Debug.LogWarning("ZIPTIDE: LIFT_DEGENERATE needs >=2 stops — disabled");
                enabled = false;
                return;
            }
            var tStops = new TVec3[stops.Length];
            for (int i = 0; i < stops.Length; i++) tStops[i] = new TVec3(stops[i].x, stops[i].y, stops[i].z);
            _cycle = new LiftCycle(tStops, travelSpeed, dwellSeconds);

            var deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.name = "LiftDeck";
            deck.transform.SetParent(transform, true);
            deck.transform.localScale = new Vector3(deckSize.x, 0.12f, deckSize.y);
            ItemFactory.ApplyURPColor(deck, new Color(0.75f, 0.55f, 0.2f)); // the traversal signal palette
            _deck = deck.transform;

            var p0 = _cycle.Evaluate(Time.time, out _, out _);
            _deck.position = new Vector3(p0.X, p0.Y, p0.Z);
            _lastPos = _deck.position;
            _built = true;
            Debug.Log("ZIPTIDE: LIFT_READY stops=" + stops.Length);
        }

        private void Update()
        {
            if (!_built) return;
            var p = _cycle.Evaluate(Time.time, out bool dwelling, out _);
            _deck.position = new Vector3(p.X, p.Y, p.Z);
            Vector3 delta = _deck.position - _lastPos;
            _lastPos = _deck.position;
            if (delta.sqrMagnitude < 1e-10f) return;

            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                _rig = rig != null ? rig.transform : null;
                if (_rig == null) return;
            }

            // Riding = feet on the deck: XZ within the deck, and rig base within a step of its top.
            Vector3 local = _rig.position - _deck.position;
            bool onDeck = Mathf.Abs(local.x) <= deckSize.x * 0.5f + 0.15f
                       && Mathf.Abs(local.z) <= deckSize.y * 0.5f + 0.15f
                       && local.y >= -0.05f && local.y <= 1.0f;
            if (onDeck) _rig.position += delta; // delta-translate — NEVER parent the rig
        }
    }

    /// <summary>
    /// HARDWIRING 1.4c — the JUMP PAD translator for the pure <see cref="JumpPad"/> ballistics. Step
    /// on the pad and it flies the rig along the precomputed arc (<see cref="JumpPad.PositionAt"/> —
    /// one source of truth with the tests): fixed apex margin so every pad feels the same, continuous
    /// locomotion suspended for the ~1s of flight (stick input mid-arc fights the launch), rig
    /// delta-positioned each frame, never parented. The ComfortVignette engages from rig motion.
    /// </summary>
    public class JumpPadRuntime : MonoBehaviour
    {
        [Tooltip("Where the arc lands (world space).")]
        public Vector3 target;
        [Tooltip("Pad trigger radius.")]
        public float padRadius = 0.8f;
        [Tooltip("Seconds before the pad can launch again.")]
        public float cooldown = 1.5f;

        private Transform _rig;
        private ActionBasedContinuousMoveProvider _move;
        private Vector3 _launchFrom;
        private TVec3 _velocity;
        private float _flightT = -1f;   // <0 = idle
        private float _flightSeconds;
        private float _nextLaunchAt;
        private bool _built;

        /// <summary>Author/patcher entry point.</summary>
        public void Init(Vector3 landingTarget)
        {
            target = landingTarget;
        }

        private void Start()
        {
            if (_built) return;
            _built = true;
            var pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = "JumpPadDisc";
            var col = pad.GetComponent<Collider>();
            if (col != null) Object.Destroy(col); // proximity-triggered, not physics-triggered
            pad.transform.SetParent(transform, true);
            pad.transform.position = transform.position;
            pad.transform.localScale = new Vector3(padRadius * 2f, 0.06f, padRadius * 2f);
            ItemFactory.ApplyURPColor(pad, new Color(0.4f, 0.9f, 0.55f)); // launch green
            Debug.Log("ZIPTIDE: JUMPPAD_READY");
        }

        private void Update()
        {
            if (!_built) return;
            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                _rig = rig != null ? rig.transform : null;
                if (_rig == null) return;
            }

            if (_flightT >= 0f)
            {
                _flightT += Time.deltaTime;
                float t = Mathf.Min(_flightT, _flightSeconds);
                var p = JumpPad.PositionAt(
                    new TVec3(_launchFrom.x, _launchFrom.y, _launchFrom.z), _velocity, 9.81f, t);
                _rig.position = new Vector3(p.X, p.Y, p.Z);
                if (_flightT >= _flightSeconds) Land();
                return;
            }

            if (Time.time < _nextLaunchAt) return;
            Vector3 flat = _rig.position - transform.position; flat.y = 0f;
            bool onPad = flat.magnitude <= padRadius
                      && Mathf.Abs(_rig.position.y - transform.position.y) <= 0.6f;
            if (onPad) Launch();
        }

        private void Launch()
        {
            _launchFrom = _rig.position;
            var from = new TVec3(_launchFrom.x, _launchFrom.y, _launchFrom.z);
            var to = new TVec3(target.x, target.y, target.z);
            _velocity = JumpPad.LaunchVelocity(from, to);

            // Flight time re-derives from the same apex math the core used (one truth, no drift).
            float g = 9.81f;
            float apexY = Mathf.Max(_launchFrom.y, target.y) + 2f;
            _flightSeconds = Mathf.Sqrt(2f * (apexY - _launchFrom.y) / g)
                           + Mathf.Sqrt(2f * (apexY - target.y) / g);
            _flightT = 0f;
            _nextLaunchAt = Time.time + _flightSeconds + cooldown;
            SuspendMove(true);
            Debug.Log("ZIPTIDE: JUMPPAD_LAUNCH t=" + _flightSeconds.ToString("F2") + "s");
        }

        private void Land()
        {
            _flightT = -1f;
            SuspendMove(false);
            Debug.Log("ZIPTIDE: JUMPPAD_LAND");
        }

        private void SuspendMove(bool suspend)
        {
            if (_move == null && _rig != null)
                _move = _rig.GetComponentInChildren<ActionBasedContinuousMoveProvider>(true);
            if (_move != null) _move.enabled = !suspend;
        }
    }
}

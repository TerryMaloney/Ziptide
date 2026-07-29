using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Content.Ship;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// P4b FREE-FLIGHT v1 — the thin scene translator over the pure <see cref="FlightModel"/>
    /// (SPACEFLIGHT_PHYSICS law: fly the HULL, never the deck). The rig NEVER moves in flight:
    /// taking the helm teleports you to the seat, suspends walking locomotion, and every tick
    /// renders the WORLD's inverse pose on the lane-content root — you see space move past the
    /// static cockpit. Comfort is triple-locked: FlightModel's math (roll never rests — barrel
    /// roll only, snap yaw, pitch clamp, reverse fraction, soft-walled lane), FlightInputCore's
    /// flick latch, and apparent motion reported to the ONE ComfortVignette. Controls mirror the
    /// on-foot scheme: stick = fly (back = reverse), L3/A = boost (the sprint finger), X/B =
    /// barrel roll. Ring course (FlightCourseCore) → dock or fly on; DOCK exits flight; RETURN
    /// travels home through TravelCoordinator (the only legal path).
    /// SPACE COMBAT 3.1 rides along: RT fires a stun bolt (SpaceCombatCore aim cone — fly to aim,
    /// don't pixel-hunt), drones DISABLE non-lethally and become salvage you fly close to claim.
    /// All fields are SERIALIZED at edit time by ScenePatcherSpaceLane (gotcha #7).
    /// Logs FLIGHT_MODE / FLIGHT_RING / FLIGHT_COURSE_DONE / FLIGHT_RETURN / FLIGHT_FIRE /
    /// FLIGHT_DISABLE / FLIGHT_SALVAGE.
    /// </summary>
    public class ShipFlightRuntime : MonoBehaviour
    {
        [Tooltip("Root of everything that IS the world in flight (rings, rocks, vista). Moves inversely.")]
        [SerializeField] private Transform laneContent;

        [Tooltip("Ring course positions in lane space (authored by the patcher).")]
        [SerializeField] private List<Vector3> ringPositions = new List<Vector3>();

        [SerializeField] private float ringRadius = 7f;

        [Tooltip("Clear inner opening of a catch ring (measured spec: 12 m bore = 6 m radius).")]
        [SerializeField] private float ringBoreRadius = 6f;

        [Tooltip("Does this lane have a floor? False in orbit; true for any atmospheric leg.")]
        [SerializeField] private bool boundsHasGround;

        [Tooltip("Height of that floor in lane space (ignored when boundsHasGround is off).")]
        [SerializeField] private float boundsGroundY;

        [Tooltip("Scene TravelCoordinator returns to from the RETURN panel.")]
        [SerializeField] private string returnScene = "W000_DriftIn";

        [Tooltip("Optional stat source — maps cruise/boost/turn onto FlightParams (null = defaults).")]
        [SerializeField] private ShipDefinition shipDefinition;

        private const float MaxDataBoost = 3f;    // data can tune boost, never past this
        private const float MinPitchRateDeg = 8f; // even the barge-est hauler still steers

        private const float SeatStrayExit = 2.5f; // rig moved away (respawn etc.) → auto-dock

        private bool _flying;
        private FlightState _state;
        private FlightParams _params;
        private FlightYawLatch _yawLatch;
        private FlightCourseCore _course;
        private ComfortVignette _vignette;
        private PlayerRigPersistence _rig;

        private Vector3 _seatWorldPos;
        private Vector3 _laneHomePos;
        private Quaternion _laneHomeRot;

        private InputAction _leftStick;
        private InputAction _rightStick;
        private InputAction _boostStickClick;  // L3 — same finger as sprint on foot
        private InputAction _boostButton;      // A — the CONTROLS_AND_FLIGHT boost button
        private InputAction _rollLeftButton;   // X
        private InputAction _rollRightButton;  // B
        private InputAction _fireAction;       // RT — fire ship weapon (CONTROLS_AND_FLIGHT)

        private SpaceTargetRuntime[] _targets = System.Array.Empty<SpaceTargetRuntime>();
        private float _lastFireTime = float.NegativeInfinity;

        // THE BOUNDS LADDER (FlightBoundsCore): the corridor polyline the swept lane follows, the
        // hulls worth not hitting, and RILL's memory of what she has already said about them.
        private FlightBoundsParams _bounds;
        private FlightBoundsVoiceState _boundsVoice;
        private Vector3[] _corridorPath = System.Array.Empty<Vector3>();
        private Transform[] _obstacles = System.Array.Empty<Transform>();
        private float[] _obstacleRadii = System.Array.Empty<float>();
        private const float ObstacleMinRadius = 1.5f; // scrap smaller than this is scenery, not traffic

        private TextMesh _statusText;
        private GameObject _returnPanel;
        private readonly List<Behaviour> _suspended = new List<Behaviour>();

        /// <summary>0-based index of the ring to fly NEXT (== rings passed so far); equals
        /// RingCountTotal once the course is complete. Drives RingCourseLightsRuntime.</summary>
        public int NextRingIndex => _course != null ? _course.NextRing : 0;

        /// <summary>How many rings the course has (0 before the patcher-authored list loads).</summary>
        public int RingCountTotal => _course != null ? _course.RingCount : 0;

        /// <summary>
        /// Signed yaw bearing from the nose to the next ring, degrees (negative = left). False when
        /// there is nothing to steer toward — not flying, or the course is finished. The helm
        /// compass reads this; the math itself lives in <see cref="CompassRibbonCore"/>.
        /// </summary>
        public bool TryGetCourseBearing(out float bearingDegrees)
        {
            bearingDegrees = 0f;
            if (!_flying || _course == null || _course.IsComplete) return false;
            int next = _course.NextRing;
            if (next < 0 || next >= ringPositions.Count) return false;
            bearingDegrees = CompassRibbonCore.BearingDegrees(
                FlightModel.Forward(_state), _state.position, ringPositions[next]);
            return true;
        }

        /// <summary>ShipDefinition → FlightParams (pure; pinned by ShipFlightParamsTests). The
        /// definition's cruise is the lane max and its boost multiplier carries over (clamped to
        /// MaxDataBoost); comfort caps (pitch clamp, snap yaw, lane radius, reverse fraction, roll
        /// rate) stay at the reviewed defaults — data can slow a ship down, never uncap comfort.</summary>
        public static FlightParams ParamsFrom(ShipDefinition def)
        {
            var p = FlightParams.Default;
            if (def == null) return p;
            if (def.cruiseSpeed > 0f) p.maxSpeed = def.cruiseSpeed;
            if (def.turnRateDegrees > 0f)
                p.pitchRateDeg = Mathf.Min(def.turnRateDegrees, FlightParams.Default.pitchRateDeg);
            if (def.boostMultiplier > 0f)
                p.boostMultiplier = Mathf.Clamp(def.boostMultiplier, 1f, MaxDataBoost);
            return p;
        }

        /// <summary>Resolved loadout → FlightParams (the SHIP-MORE #1 seam with the ship lane):
        /// the hangar's ShipStats drive how the ship actually FLIES. Speed = cruise; Boost carries
        /// (clamped to MaxDataBoost); Handling 0..10 maps onto the pitch rate with 10 = the comfort
        /// ceiling — stats make a ship statelier or livelier, never less comfortable. Snap yaw,
        /// pitch clamp, lane radius, reverse fraction, roll rate stay comfort constants.</summary>
        public static FlightParams ParamsFrom(ShipStats stats)
        {
            var p = FlightParams.Default;
            if (stats.Speed > 0f) p.maxSpeed = stats.Speed;
            if (stats.Boost > 0f) p.boostMultiplier = Mathf.Clamp(stats.Boost, 1f, MaxDataBoost);
            p.pitchRateDeg = Mathf.Clamp(FlightParams.Default.pitchRateDeg * (stats.Handling / 10f),
                MinPitchRateDeg, FlightParams.Default.pitchRateDeg);
            return p;
        }

        /// <summary>The live params source: the player's equipped hangar loadout when one exists
        /// (so a refit changes the next flight immediately), else the serialized ShipDefinition,
        /// else defaults. Returns the chassis id it resolved (null = definition/defaults).</summary>
        private FlightParams ResolveParams(out string chassisId)
        {
            chassisId = null;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            string equipped = profile != null ? ShipLocker.GetEquipped(profile, "chassis") : null;
            if (string.IsNullOrEmpty(equipped)) return ParamsFrom(shipDefinition);

            var chassis = ShipChassisPreset.Find(equipped);
            chassisId = chassis.Id;
            var modules = new List<ShipModulePreset>();
            foreach (var id in ShipLocker.EquippedModules(profile, chassis.SlotIds))
            {
                var m = ShipModulePreset.Find(id);
                if (m != null) modules.Add(m);
            }
            return ParamsFrom(ShipLoadoutCore.Resolve(chassis, modules));
        }

        private void Start()
        {
            _params = ParamsFrom(shipDefinition);
            _course = new FlightCourseCore(ringPositions.ToArray(), ringRadius);
            if (laneContent != null)
            {
                _laneHomePos = laneContent.position;
                _laneHomeRot = laneContent.rotation;
            }

            _leftStick = new InputAction("ZiptideFlightThrottle", InputActionType.Value);
            _leftStick.AddBinding("<XRController>{LeftHand}/thumbstick");
            _rightStick = new InputAction("ZiptideFlightSteer", InputActionType.Value);
            _rightStick.AddBinding("<XRController>{RightHand}/thumbstick");
            _boostStickClick = new InputAction("ZiptideFlightBoostL3", InputActionType.Button);
            _boostStickClick.AddBinding("<XRController>{LeftHand}/thumbstickClicked"); // L3, like sprint
            _boostButton = new InputAction("ZiptideFlightBoostA", InputActionType.Button);
            _boostButton.AddBinding("<XRController>{RightHand}/primaryButton");        // A
            _rollLeftButton = new InputAction("ZiptideFlightRollL", InputActionType.Button);
            _rollLeftButton.AddBinding("<XRController>{LeftHand}/primaryButton");      // X
            _rollRightButton = new InputAction("ZiptideFlightRollR", InputActionType.Button);
            _rollRightButton.AddBinding("<XRController>{RightHand}/secondaryButton");  // B
            _fireAction = new InputAction("ZiptideFlightFire", InputActionType.Button);
            _fireAction.AddBinding("<XRController>{RightHand}/trigger");               // RT

            BuildHelm();
        }

        private void OnDestroy()
        {
            _leftStick?.Dispose();
            _rightStick?.Dispose();
            _boostStickClick?.Dispose();
            _boostButton?.Dispose();
            _rollLeftButton?.Dispose();
            _rollRightButton?.Dispose();
            _fireAction?.Dispose();
        }

        private void BuildHelm()
        {
            var console = GameObject.CreatePrimitive(PrimitiveType.Cube);
            console.name = "FlightHelmConsole";
            console.transform.SetParent(transform, false);
            console.transform.localPosition = new Vector3(0f, 0.55f, 0.9f);
            console.transform.localScale = new Vector3(0.55f, 1.1f, 0.3f);
            ItemFactory.ApplyURPColor(console, new Color(0.14f, 0.20f, 0.26f));

            MakeTile(console.transform, "Tile_TAKE_HELM", new Vector3(0f, 0.42f, -0.6f),
                new Color(0.2f, 0.65f, 0.5f), "TAKE THE HELM", EnterFlight);
            MakeTile(console.transform, "Tile_DOCK", new Vector3(0f, 0.12f, -0.6f),
                new Color(0.6f, 0.5f, 0.2f), "DOCK", ExitFlight);

            var statusGo = new GameObject("FlightStatus");
            statusGo.transform.SetParent(console.transform, false);
            statusGo.transform.localPosition = new Vector3(0f, 0.75f, -0.55f);
            statusGo.transform.localScale = new Vector3(1f / 0.55f, 1f / 1.1f, 1f / 0.3f) * 0.5f;
            _statusText = statusGo.AddComponent<TextMesh>();
            _statusText.characterSize = 0.03f;
            _statusText.fontSize = 48;
            _statusText.anchor = TextAnchor.MiddleCenter;
            _statusText.alignment = TextAlignment.Center;
            _statusText.color = new Color(0.7f, 0.95f, 1f);
            _statusText.text = "FLIGHT TRIAL\nrings: " + _course.RingCount;

            _returnPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _returnPanel.name = "Tile_RETURN";
            _returnPanel.transform.SetParent(console.transform, false);
            _returnPanel.transform.localPosition = new Vector3(0f, -0.18f, -0.6f);
            _returnPanel.transform.localScale = new Vector3(0.72f, 0.2f, 0.4f);
            ItemFactory.ApplyURPColor(_returnPanel, new Color(0.25f, 0.45f, 0.8f));
            AddTileLabel(_returnPanel.transform, "RETURN HOME");
            var ret = _returnPanel.AddComponent<XRSimpleInteractable>();
            WireManager(ret);
            ret.selectEntered.AddListener(_ =>
            {
                Debug.Log("ZIPTIDE: FLIGHT_RETURN target=" + returnScene);
                if (_flying) ExitFlight();
                TravelCoordinator.TravelTo(returnScene);
            });

            ObjectiveBeacon.Attach(console, new Color(0.3f, 0.8f, 0.9f), 6f);
        }

        private void MakeTile(Transform parent, string name, Vector3 localPos, Color color,
            string label, System.Action onPress)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = name;
            tile.transform.SetParent(parent, false);
            tile.transform.localPosition = localPos;
            tile.transform.localScale = new Vector3(0.72f, 0.2f, 0.4f);
            ItemFactory.ApplyURPColor(tile, color);
            AddTileLabel(tile.transform, label);
            var interactable = tile.AddComponent<XRSimpleInteractable>();
            WireManager(interactable);
            interactable.selectEntered.AddListener(_ => onPress());
        }

        private static void AddTileLabel(Transform tile, string text)
        {
            var label = new GameObject("Label");
            label.transform.SetParent(tile, false);
            label.transform.localPosition = new Vector3(0f, 0f, -0.55f);
            label.transform.localScale = new Vector3(1f / 0.72f, 1f / 0.2f, 1f / 0.4f) * 0.3f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.03f;
            tm.fontSize = 56;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(1f, 0.95f, 0.8f);
        }

        private static void WireManager(XRBaseInteractable interactable)
        {
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
        }

        // ── Flight mode ────────────────────────────────────────────────────────────────────────────

        private void EnterFlight()
        {
            if (_flying || laneContent == null) return;
            _rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (_rig == null) return;

            _seatWorldPos = transform.position + Vector3.up * 0.1f;
            TeleportRig(_rig, _seatWorldPos);
            SuspendLocomotion(_rig);

            // Lane space == world space while the content sits at home pose, so the ship state
            // starts AT the seat and the first rendered frame is exactly the parked view.
            laneContent.SetPositionAndRotation(_laneHomePos, _laneHomeRot);
            _state = new FlightState { position = _seatWorldPos };
            _yawLatch = new FlightYawLatch { Armed = true };
            _params = ResolveParams(out string chassisId); // hangar refit changes THIS flight
            _course = new FlightCourseCore(ringPositions.ToArray(), ringRadius);
            _vignette = Object.FindObjectOfType<ComfortVignette>();

            _leftStick.Enable();
            _rightStick.Enable();
            _boostStickClick.Enable();
            _boostButton.Enable();
            _rollLeftButton.Enable();
            _rollRightButton.Enable();
            _fireAction.Enable();
            _targets = laneContent.GetComponentsInChildren<SpaceTargetRuntime>(true);
            _lastFireTime = float.NegativeInfinity;
            BuildBounds();
            _flying = true;
            UpdateStatus();
            Debug.Log("ZIPTIDE: FLIGHT_MODE on scene=" + gameObject.scene.name +
                      " rings=" + _course.RingCount + " maxSpeed=" + _params.maxSpeed +
                      " boost=" + _params.boostMultiplier +
                      " chassis=" + (chassisId ?? "definition"));
        }

        private void ExitFlight()
        {
            if (!_flying) return;
            _flying = false;
            _leftStick.Disable();
            _rightStick.Disable();
            _boostStickClick.Disable();
            _boostButton.Disable();
            _rollLeftButton.Disable();
            _rollRightButton.Disable();
            _fireAction.Disable();
            if (laneContent != null)
                laneContent.SetPositionAndRotation(_laneHomePos, _laneHomeRot);
            ResumeLocomotion();
            Debug.Log("ZIPTIDE: FLIGHT_MODE off rings=" + _course.NextRing + "/" + _course.RingCount);
        }

        private void Update()
        {
            if (!_flying || laneContent == null) return;

            if (_rig == null || (_rig.transform.position - _seatWorldPos).magnitude > SeatStrayExit)
            {
                ExitFlight(); // respawned / pulled off the seat — never fly a rig that walked away
                return;
            }

            Vector2 rawRight = _rightStick.ReadValue<Vector2>();
            var frame = FlightInputCore.Shape(
                _leftStick.ReadValue<Vector2>(), rawRight, ref _yawLatch, Time.time);
            bool boost = _boostStickClick.IsPressed() || _boostButton.IsPressed();

            if (frame.YawSnap != 0)
            {
                _state = FlightModel.SnapYaw(_state, _params, frame.YawSnap);
                // DS-12 evidence (log-only): one line per emitted snap so a device capture can
                // separate repeat-cadence, duplicate-input, and transform faults. dtMs flags frame
                // spikes coinciding with a visual jump.
                Debug.Log("ZIPTIDE: FLIGHT_TRACE snap=" + frame.YawSnap
                    + " rawX=" + rawRight.x.ToString("F2")
                    + " latchArmed=" + _yawLatch.Armed
                    + " yaw=" + _state.yawDeg.ToString("F1")
                    + " dtMs=" + (Time.deltaTime * 1000f).ToString("F1"));
            }
            if (_rollLeftButton.WasPressedThisFrame())
            {
                _state = FlightModel.StartBarrelRoll(_state, -1);
                Debug.Log("ZIPTIDE: FLIGHT_ROLL dir=left");
            }
            else if (_rollRightButton.WasPressedThisFrame())
            {
                _state = FlightModel.StartBarrelRoll(_state, +1);
                Debug.Log("ZIPTIDE: FLIGHT_ROLL dir=right");
            }
            _state = FlightModel.Tick(_state, _params, frame.Throttle, frame.Pitch, frame.Strafe,
                boost, Time.deltaTime);
            TickBounds(Time.deltaTime);

            // The rig stays still; the WORLD wears the inverse of the ship's pose (incl. any roll).
            Quaternion inv = Quaternion.Inverse(FlightModel.Orientation(_state));
            laneContent.SetPositionAndRotation(
                _seatWorldPos + inv * (_laneHomePos - _state.position), inv * _laneHomeRot);

            if (_vignette != null)
                _vignette.ReportExternalMotion(
                    Mathf.Max(Mathf.Abs(_state.speed) / Mathf.Max(1f, _params.maxSpeed),
                        Mathf.Abs(frame.Strafe) * _params.strafeFraction),
                    (frame.YawSnap != 0 ? 1f : 0f) + Mathf.Abs(frame.Pitch) * 0.4f
                        + (_state.rollDirection != 0 ? 1f : 0f)); // barrel roll = full tunnel pulse

            if (_course.Advance(_state.position))
            {
                Debug.Log("ZIPTIDE: FLIGHT_RING " + _course.NextRing + "/" + _course.RingCount);
                // The passed gate's sequencer goes steady green (CLEAR) on its own —
                // RingCourseLightsRuntime watches NextRingIndex and owns every lamp.
                UpdateStatus();
                if (_course.IsComplete)
                {
                    Debug.Log("ZIPTIDE: FLIGHT_COURSE_DONE rings=" + _course.RingCount);
                    // Past the last gate is THE OVERRUN — where everything that missed the catch
                    // ends up, which is also why there is anything worth salvaging out here.
                    var rill = FindObjectOfType<Ziptide.Gameplay.RillCompanion>();
                    if (rill != null) rill.SayById("CATCH_OVERRUN");
                }
            }

            TickCombat();
        }

        // ── The bounds ladder: warned, then nudged, then held ──────────────────────────────────────
        //
        // Terry, 2026-07-29: "if it gets too close to a building, too close to the ground… some sort
        // of auto correction if you leave the zone and then some sort of message from Rill… there
        // should be a variety of responses from Rill so it doesn't get too repetitive and stale."
        //
        // Before this, the ONLY answer the game had was FlightModel's silent 1.8 km sphere snap:
        // no warning, no gradation, nobody said anything. Now the world objects in tiers, and the
        // swept lane itself is advisory-only so wandering off it stays a choice.

        private void BuildBounds()
        {
            _bounds = FlightBoundsParams.Default;
            _bounds.laneRadius = _params.laneRadius;
            _bounds.hasGround = boundsHasGround;
            _bounds.groundY = boundsGroundY;
            _boundsVoice = default;

            // The swept corridor IS the ring line — the fiction (rings mark the trajectory) and the
            // geometry are the same object, so there is nothing to keep in sync.
            _corridorPath = new Vector3[ringPositions.Count + 1];
            _corridorPath[0] = _seatWorldPos;
            for (int i = 0; i < ringPositions.Count; i++) _corridorPath[i + 1] = ringPositions[i];

            var bodies = new List<Transform>();
            var radii = new List<float>();
            foreach (var d in laneContent.GetComponentsInChildren<DriftTumbleRuntime>(true))
                AddObstacle(bodies, radii, d.transform);
            foreach (var t in _targets)
                if (t != null) AddObstacle(bodies, radii, t.transform);
            var wreck = laneContent.Find("__ARTIFACT_HALF_A/WreckFragment");
            if (wreck != null) AddObstacle(bodies, radii, wreck);

            _obstacles = bodies.ToArray();
            _obstacleRadii = radii.ToArray();
            Debug.Log("ZIPTIDE: FLIGHT_BOUNDS_READY hulls=" + _obstacles.Length
                + " corridor=" + _corridorPath.Length + " ground=" + _bounds.hasGround);
        }

        /// <summary>Anything smaller than a ship's nose is scenery you fly THROUGH the middle of —
        /// counting the Find's scrap shell as traffic would have RILL nagging over the discovery.</summary>
        private static void AddObstacle(List<Transform> bodies, List<float> radii, Transform t)
        {
            if (t == null) return;
            var renderers = t.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;
            var b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
            float radius = b.extents.magnitude;
            if (radius < ObstacleMinRadius) return;
            bodies.Add(t);
            radii.Add(radius);
        }

        private void TickBounds(float dt)
        {
            var sample = SampleBounds();
            var reading = FlightBoundsCore.Evaluate(_state.position, sample, _bounds);
            _state = FlightBoundsCore.Apply(_state, reading, _bounds, dt);

            if (!FlightBoundsVoiceCore.ShouldSpeak(ref _boundsVoice, reading, Time.time, out string lineId))
                return;

            Debug.Log("ZIPTIDE: FLIGHT_BOUNDS kind=" + reading.Kind + " level=" + reading.Level
                + " sev=" + reading.Severity01.ToString("F2") + " line=" + lineId);
            var rill = FindObjectOfType<Ziptide.Gameplay.RillCompanion>();
            if (rill != null) rill.SayById(lineId);
        }

        private FlightBoundsSample SampleBounds()
        {
            var sample = FlightBoundsSample.Empty;
            sample.CorridorDistance = FlightBoundsCore.CorridorDistance(_state.position, _corridorPath);

            for (int i = 0; i < _obstacles.Length; i++)
            {
                var t = _obstacles[i];
                if (t == null || !t.gameObject.activeInHierarchy) continue;
                Vector3 lanePos = laneContent.InverseTransformPoint(t.position);
                Vector3 away = _state.position - lanePos;
                float clearance = away.magnitude - _obstacleRadii[i];
                if (clearance >= sample.StructureClearance) continue;
                sample.StructureClearance = clearance;
                sample.StructureAway = away.sqrMagnitude > 1e-4f ? away.normalized : Vector3.up;
            }

            // Gates: only the ring whose slab we are actually inside can be clipped, and the ring's
            // facing comes from the same PathAxis the patcher orients it with.
            for (int i = 0; i < ringPositions.Count; i++)
            {
                Vector3 axis = FlightBoundsCore.PathAxis(_corridorPath, i + 1);
                if (!FlightBoundsCore.TryGateFill(_state.position, ringPositions[i], axis,
                        ringBoreRadius, _bounds.gateSlabHalfDepth, out float fill, out Vector3 outward))
                    continue;
                if (sample.GateFill01 >= 0f && fill <= sample.GateFill01) continue;
                sample.GateFill01 = fill;
                sample.GateOutward = outward;
            }

            return sample;
        }

        // ── Space combat 3.1: fire on RT, hits resolve in lane space, wrecks salvage on approach ──

        private void TickCombat()
        {
            if (_targets.Length == 0) return;
            float now = Time.time;

            // Tell every drone where the pilot is — the only input the reaction layer needs
            // (SpaceTargetReactionCore: wake, brighten, and slide when hit).
            foreach (var t in _targets)
                if (t != null && t.gameObject.activeSelf) t.ObservePilot(_state.position);

            if (_fireAction.IsPressed() && SpaceCombatCore.CanFire(_lastFireTime, now))
            {
                _lastFireTime = now;
                Vector3 forward = FlightModel.Forward(_state);
                SpaceTargetRuntime hit = null;
                float best = float.MaxValue;
                foreach (var t in _targets)
                {
                    if (t == null || !t.gameObject.activeSelf || t.Disabled) continue;
                    Vector3 lanePos = laneContent.InverseTransformPoint(t.transform.position);
                    float d = (lanePos - _state.position).sqrMagnitude;
                    if (d < best && SpaceCombatCore.InAimCone(_state.position, forward, lanePos))
                    {
                        best = d;
                        hit = t;
                    }
                }

                // The cockpit never rotates (the WORLD does), so the bolt always streaks straight
                // out the front window in world space — from the helm, along its facing.
                Vector3 from = transform.position + Vector3.up * 1.1f + transform.forward * 1.4f;
                float len = hit != null ? Mathf.Sqrt(best) : SpaceCombatCore.BoltRange;
                TracerFx.Spawn(from, from + transform.forward * Mathf.Min(len, SpaceCombatCore.BoltRange),
                    new Color(0.4f, 0.9f, 1f, 0.9f), 0.03f, 0.1f);
                Debug.Log("ZIPTIDE: FLIGHT_FIRE hit=" + (hit != null ? hit.name : "none"));

                if (hit != null && hit.TakeHit(SpaceCombatCore.BoltDamage, now))
                {
                    Debug.Log("ZIPTIDE: FLIGHT_DISABLE target=" + hit.name);
                    Ziptide.Core.FlightSignals.TargetDisabled?.Invoke(hit.name);   // announced append (tf-space1): Tidefront space-defense counts these
                    UpdateStatus();
                }
            }

            foreach (var t in _targets)
            {
                if (t == null || !t.gameObject.activeSelf || !t.Disabled || t.Salvaged) continue;
                Vector3 lanePos = laneContent.InverseTransformPoint(t.transform.position);

                // The wreck answers the approach BEFORE it pays, so "fly closer" is something the
                // pilot discovers rather than something the status text has to tell them.
                t.ReportSalvageApproach(SpaceCombatCore.SalvageApproach01(_state.position, lanePos));

                if (SpaceCombatCore.InSalvageRange(_state.position, lanePos))
                {
                    Vector3 wreckWorld = t.transform.position;
                    double granted = t.Salvage();
                    Debug.Log("ZIPTIDE: FLIGHT_SALVAGE target=" + t.name + " granted=" + granted.ToString("F0"));
                    SalvageTractorFx.Play(wreckWorld, transform.position + Vector3.up * 1.1f);
                    UpdateStatus();
                }
            }
        }

        private void UpdateStatus()
        {
            if (_statusText == null) return;
            string targets = "";
            if (_targets.Length > 0)
            {
                int down = 0;
                foreach (var t in _targets) if (t != null && (t.Salvaged || t.Disabled)) down++;
                targets = "\nRT fire - drones down " + down + "/" + _targets.Length + " (fly close to salvage)";
            }
            _statusText.text = _course.IsComplete
                ? "COURSE COMPLETE\ndock + return home" + targets
                : "RINGS " + _course.NextRing + "/" + _course.RingCount
                  + "\nleft stick fly + slide (back = reverse)"
                  + "\nright stick steer - L3/A boost - X/B barrel roll" + targets;
        }

        // ── Rig plumbing (ShipBoardingStation patterns — teleport, never parent) ───────────────────

        private static void TeleportRig(PlayerRigPersistence rig, Vector3 worldPos)
        {
            var cc = rig.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            rig.transform.position = worldPos;
            if (cc != null) cc.enabled = true;
        }

        private void SuspendLocomotion(PlayerRigPersistence rig)
        {
            _suspended.Clear();
            Collect(rig.GetComponentsInChildren<ActionBasedContinuousMoveProvider>(true));
            Collect(rig.GetComponentsInChildren<ActionBasedContinuousTurnProvider>(true));
            Collect(rig.GetComponentsInChildren<ActionBasedSnapTurnProvider>(true));
            Collect(rig.GetComponentsInChildren<DashLocomotion>(true));
        }

        private void Collect(Behaviour[] behaviours)
        {
            foreach (var b in behaviours)
                if (b != null && b.enabled)
                {
                    b.enabled = false;
                    _suspended.Add(b);
                }
        }

        private void ResumeLocomotion()
        {
            foreach (var b in _suspended)
                if (b != null) b.enabled = true;
            _suspended.Clear();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// Data-driven ground ride using the shared comfort-clamped FlightModel with pitch locked to zero.
    /// Each archetype owns a distinct shared-material silhouette; terrain probes ignore self-collision,
    /// reject unsupported map edges, understand authored river surfaces, and require safe dismount ground.
    ///
    /// The mounted player lives in one vehicle-space frame: the vehicle owns world yaw while the HMD
    /// retains local physical-head freedom. Mounting is a compact seat-side affordance, not giant UI
    /// blocks in the rider's view; X is the explicit step-off action while riding.
    /// </summary>
    public class VehicleRuntime : MonoBehaviour, Ziptide.Core.IResonanceSensitive
    {
        [SerializeField] private string vehicleId = "tide_skiff";

        private const float GroundRayUp = 4f;
        private const float GroundRayDown = 30f;
        private const string VisualRootName = "__VehicleVisual";
        public const float SteeringDeadzone = 0.18f;
        public const float SmoothSteerRateDeg = 82f;
        private const float SteerResponsePerSecond = 5.5f;

        private VehicleDefinition _def;
        private FlightParams _params;
        private FlightState _state;
        private FlightYawLatch _yawLatch;
        private bool _riding;
        private float _restY;
        private float _nextEdgeLog;
        private float _steerSmoothed;
        private float _lastVehicleYaw;
        private PlayerRigPersistence _rig;
        private PlayerMenuRuntime _playerMenu;
        private InputAction _leftStick, _rightStick, _boostL3, _boostA, _dismountX;
        private TextMesh _label;
        private GameObject _mountAffordance;
        private readonly List<Behaviour> _suspended = new List<Behaviour>();
        private static readonly Dictionary<Color, Material> SharedMaterials = new Dictionary<Color, Material>();

        public string VehicleId => vehicleId;
        public VehicleArchetype Archetype => _def != null ? _def.archetype : VehicleArchetype.Skiff;
        public bool IsRiding => _riding;

        public void Configure(string id)
        {
            if (!string.IsNullOrEmpty(id)) vehicleId = id;
        }

        public static FlightParams ParamsFrom(VehicleDefinition def)
        {
            FlightParams p = FlightParams.Default;
            p.pitchRateDeg = 0f;
            p.pitchClampDeg = 0f;
            if (def == null) { p.maxSpeed = 14f; return p; }
            if (def.cruiseSpeed > 0f) p.maxSpeed = def.cruiseSpeed;
            if (def.boostMultiplier > 0f) p.boostMultiplier = Mathf.Clamp(def.boostMultiplier, 1f, 3f);
            if (def.roamRadius > 0f)
                p.laneRadius = Mathf.Min(def.roamRadius, FlightParams.Default.laneRadius);
            return p;
        }

        /// <summary>Pure steering seam for EditMode tests and future profile tuning.</summary>
        public static float ShapeSteer(float raw)
        {
            return Mathf.Abs(raw) < SteeringDeadzone ? 0f : Mathf.Clamp(raw, -1f, 1f);
        }

        /// <summary>
        /// Carries an XR origin through the vehicle's world-yaw delta without touching the HMD's local
        /// tracked rotation. This is the mounted-frame contract the original position-only follow lacked.
        /// </summary>
        public static Quaternion CarryRigYaw(Quaternion rigRotation, float previousVehicleYaw, float currentVehicleYaw)
        {
            float delta = Mathf.DeltaAngle(previousVehicleYaw, currentVehicleYaw);
            return Quaternion.AngleAxis(delta, Vector3.up) * rigRotation;
        }

        private void Start()
        {
            _def = Resources.Load<VehicleDefinition>("Vehicles/" + vehicleId);
            if (_def == null) Debug.LogWarning("ZIPTIDE: VEHICLE_DEF_MISSING id=" + vehicleId);
            _params = ParamsFrom(_def);
            _restY = transform.position.y;
            CreateActions();
            BuildVisualAndPanels();
        }

        private void CreateActions()
        {
            _leftStick = new InputAction("ZiptideRideThrottle", InputActionType.Value);
            _leftStick.AddBinding("<XRController>{LeftHand}/thumbstick");
            _rightStick = new InputAction("ZiptideRideSteer", InputActionType.Value);
            _rightStick.AddBinding("<XRController>{RightHand}/thumbstick");
            _boostL3 = new InputAction("ZiptideRideBoostL3", InputActionType.Button);
            _boostL3.AddBinding("<XRController>{LeftHand}/thumbstickClicked");
            _boostA = new InputAction("ZiptideRideBoostA", InputActionType.Button);
            _boostA.AddBinding("<XRController>{RightHand}/primaryButton");
            _dismountX = new InputAction("ZiptideRideDismount", InputActionType.Button);
            _dismountX.AddBinding("<XRController>{LeftHand}/primaryButton");
        }

        private void OnDisable()
        {
            if (_riding) ForceDismount();
        }

        private void OnDestroy()
        {
            _leftStick?.Dispose(); _rightStick?.Dispose();
            _boostL3?.Dispose(); _boostA?.Dispose(); _dismountX?.Dispose();
        }

        /// <summary>
        /// THE RESONANCE TELL reaching the ride (Ziptide.Core.IResonanceSensitive): lifting an
        /// artifact half kills the panel light and drains the paint toward black for a beat.
        /// LOOKS only — steering, mounting and dismounting are untouched, because stranding the
        /// player's vehicle out on the flats would be a bug dressed as a story moment.
        /// </summary>
        public void SetInstrumentPower(float power01)
        {
            power01 = Mathf.Clamp01(power01);
            if (_label != null)
                _label.color = new Color(0.95f, 0.8f, 0.5f) * (0.15f + 0.85f * power01);

            Transform visual = transform.Find(VisualRootName);
            if (visual == null) return;
            foreach (var r in visual.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null || r.sharedMaterial == null) continue;
                Color baseColor = r.sharedMaterial.HasProperty("_BaseColor")
                    ? r.sharedMaterial.GetColor("_BaseColor")
                    : r.sharedMaterial.color;
                // Never all the way to black — a vehicle that vanishes reads as deleted, and the
                // player has to be able to find it again when the lights come back.
                Color dimmed = Color.Lerp(baseColor * 0.2f, baseColor, power01);
                var block = new MaterialPropertyBlock();
                r.GetPropertyBlock(block);
                block.SetColor("_BaseColor", dimmed);
                block.SetColor("_Color", dimmed);
                r.SetPropertyBlock(block);
            }
        }

        private void BuildVisualAndPanels()
        {
            Transform existing = transform.Find(VisualRootName);
            if (existing != null) Destroy(existing.gameObject);
            Transform visual = new GameObject(VisualRootName).transform;
            visual.SetParent(transform, false);

            VehicleArchetype archetype = _def != null ? _def.archetype : VehicleArchetype.Skiff;
            VehicleVisualProfile profile = VehiclePresentationCore.Resolve(archetype);
            switch (archetype)
            {
                case VehicleArchetype.Hoverbike: BuildHoverbike(visual, profile); break;
                case VehicleArchetype.DrillCrawler: BuildCrawler(visual, profile); break;
                case VehicleArchetype.Skiff: BuildSkiff(visual, profile); break;
                default: BuildUtility(visual, profile); break;
            }
            BuildSeatAndControls(visual, profile, archetype);
            BuildMountAffordance(profile);

            GameObject labelGo = new GameObject("RideLabel");
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = SeatLocalPosition() + new Vector3(-0.48f, 0.34f, 0.10f);
            _label = labelGo.AddComponent<TextMesh>();
            _label.characterSize = 0.014f;
            _label.fontSize = 48;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(0.95f, 0.8f, 0.5f);
            SetIdleLabel();

            ObjectiveBeacon.Attach(gameObject, profile.Accent, 5f);
            int parts = visual.GetComponentsInChildren<Renderer>(true).Length;
            Debug.Log("ZIPTIDE: VEHICLE_VISUAL_READY id=" + vehicleId
                + " family=" + profile.Family + " parts=" + parts
                + " mount=seat_affordance steer=smooth dismount=X");
        }

        private static void BuildSkiff(Transform root, VehicleVisualProfile p)
        {
            Part(root, "SkiffHull", PrimitiveType.Cube, V(0, .34f, 0), V(1.25f, .34f, 2.35f), V0, p.Body, true);
            Part(root, "Deck", PrimitiveType.Cube, V(0, .57f, -.05f), V(1.02f, .12f, 1.72f), V0, Darken(p.Body), false);
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(root, "Pontoon_" + tag, PrimitiveType.Cube, V(side * .82f, .20f, -.05f),
                    V(.28f, .25f, 2.62f), V(0, side * -2f, 0), p.Body, false);
                Part(root, "ProwBlade_" + tag, PrimitiveType.Cube, V(side * .55f, .37f, 1.25f),
                    V(.42f, .22f, .72f), V(-12f, side * -12f, side * 4f), p.Accent, false);
                Part(root, "Rail_" + tag, PrimitiveType.Cube, V(side * .57f, .86f, 0),
                    V(.06f, .38f, 1.45f), V0, p.Accent, false);
            }
            Part(root, "EngineBlock", PrimitiveType.Cube, V(0, .62f, -1.02f), V(.78f, .52f, .55f), V0, Darken(p.Body), false);
            Part(root, "FanRing", PrimitiveType.Cylinder, V(0, .72f, -1.35f), V(.48f, .12f, .48f), V(90, 0, 0), p.Accent, false);
            Part(root, "FanHub", PrimitiveType.Cylinder, V(0, .72f, -1.43f), V(.18f, .10f, .18f), V(90, 0, 0), p.Glow, false);
            Part(root, "BowLamp", PrimitiveType.Sphere, V(0, .48f, 1.42f), Vector3.one * .13f, V0, p.Glow, false);
            Part(root, "CargoRack", PrimitiveType.Cube, V(0, .83f, -.62f), V(.82f, .08f, .52f), V0, p.Accent, false);
        }

        private static void BuildHoverbike(Transform root, VehicleVisualProfile p)
        {
            // Rider-eye clearance law: no primitive may rise through the central x ±0.24 m channel
            // above the seat. The original tall forks and common control bar occupied that channel.
            Part(root, "BikeChassis", PrimitiveType.Cube, V(0, .42f, 0), V(.48f, .24f, 1.92f), V0, p.Body, true);
            Part(root, "NoseCowl", PrimitiveType.Cube, V(0, .43f, .92f), V(.42f, .22f, .58f), V(-12, 0, 0), p.Accent, false);
            Part(root, "Spine", PrimitiveType.Cube, V(0, .58f, -.18f), V(.14f, .12f, 1.25f), V0, Darken(p.Body), false);
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(root, "Stabilizer_" + tag, PrimitiveType.Cube, V(side * .58f, .33f, -.12f),
                    V(.54f, .07f, .66f), V(0, side * 8f, side * 7f), p.Accent, false);
                Part(root, "Fork_" + tag, PrimitiveType.Cube, V(side * .42f, .42f, .70f),
                    V(.05f, .28f, .05f), V(12, 0, 0), Darken(p.Body), false);
                for (int end = -1; end <= 1; end += 2)
                    Part(root, "HoverPad_" + tag + "_" + end, PrimitiveType.Cylinder,
                        V(side * .58f, .14f, end * .68f), V(.28f, .05f, .34f), V0, p.Glow, false);
            }
            Part(root, "RearThruster", PrimitiveType.Cylinder, V(0, .42f, -1.08f), V(.30f, .14f, .30f), V(90, 0, 0), p.Accent, false);
            Part(root, "ThrusterGlow", PrimitiveType.Cylinder, V(0, .42f, -1.24f), V(.20f, .05f, .20f), V(90, 0, 0), p.Glow, false);
            Part(root, "Headlamp", PrimitiveType.Sphere, V(0, .48f, 1.22f), Vector3.one * .10f, V0, p.Glow, false);
        }

        private static void BuildCrawler(Transform root, VehicleVisualProfile p)
        {
            Part(root, "CrawlerHull", PrimitiveType.Cube, V(0, .52f, -.12f), V(1.38f, .58f, 2.05f), V0, p.Body, true);
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(root, "TreadBody_" + tag, PrimitiveType.Cube, V(side * .86f, .30f, -.10f),
                    V(.42f, .48f, 2.35f), V0, Darken(p.Body), false);
                for (int segment = 0; segment < 5; segment++)
                    Part(root, "Tread_" + tag + "_" + segment, PrimitiveType.Cube,
                        V(side * 1.08f, .30f, -.92f + segment * .46f), V(.08f, .56f, .34f), V0, p.Accent, false);
            }
            Part(root, "Cabin", PrimitiveType.Cube, V(0, 1.02f, -.28f), V(1.05f, .78f, .92f), V(-3, 0, 0), p.Body, false);
            Part(root, "Windshield", PrimitiveType.Cube, V(0, 1.12f, .21f), V(.82f, .42f, .07f), V(-8, 0, 0), new Color(.18f, .42f, .52f), false);
            Part(root, "RoofRack", PrimitiveType.Cube, V(0, 1.50f, -.30f), V(1.18f, .09f, 1.02f), V0, p.Accent, false);
            Part(root, "RearCargo", PrimitiveType.Cube, V(0, .93f, -1.05f), V(1.12f, .62f, .58f), V0, Darken(p.Body), false);
            for (int ring = 0; ring < 3; ring++)
            {
                float radius = .48f - ring * .11f;
                Part(root, "DrillRing_" + ring, PrimitiveType.Cylinder, V(0, .60f, 1.12f + ring * .25f),
                    V(radius, .18f, radius), V(90, 0, 0), p.Accent, false);
            }
            Part(root, "DrillTip", PrimitiveType.Cube, V(0, .60f, 1.72f), V(.18f, .18f, .38f), V(0, 45, 45), p.Glow, false);
            for (int side = -1; side <= 1; side += 2)
            {
                Part(root, "ExhaustStack_" + side, PrimitiveType.Cylinder, V(side * .52f, 1.42f, -.86f),
                    V(.11f, .48f, .11f), V0, Darken(p.Body), false);
                Part(root, "WorkLamp_" + side, PrimitiveType.Sphere, V(side * .42f, 1.24f, .36f),
                    Vector3.one * .12f, V0, p.Glow, false);
            }
        }

        private static void BuildUtility(Transform root, VehicleVisualProfile p)
        {
            Part(root, "UtilityHull", PrimitiveType.Cube, V(0, .42f, 0), V(1.15f, .46f, 2.15f), V0, p.Body, true);
            Part(root, "UtilityDeck", PrimitiveType.Cube, V(0, .72f, -.2f), V(.92f, .12f, 1.32f), V0, p.Accent, false);
            for (int side = -1; side <= 1; side += 2)
            {
                Part(root, "UtilityPod_" + side, PrimitiveType.Cube, V(side * .72f, .30f, -.15f),
                    V(.28f, .35f, 1.55f), V0, Darken(p.Body), false);
                Part(root, "UtilityGlow_" + side, PrimitiveType.Cube, V(side * .72f, .18f, -.82f),
                    V(.18f, .08f, .30f), V0, p.Glow, false);
                Part(root, "UtilityLamp_" + side, PrimitiveType.Sphere, V(side * .38f, .64f, 1.10f),
                    Vector3.one * .10f, V0, p.Glow, false);
            }
        }

        private void BuildSeatAndControls(Transform root, VehicleVisualProfile p, VehicleArchetype archetype)
        {
            Vector3 seat = SeatLocalPosition();
            Part(root, "Seat", PrimitiveType.Cube, seat, V(.52f, .16f, .56f), V(-5, 0, 0), Darken(p.Body), false);
            Part(root, "SeatBack", PrimitiveType.Cube, seat + V(0, .35f, -.24f), V(.52f, .62f, .12f), V(-8, 0, 0), Darken(p.Body), false);

            if (archetype == VehicleArchetype.Hoverbike)
            {
                // Split grips leave the central forward view open instead of one large blue crossbar.
                Part(root, "ControlGrip_L", PrimitiveType.Cube, seat + V(-.28f, .21f, .42f), V(.18f, .055f, .07f), V(0, -10, 0), p.Accent, false);
                Part(root, "ControlGrip_R", PrimitiveType.Cube, seat + V(.28f, .21f, .42f), V(.18f, .055f, .07f), V(0, 10, 0), p.Accent, false);
                Part(root, "DashGlow", PrimitiveType.Cube, seat + V(0, .12f, .34f), V(.22f, .035f, .09f), V(-20, 0, 0), p.Glow, false);
            }
            else
            {
                Part(root, "ControlBar", PrimitiveType.Cube, seat + V(0, .32f, .50f), V(.68f, .065f, .065f), V0, p.Accent, false);
                Part(root, "DashGlow", PrimitiveType.Cube, seat + V(0, .24f, .38f), V(.34f, .06f, .10f), V(-25, 0, 0), p.Glow, false);
            }
        }

        private void BuildMountAffordance(VehicleVisualProfile p)
        {
            Vector3 seat = SeatLocalPosition();
            _mountAffordance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _mountAffordance.name = "MountGrip";
            _mountAffordance.transform.SetParent(transform, false);
            _mountAffordance.transform.localPosition = seat + V(-.46f, .20f, .04f);
            _mountAffordance.transform.localScale = V(.16f, .12f, .22f);
            Paint(_mountAffordance, p.Glow);

            XRSimpleInteractable interactable = _mountAffordance.AddComponent<XRSimpleInteractable>();
            XRInteractionManager manager = Object.FindObjectOfType<XRInteractionManager>();
            if (manager != null) interactable.interactionManager = manager;
            interactable.selectEntered.AddListener(_ => Mount());
        }

        private static GameObject Part(Transform parent, string name, PrimitiveType primitive,
            Vector3 position, Vector3 scale, Vector3 euler, Color color, bool collider)
        {
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            go.transform.localRotation = Quaternion.Euler(euler);
            Collider col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.Destroy(col);
            Paint(go, color);
            return go;
        }

        private void Mount()
        {
            if (_riding) return;
            _rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (_rig == null) return;
            _playerMenu = _rig.GetComponent<PlayerMenuRuntime>();

            _state = new FlightState
            {
                position = transform.position,
                yawDeg = transform.eulerAngles.y
            };
            _yawLatch = new FlightYawLatch { Armed = true };
            _steerSmoothed = 0f;
            SuspendLocomotion(_rig);
            CharacterController cc = _rig.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            AlignHeadToVehicleForward();
            _lastVehicleYaw = transform.eulerAngles.y;
            SetActionsEnabled(true);
            _riding = true;
            if (_mountAffordance != null) _mountAffordance.SetActive(false);
            if (_label != null) _label.text = "X STEP OFF\nL DRIVE  R STEER  A/L3 BOOST";
            Debug.Log("ZIPTIDE: VEHICLE_MOUNT id=" + vehicleId + " maxSpeed=" + _params.maxSpeed
                + " yaw=" + _state.yawDeg.ToString("F1") + " steering=smooth");
            FollowSeat();
        }

        private void Dismount()
        {
            if (!_riding) return;
            Vector3 dismountPosition = transform.position;
            if (_rig != null && !TryFindSafeDismount(out dismountPosition))
            {
                if (_label != null) _label.text = "REACH SOLID GROUND\nBEFORE STEPPING OFF";
                Debug.Log("ZIPTIDE: VEHICLE_DISMOUNT_BLOCKED id=" + vehicleId + " reason=no_safe_ground");
                return;
            }

            _riding = false;
            SetActionsEnabled(false);
            if (_rig != null)
            {
                CharacterController cc = _rig.GetComponent<CharacterController>();
                _rig.transform.position = dismountPosition;
                if (cc != null) cc.enabled = true;
            }
            ResumeLocomotion();
            _playerMenu = null;
            if (_mountAffordance != null) _mountAffordance.SetActive(true);
            SetIdleLabel();
            Debug.Log("ZIPTIDE: VEHICLE_DISMOUNT id=" + vehicleId);
        }

        private void Update()
        {
            if (!_riding) return;
            if (_rig == null) { ForceDismount(); return; }
            if (_dismountX.WasPressedThisFrame()) { Dismount(); return; }

            // Y remains a guaranteed escape path while mounted. The menu owns its own locomotion pause;
            // the vehicle simply stops consuming steering/throttle while the panel is open.
            if (_playerMenu != null && _playerMenu.IsOpen)
            {
                _state.speed = 0f;
                _steerSmoothed = 0f;
                FollowSeat();
                return;
            }

            Vector2 left = _leftStick.ReadValue<Vector2>();
            Vector2 right = _rightStick.ReadValue<Vector2>();
            // The shared input shaper still owns throttle/strafe/deadzones. Vehicle yaw is deliberately
            // removed from its snap latch and handled continuously below.
            FlightInputFrame frame = FlightInputCore.Shape(left, Vector2.zero, ref _yawLatch, Time.time);
            float steerTarget = ShapeSteer(right.x);
            _steerSmoothed = Mathf.MoveTowards(_steerSmoothed, steerTarget,
                SteerResponsePerSecond * Time.deltaTime);
            _state.yawDeg = Mathf.Repeat(_state.yawDeg + _steerSmoothed * SmoothSteerRateDeg * Time.deltaTime, 360f);

            bool boost = _boostL3.IsPressed() || _boostA.IsPressed();
            FlightState proposed = FlightModel.Tick(_state, _params, frame.Throttle, 0f, frame.Strafe, boost, Time.deltaTime);

            if (TryGroundYAt(proposed.position, out float groundY))
            {
                proposed.position.y = groundY + (_def != null ? _def.hoverHeight : .6f);
                _state = proposed;
            }
            else
            {
                _state.position = transform.position;
                _state.speed = 0f;
                if (Time.time >= _nextEdgeLog)
                {
                    _nextEdgeLog = Time.time + 1f;
                    Debug.Log("ZIPTIDE: VEHICLE_EDGE_BLOCK id=" + vehicleId
                        + " position=" + proposed.position.ToString("F1"));
                }
            }

            transform.SetPositionAndRotation(_state.position, Quaternion.Euler(0f, _state.yawDeg, 0f));
            FollowSeat();
        }

        private void SetActionsEnabled(bool enabled)
        {
            if (enabled)
            {
                _leftStick.Enable(); _rightStick.Enable(); _boostL3.Enable(); _boostA.Enable(); _dismountX.Enable();
            }
            else
            {
                _leftStick.Disable(); _rightStick.Disable(); _boostL3.Disable(); _boostA.Disable(); _dismountX.Disable();
            }
        }

        private void ForceDismount()
        {
            _riding = false;
            SetActionsEnabled(false);
            if (_rig != null)
            {
                CharacterController cc = _rig.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = true;
            }
            ResumeLocomotion();
            _playerMenu = null;
            if (_mountAffordance != null) _mountAffordance.SetActive(true);
            SetIdleLabel();
        }

        private void AlignHeadToVehicleForward()
        {
            if (_rig == null) return;
            Camera cam = _rig.GetComponentInChildren<Camera>(true);
            if (cam == null) return;
            Vector3 headForward = cam.transform.forward;
            headForward.y = 0f;
            if (headForward.sqrMagnitude < 0.0001f) return;
            headForward.Normalize();
            float headYaw = Mathf.Atan2(headForward.x, headForward.z) * Mathf.Rad2Deg;
            float delta = Mathf.DeltaAngle(headYaw, transform.eulerAngles.y);
            _rig.transform.rotation = Quaternion.AngleAxis(delta, Vector3.up) * _rig.transform.rotation;
        }

        private void FollowSeat()
        {
            if (_rig == null) return;
            float currentVehicleYaw = transform.eulerAngles.y;
            _rig.transform.rotation = CarryRigYaw(_rig.transform.rotation, _lastVehicleYaw, currentVehicleYaw);
            _lastVehicleYaw = currentVehicleYaw;
            _rig.transform.position = transform.TransformPoint(SeatLocalPosition());
        }

        private Vector3 SeatLocalPosition()
        {
            return _def != null ? _def.seatLocalPos : V(0, .55f, -.2f);
        }

        private bool TryGroundYAt(Vector3 pos, out float groundY)
        {
            RaycastHit[] hits = Physics.RaycastAll(pos + Vector3.up * GroundRayUp, Vector3.down,
                GroundRayUp + GroundRayDown, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                Collider col = hit.collider;
                if (col == null || col.transform.IsChildOf(transform)) continue;
                if (_rig != null && col.transform.IsChildOf(_rig.transform)) continue;
                ToxicRiverRuntime river = col.GetComponentInParent<ToxicRiverRuntime>();
                if (river != null)
                {
                    VehicleArchetype type = _def != null ? _def.archetype : VehicleArchetype.Skiff;
                    if (type != VehicleArchetype.Skiff && type != VehicleArchetype.Hoverbike) continue;
                    Transform surface = river.transform.Find("ToxicSurface");
                    if (surface != null) { groundY = surface.position.y; return true; }
                }
                groundY = hit.point.y;
                return true;
            }
            groundY = _restY;
            return false;
        }

        private bool TryFindSafeDismount(out Vector3 position)
        {
            Vector3[] offsets =
            {
                -transform.right * 1.6f, transform.right * 1.6f,
                -transform.forward * 1.8f, transform.forward * 1.8f,
            };
            foreach (Vector3 offset in offsets)
            {
                RaycastHit[] hits = Physics.RaycastAll(transform.position + offset + Vector3.up * 2f,
                    Vector3.down, 8f, ~0, QueryTriggerInteraction.Ignore);
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                foreach (RaycastHit hit in hits)
                {
                    Collider col = hit.collider;
                    if (col == null || col.transform.IsChildOf(transform)) continue;
                    if (_rig != null && col.transform.IsChildOf(_rig.transform)) continue;
                    if (col.GetComponentInParent<ToxicRiverRuntime>() != null) continue;
                    position = hit.point + Vector3.up * .10f;
                    return true;
                }
            }
            position = transform.position;
            return false;
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
            foreach (Behaviour b in behaviours)
                if (b != null && b.enabled) { b.enabled = false; _suspended.Add(b); }
        }

        private void ResumeLocomotion()
        {
            foreach (Behaviour b in _suspended) if (b != null) b.enabled = true;
            _suspended.Clear();
        }

        private void SetIdleLabel()
        {
            if (_label == null) return;
            string display = _def != null && !string.IsNullOrEmpty(_def.DisplayName)
                ? _def.DisplayName.Replace('_', ' ') : vehicleId;
            _label.text = display + "\n< SELECT SEAT GRIP TO RIDE >";
        }

        private static Vector3 V(float x, float y, float z) => new Vector3(x, y, z);
        private static Vector3 V0 => Vector3.zero;
        private static Color Darken(Color color) => Color.Lerp(color, Color.black, .35f);

        private static void Paint(GameObject go, Color color)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;
            if (!SharedMaterials.TryGetValue(color, out Material material) || material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader == null) return;
                material = new Material(shader) { name = "Vehicle_" + ColorUtility.ToHtmlStringRGB(color) };
                if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
                else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
                SharedMaterials[color] = material;
            }
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}

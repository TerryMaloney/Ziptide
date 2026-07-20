using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// Data-driven ground ride using the comfort-clamped FlightModel with pitch locked to zero. The
    /// interaction/movement owner is unchanged; presentation now resolves per VehicleArchetype, terrain
    /// probes ignore the vehicle itself, unsupported map edges stop movement, and dismount requires a
    /// proven non-toxic landing position.
    /// </summary>
    public class VehicleRuntime : MonoBehaviour
    {
        [Tooltip("VehicleDefinition id under Resources/Vehicles (data-driven, never a hard ref).")]
        [SerializeField] private string vehicleId = "tide_skiff";

        private const float SeatStrayExit = 3f;
        private const float GroundRayUp = 4f;
        private const float GroundRayDown = 30f;
        private const string VisualRootName = "__VehicleVisual";

        private VehicleDefinition _def;
        private FlightParams _params;
        private FlightState _state;
        private FlightYawLatch _yawLatch;
        private bool _riding;
        private Vector3 _padOrigin;
        private float _restY;
        private float _nextEdgeLog;

        private PlayerRigPersistence _rig;
        private InputAction _leftStick, _rightStick, _boostL3, _boostA;
        private TextMesh _label;
        private readonly List<Behaviour> _suspended = new List<Behaviour>();
        private static readonly Dictionary<Color, Material> SharedMaterials = new Dictionary<Color, Material>();

        public string VehicleId => vehicleId;
        public VehicleArchetype Archetype => _def != null ? _def.archetype : VehicleArchetype.Skiff;
        public bool IsRiding => _riding;

        public void Configure(string id)
        {
            if (!string.IsNullOrEmpty(id)) vehicleId = id;
        }

        /// <summary>VehicleDefinition → ground FlightParams. Pitch is zero by construction.</summary>
        public static FlightParams ParamsFrom(VehicleDefinition def)
        {
            var p = FlightParams.Default;
            p.pitchRateDeg = 0f;
            p.pitchClampDeg = 0f;
            if (def == null) { p.maxSpeed = 14f; return p; }
            if (def.cruiseSpeed > 0f) p.maxSpeed = def.cruiseSpeed;
            if (def.boostMultiplier > 0f) p.boostMultiplier = Mathf.Clamp(def.boostMultiplier, 1f, 3f);
            if (def.roamRadius > 0f) p.laneRadius = Mathf.Min(def.roamRadius, FlightParams.Default.laneRadius);
            return p;
        }

        private void Start()
        {
            _def = Resources.Load<VehicleDefinition>("Vehicles/" + vehicleId);
            if (_def == null)
                Debug.LogWarning("ZIPTIDE: VEHICLE_DEF_MISSING id=" + vehicleId);
            _params = ParamsFrom(_def);
            _padOrigin = transform.position;
            _restY = transform.position.y;

            _leftStick = new InputAction("ZiptideRideThrottle", InputActionType.Value);
            _leftStick.AddBinding("<XRController>{LeftHand}/thumbstick");
            _rightStick = new InputAction("ZiptideRideSteer", InputActionType.Value);
            _rightStick.AddBinding("<XRController>{RightHand}/thumbstick");
            _boostL3 = new InputAction("ZiptideRideBoostL3", InputActionType.Button);
            _boostL3.AddBinding("<XRController>{LeftHand}/thumbstickClicked");
            _boostA = new InputAction("ZiptideRideBoostA", InputActionType.Button);
            _boostA.AddBinding("<XRController>{RightHand}/primaryButton");

            BuildVisualAndPanels();
        }

        private void OnDestroy()
        {
            _leftStick?.Dispose();
            _rightStick?.Dispose();
            _boostL3?.Dispose();
            _boostA?.Dispose();
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
                case VehicleArchetype.Hoverbike:
                    BuildHoverbike(visual, profile);
                    break;
                case VehicleArchetype.DrillCrawler:
                    BuildCrawler(visual, profile);
                    break;
                case VehicleArchetype.Skiff:
                    BuildSkiff(visual, profile);
                    break;
                default:
                    BuildUtility(visual, profile);
                    break;
            }
            BuildSeatAndControls(visual, profile);

            MakeTile("Tile_MOUNT", new Vector3(-0.9f, 1.05f, -0.15f),
                new Color(0.2f, 0.65f, 0.5f), "RIDE", Mount);
            MakeTile("Tile_DISMOUNT", new Vector3(0.9f, 1.05f, -0.15f),
                new Color(0.6f, 0.5f, 0.2f), "STEP OFF", Dismount);

            var labelGo = new GameObject("RideLabel");
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 1.65f, -0.15f);
            _label = labelGo.AddComponent<TextMesh>();
            _label.characterSize = 0.03f;
            _label.fontSize = 48;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(0.95f, 0.8f, 0.5f);
            _label.text = (_def != null ? _def.DisplayName.Replace('_', ' ') : vehicleId)
                          + "\n< RIDE to mount >";

            ObjectiveBeacon.Attach(gameObject, profile.Accent, 5f);
            int parts = visual.GetComponentsInChildren<Renderer>(true).Length;
            Debug.Log("ZIPTIDE: VEHICLE_VISUAL_READY id=" + vehicleId
                + " family=" + profile.Family + " parts=" + parts);
        }

        private static void BuildSkiff(Transform root, VehicleVisualProfile p)
        {
            Part(root, "SkiffHull", PrimitiveType.Cube, new Vector3(0f, 0.34f, 0f),
                new Vector3(1.25f, 0.34f, 2.35f), Vector3.zero, p.Body, true);
            Part(root, "Deck", PrimitiveType.Cube, new Vector3(0f, 0.57f, -0.05f),
                new Vector3(1.02f, 0.12f, 1.72f), Vector3.zero, Darken(p.Body), false);
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(root, "Pontoon_" + tag, PrimitiveType.Cube,
                    new Vector3(side * 0.82f, 0.20f, -0.05f),
                    new Vector3(0.28f, 0.25f, 2.62f), new Vector3(0f, side * -2f, 0f), p.Body, false);
                Part(root, "ProwBlade_" + tag, PrimitiveType.Cube,
                    new Vector3(side * 0.55f, 0.37f, 1.25f),
                    new Vector3(0.42f, 0.22f, 0.72f), new Vector3(-12f, side * -12f, side * 4f), p.Accent, false);
                Part(root, "Rail_" + tag, PrimitiveType.Cube,
                    new Vector3(side * 0.57f, 0.86f, 0f),
                    new Vector3(0.06f, 0.38f, 1.45f), Vector3.zero, p.Accent, false);
            }
            Part(root, "EngineBlock", PrimitiveType.Cube, new Vector3(0f, 0.62f, -1.02f),
                new Vector3(0.78f, 0.52f, 0.55f), Vector3.zero, Darken(p.Body), false);
            Part(root, "FanRing", PrimitiveType.Cylinder, new Vector3(0f, 0.72f, -1.35f),
                new Vector3(0.48f, 0.12f, 0.48f), new Vector3(90f, 0f, 0f), p.Accent, false);
            Part(root, "FanHub", PrimitiveType.Cylinder, new Vector3(0f, 0.72f, -1.43f),
                new Vector3(0.18f, 0.10f, 0.18f), new Vector3(90f, 0f, 0f), p.Glow, false);
            Part(root, "BowLamp", PrimitiveType.Sphere, new Vector3(0f, 0.48f, 1.42f),
                Vector3.one * 0.13f, Vector3.zero, p.Glow, false);
            Part(root, "CargoRack", PrimitiveType.Cube, new Vector3(0f, 0.83f, -0.62f),
                new Vector3(0.82f, 0.08f, 0.52f), Vector3.zero, p.Accent, false);
        }

        private static void BuildHoverbike(Transform root, VehicleVisualProfile p)
        {
            Part(root, "BikeChassis", PrimitiveType.Cube, new Vector3(0f, 0.48f, 0f),
                new Vector3(0.55f, 0.30f, 2.18f), Vector3.zero, p.Body, true);
            Part(root, "NoseCowl", PrimitiveType.Cube, new Vector3(0f, 0.57f, 1.08f),
                new Vector3(0.48f, 0.34f, 0.72f), new Vector3(-16f, 0f, 0f), p.Accent, false);
            Part(root, "Spine", PrimitiveType.Cube, new Vector3(0f, 0.75f, -0.18f),
                new Vector3(0.18f, 0.18f, 1.45f), Vector3.zero, Darken(p.Body), false);
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(root, "Stabilizer_" + tag, PrimitiveType.Cube,
                    new Vector3(side * 0.62f, 0.38f, -0.12f),
                    new Vector3(0.62f, 0.08f, 0.72f), new Vector3(0f, side * 8f, side * 7f), p.Accent, false);
                Part(root, "Fork_" + tag, PrimitiveType.Cube,
                    new Vector3(side * 0.25f, 0.64f, 0.78f),
                    new Vector3(0.07f, 0.68f, 0.07f), new Vector3(20f, 0f, 0f), Darken(p.Body), false);
                for (int end = -1; end <= 1; end += 2)
                    Part(root, "HoverPad_" + tag + "_" + end, PrimitiveType.Cylinder,
                        new Vector3(side * 0.58f, 0.16f, end * 0.72f),
                        new Vector3(0.30f, 0.055f, 0.38f), Vector3.zero, p.Glow, false);
            }
            Part(root, "RearThruster", PrimitiveType.Cylinder, new Vector3(0f, 0.48f, -1.20f),
                new Vector3(0.32f, 0.16f, 0.32f), new Vector3(90f, 0f, 0f), p.Accent, false);
            Part(root, "ThrusterGlow", PrimitiveType.Cylinder, new Vector3(0f, 0.48f, -1.38f),
                new Vector3(0.22f, 0.06f, 0.22f), new Vector3(90f, 0f, 0f), p.Glow, false);
            Part(root, "Headlamp", PrimitiveType.Sphere, new Vector3(0f, 0.62f, 1.42f),
                Vector3.one * 0.12f, Vector3.zero, p.Glow, false);
        }

        private static void BuildCrawler(Transform root, VehicleVisualProfile p)
        {
            Part(root, "CrawlerHull", PrimitiveType.Cube, new Vector3(0f, 0.52f, -0.12f),
                new Vector3(1.38f, 0.58f, 2.05f), Vector3.zero, p.Body, true);
            for (int side = -1; side <= 1; side += 2)
            {
                string tag = side < 0 ? "L" : "R";
                Part(root, "TreadBody_" + tag, PrimitiveType.Cube,
                    new Vector3(side * 0.86f, 0.30f, -0.10f),
                    new Vector3(0.42f, 0.48f, 2.35f), Vector3.zero, Darken(p.Body), false);
                for (int segment = 0; segment < 5; segment++)
                {
                    float z = -0.92f + segment * 0.46f;
                    Part(root, "Tread_" + tag + "_" + segment, PrimitiveType.Cube,
                        new Vector3(side * 1.08f, 0.30f, z),
                        new Vector3(0.08f, 0.56f, 0.34f), Vector3.zero, p.Accent, false);
                }
            }
            Part(root, "Cabin", PrimitiveType.Cube, new Vector3(0f, 1.02f, -0.28f),
                new Vector3(1.05f, 0.78f, 0.92f), new Vector3(-3f, 0f, 0f), p.Body, false);
            Part(root, "Windshield", PrimitiveType.Cube, new Vector3(0f, 1.12f, 0.21f),
                new Vector3(0.82f, 0.42f, 0.07f), new Vector3(-8f, 0f, 0f), new Color(0.18f, 0.42f, 0.52f), false);
            Part(root, "RoofRack", PrimitiveType.Cube, new Vector3(0f, 1.50f, -0.30f),
                new Vector3(1.18f, 0.09f, 1.02f), Vector3.zero, p.Accent, false);
            Part(root, "RearCargo", PrimitiveType.Cube, new Vector3(0f, 0.93f, -1.05f),
                new Vector3(1.12f, 0.62f, 0.58f), Vector3.zero, Darken(p.Body), false);
            for (int ring = 0; ring < 3; ring++)
            {
                float radius = 0.48f - ring * 0.11f;
                Part(root, "DrillRing_" + ring, PrimitiveType.Cylinder,
                    new Vector3(0f, 0.60f, 1.12f + ring * 0.25f),
                    new Vector3(radius, 0.18f, radius), new Vector3(90f, 0f, 0f), p.Accent, false);
            }
            Part(root, "DrillTip", PrimitiveType.Cube, new Vector3(0f, 0.60f, 1.72f),
                new Vector3(0.18f, 0.18f, 0.38f), new Vector3(0f, 45f, 45f), p.Glow, false);
            for (int side = -1; side <= 1; side += 2)
            {
                Part(root, "ExhaustStack_" + side, PrimitiveType.Cylinder,
                    new Vector3(side * 0.52f, 1.42f, -0.86f),
                    new Vector3(0.11f, 0.48f, 0.11f), Vector3.zero, Darken(p.Body), false);
                Part(root, "WorkLamp_" + side, PrimitiveType.Sphere,
                    new Vector3(side * 0.42f, 1.24f, 0.36f),
                    Vector3.one * 0.12f, Vector3.zero, p.Glow, false);
            }
        }

        private static void BuildUtility(Transform root, VehicleVisualProfile p)
        {
            Part(root, "UtilityHull", PrimitiveType.Cube, new Vector3(0f, 0.42f, 0f),
                new Vector3(1.15f, 0.46f, 2.15f), Vector3.zero, p.Body, true);
            Part(root, "UtilityDeck", PrimitiveType.Cube, new Vector3(0f, 0.72f, -0.2f),
                new Vector3(0.92f, 0.12f, 1.32f), Vector3.zero, p.Accent, false);
            for (int side = -1; side <= 1; side += 2)
            {
                Part(root, "UtilityPod_" + side, PrimitiveType.Cube,
                    new Vector3(side * 0.72f, 0.30f, -0.15f),
                    new Vector3(0.28f, 0.35f, 1.55f), Vector3.zero, Darken(p.Body), false);
                Part(root, "UtilityGlow_" + side, PrimitiveType.Cube,
                    new Vector3(side * 0.72f, 0.18f, -0.82f),
                    new Vector3(0.18f, 0.08f, 0.30f), Vector3.zero, p.Glow, false);
            }
        }

        private void BuildSeatAndControls(Transform root, VehicleVisualProfile p)
        {
            Vector3 seatPos = _def != null ? _def.seatLocalPos : new Vector3(0f, 0.55f, -0.2f);
            Part(root, "Seat", PrimitiveType.Cube, seatPos,
                new Vector3(0.52f, 0.16f, 0.56f), new Vector3(-5f, 0f, 0f), Darken(p.Body), false);
            Part(root, "SeatBack", PrimitiveType.Cube, seatPos + new Vector3(0f, 0.35f, -0.24f),
                new Vector3(0.52f, 0.62f, 0.12f), new Vector3(-8f, 0f, 0f), Darken(p.Body), false);
            Part(root, "ControlBar", PrimitiveType.Cube, seatPos + new Vector3(0f, 0.42f, 0.56f),
                new Vector3(0.82f, 0.08f, 0.08f), Vector3.zero, p.Accent, false);
            Part(root, "DashGlow", PrimitiveType.Cube, seatPos + new Vector3(0f, 0.34f, 0.42f),
                new Vector3(0.42f, 0.08f, 0.12f), new Vector3(-25f, 0f, 0f), p.Glow, false);
        }

        private static GameObject Part(Transform parent, string name, PrimitiveType primitive,
            Vector3 localPosition, Vector3 localScale, Vector3 localEuler, Color color, bool collider)
        {
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = localScale;
            go.transform.localRotation = Quaternion.Euler(localEuler);
            Collider col = go.GetComponent<Collider>();
            if (col != null && !collider) Object.Destroy(col);
            Paint(go, color);
            return go;
        }

        private void MakeTile(string name, Vector3 localPos, Color color, string text, System.Action onPress)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = name;
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = localPos;
            tile.transform.localScale = new Vector3(0.5f, 0.18f, 0.3f);
            Paint(tile, color);
            var label = new GameObject("Label");
            label.transform.SetParent(tile.transform, false);
            label.transform.localPosition = new Vector3(0f, 0f, -0.55f);
            label.transform.localScale = new Vector3(1f / 0.5f, 1f / 0.18f, 1f / 0.3f) * 0.25f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.03f;
            tm.fontSize = 56;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(1f, 0.95f, 0.8f);
            var interactable = tile.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => onPress());
        }

        private void Mount()
        {
            if (_riding) return;
            _rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (_rig == null) return;

            _state = new FlightState { position = transform.position };
            _yawLatch = new FlightYawLatch { Armed = true };
            SuspendLocomotion(_rig);
            var cc = _rig.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            _leftStick.Enable();
            _rightStick.Enable();
            _boostL3.Enable();
            _boostA.Enable();
            _riding = true;
            if (_label != null) _label.text = "RIDING\nleft stick drive - right stick turn - L3/A boost";
            Debug.Log("ZIPTIDE: VEHICLE_MOUNT id=" + vehicleId + " maxSpeed=" + _params.maxSpeed);
            FollowSeat();
        }

        private void Dismount()
        {
            if (!_riding) return;
            if (_rig != null && !TryFindSafeDismount(out Vector3 dismountPosition))
            {
                if (_label != null) _label.text = "REACH SOLID GROUND\nBEFORE STEPPING OFF";
                Debug.Log("ZIPTIDE: VEHICLE_DISMOUNT_BLOCKED id=" + vehicleId + " reason=no_safe_ground");
                return;
            }

            _riding = false;
            _leftStick.Disable();
            _rightStick.Disable();
            _boostL3.Disable();
            _boostA.Disable();
            if (_rig != null)
            {
                var cc = _rig.GetComponent<CharacterController>();
                _rig.transform.position = dismountPosition;
                if (cc != null) cc.enabled = true;
            }
            ResumeLocomotion();
            if (_label != null)
                _label.text = (_def != null ? _def.DisplayName.Replace('_', ' ') : vehicleId)
                              + "\n< RIDE to mount >";
            Debug.Log("ZIPTIDE: VEHICLE_DISMOUNT id=" + vehicleId);
        }

        private void Update()
        {
            if (!_riding) return;
            if (_rig == null) { ForceDismount(); return; }

            var frame = FlightInputCore.Shape(
                _leftStick.ReadValue<Vector2>(), _rightStick.ReadValue<Vector2>(), ref _yawLatch, Time.time);
            bool boost = _boostL3.IsPressed() || _boostA.IsPressed();

            if (frame.YawSnap != 0)
                _state = FlightModel.SnapYaw(_state, _params, frame.YawSnap);
            FlightState proposed = FlightModel.Tick(
                _state, _params, frame.Throttle, 0f, frame.Strafe, boost, Time.deltaTime);

            if (TryGroundYAt(proposed.position, out float groundY))
            {
                proposed.position.y = groundY + (_def != null ? _def.hoverHeight : 0.6f);
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

            if (Vector3.Distance(transform.position, _padOrigin) > SeatStrayExit + _params.laneRadius)
                Debug.LogWarning("ZIPTIDE: VEHICLE_SOFT_WALL_DRIFT id=" + vehicleId);
        }

        private void ForceDismount()
        {
            _riding = false;
            _leftStick?.Disable();
            _rightStick?.Disable();
            _boostL3?.Disable();
            _boostA?.Disable();
            ResumeLocomotion();
        }

        private void FollowSeat()
        {
            if (_rig == null) return;
            Vector3 seat = transform.TransformPoint(
                _def != null ? _def.seatLocalPos : new Vector3(0f, 0.55f, -0.2f));
            _rig.transform.position = seat;
        }

        private bool TryGroundYAt(Vector3 pos, out float groundY)
        {
            RaycastHit[] hits = Physics.RaycastAll(pos + Vector3.up * GroundRayUp, Vector3.down,
                GroundRayUp + GroundRayDown, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col == null || col.transform.IsChildOf(transform)) continue;
                if (_rig != null && col.transform.IsChildOf(_rig.transform)) continue;

                ToxicRiverRuntime river = col.GetComponentInParent<ToxicRiverRuntime>();
                if (river != null)
                {
                    VehicleArchetype archetype = _def != null ? _def.archetype : VehicleArchetype.Skiff;
                    if (archetype != VehicleArchetype.Skiff && archetype != VehicleArchetype.Hoverbike)
                        continue;
                    Transform surface = river.transform.Find("ToxicSurface");
                    if (surface != null)
                    {
                        groundY = surface.position.y;
                        return true;
                    }
                }

                groundY = hits[i].point.y;
                return true;
            }
            groundY = _restY;
            return false;
        }

        private bool TryFindSafeDismount(out Vector3 position)
        {
            Vector3[] offsets =
            {
                -transform.right * 1.6f,
                transform.right * 1.6f,
                -transform.forward * 1.8f,
                transform.forward * 1.8f,
            };
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector3 candidate = transform.position + offsets[i];
                RaycastHit[] hits = Physics.RaycastAll(candidate + Vector3.up * 2f, Vector3.down,
                    8f, ~0, QueryTriggerInteraction.Ignore);
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                for (int h = 0; h < hits.Length; h++)
                {
                    Collider col = hits[h].collider;
                    if (col == null || col.transform.IsChildOf(transform)) continue;
                    if (col.GetComponentInParent<ToxicRiverRuntime>() != null) continue;
                    position = hits[h].point + Vector3.up * 0.10f;
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
            foreach (var b in behaviours)
                if (b != null && b.enabled) { b.enabled = false; _suspended.Add(b); }
        }

        private void ResumeLocomotion()
        {
            foreach (var b in _suspended)
                if (b != null) b.enabled = true;
            _suspended.Clear();
        }

        private static Color Darken(Color color)
        {
            return Color.Lerp(color, Color.black, 0.35f);
        }

        private static void Paint(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
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

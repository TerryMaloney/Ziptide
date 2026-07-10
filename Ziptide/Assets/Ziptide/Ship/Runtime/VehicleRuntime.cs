using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// DRIVABLE VEHICLES 3.2 — the ground ride ("a ship for the ground"). One control language
    /// with the helm: left stick = throttle/strafe (back = reverse), right stick X = hold-to-repeat
    /// snap yaw, L3/A = boost. The math IS the ship's comfort-clamped <see cref="FlightModel"/>
    /// with a GROUND PROFILE — pitch rate zero, so a vehicle can never tilt the horizon (comfort
    /// law by construction, pinned by VehicleParamsTests). Unlike flight's world-moves render, the
    /// ride moves THROUGH the world: the rig teleport-follows the seat every frame (never parented
    /// — the ship law), terrain-hugged by raycast + hoverHeight, and the ONE ComfortVignette sees
    /// the rig's motion through its normal sampling — zero extra wiring. MOUNT panel to ride,
    /// DISMOUNT to step off beside it. Resolves its VehicleDefinition BY ID from
    /// Resources/Vehicles (the ItemFactory law). Fields serialized at edit time (gotcha #7).
    /// Logs VEHICLE_MOUNT / VEHICLE_DISMOUNT.
    /// </summary>
    public class VehicleRuntime : MonoBehaviour
    {
        [Tooltip("VehicleDefinition id under Resources/Vehicles (data-driven, never a hard ref).")]
        [SerializeField] private string vehicleId = "tide_skiff";

        private const float SeatStrayExit = 3f;
        private const float GroundRayUp = 4f, GroundRayDown = 30f;

        private VehicleDefinition _def;
        private FlightParams _params;
        private FlightState _state;
        private FlightYawLatch _yawLatch;
        private bool _riding;
        private Vector3 _padOrigin;   // roam center + the lane-space origin
        private float _restY;

        private PlayerRigPersistence _rig;
        private InputAction _leftStick, _rightStick, _boostL3, _boostA;
        private TextMesh _label;
        private readonly System.Collections.Generic.List<Behaviour> _suspended
            = new System.Collections.Generic.List<Behaviour>();

        /// <summary>VehicleDefinition → the ground FlightParams profile. Cruise/boost map with the
        /// shared clamps; PITCH RATE IS ZERO — no data, no input, no bug can ever tilt the horizon
        /// from a ground seat.</summary>
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
            var hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hull.name = "RideHull";
            hull.transform.SetParent(transform, false);
            hull.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            hull.transform.localScale = new Vector3(1.1f, 0.4f, 2.2f);
            Paint(hull, new Color(0.75f, 0.45f, 0.15f));

            var prow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prow.name = "Prow"; StripCollider(prow);
            prow.transform.SetParent(transform, false);
            prow.transform.localPosition = new Vector3(0f, 0.42f, 1.25f);
            prow.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            prow.transform.localScale = new Vector3(0.8f, 0.3f, 0.5f);
            Paint(prow, new Color(0.6f, 0.35f, 0.12f));

            var seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Seat"; StripCollider(seat);
            seat.transform.SetParent(transform, false);
            seat.transform.localPosition = _def != null ? _def.seatLocalPos : new Vector3(0f, 0.55f, -0.2f);
            seat.transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);
            Paint(seat, new Color(0.25f, 0.28f, 0.34f));

            MakeTile("Tile_MOUNT", new Vector3(-0.9f, 0.9f, 0f), new Color(0.2f, 0.65f, 0.5f), "RIDE", Mount);
            MakeTile("Tile_DISMOUNT", new Vector3(0.9f, 0.9f, 0f), new Color(0.6f, 0.5f, 0.2f), "STEP OFF", Dismount);

            var labelGo = new GameObject("RideLabel");
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 1.45f, 0f);
            _label = labelGo.AddComponent<TextMesh>();
            _label.characterSize = 0.03f;
            _label.fontSize = 48;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(0.95f, 0.8f, 0.5f);
            _label.text = (_def != null ? _def.DisplayName.Replace('_', ' ') : vehicleId)
                          + "\n< RIDE to mount >";

            ObjectiveBeacon.Attach(gameObject, new Color(0.9f, 0.6f, 0.2f), 5f);
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

        // ── Ride loop ───────────────────────────────────────────────────────────────────────────

        private void Mount()
        {
            if (_riding) return;
            _rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (_rig == null) return;

            _state = new FlightState { position = transform.position };
            _yawLatch = new FlightYawLatch { Armed = true };
            SuspendLocomotion(_rig);
            var cc = _rig.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false; // rig rides the seat; restored on dismount
            _leftStick.Enable(); _rightStick.Enable(); _boostL3.Enable(); _boostA.Enable();
            _riding = true;
            if (_label != null) _label.text = "riding\nleft stick drive - right stick turn - L3/A boost";
            Debug.Log("ZIPTIDE: VEHICLE_MOUNT id=" + vehicleId + " maxSpeed=" + _params.maxSpeed);
            FollowSeat();
        }

        private void Dismount()
        {
            if (!_riding) return;
            _riding = false;
            _leftStick.Disable(); _rightStick.Disable(); _boostL3.Disable(); _boostA.Disable();
            if (_rig != null)
            {
                var cc = _rig.GetComponent<CharacterController>();
                _rig.transform.position = transform.position - transform.right * 1.4f + Vector3.up * 0.1f;
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
            if (_rig == null) { Dismount(); return; }

            var frame = FlightInputCore.Shape(
                _leftStick.ReadValue<Vector2>(), _rightStick.ReadValue<Vector2>(), ref _yawLatch, Time.time);
            bool boost = _boostL3.IsPressed() || _boostA.IsPressed();

            if (frame.YawSnap != 0)
                _state = FlightModel.SnapYaw(_state, _params, frame.YawSnap);
            // Ground profile: pitch input is ignored by construction (pitchRateDeg = 0).
            _state = FlightModel.Tick(_state, _params, frame.Throttle, 0f, frame.Strafe, boost, Time.deltaTime);

            // Terrain hug: the ride's Y is the ground under it + hover height, not the model's.
            Vector3 pos = _state.position;
            pos.y = GroundYAt(pos) + (_def != null ? _def.hoverHeight : 0.6f);
            _state.position = pos;

            transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, _state.yawDeg, 0f));
            FollowSeat();
        }

        private void FollowSeat()
        {
            // Teleport-follow, never parent (the ship law). Locomotion is suspended and the
            // CharacterController is off, so a direct set is clean; the ComfortVignette reads this
            // rig motion through its normal sampling.
            if (_rig == null) return;
            Vector3 seat = transform.TransformPoint(_def != null ? _def.seatLocalPos : new Vector3(0f, 0.55f, -0.2f));
            _rig.transform.position = seat;
        }

        private float GroundYAt(Vector3 pos)
        {
            if (Physics.Raycast(pos + Vector3.up * GroundRayUp, Vector3.down, out var hit,
                    GroundRayUp + GroundRayDown, ~0, QueryTriggerInteraction.Ignore))
                return hit.point.y;
            return _restY; // off the map edge: hold the pad's height, the soft wall turns you back
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

        private static void StripCollider(GameObject go)
        {
            var c = go.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }

        private static void Paint(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}

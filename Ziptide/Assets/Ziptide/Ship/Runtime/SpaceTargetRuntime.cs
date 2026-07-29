using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Ship
{
    /// <summary>
    /// SPACE COMBAT 3.1 — a drifting drone target in the lane. Lives UNDER the LaneContent root
    /// (so the world-moves-around-you render carries it correctly); holds an armor state from the
    /// pure <see cref="SpaceCombatCore"/>; visuals only — ShipFlightRuntime owns the fire/hit/
    /// salvage loop and calls in. Non-lethal by construction: on disable the drone POWERS DOWN
    /// (dims, lists over, stops bobbing) and becomes a salvage prize; SalvageCacheRuntime.GrantTo
    /// pays the resource through the one economy when the pilot flies close.
    /// Built by ScenePatcherSpaceLane; all fields serialized at edit time (gotcha #7).
    /// </summary>
    public class SpaceTargetRuntime : MonoBehaviour
    {
        [SerializeField] private float maxArmor = 6f;
        [SerializeField] private string salvageResource = "scrap";
        [SerializeField] private double salvageAmount = 6.0;
        [SerializeField] private float bobAmplitude = 0.8f;

        private ShipArmorState _armor;
        private bool _salvaged;
        private Vector3 _home;
        private float _bobPhase;
        private Renderer[] _renderers;

        // Reaction layer (SpaceTargetReactionCore): the pilot's last observed lane position, the
        // current mood, and the eye we light to show it.
        private SpaceTargetMood _mood = SpaceTargetMood.Dormant;
        private float _pilotDistance = float.PositiveInfinity;
        private float _lastHitTime = float.NegativeInfinity;
        private Renderer _eye;
        private Color _eyeBase = new Color(1f, 0.3f, 0.2f);
        private float _approach;   // 0..1 salvage-approach read while downed

        public bool Disabled => _armor.Disabled;
        public bool Salvaged => _salvaged;

        /// <summary>Current reaction mood — read by tests and diagnostics.</summary>
        public SpaceTargetMood Mood => _mood;

        /// <summary>Lane-space position (local to the LaneContent root) — the space combat math
        /// runs in lane coordinates, same frame as FlightState.position.</summary>
        public Vector3 LanePosition => transform.localPosition;

        private void Awake()
        {
            _armor = new ShipArmorState { Armor = maxArmor, LastHitTime = float.NegativeInfinity };
            _home = transform.localPosition;
            _bobPhase = _home.x * 0.7f + _home.z * 0.3f;
            _renderers = GetComponentsInChildren<Renderer>();

            var eye = transform.Find("Eye");
            if (eye != null)
            {
                _eye = eye.GetComponent<Renderer>();
                if (_eye != null && _eye.sharedMaterial != null)
                {
                    var mat = _eye.sharedMaterial;
                    _eyeBase = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
                }
            }

            // The tool arms and the access panel (measured spec §3). A tender that notices you and
            // does not MOVE is a prop with a light on it; the arms coming out are the whole tell.
            _armL = transform.Find("Arm_L");
            _armR = transform.Find("Arm_R");
            _panel = transform.Find("AccessPanel");
            PoseForMood(_mood, false);
        }

        private Transform _armL, _armR, _panel;

        /// <summary>Arms stow flat along the flanks; they swing outboard as the drone wakes.</summary>
        private const float ArmStowedRoll = 0f;
        private const float ArmDeployedRoll = 62f;

        /// <summary>
        /// Pose the moving parts for the current mood. Dormant stows everything; woken and evading
        /// deploy the arms; disabled drops them slack and swings the access panel open on its hinge,
        /// which is what tells the player this thing is now cargo rather than staff.
        /// </summary>
        private void PoseForMood(SpaceTargetMood mood, bool disabled)
        {
            float deploy = disabled ? 0.75f : (mood == SpaceTargetMood.Dormant ? 0f : 1f);
            float roll = Mathf.Lerp(ArmStowedRoll, ArmDeployedRoll, deploy);
            if (_armL != null) _armL.localRotation = Quaternion.Euler(disabled ? 24f : 0f, 0f, -roll);
            if (_armR != null) _armR.localRotation = Quaternion.Euler(disabled ? 18f : 0f, 0f, roll);
            if (_panel != null) _panel.localRotation = Quaternion.Euler(disabled ? -72f : 0f, 0f, 0f);
        }

        /// <summary>
        /// The pilot reports where they are (lane space) each flight tick — that's the whole input
        /// to the reaction layer. Called by ShipFlightRuntime; a drone nobody flies past simply
        /// never hears from anyone and stays dormant.
        /// </summary>
        public void ObservePilot(Vector3 pilotLanePosition)
        {
            _pilotDistance = Vector3.Distance(pilotLanePosition, transform.localPosition);
        }

        private void Update()
        {
            if (_armor.Disabled)
            {
                // Powered down: a slow list + gentle sink, then hold. Non-lethal — it drifts, never burns.
                if (transform.localRotation.eulerAngles.z < 24f || transform.localRotation.eulerAngles.z > 300f)
                    transform.localRotation *= Quaternion.Euler(0f, 0f, 6f * Time.deltaTime);
                if (_mood != SpaceTargetMood.Dormant) SetMood(SpaceTargetMood.Dormant);
                return;
            }
            _armor = SpaceCombatCore.Recharge(_armor, maxArmor, Time.time, Time.deltaTime);

            float sinceHit = float.IsNegativeInfinity(_lastHitTime) ? -1f : Time.time - _lastHitTime;
            var mood = SpaceTargetReactionCore.Classify(_mood, _pilotDistance, sinceHit, false);
            if (mood != _mood) SetMood(mood);

            float bob = Mathf.Sin(Time.time * 0.8f + _bobPhase) * bobAmplitude
                        * SpaceTargetReactionCore.BobMultiplier(_mood);
            float strafe = SpaceTargetReactionCore.StrafeOffset(_mood, Time.time, _bobPhase);
            transform.localPosition = _home + Vector3.up * bob + Vector3.right * strafe;
        }

        private void SetMood(SpaceTargetMood mood)
        {
            _mood = mood;
            if (_eye != null)
                Paint(_eye, _eyeBase * SpaceTargetReactionCore.EyeIntensity(mood));
            PoseForMood(mood, _armor.Disabled);
            Debug.Log("ZIPTIDE: DRONE_MOOD target=" + name + " mood=" + mood
                + " dist=" + (float.IsInfinity(_pilotDistance) ? "inf" : _pilotDistance.ToString("F0")));
        }

        /// <summary>
        /// The pilot's closeness to claiming this wreck, 0..1 (SpaceCombatCore.SalvageApproach01).
        /// A downed drone glows toward salvage-teal as you close, so the pickup is something you
        /// can SEE coming instead of a number that changes somewhere off-screen.
        /// </summary>
        public void ReportSalvageApproach(float approach01)
        {
            if (!_armor.Disabled || _salvaged) return;
            approach01 = Mathf.Clamp01(approach01);
            if (Mathf.Abs(approach01 - _approach) < 0.02f) return;
            _approach = approach01;
            Tint(Color.Lerp(new Color(0.25f, 0.25f, 0.28f), new Color(0.40f, 0.85f, 0.72f), approach01));
        }

        /// <summary>A bolt from the player connects. Returns true when THIS hit disables the drone.</summary>
        public bool TakeHit(float damage, float now)
        {
            if (_armor.Disabled) return false;
            _lastHitTime = now;   // opens the evade window (SpaceTargetReactionCore.EvadeSeconds)
            _armor = SpaceCombatCore.Hit(_armor, damage, now);
            Tint(_armor.Disabled
                ? new Color(0.25f, 0.25f, 0.28f)                        // powered down
                : Color.Lerp(new Color(0.9f, 0.6f, 0.2f), new Color(0.85f, 0.25f, 0.15f),
                    1f - _armor.Armor / Mathf.Max(1f, maxArmor)));      // heat reads the armor
            // Powering down drops the arms slack and swings the access panel open — the salvage read
            // (measured spec §3). Re-posed here because a drone disabled while still Dormant would
            // otherwise never see a mood change.
            PoseForMood(_mood, _armor.Disabled);
            return _armor.Disabled;
        }

        /// <summary>Fly-close salvage: pays once, through the one economy, then the wreck fades.</summary>
        public double Salvage()
        {
            if (!_armor.Disabled || _salvaged) return 0;
            _salvaged = true;
            double granted = SalvageCacheRuntime.GrantTo(
                SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null,
                salvageResource, salvageAmount);
            gameObject.SetActive(false); // the wreck is claimed
            return granted;
        }

        private void Tint(Color color)
        {
            if (_renderers == null) return;
            foreach (var r in _renderers)
                Paint(r, color);
        }

        private static void Paint(Renderer r, Color color)
        {
            if (r == null || r.material == null) return;
            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
            else r.material.color = color;
        }
    }
}

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
            Debug.Log("ZIPTIDE: DRONE_MOOD target=" + name + " mood=" + mood
                + " dist=" + (float.IsInfinity(_pilotDistance) ? "inf" : _pilotDistance.ToString("F0")));
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

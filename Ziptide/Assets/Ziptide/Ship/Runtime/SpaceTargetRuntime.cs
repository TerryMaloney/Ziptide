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

        public bool Disabled => _armor.Disabled;
        public bool Salvaged => _salvaged;

        /// <summary>Lane-space position (local to the LaneContent root) — the space combat math
        /// runs in lane coordinates, same frame as FlightState.position.</summary>
        public Vector3 LanePosition => transform.localPosition;

        private void Awake()
        {
            _armor = new ShipArmorState { Armor = maxArmor, LastHitTime = float.NegativeInfinity };
            _home = transform.localPosition;
            _bobPhase = _home.x * 0.7f + _home.z * 0.3f;
            _renderers = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            if (_armor.Disabled)
            {
                // Powered down: a slow list + gentle sink, then hold. Non-lethal — it drifts, never burns.
                if (transform.localRotation.eulerAngles.z < 24f || transform.localRotation.eulerAngles.z > 300f)
                    transform.localRotation *= Quaternion.Euler(0f, 0f, 6f * Time.deltaTime);
                return;
            }
            _armor = SpaceCombatCore.Recharge(_armor, maxArmor, Time.time, Time.deltaTime);
            transform.localPosition = _home + Vector3.up * (Mathf.Sin(Time.time * 0.8f + _bobPhase) * bobAmplitude);
        }

        /// <summary>A bolt from the player connects. Returns true when THIS hit disables the drone.</summary>
        public bool TakeHit(float damage, float now)
        {
            if (_armor.Disabled) return false;
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
                if (r != null && r.material != null)
                {
                    if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
                    else r.material.color = color;
                }
        }
    }
}

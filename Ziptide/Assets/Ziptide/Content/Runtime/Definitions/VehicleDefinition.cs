using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>The locomotion archetypes (DRIVABLE_VEHICLES.md catalog — each biome gets a signature ride).</summary>
    public enum VehicleArchetype { Rover, Hoverbike, Skiff, GravSled, DrillCrawler, Walker }

    /// <summary>
    /// DRIVABLE VEHICLES 3.2 — a data-driven ground ride ("a ship for the ground",
    /// CONSISTENCY_SPINE). The runtime maps these numbers onto the SAME comfort-clamped FlightModel
    /// the ship flies with (pitch locked to zero for ground — a vehicle can never tilt the horizon),
    /// so ship and skiff share one control language and one comfort law. Assets live under
    /// Resources/Vehicles; runtimes resolve them BY ID (never hard refs — the ItemFactory law).
    /// </summary>
    [CreateAssetMenu(fileName = "VehicleDefinition", menuName = "Ziptide/Definitions/Vehicle")]
    public class VehicleDefinition : Definition
    {
        public VehicleArchetype archetype = VehicleArchetype.Hoverbike;

        [Tooltip("Biome this ride is native to (flavor/spawn hint).")]
        public string biomeId = "";

        [Header("Drive feel (mapped onto FlightModel with the comfort clamps)")]
        [Tooltip("Cruise speed in m/s.")]
        public float cruiseSpeed = 14f;

        [Tooltip("Boost multiplier while L3/A is held (clamped to the shared 3.0 ceiling).")]
        public float boostMultiplier = 1.6f;

        [Tooltip("Hover height above the ground in meters (0 = wheels on the deck).")]
        public float hoverHeight = 0.6f;

        [Tooltip("How far from its pad the ride may roam (soft wall, mirrors the flight lane).")]
        public float roamRadius = 220f;

        [Tooltip("Seat position local to the vehicle root.")]
        public Vector3 seatLocalPos = new Vector3(0f, 0.55f, -0.2f);
    }
}

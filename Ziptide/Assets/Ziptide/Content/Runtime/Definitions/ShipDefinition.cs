using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven ship: hull, cockpit, flight feel, and upgrade slots. THE NORTH STAR's data layer —
    /// the ship that replaces the travel door (board → cockpit → pick a world → fly out).
    /// Architecture + build plan: docs/systems/SHIPS.md. Nothing consumes this at runtime yet
    /// (the ShipBoardingStation / cockpit runtime is the 🔧/🎮 half); authoring it now means the
    /// runtime is built AGAINST data from day one, like every other system in the project.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Ship", fileName = "Ship")]
    public class ShipDefinition : ScriptableObject
    {
        [Tooltip("Unique ship id (e.g. rustbucket_scavenger — Cal's starter hull).")]
        public string shipId = "rustbucket_scavenger";

        [Tooltip("Name shown in the cockpit / hangar UI.")]
        public string displayName = "Rustbucket";

        [Header("Hull (graybox proportions — CityBuilder's berth ship uses these)")]
        [Tooltip("Overall hull size in meters (x=width, y=height, z=length).")]
        public Vector3 hullSize = new Vector3(5f, 3f, 12f);

        [Tooltip("Cockpit seat position, local to the hull center. The player sits HERE to fly.")]
        public Vector3 cockpitSeatLocalPos = new Vector3(0f, 0.4f, 4.2f);

        [Tooltip("Boarding door position, local to the hull center (where the boarding trigger lives).")]
        public Vector3 boardingDoorLocalPos = new Vector3(-2.2f, -1f, 0f);

        [Header("Flight feel (data for the future flight layer — see SHIPS.md phases)")]
        [Tooltip("Cruise speed in m/s (the cinematic fly-out pace).")]
        public float cruiseSpeed = 25f;
        [Tooltip("Boost multiplier over cruise.")]
        public float boostMultiplier = 2.5f;
        [Tooltip("Turn rate in deg/s (keep low — comfort-first in VR).")]
        public float turnRateDegrees = 30f;
        [Tooltip("Comfort vignette strength while flying, 0..1 (mirrors the gravity-gun hop comfort model).")]
        [Range(0f, 1f)] public float flightVignette = 0.6f;

        [Header("Upgrade slots (economy sink — items socket into the hull)")]
        public List<ShipSlotDef> slots = new List<ShipSlotDef>();

        [Header("Travel")]
        [Tooltip("Worlds this hull can reach. Empty = all unlocked worlds (gating still applies via " +
                 "WorldGating/flagsRequired — the ship UI must consult it like the travel doors do).")]
        public List<string> reachablePackIds = new List<string>();
    }

    /// <summary>An upgrade socket on the ship (engine, scanner, cargo, hull plating…).</summary>
    [Serializable]
    public class ShipSlotDef
    {
        [Tooltip("Unique slot id on this hull (e.g. engine_main, scanner_bay).")]
        public string slotId = "engine_main";

        [Tooltip("What kind of module fits (engine / scanner / cargo / plating / special).")]
        public string slotKind = "engine";

        [Tooltip("Item ids (Resources/Items) that can socket here. Empty = any item of the right kind.")]
        public List<string> acceptedItemIds = new List<string>();

        [Tooltip("Socket position local to the hull (where the module visibly mounts).")]
        public Vector3 localPos = Vector3.zero;
    }
}

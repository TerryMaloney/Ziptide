using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven description of a whole multi-district city world. This is the WORLD BLUEPRINT kit:
    /// edit the asset (or author a new one) to re-layout Toxic City or spin up an entirely new world,
    /// without touching patcher code. <see cref="Ziptide.Editor.Patching.CityBuilder"/> consumes it.
    ///
    /// Kept separate from the legacy <see cref="CityKitDefinition"/> (single-spine D1) so D0/D1 stay
    /// frozen as reference while this richer model drives the new ToxicCity scene and every world after.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/City Layout", fileName = "CityLayout")]
    public class CityLayoutDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Drives the city root name (\"__\"+CITYID+\"_ROOT\") and world pack id.")]
        public string cityId = "toxic_city";
        [Tooltip("Deterministic seed for prop scatter / window jitter.")]
        public int seed = 1337;

        [Header("Global")]
        [Tooltip("Canonical walkable Y — district ground slabs sit with their TOP at this height. The audit's spawn-on-walkway check reads this.")]
        public float walkwayHeight = 0f;
        public GlobalPalette palette = new GlobalPalette();

        [Header("Atmosphere")]
        public bool fogEnabled = true;
        public Color fogColor = new Color(0.18f, 0.22f, 0.16f);
        public float fogDensity = 0.018f;

        [Header("Skyline (distant silhouettes for scale)")]
        public float skylineRingRadius = 140f;
        public int skylineCount = 26;
        public float skylineMinHeight = 30f;
        public float skylineMaxHeight = 90f;

        [Header("Layout")]
        public List<DistrictDef> districts = new List<DistrictDef>();
        public List<ConnectionDef> connections = new List<ConnectionDef>();
        public List<CanalRegionDef> canals = new List<CanalRegionDef>();
        public List<DroneZoneDef> droneZones = new List<DroneZoneDef>();
        public ShipyardBerthDef shipyard = new ShipyardBerthDef();
    }

    /// <summary>Per-surface colors. A district may override via <see cref="DistrictDef.paletteOverride"/>.</summary>
    [Serializable]
    public class GlobalPalette
    {
        public Color concrete = new Color(0.34f, 0.35f, 0.37f);
        public Color metal = new Color(0.28f, 0.30f, 0.33f);
        public Color toxic = new Color(0.22f, 0.45f, 0.12f);
        public Color building1 = new Color(0.30f, 0.31f, 0.34f);
        public Color building2 = new Color(0.24f, 0.26f, 0.30f);
        public Color rail = new Color(0.45f, 0.28f, 0.18f);
        public Color catwalk = new Color(0.40f, 0.40f, 0.42f);
        public Color facadeWindow = new Color(0.08f, 0.12f, 0.14f);
        public Color skyline = new Color(0.16f, 0.17f, 0.20f);
        public Color accent = new Color(0.90f, 0.75f, 0.10f);
    }

    /// <summary>A walkable district: a ground/plaza slab ringed by facade buildings, with optional hero buildings.</summary>
    [Serializable]
    public class DistrictDef
    {
        public string id = "District";
        [Tooltip("District origin in world space. Ground slab top sits at walkwayHeight.")]
        public Vector3 anchor = Vector3.zero;
        [Tooltip("XZ footprint of the district plaza in meters.")]
        public Vector2 bounds = new Vector2(24f, 24f);
        [Tooltip("0..3 multiplier band for facade building heights.")]
        public int heightTier = 1;
        public bool useOverride = false;
        public GlobalPalette paletteOverride = new GlobalPalette();
        public List<LandmarkDef> landmarks = new List<LandmarkDef>();
        public List<HeroBuildingDef> heroBuildings = new List<HeroBuildingDef>();
        public List<PropPatchDef> props = new List<PropPatchDef>();
    }

    public enum InteriorKind { Empty, JobGiver, Mission }

    /// <summary>An ENTERABLE building: shell with a door gap + a small interior room + an interior marker.</summary>
    [Serializable]
    public class HeroBuildingDef
    {
        public string id = "HeroBuilding";
        [Tooltip("Position relative to the district anchor (XZ; Y ignored, sits on the slab).")]
        public Vector3 localPos = Vector3.zero;
        public Vector2 footprint = new Vector2(8f, 8f);
        public float height = 4.5f;
        public InteriorKind interior = InteriorKind.Empty;
        [Tooltip("Compass-ish offset of the door gap from the building center, in local XZ.")]
        public Vector3 doorLocalPos = new Vector3(0f, 0f, -4f);
        [Tooltip("Named spawn/objective marker placed inside the room.")]
        public string interiorMarkerId = "";
    }

    public enum ConnectionKind { GroundStreet, ElevatedWalkway, Bridge, Ramp }

    /// <summary>A street / walkway / bridge / ramp linking two districts. The list IS the street grid.</summary>
    [Serializable]
    public class ConnectionDef
    {
        public string fromDistrictId = "";
        public string toDistrictId = "";
        public ConnectionKind kind = ConnectionKind.GroundStreet;
        public float width = 6f;
        [Tooltip("Walkway tier: 0 = at walkwayHeight, 1 = one step up (over canals). Drives Y.")]
        public int tier = 0;
    }

    /// <summary>Decorative toxic sludge region (collider stripped so it never counts as floor).</summary>
    [Serializable]
    public class CanalRegionDef
    {
        public Vector3 center = Vector3.zero;
        public Vector2 size = new Vector2(10f, 30f);
        public float depth = 2f;
        public bool useOverride = false;
        public Color colorOverride = new Color(0.22f, 0.45f, 0.12f);
    }

    /// <summary>A spawn zone for N drones. combat=true adds DroneCombatBehavior. NOT every district has one.</summary>
    [Serializable]
    public class DroneZoneDef
    {
        public string id = "DroneZone";
        public Vector3 center = Vector3.zero;
        public float radius = 4f;
        public int count = 3;
        [Tooltip("Seconds before a downed drone respawns (0 = stays dead).")]
        public float respawnDelay = 0f;
        [Tooltip("If true, drones patrol/engage and fire stun bolts (Drone Combat V1).")]
        public bool combat = false;
        [Tooltip("Optional DroneCombatProfile asset name in Resources/Enemies (blank = serialized defaults).")]
        public string variantId = "";
    }

    /// <summary>A big silhouette block unique to a district (tower, refinery, etc.).</summary>
    [Serializable]
    public class LandmarkDef
    {
        public string name = "Landmark";
        public Vector3 localPos = Vector3.zero;
        public float height = 14f;
        public float width = 4f;
    }

    /// <summary>A density-scattered prop region (crates, pipes) — seeded, decorative.</summary>
    [Serializable]
    public class PropPatchDef
    {
        public string kind = "Crate";
        public Vector3 center = Vector3.zero;
        public Vector2 size = new Vector2(6f, 6f);
        [Range(0f, 1f)] public float density = 0.4f;
    }

    /// <summary>The shipyard berth + a static (non-flyable) ship placeholder.</summary>
    [Serializable]
    public class ShipyardBerthDef
    {
        public bool enabled = true;
        public Vector3 berthCenter = Vector3.zero;
        public Vector2 berthSize = new Vector2(18f, 14f);
        public Vector3 shipLocalPos = new Vector3(0f, 1.4f, 0f);
        public Vector3 shipSize = new Vector3(5f, 3f, 11f);
        public float shipRotationY = 0f;
    }
}

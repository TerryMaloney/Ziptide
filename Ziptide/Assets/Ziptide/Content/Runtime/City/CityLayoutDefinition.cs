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

        [Header("World identity (WorldStubGenerator — leave sceneName empty for hand-built worlds)")]
        [Tooltip("Scene to generate for this layout (e.g. \"W002_DryCistern\"). Empty = NOT auto-generated " +
                 "(ToxicCity has its own patcher). Non-empty = WorldStubGenerator + the build pipeline " +
                 "create/populate Scenes/Generated/<sceneName>.unity from this data alone.")]
        public string sceneName = "";
        [Tooltip("Travel-door / dev-menu label. Falls back to cityId if empty.")]
        public string displayName = "";
        [Tooltip("District id to spawn the player in. Empty = first district.")]
        public string spawnDistrictId = "";
        [Tooltip("Drop a taser + gravity gun by the spawn (combat worlds).")]
        public bool spawnStarterWeapons = false;

        [Header("Global")]
        [Tooltip("Canonical walkable Y — district ground slabs sit with their TOP at this height. The audit's spawn-on-walkway check reads this.")]
        public float walkwayHeight = 0f;
        public GlobalPalette palette = new GlobalPalette();

        [Header("Atmosphere")]
        public bool fogEnabled = true;
        public Color fogColor = new Color(0.18f, 0.22f, 0.16f);
        public float fogDensity = 0.018f;

        [Header("Sky theme (generated worlds: WorldStubGenerator authors a VisualThemeProfile from this — " +
                "THIS is the source of truth, not the theme asset)")]
        [Tooltip("Sky color at the horizon (gradient t=0).")]
        public Color skyHorizonColor = new Color(0.55f, 0.42f, 0.30f);
        [Tooltip("Sky color at the zenith (gradient t=1).")]
        public Color skyTopColor = new Color(0.25f, 0.28f, 0.35f);
        [Tooltip("Ground material tint applied by the theme.")]
        public Color themeGroundTint = new Color(0.40f, 0.42f, 0.40f);
        [Tooltip("Show a planet in the sky (the growing 'you're closer to the truth' cue).")]
        public bool planetVisible = true;
        public Color planetBaseColor = new Color(0.40f, 0.50f, 0.70f);
        public Color planetAccentColor = new Color(0.25f, 0.35f, 0.50f);
        [Tooltip("Planet apparent size in degrees (grows across chapters: ~8 early → ~25 late).")]
        public float planetAngularSize = 12f;

        [Header("Skyline (distant silhouettes for scale)")]
        public float skylineRingRadius = 140f;
        public int skylineCount = 26;
        public float skylineMinHeight = 30f;
        public float skylineMaxHeight = 90f;

        [Header("Experience layout (Quality Bar P1 — terrain/vista; disabled = classic district-only recipe)")]
        public ExperienceDef experience = new ExperienceDef();

        [Header("Points of Interest (Quality Bar P1c — the gameplay pockets; see docs/WORLD_RECIPE.md)")]
        public List<PoiDef> pois = new List<PoiDef>();

        [Header("Ring topology (CITY_VISUAL_SPEC §1 — off = the legacy rectangular districts)")]
        public RingCityDef rings = new RingCityDef();

        [Header("Layout")]
        public List<DistrictDef> districts = new List<DistrictDef>();
        public List<ConnectionDef> connections = new List<ConnectionDef>();
        public List<CanalRegionDef> canals = new List<CanalRegionDef>();
        public List<DroneZoneDef> droneZones = new List<DroneZoneDef>();
        public List<HazardZoneDef> hazards = new List<HazardZoneDef>();
        public List<CreatureZoneDef> creatureZones = new List<CreatureZoneDef>();
        public ShipyardBerthDef shipyard = new ShipyardBerthDef();

        /// <summary>
        /// Structural sanity check (pure, no scene). Returns a list of problems — empty == valid.
        /// Guards against committing an unwalkable / dangling-reference layout. Patcher + tests use it.
        /// </summary>
        public List<string> Validate()
        {
            var issues = new List<string>();

            if (float.IsNaN(walkwayHeight) || float.IsInfinity(walkwayHeight))
                issues.Add("walkwayHeight is not a finite number.");

            if (rings != null) issues.AddRange(rings.Validate());

            if (experience != null && experience.enabled)
            {
                if (experience.worldRadius < 60f)
                    issues.Add("experience.worldRadius " + experience.worldRadius + " is too small (min 60).");
                if (experience.heightAmplitude < 0f)
                    issues.Add("experience.heightAmplitude is negative.");
                if (experience.vista != VistaKind.None &&
                    new Vector2(experience.vistaDirection.x, experience.vistaDirection.z).sqrMagnitude < 0.001f)
                    issues.Add("experience.vistaDirection has no XZ direction for vista " + experience.vista + ".");
            }

            var poiIds = new HashSet<string>();
            foreach (var p in pois)
            {
                if (p == null) { issues.Add("Null POI entry."); continue; }
                if (string.IsNullOrEmpty(p.id)) { issues.Add("POI with empty id."); continue; }
                if (!poiIds.Add(p.id)) issues.Add("Duplicate POI id '" + p.id + "'.");
            }

            var ids = new HashSet<string>();
            foreach (var d in districts)
            {
                if (d == null) { issues.Add("Null district entry."); continue; }
                if (string.IsNullOrEmpty(d.id)) { issues.Add("District with empty id."); continue; }
                if (!ids.Add(d.id)) issues.Add("Duplicate district id '" + d.id + "'.");

                foreach (var hb in d.heroBuildings)
                {
                    if (hb == null) continue;
                    if (hb.interior != InteriorKind.Empty && string.IsNullOrEmpty(hb.interiorMarkerId))
                        issues.Add("Hero building '" + hb.id + "' in district '" + d.id + "' has interior but no interiorMarkerId.");
                }
            }

            foreach (var c in connections)
            {
                if (c == null) { issues.Add("Null connection entry."); continue; }
                if (!ids.Contains(c.fromDistrictId))
                    issues.Add("Connection from unknown district '" + c.fromDistrictId + "'.");
                if (!ids.Contains(c.toDistrictId))
                    issues.Add("Connection to unknown district '" + c.toDistrictId + "'.");
            }

            return issues;
        }
    }

    /// <summary>Terrain landform families for the Experience layout. Each maps to a height recipe in
    /// WorldExperienceBuilder — pick the one matching the world's WORLD_DATA fiction.</summary>
    public enum BiomePreset { None, Dunes, Mesas, Canyon, CavernFloor, TideFlats }

    /// <summary>Hero landmark families for arrival vistas — 40–80m kit assemblies the spawn faces.</summary>
    public enum VistaKind { None, GateSpire, Wreck, Monolith, CrystalForest, ArchRing }

    /// <summary>
    /// THE WORLD EXPERIENCE block (Quality Bar P1 — the post-device-test recipe fix). When enabled the
    /// builder adds a 250–400m heightfield terrain UNDER the districts (they become built pads on real
    /// land, linked by graded corridors), a cliff-bowl bound at worldRadius, and a composed arrival
    /// vista the spawn faces. Additive and default-OFF: arenas/ToxicCity/W000 keep the classic recipe.
    /// </summary>
    [Serializable]
    public class ExperienceDef
    {
        [Tooltip("Master switch. Off = classic district-only recipe (arenas, interiors, legacy).")]
        public bool enabled = false;
        [Tooltip("Latched by WorldLayoutLibrary's one-time upgrade pass so hand-edits are never re-seeded.")]
        public bool authored = false;

        [Header("Terrain")]
        public BiomePreset biome = BiomePreset.Dunes;
        [Tooltip("Playable radius in meters (target 250–400; the audit quality gate fails tiny worlds).")]
        public float worldRadius = 300f;
        [Tooltip("Max terrain relief in meters. The builder clamps to walkable slopes.")]
        public float heightAmplitude = 16f;
        [Tooltip("Terrain ground color (theme tint still applies to district slabs).")]
        public Color groundColor = new Color(0.45f, 0.38f, 0.28f);

        [Header("Arrival vista")]
        public VistaKind vista = VistaKind.GateSpire;
        [Tooltip("Direction (XZ) from the player spawn toward the hero landmark; spawn faces this.")]
        public Vector3 vistaDirection = new Vector3(0f, 0f, 1f);
        [Tooltip("Distance from spawn to the hero landmark (m). Fog is auto-thinned to keep it visible.")]
        public float vistaDistance = 170f;
        [Tooltip("Hero landmark height (m). 40–80 reads as monumental at distance.")]
        public float vistaHeight = 60f;
        public Color vistaColor = new Color(0.75f, 0.70f, 0.62f);
        [Tooltip("Emissive-accent color for the landmark's glow elements (ring, seams, crystals).")]
        public Color vistaAccentColor = new Color(0.95f, 0.80f, 0.35f);
    }

    /// <summary>
    /// The seven POI verbs (Quality Bar P1c). Each type has a builder in WorldPoiBuilder that stages a
    /// 15–30m gameplay pocket; contracts route through POIs by marker id ("poi_&lt;id&gt;").
    /// </summary>
    // Hardwiring 1.5: new verbs append at the END only — the enum serializes by value in authored
    // layout assets, so reordering would silently retype every existing POI.
    public enum PoiType
    {
        CombatCamp, HarvestGrove, MachineSite, RuinCache, CaveSecret, StoryAnchor, TravelBerth,
        Market, Shrine, RepairBay, Transit, Lookout,
    }

    /// <summary>A Point of Interest — where the gameplay lives. A world needs 5–9 with ≥3 distinct
    /// verbs (the audit quality gates enforce it).</summary>
    [Serializable]
    public class PoiDef
    {
        public string id = "poi";
        public PoiType type = PoiType.RuinCache;
        [Tooltip("World-space XZ; Y is ignored — the pocket sits on a level pad graded into the terrain.")]
        public Vector3 position = Vector3.zero;
        [Tooltip("0 = early/easy, 1 = mid, 2 = capstone. Drives encounter size and payout.")]
        public int tier = 0;
    }

    /// <summary>
    /// THE RING CITY (docs/project_art_plan/CITY_VISUAL_SPEC.md §1, measured off Terry's approved
    /// K1/K7 sheets). Concentric rings on a drowned tidal flat: the leaning scrap Tower island at the
    /// centre, shanty wedges cut by a canal ring and radial canals, a breached sea wall, a harbour
    /// wedge with a double breakwater, flyable outskirts, and the gate-pillar ring on the horizon.
    ///
    /// ⚠ WHY THIS EXISTS. The approved city was not merely unbuilt — it was UNEXPRESSIBLE. Districts
    /// are axis-aligned rectangles, canals are rectangles, and a landmark is {name, pos, height,
    /// width}, so there was no way to author a wedge, a ring canal, a breach, or a tower that leans.
    /// Every "build the city from the concept art" task was blocked on a data model, not on effort.
    ///
    /// Default OFF. A layout that never opts in renders exactly as it does today.
    /// </summary>
    [Serializable]
    public class RingCityDef
    {
        [Tooltip("Master switch. Off = the legacy rectangular districts, unchanged.")]
        public bool enabled = false;

        [Header("1. The Tower island (centre)")]
        [Tooltip("Radius of the island the hero tower rises from.")]
        public float islandRadius = 26f;
        [Tooltip("Stacked mismatched decks. K1/K2/K5 agree on the silhouette.")]
        public int towerDeckCount = 7;
        public float towerHeight = 78f;
        [Tooltip("The lean is the tower's signature. The spec says 10-15 degrees.")]
        public float towerLeanDegrees = 12f;
        [Tooltip("Cranes at the crown.")]
        public int crownCraneCount = 3;

        [Header("2. Shanty wedges + the canal ring")]
        [Tooltip("4-5 irregular wedges of stacked corrugated tenements.")]
        public int wedgeCount = 5;
        public float wedgeOuterRadius = 108f;
        [Tooltip("The ring canal that separates the island knot from the wedges.")]
        public float canalRingRadius = 62f;
        public float canalWidth = 9f;
        [Tooltip("Radial canals cutting the wedges apart.")]
        public int radialCanalCount = 4;
        [Tooltip("Causeway / foot bridges crossing the ring canal (~4 per the spec).")]
        public int causewayCount = 4;

        [Header("3. The old sea wall")]
        public float seaWallRadius = 126f;
        public float seaWallHeight = 7f;
        public float seaWallThickness = 3.5f;
        [Tooltip("Breaches are gameplay and flyover landmarks, not damage decoration.")]
        public int seaWallBreachCount = 2;

        [Header("4. The harbour wedge (south face)")]
        public float harbourAzimuthDegrees = 180f;
        public float harbourArcDegrees = 46f;
        [Tooltip("Double-armed stone breakwater enclosing the berth basin.")]
        public float breakwaterLength = 58f;

        [Header("5. The flyable outskirts")]
        public float outskirtsRadius = 190f;
        [Tooltip("Beached wreck clusters. K7 names them (RUSTBUCKET, SEAWEED) -- naming is the pattern.")]
        public int beachedWreckCount = 6;
        [Tooltip("Stilt-pier shanty villages linked by plank causeways.")]
        public int stiltVillageCount = 2;
        [Tooltip("Glowing green tide pools: they light the flats for the night flyover.")]
        public int tidePoolCount = 9;

        [Header("6. The gate pillar ring (far anchor, NOT geometry you can reach)")]
        public float gatePillarAzimuthDegrees = 45f;
        public float gatePillarDistance = 320f;
        public int gatePillarCount = 7;
        public float gatePillarHeight = 46f;

        /// <summary>Structural sanity. Empty list == valid. Pure; the audit and tests both read it.</summary>
        public List<string> Validate()
        {
            var issues = new List<string>();
            if (!enabled) return issues;

            if (islandRadius <= 0f) issues.Add("rings.islandRadius must be positive.");
            if (canalRingRadius <= islandRadius)
                issues.Add("rings.canalRingRadius must sit outside the tower island.");
            if (wedgeOuterRadius <= canalRingRadius)
                issues.Add("rings.wedgeOuterRadius must sit outside the canal ring.");
            if (seaWallRadius <= wedgeOuterRadius)
                issues.Add("rings.seaWallRadius must sit outside the wedges.");
            if (outskirtsRadius <= seaWallRadius)
                issues.Add("rings.outskirtsRadius must sit outside the sea wall.");
            if (gatePillarDistance <= outskirtsRadius)
                issues.Add("rings.gatePillarDistance must sit beyond the outskirts — the pillars are a "
                    + "horizon anchor, not walkable geometry.");

            if (wedgeCount < 3) issues.Add("rings.wedgeCount below 3 cannot read as a ring of districts.");
            if (canalWidth <= 0f) issues.Add("rings.canalWidth must be positive.");
            if (seaWallBreachCount >= wedgeCount)
                issues.Add("rings.seaWallBreachCount at or above the wedge count leaves no wall.");
            if (towerLeanDegrees < 0f || towerLeanDegrees > 30f)
                issues.Add("rings.towerLeanDegrees outside 0-30 stops reading as a lean.");
            if (harbourArcDegrees <= 0f || harbourArcDegrees >= 180f)
                issues.Add("rings.harbourArcDegrees must be a real arc under a half-turn.");

            return issues;
        }
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
        [Tooltip("ARCHITECTURE V2.5 H1: BuildingStyleDefinition id in Resources/BuildingStyles — non-empty " +
                 "opts this district into real generated buildings (lots + grammar) via BuildingBuilder. " +
                 "Empty = classic facade slabs (default; nothing changes until a layout/spec opts in).")]
        public string buildingStyleId = "";
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

    /// <summary>
    /// A spawn zone for N creatures (GAME_PLAN M3; generalizes DroneZoneDef). creatureId names a
    /// CreatureDefinition in Resources/Enemies; the builder attaches the behavior for its archetype
    /// (novel behavior ids get their own subclass in the factory).
    /// </summary>
    [Serializable]
    public class CreatureZoneDef
    {
        public string id = "CreatureZone";
        public Vector3 center = Vector3.zero;
        public float radius = 4f;
        public int count = 3;
        [Tooltip("Seconds before a disabled creature re-forms (0 = stays down).")]
        public float respawnDelay = 0f;
        [Tooltip("CreatureDefinition asset name in Resources/Enemies (e.g. swarm_bug, tendril, warden).")]
        public string creatureId = "swarm_bug";
    }

    /// <summary>The five biome hazard mechanics (GAME_PLAN M2). Non-lethal per canon — they push/slow, never kill.</summary>
    public enum HazardKind
    {
        Wind,      // steady horizontal push while inside (exterior worlds)
        Static,    // periodic zap: flash + brief slow ticks
        Flood,     // heavy movement slow while inside (tide flats, canals)
        Spore,     // slow-building fog: longer slow ticks (forest/canopy)
        Radiation  // escalating flash the longer you stay + a push back out
    }

    /// <summary>
    /// A biome hazard volume authored as LAYOUT data: the generator spawns a HazardZoneRuntime box per
    /// entry. Hazards make biomes mechanical, not just palettes — author them WITH the world.
    /// </summary>
    [Serializable]
    public class HazardZoneDef
    {
        public string id = "Hazard";
        public HazardKind kind = HazardKind.Wind;
        [Tooltip("Zone center (XZ; Y is derived from walkway height).")]
        public Vector3 center = Vector3.zero;
        [Tooltip("Box size of the volume (Y = how tall the effect reaches).")]
        public Vector3 size = new Vector3(8f, 4f, 8f);
        [Tooltip("Effect strength: Wind = push m/s · Static/Spore/Flood = slow factor is derived · Radiation = escalation rate.")]
        public float strength = 1.5f;
        [Tooltip("Wind only: horizontal push direction (normalized at runtime; zero = +X).")]
        public Vector3 direction = new Vector3(1f, 0f, 0f);
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

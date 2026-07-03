using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// THE WORLDSPEC (ARCHITECTURE V2 Q1, design docs/design/ARCHITECTURE_V2.md) — one document that
    /// declares a WHOLE world: identity, sky, terrain experience, POIs, layout, population, and pack
    /// contents. JSON round-trippable via JsonUtility so an LLM (or Terry) authors/edits a world as a
    /// single text file under docs/worldspecs/; WorldSpecCompiler (editor) fans it out into the
    /// EXISTING factory assets (CityLayoutDefinition + WorldPackDefinition) and the proven build
    /// pipeline does the rest. LAW: the spec is truth — change the spec, not the assets.
    ///
    /// Design choice: this REUSES the layout/pack [Serializable] classes verbatim (ExperienceDef,
    /// PoiDef, DistrictDef, CollectibleSpawnDefinition, …) instead of mirroring them — zero mapping
    /// drift, and the compiler is mostly straight assignment. Enums serialize as ints in JSON; the
    /// value tables live in docs/WORLD_RECIPE.md.
    ///
    /// v1 scope note: jobs/contracts stay authored by WorldJobLibrary (referenced by the world's
    /// flags, not embedded — a job DSL is a v2 extension); skyVistaId is validated against the sky
    /// library but BINDING stays with SkyVistaAuthor's scene-name mapping.
    /// </summary>
    [Serializable]
    public class WorldSpec
    {
        public int specVersion = 1;

        [Header("Identity")]
        public string sceneName = "";
        public string cityId = "";
        public string displayName = "";
        public int seed = 0;
        public string spawnDistrictId = "";
        public bool spawnStarterWeapons = false;
        public float walkwayHeight = 0f;

        [Header("Atmosphere + sky colors")]
        public bool fogEnabled = true;
        public Color fogColor = new Color(0.18f, 0.22f, 0.16f);
        public float fogDensity = 0.018f;
        public Color skyHorizonColor = new Color(0.55f, 0.42f, 0.30f);
        public Color skyTopColor = new Color(0.25f, 0.28f, 0.35f);
        public Color themeGroundTint = new Color(0.40f, 0.42f, 0.40f);
        public bool planetVisible = true;
        public Color planetBaseColor = new Color(0.40f, 0.50f, 0.70f);
        public Color planetAccentColor = new Color(0.25f, 0.35f, 0.50f);
        public float planetAngularSize = 12f;
        [Tooltip("Advisory reference to a SkyVistaLibrary id (validated; binding stays with SkyVistaAuthor).")]
        public string skyVistaId = "";

        [Header("Skyline")]
        public float skylineRingRadius = 140f;
        public int skylineCount = 26;
        public float skylineMinHeight = 30f;
        public float skylineMaxHeight = 90f;

        [Header("Experience (terrain + arrival vista) + POIs")]
        public ExperienceDef experience = new ExperienceDef();
        public List<PoiDef> pois = new List<PoiDef>();

        [Header("Layout")]
        public GlobalPalette palette = new GlobalPalette();
        public List<DistrictDef> districts = new List<DistrictDef>();
        public List<ConnectionDef> connections = new List<ConnectionDef>();
        public List<CanalRegionDef> canals = new List<CanalRegionDef>();
        public ShipyardBerthDef shipyard = new ShipyardBerthDef();

        [Header("Population")]
        public List<DroneZoneDef> droneZones = new List<DroneZoneDef>();
        public List<CreatureZoneDef> creatureZones = new List<CreatureZoneDef>();
        public List<HazardZoneDef> hazards = new List<HazardZoneDef>();

        [Header("Pack contents (spawned by JobDirector at scene start)")]
        public List<CollectibleSpawnDefinition> collectibles = new List<CollectibleSpawnDefinition>();
        public List<MachineSpawnDefinition> machines = new List<MachineSpawnDefinition>();
        public List<MineSpawnDefinition> mines = new List<MineSpawnDefinition>();
        public List<GardenSpawnDefinition> gardens = new List<GardenSpawnDefinition>();
        public List<BuildSocketSpawnDefinition> sockets = new List<BuildSocketSpawnDefinition>();

        [Header("Story gating")]
        public List<string> flagsRequired = new List<string>();
        public List<string> flagsGranted = new List<string>();

        public string ToJson() => JsonUtility.ToJson(this, prettyPrint: true);
        public static WorldSpec FromJson(string json) => JsonUtility.FromJson<WorldSpec>(json);
    }
}

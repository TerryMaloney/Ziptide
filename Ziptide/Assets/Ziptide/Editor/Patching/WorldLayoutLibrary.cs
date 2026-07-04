#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Code-authored <see cref="CityLayoutDefinition"/> assets for the story worlds (specs from
    /// docs/storyboard/WORLD_DATA.md, flow shapes from docs/design/WORLD_FLOW_TEMPLATES.md).
    /// CREATE-ONLY: an asset that already exists is never overwritten — the asset is the live, editable
    /// truth (tweak fields there); this library only seeds it the first time. The build pipeline calls
    /// <see cref="EnsureAllAuthored"/>, so new worlds ship with zero manual steps.
    ///
    /// TO ADD A WORLD: copy one Build* method, change the data, add it to EnsureAllAuthored. That's all —
    /// the WorldStubGenerator + WorldJobLibrary handle the scene/pack/jobs from there.
    /// </summary>
    public static class WorldLayoutLibrary
    {
        private const string LayoutFolder = "Assets/Ziptide/Content/City/Generated";

        [MenuItem("Ziptide/Worlds/Author Story World Layouts (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("World Layout Library",
                made + " layout asset(s) created under " + LayoutFolder + " (existing ones untouched).", "OK");
        }

        /// <summary>Create any missing story-world layout assets. Returns how many were created.</summary>
        public static int EnsureAllAuthored()
        {
            int made = 0;
            made += Ensure("W000_DriftIn", BuildW000DriftIn);
            made += Ensure("W002_DryCistern", BuildW002DryCistern);
            made += Ensure("W003_GlassShelf", BuildW003GlassShelf);
            made += Ensure("W004_BroadcastTomb", BuildW004BroadcastTomb);
            made += Ensure("W005_OxidizedCanopy", BuildW005OxidizedCanopy);
            made += Ensure("W006_MirrorFlats", BuildW006MirrorFlats);
            made += Ensure("W007_SableStation", BuildW007SableStation);
            made += Ensure("W008_SealedArchive", BuildW008SealedArchive);
            made += Ensure("W009_Chitinwall", BuildW009Chitinwall);
            made += Ensure("W010_TidalArray", BuildW010TidalArray);
            made += Ensure("W011_TheHum", BuildW011TheHum);
            made += Ensure("W012_MarasLastJump", BuildW012MarasLastJump);
            int upgraded = EnsureExperienceAuthored();
            if (made > 0 || upgraded > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        // ── QUALITY BAR P1 — the Experience upgrade pass ───────────────────────────────────────────
        // The library is CREATE-ONLY, so new CityLayoutDefinition fields never reach assets that
        // already exist on disk. This pass writes each world's ExperienceDef (terrain biome + arrival
        // vista, per its WORLD_DATA fiction) exactly ONCE — experience.authored latches, so hand-tuning
        // in the inspector afterwards is never overwritten. Runs inside EnsureAllAuthored → every build.

        private static int EnsureExperienceAuthored()
        {
            int n = 0;
            // W000 is a ship interior — the experience recipe stays OFF, latched so this never re-runs.
            n += Experience("W000_DriftIn", (kit, ex) => { ex.enabled = false; });

            n += Experience("W002_DryCistern", (kit, ex) =>
            {   // A vast drained basin — canyon channels where the water was; pump arches on the sky.
                ex.enabled = true; ex.biome = BiomePreset.Canyon; ex.worldRadius = 260f; ex.heightAmplitude = 18f;
                ex.groundColor = new Color(0.52f, 0.46f, 0.36f);
                ex.vista = VistaKind.ArchRing; ex.vistaDirection = new Vector3(0.3f, 0f, 1f);
                ex.vistaDistance = 150f; ex.vistaHeight = 55f;
                ex.vistaColor = new Color(0.62f, 0.58f, 0.48f); ex.vistaAccentColor = new Color(0.35f, 0.85f, 0.80f);
            });
            // Q2d hygiene (ALWAYS-RUN — outside the authored latch): ChamberA hosts the PumpHouse
            // hero building, and generated tenements collide with it — BUILDING_DOOR_BLOCKED caught
            // exactly that in dispatch 28693699592 (the gate doing its job). GalleryB carries the
            // building proof (seeded in BuildW002DryCistern); ChamberA must stay hero-only.
            {
                var w002 = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(
                    LayoutFolder + "/W002_DryCistern_Layout.asset");
                if (w002 != null)
                    foreach (var d in w002.districts)
                        if (d != null && d.id == "ChamberA" && !string.IsNullOrEmpty(d.buildingStyleId))
                        {
                            d.buildingStyleId = "";
                            EditorUtility.SetDirty(w002);
                            n++;
                        }
            }
            n += Experience("W003_GlassShelf", (kit, ex) =>
            {   // Wind-scoured stepped shelf; a shard monolith leans into the gale.
                ex.enabled = true; ex.biome = BiomePreset.Mesas; ex.worldRadius = 280f; ex.heightAmplitude = 20f;
                ex.groundColor = new Color(0.44f, 0.50f, 0.48f);
                ex.vista = VistaKind.Monolith; ex.vistaDirection = new Vector3(-0.5f, 0f, 1f);
                ex.vistaDistance = 170f; ex.vistaHeight = 62f;
                ex.vistaColor = new Color(0.55f, 0.62f, 0.66f); ex.vistaAccentColor = new Color(0.55f, 0.90f, 0.95f);
            });
            n += Experience("W004_BroadcastTomb", (kit, ex) =>
            {   // Ash dunes under a dead transmitter spine — the broadcast tower IS the vista.
                ex.enabled = true; ex.biome = BiomePreset.Dunes; ex.worldRadius = 280f; ex.heightAmplitude = 14f;
                ex.groundColor = new Color(0.40f, 0.37f, 0.42f);
                ex.vista = VistaKind.GateSpire; ex.vistaDirection = new Vector3(0f, 0f, 1f);
                ex.vistaDistance = 180f; ex.vistaHeight = 72f;
                ex.vistaColor = new Color(0.35f, 0.34f, 0.38f); ex.vistaAccentColor = new Color(0.95f, 0.30f, 0.25f);
            });
            n += Experience("W005_OxidizedCanopy", (kit, ex) =>
            {   // An overgrown gorge; canopy giants (crystal-forest kit reads as growth) on the ridge.
                ex.enabled = true; ex.biome = BiomePreset.Canyon; ex.worldRadius = 300f; ex.heightAmplitude = 22f;
                ex.groundColor = new Color(0.36f, 0.44f, 0.30f);
                ex.vista = VistaKind.CrystalForest; ex.vistaDirection = new Vector3(0.8f, 0f, 0.6f);
                ex.vistaDistance = 160f; ex.vistaHeight = 58f;
                ex.vistaColor = new Color(0.45f, 0.38f, 0.26f); ex.vistaAccentColor = new Color(0.55f, 0.95f, 0.45f);
            });
            n += Experience("W006_MirrorFlats", (kit, ex) =>
            {   // Blinding still flats; one monolith breaks the horizon. Emptiness is the point.
                ex.enabled = true; ex.biome = BiomePreset.TideFlats; ex.worldRadius = 320f; ex.heightAmplitude = 8f;
                ex.groundColor = new Color(0.72f, 0.72f, 0.68f);
                ex.vista = VistaKind.Monolith; ex.vistaDirection = new Vector3(1f, 0f, 0.1f);
                ex.vistaDistance = 200f; ex.vistaHeight = 66f;
                ex.vistaColor = new Color(0.80f, 0.80f, 0.78f); ex.vistaAccentColor = new Color(1.00f, 0.92f, 0.60f);
            });
            n += Experience("W007_SableStation", (kit, ex) =>
            {   // Dark mesa country; a colossal wreck marks how the station got here.
                ex.enabled = true; ex.biome = BiomePreset.Mesas; ex.worldRadius = 260f; ex.heightAmplitude = 22f;
                ex.groundColor = new Color(0.22f, 0.20f, 0.24f);
                ex.vista = VistaKind.Wreck; ex.vistaDirection = new Vector3(-1f, 0f, 0.4f);
                ex.vistaDistance = 150f; ex.vistaHeight = 48f;
                ex.vistaColor = new Color(0.30f, 0.28f, 0.30f); ex.vistaAccentColor = new Color(0.95f, 0.70f, 0.30f);
            });
            n += Experience("W008_SealedArchive", (kit, ex) =>
            {   // A hushed cavern floor; the seal-slab looms — an archive closed from the inside.
                ex.enabled = true; ex.biome = BiomePreset.CavernFloor; ex.worldRadius = 240f; ex.heightAmplitude = 10f;
                ex.groundColor = new Color(0.26f, 0.28f, 0.32f);
                ex.vista = VistaKind.Monolith; ex.vistaDirection = new Vector3(0.2f, 0f, -1f);
                ex.vistaDistance = 140f; ex.vistaHeight = 58f;
                ex.vistaColor = new Color(0.34f, 0.37f, 0.42f); ex.vistaAccentColor = new Color(0.40f, 0.80f, 0.95f);
            });
            n += Experience("W009_Chitinwall", (kit, ex) =>
            {   // Chitin dunes; the grown arch-ring is the Wall's living architecture.
                ex.enabled = true; ex.biome = BiomePreset.Dunes; ex.worldRadius = 280f; ex.heightAmplitude = 16f;
                ex.groundColor = new Color(0.46f, 0.38f, 0.28f);
                ex.vista = VistaKind.ArchRing; ex.vistaDirection = new Vector3(0.6f, 0f, -0.8f);
                ex.vistaDistance = 160f; ex.vistaHeight = 60f;
                ex.vistaColor = new Color(0.50f, 0.42f, 0.30f); ex.vistaAccentColor = new Color(0.70f, 0.85f, 0.30f);
            });
            n += Experience("W010_TidalArray", (kit, ex) =>
            {   // Wet tide flats; the array tower stands where the wrong tide answers to.
                ex.enabled = true; ex.biome = BiomePreset.TideFlats; ex.worldRadius = 320f; ex.heightAmplitude = 10f;
                ex.groundColor = new Color(0.34f, 0.44f, 0.46f);
                ex.vista = VistaKind.GateSpire; ex.vistaDirection = new Vector3(-0.7f, 0f, -0.7f);
                ex.vistaDistance = 190f; ex.vistaHeight = 68f;
                ex.vistaColor = new Color(0.40f, 0.48f, 0.52f); ex.vistaAccentColor = new Color(0.35f, 0.75f, 1.00f);
            });
            n += Experience("W011_TheHum", (kit, ex) =>
            {   // Bass-dark stone bowl; the resonant monolith is the sound made visible.
                ex.enabled = true; ex.biome = BiomePreset.CavernFloor; ex.worldRadius = 260f; ex.heightAmplitude = 12f;
                ex.groundColor = new Color(0.24f, 0.22f, 0.20f);
                ex.vista = VistaKind.Monolith; ex.vistaDirection = new Vector3(0f, 0f, 1f);
                ex.vistaDistance = 150f; ex.vistaHeight = 64f;
                ex.vistaColor = new Color(0.30f, 0.27f, 0.24f); ex.vistaAccentColor = new Color(0.90f, 0.65f, 0.25f);
            });
            n += Experience("W012_MarasLastJump", (kit, ex) =>
            {   // Burnt launch mesas; the gantry spire points at the sky she left through.
                ex.enabled = true; ex.biome = BiomePreset.Mesas; ex.worldRadius = 320f; ex.heightAmplitude = 18f;
                ex.groundColor = new Color(0.42f, 0.34f, 0.28f);
                ex.vista = VistaKind.GateSpire; ex.vistaDirection = new Vector3(0.4f, 0f, 1f);
                ex.vistaDistance = 200f; ex.vistaHeight = 78f;
                ex.vistaColor = new Color(0.46f, 0.42f, 0.40f); ex.vistaAccentColor = new Color(0.70f, 0.85f, 1.00f);
            });
            return n;
        }

        private static int Experience(string sceneName, System.Action<CityLayoutDefinition, ExperienceDef> author)
        {
            string path = LayoutFolder + "/" + sceneName + "_Layout.asset";
            var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(path);
            if (kit == null) return 0;
            if (kit.experience == null) kit.experience = new ExperienceDef();
            if (kit.experience.authored) return 0;

            author(kit, kit.experience);
            kit.experience.authored = true;
            if (kit.experience.enabled)
            {
                // Distant silhouettes must stand OUTSIDE the terrain bowl, and thick city fog would
                // swallow a 300m world — the builder also clamps fog to vista distance at build time.
                kit.skylineRingRadius = Mathf.Max(kit.skylineRingRadius, kit.experience.worldRadius + 60f);
                AuthorStandardPois(kit);
            }
            EditorUtility.SetDirty(kit);
            Debug.Log("[Ziptide] Experience authored → " + sceneName + " biome=" + kit.experience.biome +
                      " radius=" + kit.experience.worldRadius + " vista=" + kit.experience.vista +
                      " pois=" + kit.pois.Count);
            return 1;
        }

        /// <summary>
        /// THE STANDARD POI RING (P1c) — the default gameplay-pocket layout every experience world
        /// starts from: 8 POIs / 7 verbs arranged around the bowl relative to the vista direction, so
        /// the StoryAnchor sits en route to the hero landmark and the secret hides behind you.
        /// CREATE-ONLY per list: a world with ANY hand-authored POIs is never touched. Distances are
        /// fractions of worldRadius, capped inside the cliff rim. This ring passes every P1f quality
        /// gate by construction — a custom world's hand table must too (docs/WORLD_RECIPE.md).
        /// </summary>
        private static void AuthorStandardPois(CityLayoutDefinition kit)
        {
            if (kit.pois == null) kit.pois = new System.Collections.Generic.List<PoiDef>();
            if (kit.pois.Count > 0) return;
            var ex = kit.experience;

            var dir = new Vector2(ex.vistaDirection.x, ex.vistaDirection.z);
            if (dir.sqrMagnitude < 0.001f) dir = Vector2.up;
            dir.Normalize();
            float R = ex.worldRadius;

            Vector2 At(float deg, float frac)
            {
                float rad = deg * Mathf.Deg2Rad;
                var d = new Vector2(
                    dir.x * Mathf.Cos(rad) - dir.y * Mathf.Sin(rad),
                    dir.x * Mathf.Sin(rad) + dir.y * Mathf.Cos(rad));
                return d * (R * frac);
            }
            void Add(string id, PoiType type, Vector2 p, int tier = 0) =>
                kit.pois.Add(new PoiDef { id = id, type = type, position = new Vector3(p.x, 0f, p.y), tier = tier });

            Add("story", PoiType.StoryAnchor, dir * (ex.vistaDistance * 0.62f));  // on the vista sightline
            Add("camp_a", PoiType.CombatCamp, At(65f, 0.45f));
            Add("camp_b", PoiType.CombatCamp, At(-110f, 0.58f), 1);
            Add("grove", PoiType.HarvestGrove, At(150f, 0.40f));
            Add("works", PoiType.MachineSite, At(-40f, 0.50f));
            Add("ruin", PoiType.RuinCache, At(115f, 0.55f));
            Add("cave", PoiType.CaveSecret, At(-155f, 0.70f), 1);                 // far, behind the spawn
            Vector3 berth = kit.shipyard != null && kit.shipyard.enabled
                ? kit.shipyard.berthCenter
                : (kit.districts.Count > 0 ? kit.districts[0].anchor : Vector3.zero);
            Add("berth", PoiType.TravelBerth, new Vector2(berth.x, berth.z));
        }

        private static int Ensure(string sceneName, System.Func<CityLayoutDefinition> builder)
        {
            string path = LayoutFolder + "/" + sceneName + "_Layout.asset";
            if (AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(path) != null) return 0;
            Directory.CreateDirectory(LayoutFolder);
            var kit = builder();
            AssetDatabase.CreateAsset(kit, path);
            Debug.Log("[Ziptide] WorldLayoutLibrary authored " + path);
            return 1;
        }

        // ── W000 — The Drift In (YOUR SHIP: interior tutorial — wake, learn the verbs, cast off) ──
        private static CityLayoutDefinition BuildW000DriftIn()
        {
            var kit = NewKit("w000_drift_in", "W000_DriftIn", "Your Ship", seed: 2000);

            // Ship-interior feel: near-black enclosed sky, warm cabin fog, no planet/skyline.
            // (The record's "viewport on the gate-ring blooming open" awe shot is the ART TRACK's —
            // Picasso's SkyVista for this scene replaces the void when authored; flagged in HANDOFF.)
            kit.skyHorizonColor = new Color(0.04f, 0.04f, 0.06f);
            kit.skyTopColor = new Color(0.07f, 0.07f, 0.10f);
            kit.themeGroundTint = new Color(0.20f, 0.19f, 0.18f);
            kit.planetVisible = false;
            kit.fogEnabled = true; kit.fogColor = new Color(0.05f, 0.04f, 0.03f); kit.fogDensity = 0.05f;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.24f, 0.23f, 0.22f);
            kit.palette.building1 = new Color(0.20f, 0.20f, 0.21f);
            kit.palette.building2 = new Color(0.17f, 0.17f, 0.19f);
            kit.palette.accent = new Color(0.85f, 0.70f, 0.30f); // the cabin gold (matches the Quarters trim)

            // A tight two-room hangar: the bunk bay you wake in, and the berth bay with your ship.
            District(kit, "BunkBay", new Vector3(0, 0, 0), 14, 12, 0,
                landmark: ("Bunk", new Vector3(-4, 0, -3), 2.5f, 3f));
            District(kit, "BerthBay", new Vector3(0, 0, 18), 20, 18, 0,
                landmark: ("GantryCrane", new Vector3(7, 0, 5), 10f, 2f));
            Connect(kit, "BunkBay", "BerthBay", ConnectionKind.GroundStreet, 5);

            // No combat, no hazards — the tutorial's only pressure is curiosity.
            kit.spawnDistrictId = "BunkBay";
            kit.spawnStarterWeapons = true; // the taser on the rack = the grab lesson
            // YOUR ship, berthed right here — casting off IS the tutorial's final beat.
            kit.shipyard.enabled = true;
            kit.shipyard.berthCenter = new Vector3(0, 0, 20);
            kit.shipyard.shipRotationY = 0f;
            return kit;
        }

        // ── W002 — The Dry Cistern (underground: dark chambers, one light shaft, loop) ─────────────
        private static CityLayoutDefinition BuildW002DryCistern()
        {
            var kit = NewKit("dry_cistern", "W002_DryCistern", "The Dry Cistern", seed: 2002);

            // Underground feel: near-black sky, dense dark fog, no planet, no skyline ring.
            kit.skyHorizonColor = new Color(0.05f, 0.05f, 0.06f);
            kit.skyTopColor = new Color(0.09f, 0.09f, 0.11f);
            kit.themeGroundTint = new Color(0.18f, 0.17f, 0.16f);
            kit.planetVisible = false;
            kit.fogEnabled = true; kit.fogColor = new Color(0.02f, 0.02f, 0.03f); kit.fogDensity = 0.045f;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.22f, 0.21f, 0.20f);
            kit.palette.building1 = new Color(0.19f, 0.18f, 0.17f);
            kit.palette.building2 = new Color(0.15f, 0.15f, 0.16f);
            kit.palette.accent = new Color(0.85f, 0.70f, 0.30f); // lamp-glow gold

            District(kit, "CisternMouth", new Vector3(0, 0, 0), 16, 16, 0,
                landmark: ("LightShaft", new Vector3(4, 0, 4), 22f, 1.5f)); // the single shaft of surface light
            District(kit, "GalleryB", new Vector3(24, 0, 6), 16, 14, 0);
            // Q2d (V2.5 building proof, qqq envelope): GalleryB opts into real generated buildings —
            // the first district anywhere to wear BuildingGrammar. CI regenerates this layout fresh;
            // Terry's machine keeps a local asset (create-only), so his editor needs a one-time
            // delete of Content/City/Generated/W002_DryCistern_Layout.asset to see it locally.
            kit.districts[kit.districts.Count - 1].buildingStyleId = "toxic_tenement";
            District(kit, "DeepShaft", new Vector3(10, 0, 26), 14, 14, 1,
                landmark: ("OldWinch", new Vector3(-3, 0, 3), 9f, 2f));
            District(kit, "ChamberA", new Vector3(-16, 0, 20), 18, 16, 0,
                hero: ("PumpHouse", new Vector3(0, 0, 2), 8, 7, 4.5f, InteriorKind.Mission, "pump_house"));

            Connect(kit, "CisternMouth", "GalleryB", ConnectionKind.GroundStreet, 6);
            Connect(kit, "GalleryB", "DeepShaft", ConnectionKind.Ramp, 5);
            Connect(kit, "DeepShaft", "ChamberA", ConnectionKind.GroundStreet, 5);
            Connect(kit, "ChamberA", "CisternMouth", ConnectionKind.Ramp, 5); // loop closes

            // Light-grazers live in the dark gallery (M3): they GROW while unlit — the cistern's
            // darkness is their mechanic. The drone patrol stays as the ranged threat.
            kit.creatureZones.Add(new CreatureZoneDef { id = "Grazers_Dark", center = new Vector3(-6, 0, 26), radius = 3f, count = 2, respawnDelay = 30f, creatureId = "light_grazer" });
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Gallery", center = new Vector3(24, 0, 6), radius = 5f, count = 3, respawnDelay = 15f, combat = true, variantId = "drone_easy" });
            kit.spawnDistrictId = "CisternMouth";
            kit.spawnStarterWeapons = true;
            // M4-S1: W002 gets the first BOARDABLE berth — the ship starts replacing the travel door
            // on the Ch.1 band (SHIPS.md). Southwest of the loop, clear of all districts.
            kit.shipyard.enabled = true;
            kit.shipyard.berthCenter = new Vector3(-20, 0, -8);
            kit.shipyard.shipRotationY = 25f;
            return kit;
        }

        // ── W003 — Glass Shelf (exterior-open: bright mesas, bridges, two moons feel) ─────────────
        private static CityLayoutDefinition BuildW003GlassShelf()
        {
            var kit = NewKit("glass_shelf", "W003_GlassShelf", "Glass Shelf", seed: 2003);

            kit.skyHorizonColor = new Color(0.78f, 0.88f, 0.92f);
            kit.skyTopColor = new Color(0.22f, 0.38f, 0.62f);
            kit.themeGroundTint = new Color(0.58f, 0.62f, 0.60f);
            kit.planetVisible = true; // the faint geometric shimmer's stand-in: a small pale planet
            kit.planetBaseColor = new Color(0.85f, 0.85f, 0.90f);
            kit.planetAccentColor = new Color(0.70f, 0.75f, 0.85f);
            kit.planetAngularSize = 8f;
            kit.fogEnabled = false;
            kit.skylineCount = 0; // open horizon — no city ring
            kit.palette.concrete = new Color(0.62f, 0.66f, 0.64f); // pale glass-rock
            kit.palette.building1 = new Color(0.58f, 0.64f, 0.66f);
            kit.palette.building2 = new Color(0.52f, 0.58f, 0.62f);
            kit.palette.accent = new Color(0.30f, 0.75f, 0.80f);

            District(kit, "SkiffWreck", new Vector3(0, 0, 0), 18, 16, 0,
                landmark: ("CrashedSkiff", new Vector3(5, 0, -4), 4f, 6f));
            District(kit, "MesaBase", new Vector3(-24, 0, 14), 20, 18, 1);
            District(kit, "RelayShelfA", new Vector3(4, 0, 30), 16, 14, 2,
                landmark: ("BaffleRelayA", new Vector3(0, 0, 4), 12f, 2f));
            District(kit, "RelayShelfB", new Vector3(30, 0, 16), 16, 14, 1,
                landmark: ("BaffleRelayB", new Vector3(2, 0, 2), 12f, 2f));

            Connect(kit, "SkiffWreck", "MesaBase", ConnectionKind.GroundStreet, 6);
            Connect(kit, "MesaBase", "RelayShelfA", ConnectionKind.Bridge, 5);   // wind-ledge crossings
            Connect(kit, "RelayShelfA", "RelayShelfB", ConnectionKind.Ramp, 5);
            Connect(kit, "RelayShelfB", "SkiffWreck", ConnectionKind.Bridge, 5); // loop

            // No combat (tendrils are Phase-E) — the WIND is the antagonist here (M2 hazard):
            // crosswind lanes over the two exposed bridge crossings shove you toward the edge.
            kit.hazards.Add(new HazardZoneDef
            {
                id = "wind_crossing_a", kind = HazardKind.Wind,
                center = new Vector3(-10, 0, 22), size = new Vector3(14, 5, 10),
                strength = 1.6f, direction = new Vector3(1f, 0f, -0.4f)
            });
            kit.hazards.Add(new HazardZoneDef
            {
                id = "wind_crossing_b", kind = HazardKind.Wind,
                center = new Vector3(15, 0, 8), size = new Vector3(12, 5, 10),
                strength = 1.9f, direction = new Vector3(-0.6f, 0f, 1f)
            });
            kit.spawnDistrictId = "SkiffWreck";
            kit.spawnStarterWeapons = false;
            // M4-S1: Ch.1 band gets boardable berths (the ship replaces the door).
            kit.shipyard.enabled = true;
            kit.shipyard.berthCenter = new Vector3(18, 0, -12);
            kit.shipyard.shipRotationY = -20f;
            return kit;
        }

        // ── W004 — The Broadcast Tomb (interior dread: dark corridors, dead screens, no combat) ───
        private static CityLayoutDefinition BuildW004BroadcastTomb()
        {
            var kit = NewKit("broadcast_tomb", "W004_BroadcastTomb", "The Broadcast Tomb", seed: 2004);

            kit.skyHorizonColor = new Color(0.03f, 0.03f, 0.04f);
            kit.skyTopColor = new Color(0.05f, 0.05f, 0.07f);
            kit.themeGroundTint = new Color(0.24f, 0.24f, 0.26f); // ash grey
            kit.planetVisible = false;
            kit.fogEnabled = true; kit.fogColor = new Color(0.09f, 0.10f, 0.13f); kit.fogDensity = 0.06f;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.26f, 0.26f, 0.28f);
            kit.palette.building1 = new Color(0.20f, 0.21f, 0.23f);
            kit.palette.building2 = new Color(0.16f, 0.17f, 0.20f);
            kit.palette.facadeWindow = new Color(0.02f, 0.03f, 0.04f); // dead screens
            kit.palette.accent = new Color(0.25f, 0.55f, 0.95f);       // the ONE flickering screen / static-blue

            District(kit, "TombEntry", new Vector3(0, 0, 0), 14, 12, 0);
            District(kit, "JunctionA", new Vector3(-18, 0, 12), 14, 12, 1,
                hero: ("ScreenWallA", new Vector3(0, 0, 2), 8, 6, 5f, InteriorKind.Mission, "junction_a"));
            District(kit, "JunctionB", new Vector3(2, 0, 26), 14, 12, 1,
                hero: ("ScreenWallB", new Vector3(0, 0, 2), 8, 6, 5f, InteriorKind.Mission, "junction_b"));
            District(kit, "BroadcastCore", new Vector3(-16, 0, 40), 16, 14, 2,
                landmark: ("BroadcastSpine", new Vector3(0, 0, 4), 26f, 3f));

            // Linear-with-a-loop: corridor chain, tight widths = claustrophobic.
            Connect(kit, "TombEntry", "JunctionA", ConnectionKind.GroundStreet, 4);
            Connect(kit, "JunctionA", "JunctionB", ConnectionKind.GroundStreet, 4);
            Connect(kit, "JunctionB", "BroadcastCore", ConnectionKind.GroundStreet, 4);
            Connect(kit, "JunctionA", "BroadcastCore", ConnectionKind.Ramp, 4);

            // Deliberately NO enemies — dread, not combat (WORLD_DATA/W004 README).
            kit.spawnDistrictId = "TombEntry";
            kit.spawnStarterWeapons = false;
            // M4-S1: Ch.1 band berth.
            kit.shipyard.enabled = true;
            kit.shipyard.berthCenter = new Vector3(20, 0, -10);
            return kit;
        }

        // ── W005 — Oxidized Canopy (forest: rust dusk, vertical branch walkways, first Bloom world) ─
        private static CityLayoutDefinition BuildW005OxidizedCanopy()
        {
            var kit = NewKit("oxidized_canopy", "W005_OxidizedCanopy", "Oxidized Canopy", seed: 2005);

            kit.skyHorizonColor = new Color(0.85f, 0.45f, 0.20f); // rust-orange dusk
            kit.skyTopColor = new Color(0.35f, 0.18f, 0.14f);
            kit.themeGroundTint = new Color(0.35f, 0.24f, 0.14f);
            kit.planetVisible = true; kit.planetAngularSize = 14f; // bigger now — Ch.2's "closer to the truth"
            kit.planetBaseColor = new Color(0.55f, 0.45f, 0.60f);
            kit.planetAccentColor = new Color(0.35f, 0.28f, 0.45f);
            kit.fogEnabled = true; kit.fogColor = new Color(0.30f, 0.16f, 0.08f); kit.fogDensity = 0.02f;
            kit.skylineCount = 10; kit.skylineMinHeight = 18f; kit.skylineMaxHeight = 40f; // distant "trees"
            kit.palette.concrete = new Color(0.35f, 0.26f, 0.18f); // rusted bark
            kit.palette.building1 = new Color(0.42f, 0.28f, 0.16f);
            kit.palette.building2 = new Color(0.30f, 0.22f, 0.14f);
            kit.palette.accent = new Color(0.90f, 0.55f, 0.15f);   // spore-glow orange

            District(kit, "ForestFloor", new Vector3(0, 0, 0), 18, 16, 0);
            District(kit, "CanopyLift", new Vector3(-22, 0, 12), 16, 14, 2,
                landmark: ("LiftTrunk", new Vector3(0, 0, 3), 24f, 3f));
            District(kit, "GroveEdge", new Vector3(24, 0, 10), 16, 16, 1);
            District(kit, "Scrubber", new Vector3(2, 0, 30), 16, 14, 1,
                hero: ("SporeScrubber", new Vector3(0, 0, 2), 8, 7, 5f, InteriorKind.Mission, "scrubber"));

            Connect(kit, "ForestFloor", "CanopyLift", ConnectionKind.Ramp, 5);        // climb into the canopy
            Connect(kit, "CanopyLift", "Scrubber", ConnectionKind.ElevatedWalkway, 5, tier: 1);
            Connect(kit, "Scrubber", "GroveEdge", ConnectionKind.Ramp, 5);
            Connect(kit, "GroveEdge", "ForestFloor", ConnectionKind.GroundStreet, 6); // loop

            kit.creatureZones.Add(new CreatureZoneDef { id = "Swarm_Canopy", center = new Vector3(18, 0, 22), radius = 4f, count = 2, respawnDelay = 25f, creatureId = "swarm_bug" });
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Canopy", center = new Vector3(2, 0, 30), radius = 6f, count = 4, respawnDelay = 14f, combat = true, variantId = "drone_standard" });
            // Spore pockets under the canopy (M2 hazard): linger and the fog slows you — the scrub job
            // reads as actually needed. One on the forest floor, one by the grove.
            kit.hazards.Add(new HazardZoneDef
            {
                id = "spore_floor", kind = HazardKind.Spore,
                center = new Vector3(-8, 0, 6), size = new Vector3(10, 4, 10), strength = 1f
            });
            kit.hazards.Add(new HazardZoneDef
            {
                id = "spore_grove", kind = HazardKind.Spore,
                center = new Vector3(18, 0, 20), size = new Vector3(12, 4, 8), strength = 1f
            });
            kit.spawnDistrictId = "ForestFloor";
            kit.spawnStarterWeapons = true;
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W006 — The Mirror Flats (exterior puzzle plain: blinding salt, prism towers, watched) ──
        private static CityLayoutDefinition BuildW006MirrorFlats()
        {
            var kit = NewKit("mirror_flats", "W006_MirrorFlats", "The Mirror Flats", seed: 2006);

            kit.skyHorizonColor = new Color(0.93f, 0.91f, 0.86f); // blinding glare
            kit.skyTopColor = new Color(0.55f, 0.66f, 0.78f);
            kit.themeGroundTint = new Color(0.80f, 0.80f, 0.78f); // salt-mirror
            kit.planetVisible = true; kit.planetAngularSize = 14f;
            kit.planetBaseColor = new Color(0.75f, 0.78f, 0.85f);
            kit.planetAccentColor = new Color(0.60f, 0.66f, 0.78f);
            kit.fogEnabled = false;
            kit.skylineCount = 0; // erased horizon
            kit.palette.concrete = new Color(0.78f, 0.78f, 0.76f);
            kit.palette.building1 = new Color(0.72f, 0.74f, 0.76f);
            kit.palette.building2 = new Color(0.66f, 0.70f, 0.74f);
            kit.palette.accent = new Color(0.95f, 0.90f, 0.55f);   // prism-light gold

            District(kit, "FlatsEdge", new Vector3(0, 0, 0), 20, 18, 0);
            District(kit, "PrismTowerA", new Vector3(-26, 0, 14), 14, 12, 0,
                landmark: ("PrismA", new Vector3(0, 0, 2), 18f, 2f));
            District(kit, "PrismTowerB", new Vector3(26, 0, 16), 14, 12, 0,
                landmark: ("PrismB", new Vector3(0, 0, 2), 18f, 2f));
            District(kit, "BeamCollector", new Vector3(0, 0, 34), 16, 14, 1,
                landmark: ("Collector", new Vector3(0, 0, 3), 14f, 4f));

            Connect(kit, "FlatsEdge", "PrismTowerA", ConnectionKind.GroundStreet, 8); // wide open walks
            Connect(kit, "FlatsEdge", "PrismTowerB", ConnectionKind.GroundStreet, 8);
            Connect(kit, "PrismTowerA", "BeamCollector", ConnectionKind.GroundStreet, 7);
            Connect(kit, "PrismTowerB", "BeamCollector", ConnectionKind.GroundStreet, 7);

            // No enemies — the tension IS the emptiness (your reflection lags a half-second).
            kit.spawnDistrictId = "FlatsEdge";
            kit.spawnStarterWeapons = false;
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W007 — Sable Station (station: dark metal corridors, the gridded world in the viewport) ─
        private static CityLayoutDefinition BuildW007SableStation()
        {
            var kit = NewKit("sable_station", "W007_SableStation", "Sable Station", seed: 2007);

            kit.skyHorizonColor = new Color(0.02f, 0.02f, 0.05f); // raw space
            kit.skyTopColor = new Color(0.01f, 0.01f, 0.03f);
            kit.themeGroundTint = new Color(0.20f, 0.21f, 0.24f);
            kit.planetVisible = true; kit.planetAngularSize = 22f; // HUGE in the viewport — first clear Shell glimpse
            kit.planetBaseColor = new Color(0.30f, 0.45f, 0.55f);
            kit.planetAccentColor = new Color(0.15f, 0.60f, 0.70f); // the hexagonal grid's cyan
            kit.fogEnabled = false;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.24f, 0.25f, 0.28f); // station deck
            kit.palette.building1 = new Color(0.20f, 0.22f, 0.26f);
            kit.palette.building2 = new Color(0.16f, 0.18f, 0.22f);
            kit.palette.accent = new Color(0.90f, 0.25f, 0.20f);   // Sable red

            District(kit, "Dock", new Vector3(0, 0, 0), 16, 14, 0);
            District(kit, "AirlockRow", new Vector3(-20, 0, 10), 14, 12, 1,
                hero: ("Airlock", new Vector3(0, 0, 2), 7, 6, 4.5f, InteriorKind.Mission, "airlock"));
            District(kit, "FuelRig", new Vector3(4, 0, 26), 16, 14, 1,
                landmark: ("FuelGantry", new Vector3(3, 0, 3), 16f, 3f));
            District(kit, "ObservationDeck", new Vector3(-18, 0, 38), 16, 12, 2,
                landmark: ("Viewport", new Vector3(0, 0, 4), 10f, 8f));

            Connect(kit, "Dock", "AirlockRow", ConnectionKind.GroundStreet, 4);       // tight corridors
            Connect(kit, "AirlockRow", "FuelRig", ConnectionKind.GroundStreet, 4);
            Connect(kit, "FuelRig", "ObservationDeck", ConnectionKind.Ramp, 4);
            Connect(kit, "AirlockRow", "ObservationDeck", ConnectionKind.ElevatedWalkway, 4, tier: 1);

            // Sable guards = Phase-E (non-lethal standoff); no zones for now.
            kit.spawnDistrictId = "Dock";
            kit.spawnStarterWeapons = false;
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W008 — The Sealed Archive (interior lore vault: aged stone, gold light, walled star map) ─
        private static CityLayoutDefinition BuildW008SealedArchive()
        {
            var kit = NewKit("sealed_archive", "W008_SealedArchive", "The Sealed Archive", seed: 2008);

            kit.skyHorizonColor = new Color(0.06f, 0.05f, 0.04f);
            kit.skyTopColor = new Color(0.10f, 0.08f, 0.06f);      // domed dark, warm-tinged
            kit.themeGroundTint = new Color(0.30f, 0.27f, 0.22f);  // aged stone
            kit.planetVisible = false;
            kit.fogEnabled = true; kit.fogColor = new Color(0.10f, 0.08f, 0.05f); kit.fogDensity = 0.035f;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.34f, 0.30f, 0.24f);
            kit.palette.building1 = new Color(0.30f, 0.26f, 0.20f);
            kit.palette.building2 = new Color(0.24f, 0.21f, 0.17f);
            kit.palette.accent = new Color(0.95f, 0.80f, 0.40f);   // archive-lamp gold

            District(kit, "ArchiveMouth", new Vector3(0, 0, 0), 14, 12, 0);
            District(kit, "VaultDoorHall", new Vector3(-18, 0, 10), 14, 12, 1,
                hero: ("VaultDoor", new Vector3(0, 0, 2), 8, 6, 5f, InteriorKind.Mission, "vault_door"));
            District(kit, "PowerCore", new Vector3(2, 0, 24), 14, 12, 1,
                hero: ("CoreHousing", new Vector3(0, 0, 2), 7, 6, 4.5f, InteriorKind.Mission, "power_core"));
            District(kit, "ReaderHall", new Vector3(-16, 0, 36), 16, 14, 2,
                landmark: ("StarMapDome", new Vector3(0, 0, 4), 15f, 6f));

            Connect(kit, "ArchiveMouth", "VaultDoorHall", ConnectionKind.GroundStreet, 4);
            Connect(kit, "VaultDoorHall", "PowerCore", ConnectionKind.GroundStreet, 4);
            Connect(kit, "PowerCore", "ReaderHall", ConnectionKind.GroundStreet, 4);
            Connect(kit, "VaultDoorHall", "ReaderHall", ConnectionKind.Ramp, 4);

            kit.spawnDistrictId = "ArchiveMouth";
            kit.spawnStarterWeapons = false; // no enemies — a reading world
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W009 — Chitinwall (swarm city: carapace alleys, insect-aurora, combat-forward) ─────────
        private static CityLayoutDefinition BuildW009Chitinwall()
        {
            var kit = NewKit("chitinwall", "W009_Chitinwall", "Chitinwall", seed: 2009);

            kit.skyHorizonColor = new Color(0.45f, 0.30f, 0.12f); // insectile aurora — amber churn
            kit.skyTopColor = new Color(0.18f, 0.10f, 0.22f);     // into violet
            kit.themeGroundTint = new Color(0.30f, 0.24f, 0.16f);
            kit.planetVisible = true; kit.planetAngularSize = 16f; // clearly banded by the grid now
            kit.planetBaseColor = new Color(0.45f, 0.40f, 0.55f);
            kit.planetAccentColor = new Color(0.20f, 0.55f, 0.60f);
            kit.fogEnabled = true; kit.fogColor = new Color(0.20f, 0.14f, 0.08f); kit.fogDensity = 0.02f;
            kit.skylineCount = 22; kit.skylineMinHeight = 20f; kit.skylineMaxHeight = 60f; // the wall itself
            kit.palette.concrete = new Color(0.36f, 0.28f, 0.18f); // chitin browns
            kit.palette.building1 = new Color(0.42f, 0.32f, 0.20f);
            kit.palette.building2 = new Color(0.30f, 0.24f, 0.16f);
            kit.palette.accent = new Color(0.85f, 0.60f, 0.20f);

            District(kit, "Undercity", new Vector3(0, 0, 0), 18, 16, 1);
            District(kit, "WallGate", new Vector3(-22, 0, 10), 14, 12, 2,
                hero: ("GateHouse", new Vector3(0, 0, 2), 8, 6, 5f, InteriorKind.Mission, "wall_gate"));
            District(kit, "HiveMarket", new Vector3(22, 0, 12), 18, 16, 1,
                props: ("Stall", Vector3.zero, 12, 12, 0.5f));
            District(kit, "PylonArray", new Vector3(0, 0, 32), 16, 14, 2,
                landmark: ("DeterrentPylon", new Vector3(0, 0, 3), 20f, 2.5f));

            Connect(kit, "Undercity", "WallGate", ConnectionKind.GroundStreet, 5);  // tight alleys
            Connect(kit, "Undercity", "HiveMarket", ConnectionKind.GroundStreet, 5);
            Connect(kit, "WallGate", "PylonArray", ConnectionKind.ElevatedWalkway, 5, tier: 1);
            Connect(kit, "HiveMarket", "PylonArray", ConnectionKind.Ramp, 5);
            Connect(kit, "WallGate", "HiveMarket", ConnectionKind.Bridge, 5);       // vertical loop

            // The signature swarm world (drone stand-ins until Phase E): two pushy patrols.
            kit.creatureZones.Add(new CreatureZoneDef { id = "Tether_Wall", center = new Vector3(-18, 0, 20), radius = 3f, count = 1, respawnDelay = 40f, creatureId = "tether_swarm" });
            kit.creatureZones.Add(new CreatureZoneDef { id = "Molters_Wall", center = new Vector3(-10, 0, 30), radius = 4f, count = 2, respawnDelay = 30f, creatureId = "husk_molter" });
            kit.droneZones.Add(new DroneZoneDef { id = "Swarm_Market", center = new Vector3(22, 0, 12), radius = 6f, count = 3, respawnDelay = 12f, combat = true, variantId = "drone_standard" });
            kit.droneZones.Add(new DroneZoneDef { id = "Swarm_Pylons", center = new Vector3(0, 0, 32), radius = 5f, count = 3, respawnDelay = 12f, combat = true, variantId = "drone_standard" });
            kit.spawnDistrictId = "Undercity";
            kit.spawnStarterWeapons = true;
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W010 — Tidal Array (coastal: ringed planet, storm light, tide canals, turbine route) ───
        private static CityLayoutDefinition BuildW010TidalArray()
        {
            var kit = NewKit("tidal_array", "W010_TidalArray", "Tidal Array", seed: 2010);

            kit.skyHorizonColor = new Color(0.55f, 0.58f, 0.50f); // storm light
            kit.skyTopColor = new Color(0.20f, 0.26f, 0.34f);
            kit.themeGroundTint = new Color(0.36f, 0.40f, 0.38f); // wet slate
            kit.planetVisible = true; kit.planetAngularSize = 18f; // the vast ringed tide-puller
            kit.planetBaseColor = new Color(0.60f, 0.55f, 0.45f);
            kit.planetAccentColor = new Color(0.80f, 0.75f, 0.60f);
            kit.fogEnabled = true; kit.fogColor = new Color(0.30f, 0.34f, 0.36f); kit.fogDensity = 0.015f;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.38f, 0.40f, 0.40f);
            kit.palette.building1 = new Color(0.34f, 0.38f, 0.40f);
            kit.palette.building2 = new Color(0.28f, 0.32f, 0.36f);
            kit.palette.toxic = new Color(0.16f, 0.35f, 0.30f);    // the luminous tide
            kit.palette.accent = new Color(0.30f, 0.85f, 0.75f);

            District(kit, "ShoreCamp", new Vector3(0, 0, 0), 18, 14, 0);
            District(kit, "TurbineA", new Vector3(-24, 0, 16), 14, 12, 1,
                landmark: ("Turbine_A", new Vector3(0, 0, 3), 15f, 3f));
            District(kit, "TurbineB", new Vector3(24, 0, 18), 14, 12, 1,
                landmark: ("Turbine_B", new Vector3(0, 0, 3), 15f, 3f));
            District(kit, "SaltWorks", new Vector3(0, 0, 36), 16, 12, 1);

            // High-ground crossings over the tide flats — the flood-timing feel in graybox.
            Connect(kit, "ShoreCamp", "TurbineA", ConnectionKind.Bridge, 5);
            Connect(kit, "ShoreCamp", "TurbineB", ConnectionKind.Bridge, 5);
            Connect(kit, "TurbineA", "SaltWorks", ConnectionKind.ElevatedWalkway, 5, tier: 1);
            Connect(kit, "TurbineB", "SaltWorks", ConnectionKind.ElevatedWalkway, 5, tier: 1);

            // The tide itself: luminous no-stand flats between the districts.
            kit.canals.Add(new CanalRegionDef { center = new Vector3(-12, 0, 10), size = new Vector2(14, 24), depth = 2f });
            kit.canals.Add(new CanalRegionDef { center = new Vector3(12, 0, 12), size = new Vector2(14, 24), depth = 2f });

            // Flood drag over the same flats (M2 hazard): wade off the bridges and the tide slows you
            // hard — the crossings become the safe route, mechanically not just visually.
            kit.hazards.Add(new HazardZoneDef
            {
                id = "tide_west", kind = HazardKind.Flood,
                center = new Vector3(-12, 0, 10), size = new Vector3(14, 3, 24), strength = 1f
            });
            kit.hazards.Add(new HazardZoneDef
            {
                id = "tide_east", kind = HazardKind.Flood,
                center = new Vector3(12, 0, 12), size = new Vector3(14, 3, 24), strength = 1f
            });

            kit.spawnDistrictId = "ShoreCamp";
            kit.spawnStarterWeapons = false; // tendril world — Phase E
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W011 — The Hum (underground resonance: warm dark, pulsing crystal accents, no combat) ──
        private static CityLayoutDefinition BuildW011TheHum()
        {
            var kit = NewKit("the_hum", "W011_TheHum", "The Hum", seed: 2011);

            kit.skyHorizonColor = new Color(0.06f, 0.04f, 0.05f);
            kit.skyTopColor = new Color(0.10f, 0.07f, 0.09f);
            kit.themeGroundTint = new Color(0.22f, 0.18f, 0.18f);
            kit.planetVisible = false;
            kit.fogEnabled = true; kit.fogColor = new Color(0.06f, 0.03f, 0.05f); kit.fogDensity = 0.04f;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.26f, 0.22f, 0.22f);
            kit.palette.building1 = new Color(0.22f, 0.18f, 0.20f);
            kit.palette.building2 = new Color(0.18f, 0.15f, 0.17f);
            kit.palette.accent = new Color(0.90f, 0.30f, 0.55f);   // the crystals pulsing in time

            District(kit, "TunnelMouth", new Vector3(0, 0, 0), 14, 12, 0);
            District(kit, "ResonatorA", new Vector3(-18, 0, 12), 14, 12, 1,
                hero: ("BankA", new Vector3(0, 0, 2), 7, 6, 4.5f, InteriorKind.Mission, "resonator_bank_a"));
            District(kit, "ResonatorB", new Vector3(4, 0, 26), 14, 12, 1,
                hero: ("BankB", new Vector3(0, 0, 2), 7, 6, 4.5f, InteriorKind.Mission, "resonator_bank_b"));
            District(kit, "MinersCamp", new Vector3(-16, 0, 38), 14, 12, 0,
                landmark: ("CrystalChord", new Vector3(0, 0, 3), 12f, 2f));

            Connect(kit, "TunnelMouth", "ResonatorA", ConnectionKind.GroundStreet, 4);
            Connect(kit, "ResonatorA", "ResonatorB", ConnectionKind.Ramp, 4);
            Connect(kit, "ResonatorB", "MinersCamp", ConnectionKind.GroundStreet, 4);
            Connect(kit, "ResonatorA", "MinersCamp", ConnectionKind.Ramp, 4);

            kit.spawnDistrictId = "TunnelMouth";
            kit.spawnStarterWeapons = false; // no enemies — the Hum is the presence
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── W012 — Mara's Last Jump (void: the Shell wall fills the sky; Ch.2 capstone) ────────────
        private static CityLayoutDefinition BuildW012MarasLastJump()
        {
            var kit = NewKit("maras_last_jump", "W012_MarasLastJump", "Mara's Last Jump", seed: 2012);

            kit.skyHorizonColor = new Color(0.01f, 0.01f, 0.03f); // raw void
            kit.skyTopColor = new Color(0.03f, 0.03f, 0.06f);
            kit.themeGroundTint = new Color(0.20f, 0.21f, 0.25f); // gantry steel
            kit.planetVisible = true; kit.planetAngularSize = 30f; // the SHELL — unmissable, hexagonal-cyan
            kit.planetBaseColor = new Color(0.10f, 0.20f, 0.26f);
            kit.planetAccentColor = new Color(0.20f, 0.75f, 0.85f);
            kit.fogEnabled = false;
            kit.skylineCount = 0;
            kit.palette.concrete = new Color(0.24f, 0.25f, 0.30f);
            kit.palette.building1 = new Color(0.20f, 0.22f, 0.28f);
            kit.palette.building2 = new Color(0.16f, 0.18f, 0.24f);
            kit.palette.accent = new Color(0.95f, 0.65f, 0.15f);   // hazard-gantry orange

            District(kit, "Gantry", new Vector3(0, 0, 0), 16, 12, 0,
                landmark: ("JumpGantry", new Vector3(4, 0, 3), 18f, 2.5f));
            District(kit, "GateCoreA", new Vector3(-20, 0, 14), 14, 12, 1,
                hero: ("CoreA", new Vector3(0, 0, 2), 7, 6, 4.5f, InteriorKind.Mission, "gate_core_a"));
            District(kit, "GateCoreB", new Vector3(6, 0, 28), 14, 12, 1,
                hero: ("CoreB", new Vector3(0, 0, 2), 7, 6, 4.5f, InteriorKind.Mission, "gate_core_b"));
            District(kit, "LaunchPoint", new Vector3(-16, 0, 42), 14, 10, 2,
                landmark: ("MarasShip", new Vector3(0, 0, 2), 4f, 6f));

            // Drifting-debris feel: everything is bridges over void (the fall net catches misses).
            Connect(kit, "Gantry", "GateCoreA", ConnectionKind.Bridge, 4);
            Connect(kit, "GateCoreA", "GateCoreB", ConnectionKind.Bridge, 4);
            Connect(kit, "GateCoreB", "LaunchPoint", ConnectionKind.Bridge, 4);
            Connect(kit, "GateCoreA", "LaunchPoint", ConnectionKind.ElevatedWalkway, 4, tier: 1);

            // The Shell's sentinel at the failing gate: DORMANT statue on a fresh save —
            // but W010 grants SIGNAL_THRESHOLD_2, so by the time you stand here it WATCHES,
            // and crowding it during the capstone gets you warned. The world reacts to you.
            kit.creatureZones.Add(new CreatureZoneDef { id = "Warden_Gate", center = new Vector3(-14, 0, 42), radius = 1f, count = 1, respawnDelay = 0f, creatureId = "warden" });
            kit.spawnDistrictId = "Gantry";
            kit.spawnStarterWeapons = false; // the void is the enemy
            kit.shipyard.enabled = false;
            return kit;
        }

        // ── Spec helpers (keep the world builders readable) ────────────────────────────────────────
        private static CityLayoutDefinition NewKit(string cityId, string sceneName, string displayName, int seed)
        {
            var kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
            kit.cityId = cityId;
            kit.sceneName = sceneName;
            kit.displayName = displayName;
            kit.seed = seed;
            kit.walkwayHeight = 0f;
            return kit;
        }

        private static void District(CityLayoutDefinition kit, string id, Vector3 anchor, float w, float l, int tier,
            (string name, Vector3 pos, float height, float width)? landmark = null,
            (string id, Vector3 pos, float fw, float fl, float h, InteriorKind kind, string markerId)? hero = null,
            (string kind, Vector3 center, float pw, float pl, float density)? props = null)
        {
            var d = new DistrictDef { id = id, anchor = anchor, bounds = new Vector2(w, l), heightTier = tier };
            if (landmark.HasValue)
                d.landmarks.Add(new LandmarkDef { name = landmark.Value.name, localPos = landmark.Value.pos, height = landmark.Value.height, width = landmark.Value.width });
            if (hero.HasValue)
                d.heroBuildings.Add(new HeroBuildingDef
                {
                    id = hero.Value.id, localPos = hero.Value.pos, footprint = new Vector2(hero.Value.fw, hero.Value.fl),
                    height = hero.Value.h, interior = hero.Value.kind,
                    doorLocalPos = new Vector3(0f, 0f, -hero.Value.fl / 2f), interiorMarkerId = hero.Value.markerId,
                });
            if (props.HasValue)
                d.props.Add(new PropPatchDef { kind = props.Value.kind, center = props.Value.center, size = new Vector2(props.Value.pw, props.Value.pl), density = props.Value.density });
            kit.districts.Add(d);
        }

        private static void Connect(CityLayoutDefinition kit, string from, string to, ConnectionKind kind, float width, int tier = 0)
            => kit.connections.Add(new ConnectionDef { fromDistrictId = from, toDistrictId = to, kind = kind, width = width, tier = tier });
    }
}
#endif

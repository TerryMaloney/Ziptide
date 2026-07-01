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
            made += Ensure("W002_DryCistern", BuildW002DryCistern);
            made += Ensure("W003_GlassShelf", BuildW003GlassShelf);
            made += Ensure("W004_BroadcastTomb", BuildW004BroadcastTomb);
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
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
            District(kit, "DeepShaft", new Vector3(10, 0, 26), 14, 14, 1,
                landmark: ("OldWinch", new Vector3(-3, 0, 3), 9f, 2f));
            District(kit, "ChamberA", new Vector3(-16, 0, 20), 18, 16, 0,
                hero: ("PumpHouse", new Vector3(0, 0, 2), 8, 7, 4.5f, InteriorKind.Mission, "pump_house"));

            Connect(kit, "CisternMouth", "GalleryB", ConnectionKind.GroundStreet, 6);
            Connect(kit, "GalleryB", "DeepShaft", ConnectionKind.Ramp, 5);
            Connect(kit, "DeepShaft", "ChamberA", ConnectionKind.GroundStreet, 5);
            Connect(kit, "ChamberA", "CisternMouth", ConnectionKind.Ramp, 5); // loop closes

            // Swarm world — drone stand-in until Phase-E creature runtime (WORLD_DATA W002 note).
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Gallery", center = new Vector3(24, 0, 6), radius = 5f, count = 3, respawnDelay = 15f, combat = true, variantId = "drone_easy" });
            kit.spawnDistrictId = "CisternMouth";
            kit.spawnStarterWeapons = true;
            kit.shipyard.enabled = false;
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

            // No combat (tendrils are Phase-E) — a breather world on purpose.
            kit.spawnDistrictId = "SkiffWreck";
            kit.spawnStarterWeapons = false;
            kit.shipyard.enabled = false;
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
            (string id, Vector3 pos, float fw, float fl, float h, InteriorKind kind, string markerId)? hero = null)
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
            kit.districts.Add(d);
        }

        private static void Connect(CityLayoutDefinition kit, string from, string to, ConnectionKind kind, float width)
            => kit.connections.Add(new ConnectionDef { fromDistrictId = from, toDistrictId = to, kind = kind, width = width });
    }
}
#endif

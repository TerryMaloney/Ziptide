#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// The five launch arenas (PVP_ARENA_AAA §A2) as code-authored specs — create-only, so once an
    /// asset exists it's the editable truth. Each is themed on a story biome and designed to FPS-map
    /// fundamentals: figure-8 flow, no single power sightline, a flank route, verticality, spawns >12m
    /// apart. TO ADD AN ARENA: copy a Build* method, change the data, add it to Specs().
    /// </summary>
    public static class ArenaLayoutLibrary
    {
        private const string Folder = "Assets/Ziptide/Content/Arenas/Generated";

        [MenuItem("Ziptide/Worlds/Author Arena Layouts (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("Arena Layout Library",
                made + " arena layout(s) created under " + Folder + " (existing ones untouched).", "OK");
        }

        public static int EnsureAllAuthored()
        {
            int made = 0;
            foreach (var spec in Specs())
            {
                string path = Folder + "/" + spec.sceneName + "_Arena.asset";
                if (AssetDatabase.LoadAssetAtPath<ArenaLayoutDefinition>(path) != null)
                { Object.DestroyImmediate(spec); continue; }
                Directory.CreateDirectory(Folder);
                AssetDatabase.CreateAsset(spec, path);
                Debug.Log("[Ziptide] Authored arena " + path);
                made++;
            }
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        /// <summary>All five specs as fresh instances (public so the EditMode suite validates them).</summary>
        public static List<ArenaLayoutDefinition> Specs() => new List<ArenaLayoutDefinition>
        {
            BuildCisternArena(), BuildChitinwallArena(), BuildMirrorFlatsArena(),
            BuildTidalArena(), BuildVoidArena(),
        };

        // ── 1. Cistern — tight + dark; the glowing light-shaft hill; hammer-shortcut tunnels ───────
        private static ArenaLayoutDefinition BuildCisternArena()
        {
            var a = NewArena("arena_cistern", "Arena_Cistern", "Arena: The Cistern", "regular");
            a.skyHorizonColor = new Color(0.05f, 0.05f, 0.06f); a.skyTopColor = new Color(0.09f, 0.09f, 0.11f);
            a.groundTint = new Color(0.18f, 0.17f, 0.16f);
            a.fogEnabled = true; a.fogColor = new Color(0.02f, 0.02f, 0.03f); a.fogDensity = 0.03f;
            a.floorColor = new Color(0.20f, 0.19f, 0.18f); a.wallColor = new Color(0.24f, 0.22f, 0.20f);
            a.coverColor = new Color(0.32f, 0.28f, 0.22f); a.platformColor = new Color(0.85f, 0.70f, 0.30f); // the lit hill
            a.floorSize = new Vector2(44f, 44f);

            Platform(a, "LightShaftHill", V(0, 1, 0), V(9, 1, 9));
            Ramp(a, "Ramp_S", V(0, 0.5f, -7), -14f);
            Ramp(a, "Ramp_N", V(0, 0.5f, 7), 14f);
            Cover(a, -10, -8); Cover(a, 10, 8); Cover(a, -12, 6); Cover(a, 12, -6); Cover(a, -4, 14); Cover(a, 4, -14);
            BreakWall(a, "Tunnel_W", V(-9, 1.5f, 0), 90f);
            BreakWall(a, "Tunnel_E", V(9, 1.5f, 0), 90f);

            a.playerSpawn = V(0, 0.1f, -17);
            a.botSpawn = V(0, 1.2f, 17);
            Ways(a, G(-15, -15), G(15, -15), G(18, 0), G(15, 15), G(-15, 15), G(-18, 0), V(0, 1.6f, 0), G(0, -16));
            CoverPts(a, G(-10, -9.8f), G(-10, -6.2f), G(10, 9.8f), G(10, 6.2f), G(-12, 7.8f), G(12, -7.8f), G(-4, 15.8f), G(4, -15.8f));
            Pad(a, "taser_dart_gun", G(-1, -16)); Pad(a, "gravity_gun", G(1, -16)); Pad(a, "pistol", V(0, 2.1f, 0));
            Zone(a, "hill", V(0, 1.6f, 0), 4f);
            return a;
        }

        // ── 2. Chitinwall — vertical alleys; two catwalks; drop routes ──────────────────────────────
        private static ArenaLayoutDefinition BuildChitinwallArena()
        {
            var a = NewArena("arena_chitinwall", "Arena_Chitinwall", "Arena: Chitinwall", "regular");
            a.skyHorizonColor = new Color(0.45f, 0.30f, 0.12f); a.skyTopColor = new Color(0.18f, 0.10f, 0.22f);
            a.groundTint = new Color(0.30f, 0.24f, 0.16f);
            a.planetVisible = true; a.planetAngularSize = 16f;
            a.planetBaseColor = new Color(0.45f, 0.40f, 0.55f); a.planetAccentColor = new Color(0.20f, 0.55f, 0.60f);
            a.floorColor = new Color(0.30f, 0.24f, 0.16f); a.wallColor = new Color(0.36f, 0.28f, 0.18f);
            a.coverColor = new Color(0.42f, 0.32f, 0.20f); a.platformColor = new Color(0.36f, 0.30f, 0.22f);
            a.floorSize = new Vector2(40f, 56f);

            Platform(a, "Catwalk_W", V(-13, 2.5f, 0), V(6, 0.6f, 30));
            Platform(a, "Catwalk_E", V(13, 2.5f, 0), V(6, 0.6f, 30));
            Ramp(a, "Ramp_W", V(-13, 1.2f, -18), 18f);
            Ramp(a, "Ramp_E", V(13, 1.2f, 18), -18f);
            Cover(a, -5, -14); Cover(a, 5, -10); Cover(a, -5, -4); Cover(a, 5, 0); Cover(a, -5, 6); Cover(a, 5, 12); Cover(a, 0, 18); Cover(a, 0, -20);
            BreakWall(a, "AlleyWall_A", V(0, 1.5f, -7), 0f);
            BreakWall(a, "AlleyWall_B", V(0, 1.5f, 9), 0f);

            a.playerSpawn = V(0, 0.1f, -24);
            a.botSpawn = V(0, 1.2f, 24);
            Ways(a, V(-13, 3.2f, -12), V(-13, 3.2f, 12), V(13, 3.2f, 12), V(13, 3.2f, -12), G(0, -18), G(0, 16), G(-8, 0), G(8, 0));
            CoverPts(a, G(-5, -15.8f), G(-5, -12.2f), G(5, -11.8f), G(5, -8.2f), G(-5, -5.8f), G(5, -1.8f), G(-5, 4.2f), G(5, 10.2f), G(0, 16.2f), G(0, -21.8f));
            Pad(a, "taser_dart_gun", G(-1, -23)); Pad(a, "gravity_gun", G(1, -23));
            Pad(a, "pistol", V(-13, 3.4f, 0)); Pad(a, "pistol", V(13, 3.4f, 0));
            Zone(a, "midalley", V(0, 0, 1), 3.5f);
            return a;
        }

        // ── 3. Mirror Flats — long glare lanes; low cover; marksman country ─────────────────────────
        private static ArenaLayoutDefinition BuildMirrorFlatsArena()
        {
            var a = NewArena("arena_mirrorflats", "Arena_MirrorFlats", "Arena: Mirror Flats", "veteran");
            a.skyHorizonColor = new Color(0.93f, 0.91f, 0.86f); a.skyTopColor = new Color(0.55f, 0.66f, 0.78f);
            a.groundTint = new Color(0.80f, 0.80f, 0.78f);
            a.planetVisible = true; a.planetAngularSize = 14f;
            a.planetBaseColor = new Color(0.75f, 0.78f, 0.85f); a.planetAccentColor = new Color(0.60f, 0.66f, 0.78f);
            a.floorColor = new Color(0.78f, 0.78f, 0.76f); a.wallColor = new Color(0.70f, 0.72f, 0.74f);
            a.coverColor = new Color(0.62f, 0.66f, 0.70f); a.platformColor = new Color(0.72f, 0.74f, 0.76f);
            a.floorSize = new Vector2(60f, 36f);

            Platform(a, "Perch_W", V(-24, 1, 0), V(8, 1, 10));
            Platform(a, "Perch_E", V(24, 1, 0), V(8, 1, 10));
            Ramp(a, "Ramp_W", V(-19, 0.5f, 0), 0f, rotY: 90f);
            Ramp(a, "Ramp_E", V(19, 0.5f, 0), 0f, rotY: -90f);
            for (int i = 0; i < 6; i++)
                a.covers.Add(new ArenaBlockDef { blockName = "LowCover_" + i, position = new Vector3(-15f + i * 6f, 0.55f, (i % 2 == 0) ? -6f : 6f), size = new Vector3(2.4f, 1.1f, 1.2f) });
            BreakWall(a, "MidScreen", V(0, 1.5f, 0), 90f);

            a.playerSpawn = V(-26, 0.1f, -13);
            a.botSpawn = V(26, 1.2f, 13);
            Ways(a, V(-24, 1.6f, 0), V(24, 1.6f, 0), G(-12, -12), G(12, -12), G(12, 12), G(-12, 12), G(0, -8), G(0, 8));
            CoverPts(a, G(-15, -7.4f), G(-9, 7.4f), G(-3, -7.4f), G(3, 7.4f), G(9, -7.4f), G(15, 7.4f), G(1.2f, 0), G(-1.2f, 0));
            Pad(a, "taser_dart_gun", G(-25, -12)); Pad(a, "gravity_gun", G(-27, -12));
            Pad(a, "pistol", G(0, -14)); Pad(a, "pistol", G(0, 14));
            Zone(a, "center", V(0, 0, 0), 4f);
            return a;
        }

        // ── 4. Tidal — four islands + bridges; the flood BREATHES (live hazard mutator) ─────────────
        private static ArenaLayoutDefinition BuildTidalArena()
        {
            var a = NewArena("arena_tidal", "Arena_Tidal", "Arena: Tidal Array", "regular");
            a.skyHorizonColor = new Color(0.55f, 0.58f, 0.50f); a.skyTopColor = new Color(0.20f, 0.26f, 0.34f);
            a.groundTint = new Color(0.30f, 0.36f, 0.36f);
            a.planetVisible = true; a.planetAngularSize = 18f;
            a.planetBaseColor = new Color(0.60f, 0.55f, 0.45f); a.planetAccentColor = new Color(0.80f, 0.75f, 0.60f);
            a.fogEnabled = true; a.fogColor = new Color(0.30f, 0.34f, 0.36f); a.fogDensity = 0.012f;
            a.floorColor = new Color(0.24f, 0.32f, 0.32f); a.wallColor = new Color(0.32f, 0.36f, 0.38f);
            a.coverColor = new Color(0.38f, 0.40f, 0.36f); a.platformColor = new Color(0.42f, 0.44f, 0.42f);
            a.floorSize = new Vector2(48f, 48f);

            Platform(a, "Island_SW", V(-12, 0.6f, -12), V(12, 1.2f, 12));
            Platform(a, "Island_SE", V(12, 0.6f, -12), V(12, 1.2f, 12));
            Platform(a, "Island_NW", V(-12, 0.6f, 12), V(12, 1.2f, 12));
            Platform(a, "Island_NE", V(12, 0.6f, 12), V(12, 1.2f, 12));
            Platform(a, "Bridge_S", V(0, 0.6f, -12), V(8, 1.2f, 3));
            Platform(a, "Bridge_N", V(0, 0.6f, 12), V(8, 1.2f, 3));
            Platform(a, "Bridge_W", V(-12, 0.6f, 0), V(3, 1.2f, 8));
            Platform(a, "Bridge_E", V(12, 0.6f, 0), V(3, 1.2f, 8));
            Cover(a, -12, -12, y: 2.1f); Cover(a, 12, 12, y: 2.1f); Cover(a, 12, -12, y: 2.1f); Cover(a, -12, 12, y: 2.1f);

            a.hazards.Add(new HazardZoneDef { id = "Tide", kind = HazardKind.Flood, center = Vector3.zero, size = new Vector3(10f, 3f, 10f), strength = 1.2f });

            a.playerSpawn = V(-12, 1.3f, -12);
            a.botSpawn = V(12, 2.4f, 12);
            Ways(a, V(-12, 1.8f, -12), V(12, 1.8f, -12), V(12, 1.8f, 12), V(-12, 1.8f, 12), V(0, 1.8f, -12), V(0, 1.8f, 12), V(-12, 1.8f, 0), V(12, 1.8f, 0));
            CoverPts(a, V(-13.8f, 1.4f, -12), V(-10.2f, 1.4f, -12), V(13.8f, 1.4f, 12), V(10.2f, 1.4f, 12), V(13.8f, 1.4f, -12), V(-13.8f, 1.4f, 12));
            Pad(a, "taser_dart_gun", V(-13, 1.4f, -13)); Pad(a, "gravity_gun", V(-11, 1.4f, -13)); Pad(a, "pistol", V(0, 1.4f, 0));
            Zone(a, "center_ford", V(0, 0.6f, 0), 3.5f);
            return a;
        }

        // ── 5. Void — bridge cross under the Shell; gravity-shove country; nightmare bot ────────────
        private static ArenaLayoutDefinition BuildVoidArena()
        {
            var a = NewArena("arena_void", "Arena_Void", "Arena: The Shell Gate", "nightmare");
            a.skyHorizonColor = new Color(0.01f, 0.01f, 0.03f); a.skyTopColor = new Color(0.03f, 0.03f, 0.06f);
            a.groundTint = new Color(0.06f, 0.07f, 0.10f);
            a.planetVisible = true; a.planetAngularSize = 30f; // the Shell overhead
            a.planetBaseColor = new Color(0.10f, 0.20f, 0.26f); a.planetAccentColor = new Color(0.20f, 0.75f, 0.85f);
            a.floorColor = new Color(0.05f, 0.06f, 0.09f);      // the void reads as depth
            a.wallColor = new Color(0.14f, 0.16f, 0.22f);
            a.coverColor = new Color(0.22f, 0.25f, 0.32f); a.platformColor = new Color(0.24f, 0.26f, 0.32f);
            a.floorSize = new Vector2(52f, 52f);

            Platform(a, "Walk_NS", V(0, 1.5f, 0), V(4, 0.6f, 44));
            Platform(a, "Walk_EW", V(0, 1.5f, 0), V(44, 0.6f, 4));
            Platform(a, "Hub", V(0, 1.6f, 0), V(8, 0.8f, 8));
            Platform(a, "Ring_N", V(0, 1.5f, 20), V(20, 0.6f, 4));
            Platform(a, "Ring_S", V(0, 1.5f, -20), V(20, 0.6f, 4));
            Ramp(a, "Up_S", V(0, 0.7f, -24), 12f);
            Ramp(a, "Up_N", V(0, 0.7f, 24), -12f);
            Cover(a, -2.4f, 2.4f, y: 2.8f); Cover(a, 2.4f, -2.4f, y: 2.8f); Cover(a, 0, 19, y: 2.7f); Cover(a, 0, -19, y: 2.7f);
            BreakWall(a, "HubScreen_W", V(-4.2f, 3f, 0), 0f);
            BreakWall(a, "HubScreen_E", V(4.2f, 3f, 0), 0f);

            a.playerSpawn = V(0, 0.2f, -25);
            a.botSpawn = V(0, 3.1f, 20);
            Ways(a, V(0, 2.1f, -18), V(0, 2.2f, 0), V(0, 2.1f, 18), V(-18, 2.1f, 0), V(18, 2.1f, 0), V(8, 2.1f, 20), V(-8, 2.1f, -20), V(0, 0.3f, -24));
            CoverPts(a, V(-2.4f, 2.2f, 4.2f), V(2.4f, 2.2f, -4.2f), V(0, 2.1f, 17.2f), V(0, 2.1f, -17.2f), V(-6, 2.1f, 0), V(6, 2.1f, 0));
            Pad(a, "taser_dart_gun", V(-1, 0.3f, -24)); Pad(a, "gravity_gun", V(1, 0.3f, -24)); Pad(a, "pistol", V(0, 2.4f, 0));
            Zone(a, "hub", V(0, 2, 0), 3.5f);
            return a;
        }

        // ── Spec helpers (compile-time-safe — no tuple boxing) ─────────────────────────────────────
        private static Vector3 V(float x, float y, float z) => new Vector3(x, y, z);
        /// <summary>Ground point: (x, z) at standing height.</summary>
        private static Vector3 G(float x, float z) => new Vector3(x, 0.1f, z);

        private static ArenaLayoutDefinition NewArena(string id, string scene, string display, string bot)
        {
            var a = ScriptableObject.CreateInstance<ArenaLayoutDefinition>();
            a.arenaId = id; a.sceneName = scene; a.displayName = display; a.botDifficulty = bot;
            return a;
        }

        private static void Platform(ArenaLayoutDefinition a, string name, Vector3 pos, Vector3 size)
            => a.platforms.Add(new ArenaBlockDef { blockName = name, position = pos, size = size });

        private static void Ramp(ArenaLayoutDefinition a, string name, Vector3 pos, float tiltX, float rotY = 0f)
            => a.ramps.Add(new ArenaRampDef { rampName = name, position = pos, tiltX = tiltX, rotY = rotY });

        private static void Cover(ArenaLayoutDefinition a, float x, float z, float y = 0.9f)
            => a.covers.Add(new ArenaBlockDef { blockName = "Cover_" + (a.covers.Count + 1), position = new Vector3(x, y, z), size = new Vector3(2f, 1.8f, 2f) });

        private static void BreakWall(ArenaLayoutDefinition a, string name, Vector3 pos, float rotY)
            => a.breakableWalls.Add(new ArenaWallDef { wallName = name, position = pos, rotY = rotY });

        private static void Ways(ArenaLayoutDefinition a, params Vector3[] pts) => a.waypoints.AddRange(pts);

        private static void CoverPts(ArenaLayoutDefinition a, params Vector3[] pts) => a.coverPoints.AddRange(pts);

        private static void Pad(ArenaLayoutDefinition a, string itemId, Vector3 pos)
            => a.weaponPads.Add(new WeaponPadDef { itemId = itemId, position = pos });

        private static void Zone(ArenaLayoutDefinition a, string id, Vector3 pos, float radius)
            => a.objectiveZones.Add(new ObjectiveZoneDef { zoneId = id, position = pos, radius = radius });
    }
}
#endif

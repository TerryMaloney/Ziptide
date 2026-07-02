#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE ARENA FACTORY (M7a A2 — design docs/design/PVP_ARENA_AAA.md §A2). Generalizes the proven
    /// ScenePatcherPvP shell: any <see cref="ArenaLayoutDefinition"/> with a sceneName becomes a full
    /// PvP arena — data-driven geometry, per-arena sky, bot nav (Way_*/Cover_P*), difficulty-tagged bot,
    /// weapon pads, breakable walls, hazard mutators, match director/HUD/scanner/hammer, world pack +
    /// exit door, Build Settings — regenerated every build like the story worlds. The ORIGINAL PvP_Arena01
    /// keeps its hand-tuned patcher (same as ToxicCity); this builds arena #2 onward from assets.
    /// </summary>
    public static class ScenePatcherArena
    {
        private const string SceneFolder = "Assets/Ziptide/Scenes/Arenas";
        private const string PackFolder = "Assets/Ziptide/Content/Arenas/Packs";

        [MenuItem("Ziptide/Worlds/Build All Arenas")]
        public static void BuildAllFromMenu()
        {
            int built = 0;
            foreach (var def in AllArenas())
            {
                string scenePath = ScenePathFor(def);
                Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
                var scene = File.Exists(scenePath)
                    ? EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single)
                    : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                Populate(def);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, scenePath);
                EnsureSceneEnabled(scenePath);
                built++;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Arena Factory", built + " arena(s) built/updated.", "OK");
        }

        // ── Build-pipeline hooks (BuildAndroid calls both; idempotent, batchmode-safe) ─────────────
        public static void EnsureAllInBuildSettings()
        {
            foreach (var def in AllArenas())
            {
                string scenePath = ScenePathFor(def);
                if (!File.Exists(scenePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(scene, scenePath);
                    Debug.Log("[Ziptide] Arena factory created empty scene " + scenePath + " (populated during the build).");
                }
                EnsureSceneEnabled(scenePath);
            }
        }

        public static void PatchActiveSceneIfArena()
        {
            string active = EditorSceneManager.GetActiveScene().name;
            foreach (var def in AllArenas())
                if (def.sceneName == active) { Populate(def); return; }
        }

        // ── The shell (mirrors PopulateActivePvP, driven purely by the layout asset) ───────────────
        public static void Populate(ArenaLayoutDefinition def)
        {
            var issues = def.Validate();
            if (issues.Count > 0)
            {
                Debug.LogError("[Ziptide] Arena '" + def.name + "' SKIPPED: " + string.Join(" | ", issues));
                return;
            }

            var root = ResetRoot("__ARENA_" + def.arenaId.ToUpperInvariant() + "_ROOT");

            BuildGeometry(root, def);
            BuildBotNav(root, def);

            EnsureLighting();
            EnsureEventSystem();

            // Per-arena sky through the same pipeline the worlds use.
            var theme = ThemeAuthor.EnsureThemeAsset(def.sceneName, def.skyHorizonColor, def.skyTopColor,
                def.groundTint, def.planetVisible, def.planetBaseColor, def.planetAccentColor, def.planetAngularSize);
            var worldProfile = ThemeAuthor.EnsureWorldProfileAsset(def.sceneName, 0f, theme);
            EnsureWorldRuntime(worldProfile);

            EnsureSpawn("player", def.playerSpawn);

            var pgo = PatcherUtil.EnsureRootObject("PvpPlayer", def.playerSpawn);
            PatcherUtil.EnsureComponent<PvpPlayer>(pgo);
            var dgo = PatcherUtil.EnsureRootObject("PvpMatchDirector", Vector3.zero);
            PatcherUtil.EnsureComponent<PvpMatchDirector>(dgo);
            var hgo = PatcherUtil.EnsureRootObject("PvpHud", Vector3.zero);
            PatcherUtil.EnsureComponent<PvpHud>(hgo);

            BuildBot(root, def);
            SpawnWeaponPads(root, def);
            BuildBreakableWalls(root, def);
            SpawnHammer(root, def.playerSpawn);
            BuildHazards(root, def);

            var scanGo = PatcherUtil.EnsureRootObject("WristScanner", Vector3.zero);
            PatcherUtil.EnsureComponent<WristScanner>(scanGo);

            var pack = EnsureWorldPack(def);
            EnsureTravelStation(def);
            Debug.Log("[Ziptide] Arena built: " + def.sceneName + " (bot=" + def.botDifficulty +
                      ", pads=" + def.weaponPads.Count + ", hazards=" + def.hazards.Count + ")");
        }

        // ── Geometry from data ─────────────────────────────────────────────────────────────────────
        private static void BuildGeometry(Transform root, ArenaLayoutDefinition def)
        {
            float fx = def.floorSize.x, fz = def.floorSize.y, h = def.wallHeight;
            Cube(root, "Floor", new Vector3(0f, -0.5f, 0f), new Vector3(fx, 1f, fz), def.floorColor);
            Cube(root, "Wall_N", new Vector3(0f, h / 2f - 0.5f, fz / 2f), new Vector3(fx, h, 1f), def.wallColor);
            Cube(root, "Wall_S", new Vector3(0f, h / 2f - 0.5f, -fz / 2f), new Vector3(fx, h, 1f), def.wallColor);
            Cube(root, "Wall_E", new Vector3(fx / 2f, h / 2f - 0.5f, 0f), new Vector3(1f, h, fz), def.wallColor);
            Cube(root, "Wall_W", new Vector3(-fx / 2f, h / 2f - 0.5f, 0f), new Vector3(1f, h, fz), def.wallColor);

            foreach (var p in def.platforms)
                if (p != null) Cube(root, p.blockName, p.position, p.size, def.platformColor);
            foreach (var r in def.ramps)
                if (r != null)
                {
                    var go = Cube(root, r.rampName, r.position, r.size, def.platformColor);
                    go.transform.localRotation = Quaternion.Euler(r.tiltX, r.rotY, 0f);
                }
            foreach (var c in def.covers)
                if (c != null) Cube(root, c.blockName, c.position, c.size, def.coverColor);
        }

        private static void BuildBotNav(Transform root, ArenaLayoutDefinition def)
        {
            var old = GameObject.Find("__PVP_BOTNAV");
            if (old != null) Object.DestroyImmediate(old);
            var nav = new GameObject("__PVP_BOTNAV");
            nav.transform.SetParent(root, false);
            for (int i = 0; i < def.waypoints.Count; i++)
                NavPoint(nav.transform, "Way_" + (i + 1), def.waypoints[i]);
            for (int i = 0; i < def.coverPoints.Count; i++)
                NavPoint(nav.transform, "Cover_P" + (i + 1), def.coverPoints[i]);
        }

        private static void BuildBot(Transform root, ArenaLayoutDefinition def)
        {
            var bot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bot.name = "PvpBot";
            bot.transform.SetParent(root, false);
            bot.transform.position = def.botSpawn;
            var pb = bot.AddComponent<PvpBot>();
            pb.difficulty = def.botDifficulty;
        }

        /// <summary>A2: spawn each pad's weapon once (visible base slab marks the spot). A4 upgrades
        /// pads into timed respawners — the data (respawnSeconds) is already here.</summary>
        private static void SpawnWeaponPads(Transform root, ArenaLayoutDefinition def)
        {
            foreach (var pad in def.weaponPads)
            {
                if (pad == null || string.IsNullOrEmpty(pad.itemId)) continue;
                Cube(root, "Pad_" + pad.itemId, pad.position + new Vector3(0f, 0.05f, 0f),
                    new Vector3(0.8f, 0.1f, 0.8f), new Color(0.2f, 0.6f, 0.7f));
                var item = ItemFactory.Create(pad.itemId, pad.position + new Vector3(0f, 1.0f, 0f));
                if (item != null)
                {
                    item.transform.SetParent(root, true);
                    if (pad.itemId == "gravity_gun" && item.GetComponent<PvpComfortHop>() == null)
                        item.AddComponent<PvpComfortHop>();
                }
            }
        }

        private static void BuildBreakableWalls(Transform root, ArenaLayoutDefinition def)
        {
            foreach (var w in def.breakableWalls)
            {
                if (w == null) continue;
                var go = new GameObject(w.wallName);
                go.transform.SetParent(root, false);
                go.transform.position = w.position;
                go.transform.rotation = Quaternion.Euler(0f, w.rotY, 0f);
                var bw = go.AddComponent<BreakableWall>();
                bw.wallSize = w.size;
            }
        }

        private static void SpawnHammer(Transform root, Vector3 near)
        {
            var go = new GameObject("PvpHammer");
            go.transform.SetParent(root, false);
            go.transform.position = near + new Vector3(0f, 1.0f, 1.2f);
            // Collider + Rigidbody BEFORE the tool adds XRGrabInteractable (the proven ordering fix).
            var box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(0.18f, 0.55f, 0.18f);
            box.center = new Vector3(0f, 0.2f, 0f);
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = true;
            go.AddComponent<HammerTool>();
        }

        private static void BuildHazards(Transform root, ArenaLayoutDefinition def)
        {
            if (def.hazards == null || def.hazards.Count == 0) return;
            var hazardRoot = new GameObject("Hazards");
            hazardRoot.transform.SetParent(root, false);
            foreach (var h in def.hazards)
            {
                if (h == null) continue;
                var go = new GameObject("Hazard_" + h.id);
                go.transform.SetParent(hazardRoot.transform, false);
                go.transform.position = new Vector3(h.center.x, 0f, h.center.z);
                go.AddComponent<HazardZoneRuntime>().Init(h);
            }
        }

        // ── Scaffolding (the standard shell, same as every patcher) ────────────────────────────────
        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 10f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.intensity = 1.05f;
            go.transform.rotation = Quaternion.Euler(50f, -25f, 0f);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var go = PatcherUtil.EnsureRootObject("EventSystem", Vector3.zero);
            PatcherUtil.EnsureComponent<UnityEngine.EventSystems.EventSystem>(go);
            PatcherUtil.EnsureComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(go);
        }

        private static void EnsureWorldRuntime(WorldProfile profile)
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);
            if (profile != null)
            {
                var so = new SerializedObject(wr);
                PatcherUtil.SetObjectRef(so, "worldProfile", profile);
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureSpawn(string markerId, Vector3 pos)
        {
            string objName = markerId == "player" ? ZiptideConstants.GoSpawnPlayer : "__SPAWN_" + markerId;
            var go = PatcherUtil.EnsureRootObject(objName, pos);
            var marker = PatcherUtil.EnsureComponent<SpawnMarkerRuntime>(go);
            var so = new SerializedObject(marker);
            PatcherUtil.SetString(so, "markerId", markerId);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static WorldPackDefinition EnsureWorldPack(ArenaLayoutDefinition def)
        {
            string path = PackFolder + "/" + def.sceneName + "_WorldPack.asset";
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (pack == null)
            {
                Directory.CreateDirectory(PackFolder);
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, path);
            }
            pack.packId = def.arenaId;
            pack.displayName = string.IsNullOrEmpty(def.displayName) ? def.arenaId : def.displayName;
            pack.sceneName = def.sceneName;
            var player = pack.spawnMarkers.Find(m => m != null && m.markerId == "player");
            if (player == null)
                pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = def.playerSpawn });
            else
                player.localPosition = def.playerSpawn;
            EditorUtility.SetDirty(pack);
            return pack;
        }

        private static void EnsureTravelStation(ArenaLayoutDefinition def)
        {
            string path = PackFolder + "/" + def.sceneName + "Exit_WorldPack.asset";
            var exitPack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (exitPack == null)
            {
                Directory.CreateDirectory(PackFolder);
                exitPack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(exitPack, path);
            }
            exitPack.packId = def.arenaId + "_exit";
            exitPack.displayName = "Leave";
            exitPack.sceneName = FirstOtherBuildSceneName(def.sceneName);
            EditorUtility.SetDirty(exitPack);

            Vector3 pos = def.playerSpawn + new Vector3(0f, 0f, -3f);
            var go = PatcherUtil.EnsureRootObject(ZiptideConstants.GoWorldTravelStation, pos);
            var station = PatcherUtil.EnsureComponent<WorldTravelStation>(go);
            var so = new SerializedObject(station);
            var listProp = so.FindProperty("destinationPacks");
            if (listProp != null)
            {
                listProp.arraySize = 1;
                listProp.GetArrayElementAtIndex(0).objectReferenceValue = exitPack;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // ── Lookup / helpers ───────────────────────────────────────────────────────────────────────
        private static List<ArenaLayoutDefinition> AllArenas()
        {
            var result = new List<ArenaLayoutDefinition>();
            foreach (var guid in AssetDatabase.FindAssets("t:ArenaLayoutDefinition"))
            {
                var def = AssetDatabase.LoadAssetAtPath<ArenaLayoutDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (def != null && !string.IsNullOrEmpty(def.sceneName)) result.Add(def);
            }
            return result;
        }

        private static string ScenePathFor(ArenaLayoutDefinition def) => SceneFolder + "/" + def.sceneName + ".unity";

        private static void EnsureSceneEnabled(string scenePath)
        {
            string normalized = scenePath.Replace('\\', '/');
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(normalized, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(normalized, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        private static string FirstOtherBuildSceneName(string ownScene)
        {
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (!s.enabled || string.IsNullOrEmpty(s.path)) continue;
                string n = Path.GetFileNameWithoutExtension(s.path);
                if (n == ZiptideConstants.SceneBoot || n == ownScene) continue;
                return n;
            }
            return ZiptideConstants.SceneTestRoom;
        }

        private static Transform ResetRoot(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) Object.DestroyImmediate(existing);
            return new GameObject(name).transform;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        private static void NavPoint(Transform parent, string name, Vector3 pos)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
        }
    }
}
#endif

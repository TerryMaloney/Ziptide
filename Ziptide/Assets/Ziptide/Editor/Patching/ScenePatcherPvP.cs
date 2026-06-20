#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Builds the 1v1 PvP arena (Phase 2): a compact, multi-level anti-camp space, playable SOLO vs an
    /// AI bot now. Self-generates the scene + world pack so it ships in the cloud build and shows in Dev
    /// Warp. Wires the match director + local player + bot + HUD + the taser/gravity guns. Photon
    /// 2-headset netcode is a later shared pass that swaps the bot for a remote avatar.
    /// </summary>
    public static class ScenePatcherPvP
    {
        public const string SceneName = ZiptideConstants.ScenePvPArena;
        private const string ScenePath = ZiptideConstants.PathPvPArenaScene;
        private const string WorldPackPath = ZiptideConstants.PathPvPArenaWorldPack;
        private const string ExitPackPath = "Assets/Ziptide/Content/Worlds/Packs/PvPExit_WorldPack.asset";
        private const string DefaultWorldProfilePath = ZiptideConstants.PathDefaultWorldProfileWorlds;
        private const string RootName = "__PVP_ARENA_ROOT";

        private static readonly Color FloorColor = new Color(0.26f, 0.27f, 0.30f);
        private static readonly Color WallColor = new Color(0.32f, 0.33f, 0.37f);
        private static readonly Color CoverColor = new Color(0.45f, 0.30f, 0.20f);
        private static readonly Color PlatformColor = new Color(0.30f, 0.34f, 0.38f);

        [MenuItem("Ziptide/Worlds/Build PvP Arena")]
        public static void BuildFromMenu()
        {
            var scene = OpenOrCreateScene();
            PopulateActivePvP();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("PvP Arena",
                "Built/updated " + SceneName + ". Warp to it via Ziptide > Dev, or it ships next build.", "OK");
        }

        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
                Debug.Log("[Ziptide] Created empty PvP arena scene at " + normalized + " (will be populated by the build).");
            }
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(normalized, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(normalized, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        public static void PatchActiveScene()
        {
            if (EditorSceneManager.GetActiveScene().name != SceneName) return;
            PopulateActivePvP();
        }

        public static void PopulateActivePvP()
        {
            var root = ResetRoot(RootName);

            BuildArena(root);

            EnsureLighting();
            EnsureEventSystem();
            EnsureWorldRuntime();

            Vector3 playerSpawn = new Vector3(0f, 0.1f, -12f);
            Vector3 botSpawn = new Vector3(0f, 0.6f, 12f);
            EnsureSpawn("player", playerSpawn);

            // Local player + opponent + match brain + HUD (root singletons; the bot rides the arena root).
            var pgo = PatcherUtil.EnsureRootObject("PvpPlayer", playerSpawn);
            PatcherUtil.EnsureComponent<PvpPlayer>(pgo);
            PatcherUtil.EnsureRootObject("PvpMatchDirector", Vector3.zero);
            PatcherUtil.EnsureComponent<PvpMatchDirector>(GameObject.Find("PvpMatchDirector"));
            var hgo = PatcherUtil.EnsureRootObject("PvpHud", Vector3.zero);
            PatcherUtil.EnsureComponent<PvpHud>(hgo);

            BuildBot(root, botSpawn);
            SpawnGuns(root, playerSpawn);
            BuildBreakableWalls(root);
            SpawnHammer(root, playerSpawn);

            // Wrist scanner (scene-scoped; builds its diegetic forearm device on the rig hand at runtime).
            var scanGo = PatcherUtil.EnsureRootObject("WristScanner", Vector3.zero);
            PatcherUtil.EnsureComponent<WristScanner>(scanGo);

            EnsureWorldPack();
            EnsureTravelStation(root);
        }

        private static void BuildBreakableWalls(Transform root)
        {
            CreateBreakableWall(root, "BreakWall_W", new Vector3(-6f, 1.5f, 2f), Quaternion.identity);
            CreateBreakableWall(root, "BreakWall_E", new Vector3(6f, 1.5f, -2f), Quaternion.identity);
            CreateBreakableWall(root, "BreakWall_C", new Vector3(0f, 1.5f, 6f), Quaternion.Euler(0f, 90f, 0f));
        }

        private static void CreateBreakableWall(Transform root, string name, Vector3 pos, Quaternion rot)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            go.transform.position = pos;
            go.transform.rotation = rot;
            var bw = go.AddComponent<BreakableWall>();
            bw.wallSize = new Vector3(4f, 3f, 0.3f);
        }

        private static void SpawnHammer(Transform root, Vector3 near)
        {
            var go = new GameObject("PvpHammer");
            go.transform.SetParent(root, false);
            go.transform.position = near + new Vector3(0f, 1.0f, 1.2f);
            go.AddComponent<HammerTool>(); // RequireComponent adds XRGrabInteractable; HammerTool self-builds the visual
        }

        // ── Arena geometry ───────────────────────────────────────────────────
        private static void BuildArena(Transform root)
        {
            // Floor (solid, walkable).
            Cube(root, "Floor", new Vector3(0f, -0.5f, 0f), new Vector3(34f, 1f, 34f), FloorColor, true);

            // Perimeter walls.
            Cube(root, "Wall_N", new Vector3(0f, 2f, 17f), new Vector3(34f, 5f, 1f), WallColor, true);
            Cube(root, "Wall_S", new Vector3(0f, 2f, -17f), new Vector3(34f, 5f, 1f), WallColor, true);
            Cube(root, "Wall_E", new Vector3(17f, 2f, 0f), new Vector3(1f, 5f, 34f), WallColor, true);
            Cube(root, "Wall_W", new Vector3(-17f, 2f, 0f), new Vector3(1f, 5f, 34f), WallColor, true);

            // Center raised platform + two ramps (verticality / anti-camp).
            Cube(root, "Platform", new Vector3(0f, 1f, 0f), new Vector3(10f, 1f, 10f), PlatformColor, true);
            Ramp(root, "Ramp_S", new Vector3(0f, 0.5f, -7.5f), new Vector3(4f, 0.4f, 6f), -15f);
            Ramp(root, "Ramp_N", new Vector3(0f, 0.5f, 7.5f), new Vector3(4f, 0.4f, 6f), 15f);

            // Scattered cover.
            Cube(root, "Cover_1", new Vector3(-8f, 0.9f, -4f), new Vector3(2f, 1.8f, 2f), CoverColor, true);
            Cube(root, "Cover_2", new Vector3(8f, 0.9f, 4f), new Vector3(2f, 1.8f, 2f), CoverColor, true);
            Cube(root, "Cover_3", new Vector3(-7f, 0.9f, 8f), new Vector3(2f, 1.8f, 2f), CoverColor, true);
            Cube(root, "Cover_4", new Vector3(7f, 0.9f, -8f), new Vector3(2f, 1.8f, 2f), CoverColor, true);
        }

        private static void BuildBot(Transform root, Vector3 pos)
        {
            var bot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bot.name = "PvpBot";
            bot.transform.SetParent(root, false);
            bot.transform.position = pos;
            bot.AddComponent<PvpBot>();
        }

        private static void SpawnGuns(Transform root, Vector3 near)
        {
            var taser = ItemFactory.Create("taser_dart_gun", near + new Vector3(-0.6f, 1.0f, 1.2f));
            if (taser != null) taser.transform.SetParent(root, true);
            var grav = ItemFactory.Create("gravity_gun", near + new Vector3(0.6f, 1.0f, 1.2f));
            if (grav != null)
            {
                grav.transform.SetParent(root, true);
                // Comfort-first PvP gravity gun: firing also gives the shooter a short self-hop.
                if (grav.GetComponent<PvpComfortHop>() == null) grav.AddComponent<PvpComfortHop>();
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private static GameObject Cube(Transform parent, string name, Vector3 pos, Vector3 scale, Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            var col = go.GetComponent<Collider>();
            if (col != null) col.enabled = collider;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        private static void Ramp(Transform parent, string name, Vector3 pos, Vector3 scale, float tiltX)
        {
            var go = Cube(parent, name, pos, scale, PlatformColor, true);
            go.transform.localRotation = Quaternion.Euler(tiltX, 0f, 0f);
        }

        private static Transform ResetRoot(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) Object.DestroyImmediate(existing);
            return new GameObject(name).transform;
        }

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

        private static void EnsureWorldRuntime()
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);
            var profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(DefaultWorldProfilePath);
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

        private static void EnsureWorldPack()
        {
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(WorldPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WorldPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, WorldPackPath);
            }
            pack.packId = "pvp_arena01";
            pack.displayName = "PvP Arena";
            pack.sceneName = SceneName;
            pack.spawnMarkers.Clear();
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = new Vector3(0f, 0.1f, -12f) });
            EditorUtility.SetDirty(pack);
        }

        private static void EnsureTravelStation(Transform root)
        {
            var exitPack = EnsureExitPackAsset();
            if (exitPack == null) return;
            var go = PatcherUtil.EnsureRootObject(ZiptideConstants.GoWorldTravelStation, new Vector3(0f, 0.1f, -15.5f));
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

        private static WorldPackDefinition EnsureExitPackAsset()
        {
            string dest = FirstOtherBuildSceneName();
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(ExitPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ExitPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, ExitPackPath);
            }
            pack.packId = "pvp_exit";
            pack.displayName = "Leave";
            pack.sceneName = dest;
            EditorUtility.SetDirty(pack);
            return pack;
        }

        private static string FirstOtherBuildSceneName()
        {
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (!s.enabled || string.IsNullOrEmpty(s.path)) continue;
                string n = Path.GetFileNameWithoutExtension(s.path);
                if (n == "_Boot" || n == SceneName) continue;
                return n;
            }
            return ZiptideConstants.SceneTestRoom;
        }

        private static Scene OpenOrCreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return EditorSceneManager.GetActiveScene();
            if (File.Exists(ScenePath))
                return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            return scene;
        }
    }
}
#endif

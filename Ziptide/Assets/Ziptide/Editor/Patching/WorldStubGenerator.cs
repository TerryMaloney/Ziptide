#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// THE WORLD FACTORY (backlog E2). Turns any <see cref="CityLayoutDefinition"/> with a non-empty
    /// <c>sceneName</c> into a shippable world — no per-world patcher code, no hand-edited YAML:
    /// scene at Scenes/Generated/&lt;sceneName&gt;.unity · populated via <see cref="CityBuilder"/> ·
    /// WorldPackDefinition (+ exit pack) · spawn · JobDirector/kiosk/board · Build Settings entry.
    /// The build pipeline (BuildAndroid) re-populates generated scenes on every build, so a world is
    /// FULLY DEFINED BY ITS DATA: edit the layout asset → the world changes; author a new layout with a
    /// sceneName → a new world ships. ToxicCity keeps its own hand-tuned patcher (its layout's
    /// sceneName stays empty); this generalizes that exact shell for world #2 through #80.
    /// </summary>
    public static class WorldStubGenerator
    {
        private const string GeneratedSceneFolder = "Assets/Ziptide/Scenes/Generated";
        private const string PackFolder = "Assets/Ziptide/Content/Worlds/Packs";

        // ── Menus (Terry runs these; the build pipeline also regenerates automatically) ──────────

        [MenuItem("Ziptide/Worlds/Generate World From Selected Layout")]
        public static void GenerateFromSelection()
        {
            var layouts = SelectedLayouts();
            if (layouts.Count == 0)
            {
                EditorUtility.DisplayDialog("World Stub Generator",
                    "Select one or more CityLayoutDefinition assets (with a sceneName set) in the Project window first.", "OK");
                return;
            }
            int built = GenerateAll(layouts);
            EditorUtility.DisplayDialog("World Stub Generator",
                built + " world(s) generated/updated under " + GeneratedSceneFolder + ".\n\nThey ship in the next build; warp via Ziptide > Dev.", "OK");
        }

        [MenuItem("Ziptide/Worlds/Generate All Layout Worlds")]
        public static void GenerateAllFromMenu()
        {
            int built = GenerateAll(AllGeneratableLayouts());
            EditorUtility.DisplayDialog("World Stub Generator",
                built + " world(s) generated/updated (every CityLayoutDefinition with a sceneName).", "OK");
        }

        // ── Build-pipeline hooks (called by BuildAndroid; idempotent, batchmode-safe) ─────────────

        /// <summary>Ensure every generatable layout has its scene file + an enabled Build Settings entry.</summary>
        public static void EnsureGeneratedInBuildSettings()
        {
            foreach (var kit in AllGeneratableLayouts())
            {
                string scenePath = ScenePathFor(kit);
                if (!File.Exists(scenePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(scene, scenePath);
                    Debug.Log("[Ziptide] WorldStubGenerator created empty scene " + scenePath + " (populated during the build).");
                }
                EnsureSceneEnabled(scenePath);
            }
        }

        /// <summary>If the active scene belongs to a generated layout, (re)populate it. No-op otherwise.</summary>
        public static void PatchActiveSceneIfGenerated()
        {
            string active = EditorSceneManager.GetActiveScene().name;
            foreach (var kit in AllGeneratableLayouts())
                if (kit.sceneName == active)
                {
                    Populate(kit);
                    return;
                }
        }

        // ── The generic world shell (mirrors ScenePatcherToxicCity, driven purely by the layout) ──

        private static int GenerateAll(List<CityLayoutDefinition> layouts)
        {
            int built = 0;
            foreach (var kit in layouts)
            {
                var issues = kit.Validate();
                if (string.IsNullOrEmpty(kit.cityId)) issues.Add("empty cityId");
                if (kit.districts.Count == 0) issues.Add("no districts");
                if (issues.Count > 0)
                {
                    Debug.LogError("[Ziptide] WorldStubGenerator SKIPPED '" + kit.name + "': " + string.Join(" | ", issues));
                    continue;
                }

                string scenePath = ScenePathFor(kit);
                Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
                var scene = File.Exists(scenePath)
                    ? EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single)
                    : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                Populate(kit);

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, scenePath);
                EnsureSceneEnabled(scenePath);
                built++;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return built;
        }

        private static void Populate(CityLayoutDefinition kit)
        {
            string rootName = "__" + kit.cityId.ToUpperInvariant() + "_ROOT";
            var existing = GameObject.Find(rootName);
            if (existing != null) Object.DestroyImmediate(existing);
            var root = new GameObject(rootName).transform;

            Random.InitState(kit.seed);
            CityBuilder.Build(root, kit);

            EnsureLighting();
            EnsureEventSystem();
            EnsureWorldRuntime();

            var spawnDistrict = FindDistrict(kit, kit.spawnDistrictId) ?? kit.districts[0];
            Vector3 spawnPos = spawnDistrict.anchor + new Vector3(0f, kit.walkwayHeight + 0.1f, 0f);
            EnsureSpawn("player", spawnPos);

            var pack = EnsureWorldPack(kit, spawnPos);
            EnsureTravelStation(kit);
            EnsureDispatchAndBoard(pack, spawnPos);

            if (kit.spawnStarterWeapons)
            {
                var taser = ItemFactory.Create("taser_dart_gun", spawnPos + new Vector3(-0.6f, 1.0f, 1.0f));
                if (taser != null) taser.transform.SetParent(root, true);
                var grav = ItemFactory.Create("gravity_gun", spawnPos + new Vector3(0.6f, 1.0f, 1.0f));
                if (grav != null) grav.transform.SetParent(root, true);
            }
        }

        // ── Shell pieces (find-or-update; never duplicated) ───────────────────────────────────────

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
            var profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(ZiptideConstants.PathDefaultWorldProfileWorlds);
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

        /// <summary>
        /// Identity + spawn markers only — authored data (jobs, flagsRequired/Granted, themes) on an
        /// existing pack is PRESERVED so regeneration never wipes story wiring.
        /// </summary>
        private static WorldPackDefinition EnsureWorldPack(CityLayoutDefinition kit, Vector3 spawnPos)
        {
            string path = PackFolder + "/" + kit.sceneName + "_WorldPack.asset";
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (pack == null)
            {
                Directory.CreateDirectory(PackFolder);
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, path);
            }
            pack.packId = kit.cityId;
            pack.displayName = string.IsNullOrEmpty(kit.displayName) ? kit.cityId : kit.displayName;
            pack.sceneName = kit.sceneName;
            pack.spawnMarkers.Clear();
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = spawnPos });
            EditorUtility.SetDirty(pack);
            return pack;
        }

        private static void EnsureTravelStation(CityLayoutDefinition kit)
        {
            string path = PackFolder + "/" + kit.sceneName + "Exit_WorldPack.asset";
            var exitPack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (exitPack == null)
            {
                Directory.CreateDirectory(PackFolder);
                exitPack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(exitPack, path);
            }
            exitPack.packId = kit.cityId + "_exit";
            exitPack.displayName = "Leave";
            exitPack.sceneName = FirstOtherBuildSceneName(kit.sceneName);
            EditorUtility.SetDirty(exitPack);

            Vector3 pos = kit.shipyard != null && kit.shipyard.enabled
                ? kit.shipyard.berthCenter + new Vector3(0f, kit.walkwayHeight + 0.1f, 6f)
                : kit.districts[0].anchor + new Vector3(0f, kit.walkwayHeight + 0.1f, -6f);

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

        private static void EnsureDispatchAndBoard(WorldPackDefinition pack, Vector3 spawnPos)
        {
            var jdGo = PatcherUtil.EnsureRootObject("JobDirector", Vector3.zero);
            var director = PatcherUtil.EnsureComponent<JobDirector>(jdGo);
            var so = new SerializedObject(director);
            PatcherUtil.SetObjectRef(so, "worldPack", pack);
            so.ApplyModifiedPropertiesWithoutUndo();

            var kioskGo = PatcherUtil.EnsureRootObject("DispatchKiosk", spawnPos + new Vector3(1.5f, 1.2f, 1.5f));
            PatcherUtil.EnsureComponent<DispatchKiosk>(kioskGo);
            PatcherUtil.EnsureComponent<XRSimpleInteractable>(kioskGo);
            var col = kioskGo.GetComponent<Collider>();
            if (col == null) { col = kioskGo.AddComponent<BoxCollider>(); col.isTrigger = true; }

            var boardGo = PatcherUtil.EnsureRootObject("ObjectiveBoard", spawnPos + new Vector3(-1.5f, 1.6f, 1.5f));
            PatcherUtil.EnsureComponent<ObjectiveBoard>(boardGo);
        }

        // ── Lookup helpers ─────────────────────────────────────────────────────────────────────────

        /// <summary>Every layout that opted into generation (non-empty sceneName, not ToxicCity's own scene).</summary>
        private static List<CityLayoutDefinition> AllGeneratableLayouts()
        {
            var result = new List<CityLayoutDefinition>();
            foreach (var guid in AssetDatabase.FindAssets("t:CityLayoutDefinition"))
            {
                var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (kit == null || string.IsNullOrEmpty(kit.sceneName)) continue;
                if (kit.sceneName == ZiptideConstants.SceneToxicCity) continue; // owned by ScenePatcherToxicCity
                result.Add(kit);
            }
            return result;
        }

        private static List<CityLayoutDefinition> SelectedLayouts()
        {
            var result = new List<CityLayoutDefinition>();
            foreach (var obj in Selection.objects)
                if (obj is CityLayoutDefinition kit && !string.IsNullOrEmpty(kit.sceneName)
                    && kit.sceneName != ZiptideConstants.SceneToxicCity)
                    result.Add(kit);
            return result;
        }

        private static string ScenePathFor(CityLayoutDefinition kit)
            => GeneratedSceneFolder + "/" + kit.sceneName + ".unity";

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

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var d in kit.districts)
                if (d != null && d.id == id) return d;
            return null;
        }
    }
}
#endif

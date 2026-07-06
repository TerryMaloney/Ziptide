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

            // Per-world sky/atmosphere from the layout's theme block — the layout is the source of truth
            // ("change the sky" = edit two colors on the layout asset; regeneration rewrites the assets).
            var theme = ThemeAuthor.EnsureThemeAsset(kit);
            var worldProfile = ThemeAuthor.EnsureWorldProfileAsset(kit, theme);
            EnsureWorldRuntime(worldProfile);

            var spawnDistrict = FindDistrict(kit, kit.spawnDistrictId) ?? kit.districts[0];
            // Spawn Y samples the SAME height pipeline the terrain collider is built from
            // (H3 TerrainField + flatten grading) — assuming walkwayHeight put spawns inside the
            // new terrain on 3 worlds (SPAWN_OVERLAP_SOLID, diag run 28682909567).
            float spawnGroundY = WorldExperienceBuilder.HeightAt(kit, spawnDistrict.anchor.x, spawnDistrict.anchor.z);
            Vector3 spawnPos = new Vector3(spawnDistrict.anchor.x, spawnGroundY + 0.15f, spawnDistrict.anchor.z);
            EnsureSpawn("player", spawnPos, WorldExperienceBuilder.SpawnYawDegrees(kit));

            var pack = EnsureWorldPack(kit, spawnPos);
            EnsureTravelStation(kit);
            EnsureDispatchAndBoard(pack, spawnPos);

            // Story contract + gating from the WorldJobLibrary spec (jobs, steps, rewards, flags,
            // GoToMarker targets as pack data). No-op for worlds without a spec.
            WorldJobLibrary.EnsureJobsFor(kit, pack);

            if (kit.spawnStarterWeapons)
            {
                var taser = ItemFactory.Create("taser_dart_gun", spawnPos + new Vector3(-0.6f, 1.0f, 1.0f));
                if (taser != null) taser.transform.SetParent(root, true);
                var grav = ItemFactory.Create("gravity_gun", spawnPos + new Vector3(0.6f, 1.0f, 1.0f));
                if (grav != null) grav.transform.SetParent(root, true);
                var pistol = ItemFactory.Create("pistol", spawnPos + new Vector3(0f, 1.0f, 1.2f));
                if (pistol != null) pistol.transform.SetParent(root, true);
                // MP100 melee pack: the blade joins the starter lineup — melee is a first-class verb
                // in the regular game, not an arena exclusive (Terry, 2026-07-06).
                var blade = ItemFactory.Create("breaker_blade", spawnPos + new Vector3(-1.2f, 1.0f, 1.1f));
                if (blade != null) blade.transform.SetParent(root, true);
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

        private static void EnsureWorldRuntime(WorldProfile profile)
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);
            if (profile == null)
                profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(ZiptideConstants.PathDefaultWorldProfileWorlds);
            if (profile != null)
            {
                var so = new SerializedObject(wr);
                PatcherUtil.SetObjectRef(so, "worldProfile", profile);
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureSpawn(string markerId, Vector3 pos, float yawDegrees = 0f)
        {
            string objName = markerId == "player" ? ZiptideConstants.GoSpawnPlayer : "__SPAWN_" + markerId;
            var go = PatcherUtil.EnsureRootObject(objName, pos);
            // Arrival staging (P1b): the rig spawns with the marker's rotation, so aim it at the vista.
            go.transform.rotation = Quaternion.Euler(0f, yawDegrees, 0f);
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
            // Update ONLY the player spawn — objective markers authored by WorldJobLibrary (or by hand)
            // are preserved across regeneration, same as jobs/flags/themes.
            var player = pack.spawnMarkers.Find(m => m != null && m.markerId == "player");
            if (player == null)
                pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = spawnPos });
            else
                player.localPosition = spawnPos;

            // POI markers (P1c): keep "poi_<id>" pack entries in sync with the layout's POI network so
            // contracts route through POIs (GoToMarker) and dev warp can land at any pocket. Marker Y
            // follows the terrain pad. Hand-authored non-POI markers are untouched.
            if (kit.pois != null)
            {
                foreach (var poi in kit.pois)
                {
                    if (poi == null || string.IsNullOrEmpty(poi.id)) continue;
                    string mid = "poi_" + poi.id;
                    float y = WorldExperienceBuilder.HeightAt(kit, poi.position.x, poi.position.z) + 0.2f;
                    var pos = new Vector3(poi.position.x, y, poi.position.z);
                    var m = pack.spawnMarkers.Find(mm => mm != null && mm.markerId == mid);
                    if (m == null)
                        pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = mid, localPosition = pos });
                    else
                        m.localPosition = pos;

                    // Build sockets (P3): every MachineSite POI gets a socket on its plinth — pay
                    // credits, raise a real extractor that persists in the world save.
                    if (poi.type == PoiType.MachineSite)
                    {
                        string sid = "socket_" + poi.id;
                        // Matches WorldPoiBuilder.BuildMachineSite's SocketPlinth (0, 0.9 top, 2.6).
                        var socketPos = pos + new Vector3(0f, 0.7f, 2.6f);
                        var sdef = pack.sockets.Find(ss => ss != null && ss.id == sid);
                        if (sdef == null)
                            pack.sockets.Add(new BuildSocketSpawnDefinition { id = sid, localPosition = socketPos });
                        else
                            sdef.localPosition = socketPos;
                    }

                    // Gardens (P3): every HarvestGrove POI gets 6 planters on WorldPoiBuilder's planter
                    // grid — plants ladder round-robin (fast / mid / idle). Find-or-update by id, so
                    // hand-tuned plantIds survive; positions always follow the layout.
                    if (poi.type == PoiType.HarvestGrove)
                    {
                        string[] ladder = { "dew_bulb", "rust_fern", "glass_reed" };
                        for (int gi = 0; gi < 6; gi++)
                        {
                            // Matches WorldPoiBuilder.BuildHarvestGrove's planter layout; soil sits on
                            // the planter box top (+0.68 over the pad).
                            var offset = new Vector3((gi % 3 - 1) * 3.2f, 0.68f, (gi / 3 == 0 ? -1f : 1f) * 2.4f);
                            string gid = "garden_" + poi.id + "_" + gi;
                            var g = pack.gardens.Find(gg => gg != null && gg.id == gid);
                            if (g == null)
                            {
                                pack.gardens.Add(new GardenSpawnDefinition
                                {
                                    id = gid, plantId = ladder[gi % ladder.Length],
                                    localPosition = pos + offset - new Vector3(0f, 0.2f, 0f)
                                });
                            }
                            else
                                g.localPosition = pos + offset - new Vector3(0f, 0.2f, 0f);
                        }
                    }
                }
            }
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

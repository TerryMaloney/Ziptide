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
    /// Builds the dedicated ToxicCity world — the reusable WORLD BLUEPRINT. A thin shell over
    /// <see cref="CityBuilder"/>: it ensures the scene + layout asset, resets the city root, runs the
    /// data-driven builder, then wires the world-level singletons (spawn, world pack, travel door,
    /// dispatch/board/job director). Self-generates the empty scene so the cloud build produces it with
    /// no Unity PC and no hand-edited YAML. Idempotent + menu-invoked + populated by the build pipeline.
    ///
    /// To clone a new world: author a new CityLayoutDefinition asset + copy this ~shell pointing at it.
    /// </summary>
    public static class ScenePatcherToxicCity
    {
        public const string SceneName = ZiptideConstants.SceneToxicCity;
        private const string ScenePath = ZiptideConstants.PathToxicCityScene;
        private const string LayoutPath = ZiptideConstants.PathToxicCityLayout;
        private const string WorldPackPath = ZiptideConstants.PathToxicCityWorldPack;
        private const string DefaultWorldProfilePath = ZiptideConstants.PathDefaultWorldProfileWorlds;

        [MenuItem("Ziptide/Worlds/Build Toxic City")]
        public static void BuildFromMenu()
        {
            var scene = OpenOrCreateScene();
            PopulateActiveToxicCity();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Toxic City",
                "Built/updated " + SceneName + ".\n\nWarp to it via Ziptide > Dev, or it ships in the next build.", "OK");
        }

        /// <summary>Ensure ToxicCity is enabled in Build Settings (called by BuildAndroid). Idempotent.</summary>
        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
                Debug.Log("[Ziptide] Created empty ToxicCity scene at " + normalized + " (will be populated by the build).");
            }
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(normalized, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(normalized, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        /// <summary>No-op unless the active scene is ToxicCity. Called in the BuildAndroid per-scene loop.</summary>
        public static void PatchActiveScene()
        {
            if (EditorSceneManager.GetActiveScene().name != SceneName) return;
            PopulateActiveToxicCity();
        }

        /// <summary>Build the whole city into the currently-open ToxicCity scene.</summary>
        public static void PopulateActiveToxicCity()
        {
            var kit = EnsureLayoutAsset();

            string rootName = "__" + kit.cityId.ToUpperInvariant() + "_ROOT";
            var root = ResetRoot(rootName);

            Random.InitState(kit.seed);
            CityBuilder.Build(root, kit);

            EnsureLighting();
            EnsureEventSystem();
            EnsureWorldRuntime();

            // Spawn on the Dispatch district plaza (open, on solid ground at walkwayHeight).
            var dispatch = FindDistrict(kit, "Dispatch") ?? (kit.districts.Count > 0 ? kit.districts[0] : null);
            Vector3 spawnPos = dispatch != null
                ? dispatch.anchor + new Vector3(0f, kit.walkwayHeight + 0.1f, 0f)
                : new Vector3(0f, kit.walkwayHeight + 0.1f, 0f);
            EnsureSpawn("player", spawnPos);

            var pack = EnsureWorldPack(kit, spawnPos);
            EnsureTravelStation(kit);
            EnsureDispatchAndBoard(pack, spawnPos);
            SpawnStarterWeapons(root, spawnPos);
        }

        // A taser + gravity gun by the spawn so you can actually fight the drones without hauling one in.
        private static void SpawnStarterWeapons(Transform root, Vector3 spawnPos)
        {
            var taser = ItemFactory.Create("taser_dart_gun", spawnPos + new Vector3(-0.6f, 1.0f, 1.0f));
            if (taser != null) taser.transform.SetParent(root, true);
            var grav = ItemFactory.Create("gravity_gun", spawnPos + new Vector3(0.6f, 1.0f, 1.0f));
            if (grav != null) grav.transform.SetParent(root, true);
        }

        // ── Layout asset (self-bootstrapping default) ────────────────────────
        private static CityLayoutDefinition EnsureLayoutAsset()
        {
            var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(LayoutPath);
            if (kit != null) return kit;

            Directory.CreateDirectory(Path.GetDirectoryName(LayoutPath));
            kit = BuildDefaultToxicCity();
            AssetDatabase.CreateAsset(kit, LayoutPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] Created default ToxicCityLayout at " + LayoutPath + " (edit it to re-layout the city).");
            return kit;
        }

        /// <summary>The hand-authored Toxic City — edit the generated asset to change it without code.</summary>
        private static CityLayoutDefinition BuildDefaultToxicCity()
        {
            var kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
            kit.cityId = "toxic_city";
            kit.seed = 1337;
            kit.walkwayHeight = 0f;

            // Districts — a coherent dock-to-plaza city.
            kit.districts.Add(new DistrictDef
            {
                id = "Shipyard", anchor = new Vector3(0f, 0f, -30f), bounds = new Vector2(22f, 18f), heightTier = 1,
                landmarks = { new LandmarkDef { name = "Crane", localPos = new Vector3(8f, 0f, -6f), height = 16f, width = 2f } },
                heroBuildings = { new HeroBuildingDef { id = "ShipyardOffice", localPos = new Vector3(-7f, 0f, 2f), footprint = new Vector2(7f, 7f), height = 4f, interior = InteriorKind.JobGiver, doorLocalPos = new Vector3(0f, 0f, -3.5f), interiorMarkerId = "shipyard_office" } },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "Dispatch", anchor = new Vector3(0f, 0f, -8f), bounds = new Vector2(20f, 18f), heightTier = 1,
                heroBuildings = { new HeroBuildingDef { id = "DispatchHall", localPos = new Vector3(0f, 0f, 6f), footprint = new Vector2(8f, 7f), height = 4.5f, interior = InteriorKind.JobGiver, doorLocalPos = new Vector3(0f, 0f, -3.5f), interiorMarkerId = "dispatch_inside" } },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "Plaza", anchor = new Vector3(0f, 0f, 28f), bounds = new Vector2(24f, 22f), heightTier = 2,
                landmarks =
                {
                    new LandmarkDef { name = "OligarchTower", localPos = new Vector3(9f, 0f, 6f), height = 40f, width = 6f },
                    new LandmarkDef { name = "GardenTower", localPos = new Vector3(-9f, 0f, 6f), height = 18f, width = 5f },
                },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "Market", anchor = new Vector3(26f, 0f, 8f), bounds = new Vector2(22f, 20f), heightTier = 1,
                props = { new PropPatchDef { kind = "Stall", center = Vector3.zero, size = new Vector2(12f, 12f), density = 0.5f } },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "CanalRow", anchor = new Vector3(-26f, 0f, 8f), bounds = new Vector2(16f, 26f), heightTier = 1,
                heroBuildings = { new HeroBuildingDef { id = "RelayVault", localPos = new Vector3(0f, 0f, 0f), footprint = new Vector2(9f, 8f), height = 5f, interior = InteriorKind.Mission, doorLocalPos = new Vector3(0f, 0f, -4f), interiorMarkerId = "relay_node" } },
            });

            // Connections — main boulevard + side streets forming a loop (so it walks as a city).
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Shipyard", toDistrictId = "Dispatch", kind = ConnectionKind.GroundStreet, width = 8f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Dispatch", toDistrictId = "Plaza", kind = ConnectionKind.GroundStreet, width = 8f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Plaza", toDistrictId = "Market", kind = ConnectionKind.GroundStreet, width = 6f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Dispatch", toDistrictId = "Market", kind = ConnectionKind.GroundStreet, width = 6f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Plaza", toDistrictId = "CanalRow", kind = ConnectionKind.ElevatedWalkway, width = 5f, tier = 1 });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Dispatch", toDistrictId = "CanalRow", kind = ConnectionKind.Bridge, width = 5f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "CanalRow", toDistrictId = "Market", kind = ConnectionKind.Ramp, width = 5f });

            // Canals — toxic sludge in the gaps the elevated walkway/bridge cross.
            kit.canals.Add(new CanalRegionDef { center = new Vector3(-14f, 0f, 16f), size = new Vector2(14f, 30f), depth = 2.2f });
            kit.canals.Add(new CanalRegionDef { center = new Vector3(0f, 0f, 10f), size = new Vector2(10f, 12f), depth = 2f });

            // Drone zones — NOT every district. Passive tutorial trio + two combat patrols.
            kit.droneZones.Add(new DroneZoneDef { id = "Tutorial_Dispatch", center = new Vector3(0f, 0f, -8f), radius = 4f, count = 3, respawnDelay = 0f, combat = false });
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Market", center = new Vector3(26f, 0f, 8f), radius = 6f, count = 3, respawnDelay = 14f, combat = true });
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Canal", center = new Vector3(-14f, 0f, 16f), radius = 4f, count = 2, respawnDelay = 16f, combat = true });

            // Shipyard berth + static ship, just past the shipyard district.
            kit.shipyard = new ShipyardBerthDef
            {
                enabled = true,
                berthCenter = new Vector3(0f, 0f, -48f),
                berthSize = new Vector2(20f, 16f),
                shipLocalPos = new Vector3(0f, 1.6f, 0f),
                shipSize = new Vector3(5f, 3f, 12f),
                shipRotationY = 0f,
            };
            return kit;
        }

        // ── Root reset (deterministic rebuild) ───────────────────────────────
        private static Transform ResetRoot(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) Object.DestroyImmediate(existing);
            var go = new GameObject(name);
            return go.transform;
        }

        // ── World singletons (find-or-update; never duplicated) ──────────────
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

        private static WorldPackDefinition EnsureWorldPack(CityLayoutDefinition kit, Vector3 spawnPos)
        {
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(WorldPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WorldPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, WorldPackPath);
            }
            pack.packId = kit.cityId;
            pack.displayName = "Toxic City";
            pack.sceneName = SceneName;
            pack.spawnMarkers.Clear();
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = spawnPos });
            EditorUtility.SetDirty(pack);
            return pack;
        }

        private static void EnsureTravelStation(CityLayoutDefinition kit)
        {
            var exitPack = EnsureExitPackAsset();
            if (exitPack == null) return;

            // Place by the shipyard berth — the eventual "leave on your ship" spot.
            Vector3 pos = kit.shipyard != null
                ? kit.shipyard.berthCenter + new Vector3(0f, kit.walkwayHeight + 0.1f, 6f)
                : new Vector3(0f, kit.walkwayHeight + 0.1f, -40f);

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

        // The exit door points at the first OTHER enabled build scene (guaranteed in-build, avoids
        // TRAVEL_DEST_NOT_IN_BUILD). Replaced by the ship once the flight system lands.
        private const string ExitPackPath = "Assets/Ziptide/Content/Worlds/Packs/ToxicCityExit_WorldPack.asset";

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
            pack.packId = "toxic_city_exit";
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

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            foreach (var d in kit.districts)
                if (d != null && d.id == id) return d;
            return null;
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

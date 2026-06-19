#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Graybox of the STARTER WORLD (onboarding planet) per
    /// docs/GPT_ADDITIONS/2026-06-18_Starter_World_Blockout/. A compact, WALKABLE chain of 10 named
    /// region roots: Hub → Spaceport (+ Vehicle Port branch) → Toxic City spine (+ Canals/Slum
    /// branches) → Outskirts → Open Badlands → Mission Pocket → Dormant Ziptide gate. Primitive
    /// blockout only (platforms, ramps, landmark silhouettes, placeholder markers) — scale + pathing +
    /// region separation over visuals. Idempotent + menu-invoked + populated by the build pipeline.
    ///
    /// Reachable via Dev Warp (creates a StarterWorld WorldPack). NOT a replacement for D0_City — this
    /// is the new explorable first world to iterate on.
    /// </summary>
    public static class ScenePatcherStarterWorld
    {
        public const string SceneName = "StarterWorld";
        private const string ScenePath = "Assets/Ziptide/Scenes/StarterWorld.unity";
        private const string WorldPackPath = "Assets/Ziptide/Content/Worlds/Packs/StarterWorld_WorldPack.asset";
        private const string DefaultWorldProfilePath = "Assets/Ziptide/Content/Worlds/DefaultWorldProfile.asset";

        // Walkable platform top sits at y=0; the player spawns just above it.
        private const float PlatformTop = 0f;
        private const float PlatformThickness = 1f;

        // ── Colors (graybox palette) ────────────────────────────────────────
        private static readonly Color CityGray = new Color(0.30f, 0.31f, 0.34f);
        private static readonly Color RoadGray = new Color(0.22f, 0.22f, 0.25f);
        private static readonly Color Sludge = new Color(0.25f, 0.40f, 0.12f);
        private static readonly Color Rust = new Color(0.45f, 0.28f, 0.18f);
        private static readonly Color Sand = new Color(0.45f, 0.40f, 0.28f);
        private static readonly Color Alien = new Color(0.25f, 0.55f, 0.65f);
        private static readonly Color Marker = new Color(0.9f, 0.75f, 0.1f);

        [MenuItem("Ziptide/Dev/Build Starter World (graybox)")]
        public static void BuildFromMenu()
        {
            var scene = OpenOrCreateScene();
            PopulateActiveStarterWorld();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Starter World",
                "Built/updated " + SceneName + " graybox.\n\nWarp to it via Ziptide > Dev > Warp Window, "
                + "or it ships in the next build.", "OK");
        }

        /// <summary>Ensure StarterWorld is enabled in Build Settings (called by BuildAndroid). Idempotent.</summary>
        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                Debug.LogWarning("[Ziptide] StarterWorld scene not found — run 'Ziptide/Dev/Build Starter World (graybox)' once.");
                return;
            }
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(normalized, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(normalized, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        /// <summary>Populate the currently-open StarterWorld scene. Called by the menu + build pipeline.</summary>
        public static void PopulateActiveStarterWorld()
        {
            var worldRoot = Root("WorldRoot", Vector3.zero);

            EnsureLighting();
            EnsureEventSystem();
            EnsureWorldRuntime();

            // Region roots (named exactly per the brief) + their graybox content.
            BuildHub(Zone(worldRoot, "Hub_DockQuarter", new Vector3(0f, 0f, 0f)));
            BuildSpaceport(Zone(worldRoot, "Zone_Spaceport", new Vector3(0f, 0f, 40f)));
            BuildVehiclePort(Zone(worldRoot, "Zone_GroundVehiclePort", new Vector3(34f, 0f, 40f)));
            BuildCitySpine(Zone(worldRoot, "Zone_ToxicCity_MainSpine", new Vector3(0f, 0f, 82f)));
            BuildCanals(Zone(worldRoot, "Zone_SludgeCanals", new Vector3(-34f, 0f, 82f)));
            BuildSlum(Zone(worldRoot, "Zone_SlumWalkways", new Vector3(34f, 0f, 82f)));
            BuildOutskirts(Zone(worldRoot, "Zone_OutskirtsTransition", new Vector3(0f, 0f, 124f)));
            BuildBadlands(Zone(worldRoot, "Zone_BadlandsVehicleArea", new Vector3(0f, 0f, 180f)));
            BuildMissionPocket(Zone(worldRoot, "Zone_BadlandsMissionPocket", new Vector3(0f, 0f, 232f)));
            BuildZiptideSite(Zone(worldRoot, "Zone_DormantZiptideSite", new Vector3(0f, 0f, 272f)));

            // Organizational roots from the brief.
            Zone(worldRoot, "MissionMarkers", Vector3.zero);
            Zone(worldRoot, "SpawnPoints", Vector3.zero);
            Zone(worldRoot, "VehicleTestRoutes", Vector3.zero);
            Zone(worldRoot, "Landmarks", Vector3.zero);
            Zone(worldRoot, "LightingAndAtmosphere", Vector3.zero);

            // Connect the chain with walkway bridges so it's walkable end-to-end.
            Walkway("Walk_Hub_Spaceport", new Vector3(0f, 0f, 20f), new Vector3(6f, 0f, 22f));
            Walkway("Walk_Spaceport_VehiclePort", new Vector3(17f, 0f, 40f), new Vector3(28f, 0f, 6f));
            Walkway("Walk_Spaceport_Spine", new Vector3(0f, 0f, 61f), new Vector3(6f, 0f, 22f));
            Walkway("Walk_Spine_Canals", new Vector3(-17f, 0f, 82f), new Vector3(28f, 0f, 6f));
            Walkway("Walk_Spine_Slum", new Vector3(17f, 0f, 82f), new Vector3(28f, 0f, 6f));
            Walkway("Walk_Spine_Outskirts", new Vector3(0f, 0f, 103f), new Vector3(8f, 0f, 22f));
            Walkway("Walk_Outskirts_Badlands", new Vector3(0f, 0f, 152f), new Vector3(10f, 0f, 36f));
            Walkway("Walk_Badlands_Pocket", new Vector3(0f, 0f, 206f), new Vector3(8f, 0f, 28f));
            Walkway("Walk_Pocket_Ziptide", new Vector3(0f, 0f, 252f), new Vector3(8f, 0f, 20f));

            EnsureSpawn("player", new Vector3(0f, PlatformTop + 0.1f, -2f));
            EnsureWorldPackAsset();
        }

        // ── Zones ───────────────────────────────────────────────────────────

        private static void BuildHub(Transform z)
        {
            Platform(z, "HubPlatform", Vector3.zero, new Vector3(16f, 16f), CityGray);
            Landmark(z, "HubOverlookTower", new Vector3(-6f, 0f, -5f), 9f, CityGray);
            Box(z, "HubSign", new Vector3(0f, 1.2f, 6f), new Vector3(2f, 2.4f, 0.3f), Marker);
            MissionMarker(z, "Marker_Start", new Vector3(0f, 0.2f, 3f));
        }

        private static void BuildSpaceport(Transform z)
        {
            Platform(z, "SpaceportDeck", Vector3.zero, new Vector3(22f, 20f), CityGray);
            for (int i = 0; i < 3; i++)
                Cylinder(z, "LandingPad_" + i, new Vector3(-7f + i * 7f, 0.1f, 5f), new Vector3(4f, 0.1f, 4f), RoadGray);
            Landmark(z, "ControlTower", new Vector3(9f, 0f, -7f), 16f, CityGray);
            for (int i = 0; i < 4; i++)
                Box(z, "Crate_" + i, new Vector3(-8f + i * 2.2f, 0.6f, -6f), new Vector3(1.6f, 1.2f, 1.6f), Rust);
            MissionMarker(z, "Marker_Mission1_Spaceport", new Vector3(0f, 0.2f, 0f));
        }

        private static void BuildVehiclePort(Transform z)
        {
            Platform(z, "DepotBay", Vector3.zero, new Vector3(18f, 14f), RoadGray);
            Box(z, "Vehicle_Placeholder_1", new Vector3(-3f, 0.7f, 0f), new Vector3(2f, 1.4f, 4f), Rust);
            Box(z, "Vehicle_Placeholder_2", new Vector3(3f, 0.7f, 0f), new Vector3(2f, 1.4f, 4f), Rust);
            MissionMarker(z, "Marker_VehicleStaging", new Vector3(0f, 0.2f, 5f));
        }

        private static void BuildCitySpine(Transform z)
        {
            // Elevated highway spine — raised platform you walk along.
            Platform(z, "HighwaySpine", new Vector3(0f, 1.5f, 0f), new Vector3(10f, 28f), RoadGray);
            for (int i = 0; i < 4; i++)
                Landmark(z, "Tower_" + i, new Vector3(i % 2 == 0 ? -9f : 9f, 0f, -10f + i * 7f), 14f + i * 3f, CityGray);
            Sludge(z, "SludgeUnderSpine", new Vector3(0f, -2f, 0f), new Vector3(30f, 30f));
        }

        private static void BuildCanals(Transform z)
        {
            Platform(z, "CanalWalk", new Vector3(0f, 0f, 0f), new Vector3(8f, 24f), CityGray);
            Sludge(z, "Canal", new Vector3(-6f, -1.5f, 0f), new Vector3(10f, 26f));
            Box(z, "PumpMachinery", new Vector3(5f, 1f, 6f), new Vector3(2.5f, 2f, 2.5f), Rust);
            Box(z, "HazardSign", new Vector3(-2f, 1f, 0f), new Vector3(1.2f, 2f, 0.2f), Marker);
        }

        private static void BuildSlum(Transform z)
        {
            Platform(z, "SlumGround", Vector3.zero, new Vector3(16f, 22f), CityGray);
            // Stacked shacks for vertical density.
            for (int i = 0; i < 6; i++)
            {
                float h = 2f + (i % 3);
                Box(z, "Shack_" + i, new Vector3(-5f + (i % 3) * 5f, h * 0.5f, -6f + (i / 3) * 7f),
                    new Vector3(3f, h, 3f), Rust);
            }
            Platform(z, "UpperWalkway", new Vector3(0f, 3.2f, 4f), new Vector3(12f, 3f), RoadGray);
        }

        private static void BuildOutskirts(Transform z)
        {
            Platform(z, "OutskirtsRoad", Vector3.zero, new Vector3(14f, 24f), RoadGray);
            // City wall + broken gate (gap to walk through).
            Box(z, "CityWall_L", new Vector3(-7f, 3f, -8f), new Vector3(8f, 6f, 1.5f), CityGray);
            Box(z, "CityWall_R", new Vector3(7f, 3f, -8f), new Vector3(8f, 6f, 1.5f), CityGray);
            Box(z, "BrokenCheckpoint", new Vector3(6f, 1f, 2f), new Vector3(2f, 2f, 2f), Rust);
        }

        private static void BuildBadlands(Transform z)
        {
            Platform(z, "BadlandsGround", Vector3.zero, new Vector3(60f, 48f), Sand);
            for (int i = 0; i < 8; i++)
            {
                var p = new Vector3(Mathf.Sin(i * 1.3f) * 22f, 0f, -18f + i * 5f);
                Landmark(z, "Rock_" + i, p, 3f + (i % 4) * 2f, new Color(0.4f, 0.36f, 0.30f));
            }
            // Wide vehicle route loop markers.
            MissionMarker(z, "VehicleRoute_A", new Vector3(-20f, 0.2f, 0f));
            MissionMarker(z, "VehicleRoute_B", new Vector3(20f, 0.2f, 10f));
        }

        private static void BuildMissionPocket(Transform z)
        {
            Platform(z, "PocketArena", Vector3.zero, new Vector3(24f, 22f), Sand);
            Landmark(z, "RuinRelayStation", new Vector3(0f, 0f, -7f), 8f, Rust);
            // Cover blocks + enemy/objective placeholders.
            for (int i = 0; i < 4; i++)
                Box(z, "Cover_" + i, new Vector3(-6f + i * 4f, 0.8f, 2f), new Vector3(1.8f, 1.6f, 1.8f), Rust);
            for (int i = 0; i < 2; i++)
            {
                var drone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                drone.name = "ScavengerDrone_" + i;
                drone.transform.SetParent(z, false);
                drone.transform.localPosition = new Vector3(-3f + i * 6f, 1.6f, 4f);
                drone.transform.localScale = Vector3.one * 0.4f;
                drone.AddComponent<DroneRuntime>();
            }
            Box(z, "ObjectivePedestal", new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f), Alien);
            MissionMarker(z, "Marker_Mission2_Retrieval", new Vector3(0f, 0.3f, 0f));
        }

        private static void BuildZiptideSite(Transform z)
        {
            Platform(z, "GatePlatform", Vector3.zero, new Vector3(18f, 18f), CityGray);
            // Dormant gate ring (ring of pillars as a placeholder for the portal).
            int ringCount = 10;
            for (int i = 0; i < ringCount; i++)
            {
                float a = (i / (float)ringCount) * Mathf.PI * 2f;
                var p = new Vector3(Mathf.Cos(a) * 5f, 2.5f, Mathf.Sin(a) * 5f);
                Box(z, "GatePillar_" + i, p, new Vector3(0.6f, 5f, 0.6f), Alien);
            }
            Box(z, "ActivatorSocket", new Vector3(0f, 0.6f, 0f), new Vector3(1f, 1.2f, 1f), Marker);
            Landmark(z, "AncientStructure", new Vector3(0f, 0f, -8f), 18f, Alien);
            MissionMarker(z, "Marker_ZiptideActivation", new Vector3(0f, 0.3f, 0f));
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private static Transform Root(string name, Vector3 pos) => PatcherUtil.EnsureRootObject(name, pos).transform;

        private static Transform Zone(Transform parent, string name, Vector3 localPos)
        {
            var t = parent.Find(name);
            if (t == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                t = go.transform;
            }
            t.localPosition = localPos;
            return t;
        }

        private static void Platform(Transform parent, string name, Vector3 localCenter, Vector2 size, Color color)
        {
            var go = Prim(parent, name, PrimitiveType.Cube);
            go.transform.localPosition = localCenter + new Vector3(0f, PlatformTop - PlatformThickness * 0.5f, 0f);
            go.transform.localScale = new Vector3(size.x, PlatformThickness, size.y);
            Paint(go, color);
        }

        private static void Walkway(string name, Vector3 worldCenter, Vector3 size)
        {
            var go = GameObject.Find(name);
            if (go == null) { go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; }
            go.transform.position = worldCenter + new Vector3(0f, PlatformTop - PlatformThickness * 0.5f, 0f);
            go.transform.localScale = new Vector3(size.x, PlatformThickness, size.z);
            Paint(go, RoadGray);
        }

        private static void Box(Transform parent, string name, Vector3 localPos, Vector3 size, Color color)
        {
            var go = Prim(parent, name, PrimitiveType.Cube);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            Paint(go, color);
        }

        private static void Cylinder(Transform parent, string name, Vector3 localPos, Vector3 size, Color color)
        {
            var go = Prim(parent, name, PrimitiveType.Cylinder);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            Paint(go, color);
        }

        private static void Landmark(Transform parent, string name, Vector3 localPosXZ, float height, Color color)
        {
            var go = Prim(parent, name, PrimitiveType.Cube);
            go.transform.localPosition = new Vector3(localPosXZ.x, height * 0.5f, localPosXZ.z);
            go.transform.localScale = new Vector3(3f, height, 3f);
            Paint(go, color);
        }

        private static void Sludge(Transform parent, string name, Vector3 localCenter, Vector2 size)
        {
            var go = Prim(parent, name, PrimitiveType.Cube);
            go.transform.localPosition = localCenter;
            go.transform.localScale = new Vector3(size.x, 0.4f, size.y);
            // No collider — decorative hazard surface (the fall-safety net handles drops).
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            Paint(go, Sludge);
        }

        private static void MissionMarker(Transform parent, string name, Vector3 localPos)
        {
            var t = parent.Find(name);
            GameObject go = t != null ? t.gameObject : new GameObject(name);
            if (t == null) go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            // Pure placeholder transform — named marker for missions/spawns; no renderer/collider.
        }

        private static GameObject Prim(Transform parent, string name, PrimitiveType type)
        {
            var existing = parent.Find(name);
            if (existing != null) return existing.gameObject;
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void Paint(GameObject go, Color color) => ItemFactory.ApplyURPColor(go, color);

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
            string objName = markerId == "player" ? "__SPAWN_PLAYER" : "__SPAWN_" + markerId;
            var go = PatcherUtil.EnsureRootObject(objName, pos);
            var marker = PatcherUtil.EnsureComponent<SpawnMarkerRuntime>(go);
            var so = new SerializedObject(marker);
            PatcherUtil.SetString(so, "markerId", markerId);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureWorldPackAsset()
        {
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(WorldPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WorldPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, WorldPackPath);
            }
            pack.packId = "starter_world";
            pack.displayName = "Starter World (graybox)";
            pack.sceneName = SceneName;
            pack.spawnMarkers.Clear();
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = new Vector3(0f, 0.1f, -2f) });
            EditorUtility.SetDirty(pack);
        }

        private static Scene OpenOrCreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return EditorSceneManager.GetActiveScene();
            if (File.Exists(ScenePath))
                return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            return scene;
        }
    }
}
#endif

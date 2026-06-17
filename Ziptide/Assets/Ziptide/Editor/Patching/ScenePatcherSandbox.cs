#if UNITY_EDITOR
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
    /// Builds the Sandbox Test Lab — a clean, larger (30x30) developer scene to prototype content in
    /// before it ships to a real world. Idempotent: re-running updates in place. Menu-invoked only
    /// (NOT part of the build loop, so it can never break a build). Pairs with the Dev Warp window —
    /// the sandbox + its zone markers show up there for one-click jumping.
    ///
    /// Zones (named spawn markers + a marker post): grab, range, enemy, travel, artwall, loco.
    /// See docs/design/SANDBOX_TEST_LAB.md for the design.
    /// </summary>
    public static class ScenePatcherSandbox
    {
        public const string SceneName = "SandboxTestLab";
        private const string ScenePath = "Assets/Ziptide/Scenes/SandboxTestLab.unity";
        private const string WorldPackPath = "Assets/Ziptide/Content/Worlds/Packs/Sandbox_WorldPack.asset";
        private const string DefaultWorldProfilePath = "Assets/Ziptide/Content/World/DefaultWorldProfile.asset";
        private const float FloorSize = 30f;

        // Zone id, label, floor position. Each becomes a named SpawnMarkerRuntime (Dev Warp target).
        private static readonly (string id, string label, Vector3 pos)[] Zones =
        {
            ("grab",    "A: Grab + Holster", new Vector3(-9f, 0.1f,  9f)),
            ("range",   "B: Weapon Range",   new Vector3( 0f, 0.1f,  9f)),
            ("enemy",   "C: Enemy Sandbox",  new Vector3( 9f, 0.1f,  9f)),
            ("travel",  "D: Travel Loop",    new Vector3(-9f, 0.1f, -9f)),
            ("artwall", "E: Art Prototype",  new Vector3( 0f, 0.1f, -9f)),
            ("loco",    "F: Locomotion",     new Vector3( 9f, 0.1f, -9f)),
        };

        [MenuItem("Ziptide/Dev/Build Sandbox Test Lab")]
        public static void BuildFromMenu()
        {
            var scene = OpenOrCreateScene();
            PopulateActiveSandbox();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] Sandbox Test Lab built/updated at " + ScenePath
                + " — open via Ziptide > Dev > Warp Window.");
            EditorUtility.DisplayDialog("Sandbox Test Lab",
                "Built/updated " + SceneName + ".\n\nAdd it to Build Settings to warp into it at runtime, "
                + "or use Dev Warp > Open Scene to edit it.", "OK");
        }

        /// <summary>
        /// Populate the CURRENTLY-OPEN sandbox scene (floor, spawn, zones, gear, drones). Called by the
        /// menu and by the build pipeline (BuildAndroid) so the sandbox content is generated on every
        /// build — no manual menu step required. Idempotent.
        /// </summary>
        public static void PopulateActiveSandbox()
        {
            PopulateScene();
            EnsureWorldPackAsset();
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

        private static void PopulateScene()
        {
            EnsureLighting();
            EnsureFloor();
            EnsureWorldRuntime();
            EnsureSpawn("player", new Vector3(0f, 0.1f, 0f));
            foreach (var z in Zones)
            {
                EnsureSpawn(z.id, z.pos);
                EnsureZonePost(z.id, z.label, z.pos);
            }
            EnsureReturnDoor();
            EnsureSandboxContent();
        }

        private const string GravityGunDefPath = "Assets/Ziptide/Content/Items/Sandbox_GravityGun.asset";

        /// <summary>Drop test gear + drones in the sandbox so weapons can be exercised immediately.</summary>
        private static void EnsureSandboxContent()
        {
            EnsureGravityGunDef();

            // Place the gravity gun + a taser by the Grab zone, built via ItemFactory so both get the
            // forward-snapping Grip attach + holster-compatible setup. Guard by name = idempotent.
            if (GameObject.Find("GravityGun") == null)
                Ziptide.Gameplay.ItemFactory.Create("gravity_gun", new Vector3(-9f, 1.1f, 8f));
            if (GameObject.Find("TaserDartGun") == null)
                Ziptide.Gameplay.ItemFactory.Create("taser_dart_gun", new Vector3(-9f, 1.1f, 9.5f));

            // Test drones in the Enemy zone to shoot at.
            for (int i = 0; i < 3; i++)
            {
                string n = "SandboxDrone_" + i;
                if (GameObject.Find(n) != null) continue;
                var drone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                drone.name = n;
                drone.transform.position = new Vector3(8f + i * 1.2f, 1.6f, 9f);
                drone.transform.localScale = Vector3.one * 0.4f;
                drone.AddComponent<Ziptide.Gameplay.DroneRuntime>();
            }
        }

        private static void EnsureGravityGunDef()
        {
            var def = AssetDatabase.LoadAssetAtPath<GravityGunDefinition>(GravityGunDefPath);
            if (def == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content/Items"))
                    AssetDatabase.CreateFolder("Assets/Ziptide/Content", "Items");
                def = ScriptableObject.CreateInstance<GravityGunDefinition>();
                AssetDatabase.CreateAsset(def, GravityGunDefPath);
            }
            def.itemId = "gravity_gun";
            def.mass = 0.45f;
            EditorUtility.SetDirty(def);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 6f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void EnsureFloor()
        {
            var floor = GameObject.Find("SandboxFloor");
            if (floor == null)
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "SandboxFloor";
            }
            // Unity Plane is 10x10 at scale 1 → scale to FloorSize.
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(FloorSize / 10f, 1f, FloorSize / 10f);
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

        private static void EnsureZonePost(string id, string label, Vector3 pos)
        {
            string postName = "ZonePost_" + id;
            var post = GameObject.Find(postName);
            if (post == null)
            {
                post = GameObject.CreatePrimitive(PrimitiveType.Cube);
                post.name = postName;
                // Marker post only — no collider so it never blocks the player.
                var col = post.GetComponent<Collider>();
                if (col != null) Object.DestroyImmediate(col);
            }
            post.transform.position = new Vector3(pos.x, 1.5f, pos.z);
            post.transform.localScale = new Vector3(0.25f, 3f, 0.25f);
        }

        private static void EnsureReturnDoor()
        {
            var door = PatcherUtil.EnsureRootObject("__ReturnDoor", new Vector3(0f, 1.2f, 13.5f));
            var box = PatcherUtil.EnsureComponent<BoxCollider>(door);
            box.isTrigger = true;
            box.size = new Vector3(3f, 2.5f, 1f);
            var trigger = PatcherUtil.EnsureComponent<ProximityTravelTrigger>(door);
            trigger.SetDestination("MilestoneA_GrabCube");
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
            pack.packId = "sandbox";
            pack.displayName = "Sandbox Test Lab";
            pack.sceneName = SceneName;
            pack.spawnMarkers.Clear();
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = new Vector3(0f, 0.1f, 0f) });
            foreach (var z in Zones)
                pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = z.id, localPosition = z.pos });
            EditorUtility.SetDirty(pack);
        }
    }
}
#endif

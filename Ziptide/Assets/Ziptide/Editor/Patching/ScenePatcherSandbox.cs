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
    /// Builds the Sandbox Test Lab — a clean, larger (30x30) developer scene to prototype content in
    /// before it ships to a real world. Idempotent: re-running updates in place. Pairs with the Dev
    /// Warp window — the sandbox + its zone markers show up there for one-click jumping.
    ///
    /// The build pipeline (BuildAndroid) calls EnsureInBuildSettings() + PopulateActiveSandbox() so the
    /// sandbox is always enabled in Build Settings and populated on every build — no manual menu step.
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

        /// <summary>
        /// Ensure the sandbox scene is ENABLED in Build Settings so the build loop opens + populates it
        /// and it ships in the APK (and can be warped to at runtime). Idempotent. Called by BuildAndroid
        /// so no manual "add to Build Settings" step is ever needed — this was the reason the sandbox
        /// gear/drones never reached the headset. Mirrors ScenePatcherD0.AddSceneToBuildSettings.
        /// </summary>
        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                // Don't fabricate a scene mid-build; the menu (Build Sandbox Test Lab) creates it.
                Debug.LogWarning("[Ziptide] Sandbox scene not found at " + normalized
                    + " — run 'Ziptide/Dev/Build Sandbox Test Lab' once to create it. "
                    + "Skipping Build Settings add for this build.");
                return;
            }

            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0)
            {
                scenes.Add(new EditorBuildSettingsScene(normalized, true));
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log("[Ziptide] Sandbox added to Build Settings: " + normalized);
            }
            else if (!scenes[idx].enabled)
            {
                scenes[idx] = new EditorBuildSettingsScene(normalized, true);
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log("[Ziptide] Sandbox re-enabled in Build Settings: " + normalized);
            }
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
            EnsureEventSystem();
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

        private const string GravityGunDefPath = "Assets/Ziptide/Resources/Items/Sandbox_GravityGun.asset";

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
                drone.AddComponent<Ziptide.Gameplay.DroneRuntime>().respawnDelay = 8f; // practice: respawn so there's always something to shoot
            }

            EnsureTraversalCorner();
            EnsureFactoryCorner();
        }

        /// <summary>Hardwiring 4.1b: the automation test corner — a source→belts→corner→sink line so
        /// the conveyor layer is WATCHABLE on device: pucks ride the teal chevrons, jams compress
        /// when you stand and stare, and the sink pays scrap (ZIPTIDE: BELT_SUNK). Authoring only —
        /// BeltFloorRuntime builds everything at runtime. Idempotent by name.</summary>
        private static void EnsureFactoryCorner()
        {
            if (GameObject.Find("SandboxBeltFloor") != null) return;
            var floor = new GameObject("SandboxBeltFloor");
            floor.transform.position = new Vector3(-14f, 0f, -12f);
            var belt = floor.AddComponent<Ziptide.Gameplay.BeltFloorRuntime>();
            belt.width = 8; belt.depth = 4; belt.cellSize = 0.8f;
            belt.AuthorSource(0, 1, Ziptide.Content.Automation.BeltDir.East, "scrap");
            belt.AuthorBelt(1, 1, Ziptide.Content.Automation.BeltDir.East);
            belt.AuthorBelt(2, 1, Ziptide.Content.Automation.BeltDir.East);
            belt.AuthorBelt(3, 1, Ziptide.Content.Automation.BeltDir.East);
            belt.AuthorBelt(4, 1, Ziptide.Content.Automation.BeltDir.North); // the corner turn
            belt.AuthorBelt(4, 2, Ziptide.Content.Automation.BeltDir.East);
            belt.AuthorBelt(5, 2, Ziptide.Content.Automation.BeltDir.East);
            belt.AuthorSink(6, 2, "scrap");
        }

        /// <summary>Hardwiring 1.4b: the traversal test corner by the Locomotion zone — a climbable
        /// tower (hand-over-hand up the stud face) with a zipline off its top back toward spawn, so
        /// both signature traversal verbs are exercisable in one 30-second loop. Idempotent by name.</summary>
        private static void EnsureTraversalCorner()
        {
            if (GameObject.Find("ClimbTower") == null)
            {
                var tower = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tower.name = "ClimbTower";
                tower.transform.position = new Vector3(12.5f, 2.5f, -9f); // beside F: Locomotion
                tower.transform.localScale = new Vector3(1.2f, 5f, 3f);   // 5m — a real climb, not a hop
                var rend = tower.GetComponent<Renderer>();
                if (rend != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader != null)
                    {
                        var mat = new Material(shader);
                        mat.color = new Color(0.30f, 0.28f, 0.26f);
                        rend.sharedMaterial = mat;
                    }
                }
                tower.AddComponent<Ziptide.Gameplay.ClimbableSurface>(); // collider exists first (gotcha #6)
            }

            if (GameObject.Find("SandboxZipline") == null)
            {
                var zip = new GameObject("SandboxZipline");
                zip.AddComponent<Ziptide.Gameplay.ZiplineRuntime>()
                   .Init(new Vector3(12.5f, 5.6f, -9f),   // tower top —
                         new Vector3(0f, 1.6f, 0f));      // — down to spawn: climb up, ride home
            }

            // 1.4c: the other two verbs join the corner — the tower is now reachable THREE ways
            // (climb the studs, ride the lift, take the jump pad) and the zipline brings you home.
            if (GameObject.Find("SandboxLift") == null)
            {
                var lift = new GameObject("SandboxLift");
                lift.AddComponent<Ziptide.Gameplay.LiftRuntime>().Init(
                    new[] { new Vector3(12.5f, 0.25f, -6.4f),   // ground, north of the tower
                            new Vector3(12.5f, 5.05f, -6.4f) }, // deck meets the tower top — step across
                    speed: 1.6f, dwell: 4f);
            }
            if (GameObject.Find("SandboxJumpPad") == null)
            {
                var pad = new GameObject("SandboxJumpPad");
                pad.transform.position = new Vector3(7f, 0.05f, -9f);
                pad.AddComponent<Ziptide.Gameplay.JumpPadRuntime>()
                   .Init(new Vector3(12.5f, 5.2f, -9f)); // arcs you onto the tower top
            }
            // A4.5: the augment rack by the Grab zone — all six gems, select to equip (1 active +
            // 1 passive; re-selecting swaps). The belt orb appears at your right hip once any active
            // is equipped — touch it with the ray to FIRE.
            AugmentAuthor.EnsureAllAuthored();
            string[] augs = { "augment_surge_dash", "augment_bubble_guard", "augment_overclock",
                              "augment_magnet_palm", "augment_sure_step", "augment_sixth_sense" };
            for (int i = 0; i < augs.Length; i++)
            {
                if (GameObject.Find(augs[i]) != null) continue;
                Ziptide.Gameplay.ItemFactory.Create(augs[i], new Vector3(-12f, 1.15f, 6f + i * 0.8f));
            }

            if (GameObject.Find("SandboxGrapple") == null)
            {
                // 1.4g: the FOURTH way up — point the ray at the rose ring high on the tower's south
                // shoulder and grip: the reel pulls you up (in range from the whole corner, ~14m).
                var anchor = new GameObject("SandboxGrapple");
                anchor.transform.position = new Vector3(12.5f, 5.4f, -11.2f);
                anchor.AddComponent<Ziptide.Gameplay.GrappleAnchorRuntime>();
            }
        }

        private static void EnsureGravityGunDef()
        {
            var def = AssetDatabase.LoadAssetAtPath<GravityGunDefinition>(GravityGunDefPath);
            if (def == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Resources"))
                    AssetDatabase.CreateFolder("Assets/Ziptide", "Resources");
                if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Resources/Items"))
                    AssetDatabase.CreateFolder("Assets/Ziptide/Resources", "Items");
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

        /// <summary>
        /// The sandbox was created as an empty scene, so (unlike the world scenes) it had no
        /// EventSystem — meaning XR-ray UI clicks (the Dev Menu) died the moment you warped in. Add an
        /// EventSystem + XRUIInputModule so world-space UI is clickable here too.
        /// </summary>
        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var go = PatcherUtil.EnsureRootObject("EventSystem", Vector3.zero);
            PatcherUtil.EnsureComponent<UnityEngine.EventSystems.EventSystem>(go);
            PatcherUtil.EnsureComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(go);
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

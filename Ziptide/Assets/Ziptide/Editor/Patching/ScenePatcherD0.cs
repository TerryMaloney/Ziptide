using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Editor.Setup;
using Ziptide.Gameplay;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Idempotent scene patcher for D0 City: ensures D0_City scene exists, blockout (plaza, terraces, alley, railings), and runtime objects (JobDirector, DispatchKiosk, ObjectiveBoard, DeliveryCradle). Call from BuildAndroid or menu.
    /// </summary>
    public static class ScenePatcherD0
    {
        private const string D0ScenePath = "Assets/Ziptide/Scenes/D0_City.unity";
        private const string D0SceneName = "D0_City";

        /// <summary>
        /// Ensures D0_City.unity exists and is in Build Settings. Creates minimal scene if missing. Does not open the scene.
        /// </summary>
        public static void EnsureD0SceneExists()
        {
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(D0ScenePath);
            if (asset != null) return;

            EnsureScenesFolder();
            CreateMinimalD0Scene();
        }

        [MenuItem("Ziptide/Apply D0 City To Current Scene")]
        public static void PatchActiveSceneFromMenu()
        {
            PatchActiveScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Ziptide] D0 applied to current scene (if D0_City). Save scene and build.");
        }

        /// <summary>
        /// Patches the currently open active scene. No-op unless active scene is D0_City.
        /// </summary>
        public static void PatchActiveScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded) return;
            PatchScene(scene);
        }

        /// <summary>
        /// Patches the given scene. No-op unless scene name is D0_City.
        /// </summary>
        public static void PatchScene(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name != D0SceneName) return;
            PatchD0Content();
        }

        private static void EnsureScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide"))
                CreateFolderRecursive("Assets", "Ziptide");
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Scenes"))
                CreateFolderRecursive("Assets/Ziptide", "Scenes");
        }

        private static void CreateFolderRecursive(string parentPath, string newFolderName)
        {
            string[] parts = parentPath.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
            if (!AssetDatabase.IsValidFolder(current + "/" + newFolderName))
                AssetDatabase.CreateFolder(current, newFolderName);
        }

        private static void CreateMinimalD0Scene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var mainCam = Object.FindObjectOfType<Camera>();
            if (mainCam != null && mainCam.CompareTag("MainCamera"))
                mainCam.gameObject.SetActive(false);

            // World scenes are content-only. No XR Origin or XRInteractionManager — rig lives in _Boot (DontDestroyOnLoad).

            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Ground";
            plane.transform.position = Vector3.zero;
            plane.transform.localScale = Vector3.one;
            var groundBox = plane.AddComponent<BoxCollider>();
            groundBox.center = new Vector3(0f, -0.5f, 0f);
            groundBox.size = new Vector3(10f, 1f, 10f);

            ApplyWorldProfileToCurrentScene.ApplyWorldProfile();
            AddSceneToBuildSettings(D0ScenePath);
            bool saved = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), D0ScenePath);
            if (!saved)
                Debug.LogError("[Ziptide] Failed to save D0_City scene to " + D0ScenePath);
            else
                Debug.Log("[Ziptide] D0_City scene created: " + D0ScenePath);
        }

        private static GameObject FindAndInstantiateXROrigin()
        {
            string[] guids = AssetDatabase.FindAssets("XR Origin t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("interaction.toolkit") || path.Contains("xr.core-utils") || path.Contains("Interaction.Toolkit") || path.Contains("XR Interaction Toolkit"))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                        return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
            }
            return null;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            string normalized = scenePath.Replace('\\', '/');
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0)
            {
                scenes.Add(new EditorBuildSettingsScene(normalized, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            else if (!scenes[idx].enabled)
            {
                scenes[idx] = new EditorBuildSettingsScene(normalized, true);
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        private static void PatchD0Content()
        {
            ApplyWorldProfileToCurrentScene.ApplyWorldProfile();
            CleanupLegacyBlockout();
            EnsureD0RuntimeObjects();
        }

        private static void CleanupLegacyBlockout()
        {
            var old = GameObject.Find("D0_Blockout");
            if (old != null) Object.DestroyImmediate(old);
        }

        private static void EnsureD0RuntimeObjects()
        {
            EnsureContentFolders();
            var worldPack = EnsureD0WorldPackAsset();

            EnsureJobDirector(worldPack);
            EnsureDispatchKiosk();
            EnsureObjectiveBoard();
            EnsureDeliveryCradle();
            EnsureWorldTravelStationInD0();
        }

        private const string PacksFolder = "Assets/Ziptide/Content/Worlds/Packs";
        private const string D0WorldPackPath = PacksFolder + "/D0_WorldPack.asset";
        private const string TestRoomWorldPackPath = PacksFolder + "/TestRoom_WorldPack.asset";

        private static void EnsureContentFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                CreateFolderRecursive("Assets/Ziptide", "Content");
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content/Worlds"))
                CreateFolderRecursive("Assets/Ziptide/Content", "Worlds");
            if (!AssetDatabase.IsValidFolder(PacksFolder))
                CreateFolderRecursive("Assets/Ziptide/Content/Worlds", "Packs");
        }

        public static WorldPackDefinition EnsureD0WorldPackAsset()
        {
            var existing = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(D0WorldPackPath);
            if (existing != null)
            {
                var so = new SerializedObject(existing);
                PatcherUtil.SetString(so, "packId", "d0_city");
                PatcherUtil.SetString(so, "displayName", "Toxic City");
                PatcherUtil.SetString(so, "sceneName", "D0_City");

                var audioProfile = ScenePatcherD2.EnsureAudioProfileAsset();
                if (audioProfile != null)
                    PatcherUtil.SetObjectRef(so, "audioProfile", audioProfile);

                so.ApplyModifiedPropertiesWithoutUndo();
                return existing;
            }

            EnsureContentFolders();

            var pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
            pack.packId = "d0_city";
            pack.displayName = "Toxic City";
            pack.sceneName = "D0_City";
            AssetDatabase.CreateAsset(pack, D0WorldPackPath);
            AssetDatabase.SaveAssets();
            return pack;
        }

        private static void EnsureJobDirector(WorldPackDefinition worldPack)
        {
            var go = PatcherUtil.EnsureRootObject("JobDirector", Vector3.zero);
            var director = PatcherUtil.EnsureComponent<JobDirector>(go);
            var so = new SerializedObject(director);
            PatcherUtil.SetObjectRef(so, "worldPack", worldPack);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureDispatchKiosk()
        {
            var go = PatcherUtil.EnsureRootObject("DispatchKiosk", new Vector3(1f, 3.7f, 1f));
            PatcherUtil.EnsureComponent<DispatchKiosk>(go);
            PatcherUtil.EnsureComponent<XRSimpleInteractable>(go);
            var col = go.GetComponent<Collider>();
            if (col == null)
            {
                col = go.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }
        }

        private static void EnsureObjectiveBoard()
        {
            var go = PatcherUtil.EnsureRootObject("ObjectiveBoard", new Vector3(-1f, 4.0f, 1.5f));
            PatcherUtil.EnsureComponent<ObjectiveBoard>(go);
        }

        private static void EnsureDeliveryCradle()
        {
            var go = PatcherUtil.EnsureRootObject("DeliveryCradle", new Vector3(1.5f, 3.5f, 1f));
            if (go.GetComponent<DeliveryCradleSocketInteractor>() == null)
            {
                var socket = go.AddComponent<DeliveryCradleSocketInteractor>();
                var attach = PatcherUtil.EnsureChild(go.transform, "Attach", Vector3.zero);
                var so = new SerializedObject(socket);
                PatcherUtil.SetObjectRef(so, "m_AttachTransform", attach);
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            var col = go.GetComponent<SphereCollider>();
            if (col == null)
            {
                col = go.AddComponent<SphereCollider>();
                col.isTrigger = true;
                col.radius = 0.2f;
            }
        }

        private static void EnsureWorldTravelStationInD0()
        {
            var testRoomPack = EnsureTestRoomWorldPackAsset();
            if (testRoomPack == null) return;

            // Position 2m AHEAD of spawn (0,2.6,-12) so player faces it immediately on arrival.
            var go = PatcherUtil.EnsureRootObject("WorldTravelStation", new Vector3(0f, 2.6f, -10f));
            var station = PatcherUtil.EnsureComponent<WorldTravelStation>(go);

            var so = new SerializedObject(station);
            var listProp = so.FindProperty("destinationPacks");
            if (listProp != null)
            {
                listProp.arraySize = 1;
                listProp.GetArrayElementAtIndex(0).objectReferenceValue = testRoomPack;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Failsafe: walk-through trigger so the return door works even when rays break.
            EnsureProximityTrigger(go, testRoomPack.sceneName,
                new Vector3(0f, 1.1f, 0f), new Vector3(1.2f, 2.4f, 0.6f));
        }

        private static void EnsureProximityTrigger(GameObject parent, string destinationScene, Vector3 localOffset, Vector3 size)
        {
            const string triggerName = "__ProximityTravelTrigger";
            var existing = parent.transform.Find(triggerName);
            GameObject triggerGo;
            if (existing != null)
            {
                triggerGo = existing.gameObject;
            }
            else
            {
                triggerGo = new GameObject(triggerName);
                triggerGo.transform.SetParent(parent.transform, false);
                Undo.RegisterCreatedObjectUndo(triggerGo, triggerName);
            }
            triggerGo.transform.localPosition = localOffset;

            var col = PatcherUtil.EnsureComponent<BoxCollider>(triggerGo);
            col.size = size;
            col.isTrigger = true;

            var trigger = PatcherUtil.EnsureComponent<Ziptide.Gameplay.ProximityTravelTrigger>(triggerGo);
            var soTrigger = new SerializedObject(trigger);
            PatcherUtil.SetString(soTrigger, "destinationSceneName", destinationScene);
            PatcherUtil.SetFloat(soTrigger, "cooldownSeconds", 3f);
            soTrigger.ApplyModifiedPropertiesWithoutUndo();
        }

        private static WorldPackDefinition EnsureTestRoomWorldPackAsset()
        {
            var existing = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(TestRoomWorldPackPath);
            var scenes = EditorBuildSettings.scenes;
            string firstSceneName = null;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (!scenes[i].enabled) continue;
                string path = scenes[i].path;
                if (string.IsNullOrEmpty(path)) continue;
                int lastSlash = path.LastIndexOf('/');
                int dot = path.LastIndexOf('.');
                if (lastSlash >= 0 && dot > lastSlash)
                    firstSceneName = path.Substring(lastSlash + 1, dot - lastSlash - 1);
                break;
            }
            if (string.IsNullOrEmpty(firstSceneName))
                firstSceneName = "MilestoneA_GrabCube";

            if (existing != null)
            {
                var so = new SerializedObject(existing);
                PatcherUtil.SetString(so, "packId", "test_room");
                PatcherUtil.SetString(so, "displayName", "Test Room");
                PatcherUtil.SetString(so, "sceneName", firstSceneName);
                so.ApplyModifiedPropertiesWithoutUndo();
                return existing;
            }
            EnsureContentFolders();

            var pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
            pack.packId = "test_room";
            pack.displayName = "Test Room";
            pack.sceneName = firstSceneName;
            AssetDatabase.CreateAsset(pack, TestRoomWorldPackPath);
            AssetDatabase.SaveAssets();
            return pack;
        }
    }
}

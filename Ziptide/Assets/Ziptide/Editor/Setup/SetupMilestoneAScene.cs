using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Ziptide.Visuals;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Setup
{
    /// <summary>
    /// Creates the Milestone A "grab cube" scene from Cursor so you don't have to click through Unity menus.
    /// Menu: Ziptide > Create Milestone A scene (Grab Cube).
    /// </summary>
    public static class SetupMilestoneAScene
    {
        private const string MenuPath = "Ziptide/Create Milestone A scene (Grab Cube)";
        private const string ScenePath = "Assets/Ziptide/Scenes/MilestoneA_GrabCube.unity";

        [MenuItem(MenuPath)]
        public static void CreateMilestoneAScene()
        {
            // Ensure folder exists
            string dir = Path.GetDirectoryName(ScenePath);
            if (!string.IsNullOrEmpty(dir) && !AssetDatabase.IsValidFolder("Assets/Ziptide"))
            {
                CreateFolderRecursive("Assets", "Ziptide");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Scenes"))
            {
                CreateFolderRecursive("Assets/Ziptide", "Scenes");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Materials"))
            {
                CreateFolderRecursive("Assets/Ziptide", "Materials");
            }

            // URP Lit materials for primitives (avoids missing/incompatible materials on device)
            Material groundMat = GetOrCreateURPMaterial("Assets/Ziptide/Materials/Ground_URP.mat", new Color(0.5f, 0.5f, 0.5f, 1f));
            Material cubeMat = GetOrCreateURPMaterial("Assets/Ziptide/Materials/Cube_URP.mat", new Color(0.2f, 0.5f, 1f, 1f));

            // Create new scene (Editor API; SceneManager.CreateScene is runtime-only)
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Disable default Main Camera so XR Origin owns the view (prevents black screen on device)
            var mainCam = Object.FindObjectOfType<Camera>();
            if (mainCam != null && mainCam.CompareTag("MainCamera"))
                mainCam.gameObject.SetActive(false);

            // Try to find and instantiate XR Origin prefab from package
            GameObject xrOrigin = FindAndInstantiateXROrigin();
            if (xrOrigin != null)
            {
                xrOrigin.name = "XR Origin";
            }
            else
            {
                Debug.LogWarning("[Ziptide] XR Origin prefab not found. Add it manually: GameObject > XR > XR Origin (VR). Creating empty placeholder.");
                xrOrigin = new GameObject("XR Origin (add via GameObject > XR > XR Origin (VR))");
            }

            // XR Interaction Manager (required for grab)
            GameObject managerGo = null;
            if (Object.FindObjectOfType<XRInteractionManager>() == null)
            {
                managerGo = new GameObject("XR Interaction Manager");
                managerGo.AddComponent<XRInteractionManager>();
            }
            else
                managerGo = Object.FindObjectOfType<XRInteractionManager>().gameObject;

            // InputActionManager: enable XRI Default Input Actions at runtime
            var inputManager = managerGo.GetComponent<InputActionManager>();
            if (inputManager == null)
                inputManager = managerGo.AddComponent<InputActionManager>();
            string xriInputActionsPath = AssetDatabase.GUIDToAssetPath("c348712bda248c246b8c49b3db54643f");
            if (!string.IsNullOrEmpty(xriInputActionsPath))
            {
                var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(xriInputActionsPath);
                if (inputAsset != null)
                    inputManager.actionAssets = new List<InputActionAsset> { inputAsset };
                else
                    Debug.LogWarning("[Ziptide] SetupMilestoneAScene: XRI Default Input Actions asset could not be loaded; InputActionManager has no action assets. Import XR Interaction Toolkit Starter Assets and assign them, or grab/select will not work.");
            }
            else
            {
                Debug.LogWarning("[Ziptide] SetupMilestoneAScene: XRI Default Input Actions asset not found by GUID; InputActionManager has no action assets. Import XR Interaction Toolkit Starter Assets, or grab/select will not work.");
            }

            // EventSystem + XRUIInputModule for XR UI (required for ray/UI interaction)
            if (Object.FindObjectOfType<XRUIInputModule>() == null)
            {
                var eventSystem = Object.FindObjectOfType<EventSystem>();
                GameObject eventSystemGo = eventSystem != null ? eventSystem.gameObject : null;
                if (eventSystemGo == null)
                {
                    eventSystemGo = new GameObject("EventSystem");
                    eventSystemGo.AddComponent<EventSystem>();
                }
                if (eventSystemGo.GetComponent<XRUIInputModule>() == null)
                    eventSystemGo.AddComponent<XRUIInputModule>();
            }

            // Ground plane
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Ground";
            plane.transform.position = Vector3.zero;
            plane.transform.localScale = Vector3.one;
            if (groundMat != null)
                plane.GetComponent<Renderer>().sharedMaterial = groundMat;

            // Cube (grabbable) -- placed 1.5m forward at arm height, 25cm size
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "GrabbableCube";
            cube.transform.position = new Vector3(0f, 1f, 1.5f);
            cube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            cube.AddComponent<Rigidbody>();
            cube.AddComponent<XRGrabInteractable>();
            if (cubeMat != null)
                cube.GetComponent<Renderer>().sharedMaterial = cubeMat;

            // World dressing (A.5): default theme + WorldDirector
            EnsureDefaultThemeAndWorldDirector();

            // Save scene
            bool saved = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            if (!saved)
            {
                Debug.LogError("[Ziptide] Failed to save scene to " + ScenePath);
                return;
            }

            // Add to build settings
            AddSceneToBuildSettings(ScenePath);

            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] Milestone A scene created: " + ScenePath + ". Open it and use Build / Build And Run, or run dev_build_install.ps1 from Cursor.");
        }

        private static GameObject FindAndInstantiateXROrigin()
        {
            string[] guids = AssetDatabase.FindAssets("XR Origin t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("interaction.toolkit") || path.Contains("xr.core-utils") || path.Contains("Interaction.Toolkit")
                    || path.Contains("XR Interaction Toolkit"))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        return instance;
                    }
                }
            }
            return null;
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

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
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

        private static Material GetOrCreateURPMaterial(string assetPath, Color baseColor)
        {
            const string urpLitShaderName = "Universal Render Pipeline/Lit";
            const string baseColorProp = "_BaseColor";

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            Shader shader = Shader.Find(urpLitShaderName);
            if (shader == null)
            {
                Debug.LogWarning("[Ziptide] GetOrCreateURPMaterial: URP Lit shader not found. Materials will not be created.");
                return null;
            }
            if (mat == null)
            {
                mat = new Material(shader);
                mat.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                mat.SetColor(baseColorProp, baseColor);
                AssetDatabase.CreateAsset(mat, assetPath);
            }
            else
            {
                mat.SetColor(baseColorProp, baseColor);
            }
            return mat;
        }

        private const string ThemeFolder = "Assets/Ziptide/Content/VisualThemes";
        private const string DefaultThemePath = "Assets/Ziptide/Content/VisualThemes/DefaultVisualTheme.asset";

        private static void EnsureDefaultThemeAndWorldDirector()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                CreateFolderRecursive("Assets/Ziptide", "Content");
            if (!AssetDatabase.IsValidFolder(ThemeFolder))
                CreateFolderRecursive("Assets/Ziptide/Content", "VisualThemes");

            VisualThemeProfile defaultProfile = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(DefaultThemePath);
            if (defaultProfile == null)
            {
                defaultProfile = ScriptableObject.CreateInstance<VisualThemeProfile>();
                defaultProfile.groundTint = new Color(0.42f, 0.5f, 0.45f, 1f);
                var sky = new Gradient();
                sky.SetKeys(
                    new[] { new GradientColorKey(new Color(0.5f, 0.6f, 0.8f), 0f), new GradientColorKey(new Color(0.15f, 0.2f, 0.35f), 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
                defaultProfile.skyGradient = sky;
                defaultProfile.planet.baseColor = new Color(0.4f, 0.5f, 0.7f, 1f);
                defaultProfile.planet.accentColor = new Color(0.25f, 0.35f, 0.5f, 1f);
                defaultProfile.planet.angularSizeDegrees = 15f;
                defaultProfile.planet.distance = 50f;
                defaultProfile.planet.direction = new Vector3(0f, 0.5f, 0.866f).normalized;
                defaultProfile.planet.rotationSpeed = 5f;
                defaultProfile.planet.followPlayer = true;
                AssetDatabase.CreateAsset(defaultProfile, DefaultThemePath);
            }

            if (Object.FindObjectOfType<WorldDirector>() == null)
            {
                var go = new GameObject("WorldDirector");
                var director = go.AddComponent<WorldDirector>();
                var so = new SerializedObject(director);
                so.FindProperty("themeProfile").objectReferenceValue = defaultProfile;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}

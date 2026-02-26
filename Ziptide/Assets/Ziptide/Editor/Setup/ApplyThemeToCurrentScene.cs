using UnityEngine;
using UnityEditor;
using Ziptide.Visuals;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Setup
{
    public static class ApplyThemeToCurrentScene
    {
        private const string MenuPath = "Ziptide/Apply Theme To Current Scene";
        private const string DefaultThemePath = "Assets/Ziptide/Content/VisualThemes/DefaultVisualTheme.asset";

        [MenuItem(MenuPath)]
        public static void ApplyTheme()
        {
            VisualThemeProfile defaultProfile = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(DefaultThemePath);
            if (defaultProfile == null)
            {
                CreateDefaultVisualTheme.CreateDefaultTheme();
                defaultProfile = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(DefaultThemePath);
            }
            if (defaultProfile == null)
            {
                Debug.LogWarning("[Ziptide] Default theme not found at " + DefaultThemePath + ". Run Ziptide > Create Default Visual Theme (Sky + Planet) first.");
                return;
            }

            WorldDirector director = Object.FindObjectOfType<WorldDirector>();
            if (director == null)
            {
                GameObject go = new GameObject("WorldDirector");
                director = go.AddComponent<WorldDirector>();
                SerializedObject so = new SerializedObject(director);
                so.FindProperty("themeProfile").objectReferenceValue = defaultProfile;
                so.ApplyModifiedPropertiesWithoutUndo();
                Undo.RegisterCreatedObjectUndo(go, "Apply Theme To Current Scene");
                Debug.Log("[Ziptide] Added WorldDirector with default theme.");
            }
            else
            {
                SerializedObject so = new SerializedObject(director);
                var prop = so.FindProperty("themeProfile");
                if (prop.objectReferenceValue == null)
                {
                    prop.objectReferenceValue = defaultProfile;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    Debug.Log("[Ziptide] Assigned default theme to existing WorldDirector.");
                }
            }

            EditorUtility.SetDirty(director.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(director.gameObject.scene);
        }
    }
}

using UnityEngine;
using UnityEditor;
using Ziptide.Visuals;

namespace Ziptide.Editor.Setup
{
    public static class CreateDefaultVisualTheme
    {
        private const string MenuPath = "Ziptide/Create Default Visual Theme (Sky + Planet)";
        private const string ThemeFolder = "Assets/Ziptide/Content/VisualThemes";
        private const string DefaultAssetPath = "Assets/Ziptide/Content/VisualThemes/DefaultVisualTheme.asset";

        [MenuItem(MenuPath)]
        public static void CreateDefaultTheme()
        {
            EnsureFolder(ThemeFolder);

            VisualThemeProfile existing = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(DefaultAssetPath);
            if (existing != null)
            {
                Debug.Log("[Ziptide] Default theme already exists: " + DefaultAssetPath);
                Selection.activeObject = existing;
                return;
            }

            VisualThemeProfile profile = ScriptableObject.CreateInstance<VisualThemeProfile>();
            profile.groundTint = new Color(0.42f, 0.5f, 0.45f, 1f);

            Gradient sky = new Gradient();
            sky.SetKeys(
                new[] { new GradientColorKey(new Color(0.5f, 0.6f, 0.8f), 0f), new GradientColorKey(new Color(0.15f, 0.2f, 0.35f), 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            profile.skyGradient = sky;

            profile.planet.baseColor = new Color(0.4f, 0.5f, 0.7f, 1f);
            profile.planet.accentColor = new Color(0.25f, 0.35f, 0.5f, 1f);
            profile.planet.angularSizeDegrees = 15f;
            profile.planet.distance = 50f;
            profile.planet.direction = new Vector3(0f, 0.5f, 0.866f).normalized;
            profile.planet.rotationSpeed = 5f;
            profile.planet.followPlayer = true;

            AssetDatabase.CreateAsset(profile, DefaultAssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] Created default visual theme: " + DefaultAssetPath);
            Selection.activeObject = profile;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    string parent = current;
                    string newFolder = parts[i];
                    AssetDatabase.CreateFolder(parent, newFolder);
                }
                current = next;
            }
        }
    }
}

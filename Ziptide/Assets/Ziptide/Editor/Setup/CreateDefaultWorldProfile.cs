using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Setup
{
    public static class CreateDefaultWorldProfile
    {
        private const string MenuPath = "Ziptide/Create Default World Profile";
        private const string WorldsFolder = "Assets/Ziptide/Content/Worlds";
        private const string DefaultWorldPath = "Assets/Ziptide/Content/Worlds/DefaultWorldProfile.asset";
        private const string ThemeFolder = "Assets/Ziptide/Content/VisualThemes";
        private const string DefaultThemePath = "Assets/Ziptide/Content/VisualThemes/DefaultVisualTheme.asset";

        [MenuItem(MenuPath)]
        public static void CreateDefaultWorld()
        {
            EnsureFolder(ThemeFolder);
            EnsureFolder(WorldsFolder);

            CreateDefaultVisualTheme.CreateDefaultTheme();
            VisualThemeProfile defaultTheme = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(DefaultThemePath);
            if (defaultTheme == null)
            {
                Debug.LogWarning("[Ziptide] Could not load DefaultVisualTheme. Run Create Default Visual Theme first.");
                return;
            }

            WorldProfile existing = AssetDatabase.LoadAssetAtPath<WorldProfile>(DefaultWorldPath);
            if (existing != null)
            {
                Debug.Log("[Ziptide] Default World Profile already exists: " + DefaultWorldPath);
                Selection.activeObject = existing;
                return;
            }

            WorldProfile profile = ScriptableObject.CreateInstance<WorldProfile>();
            profile.spawnPosition = Vector3.zero;
            profile.spawnEuler = Vector3.zero;
            profile.playAreaSize = new Vector2(4f, 4f);
            profile.groundY = 0f;
            profile.respawnOnFall = true;
            profile.fallYThreshold = -2f;
            profile.defaultTheme = defaultTheme;

            List<VisualThemeProfile> available = new List<VisualThemeProfile> { defaultTheme };
            VisualThemeProfile night = GetOrCreateTheme("NightTheme", new Color(0.15f, 0.12f, 0.25f), new Color(0.05f, 0.02f, 0.15f));
            if (night != null) available.Add(night);
            VisualThemeProfile alien = GetOrCreateTheme("AlienTheme", new Color(0.3f, 0.6f, 0.25f), new Color(0.1f, 0.4f, 0.2f));
            if (alien != null) available.Add(alien);
            VisualThemeProfile desert = GetOrCreateTheme("DesertTheme", new Color(0.7f, 0.55f, 0.35f), new Color(0.9f, 0.7f, 0.4f));
            if (desert != null) available.Add(desert);

            profile.availableThemes = available;

            AssetDatabase.CreateAsset(profile, DefaultWorldPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] Created default world profile: " + DefaultWorldPath);
            Selection.activeObject = profile;
        }

        private static VisualThemeProfile GetOrCreateTheme(string name, Color groundTint, Color skyTop)
        {
            string path = ThemeFolder + "/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(path);
            if (existing != null) return existing;

            var theme = ScriptableObject.CreateInstance<VisualThemeProfile>();
            theme.groundTint = groundTint;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(groundTint, 0f), new GradientColorKey(skyTop, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            theme.skyGradient = grad;
            theme.planet.baseColor = new Color(0.5f, 0.4f, 0.6f, 1f);
            theme.planet.accentColor = new Color(0.3f, 0.2f, 0.4f, 1f);
            AssetDatabase.CreateAsset(theme, path);
            return theme;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}

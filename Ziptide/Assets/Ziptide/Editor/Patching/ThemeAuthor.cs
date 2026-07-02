#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors per-world visual assets from a <see cref="CityLayoutDefinition"/>'s sky-theme block:
    /// a <see cref="VisualThemeProfile"/> (sky gradient horizon→top, planet, ground tint) and a
    /// <see cref="WorldProfile"/> (spawn/fall defaults + that theme). The LAYOUT is the source of truth —
    /// regeneration rewrites both assets from the layout fields, so "change the sky of W007" = edit two
    /// colors on its layout asset. Used by WorldStubGenerator; safe to call repeatedly (find-or-create).
    /// </summary>
    public static class ThemeAuthor
    {
        private const string ThemeFolder = "Assets/Ziptide/Content/Worlds/Themes";
        private const string ProfileFolder = "Assets/Ziptide/Content/Worlds/Profiles";

        public static VisualThemeProfile EnsureThemeAsset(CityLayoutDefinition kit) =>
            EnsureThemeAsset(kit.sceneName, kit.skyHorizonColor, kit.skyTopColor, kit.themeGroundTint,
                kit.planetVisible, kit.planetBaseColor, kit.planetAccentColor, kit.planetAngularSize);

        /// <summary>Raw-color overload — arenas (and any future non-city host) author skies through the
        /// same pipeline without needing a CityLayoutDefinition.</summary>
        public static VisualThemeProfile EnsureThemeAsset(string sceneName, Color horizon, Color top,
            Color ground, bool planetVisible, Color planetBase, Color planetAccent, float planetSize)
        {
            string path = ThemeFolder + "/" + sceneName + "_Theme.asset";
            var theme = AssetDatabase.LoadAssetAtPath<VisualThemeProfile>(path);
            if (theme == null)
            {
                Directory.CreateDirectory(ThemeFolder);
                theme = ScriptableObject.CreateInstance<VisualThemeProfile>();
                AssetDatabase.CreateAsset(theme, path);
            }

            theme.groundTint = ground;

            var grad = new Gradient();
            grad.SetKeys(
                new[]
                {
                    new GradientColorKey(horizon, 0f), // t=0 = horizon (SkyPlanetRig samples bottom-up)
                    new GradientColorKey(top, 1f),
                },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            theme.skyGradient = grad;

            if (theme.planet == null) theme.planet = new VisualThemeProfile.PlanetSettings();
            theme.planet.baseColor = planetBase;
            theme.planet.accentColor = planetAccent;
            theme.planet.angularSizeDegrees = planetVisible ? Mathf.Max(1f, planetSize) : 1f;
            // No "off" flag on PlanetSettings — 1° at default distance reads as a dim star when hidden.

            EditorUtility.SetDirty(theme);
            return theme;
        }

        public static WorldProfile EnsureWorldProfileAsset(CityLayoutDefinition kit, VisualThemeProfile theme) =>
            EnsureWorldProfileAsset(kit.sceneName, kit.walkwayHeight, theme);

        /// <summary>Raw overload (see the theme overload above).</summary>
        public static WorldProfile EnsureWorldProfileAsset(string sceneName, float groundY, VisualThemeProfile theme)
        {
            string path = ProfileFolder + "/" + sceneName + "_WorldProfile.asset";
            var profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(path);
            if (profile == null)
            {
                Directory.CreateDirectory(ProfileFolder);
                profile = ScriptableObject.CreateInstance<WorldProfile>();
                AssetDatabase.CreateAsset(profile, path);
            }

            profile.groundY = groundY;
            profile.respawnOnFall = true;
            profile.fallYThreshold = groundY - 3f;   // below any canal/hazard depth
            profile.usePlayAreaBounds = false;       // open spaces — the global fall net handles edges
            profile.defaultTheme = theme;
            if (theme != null && !profile.availableThemes.Contains(theme))
                profile.availableThemes.Add(theme);

            EditorUtility.SetDirty(profile);
            return profile;
        }
    }
}
#endif

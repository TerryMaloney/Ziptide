#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Create-only author (BotProfileAuthor pattern) for the launch BuildingStyleDefinition assets in
    /// Resources/BuildingStyles. Never overwrites — the assets are the editable feel/look truth.
    /// Build-hooked in BuildAndroid; two starter styles cover the Salvage + Toxic Earth families
    /// (Picasso's kits fulfill their module ids per docs/design/ART_REGISTRY.md).
    /// </summary>
    public static class BuildingStyleAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/BuildingStyles";

        [MenuItem("Ziptide/City/Author Building Styles (create-only)")]
        public static void EnsureAllAuthored()
        {
            int made = 0;
            made += Ensure("salvage_row", style =>
            {
                style.surfaceFamily = "Salvage";
                style.maxStoreys = 3;
                style.windowChance = 0.45f;
                style.wallColor = new Color(0.34f, 0.30f, 0.26f);
                style.roofColor = new Color(0.22f, 0.20f, 0.18f);
            });
            made += Ensure("toxic_tenement", style =>
            {
                style.surfaceFamily = "ToxicEarthIndustrial";
                style.minStoreys = 2;
                style.maxStoreys = 3;
                style.windowChance = 0.55f;
                style.rakedRoofChance = 0.1f;
                style.hasInteriors = true; // HARDWIRING 1.3 — the tenements are the interior proof
                style.wallColor = new Color(0.30f, 0.32f, 0.28f);
                style.windowColor = new Color(0.10f, 0.16f, 0.12f);
                style.roofColor = new Color(0.18f, 0.20f, 0.17f);
            });
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            Debug.Log("[Ziptide] BuildingStyleAuthor: " + made + " new style(s); existing untouched.");
        }

        private static int Ensure(string styleId, System.Action<BuildingStyleDefinition> tune)
        {
            string path = Folder + "/" + styleId + ".asset";
            if (AssetDatabase.LoadAssetAtPath<BuildingStyleDefinition>(path) != null) return 0;
            Directory.CreateDirectory(Folder);
            var style = ScriptableObject.CreateInstance<BuildingStyleDefinition>();
            style.styleId = styleId;
            tune(style);
            AssetDatabase.CreateAsset(style, path);
            return 1;
        }
    }
}
#endif

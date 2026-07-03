#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors the garden content (Quality Bar P3) under Resources/Garden — CREATE-ONLY, same contract
    /// as WorldLayoutLibrary: existing assets are the live truth, this only seeds them the first time.
    /// Three starter plants ladder the loop (fast learner → session crop → idle payoff) and the
    /// field_gloves tool is the v1 implicit harvester (GardenService gates harvest on tool function;
    /// dedicated tools arrive with the tool-chest system). Wired into BuildAndroid.
    /// </summary>
    public static class GardenAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Garden";

        [MenuItem("Ziptide/Worlds/Author Garden Content (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAuthored();
            EditorUtility.DisplayDialog("Garden Author",
                made + " asset(s) created under " + Folder + " (existing ones untouched).", "OK");
        }

        public static int EnsureAuthored()
        {
            Directory.CreateDirectory(Folder);
            int made = 0;

            made += Plant("dew_bulb", "dew bulb", 120,
                ("credits", 10), ("spore", 2));          // 2 minutes — teaches the loop in one visit
            made += Plant("rust_fern", "rust fern", 300,
                ("credits", 25), ("mineral", 3));        // one contract's length
            made += Plant("glass_reed", "glass reed", 900,
                ("credits", 60), ("crystal", 2));        // plant, fly away, come back — idle payoff

            made += Gloves();

            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        private static int Plant(string id, string display, double growSeconds,
            params (string resourceId, double amount)[] yield)
        {
            string path = Folder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<PlantDefinition>(path) != null) return 0;
            var p = ScriptableObject.CreateInstance<PlantDefinition>();
            p.id = id;
            p.displayName = display;
            p.growSeconds = growSeconds;
            p.harvestWith = ToolFunction.Harvest;
            foreach (var (resourceId, amount) in yield)
                p.harvestYield.Add(new ResourceCost { resourceId = resourceId, amount = amount });
            AssetDatabase.CreateAsset(p, path);
            Debug.Log("[Ziptide] GardenAuthor authored plant " + path);
            return 1;
        }

        private static int Gloves()
        {
            string path = Folder + "/field_gloves.asset";
            if (AssetDatabase.LoadAssetAtPath<ToolDefinition>(path) != null) return 0;
            var t = ScriptableObject.CreateInstance<ToolDefinition>();
            t.id = "field_gloves";
            t.displayName = "field gloves";
            t.function = ToolFunction.Harvest;
            t.tier = 1;
            t.power = 1f;
            AssetDatabase.CreateAsset(t, path);
            Debug.Log("[Ziptide] GardenAuthor authored tool " + path);
            return 1;
        }
    }
}
#endif

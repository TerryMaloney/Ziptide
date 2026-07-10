#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// DRIVABLE VEHICLES 3.2 — authors the starter fleet under Resources/Vehicles, CREATE-ONLY
    /// (existing assets are the live truth). Pure spec table (the GardenAuthor idiom) so
    /// VehicleCatalogTests audit the fleet without the AssetDatabase. Wire into BuildAndroid
    /// alongside the other authors; the menu covers editor use.
    /// </summary>
    public static class VehicleAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Vehicles";

        public struct VehicleSpec
        {
            public string Id, Display, Biome;
            public VehicleArchetype Archetype;
            public float Cruise, Boost, Hover, Roam;
        }

        /// <summary>The starter fleet — each biome family gets a signature ride.</summary>
        public static VehicleSpec[] VehicleSpecs() => new[]
        {
            new VehicleSpec { Id = "tide_skiff", Display = "tide skiff", Biome = "tideflats",
                Archetype = VehicleArchetype.Skiff, Cruise = 12f, Boost = 1.6f, Hover = 0.5f, Roam = 220f },
            new VehicleSpec { Id = "dune_hoverbike", Display = "dune hoverbike", Biome = "dunes",
                Archetype = VehicleArchetype.Hoverbike, Cruise = 18f, Boost = 2.0f, Hover = 0.8f, Roam = 260f },
            new VehicleSpec { Id = "cavern_crawler", Display = "cavern crawler", Biome = "cavern",
                Archetype = VehicleArchetype.DrillCrawler, Cruise = 7f, Boost = 1.4f, Hover = 0.2f, Roam = 160f },
        };

        [MenuItem("Ziptide/Worlds/Author Vehicles (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAuthored();
            EditorUtility.DisplayDialog("Vehicle Author",
                made + " asset(s) created under " + Folder + " (existing ones untouched).", "OK");
        }

        public static int EnsureAuthored()
        {
            Directory.CreateDirectory(Folder);
            int made = 0;
            foreach (var spec in VehicleSpecs())
            {
                string path = Folder + "/" + spec.Id + ".asset";
                if (AssetDatabase.LoadAssetAtPath<VehicleDefinition>(path) != null) continue;
                var v = ScriptableObject.CreateInstance<VehicleDefinition>();
                v.id = spec.Id;
                v.displayName = spec.Display;
                v.biomeId = spec.Biome;
                v.archetype = spec.Archetype;
                v.cruiseSpeed = spec.Cruise;
                v.boostMultiplier = spec.Boost;
                v.hoverHeight = spec.Hover;
                v.roamRadius = spec.Roam;
                AssetDatabase.CreateAsset(v, path);
                Debug.Log("[Ziptide] VehicleAuthor authored " + path);
                made++;
            }
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }
    }
}
#endif

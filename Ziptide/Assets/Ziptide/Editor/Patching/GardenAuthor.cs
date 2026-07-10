#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors the garden content (Quality Bar P3 → GARDEN AAA 4.2b) under Resources/Garden —
    /// CREATE-ONLY, same contract as WorldLayoutLibrary: existing assets are the live truth, this
    /// only seeds them the first time. The catalog is a PURE spec table (the ForgeRecipeLibrary
    /// idiom) so GardenCatalogTests can audit variety/balance without touching the AssetDatabase.
    /// 4.2b: the catalog grows 3 → 24 species across the biomes, laddered from one-visit learners
    /// to overnight prizes (the Roblox-garden retention spread), every yield a REAL economy
    /// resource. `field_gloves` stays the v1 implicit harvester. Wired into BuildAndroid.
    /// </summary>
    public static class GardenAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Garden";

        public struct PlantSpec
        {
            public string Id, Display, Biome;
            public double GrowSeconds;
            public double FreshOverride, OverripeOverride; // 0 = garden defaults
            public (string resourceId, double amount)[] Yield;
            public string[] TendTools; // tool ids that may tend this plant (4.2c hands layer)
        }

        // 4.2c: every plant answers to the watering can; longer crops also reward pruning.
        private static readonly string[] TendWater = { "watering_can" };
        private static readonly string[] TendWaterPrune = { "watering_can", "prune_snips" };

        private static PlantSpec Spec(string id, string display, string biome, double grow,
            (string, double)[] yield, double fresh = 0, double overripe = 0)
            => new PlantSpec { Id = id, Display = display, Biome = biome, GrowSeconds = grow,
                Yield = yield, FreshOverride = fresh, OverripeOverride = overripe,
                TendTools = grow > 600 ? TendWaterPrune : TendWater };

        /// <summary>The whole catalog as pure data — tests audit THIS; EnsureAuthored bakes it.</summary>
        public static PlantSpec[] PlantSpecs() => new[]
        {
            // ── The original ladder (ids unchanged — live assets stay the truth) ──
            Spec("dew_bulb", "dew bulb", "", 120,
                new[] { ("credits", 10.0), ("spore", 2.0) }, fresh: 60),          // teaches the loop in one visit
            Spec("rust_fern", "rust fern", "", 300,
                new[] { ("credits", 25.0), ("mineral", 3.0) }),                   // one contract's length
            Spec("glass_reed", "glass reed", "", 900,
                new[] { ("credits", 60.0), ("crystal", 2.0) }),                   // plant, fly away, come back

            // ── Dunes — dry heat; salt and mirage flora ──
            Spec("sun_brittle", "sun brittle", "dunes", 240,
                new[] { ("credits", 18.0), ("salt", 2.0) }, fresh: 60),
            Spec("bone_melon", "bone melon", "dunes", 1800,
                new[] { ("credits", 95.0), ("salt", 4.0) }, overripe: 1800),
            Spec("mirage_pod", "mirage pod", "dunes", 600,
                new[] { ("prism", 1.0), ("spore", 2.0) }),

            // ── Mesas — high sun, mineral veins ──
            Spec("ridge_thistle", "ridge thistle", "mesas", 300,
                new[] { ("mineral", 4.0), ("credits", 15.0) }),
            Spec("ember_root", "ember root", "mesas", 1200,
                new[] { ("crystal", 2.0), ("credits", 45.0) }),
            Spec("sky_lantern_vine", "sky lantern vine", "mesas", 2700,
                new[] { ("prism", 2.0), ("credits", 110.0) }, overripe: 2400),

            // ── Canyon — shade and tangle ──
            Spec("echo_moss", "echo moss", "canyon", 180,
                new[] { ("spore", 3.0) }, fresh: 45),
            Spec("tangle_briar", "tangle briar", "canyon", 900,
                new[] { ("carapace", 2.0), ("credits", 40.0) }),
            Spec("hollow_gourd", "hollow gourd", "canyon", 3600,
                new[] { ("credits", 180.0) }, overripe: 2700),

            // ── Cavern floor — glow flora; the deep prizes ──
            Spec("glow_cap", "glow cap", "cavern", 150,
                new[] { ("spore", 2.0), ("credits", 8.0) }, fresh: 45),
            Spec("crystal_sprout", "crystal sprout", "cavern", 1500,
                new[] { ("crystal", 3.0) }),
            Spec("shade_lotus", "shade lotus", "cavern", 5400,
                new[] { ("memory_shard", 1.0), ("credits", 140.0) }, overripe: 3600),

            // ── Tide flats — brine and foam ──
            Spec("brine_pearl", "brine pearl", "tideflats", 450,
                new[] { ("salt", 3.0), ("credits", 20.0) }),
            Spec("tide_kelp", "tide kelp", "tideflats", 240,
                new[] { ("spore", 2.0), ("salt", 1.0) }, fresh: 60),
            Spec("foam_berry", "foam berry", "tideflats", 750,
                new[] { ("credits", 55.0) }),

            // ── Toxic city — gutter flora that thrives on grime ──
            Spec("sludge_melon", "sludge melon", "", 360,
                new[] { ("credits", 28.0), ("spore", 1.0) }),
            Spec("pipe_ivy", "pipe ivy", "", 600,
                new[] { ("mineral", 3.0), ("credits", 22.0) }),
            Spec("filter_bloom", "filter bloom", "", 1200,
                new[] { ("data_chip", 1.0), ("credits", 35.0) }),

            // ── Exotics — the overnight chase (pair with genetics for the real prizes) ──
            Spec("static_thorn", "static thorn", "mesas", 1000,
                new[] { ("crystal", 1.0), ("spore", 3.0) }),
            Spec("fuel_gourd", "fuel gourd", "dunes", 2400,
                new[] { ("fuel_cell", 1.0), ("credits", 60.0) }),
            Spec("resonant_orchid", "resonant orchid", "cavern", 7200,
                new[] { ("resonator", 1.0), ("credits", 200.0) }, fresh: 300, overripe: 5400),
        };

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
            foreach (var spec in PlantSpecs())
                made += Plant(spec);
            made += Gloves();
            made += Tool("watering_can", "watering can", ToolFunction.Water, 1, 1f);
            made += Tool("prune_snips", "prune snips", ToolFunction.Prune, 1, 1f);
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        private static int Plant(PlantSpec spec)
        {
            string path = Folder + "/" + spec.Id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<PlantDefinition>(path) != null) return 0;
            var p = ScriptableObject.CreateInstance<PlantDefinition>();
            p.id = spec.Id;
            p.displayName = spec.Display;
            p.biomeId = spec.Biome;
            p.growSeconds = spec.GrowSeconds;
            p.freshWindowSecondsOverride = spec.FreshOverride;
            p.overripeAfterSecondsOverride = spec.OverripeOverride;
            p.harvestWith = ToolFunction.Harvest;
            if (spec.TendTools != null)
                p.tendToolIds.AddRange(spec.TendTools);
            foreach (var (resourceId, amount) in spec.Yield)
                p.harvestYield.Add(new ResourceCost { resourceId = resourceId, amount = amount });
            AssetDatabase.CreateAsset(p, path);
            Debug.Log("[Ziptide] GardenAuthor authored plant " + path);
            return 1;
        }

        private static int Tool(string id, string display, ToolFunction function, int tier, float power)
        {
            string path = Folder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<ToolDefinition>(path) != null) return 0;
            var t = ScriptableObject.CreateInstance<ToolDefinition>();
            t.id = id;
            t.displayName = display;
            t.function = function;
            t.tier = tier;
            t.power = power;
            AssetDatabase.CreateAsset(t, path);
            Debug.Log("[Ziptide] GardenAuthor authored tool " + path);
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

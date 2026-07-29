#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// META-LOOP A — seeds a ResourceDefinition for EVERY resource id the game already uses
    /// (CREATE-ONLY, the library contract) into Resources/Economy, plus the first factory recipe
    /// (the golden loop's stun_charge_cell). After this, an unregistered id FAILS the build
    /// (EconomyAuditRules) — future models must add the definition first, on purpose.
    /// </summary>
    public static class EconomyAuthor
    {
        private const string Folder = "Assets/Ziptide/Resources/Economy";
        private const string RecipeFolder = "Assets/Ziptide/Resources/Recipes";

        [MenuItem("Ziptide/Worlds/Author Economy Content (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAuthored();
            EditorUtility.DisplayDialog("Economy Author", made + " asset(s) created (existing untouched).", "OK");
        }

        public static int EnsureAuthored()
        {
            Directory.CreateDirectory(Folder);
            Directory.CreateDirectory(RecipeFolder);
            int n = 0;

            // (id, display, category, rarity, sourceWorlds, storyTags)
            n += Res("credits", "credits", ResourceCategory.Data, 0, "", "guild");
            n += Res("mineral", "mineral", ResourceCategory.Mineral, 0, "W002_DryCistern", "guild_work");
            n += Res("crystal", "crystal", ResourceCategory.Mineral, 1, "W003_GlassShelf", "guild_work");
            n += Res("spore", "spore", ResourceCategory.Bio, 0, "W005_OxidizedCanopy", "canopy");
            n += Res("salt", "salt", ResourceCategory.Mineral, 0, "W010_TidalArray", "tides");
            n += Res("prism", "prism", ResourceCategory.Artifact, 1, "W006_MirrorFlats", "light");
            n += Res("carapace", "carapace", ResourceCategory.Bio, 1, "W009_Chitinwall", "the_wall");
            n += Res("data_chip", "data chip", ResourceCategory.Data, 1, "W008_SealedArchive", "architects");
            n += Res("fuel_cell", "fuel cell", ResourceCategory.Energy, 1, "W007_SableStation", "sable");
            n += Res("resonator", "resonator", ResourceCategory.Salvage, 1, "W011_TheHum", "the_hum");
            n += Res("jump_core", "jump core", ResourceCategory.Artifact, 2, "W012_MarasLastJump", "mara");
            n += Res("memory_shard", "memory shard", ResourceCategory.Artifact, 2, "W004_BroadcastTomb", "transmission");
            // What the Overrun pays. Every pod that missed the catch ends up out there, so scrap is
            // the space leg's currency — disabled ring-tenders and canal caches both grant it, and
            // an unregistered id is a build blocker by design (the one-economy law).
            n += Res("scrap", "scrap", ResourceCategory.Salvage, 0, "SpaceLane_Trial", "the_catch");
            // The first crafted component — the golden loop's output.
            n += Res("stun_charge_cell", "stun charge cell", ResourceCategory.CraftedComponent, 1,
                "W002_DryCistern", "guild_work");

            n += StunCellRecipe();
            if (n > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return n;
        }

        private static int Res(string id, string display, ResourceCategory cat, int rarity,
            string sourceWorld, string storyTag)
        {
            string path = Folder + "/" + id + ".asset";
            if (AssetDatabase.LoadAssetAtPath<ResourceDefinition>(path) != null) return 0;
            var r = ScriptableObject.CreateInstance<ResourceDefinition>();
            r.id = id; r.displayName = display; r.category = cat; r.rarity = rarity;
            if (!string.IsNullOrEmpty(sourceWorld)) r.sourceWorlds.Add(sourceWorld);
            if (!string.IsNullOrEmpty(storyTag)) r.storyTags.Add(storyTag);
            AssetDatabase.CreateAsset(r, path);
            return 1;
        }

        private static int StunCellRecipe()
        {
            string path = RecipeFolder + "/stun_charge_cell.asset";
            if (AssetDatabase.LoadAssetAtPath<RecipeDefinition>(path) != null) return 0;
            var r = ScriptableObject.CreateInstance<RecipeDefinition>();
            r.id = "recipe_stun_charge_cell";
            r.displayName = "stun charge cell";
            r.costs.Add(new ResourceCost { resourceId = "spore", amount = 2 });
            r.costs.Add(new ResourceCost { resourceId = "mineral", amount = 2 });
            r.producesId = "stun_charge_cell";
            r.producesAmount = 1;
            r.requiredMachineType = MachineType.BioRefiner;
            r.durationTicks = 12; // one VR minute at 5s/tick
            r.sourceWorlds.Add("W002_DryCistern");
            r.storyTags.Add("guild_work");
            AssetDatabase.CreateAsset(r, path);
            Debug.Log("[Ziptide] EconomyAuthor authored " + path);
            return 1;
        }
    }
}
#endif

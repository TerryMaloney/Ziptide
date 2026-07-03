#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Editor.Audit
{
    /// <summary>
    /// Art-track audit rules for the Forge (project-wide, run once per audit pass, not per scene):
    /// an item definition pointing at a recipe that doesn't resolve, a recipe that fails validation,
    /// or a recipe over its triangle budget fails the build — a broken look never silently ships as
    /// an invisible gun or a magenta blob.
    /// </summary>
    public static class ForgeAuditRules
    {
        public static void Run(SceneAuditReport report)
        {
            // Item definitions that opted into a forge look must resolve.
            foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition"))
            {
                var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (def == null || string.IsNullOrEmpty(def.forgeRecipeId)) continue;
                var recipe = AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(
                    "Assets/Ziptide/Resources/Forge/" + def.forgeRecipeId + ".asset");
                if (recipe == null)
                    report.Blocker("FORGE_RECIPE_MISSING",
                        "ItemDefinition '" + def.name + "' points at forgeRecipeId='" + def.forgeRecipeId
                        + "' but Resources/Forge has no such recipe. ForgeAuthor/ForgeRecipeLibrary should have seeded it.");
            }

            // Every authored recipe must validate and stay inside its budget.
            foreach (var guid in AssetDatabase.FindAssets("t:ForgeRecipeDefinition"))
            {
                var recipe = AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (recipe == null) continue;
                var issues = recipe.Validate();
                if (issues.Count > 0)
                    report.Blocker("FORGE_RECIPE_INVALID",
                        "Recipe '" + recipe.recipeId + "': " + string.Join(" | ", issues));
                else
                {
                    int tris = ForgeMesh.CountTriangles(recipe);
                    if (tris > recipe.budgetTris)
                        report.Blocker("FORGE_RECIPE_OVER_BUDGET",
                            "Recipe '" + recipe.recipeId + "' builds " + tris + " tris, budget " + recipe.budgetTris + ".");
                }
            }
        }
    }
}
#endif

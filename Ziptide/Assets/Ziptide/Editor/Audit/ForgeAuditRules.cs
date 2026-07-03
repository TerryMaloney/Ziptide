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

            // Builder specs by id (for locked-vs-builder divergence warnings).
            var builderById = new System.Collections.Generic.Dictionary<string, ForgeRecipeDefinition>();
            foreach (var spec in Ziptide.Editor.Patching.ForgeRecipeLibrary.Specs())
                builderById[spec.Key] = null; // built lazily below only when needed

            // Every authored recipe must validate, stay inside its budget, and honor its lifecycle.
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

                // Lifecycle rules (R2). Locked = pinned to the human-baked baseline hash.
                if (recipe.qualityState == ForgeQualityState.Locked
                    && !string.IsNullOrEmpty(recipe.lockedContentHash))
                {
                    if (recipe.ComputeContentHash() != recipe.lockedContentHash)
                        report.Blocker("FORGE_LOCKED_DRIFT",
                            "LOCKED recipe '" + recipe.recipeId + "' changed after approval. Re-lock "
                            + "deliberately (Ziptide > Art > Lock Selected Forge Recipe) or revert.");
                    else if (builderById.ContainsKey(recipe.recipeId))
                    {
                        foreach (var spec in Ziptide.Editor.Patching.ForgeRecipeLibrary.Specs())
                            if (spec.Key == recipe.recipeId)
                            {
                                var built = spec.Value();
                                if (built.ComputeContentHash() != recipe.lockedContentHash)
                                    report.Warning("FORGE_LOCKED_BUILDER_DIVERGED",
                                        "Builder spec for LOCKED '" + recipe.recipeId + "' no longer matches "
                                        + "the approved asset (create-only protects it; review the builder).");
                                break;
                            }
                    }
                }
                if (recipe.qualityState == ForgeQualityState.Deprecated)
                    report.Warning("FORGE_DEPRECATED",
                        "Recipe '" + recipe.recipeId + "' is Deprecated — story no longer uses it; "
                        + "never auto-delete, retire consumers first.");
            }
        }
    }
}
#endif

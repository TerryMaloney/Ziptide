using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// Pure helper for spending a <see cref="RecipeDefinition"/>'s costs against a
    /// <see cref="PlayerProfile"/> inventory — the shared primitive behind build / repair / craft.
    /// No Unity scene refs; fully EditMode-testable. All-or-nothing: a failed spend deducts nothing.
    /// </summary>
    public static class RecipeService
    {
        /// <summary>True if the profile can pay every cost in the recipe. A null / empty recipe is free.</summary>
        public static bool CanAfford(PlayerProfile profile, RecipeDefinition recipe)
        {
            if (recipe == null || recipe.costs == null || recipe.costs.Count == 0) return true;
            if (profile == null) return false;
            for (int i = 0; i < recipe.costs.Count; i++)
            {
                var c = recipe.costs[i];
                if (c == null || string.IsNullOrEmpty(c.resourceId) || c.amount <= 0) continue;
                if (profile.GetResource(c.resourceId) < c.amount) return false;
            }
            return true;
        }

        /// <summary>Deduct the recipe's costs from the profile if (and only if) affordable.
        /// Returns true on success; on failure nothing is deducted.</summary>
        public static bool TrySpend(PlayerProfile profile, RecipeDefinition recipe)
        {
            if (!CanAfford(profile, recipe)) return false;
            if (recipe == null || recipe.costs == null) return true;
            for (int i = 0; i < recipe.costs.Count; i++)
            {
                var c = recipe.costs[i];
                if (c == null || string.IsNullOrEmpty(c.resourceId) || c.amount <= 0) continue;
                profile.AddResource(c.resourceId, -c.amount);
            }
            return true;
        }
    }
}

#if UNITY_EDITOR
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Art
{
    /// <summary>
    /// E5.1 — the TEXTURED building kit (FORGE_II §P5). Upgrades the wall-module registry ids by
    /// WRAPPING the primitive kit's factories (via <see cref="ArtModuleRegistry.TryGetFactory"/>):
    /// the placed module is still the full structured primitive wall (safe to serialize into scenes,
    /// reads right in the editor), now carrying a <see cref="ForgeModuleLook"/> that swaps in the
    /// Forge recipe's baked textured mesh at runtime — the exact ItemFactory/ForgeVisualApplier
    /// pattern, so no gitignored bake is ever referenced by a saved scene.
    ///
    /// Registration order is EXPLICIT: BuildingKitLibrary.EnsureRegistered() calls this at its tail
    /// (last-registration-wins needs deterministic order, and [InitializeOnLoadMethod] order isn't).
    /// Consumers (BuildingBuilder) are untouched; a style with no recipe keeps its primitive factory.
    /// </summary>
    public static class ForgeBuildingKit
    {
        /// <summary>Idempotent: wraps each wall id's current factory once per registration pass
        /// (BuildingKitLibrary re-registers the primitives first, so re-runs never double-wrap).</summary>
        public static void EnsureRegistered()
        {
            WrapWall("salvage_row", window: false);
            WrapWall("salvage_row", window: true);
            WrapWall("toxic_tenement", window: false);
            WrapWall("toxic_tenement", window: true);
        }

        private static void WrapWall(string styleId, bool window)
        {
            string id = "buildingModule:" + styleId + "/" + (window ? "WallWindow" : "WallSolid");
            string recipeId = Ziptide.Editor.Patching.ForgeRecipeLibrary.WallRecipeId(styleId, window);
            if (!ArtModuleRegistry.TryGetFactory(id, out var primitive)) return; // nothing to upgrade

            ArtModuleRegistry.Register(id, () =>
            {
                var go = primitive();
                if (go == null) return null;
                var look = go.AddComponent<ForgeModuleLook>();
                look.recipeId = recipeId;   // Pane stays whitelisted by default — the interior-mapped
                return go;                  // window is a shader feature the bake can't replace.
            });
        }
    }
}
#endif

using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Shared per-color URP/Lit material cache for Forge assets (same semantics as CityBuilder's
    /// Mat(Color) — duplicated by design: CityBuilder is story-lane). Palette colors shared across
    /// recipes collapse into shared materials, so the per-world 25-material budget holds as long as
    /// families share palettes (enforced by the palette-conformance tests).
    /// </summary>
    public static class ForgeMaterials
    {
        private const string URPLitShaderName = "Universal Render Pipeline/Lit";
        private static readonly Dictionary<Color, Material> Cache = new Dictionary<Color, Material>();

        public static Material Mat(Color color)
        {
            if (Cache.TryGetValue(color, out var cached) && cached != null) return cached;
            var shader = Shader.Find(URPLitShaderName);
            if (shader == null) return null;
            var mat = new Material(shader) { name = "ForgeMat_" + ColorUtility.ToHtmlStringRGB(color) };
            mat.SetColor("_BaseColor", color);
            Cache[color] = mat;
            return mat;
        }

        /// <summary>Materials for a recipe's used palette slots, in submesh order (ForgeMesh contract).</summary>
        public static Material[] ForRecipe(ForgeRecipeDefinition recipe)
        {
            var slots = ForgeMesh.UsedPaletteSlots(recipe);
            var mats = new Material[Mathf.Max(1, slots.Count)];
            for (int i = 0; i < slots.Count; i++)
            {
                Color c = recipe.palette != null && slots[i] < recipe.palette.Length
                    ? recipe.palette[slots[i]]
                    : Color.magenta; // loud fallback — Validate() should have caught this
                mats[i] = Mat(c);
            }
            if (slots.Count == 0) mats[0] = Mat(Color.magenta);
            return mats;
        }
    }
}

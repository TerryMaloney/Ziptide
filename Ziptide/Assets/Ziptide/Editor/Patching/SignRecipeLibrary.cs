#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.7 create-only recipe book for the three physical sign bodies. Glyphs are a
    /// separate shared emissive surface so destination hue/seed can vary without material instances
    /// or duplicate body recipes.
    /// </summary>
    public static class SignRecipeLibrary
    {
        public const string HangingRecipeId = "sign_shell_hanging";
        public const string WallPlateRecipeId = "sign_shell_wall_plate";
        public const string ChevronRecipeId = "sign_shell_route_chevron";
        public const int SignTriangleBudget = 400;

        public static List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>> Specs()
        {
            return new List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>>
            {
                Spec(HangingRecipeId, BuildHanging),
                Spec(WallPlateRecipeId, BuildWallPlate),
                Spec(ChevronRecipeId, BuildChevron),
            };
        }

        [MenuItem("Ziptide/Art/Author Shell Sign Recipes (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog(
                "Shell Sign Recipes",
                made + " recipe asset(s) created under " + ForgeRecipeLibrary.RecipeFolder +
                " (existing assets untouched).",
                "OK");
        }

        public static int EnsureAllAuthored()
        {
            int made = 0;
            Directory.CreateDirectory(ForgeRecipeLibrary.RecipeFolder);
            foreach (var spec in Specs())
            {
                string path = ForgeRecipeLibrary.RecipeFolder + "/" + spec.Key + ".asset";
                if (AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(path) != null) continue;
                AssetDatabase.CreateAsset(spec.Value(), path);
                Debug.Log("[Ziptide] SignRecipeLibrary authored " + path);
                made++;
            }
            if (made > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return made;
        }

        private static KeyValuePair<string, System.Func<ForgeRecipeDefinition>> Spec(
            string id,
            System.Func<ForgeRecipeDefinition> builder)
        {
            return new KeyValuePair<string, System.Func<ForgeRecipeDefinition>>(id, builder);
        }

        private static ForgeRecipeDefinition BuildHanging()
        {
            var d = NewSign(
                HangingRecipeId,
                "Suspended Shell-script sign with a worn framed panel and two hanging straps.");
            d.parts = new[]
            {
                Part("Panel", new Vector3(0.90f, 0.38f, 0.07f), Vector3.zero, 0, 0.018f),
                Part("TopRail", new Vector3(0.98f, 0.08f, 0.10f), new Vector3(0f, 0.19f, 0f), 1, 0.012f),
                Part("BottomRail", new Vector3(0.98f, 0.06f, 0.09f), new Vector3(0f, -0.19f, 0f), 1, 0.010f),
                Part("Strap", new Vector3(0.055f, 0.30f, 0.045f), new Vector3(0.31f, 0.38f, 0f), 1, 0.008f, true),
                new ForgePart
                {
                    name = "TopHook",
                    op = ForgeOp.Torus,
                    segments = 8,
                    size = new Vector3(0.34f, 0.035f, 0.02f),
                    position = new Vector3(0f, 0.55f, 0f),
                    eulerRotation = new Vector3(90f, 0f, 0f),
                    paletteSlot = 2,
                },
            };
            return d;
        }

        private static ForgeRecipeDefinition BuildWallPlate()
        {
            var d = NewSign(
                WallPlateRecipeId,
                "Low-profile Shell-script wall plate for jobs, caches and local district identity.");
            d.parts = new[]
            {
                Part("Panel", new Vector3(0.68f, 0.30f, 0.055f), Vector3.zero, 0, 0.015f),
                Part("TopFrame", new Vector3(0.74f, 0.055f, 0.075f), new Vector3(0f, 0.165f, 0f), 1, 0.008f),
                Part("BottomFrame", new Vector3(0.74f, 0.055f, 0.075f), new Vector3(0f, -0.165f, 0f), 1, 0.008f),
                Part("SideFrame", new Vector3(0.055f, 0.28f, 0.075f), new Vector3(0.365f, 0f, 0f), 1, 0.008f, true),
                new ForgePart
                {
                    name = "BoltRow",
                    op = ForgeOp.GreebleStrip,
                    segments = 4,
                    size = new Vector3(0.03f, 0.03f, 0.54f),
                    position = new Vector3(0f, -0.13f, 0.06f),
                    eulerRotation = new Vector3(0f, 90f, 0f),
                    paletteSlot = 2,
                },
            };
            return d;
        }

        private static ForgeRecipeDefinition BuildChevron()
        {
            var d = NewSign(
                ChevronRecipeId,
                "Compact route chevron: two armored strokes around a small mounting block.");
            d.parts = new[]
            {
                Part("Mount", new Vector3(0.28f, 0.32f, 0.07f), Vector3.zero, 0, 0.012f),
                new ForgePart
                {
                    name = "UpperStroke",
                    op = ForgeOp.BeveledBox,
                    bevel = 0.012f,
                    size = new Vector3(0.36f, 0.085f, 0.09f),
                    position = new Vector3(0.08f, 0.10f, 0.04f),
                    eulerRotation = new Vector3(0f, 0f, -35f),
                    paletteSlot = 2,
                },
                new ForgePart
                {
                    name = "LowerStroke",
                    op = ForgeOp.BeveledBox,
                    bevel = 0.012f,
                    size = new Vector3(0.36f, 0.085f, 0.09f),
                    position = new Vector3(0.08f, -0.10f, 0.04f),
                    eulerRotation = new Vector3(0f, 0f, 35f),
                    paletteSlot = 2,
                },
            };
            return d;
        }

        private static ForgeRecipeDefinition NewSign(string id, string role)
        {
            var d = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            d.name = id;
            d.recipeId = id;
            d.surfaceFamily = ForgePalettes.FamilyToxicIndustrial;
            d.storyTags = new[] { "prop", "sign" };
            d.palette = new[]
            {
                new Color(0.30f, 0.32f, 0.28f),
                new Color(0.24f, 0.23f, 0.22f),
                new Color(0.42f, 0.34f, 0.24f),
            };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.55f, grime = 0.50f, panelDensity = 2f },
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.65f, grime = 0.55f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.45f, grime = 0.25f },
            };
            d.budgetTris = SignTriangleBudget;
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = role;
            d.storyRefs = new[] { "shell_script", "wayfinding" };
            d.worldRuleRefs = new[] { "generated_worlds" };
            d.tokenRefs = new[] { "signage", "diegetic_wayfinding", "toxic_industrial" };
            d.sockets = new ForgeSocket[0];
            return d;
        }

        private static ForgePart Part(
            string name,
            Vector3 size,
            Vector3 position,
            int paletteSlot,
            float bevel,
            bool mirrorX = false)
        {
            return new ForgePart
            {
                name = name,
                op = ForgeOp.BeveledBox,
                size = size,
                position = position,
                paletteSlot = paletteSlot,
                bevel = bevel,
                mirrorX = mirrorX,
            };
        }
    }
}
#endif

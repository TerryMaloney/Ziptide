#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// FORGE III F3.7 create-only catalog for three physical sign bodies plus three shared glyph
    /// texture/material pairs. The bodies remain Forge assets; the glyph surfaces are opaque unlit
    /// panels, so signs need no runtime Texture2D/Material allocation and no material instance per sign.
    /// </summary>
    public static class SignRecipeLibrary
    {
        public const string HangingRecipeId = "sign_shell_hanging";
        public const string WallPlateRecipeId = "sign_shell_wall_plate";
        public const string ChevronRecipeId = "sign_shell_route_chevron";
        public const int SignTriangleBudget = 400;

        public const string GlyphFolder = "Assets/Ziptide/Resources/Signage";
        public const int GlyphTextureSize = 64;

        public static List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>> Specs()
        {
            return new List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>>
            {
                Spec(HangingRecipeId, BuildHanging),
                Spec(WallPlateRecipeId, BuildWallPlate),
                Spec(ChevronRecipeId, BuildChevron),
            };
        }

        [MenuItem("Ziptide/Art/Author Shell Sign Assets (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog(
                "Shell Sign Assets",
                made + " create-only recipe/glyph asset(s) authored. Existing assets were untouched.",
                "OK");
        }

        public static int EnsureAllAuthored()
        {
            int made = 0;
            Directory.CreateDirectory(ForgeRecipeLibrary.RecipeFolder);
            Directory.CreateDirectory(GlyphFolder);

            foreach (var spec in Specs())
            {
                string path = ForgeRecipeLibrary.RecipeFolder + "/" + spec.Key + ".asset";
                if (AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(path) != null) continue;
                AssetDatabase.CreateAsset(spec.Value(), path);
                Debug.Log("[Ziptide] SignRecipeLibrary authored " + path);
                made++;
            }

            foreach (SignDestinationClass destination in System.Enum.GetValues(typeof(SignDestinationClass)))
                made += EnsureGlyphAssets(destination);

            if (made > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return made;
        }

        public static string GlyphTexturePath(SignDestinationClass destination)
        {
            return GlyphFolder + "/shell_glyph_" + destination.ToString().ToLowerInvariant() + ".asset";
        }

        public static string GlyphMaterialPath(SignDestinationClass destination)
        {
            return GlyphFolder + "/shell_glyph_" + destination.ToString().ToLowerInvariant() + ".mat";
        }

        /// <summary>Opaque dark panel with the destination-class hue baked into the Shell script.</summary>
        public static Color32[] BuildPanelPixels(SignDestinationClass destination)
        {
            byte[] alpha = ShellGlyphBaker.BakeAlpha(
                GlyphTextureSize,
                SeedFor(destination),
                destination);
            Color hue = HueFor(destination);
            var pixels = new Color32[alpha.Length];
            Color dark = new Color(0.018f, 0.026f, 0.024f, 1f);
            for (int i = 0; i < alpha.Length; i++)
            {
                float t = alpha[i] / 255f;
                Color color = Color.Lerp(dark, hue, Mathf.SmoothStep(0f, 1f, t));
                color.a = 1f;
                pixels[i] = color;
            }
            return pixels;
        }

        public static Color HueFor(SignDestinationClass destination)
        {
            string hex;
            switch (destination)
            {
                case SignDestinationClass.Travel:
                    hex = ZiptideConstants.SignHueTravelHex;
                    break;
                case SignDestinationClass.Job:
                    hex = ZiptideConstants.SignHueJobHex;
                    break;
                default:
                    hex = ZiptideConstants.SignHueVendorHex;
                    break;
            }

            if (ColorUtility.TryParseHtmlString("#" + hex, out Color color)) return color;
            return Color.magenta;
        }

        private static int EnsureGlyphAssets(SignDestinationClass destination)
        {
            int made = 0;
            string texturePath = GlyphTexturePath(destination);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
            {
                texture = new Texture2D(
                    GlyphTextureSize,
                    GlyphTextureSize,
                    TextureFormat.RGBA32,
                    true,
                    true)
                {
                    name = "ShellGlyph_" + destination,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
                texture.SetPixels32(BuildPanelPixels(destination));
                texture.Apply(true, false);
                AssetDatabase.CreateAsset(texture, texturePath);
                Debug.Log("[Ziptide] SignRecipeLibrary authored " + texturePath);
                made++;
            }

            string materialPath = GlyphMaterialPath(destination);
            if (AssetDatabase.LoadAssetAtPath<Material>(materialPath) == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Unlit/Texture");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                if (shader == null)
                {
                    Debug.LogWarning("[Ziptide] Shell sign material skipped: no supported unlit shader.");
                    return made;
                }

                var material = new Material(shader)
                {
                    name = "ShellGlyph_" + destination,
                    enableInstancing = true
                };
                if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
                if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
                if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", Color.white);
                if (material.HasProperty("_Color")) material.SetColor("_Color", Color.white);
                AssetDatabase.CreateAsset(material, materialPath);
                Debug.Log("[Ziptide] SignRecipeLibrary authored " + materialPath);
                made++;
            }

            return made;
        }

        private static int SeedFor(SignDestinationClass destination)
        {
            return 7103 + (int)destination * 977;
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

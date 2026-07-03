#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Code-authored <see cref="ForgeRecipeDefinition"/> assets — THE STUDIO'S RECIPE BOOK. This is
    /// the file an LLM session edits when Terry prompts for an asset ("make the taser a beat-up
    /// salvage pistol"): add/edit a Build* method, push, and the forge-photos workflow renders
    /// turnarounds to iterate against. CREATE-ONLY like SkyVistaLibrary: an existing asset under
    /// Resources/Forge is the live, editable truth; this library only seeds it the first time
    /// (delete the asset to reseed from code). Loop + prompt grammar:
    /// docs/project_art_plan/FORGE_STUDIO_GUIDE.md.
    /// </summary>
    public static class ForgeRecipeLibrary
    {
        public const string RecipeFolder = "Assets/Ziptide/Resources/Forge";

        [MenuItem("Ziptide/Art/Author Forge Recipes (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("Forge Recipe Library",
                made + " recipe asset(s) created under " + RecipeFolder + " (existing ones untouched).", "OK");
        }

        /// <summary>recipeId → builder, the studio's full catalog. Pure; test- and booth-inspectable.</summary>
        public static List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>> Specs()
        {
            return new List<KeyValuePair<string, System.Func<ForgeRecipeDefinition>>>
            {
                Spec("taser_gun_mk1", BuildTaserGunMk1),
            };
        }

        /// <summary>Create any missing recipe assets (build-hooked later via ForgeAuthor). Returns count.</summary>
        public static int EnsureAllAuthored()
        {
            int made = 0;
            foreach (var spec in Specs())
            {
                string path = RecipeFolder + "/" + spec.Key + ".asset";
                if (AssetDatabase.LoadAssetAtPath<ForgeRecipeDefinition>(path) != null) continue;
                Directory.CreateDirectory(RecipeFolder);
                var def = spec.Value();
                AssetDatabase.CreateAsset(def, path);
                Debug.Log("[Ziptide] ForgeRecipeLibrary authored " + path);
                made++;
            }
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        private static KeyValuePair<string, System.Func<ForgeRecipeDefinition>> Spec(
            string id, System.Func<ForgeRecipeDefinition> builder)
            => new KeyValuePair<string, System.Func<ForgeRecipeDefinition>>(id, builder);

        // ── Recipes ────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The taser: a field-repaired salvage-yard stun pistol (Cal's starter tool). Drill-style
        /// grip ~110°, boxy receiver, short barrel with a teal muzzle ring, coil greebles on top,
        /// glowing teal charge windows on the flanks. Barrel along +Z, up +Y (ASSET_SWAP_PIPELINE).
        /// ~22 cm long. Palette: rusted red-brown body, worn gunmetal, tape gray, RILL-teal charge.
        /// </summary>
        private static ForgeRecipeDefinition BuildTaserGunMk1()
        {
            var d = NewRecipe("taser_gun_mk1", ForgePalettes.FamilySalvage,
                new[] { "gear", "handheld", "starter", "stun" },
                new[]
                {
                    new Color(0.42f, 0.22f, 0.14f),  // 0 rusted red-brown body
                    new Color(0.25f, 0.26f, 0.28f),  // 1 worn gunmetal
                    new Color(0.55f, 0.52f, 0.48f),  // 2 salvage tape gray
                    new Color(0.30f, 0.80f, 0.95f),  // 3 charge teal (RILL's color — Cal's tech hums with it)
                },
                budgetTris: 3000);

            d.parts = new[]
            {
                new ForgePart // receiver
                {
                    name = "Receiver", op = ForgeOp.BeveledBox,
                    size = new Vector3(0.036f, 0.05f, 0.13f), bevel = 0.006f,
                    position = new Vector3(0f, 0.01f, -0.01f), paletteSlot = 0
                },
                new ForgePart // barrel forward along +Z
                {
                    name = "Barrel", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.026f, 0.09f, 0.026f),
                    position = new Vector3(0f, 0.012f, 0.095f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1
                },
                new ForgePart // muzzle ring — the teal business end
                {
                    name = "MuzzleRing", op = ForgeOp.Tube, segments = 10, wallThickness = 0.006f,
                    size = new Vector3(0.034f, 0.018f, 0.034f),
                    position = new Vector3(0f, 0.012f, 0.132f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 3
                },
                new ForgePart // coil housing greebles on top
                {
                    name = "CoilBank", op = ForgeOp.GreebleStrip, segments = 5,
                    size = new Vector3(0.018f, 0.02f, 0.08f),
                    position = new Vector3(0f, 0.045f, 0.005f), paletteSlot = 2
                },
                new ForgePart // charge windows on both flanks (mirrored)
                {
                    name = "ChargeWindow", op = ForgeOp.BeveledBox, bevel = 0.001f,
                    size = new Vector3(0.005f, 0.016f, 0.06f),
                    position = new Vector3(0.02f, 0.014f, 0.005f),
                    mirrorX = true, paletteSlot = 3
                },
                new ForgePart // drill-style grip, ~110° to the barrel
                {
                    name = "Grip", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.03f, 0.085f, 0.04f),
                    position = new Vector3(0f, -0.052f, -0.043f),
                    eulerRotation = new Vector3(-20f, 0f, 0f), paletteSlot = 0
                },
                new ForgePart // trigger nub under the receiver front
                {
                    name = "TriggerBlock", op = ForgeOp.Wedge,
                    size = new Vector3(0.018f, 0.02f, 0.028f),
                    position = new Vector3(0f, -0.018f, 0.02f),
                    eulerRotation = new Vector3(180f, 0f, 0f), paletteSlot = 1
                },
                new ForgePart // rounded back cap
                {
                    name = "BackCap", op = ForgeOp.SphereSection, bevel = 0.55f, segments = 10,
                    size = new Vector3(0.034f, 0.05f, 0.03f), smooth = true,
                    position = new Vector3(0f, 0.01f, -0.078f),
                    eulerRotation = new Vector3(-90f, 0f, 0f), paletteSlot = 0
                },
            };

            d.sockets = new[]
            {
                // +45° X = the Quest controller forward-tilt fix (ASSET_SWAP_PIPELINE.md §4).
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, -0.045f, -0.05f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.012f, 0.145f), localEuler = Vector3.zero },
            };

            return d;
        }

        // ── Builders ───────────────────────────────────────────────────────────────────────────────

        private static ForgeRecipeDefinition NewRecipe(string id, string family, string[] tags, Color[] palette, int budgetTris)
        {
            var d = ScriptableObject.CreateInstance<ForgeRecipeDefinition>();
            d.name = id;
            d.recipeId = id;
            d.surfaceFamily = family;
            d.storyTags = tags;
            d.palette = palette;
            d.budgetTris = budgetTris;
            return d;
        }
    }
}
#endif

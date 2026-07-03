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
                Spec("tox_canal_stalker_01", BuildToxCanalStalker),
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

            // Lifecycle + structured refs (R2): the dependency auditor reads the arrays, never prose.
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Cal's starter stun tool — salvage-tech that hums with RILL's teal.";
            d.storyRefs = new[] { "rill_teal", "salvage_tech", "stun_combat" };
            d.worldRuleRefs = new[] { "ToxicCity" };
            d.tokenRefs = new[] { "rusted_metal", "glow_teal", "panel_lines" };

            // FORGE II styles (parallel to the palette): the texture bake reads these.
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.55f, grime = 0.5f, panelDensity = 3f },  // 0 body
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.35f, grime = 0.3f },                       // 1 gunmetal
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.45f, grime = 0.4f, panelDensity = 2f }, // 2 tape
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, wear = 0.1f, grime = 0f,                            // 3 charge teal
                    emissive = new Color(0.30f, 0.80f, 0.95f), emissiveIntensity = 3f },
            };

            d.parts = new[]
            {
                new ForgePart // receiver
                {
                    name = "Receiver", op = ForgeOp.BeveledBox,
                    size = new Vector3(0.036f, 0.054f, 0.13f), bevel = 0.006f,
                    position = new Vector3(0f, 0.01f, -0.01f), paletteSlot = 0
                },
                new ForgePart // barrel forward along +Z — stubby and thick (v2: was long/thin, read as a toy)
                {
                    name = "Barrel", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.032f, 0.065f, 0.032f),
                    position = new Vector3(0f, 0.012f, 0.085f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1
                },
                new ForgePart // muzzle ring — the teal business end
                {
                    name = "MuzzleRing", op = ForgeOp.Tube, segments = 10, wallThickness = 0.007f,
                    size = new Vector3(0.04f, 0.02f, 0.04f),
                    position = new Vector3(0f, 0.012f, 0.112f),
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
                new ForgePart // drill-style grip, ~114° to the barrel (v2: more rake, longer)
                {
                    name = "Grip", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.03f, 0.09f, 0.04f),
                    position = new Vector3(0f, -0.054f, -0.044f),
                    eulerRotation = new Vector3(-24f, 0f, 0f), paletteSlot = 0
                },
                new ForgePart // trigger nub under the receiver front (v2: bigger, it vanished in photos)
                {
                    name = "TriggerBlock", op = ForgeOp.Wedge,
                    size = new Vector3(0.02f, 0.024f, 0.032f),
                    position = new Vector3(0f, -0.02f, 0.022f),
                    eulerRotation = new Vector3(180f, 0f, 0f), paletteSlot = 1
                },
                new ForgePart // rounded back cap (v2: full ellipsoid half-sunk in the back — the rotated
                              // dome bulged sideways in the v1 turnarounds)
                {
                    name = "BackCap", op = ForgeOp.SphereSection, bevel = 1f, segments = 10,
                    size = new Vector3(0.036f, 0.046f, 0.03f), smooth = true,
                    position = new Vector3(0f, 0.01f, -0.077f), paletteSlot = 0
                },
            };

            d.sockets = new[]
            {
                // +45° X = the Quest controller forward-tilt fix (ASSET_SWAP_PIPELINE.md §4).
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, -0.045f, -0.05f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.012f, 0.125f), localEuler = Vector3.zero },
            };

            return d;
        }

        /// <summary>
        /// Toxic canal ambush predator (reconciliation R4, external-spec example adopted with canon
        /// corrections): feral Bloom-adapted fauna (story tie per CREATURE_DESIGN law) — wet
        /// black-rubber hide, scavenged oxidized-bronze shell plates, teal toxin sacs, filter-gill
        /// front, crab stance. Scan pulse exposes the WeakPoint (gill cluster). STATIC PROXY tier —
        /// the skinned crab-walk body arrives with FORGE II P3; this recipe already carries the
        /// contract (scale, sockets, palette, refs) that body inherits. NOT a xenomorph, NOT a
        /// generic bug: no fangs-and-spikes, its threat reads through the coiled leg stance + glow.
        /// </summary>
        private static ForgeRecipeDefinition BuildToxCanalStalker()
        {
            var d = NewRecipe("tox_canal_stalker_01", ForgePalettes.FamilyToxicIndustrial,
                new[] { "creature", "standard", "ambush" },
                new[]
                {
                    new Color(0.07f, 0.07f, 0.08f),  // 0 wet black rubber hide
                    new Color(0.36f, 0.30f, 0.18f),  // 1 oxidized bronze plates
                    new Color(0.30f, 0.80f, 0.95f),  // 2 toxin-sac teal (the Bloom's tell)
                    new Color(0.42f, 0.40f, 0.36f),  // 3 pale chitin underside
                },
                budgetTris: 2000);

            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "Feral Bloom-adapted canal fauna — ambushes from toxic water; scan pulse exposes its gill cluster.";
            d.storyRefs = new[] { "bloom_fauna", "toxin", "scan_pulse", "stun_combat" };
            d.worldRuleRefs = new[] { "ToxicCity", "W001" };
            d.tokenRefs = new[] { "wet_rubber", "oxidized_bronze", "glow_teal" };

            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Slime, wear = 0f, grime = 0.25f },                    // 0 wet hide
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.6f, grime = 0.5f },             // 1 scavenged plate
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.30f, 0.80f, 0.95f), // 2 toxin sacs
                    emissiveIntensity = 2.5f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.15f, grime = 0.3f },             // 3 underside
            };

            d.parts = new[]
            {
                new ForgePart // rubbery body core
                {
                    name = "Body", op = ForgeOp.SphereSection, bevel = 1f, segments = 12, smooth = true,
                    size = new Vector3(0.6f, 0.3f, 0.85f), position = new Vector3(0f, 0.3f, 0f), paletteSlot = 0
                },
                new ForgePart // bronze carapace dome over the back
                {
                    name = "Carapace", op = ForgeOp.SphereSection, bevel = 0.5f, segments = 12, smooth = true,
                    size = new Vector3(0.66f, 0.34f, 0.8f), position = new Vector3(0f, 0.36f, -0.06f), paletteSlot = 1
                },
                new ForgePart // scavenged industrial plate, crooked on top
                {
                    name = "ScrapPlate", op = ForgeOp.BeveledBox, bevel = 0.008f,
                    size = new Vector3(0.3f, 0.035f, 0.42f), position = new Vector3(0.09f, 0.52f, 0.02f),
                    eulerRotation = new Vector3(4f, 18f, 6f), paletteSlot = 1
                },
                new ForgePart // front leg pair — coiled crab stance (mirrored)
                {
                    name = "LegFront", op = ForgeOp.BeveledBox, bevel = 0.006f,
                    size = new Vector3(0.07f, 0.46f, 0.09f), position = new Vector3(0.34f, 0.22f, 0.24f),
                    eulerRotation = new Vector3(0f, -20f, 55f), mirrorX = true, paletteSlot = 0
                },
                new ForgePart // mid leg pair
                {
                    name = "LegMid", op = ForgeOp.BeveledBox, bevel = 0.006f,
                    size = new Vector3(0.07f, 0.5f, 0.09f), position = new Vector3(0.37f, 0.24f, -0.02f),
                    eulerRotation = new Vector3(0f, 0f, 60f), mirrorX = true, paletteSlot = 0
                },
                new ForgePart // rear leg pair — the spring-lunge coil
                {
                    name = "LegRear", op = ForgeOp.BeveledBox, bevel = 0.006f,
                    size = new Vector3(0.08f, 0.54f, 0.1f), position = new Vector3(0.33f, 0.26f, -0.28f),
                    eulerRotation = new Vector3(0f, 25f, 62f), mirrorX = true, paletteSlot = 0
                },
                new ForgePart // toxin sacs on the flanks (the glowing tell — mirrored)
                {
                    name = "ToxinSac", op = ForgeOp.SphereSection, bevel = 1f, segments = 10, smooth = true,
                    size = new Vector3(0.15f, 0.13f, 0.18f), position = new Vector3(0.27f, 0.33f, -0.08f),
                    mirrorX = true, paletteSlot = 2
                },
                new ForgePart // filter-gill bank across the front (its face IS an air filter)
                {
                    name = "GillBank", op = ForgeOp.GreebleStrip, segments = 6,
                    size = new Vector3(0.09f, 0.07f, 0.3f), position = new Vector3(0f, 0.27f, 0.44f),
                    eulerRotation = new Vector3(0f, 90f, 0f), paletteSlot = 3
                },
                new ForgePart // pale underside plate
                {
                    name = "Underside", op = ForgeOp.SphereSection, bevel = 0.4f, segments = 10, smooth = true,
                    size = new Vector3(0.5f, 0.16f, 0.7f), position = new Vector3(0f, 0.2f, 0f),
                    eulerRotation = new Vector3(180f, 0f, 0f), paletteSlot = 3
                },
            };

            d.sockets = new[]
            {
                // Scan pulse exposes this; taser hits here for the clean disable (creature contract).
                new ForgeSocket { name = "WeakPoint", localPosition = new Vector3(0f, 0.27f, 0.46f) },
                new ForgeSocket { name = "Eye", localPosition = new Vector3(0f, 0.42f, 0.38f) },
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

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
                Spec("pistol_scrap_mk1", BuildPistolScrapMk1),
                Spec("static_net_lobber", BuildStaticNetLobber),
                Spec("sonic_thumper_maul", BuildSonicThumperMaul),
                Spec("prism_beam_rifle", BuildPrismBeamRifle),
                Spec("breaker_blade_mk1", BuildBreakerBlade),
                Spec("tide_pike_mk1", BuildTidePike),
                Spec("p2_tide_totem", BuildP2TideTotem),
                // E5.1 building kit — the TEXTURED wall modules (consumed via ForgeBuildingKit →
                // ArtModuleRegistry; runtime swap by ForgeModuleLook, the item-applier pattern).
                Spec("bldg_salvage_wall_solid", () => BuildWallModule("salvage_row", window: false)),
                Spec("bldg_salvage_wall_window", () => BuildWallModule("salvage_row", window: true)),
                Spec("bldg_tenement_wall_solid", () => BuildWallModule("toxic_tenement", window: false)),
                Spec("bldg_tenement_wall_window", () => BuildWallModule("toxic_tenement", window: true)),
                // E5.3 flora + props (LeafCard/Leaf-alpha assets; scatter consumes the plants via
                // WorldDressingBuilder's tuft holders + ForgeModuleLook).
                Spec(FrondRecipeId, BuildFrondW005),
                Spec(ReedRecipeId, BuildReedW001),
                Spec("prop_patched_crate", BuildPatchedCrate),
                Spec("prop_pipe_cluster", BuildPipeCluster),
                Spec("prop_dispatch_console", BuildDispatchConsole),
            };
        }

        /// <summary>Scatter-consumed plant ids (WorldDressingBuilder + tests share these).</summary>
        public const string FrondRecipeId = "flora_frond_w005";
        public const string ReedRecipeId = "flora_reed_w001";

        /// <summary>Registry id → the recipe that skins it (ForgeBuildingKit + tests share this).</summary>
        public static string WallRecipeId(string styleId, bool window)
            => (styleId == "salvage_row" ? "bldg_salvage_wall_" : "bldg_tenement_wall_")
               + (window ? "window" : "solid");

        /// <summary>
        /// E5.1: one TEXTURED wall module (FORGE_II §P5/E5.1). Matches the primitive kit's canonical
        /// envelope EXACTLY (BuildingKitLibrary: 3.0m wide × 3.2m storey × 0.25m deep, origin at wall
        /// center; window reveal 0.8×0.62 at local y=0.08) so the runtime swap is invisible to layout,
        /// colliders, and the interior-mapped pane. Same silhouette grammar as the primitive kit
        /// (panel + edge ribs + skirt + top band + per-style accent) but as ONE textured mesh:
        /// wear at the ribs, grime settling on the skirt, panel seams on the face — plus P2 ops for
        /// the tenement's plumbing (Capsule standpipe, Frustum vent hood).
        /// </summary>
        private static ForgeRecipeDefinition BuildWallModule(string styleId, bool window)
        {
            bool salvage = styleId == "salvage_row";
            var d = NewRecipe(WallRecipeId(styleId, window), ForgePalettes.FamilyToxicIndustrial,
                new[] { "prop", "buildingModule" },
                salvage
                    ? new[]
                    {
                        new Color(0.34f, 0.30f, 0.26f), // 0 scrap-brown wall
                        new Color(0.24f, 0.23f, 0.22f), // 1 trim
                        new Color(0.42f, 0.34f, 0.24f), // 2 patch-plate accent
                    }
                    : new[]
                    {
                        new Color(0.30f, 0.32f, 0.28f), // 0 mossy grey-green wall
                        new Color(0.24f, 0.23f, 0.22f), // 1 trim
                        new Color(0.20f, 0.26f, 0.20f), // 2 industrial accent
                    },
                budgetTris: 1200);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = (salvage ? "Salvage-row" : "Toxic-tenement") + " wall module ("
                + (window ? "window" : "solid") + ") — the E5.1 textured building kit.";
            d.storyRefs = new[] { "building_kit" };
            d.worldRuleRefs = new[] { "W002_DryCistern", "ToxicCity" };
            d.tokenRefs = new[] { salvage ? "rusted_metal" : "mossy_industrial" };
            d.slotStyles = salvage
                ? new[]
                {
                    new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.55f, grime = 0.5f, panelDensity = 3f },
                    new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.45f, grime = 0.4f },
                    new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.7f, grime = 0.3f },
                }
                : new[]
                {
                    new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.5f, grime = 0.6f, panelDensity = 2f },
                    new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.4f, grime = 0.5f },
                    new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.5f, grime = 0.65f },
                };

            var parts = new List<ForgePart>();
            const float W = 3.0f, H = 3.2f, D = 0.25f;

            if (!window)
            {
                parts.Add(new ForgePart
                {
                    name = "Panel", op = ForgeOp.BeveledBox, bevel = 0.02f,
                    size = new Vector3(W - 0.36f, H - 0.5f, D - 0.06f), paletteSlot = 0,
                });
            }
            else
            {
                // Reveal 0.8 × 0.62 at y = 0.08 — the primitive kit's exact hole, so the
                // interior-mapped pane child lands inside it untouched.
                const float holeW = 0.8f, holeH = 0.62f, holeY = 0.08f;
                float panelH = H - 0.5f;                       // 2.7, spans -1.35..1.35
                float sideW = (W - 0.36f - holeW) * 0.5f;      // 0.92
                float below = panelH * 0.5f + (holeY - holeH * 0.5f); // 1.12
                float above = panelH * 0.5f - (holeY + holeH * 0.5f); // 0.96
                parts.Add(new ForgePart { name = "PanelL", op = ForgeOp.BeveledBox, bevel = 0.02f,
                    size = new Vector3(sideW, panelH, D - 0.06f),
                    position = new Vector3(-(holeW + sideW) * 0.5f, 0f, 0f), paletteSlot = 0 });
                parts.Add(new ForgePart { name = "PanelR", op = ForgeOp.BeveledBox, bevel = 0.02f,
                    size = new Vector3(sideW, panelH, D - 0.06f),
                    position = new Vector3((holeW + sideW) * 0.5f, 0f, 0f), paletteSlot = 0 });
                parts.Add(new ForgePart { name = "PanelB", op = ForgeOp.BeveledBox, bevel = 0.02f,
                    size = new Vector3(holeW, below, D - 0.06f),
                    position = new Vector3(0f, holeY - holeH * 0.5f - below * 0.5f, 0f), paletteSlot = 0 });
                parts.Add(new ForgePart { name = "PanelT", op = ForgeOp.BeveledBox, bevel = 0.02f,
                    size = new Vector3(holeW, above, D - 0.06f),
                    position = new Vector3(0f, holeY + holeH * 0.5f + above * 0.5f, 0f), paletteSlot = 0 });
                parts.Add(new ForgePart { name = "Sill", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(holeW + 0.18f, 0.08f, D + 0.1f),
                    position = new Vector3(0f, holeY - holeH * 0.5f - 0.04f, 0.06f), paletteSlot = 1 });
                parts.Add(new ForgePart { name = "Header", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(holeW + 0.12f, 0.1f, D + 0.04f),
                    position = new Vector3(0f, holeY + holeH * 0.5f + 0.05f, 0.03f), paletteSlot = 1 });
            }

            // Shared shell — ribs (mirrored), skirt, top band: the depth silhouette.
            parts.Add(new ForgePart { name = "Rib", op = ForgeOp.BeveledBox, bevel = 0.015f,
                size = new Vector3(0.18f, H, D + 0.08f),
                position = new Vector3(W * 0.5f - 0.09f, 0f, 0f), mirrorX = true, paletteSlot = 1 });
            parts.Add(new ForgePart { name = "Skirt", op = ForgeOp.BeveledBox, bevel = 0.01f,
                size = new Vector3(W, 0.4f, D + 0.06f),
                position = new Vector3(0f, -(H * 0.5f - 0.2f), 0f), paletteSlot = 1 });
            parts.Add(new ForgePart { name = "Band", op = ForgeOp.BeveledBox, bevel = 0.01f,
                size = new Vector3(W, 0.22f, D + 0.05f),
                position = new Vector3(0f, H * 0.5f - 0.11f, 0f), paletteSlot = 1 });

            if (salvage)
            {
                // Bolted patch plate, slightly skewed — the salvage tell (solid face only).
                if (!window)
                {
                    parts.Add(new ForgePart { name = "PatchPlate", op = ForgeOp.BeveledBox, bevel = 0.008f,
                        size = new Vector3(0.9f, 0.7f, 0.04f),
                        position = new Vector3(0.62f, 0.32f, (D - 0.06f) * 0.5f + 0.02f),
                        eulerRotation = new Vector3(0f, 0f, 4f), paletteSlot = 2 });
                    parts.Add(new ForgePart { name = "BoltRow", op = ForgeOp.GreebleStrip, segments = 5,
                        size = new Vector3(0.05f, 0.05f, 0.8f),
                        position = new Vector3(0.62f, 0.66f, (D - 0.06f) * 0.5f + 0.03f),
                        eulerRotation = new Vector3(0f, 90f, 0f), paletteSlot = 2 });
                }
            }
            else
            {
                // Tenement plumbing — P2 ops earning their keep in architecture:
                // a capsule standpipe up the module edge + a frustum vent hood breathing out.
                parts.Add(new ForgePart { name = "Standpipe", op = ForgeOp.Capsule, segments = 8, smooth = true,
                    size = new Vector3(0.11f, H - 0.15f, 0.11f),
                    position = new Vector3(1.15f, 0f, (D + 0.16f) * 0.5f), paletteSlot = 2 });
                parts.Add(new ForgePart { name = "PipeBracket", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.2f, 0.06f, 0.2f),
                    position = new Vector3(1.15f, 0.8f, D * 0.5f + 0.02f), paletteSlot = 1 });
                if (!window)
                {
                    parts.Add(new ForgePart { name = "VentHood", op = ForgeOp.Frustum, segments = 8, smooth = true,
                        size = new Vector3(0.32f, 0.22f, 0.14f),
                        position = new Vector3(-0.85f, 0.95f, (D - 0.06f) * 0.5f + 0.1f),
                        // +90°X maps the frustum's +Y axis to +Z: wide mouth against the wall,
                        // narrowing OUTWARD (Euler(-90) would point it into the wall).
                        eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1 });
                }
            }

            d.parts = parts.ToArray();
            return d;
        }

        // ── E5.3 FLORA + PROPS ──────────────────────────────────────────────

        /// <summary>
        /// flora_frond_w005 — the W005 Oxidized Canopy frond: three drooping LeafCards (the P2 bend
        /// modifier arcs them like real fronds) around a bark stem, topped by a small glowing spore
        /// nub (the rubric's emissive focal). Scattered by the dressing pass in Canyon biomes.
        /// </summary>
        private static ForgeRecipeDefinition BuildFrondW005()
        {
            var d = NewRecipe(FrondRecipeId, ForgePalettes.FamilySalvage,
                new[] { "plant" },
                new[]
                {
                    new Color(0.30f, 0.22f, 0.14f), // 0 stem — dry bark
                    new Color(0.55f, 0.52f, 0.20f), // 1 fronds — oxidized amber-green
                    new Color(0.95f, 0.65f, 0.25f), // 2 spore nub — amber glow
                },
                budgetTris: 900);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "W005 canopy frond — the dressing pass's Canyon-biome flora.";
            d.storyRefs = new[] { "dressing_scatter" };
            d.worldRuleRefs = new[] { "W005_OxidizedCanopy" };
            d.tokenRefs = new[] { "oxidized_flora" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Bark, wear = 0.3f, grime = 0.35f },
                new ForgeStyleSpec { style = ForgeStyle.Leaf, grime = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.95f, 0.6f, 0.2f), emissiveIntensity = 1.2f },
            };
            d.parts = new[]
            {
                // Photo-loop v2: v1's wide cards curled into an avocado-shaped CLUMP and buried the
                // spore nub. Now a fern read: narrow blades arcing OUTWARD from a visible stem,
                // the glowing nub riding above the crown where a focal belongs.
                new ForgePart { name = "Stem", op = ForgeOp.Capsule, segments = 8, smooth = true,
                    size = new Vector3(0.05f, 0.85f, 0.05f), position = new Vector3(0f, 0.4f, 0f),
                    taper = 0.15f, paletteSlot = 0 },
                new ForgePart { name = "FrondA", op = ForgeOp.LeafCard,
                    size = new Vector3(0.26f, 1.05f, 0f), position = new Vector3(0f, 0.1f, 0f),
                    bendDegrees = 38f, paletteSlot = 1 },
                new ForgePart { name = "FrondB", op = ForgeOp.LeafCard,
                    size = new Vector3(0.24f, 0.95f, 0f), position = new Vector3(0.02f, 0.11f, 0.02f),
                    eulerRotation = new Vector3(0f, 90f, 0f), bendDegrees = 34f, paletteSlot = 1 },
                new ForgePart { name = "FrondC", op = ForgeOp.LeafCard,
                    size = new Vector3(0.22f, 0.88f, 0f), position = new Vector3(-0.02f, 0.12f, -0.01f),
                    eulerRotation = new Vector3(0f, 45f, 0f), bendDegrees = -36f, paletteSlot = 1 },
                new ForgePart { name = "FrondD", op = ForgeOp.LeafCard,
                    size = new Vector3(0.22f, 0.8f, 0f), position = new Vector3(0.01f, 0.12f, -0.02f),
                    eulerRotation = new Vector3(0f, 135f, 0f), bendDegrees = -30f, paletteSlot = 1 },
                new ForgePart { name = "SporeNub", op = ForgeOp.OrganicBlob, segments = 10, smooth = true,
                    size = new Vector3(0.1f, 0.13f, 0.1f), position = new Vector3(0f, 0.88f, 0f),
                    noiseAmplitude = 0.006f, noiseFrequency = 11f, noiseSeed = 17, paletteSlot = 2 },
            };
            return d;
        }

        /// <summary>
        /// flora_reed_w001 — the toxic-canal reed: three tall narrow LeafCards leaning out of a
        /// noised mud clump. No emissive — reeds are background texture, not focal points; the
        /// dressing pass scatters them in TideFlats biomes.
        /// </summary>
        private static ForgeRecipeDefinition BuildReedW001()
        {
            var d = NewRecipe(ReedRecipeId, ForgePalettes.FamilyToxicIndustrial,
                new[] { "plant" },
                new[]
                {
                    new Color(0.22f, 0.20f, 0.16f), // 0 mud clump
                    new Color(0.35f, 0.52f, 0.42f), // 1 reeds — sickly canal teal-green
                },
                budgetTris: 900);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "Toxic-canal reed — the dressing pass's TideFlats-biome flora.";
            d.storyRefs = new[] { "dressing_scatter" };
            d.worldRuleRefs = new[] { "ToxicCity" };
            d.tokenRefs = new[] { "canal_flora" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Stone, grime = 0.5f },
                new ForgeStyleSpec { style = ForgeStyle.Leaf, grime = 0.45f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "MudClump", op = ForgeOp.OrganicBlob, segments = 10, smooth = true,
                    size = new Vector3(0.46f, 0.16f, 0.46f), position = new Vector3(0f, 0.05f, 0f),
                    noiseAmplitude = 0.012f, noiseFrequency = 8f, noiseSeed = 23, paletteSlot = 0 },
                new ForgePart { name = "ReedA", op = ForgeOp.LeafCard,
                    size = new Vector3(0.14f, 1.3f, 0f), position = new Vector3(0f, 0.06f, 0f),
                    eulerRotation = new Vector3(0f, 0f, 4f), bendDegrees = 10f, paletteSlot = 1 },
                new ForgePart { name = "ReedB", op = ForgeOp.LeafCard,
                    size = new Vector3(0.12f, 1.05f, 0f), position = new Vector3(0.12f, 0.06f, 0.05f),
                    eulerRotation = new Vector3(0f, 55f, -6f), bendDegrees = -14f, paletteSlot = 1 },
                new ForgePart { name = "ReedC", op = ForgeOp.LeafCard,
                    size = new Vector3(0.13f, 1.18f, 0f), position = new Vector3(-0.1f, 0.06f, -0.07f),
                    eulerRotation = new Vector3(0f, 115f, 5f), bendDegrees = 8f, paletteSlot = 1 },
            };
            return d;
        }

        /// <summary>prop_patched_crate — the salvage crate: banded, patch-plated, bolted.</summary>
        private static ForgeRecipeDefinition BuildPatchedCrate()
        {
            var d = NewRecipe("prop_patched_crate", ForgePalettes.FamilySalvage,
                new[] { "prop" },
                new[]
                {
                    new Color(0.38f, 0.30f, 0.20f), // 0 crate body
                    new Color(0.22f, 0.22f, 0.24f), // 1 straps
                    new Color(0.45f, 0.28f, 0.16f), // 2 patch plate
                },
                budgetTris: 800);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "Patched salvage crate — the E5.3 dressing prop set.";
            d.storyRefs = new[] { "dressing_scatter" };
            d.worldRuleRefs = new[] { "W002_DryCistern", "ToxicCity" };
            d.tokenRefs = new[] { "rusted_metal" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.5f, grime = 0.45f, panelDensity = 2f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.6f, grime = 0.35f },
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.5f, grime = 0.5f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Body", op = ForgeOp.BeveledBox, bevel = 0.03f,
                    size = new Vector3(0.7f, 0.62f, 0.7f), position = new Vector3(0f, 0.31f, 0f),
                    paletteSlot = 0 },
                new ForgePart { name = "BandLow", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(0.74f, 0.07f, 0.74f), position = new Vector3(0f, 0.16f, 0f),
                    paletteSlot = 1 },
                new ForgePart { name = "BandHigh", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(0.74f, 0.07f, 0.74f), position = new Vector3(0f, 0.48f, 0f),
                    paletteSlot = 1 },
                new ForgePart { name = "PatchPlate", op = ForgeOp.BeveledBox, bevel = 0.008f,
                    size = new Vector3(0.26f, 0.2f, 0.03f), position = new Vector3(0.14f, 0.34f, 0.36f),
                    eulerRotation = new Vector3(0f, 0f, 5f), paletteSlot = 2 },
                new ForgePart { name = "BoltRow", op = ForgeOp.GreebleStrip, segments = 4,
                    size = new Vector3(0.04f, 0.04f, 0.24f), position = new Vector3(0.14f, 0.45f, 0.375f),
                    eulerRotation = new Vector3(0f, 90f, 0f), paletteSlot = 1 },
            };
            return d;
        }

        /// <summary>prop_pipe_cluster — three staggered standpipes with torus flanges and one
        /// glowing gauge (the emissive focal). The tenement wall's plumbing language, freestanding.</summary>
        private static ForgeRecipeDefinition BuildPipeCluster()
        {
            var d = NewRecipe("prop_pipe_cluster", ForgePalettes.FamilyToxicIndustrial,
                new[] { "prop" },
                new[]
                {
                    // (photo v2: 0.35/0.24/0.18 read as glossy chocolate — greyed toward iron)
                    new Color(0.32f, 0.27f, 0.23f), // 0 pipes — rusted iron
                    new Color(0.50f, 0.50f, 0.55f), // 1 flanges — bare steel
                    new Color(0.30f, 0.90f, 0.80f), // 2 gauge — toxic teal glow
                },
                budgetTris: 1400);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "Freestanding pipe cluster — the E5.3 dressing prop set.";
            d.storyRefs = new[] { "dressing_scatter" };
            d.worldRuleRefs = new[] { "ToxicCity" };
            d.tokenRefs = new[] { "rusted_metal" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.5f, grime = 0.6f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.65f, grime = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.3f, 0.9f, 0.8f), emissiveIntensity = 1.4f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "PipeA", op = ForgeOp.Cylinder, segments = 10, smooth = true,
                    size = new Vector3(0.14f, 1.5f, 0.14f), position = new Vector3(0f, 0.75f, 0f),
                    paletteSlot = 0 },
                new ForgePart { name = "PipeB", op = ForgeOp.Cylinder, segments = 10, smooth = true,
                    size = new Vector3(0.10f, 1.1f, 0.10f), position = new Vector3(0.16f, 0.55f, 0.03f),
                    paletteSlot = 0 },
                new ForgePart { name = "PipeC", op = ForgeOp.Cylinder, segments = 10, smooth = true,
                    size = new Vector3(0.08f, 0.85f, 0.08f), position = new Vector3(-0.14f, 0.42f, -0.04f),
                    paletteSlot = 0 },
                new ForgePart { name = "FlangeA", op = ForgeOp.Torus, segments = 12, smooth = true,
                    size = new Vector3(0.2f, 0.05f, 0.01f), position = new Vector3(0f, 0.5f, 0f),
                    paletteSlot = 1 },
                new ForgePart { name = "FlangeB", op = ForgeOp.Torus, segments = 12, smooth = true,
                    size = new Vector3(0.15f, 0.04f, 0.01f), position = new Vector3(0.16f, 0.85f, 0.03f),
                    paletteSlot = 1 },
                // (photo v2: at z=0.08 the gauge sat half-buried in PipeA — pushed clear of the rim)
                new ForgePart { name = "Gauge", op = ForgeOp.SphereSection, bevel = 0.55f, segments = 10,
                    smooth = true, size = new Vector3(0.12f, 0.07f, 0.12f),
                    position = new Vector3(0f, 1.12f, 0.115f), eulerRotation = new Vector3(90f, 0f, 0f),
                    paletteSlot = 2 },
            };
            return d;
        }

        /// <summary>prop_dispatch_console — the pedestal console: frustum base, raked deck, glowing
        /// teal screen (strong emissive focal), greeble key row. Dock/dispatch dressing.</summary>
        private static ForgeRecipeDefinition BuildDispatchConsole()
        {
            var d = NewRecipe("prop_dispatch_console", ForgePalettes.FamilyToxicIndustrial,
                new[] { "prop" },
                new[]
                {
                    new Color(0.26f, 0.28f, 0.32f), // 0 chassis — gunmetal
                    new Color(0.40f, 0.42f, 0.48f), // 1 trim
                    new Color(0.35f, 0.85f, 0.75f), // 2 screen — teal glow
                },
                budgetTris: 1100);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "Dispatch console — dock/berth dressing, the E5.3 prop set.";
            d.storyRefs = new[] { "dressing_scatter" };
            d.worldRuleRefs = new[] { "ToxicCity" };
            d.tokenRefs = new[] { "glow_teal" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.45f, grime = 0.4f, panelDensity = 1.5f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.55f, grime = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.35f, 0.85f, 0.75f), emissiveIntensity = 1.4f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Base", op = ForgeOp.Frustum, segments = 10, smooth = true,
                    size = new Vector3(0.55f, 0.8f, 0.40f), position = new Vector3(0f, 0.4f, 0f),
                    paletteSlot = 0 },
                new ForgePart { name = "Deck", op = ForgeOp.Wedge,
                    size = new Vector3(0.6f, 0.2f, 0.45f), position = new Vector3(0f, 0.9f, 0f),
                    eulerRotation = new Vector3(0f, 180f, 0f), paletteSlot = 0 },
                new ForgePart { name = "Screen", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(0.42f, 0.26f, 0.03f), position = new Vector3(0f, 0.98f, 0.1f),
                    eulerRotation = new Vector3(-28f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "KeyRow", op = ForgeOp.GreebleStrip, segments = 6,
                    size = new Vector3(0.05f, 0.03f, 0.38f), position = new Vector3(0f, 0.87f, 0.16f),
                    eulerRotation = new Vector3(-28f, 90f, 0f), paletteSlot = 1 },
                new ForgePart { name = "Collar", op = ForgeOp.Torus, segments = 12, smooth = true,
                    size = new Vector3(0.5f, 0.05f, 0.01f), position = new Vector3(0f, 0.12f, 0f),
                    paletteSlot = 1 },
            };
            return d;
        }

        /// <summary>
        /// P2 ACCEPTANCE ARTIFACT (like tox_canal_stalker was for R4): one prop that exercises every
        /// P2 op — Frustum pedestal, Capsule stem, Torus collar, mirrored SweepSpline tentacles, a
        /// noisy OrganicBlob head — plus taper+bend on a LEGACY op (Cylinder spike) to prove the
        /// modifiers are op-agnostic. If this renders right in the booth, the vocabulary works.
        /// Not referenced by gameplay; it exists to be photographed (and later, dressed into a world).
        /// </summary>
        private static ForgeRecipeDefinition BuildP2TideTotem()
        {
            var d = NewRecipe("p2_tide_totem", ForgePalettes.FamilyToxicIndustrial,
                new[] { "prop", "showcase" },
                new[]
                {
                    new Color(0.22f, 0.23f, 0.25f), // 0 weathered stone base
                    new Color(0.16f, 0.30f, 0.30f), // 1 dark organic mass
                    new Color(0.30f, 0.80f, 0.85f), // 2 teal tidal glow
                    new Color(0.55f, 0.50f, 0.42f), // 3 pale barnacle flesh
                },
                budgetTris: 1500);
            d.qualityState = ForgeQualityState.Proxy;
            d.storyRole = "Tide totem — a barnacled marker the tide left behind; the P2 op-showcase piece.";
            d.storyRefs = new[] { "showcase" };
            d.worldRuleRefs = new[] { "ToxicCity" }; // its palette family's home world
            d.tokenRefs = new[] { "teal_energy", "organic" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Stone, wear = 0.5f, grime = 0.55f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.06f, grime = 0.4f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.30f, 0.85f, 0.9f), emissiveIntensity = 1.6f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.03f, grime = 0.55f },
            };
            d.parts = new[]
            {
                // Frustum pedestal (truncated cone, wider at the base).
                new ForgePart
                {
                    name = "Pedestal", op = ForgeOp.Frustum, segments = 10,
                    size = new Vector3(0.34f, 0.14f, 0.22f),
                    position = new Vector3(0f, 0.07f, 0f), paletteSlot = 0,
                },
                // Capsule stem, smooth.
                new ForgePart
                {
                    name = "Stem", op = ForgeOp.Capsule, segments = 10, smooth = true,
                    size = new Vector3(0.12f, 0.42f, 0.12f),
                    position = new Vector3(0f, 0.32f, 0f), paletteSlot = 1,
                },
                // Torus collar around the stem — the glow ring.
                new ForgePart
                {
                    name = "Collar", op = ForgeOp.Torus, segments = 14, smooth = true,
                    size = new Vector3(0.22f, 0.045f, 0.01f),
                    position = new Vector3(0f, 0.34f, 0f), paletteSlot = 2,
                },
                // Mirrored sweep tentacles curling up and out — the SweepSpline proof.
                new ForgePart
                {
                    name = "Tentacle", op = ForgeOp.SweepSpline, segments = 9, smooth = true, mirrorX = true,
                    size = new Vector3(0.06f, 0.1f, 0.1f),
                    spline = new[]
                    {
                        new Vector3(0.05f, 0.16f, 0f),
                        new Vector3(0.20f, 0.24f, 0.05f),
                        new Vector3(0.26f, 0.46f, -0.03f),
                    },
                    profile = new[] { new Vector2(0.035f, 0f), new Vector2(0.025f, 0f), new Vector2(0.010f, 0f) },
                    paletteSlot = 1,
                },
                // Noisy OrganicBlob head — fBm displacement is THE organic tell.
                new ForgePart
                {
                    name = "Head", op = ForgeOp.OrganicBlob, segments = 12, smooth = true,
                    size = new Vector3(0.20f, 0.17f, 0.20f),
                    noiseAmplitude = 0.018f, noiseFrequency = 14f, noiseSeed = 12,
                    position = new Vector3(0f, 0.56f, 0f), paletteSlot = 3,
                },
                // Taper + bend on a LEGACY Cylinder: modifiers must be op-agnostic.
                new ForgePart
                {
                    name = "Spike", op = ForgeOp.Cylinder, segments = 8, smooth = true,
                    size = new Vector3(0.05f, 0.26f, 0.05f),
                    taper = 0.8f, bendDegrees = 40f,
                    position = new Vector3(0f, 0.70f, 0f), paletteSlot = 2,
                },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Crown", localPosition = new Vector3(0f, 0.66f, 0f) },
            };
            return d;
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
                new ForgePart // scavenged industrial plate — sunk into the dome, heavy grime
                {
                    name = "ScrapPlate", op = ForgeOp.BeveledBox, bevel = 0.008f,
                    size = new Vector3(0.28f, 0.05f, 0.38f), position = new Vector3(0.08f, 0.46f, 0.0f),
                    eulerRotation = new Vector3(6f, 22f, 9f), paletteSlot = 1
                },
                // Legs v3: each leg is hip→knee→foot with the knee ABOVE the hip (arched crab
                // stance); Limb() spans the joint points so the knee connects by construction
                // (v2's hand-tuned eulers left the segments floating apart).
                Limb("LegFrontUpper", new Vector3(0.24f, 0.30f, 0.20f), new Vector3(0.52f, 0.48f, 0.28f), 0.07f, 0.085f, 0),
                Limb("LegFrontLower", new Vector3(0.52f, 0.48f, 0.28f), new Vector3(0.64f, 0.02f, 0.38f), 0.055f, 0.065f, 0),
                Limb("LegMidUpper", new Vector3(0.24f, 0.30f, 0.00f), new Vector3(0.55f, 0.48f, -0.02f), 0.07f, 0.085f, 0),
                Limb("LegMidLower", new Vector3(0.55f, 0.48f, -0.02f), new Vector3(0.70f, 0.02f, -0.05f), 0.055f, 0.065f, 0),
                Limb("LegRearUpper", new Vector3(0.22f, 0.30f, -0.24f), new Vector3(0.50f, 0.46f, -0.34f), 0.07f, 0.085f, 0),
                Limb("LegRearLower", new Vector3(0.50f, 0.46f, -0.34f), new Vector3(0.60f, 0.02f, -0.48f), 0.055f, 0.065f, 0),
                new ForgePart // toxin sacs — outboard for silhouette, raised clear of the leg
                // sight-lines (v4: a sac behind a leg bleeds a 1px glow outline around it)
                {
                    name = "ToxinSac", op = ForgeOp.SphereSection, bevel = 1f, segments = 10, smooth = true,
                    size = new Vector3(0.18f, 0.15f, 0.2f), position = new Vector3(0.3f, 0.44f, -0.11f),
                    mirrorX = true, paletteSlot = 2
                },
                new ForgePart // filter-gill bank across the front, sunk into the hide (v2 poked out like a plank)
                {
                    name = "GillBank", op = ForgeOp.GreebleStrip, segments = 6,
                    size = new Vector3(0.11f, 0.1f, 0.34f), position = new Vector3(0f, 0.3f, 0.42f),
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

        // ── THE ARSENAL (Terry: "everything AAA") — every gun in the game gets a real recipe, so
        // every gun inherits UVs, the four baked maps, and the E1.4 on-device textured look. ──────

        private static ForgeRecipeDefinition BuildPistolScrapMk1()
        {
            var d = NewRecipe("pistol_scrap_mk1", ForgePalettes.FamilyToxicIndustrial,
                new[] { "handheld", "standard" },
                new[]
                {
                    new Color(0.17f, 0.18f, 0.20f), // 0 gunmetal frame
                    new Color(0.55f, 0.56f, 0.58f), // 1 bare slide
                    new Color(0.23f, 0.17f, 0.12f), // 2 stippled grip
                    new Color(1f, 0.62f, 0.25f),    // 3 amber sight
                },
                budgetTris: 2000);
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Standard salvage sidearm — the working stiff's holdout.";
            d.storyRefs = new[] { "salvage", "stun_combat" };
            d.worldRuleRefs = new[] { "ToxicCity" };
            d.tokenRefs = new[] { "gunmetal", "bare_steel" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.5f, grime = 0.4f, panelDensity = 2.5f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.7f, grime = 0.25f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.07f, grime = 0.35f }, // stipple grip
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(1f, 0.62f, 0.25f), emissiveIntensity = 2f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Frame", op = ForgeOp.BeveledBox, bevel = 0.004f,
                    size = new Vector3(0.034f, 0.05f, 0.16f), position = Vector3.zero, paletteSlot = 0 },
                new ForgePart { name = "Slide", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.037f, 0.03f, 0.17f), position = new Vector3(0f, 0.04f, 0f), paletteSlot = 1 },
                new ForgePart { name = "SlideSerrations", op = ForgeOp.GreebleStrip, segments = 5,
                    size = new Vector3(0.036f, 0.012f, 0.05f), position = new Vector3(0f, 0.056f, -0.05f), paletteSlot = 1 },
                new ForgePart { name = "Barrel", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.026f, 0.05f, 0.026f), position = new Vector3(0f, 0.045f, 0.095f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1 },
                new ForgePart { name = "MuzzleRing", op = ForgeOp.Tube, segments = 10, wallThickness = 0.004f,
                    size = new Vector3(0.034f, 0.02f, 0.034f), position = new Vector3(0f, 0.045f, 0.112f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 0 },
                new ForgePart { name = "Grip", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.03f, 0.095f, 0.042f), position = new Vector3(0f, -0.062f, -0.055f),
                    eulerRotation = new Vector3(-18f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "GripPlate", op = ForgeOp.BeveledBox, bevel = 0.002f,
                    size = new Vector3(0.005f, 0.08f, 0.034f), position = new Vector3(0.018f, -0.062f, -0.055f),
                    eulerRotation = new Vector3(-18f, 0f, 0f), mirrorX = true, paletteSlot = 0 },
                new ForgePart { name = "TriggerGuard", op = ForgeOp.BeveledBox, bevel = 0.002f,
                    size = new Vector3(0.008f, 0.008f, 0.055f), position = new Vector3(0f, -0.03f, -0.012f), paletteSlot = 0 },
                new ForgePart { name = "Hammer", op = ForgeOp.BeveledBox, bevel = 0.002f,
                    size = new Vector3(0.012f, 0.02f, 0.014f), position = new Vector3(0f, 0.052f, -0.09f), paletteSlot = 1 },
                new ForgePart { name = "SightDot", op = ForgeOp.BeveledBox, bevel = 0.001f,
                    size = new Vector3(0.006f, 0.008f, 0.01f), position = new Vector3(0f, 0.062f, -0.075f), paletteSlot = 3 },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, -0.05f, -0.05f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.045f, 0.125f), localEuler = Vector3.zero },
            };
            return d;
        }

        private static ForgeRecipeDefinition BuildStaticNetLobber()
        {
            var d = NewRecipe("static_net_lobber", ForgePalettes.FamilyToxicIndustrial,
                new[] { "handheld", "standard" },
                new[]
                {
                    new Color(0.25f, 0.34f, 0.24f), // 0 moss-painted body
                    new Color(0.36f, 0.28f, 0.20f), // 1 oxidized mouth
                    new Color(0.30f, 0.45f, 0.30f), // 2 charge tank
                    new Color(0.3f, 0.85f, 0.5f),   // 3 net-green glow
                },
                budgetTris: 2000);
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Wide-mouth static-net lobber — arena crowd control.";
            d.storyRefs = new[] { "stun_combat", "arena" };
            d.worldRuleRefs = new[] { "PvP_Arena01" };
            d.tokenRefs = new[] { "moss_paint", "net_green" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.45f, grime = 0.5f, panelDensity = 2f },
                new ForgeStyleSpec { style = ForgeStyle.RustedMetal, wear = 0.5f, grime = 0.5f },
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.3f, 0.85f, 0.5f), emissiveIntensity = 2.2f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Body", op = ForgeOp.BeveledBox, bevel = 0.006f,
                    size = new Vector3(0.09f, 0.075f, 0.2f), position = new Vector3(0f, 0f, -0.01f), paletteSlot = 0 },
                new ForgePart { name = "TopCoils", op = ForgeOp.GreebleStrip, segments = 5,
                    size = new Vector3(0.07f, 0.03f, 0.12f), position = new Vector3(0f, 0.055f, -0.02f), paletteSlot = 1 },
                new ForgePart { name = "Mouth", op = ForgeOp.Tube, segments = 12, wallThickness = 0.009f,
                    size = new Vector3(0.1f, 0.05f, 0.1f), position = new Vector3(0f, 0.01f, 0.115f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1 },
                new ForgePart { name = "MouthCore", op = ForgeOp.Cylinder, segments = 12,
                    size = new Vector3(0.07f, 0.012f, 0.07f), position = new Vector3(0f, 0.01f, 0.1f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 3 },
                new ForgePart { name = "Tank", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.056f, 0.1f, 0.056f), position = new Vector3(0f, -0.055f, -0.05f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "SideVents", op = ForgeOp.GreebleStrip, segments = 4,
                    size = new Vector3(0.03f, 0.018f, 0.11f), position = new Vector3(0.055f, 0.012f, -0.03f),
                    eulerRotation = new Vector3(0f, 0f, -90f), mirrorX = true, paletteSlot = 1 },
                new ForgePart { name = "Grip", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.03f, 0.09f, 0.045f), position = new Vector3(0f, -0.072f, -0.1f),
                    eulerRotation = new Vector3(-18f, 0f, 0f), paletteSlot = 0 },
                new ForgePart { name = "TriggerGuard", op = ForgeOp.BeveledBox, bevel = 0.002f,
                    size = new Vector3(0.008f, 0.01f, 0.06f), position = new Vector3(0f, -0.045f, -0.045f), paletteSlot = 0 },
                new ForgePart { name = "StockPlate", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.055f, 0.055f, 0.03f), position = new Vector3(0f, 0f, -0.125f), paletteSlot = 1 },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, -0.06f, -0.095f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.01f, 0.14f), localEuler = Vector3.zero },
            };
            return d;
        }

        private static ForgeRecipeDefinition BuildSonicThumperMaul()
        {
            var d = NewRecipe("sonic_thumper_maul", ForgePalettes.FamilyToxicIndustrial,
                new[] { "handheld", "standard" },
                new[]
                {
                    new Color(0.32f, 0.30f, 0.28f), // 0 iron head
                    new Color(0.72f, 0.48f, 0.18f), // 1 hazard amber
                    new Color(0.15f, 0.14f, 0.14f), // 2 wrapped haft
                    new Color(1f, 0.62f, 0.25f),    // 3 resonator glow
                },
                budgetTris: 2000);
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Sonic thumper — a dockworker's breaching maul that hits like a subwoofer.";
            d.storyRefs = new[] { "stun_combat", "arena" };
            d.worldRuleRefs = new[] { "PvP_Arena01" };
            d.tokenRefs = new[] { "hazard_amber", "iron" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.55f, grime = 0.45f },
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.6f, grime = 0.35f, panelDensity = 1.5f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.06f, grime = 0.4f }, // grip wrap
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(1f, 0.62f, 0.25f), emissiveIntensity = 2.4f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Haft", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.048f, 0.26f, 0.048f), position = new Vector3(0f, 0f, -0.1f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "PommelGlow", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.038f, 0.016f, 0.038f), position = new Vector3(0f, 0f, -0.235f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 3 },
                new ForgePart { name = "Head", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(0.13f, 0.11f, 0.12f), position = new Vector3(0f, 0f, 0.1f), paletteSlot = 0 },
                new ForgePart { name = "FacePlate", op = ForgeOp.GreebleStrip, segments = 4,
                    size = new Vector3(0.11f, 0.02f, 0.09f), position = new Vector3(0f, 0f, 0.168f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1 },
                new ForgePart { name = "RingFront", op = ForgeOp.Tube, segments = 12, wallThickness = 0.01f,
                    size = new Vector3(0.15f, 0.022f, 0.15f), position = new Vector3(0f, 0f, 0.145f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1 },
                new ForgePart { name = "RingBack", op = ForgeOp.Tube, segments = 12, wallThickness = 0.01f,
                    size = new Vector3(0.15f, 0.022f, 0.15f), position = new Vector3(0f, 0f, 0.055f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 1 },
                new ForgePart { name = "TopFin", op = ForgeOp.Wedge,
                    size = new Vector3(0.02f, 0.05f, 0.1f), position = new Vector3(0f, 0.075f, 0.1f), paletteSlot = 1 },
                new ForgePart { name = "SideCell", op = ForgeOp.BeveledBox, bevel = 0.002f,
                    size = new Vector3(0.012f, 0.03f, 0.06f), position = new Vector3(0.068f, 0f, 0.1f),
                    mirrorX = true, paletteSlot = 3 },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, 0f, -0.16f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.175f), localEuler = Vector3.zero },
            };
            return d;
        }

        private static ForgeRecipeDefinition BuildPrismBeamRifle()
        {
            var d = NewRecipe("prism_beam_rifle", ForgePalettes.FamilyToxicIndustrial,
                new[] { "handheld", "standard" },
                new[]
                {
                    new Color(0.22f, 0.20f, 0.26f), // 0 violet-gray chassis
                    new Color(0.50f, 0.50f, 0.55f), // 1 bare fittings
                    new Color(0.14f, 0.13f, 0.15f), // 2 grip
                    new Color(0.85f, 0.35f, 0.9f),  // 3 prism magenta
                },
                budgetTris: 2000);
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Prism beam rifle — charge, aim, one refracted lance of light.";
            d.storyRefs = new[] { "stun_combat", "arena" };
            d.worldRuleRefs = new[] { "PvP_Arena01" };
            d.tokenRefs = new[] { "prism_magenta" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.5f, grime = 0.3f, panelDensity = 3f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.6f, grime = 0.25f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.07f, grime = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.85f, 0.35f, 0.9f), emissiveIntensity = 2.6f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Receiver", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.045f, 0.06f, 0.22f), position = new Vector3(0f, 0f, -0.03f), paletteSlot = 0 },
                new ForgePart { name = "BarrelShroud", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.036f, 0.045f, 0.18f), position = new Vector3(0f, 0.005f, 0.14f), paletteSlot = 0 },
                new ForgePart { name = "ShroudRibs", op = ForgeOp.GreebleStrip, segments = 6,
                    size = new Vector3(0.03f, 0.015f, 0.14f), position = new Vector3(0f, 0.035f, 0.14f), paletteSlot = 1 },
                new ForgePart { name = "ChargeCoil", op = ForgeOp.Tube, segments = 12, wallThickness = 0.006f,
                    size = new Vector3(0.06f, 0.03f, 0.06f), position = new Vector3(0f, 0.005f, 0.2f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 3 },
                new ForgePart { name = "PrismHousing", op = ForgeOp.SphereSection, bevel = 1f, segments = 10, smooth = true,
                    size = new Vector3(0.06f, 0.06f, 0.07f), position = new Vector3(0f, 0.005f, 0.245f), paletteSlot = 1 },
                new ForgePart { name = "Crystal", op = ForgeOp.BeveledBox, bevel = 0.003f,
                    size = new Vector3(0.02f, 0.02f, 0.05f), position = new Vector3(0f, 0.005f, 0.275f),
                    eulerRotation = new Vector3(0f, 0f, 45f), paletteSlot = 3 },
                new ForgePart { name = "Stock", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.035f, 0.055f, 0.09f), position = new Vector3(0f, -0.01f, -0.165f), paletteSlot = 1 },
                new ForgePart { name = "Grip", op = ForgeOp.BeveledBox, bevel = 0.005f,
                    size = new Vector3(0.028f, 0.09f, 0.04f), position = new Vector3(0f, -0.07f, -0.06f),
                    eulerRotation = new Vector3(-20f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "TopRail", op = ForgeOp.GreebleStrip, segments = 7,
                    size = new Vector3(0.014f, 0.012f, 0.16f), position = new Vector3(0f, 0.052f, -0.01f), paletteSlot = 1 },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, -0.058f, -0.055f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0.005f, 0.29f), localEuler = Vector3.zero },
            };
            return d;
        }

        // ── Builders ───────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// A limb segment spanning two joint points. Both ends extend 0.03 past the joint so
        /// consecutive segments interpenetrate there — the knee connects by construction instead
        /// of by hand-tuned eulers (which drift; see stalker v2).
        /// </summary>
        private static ForgePart Limb(string name, Vector3 from, Vector3 to, float width, float depth, int paletteSlot)
        {
            Vector3 d = to - from;
            float len = d.magnitude;
            return new ForgePart
            {
                name = name, op = ForgeOp.BeveledBox, bevel = 0.005f,
                size = new Vector3(width, len + 0.06f, depth),
                position = (from + to) * 0.5f,
                eulerRotation = Quaternion.FromToRotation(Vector3.up, d / len).eulerAngles,
                mirrorX = true, paletteSlot = paletteSlot
            };
        }

        /// <summary>
        /// Breaker Blade — a salvage energy cleaver (the melee pair, MP lane). Wrapped haft + gunmetal
        /// crossguard + a broad steel blade with a glowing CYAN energy edge running its length to a
        /// wedge tip (the runbook's "flat cyan blade"). Blade forward +Z, haft back -Z. ~56 cm.
        /// </summary>
        private static ForgeRecipeDefinition BuildBreakerBlade()
        {
            var d = NewRecipe("breaker_blade_mk1", ForgePalettes.FamilyToxicIndustrial,
                new[] { "handheld", "standard", "melee" },
                new[]
                {
                    new Color(0.13f, 0.13f, 0.15f), // 0 wrapped haft (dark)
                    new Color(0.56f, 0.61f, 0.67f), // 1 blade steel
                    new Color(0.30f, 0.32f, 0.36f), // 2 gunmetal fittings
                    new Color(0.35f, 0.85f, 0.95f), // 3 cyan energy edge
                },
                budgetTris: 2000);
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Breaker Blade — a scavenged breaching cleaver with a lit energy edge; true contact melee.";
            d.storyRefs = new[] { "melee_combat", "arena" };
            d.worldRuleRefs = new[] { "PvP_Arena01" };
            d.tokenRefs = new[] { "cyan_energy", "salvage" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.06f, grime = 0.4f },        // grip wrap
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.5f, grime = 0.35f },          // blade
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.55f, grime = 0.4f, panelDensity = 1.2f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.35f, 0.85f, 0.95f), emissiveIntensity = 2.6f },
            };
            d.parts = new[]
            {
                new ForgePart { name = "Haft", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.03f, 0.13f, 0.03f), position = new Vector3(0f, 0f, -0.14f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 0 },
                new ForgePart { name = "Pommel", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.038f, 0.018f, 0.038f), position = new Vector3(0f, 0f, -0.205f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "Crossguard", op = ForgeOp.BeveledBox, bevel = 0.004f,
                    size = new Vector3(0.15f, 0.03f, 0.035f), position = new Vector3(0f, 0f, -0.06f), paletteSlot = 2 },
                new ForgePart { name = "Blade", op = ForgeOp.BeveledBox, bevel = 0.006f,
                    size = new Vector3(0.014f, 0.085f, 0.40f), position = new Vector3(0f, 0f, 0.17f), paletteSlot = 1 },
                new ForgePart { name = "Tip", op = ForgeOp.Wedge,
                    size = new Vector3(0.014f, 0.085f, 0.08f), position = new Vector3(0f, 0f, 0.41f), paletteSlot = 1 },
                new ForgePart { name = "EdgeGlow", op = ForgeOp.BeveledBox, bevel = 0.004f,
                    size = new Vector3(0.02f, 0.028f, 0.40f), position = new Vector3(0f, 0.03f, 0.17f), paletteSlot = 3 },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, 0f, -0.14f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.45f), localEuler = Vector3.zero },
            };
            return d;
        }

        /// <summary>
        /// Tide Pike — a long tidal thrust pike (the melee pair's REACH identity). Dark wrapped shaft +
        /// bronze collar + a leaf spearhead with teal energy runnels + swept-back barbs. Head forward +Z.
        /// ~74 cm (the long one). Grip in the rear third; Muzzle at the point.
        /// </summary>
        private static ForgeRecipeDefinition BuildTidePike()
        {
            var d = NewRecipe("tide_pike_mk1", ForgePalettes.FamilyToxicIndustrial,
                new[] { "handheld", "standard", "melee" },
                new[]
                {
                    new Color(0.14f, 0.15f, 0.16f), // 0 dark shaft
                    new Color(0.50f, 0.56f, 0.60f), // 1 head steel
                    new Color(0.45f, 0.38f, 0.24f), // 2 bronze fittings
                    new Color(0.30f, 0.80f, 0.85f), // 3 teal tidal glow
                },
                budgetTris: 2000);
            d.qualityState = ForgeQualityState.ProxyPlus;
            d.storyRole = "Tide Pike — a long reaching thrust-spear; the tidefront's boarding pike.";
            d.storyRefs = new[] { "melee_combat", "arena" };
            d.worldRuleRefs = new[] { "PvP_Arena01" };
            d.tokenRefs = new[] { "teal_energy", "bronze" };
            d.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.05f, grime = 0.45f },        // shaft wrap
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.5f, grime = 0.3f },            // head
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.6f, grime = 0.4f },         // bronze
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.30f, 0.80f, 0.85f), emissiveIntensity = 2.4f },
            };
            d.parts = new[]
            {
                // Shaft spans the whole length so the head (front) and butt-cap (back) stay CONNECTED —
                // v1 shipped exploded because a 0.34 shaft couldn't reach a head at +0.30 / cap at -0.34.
                new ForgePart { name = "Shaft", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.022f, 0.66f, 0.022f), position = new Vector3(0f, 0f, -0.02f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 0 },
                new ForgePart { name = "ButtCap", op = ForgeOp.Cylinder, segments = 10,
                    size = new Vector3(0.028f, 0.018f, 0.028f), position = new Vector3(0f, 0f, -0.345f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "Collar", op = ForgeOp.Tube, segments = 12, wallThickness = 0.008f,
                    size = new Vector3(0.05f, 0.03f, 0.05f), position = new Vector3(0f, 0f, 0.30f),
                    eulerRotation = new Vector3(90f, 0f, 0f), paletteSlot = 2 },
                new ForgePart { name = "Head", op = ForgeOp.BeveledBox, bevel = 0.006f,
                    size = new Vector3(0.014f, 0.062f, 0.18f), position = new Vector3(0f, 0f, 0.42f), paletteSlot = 1 },
                new ForgePart { name = "HeadTip", op = ForgeOp.Wedge,
                    size = new Vector3(0.014f, 0.062f, 0.09f), position = new Vector3(0f, 0f, 0.55f), paletteSlot = 1 },
                new ForgePart { name = "Barb", op = ForgeOp.Wedge,
                    size = new Vector3(0.012f, 0.028f, 0.06f), position = new Vector3(0.03f, 0f, 0.34f),
                    eulerRotation = new Vector3(0f, 200f, 0f), mirrorX = true, paletteSlot = 2 },
                new ForgePart { name = "HeadGlow", op = ForgeOp.BeveledBox, bevel = 0.003f,
                    size = new Vector3(0.02f, 0.016f, 0.17f), position = new Vector3(0f, 0f, 0.42f), paletteSlot = 3 },
            };
            d.sockets = new[]
            {
                new ForgeSocket { name = "Grip", localPosition = new Vector3(0f, 0f, -0.12f), localEuler = new Vector3(45f, 0f, 0f) },
                new ForgeSocket { name = "Muzzle", localPosition = new Vector3(0f, 0f, 0.59f), localEuler = Vector3.zero },
            };
            return d;
        }

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

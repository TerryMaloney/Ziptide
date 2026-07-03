# 🎨 THE ART REGISTRY — the contract between game generators and the art track (V2.5 H4)

**The one law:** builders/gameplay request a LOOK by **id** and never know the fulfillment tech.
The art track (Picasso) owns fulfillment: primitives today, Forge meshes next, imported kits later —
**swapping the tech behind an id must never touch a consumer.** Code seam:
`Editor/Art/ArtModuleRegistry.cs` (`Register(id, factory)` / `TryBuild(id, out go)` — unfulfilled ids
fall back to the caller's primitive, the proven `ForgeVisualApplier.TryApply` shape).

## The id families
| Family | Format | Consumer today | Fulfillment (Picasso's lane) |
|---|---|---|---|
| Building modules | `buildingModule:<styleId>/<Module>` (Module = WallSolid/WallWindow/Doorway/CornerTrim/RoofFlat/RoofRaked) | `BuildingBuilder` (H1, live) | Forge-recipe family per style — **highest-leverage next art unit** |
| Weapon/prop looks | `forge:<recipeId>` (via `ItemDefinition.forgeRecipeId`) | `ItemFactory` (shipped) | `ForgeRecipeLibrary` (shipped — taser proves it) |
| Surface families | `surfaceSet:<family>` (Toxic Earth, Salvage, Alien Origami, … per `ART_DIRECTION_MASTER_PLAN`) | CityBuilder/ScenePatcherArena material lookups (planned swap) | Picasso's declared `SurfaceSetDefinition` — **this registry IS that plan's canonical home** |
| Scatter/props | `scatterKind:<id>` (rocks/flora/debris/crystals/bones…) | `WorldDressingBuilder` (swap planned, H5) | kit meshes per biome |
| World art kits | `worldArtKit:<world>` | W001 pass (Picasso's board) | the W001 Toxic Venice brief |
| Skies | (already id-native: `VisualThemeProfile.skyVista`) | shipped | `SkyVistaLibrary` (shipped) |

## Rules
1. **Ids are forever.** Rename = alias, never break. Consumers hard-code ids; specs reference them.
2. **Fallback is sacred.** Every consumer must render acceptably with ZERO fulfillments (primitive
   fallback) — art can never block a build.
3. **Budget rides the id.** Fulfillments must pass the FORGE_* budgets and (once live) the
   PERF_BUDGET gate; an over-budget kit fails CI, not the headset.
4. **Registration lives in the art lane** (editor authors under `Editor/`), consumers never register.
5. **Deferred tech, recorded triggers:** runtime glTF import (glTFast) joins if assets ever come
   from OUTSIDE the repo (marketplace/Tripo at scale); Addressables joins on RAM pressure or >20
   worlds of real kits. Neither changes an id.

*Written by the architect (V2.5); co-owned with the art track — Picasso extends the table, never the laws.*

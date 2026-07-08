# WORLD-BUILDING AT SCALE — mass VR map production (design doc)

> **Hardwiring §1 / Phase 0–1.** How Ziptide builds many rich, consistent VR worlds cheaply on Quest.
> Status: skeleton for Fable 5. Pairs with `ART_REGISTRY.md`, `CITY_DESIGN.md`,
> `WORLD_FLOW_TEMPLATES.md`, `BUILDING_INTERIORS.md`, `VERTICAL_AND_CAVERN_WORLDS.md`.

## Current state (code survey)
Generation spine is real + tested: `WorldSpec → WorldSpecCompiler → CityLayout →
WorldExperienceBuilder / WorldPoiBuilder / WorldDressingBuilder / BuildingBuilder`; `TerrainField`
wired; `BuildingGrammar` assembles buildings; 8 audit gates. **The gap: `ArtModuleRegistry` has ZERO
registered kits**, so every `buildingModule:`/`surfaceSet:` id falls back to a primitive box. Only 2
BuildingStyle assets exist. That empty seam is why worlds read as empty lots.

## Locked decisions
Modular kits, Quest-safe (no voxel). Kits are Forge-baked + atlas'd + registry-driven.

## What to build
- [ ] **Kit model:** extend `BuildingStyle` into a *kit* — a family of Forge modules (facade panel,
  window, door, roof cap, balcony, corner, greeble) sharing one trim-sheet atlas → one material family.
- [ ] **Fulfill `ArtModuleRegistry`** with ≥4 kits (salvage_row, toxic_tenement + 2 new biomes).
  `KIT_FULFILLED` audit rule blocks a build that would ship primitive fallback for a shipped world.
- [ ] **Biome → kit + palette** field on `WorldSpec` so each world coherently selects its look.
- [ ] **Interior-mapping windows** (see `BUILDING_INTERIORS.md`) applied by the building builder.
- [ ] **POI catalog** — `WorldPoiBuilder` gains a registry of authored POI prefabs stamped at layout
  anchors; ≥12 types (market, shrine, garden grove, repair bay, transit, story node, machine bay…).
- [ ] **Scatter/density** — fulfill `ScatterField` + `WorldDressingBuilder` with real prop kits.
- [ ] **Budget caps** per content type in `PerfBudgetAuditRules`.

## Technique research TODO (cite at build time — was cut off by rate limit)
Trim sheets + atlas kit pipelines; POI/prefab-set stamping; WFC vs grammar for module assembly;
Quest draw-call/tri/texture budgets, static batching + GPU instancing, occlusion/portal culling, LOD +
distance impostors; additive chunk streaming for large worlds. Fill `## Sourced technique` before building.

## Tests + audit
Kit assembly deterministic per seed; `KIT_FULFILLED`, `WORLD_CONTENT`, reachability, PerfBudget gates green.

## Additions Bank
Pull from `WORLDS_50.md` (traversal/secrets/weather) as kits land.

## 🚀 Room to expand
Wave-function-collapse city blocks; procedural signage/decals from the Forge; weather + time-of-day as a
world layer; landmark/skyline silhouettes for navigation; a "world recipe" authoring pass so Terry can
type a world into being (`WORLD_RECIPE.md`). Add worlds beyond the 12 — the machine is the point.

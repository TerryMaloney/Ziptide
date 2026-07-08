# BUILDING INTERIORS — faked + walkable (design doc)

> **Hardwiring §2 / Phase 1.** Buildings you can look into and, for some, walk inside. Status: skeleton
> for Fable 5. Pairs with `WORLD_BUILDING_AT_SCALE.md`, `ART_REGISTRY.md`.

## Current state (code survey)
Buildings render exteriors only — no way inside. `RoomPartitioner` (BSP room graph) exists as a pure
core but is **not wired to any walkable interior**.

## What to build — two tiers (do both)

### Tier A — faked interiors on every building  *(cheap, ships first)*
- [ ] **Interior-mapping URP shader** — parallax "rooms" behind windows with zero geometry.
- [ ] Window material variant applied by the building builder; per-biome interior atlas.
- [ ] Randomized (seeded) room contents so a facade reads as inhabited, not tiled.

### Tier B — walkable interiors on "enterable" buildings
- [ ] `enterable` flag + `interiorKitId` on `BuildingStyle`.
- [ ] **Interior kit** (floor/wall/ceiling/doorway/stair/furniture/fixture) in `ArtModuleRegistry`.
- [ ] `RoomPartitioner` → **interior mesh builder**: rooms → walkable geometry + doorways.
- [ ] **Portal / occlusion system** — only the current room draws (mandatory on Quest).
- [ ] **Door + threshold system** — VR grab/push doors; for large buildings the interior is its own
  additive sub-scene entered via a travel doorway (honor the world contract).
- [ ] **Interior POI pass** — loot, machines, garden plots, NPCs, story nodes placed per room.
- [ ] Interior audit rule (reachable, spawn-safe, budget).

## Consistency-spine hooks
Same Forge kits + `ArtModuleRegistry` + PerfBudget as exteriors; interior sub-scenes are world scenes
via `TravelCoordinator`; interior props are Definitions-by-id.

## Technique research TODO (cite at build time)
Interior-mapping parallax shader math; when to fake vs build; portal culling on mobile VR; modular
interior kit conventions. Fill `## Sourced technique` before building Tier B.

## Tests + audit
Interior mesh deterministic per seed; portal culling verified (rooms cull); reachability + budget green.

## 🚀 Room to expand
Multi-floor interiors (ties to verticality); destructible/searchable furniture; interiors that host
garden/automation/shop systems; NPC homes with story; a "safehouse" the player furnishes (cosmetic
layer). Push interiors as gameplay space, not dressing.

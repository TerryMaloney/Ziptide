# BUILDING INTERIORS — faked + walkable (design doc)

> **Hardwiring §2 / Phase 1.** Buildings you can look into and, for some, walk inside. Status: skeleton
> for Fable 5. Pairs with `WORLD_BUILDING_AT_SCALE.md`, `ART_REGISTRY.md`.

## Current state (code survey)
Buildings render exteriors only — no way inside. `RoomPartitioner` (BSP room graph) exists as a pure
core but is **not wired to any walkable interior**.

## What to build — two tiers (do both)

### Tier A — faked interiors on every building  *(cheap, ships first)*
- [x] **Interior-mapping URP shader** — ✅ SHIPPED (hardwiring 1.2): `Ziptide/InteriorMapping`
  (`Visuals/Shaders/`), the game's first custom shader — object-space room raycast, procedural
  per-object room tint + lit/dark variation, stereo-instanced macros, no textures (Meta: shader ALU
  is the #1 Quest GPU cost — kept minimal). 🎮 device-verify the parallax read + perf.
- [x] Window material applied by the kit — `BuildingKitLibrary.InteriorPane` builds the material at
  patch time (serialized → shader ships in the APK), flat-color fallback if the shader is missing.
- [ ] Per-biome interior ATLAS (textured rooms) — Picasso upgrade path via the same material.
- [x] Seeded room variation — per-object hash drives tint + dark-room chance (a living skyline).

### Tier B — walkable interiors on "enterable" buildings
- [x] Opt-in flag — ✅ `BuildingStyleDefinition.hasInteriors` (default OFF; `toxic_tenement` enabled —
  W002's tenements are the proof).
- [ ] **Interior kit** (authored wall/doorway/furniture modules) in `ArtModuleRegistry` — Picasso
  upgrade path; v1 interiors are primitive trim-dark walls + warm light panels.
- [x] `RoomPartitioner` → **interior mesh builder** — ✅ SHIPPED (hardwiring 1.3): pure
  `InteriorMeshCore` (walls-from-plan rasterize+merge, doorway entry-carve, 5 tests) +
  `InteriorBuilder` (wall cubes w/ colliders, one warm light panel per room; the shell's storey slabs
  are floor+ceiling). Deterministic per lot; renderer-budget gate audits the cost.
- [ ] **Portal / occlusion system** — only the current room draws (needed before interiors go dense).
- [ ] **Door + threshold system** — VR grab/push doors; big-building additive sub-scenes.
- [ ] **Interior POI pass** — loot, machines, garden plots, NPCs, story nodes placed per room.
- [ ] Interior audit rule (reachable, spawn-safe, budget) — extend the door-blocked gate inward.

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

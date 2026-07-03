# 🏗 ACTIVE SPRINT — ARCHITECTURE V2 (architect lane; opened 2026-07-03)

> **Takeover prompt: "Read docs/SPRINT_ARCHITECTURE.md and continue."** Fourth track file, same rules
> as the other three (SPRINT.md story · SPRINT_MULTIPLAYER.md MP · SPRINT_ART.md art): pull --rebase,
> small CI-green commits, update THIS file in the same commit as every push, HANDOFF entry per session.
> Design source: **`docs/design/ARCHITECTURE_V2.md`** (the laws + phase map + deferrals — read it once).
> This track owns ONLY NEW files (Spec/ cores/gates/GamePool/docs); the two integration one-liners into
> `CityBuilder`/`WorldExperienceBuilder` are T-Dog's, announced in HANDOFF (ppp).

**Program goal (Terry, 2026-07-03):** the PDF overhaul — backend + literal buildings — executable by
any LLM: request a change as data, build, gates catch mistakes.

---

## Task board
| # | Task | Status |
|---|------|--------|
| Q0 | Program docs: ARCHITECTURE_V2 design + this board + HANDOFF (ppp) + PRIORITIES rewrite + tools/*.ps1 ASCII fix (Terry's PS 5.1 parse error) | ✅ `5b5b1b6` |
| Q1a | **`WorldSpec`** SHIPPED: one serializable class = the whole world (identity/seed · sky/fog/planet · experience terrain+vista · POIs · palette/districts/connections/canals/shipyard · drones/creatures/hazards · collectibles/machines/mines/gardens/sockets · flags · advisory skyVistaId). REUSES the layout/pack [Serializable] classes verbatim — zero mapping drift; JSON round-trip via JsonUtility (enums as ints, tables in WORLD_RECIPE) | ✅ this commit |
| Q1b | **`WorldSpecValidator`** SHIPPED (pure): stable CODE-token errors — identity/seed, experience bounds, POI count/verbs/spacing/bounds/dup + StoryAnchor-when-flags predicate, district/connection/spawn refs, registry checks (creatures/plants/items; null set = permissive), mine/socket economy sanity. **14 tests** | ✅ this commit |
| Q1c | **`WorldSpecCompiler`** SHIPPED (Editor): `CompileAll()` reads `docs/worldspecs/*.spec.json` → validate vs REAL registries → fan out to `<scene>_Layout.asset` + `<scene>_WorldPack.asset` (create-or-update, WorldStubGenerator enriches at build) · `ExportAllFromMenu()` reverse-generates a spec per existing world · `SPEC_DRIFT` WARN on round-trip divergence · BuildAndroid hook (announced append) | ✅ this commit |
| Q1d | **Proof (🔧UNITY)**: Terry runs `Ziptide → Worlds → Export All World Specs (JSON)` once and commits `docs/worldspecs/*.spec.json` — every world gets its editable truth document; from then on "change a world" = edit its spec (fidelity is by-construction: export and compile share the same object graph). Runbook §2k | ⬜ Terry |
| Q2a | **`LotPartitioner`** SHIPPED (pure, seeded xorshift): recursive subdivision of district rects → lots + right-of-ways (perimeter ring + internal avenues); base cases min-area (child-length bound `req`), MinSide, aspect law (violations only when no legal cut existed — provable), **frontage GUARANTEE** (a cut that would landlock a child becomes a street both children front). **11 tests** incl. 25-seed frontage sweep + area conservation | ✅ this commit |
| Q2b/H1 | **`BuildingGrammar` SHIPPED** (pure, seeded): WFC-lite — the DOOR collapses first onto a street-fronted `Lot.Front*` edge (LotPartitioner guarantees one), windows/solids fill seeded, contradiction-free socket set = no backtracking ever. `BuildingStyleDefinition` (SO + `ToData()` mirror, `styleId`/`surfaceFamily` = Art Registry keys) + `BuildingStyleAuthor` (create-only: salvage_row + toxic_tenement → Resources/BuildingStyles, build-hooked). **11 tests** incl. end-to-end partition→plan door law | ✅ this commit |
| Q2c/H1 | **`BuildingBuilder` SHIPPED** (Editor): plans → multi-storey shells w/ ENTERABLE doorways (+`__DOOR` audit markers); modules resolve via **`ArtModuleRegistry`** (`buildingModule:<styleId>/<Module>`, primitive fallback — Picasso's seam). DORMANT wire in CityBuilder's district pass (announced): opts in via new `DistrictDef.buildingStyleId` — zero change to existing worlds until a layout/spec sets it. Gates live: `BUILDING_DOOR_BLOCKED` (raycast from markers) + `BUILDING_OVER_BUDGET` (renderer cap/district); `LOT_OVERLAP` enforced in the pure tests | ✅ this commit |
| Q2d | W002 warren proof: set `buildingStyleId` on one W002 district (via spec once Terry exports, or layout edit) + APK + runbook gate ("does it read as a place?") | ⬜ next operator |
| Q3 | **`TerrainField`** (pure, ~15 tests): octaved fBM + domain warp + temp×moisture biome matrix; BiomePreset → parameter set mapping; `TERRAIN_SLOPE_UNWALKABLE` gate. T-Dog swaps WorldExperienceBuilder's height fn (announced) | ⬜ |
| Q4a | **`GamePool`** (Gameplay): pooled spawn/despawn; call-site swaps in PvpBolt/darts/arcs/thump rings/nets (behavior-preserving) | ⬜ |
| Q4b | **`PERF_BUDGET` audit rule**: per-scene tris/materials/renderer-count/lights vs QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md — WARN 80%, BLOCK over caps, exempt `_Boot`. + shader-variant WARN | ⬜ |
| Q5 | Process: LFS-ready commented `.gitattributes` stanzas + V2 laws → CLAUDE.md/HOW_TO_CHANGE_ANYTHING + WorldSpec how-to row | ⬜ |
| — | Close per phase: CI green → APK dispatch → audit green → runbook rows → HANDOFF | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **State:** Q0 + Q1a/b/c shipped — verify CI on this push. Q1d is Terry's one-menu export
  (runbook §2k); until he commits the exported specs, the spec folder is empty and CompileAll is a
  no-op (safe by design).
- **Next action:** **Q2b — `BuildingGrammar`** (pure, seeded socketed-module assembly over the lots:
  wallSolid/wallWindow/doorway/floor/roofFlat/roofRaked per storey; door ALWAYS on a `Lot` frontage
  edge — the Lot struct carries FrontS/E/N/W exactly for this), then Q2c `BuildingBuilder` (Editor
  static T-Dog calls from CityBuilder's district pass) + the three BUILDING_* gates.
- **Verified facts (don't re-derive):** world factory chain = `WorldLayoutLibrary` (create-only
  authors) → `CityLayoutDefinition` (+`experience` since P1) → `WorldStubGenerator`/`CityBuilder` →
  `WorldExperienceBuilder`/`WorldPoiBuilder`/`WorldDressingBuilder`. POI verbs: CombatCamp/HarvestGrove/
  MachineSite/RuinCache/CaveSecret/StoryAnchor/TravelBerth. Quality gates live in
  `Editor/Audit/ExperienceAuditRules.cs` (7 blockers) — new gates follow that file's pattern.
  Registries for validator lookups: `Resources/Items` (ItemFactory), `Resources/Enemies`
  (CreatureDefinition), `Resources/Bots`, `Resources/Garden`, `SkyVistaLibrary.Specs()`,
  `ForgeRecipeLibrary`. No LFS/Addressables/NavMesh/pooling exists (deliberate — see design doc
  deferrals).
- **Branch:** `terry-local-wip`; three other tracks push often — pull --rebase before every push.

---
*Opened 2026-07-03 by the architect (Fable 5) from Terry's PDF directive; plan approved.*

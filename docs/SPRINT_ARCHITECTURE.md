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
| Q2a | **`LotPartitioner`** (pure, seeded, ~15 tests): OBB recursive subdivision of district rects → lots + right-of-ways; base cases min-area/frontage/aspect w/ probabilistic acceptance | ⬜ |
| Q2b | **`BuildingGrammar`** (pure, seeded, ~20 tests): socketed modules (wallSolid/wallWindow/doorway/floor/roofFlat/roofRaked/cornerTrim/balcony), WFC-lite lowest-entropy assembly, 1–3 storeys, door-always-on-frontage proven by socket design. `BuildingStyleDefinition` = data | ⬜ |
| Q2c | **`BuildingBuilder`** (Editor, static entry T-Dog calls from CityBuilder's district pass): grammar output → primitives now / Forge-kit lookup later. Gates: `BUILDING_DOOR_BLOCKED` · `LOT_OVERLAP` · `BUILDING_OVER_BUDGET` | ⬜ |
| Q2d | W002 warren proof build + runbook gate ("does it read as a place?") | ⬜ |
| Q3 | **`TerrainField`** (pure, ~15 tests): octaved fBM + domain warp + temp×moisture biome matrix; BiomePreset → parameter set mapping; `TERRAIN_SLOPE_UNWALKABLE` gate. T-Dog swaps WorldExperienceBuilder's height fn (announced) | ⬜ |
| Q4a | **`GamePool`** (Gameplay): pooled spawn/despawn; call-site swaps in PvpBolt/darts/arcs/thump rings/nets (behavior-preserving) | ⬜ |
| Q4b | **`PERF_BUDGET` audit rule**: per-scene tris/materials/renderer-count/lights vs QUEST_ART_AUDIO_PERFORMANCE_BUDGET.md — WARN 80%, BLOCK over caps, exempt `_Boot`. + shader-variant WARN | ⬜ |
| Q5 | Process: LFS-ready commented `.gitattributes` stanzas + V2 laws → CLAUDE.md/HOW_TO_CHANGE_ANYTHING + WorldSpec how-to row | ⬜ |
| — | Close per phase: CI green → APK dispatch → audit green → runbook rows → HANDOFF | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **State:** Q0 + Q1a/b/c shipped — verify CI on this push. Q1d is Terry's one-menu export
  (runbook §2k); until he commits the exported specs, the spec folder is empty and CompileAll is a
  no-op (safe by design).
- **Next action:** **Q2a — `LotPartitioner`** (pure, seeded OBB recursive subdivision + tests), then
  Q2b `BuildingGrammar` (socketed WFC-lite), then Q2c `BuildingBuilder` + gates. The Terry-visible win.
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

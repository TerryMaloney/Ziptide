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
| Q0 | Program docs: ARCHITECTURE_V2 design + this board + HANDOFF (ppp) + PRIORITIES rewrite + tools/*.ps1 ASCII fix (Terry's PS 5.1 parse error) | 🔄 this commit |
| Q1a | **`WorldSpec`** (`Content/Runtime/Spec/WorldSpec.cs`): one serializable class = the whole world declaratively (identity/seed · biome+terrain params · vista · POIs w/ verb+tier+storyAnchor · building style/density per district · creatures · sky id · contracts · garden/socket/mine placements · forge bindings). JSON round-trip via JsonUtility | ⬜ |
| Q1b | **`WorldSpecValidator`** (pure, ~20 tests): actionable errors — unknown ids vs registries, predicate rules (storyAnchor⇒poi exists, POI spacing, seed nonzero, budget ceilings), never silent | ⬜ |
| Q1c | **`WorldSpecCompiler`** (Editor): spec → EXISTING assets (CityLayoutDefinition+.experience, packs, sky assignment, job routing). Feeds the proven factory; generates no geometry itself | ⬜ |
| Q1d | **W002 proof**: express W002 as `docs/worldspecs/W002.spec.json`, compile, diff against today's authored assets (equivalent or explained). `SPEC_DRIFT` WARN rule | ⬜ |
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
- **State:** program opened; Q0 docs in this commit. Head at open: `954b969` (T-Dog P0–P4a +
  Picasso ART-2 all landed — see HANDOFF ooo/nnn; the SPRINT.md board under-reports: P1c–g/P3/P4a ARE
  committed).
- **Next action:** **Q1a+Q1b** — the WorldSpec class + pure validator + tests (one commit), then Q1c
  compiler, then the W002 proof. Then Q2 (buildings) — the Terry-visible win.
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

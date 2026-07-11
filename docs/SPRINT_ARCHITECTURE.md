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
| FH-X01 | **First-hour contract asset** — envelope: `docs/first_hour/envelopes/FH-X01-CONTRACT-ASSET.json`; log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHX01_IMPLEMENTATION_LOG.md`. Deterministic Editor import from approved JSON to one Resources asset; WARN-only drift/invalid/missing audit; no runtime JSON parser. | 🟡 GPT-5.6 CLAIMED 2026-07-11 |
| CI-V1 | **Durable branch CI verdict** — log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_CI_VERDICT/CI_VERDICT_IMPLEMENTATION_LOG.md`. Final isolated CI job writes tested SHA + GREEN/RED to `docs/CI_VERDICT.md`; stale-head/race safe, no Unity job changes, no recursion. | ✅ PROVEN — caught a real red, then recorded GREEN run `29151953887`; FILE CLAIM RELEASED |
| FH-02A | **Continuity manifest + report-only validator** — log: `docs/GPT_ADDITIONS/2026-07-10_GPT56_FIRST_HOUR/FH02A_IMPLEMENTATION_LOG.md`. `docs/continuity/*` + `tools/continuity_gate.py` + stdlib tests + isolated non-blocking report job shipped. No Unity/runtime behavior and no blocker promotion. | 🟡 IMPLEMENTED — first real Actions artifact pending; FILE CLAIM RELEASED |
| Q0 | Program docs: ARCHITECTURE_V2 design + this board + HANDOFF (ppp) + PRIORITIES rewrite + tools/*.ps1 ASCII fix (Terry's PS 5.1 parse error) | ✅ `5b5b1b6` |
| Q1a | **`WorldSpec`** SHIPPED: one serializable class = the whole world (identity/seed · sky/fog/planet · experience terrain+vista · POIs · palette/districts/connections/canals/shipyard · drones/creatures/hazards · collectibles/machines/mines/gardens/sockets · flags · advisory skyVistaId). REUSES the layout/pack [Serializable] classes verbatim — zero mapping drift; JSON round-trip via JsonUtility (enums as ints, tables in WORLD_RECIPE) | ✅ this commit |
| Q1b | **`WorldSpecValidator`** SHIPPED (pure): stable CODE-token errors — identity/seed, POI count/verbs/spacing/bounds/dup + StoryAnchor-when-flags predicate, district/connection/spawn refs, registry checks (creatures/plants/items; null set = permissive), mine/socket economy sanity. **14 tests** | ✅ this commit |
| Q1c | **`WorldSpecCompiler`** SHIPPED (Editor): `CompileAll()` reads `docs/worldspecs/*.spec.json` → validate vs REAL registries → fan out to `<scene>_Layout.asset` + `<scene>_WorldPack.asset` (create-or-update, WorldStubGenerator enriches at build) · `ExportAllFromMenu()` reverse-generates a spec per existing world · `SPEC_DRIFT` WARN on round-trip divergence · BuildAndroid hook (announced append) | ✅ this commit |
| Q1d | **Proof (🔧UNITY)**: Terry runs `Ziptide → Worlds → Export All World Specs (JSON)` once and commits `docs/worldspecs/*.spec.json` — every world gets its editable truth document; from then on "change a world" = edit its spec (fidelity is by-construction: export and compile share the same object graph). Runbook §2k | ⬜ Terry |
| Q2a | **`LotPartitioner`** SHIPPED (pure, seeded xorshift): recursive subdivision of district rects → lots + right-of-ways (perimeter ring + internal avenues); base cases min-area (child-length bound `req`), MinSide, aspect law (violations only when no legal cut existed — provable), **frontage GUARANTEE** (a cut that would landlock a child becomes a street both children front). **11 tests** incl. 25-seed frontage sweep + area conservation | ✅ this commit |
| Q2b/H1 | **`BuildingGrammar` SHIPPED** (pure, seeded): WFC-lite — the DOOR collapses first onto a street-fronted `Lot.Front*` edge (LotPartitioner guarantees one), windows/solids fill seeded, contradiction-free socket set = no backtracking ever. `BuildingStyleDefinition` (SO + `ToData()` mirror, `styleId`/`surfaceFamily` = Art Registry keys) + `BuildingStyleAuthor` (create-only: salvage_row + toxic_tenement → Resources/BuildingStyles, build-hooked). **11 tests** incl. end-to-end partition→plan door law | ✅ this commit |
| Q2c/H1 | **`BuildingBuilder` SHIPPED** (Editor): plans → multi-storey shells w/ ENTERABLE doorways (+`__DOOR` audit markers); modules resolve via **`ArtModuleRegistry`** (`buildingModule:<styleId>/<Module>`, primitive fallback — Picasso's seam). DORMANT wire in CityBuilder's district pass (announced): opts in via new `DistrictDef.buildingStyleId` — zero change to existing worlds until a layout/spec sets it. Gates live: `BUILDING_DOOR_BLOCKED` (raycast from markers) + `BUILDING_OVER_BUDGET` (renderer cap/district); `LOT_OVERLAP` enforced in the pure tests | ✅ this commit |
| Q2d | W002 warren proof: set `buildingStyleId="toxic_tenement"` on one W002 district (via spec once Terry exports, or layout edit) + APK + runbook gate ("does it read as a place?") | ⬜ next operator |
| H3/Q3 | **`TerrainField`** → **ENVELOPE TO T-DOG** (HANDOFF qqq; they have Fable): pure fBM (3–5 octaves over the existing seed-hash idiom) + domain warp + temp×moisture biome matrix; each `BiomePreset` = a parameter set; ~15 tests (same-seed identity, slope bound, output range); swap `WorldExperienceBuilder`'s height fn; `TERRAIN_SLOPE_UNWALKABLE` gate | 📨 T-Dog |
| H2 | **`RoomPartitioner`** → **ENVELOPE TO T-DOG**: BSP interior rooms + corridors carved walking back up the tree; LotPartitioner (`Content/Runtime/City/LotPartitioner.cs`) is the template (same Rng struct, same min-area/aspect base cases, + an access rule: every room reaches a corridor); ~12 tests incl. full-reachability; feeds ship decks / hero interiors / hive worlds | 📨 T-Dog |
| H5 | **`ScatterField`** → **ENVELOPE TO T-DOG**: pure Poisson-disk scatter, density-by-channel (biome-matrix ready), exclusion masks (pads/corridors/POIs/route), per-kind min spacing; ~10 tests; swap into `WorldDressingBuilder`; then a `scatterSpec` WorldSpec v2 field (this track adds the spec field after) | 📨 T-Dog |
| Q4a | **`GamePool` TOOL SHIPPED** (Opus 4.8): pure `Core/Runtime/PoolCore.cs` (generic free-list, Created/Reused/Live/Free bookkeeping, retained cap, get/release hooks) + **10 EditMode tests** + `Core/Runtime/GamePool.cs` (GameObject wrapper: keyed pools, park-under-inactive-root, per-scene reset, `Prewarm`, `LogStats`). Adds files, changes NO existing behaviour. **Scope split (deliberate):** the ~8 live call-site swaps (PvpBolt/darts/arcs/rings/nets) are device-sensitive combat/VR files in the MP/Gameplay lanes — enveloped to those lanes (HANDOFF, exact sites) rather than swapped from the architecture chair. This is the dependency the bank's pooled-mover ideas name (INDUSTRY #2/#19/#25, COMBAT #42, CREATURES #20). | ✅ this commit; adoption 📨 |
| Q4b | **`PERF_BUDGET` audit rule** → **ENVELOPE TO PICASSO** (their budget doc; ExperienceAuditRules is the pattern): tris/materials/renderers/lights per scene, WARN 80% / BLOCK over, exempt `_Boot` | 📨 Picasso |
| H4 | **Art Registry** SHIPPED: `Editor/Art/ArtModuleRegistry.cs` (resolve-through, primitive fallback) + **`docs/design/ART_REGISTRY.md`** (id families + laws + deferral triggers). Picasso fulfills `buildingModule:*` ids (envelope in qqq) | ✅ `cd79dac`+ |
| H6 | **TAKEOVER KIT SHIPPED**: `docs/OPERATOR_START_HERE.md` (model-agnostic manual: blackboard, envelopes, circuit breaker, Opus calibration) + CLAUDE.md pointer swap + FABLE5_START_HERE legacy banner + `design/SPACEFLIGHT_PHYSICS.md` (P4b rails) + PRIORITIES rev 4 + HANDOFF (qqq) briefings | ✅ this commit |
| Q5 | LFS-ready commented `.gitattributes` stanzas (only remaining Q5 sliver — laws landed via OPERATOR_START_HERE) | ✅ by T-Dog (HANDOFF rrr): pseudo-binary guards + binaries + LFS stanzas; runbook §2l driver setup |
| — | Close per phase: CI green → APK dispatch → audit green → runbook rows → HANDOFF | recurring |

## ▶ RESUMING? — current state & exact next action
- **ACTIVE CLAIM (2026-07-11): FH-X01** — GPT-5.6 owns only the exact files in `FHX01_IMPLEMENTATION_LOG.md`, including announced append-only BuildAndroid/WorldAuditRunner hooks. Do not edit those files until the claim releases.
- **CI-V1 (2026-07-11): PROVEN, FILE CLAIM RELEASED** — durable verdict caught a real Unity red, then recorded GREEN for `b388f1b` in run `29151953887`. Read `docs/CI_VERDICT.md`; a later normal commit makes the verdict stale until its own run completes.
- **FH-02A (2026-07-10): IMPLEMENTED, FILE CLAIM RELEASED** — report-only continuity manifest/validator/tests and the non-blocking project-contract job are in the branch. First real Actions artifact is still pending; warnings must be reviewed, not fixed blindly. See `FH02A_IMPLEMENTATION_LOG.md`.
- **State (2026-07-03, the LAST Fable architect session):** Q1 + Q2a + H1 buildings + H4 registry +
  H6 takeover kit all shipped; H2/H3/H5 enveloped to T-Dog, Q4b to Picasso (HANDOFF qqq). This
  track's operator is **Opus 4.8 from here** — that is fine by design; read `OPERATOR_START_HERE.md`.
- **Bank pulls shipped (architecture lane):** GamePool (Q4a, `94f74ac` ✅). **WORLDS_50 #13 POI
  reachability** — pure `GridReachability` BFS core (8 tests) + `WorldReachabilityAuditRules`
  (WARN-only: coarse raycast grid → flood from spawn → warn on disconnected POIs; deliberately a
  WARN not a blocker so a coarse-grid false positive can't fail a good build) + one WorldAuditRunner
  line. Complements the H3 TERRAIN_SLOPE_UNWALKABLE blocker (area vs connectivity).
- **Next architecture action after FH-X01 green:** claim `FH-X02-PROGRESSION-CORE`; do not start it early.
- **GamePool adoption (envelope, MP/Gameplay lanes):** swap the CreatePrimitive+Destroy hot spawns to
  `GamePool.Get(key, factory, pos)` / `GamePool.Release(key, go)`. Exact sites: `PvpBolt`,
  `TaserDartProjectile`, creature stun-arc bursts, `ThumpRingVisual`, `StaticNetProjectile`. Each is
  behaviour-preserving; verify visuals identical on device. Gameplay-critical projectiles carry hit
  semantics — do them WITH a device pass, not blind.
- **Circuit breaker applies** (OPERATOR_START_HERE law 5): 3 CI-reds on one task → stop, write up,
  move on.
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

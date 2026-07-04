# 🟡 ACTIVE SPRINT — QUALITY BAR PROGRAM (opened 2026-07-03, post-first-device-test pivot)

> **Takeover prompt: "Read docs/SPRINT.md and continue."** Roadmap: `docs/GAME_PLAN.md`.
> **The approved plan of record:** Terry's first device test said the SYSTEMS work but the EXPERIENCE
> fails — worlds are tiny senseless box-mazes, the ship reads "poor Roblox", menu re-click bug is back,
> entry text unreadable, items drop dead. The fix: keep the world FACTORY, replace the RECIPE.
> Quality targets Terry named: **No Man's Sky** worlds · **Fortnite** fun · **Roblox+** garden/building ·
> **Star Wars** flight. Everything must end up executable by a **mid-level LLM** (data schemas +
> fill-in recipes + build-failing quality gates, never taste-dependent code).
> Prior sprint M4 (ship S1/S2/Quarters/W000): `docs/sprints/` + HANDOFF ddd–ggg. S3 sockets folded
> into P2/P4 below.

## Task board
| # | Task | Status |
|---|------|--------|
| P0.1 | **DevMenu click-once fix** — UI-session rebind on Show + `MENU_CLICK`/`MENU_UI` diags | ✅ `e17eff9` CI-green |
| P0.2 | **DevMenu pager** — 6 worlds/page + PREV/NEXT | ✅ with P0.1 |
| P0.3 | **RILL subtitle readable** — `SubtitleText.Wrap` (5 tests), smaller, lower, fade-in | ✅ `c186afa` |
| P0.4 | **Release feel** — `ReleaseFeel` throw rescue + pulse + FIRST_RELEASE RILL hint | ✅ `749f370` |
| P0.5 | **Pistol visible** — joins starter spawns (3 guns) | ✅ with P0.4 |
| P1a | **Terrain, not slabs** — heightfield 240–320m, 5 biome presets, cliff bowl, slope clamp, pads/corridors graded in | ✅ `27f3ef9` CI-green |
| P1b | **Arrival vista** — 5 hero-landmark kits + midground + spawn faces it + fog auto-thin | ✅ with P1a |
| P1f | **QUALITY GATES** — WORLD_TOO_SMALL / TERRAIN_MISSING / NO_VISTA_LANDMARK (`657086a`) + POI_COUNT_LOW / VERB_VARIETY_LOW / STORY_ANCHOR_MISSING / EST_PLAY_MINUTES_LOW (`4c04320`) — all build-failing | ✅ |
| P1c | **POI system** — PoiDef + 7 pocket builders + poi_ markers + standard 8-POI ring + pure PoiQuality (5 tests) | ✅ `4c04320` |
| P1d | **Breadcrumb route** — cairns every 20m along spawn→POIs route | ✅ `45f5bf5` |
| P1e | **Dressing/scatter** — biome-keyed prop clusters, masked, static-batched | ✅ `45f5bf5` |
| P1g | **Re-recipe W002–W012** — contracts route through POIs (GoPoi/PickupAtPoi/MachineAtPoi/MineAtPoi); W000 stays interior | ✅ this commit |
| P3 | **Garden** (GardenPlotRuntime plant→grow→harvest at HarvestGrove POIs) + **BuildSocket** pay-to-build extractors persisted in WorldState | ✅ `a17d44a` CI-green |
| P4a | **Interim ship hull** — 19-part `ShipHullBuilder` silhouette replaces the 4-cube hull | ✅ `954b969` CI-green |
| P5 | **docs/WORLD_RECIPE.md** — the mid-level-LLM handbook, worked W005 example + spec-first chapter (V2 Q1 aligned) | ✅ this commit |
| — | — **TAKEOVER (2026-07-04): architect is Opus now; T-Dog owns everything but art. HANDOFF (qqq) envelopes below are THE queue.** — | — |
| H3 | **`TerrainField`** (envelope qqq): pure fBM 3–5 octaves + domain warp + temp×moisture biome matrix (each BiomePreset = parameter set); ~15 tests (same-seed identity, slope bound, output range); swap `WorldExperienceBuilder` height fn; `TERRAIN_SLOPE_UNWALKABLE` gate | ✅ this commit: TerrainField (fBM+warp+climate, 15 tests, walkable-fraction contract measured empirically) + builder swap + TERRAIN_SLOPE_UNWALKABLE gate |
| H2 | **`RoomPartitioner`** (envelope qqq): BSP rooms + corridors carved walking back up the tree; template = `Content/Runtime/City/LotPartitioner.cs` (same Rng struct + base cases + access rule); ~12 tests incl. full reachability; feeds ship decks / hero interiors | ✅ this commit: BSP + access-rule corridors (room-center endpoints ⇒ connectivity by induction), 12 tests incl. 25-seed sweep + odd footprints, all contracts pre-verified by simulation |
| H5 | **`ScatterField`** (envelope qqq): pure Poisson-disk, density channels, exclusion masks (pads/corridors/POIs/route), per-kind min spacing; ~10 tests; swap into `WorldDressingBuilder` | ✅ this commit: Bridson Poisson + moisture density channel (TerrainField.Climate) + capsule/disc masks + per-kind spacing, 11 tests, dressing swap done |
| Q2d | **Building proof**: set `buildingStyleId="toxic_tenement"` on one W002 district → APK dispatch → runbook "does it read as a place?" gate | ✅ **APK run `28684427359` GREEN (77 MB artifact)** — W002 GalleryB wears real buildings; Terry's §2n walk is the open gate |
| SPWN | **THE SPAWN WAVE — closed (HANDOFF ttt→vvv):** two layers, BOTH audit-check bugs — wrong marker (POIs plant `poi_*` markers) then ankle-height sphere always grazing ground. Fixes: `__SPAWN_PLAYER` by name · all overlaps listed · check at torso height · spawn-Y/POI-exclusion hygiene retained | ✅ `28684427359` green |
| P4b | **S4 flight scene v1** — READ `docs/design/SPACEFLIGHT_PHYSICS.md` FIRST; comfort-capped cockpit flight in a bounded SpaceLane scene (world moves, never the camera; NO floating origin — trigger not fired); pure `FlightModel` core + tests before the scene translator | ⬜ last big piece |
| ML | **META-LOOP ARCHITECTURE LOCK** (Terry's brief + GPT addendum; **build plan = `docs/design/ZIPTIDE_META_LOOP.md`**): ResourceDefinition registry + RESOURCE_ID_UNREGISTERED gate · transaction LEDGER + RewardRouter (8 chokepoints wired — the mode contract) · RecipeDefinition factory fields · ProductionGraph (validate/tick/capped catch-up) + WorldState.factory · save schemaVersion v2 + migration fixture · EconomyFlowModel + NO_SOURCE/NO_SINK/UNUSED warns + generated ECONOMY_FLOW_REPORT · **GoldenMetaLoopTests = the acceptance test**. Conquest command-model + income routing ENVELOPED to architecture track; proxy kits + Forge-staleness glue ENVELOPED to Picasso | ✅ this commit |
| A1 | **Succession gap-closures from Terry's PDF** (triage in HANDOFF rrr): `.gitattributes` (+ runbook UnityYamlMerge driver setup for Terry) · 2 new deferral records in `ARCHITECTURE_V2.md` (runtime asset streaming, gateway rate-limiting) · metavr MCP evaluation runbook item | ✅ this commit |

## ▶ RESUMING? — current state & exact next action
- **Current:** **P0–P5 of the Quality Bar Program COMPLETE and CI-green** (menu/subtitle/release
  fixes · terrain+vista · POIs+gates · route+dressing · contracts-through-POIs · garden+sockets ·
  interim hull · handbook). Rebased onto architect's takeover kit `a21fffb`; read
  **`docs/OPERATOR_START_HERE.md`** — its laws (incl. THE CIRCUIT BREAKER) govern this board too.
- **Next action:** **P4b — S4 flight scene v1** (the last ⬜ on this board). READ
  `docs/design/SPACEFLIGHT_PHYSICS.md` FIRST — the rails are law (world moves, never the camera;
  never parent the rig to the hull; NO floating origin). Build order: pure `FlightModel` core
  (thrust/damping/comfort caps, seeded, ~12 EditMode tests FIRST) → SpaceLane bounded scene via a
  patcher → cockpit translator reading FlightModel → runbook 🎮 item. Follow LotPartitioner→H2 as
  the worked example of "pure core first, translator second."
  **Meta-loop follow-ups live in `docs/design/ZIPTIDE_META_LOOP.md`** (conquest command
  model → architecture track; proxy kits → Picasso; ecology/mutation content → next wave).
- **Device gate:** Terry §2j/§2k runbook rows still open — his ❌s re-prioritize everything.
  §2n (NEW) = the Q2d "does it read as a place?" gate on W002 GalleryB.
- **Lane note (2026-07-04):** T-Dog's Fable ended; Picasso (art, last Fable session) covered Q2d
  and owns cross-track triage until their window ends too — after that ANY capable model resumes
  any track per `OPERATOR_START_HERE.md`.
- **Branch:** `terry-local-wip`. Q2d head: this commit.

## Working rules (unchanged)
CI green per push; SHIPS.md guardrails are law (no rig parenting, no TravelCoordinator bypass, comfort
first — never move the camera); TextMesh only (menus use TMP UGUI already in DevMenu — dev-only file);
.meta per new file; pull --rebase before push; report-only zones need Terry's sign-off.

---
*Quality Bar Program opened 2026-07-03 by T-Dog after Terry's first full device test.*

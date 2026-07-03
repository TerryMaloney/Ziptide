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
| P3 | **Garden** (HarvestGrove plots: plant→tend→morph→harvest; PlotState/GardenService backend EXISTS) + **BuildSocket** machine placement persisted in WorldState | ⬜ |
| P4a | **Interim ship hull** — 15–20 part procedural silhouette replaces the 4-cube hull | ⬜ |
| P5 | **docs/WORLD_RECIPE.md** — the mid-level-LLM fill-in handbook, worked W005 example | ⬜ ships with P1g |
| P4b | **S4 free-flight scene** (SpaceLane; comfort-capped cockpit flight) | ⬜ last |

## ▶ RESUMING? — current state & exact next action
- **Current:** **P0 + P1 (a–g) COMPLETE** — the World Experience Engine is live end to end: terrain →
  vista → POIs → route → dressing → gates → contracts through POIs. CI green through `45f5bf5`;
  P1g is this commit. Terry's §2j runbook pass ("does it feel like a world?") is queued and will
  re-prioritize everything.
- **Next action:** **P3 garden** — promote GardenPlotRuntime at HarvestGrove POIs (backend
  PlotState/GardenService exists + tested; build the world-object layer mirroring MiningRigRuntime:
  serialized def per gotcha #7, JobDirector-style pack data or direct scene placement at the grove's
  Planter pads). Then BuildSocket at MachineSite plinths (persist in WorldState like mines). Then P4a
  interim ship hull (`ShipHullBuilder`, 15–20 parts replacing the 4-cube body in
  `CityBuilder.BuildShipyard`). Then P5 `docs/WORLD_RECIPE.md`. Then P4b S4 flight scene. APK dispatch
  (workflow_dispatch on ci.yml) after P3+P4a to exercise the quality gates end-to-end.
- **Device gate:** Terry §2j on next sideload — his ❌s re-prioritize before P2 encounter depth.
- **Lane:** architect = PvP/arenas/bots (their A4 arsenal doubles as story-world weapons); Picasso =
  asset creator (ship hull mesh is their first big target — P4a is the stopgap), SkyVistas, audio.
- **Branch:** `terry-local-wip`. CI-green head: `45f5bf5` (P1g pending CI).

## Working rules (unchanged)
CI green per push; SHIPS.md guardrails are law (no rig parenting, no TravelCoordinator bypass, comfort
first — never move the camera); TextMesh only (menus use TMP UGUI already in DevMenu — dev-only file);
.meta per new file; pull --rebase before push; report-only zones need Terry's sign-off.

---
*Quality Bar Program opened 2026-07-03 by T-Dog after Terry's first full device test.*

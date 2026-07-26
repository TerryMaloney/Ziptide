# 🟡 ACTIVE SPRINT — QUALITY BAR PROGRAM (opened 2026-07-03, re-affirmed 2026-07-18, post-first-device-test pivot)

> **Takeover prompt: "Read docs/SPRINT.md and continue."** Roadmap: `docs/GAME_PLAN.md`.
> **CURRENT cross-project order:** `docs/CURRENT_EXECUTION_CHECKLIST.md`. Read it before treating an
> older unchecked row here as current; implementation logs and `CI_VERDICT.md` override stale prose.
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
| FH-S01 | **First-hour observation adapter** — read-only LOOK/MOVE/W001-arrival observation; no rig mutation, profile writes, travel or presentation ownership. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS01_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29160235239`; device evidence pending; FILE CLAIM RELEASED |
| FH-S02 | **First-hour holster adapter** — existing accepted holster selection to semantic completion and canonical flag; no socket-rule, travel, input or autosave ownership. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS02_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29160898205`; device evidence pending; FILE CLAIM RELEASED |
| FH-S03 | **First-hour travel adapter** — existing canonical `TRAVEL_OK` after XRI readiness + inventory restoration to first/return signals. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS03_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29161731580`; device evidence pending; FILE CLAIM RELEASED |
| FH-M01 | **Wrist-scanner result adapter** — immutable exact `IScannable` snapshot once per real pulse, including empty; no scanner feel/filter/presentation changes. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHM01_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29162763650`; FILE CLAIM RELEASED |
| FH-S04 | **First-hour repair/scan adapter** — existing repair stages + exact scannable identity; no second scanner or repair state. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS04_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29163421229`; device evidence pending; FILE CLAIM RELEASED |
| FH-S05 | **First-hour creature-resolution adapter** — existing non-lethal disable to neutral owner event; no damage/reward/ecology/respawn/visual changes. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS05_IMPLEMENTATION_LOG.md`. | ⏸ RELEASED UNBUILT; WAIT for Picasso-owned `FH-A01-SIGNATURE-CREATURE-PRESENTATION` |
| FH-S06 | **First-hour zipline adapter** — exact designated-line arrival; release/wrong line do not count; existing ride owner unchanged. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS06_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29164079292`; device evidence pending; FILE CLAIM RELEASED |
| FH-S07 | **Home/W000 surfaces** — New/Continue/Settings, device comfort presets, W000 console/bunk/first helm; existing save/travel/locomotion/PUNCH IT owners preserved. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_FIRST_HOUR/FHS07_IMPLEMENTATION_LOG.md`. | 🟡 RE-DATED **2026-07-26** (HANDOFF rb112) — CODE + UNITY CI GREEN run `29165306485`; **device FAIL 2026-07-25: this row's own boot surface (New/Continue/Settings) was unreachable — anchor fix shipped `54e23022`, MISS_LEDGER #18**; Terry author bake + headset evidence now pending on the NEXT Golden candidate; FILE CLAIM RELEASED |
| FH-A01 | **Signature-creature presentation** — W001 species passport + review artifacts; Picasso-owned. | ⏸ PICASSO QUEUE; unblocks FH-S05 |
| FH-S08 | **W001 final orchestration** — director, all 15 teaching lines, hesitation hints, instance filters, reload resume, visible ship payoff. | ⛔ LAST; waits for every dependency and required bake/device evidence |
| ASYNC | **Async destination loading behind THE ZIPTIDE crest** — one TravelCoordinator owner, 0.9 activation hold, 20s timeout escape. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_ASYNC_TRAVEL/ASYNC_TRAVEL_IMPLEMENTATION_LOG.md`. | ✅ UNITY CI GREEN run `29167548682`; device frame-pacing comparison pending; FILE CLAIM RELEASED |
| P0.6 | **Controller-free DevMenu access** — Y+B removed; F2 in Editor, headset-native two-controller forehead gesture in development builds, ADB optional backup, zero gameplay buttons reserved. Log: `docs/GPT_ADDITIONS/2026-07-11_GPT56_DEV_MENU/DEV_MENU_ACCESS_LOG.md`. | ✅ UNITY CI GREEN run `29154553182`; headset feel check pending; FILE CLAIM RELEASED |
| FH-01D | **Opus lane launch kit** — log: `docs/GPT_ADDITIONS/2026-07-10_GPT56_FIRST_HOUR/FH01D_IMPLEMENTATION_LOG.md`. Four validated copy-paste takeover prompts assign all 12 FH-01C envelopes exactly once, name first claims/blockers, and enforce ownership/stop conditions. No runtime changes. | ✅ IMPLEMENTED; launch-kit contract reports green on later runs; FILE CLAIM RELEASED |
| FH-01C | **Owner-specific first-hour adapter envelopes** — log: `docs/GPT_ADDITIONS/2026-07-10_GPT56_FIRST_HOUR/FH01C_IMPLEMENTATION_LOG.md`. Twelve machine-readable lane envelopes cover 18 non-direct beats and all 15 teaching lines. | ✅ IMPLEMENTED; contract reports green; FILE CLAIM RELEASED |
| FH-01B | **First-hour binding inventory** — evidence-map all 22 completion signals + all teaching lines to existing files/tokens. | ✅ IMPLEMENTED; contract reports green; FILE CLAIM RELEASED |
| FH-01A | **First-hour beat contract** — `docs/first_hour/*` + validator/tests; 22 beats, 15 verbs exactly once. | ✅ IMPLEMENTED; contract reports green; FILE CLAIM RELEASED |
| P0.1 | **DevMenu click-once fix** — UI-session rebind on Show + `MENU_CLICK`/`MENU_UI` diags | ✅ `e17eff9` CI-green |
| P0.2 | **DevMenu pager** — 6 worlds/page + PREV/NEXT | ✅ with P0.1 |
| P0.3 | **RILL subtitle readable** — `SubtitleText.Wrap` (5 tests), smaller, lower, fade-in | ✅ `c186afa` |
| P0.4 | **Release feel** — `ReleaseFeel` throw rescue + pulse + FIRST_RELEASE RILL hint | ✅ `749f370` |
| P0.5 | **Pistol visible** — joins starter spawns (3 guns) | ✅ with P0.4 |
| P1a | **Terrain, not slabs** — heightfield 240–320m, 5 biome presets, cliff bowl, slope clamp, pads/corridors graded in | ✅ `27f3ef9` CI-green |
| P1b | **Arrival vista** — 5 hero-landmark kits + midground + spawn faces it + fog auto-thin | ✅ with P1a |
| P1f | **QUALITY GATES** — WORLD_TOO_SMALL / TERRAIN_MISSING / NO_VISTA_LANDMARK + POI_COUNT_LOW / VERB_VARIETY_LOW / STORY_ANCHOR_MISSING / EST_PLAY_MINUTES_LOW — all build-failing | ✅ |
| P1c | **POI system** — PoiDef + 7 pocket builders + poi_ markers + standard 8-POI ring + pure PoiQuality | ✅ `4c04320` |
| P1d | **Breadcrumb route** — cairns every 20m along spawn→POIs route | ✅ `45f5bf5` |
| P1e | **Dressing/scatter** — biome-keyed prop clusters, masked, static-batched | ✅ `45f5bf5` |
| P1g | **Re-recipe W002–W012** — contracts route through POIs; W000 stays interior | ✅ |
| P3 | **Garden** + **BuildSocket** pay-to-build extractors persisted in WorldState | ✅ `a17d44a` CI-green |
| P4a | **Interim ship hull** — 19-part silhouette replaces 4-cube hull | ✅ `954b969` CI-green |
| P5 | **WORLD_RECIPE handbook** — worked W005 example + spec-first chapter | ✅ |
| H3 | **TerrainField** fBM+warp+climate + builder swap + slope gate | ✅ |
| H2 | **RoomPartitioner** BSP + access corridors | ✅ |
| H5 | **ScatterField** Poisson + masks + dressing swap | ✅ |
| Q2d | **Building proof** W002 GalleryB wears real buildings | ✅ APK green; Terry visual gate open |
| SPWN | **Spawn wave fixes** — correct marker + torso-height overlap checks | ✅ |
| P4b | **Flight/SpaceLane program** — core, translator, controls, combat and mission integration | ✅ CODE SHIPPED; Terry first bake/device pass open |
| ML | **Meta-loop architecture** — registries, ledger, RewardRouter, production graph, economy reports/gates | ✅ |
| A1 | **Succession gap closures** — git attributes, deferrals, MCP runbook | ✅ |
| — | — **STORY BIBLE LOCK — CLOSED 2026-07-06.** — | ✅ |
| SBL1 | Canon lock: `STORY_BIBLE.md` + `THE_TRANSMISSION.md` | ✅ `9d59f84` |
| SBL2 | Continuity fixes | ✅ `169d36c` |
| SBL3 | Ch.8–12 endgame rewrite | ✅ `cfe1564` |
| SBL4 | DLC consistency + continuity audit | ✅ `de717ca` |
| SBL5 | M5 prerequisite and close | ✅ `de717ca` |
| — | — **THE SOUL PASS — CLOSED 2026-07-06.** — | ✅ |
| SOUL1 | Cal/RILL relationship and ambient-line voice guide | ✅ `8bb00a2` |
| SOUL2 | Named-secondary-voices / no-faction-mouthpieces laws | ✅ `8bb00a2` |
| SOUL3 | Mara quoted arc | ✅ `7bc23e3` + `fc07d40` |
| SOUL4 | Sable quoted arc | ✅ `7bc23e3` + `fc07d40` |
| SOUL5 | Aegis-Nine/Nine arc | ✅ `7bc23e3` + `fc07d40` |
| SOUL6 | Close | ✅ |
| — | — **DEPTH PASS 2 — CLOSED 2026-07-06.** — | ✅ |
| D2-1 | `RillLine.speaker` + subtitle formatting/tests | ✅ `ea00f2c` |
| D2-2 | Cal authored lines | ✅ `ea00f2c` |
| D2-3 | Voice pipeline guide | ✅ `ef1b27c` |
| D2-4 | Sky mapping + W057 staging | ✅ `f4e57cb` |
| D2-5 | W007 sky retune; Terry reseed queued | ✅ `f4e57cb` |
| D2-6 | Close | ✅ |

## ▶ RESUMING? — current state & exact next action

- **Start with `docs/CURRENT_EXECUTION_CHECKLIST.md`.** This board is the general gameplay/history detail; the current checklist is the cross-project status layer.
- **No active GPT protected-file claim at this reconciliation commit.** Async travel is closed and its claim released.
- **Concurrent art lane:** Picasso/Opus 4.8 owns `Visuals/**`, Forge/art authoring, water, materials, meshes, shaders, art patchers/audits, and `SPRINT_ART.md`.
- **First hour:** X01/X02, S01/S02/S03, M01, S04, S06 and S07 are code/CI green. S07 awaits Terry bake/device. Picasso FH-A01 is next on the art lane; it unblocks S05. S08 is last and waits for all dependency/device evidence.
- **General latest completed item:** async travel is code/CI green (`c7b5d52`, run `29167548682`); Terry compares five travels and checks `TRAVEL_TIMEOUT`, restoration, and frame pacing.
- **Safe independent queue while Terry/Picasso are away:** reconcile stale status docs → UI readability/reach audit → haptic coverage checklist → behavior/catalog breadth gates. Exact order and exclusions: `CURRENT_EXECUTION_CHECKLIST.md` §7.
- **Device gate:** the consolidated Unity/headset batch in `CURRENT_EXECUTION_CHECKLIST.md` §3C and `TERRY_RUNBOOK.md` remains priority zero when Terry reaches the PC.
- **Branch:** `terry-local-wip`; reread the live head before every write.

## Working rules (unchanged)
CI green per push; SHIPS.md guardrails are law (no rig parenting, no TravelCoordinator bypass, comfort
first — never move the camera); TextMesh only (menus use TMP UGUI already in DevMenu — dev-only file);
.meta per new file; pull --rebase before push; report-only zones need Terry's sign-off.

---
*Quality Bar Program opened 2026-07-03 by T-Dog after Terry's first full device test; state reconciled 2026-07-11.*

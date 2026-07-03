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
| P0.1 | **DevMenu click-once fix** — persistent UI session (ray interactors re-register with the CURRENT scene's XRUIInputModule on Show) + `MENU_CLICK`/`MENU_UI` diags | 🔨 in progress |
| P0.2 | **DevMenu pager** — 6 worlds/page + PREV/NEXT (menu was unusably tall) | 🔨 with P0.1 |
| P0.3 | **RILL subtitle readable** — word-wrap ~38 chars, 25% smaller, lower anchor, fade-in | ⬜ |
| P0.4 | **Release feel** — throw-velocity rescue on release + highlight pulse + `ITEM_RELEASE` log + RILL holster hint on first release | ⬜ |
| P0.5 | **Pistol visible** — `pulse_pistol` joins starter-weapon spawns (3 guns, not 2) | ⬜ |
| P1a | **Terrain, not slabs** — heightfield ground 250–400m, seeded biome curve presets (dunes/mesas/canyon/cavern/tide-flats), bounds ring, slope validator | ⬜ |
| P1b | **Arrival vista** — spawn faces a composed vista: 40–80m hero landmark + midground + SkyVista | ⬜ |
| P1f | **QUALITY GATES** — audit rules that FAIL the build: WORLD_TOO_SMALL / POI_COUNT_LOW / NO_VISTA_LANDMARK / VERB_VARIETY_LOW / EST_PLAY_MINUTES_LOW / STORY_ANCHOR_MISSING | ⬜ (right after P1a+b) |
| P1c | **POI system** — PoiDef {type,pos,tier}; builders: CombatCamp, HarvestGrove, MachineSite, RuinCache, CaveSecret, StoryAnchor, TravelBerth; contracts route through POIs | ⬜ |
| P1d | **Path/breadcrumb network** — graded main route + edge lights linking arrival→POIs→StoryAnchor | ⬜ |
| P1e | **Dressing/scatter pass** — density-graded biome props + ambient motion, masked off paths/POIs | ⬜ |
| P1g | **Re-recipe W002–W012** through the new engine (W000 stays interior) | ⬜ |
| P3 | **Garden** (HarvestGrove plots: plant→tend→morph→harvest; PlotState/GardenService backend EXISTS) + **BuildSocket** machine placement persisted in WorldState | ⬜ |
| P4a | **Interim ship hull** — 15–20 part procedural silhouette replaces the 4-cube hull | ⬜ |
| P5 | **docs/WORLD_RECIPE.md** — the mid-level-LLM fill-in handbook, worked W005 example | ⬜ ships with P1g |
| P4b | **S4 free-flight scene** (SpaceLane; comfort-capped cockpit flight) | ⬜ last |

## ▶ RESUMING? — current state & exact next action
- **Current:** Program just opened. Approved plan: `/root/.claude/plans/` copy is transient — THIS file
  + `docs/GAME_PLAN.md` are the durable record. Working P0.1+P0.2 (one commit: `DevMenu.cs`).
- **Next action:** finish DevMenu (persistent EventSystem/XRUIInputModule fallback + ray re-register on
  Show + pager), then P0.3 (`RillCompanion.DriveSubtitle` wrap/fade), then P0.4+P0.5, one commit each,
  CI-green between. Then P1a+b in `WorldExperienceBuilder` (new file) + additive fields on
  `CityLayoutDefinition` (ANNOUNCE in HANDOFF — shared with architect's arenas).
- **Device gate:** after P0 + P1a/b land, Terry does the "does it feel like a world?" pass BEFORE we
  scale P1c–g across all worlds.
- **Lane:** architect = PvP/arenas/bots (their A4 arsenal doubles as story-world weapons); Picasso =
  asset creator (ship hull is their first big target), SkyVistas, audio. I own P0/P1/P3/P4/P5.
- **Branch:** `terry-local-wip`. CI-green head at open: `b9def1c`.

## Working rules (unchanged)
CI green per push; SHIPS.md guardrails are law (no rig parenting, no TravelCoordinator bypass, comfort
first — never move the camera); TextMesh only (menus use TMP UGUI already in DevMenu — dev-only file);
.meta per new file; pull --rebase before push; report-only zones need Terry's sign-off.

---
*Quality Bar Program opened 2026-07-03 by T-Dog after Terry's first full device test.*

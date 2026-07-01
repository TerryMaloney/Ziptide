# 🔄 SPRINT — THE MODULARITY SPRINT (live state — read this FIRST if you're taking over)

> **TAKEOVER PROMPT IS ONE LINE:** *"Read `docs/SPRINT.md` and continue the sprint."* This file is the
> crash-resume state: what the sprint is, what's done, what's in flight, and the exact next action.
> **The operator updates this file in the same commit as every push.** If usage dies mid-task, the next
> model (T-Dog-account Fable 5, or Opus) resumes here with zero context loss.

**Sprint goal (Terry, 2026-07-01):** make every system modular (worlds/weapons/sky/creatures/ships —
changeable via data without breaking anything), build max graybox story-world content, document it all in
`HOW_TO_CHANGE_ANYTHING.md` so **Opus can finish the game without breaking it**. Full plan approved by
Terry; constraints: everything ⚙CI-verified, small commits, don't touch rig/PvP files awaiting Terry's
device pass, batches sized so a cutoff never strands half-done work.

---

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | `SPRINT.md` (this file) + START_HERE pointer | 🔄 this commit |
| 1 | `HOW_TO_CHANGE_ANYTHING.md` playbook (skeleton now; +1 section as each task lands) | 🔄 this commit |
| 2 | Per-world sky/theme modularity (`themeSpec` in layout → generator authors Theme+WorldProfile) | ⬜ |
| 3 | Weapon visual/feel data-drive (`ItemDefinition` visual fields + `ItemFactory` fallback) | ⬜ |
| 4 | **WORLD BUILDOUT W002–W012** (`WorldFlowKits` + `WorldLayoutLibrary` + `WorldJobLibrary`; batches a: W002–W004, b: W005–W008, c: W009–W012; APK dispatch after each) | ⬜ **GATED** on APK run `28547683786` succeeding |
| 5 | Ship modular foundation (`ShipDefinition` data SO + `docs/systems/SHIPS.md`) | ⬜ |
| 6 | Creature data modularity (`DroneCombatProfile` variants per biome + `CreatureDefinition` catalog) | ⬜ |
| 7 | Sprint close (HANDOFF/checklist/runbook refresh + final APK green) | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** Task 0+1 (this file + the playbook skeleton), committing now.
- **Next action after this commit:** Task 2 (theme modularity) + Task 3 (weapon fields) — both small
  ⚙CI items, one push, then check CI.
- **Gate to check before Task 4:** APK dispatch run **28547683786** (branch `terry-local-wip`,
  workflow_dispatch on ci.yml) must have `conclusion: success` — it validates the WorldStubGenerator
  build hook + gives the audit baseline. Check via GitHub MCP `actions_get` → `get_workflow_run`. If it
  FAILED: read the failed job log, fix the generated-worlds hook in `BuildAndroid.cs` /
  `WorldStubGenerator.cs` before authoring any worlds.
- **Nothing else is in flight.** Branch `terry-local-wip`; last known CI-green commit: `c855213`
  (124/124 EditMode tests).

## Specs the tasks execute from (don't re-derive)
- Approved sprint plan: mirrored below in "Task specs"; story data: `docs/storyboard/WORLD_DATA.md`
  (W000–W012 records — packId/sceneName/flow/theme/flags/jobs/markers each); flow archetypes:
  `docs/design/WORLD_FLOW_TEMPLATES.md`; city look rules: `docs/design/CITY_DESIGN.md`.
- **Verified type facts** (explored this session — trust these):
  - `VisualThemeProfile`: `groundTint` (Color), `skyGradient` (Gradient), `planet` =
    PlanetSettings{baseColor, accentColor, angularSizeDegrees, distance, direction, rotationSpeed,
    followPlayer}. Applied by `WorldRuntime.Start` → `WorldDirector.ApplyTheme(worldProfile.defaultTheme)`.
  - `ItemDefinition`: itemId, modelPrefab, mass, colliderSizeOverride — NO visual fields yet; ItemFactory
    hardcodes scale/color/grip/muzzle per weapon (Pistol .08/.04/.2 grey · Taser .07/.06/.25 cyan ·
    GravityGun .08/.07/.22 violet; grips ~(0,-0.01,-0.05/-0.06); muzzles z=0.12/0.14/0.15).
  - Audit blockers that matter for generated scenes: SPAWN_MISSING / SPAWN_NO_FLOOR (raycast 6m down from
    +0.2m) / SPAWN_OVERLAP_SOLID (0.35m sphere, floor + below-feet excluded) / SPAWN_TRAPPED (8 rays,
    need ≥2 free) / TRAVEL_DEST_NOT_IN_BUILD. City-only checks (walkway-Y, `__*_ROOT`, mandatory door)
    bind only scenes named `*City*` → generated scenes named `W###_*` avoid them. Generator's placement
    already passes all of these by construction.
  - Drone variants ALREADY work: `DroneZoneDef.variantId` → loads `DroneCombatProfile` from
    `Resources/Enemies/<variantId>` in `CityBuilder.MakeDrone`.
  - `CreatureDefinition` (archetype/maxHealth/moveSpeed/damage/biomeId/loot/shockable) exists, consumed
    by NOTHING at runtime yet (Phase E).

## Task specs (condensed — enough to execute without the chat)
- **T2 themes:** add optional `themeSpec` block to `CityLayoutDefinition` (additive fields: skyTop,
  skyHorizon, groundTint, fog on/color/density already exist top-level; planet on/base/accent/size).
  `WorldStubGenerator.Populate` authors `<Scene>_Theme` (VisualThemeProfile from spec) +
  `<Scene>_WorldProfile` (clone defaults, set defaultTheme) and wires it to the scene's `WorldRuntime`
  instead of the shared DefaultWorldProfile. Preserve-on-regen like the pack.
- **T3 weapons:** `ItemDefinition` + `localScale`/`gripLocalPos`/`muzzleLocalPos` (Vector3, zero =
  "keep ItemFactory hardcoded default") + `baseColor` (Color, alpha 0 = keep default) + `overrideVisuals`
  bool if cleaner. ItemFactory reads with fallback. Behavior identical until authored.
- **T4 worlds:** `WorldFlowKits` (Editor): archetype builders city/underground/exterior/interior/coastal/
  void-station returning populated `CityLayoutDefinition`s (districts/heights/connections/canals-as-
  hazard/palette/skyline/fog per biome; heights+palette per CITY_DESIGN). `WorldLayoutLibrary` (Editor):
  W002–W012 specs from WORLD_DATA records (sceneName `W###_Name`, markers for every GoToMarker id as
  hero-building `interiorMarkerId`s, drone zones only where records have combat, themeSpec from records'
  sky lines, spawnStarterWeapons=true on combat worlds). Menu + **self-bootstrap from BuildAndroid**
  (call `WorldLayoutLibrary.EnsureAllAuthored()` before `WorldStubGenerator.EnsureGeneratedInBuildSettings`;
  only creates missing assets — never overwrites Terry's edits). `WorldJobLibrary` (Editor): per-record
  jobs → real step assets + reward + completionFlag; attach to pack; set pack `flagsRequired/flagsGranted`
  (W004 grants `FRAGMENT_T1_FOUND`). W001 = ToxicCity patcher (leave); W000 parked (needs ship system).
- **T5 ships:** `ShipDefinition` SO in Content (shipId/displayName/hullSize/cockpitSeatLocalPos/
  flight params cruise/boost/turnRate/upgrade `ShipSlotDef` list/berth footprint) + `docs/systems/SHIPS.md`
  (architecture: ship = mobile travel station → boarding → cockpit → pick destination →
  `TravelCoordinator.TravelTo`; the locked travel contract is honored; free-flight is a later 🎮 layer).
- **T6 creatures:** editor util authors `Resources/Enemies/<variant>` DroneCombatProfile assets
  (easy/standard/veteran per chapter band); layout library sets zone `variantId`s; author
  `CreatureDefinition` assets for the W002–W012 creature catalog (documented as data-ready, unconsumed).

## Working rules (unchanged)
CI green after every push (red → stop, fix; see `CLAUDE.md`). APK verification = trigger ci.yml
workflow_dispatch on `terry-local-wip` (GitHub MCP `actions_run_trigger`), wait ~20 min, check
`build-android` job + audit. Don't touch: `PlayerRigPersistence`, PvP scene files, XRI samples,
`TravelCoordinator` — Terry's device round verifies those as-is.

---
*Sprint opened 2026-07-01 by the operator (Fable 5, Architect account). When Task 7 closes this file gets
a final "SPRINT COMPLETE" stamp + handoff summary.*

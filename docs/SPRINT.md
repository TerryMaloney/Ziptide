# 🟡 ACTIVE SPRINT — M2: THE JOB IS REAL (opened 2026-07-01)

> **Takeover prompt for any fresh model: "Read docs/SPRINT.md and continue."** Live state, updated in
> the same commit as every push. Roadmap: `docs/GAME_PLAN.md` (this = milestone **M2**). Playbook:
> `docs/HOW_TO_CHANGE_ANYTHING.md`. Prior sprints: `docs/sprints/` (M1 just closed — APK run
> `28581416414` green).

**Sprint goal:** the contract-tech FANTASY becomes hands-on. Machines are repaired with your hands
(panel → part → power), biomes get mechanical hazards, and the economy becomes visible world objects
(mining rig / garden plot you collect from). **Acceptance gate (GAME_PLAN M2):** W002 plays a full
"arrive → collect → repair → paid" loop. All ⚙CI; same constraints as M1 (small commits, don't touch
rig/PvP/XRI samples, CI green per push, APK dispatch at the end).

---

## Task board
| # | Task | Status |
|---|------|--------|
| 0 | Archive M1 sprint; open this one | ✅ `756956e` |
| 1 | **Repair loop**: `RepairableMachine` (3 hands-on stages: grab panel off → seat the part → flip the switch) + `MachineSpawnDefinition` pack data + JobDirector spawn + `RepairMachineCountStepDefinition` + `JobRuntime.ReportRepair` (+ bank, like collect) + `WorldJobLibrary .Repair()/.Machine()` verbs + **W002 finale = repair the cistern pump** + validator guard + 7 tests | ✅ this commit |
| 2 | **Hazard zones**: `HazardZoneDef` list on `CityLayoutDefinition` + `HazardZoneRuntime` (SERIALIZED def — gotcha #7; poll-based; Wind push / Static+Spore ticks / Flood drag / Radiation escalate+shove; slab visual) + `CityBuilder.BuildHazardZones` + authored W003 wind crossings / W005 spore pockets / W010 tide-flat flood | ✅ this commit |
| 3 | **Economy world objects**: `MiningRigRuntime` (binds a `MineState` in this world's `WorldState`; select → `ProfileEconomy.CollectMine` → credits HUD moves; `ZIPTIDE: MINE_COLLECT`) + `GardenPlotRuntime` (PlotState grow/harvest) + pack/layout spawn + W002 gets a mineral rig + tests for any new pure logic | ⬜ |
| 4 | Starter-gear trio (Scan Pulse → Taser → Gravity Glove onboarding) — **DEFERRED to W000/M4** (onboarding order needs the tutorial world; gear itself already exists). Documented here so nobody re-derives. | ⏸ deferred |
| 5 | Close: HANDOFF (bbb), runbook §2d M2 smoke, checklist, MASTER_CHECKLIST, **APK dispatch green** | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** Task 1 committed — the repair loop is LIVE. `RepairableMachine` (pull the
  rusted panel off → the exposed socket shows the fault → fetch the part from where the pack spawned it
  → it snaps in at 0.3 m → flip the switch → lamp goes green) + `MachineSpawnDefinition`/`machines` pack
  list + JobDirector `CreateMachines` + `ReportRepair` + `RepairMachineCountStepDefinition` +
  `JobRuntime` repair BANK (early/pre-accept fixes credit later; blank machineId = any machine, drains
  across banked ids) + `.Machine()/.Repair()` library verbs. **W002's contract now ends with actually
  repairing the cistern pump** (valve spawns back at the shaft — the fetch is the job): the M2 gate loop
  "arrive → collect → repair → paid" is authored. Validator: machine sanity + Repair↔machine guard.
  Tests: `JobRuntimeRepairTests` (5) + 2 validator. Kept W002's final Go("pump_house") BEFORE the
  machine so the objective board walks you there first.
- **Current micro-step (task 2 committed):** hazards live. `HazardZoneDef`+`HazardKind` on the layout;
  `HazardZoneRuntime` holds a **SERIALIZED def** (the generator assigns at edit time — private fields
  don't cross into the saved scene, gotcha #7) and builds bounds+slab in `Awake`; detection = position
  poll (no rig physics coupling); slows go through the self-healing stun path. Authored: W003 crosswind
  bridge lanes, W005 spore pockets, W010 tide-flat flood drag. Layout assets are NOT committed — CI
  re-authors them fresh each build, so builder edits ship (Terry's stale local copies just have empty
  hazard lists until regenerated).
- **Next action:** verify CI → Task 3 (economy world objects): `Gameplay/Runtime/Story/MiningRigRuntime.cs`
  — pack-data spawn (`MiningRigSpawnDefinition`? keep simpler: reuse `MachineSpawnDefinition`? NO — new
  small `[Serializable] EconomySpawnDefinition` {kind Mine|Garden, id, resourceId/plantId, ratePerSecond,
  storageCap, localPosition} + `economy` list on the pack). Rig binds a `MineState` in
  `profile.GetWorld(sceneName).mines` (create if missing, keyed by machineId=id), shows stored on a
  TextMesh, select → `ProfileEconomy.CollectMine` → `ZIPTIDE: MINE_COLLECT amt=…`. Garden: `PlotState`
  plant/harvest via select (GardenService if it fits, else direct PlotState). W002 gets a mineral rig
  near the pump. Library verb `.Mine(id, resourceId, rate, cap, pos)`. Tests for any pure seam
  (MineState binding logic → extract a pure helper if needed).
- **Then:** Task 5 close + APK dispatch.
- **Branch:** `terry-local-wip`. CI-green: `468458b` (task 1). This push pending.

## Specs (verified against code this session — don't re-derive)
- `JobRuntime` step pattern + bank: see `_collectBank`/`ApplyCollectBank` (added in M1) — mirror for repair.
- `WorldPackDefinition` lists: `spawnMarkers` / `collectibles` / `choices` (+ now `machines`); JobDirector
  materializes each at Start (`CreateSpawnMarkers/CreateCollectibles/CreateChoices` — add `CreateMachines`).
- `WorldJobLibrary.Spec`: steps tuple `(kind, markerId, pos, count)` — reuse markerId slot for machineId;
  `pickups` list pattern for machines. Authoring switch in `EnsureJobsFor` (add "repair" branch).
- Economy: `PlayerProfile.GetWorld(worldId, createIfMissing)` → `WorldState{mines, plots}`;
  `ProfileEconomy.CollectMine(profile, mine)` moves stored → balance; `WorldRuntime.Start` already calls
  `EnterWorld` (ECON_RESOLVE) keyed by scene name → **use scene name as worldId** in MiningRigRuntime.
  `MineState{machineId, resourceId, ratePerSecond, stored, storageCap, lastResolvedAtUnix}`.
- `PlayerStunReceiver.ApplyStun(seconds, slowFactor)` = the slow/flash effect hazards reuse (rig-ensured).
- `CityLayoutDefinition` is the layout SO (Content/City) — hazard defs go there so worlds author hazards
  as data; `WorldStubGenerator.PatchActiveSceneIfGenerated` + `Populate` is where scene objects get built.
- TextMesh only (no TMP). New .cs files need .meta (uuid4 hex). Collider BEFORE grab component.

## Working rules (unchanged)
CI green after every push (red → stop, fix — M1's #177 proved the gate works). APK verify =
`actions_run_trigger` on ci.yml, ref terry-local-wip, ~20–30 min, check Build Android APK + artifact.

---
*Sprint opened 2026-07-01 by the operator (Fable 5) — GAME_PLAN M2. On close: ✅ stamp + archive.*

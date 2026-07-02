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
| 2 | **Hazard zones**: `HazardZoneDef` list on `CityLayoutDefinition` (Wind/Static/Flood/Spore/Radiation, center/size/strength) + `HazardZoneRuntime` (trigger volume; push / slow-stun via `PlayerStunReceiver.ApplyStun`; `ZIPTIDE: HAZARD`) + generator spawn + author W003 wind / W005 spore / W010 flood in `WorldLayoutLibrary` + validator/test | ⬜ |
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
- **Next action:** verify CI on this push → Task 2 (hazard zones): `HazardZoneDef` [Serializable] on
  `CityLayoutDefinition` (enum Wind/Static/Flood/Spore/Radiation + center/size/strength) + a `hazards`
  list; `Gameplay/Runtime/World/HazardZoneRuntime.cs` — self-built trigger volume (BoxCollider
  isTrigger + translucent tint), OnTriggerStay against the rig: Wind = steady push (CharacterController
  .Move via rig transform nudge — CAREFUL: move the XR Origin root, not the camera), Static/Spore =
  periodic `PlayerStunReceiver.ApplyStun(0.4f, 0.55f)` ticks, Flood = strong slow while inside,
  Radiation = escalating flash + push-back. `ZIPTIDE: HAZARD kind=… enter/exit`. Spawned by
  `WorldStubGenerator.Populate` from the layout's hazards list. Author: W003 wind lanes between mesas,
  W005 spore pockets under the canopy, W010 flood strips on the tide flats (edit `WorldLayoutLibrary`).
  Validator/test: pure `HazardZoneDef` sanity if a pure seam exists; otherwise author-data eyeball +
  audit reliance (zones are triggers — the spawn-overlap audit ignores them).
- **Then:** Task 3 (mining rig / garden plot) → Task 5 close + APK dispatch.
- **Branch:** `terry-local-wip`. CI-green head: `970be83`; `756956e` (sprint open, docs-only) pending.

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

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
| 0 | Archive M1 sprint; open this one | 🟡 this commit |
| 1 | **Repair loop**: `RepairableMachine` (3 hands-on stages: grab panel off → seat the part → flip the switch) + `MachineSpawnDefinition` pack data + JobDirector spawn + `RepairMachineCountStepDefinition` + `JobRuntime.ReportRepair` (+ bank, like collect) + `WorldJobLibrary .Repair()/.Machine()` verbs + **W002 finale = repair the cistern pump** + tests | ⬜ |
| 2 | **Hazard zones**: `HazardZoneDef` list on `CityLayoutDefinition` (Wind/Static/Flood/Spore/Radiation, center/size/strength) + `HazardZoneRuntime` (trigger volume; push / slow-stun via `PlayerStunReceiver.ApplyStun`; `ZIPTIDE: HAZARD`) + generator spawn + author W003 wind / W005 spore / W010 flood in `WorldLayoutLibrary` + validator/test | ⬜ |
| 3 | **Economy world objects**: `MiningRigRuntime` (binds a `MineState` in this world's `WorldState`; select → `ProfileEconomy.CollectMine` → credits HUD moves; `ZIPTIDE: MINE_COLLECT`) + `GardenPlotRuntime` (PlotState grow/harvest) + pack/layout spawn + W002 gets a mineral rig + tests for any new pure logic | ⬜ |
| 4 | Starter-gear trio (Scan Pulse → Taser → Gravity Glove onboarding) — **DEFERRED to W000/M4** (onboarding order needs the tutorial world; gear itself already exists). Documented here so nobody re-derives. | ⏸ deferred |
| 5 | Close: HANDOFF (bbb), runbook §2d M2 smoke, checklist, MASTER_CHECKLIST, **APK dispatch green** | ⬜ |

## ▶ RESUMING? — current state & exact next action
- **Current micro-step:** sprint opened (this commit archives M1 → `docs/sprints/SPRINT_2026-07-01_M1_STORY.md`).
- **Next action:** Task 1 — the repair loop. Files: (i) `Content/Runtime/WorldPacks/MachineSpawnDefinition.cs`
  ([Serializable]: machineId, displayName, localPosition, partItemId, partLocalPosition); `machines` list
  on `WorldPackDefinition`. (ii) `Content/Runtime/Jobs/RepairMachineCountStepDefinition.cs`
  {machineId (blank = any), count} — mirror the Collect step. (iii) `JobRuntime.ReportRepair(machineId)`
  + a repair BANK exactly like `_collectBank` (machines can be fixed before the step is current).
  (iv) `Gameplay/Runtime/Story/RepairableMachine.cs` — self-built: body + sparking "broken" tint; stage 1
  PANEL = grab the panel plate off (XRGrabInteractable, on selectEntered detach → stage done); stage 2
  PART = a grabbable part spawns at partLocalPosition; when it comes within 0.3 m of the socket → snap +
  consume; stage 3 SWITCH = XRSimpleInteractable lever → machine hums (color/light), calls
  `JobDirector.ReportRepair(machineId)`, `ZIPTIDE: MACHINE_REPAIRED id=…`. Collider-before-interactable
  (gotcha #6); manager wiring + retry (travel-door pattern). (v) JobDirector `CreateMachines()` (Marker_
  pattern) + `ReportRepair` passthrough. (vi) `WorldJobLibrary`: `.Repair(machineId)` step verb +
  `.Machine(machineId, pos, partItemId, partPos)`; **W002**: replace the final Go("pump_house") with
  Machine("cistern_pump", pump-house pos, part "pump_valve" spawned back at the shaft) + Repair step —
  the gate loop. (vii) `WorldPackValidator`: Repair step ↔ machines cross-check (like the Collect guard).
  (viii) Tests: `JobRuntimeRepairTests` (in-order, early-repair bank, machineId filter, any-machine).
- **Then:** Task 2 → 3 → 5. Each commit updates this board + this section.
- **Branch:** `terry-local-wip`. CI-green head: `970be83`.

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

# FH-S04 IMPLEMENTATION LOG — FIRST-HOUR REPAIR / SCAN ADAPTER

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11; dependency cleared by FH-M01 green run `29162763650`  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE — FILE CLAIM POSTED  
**Envelope:** `docs/first_hour/envelopes/FH-S04-REPAIR-SCAN.json`

## Dependencies

- ✅ `FH-X02-PROGRESSION-CORE`
- ✅ `FH-M01-SCANNER-RESULT` — implementation `5685dc8`, combined tested head `002e20b`, CI green

## Claimed files

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairableMachine.cs`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Story/RepairStage.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/RepairableMachineSignalTests.cs` + `.meta`
- this log

## Concurrent-lane boundary

Picasso/Opus 4.8 is active on art/Forge work. FH-S04 claims no `Visuals/**`, Forge, art-authoring, material, mesh, shader, scene, patcher, creature-art, `SPRINT_ART.md`, or Picasso planning file. Live head must be checked before every write; no force-pushes.

## Goal

Expose the existing physical repair progression and exact scanner identity without adding a second repair state machine or scanner:

- `FH_SCAN_FAULT` → `FIRST_MACHINE_FAULT_SCANNED`
- `FH_REPAIR_ACCESS` → `FIRST_MACHINE_ACCESS_OPENED`
- `FH_REPAIR_PART_SEATED` → `FIRST_REPAIR_PART_SEATED`

## Planned additive seam

- Replace only the private enum type with public `RepairStage`; keep the same four values and the same `_stage` field as the single source of truth.
- Add read-only `CurrentStage` and neutral `StageChanged` notification after each established physical transition.
- Keep `IsRepaired` as the same comparison against the same `_stage` field.
- Implement existing `IScannable` directly on `RepairableMachine`:
  - transform = this transform;
  - kind = `Objective`;
  - active until `Running`.
- Add pure identity-match helper in `RepairStage.cs`: a scan matches only when the immutable `WristScanResult` contains the exact designated `RepairableMachine` instance.
- No tutorial fields, completed flags, progression writes, scanner ownership or repair orchestration inside the machine.
- Diagnostic belongs to the adapter consumer: `ZIPTIDE: FIRST_HOUR_MACHINE_SCAN id=<id> matched=<bool>`.

## Locked exclusions

- no second repair stage/state field;
- no changes to panel grab, part distance/socket seating, power switch, `JobDirector.ReportRepair`, labels, visuals, colliders, XR wiring or reward/job behavior;
- no scanner pulse/filter/radar/tag/haptic/audio/cooldown changes;
- no profile or save writes;
- no TutorialDirector implementation in this envelope;
- no Picasso/art files.

## Planned tests

- exact `Panel → Part → Power → Running` ordering;
- public stage values remain stable;
- `CurrentStage` and `IsRepaired` derive from the same `_stage` source;
- scannable active for Panel/Part/Power and inactive at Running;
- scan identity requires the exact designated machine, not matching ID/name/kind alone;
- empty/unrelated scanner results do not match;
- stage subscriber failure cannot break physical repair transitions;
- source guard preserves panel, seat, power, report-repair and existing diagnostic order;
- no second repair state, tutorial field, profile write or scanner implementation.

## Fallback

Without a scanner subscriber, the full physical repair remains playable. Without a stage subscriber, repair behavior is unchanged. Subscriber exceptions are isolated from the physical owner.

## Collision rule

Do not edit the claimed files until this log is closed or explicitly released.
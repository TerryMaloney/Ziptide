# FH-S01 IMPLEMENTATION LOG — FIRST-HOUR OBSERVATION ADAPTER

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11 (“please continue”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE  
**Envelope:** `docs/first_hour/envelopes/FH-S01-OBSERVATION.json`

## Goal

Add a read-only first-hour observer for three natural player actions:

- `FH_LOOK_AT_RILL` → `PLAYER_LOOKED_AT_RILL`
- `FH_MOVE_IN_QUARTERS` → `PLAYER_MOVED_SAFE_DISTANCE`
- `FH_W001_ARRIVAL` → `W001_ARRIVAL_ORIENTATION_COMPLETE`

The adapter observes only. It never moves or locks the rig, changes locomotion, writes profiles, loads scenes, presents subtitles, or owns RILL.

## Claimed files

New files plus Unity `.meta` files:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationCore.cs`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs`
- `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial.meta`

Coordination only:

- `docs/SPRINT.md`
- this log

No existing gameplay/runtime implementation file is touched.

## Locked behavior

### LOOK

- Existing target name: `__RillOrb`.
- Resettable gaze-cone dwell.
- Missing camera or target is a logged one-time no-op and leaves the beat open.
- Looking outside the cone resets dwell.

### MOVE

- Camera/world pose is sampled read-only; physical room-scale movement and locomotion both count.
- Planar movement only.
- Completion: `1.5 m` cumulative OR `1.0 m` net planar displacement.
- Tracking begins only when `BeginBeat("FH_MOVE_IN_QUARTERS")` is called, so earlier movement cannot count.

### W001 ARRIVAL

- No forced target and no camera/rig mutation.
- Completion: `3 s` valid unrestricted observation plus at least `25°` natural planar head-direction change from the opening direction.
- Tracking begins only when `BeginBeat("FH_W001_ARRIVAL")` is called after canonical travel completion.

## Adapter contract

- Self-bootstraps exactly one `DontDestroyOnLoad` adapter.
- Remains dormant until a recognized beat is activated.
- Exposes `BeginBeat`, `CancelBeat`, active state and `SignalCompleted` event.
- Emits each activated beat once; release/next-beat activation resets its state.
- Logs:
  - `ZIPTIDE: FIRST_HOUR_OBSERVE beat=<id> result=waiting value=<n>`
  - `ZIPTIDE: FIRST_HOUR_OBSERVE beat=<id> result=complete value=<n>`
- The later TutorialDirector owns sequencing and profile writes.

## Planned tests

- gaze cone and dwell reset;
- invalid/missing target vector no-op;
- movement thresholds and frame-rate/path-sampling independence;
- vertical movement ignored;
- arrival requires both dwell and natural head-direction change;
- arrival uses no forced target;
- completion latches once;
- source-level no rig mutation/profile/travel ownership.

## Acceptance

- Pure tests green.
- Unity compile green.
- No rig mutation or input lock.
- Device tuning remains required for cone/dwell feel.

## Collision rule

Do not edit the claimed files until this log is closed. Other lanes may continue outside this scope.

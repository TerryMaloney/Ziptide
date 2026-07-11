# FH-S01 IMPLEMENTATION LOG — FIRST-HOUR OBSERVATION ADAPTER

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11 (“please continue”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ IMPLEMENTED — UNITY CI GREEN; FILE CLAIM RELEASED  
**Envelope:** `docs/first_hour/envelopes/FH-S01-OBSERVATION.json`

## Goal

Add a read-only first-hour observer for three natural player actions:

- `FH_LOOK_AT_RILL` → `PLAYER_LOOKED_AT_RILL`
- `FH_MOVE_IN_QUARTERS` → `PLAYER_MOVED_SAFE_DISTANCE`
- `FH_W001_ARRIVAL` → `W001_ARRIVAL_ORIENTATION_COMPLETE`

The adapter observes only. It never moves or locks the rig, changes locomotion, writes profiles, loads scenes, presents subtitles, or owns RILL.

## Delivered files

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationCore.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial/FirstHourObservationAdapter.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/FirstHourObservationCoreTests.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Gameplay/Runtime/Tutorial.meta`

No existing gameplay/runtime implementation file was touched.

## Shipped behavior

### LOOK

- Existing target name: `__RillOrb`.
- `18°` gaze cone and `0.75 s` continuous dwell.
- Missing camera or target logs once and leaves the beat open.
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
- Emits each activated beat once; next-beat activation resets its state.
- Logs:
  - `ZIPTIDE: FIRST_HOUR_OBSERVE beat=<id> result=waiting value=<n>`
  - `ZIPTIDE: FIRST_HOUR_OBSERVE beat=<id> result=complete value=<n>`
- The later TutorialDirector owns sequencing and profile writes.

## Test coverage

- gaze cone and continuous-dwell reset;
- invalid/missing target vector no-op;
- movement thresholds, vertical exclusion and straight-path sampling independence;
- arrival requires both dwell and natural head-direction change;
- arrival uses no forced target;
- completion latches until explicit reset;
- exact three owned beat/signal mappings;
- source-level guards against rig mutation, travel, profile, input and clock ownership.

## CI proof

- tested SHA: `05cd00fde8fef3e983da14669b9a3b474dd303e3`
- run ID: `29160235239`
- Unity EditMode: `success`
- project-contract reports: `success`
- Android: `skipped` as expected for an ordinary branch push
- overall: `GREEN`

## Device evidence pending

During Terry's next headset session:

1. LOOK completes naturally without requiring exact pixel-perfect aim.
2. MOVE completes after a short natural movement and does not count vertical head bob.
3. W001 arrival completes after looking around naturally; no target, yank or lock appears.
4. Missing-target diagnostics identify any scene-wiring problem.

## Closure

FH-S01 is complete and the Story/Ship file claim is released. The next launch-order envelope is `FH-S02-HOLSTER`.

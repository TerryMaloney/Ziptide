# FH-S06 IMPLEMENTATION LOG — FIRST-HOUR ZIPLINE COMPLETION ADAPTER

**Owner:** GPT-5.6 Thinking, Story/Ship lane  
**Authorized by:** Terry, 2026-07-11 (“go ahead and hit the next thing”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE — FILE CLAIM POSTED  
**Envelope:** `docs/first_hour/envelopes/FH-S06-ZIPLINE.json`

## Dependency

- ✅ `FH-X02-PROGRESSION-CORE`

## Claimed files

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/World/ZiplineRuntime.cs`
- `Ziptide/Assets/Ziptide/Tests/EditMode/ZiplineSignalTests.cs` + `.meta`
- this log

## Concurrent-lane boundary

Picasso/Opus 4.8 remains active on art/Forge/water. FH-S06 claims no `Visuals/**`, Forge, water, materials, meshes, shaders, art patchers, `SPRINT_ART.md`, or Picasso planning files. Live head is rechecked before every write; no force-pushes.

## Goal

Expose the existing zipline ride lifecycle so the first-hour translator can complete `FIRST_JOB_ZIPLINE_USED` only when the designated job zipline reaches its end.

## Planned additive seam

- Add instance `RideStarted` notification adjacent to the existing `ZIPLINE_RIDE_START` log.
- Add instance `RideEnded(reason, progress)` notification adjacent to the existing `ZIPLINE_RIDE_END` log.
- Add neutral exact-instance arrival helper:
  - designated line instance required;
  - reason must equal `arrived`;
  - `released` never counts;
  - missing designated line safely returns false.
- Isolate subscriber failures so they cannot interrupt ride movement or handle reset.
- Preserve the existing `ZiplineRide` core, speed cap, Step call, rig delta translation, release behavior, arrival test and glide-home behavior exactly.

## Locked exclusions

- no changes to ride acceleration, drag, gravity, push-off, speed cap or arrival math;
- no rig parenting or camera movement;
- no comfort/vignette changes;
- no second zipline mover or traversal state;
- no profile/save/progression writes inside `ZiplineRuntime`;
- no tutorial fields inside the runtime;
- no Picasso/art files.

## Planned tests

- ride start publishes once even if BeginRide is called again while active;
- arrived end publishes reason and final progress;
- released end publishes but does not satisfy designated-arrival helper;
- exact designated line identity required;
- missing rig is safe and still publishes lifecycle;
- one throwing subscriber cannot block later subscribers or ride cleanup;
- source guard preserves `ZiplineRide.Step`, `_rig.position += delta`, release/arrival calls and no-parenting law;
- no profile, save, tutorial or second ride owner introduced.

## Fallback

No subscribers means unchanged behavior. Missing designated line or early release leaves the beat open. Subscriber failures are logged and isolated.

## Device evidence pending

Only a full arrival should complete; releasing mid-line must not. Ride comfort and speed must feel unchanged.

## Collision rule

Do not edit the claimed files until this log is closed or explicitly released.
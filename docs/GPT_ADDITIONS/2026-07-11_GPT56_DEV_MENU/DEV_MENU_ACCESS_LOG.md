# DEV-MENU-V2 IMPLEMENTATION LOG — CONTROLLER-FREE ACCESS

**Owner:** GPT-5.6 Thinking, Story/Ship DevTools slice  
**Authorized by:** Terry, 2026-07-11 (“I still need to use the menu from the headset without a computer”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 REOPENED — HEADSET-NATIVE GESTURE ACTIVE

## Problem

The original Y+B summon stole gameplay buttons. The first replacement removed that conflict but made ADB/computer access the only Quest summon path, which does not satisfy headset-only testing.

## Final access model

Reserve **zero controller buttons** while keeping the menu available from any world:

- `UNITY_EDITOR`: F2 toggles the menu.
- Quest development build: hold both tracked controllers close together just above the forehead for two seconds.
- ADB marker-file access remains an optional backup, not the normal headset workflow.
- Shipping builds compile all DevMenu access code out.

The gesture uses controller **pose only**, not buttons, triggers, grips, sticks or gameplay actions. It latches after one activation and must be released before it can toggle again.

## Current claimed files

New:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenuGesture.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/DevMenuGestureTests.cs` + `.meta`

Updated:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
- `tools/tests/test_dev_menu_access_gate.py`
- `docs/DEV_MENU_ACCESS.md`
- `docs/design/CONTROL_SCHEME.md`
- this log

Do not edit those files until this claim releases.

## Gesture contract

The pose is active only when:

1. head, left controller and right controller tracking positions are all available;
2. both controllers are above the head by the configured minimum;
3. both controllers remain close to the head;
4. the controllers are close together;
5. the pose is held continuously for two seconds.

Breaking the pose resets progress. After activation, the gesture stays latched until the pose is released, preventing rapid menu flicker.

## Preserved behavior

- Existing menu canvas, paging, close button, warp path and UI-session rebind remain unchanged.
- `TravelCoordinator`, scenes, prefabs, input-action assets, rig and locomotion are untouched.
- B remains QuickSwap.
- L3 remains sprint/auto-run.
- R3 remains crouch/slide.
- Y remains available for a future player menu.
- The already-green ADB backup remains available.

## Safety behavior

1. Missing tracking data fails closed.
2. Negative/invalid delta time cannot advance the hold.
3. No controller button feature is read.
4. The detector is omitted from non-development builds.
5. Release builds cannot open the DevMenu.
6. Repository regression tests reject face-button/stick-click polling in DevMenu access code.

## Previous verification

The ADB-only foundation compiled green:

- tested SHA: `b070c968f250a5c0cb49cd70c64ba7d65eb0504f`
- run ID: `29153664078`
- Unity EditMode: `success`
- project-contract reports: `success`
- overall: `GREEN`

## Acceptance for this addendum

1. Unity EditMode CI is green with the pose detector and tests.
2. Y+B never affects DevMenu.
3. Holding both controllers above the forehead for two seconds toggles the menu from any world.
4. Keeping the pose held does not retrigger; releasing and repeating toggles again.
5. ADB remains a backup only.
6. Headset feel/range is verified during Terry's next device session.

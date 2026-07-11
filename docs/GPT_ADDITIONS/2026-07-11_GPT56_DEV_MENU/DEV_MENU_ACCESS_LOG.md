# DEV-MENU-V2 IMPLEMENTATION LOG — CONTROLLER-FREE ACCESS

**Owner:** GPT-5.6 Thinking, Story/Ship DevTools slice  
**Authorized by:** Terry, 2026-07-11 (“I still need to use the menu from the headset without a computer”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ IMPLEMENTED — UNITY CI GREEN; HEADSET FEEL CHECK PENDING

## Problem

The original Y+B summon stole gameplay buttons. The first replacement removed that conflict but made ADB/computer access the only Quest summon path, which did not satisfy headset-only testing.

## Final access model

Reserve **zero controller buttons** while keeping the menu available from any world:

- `UNITY_EDITOR`: F2 toggles the menu.
- Quest development build: hold both tracked controllers close together just above the forehead for two seconds.
- ADB marker-file access remains an optional backup, not the normal headset workflow.
- Shipping builds compile all DevMenu access code out.

The gesture uses controller **pose only**, not buttons, triggers, grips, sticks or gameplay actions. It latches after one activation and must be released before it can toggle again.

## Delivered files

New:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenuGesture.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/DevMenuGestureTests.cs` + `.meta`

Updated:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
- `tools/tests/test_dev_menu_access_gate.py`
- `docs/DEV_MENU_ACCESS.md`
- `docs/design/CONTROL_SCHEME.md`
- this log

## Gesture contract

The pose is active only when:

1. head, left controller and right controller tracking positions are all available;
2. both controllers are above the head by the configured minimum;
3. both controllers remain close to the head;
4. the controllers are close together;
5. the pose is held continuously for two seconds.

Breaking the pose resets progress. After activation, the gesture stays latched until the pose is released, preventing rapid menu flicker.

Current tuning constants:

- hold: `2.0 s`
- minimum above head: `0.05 m`
- maximum controller-to-head distance: `0.55 m`
- maximum controller separation: `0.35 m`

These are deliberately broad for easy headset use and may be tuned after Terry's feel check without changing gameplay controls.

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
6. Repository regression tests reject face-button, trigger, grip, menu-button and stick-click polling in DevMenu access code.

## Verification

Exact headset-gesture implementation:

- tested SHA: `ab37cb6ea665dcd2fe4a407287da2bc7bdbdfa84`
- run ID: `29154553182`
- Unity EditMode: `success`
- project-contract reports: `success`
- Android: `skipped` as expected for a normal branch push
- overall: `GREEN`

The tests cover pose geometry, continuous hold timing, release-to-rearm latching, non-retrigger while held, inclusive thresholds and negative delta time.

## Remaining device check

During Terry's next headset session:

1. Press Y and B separately and together — DevMenu must not react.
2. Hold both controllers close together above the forehead for two seconds — DevMenu should open.
3. Keep holding — it must not repeatedly toggle.
4. Release and repeat — it should toggle again.
5. Warp to another world and repeat — menu must still work and remain clickable.
6. Confirm a non-development build contains no DevMenu behavior.

Full usage instructions are in `docs/DEV_MENU_ACCESS.md`.

## Closure

The implementation claim is released. Only headset feel/range tuning remains; no computer is required to summon the menu during normal development testing.

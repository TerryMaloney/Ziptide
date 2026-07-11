# DEV-MENU-V2 IMPLEMENTATION LOG — CONTROLLER-FREE ACCESS

**Owner:** GPT-5.6 Thinking, Story/Ship DevTools slice  
**Authorized by:** Terry, 2026-07-11 (“keep going… Y&B must be gameplay buttons… dev login or something”)  
**Branch:** `terry-local-wip`  
**Status:** ✅ IMPLEMENTED — UNITY CI GREEN; HEADSET CHECK PENDING

## Problem

`DevMenu` summoned from Quest Y+B. B is already the shipped QuickSwap verb and Y is reserved for a future player-facing menu. L3/R3 were also unavailable because the locked control scheme uses them for sprint/auto-run and crouch/slide.

## Final solution

Reserve **zero controller buttons** for developer access.

- `UNITY_EDITOR`: F2 remains the local summon.
- Quest development builds: possession of an authorized ADB connection is the developer-login boundary.
- `tools/dev_menu_access.ps1` writes/removes two marker files under the app's external persistent-data directory:
  - `.ziptide_dev_access` — persistent development unlock.
  - `.ziptide_dev_open` — one-shot open request, deleted by the running app after consumption.
- `DevMenu` polls only in Editor/Development builds.
- Shipping builds still compile `DevMenu` and `DevAccessGate` out.
- No Quest controller feature is read by `DevMenu`.

## Delivered files

New:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevAccessGate.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/DevAccessGateTests.cs` + `.meta`
- `tools/dev_menu_access.ps1`
- `tools/tests/test_dev_menu_access_gate.py`
- `docs/DEV_MENU_ACCESS.md`
- this log

Updated:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
- `docs/design/CONTROL_SCHEME.md`
- `docs/SPRINT.md`

## Preserved behavior

- Existing menu canvas, paging, close button, warp path and UI-session rebind remain unchanged.
- `TravelCoordinator`, scenes, prefabs, input-action assets, rig and locomotion were not touched.
- B remains QuickSwap.
- L3 remains sprint/auto-run.
- R3 remains crouch/slide.
- Y remains available for a future player menu.

## Safety behavior

1. Access fails closed when storage is missing/inaccessible.
2. The access marker is required on Quest development builds.
3. The open marker is one-shot and must be successfully deleted before the menu appears.
4. `Lock` removes both markers; a visible menu hides on the next poll.
5. No password, secret, Android plugin/activity or manifest change was added.
6. Release builds ignore all marker files because the code is absent.

## Verification

Durable CI verdict:

- tested SHA: `b070c968f250a5c0cb49cd70c64ba7d65eb0504f`
- run ID: `29153664078`
- Unity EditMode: `success`
- project-contract reports: `success`
- Android: `skipped` as expected for an ordinary branch push
- overall: `GREEN`

That tested commit includes the controller-free runtime, C# tests, PowerShell helper, documentation and repository-level regression guard. Later commits only removed an accidental unrelated Architecture claim.

Static regression coverage rejects future controller polling tokens in `DevMenu` and verifies marker-name agreement between C# and PowerShell.

## Headset acceptance still pending

On the next development APK/device session:

1. Press Y and B separately and together — the DevMenu must not react.
2. Run `.\tools\dev_menu_access.ps1 -Action Open` — the menu should appear without controller input.
3. Warp, close and open again — UI must remain clickable after travel.
4. Run `-Action Lock` — the visible menu should close and access should be revoked.
5. Confirm a non-development build contains no DevMenu behavior.

Full operator instructions are in `docs/DEV_MENU_ACCESS.md`.

## Closure

P0.6 implementation is complete and the file claim is released. Device acceptance remains a normal runbook check; it does not block other Story/Ship work.

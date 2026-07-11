# DEV-MENU-V2 IMPLEMENTATION LOG — CONTROLLER-FREE ACCESS

**Owner:** GPT-5.6 Thinking, Story/Ship DevTools slice  
**Authorized by:** Terry, 2026-07-11 (“keep going… Y&B must be gameplay buttons… dev login or something”)  
**Branch:** `terry-local-wip`  
**Status:** 🟡 ACTIVE

## Problem

`DevMenu` currently summons from Quest Y+B. B is already the shipped QuickSwap verb and Y is needed for future player-facing controls. L3/R3 are also unavailable as a replacement because the locked control scheme already uses them for sprint/auto-run and crouch/slide.

## Locked solution

Reserve **zero controller buttons** for developer access.

- `UNITY_EDITOR`: F2 remains the local summon.
- Quest development builds: ADB possession is the developer-login boundary.
- `tools/dev_menu_access.ps1` writes/removes two marker files under the app's external persistent-data directory:
  - `.ziptide_dev_access` — persistent development unlock.
  - `.ziptide_dev_open` — one-shot open request, deleted by the running app after consumption.
- `DevMenu` polls only in Editor/Development builds. It is otherwise dormant and compiled out of shipping builds.
- No controller feature usage remains in `DevMenu`.

## Claimed files

New:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevAccessGate.cs` + `.meta`
- `Ziptide/Assets/Ziptide/Tests/EditMode/DevAccessGateTests.cs` + `.meta`
- `tools/dev_menu_access.ps1`
- `docs/DEV_MENU_ACCESS.md`
- this log

Existing:

- `Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs`
- `docs/design/CONTROL_SCHEME.md`
- `docs/SPRINT.md`

## Exclusions

- No scene, prefab, input-action asset, package, project-setting, rig, locomotion, QuickSwap, or player-menu change
- No Android manifest/plugin/activity change
- No secret or password embedded in the APK
- No runtime access in non-development builds
- No controller chord of any kind

## Safety/acceptance

1. `DevMenu.cs` contains no `CommonUsages.secondaryButton` or controller summon path.
2. F2 still toggles in Editor.
3. Access marker is required on Quest development builds.
4. One-shot open marker is deleted only after successful consumption.
5. Missing/inaccessible storage fails closed and never throws.
6. Pure file-system logic is covered by EditMode tests using a temporary directory.
7. PowerShell supports `Open`, `Unlock`, `Lock`, and `Status` without Unity GUI steps.
8. Existing menu UI, paging, warp path, and UI-session rebind remain unchanged.
9. CI must record Unity EditMode green; headset verification remains required for ADB open.

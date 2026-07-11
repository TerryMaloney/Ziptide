# Controller-Free Developer Menu Access

The in-headset world-warp menu is a development tool. It must never consume buttons that belong to gameplay.

## Final access model

- **Unity Editor:** press `F2`.
- **Quest development build:** use `tools/dev_menu_access.ps1` over an authorized ADB connection.
- **Shipping build:** `DevMenu` and `DevAccessGate` are compiled out by `UNITY_EDITOR || DEVELOPMENT_BUILD`.

There is no Quest controller summon chord. Y, B, L3, R3, triggers and grips remain gameplay-owned.

## Open the menu on a connected Quest

From the repository root in PowerShell:

```powershell
.\tools\dev_menu_access.ps1 -Action Open
```

This does three things:

1. verifies that an authorized Quest is connected;
2. writes the persistent `.ziptide_dev_access` marker;
3. writes the one-shot `.ziptide_dev_open` request and brings ZIPTIDE forward if possible.

The running development build polls its own persistent-data directory four times per second. It deletes the open marker before showing the existing menu, so the request cannot replay.

If ZIPTIDE is not installed or cannot be brought forward automatically, launch it normally on the headset. A pending open request will be consumed after startup.

## Other commands

Unlock access without opening:

```powershell
.\tools\dev_menu_access.ps1 -Action Unlock
```

Inspect marker state:

```powershell
.\tools\dev_menu_access.ps1 -Action Status
```

Revoke access and remove pending requests:

```powershell
.\tools\dev_menu_access.ps1 -Action Lock
```

A running menu closes on its next access poll after `Lock`.

## Why ADB is the login boundary

The dev menu is not a player feature and does not need an account/password system embedded in the game. An authorized ADB connection already requires:

- a developer-mode headset;
- physical USB or authorized wireless debugging access;
- acceptance of the debugging key on the headset.

That is a stronger and simpler development boundary than a discoverable in-game PIN. No secret is stored in source or the APK.

## Marker contract

The helper targets the same external app-files directory Unity uses for `Application.persistentDataPath` on Android:

```text
/sdcard/Android/data/com.terrymaloney.ziptide/files/
```

Markers:

- `.ziptide_dev_access` — persistent unlock; survives normal app restarts and `adb install -r` updates.
- `.ziptide_dev_open` — one-shot open request; deleted before the menu is shown.

Uninstalling the app removes its app data. Release builds ignore any leftover marker because the dev code is not compiled.

## Preserved behavior

This change does not alter:

- menu canvas, pagination or close button;
- UI-session rebinding after travel;
- `DevWarp` or `TravelCoordinator` behavior;
- world manifest generation;
- player input actions;
- QuickSwap on B;
- sprint/auto-run on L3;
- crouch/slide on R3.

## Device verification

After a development APK is installed:

1. Press Y and B individually and together during gameplay — the dev menu must not react.
2. Run `-Action Status` — it should initially report locked unless previously unlocked.
3. Run `-Action Open` — the menu should appear without controller input.
4. Warp, close, then run `-Action Open` again — the menu must remain clickable after travel.
5. Run `-Action Lock` — a visible menu should hide and later open requests must require a new unlock.
6. Install a non-development APK — no DevMenu object or marker behavior may exist.

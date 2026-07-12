# HEADSET TEST NIGHT — HANDOFF

**Date:** 2026-07-12  
**Branch:** `terry-local-wip`  
**Owner:** GPT-5.6 integration/headset-prep lane  
**Purpose:** close the Field Camera handoff and remove the known Photon/two-ADB-device setup traps before Terry's home test.

## Did

### Field Camera close-out

- Confirmed the full Field Camera completion envelope is on the branch and Unity EditMode green.
- Confirmed the current implementation is no longer shutter-only:
  - 512×384 PNG capture;
  - live viewfinder capped at 10 Hz;
  - composition scoring;
  - bounded 24-photo profile album and file eviction;
  - one grabbable camera in Quarters;
  - six newest captures on the Quarters wall;
  - owned RenderTexture/Texture2D/material cleanup.
- Replaced the stale deployment page with the actual final camera device checks.

### Photon state audit

Verified from committed project files:

- PUN2 is already imported under `Assets/Photon/**`;
- the Realtime App ID is present;
- `ZIPTIDE_PHOTON` is enabled for Android and Standalone;
- the Photon transport and bootstrap are present;
- A6 v1 head/hand presence is implemented;
- A6.2 fire/hit/score authority remains unbuilt.

The old instructions to create/import/configure Photon were stale and have been replaced. Do not reimport PUN2.

### Previous two-headset failure prevention

Photon's Best Region selection is not deterministic. Rooms are isolated per region, and even two clients on the same network can select different regions. The previous blank region configuration could therefore produce two separate `ZIP-001 (1/2)` rooms.

For development APKs, `PhotonServerSettings.DevRegion` is now pinned to `usw`. `BuildAndroid` produces Development builds, so both Oregon test headsets use the same Photon region without changing release behavior.

### PC/headset automation

Added `tools/two_quest_test.ps1`:

- checks `terry-local-wip`;
- warns on local changes;
- verifies PUN2, App ID, Android define, transport/bootstrap and deterministic region;
- builds once through `dev_build_install.ps1 -BuildOnly`;
- installs the identical APK to every authorized ADB Quest;
- launches each headset;
- waits for Terry's GO ONLINE test;
- captures `Builds/quest_<serial>_photon.log` separately per device;
- checks `NET_STARTER_INSTALLED`, `LOBBY_ONLINE_START`, `NET_ROOM_JOINED`, `NET_PRESENCE`, disconnects and fatal markers;
- supports one-cable sequential installs with `-SkipBuild -InstallOnly`.

Updated:

- `docs/TWO_QUEST_SETUP.md`
- `docs/GET_IT_ON_THE_HEADSETS.md`

## Next — Terry, after Architect's final push

```powershell
cd C:\Ziptide
git checkout terry-local-wip
git pull --rebase origin terry-local-wip
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1
```

If only one USB cable is available:

```powershell
# First Quest: build + install
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -InstallOnly

# Swap cable; second Quest: reuse existing APK
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -SkipBuild -InstallOnly
```

Then perform:

1. boot/core smoke;
2. Field Camera final verification;
3. both headsets into the same arena;
4. GO ONLINE on both;
5. confirm `ZIP-001 (2/2)` and remote helmet/gloves;
6. preserve build and Photon logs.

## Heads-up

- Do not claim complete human-vs-human PvP from A6 v1. It is connection/presence only.
- A6.2 remains the next multiplayer implementation sprint after transport proof.
- Do not reimport Photon, rerun the wizard, change App IDs, or hand-edit scenes in response to a failure.
- A normal branch push skips Android APK generation; Terry's local script performs the actual APK build.
- Device completion remains unclaimed until Terry reports the headset results.

## Commits

- `5cdc712` — deterministic Photon USA-West development region.
- `0bf05c3` — two-Quest build/install/launch/log smoke script.
- `8a68bb4` — current Photon/two-Quest setup guide.
- `443b128` — current headset deployment and Field Camera test page.

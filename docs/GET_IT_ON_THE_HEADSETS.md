# 📦 GET ZIPTIDE ON THE HEADSETS — current home-test path

**Status date:** 2026-07-12  
**Branch:** `terry-local-wip`  
**Unity:** `2022.3.62f3`

This is the short operational page. The detailed one-system checks remain in `TERRY_RUNBOOK.md`; Photon detail is in `TWO_QUEST_SETUP.md`.

## 1. Wait for the final branch push, then pull exactly once

After Architect says its current work is pushed:

```powershell
cd C:\Ziptide
git checkout terry-local-wip
git pull --rebase origin terry-local-wip
git status --short
git rev-parse --short HEAD
```

Do not build from `main`. If `git status --short` prints local changes you do not recognize, stop before pulling or building and preserve them.

## 2. Connect and authorize the Quest hardware

For each headset:

1. Developer Mode enabled.
2. Connect USB.
3. Put on the headset.
4. Approve **Allow USB debugging** and select **Always allow from this computer**.
5. Verify from PowerShell:

```powershell
adb devices
```

Every connected headset must show `device`, not `unauthorized` or `offline`.

## 3. Preferred path for tonight — one build, every connected Quest

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1
```

This performs the full canonical path:

- validates branch and Photon configuration;
- runs the scene patchers/authors;
- executes the world audit;
- builds one development APK;
- installs the identical APK to all authorized Quests;
- launches both;
- captures separate Photon logs after the in-headset test.

APK output:

```text
C:\Ziptide\Ziptide\Builds\Android\Ziptide.apk
```

Build log:

```text
C:\Ziptide\Ziptide\Builds\android_build.log
```

## 4. One cable instead of two

First headset, including the build:

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -InstallOnly
```

Swap cable to headset two, then install the same APK without rebuilding:

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -SkipBuild -InstallOnly
```

After both are installed, launch from **Library → Unknown Sources → ZIPTIDE**.

## 5. Single-headset fallback

For a normal one-device build/install/log smoke:

```powershell
powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\quest_smoke.ps1
```

The older `dev_build_install.ps1` remains appropriate when exactly one Quest is connected. Use `two_quest_test.ps1` when multiple ADB devices are present.

## 6. Tonight's priority checks

### A. Boot and core stability

- clean boot without black screen or crash;
- XR hands/controllers active;
- locomotion and rays work;
- headset-native developer menu opens with the two-controller forehead gesture;
- travel completes without `TRAVEL_FAIL`, `TRAVEL_TIMEOUT`, duplicate rig or inventory restore errors.

### B. Field Camera — final feature verification

The old “shutter-only” state is obsolete. The current green build includes real capture and the Quarters photo wall.

1. Take the Field Camera from the Quarters or Sandbox.
2. Confirm the back screen shows a live view and does not recursively film itself.
3. Capture a sky, landmark and creature.
4. Confirm haptic shutter plus `PHOTO_CAPTURED` logs.
5. Return to Quarters and confirm the newest six photos appear, newest first.
6. Verify frame colors change with rating.
7. Holster the camera, travel, and retrieve it.
8. Watch performance while the viewfinder is active.
9. Long check later: exceed 24 captures and confirm old files are evicted.

### C. Two-Quest Photon presence

1. Both players enter the same arena.
2. Both press **GO ONLINE**.
3. Both boards reach `NET: in ZIP-001 (2/2)`.
4. Each player sees the other's helmet and amber gloves tracking head/hands.
5. Record avatar scale, latency and any disconnect.

The current A6 v1 build proves connection and presence. Human-vs-human shooting/hit authority remains A6.2 and should not be mistaken for a setup failure.

## 7. What to send back after the run

Paste or attach:

- the final pulled commit from `git rev-parse --short HEAD`;
- whether the build completed and APK installed on one or both Quests;
- `Builds\android_build.log` if the build fails;
- `Builds\quest_<serial>_photon.log` if Photon fails;
- exact visible behavior and the last relevant `ZIPTIDE:` lines;
- camera, travel, controls, frame-rate and two-player feel notes.

Do not reimport Photon, hand-edit scenes, or make speculative Unity changes after a failure. The logs identify the owner and preserve a clean recovery path.

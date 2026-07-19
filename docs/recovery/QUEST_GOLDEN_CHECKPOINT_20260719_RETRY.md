# QUEST GOLDEN CHECKPOINT — 2026-07-19 CORRECTION RETRY

**Authorization status:** **AUTHORIZED** for the exact artifact named below.  
**Purpose:** retry the same bounded recovery route after correcting the weapon pose/scale and gate-coupler interaction defects Terry demonstrated on Quest on 2026-07-19.  
**Do not substitute:** a local rebuild, a different SHA, a renamed older APK, or an artifact with a different hash voids this authorization.

## 1. Exact candidate and proof record

| Field | Required value |
|---|---|
| Runtime source SHA | `c3f9a4d21e6ea6d68c73755b63be102b6148d436` |
| Static recovery status | all recovery tool tests and report generators PASS on the exact SHA |
| Ordinary CI | run `29702879777`: EditMode success + patch-scenes/world-audit success |
| EditMode count | **1059/1059 passed** |
| Patch/world audit | success; **0 blockers** |
| Recovery PlayMode | run `29702879784`, attempt 1: **43/43 passed**, 0 failed/skipped/inconclusive |
| Golden Android | run `29702879798`, attempt 1: build and independent verification success |
| Unity | `2022.3.62f3` |
| Build method | `Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK` |
| Build profile | `GoldenSlice` |
| Compile define | `ZIPTIDE_RECOVERY_GOLDEN` |
| Locked scenes | `_Boot`, `W000_DriftIn`, `ToxicCity` |
| Artifact | `recovery-golden-apk-c3f9a4d21e6ea6d68c73755b63be102b6148d436` |
| APK filename | `Ziptide.apk` |
| APK bytes | `108770268` |
| APK SHA-256 | `1e016f16468ce6cca37aed127fae0343b5f298bf74b0da11e62a3a8b91003806` |
| Build-profile SHA-256 | `0f545ce4194d34bb64a7d1841299c58073ba27ff21b9002213f5a026a38ea3a7` |
| Golden audit artifact | `recovery-golden-audit-c3f9a4d21e6ea6d68c73755b63be102b6148d436` |
| Independent artifact review | APK and profile extracted independently; profile result/define/scenes verified; hashes recomputed; Golden audit `totalBlockers=0` |

## 2. Corrections under test

1. Forge-authored weapons no longer inherit the tiny primitive root scale a second time. The primitive dimensions are preserved in the physical collider and the metre-authored Forge visual uses a unit-scale root.
2. Item local `+Z` is the canonical held-forward axis. Gun and Forge Grip sockets no longer apply the rejected upward `+45°` pitch.
3. Gravity gun, Breaker Blade, and Tide Pike use corrected human-scale bounds and forward-aligned Grip data.
4. The coupler's old round red button-looking lamp is gone. Status is now a flat, non-interactable indicator.
5. The real final control is a large, centred, labelled `PRESS POWER` target. It appears only after the replacement part seats and remains entirely within the child-reach envelope.
6. Coupler panel and replacement-part grabs enter XRI as constrained dynamic rigidbodies rather than kinematic throw bodies.

## 3. PC/headset preflight

- Quest charged above 40%, Developer Mode enabled, direct USB connection.
- Accept USB debugging in-headset and choose **Always allow from this computer** when offered.
- Unity Editor closed.
- No other Ziptide build/session running.
- Clear guardian/play space.

From PowerShell:

```powershell
cd C:\Ziptide
adb start-server
adb devices
```

Exactly the intended Quest must appear with status `device`.

## 4. Verify and install the exact APK

Assuming the downloaded file is in Downloads:

```powershell
$apk = "$env:USERPROFILE\Downloads\Ziptide_Golden_c3f9a4d.apk"
Get-FileHash $apk -Algorithm SHA256
```

The hash must exactly equal:

```text
1e016f16468ce6cca37aed127fae0343b5f298bf74b0da11e62a3a8b91003806
```

Then install and clear the old log buffer:

```powershell
adb logcat -c
adb install -r $apk
```

Do not run `dev_build_install.ps1` or `quest_smoke.ps1`; either would replace the certified install with a different build.

## 5. Start evidence capture before launch

In a second PowerShell window:

```powershell
cd C:\Ziptide
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$log = ".\Ziptide\Builds\quest_checkpoint_retry_${stamp}.log"
adb logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath $log
```

Launch Ziptide manually from the Quest library.

## 6. Exact bounded retry route

Run only:

`cold boot -> NEW GAME -> W000 -> repair gate coupler -> PUNCH IT -> ToxicCity -> W000 -> save/relaunch/CONTINUE`

### A. Boot and W000 foundation

- Home Hub survives 60 seconds; boot hold blocks movement/turning.
- One rig, correct controller tracking, readable front-facing Home surfaces.
- NEW GAME travels once and the travel shell clears.
- W000 has valid standing height, immediate movement, snap turn, and smooth turn.

### B. Corrected weapon checks

- Grab the pistol and taser exposed by the Golden route.
- Both must be clearly hand-sized rather than centimetre-sized.
- Their muzzle/laser direction must follow the controller/index-finger forward direction rather than pointing upward.
- Release and recover/holster them; they must not disappear or remain floating.
- Record unavailable melee families as **NOT OBSERVED**; do not broaden the Golden route or use a debug warp merely to find them.

### C. Corrected gate-coupler checks

- Pull the access panel.
- Seat the replacement part.
- Confirm the old round red button-looking object is absent.
- After seating, the only newly presented final control must be the large front-centred `PRESS POWER` target.
- Confirm a child-height reach can select the full target without reaching above shoulder/head height.
- Press it once.
- The status indicator must turn green, the machine must reach RUNNING, and PUNCH IT must arm.
- PUNCH IT must launch exactly one canonical departure.

Expected log evidence includes:

```text
ZIPTIDE: MACHINE_STAGE id=gate_coupler stage=panel_off
ZIPTIDE: MACHINE_STAGE id=gate_coupler stage=part_seated
ZIPTIDE: MACHINE_REPAIRED id=gate_coupler
ZIPTIDE: REPAIR_TRACE ... armed=True ... repaired=True
ZIPTIDE: FLIGHT_LAUNCH
```

There must be no XRI warning about throwing the coupler panel or replacement part while kinematic.

### D. Complete the unchanged recovery route

- Arrive in ToxicCity once; transition fully clears.
- Immediately verify movement, snap turn, and smooth turn.
- Return canonically to W000 once.
- Verify rig/input/item state remains valid after the round trip.
- Exit normally after autosave, relaunch, select CONTINUE once, and verify valid state and immediate controls.

## 7. Stop and preserve evidence

Any crash, load loop, dead controls after travel, duplicate rig, uncleared travel shell, invalid spawn, failed coupler completion, blocked PUNCH IT after repair, or severe sustained judder is a retry failure.

After the route, stop the live log with `Ctrl+C`, then capture the bounded buffer:

```powershell
cd C:\Ziptide
adb logcat -d -v threadtime -s Unity Ziptide > .\Ziptide\Builds\quest_checkpoint_retry_extra_dump.log
```

Search both files:

```powershell
Select-String -Path .\Ziptide\Builds\quest_checkpoint_retry*.log -Pattern `
  "Exception|NullReference|TRAVEL_FAIL|XRI_NOT_READY|INPUT_MUTATION_SETTLE_FAIL|INPUT_MUTATION_SETTLE_TIMEOUT|MACHINE_STAGE|MACHINE_REPAIRED|REPAIR_TRACE|FLIGHT_BLOCKED|FLIGHT_LAUNCH|SPAWN_AT|TRAVEL_OK|HEALTH_SWEEP|kinematic Rigidbody"
```

Preserve the APK, both SHA-256 values, Quest model/OS/serial, start/end time, full logs, and PASS/FAIL/NOT OBSERVED notes. Recovery exits only after Terry accepts this exact Quest route.

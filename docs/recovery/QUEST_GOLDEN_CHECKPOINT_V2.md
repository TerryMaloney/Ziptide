# ZIPTIDE QUEST GOLDEN CHECKPOINT V2

**Status:** HOLD until the authorization record names one exact fully green source SHA and artifact.  
**Expected headset time:** 12–20 minutes.  
**Locked route:** Home Hub → W000 → ToxicCity → W000 → relaunch/CONTINUE.  
**Purpose:** prove only physical Quest behavior that desktop automation cannot prove.

Do not broaden this pass into flight, space combat, gardens, factories, vehicles, arenas, multiplayer, mass-world touring, or visual redesign.

---

## 1. Authorization record

Every field must be completed before installation.

| Field | Required value |
|---|---|
| Source SHA | `PENDING` |
| Normal PlayMode | `PENDING — exact required total, zero fail/skip/inconclusive` |
| Clean package proof | `PENDING when packages or foundational input/travel changed` |
| Ordinary CI | `PENDING — EditMode + patch/world audit green` |
| Contract/static scan | `PENDING — current for scanned inputs` |
| Golden Android | `PENDING — success on same source SHA` |
| Build profile | `GoldenSlice` |
| Compile define | `ZIPTIDE_RECOVERY_GOLDEN` |
| Locked scenes | `_Boot`, `W000_DriftIn`, `ToxicCity` only |
| APK artifact | `PENDING` |
| APK SHA-256 | `PENDING` |
| Build-profile report SHA-256 | `PENDING` |
| Artifact review | `PENDING` |
| Authorized by | `PENDING` |

**Stop:** any pending, stale, mismatched, or contradicted row means the checkpoint is not authorized.

---

## 2. Immutable-artifact rule

Use the downloaded authorized APK named above. Do not rebuild locally and call it the same candidate.

During this checkpoint, **do not run `tools/quest_smoke.ps1`**. That script invokes `dev_build_install.ps1`, performs a new Unity build, and installs the locally rebuilt APK before scanning. It is useful for later iterative development, not for an immutable exact-artifact checkpoint.

---

## 3. PC and Quest preflight

- [ ] Quest charge above 40%.
- [ ] Developer Mode enabled.
- [ ] Safe guardian/play area.
- [ ] USB connected directly to the PC.
- [ ] Unity Editor closed.
- [ ] No other Ziptide session running.
- [ ] Accept the Quest USB-debugging prompt and choose **Always allow from this computer** when offered.

From PowerShell:

```powershell
cd C:\Ziptide
adb start-server
adb devices
```

- [ ] Exactly the intended Quest appears as `device`, not `unauthorized` or `offline`.
- [ ] Record the device serial, model, and Quest OS version.

---

## 4. Download, verify, and install the exact APK

1. Open the exact authorized Golden workflow run.
2. Download the named APK artifact.
3. Extract it to a new folder named for the source SHA.
4. Verify the inner APK—not the artifact ZIP:

```powershell
Get-FileHash .\Ziptide.apk -Algorithm SHA256
```

- [ ] Hash exactly matches the authorization record.
- [ ] Folder and APK are labeled with the source SHA.

Install without rebuilding:

```powershell
adb logcat -c
adb install -r .\Ziptide.apk
```

- [ ] Installation reports success.

---

## 5. Start continuous evidence capture

In a second PowerShell window:

```powershell
cd C:\Ziptide
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$log = ".\Ziptide\Builds\quest_checkpoint_${stamp}.log"
adb logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath $log
```

- [ ] Logging begins before app launch.
- [ ] Record local start time and log filename.
- [ ] Launch Ziptide from the Quest library.

On a blocker, stop exploration, note the exact time and nearest action, preserve the log, and exit.

---

# 6. Headset route

## A. Cold boot and Home Hub — approximately 2 minutes

- [ ] Remain at Home Hub for 60 seconds: no crash, fall, sink, respawn flash, black loop, repeated load, or unexpected travel.
- [ ] Push both sticks hard in every direction for five seconds while Home Hub owns the rig: no translation or turning.
- [ ] Head and both controllers track correctly.
- [ ] No doubled hands, cameras, rays, cursors, or competing interaction surfaces.
- [ ] Title, NEW GAME, CONTINUE state, and SETTINGS are front-facing and readable.
- [ ] Open SETTINGS once, close it once, and select NEW GAME once: no dead first click or double activation.

## B. First W000 arrival — approximately 3 minutes

- [ ] One transition produces one W000 arrival.
- [ ] The pale travel shell fully clears.
- [ ] Standing height and floor relationship feel valid; no wall/floor/ship intersection or immediate respawn.
- [ ] Immediately—not after waiting—test for 15 seconds:
  - [ ] move forward/back/left/right;
  - [ ] snap turn both directions;
  - [ ] smooth/continuous turn when enabled;
  - [ ] stop and restart movement.
- [ ] No dead stick, delayed wake, violent turn, exception symptom, or menu workaround.
- [ ] W000 renders real geometry rather than black, blank, uniform, pink, or missing surfaces.
- [ ] Ship/helm/quarters/disembark panels exist only where expected and do not follow the head.
- [ ] Grab, release, and recover/holster one intended starter item or weapon.
- [ ] Inspect `ObjectiveBoard/ObjectiveCanvas/ObjectiveText` from the intended standing position. It must be readable, front-facing, not clipped, and not severely overlapped. This is a required manual check because the current automated UI audit cannot fully measure world-space `TextMeshProUGUI` CanvasRenderer geometry.

## C. W000 to ToxicCity — approximately 3 minutes

Use the intended Golden travel surface/helm, not ADB or an unrelated debug warp.

- [ ] ToxicCity is identified clearly and accepts one confirmation.
- [ ] Only one travel begins.
- [ ] Transition is comfortable and fully clears.
- [ ] ToxicCity appears as actual city/world geometry with sky and lighting—not the inside of the travel flash.
- [ ] Spawn height and clearance feel valid.
- [ ] Immediately repeat move, snap turn, smooth turn, stop, and restart for 15 seconds.
- [ ] No control loss, violent one-frame turn, duplicate rig, or input-manager symptom.
- [ ] RILL/dialogue text is readable from the normal viewpoint; no severe clipping or overlap.
- [ ] No leaked Home Hub or W000 surfaces.
- [ ] Slowly rotate 360 degrees and observe frame pacing and comfort.

## D. ToxicCity to returned W000 — approximately 3 minutes

- [ ] Return through the canonical travel path.
- [ ] One transition returns to W000 and fully clears.
- [ ] Immediately repeat all movement and turning checks for 15 seconds.
- [ ] Same rig, hands, rays, and input behavior remain present.
- [ ] W000 composition and panels remain correct.
- [ ] Earlier interaction/persistence probe remains correct according to intended save behavior.
- [ ] No cumulative hitch, doubled audio, material corruption, or obvious resource-related presentation change.

## E. Save, relaunch, and CONTINUE — approximately 2–4 minutes

- [ ] Exit normally after the return autosave.
- [ ] Relaunch Ziptide.
- [ ] CONTINUE is available and activates once.
- [ ] Expected location/state loads without duplicate owners, broken rig, or invalid spawn.
- [ ] Movement, snap turn, and smooth turn work immediately.
- [ ] Chosen persistence probe remains correct.

---

## 7. Immediate blockers

Any one of these fails the checkpoint:

- crash, freeze, ANR, or repeated boot/load loop;
- fall, sink, or respawn loop;
- locomotion active while Home Hub boot hold owns the rig;
- movement or turning dead/glitchy immediately after travel or CONTINUE;
- Input System/XRI exception symptom;
- duplicate rig, camera, hands, rays, or input manager;
- travel shell does not clear;
- wrong scene, double travel, missing arrival, or invalid standing spawn;
- blank/uniform view, pink/error material, or missing active geometry;
- critical Objective Board or RILL instruction text unreadable from its intended position;
- save/CONTINUE loss or duplication;
- sustained judder or comfort failure preventing completion.

Non-blocking art-quality notes are still recorded, but do not replace the pass/fail checks.

---

## 8. Finish and scan the captured log

1. Remove the headset.
2. Press `Ctrl+C` in the live-log window.
3. Search the exact captured file without rebuilding:

```powershell
Select-String -Path .\Ziptide\Builds\quest_checkpoint_*.log -Pattern `
  "Exception|NullReference|TRAVEL_FAIL|XRI_NOT_READY|INPUT_MUTATION_SETTLE_FAIL|INPUT_MUTATION_SETTLE_TIMEOUT|BOOT_HOLD|SPAWN_AT|TRAVEL_OK|HEALTH_SWEEP"
```

- [ ] No unhandled exception or `NullReferenceException`.
- [ ] No `TRAVEL_FAIL`.
- [ ] No `XRI_NOT_READY` at a successful arrival.
- [ ] No input-settlement failure or timeout.
- [ ] Expected travel sequence exists once per hop.
- [ ] Boot hold is active at Home Hub and releases only after safe content spawn.
- [ ] Expected scene health sweeps contain no blocker.

Preserve:

- source SHA;
- exact APK and SHA-256;
- Golden and clean-proof run IDs;
- Quest serial/model/OS;
- start/end times;
- full log;
- PASS/FAIL/NOT OBSERVED for every row;
- photos or video only where they clarify a visible problem.

---

## 9. Result template

```text
ZIPTIDE GOLDEN QUEST CHECKPOINT
Source SHA:
APK artifact:
APK SHA-256:
Normal PlayMode run:
Clean package run:
Ordinary CI run:
Golden run:
Quest model / OS:
Device serial:
Start/end time:

A Cold boot/Home Hub: PASS / FAIL
B First W000 arrival: PASS / FAIL
C W000 -> ToxicCity: PASS / FAIL
D ToxicCity -> W000: PASS / FAIL
E Save/relaunch/CONTINUE: PASS / FAIL
Log scan: PASS / FAIL

First failing step and exact time:
What was visible/felt:
Nearest ZIPTIDE log lines:
Log filename:
Additional non-blocking quality notes:
```

---

## 10. After the checkpoint

If all blocker rows pass, record the recovery exit and resume feature development from `docs/PROJECT_COMPLETION_ROADMAP.md`.

If anything fails, the exact artifact, log, time, and failing action become the next source of truth. Do not install another build or tour unrelated systems during the same session.

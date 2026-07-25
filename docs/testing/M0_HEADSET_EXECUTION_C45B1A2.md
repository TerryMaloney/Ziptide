# M0 HEADSET EXECUTION — EXACT GOLDEN `c45b1a2`

**Status:** current single-session authority for Terry's next Quest test. This card supersedes the artifact/install instructions in `docs/TERRY_RUNBOOK.md` §0 and `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md`, which still identify the older `2b158b4` candidate. The detailed gameplay verdict remains governed by `docs/testing/HEADSET_RETRY_C45B1A2.md`.

## 1. Exact authorized artifact

| Field | Required value |
|---|---|
| Source SHA | `c45b1a295e50637d81aab14c09e36eb08bf6ed58` |
| Golden run | `29786604008` |
| APK artifact | `recovery-golden-apk-c45b1a295e50637d81aab14c09e36eb08bf6ed58` |
| Artifact ID | `8478885142` |
| Artifact expiry | `2026-10-18` |
| Artifact ZIP SHA-256 | `77da9927f307f636f27f228e8ea1c30028e8d1810cb281551458dfda52cbdd75` |
| APK path inside ZIP | `Android\Ziptide.apk` |
| APK size | `102482894` bytes |
| APK SHA-256 | `296dd92e6488e93019014ce7624c1092989180513a02640efa53cf2c5d92f8e1` |
| Build profile | `GoldenSlice` |
| Compile define | `ZIPTIDE_RECOVERY_GOLDEN` |
| Locked scenes | `_Boot`, `W000_DriftIn`, `ToxicCity` |

Artifact page:

`https://github.com/TerryMaloney/Ziptide/actions/runs/29786604008`

### Hard stop

Do not test if:

- the artifact name differs;
- the APK hash differs;
- the run or source SHA differs;
- a locally rebuilt APK replaced it;
- `dev_build_install.ps1` or `quest_smoke.ps1` was run after installing the exact candidate;
- another operator changes proof-relevant runtime and attempts to call that new build the same checkpoint.

The audit artifact for the same run reported zero audit blockers and a successful GoldenSlice build. Audit warnings remain content/presentation debt and do not replace the bounded device verdict.

## 2. PC and Quest preflight

- Quest above 40% charge.
- Developer Mode enabled.
- Direct USB connection to PC.
- Accept USB debugging and choose **Always allow from this computer** if offered.
- Unity Editor closed.
- No Ziptide instance running.
- Clear safe play space.

PowerShell:

```powershell
cd C:\Ziptide
adb start-server
adb devices
```

Pass only when exactly the intended Quest appears as `device`, not `unauthorized` or `offline`.

Record:

```text
Quest model:
Quest OS:
ADB serial:
Test start time:
```

## 3. Download and verify

1. Open run `29786604008`.
2. Download artifact `recovery-golden-apk-c45b1a295e50637d81aab14c09e36eb08bf6ed58`.
3. Extract to a unique folder such as:

```text
C:\Ziptide\QuestBuilds\c45b1a2\
```

4. The APK should be at:

```text
C:\Ziptide\QuestBuilds\c45b1a2\Android\Ziptide.apk
```

5. Verify:

```powershell
cd C:\Ziptide\QuestBuilds\c45b1a2
Get-FileHash .\Android\Ziptide.apk -Algorithm SHA256
```

Required hash:

```text
296DD92E6488E93019014CE7624C1092989180513A02640EFA53CF2C5D92F8E1
```

Do not continue on any mismatch.

## 4. Start clean evidence capture

First PowerShell window:

```powershell
adb logcat -c
adb install -r .\Android\Ziptide.apk
```

Second PowerShell window, before launching the app:

```powershell
cd C:\Ziptide
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
adb logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath ".\Ziptide\Builds\quest_checkpoint_${stamp}.log"
```

Keep the second window running through both route passes.

Launch Ziptide from the Quest library.

## 5. Bounded route — pass one

Do not broaden this into gardens, arenas, multiplayer, flight, world touring, factories, or art review.

### A. Initial stability and tracking

- Head and both controllers track normally.
- No doubled hands, camera, rays, or interaction cursors.
- No sinking, fall/respawn loop, repeated load, black loop, or frozen frame.
- Basic movement/turning is normal where gameplay permits it.

Record any blocker immediately and stop broad exploration.

### B. Gun scale and pose

Pick up one gun.

Pass when:

- it does not look miniature;
- grip fills the hand naturally;
- visible body and collider agree;
- relaxed wrist points firing direction naturally forward;
- movement/turning does not rotate the item independently.

Record:

```text
Gun family:
Gun scale: PASS / FAIL — approximate percentage correction
Gun angle: PASS / FAIL — direction and approximate degrees
Gun collider/body agreement: PASS / FAIL
```

### C. Melee scale and pose

Pick up one melee item, preferably the Breaker Blade present in the candidate route.

Pass when:

- it does not look miniature;
- grip fills the hand naturally;
- visible body and collider agree;
- intended reversed melee pose is used rather than gun pose;
- no gun laser appears at the tip;
- movement/turning does not rotate it independently.

Record:

```text
Melee family:
Melee scale: PASS / FAIL — approximate percentage correction
Melee angle: PASS / FAIL — direction and approximate degrees
Melee laser absent: PASS / FAIL
```

### D. Coupler seating

Complete the coupler sequence.

Pass when:

- red indicator reads as status rather than a button;
- final control appears distinctly as `PRESS POWER` only after seating;
- control is reachable without overhead reach or tiptoe;
- control is comfortable for a child-height player;
- selection target is broad enough without precision aiming;
- releasing the seated part/panel does not throw or drift it away.

Record separately:

```text
Coupler part seats: PASS / FAIL
Red status clarity: PASS / FAIL
PRESS POWER appears at correct stage: PASS / FAIL
PRESS POWER child reach: PASS / FAIL
PRESS POWER selectable: PASS / FAIL
Released panel/part remains stable: PASS / FAIL
```

### E. Power and ToxicCity travel

Activate `PRESS POWER`, continue through `PUNCH IT`, and complete travel.

Pass when:

- prompt advances;
- travel completes exactly once;
- transition clears;
- ToxicCity renders rather than a blank/travel shell;
- movement works immediately after arrival;
- snap/continuous turning works immediately where enabled;
- no controller action is lost;
- no unintended developer menu opens;
- Y/B gameplay inputs remain usable.

Immediately test movement and turning for at least 15 seconds after arrival.

Record:

```text
PRESS POWER progression: PASS / FAIL
PUNCH IT progression: PASS / FAIL
ToxicCity arrival: PASS / FAIL
Travel shell clears: PASS / FAIL
Movement after travel: PASS / FAIL
Turning after travel: PASS / FAIL
Y/B gameplay available: PASS / FAIL
Unintended dev menu absent: PASS / FAIL
```

## 6. Stability repeat — pass two

Repeat the complete coupler → power → PUNCH IT → ToxicCity route in the same installed build.

This is mandatory. The failure class involved state restoration across lifecycle/travel transitions.

Record:

```text
Second coupler seating: PASS / FAIL
Second PRESS POWER: PASS / FAIL
Second travel: PASS / FAIL
Second movement/turning recovery: PASS / FAIL
New cumulative problem: NONE / note
```

A first-pass success with a second-pass failure is an M0 failure.

## 7. End evidence capture

After the second route:

1. Remove headset.
2. Press `Ctrl+C` in the live-log window.
3. Pull one additional dump:

```powershell
cd C:\Ziptide
adb logcat -d -v threadtime -s Unity Ziptide > .\Ziptide\Builds\quest_checkpoint_extra_dump.log
```

4. Search:

```powershell
Select-String -Path .\Ziptide\Builds\quest_checkpoint_*.log -Pattern `
  "Exception|NullReference|TRAVEL_FAIL|XRI_NOT_READY|INPUT_MUTATION_SETTLE_FAIL|INPUT_MUTATION_SETTLE_TIMEOUT|BOOT_HOLD|SPAWN_AT|TRAVEL_OK|REPAIR_TRACE|HEALTH_SWEEP"
```

Preserve:

- exact APK ZIP and APK;
- APK SHA-256;
- source SHA and run number;
- Quest model/OS/serial;
- start/end times;
- full log and extra dump;
- verdict block below;
- short video/photo only for a visual or interaction failure needing clarification.

## 8. Final verdict block

```text
ZIPTIDE M0 EXACT GOLDEN VERDICT
Source SHA: c45b1a295e50637d81aab14c09e36eb08bf6ed58
Golden run: 29786604008
Artifact: recovery-golden-apk-c45b1a295e50637d81aab14c09e36eb08bf6ed58
APK SHA-256: 296dd92e6488e93019014ce7624c1092989180513a02640efa53cf2c5d92f8e1
Quest model / OS:
Device serial:
Start/end time:

Initial stability/tracking: PASS / FAIL — note
Gun scale: PASS / FAIL — note
Gun angle: PASS / FAIL — note
Gun collider/body agreement: PASS / FAIL — note
Melee scale: PASS / FAIL — note
Melee angle: PASS / FAIL — note
Melee laser absent: PASS / FAIL — note
Coupler part seats: PASS / FAIL — note
Coupler status clarity: PASS / FAIL — note
PRESS POWER stage clarity: PASS / FAIL — note
PRESS POWER child reach: PASS / FAIL — note
PRESS POWER selectability: PASS / FAIL — note
Released part remains stable: PASS / FAIL — note
PUNCH IT / ToxicCity travel: PASS / FAIL — note
Travel shell clears: PASS / FAIL — note
Movement after travel: PASS / FAIL — note
Turning after travel: PASS / FAIL — note
Y/B gameplay available: PASS / FAIL — note
Unintended dev menu absent: PASS / FAIL — note
Second complete route: PASS / FAIL — note
Checkpoint log attached: YES / NO
Extra dump attached: YES / NO
Video/photo attached: YES / NO / not needed
Overall M0 verdict: PASS / FAIL
```

## 9. Immediate branch after Terry's verdict

### If any blocker fails

- preserve exact evidence;
- do not rebuild immediately;
- classify the first failed surface and likely owner;
- route through `docs/recovery/RECOVERY_DEBUG_FAST_PATH.md`;
- fix only the observed failure class;
- add or strengthen the appropriate automated proof without manufacturing correlated repeat evidence;
- produce a newly named candidate and repeat the bounded route;
- do not activate BioRefiner, growing runtime, Forge expansion, worlds, conveyors, multiplayer, or broad content work.

### If all required checks pass twice

- record `QUEST` proof for the named M0 surfaces only;
- explicitly list and lift only the freezes justified by the verdict;
- preserve the exact known-good APK, hash, source SHA, and evidence package;
- reconcile lifecycle S1 evidence and schedule later S2–S5 device checks separately;
- begin the W000→W001 first-hour model-band packet;
- activate **BioRefiner Mk I** as the single growing/invention back-half pilot:
  1. archive keeper binaries and prompts;
  2. validate keeper sidecar;
  3. create proxy dimensions and standing/seated/child reach plate;
  4. choose one representative raw/refined material pair;
  5. write a narrow Forge intake/recipe;
  6. build and compare against the keeper;
  7. close only after headset verdict.

M0 passing does not automatically authorize every paused project lane.

# HEADSET RETRY CARD — authorized Golden `c45b1a2`

> **START HERE FOR THE COMPUTER/INSTALL SESSION:** `docs/testing/M0_HEADSET_EXECUTION_C45B1A2.md` contains the verified current artifact name, run, APK path, APK SHA-256, install/log commands, bounded two-pass route, and post-verdict branch. It supersedes the old `2b158b4` artifact/install instructions still present in `docs/TERRY_RUNBOOK.md` and `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md`.

**Artifact:** `recovery-golden-apk-c45b1a2…`  
**Golden Android run:** `29786604008`  
**Exact gameplay source:** `c45b1a295e50637d81aab14c09e36eb08bf6ed58`  
**Status:** authorized after ordinary CI green + Recovery PlayMode 43/43 twice on the same SHA.

## Freeze rule

Install and test the exact authorized Golden artifact above. Do not substitute a newly generated APK,
and do not rebuild the gameplay source before this verdict. Planning, documentation, concepts, and
fast-preflight tooling added after `c45b1a2` do not replace the exact-SHA device proof.

## Test route

### 1. Weapon human scale

Pick up at least one gun and one melee weapon.

Pass when:

- neither weapon looks miniature;
- the grip fills the hand naturally;
- the visible weapon body and collider agree;
- the melee weapon has no gun laser at its tip.

Record separately:

- gun scale: PASS / FAIL + approximate correction needed;
- melee scale: PASS / FAIL + approximate correction needed.

### 2. Held angle and hand pose

With the wrist relaxed and pointed naturally forward:

- the gun's firing direction should follow the hand instead of pointing sharply up/down/sideways;
- the melee weapon should use its intended reversed Breaker Blade pose rather than the gun pose;
- rotate and move the thumbsticks while holding each item; the item must not rotate independently.

Record gun and melee angle separately. A useful failure note is a direction plus rough degrees, for
example: “gun nose about 25° too high” or “blade rotated about 45° inward.”

### 3. Coupler clarity and reach

Complete the coupler seating sequence.

Pass when:

- the red status indicator reads as status, not a clickable button;
- the actual final control appears distinctly as `PRESS POWER` only after the part is seated;
- the switch is selectable without reaching overhead or standing on tiptoe;
- a child-height player can target it comfortably;
- its target is broad enough to select without precision aiming;
- releasing the seated part/panel does not throw or drift it away.

### 4. `PUNCH IT` / ToxicCity travel

Activate the final power control and continue through the travel route.

Pass when:

- the prompt advances;
- travel completes instead of hanging or returning to the prior stage;
- movement and turning work after arrival;
- no controller action is lost after the transition;
- Y/B gameplay inputs remain available and no unintended developer menu opens.

### 5. Stability repeat

Repeat the coupler → power → travel route once more in the same installed build. The second pass is
important because the repaired failure involved state restoration after lifecycle transitions.

## Evidence to save on any failure

Do not diagnose from memory. Save:

1. a short video or exact description of what the hands saw;
2. which step failed and whether it failed on first or second pass;
3. the headset checkpoint/logcat file from that run;
4. for angle/scale issues, the weapon family and rough direction/magnitude of the error;
5. for the coupler, whether the control was invisible, unreachable, or visible-but-unselectable.

Any failure re-enters `docs/recovery/RECOVERY_DEBUG_FAST_PATH.md` with the exact artifact and observed
owner/surface. Do not reopen the resolved XRI diagnosis without new input exceptions.

## Verdict format

```text
Artifact: recovery-golden-apk-c45b1a2…
Gun scale: PASS / FAIL — note
Gun angle: PASS / FAIL — note
Melee scale: PASS / FAIL — note
Melee angle: PASS / FAIL — note
Coupler status clarity: PASS / FAIL — note
PRESS POWER child reach: PASS / FAIL — note
PRESS POWER selectability: PASS / FAIL — note
PUNCH IT / ToxicCity travel: PASS / FAIL — note
Movement + turning after travel: PASS / FAIL — note
Second route repeat: PASS / FAIL — note
Checkpoint/log attached: YES / NO
```

# QUEST GOLDEN CHECKPOINT — BOUNDED RECOVERY PASS

**Authorization status:** **AUTHORIZED — 2026-07-17.** The table below is complete from one exact
green source SHA; the certified artifact is the one named in §1 and nothing else. A local rebuild,
a different SHA, or a hash mismatch voids this authorization on the spot.  
**Purpose:** prove only the device-dependent claims that desktop recovery cannot establish.  
**Expected duration in headset:** 12–20 minutes.  
**Locked content:** `_Boot` → `W000_DriftIn` → `ToxicCity` → `W000_DriftIn`.  
**Do not broaden this session into flight, gardens, factories, arenas, multiplayer, all-world touring, or visual redesign.** Those receive separate device campaigns after recovery exits.

---

## 1. Authorization record — operator fills before Terry installs anything

| Field | Required value |
|---|---|
| Source SHA | `2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295` |
| PlayMode observation | **43/43, 0 failed/skipped/inconclusive** — CLEAN lane run `29540554179` (Library deleted before every job; the strongest form of the proof). Prior normal-lane 43/43 on runtime-identical parent `19b86b6` (run `29532312905`). |
| Ordinary CI run | Green on `2b158b4` — EditMode + patch/world audit (same push as the clean proof). |
| Contract scan | Green — fresh dispatch run `29545553407` on `c3ff88a` (post-candidate head; diff to the source SHA is docs/workflow/lock-pin only, see §1a). |
| Golden Android run | **Success on the exact source SHA** — the clean lane's own Golden job, run `29540554179`, built from a deleted Library. |
| Build profile | `GoldenSlice` — verified in the artifact's `recovery_golden_build_profile.json` (`"result": "Succeeded"`). |
| Compile define | `ZIPTIDE_RECOVERY_GOLDEN` — verified in the same report. |
| Locked scenes | `_Boot`, `W000_DriftIn`, `ToxicCity` only — verified in the same report. |
| APK artifact | `clean-golden-apk-2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295` (attached to run `29540554179`) |
| APK SHA-256 | `9bfe13ac0acda6718c3ae1664919cc5609385e50691b8676219c552d8e555a10` |
| Build-profile report SHA-256 | `89c373563115fb3bee55ce095777180f5fa45f4f309f07babffde2d07767f722` |
| Artifact review | Sampled 2026-07-17 by Fable 5 (independent of the recovery build lane): APK SHA-256 **recomputed independently, byte-identical** to the workflow record; build profile scenes/define/result verified; resolved lock confirms Input System `1.6.3` + XRI `2.4.3` + OpenXR `1.14.3` from a cold import; clean-run player log contains **zero NullReferenceExceptions**, zero settle timeouts, and both golden visual captures (`RECOVERY_GOLDEN_VISUAL_OK 2/2`); blank-frame/UI/spawn/census/perf evidence classes previously human-reviewed at rb35/rb37 on the same suite. |
| Authorized by | Fable 5, 2026-07-17, at Terry's direction (rb38). |

**§1a — lineage notes (read before disputing currency):** commits on `terry-local-wip` after the
source SHA are evidence-only (generated observations, HANDOFF, this document) plus the bot-pinned
`Ziptide/Packages/packages-lock.json` from the clean run itself. That pinned lock differs from the
Android artifact's lock ONLY in Linux host toolchain entries (`com.unity.sysroot*`,
`com.unity.toolchain.linux-x86_64`) — CI-host build tooling, not shipped content; the gameplay
package graph is identical. No proof-relevant runtime source has changed since the source SHA.

**Stop:** if any row is pending, stale, from a different SHA, or contradicted by a later proof-relevant commit, the headset pass is not authorized.

---

## 2. PC and headset preflight

### Required hardware/state

- [ ] Quest charged above 40%.
- [ ] Developer Mode enabled.
- [ ] USB cable connected directly to the PC.
- [ ] Inside the headset, accept the USB-debugging prompt and select **Always allow from this computer** if offered.
- [ ] Unity Editor closed.
- [ ] No other Ziptide build/session running.
- [ ] Enough clear guardian/play space to stand, rotate and walk several steps safely.

### Verify the connected device

From PowerShell:

```powershell
cd C:\Ziptide
adb start-server
adb devices
```

- [ ] Exactly the intended Quest appears with status `device`, not `unauthorized` or `offline`.
- [ ] Record the serial shown by `adb devices` in the test result.

### Get the exact candidate

Preferred checkpoint method: use the **authorized Golden APK artifact** named in §1. Do not rebuild a different commit locally and call it the same candidate.

1. Download the named artifact from the exact Golden Android run —
   **https://github.com/TerryMaloney/Ziptide/actions/runs/29540554179** → *Artifacts* →
   `clean-golden-apk-2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295`.
2. Unzip it into a new folder named for the source SHA, e.g. `C:\Ziptide\QuestBuilds\2b158b4\`
   (the APK is inside at `Ziptide\Builds\Android\Ziptide.apk`).
3. Verify checksum:

```powershell
Get-FileHash .\Ziptide.apk -Algorithm SHA256
```

- [ ] Hash exactly matches §1.
- [ ] File is named/stored with the source SHA so it cannot be confused with another APK.

### Install cleanly and start evidence capture

```powershell
adb logcat -c
adb install -r .\Ziptide.apk
```

Open a second PowerShell window and keep this running for the whole pass:

```powershell
cd C:\Ziptide
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
adb logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath ".\Ziptide\Builds\quest_checkpoint_${stamp}.log"
```

- [ ] Log file begins before app launch.
- [ ] Record local start time.
- [ ] Launch Ziptide from the Quest library, not from a different APK shortcut.

---

## 3. Headset test — execute in this order

Use **PASS**, **FAIL**, or **NOT OBSERVED** for every row. On any blocker failure, stop broad exploration, note the time and nearest visible action, preserve the log, and exit the app.

### A. Cold boot and Home Hub — 2 minutes

- [ ] **Cold boot survives 60 seconds.** Put on the headset before launch and remain at the Home Hub for a full minute. No sinking, falling, respawn flash, repeated scene load, black loop, or unexpected travel.
- [ ] **Boot hold owns locomotion.** While Home Hub owns the rig, push both sticks hard in every direction for at least five seconds. The player must not translate or turn.
- [ ] **Tracking is correct.** Head and both controllers track without frozen hands, large offsets or controller swapping.
- [ ] **One canonical rig.** No duplicate camera view, doubled hands, doubled rays or competing interaction cursors.
- [ ] **Home surfaces are readable.** NEW GAME, CONTINUE/availability, SETTINGS and title are front-facing, not mirrored, not severely clipped and do not overlap.
- [ ] **Selection works once.** Point/select SETTINGS once, close it, then select NEW GAME once. No double activation, dead first click or repeated travel.

Expected log evidence includes `BOOT_HOLD on`, one Home Hub ready sequence and no exception.

### B. First W000 arrival — 3 minutes

- [ ] **Exactly one travel and one arrival.** NEW GAME produces one transition and lands once in W000.
- [ ] **Transition clears.** The white/pale travel shell fully disappears; vision is not trapped inside the teleport effect.
- [ ] **Standing pose is valid.** Floor is below the player, head height feels normal, torso is not inside a wall/ship/floor, and no immediate respawn occurs.
- [ ] **Controls wake safely.** Immediately after arrival—not after waiting—perform all of the following for 15 seconds:
  - [ ] move forward/back/left/right;
  - [ ] snap turn both directions;
  - [ ] continuous/smooth turn if enabled by the current preset;
  - [ ] stop and restart movement.
- [ ] **No dead/glitchy travel controls.** No frozen stick, delayed wake, violent turn, exception-induced control loss or need to reopen a menu.
- [ ] **W000 is actually rendered.** The player sees the ship/interior/world, not a uniform clear color, black frame, pink materials or missing geometry.
- [ ] **Panels are physically owned.** Ship/helm/quarters/disembark surfaces appear only where expected, face the player from the usable side and do not follow the head unnaturally.
- [ ] **Starter interaction exists.** At least one intended item/weapon can be seen, grabbed, released and recovered/holstered without dropping dead or disappearing.
- [ ] **No obvious fallback art blocker.** No magenta/error shader, null-material object, emergency `RuntimeMaterialFixer` surface or invisible active object at the checkpoint.

### C. W000 → ToxicCity — 3 minutes

Use the intended Golden travel surface/helm. Do not use ADB or an unrelated debug warp.

- [ ] Destination selection clearly identifies ToxicCity and accepts one confirmation.
- [ ] One travel begins; input/menu cannot launch a second concurrent travel.
- [ ] Transition is comfortable and fully clears.
- [ ] ToxicCity appears as an actual city/world with geometry, sky, lighting and readable RILL/dialogue—not the pale-cyan inside of the travel shell.
- [ ] Spawn height and clearance feel valid.
- [ ] **Critical race check:** immediately repeat move + snap turn + continuous turn for 15 seconds.
- [ ] No control loss, one-frame violent turn, exception, duplicate rig or input manager symptom.
- [ ] Scene has no obvious Home Hub/W000 leak.
- [ ] Readable destination text is not mirrored/clipped/overlapped from the normal viewpoint.
- [ ] Visual frame pacing is acceptable while looking across the scene and rotating 360° slowly.

### D. ToxicCity → returned W000 — 3 minutes

- [ ] Return through the canonical travel path.
- [ ] Transition clears and returns to W000 once.
- [ ] **Critical repeat check:** immediately repeat move + snap turn + continuous turn for 15 seconds.
- [ ] The same persistent rig/hands/input behavior is present; no duplicate or reset-to-broken state.
- [ ] W000 composition and panels are still correct.
- [ ] The earlier grabbed/holstered or probe state persists according to the build’s intended save/overlay behavior.
- [ ] No obvious cumulative hitch, audio duplication, material corruption or resource-related presentation change after the round trip.

### E. Save, relaunch and continue — 2–4 minutes

- [ ] Exit the app normally after the return autosave has occurred.
- [ ] Relaunch Ziptide.
- [ ] CONTINUE is available and selects once.
- [ ] Continue reaches the expected valid location/state without a broken rig, invalid spawn or duplicated persistent owners.
- [ ] Movement and both turn methods work immediately after continue.
- [ ] The chosen persistence probe/item/resource state remains correct.

---

## 4. Immediate blocker conditions

Any one of these is a **checkpoint failure**:

- crash, freeze, ANR or repeated boot/load loop;
- fall/sink/respawn loop at Home Hub or arrival;
- movement or turning active while Home Hub boot hold owns the rig;
- move/snap/smooth-turn dead or throwing immediately after travel;
- `NullReferenceException` in Input System/XRI locomotion;
- duplicate rig/camera/XRI/InputActionManager symptoms;
- travel effect never clears or view is uniform/blank/pale shell;
- wrong scene, double travel, missing arrival or invalid standing spawn;
- save/continue loses or duplicates the proof state;
- pink/error/null-material visible blocker;
- severe sustained judder or comfort failure preventing completion.

Non-blocking visual/art notes should still be captured, but they do not replace the bounded pass/fail criteria above.

---

## 5. End evidence capture

After the final relaunch/continue check:

1. Remove the headset.
2. In the live-log PowerShell window press `Ctrl+C`.
3. Pull one additional bounded dump — **do NOT use `quest_smoke.ps1` here; it rebuilds and
   replaces the certified install.** Use the raw buffer dump instead:

```powershell
cd C:\Ziptide
adb logcat -d -v threadtime -s Unity Ziptide > .\Ziptide\Builds\quest_checkpoint_extra_dump.log
```

4. Search the captured file:

```powershell
Select-String -Path .\Ziptide\Builds\quest_checkpoint_*.log -Pattern `
  "Exception|NullReference|TRAVEL_FAIL|XRI_NOT_READY|INPUT_MUTATION_SETTLE_FAIL|INPUT_MUTATION_SETTLE_TIMEOUT|BOOT_HOLD|SPAWN_AT|TRAVEL_OK|HEALTH_SWEEP"
```

- [ ] No unhandled exception/NullReference.
- [ ] No `TRAVEL_FAIL`.
- [ ] No `XRI_NOT_READY` at a successful arrival.
- [ ] No `INPUT_MUTATION_SETTLE_FAIL` or timeout.
- [ ] Expected travel sequence exists exactly once per hop.
- [ ] Boot hold turns on at Home Hub and releases only after safe content spawn.
- [ ] Health sweeps exist for expected scenes without blockers.

Preserve:

- exact APK and SHA-256;
- source SHA;
- Quest serial/model/OS version;
- test start/end times;
- full log file;
- PASS/FAIL notes for every row;
- photos/video only where a visual issue needs clarification.

---

## 6. Result template

```text
ZIPTIDE GOLDEN QUEST CHECKPOINT
Source SHA:
APK SHA-256:
Golden run:
Quest model / OS:
Device serial:
Start/end time:

A Cold boot/Home Hub: PASS / FAIL
B First W000 arrival: PASS / FAIL
C W000 -> ToxicCity: PASS / FAIL
D ToxicCity -> W000: PASS / FAIL
E Save/relaunch/continue: PASS / FAIL

First failing step and exact time:
What was visible/felt:
Nearest ZIPTIDE log lines:
Log filename:
Additional non-blocking quality notes:
```

---

## 7. What happens after this pass

### If all blocker rows pass

- Recovery may issue an exit report for the proven foundation.
- The wider project resumes from `docs/PROJECT_COMPLETION_ROADMAP.md`.
- Separate targeted Quest campaigns follow for ship art/flight, shooter combat, creatures, gardens/factories, worlds/skyscapes, vehicles and multiplayer.

### If any blocker row fails

- The exact log/artifact becomes the next recovery source of truth.
- Do not install a different build, test unrelated systems or “work around” the failure during the same session.
- Recovery remains open until the same unchanged route is re-proven in automation and on Quest.

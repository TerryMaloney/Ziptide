# Ziptide headset cutoff — September 30, 2026

Tested source: **91d837c25ba09a9dee82d9c2f2ef7dad708bb6d8**.
Branch: `claude/quest-recovery-20260929`. Unity: **6000.2.9f1**.
CI: https://github.com/TerryMaloney/Ziptide/actions/runs/36709452926
Unity EditMode and generated-scene patch/audit passed. Android APK job was skipped;
Windows build, installation, VR interaction and performance are not yet certified.

Included: W000 objective/pickup/repair-stage persistence, completion receipts, save preparation,
reward validation and explicit repair-listener cleanup. Other worlds retain existing persistence
behavior. No last-scene/player-position restoration was added. The tutorial profile-reset follow-up
is NOT included: its nine-case draft is preserved in `drafts/20260930_profile_lifecycle.patch`.
Do not apply that draft to this test build. RUN-02 remains open.

## 1. Prepare the PC and preserve old work

Use normal Windows PowerShell, not an administrator window. Have Git and Unity Hub installed.
In Unity Hub, install **6000.2.9f1** with **Android Build Support**, **Android SDK & NDK Tools**,
and **OpenJDK**. Activate/sign in to the editor if needed.

Use a NEW folder so existing local scenes/art stay untouched. Run each block in order. Stop if
any command fails. This pins the green source even if the branch advances later.

```powershell
$repo = 'C:\Ziptide-Headset-20260930'
$cutoff = '91d837c25ba09a9dee82d9c2f2ef7dad708bb6d8'
if (Test-Path $repo) { throw 'Choose a NEW folder; do not overwrite an existing checkout.' }
git clone --branch claude/quest-recovery-20260929 https://github.com/TerryMaloney/Ziptide.git $repo
if ($LASTEXITCODE -ne 0) { throw 'Clone failed. Stop here.' }
Set-Location $repo
git switch --detach $cutoff
if ($LASTEXITCODE -ne 0) { throw 'Could not select the cutoff. Stop here.' }
git log -1 --oneline
$unity = 'C:\Program Files\Unity\Hub\Editor\6000.2.9f1\Editor\Unity.exe'
$adb = Join-Path (Split-Path $unity -Parent) 'Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe'
if (!(Test-Path $unity) -or !(Test-Path $adb)) { throw 'Install the editor and Android modules, or correct $unity and $adb.' }
```

A detached checkout is intentional for this fixed test revision. In Hub, Add project from disk:
`C:\Ziptide-Headset-20260930\Ziptide` (the INNER folder). If opened, let import finish, then save
and close the editor before running the build. Do not delete lock files or kill Unity to bypass errors.
If Git asks for access, use your normal GitHub sign-in; do not paste credentials into chat.

## 2. Build the full route without needing the headset

Keep the first PowerShell window open so its variables remain available.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\ziptide_snapshot.ps1 -AdbExe $adb
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\dev_build_install.ps1 -UnityExe $unity -BuildProfile FullDevelopment -BuildOnly -PreflightOnly
if ($LASTEXITCODE -ne 0) { throw 'Prerequisites failed. Stop and save the error.' }
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\dev_build_install.ps1 -UnityExe $unity -BuildProfile FullDevelopment -BuildOnly
if ($LASTEXITCODE -ne 0) { throw 'Build failed. Keep android_build.log; do not install an older APK.' }
```

Use **FullDevelopment**, not GoldenSlice. This invokes the required authoring/scene patch pipeline,
including W000 checkpoint metadata, before building. Generated files may make this new checkout dirty;
that is recorded in the manifest. Do not pull/reset it during the test.

The script prints `Evidence: ...` and `BUILT (not installed): ...`. Copy the evidence folder below:

```powershell
$buildSession = 'C:\Ziptide-Headset-20260930\Ziptide\Builds\sessions\PASTE-BUILD-SESSION'
$manifest = Join-Path $buildSession 'manifest.json'
Get-Content $manifest
```

Keep its APK, `manifest.json`, `android_build.log` and `source-before.patch` together.

## 3. Connect, install and boot-check

Enable Quest developer mode/USB debugging, use a data-capable USB cable, put on the headset and
accept the USB debugging prompt. The device must show `device`, not `unauthorized` or `offline`.

```powershell
& $adb devices -l
$serial = 'PASTE-QUEST-SERIAL-FROM-DEVICE-LIST'
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\quest_install.ps1 -ManifestPath $manifest -AdbExe $adb -Serial $serial -Logcat
if ($LASTEXITCODE -ne 0) { throw 'Install or boot check failed. Keep the evidence folder and error.' }
```

Installation uses `install -r` and preserves app data. A signature conflict is a stop/report condition;
do not uninstall or clear data to work around it. The boot check launches the game and captures 15
seconds. It does not prove gameplay. If needed, find Ziptide in the headset's Unknown Sources list.

## 4. Start a full play-session log (window 1)

This restarts the installed app and clears the DEVICE log buffer. It preserves previous capture files.
Use it before each play/reload pass; do not run multiple captures against the same app concurrently.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\level1_test.ps1 -NoBuild -AdbExe $adb -Serial $serial
```

It prints `Capture: ...`. Leave it running while playing. Press Enter in this window ONLY after the
pass is finished. The session records logcat, ADB stderr, installed package details and capture metadata.
Keep the install manifest too: the capture alone cannot identify the source of a preinstalled APK.
A capture-check failure does not erase the logs; send them anyway.

## 5. Screenshots, video and marks (window 2)

Open a SECOND PowerShell window. Variables do not carry over; set these again:

```powershell
Set-Location 'C:\Ziptide-Headset-20260930'
$adb = 'C:\Program Files\Unity\Hub\Editor\6000.2.9f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe'
$serial = 'PASTE-QUEST-SERIAL'
$captureSession = 'PASTE-THE-CAPTURE-FOLDER-PRINTED-IN-WINDOW-1'
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\quest_capture.ps1 -OutDir $captureSession -AdbExe $adb -Serial $serial
```

Keys in this second window: **S** screenshot, **R** start/stop video, **M** timestamped log mark,
**Q** finish and pull the last recording. Clips stop after three minutes; press R to save that clip.
These are keyboard controls, so take the headset off or have someone operate the window. If capture
is unsupported/fails, keep logcat and use the headset's own recording; note the time and symptom.

## 6. Headset checklist — check in this order

Mark each item PASS / FAIL / NOT TESTED. Record the visible objective and world at any stall.
First use Continue if there is existing progress you want to inspect. **New Game replaces the current
profile**; use it only when willing to replace that progress. Do not use adb clear-data or uninstall.

### Basic route (highest priority)

- [ ] Cold boot: menu readable and reachable; controller rays/buttons work; no black screen or stuck loading.
- [ ] New Game or Continue: enters the expected existing route; no duplicate rig, lost hands or floor drop.
- [ ] Movement/turning: controls work, comfort settings make sense, no persistent judder or nausea.
- [ ] Grab/drop/holster: keepsake and ordinary items work; holstered items survive travel.
- [ ] W000: objective text changes correctly; manifest can be collected; panel, part and power interactions work.
- [ ] Launch/travel: correct destination, stable arrival, working hands/movement after transition.
- [ ] City: follow available objectives; note missing markers, unreachable items, empty required spaces or blocked doors.
- [ ] Return trip: arrives safely; no duplicate objects or accumulating performance loss. Repeat travel once if practical.

### W000 resume (new in this cutoff)

These checkpoints must be tested BEFORE moving on to the next stage. If the manifest/job is auto-started
or the sequence cannot be reached naturally, mark that subcase NOT TESTED and describe the actual flow.

- [ ] On a fresh willing-to-reset profile, collect the manifest before accepting the job if possible.
- [ ] Wait several seconds for saving; pause via the Quest system menu, then return. Confirm no lost progress.
- [ ] Finish the capture (Enter), then rerun the window-1 command to cold-relaunch without rebuilding.
      Continue and enter W000 through its existing route. Manifest stays gone and objective credit is retained.
- [ ] Remove the coupler panel; save/pause, end capture and cold-relaunch. Panel stays off; part/socket step remains usable.
- [ ] Seat the part; repeat. Power switch remains available; the consumed loose part does not respawn.
- [ ] Power on; repeat. Coupler remains running without replayed repair haptics/audio or repeated completion.
- [ ] Complete/revisit W000. Job does not reset; world access remains correct. W000 pays zero credits by design.
- [ ] Check tutorial prompts agree with physical progress. Record any stuck/old prompt—the profile-reset follow-up is not shipped.

Look in logs for `MISSION_RESTORED`, `MISSION_CHECKPOINT_SAVED` and `SAVE_OK`. Treat
`MISSION_RESTORE_BLOCKED`, `MISSION_COMMIT_FAIL`, `SAVE_FAIL` or missing mission logs on W000
as evidence to send, not something to bypass. Pause saves can emit SAVE_OK without the checkpoint tag.
Do not force-stop immediately after an action before allowing a successful save. This is not a power-loss torture test.

### Extra checks if time remains

- [ ] Suspend/resume headset while holding an item; controls and objective remain usable.
- [ ] Second New Game in the same process: record stale tutorial/hint/return behavior as the known RUN-02 target.
- [ ] Visual/audio review: readable panels, scale, collisions, lighting, sound balance, missing assets, worst judder location.
- [ ] One short clip each of ship, city arrival, repair interaction and any failure.

## 7. Package what to send back

In window 2, Q saves any recording. In window 1, Enter stops logcat. Then:

```powershell
$captureSession = 'PASTE-THE-CAPTURE-FOLDER'
$notes = @'
Build: 91d837c2 / FullDevelopment
Headset model:
Passes completed:
First failure / visible objective / world:
Exact actions just before failure:
Screenshot or clip filename / time mark:
Resume state expected versus observed:
Comfort/performance observations:
Not tested:
'@
$notes | Set-Content (Join-Path $captureSession 'terry-notes.txt') -Encoding UTF8
notepad (Join-Path $captureSession 'terry-notes.txt')
$reportZip = Join-Path $env:USERPROFILE ('Desktop\Ziptide-report-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.zip')
$evidence = @($captureSession, $manifest, (Join-Path $buildSession 'android_build.log'), (Join-Path $buildSession 'source-before.patch'))
$evidence += @(Get-ChildItem $buildSession -Directory -Filter 'install-*' | Select-Object -ExpandProperty FullName)
Compress-Archive -Path $evidence -DestinationPath $reportZip
Write-Host $reportZip
```

Send the ZIP and your checklist verdict. Add earlier capture folders if the problem spans reloads.
Large videos can be uploaded separately; the APK is not needed for initial diagnosis. If the build
failed, send `android_build.log`, manifest if present, and the exact PowerShell error instead.

## Assistant continuation after this cutoff

No new runtime scope is authorized by this handoff itself. Next work already queued: review device
results, then re-review and apply the parked RUN-02 draft with its nine behavioral tests through full
preflight and fresh Unity CI. Draft changes only FirstHourDirector and tests; RILL queue/flag history
and W001 payoff latches remain separate audit items. Preserve the existing unrelated audio temp file.

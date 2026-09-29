# Quest recovery - current operator entry point

Updated 2026-09-29. Candidate branch: `claude/quest-recovery-20260929`, based on migration
`8fd80a8bc3b201cf0baac7484d7b2a2b09a3955a`. Engine stays **6000.2.9f1**.

**Publication status:** changes are committed locally; automatic approval review rejected the
GitHub push because it requires explicit authorization for this remote write. The candidate
branch is not yet on GitHub, so the clone command below becomes usable only after that push.
Unity CI has not run for this candidate.

## Goal and honest status

Make the existing game repeatably buildable, installable, and observable before changing its
runtime spine. This is a recovery candidate, not a finished first level or release build.
Terry owns artistic direction. The assistant owns mechanics, engineering and supporting tools.

- Local verification: 244 Python gate tests and 25 PowerShell behavior checks passed;
  actual offline repository gates pass, with existing release holds and advisory findings retained.
- PowerShell checks ran with 7.4.6 on Linux; Windows 5.1, Unity compilation/generated audit,
  Android build, USB installation and headset performance need their own evidence.
- Historical blocker: Unity 6000.2.9f1 lacked Android Build Support on Terry's PC in August.
  This has not been rechecked on that machine.
- No scenes, prefabs, rig/input/travel/save ownership, graphics or global physics settings
  have been edited by this recovery pass.
- Outstanding game work: mission persistence, route reconciliation, production records,
  performance/material counts, W002 reuse, then W003-W005 production proof. See
  [whole-project plan](production/WHOLE_PROJECT_PLAN_20260928.md).

## Preserve local work first

Use a **separate checkout** for this candidate; the Unity build regenerates assets and scenes.
Keep the old PC folder intact, including unsaved or uncommitted art and scene work. A clean
candidate tests the repository baseline; local-only work can be compared and integrated later.
Do not run `git reset --hard`, clean the old checkout, or blindly pull a branch into it.

From PowerShell, choose a new, unused folder (example):

```powershell
git clone --branch claude/quest-recovery-20260929 https://github.com/TerryMaloney/Ziptide.git C:\Ziptide-Recovery-20260929
cd C:\Ziptide-Recovery-20260929
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\ziptide_snapshot.ps1
```

If clone fails, stop there; do not run the following commands in another checkout. Import this
new `Ziptide` subfolder into Unity Hub. Sign in/activate the editor locally as needed.
In Hub, install 6000.2.9f1 with **Android Build Support**, **Android SDK & NDK Tools**, and
**OpenJDK**. Open the project once if needed, let import finish, then save and close the editor.
The preflight uses the editor's `-version` output and checks the bundled Android toolchain.
A custom editor path can be passed using `-UnityExe`; it must be the same version.
External SDK layouts are not handled automatically by this candidate.

## Prepare an APK without a headset

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\dev_build_install.ps1 -BuildProfile FullDevelopment -BuildOnly -PreflightOnly
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\dev_build_install.ps1 -BuildProfile FullDevelopment -BuildOnly
```

Preflight checks installation, modules, and project locks. It does not certify licensing or
Android compilation; the actual build determines those. Nothing kills Unity or removes locks.
If a lock persists after closing, reopen and close the project normally; share the error if it persists.

Every attempt uses `Ziptide\Builds\sessions\<timestamp-id>`. A successful build produces:

- `Ziptide-FullDevelopment.apk` and `manifest.json` with SHA-256, source commit, dirty file list,
  before/after generation state, exact editor, build profile and last successful stage.
- `android_build.log` and `source-before.patch` for diagnosis. Untracked files are listed but
  their contents are not backed up: use the clean checkout above for reproducible candidates.
- Any older shared output is preserved as `previous.apk` and cannot satisfy the new build.

The script's old default **GoldenSlice** is retained for existing recovery callers. It is a
three-scene diagnostics profile; use **FullDevelopment** for the full first route.
Required contract generation now happens inside the shared Unity patch/audit pipeline.
`BuildAndroid.APK` remains an explicitly build-only low-level entry point; use the patch-and-build
commands above for prepared content. `-SkipBake` is retired.

## Load the saved APK onto Quest

Enable developer mode/USB debugging, connect by USB, and accept the debugging prompt in the
headset. The scripts require exactly one authorized device or an explicit `-Serial`.
They prefer Unity's bundled ADB when building. For install-only, provide its path if ADB is
not already on PATH:

```powershell
$adb = 'C:\Program Files\Unity\Hub\Editor\6000.2.9f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe'
# Replace the path below with the manifest printed by the successful build.
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\quest_install.ps1 -ManifestPath 'C:\Ziptide-Recovery-20260929\Ziptide\Builds\sessions\YOUR-SESSION\manifest.json' -AdbExe $adb -Logcat
```

Installation verifies the APK hash before sending it to the selected headset. It updates the
app (`adb install -r`) without clearing its saved data. A signing conflict is an error: the
script will not uninstall the game or erase saves to bypass it. Keep each APK with its manifest.
An install receipt and fresh boot log are saved beside the build session.

Alternatively, build/install/boot-check in one command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\quest_smoke.ps1 -BuildProfile FullDevelopment
```

`built`, `installed`, `launched`, and `boot-smoke-passed` are separate stages. Missing device,
failed install/launch, empty log, wrong profile and detected runtime failures return nonzero.
A 15-second boot observation does not prove route completion or comfortable frame rate.

## Play and report

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\level1_test.ps1 -NoBuild -AdbExe $adb
```

This retests the installed app without requiring Unity. It records installed package metadata
and explicitly labels its source unknown; the current checkout SHA is not falsely assigned to it.
Match it to the earlier install manifest/receipt when reporting. Capture begins at launch and
stops when you press Enter. In a second PowerShell window:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\quest_capture.ps1 -OutDir 'PASTE-CAPTURE-SESSION-FOLDER' -AdbExe $adb
```

Use S for screenshots, R for video and M for a log mark. Upload the session logs/manifest,
short clips and your verdict here; the APK itself is unnecessary for initial diagnosis.
For video/casting, Meta Quest Developer Hub is also suitable.

First test boot/New Game, hands and movement, grab/holster, ship launch/travel, city arrival,
return/escape, quit and Continue. Only then attempt the full
[Level 1 test card](production/TONIGHT_TEST_CARD.md). Record stalled steps and current objective.
This card is a functional test, not permission to call unfinished art or empty interiors done.

## Visibility and low-cost connections

1. **Existing GitHub connection:** code, history, CI logs and build artifacts. It is already working.
2. **Windows-local Codex/ChatGPT workflow:** open this candidate checkout on the development PC.
   Local tools can run installed PowerShell, Unity batch builds and ADB within granted permissions.
   This cloud conversation currently has no access to the PC's filesystem, Unity window or USB.
3. **Computer Use in the Windows desktop app, if offered on your account:** grant access to Unity
   and Quest Developer Hub for scene/UI inspection. Keep target windows visible. This is the
   visual complement to command-line evidence; availability and plan access need checking locally.
4. **Meta Quest Developer Hub:** capture views/video and performance traces; attach those here.
   Six useful views: ship interior, arrival, main route, relay, flats, and return.
5. **Optional scrcpy:** free/open-source Android mirroring/recording. Prefer MQDH first for Quest;
   headset display layouts and compatibility need local verification.

No paid assets, cloud GPU service or custom Unity MCP is required to start. A dedicated Unity
bridge can be evaluated later if local commands and Computer Use leave a specific gap. More
connections alone do not prove VR comfort or performance. Do not send account passwords/API keys.

Official references checked 2026-09-29:
- [Unity 6.2 Android setup](https://docs.unity3d.com/6000.2/Documentation/Manual/android-sdksetup.html)
- [Unity command-line arguments](https://docs.unity3d.com/6000.2/Documentation/Manual/EditorCommandLineArguments.html)
- [Codex CLI](https://developers.openai.com/codex/cli)
- [Windows local sandbox](https://developers.openai.com/codex/windows)
- [Computer Use](https://developers.openai.com/codex/app/computer-use)
- [Quest Developer Hub](https://developers.meta.com/horizon/documentation/unity/ts-mqdh-getting-started/)
- [MQDH performance](https://developers.meta.com/horizon/documentation/unity/ts-mqdh-logs-metrics/)
- [scrcpy official repository](https://github.com/Genymobile/scrcpy)

## Next acceptance gates

1. Current candidate Unity compile/EditMode and generated scene audit green.
2. Windows prerequisites and full Android build succeed; hash and logs retained.
3. Exact candidate installed and boot profile verified on the chosen headset.
4. Route, repeated travel and save/resume tested; performance captured in dense scenes.
5. Strengthen shared mission state/production records, prove W002 reuse, then W003-W005.

The current hardware/environment handoff is necessary because this session cannot run Terry's
Windows Unity installation or connect to his USB headset. It is not a request to approve more planning.

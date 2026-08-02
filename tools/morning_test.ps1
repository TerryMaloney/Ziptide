<#
.SYNOPSIS
    ONE COMMAND for the 2026-08-01 Level 1 pass: get the APK on the headset and start logging.

.DESCRIPTION
    The default path DOES NOT BUILD ANYTHING. CI already ran the exact method
    dev_build_install.ps1 runs (RecoveryBuildAndroid.PatchScenesThenGoldenAPK) on the current
    code and left a verified APK in the run's artifacts, so the fastest morning is: download,
    install, play. No Unity, no 20-minute bake, nothing to go wrong before coffee.

    It reuses the scripts that already exist rather than reinventing them:
      install_latest.ps1  finds the APK, prints its SHA-256, uninstalls first (the signature law),
                          clears logcat and installs
      level1_test.ps1     the local bake + build + install path, used only with -Local

.PARAMETER ApkPath
    Path to an APK you already downloaded. Skips the gh download entirely.

.PARAMETER Local
    Build it yourself instead of using CI's APK. Runs level1_test.ps1 (bake + build + install).
    Use this if you also want the generated theme/profile assets committed — see step 4 below.

.PARAMETER Run
    CI run id holding the Golden APK. Defaults to the verified 2026-08-01 run.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\morning_test.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\morning_test.ps1 -ApkPath "$HOME\Downloads\Ziptide.apk"

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\morning_test.ps1 -Local
#>
param(
    [string]$Repo = "C:\Ziptide",
    [string]$ApkPath,
    [switch]$Local,
    [string]$Run = "30722864917"
)

$ErrorActionPreference = 'Stop'
function Say($m, $c = 'Cyan') { Write-Host $m -ForegroundColor $c }

Push-Location $Repo
try {
    Say "===== ZIPTIDE — MORNING TEST =====`n"

    # ── 1. Latest code ───────────────────────────────────────────────────────
    Say "[1/4] pulling terry-local-wip..."
    git pull origin terry-local-wip
    Say ("      at " + (git rev-parse --short HEAD) + " on " + (git branch --show-current)) 'DarkGray'

    # ── 2. Get an APK ────────────────────────────────────────────────────────
    if ($Local) {
        Say "`n[2/4] LOCAL BUILD (-Local): handing over to level1_test.ps1..." 'Yellow'
        Say "      This bakes the contract, builds, installs and captures a log by itself." 'DarkGray'
        & "$Repo\tools\level1_test.ps1"
        Say "`nlevel1_test.ps1 owns the rest of the run. See step 4 below for the assets to commit." 'Green'
        return
    }

    if (-not $ApkPath) {
        Say "`n[2/4] fetching CI's verified APK (run $Run)..."
        $gh = Get-Command gh -ErrorAction SilentlyContinue
        $dest = Join-Path $Repo "QuestBuilds\ci_$Run"

        if ($gh) {
            New-Item -ItemType Directory -Force -Path $dest | Out-Null
            gh run download $Run --dir $dest
            $found = Get-ChildItem $dest -Recurse -Filter *.apk -ErrorAction SilentlyContinue |
                     Sort-Object Length -Descending | Select-Object -First 1
            if ($found) { $ApkPath = $found.FullName }
        }

        if (-not $ApkPath) {
            # No gh, or it found nothing. GitHub artifact downloads need auth, so a browser is the
            # honest fallback rather than a curl that will 404 and look like a broken script.
            Say "`n  Could not fetch it automatically (the GitHub CLI is not installed or not logged in)." 'Yellow'
            Say "  Download it in the browser — it is the only manual step:" 'Yellow'
            Say "    https://github.com/TerryMaloney/Ziptide/actions/runs/$Run" 'White'
            Say "    artifact: recovery-golden-apk-bd7c733a...  (110 MB)" 'White'
            Say "`n  Then re-run with the path:" 'Yellow'
            Say "    .\tools\morning_test.ps1 -ApkPath `"`$HOME\Downloads\Ziptide.apk`"" 'White'
            Say "`n  Or build it yourself instead:" 'Yellow'
            Say "    .\tools\morning_test.ps1 -Local" 'White'
            return
        }
    }

    # ── 3. Install (install_latest.ps1 owns the signature law + logcat clear) ─
    Say "`n[3/4] installing $ApkPath ..."
    & "$Repo\tools\install_latest.ps1" -Path $ApkPath

    # ── 4. What to watch for ─────────────────────────────────────────────────
    Say "`n[4/4] WHAT TO LOOK AT, in the order you will meet it:" 'Cyan'
    Say "  1. Off the ramp - junk on the berth deck. ONE crate is grabbable and worth nothing."
    Say "  2. LOOK UP - three gantry arches, open at the seaward end so the sky stays the backdrop."
    Say "  3. The crane - 4.5 m wide, 30 cm rungs, walkway, cab. The hook creeps on a 30 s loop."
    Say "  4. Lanterns start at the QUAY now, not at Dispatch. Your first step already follows them."
    Say "  5. THE SKY. ToxicCity renders its own authored theme for the first time ever."
    Say "`n  The sky is the one nobody has laid eyes on. If it reads wrong the commit is 78babef9." 'Yellow'

    Say "`nPaste this in a SECOND window BEFORE you put the headset on:" 'Yellow'
    Say '  cd C:\Ziptide' 'White'
    Say "  `$stamp = Get-Date -Format 'yyyyMMdd_HHmmss'" 'White'
    Say "  adb logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath `".\Ziptide\Builds\quest_`${stamp}.log`"" 'White'

    Say "`nAfterwards, Ctrl+C that window and run:" 'Yellow'
    Say '  .\tools\morning_check.ps1' 'White'
}
finally { Pop-Location }

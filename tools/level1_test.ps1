<#
.SYNOPSIS
    ONE COMMAND: bake the first level, build the full APK, install it, launch it, and capture logcat.

.DESCRIPTION
    Replaces the six-menu bake batch in docs/production/LEVEL1_BAKE_AND_SMOKE.md §1 plus the build
    and install in §2.

    The APK build (BuildAndroid.PatchScenesThenAPK) already runs five of the six bake steps as
    required hooks: W000 surfaces, Compile World Specs, Build Toxic City, Build Space Lane, and
    Generate All Layout Worlds. It does NOT run "Build Toxic City Contract", so this script runs
    that one first, by itself, in batch mode. That ordering matters: the committed contract asset
    is a stale five-step version with no expedition leg, so skipping it means playing the old job.

    Everything runs unattended. Nothing here needs the Unity editor open -- close it first.

.PARAMETER SkipBake
    Skip the contract bake and go straight to build+install. Use only when re-installing an
    unchanged tree.

.PARAMETER NoBuild
    Skip build+install entirely; just relaunch the installed APK and capture a fresh log. Use this
    for a second run at the same build.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\level1_test.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\level1_test.ps1 -NoBuild
#>
param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe",
    [string]$ProjectRoot = "",
    [switch]$SkipBake,
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot  = Split-Path -Parent $scriptDir

if ($ProjectRoot -eq "") {
    $ps = Get-ChildItem $repoRoot -Directory -Recurse -Filter ProjectSettings -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($ps) { $ProjectRoot = Split-Path $ps.FullName -Parent } else { $ProjectRoot = Join-Path $repoRoot "Ziptide" }
}

$stamp    = Get-Date -Format "yyyyMMdd-HHmmss"
$outDir   = Join-Path $repoRoot "Builds\session-$stamp"
$logPath  = Join-Path $outDir "logcat.log"
$bakeLog  = Join-Path $outDir "bake_contract.log"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

Write-Host ""
Write-Host "=== ZIPTIDE LEVEL 1 TEST =========================================" -ForegroundColor Cyan
Write-Host "  Session folder : $outDir"
Write-Host "  Project        : $ProjectRoot"

# --- The SHA under test, so a defect report can name the exact source ---------------------
Push-Location $repoRoot
try {
    $sha    = (& git rev-parse --short HEAD 2>$null)
    $branch = (& git rev-parse --abbrev-ref HEAD 2>$null)
    $dirty  = (& git status --porcelain 2>$null)
    Write-Host "  Source         : $branch @ $sha$(if ($dirty) { '  (UNCOMMITTED CHANGES)' })"
    "branch=$branch sha=$sha dirty=$([bool]$dirty)" | Out-File (Join-Path $outDir "source.txt") -Encoding utf8
} finally { Pop-Location }
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host ""

# --- Device check up front: a missing headset should fail in 2 seconds, not after a build ---
$devices = (& adb devices) -join "`n"
if ($devices -notmatch "\sdevice\s*$" -and $devices -notmatch "\sdevice\r?\n") {
    Write-Host "adb sees no authorised device:" -ForegroundColor Red
    Write-Host $devices
    Write-Host "Plug in the Quest, accept the USB debugging prompt in the headset, then re-run." -ForegroundColor Yellow
    exit 1
}
Write-Host "[1/5] Device connected." -ForegroundColor Green

# --- Step 1: the one bake step the APK build does not run ---------------------------------
if (-not $SkipBake -and -not $NoBuild) {
    Write-Host "[2/5] Baking the Toxic City contract (the six-step job, incl. the expedition)..." -ForegroundColor Green
    $lock = Join-Path $ProjectRoot "Temp\UnityLockfile"
    if (Test-Path $lock) { Write-Host "      note: Unity lockfile present - close the editor if this hangs." -ForegroundColor Yellow }

    & $UnityExe -batchmode -quit -nographics -projectPath $ProjectRoot `
        -executeMethod Ziptide.Editor.Patching.ToxicCityContractBuilder.Build `
        -logFile $bakeLog
    if ($LASTEXITCODE -ne 0) {
        Write-Host "      Contract bake FAILED (exit $LASTEXITCODE). Log: $bakeLog" -ForegroundColor Red
        Write-Host "      Last errors:" -ForegroundColor Red
        Select-String -Path $bakeLog -Pattern "error CS|Exception|Aborting" |
            Select-Object -Last 10 | ForEach-Object { Write-Host "        $($_.Line)" }
        exit 1
    }
    Write-Host "      Contract baked. Log: $bakeLog"
} else {
    Write-Host "[2/5] Contract bake skipped." -ForegroundColor Yellow
}

# --- Step 2: build + install the FULL profile (not the three-scene recovery slice) ---------
if (-not $NoBuild) {
    Write-Host "[3/5] Building + installing the full APK (this runs the rest of the bake)..." -ForegroundColor Green
    & "$scriptDir\dev_build_install.ps1" -ProjectRoot $ProjectRoot -UnityExe $UnityExe -BuildProfile FullDevelopment
    if ($LASTEXITCODE -ne 0) { Write-Host "      Build/install FAILED." -ForegroundColor Red; exit 1 }
} else {
    Write-Host "[3/5] Build skipped (-NoBuild): relaunching what is already installed." -ForegroundColor Yellow
}

# --- Step 3: clean log, launch, capture -----------------------------------------------------
$pkg = "com.terrymaloney.ziptide"
Write-Host "[4/5] Clearing the log buffer and launching $pkg ..." -ForegroundColor Green
& adb logcat -c 2>$null
& adb shell monkey -p $pkg -c android.intent.category.LAUNCHER 1 | Out-Null

Start-Sleep -Seconds 2
# ZIPTIDE_CAPTURE must be in the filterspec or the marks and screenshot stamps that
# quest_capture.ps1 writes are silenced by -s and the correlation is lost.
$logcat = Start-Process -FilePath "adb" `
    -ArgumentList @("logcat", "-v", "time", "-s",
                    "Unity:V", "ZIPTIDE_CAPTURE:V", "CRASH:V", "AndroidRuntime:E", "DEBUG:V") `
    -RedirectStandardOutput $logPath -NoNewWindow -PassThru

Write-Host "[5/5] Capturing." -ForegroundColor Green
Write-Host ""
Write-Host "  LOG  -> $logPath" -ForegroundColor Cyan
Write-Host "  PLAY the route now. In a SECOND PowerShell window run:" -ForegroundColor Cyan
Write-Host "     powershell -ExecutionPolicy Bypass -File $scriptDir\quest_capture.ps1" -ForegroundColor White
Write-Host "  for screenshots (S) and video (R)." -ForegroundColor Cyan
Write-Host ""
Write-Host "  Press ENTER here when you are finished playing to stop the capture." -ForegroundColor Yellow
[void](Read-Host)

if ($logcat -and -not $logcat.HasExited) { Stop-Process -Id $logcat.Id -Force -ErrorAction SilentlyContinue }
Start-Sleep -Milliseconds 400

# --- Step 4: the summary that turns a session into a report ---------------------------------
Write-Host ""
Write-Host "=== CAPTURE SUMMARY ==============================================" -ForegroundColor Cyan
if (Test-Path $logPath) {
    $lines = @(Get-Content $logPath -ErrorAction SilentlyContinue)
    Write-Host ("  log lines            : {0}" -f $lines.Count)

    $bad = $lines | Select-String -Pattern "NullReferenceException|Exception:|AndroidRuntime|FATAL|MISSING|_FAIL|BLOCKED"
    $tags = $lines | Select-String -Pattern "ZIPTIDE: ([A-Z0-9_]+)" -AllMatches |
        ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

    Write-Host ("  distinct ZIPTIDE tags: {0}" -f @($tags).Count)
    if (@($tags).Count -gt 0) { Write-Host ("    " + (@($tags) -join ", ")) -ForegroundColor DarkGray }

    if (@($bad).Count -gt 0) {
        Write-Host ("  PROBLEM LINES        : {0}   <-- paste these" -f @($bad).Count) -ForegroundColor Red
        @($bad) | Select-Object -First 20 | ForEach-Object { Write-Host ("    " + $_.Line) -ForegroundColor Red }
        @($bad) | ForEach-Object { $_.Line } | Out-File (Join-Path $outDir "problems.txt") -Encoding utf8
        Write-Host ("  (all of them: {0})" -f (Join-Path $outDir "problems.txt")) -ForegroundColor Red
    } else {
        Write-Host "  PROBLEM LINES        : 0" -ForegroundColor Green
    }
    @($tags) | Out-File (Join-Path $outDir "tags_seen.txt") -Encoding utf8
}
Write-Host ""
Write-Host "  EVERYTHING FOR THE REPORT IS IN:" -ForegroundColor Cyan
Write-Host "     $outDir" -ForegroundColor White
Write-Host "==================================================================" -ForegroundColor Cyan

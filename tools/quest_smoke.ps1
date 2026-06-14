<#
.SYNOPSIS
    Optional smoke test: build, install, capture logcat, then scan for exceptions/fatals. Exits non-zero if found.
.DESCRIPTION
    Calls dev_build_install.ps1 -Logcat, then scans Builds/quest_logcat.log for Exception, NullReferenceException, AndroidRuntime fatal.
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\quest_smoke.ps1
#>
param(
    [string]$ProjectRoot = ""
)

$ErrorActionPreference = "Stop"

if ($ProjectRoot -eq "") {
    $ps = Get-ChildItem C:\Ziptide -Directory -Recurse -Filter ProjectSettings -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($ps) {
        $ProjectRoot = Split-Path $ps.FullName -Parent
    } else {
        $ProjectRoot = "C:\Ziptide\Ziptide"
    }
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
& "$scriptDir\dev_build_install.ps1" -Logcat -ProjectRoot $ProjectRoot
$installExit = $LASTEXITCODE
if ($installExit -ne 0) {
    exit $installExit
}

# ── Scan build log for audit failures (build-time, not device logcat) ──────
$buildLogFile = Join-Path $ProjectRoot "Builds\android_build.log"
$buildBad = $false
if (Test-Path $buildLogFile) {
    $buildContent = Get-Content $buildLogFile -Raw -ErrorAction SilentlyContinue
    if ($buildContent -match "ZIPTIDE: AUDIT_FAIL") {
        Write-Host "quest_smoke: found 'ZIPTIDE: AUDIT_FAIL' in build log ($buildLogFile)"
        $buildBad = $true
    }
    if ($buildContent -match "World audit FAILED") {
        Write-Host "quest_smoke: found 'World audit FAILED' in build log ($buildLogFile)"
        $buildBad = $true
    }
} else {
    Write-Host "quest_smoke: build log not found at $buildLogFile (skipping audit check)"
}
if ($buildBad) {
    Write-Host "quest_smoke: FAILED - world audit blockers detected. See docs/AUDIT_REPORT.md."
    exit 1
}

# ── Scan device logcat for runtime failures ──────────────────────────────────
$logcatFile = Join-Path $ProjectRoot "Builds\quest_logcat.log"
if (-not (Test-Path $logcatFile)) {
    Write-Host "quest_smoke: logcat file not found: $logcatFile"
    exit 0
}

$content = Get-Content $logcatFile -Raw -ErrorAction SilentlyContinue
if (-not $content) {
    exit 0
}

$bad = $false
if ($content -match "Exception") { Write-Host "quest_smoke: found 'Exception' in logcat"; $bad = $true }
if ($content -match "NullReferenceException") { Write-Host "quest_smoke: found 'NullReferenceException' in logcat"; $bad = $true }
if ($content -match "AndroidRuntime.*FATAL") { Write-Host "quest_smoke: found 'AndroidRuntime FATAL' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: XRI_MISSING") { Write-Host "quest_smoke: found 'ZIPTIDE: XRI_MISSING' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: NO_RAY_INTERACTORS") { Write-Host "quest_smoke: found 'ZIPTIDE: NO_RAY_INTERACTORS' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: INPUT_ACTIONS_MISSING") { Write-Host "quest_smoke: found 'ZIPTIDE: INPUT_ACTIONS_MISSING' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: DUP_SINGLETON") { Write-Host "quest_smoke: found 'ZIPTIDE: DUP_SINGLETON' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: INVENTORY_RESTORE_FAIL") { Write-Host "quest_smoke: found 'ZIPTIDE: INVENTORY_RESTORE_FAIL' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: ITEM_DEF_NOT_FOUND") { Write-Host "quest_smoke: found 'ZIPTIDE: ITEM_DEF_NOT_FOUND' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: TRAVEL_FAIL") { Write-Host "quest_smoke: found 'ZIPTIDE: TRAVEL_FAIL' in logcat"; $bad = $true }
if ($content -match "ZIPTIDE: XRI_NOT_READY") { Write-Host "quest_smoke: found 'ZIPTIDE: XRI_NOT_READY' in logcat"; $bad = $true }

if ($bad) {
    Write-Host "quest_smoke: FAILED - exceptions/fatals detected. Inspect: $logcatFile"
    exit 1
}

Write-Host "quest_smoke: PASSED - no Exception/NullReferenceException/FATAL in logcat"
exit 0

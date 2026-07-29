<#
.SYNOPSIS
    Screenshots and video from the headset while you play. Run in a SECOND window next to
    level1_test.ps1.

.DESCRIPTION
    One key per action, so it is usable with the headset on and one hand free:

        S  screenshot now
        R  start / stop recording (Quest caps a single clip at ~3 min; this auto-restarts)
        M  drop a MARK line into the log ("the thing I just saw happened HERE")
        Q  quit (stops any recording and pulls everything)

    Files land beside the session's log, so a defect report is one folder.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\quest_capture.ps1
#>
param(
    [string]$OutDir = ""
)

$ErrorActionPreference = "Continue"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot  = Split-Path -Parent $scriptDir

# Default to the newest session folder level1_test.ps1 made, so both scripts agree without being told.
if ($OutDir -eq "") {
    $latest = Get-ChildItem (Join-Path $repoRoot "Builds") -Directory -Filter "session-*" -ErrorAction SilentlyContinue |
              Sort-Object Name -Descending | Select-Object -First 1
    $OutDir = if ($latest) { $latest.FullName } else { Join-Path $repoRoot ("Builds\session-" + (Get-Date -Format "yyyyMMdd-HHmmss")) }
}
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$devTmp    = "/sdcard/ziptide_capture"
$recording = $false
$recProc   = $null
$recName   = ""
$shots     = 0
$clips     = 0

& adb shell mkdir -p $devTmp 2>$null | Out-Null

function Stop-Recording {
    if (-not $script:recording) { return }
    # screenrecord finalises the mp4 on SIGINT; killing it and waiting is the reliable path.
    & adb shell pkill -SIGINT screenrecord 2>$null | Out-Null
    Start-Sleep -Seconds 2
    if ($script:recProc -and -not $script:recProc.HasExited) {
        Stop-Process -Id $script:recProc.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "  pulling $script:recName ..." -ForegroundColor DarkGray
    & adb pull "$devTmp/$script:recName" (Join-Path $OutDir $script:recName) 2>$null | Out-Null
    & adb shell rm "$devTmp/$script:recName" 2>$null | Out-Null
    $script:recording = $false
    Write-Host "  RECORDING SAVED -> $script:recName" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== ZIPTIDE CAPTURE ==============================================" -ForegroundColor Cyan
Write-Host "  Saving to: $OutDir"
Write-Host ""
Write-Host "   S = screenshot     R = record start/stop     M = mark the log     Q = quit" -ForegroundColor White
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host ""

while ($true) {
    $key = [System.Console]::ReadKey($true)
    switch ($key.Key) {

        "S" {
            $shots++
            $name = "shot_{0:d2}_{1}.png" -f $shots, (Get-Date -Format "HHmmss")
            & adb shell screencap -p "$devTmp/$name" 2>$null | Out-Null
            & adb pull "$devTmp/$name" (Join-Path $OutDir $name) 2>$null | Out-Null
            & adb shell rm "$devTmp/$name" 2>$null | Out-Null
            # Timestamp it in the log too, so a screenshot can be located in the logcat stream.
            & adb shell log -t ZIPTIDE_CAPTURE "SCREENSHOT $name" 2>$null | Out-Null
            Write-Host ("  [{0}] SHOT  -> {1}" -f (Get-Date -Format "HH:mm:ss"), $name) -ForegroundColor Green
        }

        "R" {
            if ($recording) {
                Stop-Recording
            } else {
                $clips++
                $recName = "clip_{0:d2}_{1}.mp4" -f $clips, (Get-Date -Format "HHmmss")
                $recProc = Start-Process -FilePath "adb" `
                    -ArgumentList @("shell", "screenrecord", "--bit-rate", "8000000",
                                    "--time-limit", "180", "$devTmp/$recName") `
                    -NoNewWindow -PassThru
                $recording = $true
                & adb shell log -t ZIPTIDE_CAPTURE "RECORD_START $recName" 2>$null | Out-Null
                Write-Host ("  [{0}] REC   -> {1}  (R again to stop; auto-stops at 3 min)" -f (Get-Date -Format "HH:mm:ss"), $recName) -ForegroundColor Yellow
            }
        }

        "M" {
            $mark = Get-Date -Format "HH:mm:ss"
            & adb shell log -t ZIPTIDE_CAPTURE "MARK $mark" 2>$null | Out-Null
            Write-Host ("  [{0}] MARK  -- note what you just saw" -f $mark) -ForegroundColor Magenta
        }

        "Q" {
            Stop-Recording
            Write-Host ""
            Write-Host ("  {0} screenshot(s), {1} clip(s) in:" -f $shots, $clips) -ForegroundColor Cyan
            Write-Host "  $OutDir" -ForegroundColor White
            Write-Host ""
            exit 0
        }
    }
}

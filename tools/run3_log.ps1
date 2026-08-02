<#
.SYNOPSIS
    STEP 3 of 4. Capture the headset log. Run this in a SECOND window, then put the headset on.

.DESCRIPTION
    Clears the ring buffer first so the capture starts at the moment you do, then streams the Unity
    and Ziptide tags to the screen and to a timestamped file under Ziptide\Builds.

    Press Ctrl+C when you are done playing. The file stays; step 4 grades it.

    Note: if you ran step 2 without -ApkPath, level1_test.ps1 already captured a log for you and
    you can skip straight to step 4.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run3_log.ps1
#>
param([string]$Repo = "C:\Ziptide")

$ErrorActionPreference = 'Stop'
Push-Location $Repo
try {
    Write-Host "===== 3/4  LOG =====" -ForegroundColor Cyan

    $dir = Join-Path $Repo "Ziptide\Builds"
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
    $log = Join-Path $dir ("quest_" + $stamp + ".log")

    adb logcat -c
    Write-Host ("  writing to: " + $log) -ForegroundColor White
    Write-Host "  put the headset on now. Ctrl+C here when you are finished." -ForegroundColor Yellow
    Write-Host ""

    adb logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath $log
}
finally { Pop-Location }

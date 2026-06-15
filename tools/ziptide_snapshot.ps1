<#
.SYNOPSIS
    Quick session/state snapshot. Run this FIRST every session and before each Quest test
    so we always know exactly what is built and what the device last reported.
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\ziptide_snapshot.ps1
#>
param([string]$Repo = "C:\Ziptide")

Write-Host "===== ZIPTIDE SNAPSHOT =====" -ForegroundColor Cyan
Push-Location $Repo
Write-Host ("Branch : " + (git branch --show-current))
Write-Host ("Commit : " + (git rev-parse --short HEAD) + "  " + (git log -1 --pretty=%s))
Write-Host "Uncommitted changes:"
git status --short
Write-Host "Last 5 commits:"
git log -5 --oneline
Pop-Location

Write-Host ""
Write-Host "Build scenes (EditorBuildSettings):" -ForegroundColor Cyan
Get-Content "$Repo\Ziptide\ProjectSettings\EditorBuildSettings.asset" -ErrorAction SilentlyContinue | Select-String "path:"

Write-Host ""
Write-Host "Last ZIPTIDE device tags (from current logcat buffer):" -ForegroundColor Cyan
$tags = adb logcat -d -s Unity 2>$null | Select-String "ZIPTIDE:|LOCO_STATE" | Select-Object -Last 30
if ($tags) { $tags } else { Write-Host "  (none in buffer — launch the app, then re-run)" }

Write-Host ""
Write-Host "REMINDER: pull + rebuild before testing so the APK matches the commit above:" -ForegroundColor Yellow
Write-Host "  git pull; powershell -ExecutionPolicy Bypass -File $Repo\tools\dev_build_install.ps1"

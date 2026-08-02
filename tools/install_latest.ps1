# install_latest.ps1 - one permanent installer. Never regenerate install instructions again.
#
# USAGE (from anywhere):
#   .\tools\install_latest.ps1                      # finds the newest Ziptide.apk in the usual places
#   .\tools\install_latest.ps1 -Path C:\some\Ziptide.apk
#   .\tools\install_latest.ps1 -Serial 1WMHH1234    # when two headsets are plugged in
#   .\tools\install_latest.ps1 -Both                # install to EVERY connected headset
#
# It always: finds the APK, prints its SHA-256, UNINSTALLS FIRST (the signature law),
# clears logcat, installs, and prints the logging command to paste next.

param(
    [string]$Path,
    [string]$Serial,
    [switch]$Both
)

$ErrorActionPreference = 'Stop'
$pkg = 'com.terrymaloney.ziptide'

# -- 1. Locate the APK --------------------------------------------------------
if (-not $Path) {
    $searchRoots = @(
        "$HOME\Downloads", "$HOME\Desktop",
        "C:\Ziptide\QuestBuilds", "C:\Ziptide\Builds", "C:\Ziptide"
    ) | Where-Object { Test-Path $_ }

    $found = Get-ChildItem $searchRoots -Recurse -Filter *.apk -ErrorAction SilentlyContinue |
             Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $found) { Write-Host "No .apk found. Pass one with -Path." -ForegroundColor Red; exit 1 }
    $Path = $found.FullName
}
if (-not (Test-Path $Path)) { Write-Host "Not found: $Path" -ForegroundColor Red; exit 1 }

$apk  = (Resolve-Path $Path).Path
$info = Get-Item $apk
Write-Host ""
Write-Host "APK    : $apk"
Write-Host "Built  : $($info.LastWriteTime)   Size: $([math]::Round($info.Length/1MB,1)) MB"
Write-Host "SHA-256: $((Get-FileHash $apk -Algorithm SHA256).Hash)" -ForegroundColor Cyan
Write-Host "         ^ record this in the verdict block" -ForegroundColor DarkGray

# -- 2. Find the headset(s) ---------------------------------------------------
adb start-server | Out-Null
# Force an array even when exactly one device is present. Without @(...), PowerShell stores a
# single serial as a scalar string and $targets[0] becomes only its first character.
$devices = @((adb devices) | Select-String "`tdevice$" | ForEach-Object { ($_ -split "`t")[0] })

if ($devices.Count -eq 0) {
    Write-Host "No authorized Quest found. Plug in, then accept 'Allow USB debugging' in the headset." -ForegroundColor Red
    adb devices
    exit 1
}

$targets = @()
if     ($Serial) { $targets = @($Serial) }
elseif ($Both)   { $targets = @($devices) }
elseif ($devices.Count -eq 1) { $targets = @($devices[0]) }
else {
    Write-Host "`nMore than one headset connected:" -ForegroundColor Yellow
    $devices | ForEach-Object { Write-Host "  $_" }
    Write-Host "Re-run with -Serial <one of the above>, or -Both to do all of them." -ForegroundColor Yellow
    exit 1
}

# -- 3. Uninstall-first, then install (see HANDOFF rb109) ---------------------
# Local builds are signed with the PC's debug key, CI artifacts with the runner's.
# Crossing that boundary always throws INSTALL_FAILED_UPDATE_INCOMPATIBLE, so the
# uninstall is unconditional and its failure is expected and ignored.
foreach ($t in $targets) {
    Write-Host "`n--- $t ---" -ForegroundColor Green
    Write-Host "uninstalling any previous build (an error here is normal)..." -ForegroundColor DarkGray
    adb -s $t uninstall $pkg 2>&1 | Out-Null
    adb -s $t logcat -c 2>&1 | Out-Null
    Write-Host "installing..."
    adb -s $t install $apk
}

# -- 4. Tell the human exactly what to do next --------------------------------
$first = @($targets)[0]
Write-Host ""
Write-Host "DONE. Launch Ziptide from the Quest library." -ForegroundColor Green
Write-Host ""
Write-Host "Before you put the headset on, paste this in a SECOND PowerShell window:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  cd C:\Ziptide" -ForegroundColor White
Write-Host "  `$stamp = Get-Date -Format 'yyyyMMdd_HHmmss'" -ForegroundColor White
Write-Host "  adb -s $first logcat -v threadtime -s Unity Ziptide | Tee-Object -FilePath `".\Ziptide\Builds\quest_`${stamp}.log`"" -ForegroundColor White
Write-Host ""
Write-Host "Afterwards, Ctrl+C that window and run:" -ForegroundColor Yellow
Write-Host "  Select-String -Path .\Ziptide\Builds\quest_*.log -Pattern `"HOME_HUB_ANCHOR|BOOT_HOLD|NO_RAY_INTERACTORS|TRAVEL_OK|TRAVEL_FAIL|Exception`"" -ForegroundColor White
Write-Host ""

<#
.SYNOPSIS
    STEP 2 of 4. Bake, build, install and launch. Unattended, about 20 minutes.

.DESCRIPTION
    Hands over to level1_test.ps1, which already owns the whole path: the Toxic City contract bake,
    BuildAndroid.PatchScenesThenAPK (which runs the W000 surface author, the layout library, the
    world spec compiler and the scene patchers as required hooks), the uninstall-first install, the
    launch, and a captured logcat.

    Close the Unity editor first. Nothing here needs it open, and an open editor holds the project
    lock that batch mode needs.

    USE -ApkPath INSTEAD if CI already built this commit and you just want it on the headset. That
    APK is baked too, so it is equally valid - it is only quicker.

.PARAMETER ApkPath
    Skip the local build and install this APK (for example one downloaded from the CI run).

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run2_build_install.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run2_build_install.ps1 -ApkPath "$HOME\Downloads\Ziptide.apk"
#>
param(
    [string]$Repo = "C:\Ziptide",
    [string]$ApkPath
)

$ErrorActionPreference = 'Stop'
Push-Location $Repo
try {
    Write-Host "===== 2/4  BUILD + INSTALL =====" -ForegroundColor Cyan

    $devices = (adb devices) -split "`n" | Where-Object { $_ -match "\tdevice$" }
    if (-not $devices) {
        Write-Host "No headset visible to adb. Plug it in, put it on, and accept the USB prompt." -ForegroundColor Red
        return
    }
    Write-Host ("  headset: " + ($devices -join ', ')) -ForegroundColor DarkGray

    if ($ApkPath) {
        if (-not (Test-Path $ApkPath)) {
            Write-Host ("Not found: " + $ApkPath) -ForegroundColor Red
            return
        }
        Write-Host "  installing a prebuilt APK (no local bake)" -ForegroundColor Yellow
        & "$Repo\tools\install_latest.ps1" -Path $ApkPath
        Write-Host ""
        Write-Host "  NEXT: step 3 starts the log capture in a SECOND window." -ForegroundColor Yellow
        Write-Host "    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run3_log.ps1" -ForegroundColor White
        return
    }

    Write-Host "  full bake + build + install + launch + log (about 20 minutes)" -ForegroundColor Yellow
    Write-Host "  level1_test.ps1 captures its own logcat, so step 3 is optional after this." -ForegroundColor DarkGray
    & "$Repo\tools\level1_test.ps1"
}
finally { Pop-Location }

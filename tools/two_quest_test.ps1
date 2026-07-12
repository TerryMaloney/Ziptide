<#
.SYNOPSIS
    Build ZIPTIDE once, install the same APK on every authorized Quest, launch it, and collect Photon smoke logs.
.DESCRIPTION
    This is the canonical at-home two-headset path. It verifies the committed PUN2/App ID/define/region setup,
    calls dev_build_install.ps1 in BuildOnly mode, installs to each device returned by adb devices, launches both,
    then captures one Unity log per headset after Terry performs the in-VR GO ONLINE test.

    Use -SkipBuild -InstallOnly to install the already-built APK while swapping one USB cable between headsets.
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\two_quest_test.ps1 -SkipBuild -InstallOnly
#>
[CmdletBinding()]
param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe",
    [string]$ProjectRoot = "",
    [string]$ApkPath = "",
    [switch]$SkipBuild,
    [switch]$InstallOnly,
    [switch]$NoLaunch,
    [int]$StartupWaitSeconds = 8
)

$ErrorActionPreference = "Stop"

function Resolve-ZiptideProjectRoot {
    param([string]$RequestedRoot)

    if (-not [string]::IsNullOrWhiteSpace($RequestedRoot)) {
        return $RequestedRoot
    }

    $defaultRoot = "C:\Ziptide\Ziptide"
    if (Test-Path (Join-Path $defaultRoot "ProjectSettings\ProjectSettings.asset")) {
        return $defaultRoot
    }

    $projectSettings = Get-ChildItem "C:\Ziptide" -Directory -Recurse -Filter "ProjectSettings" -ErrorAction SilentlyContinue |
        Where-Object { Test-Path (Join-Path $_.FullName "ProjectSettings.asset") } |
        Select-Object -First 1
    if ($projectSettings) {
        return (Split-Path $projectSettings.FullName -Parent)
    }

    throw "Could not locate the Unity project. Pass -ProjectRoot explicitly."
}

function Get-AuthorizedAdbDevices {
    $devices = @()
    $adbOutput = & adb devices 2>&1
    foreach ($line in $adbOutput) {
        if ($line -match '^\s*(\S+)\s+device\s*$') {
            $devices += $matches[1]
        }
        elseif ($line -match '^\s*(\S+)\s+unauthorized\s*$') {
            Write-Warning "Quest $($matches[1]) is unauthorized. Put it on and approve USB debugging."
        }
        elseif ($line -match '^\s*(\S+)\s+offline\s*$') {
            Write-Warning "Quest $($matches[1]) is offline. Reconnect USB and restart ADB."
        }
    }
    return ,$devices
}

function Assert-PhotonReady {
    param([string]$Root)

    $photonSettings = Join-Path $Root "Assets\Photon\PhotonUnityNetworking\Resources\PhotonServerSettings.asset"
    $projectSettings = Join-Path $Root "ProjectSettings\ProjectSettings.asset"
    $transport = Join-Path $Root "Assets\ZiptideNet\PhotonPvpTransport.cs"
    $bootstrap = Join-Path $Root "Assets\ZiptideNet\NetBootstrap.cs"

    foreach ($required in @($photonSettings, $projectSettings, $transport, $bootstrap)) {
        if (-not (Test-Path $required)) {
            throw "Photon preflight failed. Missing: $required"
        }
    }

    $photonText = Get-Content $photonSettings -Raw
    $appId = [regex]::Match($photonText, '(?m)^\s*AppIdRealtime:\s*(\S+)\s*$')
    if (-not $appId.Success -or [string]::IsNullOrWhiteSpace($appId.Groups[1].Value)) {
        throw "Photon preflight failed: AppIdRealtime is empty in PhotonServerSettings.asset."
    }

    $region = [regex]::Match($photonText, '(?m)^\s*(?:FixedRegion|DevRegion):\s*(\S+)\s*$')
    if (-not $region.Success) {
        throw "Photon preflight failed: no deterministic FixedRegion/DevRegion is configured."
    }

    $projectText = Get-Content $projectSettings -Raw
    $androidDefines = [regex]::Match($projectText, '(?m)^\s*Android:\s*(.*ZIPTIDE_PHOTON.*)$')
    if (-not $androidDefines.Success) {
        throw "Photon preflight failed: ZIPTIDE_PHOTON is not enabled for Android."
    }

    Write-Host "Photon preflight: PUN2 present; App ID present; Android define enabled; region=$($region.Groups[1].Value)." -ForegroundColor Green
}

$ProjectRoot = Resolve-ZiptideProjectRoot $ProjectRoot
$repoRoot = Split-Path $ProjectRoot -Parent
$buildsRoot = Join-Path $ProjectRoot "Builds"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$packageName = "com.terrymaloney.ziptide"
$activityName = "com.unity3d.player.UnityPlayerActivity"

Write-Host "ProjectRoot: $ProjectRoot"
Write-Host "RepoRoot: $repoRoot"

if (-not (Get-Command adb -ErrorAction SilentlyContinue)) {
    throw "adb was not found on PATH. Install Meta Quest Developer Hub/Android platform-tools or add adb to PATH."
}

if (Get-Command git -ErrorAction SilentlyContinue) {
    $branch = ((& git -C $repoRoot branch --show-current 2>$null) | Out-String).Trim()
    $head = ((& git -C $repoRoot rev-parse --short HEAD 2>$null) | Out-String).Trim()
    Write-Host "Git: branch=$branch head=$head"
    if ($branch -and $branch -ne "terry-local-wip") {
        throw "Wrong branch: $branch. Checkout and pull terry-local-wip before building."
    }
    $dirty = ((& git -C $repoRoot status --porcelain 2>$null) | Out-String).Trim()
    if ($dirty) {
        Write-Warning "The working tree has local changes. The build will include them. Review git status before pushing."
    }
}

Assert-PhotonReady $ProjectRoot

if (-not $SkipBuild) {
    $builder = Join-Path $scriptDir "dev_build_install.ps1"
    if (-not (Test-Path $builder)) {
        throw "Missing build script: $builder"
    }
    Write-Host "Building one APK for all connected Quests..." -ForegroundColor Cyan
    & $builder -UnityExe $UnityExe -ProjectRoot $ProjectRoot -BuildOnly
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

if ([string]::IsNullOrWhiteSpace($ApkPath)) {
    $ApkPath = Join-Path $ProjectRoot "Builds\Android\Ziptide.apk"
}
if (-not (Test-Path $ApkPath)) {
    throw "APK not found: $ApkPath"
}

& adb start-server | Out-Null
Start-Sleep -Seconds 2
$devices = @(Get-AuthorizedAdbDevices)
if ($devices.Count -eq 0) {
    Write-Warning "No authorized Quest is connected. APK is ready at: $ApkPath"
    Write-Host "Connect a headset, approve USB debugging, then rerun with -SkipBuild -InstallOnly."
    exit 2
}

Write-Host "Authorized Quest devices: $($devices -join ', ')"
foreach ($serial in $devices) {
    Write-Host "Installing ZIPTIDE on $serial..." -ForegroundColor Cyan
    & adb -s $serial install -r $ApkPath
    if ($LASTEXITCODE -ne 0) {
        throw "APK install failed on $serial."
    }
    Write-Host "Installed on $serial." -ForegroundColor Green
}

if ($InstallOnly -or $NoLaunch) {
    Write-Host "Install complete. APK: $ApkPath" -ForegroundColor Green
    if ($devices.Count -lt 2) {
        Write-Host "Swap the USB cable to the other Quest and rerun with -SkipBuild -InstallOnly."
    }
    exit 0
}

foreach ($serial in $devices) {
    & adb -s $serial logcat -c 2>&1 | Out-Null
    & adb -s $serial shell am force-stop $packageName 2>&1 | Out-Null
    & adb -s $serial shell am start -n "$packageName/$activityName" 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Could not launch ZIPTIDE automatically on $serial. Launch it from Unknown Sources."
    }
}

Start-Sleep -Seconds ([Math]::Max(1, $StartupWaitSeconds))
Write-Host ""
Write-Host "IN-HEADSET PHOTON TEST" -ForegroundColor Yellow
Write-Host "1. On both headsets, enter the SAME arena."
Write-Host "2. On both Match Boards, press GO ONLINE."
Write-Host "3. Each board should reach: NET: in ZIP-001 (2/2)."
Write-Host "4. Move head and hands; each player should see the other's amber helmet and gloves."
Write-Host "5. This build tests online PRESENCE only. PvP fire/hit authority is the later A6.2 sprint."
if ($devices.Count -lt 2) {
    Write-Warning "Only one Quest is connected to ADB, so this script cannot collect both logs. The visual two-headset test still works after both APKs are installed."
}

Read-Host "Press ENTER after the GO ONLINE test to capture logs" | Out-Null
$null = New-Item -ItemType Directory -Force -Path $buildsRoot

$anyFatal = $false
$allComplete = $devices.Count -ge 2
foreach ($serial in $devices) {
    $safeSerial = $serial -replace '[^A-Za-z0-9_.-]', '_'
    $logFile = Join-Path $buildsRoot "quest_${safeSerial}_photon.log"
    $raw = & adb -s $serial logcat -d -s Unity 2>&1
    $raw | Set-Content -Path $logFile -Encoding utf8
    $text = $raw -join [Environment]::NewLine

    Write-Host ""
    Write-Host "--- $serial Photon summary ---" -ForegroundColor Cyan
    $netLines = $raw | Select-String -Pattern 'ZIPTIDE: (NET_|LOBBY_ONLINE)' | ForEach-Object { $_.Line }
    if ($netLines) { $netLines | Select-Object -Last 40 | ForEach-Object { Write-Host $_ } }
    else { Write-Warning "No ZIPTIDE network tags found on $serial. Log: $logFile" }

    foreach ($requiredTag in @('NET_STARTER_INSTALLED', 'LOBBY_ONLINE_START', 'NET_ROOM_JOINED', 'NET_PRESENCE')) {
        if ($text -notmatch [regex]::Escape($requiredTag)) {
            Write-Warning "$serial missing $requiredTag"
            $allComplete = $false
        }
    }

    if ($text -match 'AndroidRuntime.*FATAL|NullReferenceException|ZIPTIDE: NET_DISCONNECTED') {
        Write-Warning "$serial contains a fatal/runtime disconnect marker. Inspect $logFile"
        $anyFatal = $true
    }
    Write-Host "Saved: $logFile"
}

if ($anyFatal) {
    Write-Host "TWO-QUEST SMOKE: FAILED — runtime/disconnect evidence found." -ForegroundColor Red
    exit 1
}
if (-not $allComplete) {
    Write-Host "TWO-QUEST SMOKE: INCOMPLETE — install succeeded, but the full 2/2 presence evidence was not captured." -ForegroundColor Yellow
    exit 2
}

Write-Host "TWO-QUEST SMOKE: PASSED — both clients joined and exchanged presence." -ForegroundColor Green
exit 0

param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe",
    [string]$ProjectRoot = "",
    [switch]$Logcat,
    [switch]$BuildOnly
)

if ($ProjectRoot -eq "") {
    $ps = Get-ChildItem C:\Ziptide -Directory -Recurse -Filter ProjectSettings -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($ps) {
        $ProjectRoot = Split-Path $ps.FullName -Parent
    } else {
        $ProjectRoot = "C:\Ziptide\Ziptide"
    }
}

Write-Host "ProjectRoot: $ProjectRoot"
Write-Host "UnityExe: $UnityExe"

# --- Preflight: ensure no Unity instance holds the project so batch mode can open it ---
$libraryEditorInstance = Join-Path $ProjectRoot "Library\EditorInstance.json"
if (Test-Path $libraryEditorInstance) {
    try {
        $instance = Get-Content $libraryEditorInstance -Raw | ConvertFrom-Json
        $pidToStop = $instance.process_id
        if ($pidToStop) {
            $proc = Get-Process -Id $pidToStop -ErrorAction SilentlyContinue
            if ($proc) {
                Write-Host "Preflight: stopping Unity process (PID $pidToStop) that had this project open."
                Stop-Process -Id $pidToStop -Force -ErrorAction SilentlyContinue
                Start-Sleep -Seconds 1
            }
        }
    } catch { }
    Remove-Item $libraryEditorInstance -Force -ErrorAction SilentlyContinue
    Write-Host "Preflight: removed project lock file (EditorInstance.json)."
}
$unityProcs = Get-Process -Name "Unity" -ErrorAction SilentlyContinue
if ($unityProcs) {
    foreach ($p in $unityProcs) {
        Write-Host "Preflight: stopping lingering Unity Editor (PID $($p.Id))."
        Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 4
}
$lockRetry = Join-Path $ProjectRoot "Library\EditorInstance.json"
if (Test-Path $lockRetry) {
    Remove-Item $lockRetry -Force -ErrorAction SilentlyContinue
    Write-Host "Preflight: removed lingering lock file after process cleanup."
}

adb devices

$buildLogDir = Join-Path $ProjectRoot "Builds"
$null = New-Item -ItemType Directory -Force -Path $buildLogDir
$logFile = Join-Path $buildLogDir "android_build.log"

Write-Host "Starting Unity batch build (this takes 1-5 minutes)..."
$unityArgs = "-batchmode -nographics -quit -projectPath `"$ProjectRoot`" -executeMethod Ziptide.Build.BuildAndroid.PatchScenesThenAPK -logFile `"$logFile`""
$proc = Start-Process -FilePath $UnityExe -ArgumentList $unityArgs -Wait -PassThru -NoNewWindow
$unityExit = $proc.ExitCode
Write-Host "Unity exited with code: $unityExit"
if ($unityExit -ne 0) {
    Write-Error "Unity build failed (exit $unityExit). Check log: $logFile"
    exit $unityExit
}

$apk = Join-Path $ProjectRoot "Builds\Android\Ziptide.apk"
if (-not (Test-Path $apk)) {
    Write-Error "APK was not created. Close the Unity Editor completely and run this script again. Check build log: $logFile"
    exit 1
}

if ($BuildOnly) {
    Write-Host "BuildOnly: APK built successfully. Skipping install and logcat. Output: $apk"
    exit 0
}

# Unity batchmode kills the ADB server on shutdown; restart it and wait for device reconnection
Write-Host "Restarting ADB server (Unity may have killed it)..."
adb start-server | Out-Null
Start-Sleep -Seconds 3
Write-Host "Checking for device..."
$devicesOut = adb devices 2>&1 | Out-String
if ($devicesOut -notmatch "(?m)^\S+\s+device\s*$") {
    Write-Host "No device connected. Build OK. APK: $apk" -ForegroundColor Yellow
    Write-Host "To install + logcat: connect Quest (USB + USB debugging), then run this script again."
    exit 0
}

adb install -r $apk
if ($LASTEXITCODE -ne 0) {
    Write-Error "APK install failed (exit code $LASTEXITCODE). Ensure device is connected and USB debugging is enabled."
    exit 1
}
Write-Host "Installed: $apk"

if ($Logcat) {
    $pkg = "com.terrymaloney.ziptide"
    $activity = "com.unity3d.player.UnityPlayerActivity"
    Write-Host "Logcat: clearing buffer, launching app, waiting 5s..."
    adb logcat -c 2>&1 | Out-Null
    adb shell am start -n "${pkg}/${activity}" 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Failed to launch app via adb; capture logcat manually after launching on device."
    }
    Start-Sleep -Seconds 5
    $logcatFile = Join-Path $buildLogDir "quest_logcat.log"
    $logcatContent = adb logcat -d -s Unity -s Ziptide 2>&1
    if ($logcatContent) { $logcatContent | Set-Content -Path $logcatFile -Encoding utf8 }
    else { "" | Set-Content -Path $logcatFile -Encoding utf8 }
    Write-Host "Logcat saved: $logcatFile"
    if (Test-Path $logcatFile) { Get-Content $logcatFile }
}

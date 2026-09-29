param(
    [string]$UnityExe = "",
    [string]$ProjectRoot = "",
    [ValidateSet("GoldenSlice", "FullDevelopment")]
    [string]$BuildProfile = "GoldenSlice",
    [string]$AdbExe = "",
    [string]$Serial = "",
    [switch]$Logcat,
    [switch]$BuildOnly,
    [switch]$PreflightOnly
)
. "$PSScriptRoot/quest_common.ps1"
try {
    if ($BuildOnly -and $Logcat) { throw '-BuildOnly and -Logcat are mutually exclusive.' }
    $ProjectRoot = Resolve-QuestProject $ProjectRoot
    $UnityExe = Resolve-QuestEditor $ProjectRoot $UnityExe
    Assert-QuestProjectClosed $ProjectRoot
    if (-not $BuildOnly) {
        $AdbExe = Resolve-QuestAdb $AdbExe $UnityExe
        $Serial = Select-QuestDevice $AdbExe $Serial
    }
    if ($PreflightOnly) { Write-Host 'PREFLIGHT PASS (no build or installation attempted)'; exit 0 }
    $session = New-QuestSession $ProjectRoot
    $log = Join-Path $session 'android_build.log'
    $method = if ($BuildProfile -eq 'GoldenSlice') {
        'Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK'
    } else { 'Ziptide.Build.BuildAndroid.PatchScenesThenAPK' }
    $manifest = [ordered]@{ schemaVersion=1; startedUtc=[DateTime]::UtcNow.ToString('o'); sourceBefore= (Get-QuestSource $ProjectRoot); profile=$BuildProfile; unity=$UnityExe; method=$method; stage='preflight'; device=$Serial }
    $manifestPath = Join-Path $session 'manifest.json'
    $manifest | ConvertTo-Json -Depth 8 | Set-Content $manifestPath -Encoding UTF8
    & git -C $ProjectRoot diff --binary HEAD | Set-Content (Join-Path $session 'source-before.patch') -Encoding UTF8
    if ($LASTEXITCODE -ne 0) { throw 'Cannot capture source diff.' }
    Write-Host "Evidence: $session"
    $apk = Join-Path $ProjectRoot 'Builds/Android/Ziptide.apk'
    # Preserve the previous output but ensure it cannot satisfy this build.
    if (Test-Path -LiteralPath $apk) { Move-Item -LiteralPath $apk -Destination (Join-Path $session 'previous.apk') }
    $argsLine = "-batchmode -nographics -quit -projectPath `"$ProjectRoot`" -executeMethod $method -logFile `"$log`""
    $process = Start-Process -FilePath $UnityExe -ArgumentList $argsLine -Wait -PassThru -NoNewWindow
    if ($process.ExitCode -ne 0) { throw "Unity failed ($($process.ExitCode)). See $log" }
    if (-not (Test-Path $apk) -or (Get-Item $apk).Length -eq 0) { throw 'Unity did not produce a fresh, nonempty APK.' }
    if (-not (Test-Path $log)) { throw 'Unity build log is missing.' }
    $buildText = Get-Content $log -Raw
    if ($buildText -match 'ZIPTIDE: AUDIT_FAIL|World audit FAILED|ZIPTIDE: BUILD_HOOK_FAIL') { throw 'Build/audit failure recorded in log.' }
    $profileProof = "ZIPTIDE: BUILD_PROFILE profile=$BuildProfile"
    if ($buildText -notmatch [regex]::Escape($profileProof)) { throw "Build profile proof missing: $profileProof" }
    $savedApk = Join-Path $session ("Ziptide-$BuildProfile.apk")
    Copy-Item -LiteralPath $apk -Destination $savedApk
    $manifest['apk'] = Split-Path $savedApk -Leaf
    $manifest['apkSha256'] = (Get-FileHash -LiteralPath $savedApk -Algorithm SHA256).Hash
    $manifest['sourceAfterGeneration'] = Get-QuestSource $ProjectRoot
    $manifest['stage'] = 'built'
    $manifest | ConvertTo-Json -Depth 8 | Set-Content $manifestPath -Encoding UTF8
    if ($BuildOnly) { Write-Host "BUILT (not installed): $savedApk"; exit 0 }
    Invoke-QuestAdb $AdbExe @('start-server') | Out-Null
    $Serial = Select-QuestDevice $AdbExe $Serial
    $install = Invoke-QuestAdb $AdbExe @('-s',$Serial,'install','-r',$savedApk)
    if ($install -notmatch '(?m)^Success\s*$') { throw "Install did not confirm Success: $install" }
    $manifest['stage'] = 'installed'
    $manifest | ConvertTo-Json -Depth 8 | Set-Content $manifestPath -Encoding UTF8
    if ($Logcat) {
        Start-QuestApp $AdbExe $Serial
        $manifest['stage'] = 'launched'
        $manifest | ConvertTo-Json -Depth 8 | Set-Content $manifestPath -Encoding UTF8
        Start-Sleep -Seconds 15
        $logcatFile = Join-Path $session 'quest_logcat.log'
        Save-QuestLog $AdbExe $Serial $logcatFile
        Assert-QuestLog $logcatFile $BuildProfile
        $manifest['stage'] = 'boot-smoke-passed'
        $manifest | ConvertTo-Json -Depth 8 | Set-Content $manifestPath -Encoding UTF8
    }
    Write-Host "SUCCESS stage=$($manifest.stage) profile=$BuildProfile evidence=$session (gameplay not certified)"
    exit 0
} catch {
    # The last successful stage remains in the manifest; failed attempts cannot inherit old evidence.
    Write-Error $_ -ErrorAction Continue
    exit 1
}

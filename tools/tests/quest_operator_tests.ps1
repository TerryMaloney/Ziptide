# Dependency-free behavioral regression tests. Run with Windows PowerShell 5.1 or pwsh.
. "$PSScriptRoot/../quest_common.ps1"
$count = 0
function Check([string]$Name, [scriptblock]$Action) {
    & $Action
    $script:count++
    Write-Host "PASS $Name"
}
function Must-Fail([scriptblock]$Action) {
    $failed = $false
    try { & $Action | Out-Null } catch { $failed = $true }
    if (-not $failed) { throw 'Expected failure, got success.' }
}
$temp = Join-Path ([IO.Path]::GetTempPath()) ('quest-test-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $temp 'ProjectSettings') -Force | Out-Null
Set-Content (Join-Path $temp 'ProjectSettings/ProjectVersion.txt') 'm_EditorVersion: 6000.2.9f1'
try {
    Check 'explicit project is resolved' { if ((Resolve-QuestProject $temp) -ne $temp) { throw 'Wrong root' } }
    Check 'invalid project rejected' { Must-Fail { Resolve-QuestProject (Join-Path $temp 'ProjectSettings') } }
    Check 'version read from project' { if ((Get-QuestEditorVersion $temp) -ne '6000.2.9f1') { throw 'Wrong version' } }
    Check 'missing editor rejected before build' { Must-Fail { Resolve-QuestEditor $temp (Join-Path $temp 'absent.exe') } }
    Check 'closed project accepted' { Assert-QuestProjectClosed $temp }
    New-Item -ItemType Directory -Path (Join-Path $temp 'Temp') | Out-Null
    Set-Content (Join-Path $temp 'Temp/UnityLockfile') 'keep'
    Check 'open/stale lock blocks without deleting' {
        Must-Fail { Assert-QuestProjectClosed $temp }
        if ((Get-Content (Join-Path $temp 'Temp/UnityLockfile')) -ne 'keep') { throw 'Lock changed' }
    }
    $first = New-QuestSession $temp; $second = New-QuestSession $temp
    Check 'sessions cannot reuse previous output' { if ($first -eq $second) { throw 'Session collision' } }
    $log = Join-Path $temp 'device.log'
    Check 'missing log fails' { Must-Fail { Assert-QuestLog $log 'FullDevelopment' } }
    Set-Content $log ''
    Check 'empty log fails' { Must-Fail { Assert-QuestLog $log 'FullDevelopment' } }
    Set-Content $log 'Some Android output'
    Check 'unrelated log fails' { Must-Fail { Assert-QuestLog $log 'FullDevelopment' } }
    $proof = 'ZIPTIDE: RECOVERY_EXPOSURE buildProfile=FullDevelopment profile=FullDevelopment features=TravelCoordinator'
    Set-Content $log $proof
    Check 'full profile accepted' { Assert-QuestLog $log 'FullDevelopment' }
    Check 'wrong profile rejected' { Must-Fail { Assert-QuestLog $log 'GoldenSlice' } }
    foreach ($bad in @('NullReferenceException', 'FATAL EXCEPTION: main', 'Fatal signal 11', 'ZIPTIDE: TRAVEL_FAIL')) {
        Set-Content $log "$proof`n$bad"
        Check "reject $bad" { Must-Fail { Assert-QuestLog $log 'FullDevelopment' } }
    }
    Set-Content $log 'ZIPTIDE: RECOVERY_EXPOSURE buildProfile=GoldenSlice profile=GoldenSlice features=PvpProgression'
    Check 'golden excluded feature rejected' { Must-Fail { Assert-QuestLog $log 'GoldenSlice' } }
    # Mock only the external ADB boundary; execute selection and launch logic unchanged.
    function Invoke-QuestAdb([string]$AdbExe, [string[]]$AdbArguments) {
        $script:calls += ,$AdbArguments
        return $script:response
    }
    $script:calls = @()
    $script:response = "List of devices attached`nquest1`tdevice`n"
    Check 'single device selected' { if ((Select-QuestDevice 'adb' '') -ne 'quest1') { throw 'Wrong device' } }
    $script:response = "List of devices attached`nquest1`tunauthorized`n"
    Check 'unauthorized device rejected' { Must-Fail { Select-QuestDevice 'adb' '' } }
    $script:response = 'List of devices attached'
    Check 'no device rejected' { Must-Fail { Select-QuestDevice 'adb' '' } }
    $script:response = "List of devices attached`nquest1`tdevice`nquest2`tdevice`n"
    Check 'ambiguous devices rejected' { Must-Fail { Select-QuestDevice 'adb' '' } }
    Check 'explicit serial honored' { if ((Select-QuestDevice 'adb' 'quest2') -ne 'quest2') { throw 'Wrong serial' } }
    Check 'unknown serial rejected' { Must-Fail { Select-QuestDevice 'adb' 'quest3' } }
    $script:response = 'No activities found to run, monkey aborted.'
    Check 'failed launch rejected even with zero adb exit' { Must-Fail { Start-QuestApp 'adb' 'quest1' } }
    $script:response = 'Events injected: 1'
    $script:calls = @()
    Check 'launch explicitly stops clears and selects serial' {
        Start-QuestApp 'adb' 'quest1'
        if ($script:calls.Count -ne 3) { throw 'Expected stop, clear, launch' }
        foreach ($call in $script:calls) { if ($call[0] -ne '-s' -or $call[1] -ne 'quest1') { throw 'Unscoped command' } }
    }
    Write-Host "$count Quest operator checks passed. Unity/USB not exercised."
} finally { Remove-Item -LiteralPath $temp -Recurse -Force }

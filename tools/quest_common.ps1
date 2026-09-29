# Shared Windows PowerShell 5.1-compatible operator helpers. No editor termination or lock deletion.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$script:QuestPackage = 'com.terrymaloney.ziptide'

function Resolve-QuestProject([string]$ProjectRoot) {
    if (-not $ProjectRoot) { $ProjectRoot = Join-Path (Split-Path $PSScriptRoot -Parent) 'Ziptide' }
    $root = (Resolve-Path -LiteralPath $ProjectRoot).Path
    if (-not (Test-Path -LiteralPath (Join-Path $root 'ProjectSettings/ProjectVersion.txt'))) {
        throw "Not a Unity project: $root"
    }
    return $root
}
function Get-QuestEditorVersion([string]$ProjectRoot) {
    $line = Select-String -LiteralPath (Join-Path $ProjectRoot 'ProjectSettings/ProjectVersion.txt') -Pattern '^m_EditorVersion:\s*(\S+)'
    if (-not $line) { throw 'ProjectVersion.txt has no editor version.' }
    return $line.Matches[0].Groups[1].Value
}
function Resolve-QuestEditor([string]$ProjectRoot, [string]$UnityExe) {
    $version = Get-QuestEditorVersion $ProjectRoot
    if (-not $UnityExe) { $UnityExe = "C:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe" }
    if (-not (Test-Path -LiteralPath $UnityExe)) { throw "Install Unity $version in Hub, or pass -UnityExe. Missing: $UnityExe" }
    $exe = (Resolve-Path -LiteralPath $UnityExe).Path
    # A custom install location is allowed, but an accidental engine migration is not.
    $versionOutput = [IO.Path]::GetTempFileName()
    $versionError = [IO.Path]::GetTempFileName()
    try {
        $probe = Start-Process -FilePath $exe -ArgumentList '-version' -Wait -PassThru -NoNewWindow -RedirectStandardOutput $versionOutput -RedirectStandardError $versionError
        $actual = Get-Content -LiteralPath $versionOutput -Raw
        if ($probe.ExitCode -ne 0 -or $actual -notmatch ('(?<![0-9A-Za-z.])' + [regex]::Escape($version) + '(?![0-9A-Za-z.])')) {
            throw "Expected Unity $version; version probe returned '$actual' (exit $($probe.ExitCode)): $exe"
        }
    } finally { Remove-Item -LiteralPath $versionOutput,$versionError -ErrorAction SilentlyContinue }
    $android = Join-Path (Split-Path $exe -Parent) 'Data/PlaybackEngines/AndroidPlayer'
    foreach ($part in @('', 'SDK/platform-tools/adb.exe', 'NDK/source.properties', 'OpenJDK/bin/java.exe')) {
        if (-not (Test-Path -LiteralPath (Join-Path $android $part))) {
            throw "Unity $version needs Android Build Support, Android SDK & NDK Tools, and OpenJDK via Hub > Add modules. Missing: $part"
        }
    }
    return $exe
}
function Assert-QuestProjectClosed([string]$ProjectRoot) {
    # Conservative: a lock may be stale; let the user/editor resolve it, never remove it blindly.
    foreach ($relative in @('Temp/UnityLockfile', 'Library/EditorInstance.json')) {
        if (Test-Path -LiteralPath (Join-Path $ProjectRoot $relative)) {
            throw "Save and close this Unity project before batch building ($relative exists). If stale, open and close it normally. No process was killed."
        }
    }
}
function Resolve-QuestAdb([string]$AdbExe, [string]$UnityExe = '') {
    if ($AdbExe) { return (Get-Command $AdbExe -ErrorAction Stop).Source }
    if ($UnityExe) {
        $bundled = Join-Path (Split-Path $UnityExe -Parent) 'Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe'
        if (Test-Path -LiteralPath $bundled) { return $bundled }
    }
    $cmd = Get-Command adb -ErrorAction SilentlyContinue
    if (-not $cmd) { throw 'ADB not found. Add platform-tools to PATH or pass -AdbExe.' }
    return $cmd.Source
}
function Invoke-QuestAdb([string]$AdbExe, [string[]]$AdbArguments) {
    # Capture stderr without Windows PowerShell turning native stderr into a terminating error.
    $old = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try { $output = & $AdbExe @AdbArguments 2>&1; $code = $LASTEXITCODE }
    finally { $ErrorActionPreference = $old }
    $value = ($output | Out-String).Trim()
    if ($code -ne 0) { throw "ADB failed ($code): $($AdbArguments -join ' ')`n$value" }
    return $value
}
function Select-QuestDevice([string]$AdbExe, [string]$Serial) {
    $list = Invoke-QuestAdb $AdbExe @('devices')
    $ready = @($list -split '\r?\n' | ForEach-Object { if ($_ -match '^(\S+)\s+device\s*$') { $Matches[1] } })
    if ($Serial) {
        if ($ready -notcontains $Serial) { throw "Device '$Serial' is missing/offline/unauthorized. Accept USB debugging in the headset.`n$list" }
        return $Serial
    }
    if ($ready.Count -ne 1) { throw "Expected exactly one authorized device; found $($ready.Count). Use -Serial when several are connected.`n$list" }
    return $ready[0]
}
function New-QuestSession([string]$ProjectRoot) {
    $folder = Join-Path $ProjectRoot ('Builds/sessions/' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + '-' + [guid]::NewGuid().ToString('N').Substring(0,6))
    return (New-Item -ItemType Directory -Path $folder -Force).FullName
}
function Get-QuestSource([string]$ProjectRoot) {
    $sha = & git -C $ProjectRoot rev-parse HEAD
    if ($LASTEXITCODE -ne 0) { throw 'Cannot determine source commit.' }
    $status = @(& git -C $ProjectRoot status --porcelain)
    if ($LASTEXITCODE -ne 0) { throw 'Cannot determine working-tree state.' }
    return @{ commit = "$sha"; dirty = ($status.Count -gt 0); changes = $status }
}
function Start-QuestApp([string]$AdbExe, [string]$Serial) {
    Invoke-QuestAdb $AdbExe @('-s',$Serial,'shell','am','force-stop',$script:QuestPackage) | Out-Null
    Invoke-QuestAdb $AdbExe @('-s',$Serial,'logcat','-c') | Out-Null
    $launch = Invoke-QuestAdb $AdbExe @('-s',$Serial,'shell','monkey','-p',$script:QuestPackage,'-c','android.intent.category.LAUNCHER','1')
    if ($launch -notmatch 'Events injected:\s*1' -or $launch -match 'aborted|Error:|No activities') { throw "Launch failed: $launch" }
}
function Assert-QuestLog([string]$LogFile, [string]$BuildProfile) {
    if (-not (Test-Path -LiteralPath $LogFile)) { throw 'Fresh device log is missing.' }
    $content = Get-Content -LiteralPath $LogFile -Raw
    if ([string]::IsNullOrWhiteSpace($content)) { throw 'Fresh device log is empty.' }
    $proof = "ZIPTIDE: RECOVERY_EXPOSURE buildProfile=$BuildProfile profile=$BuildProfile"
    if ($content -notmatch [regex]::Escape($proof)) { throw "Runtime profile proof missing: $proof" }
    if ($content -match 'Exception|FATAL EXCEPTION|Fatal signal|ZIPTIDE: (XRI_MISSING|NO_RAY_INTERACTORS|INPUT_ACTIONS_MISSING|DUP_SINGLETON|INVENTORY_RESTORE_FAIL|ITEM_DEF_NOT_FOUND|TRAVEL_FAIL|XRI_NOT_READY)') {
        throw "Runtime errors found. Inspect $LogFile"
    }
    if ($BuildProfile -eq 'GoldenSlice' -and $content -match 'ZIPTIDE: RECOVERY_EXPOSURE .*?(ConquestMissionInjector|PvpProgression)') { throw 'GoldenSlice exposed excluded features.' }
}
function Save-QuestLog([string]$AdbExe, [string]$Serial, [string]$LogFile) {
    # Keep crash buffers and capture marks as well as Unity messages.
    Invoke-QuestAdb $AdbExe @('-s',$Serial,'logcat','-d','-v','threadtime','Unity:V','ZIPTIDE_CAPTURE:V','CRASH:V','AndroidRuntime:V','DEBUG:V','libc:F','*:S') |
        Set-Content -LiteralPath $LogFile -Encoding UTF8
}

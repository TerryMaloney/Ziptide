[CmdletBinding()]
param(
    [switch]$Json,
    [switch]$Strict,
    [string]$OutputPath = "Builds/Reports/dev_capabilities.json"
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$RepoRoot = Split-Path -Parent $PSScriptRoot
$Results = [System.Collections.Generic.List[object]]::new()

function Add-Result {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][ValidateSet('READY','PARTIAL','MISSING','INFO')][string]$Status,
        [Parameter(Mandatory = $true)][string]$Detail,
        [string]$ResolvedPath = ''
    )
    $Results.Add([pscustomobject]@{
        name = $Name
        status = $Status
        detail = $Detail.Trim()
        path = $ResolvedPath
    }) | Out-Null
}

function Resolve-CommandPath {
    param([Parameter(Mandatory = $true)][string[]]$Names)
    foreach ($name in $Names) {
        $command = Get-Command $name -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($null -ne $command) {
            return $command.Source
        }
    }
    return $null
}

function First-Line {
    param([object]$Value)
    $text = ($Value | Out-String).Trim()
    if ([string]::IsNullOrWhiteSpace($text)) { return '' }
    return ($text -split "`r?`n")[0].Trim()
}

Push-Location $RepoRoot
try {
    Add-Result 'PowerShell' 'READY' ("PowerShell {0} ({1})" -f $PSVersionTable.PSVersion, $PSVersionTable.PSEdition) $PSHOME

    if (Test-Path (Join-Path $RepoRoot 'Ziptide/ProjectSettings/ProjectVersion.txt')) {
        $projectVersion = Get-Content (Join-Path $RepoRoot 'Ziptide/ProjectSettings/ProjectVersion.txt') -Raw
        Add-Result 'ZIPTIDE checkout' 'READY' ($projectVersion.Trim()) $RepoRoot
    }
    else {
        Add-Result 'ZIPTIDE checkout' 'MISSING' 'Unity project was not found under Ziptide/.' $RepoRoot
    }

    $git = Resolve-CommandPath @('git.exe','git')
    if ($git) {
        $gitVersion = First-Line (& $git --version 2>&1)
        $branch = First-Line (& $git branch --show-current 2>&1)
        $status = (& $git status --short 2>&1 | Out-String).Trim()
        $workingTree = if ([string]::IsNullOrWhiteSpace($status)) { 'clean' } else { 'has local changes' }
        Add-Result 'Git' 'READY' ("{0}; branch={1}; working tree={2}" -f $gitVersion, $branch, $workingTree) $git
    }
    else {
        Add-Result 'Git' 'MISSING' 'git was not found on PATH.'
    }

    $gh = Resolve-CommandPath @('gh.exe','gh')
    if ($gh) {
        $ghVersion = First-Line (& $gh --version 2>&1)
        & $gh auth status *> $null
        if ($LASTEXITCODE -eq 0) {
            Add-Result 'GitHub CLI' 'READY' ("{0}; authenticated" -f $ghVersion) $gh
        }
        else {
            Add-Result 'GitHub CLI' 'PARTIAL' ("{0}; installed but not authenticated" -f $ghVersion) $gh
        }
    }
    else {
        Add-Result 'GitHub CLI' 'MISSING' 'gh was not found on PATH.'
    }

    $python = Resolve-CommandPath @('py.exe','py','python.exe','python','python3.exe','python3')
    if ($python) {
        $pythonName = [System.IO.Path]::GetFileNameWithoutExtension($python).ToLowerInvariant()
        if ($pythonName -eq 'py') {
            $pythonVersion = First-Line (& $python -3 --version 2>&1)
        }
        else {
            $pythonVersion = First-Line (& $python --version 2>&1)
        }
        Add-Result 'Python 3' 'READY' $pythonVersion $python
    }
    else {
        Add-Result 'Python 3' 'MISSING' 'Python 3 was not found as py, python, or python3.'
    }

    $unityCandidates = [System.Collections.Generic.List[string]]::new()
    if (-not [string]::IsNullOrWhiteSpace($env:UNITY_EDITOR_PATH)) {
        $unityCandidates.Add($env:UNITY_EDITOR_PATH)
    }
    $unityCandidates.Add('C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe')
    $unityFromPath = Resolve-CommandPath @('Unity.exe','Unity')
    if ($unityFromPath) { $unityCandidates.Add($unityFromPath) }
    $hubRoot = 'C:\Program Files\Unity\Hub\Editor'
    if (Test-Path $hubRoot) {
        Get-ChildItem $hubRoot -Directory -ErrorAction SilentlyContinue |
            ForEach-Object {
                $candidate = Join-Path $_.FullName 'Editor\Unity.exe'
                if (Test-Path $candidate) { $unityCandidates.Add($candidate) }
            }
    }
    $unity = $unityCandidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique -First 1
    if ($unity) {
        $versionMatch = [regex]::Match($unity, 'Editor\\([^\\]+)\\Editor\\Unity\.exe$', 'IgnoreCase')
        $versionText = if ($versionMatch.Success) { $versionMatch.Groups[1].Value } else { 'version unresolved from path' }
        $unityStatus = if ($versionText -eq '2022.3.62f3') { 'READY' } else { 'PARTIAL' }
        Add-Result 'Unity Editor' $unityStatus ("found {0}; project requires 2022.3.62f3" -f $versionText) $unity
    }
    else {
        Add-Result 'Unity Editor' 'MISSING' 'Unity.exe was not found. Set UNITY_EDITOR_PATH or install Unity 2022.3.62f3 through Unity Hub.'
    }

    $java = Resolve-CommandPath @('java.exe','java')
    if ($java) {
        $javaVersion = First-Line (& $java -version 2>&1)
        Add-Result 'Java' 'READY' $javaVersion $java
    }
    else {
        Add-Result 'Java' 'MISSING' 'java was not found on PATH. Unity Android support may still include an embedded JDK.'
    }

    $adbCandidates = [System.Collections.Generic.List[string]]::new()
    $adbFromPath = Resolve-CommandPath @('adb.exe','adb')
    if ($adbFromPath) { $adbCandidates.Add($adbFromPath) }
    if (-not [string]::IsNullOrWhiteSpace($env:ANDROID_SDK_ROOT)) {
        $adbCandidates.Add((Join-Path $env:ANDROID_SDK_ROOT 'platform-tools\adb.exe'))
    }
    if (-not [string]::IsNullOrWhiteSpace($env:ANDROID_HOME)) {
        $adbCandidates.Add((Join-Path $env:ANDROID_HOME 'platform-tools\adb.exe'))
    }
    if (-not [string]::IsNullOrWhiteSpace($env:LOCALAPPDATA)) {
        $adbCandidates.Add((Join-Path $env:LOCALAPPDATA 'Android\Sdk\platform-tools\adb.exe'))
    }
    $adb = $adbCandidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique -First 1
    if ($adb) {
        $adbVersion = First-Line (& $adb version 2>&1)
        $deviceLines = & $adb devices -l 2>&1 |
            Where-Object { $_ -match '^\S+\s+(device|unauthorized|offline)(\s|$)' }
        $authorized = @($deviceLines | Where-Object { $_ -match '^\S+\s+device(\s|$)' }).Count
        $unauthorized = @($deviceLines | Where-Object { $_ -match '^\S+\s+unauthorized(\s|$)' }).Count
        $offline = @($deviceLines | Where-Object { $_ -match '^\S+\s+offline(\s|$)' }).Count
        if ($authorized -eq 1 -and $unauthorized -eq 0 -and $offline -eq 0) {
            Add-Result 'ADB / Quest' 'READY' ("{0}; one authorized Quest/device connected" -f $adbVersion) $adb
        }
        elseif ($authorized -gt 1) {
            Add-Result 'ADB / Quest' 'PARTIAL' ("{0}; {1} authorized devices connected—select one explicitly" -f $adbVersion, $authorized) $adb
        }
        elseif ($unauthorized -gt 0) {
            Add-Result 'ADB / Quest' 'PARTIAL' ("{0}; device detected but USB debugging authorization is pending in-headset" -f $adbVersion) $adb
        }
        elseif ($offline -gt 0) {
            Add-Result 'ADB / Quest' 'PARTIAL' ("{0}; device is offline—reconnect USB and restart ADB" -f $adbVersion) $adb
        }
        else {
            Add-Result 'ADB / Quest' 'PARTIAL' ("{0}; ADB is ready but no device is connected" -f $adbVersion) $adb
        }
    }
    else {
        Add-Result 'ADB / Quest' 'MISSING' 'adb.exe was not found on PATH or in common Android SDK locations.'
    }

    $preflight = Join-Path $RepoRoot 'tools\dev_preflight.ps1'
    if (Test-Path $preflight) {
        Add-Result 'ZIPTIDE fast preflight' 'READY' 'Run .\tools\dev_preflight.ps1 before every source push.' $preflight
    }
    else {
        Add-Result 'ZIPTIDE fast preflight' 'MISSING' 'tools/dev_preflight.ps1 is missing.'
    }

    $ready = @($Results | Where-Object status -eq 'READY').Count
    $partial = @($Results | Where-Object status -eq 'PARTIAL').Count
    $missing = @($Results | Where-Object status -eq 'MISSING').Count
    $report = [ordered]@{
        schemaVersion = 1
        generatedAtUtc = [DateTime]::UtcNow.ToString('o')
        repositoryRoot = $RepoRoot
        summary = [ordered]@{
            ready = $ready
            partial = $partial
            missing = $missing
        }
        capabilities = @($Results)
    }

    $resolvedOutput = Join-Path $RepoRoot $OutputPath
    $outputDirectory = Split-Path -Parent $resolvedOutput
    if ($outputDirectory) { New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null }
    $report | ConvertTo-Json -Depth 6 | Set-Content -Encoding UTF8 $resolvedOutput

    if ($Json) {
        $report | ConvertTo-Json -Depth 6
    }
    else {
        Write-Host '== ZIPTIDE DEVELOPMENT CAPABILITY AUDIT ==' -ForegroundColor Cyan
        $Results | Format-Table -AutoSize name, status, detail
        Write-Host ("Report: {0}" -f $resolvedOutput)
        Write-Host ("READY={0} PARTIAL={1} MISSING={2}" -f $ready, $partial, $missing)
    }

    if ($Strict -and $missing -gt 0) { exit 2 }
}
finally {
    Pop-Location
}

param([string]$Repo = "", [string]$AdbExe = "", [string]$Serial = "")
. "$PSScriptRoot/quest_common.ps1"
if (-not $Repo) { $Repo = Split-Path $PSScriptRoot -Parent }
try {
    $project = Resolve-QuestProject (Join-Path $Repo 'Ziptide')
    $version = Get-QuestEditorVersion $project
    Write-Output 'ZIPTIDE READ-ONLY SNAPSHOT (no build, pull, reset, installation, or log clearing)'
    Write-Output "Expected Unity: $version"
    & git -C $Repo status --short --branch
    & git -C $Repo log -1 --format='%H %s'
    $editor = "C:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe"
    foreach ($path in @($editor, (Join-Path (Split-Path $editor -Parent) 'Data/PlaybackEngines/AndroidPlayer'), (Join-Path $project 'Temp/UnityLockfile'), (Join-Path $project 'Library/EditorInstance.json'))) {
        Write-Output "Exists=$(Test-Path -LiteralPath $path) $path"
    }
    try {
        $adb = Resolve-QuestAdb $AdbExe $editor
        Write-Output (Invoke-QuestAdb $adb @('devices','-l'))
        $selected = Select-QuestDevice $adb $Serial
        Write-Output (Invoke-QuestAdb $adb @('-s',$selected,'shell','getprop','ro.product.model'))
        Write-Output (Invoke-QuestAdb $adb @('-s',$selected,'shell','dumpsys','package',$script:QuestPackage))
    } catch { Write-Output "Device observation unavailable: $_" }
    Write-Output 'Installed source SHA is unknown unless matched to an installation manifest. Save this output and the latest Unity build log.'
} catch { Write-Error $_ -ErrorAction Continue; exit 1 }

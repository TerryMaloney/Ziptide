# Install a previously built, hash-verified session without opening Unity or rebuilding.
param(
    [Parameter(Mandatory=$true)][string]$ManifestPath,
    [string]$AdbExe = "",
    [string]$Serial = "",
    [switch]$Logcat
)
. "$PSScriptRoot/quest_common.ps1"
try {
    $ManifestPath = (Resolve-Path -LiteralPath $ManifestPath).Path
    $build = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
    if ($build.schemaVersion -ne 1 -or $build.profile -notin @('GoldenSlice','FullDevelopment')) { throw 'Unrecognized build manifest.' }
    $apk = Join-Path (Split-Path $ManifestPath -Parent) $build.apk
    if ((Get-FileHash -LiteralPath $apk -Algorithm SHA256).Hash -ne $build.apkSha256) { throw 'APK hash does not match manifest; nothing installed.' }
    $AdbExe = Resolve-QuestAdb $AdbExe
    $Serial = Select-QuestDevice $AdbExe $Serial
    $folder = Join-Path (Split-Path $ManifestPath -Parent) ('install-' + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $folder | Out-Null
    $result = Invoke-QuestAdb $AdbExe @('-s',$Serial,'install','-r',$apk)
    if ($result -notmatch '(?m)^Success\s*$') { throw "Installation not confirmed: $result" }
    $receipt = @{ buildManifest=$ManifestPath; apkSha256=$build.apkSha256; profile=$build.profile; device=$Serial; installedUtc=[DateTime]::UtcNow.ToString('o'); stage='installed' }
    $receipt | ConvertTo-Json | Set-Content (Join-Path $folder 'receipt.json') -Encoding UTF8
    if ($Logcat) {
        Start-QuestApp $AdbExe $Serial
        Start-Sleep -Seconds 15
        $log = Join-Path $folder 'quest_logcat.log'
        Save-QuestLog $AdbExe $Serial $log
        Assert-QuestLog $log $build.profile
        $receipt.stage = 'boot-smoke-passed'
        $receipt | ConvertTo-Json | Set-Content (Join-Path $folder 'receipt.json') -Encoding UTF8
    }
    Write-Host "SUCCESS stage=$($receipt.stage) evidence=$folder (gameplay not certified)"
    exit 0
} catch { Write-Error $_ -ErrorAction Continue; exit 1 }

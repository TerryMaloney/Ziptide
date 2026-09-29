param(
    [string]$UnityExe = "",
    [string]$ProjectRoot = "",
    [string]$AdbExe = "",
    [string]$Serial = "",
    [switch]$SkipBake,
    [switch]$NoBuild
)
. "$PSScriptRoot/quest_common.ps1"
try {
    if ($SkipBake) { throw '-SkipBake is retired: required generation is part of every build. Use -NoBuild only to retest the installed app.' }
    $ProjectRoot = Resolve-QuestProject $ProjectRoot
    if (-not $NoBuild) {
        & "$PSScriptRoot/dev_build_install.ps1" -ProjectRoot $ProjectRoot -UnityExe $UnityExe -BuildProfile FullDevelopment -AdbExe $AdbExe -Serial $Serial
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
        # Use the same bundled ADB selected by the builder when PATH has none.
        $UnityExe = Resolve-QuestEditor $ProjectRoot $UnityExe
    }
    $AdbExe = Resolve-QuestAdb $AdbExe $UnityExe
    $Serial = Select-QuestDevice $AdbExe $Serial
    $session = New-QuestSession $ProjectRoot
    $logPath = Join-Path $session 'quest_logcat.log'
    # Current checkout is intentionally not attributed to a preinstalled binary.
    $identity = Invoke-QuestAdb $AdbExe @('-s',$Serial,'shell','dumpsys','package',$script:QuestPackage)
    $identity | Set-Content (Join-Path $session 'installed-package.txt') -Encoding UTF8
    @{ stage='capture'; device=$Serial; installedSource='unknown; see build manifest if installed this session'; noBuild=[bool]$NoBuild } |
        ConvertTo-Json | Set-Content (Join-Path $session 'capture.json') -Encoding UTF8
    Start-QuestApp $AdbExe $Serial
    $argsLine = "-s `"$Serial`" logcat -v threadtime Unity:V ZIPTIDE_CAPTURE:V CRASH:V AndroidRuntime:V DEBUG:V libc:F *:S"
    $capture = Start-Process -FilePath $AdbExe -ArgumentList $argsLine -RedirectStandardOutput $logPath -RedirectStandardError (Join-Path $session 'adb-stderr.log') -NoNewWindow -PassThru
    try {
        Write-Host "Capture: $session"
        Write-Host 'Play the route in docs/production/TONIGHT_TEST_CARD.md. Press ENTER when finished.'
        [void](Read-Host)
    } finally {
        if (-not $capture.HasExited) { Stop-Process -Id $capture.Id -ErrorAction SilentlyContinue }
    }
    Assert-QuestLog $logPath 'FullDevelopment'
    Write-Host "CAPTURE CHECK PASS; route completion still requires your verdict. Upload: $session"
    exit 0
} catch { Write-Error $_ -ErrorAction Continue; exit 1 }

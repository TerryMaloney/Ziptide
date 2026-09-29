param(
    [string]$ProjectRoot = "",
    [ValidateSet("GoldenSlice", "FullDevelopment")]
    [string]$BuildProfile = "GoldenSlice",
    [string]$UnityExe = "",
    [string]$AdbExe = "",
    [string]$Serial = ""
)
# The canonical command requires fresh APK + install + launch + nonempty profile-proven log.
# Golden proof: ZIPTIDE: BUILD_PROFILE profile=GoldenSlice
# Runtime proof: ZIPTIDE: RECOVERY_EXPOSURE buildProfile=GoldenSlice profile=GoldenSlice
& "$PSScriptRoot/dev_build_install.ps1" -ProjectRoot $ProjectRoot -UnityExe $UnityExe -BuildProfile $BuildProfile -AdbExe $AdbExe -Serial $Serial -Logcat
exit $LASTEXITCODE

<#
.SYNOPSIS
    STEP 1 of 4. Get the latest code and show what you are about to test.

.DESCRIPTION
    Nothing is built here. This only fetches terry-local-wip and prints the commit, so that when
    something looks wrong later there is no doubt about which source produced it.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run1_pull.ps1
#>
param([string]$Repo = "C:\Ziptide")

$ErrorActionPreference = 'Stop'
Push-Location $Repo
try {
    Write-Host "===== 1/4  PULL =====" -ForegroundColor Cyan

    git pull origin terry-local-wip
    if ($LASTEXITCODE -ne 0) {
        Write-Host "git pull failed. Fix that before anything else." -ForegroundColor Red
        return
    }

    $sha    = (git rev-parse --short HEAD)
    $branch = (git branch --show-current)
    $subject = (git log -1 --pretty=%s)

    Write-Host ""
    Write-Host ("  branch : " + $branch) -ForegroundColor White
    Write-Host ("  commit : " + $sha) -ForegroundColor White
    Write-Host ("  message: " + $subject) -ForegroundColor DarkGray

    Write-Host ""
    Write-Host "  NEXT: step 2 builds and installs. This change needs a real bake" -ForegroundColor Yellow
    Write-Host "  (the W000 layout and its first-hour surfaces are regenerated), so" -ForegroundColor Yellow
    Write-Host "  do NOT reinstall an older APK and expect to see the fixes." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run2_build_install.ps1" -ForegroundColor White
}
finally { Pop-Location }

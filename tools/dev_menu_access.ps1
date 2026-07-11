[CmdletBinding()]
param(
    [ValidateSet("Open", "Unlock", "Lock", "Status")]
    [string]$Action = "Open",
    [string]$Package = "com.terrymaloney.ziptide"
)

$ErrorActionPreference = "Stop"
$remoteRoot = "/sdcard/Android/data/$Package/files"
$accessMarker = "$remoteRoot/.ziptide_dev_access"
$openMarker = "$remoteRoot/.ziptide_dev_open"

function Invoke-AdbChecked {
    param([string[]]$Arguments)
    & adb @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "adb failed ($LASTEXITCODE): adb $($Arguments -join ' ')"
    }
}

function Assert-DeviceConnected {
    $devices = adb devices 2>&1 | Out-String
    if ($devices -notmatch "(?m)^\S+\s+device\s*$") {
        throw "No authorized Quest found. Connect USB, enable developer mode/USB debugging, and accept the headset prompt."
    }
}

function Push-Marker {
    param(
        [string]$RemotePath,
        [string]$Label
    )

    $temp = Join-Path ([System.IO.Path]::GetTempPath()) ("ziptide-" + [Guid]::NewGuid().ToString("N") + ".txt")
    try {
        Set-Content -Path $temp -Value ("$Label " + [DateTime]::UtcNow.ToString("o")) -Encoding ascii -NoNewline
        Invoke-AdbChecked @("push", $temp, $RemotePath)
    }
    finally {
        Remove-Item $temp -Force -ErrorAction SilentlyContinue
    }
}

Assert-DeviceConnected

switch ($Action) {
    "Unlock" {
        Invoke-AdbChecked @("shell", "mkdir", "-p", $remoteRoot)
        Push-Marker -RemotePath $accessMarker -Label "developer-access"
        Write-Host "ZIPTIDE developer access unlocked." -ForegroundColor Green
        Write-Host "Use -Action Open whenever you want the in-headset warp menu."
    }

    "Open" {
        Invoke-AdbChecked @("shell", "mkdir", "-p", $remoteRoot)
        Push-Marker -RemotePath $accessMarker -Label "developer-access"
        Push-Marker -RemotePath $openMarker -Label "open-request"

        # Bring the app forward if it is installed but not currently running. This does not reserve
        # or synthesize any controller input; a running app consumes the marker on its next poll.
        & adb shell monkey -p $Package -c android.intent.category.LAUNCHER 1 | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Open marker was written, but the app could not be launched automatically. Start ZIPTIDE on the headset; the menu will open after launch."
        }
        else {
            Write-Host "ZIPTIDE developer menu open request sent." -ForegroundColor Green
        }
    }

    "Lock" {
        Invoke-AdbChecked @("shell", "rm", "-f", $accessMarker, $openMarker)
        Write-Host "ZIPTIDE developer access locked. A visible dev menu will close on its next poll." -ForegroundColor Yellow
    }

    "Status" {
        $listing = adb shell ls -la $remoteRoot 2>&1 | Out-String
        $unlocked = $listing -match [Regex]::Escape(".ziptide_dev_access")
        $pendingOpen = $listing -match [Regex]::Escape(".ziptide_dev_open")
        Write-Host ("Access: " + $(if ($unlocked) { "UNLOCKED" } else { "LOCKED" }))
        Write-Host ("Open request: " + $(if ($pendingOpen) { "PENDING" } else { "NONE" }))
    }
}

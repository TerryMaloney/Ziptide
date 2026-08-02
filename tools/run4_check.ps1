<#
.SYNOPSIS
    STEP 4 of 4. Grade the captured log against the 2026-08-02 fixes, then hand you the paste-back.

.DESCRIPTION
    Answers the half of the checklist a machine can answer. Every line below is a verdict on one
    named defect from the 2026-08-01 device pass, so a FAIL here points at a specific fix rather
    than at "something is off".

    ONLY RUNTIME TAGS ARE CHECKED. CITY_MATERIALS, APPROACH_DRESSED, WAYFINDING and
    WORLD_ATMO_AUTHOR are logged by the EDITOR during a bake and never reach a headset; their
    geometry is checked with your eyes, in docs\DEVICE_CHECK_2026-08-02.md.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run4_check.ps1
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\run4_check.ps1 -Log .\Ziptide\Builds\quest_20260802_0830.log
#>
param(
    [string]$Repo = "C:\Ziptide",
    [string]$Log
)

$ErrorActionPreference = 'Stop'
Push-Location $Repo
try {
    Write-Host "===== 4/4  CHECK =====" -ForegroundColor Cyan

    if (-not $Log) {
        $candidates = @()
        $candidates += Get-ChildItem ".\Ziptide\Builds\quest_*.log" -ErrorAction SilentlyContinue
        $candidates += Get-ChildItem ".\Builds\session-*\logcat.log" -Recurse -ErrorAction SilentlyContinue
        $newest = $candidates | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        if ($newest) { $Log = $newest.FullName }
    }
    if (-not $Log -or -not (Test-Path $Log)) {
        Write-Host "No log found. Pass one with -Log." -ForegroundColor Red
        return
    }

    Write-Host ("  log : " + $Log) -ForegroundColor DarkGray
    Write-Host ("  size: " + [math]::Round((Get-Item $Log).Length / 1KB) + " KB") -ForegroundColor DarkGray
    Write-Host ""

    $text = Get-Content $Log -Raw
    function Hits($pattern) { return ([regex]::Matches($text, $pattern)).Count }

    $results = @()
    function Verdict($ok, $name, $detail) {
        if ($ok) {
            Write-Host ("  [PASS] " + $name) -ForegroundColor Green
            $script:results += ("PASS   " + $name)
        } else {
            Write-Host ("  [FAIL] " + $name + "  -- " + $detail) -ForegroundColor Red
            $script:results += ("FAIL   " + $name + " -- " + $detail)
        }
    }
    function Note($name, $value) {
        Write-Host ("  [ .. ] " + $name + ": " + $value) -ForegroundColor DarkGray
        $script:results += ("INFO   " + $name + ": " + $value)
    }

    # -- The two blockers ------------------------------------------------------
    Write-Host "-- THE TWO BLOCKERS -----------------------------------------" -ForegroundColor Cyan

    $falls = Hits 'FALL_SAFETY y='
    Verdict ($falls -eq 0) "Nobody fell off the world" ("FALL_SAFETY fired " + $falls + " time(s)")

    $boards = Hits 'SHIP_BOARD stand='
    $boardAborts = Hits 'SHIP_BOARD_ABORT'
    if ($boards -gt 0) {
        Verdict ($boardAborts -eq 0) "BOARD SHIP put you on the deck" ("refused " + $boardAborts + " time(s) - the deck was not proven, see the source= field")
    } else {
        Note "BOARD SHIP" "never pressed"
    }

    $selected = Hits 'FLIGHT_DESTINATION_SELECTED'
    $launched = Hits 'FLIGHT_LAUNCH'
    $blocked  = Hits 'FLIGHT_BLOCKED'
    Note "destination selected" $selected
    Note "PUNCH IT pressed and armed (FLIGHT_LAUNCH)" $launched
    Note "PUNCH IT pressed and refused (FLIGHT_BLOCKED)" $blocked
    if ($selected -gt 0 -and $launched -eq 0 -and $blocked -eq 0) {
        Write-Host "  [FAIL] you selected a destination but never pressed PUNCH IT" -ForegroundColor Red
        Write-Host "         (that was the 2026-08-01 symptom: 7 selections, 0 launches)" -ForegroundColor Red
        $results += "FAIL   destination selected but PUNCH IT never pressed"
    }

    $arrived = Hits 'TRAVEL_OK'
    Verdict ($arrived -gt 0) "A scene change actually completed (TRAVEL_OK)" "you never left the scene you started in"
    if ($text -match 'REPAIR_TRACE[^\r\n]*scene=(\w+)') {
        Note "last scene seen in the log" $Matches[1]
    }

    # -- Today's fixes ---------------------------------------------------------
    Write-Host ""
    Write-Host "-- TODAY'S FIXES --------------------------------------------" -ForegroundColor Cyan

    if ($text -match 'TURN_MODE[^\r\n]*speed=(\d+)') {
        $speed = [int]$Matches[1]
        Verdict ($speed -eq 75) "Turn speed is the new 75 deg/s" ("still reading " + $speed + " - you are on an old APK")
    } else { Note "TURN_MODE" "not logged (rig never finished booting?)" }

    if ($text -match 'MELEE_GRIP_SEMANTIC[^\r\n]*rake=(\d+)') {
        Verdict ($Matches[1] -eq '40') "Sword grip is raked (blade rides above the fist)" ("rake=" + $Matches[1])
    } else { Note "MELEE_GRIP_SEMANTIC" "no melee weapon spawned in this session" }

    $holsterSpam = Hits 'HOLSTER_TARGET'
    Verdict ($holsterSpam -lt 25) "HOLSTER_TARGET no longer logs every frame" ($holsterSpam.ToString() + " lines - the per-frame log is back")

    $dropped = [regex]::Matches($text, 'dropped=(\d+)')
    if ($dropped.Count -gt 0) {
        $worst = ($dropped | ForEach-Object { [int]$_.Groups[1].Value } | Measure-Object -Maximum).Maximum
        Note "worst logcat drop reported" $worst
    }

    if ($text -match 'CASTOFF_CONSOLE_LOCATION mode=(\w+)') {
        Verdict ($Matches[1] -eq 'cockpit') "PUNCH IT console built on the cockpit deck" ("mode=" + $Matches[1])
    } else { Note "CASTOFF_CONSOLE_LOCATION" "not logged" }

    if ($text -match 'FIRST_HELM_DOCKED anchor=(\w+)') {
        Verdict ($Matches[1] -eq 'console') "W001 tile docked beside PUNCH IT" ("anchor=" + $Matches[1] + " - it is loose on the hull again")
    } else { Note "FIRST_HELM_DOCKED" "not logged" }

    # -- Anything that actually broke -----------------------------------------
    Write-Host ""
    Write-Host "-- FAILURES -------------------------------------------------" -ForegroundColor Cyan
    $bad = Select-String -Path $Log -Pattern 'NullReferenceException|Exception:|AndroidRuntime|FATAL|TRAVEL_FAIL|DUP_SINGLETON|NO_RAY_INTERACTORS|INPUT_ACTIONS_MISSING|ITEM_DEF_NOT_FOUND'
    if ($bad) {
        $groups = $bad | Group-Object { ($_.Line -replace '^.*?ZIPTIDE: ', 'ZIPTIDE: ') } |
                  Sort-Object Count -Descending | Select-Object -First 15
        foreach ($g in $groups) {
            Write-Host ("  x{0,-4} {1}" -f $g.Count, $g.Name.Substring(0, [Math]::Min(150, $g.Name.Length))) -ForegroundColor Red
        }
        Write-Host ("`n  " + $bad.Count + " failure line(s) total.") -ForegroundColor Red
    } else {
        Write-Host "  none. No exceptions, no fatals, no travel failures." -ForegroundColor Green
    }

    # -- Paste-back block -----------------------------------------------------
    $out = ".\Ziptide\Builds\check_result.txt"
    $summary = @()
    $summary += "ZIPTIDE CHECK - " + (Get-Date -Format 'yyyy-MM-dd HH:mm')
    $summary += "commit: " + (git rev-parse --short HEAD)
    $summary += "log:    " + (Split-Path $Log -Leaf)
    $summary += ""
    $summary += $results
    $summary += ""
    $summary += "failure lines: " + $(if ($bad) { $bad.Count } else { 0 })
    if ($bad) { $summary += ($bad | Select-Object -First 15 | ForEach-Object { "  " + $_.Line.Trim() }) }
    $summary | Set-Content $out -Encoding ASCII

    Write-Host ""
    Write-Host "-- NEXT -----------------------------------------------------" -ForegroundColor Cyan
    Write-Host ("  Machine half written to: " + $out) -ForegroundColor White
    Write-Host "  Now fill in the LOOK/FEEL half - only you can answer those:" -ForegroundColor White
    Write-Host "     docs\DEVICE_CHECK_2026-08-02.md" -ForegroundColor White
    Write-Host "  Paste both back and the next session starts from evidence." -ForegroundColor White
}
finally { Pop-Location }

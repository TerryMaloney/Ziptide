<#
.SYNOPSIS
    Reads a captured logcat and answers the half of the checklist a machine can answer.

.DESCRIPTION
    Run this after Ctrl+C-ing the logcat window. It prints PASS/FAIL/ABSENT per runtime signal,
    then every exception it found, then a block you can paste straight back to the operator.

    ONLY RUNTIME TAGS ARE CHECKED HERE. `APPROACH_DRESSED`, `WAYFINDING`, `CITY_MATERIALS` and
    `WORLD_ATMO_AUTHOR` are logged by the EDITOR during a bake, so they never reach a headset and
    are absent from a log by design. The geometry they describe is checked with your eyes instead -
    that is what docs/DEVICE_CHECK_2026-08-01.md is for.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\morning_check.ps1
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File C:\Ziptide\tools\morning_check.ps1 -Log .\Ziptide\Builds\quest_20260802_0830.log
#>
param(
    [string]$Repo = "C:\Ziptide",
    [string]$Log
)

$ErrorActionPreference = 'Stop'
Push-Location $Repo
try {
    if (-not $Log) {
        $Log = (Get-ChildItem ".\Ziptide\Builds\quest_*.log" -ErrorAction SilentlyContinue |
                Sort-Object LastWriteTime -Descending | Select-Object -First 1).FullName
    }
    if (-not $Log -or -not (Test-Path $Log)) {
        Write-Host "No log found. Pass one with -Log." -ForegroundColor Red; return
    }

    Write-Host "===== ZIPTIDE MORNING CHECK =====" -ForegroundColor Cyan
    Write-Host ("log: " + $Log) -ForegroundColor DarkGray
    Write-Host ("size: " + [math]::Round((Get-Item $Log).Length / 1KB) + " KB`n") -ForegroundColor DarkGray

    $text = Get-Content $Log -Raw

    # name, pattern, what a miss MEANS. The meaning is the point - a bare tag list tells the next
    # person nothing about whether an absence is a bug or just a route they did not walk.
    $signals = @(
        @{ n = 'Boot held the rig at the menu';  p = 'BOOT_HOLD';                m = 'boot may not have gated the sticks' },
        @{ n = 'Spawned into a world';           p = 'SPAWN_AT';                 m = 'never landed anywhere' },
        @{ n = 'Travel succeeded';               p = 'TRAVEL_OK';                m = 'no scene change completed' },
        @{ n = 'ATMOSPHERE applied (NEW today)'; p = 'WORLD_ATMO applied=1';     m = 'the acid haze/motes did not bind' },
        @{ n = 'Armoury rack built';             p = 'ARMOURY_RACK';             m = 'you never boarded the ship' },
        @{ n = 'Armoury gate evaluated';         p = 'ARMOURY_GATE';             m = 'you never boarded the ship' },
        @{ n = 'Armor system live (NEW)';        p = 'ARMOR_READY|ARMOR_HUD';    m = 'the player cannot be hurt' },
        @{ n = 'Wayfinding beacon targeting';    p = 'BEACON_TARGET';            m = 'no objective was active' }
    )

    Write-Host "-- RUNTIME SIGNALS ------------------------------------------" -ForegroundColor Cyan
    $results = @()
    foreach ($s in $signals) {
        $hits = ([regex]::Matches($text, $s.p)).Count
        if ($hits -gt 0) {
            Write-Host ("  [PASS] {0}  ({1} hit(s))" -f $s.n, $hits) -ForegroundColor Green
            $results += "PASS  $($s.n)"
        } else {
            Write-Host ("  [ -- ] {0}  -- absent: {1}" -f $s.n, $s.m) -ForegroundColor Yellow
            $results += "ABSENT $($s.n) -- $($s.m)"
        }
    }

    # -- Anything that actually broke -----------------------------------------
    Write-Host "`n-- FAILURES -------------------------------------------------" -ForegroundColor Cyan
    $bad = Select-String -Path $Log -Pattern 'NullReferenceException|Exception:|AndroidRuntime|FATAL|TRAVEL_FAIL|DUP_SINGLETON|NO_RAY_INTERACTORS|INPUT_ACTIONS_MISSING|ITEM_DEF_NOT_FOUND'
    if ($bad) {
        $groups = $bad | Group-Object { ($_.Line -replace '^.*?ZIPTIDE: ', 'ZIPTIDE: ') } |
                  Sort-Object Count -Descending | Select-Object -First 15
        foreach ($g in $groups) {
            Write-Host ("  x{0,-4} {1}" -f $g.Count, $g.Name.Substring(0, [Math]::Min(150, $g.Name.Length))) -ForegroundColor Red
        }
        Write-Host ("`n  {0} failure line(s) total." -f $bad.Count) -ForegroundColor Red
    } else {
        Write-Host "  none. No exceptions, no fatals, no travel failures." -ForegroundColor Green
    }

    # -- Paste-back block -----------------------------------------------------
    $out = ".\Ziptide\Builds\morning_check_result.txt"
    $summary = @()
    $summary += "ZIPTIDE MORNING CHECK - " + (Get-Date -Format 'yyyy-MM-dd HH:mm')
    $summary += "commit: " + (git rev-parse --short HEAD)
    $summary += "log:    " + (Split-Path $Log -Leaf)
    $summary += ""
    $summary += $results
    $summary += ""
    $summary += "failure lines: " + $(if ($bad) { $bad.Count } else { 0 })
    if ($bad) { $summary += ($bad | Select-Object -First 15 | ForEach-Object { "  " + $_.Line.Trim() }) }
    $summary | Set-Content $out -Encoding UTF8

    Write-Host "`n-- NEXT -----------------------------------------------------" -ForegroundColor Cyan
    Write-Host "  Machine half written to: $out" -ForegroundColor White
    Write-Host "  Now fill in the LOOK/FEEL half - only you can answer those:" -ForegroundColor White
    Write-Host "     docs\DEVICE_CHECK_2026-08-01.md" -ForegroundColor White
    Write-Host "  Paste both back and the next session starts from evidence." -ForegroundColor White
}
finally { Pop-Location }

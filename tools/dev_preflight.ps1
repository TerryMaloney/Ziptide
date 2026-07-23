[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$RepoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $RepoRoot
try {
    $Python = $null
    if (Get-Command py -ErrorAction SilentlyContinue) {
        $Python = @('py', '-3')
    }
    elseif (Get-Command python -ErrorAction SilentlyContinue) {
        $Python = @('python')
    }
    elseif (Get-Command python3 -ErrorAction SilentlyContinue) {
        $Python = @('python3')
    }
    else {
        throw 'Python 3 was not found. Install Python 3 or add py/python/python3 to PATH.'
    }

    function Invoke-Python {
        param([Parameter(Mandatory = $true)][string[]]$Arguments)
        $exe = $Python[0]
        $prefix = @()
        if ($Python.Count -gt 1) {
            $prefix = $Python[1..($Python.Count - 1)]
        }
        & $exe @prefix @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "Python preflight step failed with exit code $LASTEXITCODE."
        }
    }

    Write-Host '== ZIPTIDE FAST PREFLIGHT ==' -ForegroundColor Cyan

    Write-Host '[1/3] Factory governance gate'
    Invoke-Python @('tools/factory_governance_gate.py')

    Write-Host '[2/3] Fast Python gate tests'
    Invoke-Python @('-m', 'unittest', 'discover', '-s', 'tools/tests', '-p', 'test_*_gate.py', '-v')

    Write-Host '[3/3] Git whitespace/conflict check'
    & git diff --check
    if ($LASTEXITCODE -ne 0) {
        throw "git diff --check failed with exit code $LASTEXITCODE."
    }

    Write-Host 'ZIPTIDE FAST PREFLIGHT: PASS' -ForegroundColor Green
}
finally {
    Pop-Location
}

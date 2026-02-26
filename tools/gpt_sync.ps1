param(
    [string]$RepoRoot = "C:\Ziptide",
    [string]$OutFile = "C:\Ziptide\GPT_PROJECT_UPDATE.md"
)

Push-Location $RepoRoot
$commit = (git rev-parse --short HEAD 2>$null)
$status = (git status -sb 2>$null)
$log = (git log -1 --pretty=fuller 2>$null)
$diffStat = (git diff --stat HEAD~1..HEAD 2>$null)

@"
# ZIPTIDE Sync
Time: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Commit: $commit
Repo: https://github.com/TerryMaloney/Ziptide.git

## git status
$status

## last commit
$log

## diff stat (last commit)
$diffStat
"@ | Set-Content -Encoding UTF8 $OutFile

Pop-Location
Write-Host "Wrote: $OutFile"

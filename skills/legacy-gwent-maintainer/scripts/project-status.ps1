[CmdletBinding()]
param(
    [switch]$Remote,
    [switch]$GitHub,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'

function Invoke-Git {
    param([string[]]$GitArguments)

    $output = @(& git -C $script:RepoRoot @GitArguments 2>&1 | ForEach-Object { "$_" })
    if ($LASTEXITCODE -ne 0) {
        throw "git $($GitArguments -join ' ') failed:`n$($output -join "`n")"
    }
    return ($output -join "`n").Trim()
}

function Get-OptionalCommandResult {
    param([scriptblock]$Action)

    try {
        return & $Action
    }
    catch {
        return [pscustomobject]@{ Error = $_.Exception.Message }
    }
}

$invokedSkillRoot = [IO.Path]::GetFullPath((Split-Path $PSScriptRoot -Parent))
$skillRootItem = Get-Item -Force -LiteralPath $invokedSkillRoot
$physicalSkillRoot = if ($skillRootItem.LinkType -and $skillRootItem.Target) {
    [IO.Path]::GetFullPath(@($skillRootItem.Target)[0])
} else {
    $skillRootItem.FullName
}
$candidateRoot = [IO.Path]::GetFullPath((Join-Path $physicalSkillRoot '..\..'))
$repoOutput = @(& git -C $candidateRoot rev-parse --show-toplevel 2>$null |
    ForEach-Object { "$_" })
if ($LASTEXITCODE -ne 0 -or $repoOutput.Count -eq 0) {
    throw "The canonical skill is not inside a Git checkout: $physicalSkillRoot"
}
$script:RepoRoot = [IO.Path]::GetFullPath(($repoOutput -join "`n").Trim())

$branch = Invoke-Git @('branch', '--show-current')
$head = Invoke-Git @('rev-parse', 'HEAD')
$upstream = Get-OptionalCommandResult { Invoke-Git @('rev-parse', '--abbrev-ref', '@{upstream}') }
$behind = $null
$ahead = $null
if ($upstream -is [string] -and -not [string]::IsNullOrWhiteSpace($upstream)) {
    $counts = (Invoke-Git @('rev-list', '--left-right', '--count', "$upstream...HEAD")) -split '\s+'
    $behind = [int]$counts[0]
    $ahead = [int]$counts[1]
}

$dirtyOutput = @(& git -C $script:RepoRoot status --porcelain=v1 2>&1 |
    ForEach-Object { "$_" })
if ($LASTEXITCODE -ne 0) {
    throw "git status --porcelain=v1 failed:`n$($dirtyOutput -join "`n")"
}
$dirtyLines = @($dirtyOutput | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
$mapPath = Join-Path $script:RepoRoot 'src\Cynthia.Card\src\Cynthia.Card.Common\GwentGame\GwentMap.cs'
$mapSource = Get-Content -Raw -LiteralPath $mapPath
$versionMatches = @([regex]::Matches($mapSource, 'new Version\(([^)]+)\)'))
$versions = @($versionMatches | ForEach-Object {
    (($_.Groups[1].Value -split ',') | ForEach-Object { $_.Trim() }) -join '.'
})
$cardMapVersion = if ($versions.Count -gt 0) { $versions[-1] } else { $null }
$developmentCardMapVersion = if ($versions.Count -gt 1) { $versions[0] } else { $null }

$listeners = @()
foreach ($port in @(5010, 5020, 5021, 28021)) {
    try {
        foreach ($connection in @(Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction Stop)) {
            $process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue
            $listeners += [pscustomobject]@{
                Port = $port
                Pid = $connection.OwningProcess
                Process = if ($process) { $process.ProcessName } else { $null }
            }
        }
    }
    catch {
        # No listener or the platform does not expose Get-NetTCPConnection.
    }
}

$status = [ordered]@{
    CheckedAt = (Get-Date).ToString('o')
    Repository = [pscustomobject]@{
        Root = $script:RepoRoot
        Branch = $branch
        Head = $head
        Upstream = if ($upstream -is [string]) { $upstream } else { $null }
        Behind = $behind
        Ahead = $ahead
        Dirty = ($dirtyLines.Count -gt 0)
        ChangedPaths = $dirtyLines
        CardMapVersion = $cardMapVersion
        DevelopmentCardMapVersion = $developmentCardMapVersion
    }
    LocalListeners = $listeners
}

if ($Remote) {
    $status.Remote = Get-OptionalCommandResult {
        $remoteCommand = 'printf "service=%s\n" "$(systemctl is-active card-diy-ai.service 2>/dev/null || true)"; printf "release=%s\n" "$(readlink -f /usr/share/card-diy-ai/current 2>/dev/null || true)"; printf "health=%s\n" "$(curl -sS -o /dev/null -w "%{http_code}" --max-time 5 http://127.0.0.1:5010/healthz 2>/dev/null || true)"'
        $lines = @(& ssh -o BatchMode=yes -o ConnectTimeout=5 root@cynthia.ovyno.com $remoteCommand 2>&1 |
            ForEach-Object { "$_" })
        if ($LASTEXITCODE -ne 0) {
            throw "Remote status failed:`n$($lines -join "`n")"
        }
        $values = @{}
        foreach ($line in $lines) {
            if ($line -match '^([^=]+)=(.*)$') {
                $values[$Matches[1]] = $Matches[2]
            }
        }
        [pscustomobject]@{
            Service = $values.service
            Release = $values.release
            HealthHttp = $values.health
        }
    }
}

if ($GitHub) {
    $status.GitHubRuns = Get-OptionalCommandResult {
        if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
            throw 'GitHub CLI (gh) is not installed.'
        }
        $ghArguments = @(
            'run', 'list',
            '--repo', 'LegacyGwent/LegacyGwent',
            '--branch', 'diy-ai',
            '--limit', '5',
            '--json', 'databaseId,workflowName,status,conclusion,headSha,createdAt,url'
        )
        $raw = @(& gh @ghArguments 2>&1 |
            ForEach-Object { "$_" })
        if ($LASTEXITCODE -ne 0) {
            throw "GitHub status failed:`n$($raw -join "`n")"
        }
        ($raw -join "`n") | ConvertFrom-Json
    }
}

$result = [pscustomobject]$status
if ($Json) {
    $result | ConvertTo-Json -Depth 8
} else {
    $result
}

param(
    [switch]$KeepMongoDB
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-common.ps1")

$serverPid = Get-ListeningProcessId -Port $script:ServerPort
if ($serverPid) {
    $server = Get-Process -Id $serverPid -ErrorAction Stop
    if ($server.ProcessName -notin @("dotnet", "Cynthia.Card.Server")) {
        throw "Refusing to stop unexpected process $($server.ProcessName) on port $script:ServerPort."
    }
    Stop-Process -Id $serverPid
    Wait-Process -Id $serverPid -Timeout 15 -ErrorAction SilentlyContinue
    Write-Host "Stopped Card DIY development server (PID $serverPid)."
}
Remove-Item -LiteralPath $script:ServerPidFile -Force -ErrorAction SilentlyContinue

if (-not $KeepMongoDB) {
    $mongoPid = Get-ListeningProcessId -Port $script:MongoPort
    if ($mongoPid) {
        $mongo = Get-Process -Id $mongoPid -ErrorAction Stop
        if ($mongo.Path -notlike "$script:MongoRoot*") {
            throw "Refusing to stop unexpected process $($mongo.Path) on port $script:MongoPort."
        }
        $mongoShell = Get-MongoShellExecutable
        if ($mongoShell) {
            & $mongoShell --quiet --host 127.0.0.1 --port $script:MongoPort `
                --eval "db.getSiblingDB('admin').shutdownServer()" 2>$null
        }
        if (Get-Process -Id $mongoPid -ErrorAction SilentlyContinue) {
            Stop-Process -Id $mongoPid
        }
        Write-Host "Stopped MongoDB (PID $mongoPid)."
    }
}

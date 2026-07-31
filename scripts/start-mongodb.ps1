Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-common.ps1")

$existingPid = Get-ListeningProcessId -Port $script:MongoPort
if ($existingPid) {
    $existing = Get-Process -Id $existingPid -ErrorAction Stop
    if ($existing.Path -notlike "$script:MongoRoot*") {
        throw "Port $script:MongoPort is already occupied by $($existing.Path)."
    }
    Write-Host "MongoDB is already listening on 127.0.0.1:$script:MongoPort (PID $existingPid)."
    return
}

$mongod = Get-MongoExecutable
New-Item -ItemType Directory -Path $script:MongoDataRoot, $script:MongoLogRoot -Force | Out-Null
$mongoLog = Join-Path $script:MongoLogRoot "mongodb.log"

$process = Start-Process -FilePath $mongod -ArgumentList @(
    "--dbpath", $script:MongoDataRoot,
    "--port", $script:MongoPort,
    "--bind_ip", "127.0.0.1",
    "--logpath", $mongoLog,
    "--logappend"
) -WindowStyle Hidden -PassThru

$deadline = (Get-Date).AddSeconds(30)
do {
    Start-Sleep -Milliseconds 500
    if ($process.HasExited) {
        throw "MongoDB exited during startup. See $mongoLog"
    }
    $listeningPid = Get-ListeningProcessId -Port $script:MongoPort
} while (-not $listeningPid -and (Get-Date) -lt $deadline)

if (-not $listeningPid) {
    Stop-Process -Id $process.Id -ErrorAction SilentlyContinue
    throw "MongoDB did not listen on port $script:MongoPort within 30 seconds. See $mongoLog"
}

Write-Host "MongoDB 4.4.29 is listening on mongodb://127.0.0.1:$script:MongoPort (PID $listeningPid)."

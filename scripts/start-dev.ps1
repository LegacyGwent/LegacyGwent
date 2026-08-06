param(
    [switch]$NoWatch,
    [switch]$Background,
    [string]$FeatureManifest,
    [switch]$EnableRuleFixtures
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-common.ps1")
& (Join-Path $PSScriptRoot "start-mongodb.ps1")

$existingPid = Get-ListeningProcessId -Port $script:ServerPort
if ($existingPid) {
    throw "Port $script:ServerPort is already in use by PID $existingPid."
}
if (-not (Test-DotNetServerSdk)) {
    throw ".NET $script:DotNetSdkChannel SDK is missing. Run scripts\setup-dev.ps1 first."
}

$env:DOTNET_ROOT = Split-Path -Parent $script:DotNetExe
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://127.0.0.1:$script:ServerPort"
$env:MONGO_CONNECTION_STRING = "mongodb://127.0.0.1:$script:MongoPort/$script:MongoDatabase"
if ($FeatureManifest) {
    $env:GWENT_FEATURE_MANIFEST = $FeatureManifest
} else {
    Remove-Item Env:GWENT_FEATURE_MANIFEST -ErrorAction SilentlyContinue
}
if ($EnableRuleFixtures) {
    $env:GWENT_ENABLE_RULE_FIXTURES = "1"
} else {
    Remove-Item Env:GWENT_ENABLE_RULE_FIXTURES -ErrorAction SilentlyContinue
}

if ($NoWatch) {
    & $script:DotNetExe build $script:ServerProject --nologo
    $arguments = @($script:ServerDll)
} else {
    $arguments = @("watch", "--project", $script:ServerProject, "run")
}

if ($Background) {
    New-Item -ItemType Directory -Path $script:MongoLogRoot -Force | Out-Null
    $stdout = Join-Path $script:MongoLogRoot "card-diy-dev.stdout.log"
    $stderr = Join-Path $script:MongoLogRoot "card-diy-dev.stderr.log"
    $process = Start-Process -FilePath $script:DotNetExe -ArgumentList $arguments `
        -WorkingDirectory $script:ServerProjectRoot -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    Set-Content -LiteralPath $script:ServerPidFile -Value $process.Id
    Write-Host "Card $script:DevProfile development server started in background (PID $($process.Id))."
    Write-Host "Logs: $stdout and $stderr"
    return
}

Push-Location $script:ServerProjectRoot
try {
    & $script:DotNetExe @arguments
}
finally {
    Pop-Location
}

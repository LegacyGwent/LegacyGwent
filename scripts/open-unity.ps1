param(
    [string]$ServerUrl
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-common.ps1")

if (-not (Test-Path -LiteralPath $script:UnityExe)) {
    throw "Unity $script:UnityVersion is missing. Run scripts\setup-dev.ps1 first."
}

Sync-UnityCommonAssembly
if ([string]::IsNullOrWhiteSpace($ServerUrl)) {
    $ServerUrl = "http://127.0.0.1:$script:ServerPort"
}
$ServerUrl = $ServerUrl.TrimEnd("/")
$previousServerUrl = $env:GWENT_SERVER_URL
try {
    $env:GWENT_SERVER_URL = $ServerUrl
    Start-Process -FilePath $script:UnityExe -ArgumentList @("-projectPath", $script:UnityProjectRoot)
}
finally {
    $env:GWENT_SERVER_URL = $previousServerUrl
}
Write-Host "Opening the client project in Unity $script:UnityVersion."
Write-Host "The Unity client will connect to $ServerUrl."
Write-Host "If prompted, sign in through Unity Hub and activate a Unity Personal license."

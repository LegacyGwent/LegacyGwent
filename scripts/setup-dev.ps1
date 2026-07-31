param(
    [switch]$SkipUnity
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "dev-common.ps1")

$dotnetInstaller = Join-Path $env:TEMP "legacygwent-dotnet-install.ps1"
$mongoArchive = Join-Path $script:DevRoot "mongodb-windows-x86_64-4.4.29.zip"
$mongoUrl = "https://fastdl.mongodb.org/windows/mongodb-windows-x86_64-4.4.29.zip"
$unityInstaller = Join-Path $script:DevRoot "UnitySetup64-2019.4.1f1.exe"
$unityUrl = "https://download.unity3d.com/download_unity/e6c045e14e4e/Windows64EditorInstaller/UnitySetup64-2019.4.1f1.exe"

New-Item -ItemType Directory -Path $script:DevRoot -Force | Out-Null

if (-not (Test-Path -LiteralPath $script:DotNetExe)) {
    Write-Host "Installing ASP.NET Core Runtime 3.1.32..."
    curl.exe -L --fail --silent --show-error "https://dot.net/v1/dotnet-install.ps1" -o $dotnetInstaller
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $dotnetInstaller `
        -Runtime aspnetcore -Version 3.1.32 -InstallDir (Split-Path -Parent $script:DotNetExe) -NoPath
}

if (-not (Get-ChildItem -LiteralPath $script:MongoRoot -Filter "mongod.exe" -File -Recurse -ErrorAction SilentlyContinue)) {
    Write-Host "Installing MongoDB 4.4.29..."
    New-Item -ItemType Directory -Path $script:MongoRoot -Force | Out-Null
    if (-not (Test-Path -LiteralPath $mongoArchive)) {
        curl.exe -L --fail --show-error $mongoUrl -o $mongoArchive
    }
    Expand-Archive -LiteralPath $mongoArchive -DestinationPath $script:MongoRoot -Force
}

if (-not $SkipUnity) {
    $unityHub = Join-Path $env:ProgramFiles "Unity Hub\Unity Hub.exe"
    if (-not (Test-Path -LiteralPath $unityHub)) {
        Write-Host "Installing Unity Hub..."
        winget install --id Unity.UnityHub --exact --silent `
            --accept-package-agreements --accept-source-agreements --disable-interactivity
    }

    if (-not (Test-Path -LiteralPath $script:UnityExe)) {
        Write-Host "Installing Unity Editor 2019.4.1f1..."
        if (-not (Test-Path -LiteralPath $unityInstaller)) {
            curl.exe -L --fail --show-error $unityUrl -o $unityInstaller
        }
        $unityInstall = Start-Process -FilePath $unityInstaller `
            -ArgumentList @("/S", "/D=$script:UnityEditorRoot") `
            -WindowStyle Hidden -Wait -PassThru
        if ($unityInstall.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $script:UnityExe)) {
            throw "Unity Editor installation failed with exit code $($unityInstall.ExitCode)."
        }
    }
}

Write-Host "Development prerequisites are present under $script:DevRoot"
Write-Host "Run .\scripts\start-dev.ps1 to start MongoDB and the server."

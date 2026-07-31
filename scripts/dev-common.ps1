Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$script:RepoRoot = Split-Path -Parent $PSScriptRoot
$script:DevRoot = Join-Path $env:LOCALAPPDATA "LegacyGwentDev"
$script:DevProfile = if ($env:GWENT_DEV_PROFILE) { $env:GWENT_DEV_PROFILE } else { "diy" }
$script:DotNetExe = Join-Path $script:DevRoot "dotnet\dotnet.exe"
$script:DotNetSdkChannel = "10.0"
$script:MongoRoot = Join-Path $script:DevRoot "mongodb-4.4.29"
$script:MongoDataRoot = if ($script:DevProfile -eq "diy") {
    Join-Path $script:DevRoot "data\mongodb"
} else {
    Join-Path $script:DevRoot "data\mongodb-$script:DevProfile"
}
$script:MongoLogRoot = if ($script:DevProfile -eq "diy") {
    Join-Path $script:DevRoot "logs"
} else {
    Join-Path $script:DevRoot "logs\$script:DevProfile"
}
$script:MongoPort = if ($env:GWENT_DEV_MONGO_PORT) { [int]$env:GWENT_DEV_MONGO_PORT } else { 28020 }
$script:ServerPort = if ($env:GWENT_DEV_SERVER_PORT) { [int]$env:GWENT_DEV_SERVER_PORT } else { 5005 }
$script:MongoDatabase = if ($env:GWENT_DEV_DATABASE) { $env:GWENT_DEV_DATABASE } else { "gwent-diy" }
$script:ServerProjectRoot = Join-Path $script:RepoRoot "src\Cynthia.Card\src\Cynthia.Card.Server"
$script:ServerProject = Join-Path $script:ServerProjectRoot "Cynthia.Card.Server.csproj"
$script:ServerDll = Join-Path $script:ServerProjectRoot "bin\Debug\net10.0\Cynthia.Card.Server.dll"
$script:ServerPidFile = Join-Path $script:DevRoot "card-$script:DevProfile-dev.pid"
$script:CommonProjectRoot = Join-Path $script:RepoRoot "src\Cynthia.Card\src\Cynthia.Card.Common"
$script:CommonProject = Join-Path $script:CommonProjectRoot "Cynthia.Card.Common.csproj"
$script:CommonDll = Join-Path $script:CommonProjectRoot "bin\Debug\netstandard2.0\Cynthia.Card.Common.dll"
$script:UnityProjectRoot = Join-Path $script:RepoRoot "src\Cynthia.Card.Unity\src\Cynthia.Unity.Card"
$script:UnityCommonDll = Join-Path $script:UnityProjectRoot "Assets\Assemblies\Cynthia.Card.Common.dll"
$script:UnityEditorRoot = Join-Path $script:DevRoot "Unity\2019.4.1f1"
$script:UnityExe = Join-Path $script:UnityEditorRoot "Editor\Unity.exe"

function Test-DotNetServerSdk {
    if (-not (Test-Path -LiteralPath $script:DotNetExe)) {
        return $false
    }

    $installedSdks = @(& $script:DotNetExe --list-sdks 2>$null)
    return [bool]($installedSdks | Where-Object { $_ -match '^10\.0\.' } | Select-Object -First 1)
}

function Get-MongoExecutable {
    $path = Get-ChildItem -LiteralPath $script:MongoRoot -Filter "mongod.exe" -File -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
    if (-not $path) {
        throw "MongoDB is missing. Run scripts\setup-dev.ps1 first."
    }
    return $path
}

function Get-MongoShellExecutable {
    return Get-ChildItem -LiteralPath $script:MongoRoot -Filter "mongo.exe" -File -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
}

function Get-ListeningProcessId {
    param([Parameter(Mandatory = $true)][int]$Port)

    return Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty OwningProcess
}

function Sync-UnityCommonAssembly {
    Write-Host "Building Cynthia.Card.Common for the Unity client..."
    & $script:DotNetExe build $script:CommonProject --configuration Debug --nologo
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $script:CommonDll)) {
        throw "Cynthia.Card.Common build failed with exit code $LASTEXITCODE."
    }

    Copy-Item -LiteralPath $script:CommonDll -Destination $script:UnityCommonDll -Force
    Write-Host "Synced Cynthia.Card.Common.dll into Unity Assets\Assemblies."
}

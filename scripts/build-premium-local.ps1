param(
    [ValidateSet('Both', 'Windows', 'Android')][string]$Target = 'Both',
    [string]$OutputRoot,
    [switch]$CheckSourcesOnly,
    [switch]$Background
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'dev-common.ps1')

function Join-ProcessArguments {
    param([string[]]$Values)
    if (@($Values | Where-Object { $_.Contains('"') }).Count -gt 0) {
        throw 'Build arguments cannot contain double quotes.'
    }
    return ($Values | ForEach-Object { '"' + $_ + '"' }) -join ' '
}

if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $script:DevRoot ('builds\premium-local\' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
}
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)

if ($Background) {
    if ($CheckSourcesOnly) { throw 'Use -CheckSourcesOnly in the foreground; it does not launch Unity.' }
    New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
    $hostExe = (Get-Process -Id $PID).Path
    $launcherArgs = @('-NoProfile', '-File', $PSCommandPath, '-Target', $Target, '-OutputRoot', $OutputRoot)
    if ([IO.Path]::GetFileName($hostExe) -ieq 'powershell.exe') {
        $launcherArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $PSCommandPath,
            '-Target', $Target, '-OutputRoot', $OutputRoot)
    }
    $process = Start-Process -FilePath $hostExe -ArgumentList (Join-ProcessArguments $launcherArgs) -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput (Join-Path $OutputRoot 'driver.stdout.log') `
        -RedirectStandardError (Join-Path $OutputRoot 'driver.stderr.log')
    Write-Host "Started local premium build PID $($process.Id). Logs: $OutputRoot"
    return
}

$catalogPath = Join-Path $script:UnityProjectRoot 'Assets\DynamicCards\Content\catalog.json'
$manifestPath = Join-Path $script:RepoRoot 'build-config\premium-content.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$catalog = Get-Content -LiteralPath $catalogPath -Raw | ConvertFrom-Json
$actualCatalogHash = (Get-FileHash -LiteralPath $catalogPath -Algorithm SHA256).Hash.ToLowerInvariant()
if ($actualCatalogHash -ne $manifest.catalogSha256 -or $catalog.cards.Count -ne $manifest.cards) {
    throw 'Premium catalog differs from the pinned source manifest.'
}
$missing = @($catalog.cards | ForEach-Object { $_.prefab; $_.audio } |
    Where-Object { $_ -and -not (Test-Path -LiteralPath (Join-Path $script:UnityProjectRoot $_) -PathType Leaf) } |
    Select-Object -First 3)
if ($missing.Count -gt 0) {
    throw "Premium sources are not restored. First missing: $($missing -join ', ')."
}
if ($CheckSourcesOnly) {
    Write-Host "Verified pinned premium catalog and $($catalog.cards.Count) card source paths. No Unity process started."
    return
}
if (-not (Test-Path -LiteralPath $script:UnityExe -PathType Leaf)) {
    throw "Unity $script:UnityVersion is missing at $script:UnityExe. Install the tracked editor before building."
}

$settings = Get-Content -LiteralPath (Join-Path $script:UnityProjectRoot 'ProjectSettings\ProjectSettings.asset') -Raw
if ($settings -notmatch '(?m)^\s*bundleVersion:\s*(\d+\.\d+\.\d+)\s*$') {
    throw 'Cannot read Unity client bundleVersion.'
}
$clientVersion = $Matches[1]
if ($settings -notmatch '(?m)^\s*AndroidBundleVersionCode:\s*(\d+)\s*$') {
    throw 'Cannot read AndroidBundleVersionCode.'
}
$androidCode = $Matches[1]
$components = $clientVersion.Split('.') | ForEach-Object { [int]$_ }
$computedCode = $components[0] * 1000000 + $components[1] * 1000 + $components[2]
if ([int]$androidCode -ne $computedCode) { throw 'Tracked Android version code does not match bundleVersion.' }

$python = Get-Command python -ErrorAction Stop | Select-Object -ExpandProperty Source
Sync-UnityCommonAssembly
New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null

$androidSigning = $null
if ($Target -in @('Both', 'Android')) {
    $androidSupport = Join-Path $script:UnityEditorRoot 'Editor\Data\PlaybackEngines\AndroidPlayer'
    foreach ($component in @('SDK', 'NDK', 'OpenJDK')) {
        if (-not (Test-Path -LiteralPath (Join-Path $androidSupport $component) -PathType Container)) {
            throw "Unity Android Build Support is incomplete: missing $component under $androidSupport."
        }
    }
    $keytool = Join-Path $androidSupport 'OpenJDK\bin\keytool.exe'
    if (-not (Test-Path -LiteralPath $keytool -PathType Leaf)) {
        $keytool = Get-Command keytool -ErrorAction Stop | Select-Object -ExpandProperty Source
    }
    $keyPath = if ($env:GWENT_ANDROID_DEBUG_KEYSTORE) { $env:GWENT_ANDROID_DEBUG_KEYSTORE } else { Join-Path $env:USERPROFILE '.android\debug.keystore' }
    if (-not (Test-Path -LiteralPath $keyPath -PathType Leaf)) {
        $keyPath = Join-Path $script:DevRoot 'keys\premium-debug.keystore'
        if (-not (Test-Path -LiteralPath $keyPath -PathType Leaf)) {
            New-Item -ItemType Directory -Force -Path (Split-Path -Parent $keyPath) | Out-Null
            & $keytool -genkeypair -noprompt -storetype JKS -keystore $keyPath -storepass android `
                -alias androiddebugkey -keypass android -keyalg RSA -keysize 2048 -validity 10000 `
                -dname 'CN=Android Debug,O=Android,C=US' 1>$null
            if ($LASTEXITCODE -ne 0) { throw 'Could not generate local Android debug keystore.' }
        }
    }
    & $keytool -list -keystore $keyPath -storepass android -alias androiddebugkey 1>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Debug keystore $keyPath is not readable with the conventional android password and alias."
    }
    $androidSigning = $keyPath
}

$targets = if ($Target -eq 'Both') { @('StandaloneWindows64', 'Android') }
    elseif ($Target -eq 'Windows') { @('StandaloneWindows64') } else { @('Android') }
foreach ($buildTarget in $targets) {
    $targetDir = Join-Path $OutputRoot $buildTarget
    New-Item -ItemType Directory -Force -Path $targetDir | Out-Null
    $buildPath = if ($buildTarget -eq 'Android') {
        Join-Path $targetDir "DiyGwent-AITest-Android-$clientVersion-premium-test.apk"
    } else {
        Join-Path $targetDir "DiyGwent-AITest-Windows-$clientVersion-premium.exe"
    }
    $unityLog = Join-Path $targetDir 'unity-build.log'
    if (Test-Path -LiteralPath $buildPath) {
        throw "Build output already exists; choose a fresh OutputRoot: $buildPath"
    }
    $arguments = @('-batchmode', '-quit', '-projectPath', $script:UnityProjectRoot,
        '-buildTarget', $buildTarget, '-executeMethod', 'LegacyClientBuild.Build',
        '-clientVariant', 'premium', '-buildVersion', $clientVersion,
        '-customBuildPath', $buildPath, '-logFile', $unityLog)
    if ($buildTarget -eq 'Android') {
        $arguments += @('-androidVersionCode', $androidCode, '-androidKeystoreName', $androidSigning,
            '-androidKeystorePass', 'android', '-androidKeyaliasName', 'androiddebugkey',
            '-androidKeyaliasPass', 'android')
    }
    Write-Host "Building $buildTarget premium client with Unity $script:UnityVersion. Log: $unityLog"
    $process = Start-Process -FilePath $script:UnityExe -ArgumentList (Join-ProcessArguments $arguments) -WorkingDirectory $script:UnityProjectRoot `
        -WindowStyle Hidden -PassThru
    $process.WaitForExit()
    if ($process.ExitCode -ne 0) { throw "Unity $buildTarget build failed with exit code $($process.ExitCode). See $unityLog" }
    if (-not (Test-Path -LiteralPath $buildPath -PathType Leaf)) { throw "Unity did not create $buildPath" }
    $artifact = if ($buildTarget -eq 'Android') { $buildPath } else { $targetDir }
    & $python (Join-Path $script:RepoRoot 'scripts\verify-client-content.py') $artifact --variant premium --target $buildTarget
    if ($LASTEXITCODE -ne 0) { throw "$buildTarget content verification failed. See $unityLog" }
    $versionFile = Join-Path $targetDir 'version.txt'
    if (-not (Test-Path -LiteralPath $versionFile -PathType Leaf) -or
        (Get-Content -LiteralPath $versionFile -Raw).Trim() -ne $clientVersion) {
        throw "$buildTarget version.txt does not match $clientVersion."
    }
    Write-Host "Verified $buildTarget premium player: $buildPath"
}

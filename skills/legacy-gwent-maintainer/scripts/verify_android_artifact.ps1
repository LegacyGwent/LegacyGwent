[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ApkPath,
    [string]$ExpectedVersion = '2.1.9',
    [int]$ExpectedVersionCode = 2001009,
    [string]$ExpectedPackage = 'cynthia.diy.ai.card',
    [string]$ExpectedLabel = 'DiyGwent AITest',
    [string]$ExpectedEndpoint = 'http://106.15.38.165:5010',
    [string]$ExpectedAbi = 'armeabi-v7a',
    [string]$AndroidSdkRoot
)

$ErrorActionPreference = 'Stop'

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) {
        throw $Message
    }
}

function Invoke-Checked {
    param([string]$FilePath, [string[]]$Arguments)
    $output = @(& $FilePath @Arguments 2>&1 | ForEach-Object { "$_" })
    if ($LASTEXITCODE -ne 0) {
        throw "$FilePath failed with exit code $LASTEXITCODE`n$($output -join "`n")"
    }
    return $output
}

function Test-ByteSequence {
    param([byte[]]$Data, [byte[]]$Needle)
    if ($Needle.Length -eq 0 -or $Data.Length -lt $Needle.Length) {
        return $false
    }
    for ($index = 0; $index -le $Data.Length - $Needle.Length; $index++) {
        $matched = $true
        for ($offset = 0; $offset -lt $Needle.Length; $offset++) {
            if ($Data[$index + $offset] -ne $Needle[$offset]) {
                $matched = $false
                break
            }
        }
        if ($matched) {
            return $true
        }
    }
    return $false
}

$apk = (Resolve-Path -LiteralPath $ApkPath).Path
Assert-True ((Get-Item -LiteralPath $apk).Length -gt 0) 'APK is empty.'

if ([string]::IsNullOrWhiteSpace($AndroidSdkRoot)) {
    $sdkCandidates = @(
        $env:ANDROID_SDK_ROOT,
        $env:ANDROID_HOME,
        $(if ($env:LOCALAPPDATA) { Join-Path $env:LOCALAPPDATA 'Android\Sdk' })
    ) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }
    $AndroidSdkRoot = $sdkCandidates | Select-Object -First 1
}
Assert-True (-not [string]::IsNullOrWhiteSpace($AndroidSdkRoot)) 'Android SDK was not found.'

$buildTools = Get-ChildItem -LiteralPath (Join-Path $AndroidSdkRoot 'build-tools') -Directory |
    Sort-Object { try { [version]$_.Name } catch { [version]'0.0' } } -Descending |
    Select-Object -First 1
Assert-True ($null -ne $buildTools) 'Android build-tools were not found.'

$aapt = Join-Path $buildTools.FullName 'aapt.exe'
$apksigner = Join-Path $buildTools.FullName 'apksigner.bat'
$sevenZip = (Get-Command 7z -ErrorAction Stop).Source
Assert-True (Test-Path -LiteralPath $aapt) 'aapt.exe was not found.'
Assert-True (Test-Path -LiteralPath $apksigner) 'apksigner.bat was not found.'

$badging = Invoke-Checked $aapt @('dump', 'badging', $apk)
$badgingText = $badging -join "`n"
$packageLine = $badging | Where-Object { $_ -match '^package:' } | Select-Object -First 1
Assert-True ($packageLine -like "*name='$ExpectedPackage'*") 'Unexpected Android package ID.'
Assert-True ($packageLine -like "*versionCode='$ExpectedVersionCode'*") 'Unexpected Android versionCode.'
Assert-True ($packageLine -like "*versionName='$ExpectedVersion'*") 'Unexpected Android versionName.'
Assert-True ($badgingText.Contains("application-label:'$ExpectedLabel'")) 'Unexpected Android label.'
Assert-True ($badgingText.Contains("uses-permission: name='android.permission.INTERNET'")) 'INTERNET permission is missing.'

$nativeLine = $badging | Where-Object { $_ -match '^native-code:' } | Select-Object -First 1
$nativeAbis = @([regex]::Matches("$nativeLine", "'([^']+)'") | ForEach-Object { $_.Groups[1].Value })
Assert-True ($nativeAbis.Count -eq 1 -and $nativeAbis[0] -eq $ExpectedAbi) "Unexpected native ABIs: $($nativeAbis -join ', ')."

$manifest = Invoke-Checked $aapt @('dump', 'xmltree', $apk, 'AndroidManifest.xml')
$manifestText = $manifest -join "`n"
Assert-True ($manifestText -match 'android:usesCleartextTraffic.*(0xffffffff|true)') 'Cleartext HTTP opt-in is missing.'

$signature = Invoke-Checked $apksigner @('verify', '--verbose', '--print-certs', $apk)
$signatureText = $signature -join "`n"
Assert-True ($signatureText -match '(?m)^Verifies\s*$') 'apksigner did not report Verifies.'
Assert-True ($signatureText -match '(?m)^Verified using v1 scheme \(JAR signing\): true\s*$') 'APK v1 signature is invalid.'
Assert-True ($signatureText -match '(?m)^Verified using v2 scheme \(APK Signature Scheme v2\): true\s*$') 'APK v2 signature is invalid.'
Assert-True ($signatureText -match '(?m)^Number of signers: [1-9][0-9]*\s*$') 'APK signer count is invalid.'
$digestLine = $signature | Where-Object { $_ -match 'certificate SHA-256 digest:' } | Select-Object -First 1
$certificateDigest = (($digestLine -replace '^.*certificate SHA-256 digest:\s*', '') -replace '[:\s]', '').ToLowerInvariant()
Assert-True ($certificateDigest -match '^[0-9a-f]{64}$') 'APK certificate digest is malformed.'

$zipList = Invoke-Checked $sevenZip @('l', '-slt', $apk)
$zipAbis = @($zipList | ForEach-Object {
    if ($_ -match '^Path = lib[\\/]([^\\/]+)[\\/]') { $Matches[1] }
} | Sort-Object -Unique)
Assert-True ($zipAbis.Count -eq 1 -and $zipAbis[0] -eq $ExpectedAbi) "ZIP native ABIs differ: $($zipAbis -join ', ')."

$tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$tempDir = Join-Path $tempRoot "legacy-gwent-apk-$([guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $tempDir | Out-Null
try {
    Invoke-Checked $sevenZip @('e', '-y', "-o$tempDir", $apk, 'assets/bin/Data/Managed/Assembly-CSharp.dll') | Out-Null
    $assemblyPath = Join-Path $tempDir 'Assembly-CSharp.dll'
    Assert-True (Test-Path -LiteralPath $assemblyPath) 'Assembly-CSharp.dll was not found in the APK.'
    $assemblyBytes = [IO.File]::ReadAllBytes($assemblyPath)
    $markers = @($ExpectedEndpoint, 'ServerEndpoint', 'DiyAi.TextLanguage', 'DiyAi.AudioLanguage', 'cn')
    foreach ($marker in $markers) {
        $utf8 = [Text.Encoding]::UTF8.GetBytes($marker)
        $utf16 = [Text.Encoding]::Unicode.GetBytes($marker)
        Assert-True ((Test-ByteSequence $assemblyBytes $utf8) -or (Test-ByteSequence $assemblyBytes $utf16)) "Missing embedded marker: $marker"
    }
}
finally {
    $resolvedTemp = [IO.Path]::GetFullPath($tempDir)
    if ($resolvedTemp.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase)) {
        Remove-Item -LiteralPath $resolvedTemp -Recurse -Force
    }
}

[pscustomobject]@{
    Apk = $apk
    Size = (Get-Item -LiteralPath $apk).Length
    Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $apk).Hash
    Package = $ExpectedPackage
    Version = $ExpectedVersion
    VersionCode = $ExpectedVersionCode
    Label = $ExpectedLabel
    Abis = $nativeAbis
    CertificateSha256 = $certificateDigest
    EndpointMarker = $ExpectedEndpoint
    ChineseDefaultMarkers = @('DiyAi.TextLanguage', 'DiyAi.AudioLanguage', 'cn')
}

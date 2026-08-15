[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$commonRoot = Join-Path $repoRoot 'src\Cynthia.Card\src\Cynthia.Card.Common'
$unityAssets = Join-Path $repoRoot 'src\Cynthia.Card.Unity\src\Cynthia.Unity.Card\Assets'
$unityAddressables = Join-Path $unityAssets 'Addressables'
$mapPath = Join-Path $commonRoot 'GwentGame\GwentMap.cs'
$effectsPath = Join-Path $commonRoot 'CardEffects'
$artPath = Join-Path $unityAddressables 'Cards'
$fullArtGroupPath = Join-Path $unityAssets 'AddressableAssetsData\AssetGroups\Default Local Group.asset'
$miniatureGroupPath = Join-Path $unityAssets 'AddressableAssetsData\AssetGroups\Miniatures.asset'
$webPreviewPath = Join-Path $repoRoot 'src\Cynthia.Card\src\Cynthia.Card.Server\wwwroot\scale'
$imageExtensions = @('.png', '.jpg', '.jpeg', '.tga')

$artIds = @(Get-ChildItem -LiteralPath $artPath -File |
    Where-Object { $imageExtensions -contains $_.Extension.ToLowerInvariant() } |
    ForEach-Object BaseName |
    Sort-Object -Unique)
$miniatureIds = @(Select-String -LiteralPath $miniatureGroupPath -Pattern '^\s+m_Address:\s*(.+)$' |
    ForEach-Object { $_.Matches[0].Groups[1].Value.Trim() -replace '_slot$', '' } |
    Sort-Object -Unique)
$fullArtAddressIds = @(Select-String -LiteralPath $fullArtGroupPath -Pattern '^\s+m_Address:\s*(.+)$' |
    ForEach-Object { $_.Matches[0].Groups[1].Value.Trim() } |
    Where-Object { $_ -match '^[A-Za-z0-9]+$' } |
    Sort-Object -Unique)
$webPreviewIds = @(Get-ChildItem -LiteralPath $webPreviewPath -File -Filter '*.png' |
    ForEach-Object BaseName |
    Sort-Object -Unique)

$lines = Get-Content -LiteralPath $mapPath
$entries = @()
for ($lineIndex = 0; $lineIndex -lt $lines.Count - 1; $lineIndex++) {
    if ($lines[$lineIndex] -notmatch '^\s*"([^"]+)"\s*,') {
        continue
    }

    $key = $Matches[1]
    $nextLine = $lineIndex + 1
    while ($nextLine -lt $lines.Count -and [string]::IsNullOrWhiteSpace($lines[$nextLine])) {
        $nextLine++
    }
    if ($nextLine -ge $lines.Count -or $lines[$nextLine].Trim() -ne 'new GwentCard()') {
        continue
    }

    $artId = $null
    $cardIdProperty = $null
    for ($entryLine = $nextLine + 1; $entryLine -lt [Math]::Min($lines.Count, $nextLine + 50); $entryLine++) {
        if ($lines[$entryLine] -match 'CardId\s*=\s*"([^"]+)"') {
            $cardIdProperty = $Matches[1]
        }
        if ($lines[$entryLine] -match 'CardArtsId\s*=\s*"([^"]+)"') {
            $artId = $Matches[1]
            break
        }
    }

    $entries += [pscustomobject]@{
        Key = $key
        CardIdProperty = $cardIdProperty
        ArtId = $artId
        Line = $lineIndex + 1
    }
}

$effectSource = @(Get-ChildItem -LiteralPath $effectsPath -Recurse -Filter '*.cs' |
    ForEach-Object { Get-Content -Raw -LiteralPath $_.FullName }) -join [Environment]::NewLine
$effectIds = @([regex]::Matches($effectSource, 'CardEffectId\s*\(\s*"([^"]+)"\s*\)') |
    ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -ne 'None' } |
    Sort-Object -Unique)

$artSet = [Collections.Generic.HashSet[string]]::new([string[]]$artIds)
$miniatureSet = [Collections.Generic.HashSet[string]]::new([string[]]$miniatureIds)
$fullArtAddressSet = [Collections.Generic.HashSet[string]]::new([string[]]$fullArtAddressIds)
$effectSet = [Collections.Generic.HashSet[string]]::new([string[]]$effectIds)
$mappedArtSet = [Collections.Generic.HashSet[string]]::new(
    [string[]]@($entries.ArtId | Where-Object { $_ } | Sort-Object -Unique))
$effectArtSet = [Collections.Generic.HashSet[string]]::new()
foreach ($entry in $entries) {
    if ($effectSet.Contains($entry.Key)) {
        [void]$effectArtSet.Add($entry.ArtId)
    }
}

$unassignedArt = @($artSet | Where-Object { -not $mappedArtSet.Contains($_) })
$definedWithoutEffectArt = @($mappedArtSet | Where-Object { -not $effectArtSet.Contains($_) })
$entriesWithoutEffect = @($entries | Where-Object { -not $effectSet.Contains($_.Key) })
$keyPropertyMismatches = @($entries |
    Where-Object { $_.CardIdProperty -and $_.Key -ne $_.CardIdProperty })
$unassignedWebPreviews = @($webPreviewIds | Where-Object { -not $mappedArtSet.Contains($_) })
$fullArtMissingAddressable = @($artIds | Where-Object { -not $fullArtAddressSet.Contains($_) })
$addressableMissingFullArt = @($fullArtAddressIds | Where-Object { -not $artSet.Contains($_) })
$mappedArtMissingFullArt = @($mappedArtSet | Where-Object { -not $artSet.Contains($_) })
$mappedArtMissingMiniature = @($mappedArtSet | Where-Object { -not $miniatureSet.Contains($_) })
$webPreviewMissingFullArt = @($webPreviewIds | Where-Object { -not $artSet.Contains($_) })

[pscustomobject]@{
    FullArtAssets = $artSet.Count
    FullArtAddressableEntries = $fullArtAddressSet.Count
    FullArtMissingAddressable = $fullArtMissingAddressable.Count
    FullArtMissingAddressableIds = $fullArtMissingAddressable
    AddressableEntriesMissingFullArt = $addressableMissingFullArt.Count
    AddressableEntriesMissingFullArtIds = $addressableMissingFullArt
    CardMapEntries = $entries.Count
    ExplicitCardEffectIds = $effectSet.Count
    UniqueArtAssignedToCards = $mappedArtSet.Count
    MappedArtMissingFullArt = $mappedArtMissingFullArt.Count
    MappedArtMissingFullArtIds = $mappedArtMissingFullArt
    MappedArtMissingMiniature = $mappedArtMissingMiniature.Count
    MappedArtMissingMiniatureIds = $mappedArtMissingMiniature
    UniqueArtUsedByEffects = $effectArtSet.Count
    UniqueArtDefinedWithoutEffect = $definedWithoutEffectArt.Count
    UnassignedArt = $unassignedArt.Count
    ArtWithoutExplicitEffect = $artSet.Count - $effectArtSet.Count
    UnassignedArtWithMiniature = @($unassignedArt | Where-Object { $miniatureSet.Contains($_) }).Count
    UnassignedArtMissingMiniature = @($unassignedArt | Where-Object { -not $miniatureSet.Contains($_) }).Count
    WebPreviewArt = $webPreviewIds.Count
    WebPreviewArtAssignedToCards = @($webPreviewIds | Where-Object { $mappedArtSet.Contains($_) }).Count
    UnassignedWebPreviewArt = $unassignedWebPreviews.Count
    UnassignedWebPreviewWithFullArt = @($unassignedWebPreviews | Where-Object { $artSet.Contains($_) }).Count
    UnassignedWebPreviewMissingFullArt = @($unassignedWebPreviews | Where-Object { -not $artSet.Contains($_) }).Count
    WebPreviewMissingFullArt = $webPreviewMissingFullArt.Count
    WebPreviewMissingFullArtIds = $webPreviewMissingFullArt
    UnassignedWebPreviewWithMiniature = @($unassignedWebPreviews | Where-Object { $miniatureSet.Contains($_) }).Count
    CardEntriesWithoutEffect = $entriesWithoutEffect.Count
    CardKeyPropertyMismatches = $keyPropertyMismatches.Count
}

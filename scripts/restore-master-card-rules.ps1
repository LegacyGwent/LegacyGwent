[CmdletBinding()]
param(
    [string]$MasterRef = "origin/master",
    [string]$DiyAiRef = "origin/diy-ai"
)

$ErrorActionPreference = "Stop"

$repoRoot = (git rev-parse --show-toplevel).Trim()
if (-not $repoRoot) {
    throw "Run this script inside the LegacyGwent repository."
}

$mapPath = "src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/GwentMap.cs"
$absoluteMapPath = Join-Path $repoRoot $mapPath
$currentText = [IO.File]::ReadAllText($absoluteMapPath)
$masterText = (git show "${MasterRef}:$mapPath") -join "`n"
$masterText = $masterText.Replace("`n", "`r`n")
$diyAiText = (git show "${DiyAiRef}:$mapPath") -join "`n"
$diyAiText = $diyAiText.Replace("`n", "`r`n")

$entryPattern = [regex]'(?m)^(?<open> {12}\{\r?\n)(?<key> {16}"(?<id>\d+)"\s*,)'

function Get-CardEntries([string]$text) {
    $matches = $entryPattern.Matches($text)
    $result = @{}
    for ($i = 0; $i -lt $matches.Count; $i++) {
        $start = $matches[$i].Index
        $end = if ($i + 1 -lt $matches.Count) { $matches[$i + 1].Index } else { $text.LastIndexOf("        };", [StringComparison]::Ordinal) }
        if ($end -le $start) {
            throw "Unable to determine card entry boundary at index $start."
        }
        $id = $matches[$i].Groups['id'].Value
        $result[$id] = [pscustomobject]@{
            Id = $id
            Start = $start
            Length = $end - $start
            Text = $text.Substring($start, $end - $start)
        }
    }
    return $result
}

function Get-SemanticSignature([string]$block) {
    $properties = @(
        'CardId', 'Name', 'Strength', 'Group', 'Faction', 'CardUseInfo', 'CardType',
        'IsDoomed', 'Countdown', 'IsCountdown', 'IsDerive', 'Categories', 'HideTags',
        'CrewCount', 'IsConcealCard', 'Flavor', 'Info', 'CardArtsId'
    )
    $values = foreach ($property in $properties) {
        $match = [regex]::Match($block, "(?m)^\s*$property\s*=\s*(?<value>.*?)(?:,\s*)?$")
        $value = if ($match.Success) { $match.Groups['value'].Value.Trim() } else { '<missing>' }
        $value = [regex]::Replace($value, '//.*$', '')
        $value = $value.Trim().TrimEnd(',')
        if (-not $value.StartsWith('"')) {
            $value = [regex]::Replace($value, '\s+', '')
        }
        "$property=$value"
    }
    return $values -join "`n"
}

function Get-LinkedCards([string]$block) {
    $match = [regex]::Match($block, 'LinkedCards\s*=\s*new List<String>\s*\{(?<ids>[^}]*)\}')
    if (-not $match.Success) {
        return @()
    }
    return [regex]::Matches($match.Groups['ids'].Value, '"(?<id>\d+)"') | ForEach-Object { $_.Groups['id'].Value }
}

function Add-LinkedCards([string]$masterBlock, [string[]]$linkedCards) {
    $masterBlock = [regex]::Replace(
        $masterBlock,
        '(?m)^\s*LinkedCards\s*=\s*new List<String>\s*\{[^}]*\}\s*,?\r?\n',
        '')
    $line = '                    LinkedCards=new List<String> {' + (($linkedCards | ForEach-Object { '"' + $_ + '"' }) -join ',') + '},'
    $closing = [regex]'\r?\n {16}\}\r?\n {12}\},\r?\n?$'
    if (-not $closing.IsMatch($masterBlock)) {
        throw "Unable to locate GwentCard initializer closing lines."
    }
    return $closing.Replace($masterBlock, "`r`n$line`r`n                }`r`n            },`r`n")
}

$currentEntries = Get-CardEntries $currentText
$masterEntries = Get-CardEntries $masterText
$diyAiEntries = Get-CardEntries $diyAiText

$protectedOriginalCards = [Collections.Generic.HashSet[string]]::new([string[]]@(
    '12032', '12004', '12012', '34007', '12034', '24016', '24005',
    '22004', '24015', '22009', '23011', '24032', '12036', '44021',
    # Gold, silver and copper weather cards intentionally retain DIY-AI rules.
    '14005', '14011', '14019', '14026', '12033', '12009', '13043', '13035', '53020'
))
$masterIds = [Collections.Generic.HashSet[string]]::new([string[]]$masterEntries.Keys)
$targets = @(
    $masterEntries.Keys |
        Where-Object {
            $currentEntries.ContainsKey($_) -and
            -not $protectedOriginalCards.Contains($_) -and
            (Get-SemanticSignature $diyAiEntries[$_].Text) -ne (Get-SemanticSignature $masterEntries[$_].Text)
        } |
        Sort-Object
)

if ($targets.Count -lt 80 -or $targets.Count -gt 96) {
    throw "Unexpected rollback scope: $($targets.Count) ordinary original card definitions."
}

$replacementIds = @($targets + $protectedOriginalCards | Sort-Object -Unique)
$replacements = foreach ($id in $replacementIds) {
    if (-not $currentEntries.ContainsKey($id) -or -not $diyAiEntries.ContainsKey($id)) {
        throw "Protected or rollback card $id is missing from the DIY-AI map."
    }
    $sourceBlock = if ($protectedOriginalCards.Contains($id)) { $diyAiEntries[$id].Text } else { $masterEntries[$id].Text }
    $linked = @(Get-LinkedCards $diyAiEntries[$id].Text | Where-Object { $masterIds.Contains($_) })
    [pscustomobject]@{
        Start = $currentEntries[$id].Start
        Length = $currentEntries[$id].Length
        Text = Add-LinkedCards $sourceBlock $linked
    }
}

foreach ($replacement in ($replacements | Sort-Object Start -Descending)) {
    $currentText = $currentText.Remove($replacement.Start, $replacement.Length).Insert($replacement.Start, $replacement.Text)
}

[IO.File]::WriteAllText($absoluteMapPath, $currentText, [Text.UTF8Encoding]::new($false))
Write-Host "Restored $($targets.Count) ordinary card definitions from $MasterRef; preserved AI and weather rules from $DiyAiRef."

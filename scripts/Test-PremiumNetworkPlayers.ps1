param([ValidateSet(1,2)][int]$Round=1,[ValidatePattern('^[A-Za-z0-9_-]+$')][string]$RunId='')
$ErrorActionPreference='Stop'
$work=Join-Path $PSScriptRoot '../work/PremiumNetwork'
$exe=[IO.Path]::GetFullPath((Join-Path $work 'Client/PremiumNetwork.exe'))
if($RunId){$work=Join-Path $work ('runs/'+$RunId)}
if(!(Test-Path -LiteralPath $exe)){throw 'Build the development test client first.'}
$folders=@()
foreach($side in @('a','b')) {
    $folder=Join-Path $work "round$Round-$side"
    if(Test-Path -LiteralPath (Join-Path $folder 'samples.json')){throw "Existing evidence must be retained: $folder"}
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    $log=[IO.Path]::GetFullPath((Join-Path $folder 'player.log'))
    $playerArguments=@("-premium-network-$side","-premium-network-round=$Round",'-screen-width','1440','-screen-height','900','-screen-fullscreen','0','-logFile',('"'+$log+'"'))
    if($RunId){$playerArguments+="-premium-network-run=$RunId"}
    $process=Start-Process -FilePath $exe -ArgumentList $playerArguments -WindowStyle Hidden -PassThru
    @{pid=$process.Id;exe=$exe} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $folder 'process.json')
    $folders+=$folder
}
function Wait-Stage([string]$filename,[int]$seconds) {
    $deadline=[DateTime]::UtcNow.AddSeconds($seconds)
    while(@($folders | Where-Object {!(Test-Path -LiteralPath (Join-Path $_ $filename))}).Count -gt 0) {
        foreach($folder in $folders) {if(Test-Path -LiteralPath (Join-Path $folder 'error.txt')){throw (Get-Content -Raw -LiteralPath (Join-Path $folder 'error.txt'))}}
        if([DateTime]::UtcNow -gt $deadline){throw "Timed out waiting for $filename"}
        Start-Sleep -Milliseconds 500
    }
}
Wait-Stage 'account.json' 80
foreach($folder in $folders) {
    $account=Get-Content -Raw -LiteralPath (Join-Path $folder 'account.json') | ConvertFrom-Json
    if($account.username -notin @('premium-network-a','premium-network-b')){throw 'Unexpected fixture account'}
    & "$PSScriptRoot/Grant-MeteoritePowder.ps1" -PlayerId $account.id -Amount 10000 -RewardId 'premium-network-fixture-20260913' -Reason 'Local two-client animation verification'
    Set-Content -LiteralPath (Join-Path $folder 'funded') -Value 'ready'
}
Wait-Stage 'prepared.json' 90
foreach($folder in $folders){
    Set-Content -LiteralPath (Join-Path $folder 'match') -Value 'ready'
    # The visual fixture enters one waiting room before its opponent joins.
    # Simultaneous room creation is a separate matchmaking concurrency scenario.
    Start-Sleep -Milliseconds 1500
}
Write-Output "Both independent clients have joined local round $Round."
Wait-Stage 'complete.json' 180
foreach($folder in $folders){Get-Content -LiteralPath (Join-Path $folder 'complete.json')}

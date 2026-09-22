param([string]$Endpoint='ws://127.0.0.1:5016/hub/gwent',[string]$OutputPath="$PSScriptRoot/../work/DailyQuests/hub-tests.json")
$ErrorActionPreference='Stop'
$ws=[Net.WebSockets.ClientWebSocket]::new()
$cts=[Threading.CancellationTokenSource]::new(45000)
$events=[Collections.Generic.List[object]]::new()
$checks=[Collections.Generic.List[string]]::new()
$script:nextId=0
function Send-Json($obj) {
    $bytes=[Text.Encoding]::UTF8.GetBytes(($obj|ConvertTo-Json -Depth 40 -Compress)+[char]30)
    $null=$ws.SendAsync([ArraySegment[byte]]::new($bytes),[Net.WebSockets.WebSocketMessageType]::Text,$true,$cts.Token).GetAwaiter().GetResult()
}
function Receive-Json {
    $stream=[IO.MemoryStream]::new(); $buffer=[byte[]]::new(131072)
    do {
        $part=$ws.ReceiveAsync([ArraySegment[byte]]::new($buffer),$cts.Token).GetAwaiter().GetResult()
        if($part.MessageType -eq [Net.WebSockets.WebSocketMessageType]::Close){throw 'Socket closed'}
        $stream.Write($buffer,0,$part.Count)
    } while(!$part.EndOfMessage)
    $value=[Text.Encoding]::UTF8.GetString($stream.ToArray());$stream.Dispose()
    foreach($line in $value.Split([char]30)) { if($line) { $obj=$line|ConvertFrom-Json; if($obj.target){$events.Add($obj)}; Write-Output $obj } }
}
function Invoke-Hub($target,$arguments) {
    $script:nextId++; $id=[string]$script:nextId
    Send-Json @{type=1; invocationId=$id; target=$target; arguments=$arguments}
    while($true) { foreach($msg in @(Receive-Json)) { if($msg.type -eq 3 -and $msg.invocationId -eq $id) { if($msg.error){throw $msg.error};return $msg.result } } }
}
function Check($condition,$label) { if(!$condition){throw $label};$checks.Add($label) }
try {
    $null=$ws.ConnectAsync([Uri]$Endpoint,$cts.Token).GetAwaiter().GetResult()
    Send-Json @{protocol='json';version=1};$null=Receive-Json
    Check ((Invoke-Hub 'GetDailyQuests' @()).status -eq 'unauthenticated') 'unauthenticated daily access rejected'
    $name='daily-hub-'+[guid]::NewGuid().ToString('N').Substring(0,12)
    Check (Invoke-Hub 'Register' @($name,'DailyHubTest2026',$name)) 'isolated local account registered'
    $user=Invoke-Hub 'Login' @($name,'DailyHubTest2026')
    $deadline=[DateTime]::UtcNow.AddSeconds(10)
    do {$wallet=Invoke-Hub 'GetPremiumCollection' @();if($wallet.collection.meteoritePowder -ne 5020){Start-Sleep -Milliseconds 100}} while($wallet.collection.meteoritePowder -ne 5020 -and [DateTime]::UtcNow -lt $deadline)
    Check ($wallet.collection.meteoritePowder -eq 5020) 'server grants 5000 initial plus 20 daily powder without a client grant'
    $state=Invoke-Hub 'GetDailyQuests' @()
    Check ($state.wallet.collection.id -eq $user.id) 'daily data belongs to authenticated connection'
    Check ($state.dailyCap -eq 155 -and $state.wallet.collection.dailyQuests.crowns -eq 0) 'fresh progress and GG-inclusive approved cap'
    $remaining=([DateTimeOffset]::Parse($state.resetUtc)-[DateTimeOffset]::Parse($state.serverUtc)).TotalSeconds
    Check ($remaining -gt 0 -and $remaining -le 86400 -and ([DateTimeOffset]$state.resetUtc).UtcDateTime.Hour -eq 16) 'server supplies UTC time and China midnight boundary'
    1..5 | ForEach-Object {$null=Invoke-Hub 'GetDailyQuests' @()}
    Check ((Invoke-Hub 'GetPremiumCollection' @()).collection.meteoritePowder -eq 5020) 'repeated refresh cannot duplicate either grant'
    $rejected=$false
    try {$null=Invoke-Hub 'AwardDailyCrown' @($name,'forged',6)} catch {$rejected=$true}
    Check $rejected 'no public method accepts client crown awards'
    $rejected=$false
    try {$null=Invoke-Hub 'GetDailyQuests' @('another-user','2099-01-01')} catch {$rejected=$true}
    Check $rejected 'client cannot choose reward identity or date'
    $null=Invoke-Hub 'Login' @($name,'DailyHubTest2026')
    Check ((Invoke-Hub 'GetDailyQuests' @()).wallet.collection.meteoritePowder -eq 5020) 'same-day relogin preserves one login reward'
    $output=$OutputPath
    @{passed=$true;checks=$checks;playerId=$user.id}|ConvertTo-Json -Depth 6|Set-Content -LiteralPath $output
    Get-Content -LiteralPath $output
} finally {$ws.Dispose();$cts.Dispose()}

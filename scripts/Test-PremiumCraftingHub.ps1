param([string]$OutputPath = "$PSScriptRoot/../work/PremiumCrafting/hub-check.json",[string]$Endpoint='ws://127.0.0.1:5016/hub/gwent',[string]$MongoUri='mongodb://127.0.0.1:28121/gwentdiy')
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
    Check ((Invoke-Hub 'GetPremiumCollection' @()).status -eq 'unauthenticated') 'unauthenticated hub cannot access wallet'
    Check ((Invoke-Hub 'CraftPremium' @('14002')).status -eq 'unauthenticated') 'unauthenticated hub cannot craft'
    $name='premium-hub-'+[guid]::NewGuid().ToString('N').Substring(0,12)
    Check (Invoke-Hub 'Register' @($name,'PremiumHubTest2026',$name)) 'register isolated test account'
    $user=Invoke-Hub 'Login' @($name,'PremiumHubTest2026')
    $deadline=[DateTime]::UtcNow.AddSeconds(10)
    do {$account=Invoke-Hub 'GetPremiumCollection' @();if($account.collection.meteoritePowder -ne 3020){Start-Sleep -Milliseconds 100}} while($account.collection.meteoritePowder -ne 3020 -and [DateTime]::UtcNow -lt $deadline)
    $initialPowder=$account.collection.meteoritePowder
    Check ($account.collection.id -eq $user.id -and $initialPowder -eq 3020) 'wallet belongs to connection identity and includes initial plus daily rewards'
    $card='14002'
    $crafted=Invoke-Hub 'CraftPremium' @($card)
    Check ($crafted.status -eq 'ok' -and $crafted.collection.meteoritePowder -eq ($initialPowder-100)) 'hub atomically charges and unlocks'
    Check ((Invoke-Hub 'CraftPremium' @($card)).status -eq 'already_owned') 'hub retry does not duplicate charge'
    Check ((Invoke-Hub 'SelectPremium' @($card,0)).status -eq 'ok') 'standard version can be equipped'
    Check ((Invoke-Hub 'SelectPremium' @($card,1)).status -eq 'ok') 'owned premium can be equipped'
    Check ((Invoke-Hub 'SelectPremium' @('12001',1)).status -eq 'not_owned') 'unowned premium cannot be equipped'
    Check ((Invoke-Hub 'SelectPremium' @($card,2)).status -eq 'invalid_selection') 'invalid version flag is rejected'
    $relogin=Invoke-Hub 'Login' @($name,'PremiumHubTest2026')
    $again=Invoke-Hub 'GetPremiumCollection' @()
    Check ($again.collection.meteoritePowder -eq ($initialPowder-100) -and $again.collection.selectedCards -contains $card) 'relogin preserves balance and selected version'
    @{passed=$true;checks=$checks;playerId=$user.id;cardId=$card;balance=$again.collection.meteoritePowder} | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $OutputPath
    Get-Content -LiteralPath $OutputPath
} finally { $ws.Dispose();$cts.Dispose() }

param(
    [Parameter(Mandatory=$true)][string]$PlayerId,
    [Parameter(Mandatory=$true)][ValidateRange(1,1000000000)][long]$Amount,
    [Parameter(Mandatory=$true)][string]$RewardId,
    [Parameter(Mandatory=$true)][string]$Reason,
    [string]$MongoUri = 'mongodb://127.0.0.1:28121/gwentdiy',
    [string]$MongoShell = ''
)
$ErrorActionPreference = 'Stop'
# Windows PowerShell may evaluate parameter defaults before PSScriptRoot is available.
if ([string]::IsNullOrWhiteSpace($MongoShell)) {
    $bundled = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../work/LocalServer/mongodb/mongodb-win32-x86_64-2012plus-4.2.25/bin/mongo.exe'))
    if (![string]::IsNullOrWhiteSpace($env:MONGO_SHELL)) { $MongoShell = $env:MONGO_SHELL }
    elseif (Test-Path -LiteralPath $bundled -PathType Leaf) { $MongoShell = $bundled }
    else {
        $command = Get-Command mongosh,mongo -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($command) { $MongoShell = $command.Source }
    }
}
if (!(Test-Path -LiteralPath $MongoShell -PathType Leaf)) { throw "Mongo shell was not found: $MongoShell" }
if ([string]::IsNullOrWhiteSpace($RewardId) -or [string]::IsNullOrWhiteSpace($Reason)) { throw 'RewardId and Reason are required.' }
$payload = @{ playerId=$PlayerId; amount=$Amount; rewardId=$RewardId; reason=$Reason } | ConvertTo-Json -Compress
$scriptText = 'var reward = ' + $payload + @'
;
var user = db.user.findOne({_id: reward.playerId});
if (!user) { throw new Error('Player ID was not found; no powder granted.'); }
db.premium_collection.updateOne({_id: reward.playerId}, {$setOnInsert: {MeteoritePowder: NumberLong(0), Revision: NumberLong(0), OwnedCards: [], SelectedCards: [], Rewards: []}}, {upsert: true});
var result = db.premium_collection.updateOne({_id: reward.playerId, 'Rewards.RewardId': {$ne: reward.rewardId}, MeteoritePowder: {$lte: NumberLong('9007198254740991')}}, {
  $inc: {MeteoritePowder: NumberLong(reward.amount), Revision: NumberLong(1)},
  $push: {Rewards: {RewardId: reward.rewardId, Amount: NumberLong(reward.amount), Reason: reward.reason, GrantedUtc: new Date().toISOString()}}
});
var account = db.premium_collection.findOne({_id: reward.playerId});
printjson({playerId: reward.playerId, granted: result.modifiedCount === 1, rewardId: reward.rewardId, powder: account.MeteoritePowder});
'@
$grantFile = Join-Path ([IO.Path]::GetTempPath()) ('legacy-powder-' + [guid]::NewGuid().ToString('N') + '.js')
try {
    [IO.File]::WriteAllText($grantFile, $scriptText, [Text.UTF8Encoding]::new($false))
    & $MongoShell $MongoUri --quiet $grantFile
    if ($LASTEXITCODE -ne 0) { throw 'Powder grant failed.' }
} finally { Remove-Item -LiteralPath $grantFile -ErrorAction SilentlyContinue }

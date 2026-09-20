param([ValidatePattern('^[A-Za-z0-9_-]+$')][string]$RunId=(Get-Date -Format 'yyyyMMdd-HHmmss'))
$ErrorActionPreference='Stop'
$repo=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$work=Join-Path $repo ('work/PremiumNetwork/runs/'+$RunId)
New-Item -ItemType Directory -Path (Join-Path $work 'data') -Force | Out-Null
if(Get-NetTCPConnection -State Listen | Where-Object {$_.LocalPort -in 5016,28121}){throw 'Visual fixture ports already occupied'}
$mongoPath=Join-Path $repo 'work/LocalServer/mongodb/mongodb-win32-x86_64-2012plus-4.2.25/bin/mongod.exe'
$dotnet=(Get-Command dotnet -ErrorAction Stop).Source
$serverDir=Join-Path $repo 'src/Cynthia.Card/src/Cynthia.Card.Server'
$clientExe=Join-Path $repo 'work/PremiumNetwork/Client/PremiumNetwork.exe'
$mongo=$null;$server=$null
$oldMongo=$env:MONGO_CONNECTION_STRING;$oldAspNet=$env:ASPNETCORE_URLS
$oldGwent=$env:GWENT_SERVER_URL;$oldLookup=$env:DOTNET_MULTILEVEL_LOOKUP
function Stop-ClientFixtures {
    Get-ChildItem -LiteralPath $work -Recurse -Filter process.json | ForEach-Object {
        $info=Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json
        $p=Get-Process -Id $info.pid -ErrorAction SilentlyContinue
        if($p -and $p.Path -eq $clientExe){Stop-Process -Id $p.Id}
    }
}
try {
    $mongo=Start-Process $mongoPath -ArgumentList @('--bind_ip','127.0.0.1','--port','28121','--dbpath',('"'+(Join-Path $work 'data')+'"'),'--logpath',('"'+(Join-Path $work 'mongo.log')+'"')) -WindowStyle Hidden -PassThru
    $limit=[DateTime]::UtcNow.AddSeconds(30)
    while(!(Get-NetTCPConnection -State Listen -LocalPort 28121 -ErrorAction SilentlyContinue)){if($mongo.HasExited -or [DateTime]::UtcNow -gt $limit){throw 'Mongo startup failed'};Start-Sleep -Milliseconds 300}
    $env:MONGO_CONNECTION_STRING='mongodb://127.0.0.1:28121'
    $env:ASPNETCORE_URLS='http://127.0.0.1:5016'
    $env:GWENT_SERVER_URL='http://127.0.0.1:5016'
    $env:DOTNET_MULTILEVEL_LOOKUP='0'
    & $dotnet build (Join-Path $serverDir 'Cynthia.Card.Server.csproj') -c Release
    if($LASTEXITCODE -ne 0){throw 'Server build failed'}
    $server=Start-Process $dotnet -ArgumentList @(('"'+(Join-Path $serverDir 'bin/Release/net10.0/Cynthia.Card.Server.dll')+'"')) -WorkingDirectory $serverDir -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $work 'server.log') -RedirectStandardError (Join-Path $work 'server-errors.log')
    $limit=[DateTime]::UtcNow.AddSeconds(30)
    while(!(Get-NetTCPConnection -State Listen -LocalPort 5016 -ErrorAction SilentlyContinue)){if($server.HasExited -or [DateTime]::UtcNow -gt $limit){throw 'Server startup failed'};Start-Sleep -Milliseconds 300}
    foreach($round in 1,2){
        & (Join-Path $repo 'scripts/Test-PremiumNetworkPlayers.ps1') -Round $round -RunId $RunId
        Stop-ClientFixtures
    }
    & python (Join-Path $repo 'scripts/Verify-PremiumNetwork.py') $RunId
    if($LASTEXITCODE -ne 0){throw 'Two-client visual regression failed'}
} finally {
    Stop-ClientFixtures
    foreach($owned in @(@{p=$server;path=$dotnet},@{p=$mongo;path=$mongoPath})){
        if($owned.p){$p=Get-Process -Id $owned.p.Id -ErrorAction SilentlyContinue;if($p -and $p.Path -eq $owned.path){Stop-Process -Id $p.Id}}
    }
    $env:MONGO_CONNECTION_STRING=$oldMongo;$env:ASPNETCORE_URLS=$oldAspNet
    $env:GWENT_SERVER_URL=$oldGwent;$env:DOTNET_MULTILEVEL_LOOKUP=$oldLookup
}

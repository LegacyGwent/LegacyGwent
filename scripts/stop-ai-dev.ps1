param(
    [switch]$KeepMongoDB
)

$previous = @{
    Profile = $env:GWENT_DEV_PROFILE
    ServerPort = $env:GWENT_DEV_SERVER_PORT
    MongoPort = $env:GWENT_DEV_MONGO_PORT
    Database = $env:GWENT_DEV_DATABASE
}
try {
    $env:GWENT_DEV_PROFILE = "diy-ai"
    $env:GWENT_DEV_SERVER_PORT = "5010"
    $env:GWENT_DEV_MONGO_PORT = "28021"
    $env:GWENT_DEV_DATABASE = "gwent-diy-ai"
    & (Join-Path $PSScriptRoot "stop-dev.ps1") -KeepMongoDB:$KeepMongoDB
}
finally {
    $env:GWENT_DEV_PROFILE = $previous.Profile
    $env:GWENT_DEV_SERVER_PORT = $previous.ServerPort
    $env:GWENT_DEV_MONGO_PORT = $previous.MongoPort
    $env:GWENT_DEV_DATABASE = $previous.Database
}

#!/bin/bash
# for development environment only
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
cd "$parent_path/.."
sh scripts/refresh.sh
set ASPNETCORE_ENVIRONMENT=Development
mongod --port 28020 & dotnet watch --project src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj run && fg
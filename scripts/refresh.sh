#!/bin/bash
# refresh Cynthia.Card.Common.dll after modifying Cynthia.Card.Common
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
cd "$parent_path/.."
python ./scripts/language.py
dotnet build ./src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj
cp ./src/Cynthia.Card/src/Cynthia.Card.Common/bin/Debug/netstandard2.0/Cynthia.Card.Common.dll ./src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Assemblies/Cynthia.Card.Common.dll

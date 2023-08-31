@echo off
:: Refresh Cynthia.Card.Common.dll after modifying Cynthia.Card.Common
set "parent_path=%~dp0"
cd /d "%parent_path%..\"
dotnet build .\src\Cynthia.Card\src\Cynthia.Card.Server\Cynthia.Card.Server.csproj
copy .\src\Cynthia.Card\src\Cynthia.Card.Common\bin\Debug\netstandard2.0\Cynthia.Card.Common.dll .\src\Cynthia.Card.Unity\src\Cynthia.Unity.Card\Assets\Assemblies\Cynthia.Card.Common.dll

@echo off
:: For development environment only
set "parent_path=%~dp0"
cd /d "%parent_path%..\"
call scripts\refresh.bat
set "ASPNETCORE_ENVIRONMENT=Development"
dotnet watch --project src\Cynthia.Card\src\Cynthia.Card.Server\Cynthia.Card.Server.csproj run

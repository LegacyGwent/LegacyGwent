FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
CMD dotnet build src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj \
    && dotnet watch --project src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj run
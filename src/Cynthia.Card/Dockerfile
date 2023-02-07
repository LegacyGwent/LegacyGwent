# dotnet docker file
FROM mcr.microsoft.com/dotnet/sdk:3.1 as build
WORKDIR /app

COPY src/Cynthia.Card.Common/Cynthia.Card.Common.csproj ./src/Cynthia.Card.Common/
COPY src/Cynthia.Card.Server/Cynthia.Card.Server.csproj ./src/Cynthia.Card.Server/
COPY src/Cynthia.Card.AI/Cynthia.Card.AI.csproj ./src/Cynthia.Card.AI/
RUN ["dotnet", "restore", "src/Cynthia.Card.Server/Cynthia.Card.Server.csproj"]
COPY src src

WORKDIR /app
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/build", "src/Cynthia.Card.Server/Cynthia.Card.Server.csproj"]

FROM mcr.microsoft.com/dotnet/aspnet:3.1 as runtime
COPY --from=build /build ./
EXPOSE 5005
CMD ["dotnet", "Cynthia.Card.Server.dll"]
# dotnet docker file
FROM mcr.microsoft.com/dotnet/sdk:3.1 as build
WORKDIR /app

COPY *.sln ./
COPY src/Cynthia.Card.Common/*.csproj ./src/Cynthia.Card.Common/
COPY src/Cynthia.Card.Server/*.csproj ./src/Cynthia.Card.Server/
COPY src/Cynthia.Card.AI/*.csproj ./src/Cynthia.Card.AI/
COPY test/AITest/*.csproj ./test/AITest/
COPY test/ConsoleTest/*.csproj ./test/ConsoleTest/
RUN dotnet restore
COPY . .

WORKDIR /app
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/build"]

FROM mcr.microsoft.com/dotnet/aspnet:3.1 as runtime
COPY --from=build /build ./
EXPOSE 5005
CMD ["dotnet", "Cynthia.Card.Server.dll"]
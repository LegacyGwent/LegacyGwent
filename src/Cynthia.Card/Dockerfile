# dotnet docker file
FROM mcr.microsoft.com/dotnet/sdk:3.1 as build
WORKDIR /app

# Copy everything
COPY src/Cynthia.Card.Common/Cynthia.Card.Common.csproj ./src/Cynthia.Card.Common/
COPY src/Cynthia.Card.Server/Cynthia.Card.Server.csproj ./src/Cynthia.Card.Server/
COPY src/Cynthia.Card.AI/Cynthia.Card.AI.csproj ./src/Cynthia.Card.AI/

# Restore as distinct layers
RUN ["dotnet", "restore", "src/Cynthia.Card.Server/Cynthia.Card.Server.csproj"]
COPY src src

# Build and publish a release
WORKDIR /app
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/build", "src/Cynthia.Card.Server/Cynthia.Card.Server.csproj"]

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1 as runtime
COPY --from=build /build ./
EXPOSE 5005
CMD ["dotnet", "Cynthia.Card.Server.dll"]
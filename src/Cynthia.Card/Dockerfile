# .NET 10 build and runtime smoke image. Production deploys the same
# self-contained linux-x64 publish output directly under systemd.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy everything
COPY src/Cynthia.Card.Common/Cynthia.Card.Common.csproj ./src/Cynthia.Card.Common/
COPY src/Cynthia.Card.Server/Cynthia.Card.Server.csproj ./src/Cynthia.Card.Server/
COPY src/Cynthia.Card.AI/Cynthia.Card.AI.csproj ./src/Cynthia.Card.AI/

# Restore as distinct layers
RUN ["dotnet", "restore", "--runtime", "linux-x64", "src/Cynthia.Card.Server/Cynthia.Card.Server.csproj"]
COPY src src

# Build and publish a self-contained Linux release.
WORKDIR /app
RUN dotnet publish \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained true \
    --no-restore \
    --output /build \
    src/Cynthia.Card.Server/Cynthia.Card.Server.csproj \
    && test -x /build/Cynthia.Card.Server

# A self-contained publish only needs the native runtime dependencies.
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV DOTNET_SYSTEM_GLOBALIZATION_PREDEFINED_CULTURES_ONLY=0
COPY --from=build /build ./
EXPOSE 5005
EXPOSE 5010
ENTRYPOINT ["./Cynthia.Card.Server"]

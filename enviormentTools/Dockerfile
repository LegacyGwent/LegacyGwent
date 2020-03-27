# 不建议使用这个dockerfile，需要配合docker compose使用
FROM mcr.microsoft.com/dotnet/core/sdk:3.0
RUN apt update \
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv 9DA31620334BD75D9DCB49F368818C72E52529D4 \
    && echo "deb http://repo.mongodb.org/apt/debian stretch/mongodb-org/4.0 main" | tee /etc/apt/sources.list.d/mongodb-org.list \
    && echo "deb http://deb.debian.org/debian/ stretch main" | tee /etc/apt/sources.list.d/debian-stretch.list \
    && apt update \
    && apt install libcurl3 -y \
    && apt install mongodb-org -y \
    && mkdir /data \
    && mkdir /data/db \
    && nohup mongod --port 28020 &
WORKDIR /app
CMD dotnet build src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj \
    && dotnet watch --project src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj run
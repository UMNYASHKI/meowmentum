# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /src
ARG BUILD_CONFIGURATION=Release
COPY --from=proto / /src/proto/

RUN mkdir -p /src/dotnet/Meowmentum.Server.Dotnet.Api

WORKDIR /src/dotnet/Meowmentum.Server.Dotnet.Api

COPY ./Meowmentum.Server.Dotnet.Api ./Meowmentum.Server.Dotnet.Api
COPY ./Meowmentum.Server.Dotnet.Business ./Meowmentum.Server.Dotnet.Business
COPY ./Meowmentum.Server.Dotnet.Core ./Meowmentum.Server.Dotnet.Core
COPY ./Meowmentum.Server.Dotnet.Shared ./Meowmentum.Server.Dotnet.Shared
COPY ./Meowmentum.Server.Dotnet.Infrastructure ./Meowmentum.Server.Dotnet.Infrastructure
COPY ./Meowmentum.Server.Dotnet.Persistence ./Meowmentum.Server.Dotnet.Persistence

RUN dotnet restore "./Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api.csproj"
WORKDIR "/src/dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api"
RUN dotnet build "./Meowmentum.Server.Dotnet.Api.csproj" -c $BUILD_CONFIGURATION
RUN dotnet ef migrations bundle --no-build --configuration $BUILD_CONFIGURATION --output /app/efbundle

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Meowmentum.Server.Dotnet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meowmentum.Server.Dotnet.Api.dll"]

# This stage is used to create the migration container, that stays up to be exec-able + automigrates on local env
FROM base AS migrate
WORKDIR /app
COPY --from=build /app/efbundle ./efbundle

RUN echo $' \n\
if [ "$ENVIRONMENT" = "local" -a "$CONFIG_NO_LOCAL_MIGRATE" != "1" ]; then \n\
    ./efbundle \n\
    echo "Local migrations executed, to disable set CONFIG_NO_LOCAL_MIGRATE=1" \n\
fi \n\
\n\
trap : TERM INT \n\
sleep infinity & wait \n\
' > migrate.sh

ENTRYPOINT ["bash", "migrate.sh"]

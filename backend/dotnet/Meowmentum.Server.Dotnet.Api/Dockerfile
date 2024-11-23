# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release
COPY --from=proto / /src/proto/

RUN mkdir -p /src/dotnet/Meowmentum.Server.Dotnet.Api

WORKDIR /src/dotnet/Meowmentum.Server.Dotnet.Api

COPY dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api ./Meowmentum.Server.Dotnet.Api
COPY dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Business ./Meowmentum.Server.Dotnet.Business
COPY dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Core ./Meowmentum.Server.Dotnet.Core
COPY dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Shared ./Meowmentum.Server.Dotnet.Shared
COPY dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Infrastructure ./Meowmentum.Server.Dotnet.Infrastructure
COPY dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Persistence ./Meowmentum.Server.Dotnet.Persistence

RUN dotnet restore "./Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api.csproj"
COPY . .
WORKDIR "/src/dotnet/Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api"
RUN dotnet build "./Meowmentum.Server.Dotnet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Meowmentum.Server.Dotnet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meowmentum.Server.Dotnet.Api.dll"]
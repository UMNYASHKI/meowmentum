# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
# TODO: Remove EXPOSE from dockerfile, use buildtool to run local environment with nginx
EXPOSE 8080
# TODO: There is no need for HTTPS webserver, TLS will be handled externally by reverse proxy in production
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api.csproj", "Meowmentum.Server.Dotnet.Api/"]
COPY ["Meowmentum.Server.Dotnet.Business/Meowmentum.Server.Dotnet.Business.csproj", "Meowmentum.Server.Dotnet.Business/"]
COPY ["Meowmentum.Server.Dotnet.Core/Meowmentum.Server.Dotnet.Core.csproj", "Meowmentum.Server.Dotnet.Core/"]
COPY ["Meowmentum.Server.Dotnet.Shared/Meowmentum.Server.Dotnet.Shared.csproj", "Meowmentum.Server.Dotnet.Shared/"]
COPY ["Meowmentum.Server.Dotnet.Infrastructure/Meowmentum.Server.Dotnet.Infrastructure.csproj", "Meowmentum.Server.Dotnet.Infrastructure/"]
COPY ["Meowmentum.Server.Dotnet.Persistence/Meowmentum.Server.Dotnet.Persistence.csproj", "Meowmentum.Server.Dotnet.Persistence/"]
RUN dotnet restore "./Meowmentum.Server.Dotnet.Api/Meowmentum.Server.Dotnet.Api.csproj"
COPY . .
WORKDIR "/src/Meowmentum.Server.Dotnet.Api"
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
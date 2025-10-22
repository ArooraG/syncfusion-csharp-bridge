# Dockerfile

# 1. Build Stage: Stable .NET 8.0 SDK use karein
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Project file copy karein
COPY ["SyncfusionBridgeAPI.csproj", "."]

# Zaroori packages restore karein
RUN dotnet restore "SyncfusionBridgeAPI.csproj"

# Baaki sab files copy karein
COPY . .

# Project ko taiyar karein (Build/Publish)
RUN dotnet publish "SyncfusionBridgeAPI.csproj" -c Release -o /app/publish

# 2. Final Stage: sirf app ko chalaane ki zaroorat hai
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Published files copy karein
COPY --from=build /app/publish .

# App ko chalao (yeh aapka Start Command hai)
ENTRYPOINT ["dotnet", "SyncfusionBridgeAPI.dll"]
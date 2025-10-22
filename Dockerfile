
# Dockerfile (CLEAN VERSION)

# 1. Build Stage: .NET SDK image use karo
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Project file copy karo
COPY ["SyncfusionBridgeAPI.csproj", "."]

# Zaroori packages restore karo
RUN dotnet restore "SyncfusionBridgeAPI.csproj"

# Baaki sab files copy karo
COPY . .

# Project ko taiyar karo (Build/Publish)
RUN dotnet publish "SyncfusionBridgeAPI.csproj" -c Release -o /app/publish

# 2. Final Stage: sirf app ko chalaane ki zaroorat hai
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Published files copy karo
COPY --from=build /app/publish .

# App ko chalao (yeh aapka Start Command hai)
# Dockerfile (CLEAN VERSION)

# 1. Build Stage: .NET SDK image use karo
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Project file copy karo
COPY ["SyncfusionBridgeAPI.csproj", "."]

# Zaroori packages restore karo
RUN dotnet restore "SyncfusionBridgeAPI.csproj"

# Baaki sab files copy karo
COPY . .

# Project ko taiyar karo (Build/Publish)
RUN dotnet publish "SyncfusionBridgeAPI.csproj" -c Release -o /app/publish

# 2. Final Stage: sirf app ko chalaane ki zaroorat hai
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Published files copy karo
COPY --from=build /app/publish .

# App ko chalao (yeh aapka Start Command hai)
 8e0d5715570f8015c845de0a3747ae15be436084
ENTRYPOINT ["dotnet", "SyncfusionBridgeAPI.dll"]
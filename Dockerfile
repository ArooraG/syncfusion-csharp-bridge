# 1. Build Stage: yahan app ko build/publish karte hain
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Pehle sirf project file copy karein
COPY ["SyncfusionBridgeAPI.csproj", "."]

# ZAROORI LINE: GUMSHUDA PACKAGE KO ZABARDASTI INSTALL KAREIN
RUN dotnet add package Syncfusion.Pdf.Imaging.Net.Core --version 26.1.35

# Ab baaqi packages restore karein
RUN dotnet restore "SyncfusionBridgeAPI.csproj"

# Baaki sab files copy karein
COPY . .

# Project ko publish karein
RUN dotnet publish "SyncfusionBridgeAPI.csproj" -c Release -o /app/publish


# 2. Final Stage: sirf app ko chalaane ki zaroorat hai
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

# App ko chalaayein
ENTRYPOINT ["dotnet", "SyncfusionBridgeAPI.dll"]
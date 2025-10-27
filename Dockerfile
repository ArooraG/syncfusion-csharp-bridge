# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file to leverage Docker layer caching
COPY ["SyncfusionBridgeAPI.csproj", "."]

# --- THE GUARANTEED FIX ---
# Manually add the missing package to the .csproj file inside the container
RUN dotnet add package Syncfusion.Pdf.Imaging.Net.Core --version 26.1.35

# Restore all other dependencies
RUN dotnet restore "SyncfusionBridgeAPI.csproj"

# Copy the rest of the application files
COPY . .

# Publish the application
WORKDIR "/src/."
RUN dotnet publish "SyncfusionBridgeAPI.csproj" -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SyncfusionBridgeAPI.dll"]
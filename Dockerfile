# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all files first
COPY . .

# --- START OF DEBUGGING ---
# Hum ab dekhenge ke .csproj file mein andar hai kya.
RUN echo "--- Verifying .csproj content BEFORE restore ---"
RUN cat SyncfusionBridgeAPI.csproj
RUN echo "----------------------------------------------"
# --- END OF DEBUGGING ---

# Restore dependencies
RUN dotnet restore "SyncfusionBridgeAPI.csproj"

# Publish the application
RUN dotnet publish "SyncfusionBridgeAPI.csproj" -c Release -o /app/publish


# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SyncfusionBridgeAPI.dll"]
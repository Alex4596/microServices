FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only required csproj files
COPY src/Services/OrderService/OrderService.csproj src/Services/OrderService/
COPY src/BuildingBlocks/SharedContracts/SharedContracts.csproj src/BuildingBlocks/SharedContracts/

RUN dotnet restore "src/Services/OrderService/OrderService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR /src/src/Services/OrderService
RUN dotnet build "OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage - remove conflicting appsettings from the other service
FROM build AS publish
WORKDIR /src/src/Services/OrderService

# Delete InventoryService's appsettings to avoid NETSDK1152
RUN find /src -name "appsettings*.json" -path "/src/src/Services/InventoryService/*" -delete || true

RUN dotnet publish "OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]

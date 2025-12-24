FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/Services/InventoryService/InventoryService.csproj src/Services/InventoryService/
COPY src/BuildingBlocks/SharedContracts/SharedContracts.csproj src/BuildingBlocks/SharedContracts/

RUN dotnet restore "src/Services/InventoryService/InventoryService.csproj"

COPY . .

WORKDIR /src/src/Services/InventoryService
RUN dotnet build "InventoryService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
WORKDIR /src/src/Services/InventoryService

# Delete OrderService's appsettings to avoid NETSDK1152
RUN find /src -name "appsettings*.json" -path "/src/src/Services/OrderService/*" -delete || true

RUN dotnet publish "InventoryService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]

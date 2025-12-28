FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/Services/PaymentService/PaymentService.csproj src/Services/PaymentService/
COPY src/BuildingBlocks/SharedContracts/SharedContracts.csproj src/BuildingBlocks/SharedContracts/
RUN dotnet restore "src/Services/PaymentService/PaymentService.csproj"

COPY . .

WORKDIR /src/src/Services/PaymentService
RUN dotnet build "PaymentService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
WORKDIR /src/src/Services/PaymentService

RUN find /src -name "appsettings*.json" -path "*/OrderService/*" -delete || true
RUN find /src -name "appsettings*.json" -path "*/InventoryService/*" -delete || true

RUN dotnet publish "PaymentService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]

FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/PhoneLineOrders.OrdersPlacedSubscriber/PhoneLineOrderer.OrdersPlacedSubscriber.csproj src/PhoneLineOrders.OrdersPlacedSubscriber/
COPY src/PhoneLineOrderer/PhoneLineOrderer.csproj src/PhoneLineOrderer/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/PhoneLineOrders.OrdersPlacedSubscriber/PhoneLineOrderer.OrdersPlacedSubscriber.csproj
COPY . .
WORKDIR /src/src/PhoneLineOrders.OrdersPlacedSubscriber
RUN dotnet build PhoneLineOrderer.OrdersPlacedSubscriber.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish PhoneLineOrderer.OrdersPlacedSubscriber.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PhoneLineOrderer.OrdersPlacedSubscriber.dll"]

FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Customers.PhoneLineOrderCompletedSubscriber/Customers.PhoneLineOrderCompletedSubscriber.csproj src/Customers.PhoneLineOrderCompletedSubscriber/
COPY src/Customers/Customers.csproj src/Customers/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/Customers.PhoneLineOrderCompletedSubscriber/Customers.PhoneLineOrderCompletedSubscriber.csproj
COPY . .
WORKDIR /src/src/Customers.PhoneLineOrderCompletedSubscriber
RUN dotnet build Customers.PhoneLineOrderCompletedSubscriber.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Customers.PhoneLineOrderCompletedSubscriber.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Customers.PhoneLineOrderCompletedSubscriber.dll"]

FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/FakeBt.OrderUpdater/FakeBt.OrderUpdater.csproj src/FakeBt.OrderUpdater/
COPY src/FakeBt/FakeBt.csproj src/FakeBt/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/FakeBt.OrderUpdater/FakeBt.OrderUpdater.csproj
COPY . .
WORKDIR /src/src/FakeBt.OrderUpdater
RUN dotnet build FakeBt.OrderUpdater.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FakeBt.OrderUpdater.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FakeBt.OrderUpdater.dll"]

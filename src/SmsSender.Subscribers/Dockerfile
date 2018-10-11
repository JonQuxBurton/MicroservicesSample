FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/SmsSender.Subscribers/SmsSender.Subscribers.csproj src/SmsSender.Subscribers/
COPY src/SmsSender/SmsSender.csproj src/SmsSender/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/SmsSender.Subscribers/SmsSender.Subscribers.csproj
COPY . .
WORKDIR /src/src/SmsSender.Subscribers
RUN dotnet build SmsSender.Subscribers.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SmsSender.Subscribers.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SmsSender.Subscribers.dll"]

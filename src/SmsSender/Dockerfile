FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/SmsSender/SmsSender.csproj src/SmsSender/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/SmsSender/SmsSender.csproj
COPY . .
WORKDIR /src/src/SmsSender
RUN dotnet build SmsSender.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SmsSender.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SmsSender.dll"]

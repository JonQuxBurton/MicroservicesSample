FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/FakeBt/FakeBt.csproj", "src/FakeBt/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/FakeBt/FakeBt.csproj"
COPY . .
WORKDIR "/src/src/FakeBt"
RUN dotnet build "FakeBt.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FakeBt.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FakeBt.dll"]
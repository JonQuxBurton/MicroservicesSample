FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Customers/Customers.csproj src/Customers/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/Customers/Customers.csproj
COPY . .
WORKDIR /src/src/Customers
RUN dotnet build Customers.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Customers.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Customers.dll"]

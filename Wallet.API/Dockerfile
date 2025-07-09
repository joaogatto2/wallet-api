FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ./ ./
RUN ls -la
RUN dotnet restore Wallet.API/Wallet.API.csproj
RUN dotnet build Wallet.API/Wallet.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Wallet.API/Wallet.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.API.dll"]

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY SmartMeterToMqtt.csproj .
RUN dotnet restore SmartMeterToMqtt.csproj
COPY . .
RUN dotnet build SmartMeterToMqtt.csproj -c Release -o /app/build
RUN dotnet publish SmartMeterToMqtt.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SmartMeterToMqtt.dll"]
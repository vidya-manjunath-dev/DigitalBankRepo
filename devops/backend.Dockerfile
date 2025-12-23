# Multi-stage build for .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["DigitalBankLite.API/DigitalBankLite.API.csproj", "DigitalBankLite.API/"]
RUN dotnet restore "DigitalBankLite.API/DigitalBankLite.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/DigitalBankLite.API"
RUN dotnet build "DigitalBankLite.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DigitalBankLite.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "DigitalBankLite.API.dll"]

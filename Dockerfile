# Build stage (Linux Alpine tabanlı .NET 8 SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY DeviceTracker.Api.csproj .
RUN dotnet restore DeviceTracker.Api.csproj

COPY . .
RUN dotnet publish DeviceTracker.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage (yalnızca hafif ASP.NET Core runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER app

ENTRYPOINT ["dotnet", "DeviceTracker.Api.dll"]

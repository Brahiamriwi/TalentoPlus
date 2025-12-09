# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TalentoPlus.sln .
COPY TalentoPlus.Core/TalentoPlus.Core.csproj TalentoPlus.Core/
COPY TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj TalentoPlus.Infrastructure/
COPY TalentoPlus.Web/TalentoPlus.Web.csproj TalentoPlus.Web/
COPY TalentoPlus.API/TalentoPlus.API.csproj TalentoPlus.API/
COPY TalentoPlus.Tests/TalentoPlus.Tests.csproj TalentoPlus.Tests/

# Restore dependencies
RUN dotnet restore TalentoPlus.Web/TalentoPlus.Web.csproj

# Copy source code
COPY . .

# Build and publish
WORKDIR /src/TalentoPlus.Web
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install libgdiplus for QuestPDF
RUN apt-get update && apt-get install -y libgdiplus libc6-dev && rm -rf /var/lib/apt/lists/*

EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TalentoPlus.Web.dll"]

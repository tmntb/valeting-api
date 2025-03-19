# Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

# Create safe user to run the app
RUN adduser --shell /bin/false --disabled-password -H --uid 1001 default

# Intall globaliztion support
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy all the project files
COPY Valeting.Common/Valeting.Common.csproj Valeting.Common/
COPY Valeting.Repository/Valeting.Repository.csproj Valeting.Repository/
COPY Valeting.Core/Valeting.Core.csproj Valeting.Core/
COPY Valeting.API/Valeting.API.csproj Valeting.API/

# Restore dependencies
RUN dotnet restore "./Valeting.API/Valeting.API.csproj"

# Copy all the project files
COPY Valeting.Common/ Valeting.Common/
COPY Valeting.Repository/ Valeting.Repository/
COPY Valeting.Core/ Valeting.Core/
COPY Valeting.API/ Valeting.API/

# Publish stage
ARG BUILD_CONFIGURATION=Release
FROM build AS publish
RUN dotnet publish "./Valeting.API/Valeting.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish --verbosity n --no-restore

# Final stage
FROM base AS final
USER default
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Valeting.API.dll"]
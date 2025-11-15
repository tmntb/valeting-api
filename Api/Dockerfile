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
COPY Common/Common.csproj Common/
COPY Repository/Repository.csproj Repository/
COPY Service/Service.csproj Service/
COPY Api/Api.csproj Api/

# Restore dependencies
RUN dotnet restore "./Api/Api.csproj"

# Copy all the project files
COPY Common/ Common/
COPY Repository/ Repository/
COPY Service/ Service/
COPY Api/ Api/

# Publish stage
ARG BUILD_CONFIGURATION=Release
FROM build AS publish
RUN dotnet publish "./Api/Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --verbosity n --no-restore

# Final stage
FROM base AS final
USER default
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
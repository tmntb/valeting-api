#!/bin/bash

# Detect architecture
ARCH=$(uname -m)

# If no arguments provided, default to "up"
if [ $# -eq 0 ]; then
    ARGS="up"
else
    ARGS="$@"
fi

if [ "$ARCH" = "arm64" ] || [ "$ARCH" = "aarch64" ]; then
    echo "🍎 Detected Mac ARM (M1/M2/M3) - using Azure SQL Edge"
    docker compose -f docker-compose.yml -f docker-compose.mac.yml $ARGS
else
    echo "💻 Detected AMD64 - using standard SQL Server"
    docker compose $ARGS
fi
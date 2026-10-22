#!/bin/bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$DIR"

echo "=================================================="
echo "    Starting JX1M Game Cluster with Docker        "
echo "=================================================="

# Check Docker running
if ! docker info >/dev/null 2>&1; then
    echo "ERROR: Docker daemon is not running! Please start Docker Desktop first."
    exit 1
fi

echo "[1/3] Building images..."
docker compose build

echo "[2/3] Starting containers..."
docker compose up -d

echo "[3/3] Checking cluster health..."
docker compose ps

echo "=================================================="
echo "Cluster started successfully!"
echo " - Web SDK:     http://localhost:8081 (or http://localhost:81)"
echo " - Game Server: 127.0.0.1:3001 (TCP)"
echo " - Game DB:     127.0.0.1:23001 (TCP)"
echo " - MySQL:       127.0.0.1:3306"
echo ""
echo "To view logs:   docker compose logs -f"
echo "To stop:        ./scripts/stop_docker.sh"
echo "=================================================="

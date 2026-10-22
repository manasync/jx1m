#!/bin/bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$DIR"

DOTNET_BIN=$(which dotnet 2>/dev/null || echo "$HOME/.dotnet/dotnet")

if [ ! -f "$DOTNET_BIN" ]; then
    echo "ERROR: dotnet SDK not found in PATH or at $HOME/.dotnet/dotnet"
    exit 1
fi

echo "=================================================="
echo "    Starting JX1M Game Cluster (Host/Native)      "
echo "=================================================="
echo "Using .NET: $DOTNET_BIN ($($DOTNET_BIN --version))"

mkdir -p "$DIR/logs"

# 1. Start Web SDK
echo "[1/3] Starting KiemTheWebSDK..."
(cd "$DIR/server/src/WEB/KiemTheWebSDK" && "$DOTNET_BIN" run --urls "http://0.0.0.0:8081" > "$DIR/logs/websdk.log" 2>&1) &
PID_WEB=$!
echo " - WebSDK started (PID: $PID_WEB), listening on http://localhost:8081"
echo "$PID_WEB" > "$DIR/logs/websdk.pid"

# 2. Start GameDBServer
echo "[2/3] Starting GameDBServer..."
if [ -f "$DIR/server/src/GameDBServer/GameDBServer/bin/Debug/net6.0/GameDBServer.dll" ]; then
    (cd "$DIR/server/src/GameDBServer/GameDBServer/bin/Debug/net6.0" && "$DOTNET_BIN" GameDBServer.dll > "$DIR/logs/gamedbserver.log" 2>&1) &
    PID_DB=$!
    echo " - GameDBServer started (PID: $PID_DB), listening on port 23001"
    echo "$PID_DB" > "$DIR/logs/gamedbserver.pid"
else
    echo " - GameDBServer binary not found, please build with: dotnet build server/src/GameDBServer/GameDBServer/GameDBServer.csproj"
fi

# 3. Start GameServer
echo "[3/3] Starting GameServer..."
if [ -f "$DIR/server/src/GameServer/KTGameServer/GameServer/bin/Debug/net6.0/GameServer.dll" ]; then
    (cd "$DIR/server/src/GameServer/KTGameServer/GameServer/bin/Debug/net6.0" && "$DOTNET_BIN" GameServer.dll > "$DIR/logs/gameserver.log" 2>&1) &
    PID_GAME=$!
    echo " - GameServer started (PID: $PID_GAME), listening on port 3001"
    echo "$PID_GAME" > "$DIR/logs/gameserver.pid"
else
    echo " - GameServer binary not found, please build with: dotnet build server/src/GameServer/KTGameServer/KTGameServer.sln"
fi

echo "=================================================="
echo "All services launched in background!"
echo " - Logs are written to: $DIR/logs/"
echo " - To tail logs: tail -f logs/*.log"
echo " - To stop all:  ./scripts/stop_local.sh"
echo "=================================================="

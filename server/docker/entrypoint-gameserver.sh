#!/bin/bash
set -e

DB_SERVER_HOST="${DB_SERVER_HOST:-gamedbserver}"
DB_SERVER_PORT="${DB_SERVER_PORT:-23001}"
LOG_DB_SERVER_HOST="${LOG_DB_SERVER_HOST:-$DB_SERVER_HOST}"
LOG_DB_SERVER_PORT="${LOG_DB_SERVER_PORT:-$DB_SERVER_PORT}"
WEBSDK_HOST="${WEBSDK_HOST:-websdk}"
WEBSDK_PORT="${WEBSDK_PORT:-8081}"

echo "[GameServer] Configuring GameServer connection to GameDBServer at $DB_SERVER_HOST:$DB_SERVER_PORT..."

if [ -f "AppConfig.xml" ]; then
    sed -i "s/<DBServer pool=\"[0-9]*\" ip=\"[^\"]*\" port=\"[0-9]*\"/<DBServer pool=\"10\" ip=\"$DB_SERVER_HOST\" port=\"$DB_SERVER_PORT\"/g" AppConfig.xml
    sed -i "s/<LogDBServer pool=\"[0-9]*\" ip=\"[^\"]*\" port=\"[0-9]*\"/<LogDBServer pool=\"10\" ip=\"$LOG_DB_SERVER_HOST\" port=\"$LOG_DB_SERVER_PORT\"/g" AppConfig.xml
    sed -i "s|http://localhost:81/ActiveGiftCode.aspx|http://$WEBSDK_HOST:$WEBSDK_PORT/ActiveGiftCode.aspx|g" AppConfig.xml
fi

if [ -d "Config/MapConfig" ]; then
    echo "[GameServer] Normalizing map config filenames for Linux..."
    find Config/MapConfig -name "npcs.xml" -execdir cp -n npcs.xml NPCs.xml \; 2>/dev/null || true
    find Config/MapConfig -name "teleports.xml" -execdir cp -n teleports.xml Teleports.xml \; 2>/dev/null || true
    find Config/MapConfig -name "zones.xml" -execdir cp -n zones.xml Zones.xml \; 2>/dev/null || true
    find Config/MapConfig -name "monsters.xml" -execdir cp -n monsters.xml Monsters.xml \; 2>/dev/null || true
    find Config/MapConfig -name "growpoints.xml" -execdir cp -n growpoints.xml GrowPoints.xml \; 2>/dev/null || true
    find Config/MapConfig -name "mapsetting.xml" -execdir cp -n mapsetting.xml MapSetting.xml \; 2>/dev/null || true
    find Config/MapConfig -name "obs.xml" -execdir cp -n obs.xml Obs.xml \; 2>/dev/null || true
    find Config/MapConfig -name "obs.txt" -execdir cp -n obs.txt Obs.txt \; 2>/dev/null || true
fi

echo "[GameServer] Starting GameServer..."
exec dotnet GameServer.dll

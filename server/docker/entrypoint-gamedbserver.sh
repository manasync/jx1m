#!/bin/bash
set -e

DB_HOST="${DB_HOST:-mysql}"
DB_PORT="${DB_PORT:-3306}"

echo "[GameDBServer] Configuring Database host to $DB_HOST:$DB_PORT..."

if [ -f "AppConfig.xml" ]; then
    # Replace ip="127.0.0.1" port="3306" with target DB_HOST and DB_PORT
    sed -i "s/ip=\"127.0.0.1\" port=\"3306\"/ip=\"$DB_HOST\" port=\"$DB_PORT\"/g" AppConfig.xml
fi

echo "[GameDBServer] Starting GameDBServer on port 23001..."
exec dotnet GameDBServer.dll

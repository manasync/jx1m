#!/bin/bash
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "Stopping local JX1M services..."

for pidfile in "$DIR/logs/"*.pid; do
    if [ -f "$pidfile" ]; then
        pid=$(cat "$pidfile")
        if kill -0 "$pid" 2>/dev/null; then
            echo "Killing process $pid ($(basename "$pidfile" .pid))..."
            kill "$pid" 2>/dev/null || kill -9 "$pid" 2>/dev/null
        fi
        rm -f "$pidfile"
    fi
done

echo "Stopped."

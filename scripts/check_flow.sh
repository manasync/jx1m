#!/bin/bash
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$DIR"

echo "=================================================================="
echo "    JX1M PRE-FLIGHT INTEGRATION TEST & FLOW VERIFICATION TOOL     "
echo "=================================================================="

# 1. Run Backend E2E Bot (WebSDK -> DB -> GameServer -> Role -> World)
echo ""
echo ">>> [STAGE 1/2] RUNNING BACKEND & PROTOCOL E2E TEST (HEADLESS BOT)..."
python3 scripts/test_e2e_full.py

if [ $? -ne 0 ]; then
    echo "[-] STAGE 1 FAILED! Fix backend / database issues before launching game."
    exit 1
fi

# 2. Run Unity Client Pre-flight Validator (Batchmode)
echo ""
echo ">>> [STAGE 2/2] RUNNING UNITY CLIENT ASSET & SCENE INTEGRITY CHECK..."
UNITY_BIN="/Applications/Unity/Hub/Editor/2021.3.30f1/Unity.app/Contents/MacOS/Unity"

if [ -f "$UNITY_BIN" ]; then
    "$UNITY_BIN" -batchmode -quit -projectPath client/src -executeMethod PreflightValidator.RunChecks -logFile preflight.log
    if [ $? -ne 0 ]; then
        echo "[-] STAGE 2 FAILED! Asset bundles or config files are corrupted or missing."
        echo "Check preflight.log for details."
        exit 1
    fi
    echo "  [OK] Unity Client preflight checks completed successfully!"
else
    echo "  [WARN] Unity binary not found at $UNITY_BIN, skipped client asset check."
fi

echo ""
echo "=================================================================="
echo "  SUCCESS: ALL FLOWS ARE VERIFIED AND GREEN! READY TO PLAY GAME! "
echo "=================================================================="

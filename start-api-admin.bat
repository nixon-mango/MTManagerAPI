@echo off
echo ========================================
echo   MT5 Web API - Network Access (Admin)
echo ========================================
echo.

REM Check if running as Administrator
net session >nul 2>&1
if %errorLevel% NEQ 0 (
    echo This script requires Administrator privileges to bind to 0.0.0.0
    echo.
    echo Please run as Administrator:
    echo 1. Right-click on this batch file
    echo 2. Select "Run as administrator"
    echo.
    pause
    exit /b 1
)

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API with network access...
echo.
echo Server will be accessible from:
echo   - Local machine: http://localhost:8080
echo   - Static IP: http://YOUR_STATIC_IP:8080
echo   - Network: http://YOUR_STATIC_IP:8080 (from other machines)
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.

MT5WebAPI.exe --host 0.0.0.0 --port 8080

echo.
echo Server stopped.
pause
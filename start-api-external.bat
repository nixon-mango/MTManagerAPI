@echo off
echo ========================================
echo   MT5 Web API - External Access Mode
echo ========================================
echo.

REM Check if running as Administrator
net session >nul 2>&1
if %errorLevel% NEQ 0 (
    echo ⚠️  Administrator privileges required for external access
    echo.
    echo To bind to 0.0.0.0 (all network interfaces), you need to:
    echo 1. Right-click on this batch file
    echo 2. Select "Run as administrator"
    echo.
    echo Alternatively, you can:
    echo • Use setup-url-reservation.bat (one-time setup as admin)
    echo • Use your specific static IP instead of 0.0.0.0
    echo.
    pause
    exit /b 1
)

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API for external access...
echo.
echo The API will be accessible from:
echo   - Local machine: http://localhost:8080
echo   - Network: http://YOUR_STATIC_IP:8080
echo.
echo Make sure Windows Firewall allows port 8080!
echo Run configure-firewall.bat if needed.
echo.
echo Press Ctrl+C to stop the server
echo.

MT5WebAPI.exe --host 0.0.0.0 --port 8080

echo.
echo Server stopped.
pause
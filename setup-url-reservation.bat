@echo off
echo ========================================
echo   MT5 Web API - URL Reservation Setup
echo ========================================
echo.

REM Check if running as Administrator
net session >nul 2>&1
if %errorLevel% NEQ 0 (
    echo This script requires Administrator privileges.
    echo.
    echo Please run as Administrator:
    echo 1. Right-click on this batch file
    echo 2. Select "Run as administrator"
    echo.
    pause
    exit /b 1
)

echo This script will configure Windows to allow the MT5 Web API
echo to bind to 0.0.0.0:8080 without requiring Administrator privileges.
echo.
echo This is a one-time setup that will allow you to run:
echo   MT5WebAPI.exe --host 0.0.0.0 --port 8080
echo.
echo without Administrator privileges in the future.
echo.
pause

echo Adding URL reservation for http://0.0.0.0:8080/...
netsh http add urlacl url=http://0.0.0.0:8080/ user=Everyone

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ URL reservation added successfully!
    echo.
    echo You can now run the MT5 Web API with network access
    echo without Administrator privileges:
    echo.
    echo   MT5WebAPI.exe --host 0.0.0.0 --port 8080
    echo.
    echo The server will be accessible from:
    echo   - Local machine: http://localhost:8080
    echo   - Static IP: http://YOUR_STATIC_IP:8080
    echo   - Network: http://YOUR_STATIC_IP:8080
    echo.
) else (
    echo.
    echo ❌ Failed to add URL reservation.
    echo.
    echo You will need to run the MT5 Web API as Administrator
    echo to bind to 0.0.0.0, or use your specific static IP instead:
    echo.
    echo   MT5WebAPI.exe --host YOUR_STATIC_IP --port 8080
    echo.
)

echo.
echo Current URL reservations:
netsh http show urlacl url=http://0.0.0.0:8080/

echo.
pause
@echo off
echo ========================================
echo   MT5 Web API - Static IP Mode
echo ========================================
echo.

REM You need to replace YOUR_STATIC_IP with your actual IP address
set STATIC_IP=YOUR_STATIC_IP

echo IMPORTANT: Edit this file and replace YOUR_STATIC_IP with your actual static IP address
echo.
echo To find your IP address, run: ipconfig
echo Look for "IPv4 Address" in the output.
echo.

if "%STATIC_IP%"=="YOUR_STATIC_IP" (
    echo ❌ Please edit this batch file first:
    echo.
    echo 1. Open start-with-static-ip.bat in a text editor
    echo 2. Replace YOUR_STATIC_IP with your actual IP address
    echo 3. Save the file and run again
    echo.
    echo Example: set STATIC_IP=192.168.1.100
    echo.
    pause
    exit /b 1
)

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API with static IP binding...
echo.
echo Server will be accessible from:
echo   - Local machine: http://localhost:8080
echo   - Static IP: http://%STATIC_IP%:8080
echo   - Network: http://%STATIC_IP%:8080 (from other machines)
echo.
echo No Administrator privileges required for this method!
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.

MT5WebAPI.exe --host %STATIC_IP% --port 8080

echo.
echo Server stopped.
pause
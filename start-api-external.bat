@echo off
echo ========================================
echo   MT5 Web API - External Access Mode
echo ========================================
echo.

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API for external access...
echo.
echo The API will be accessible from:
echo   - Local machine: http://localhost:8080
echo   - Network: http://YOUR_STATIC_IP:8080
echo.
echo Make sure Windows Firewall allows port 8080!
echo.
echo Press Ctrl+C to stop the server
echo.

MT5WebAPI.exe --host 0.0.0.0 --port 8080

echo.
echo Server stopped.
pause
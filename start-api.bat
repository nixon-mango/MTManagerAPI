@echo off
echo ========================================
echo   MT5 Web API Server - Network Access
echo ========================================
echo.

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API Server...
echo.
echo Server will be accessible from:
echo   - Local machine: http://localhost:8080
echo   - Network access: http://YOUR_STATIC_IP:8080
echo   - All interfaces: http://0.0.0.0:8080
echo.
echo Make sure Windows Firewall allows port 8080!
echo Run configure-firewall.bat as Administrator if needed.
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.

MT5WebAPI.exe

echo.
echo Server stopped.
pause
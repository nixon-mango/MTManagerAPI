@echo off
echo ========================================
echo   MT5 Web API - Local Access Only
echo ========================================
echo.

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API for local access only...
echo.
echo The API will be accessible from:
echo   - Local machine: http://localhost:8080
echo.
echo Press Ctrl+C to stop the server
echo.

MT5WebAPI.exe --host localhost --port 8080

echo.
echo Server stopped.
pause
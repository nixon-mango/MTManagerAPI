@echo off
echo ========================================
echo   MT5 Web API - Smart Network Startup
echo ========================================
echo.

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Detecting your network configuration...

REM Get the local IP address
for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /c:"IPv4 Address"') do (
    set LOCAL_IP=%%a
    set LOCAL_IP=!LOCAL_IP: =!
    goto :found_ip
)

:found_ip
if "%LOCAL_IP%"=="" (
    echo ❌ Could not detect your IP address
    echo Using localhost only...
    set BIND_HOST=localhost
) else (
    echo ✓ Detected IP address: %LOCAL_IP%
    set BIND_HOST=%LOCAL_IP%
)

echo.
echo Attempting to start MT5 Web API...
echo.

REM Try to start with detected IP first
echo Trying to bind to %BIND_HOST%:8080...
MT5WebAPI.exe --host %BIND_HOST% --port 8080

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ❌ Failed to bind to %BIND_HOST%
    echo.
    echo Falling back to localhost only...
    echo.
    MT5WebAPI.exe --host localhost --port 8080
)

echo.
echo Server stopped.
pause
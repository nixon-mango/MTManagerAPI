@echo off
echo ========================================
echo   MT5 Web API - Secure Mode
echo ========================================
echo.

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Starting MT5 Web API in secure mode...
echo.
echo Security features:
echo   ✓ API Key authentication required
echo   ✓ Request logging enabled
echo   ✓ Origin validation (if configured)
echo.
echo Make sure you have:
echo   1. Generated an API key (use generate-api-key.bat)
echo   2. Updated App.config with your key
echo   3. Set RequireApiKey=true in App.config
echo.

REM Check if App.config has security enabled
findstr /C:"RequireApiKey.*true" App.config >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ⚠️  WARNING: RequireApiKey not set to true in App.config
    echo.
    echo To enable security:
    echo 1. Run generate-api-key.bat to get a key
    echo 2. Edit App.config and set:
    echo    ^<add key="RequireApiKey" value="true" /^>
    echo    ^<add key="ApiKeys" value="YOUR_KEY" /^>
    echo.
    echo Starting server anyway...
    echo.
)

echo Press Ctrl+C to stop the server
echo ========================================
echo.

MT5WebAPI.exe --host 0.0.0.0 --port 8080

echo.
echo Server stopped.
pause
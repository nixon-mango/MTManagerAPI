@echo off
echo ========================================
echo   MT5 Web API - Generate API Key
echo ========================================
echo.

cd /d "%~dp0\MT5WebAPI\bin\Debug"

echo Generating a new secure API key...
echo.

MT5WebAPI.exe --generate-key

echo.
echo Next steps:
echo 1. Copy the generated key above
echo 2. Edit MT5WebAPI\App.config
echo 3. Add the key to the ApiKeys setting
echo 4. Set RequireApiKey to true
echo 5. Restart the server
echo.
echo Example App.config:
echo   ^<add key="RequireApiKey" value="true" /^>
echo   ^<add key="ApiKeys" value="YOUR_GENERATED_KEY" /^>
echo.
pause
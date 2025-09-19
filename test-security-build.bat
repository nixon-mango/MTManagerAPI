@echo off
echo ========================================
echo   MT5 Web API - Security Build Test
echo ========================================
echo.

echo Testing if security implementation builds correctly...
echo.

REM Build the solution
echo Building solution...
call build.bat

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Build successful! Security features are ready.
    echo.
    echo Next steps to enable security:
    echo 1. Generate API key: generate-api-key.bat
    echo 2. Update App.config with the key
    echo 3. Set RequireApiKey=true in App.config
    echo 4. Start server: MT5WebAPI.exe --host YOUR_STATIC_IP --port 8080
    echo.
    echo Test security:
    echo   .\test-api-security.ps1 -ApiUrl http://YOUR_STATIC_IP:8080 -ApiKey YOUR_KEY
    echo.
) else (
    echo.
    echo ❌ Build failed. Check the error messages above.
    echo.
    echo Common issues:
    echo • Missing SecurityConfig.cs in project file
    echo • Missing System.Configuration reference
    echo • Namespace issues
    echo.
)

pause
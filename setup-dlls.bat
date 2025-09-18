@echo off
echo Setting up MT5 Manager API DLLs...

REM Create directories if they don't exist
if not exist "MT5ManagerAPI\bin\Debug" mkdir "MT5ManagerAPI\bin\Debug"
if not exist "MT5ConsoleApp\bin\Debug" mkdir "MT5ConsoleApp\bin\Debug"
if not exist "MT5WebAPI\bin\Debug" mkdir "MT5WebAPI\bin\Debug"

REM Check if DLLs directory exists
if not exist "DLLs" (
    echo.
    echo Please create a 'DLLs' directory and place your MT5 Manager API DLL files there.
    echo Required files:
    echo   - MetaQuotes.MT5CommonAPI.dll ^(or MetaQuotes.MT5CommonAPI64.dll^)
    echo   - MetaQuotes.MT5GatewayAPI.dll ^(or MetaQuotes.MT5GatewayAPI64.dll^)
    echo   - MetaQuotes.MT5ManagerAPI.dll ^(or MetaQuotes.MT5ManagerAPI64.dll^)
    echo   - MetaQuotes.MT5WebAPI.dll
    echo   - MT5APIGateway.dll ^(or MT5APIGateway64.dll^)
    echo   - MT5APIManager.dll ^(or MT5APIManager64.dll^)
    echo.
    pause
    exit /b 1
)

REM Copy DLLs to all project directories
echo Copying DLLs to MT5ManagerAPI...
copy "DLLs\*.dll" "MT5ManagerAPI\bin\Debug\" >nul 2>&1

echo Copying DLLs to MT5ConsoleApp...
copy "DLLs\*.dll" "MT5ConsoleApp\bin\Debug\" >nul 2>&1

echo Copying DLLs to MT5WebAPI...
copy "DLLs\*.dll" "MT5WebAPI\bin\Debug\" >nul 2>&1

echo.
echo ✓ DLL setup completed!
echo.
echo You can now run the applications:
echo   - MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
echo   - MT5WebAPI\bin\Debug\MT5WebAPI.exe
echo.
pause
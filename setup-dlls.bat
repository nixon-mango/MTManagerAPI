@echo off
echo Setting up MT5 Manager API DLLs...

REM Create directories if they don't exist
if not exist "MT5ManagerAPI\bin\Debug" mkdir "MT5ManagerAPI\bin\Debug"
if not exist "MT5ConsoleApp\bin\Debug" mkdir "MT5ConsoleApp\bin\Debug"
if not exist "MT5WebAPI\bin\Debug" mkdir "MT5WebAPI\bin\Debug"

REM Check if DLLs directory exists
if not exist "DLLs" (
    echo.
    echo ERROR: 'DLLs' directory not found!
    echo.
    echo Please create a 'DLLs' directory in the root folder and place your MT5 Manager API DLL files there.
    echo.
    echo Required files ^(choose 32-bit OR 64-bit versions consistently^):
    echo.
    echo .NET Wrapper DLLs:
    echo   - MetaQuotes.MT5CommonAPI.dll ^(or MetaQuotes.MT5CommonAPI64.dll^)
    echo   - MetaQuotes.MT5GatewayAPI.dll ^(or MetaQuotes.MT5GatewayAPI64.dll^)
    echo   - MetaQuotes.MT5ManagerAPI.dll ^(or MetaQuotes.MT5ManagerAPI64.dll^)
    echo   - MetaQuotes.MT5WebAPI.dll
    echo.
    echo Native DLLs:
    echo   - MT5APIGateway.dll ^(or MT5APIGateway64.dll^)
    echo   - MT5APIManager.dll ^(or MT5APIManager64.dll^)
    echo.
    echo Example structure:
    echo   DLLs\
    echo   ├── MetaQuotes.MT5CommonAPI.dll
    echo   ├── MetaQuotes.MT5GatewayAPI.dll
    echo   ├── MetaQuotes.MT5ManagerAPI.dll
    echo   ├── MetaQuotes.MT5WebAPI.dll
    echo   ├── MT5APIGateway.dll
    echo   └── MT5APIManager.dll
    echo.
    pause
    exit /b 1
)

echo Found DLLs directory. Checking for required files...

REM Check for required DLL files
set MISSING_DLLS=0

if not exist "DLLs\MetaQuotes.MT5CommonAPI.dll" if not exist "DLLs\MetaQuotes.MT5CommonAPI64.dll" (
    echo ✗ Missing: MetaQuotes.MT5CommonAPI.dll ^(or 64-bit version^)
    set MISSING_DLLS=1
)

if not exist "DLLs\MetaQuotes.MT5ManagerAPI.dll" if not exist "DLLs\MetaQuotes.MT5ManagerAPI64.dll" (
    echo ✗ Missing: MetaQuotes.MT5ManagerAPI.dll ^(or 64-bit version^)
    set MISSING_DLLS=1
)

if %MISSING_DLLS%==1 (
    echo.
    echo ERROR: Required DLL files are missing. Please add them to the DLLs directory.
    pause
    exit /b 1
)

echo ✓ Required DLL files found.
echo.

REM Copy DLLs to all project directories
echo Copying DLLs to MT5ManagerAPI...
copy "DLLs\*.dll" "MT5ManagerAPI\bin\Debug\" >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ✗ Failed to copy DLLs to MT5ManagerAPI
) else (
    echo ✓ DLLs copied to MT5ManagerAPI
)

echo Copying DLLs to MT5ConsoleApp...
copy "DLLs\*.dll" "MT5ConsoleApp\bin\Debug\" >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ✗ Failed to copy DLLs to MT5ConsoleApp
) else (
    echo ✓ DLLs copied to MT5ConsoleApp
)

echo Copying DLLs to MT5WebAPI...
copy "DLLs\*.dll" "MT5WebAPI\bin\Debug\" >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ✗ Failed to copy DLLs to MT5WebAPI
) else (
    echo ✓ DLLs copied to MT5WebAPI
)

echo.
echo ✓ DLL setup completed successfully!
echo.
echo Next steps:
echo 1. Run 'build.bat' to compile the solution
echo 2. Test with: MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
echo 3. Or run Web API: MT5WebAPI\bin\Debug\MT5WebAPI.exe
echo.
pause
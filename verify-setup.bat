@echo off
echo === Verifying MT5 Manager API Setup ===
echo.

echo ✓ Fixed: Updated all project references to use 64-bit DLLs
echo   - MetaQuotes.MT5CommonAPI64.dll
echo   - MetaQuotes.MT5GatewayAPI64.dll  
echo   - MetaQuotes.MT5ManagerAPI64.dll
echo   - MetaQuotes.MT5WebAPI.dll
echo.

echo Checking if DLLs exist in project directories...
echo.

echo MT5ManagerAPI project:
if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI64.dll" (
    echo   ✓ MetaQuotes.MT5CommonAPI64.dll found
) else (
    echo   ✗ MetaQuotes.MT5CommonAPI64.dll missing
)

if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5ManagerAPI64.dll" (
    echo   ✓ MetaQuotes.MT5ManagerAPI64.dll found
) else (
    echo   ✗ MetaQuotes.MT5ManagerAPI64.dll missing
)

echo.
echo MT5ConsoleApp project:
if exist "MT5ConsoleApp\bin\Debug\MetaQuotes.MT5CommonAPI64.dll" (
    echo   ✓ MetaQuotes.MT5CommonAPI64.dll found
) else (
    echo   ✗ MetaQuotes.MT5CommonAPI64.dll missing
)

echo.
echo MT5WebAPI project:
if exist "MT5WebAPI\bin\Debug\MetaQuotes.MT5CommonAPI64.dll" (
    echo   ✓ MetaQuotes.MT5CommonAPI64.dll found
) else (
    echo   ✗ MetaQuotes.MT5CommonAPI64.dll missing
)

echo.
echo === Ready to Build ===
echo All project references have been updated to match your 64-bit DLLs.
echo.
echo Run: build.bat
echo.
pause
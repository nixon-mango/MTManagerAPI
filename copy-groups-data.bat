@echo off
echo === Copying MT5 Groups Data to Debug Folders ===
echo.

REM Create debug directories if they don't exist
if not exist "MT5ManagerAPI\bin\Debug" mkdir "MT5ManagerAPI\bin\Debug"
if not exist "MT5WebAPI\bin\Debug" mkdir "MT5WebAPI\bin\Debug"
if not exist "MT5ConsoleApp\bin\Debug" mkdir "MT5ConsoleApp\bin\Debug"

REM Copy the comprehensive groups JSON file to all debug folders
if exist "complete_mt5_groups.json" (
    echo Copying complete_mt5_groups.json to debug folders...
    copy "complete_mt5_groups.json" "MT5ManagerAPI\bin\Debug\" >nul
    copy "complete_mt5_groups.json" "MT5WebAPI\bin\Debug\" >nul
    copy "complete_mt5_groups.json" "MT5ConsoleApp\bin\Debug\" >nul
    echo ✓ Groups data copied to all debug folders
) else (
    echo ✗ complete_mt5_groups.json not found in current directory
    echo   Please ensure the file exists in the workspace root
)

echo.
echo Debug folder contents:
echo.
echo MT5ManagerAPI\bin\Debug:
if exist "MT5ManagerAPI\bin\Debug\complete_mt5_groups.json" (
    echo   ✓ complete_mt5_groups.json [%~zMT5ManagerAPI\bin\Debug\complete_mt5_groups.json% bytes]
) else (
    echo   ✗ complete_mt5_groups.json missing
)

echo.
echo MT5WebAPI\bin\Debug:
if exist "MT5WebAPI\bin\Debug\complete_mt5_groups.json" (
    echo   ✓ complete_mt5_groups.json [%~zMT5WebAPI\bin\Debug\complete_mt5_groups.json% bytes]
) else (
    echo   ✗ complete_mt5_groups.json missing
)

echo.
echo MT5ConsoleApp\bin\Debug:
if exist "MT5ConsoleApp\bin\Debug\complete_mt5_groups.json" (
    echo   ✓ complete_mt5_groups.json [%~zMT5ConsoleApp\bin\Debug\complete_mt5_groups.json% bytes]
) else (
    echo   ✗ complete_mt5_groups.json missing
)

echo.
echo === Copy operation completed ===
pause
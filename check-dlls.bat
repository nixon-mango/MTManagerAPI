@echo off
echo Checking MT5 Manager API DLL setup...
echo.

echo === Checking DLLs directory ===
if exist "DLLs" (
    echo ✓ DLLs directory exists
    dir "DLLs\*.dll" /b
) else (
    echo ✗ DLLs directory not found
)

echo.
echo === Checking MT5ManagerAPI project DLLs ===
if exist "MT5ManagerAPI\bin\Debug" (
    echo ✓ MT5ManagerAPI\bin\Debug exists
    dir "MT5ManagerAPI\bin\Debug\*.dll" /b 2>nul
    if %ERRORLEVEL% neq 0 echo   (No DLL files found)
) else (
    echo ✗ MT5ManagerAPI\bin\Debug not found
)

echo.
echo === Checking specific required DLLs ===
if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI.dll" (
    echo ✓ MetaQuotes.MT5CommonAPI.dll found (32-bit)
) else if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI64.dll" (
    echo ✓ MetaQuotes.MT5CommonAPI64.dll found (64-bit)
) else (
    echo ✗ MetaQuotes.MT5CommonAPI.dll not found
)

if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5ManagerAPI.dll" (
    echo ✓ MetaQuotes.MT5ManagerAPI.dll found (32-bit)
) else if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5ManagerAPI64.dll" (
    echo ✓ MetaQuotes.MT5ManagerAPI64.dll found (64-bit)
) else (
    echo ✗ MetaQuotes.MT5ManagerAPI.dll not found
)

echo.
echo === Architecture Detection ===
if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI64.dll" (
    echo Detected: 64-bit DLLs
    set ARCH=64
) else if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI.dll" (
    echo Detected: 32-bit DLLs
    set ARCH=32
) else (
    echo Cannot detect architecture - no MetaQuotes.MT5CommonAPI DLL found
    set ARCH=unknown
)

echo.
pause
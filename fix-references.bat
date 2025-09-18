@echo off
echo Fixing MT5 Manager API project references...
echo.

REM Check if we have 32-bit or 64-bit DLLs
if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI64.dll" (
    echo Detected 64-bit DLLs - updating project references...
    set ARCH=64
) else if exist "MT5ManagerAPI\bin\Debug\MetaQuotes.MT5CommonAPI.dll" (
    echo Detected 32-bit DLLs - updating project references...
    set ARCH=32
) else (
    echo ERROR: No MetaQuotes.MT5CommonAPI DLL found!
    echo Please ensure DLLs are copied to MT5ManagerAPI\bin\Debug\
    pause
    exit /b 1
)

REM Create a backup of the original project file
if not exist "MT5ManagerAPI\MT5ManagerAPI.csproj.backup" (
    copy "MT5ManagerAPI\MT5ManagerAPI.csproj" "MT5ManagerAPI\MT5ManagerAPI.csproj.backup" >nul
    echo ✓ Backup created: MT5ManagerAPI.csproj.backup
)

REM Update references based on architecture
if "%ARCH%"=="64" (
    echo Updating references for 64-bit DLLs...
    powershell -Command "(Get-Content 'MT5ManagerAPI\MT5ManagerAPI.csproj') -replace 'MetaQuotes\.MT5CommonAPI\.dll', 'MetaQuotes.MT5CommonAPI64.dll' -replace 'MetaQuotes\.MT5GatewayAPI\.dll', 'MetaQuotes.MT5GatewayAPI64.dll' -replace 'MetaQuotes\.MT5ManagerAPI\.dll', 'MetaQuotes.MT5ManagerAPI64.dll' | Set-Content 'MT5ManagerAPI\MT5ManagerAPI.csproj'"
) else (
    echo Updating references for 32-bit DLLs...
    powershell -Command "(Get-Content 'MT5ManagerAPI\MT5ManagerAPI.csproj') -replace 'MetaQuotes\.MT5CommonAPI64\.dll', 'MetaQuotes.MT5CommonAPI.dll' -replace 'MetaQuotes\.MT5GatewayAPI64\.dll', 'MetaQuotes.MT5GatewayAPI.dll' -replace 'MetaQuotes\.MT5ManagerAPI64\.dll', 'MetaQuotes.MT5ManagerAPI.dll' | Set-Content 'MT5ManagerAPI\MT5ManagerAPI.csproj'"
)

echo ✓ Project references updated for %ARCH%-bit DLLs
echo.
echo Now try building again with: build.bat
echo.
pause
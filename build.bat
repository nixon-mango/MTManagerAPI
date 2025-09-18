@echo off
echo Building MT5 Manager API Solution...

REM Check if MSBuild is available
where msbuild >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo MSBuild not found in PATH. Please ensure Visual Studio or Build Tools are installed.
    pause
    exit /b 1
)

REM Build the solution
msbuild MT5ManagerAPI.sln /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

if %ERRORLEVEL% equ 0 (
    echo.
    echo ✓ Build completed successfully!
    echo.
    echo Next steps:
    echo 1. Copy your MT5 Manager API DLL files to the bin\Debug directories
    echo 2. Run setup-dlls.bat to copy DLLs to all projects
    echo 3. Test with MT5ConsoleApp.exe
) else (
    echo.
    echo ✗ Build failed. Please check the error messages above.
)

pause
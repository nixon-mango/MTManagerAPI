@echo off
echo Building MT5 Manager API Solution...

REM Set MSBuild path
set MSBUILD_PATH="C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"

REM Check if MSBuild exists at the specified path
if not exist %MSBUILD_PATH% (
    echo MSBuild not found at: %MSBUILD_PATH%
    echo Please ensure Visual Studio Build Tools 2022 are installed.
    pause
    exit /b 1
)

echo Using MSBuild at: %MSBUILD_PATH%
echo.

REM Build the solution
%MSBUILD_PATH% MT5ManagerAPI.sln /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

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
@echo off
echo === Final MT5 Manager API Build Fix ===
echo.

echo Fixed issues:
echo ✓ Changed PlatformTarget from AnyCPU to x64 to match 64-bit DLLs
echo ✓ Fixed invalid GUID formats in AssemblyInfo files
echo ✓ Updated build script to explicitly target x64 platform
echo.

echo Building with corrected settings...
echo.

call build.bat

if %ERRORLEVEL% equ 0 (
    echo.
    echo 🎉 SUCCESS! All projects built successfully!
    echo.
    echo Architecture warnings should be gone now.
    echo Your applications are ready to run:
    echo.
    echo 📱 Console App:
    echo   MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
    echo.
    echo 🌐 Web API Server:
    echo   MT5WebAPI\bin\Debug\MT5WebAPI.exe
    echo.
    echo 📚 Library:
    echo   MT5ManagerAPI\bin\Debug\MT5ManagerAPI.dll
    echo.
    echo All projects now target x64 architecture to match your 64-bit MT5 DLLs.
    echo.
) else (
    echo.
    echo ❌ Build still failed. Check the output above for remaining errors.
    echo.
)

pause
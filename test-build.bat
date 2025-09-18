@echo off
echo === Testing MT5 Manager API Build ===
echo.

echo Fixed compilation errors:
echo ✓ Added explicit cast for EnUsersRights to uint
echo ✓ Handled missing CIMTAccount methods (MarginSOCall, MarginSOSO, Commission, Currency)
echo ✓ Used default values for unavailable properties
echo.

echo Building solution...
echo.

call build.bat

if %ERRORLEVEL% equ 0 (
    echo.
    echo ✅ BUILD SUCCESSFUL!
    echo.
    echo All projects compiled successfully. You can now run:
    echo   - Console App: MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
    echo   - Web API: MT5WebAPI\bin\Debug\MT5WebAPI.exe
    echo.
) else (
    echo.
    echo ❌ BUILD FAILED
    echo.
    echo If there are still errors, they may be due to API version differences.
    echo Check the build output above for specific error details.
    echo.
)

pause
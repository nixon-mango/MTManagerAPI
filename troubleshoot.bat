@echo off
echo === MT5 Manager API Build Troubleshooting ===
echo.

echo 1. Checking DLL files...
call check-dlls.bat

echo.
echo 2. Trying to fix project references...
call fix-references.bat

echo.
echo 3. Testing alternative project file...
echo.
echo Backing up current project file...
if not exist "MT5ManagerAPI\MT5ManagerAPI.csproj.original" (
    copy "MT5ManagerAPI\MT5ManagerAPI.csproj" "MT5ManagerAPI\MT5ManagerAPI.csproj.original" >nul
    echo ✓ Original project file backed up
)

echo Copying alternative project file...
copy "MT5ManagerAPI\MT5ManagerAPI-Alternative.csproj" "MT5ManagerAPI\MT5ManagerAPI.csproj" >nul
echo ✓ Alternative project file in place

echo.
echo 4. Attempting build with alternative project file...
call build.bat

echo.
echo === Troubleshooting completed ===
echo.
echo If build still fails:
echo 1. Check that DLL files are not blocked (right-click → Properties → Unblock)
echo 2. Verify all DLLs are the same architecture (32-bit or 64-bit)
echo 3. Try copying DLLs directly from your MT5 installation
echo 4. Check Windows Event Viewer for additional error details
echo.
pause
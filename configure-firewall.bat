@echo off
echo ========================================
echo   MT5 Web API - Firewall Configuration
echo ========================================
echo.

echo This script will configure Windows Firewall to allow
echo incoming connections to the MT5 Web API on port 8080.
echo.
echo You need to run this as Administrator!
echo.
pause

echo Adding firewall rule for MT5 Web API...
netsh advfirewall firewall add rule name="MT5WebAPI-Port8080" dir=in action=allow protocol=TCP localport=8080

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Firewall rule added successfully!
    echo.
    echo The MT5 Web API is now allowed through Windows Firewall.
    echo You can now access the API from other machines on your network.
    echo.
) else (
    echo.
    echo ❌ Failed to add firewall rule.
    echo.
    echo Please run this script as Administrator:
    echo 1. Right-click on this batch file
    echo 2. Select "Run as administrator"
    echo.
)

echo Current firewall rules for MT5WebAPI:
netsh advfirewall firewall show rule name="MT5WebAPI-Port8080"

echo.
pause
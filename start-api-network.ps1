# MT5 Web API Network Startup Script
param(
    [string]$Port = "8080",
    [switch]$AsAdmin,
    [switch]$Help
)

if ($Help) {
    Write-Host "MT5 Web API Network Startup Script" -ForegroundColor Green
    Write-Host "===================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\start-api-network.ps1 [-Port 8080] [-AsAdmin]"
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  -Port     Port number (default: 8080)"
    Write-Host "  -AsAdmin  Force Administrator mode (binds to 0.0.0.0)"
    Write-Host "  -Help     Show this help"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\start-api-network.ps1                    # Auto-detect best binding"
    Write-Host "  .\start-api-network.ps1 -Port 8081        # Use different port"
    Write-Host "  .\start-api-network.ps1 -AsAdmin          # Force 0.0.0.0 binding"
    exit
}

Write-Host "🌐 MT5 Web API Network Startup" -ForegroundColor Green
Write-Host "===============================" -ForegroundColor Green
Write-Host ""

# Change to Web API directory
$webApiPath = Join-Path $PSScriptRoot "MT5WebAPI\bin\Debug"
if (-not (Test-Path $webApiPath)) {
    Write-Host "❌ MT5WebAPI.exe not found at: $webApiPath" -ForegroundColor Red
    Write-Host "   Make sure you've built the solution first." -ForegroundColor Yellow
    exit 1
}

Set-Location $webApiPath

# Detect local IP address
Write-Host "🔍 Detecting network configuration..." -ForegroundColor Yellow

try {
    $localIP = (Get-NetIPAddress -AddressFamily IPv4 -InterfaceAlias "Ethernet*" | Where-Object {$_.IPAddress -ne "127.0.0.1"})[0].IPAddress
    if (-not $localIP) {
        $localIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -like "192.168.*" -or $_.IPAddress -like "10.*" -or $_.IPAddress -like "172.*"})[0].IPAddress
    }
} catch {
    $localIP = $null
}

# Determine binding strategy
if ($AsAdmin) {
    $bindHost = "0.0.0.0"
    $requiresAdmin = $true
} elseif ($localIP) {
    $bindHost = $localIP
    $requiresAdmin = $false
} else {
    $bindHost = "localhost"
    $requiresAdmin = $false
}

Write-Host "✓ Network configuration detected:" -ForegroundColor Green
if ($localIP) {
    Write-Host "   Your IP address: $localIP" -ForegroundColor Cyan
}
Write-Host "   Binding to: $bindHost" -ForegroundColor Cyan
Write-Host "   Port: $Port" -ForegroundColor Cyan

if ($requiresAdmin) {
    # Check if running as Administrator
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
    
    if (-not $isAdmin) {
        Write-Host ""
        Write-Host "⚠️  Administrator privileges required for 0.0.0.0 binding" -ForegroundColor Yellow
        Write-Host "   Restarting script as Administrator..." -ForegroundColor Yellow
        
        $arguments = "-File `"$($MyInvocation.MyCommand.Path)`" -Port $Port -AsAdmin"
        Start-Process PowerShell -ArgumentList $arguments -Verb RunAs
        exit
    }
}

Write-Host ""
Write-Host "🚀 Starting MT5 Web API Server..." -ForegroundColor Green
Write-Host ""

if ($bindHost -eq "0.0.0.0") {
    Write-Host "The API will be accessible from:" -ForegroundColor Cyan
    Write-Host "  - Local machine: http://localhost:$Port" -ForegroundColor White
    Write-Host "  - Static IP: http://$localIP:$Port" -ForegroundColor White
    Write-Host "  - Network: http://$localIP:$Port (from other machines)" -ForegroundColor White
} elseif ($bindHost -eq "localhost") {
    Write-Host "The API will be accessible from:" -ForegroundColor Cyan
    Write-Host "  - Local machine only: http://localhost:$Port" -ForegroundColor White
} else {
    Write-Host "The API will be accessible from:" -ForegroundColor Cyan
    Write-Host "  - Local machine: http://localhost:$Port" -ForegroundColor White
    Write-Host "  - Static IP: http://${bindHost}:$Port" -ForegroundColor White
    Write-Host "  - Network: http://${bindHost}:$Port (from other machines)" -ForegroundColor White
}

Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host "===============================" -ForegroundColor Green
Write-Host ""

try {
    # Start the server
    $process = Start-Process -FilePath ".\MT5WebAPI.exe" -ArgumentList "--host $bindHost --port $Port" -NoNewWindow -PassThru
    
    # Wait for the process to exit
    $process.WaitForExit()
    
    Write-Host ""
    Write-Host "Server stopped." -ForegroundColor Yellow
} catch {
    Write-Host ""
    Write-Host "❌ Error starting server: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "   1. Try running as Administrator" -ForegroundColor Gray
    Write-Host "   2. Check if port $Port is already in use: netstat -an | findstr :$Port" -ForegroundColor Gray
    Write-Host "   3. Try a different port: .\start-api-network.ps1 -Port 8081" -ForegroundColor Gray
    Write-Host "   4. Use localhost only: MT5WebAPI.exe --host localhost --port $Port" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
# Test script to verify external access to MT5 Web API
param(
    [string]$StaticIP = "",
    [int]$Port = 8080,
    [switch]$Help
)

if ($Help -or $StaticIP -eq "") {
    Write-Host "MT5 Web API External Access Test Script" -ForegroundColor Green
    Write-Host "=======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\test-external-access.ps1 -StaticIP YOUR_STATIC_IP [-Port 8080]"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\test-external-access.ps1 -StaticIP 192.168.1.100"
    Write-Host "  .\test-external-access.ps1 -StaticIP 192.168.1.100 -Port 8080"
    Write-Host ""
    Write-Host "To find your static IP address, run: ipconfig"
    exit
}

$baseUrl = "http://${StaticIP}:${Port}"

Write-Host "🌐 Testing MT5 Web API External Access" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green
Write-Host "Target: $baseUrl" -ForegroundColor Cyan
Write-Host ""

# Test basic connectivity
Write-Host "🔍 Testing basic connectivity..." -ForegroundColor Yellow
try {
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    $tcpClient.Connect($StaticIP, $Port)
    $tcpClient.Close()
    Write-Host "✅ Port $Port is open and accessible" -ForegroundColor Green
} catch {
    Write-Host "❌ Cannot connect to ${StaticIP}:${Port}" -ForegroundColor Red
    Write-Host "   Make sure:" -ForegroundColor Yellow
    Write-Host "   1. MT5WebAPI server is running with --host 0.0.0.0" -ForegroundColor Yellow
    Write-Host "   2. Windows Firewall allows port $Port" -ForegroundColor Yellow
    Write-Host "   3. Your static IP address is correct" -ForegroundColor Yellow
    exit 1
}

# Test API endpoints
Write-Host ""
Write-Host "🧪 Testing API endpoints..." -ForegroundColor Yellow

$endpoints = @(
    @{ Path = "/api/status"; Description = "API Status" },
    @{ Path = "/api/users/stats"; Description = "User Discovery Statistics" },
    @{ Path = "/api/users/real"; Description = "Real Users" }
)

$successCount = 0
$totalTests = $endpoints.Count

foreach ($endpoint in $endpoints) {
    try {
        Write-Host "   Testing $($endpoint.Description)..." -NoNewline
        $response = Invoke-RestMethod -Uri "$baseUrl$($endpoint.Path)" -Method GET -TimeoutSec 10
        
        if ($response.success -eq $true) {
            Write-Host " ✅ Success" -ForegroundColor Green
            $successCount++
        } elseif ($response.success -eq $false) {
            Write-Host " ⚠️  API Error: $($response.error)" -ForegroundColor Yellow
        } else {
            Write-Host " ✅ Response received" -ForegroundColor Green
            $successCount++
        }
    }
    catch {
        Write-Host " ❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Summary
Write-Host ""
Write-Host "📊 Test Results Summary" -ForegroundColor Cyan
Write-Host "=======================" -ForegroundColor Cyan
Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Successful: $successCount" -ForegroundColor Green
Write-Host "Failed: $($totalTests - $successCount)" -ForegroundColor Red

if ($successCount -eq $totalTests) {
    Write-Host ""
    Write-Host "🎉 All tests passed! Your API is accessible externally." -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now access your API from other machines using:" -ForegroundColor Cyan
    Write-Host "  $baseUrl" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "⚠️  Some tests failed. Common issues:" -ForegroundColor Yellow
    Write-Host "   • Server not running or not bound to 0.0.0.0" -ForegroundColor Gray
    Write-Host "   • Windows Firewall blocking port $Port" -ForegroundColor Gray
    Write-Host "   • Not connected to MT5 server yet" -ForegroundColor Gray
    Write-Host ""
    Write-Host "💡 To fix firewall issues, run as Administrator:" -ForegroundColor Cyan
    Write-Host "   configure-firewall.bat" -ForegroundColor White
}

Write-Host ""
Write-Host "🔗 Next steps:" -ForegroundColor Cyan
Write-Host "• Update your client applications to use: $baseUrl" -ForegroundColor Gray
Write-Host "• Test from other machines on your network" -ForegroundColor Gray
Write-Host "• Consider security measures for production use" -ForegroundColor Gray
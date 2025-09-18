# Test script for MT5 Manager API Swagger specification
# This script validates the Swagger file and tests API endpoints

param(
    [string]$ApiUrl = "http://localhost:8080",
    [string]$SwaggerFile = "swagger.yaml"
)

Write-Host "🚀 MT5 Manager API Swagger Test Script" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

# Check if Swagger file exists
if (-not (Test-Path $SwaggerFile)) {
    Write-Host "❌ Swagger file not found: $SwaggerFile" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Swagger file found: $SwaggerFile" -ForegroundColor Green

# Function to test API endpoint
function Test-ApiEndpoint {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [string]$Body = $null,
        [string]$Description
    )
    
    Write-Host "🔍 Testing: $Description" -ForegroundColor Yellow
    Write-Host "   URL: $Method $Url" -ForegroundColor Gray
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
            "Accept" = "application/json"
        }
        
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $headers
            TimeoutSec = 10
        }
        
        if ($Body) {
            $params.Body = $Body
        }
        
        $response = Invoke-RestMethod @params
        
        if ($response.success -eq $true) {
            Write-Host "   ✅ Success" -ForegroundColor Green
        } elseif ($response.success -eq $false) {
            Write-Host "   ⚠️  API Error: $($response.error)" -ForegroundColor Yellow
        } else {
            Write-Host "   ✅ Response received" -ForegroundColor Green
        }
        
        return $true
    }
    catch {
        Write-Host "   ❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Test if server is running
Write-Host "🔍 Checking if MT5 Web API server is running..." -ForegroundColor Cyan
$serverRunning = Test-ApiEndpoint -Url "$ApiUrl/api/status" -Description "Server Status Check"

if (-not $serverRunning) {
    Write-Host ""
    Write-Host "❌ MT5 Web API server is not running on $ApiUrl" -ForegroundColor Red
    Write-Host "💡 To start the server, run:" -ForegroundColor Yellow
    Write-Host "   MT5WebAPI\bin\Debug\MT5WebAPI.exe --host localhost --port 8080" -ForegroundColor Gray
    Write-Host ""
    Write-Host "📚 Once the server is running, you can:" -ForegroundColor Cyan
    Write-Host "   • View Swagger UI: docs/swagger-ui.html" -ForegroundColor Gray
    Write-Host "   • Test endpoints manually with curl or Postman" -ForegroundColor Gray
    Write-Host "   • Use the Interactive Playground: docs/PLAYGROUND.md" -ForegroundColor Gray
    exit 1
}

Write-Host ""
Write-Host "🎉 Server is running! Testing API endpoints..." -ForegroundColor Green
Write-Host ""

# Test various endpoints
$tests = @(
    @{
        Url = "$ApiUrl/api/status"
        Method = "GET"
        Description = "Get API Status"
    },
    @{
        Url = "$ApiUrl/api/connect"
        Method = "POST"
        Body = '{"server":"demo.server.com:443","login":12345,"password":"test"}'
        Description = "Connect to MT5 Server (will fail without valid credentials)"
    },
    @{
        Url = "$ApiUrl/api/user/12345"
        Method = "GET"
        Description = "Get User Info (will fail if not connected)"
    },
    @{
        Url = "$ApiUrl/api/account/12345"
        Method = "GET"
        Description = "Get Account Info (will fail if not connected)"
    },
    @{
        Url = "$ApiUrl/api/users"
        Method = "GET"
        Description = "Get All Users (will fail if not connected)"
    },
    @{
        Url = "$ApiUrl/api/group/demo/users"
        Method = "GET"
        Description = "Get Users in Group (will fail if not connected)"
    }
)

$successCount = 0
$totalTests = $tests.Count

foreach ($test in $tests) {
    $result = Test-ApiEndpoint -Url $test.Url -Method $test.Method -Body $test.Body -Description $test.Description
    if ($result) { $successCount++ }
    Write-Host ""
}

# Summary
Write-Host "📊 Test Summary" -ForegroundColor Cyan
Write-Host "===============" -ForegroundColor Cyan
Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Successful: $successCount" -ForegroundColor Green
Write-Host "Failed: $($totalTests - $successCount)" -ForegroundColor Red

if ($successCount -eq $totalTests) {
    Write-Host ""
    Write-Host "🎉 All tests passed! Your API is working correctly." -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "⚠️  Some tests failed. This is expected if:" -ForegroundColor Yellow
    Write-Host "   • You haven't connected to an MT5 server yet" -ForegroundColor Gray
    Write-Host "   • You don't have valid Manager credentials" -ForegroundColor Gray
    Write-Host "   • The test user/group doesn't exist" -ForegroundColor Gray
}

Write-Host ""
Write-Host "📚 Next Steps:" -ForegroundColor Cyan
Write-Host "• Open docs/swagger-ui.html in your browser to explore the API" -ForegroundColor Gray
Write-Host "• Try the Interactive Playground: docs/PLAYGROUND.md" -ForegroundColor Gray
Write-Host "• Read the complete API documentation: docs/API.md" -ForegroundColor Gray
Write-Host ""
Write-Host "🔗 Swagger Resources:" -ForegroundColor Cyan
Write-Host "• YAML Specification: swagger.yaml" -ForegroundColor Gray
Write-Host "• JSON Specification: swagger.json" -ForegroundColor Gray
Write-Host "• Interactive UI: docs/swagger-ui.html" -ForegroundColor Gray
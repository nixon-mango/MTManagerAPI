# Test script for MT5 Web API security features
param(
    [string]$ApiUrl = "http://localhost:8080",
    [string]$ApiKey = "",
    [switch]$Help
)

if ($Help) {
    Write-Host "MT5 Web API Security Test Script" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\test-api-security.ps1 -ApiUrl http://YOUR_IP:8080 -ApiKey YOUR_API_KEY"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\test-api-security.ps1                                    # Test localhost without key"
    Write-Host "  .\test-api-security.ps1 -ApiKey abc123                     # Test localhost with key"
    Write-Host "  .\test-api-security.ps1 -ApiUrl http://192.168.1.100:8080  # Test static IP"
    exit
}

Write-Host "🔐 MT5 Web API Security Test" -ForegroundColor Green
Write-Host "============================" -ForegroundColor Green
Write-Host "Target: $ApiUrl" -ForegroundColor Cyan
Write-Host ""

# Test 1: Check if server is running
Write-Host "🔍 Test 1: Server Connectivity" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$ApiUrl/api/status" -Method GET -TimeoutSec 5 -UseBasicParsing
    Write-Host "✅ Server is running (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "❌ Server not accessible: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Make sure the MT5 Web API server is running" -ForegroundColor Yellow
    exit 1
}

# Test 2: Check security status (without API key)
Write-Host ""
Write-Host "🔍 Test 2: Security Status (No API Key)" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/status" -Method GET -TimeoutSec 5
    
    if ($response.success -eq $true) {
        Write-Host "✅ Request succeeded - Security is DISABLED" -ForegroundColor Green
        Write-Host "   API is accessible without authentication" -ForegroundColor Gray
        $securityEnabled = $false
    } else {
        Write-Host "⚠️  Unexpected response format" -ForegroundColor Yellow
        $securityEnabled = $false
    }
} catch {
    if ($_.Exception.Message -like "*401*" -or $_.Exception.Message -like "*Unauthorized*") {
        Write-Host "🔒 Request failed (401) - Security is ENABLED" -ForegroundColor Cyan
        Write-Host "   API key authentication is required" -ForegroundColor Gray
        $securityEnabled = $true
    } else {
        Write-Host "❌ Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
        $securityEnabled = $false
    }
}

# Test 3: Test with API key (if provided)
if ($ApiKey -ne "") {
    Write-Host ""
    Write-Host "🔍 Test 3: Authentication with API Key" -ForegroundColor Yellow
    
    $headers = @{
        "X-API-Key" = $ApiKey
        "Content-Type" = "application/json"
    }
    
    try {
        $response = Invoke-RestMethod -Uri "$ApiUrl/api/status" -Method GET -Headers $headers -TimeoutSec 5
        
        if ($response.success -eq $true) {
            Write-Host "✅ Authentication successful with provided API key" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Unexpected response: $($response.error)" -ForegroundColor Yellow
        }
    } catch {
        if ($_.Exception.Message -like "*401*") {
            Write-Host "❌ Authentication failed - Invalid API key" -ForegroundColor Red
        } else {
            Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    # Test different endpoints with API key
    Write-Host ""
    Write-Host "🔍 Test 4: Testing Secured Endpoints" -ForegroundColor Yellow
    
    $endpoints = @(
        "/api/users/stats",
        "/api/users/real"
    )
    
    foreach ($endpoint in $endpoints) {
        try {
            Write-Host "   Testing $endpoint..." -NoNewline
            $response = Invoke-RestMethod -Uri "$ApiUrl$endpoint" -Method GET -Headers $headers -TimeoutSec 10
            
            if ($response.success -eq $true) {
                Write-Host " ✅ Success" -ForegroundColor Green
            } else {
                Write-Host " ⚠️  API Error: $($response.error)" -ForegroundColor Yellow
            }
        } catch {
            if ($_.Exception.Message -like "*401*") {
                Write-Host " ❌ Authentication failed" -ForegroundColor Red
            } else {
                Write-Host " ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
} else {
    Write-Host ""
    Write-Host "💡 To test API key authentication:" -ForegroundColor Cyan
    Write-Host "   .\test-api-security.ps1 -ApiKey YOUR_API_KEY" -ForegroundColor Gray
}

# Summary
Write-Host ""
Write-Host "📊 Security Test Summary" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

if ($securityEnabled) {
    Write-Host "🔒 Security Status: ENABLED" -ForegroundColor Green
    Write-Host "   API key authentication is required" -ForegroundColor Gray
    Write-Host "   Unauthorized requests will be rejected with 401" -ForegroundColor Gray
} else {
    Write-Host "⚠️  Security Status: DISABLED" -ForegroundColor Yellow
    Write-Host "   API is accessible without authentication" -ForegroundColor Gray
    Write-Host "   Consider enabling security for production use" -ForegroundColor Gray
}

Write-Host ""
Write-Host "🔧 To enable security:" -ForegroundColor Cyan
Write-Host "1. Generate API key: generate-api-key.bat" -ForegroundColor Gray
Write-Host "2. Update App.config with the key" -ForegroundColor Gray
Write-Host "3. Set RequireApiKey=true" -ForegroundColor Gray
Write-Host "4. Restart server" -ForegroundColor Gray

Write-Host ""
Write-Host "📚 Documentation:" -ForegroundColor Cyan
Write-Host "• Security Guide: docs/SECURITY_GUIDE.md" -ForegroundColor Gray
Write-Host "• Swagger UI: docs/swagger-ui.html" -ForegroundColor Gray
# PowerShell script to test the MT5 Web API
param(
    [string]$Server = "localhost:8080",
    [string]$MT5Server = "",
    [string]$Login = "",
    [string]$Password = "",
    [string]$TestUser = ""
)

$BaseUrl = "http://$Server"

Write-Host "=== MT5 Manager API Test Script ===" -ForegroundColor Green
Write-Host

# Function to make HTTP requests
function Invoke-ApiRequest {
    param(
        [string]$Method = "GET",
        [string]$Endpoint,
        [object]$Body = $null
    )
    
    $url = "$BaseUrl$Endpoint"
    $headers = @{ "Content-Type" = "application/json" }
    
    try {
        if ($Body) {
            $jsonBody = $Body | ConvertTo-Json -Depth 10
            $response = Invoke-RestMethod -Uri $url -Method $Method -Headers $headers -Body $jsonBody
        } else {
            $response = Invoke-RestMethod -Uri $url -Method $Method -Headers $headers
        }
        return $response
    }
    catch {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Test server status
Write-Host "1. Testing server status..." -ForegroundColor Yellow
$status = Invoke-ApiRequest -Endpoint "/api/status"
if ($status) {
    Write-Host "✓ Server is running" -ForegroundColor Green
    Write-Host "  Connected: $($status.data.connected)" -ForegroundColor Cyan
} else {
    Write-Host "✗ Server is not responding" -ForegroundColor Red
    exit 1
}

# Connect to MT5 if credentials provided
if ($MT5Server -and $Login -and $Password) {
    Write-Host "`n2. Connecting to MT5 server..." -ForegroundColor Yellow
    $connectRequest = @{
        server = $MT5Server
        login = [uint64]$Login
        password = $Password
    }
    
    $connectResult = Invoke-ApiRequest -Method "POST" -Endpoint "/api/connect" -Body $connectRequest
    if ($connectResult -and $connectResult.success) {
        Write-Host "✓ Connected to MT5 server successfully" -ForegroundColor Green
        Write-Host "  Server: $($connectResult.data.server)" -ForegroundColor Cyan
        Write-Host "  Login: $($connectResult.data.login)" -ForegroundColor Cyan
        
        # Test user information if test user provided
        if ($TestUser) {
            Write-Host "`n3. Testing user information..." -ForegroundColor Yellow
            $userInfo = Invoke-ApiRequest -Endpoint "/api/user/$TestUser"
            if ($userInfo -and $userInfo.success) {
                Write-Host "✓ User information retrieved" -ForegroundColor Green
                Write-Host "  Name: $($userInfo.data.name)" -ForegroundColor Cyan
                Write-Host "  Group: $($userInfo.data.group)" -ForegroundColor Cyan
                Write-Host "  Email: $($userInfo.data.email)" -ForegroundColor Cyan
            } else {
                Write-Host "✗ Failed to get user information" -ForegroundColor Red
            }
            
            Write-Host "`n4. Testing account information..." -ForegroundColor Yellow
            $accountInfo = Invoke-ApiRequest -Endpoint "/api/account/$TestUser"
            if ($accountInfo -and $accountInfo.success) {
                Write-Host "✓ Account information retrieved" -ForegroundColor Green
                Write-Host "  Balance: $($accountInfo.data.balance)" -ForegroundColor Cyan
                Write-Host "  Equity: $($accountInfo.data.equity)" -ForegroundColor Cyan
                Write-Host "  Margin: $($accountInfo.data.margin)" -ForegroundColor Cyan
            } else {
                Write-Host "✗ Failed to get account information" -ForegroundColor Red
            }
        }
        
        # Disconnect
        Write-Host "`n5. Disconnecting..." -ForegroundColor Yellow
        $disconnectResult = Invoke-ApiRequest -Method "POST" -Endpoint "/api/disconnect"
        if ($disconnectResult -and $disconnectResult.success) {
            Write-Host "✓ Disconnected successfully" -ForegroundColor Green
        }
    } else {
        Write-Host "✗ Failed to connect to MT5 server" -ForegroundColor Red
        if ($connectResult) {
            Write-Host "  Error: $($connectResult.error)" -ForegroundColor Red
        }
    }
} else {
    Write-Host "`nTo test MT5 connection, run with parameters:" -ForegroundColor Yellow
    Write-Host "  .\test-api.ps1 -MT5Server 'your-server' -Login 12345 -Password 'your-password' -TestUser 67890" -ForegroundColor Cyan
}

Write-Host "`nTest completed." -ForegroundColor Green
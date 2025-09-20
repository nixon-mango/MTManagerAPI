# MT5 Group APIs Test Script
# This script demonstrates the new Group management APIs

param(
    [string]$BaseUrl = "http://localhost:8080",
    [string]$ApiKey = "your-api-key-here"
)

Write-Host "🚀 Testing MT5 Group Management APIs" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Cyan
Write-Host ""

# Headers for API requests
$headers = @{
    "X-API-Key" = $ApiKey
    "Content-Type" = "application/json"
}

# Test 1: Get All Groups
Write-Host "📊 Test 1: Get All Groups" -ForegroundColor Yellow
Write-Host "GET $BaseUrl/api/groups" -ForegroundColor Gray
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/groups" -Method GET -Headers $headers
    if ($response.success) {
        Write-Host "✅ Success! Found $($response.data.summary.total_groups) groups" -ForegroundColor Green
        Write-Host "   - Real groups: $($response.data.summary.real_groups)" -ForegroundColor White
        Write-Host "   - Demo groups: $($response.data.summary.demo_groups)" -ForegroundColor White
        Write-Host "   - Manager groups: $($response.data.summary.manager_groups)" -ForegroundColor White
        Write-Host "   - Total users: $($response.data.summary.total_users)" -ForegroundColor White
        
        Write-Host ""
        Write-Host "📋 Available Groups:" -ForegroundColor Cyan
        foreach ($group in $response.data.groups | Select-Object -First 5) {
            Write-Host "   • $($group.name) ($($group.user_count) users, leverage: $($group.leverage))" -ForegroundColor White
        }
        if ($response.data.groups.Count -gt 5) {
            Write-Host "   ... and $($response.data.groups.Count - 5) more groups" -ForegroundColor Gray
        }
        
        # Store first group name for next test
        $firstGroupName = $response.data.groups[0].name
    } else {
        Write-Host "❌ Failed: $($response.error)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=" * 60
Write-Host ""

# Test 2: Get Specific Group Details
if ($firstGroupName) {
    Write-Host "🔍 Test 2: Get Specific Group Details" -ForegroundColor Yellow
    $encodedGroupName = [System.Web.HttpUtility]::UrlEncode($firstGroupName)
    Write-Host "GET $BaseUrl/api/groups/$encodedGroupName" -ForegroundColor Gray
    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/groups/$encodedGroupName" -Method GET -Headers $headers
        if ($response.success) {
            $group = $response.data.group
            $stats = $response.data.statistics
            
            Write-Host "✅ Success! Group details for: $($group.name)" -ForegroundColor Green
            Write-Host "   Description: $($group.description)" -ForegroundColor White
            Write-Host "   Currency: $($group.currency)" -ForegroundColor White
            Write-Host "   Leverage: 1:$($group.leverage)" -ForegroundColor White
            Write-Host "   Margin Call: $($group.margin_call)%" -ForegroundColor White
            Write-Host "   Stop Out: $($group.margin_stop_out)%" -ForegroundColor White
            Write-Host "   Commission: $($group.commission)" -ForegroundColor White
            Write-Host "   Is Demo: $($group.is_demo)" -ForegroundColor White
            Write-Host ""
            Write-Host "📈 Statistics:" -ForegroundColor Cyan
            Write-Host "   Total Users: $($stats.user_count)" -ForegroundColor White
            Write-Host "   Active Users (30 days): $($stats.active_users)" -ForegroundColor White
            Write-Host "   Average Leverage: $([math]::Round($stats.avg_leverage, 2))" -ForegroundColor White
            
            if ($stats.countries -and $stats.countries.Count -gt 0) {
                Write-Host "   Top Countries:" -ForegroundColor White
                foreach ($country in $stats.countries | Select-Object -First 3) {
                    Write-Host "     • $($country.country): $($country.count) users" -ForegroundColor Gray
                }
            }
        } else {
            Write-Host "❌ Failed: $($response.error)" -ForegroundColor Red
        }
    } catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=" * 60
Write-Host ""

# Test 3: Update Group Configuration
if ($firstGroupName) {
    Write-Host "⚙️ Test 3: Update Group Configuration" -ForegroundColor Yellow
    $encodedGroupName = [System.Web.HttpUtility]::UrlEncode($firstGroupName)
    Write-Host "PUT $BaseUrl/api/groups/$encodedGroupName" -ForegroundColor Gray
    
    # Sample update request
    $updateRequest = @{
        description = "Updated via API test - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
        leverage = 250
        margin_call = 75.0
        margin_stop_out = 45.0
        commission = 5.0
    } | ConvertTo-Json
    
    Write-Host "Request Body:" -ForegroundColor Gray
    Write-Host $updateRequest -ForegroundColor DarkGray
    
    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/groups/$encodedGroupName" -Method PUT -Headers $headers -Body $updateRequest
        if ($response.success) {
            Write-Host "✅ Success! Group updated: $($response.data.group_name)" -ForegroundColor Green
            Write-Host "   Updated properties:" -ForegroundColor White
            
            $changes = $response.data.updated_properties
            if ($changes) {
                foreach ($property in $changes.PSObject.Properties) {
                    $change = $property.Value
                    Write-Host "     • $($property.Name): $($change.from) → $($change.to)" -ForegroundColor Cyan
                }
            } else {
                Write-Host "     No changes detected (values may already match)" -ForegroundColor Gray
            }
        } else {
            Write-Host "❌ Failed: $($response.error)" -ForegroundColor Red
        }
    } catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=" * 60
Write-Host ""

# Test 4: Error Handling - Non-existent Group
Write-Host "🚫 Test 4: Error Handling (Non-existent Group)" -ForegroundColor Yellow
Write-Host "GET $BaseUrl/api/groups/nonexistent-group" -ForegroundColor Gray
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/groups/nonexistent-group" -Method GET -Headers $headers
    if (!$response.success) {
        Write-Host "✅ Correct error handling: $($response.error)" -ForegroundColor Green
    } else {
        Write-Host "❌ Unexpected success for non-existent group" -ForegroundColor Red
    }
} catch {
    $errorResponse = $_.Exception.Response
    if ($errorResponse.StatusCode -eq 404 -or $errorResponse.StatusCode -eq 400) {
        Write-Host "✅ Correct HTTP error status: $($errorResponse.StatusCode)" -ForegroundColor Green
    } else {
        Write-Host "❌ Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🎉 Group API Testing Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "📚 Available Endpoints:" -ForegroundColor Cyan
Write-Host "   GET  /api/groups                    - Get all groups with statistics" -ForegroundColor White
Write-Host "   GET  /api/groups/{name}             - Get specific group details" -ForegroundColor White
Write-Host "   PUT  /api/groups/{name}             - Update group configuration" -ForegroundColor White
Write-Host ""
Write-Host "💡 Usage Examples:" -ForegroundColor Cyan
Write-Host "   # Get all groups" -ForegroundColor White
Write-Host "   curl -H 'X-API-Key: your-key' http://localhost:8080/api/groups" -ForegroundColor Gray
Write-Host ""
Write-Host "   # Get specific group" -ForegroundColor White
Write-Host "   curl -H 'X-API-Key: your-key' http://localhost:8080/api/groups/real%5CExecutive" -ForegroundColor Gray
Write-Host ""
Write-Host "   # Update group" -ForegroundColor White
Write-Host "   curl -X PUT -H 'X-API-Key: your-key' -H 'Content-Type: application/json' \\" -ForegroundColor Gray
Write-Host "        -d '{\"leverage\":200,\"margin_call\":70}' \\" -ForegroundColor Gray
Write-Host "        http://localhost:8080/api/groups/real%5CExecutive" -ForegroundColor Gray
Write-Host ""
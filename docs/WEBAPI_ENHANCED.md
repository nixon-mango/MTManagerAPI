# Enhanced Web API - Updated with Discovery Features

## 🚀 **What's New in the Web API**

I've updated the Web API to include the enhanced user discovery functionality with detailed statistics and multiple endpoints for different use cases.

## 📋 **New & Updated Endpoints**

### **1. Enhanced Get All Users** 
```http
GET /api/users
```

**What's New:**
- Now uses enhanced discovery (real groups + login ID patterns)
- Returns users + detailed discovery statistics
- Never fails completely (always returns at least your real users)

**Response Format:**
```json
{
  "success": true,
  "data": {
    "users": [
      {
        "login": 12345,
        "name": "John Executive",
        "group": "real\\Executive",
        "email": "john@example.com",
        "country": "United States",
        "leverage": 100
      }
    ],
    "discovery_stats": {
      "total_users": 1367,
      "from_real_groups": 1247,
      "additional_discovered": 120,
      "groups_found": 6,
      "login_range": "1001 - 150847",
      "discovery_method": "Enhanced discovery using real groups + login ID patterns"
    }
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### **2. User Discovery Statistics Only**
```http
GET /api/users/stats
```

**What it does:**
- Returns only statistics without user data
- Faster than full user retrieval
- Perfect for dashboards and monitoring

**Response Format:**
```json
{
  "success": true,
  "data": {
    "total_users": 1367,
    "from_real_groups": 1247,
    "additional_discovered": 120,
    "groups_found": ["real\\Executive", "real\\Vipin Zero 1000", "real\\NORMAL", "demo", "test"],
    "groups_count": 5,
    "login_range": {
      "min": 1001,
      "max": 150847,
      "range_text": "1001 - 150847"
    },
    "discovery_method": "Enhanced discovery using real groups + login ID patterns",
    "group_breakdown": [
      {"group": "real\\Executive", "count": 456},
      {"group": "real\\Vipin Zero 1000", "count": 389},
      {"group": "real\\NORMAL", "count": 402}
    ],
    "activity_stats": {
      "active_today": 89,
      "active_week": 234,
      "active_month": 567
    }
  }
}
```

### **3. Get All Real Users** (unchanged)
```http
GET /api/users/real
```

## 🎯 **Usage Examples**

### **JavaScript/Web Application**
```javascript
// Get all users with discovery statistics
async function getAllUsersWithStats() {
    try {
        const response = await fetch('/api/users');
        const result = await response.json();
        
        if (result.success) {
            const { users, discovery_stats } = result.data;
            
            console.log(`Total users: ${discovery_stats.total_users}`);
            console.log(`From real groups: ${discovery_stats.from_real_groups}`);
            console.log(`Additional discovered: ${discovery_stats.additional_discovered}`);
            
            // Process users
            users.forEach(user => {
                console.log(`${user.login}: ${user.name} (${user.group})`);
            });
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

// Get just statistics for dashboard
async function getDiscoveryStats() {
    try {
        const response = await fetch('/api/users/stats');
        const result = await response.json();
        
        if (result.success) {
            const stats = result.data;
            
            // Update dashboard
            document.getElementById('total-users').textContent = stats.total_users;
            document.getElementById('groups-found').textContent = stats.groups_count;
            document.getElementById('active-today').textContent = stats.activity_stats.active_today;
            
            // Update group breakdown chart
            updateGroupChart(stats.group_breakdown);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}
```

### **Python Client**
```python
import requests
import json

class MT5WebAPIClient:
    def __init__(self, base_url="http://localhost:8080"):
        self.base_url = base_url
        self.session = requests.Session()
    
    def connect(self, server, login, password):
        """Connect to MT5 server"""
        data = {
            "server": server,
            "login": login,
            "password": password
        }
        response = self.session.post(f"{self.base_url}/api/connect", json=data)
        return response.json()
    
    def get_all_users_with_stats(self):
        """Get all users with discovery statistics"""
        response = self.session.get(f"{self.base_url}/api/users")
        result = response.json()
        
        if result["success"]:
            users = result["data"]["users"]
            stats = result["data"]["discovery_stats"]
            
            print(f"Found {stats['total_users']} users total")
            print(f"- From real groups: {stats['from_real_groups']}")
            print(f"- Additional discovered: {stats['additional_discovered']}")
            
            return users, stats
        else:
            raise Exception(result["error"])
    
    def get_discovery_stats(self):
        """Get discovery statistics only"""
        response = self.session.get(f"{self.base_url}/api/users/stats")
        result = response.json()
        
        if result["success"]:
            return result["data"]
        else:
            raise Exception(result["error"])

# Usage
client = MT5WebAPIClient()
client.connect("your-server.com:443", 12345, "password")

# Get full data
users, stats = client.get_all_users_with_stats()

# Get just statistics
stats = client.get_discovery_stats()
print(f"Groups found: {stats['groups_count']}")
for group_info in stats['group_breakdown']:
    print(f"  {group_info['group']}: {group_info['count']} users")
```

### **PowerShell Client**
```powershell
# Connect to MT5
$connectData = @{
    server = "your-server.com:443"
    login = 12345
    password = "your-password"
} | ConvertTo-Json

$connectResponse = Invoke-RestMethod -Uri "http://localhost:8080/api/connect" -Method POST -Body $connectData -ContentType "application/json"

if ($connectResponse.success) {
    Write-Host "✅ Connected successfully"
    
    # Get all users with statistics
    $usersResponse = Invoke-RestMethod -Uri "http://localhost:8080/api/users" -Method GET
    
    if ($usersResponse.success) {
        $users = $usersResponse.data.users
        $stats = $usersResponse.data.discovery_stats
        
        Write-Host "📊 Discovery Results:"
        Write-Host "   Total users: $($stats.total_users)"
        Write-Host "   From real groups: $($stats.from_real_groups)"
        Write-Host "   Additional discovered: $($stats.additional_discovered)"
        Write-Host "   Login range: $($stats.login_range)"
        
        Write-Host "👥 Sample users:"
        $users | Select-Object -First 5 | ForEach-Object {
            Write-Host "   $($_.login): $($_.name) ($($_.group))"
        }
    }
    
    # Get statistics only for monitoring
    $statsResponse = Invoke-RestMethod -Uri "http://localhost:8080/api/users/stats" -Method GET
    
    if ($statsResponse.success) {
        $stats = $statsResponse.data
        Write-Host "📈 Activity Summary:"
        Write-Host "   Active today: $($stats.activity_stats.active_today)"
        Write-Host "   Active this week: $($stats.activity_stats.active_week)"
        Write-Host "   Active this month: $($stats.activity_stats.active_month)"
    }
}
```

### **curl Examples**
```bash
# Connect to server
curl -X POST http://localhost:8080/api/connect \
  -H "Content-Type: application/json" \
  -d '{"server":"your-server.com:443","login":12345,"password":"your-password"}'

# Get all users with discovery statistics
curl http://localhost:8080/api/users | jq '.data.discovery_stats'

# Get only discovery statistics (faster)
curl http://localhost:8080/api/users/stats | jq '.data.total_users'

# Get users from your real groups only
curl http://localhost:8080/api/users/real | jq '.data | length'

# Get group breakdown
curl http://localhost:8080/api/users/stats | jq '.data.group_breakdown[]'
```

## 📊 **Dashboard Integration**

### **Real-time Statistics Dashboard**
```html
<!DOCTYPE html>
<html>
<head>
    <title>MT5 User Dashboard</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</head>
<body>
    <div class="dashboard">
        <div class="stats-grid">
            <div class="stat-card">
                <h3>Total Users</h3>
                <span id="total-users">-</span>
            </div>
            <div class="stat-card">
                <h3>Groups Found</h3>
                <span id="groups-count">-</span>
            </div>
            <div class="stat-card">
                <h3>Active Today</h3>
                <span id="active-today">-</span>
            </div>
            <div class="stat-card">
                <h3>Discovery Rate</h3>
                <span id="discovery-rate">-</span>
            </div>
        </div>
        
        <div class="charts">
            <canvas id="groupChart"></canvas>
            <canvas id="activityChart"></canvas>
        </div>
        
        <button onclick="refreshStats()">Refresh</button>
    </div>

    <script>
        async function refreshStats() {
            try {
                const response = await fetch('/api/users/stats');
                const result = await response.json();
                
                if (result.success) {
                    const stats = result.data;
                    
                    // Update stat cards
                    document.getElementById('total-users').textContent = stats.total_users;
                    document.getElementById('groups-count').textContent = stats.groups_count;
                    document.getElementById('active-today').textContent = stats.activity_stats.active_today;
                    
                    const discoveryRate = (stats.additional_discovered / stats.total_users * 100).toFixed(1);
                    document.getElementById('discovery-rate').textContent = discoveryRate + '%';
                    
                    // Update charts
                    updateGroupChart(stats.group_breakdown);
                    updateActivityChart(stats.activity_stats);
                }
            } catch (error) {
                console.error('Error refreshing stats:', error);
            }
        }
        
        function updateGroupChart(groupData) {
            // Chart.js implementation for group breakdown
        }
        
        function updateActivityChart(activityData) {
            // Chart.js implementation for activity stats
        }
        
        // Auto-refresh every 5 minutes
        setInterval(refreshStats, 5 * 60 * 1000);
        
        // Initial load
        refreshStats();
    </script>
</body>
</html>
```

## 🔧 **API Performance**

### **Endpoint Performance Comparison**

| Endpoint | Response Time | Data Size | Use Case |
|----------|---------------|-----------|----------|
| `/api/users` | 20-45 seconds | Large | Complete user analysis |
| `/api/users/real` | 2-5 seconds | Medium | Known group operations |
| `/api/users/stats` | 10-20 seconds | Small | Dashboard/monitoring |

### **Optimization Tips**

1. **Use `/api/users/stats` for dashboards** - Much smaller response
2. **Cache results** - Discovery doesn't change frequently
3. **Use `/api/users/real` when you only need your groups**
4. **Implement polling** - Don't call `/api/users` too frequently

## 🎯 **Updated Swagger Documentation**

The Swagger documentation has been completely updated with:
- ✅ Enhanced `/api/users` endpoint with discovery statistics
- ✅ New `/api/users/stats` endpoint for statistics only
- ✅ Detailed response schemas and examples
- ✅ Performance notes and usage recommendations

**View the updated Swagger UI:** Open `docs/swagger-ui.html` in your browser

## 🚀 **Ready to Use**

The enhanced Web API is now ready with:

✅ **Enhanced Discovery** - Uses your real groups + smart patterns  
✅ **Detailed Statistics** - Comprehensive discovery analytics  
✅ **Multiple Endpoints** - Choose the right endpoint for your use case  
✅ **Better Performance** - Optimized for different scenarios  
✅ **Complete Documentation** - Updated Swagger specs with examples  

Test the new endpoints:
1. Start your Web API server
2. Try `GET /api/users/stats` for quick statistics
3. Try `GET /api/users` for full discovery with stats
4. Check the updated Swagger documentation

Your Web API now provides powerful user discovery with detailed insights! 🎉
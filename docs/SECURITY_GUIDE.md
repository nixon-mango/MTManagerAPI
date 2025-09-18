# 🔐 Security Guide - API Key Authentication

## 🎯 **API Security Implementation**

I've implemented a comprehensive API key authentication system to secure your MT5 Web API. Here's how to set it up and use it.

## 🚀 **Quick Security Setup**

### **Step 1: Generate an API Key**
```cmd
MT5WebAPI.exe --generate-key
```

**Output:**
```
🔑 Generated new API key:
   Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG

To use this key:
1. Add it to App.config:
   <add key="ApiKeys" value="Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG" />
2. Enable security:
   <add key="RequireApiKey" value="true" />
3. Restart the server
```

### **Step 2: Update App.config**
```xml
<appSettings>
    <!-- Enable API Key Authentication -->
    <add key="RequireApiKey" value="true" />
    <add key="ApiKeys" value="Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG" />
    <add key="ApiKeyHeader" value="X-API-Key" />
    <add key="LogSecurityEvents" value="true" />
    
    <!-- Optional: Restrict origins -->
    <add key="AllowedOrigins" value="http://localhost,http://192.168.1.0/24" />
</appSettings>
```

### **Step 3: Start Secure Server**
```cmd
MT5WebAPI.exe --host YOUR_STATIC_IP --port 8080
```

**Output:**
```
✓ Web server started successfully
  Listening on: http://192.168.1.100:8080/
🔒 Security: API Key authentication ENABLED
   Header: X-API-Key
   Valid keys: 1
```

## 🔧 **Configuration Options**

### **Security Settings**
```xml
<appSettings>
    <!-- Core Security -->
    <add key="RequireApiKey" value="true" />          <!-- Enable/disable authentication -->
    <add key="ApiKeyHeader" value="X-API-Key" />      <!-- Header name for API key -->
    <add key="LogSecurityEvents" value="true" />      <!-- Log auth attempts -->
    
    <!-- API Keys (comma-separated for multiple keys) -->
    <add key="ApiKeys" value="key1,key2,key3" />
    
    <!-- CORS Configuration -->
    <add key="AllowedOrigins" value="*" />            <!-- Allow all origins -->
    <!-- OR restrict to specific origins: -->
    <!-- <add key="AllowedOrigins" value="http://localhost,http://192.168.1.0/24,https://yourdomain.com" /> -->
</appSettings>
```

### **Multiple API Keys**
```xml
<!-- Support multiple clients with different keys -->
<add key="ApiKeys" value="web-app-key-123,mobile-app-key-456,admin-key-789" />
```

## 🌐 **Using the Secured API**

### **Method 1: HTTP Header (Recommended)**
```bash
# Include API key in header
curl -H "X-API-Key: Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG" \
     http://YOUR_STATIC_IP:8080/api/users/stats
```

### **Method 2: Query Parameter**
```bash
# Include API key as query parameter
curl "http://YOUR_STATIC_IP:8080/api/users/stats?api_key=Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG"
```

### **Method 3: JavaScript/Web Apps**
```javascript
// Using fetch with headers
const API_KEY = 'Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG';
const API_BASE = 'http://YOUR_STATIC_IP:8080';

async function callSecureAPI(endpoint) {
    const response = await fetch(`${API_BASE}${endpoint}`, {
        headers: {
            'X-API-Key': API_KEY,
            'Content-Type': 'application/json'
        }
    });
    
    return await response.json();
}

// Usage
const users = await callSecureAPI('/api/users/stats');
console.log(users);
```

### **Method 4: Python Client**
```python
import requests

class SecureMT5Client:
    def __init__(self, base_url, api_key):
        self.base_url = base_url
        self.headers = {
            'X-API-Key': api_key,
            'Content-Type': 'application/json'
        }
        self.session = requests.Session()
        self.session.headers.update(self.headers)
    
    def get_users_stats(self):
        response = self.session.get(f"{self.base_url}/api/users/stats")
        return response.json()
    
    def connect_mt5(self, server, login, password):
        data = {"server": server, "login": login, "password": password}
        response = self.session.post(f"{self.base_url}/api/connect", json=data)
        return response.json()

# Usage
client = SecureMT5Client(
    base_url="http://YOUR_STATIC_IP:8080",
    api_key="Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG"
)

stats = client.get_users_stats()
print(f"Total users: {stats['data']['total_users']}")
```

### **Method 5: C# Client**
```csharp
public class SecureMT5ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public SecureMT5ApiClient(string baseUrl, string apiKey)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task<ApiResponse<object>> GetUserStatsAsync()
    {
        return await GetAsync<ApiResponse<object>>("/api/users/stats");
    }
}

// Usage
var client = new SecureMT5ApiClient(
    "http://YOUR_STATIC_IP:8080", 
    "Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG"
);

var stats = await client.GetUserStatsAsync();
```

## 🛡️ **Security Features**

### **Authentication Methods**
- ✅ **HTTP Header:** `X-API-Key: your-key` (recommended)
- ✅ **Query Parameter:** `?api_key=your-key` (fallback)
- ✅ **Multiple Keys:** Support for different client applications
- ✅ **Origin Validation:** Restrict by domain/IP if needed

### **Security Logging**
```
✅ Authentication successful from 192.168.1.50:12345
⚠️  Authentication failed: Missing API key from 192.168.1.60:54321
⚠️  Authentication failed: Invalid API key from 192.168.1.70:98765
```

### **Error Responses**
```json
// Missing API key
{
  "success": false,
  "error": "Missing API key. Include 'X-API-Key' header or 'api_key' query parameter.",
  "timestamp": "2024-01-15T10:30:00Z"
}

// Invalid API key
{
  "success": false,
  "error": "Invalid API key.",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🔧 **Configuration Examples**

### **Development (Relaxed Security)**
```xml
<appSettings>
    <add key="RequireApiKey" value="false" />     <!-- Disabled for testing -->
    <add key="AllowedOrigins" value="*" />        <!-- Allow all origins -->
    <add key="LogSecurityEvents" value="true" />  <!-- Log for debugging -->
</appSettings>
```

### **Production (Strict Security)**
```xml
<appSettings>
    <add key="RequireApiKey" value="true" />
    <add key="ApiKeys" value="prod-key-1,prod-key-2" />
    <add key="AllowedOrigins" value="https://yourdomain.com,https://app.yourdomain.com" />
    <add key="LogSecurityEvents" value="true" />
</appSettings>
```

### **Internal Network (Medium Security)**
```xml
<appSettings>
    <add key="RequireApiKey" value="true" />
    <add key="ApiKeys" value="internal-network-key" />
    <add key="AllowedOrigins" value="http://192.168.1.0/24" />  <!-- Local network only -->
    <add key="LogSecurityEvents" value="true" />
</appSettings>
```

## 🎮 **Testing Security**

### **Test Without API Key (Should Fail)**
```bash
curl http://YOUR_STATIC_IP:8080/api/users/stats

# Expected response:
# {
#   "success": false,
#   "error": "Missing API key. Include 'X-API-Key' header or 'api_key' query parameter."
# }
```

### **Test With Valid API Key (Should Succeed)**
```bash
curl -H "X-API-Key: Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG" \
     http://YOUR_STATIC_IP:8080/api/users/stats

# Expected response:
# {
#   "success": true,
#   "data": { ... }
# }
```

### **Test With Invalid API Key (Should Fail)**
```bash
curl -H "X-API-Key: invalid-key" \
     http://YOUR_STATIC_IP:8080/api/users/stats

# Expected response:
# {
#   "success": false,
#   "error": "Invalid API key."
# }
```

## 🔄 **Migration Guide**

### **Existing Clients (No Security)**
```javascript
// Before (no security)
fetch('/api/users/stats')
```

### **Updated Clients (With Security)**
```javascript
// After (with API key)
fetch('/api/users/stats', {
    headers: {
        'X-API-Key': 'Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG'
    }
})
```

## 🚨 **Security Best Practices**

### **API Key Management**
- ✅ **Generate strong keys** using the built-in generator
- ✅ **Use different keys** for different applications
- ✅ **Store keys securely** (environment variables, secure config)
- ✅ **Rotate keys regularly** (generate new ones periodically)
- ✅ **Never log API keys** in plain text

### **Network Security**
- ✅ **Use HTTPS** in production (not implemented yet)
- ✅ **Restrict origins** to known domains/IPs
- ✅ **Monitor authentication logs** for suspicious activity
- ✅ **Use firewall rules** to limit access

### **Configuration Security**
```xml
<!-- Good: Use environment variables -->
<add key="ApiKeys" value="%MT5_API_KEYS%" />

<!-- Better: Use encrypted configuration sections -->
<!-- (Advanced topic - requires additional setup) -->
```

## 🎉 **Summary**

Your MT5 Web API now has:

✅ **Flexible Authentication** - Enable/disable as needed  
✅ **Multiple API Keys** - Support different clients  
✅ **Header or Query Auth** - Flexible authentication methods  
✅ **Origin Validation** - Restrict by domain/IP  
✅ **Security Logging** - Monitor authentication attempts  
✅ **Easy Management** - Generate keys via command line  

## 🚀 **Next Steps**

1. **Build the solution** (includes new security features)
2. **Generate an API key:** `MT5WebAPI.exe --generate-key`
3. **Update App.config** with your key and enable security
4. **Test authentication** with curl or Postman
5. **Update client applications** to include API key

Your MT5 Web API is now **secure and ready for network access**! 🔐🌐
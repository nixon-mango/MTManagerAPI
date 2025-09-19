# 🔐 Quick Security Setup - API Key Authentication

## 🚀 **5-Minute Security Setup**

### **Step 1: Generate API Key**
```cmd
generate-api-key.bat
```

**Output:**
```
🔑 Generated new API key:
   Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG
```

### **Step 2: Update Configuration**
Edit `MT5WebAPI\App.config`:
```xml
<appSettings>
    <!-- Enable Security -->
    <add key="RequireApiKey" value="true" />
    <add key="ApiKeys" value="Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG" />
</appSettings>
```

### **Step 3: Start Secure Server**
```cmd
MT5WebAPI.exe --host YOUR_STATIC_IP --port 8080
```

**Output:**
```
✓ Web server started successfully
🔒 Security: API Key authentication ENABLED
   Header: X-API-Key
   Valid keys: 1
```

### **Step 4: Test Security**
```bash
# Should fail (no API key)
curl http://YOUR_STATIC_IP:8080/api/status

# Should succeed (with API key)
curl -H "X-API-Key: Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG" \
     http://YOUR_STATIC_IP:8080/api/status
```

## 🎯 **Usage Examples**

### **Web Applications**
```javascript
const API_KEY = 'Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG';

fetch('http://YOUR_STATIC_IP:8080/api/users/stats', {
    headers: {
        'X-API-Key': API_KEY
    }
})
.then(response => response.json())
.then(data => console.log(data));
```

### **Mobile Apps**
```javascript
// React Native, Flutter, etc.
const apiClient = axios.create({
    baseURL: 'http://YOUR_STATIC_IP:8080',
    headers: {
        'X-API-Key': 'Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG'
    }
});
```

### **Desktop Applications**
```csharp
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("X-API-Key", "Kj8mN2pQ7rS9tU4vW6xY1zA3bC5dE8fG");
httpClient.BaseAddress = new Uri("http://YOUR_STATIC_IP:8080/");
```

## 🛡️ **Security Features**

### **✅ What You Get**
- **API Key Authentication** - Secure access control
- **Header or Query Auth** - Flexible authentication methods
- **Multiple Keys Support** - Different keys for different apps
- **Origin Validation** - Restrict by domain/IP
- **Security Logging** - Monitor authentication attempts
- **Easy Key Generation** - Built-in secure key generator

### **✅ Authentication Methods**
```bash
# Method 1: HTTP Header (recommended)
curl -H "X-API-Key: your-key" http://YOUR_STATIC_IP:8080/api/users

# Method 2: Query Parameter
curl "http://YOUR_STATIC_IP:8080/api/users?api_key=your-key"
```

### **✅ Error Responses**
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

## 🔧 **Configuration Options**

### **Basic Security (Default)**
```xml
<add key="RequireApiKey" value="true" />
<add key="ApiKeys" value="your-generated-key" />
<add key="AllowedOrigins" value="*" />
```

### **Strict Security (Production)**
```xml
<add key="RequireApiKey" value="true" />
<add key="ApiKeys" value="web-key,mobile-key,admin-key" />
<add key="AllowedOrigins" value="https://yourdomain.com,https://app.yourdomain.com" />
```

### **Development Mode (No Security)**
```xml
<add key="RequireApiKey" value="false" />
```

## 🧪 **Testing Security**

### **Test Script**
```powershell
# Test security configuration
.\test-api-security.ps1 -ApiUrl http://YOUR_STATIC_IP:8080 -ApiKey YOUR_API_KEY
```

### **Manual Testing**
```bash
# Test without key (should fail if security enabled)
curl http://YOUR_STATIC_IP:8080/api/users/stats

# Test with key (should succeed)
curl -H "X-API-Key: YOUR_KEY" http://YOUR_STATIC_IP:8080/api/users/stats
```

## 🎉 **Ready to Use**

Your MT5 Web API now has:

✅ **Secure API Key Authentication**  
✅ **Flexible Configuration**  
✅ **Multiple Authentication Methods**  
✅ **Easy Key Management**  
✅ **Comprehensive Testing Tools**  

## 📚 **Helper Scripts Created**

- **`generate-api-key.bat`** - Generate secure API keys
- **`start-api-secure.bat`** - Start server in secure mode
- **`test-api-security.ps1`** - Test security configuration

Your API is now **secure and ready for network access** with API key authentication! 🔐🌐
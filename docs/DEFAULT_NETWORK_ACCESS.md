# 🌐 Default Network Access Configuration

## ✅ **Default Changed to Network Access**

I've updated the MT5 Web API to **default to network access** instead of localhost-only access.

## 🚀 **What Changed**

### **Before (Localhost Only)**
```csharp
string host = "localhost";  // Only accessible locally
```

### **After (Network Access)**
```csharp
string host = "0.0.0.0";    // Accessible from network
```

## 🎯 **How to Start the Server**

### **Method 1: Simple Startup (New Default)**
```cmd
# Just run the executable - now defaults to 0.0.0.0:8080
MT5WebAPI.exe

# Output:
# ✓ Web server started successfully
# Listening on: http://0.0.0.0:8080/
```

### **Method 2: Use Batch File**
```cmd
start-api.bat
```

### **Method 3: Command Line Override**
```cmd
# Still can override if needed
MT5WebAPI.exe --host localhost --port 8080    # Local only
MT5WebAPI.exe --host 192.168.1.100 --port 80  # Specific IP
```

## 🌐 **Access Points**

After starting the server, it's accessible from:

### **✅ Local Machine**
```bash
curl http://localhost:8080/api/status
curl http://127.0.0.1:8080/api/status
```

### **✅ Static IP**
```bash
curl http://YOUR_STATIC_IP:8080/api/status
```

### **✅ Network (Other Machines)**
```bash
curl http://YOUR_STATIC_IP:8080/api/status
```

## 🔧 **Server Output**

When you start the server, you'll now see:
```
=== MT5 Manager Web API Server ===

✓ Web server started successfully
  Listening on: http://0.0.0.0:8080/

Available endpoints:
  POST /api/connect        - Connect to MT5 server
  GET  /api/users          - Get all users (enhanced discovery)
  GET  /api/users/real     - Get users from your real groups
  GET  /api/users/stats    - Get user discovery statistics
  GET  /api/user/{login}   - Get user information
  GET  /api/account/{login} - Get account information
  POST /api/balance        - Perform balance operation
  GET  /api/status         - Get connection status

Press 'q' to quit the server...
```

## 🛡️ **Security Setup**

### **Configure Windows Firewall**
```cmd
# Run as Administrator
configure-firewall.bat

# Or manually:
netsh advfirewall firewall add rule name="MT5WebAPI-Port8080" dir=in action=allow protocol=TCP localport=8080
```

### **Test Firewall Configuration**
```cmd
# Check if rule was added
netsh advfirewall firewall show rule name="MT5WebAPI-Port8080"
```

## 🧪 **Testing External Access**

### **Test Script**
```powershell
# Test from current machine
.\test-external-access.ps1 -StaticIP YOUR_STATIC_IP

# Expected output:
# 🌐 Testing MT5 Web API External Access
# Target: http://YOUR_STATIC_IP:8080
# ✅ Port 8080 is open and accessible
# ✅ API Status - Success
# 🎉 All tests passed!
```

### **Manual Testing**
```bash
# Test basic connectivity
curl http://YOUR_STATIC_IP:8080/api/status

# Test enhanced endpoints
curl http://YOUR_STATIC_IP:8080/api/users/stats
curl http://YOUR_STATIC_IP:8080/api/users/real
```

## 📱 **Update Client Applications**

### **JavaScript/Web Apps**
```javascript
// Update your API base URL
const API_BASE_URL = 'http://YOUR_STATIC_IP:8080';

// Example usage
fetch(`${API_BASE_URL}/api/users/stats`)
    .then(response => response.json())
    .then(data => console.log(data));
```

### **Mobile Apps**
```javascript
// React Native, Flutter, etc.
const API_CONFIG = {
    baseURL: 'http://YOUR_STATIC_IP:8080',
    timeout: 30000
};
```

### **Desktop Applications**
```csharp
// C# applications
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://YOUR_STATIC_IP:8080/");
```

## 🎯 **Summary**

### **What You Need to Do**

1. **Build the solution** (LINQ errors are fixed)
2. **Run `start-api.bat`** (now defaults to 0.0.0.0:8080)
3. **Run `configure-firewall.bat`** as Administrator
4. **Test with your static IP**

### **What You Get**

✅ **Network Access** - API accessible from any machine on your network  
✅ **Static IP Support** - Use your static IP address  
✅ **Easy Startup** - Simple batch file execution  
✅ **Automatic Configuration** - Defaults to network access  
✅ **Enhanced API** - All discovery features available externally  

Your MT5 Web API now **defaults to network access** and is ready to be used from anywhere on your network using your static IP address! 🌐🚀
# 🌐 Static IP Access Setup Guide

## 🎯 **Making Your MT5 Web API Accessible via Static IP**

Currently, your API only works locally (`localhost`). Here's how to make it accessible from external networks using your static IP address.

## 🚀 **Quick Setup (Command Line)**

### **Option 1: Bind to All Interfaces (Recommended)**
```cmd
# This makes the API accessible from any IP address
MT5WebAPI.exe --host 0.0.0.0 --port 8080
```

### **Option 2: Bind to Your Specific Static IP**
```cmd
# Replace 192.168.1.100 with your actual static IP
MT5WebAPI.exe --host 192.168.1.100 --port 8080
```

### **Option 3: Use a Different Port**
```cmd
# Use port 80 (HTTP default) or 443 (HTTPS)
MT5WebAPI.exe --host 0.0.0.0 --port 80
```

## 🔧 **Detailed Configuration Steps**

### **Step 1: Find Your Static IP Address**
```cmd
# Windows - Find your IP address
ipconfig

# Look for your static IP in the output:
# Ethernet adapter:
#   IPv4 Address: 192.168.1.100  ← Your static IP
```

### **Step 2: Start Server with Static IP Binding**
```cmd
# Navigate to your Web API directory
cd C:\Users\Administrator\Documents\MTManagerAPI\MT5WebAPI\bin\Debug

# Start server bound to all interfaces (recommended)
MT5WebAPI.exe --host 0.0.0.0 --port 8080

# You should see:
# ✓ Web server started successfully
# Listening on: http://0.0.0.0:8080/
```

### **Step 3: Test Local Access First**
```cmd
# Test from the same machine
curl http://localhost:8080/api/status
curl http://YOUR_STATIC_IP:8080/api/status
```

### **Step 4: Configure Windows Firewall**
```cmd
# Add firewall rule to allow incoming connections
netsh advfirewall firewall add rule name="MT5WebAPI" dir=in action=allow protocol=TCP localport=8080

# Or use Windows Firewall GUI:
# Control Panel → System and Security → Windows Defender Firewall → Advanced Settings
# → Inbound Rules → New Rule → Port → TCP → 8080 → Allow
```

### **Step 5: Test External Access**
```cmd
# From another machine on the network
curl http://YOUR_STATIC_IP:8080/api/status

# From internet (if router configured)
curl http://YOUR_PUBLIC_IP:8080/api/status
```

## 🌍 **Network Configuration Options**

### **Local Network Access Only**
```cmd
# Bind to your local static IP (e.g., 192.168.1.100)
MT5WebAPI.exe --host 192.168.1.100 --port 8080

# Accessible from:
# ✅ Same machine: http://localhost:8080
# ✅ Local network: http://192.168.1.100:8080
# ❌ Internet: Not accessible
```

### **Internet Access (Requires Router Configuration)**
```cmd
# Bind to all interfaces
MT5WebAPI.exe --host 0.0.0.0 --port 8080

# Configure router port forwarding:
# External Port: 8080 → Internal IP: 192.168.1.100:8080

# Accessible from:
# ✅ Same machine: http://localhost:8080
# ✅ Local network: http://192.168.1.100:8080
# ✅ Internet: http://YOUR_PUBLIC_IP:8080
```

## 🛡️ **Security Configuration**

### **Basic Security Setup**
The current Web API has CORS enabled by default, but for production use, consider these security measures:

#### **1. Restrict CORS Origins**
Create a configuration file `appsettings.json`:
```json
{
  "server": {
    "host": "0.0.0.0",
    "port": 8080,
    "allowed_origins": [
      "http://192.168.1.0/24",
      "http://your-domain.com"
    ]
  }
}
```

#### **2. Use HTTPS (Recommended for Internet Access)**
```cmd
# Generate self-signed certificate (for testing)
makecert -r -pe -n "CN=YOUR_STATIC_IP" -ss my -sr localmachine -a sha256 -len 2048 -cy end

# Start with HTTPS (requires certificate setup)
MT5WebAPI.exe --host 0.0.0.0 --port 443 --https
```

#### **3. Add Authentication (Recommended)**
Consider adding API key authentication for external access.

## 📊 **Testing Your Setup**

### **Test Script (PowerShell)**
```powershell
# Test script to verify external access
param(
    [string]$StaticIP = "192.168.1.100",
    [int]$Port = 8080
)

$baseUrl = "http://${StaticIP}:${Port}"

Write-Host "Testing MT5 Web API at $baseUrl" -ForegroundColor Green

# Test endpoints
$endpoints = @(
    "/api/status",
    "/api/users/stats"
)

foreach ($endpoint in $endpoints) {
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl$endpoint" -Method GET -TimeoutSec 10
        Write-Host "✅ $endpoint - Success" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ $endpoint - Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}
```

### **Test from Different Locations**

#### **Same Machine**
```bash
curl http://localhost:8080/api/status
curl http://YOUR_STATIC_IP:8080/api/status
```

#### **Another Machine on Local Network**
```bash
curl http://YOUR_STATIC_IP:8080/api/status
```

#### **From Internet (if configured)**
```bash
curl http://YOUR_PUBLIC_IP:8080/api/status
```

## 🔧 **Advanced Configuration**

### **Create a Batch File for Easy Startup**
Create `start-api-external.bat`:
```cmd
@echo off
echo Starting MT5 Web API for External Access...
echo.

cd /d "%~dp0"
MT5WebAPI.exe --host 0.0.0.0 --port 8080

pause
```

### **Create a Windows Service (Optional)**
For production use, consider creating a Windows Service:

```cmd
# Install as Windows Service (requires additional setup)
sc create "MT5WebAPI" binPath="C:\Path\To\MT5WebAPI.exe --host 0.0.0.0 --port 8080" start=auto
sc start "MT5WebAPI"
```

### **Load Balancing (Multiple Instances)**
```cmd
# Run multiple instances on different ports
start MT5WebAPI.exe --host 0.0.0.0 --port 8080
start MT5WebAPI.exe --host 0.0.0.0 --port 8081
start MT5WebAPI.exe --host 0.0.0.0 --port 8082
```

## 🌐 **Client Access Examples**

### **JavaScript (Web Application)**
```javascript
// Update your base URL to use static IP
const API_BASE_URL = 'http://YOUR_STATIC_IP:8080';

// Connect to MT5
async function connectToMT5() {
    const response = await fetch(`${API_BASE_URL}/api/connect`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            server: 'your-server.com:443',
            login: 12345,
            password: 'your-password'
        })
    });
    
    const result = await response.json();
    return result;
}

// Get all users
async function getAllUsers() {
    const response = await fetch(`${API_BASE_URL}/api/users`);
    const result = await response.json();
    return result;
}
```

### **Python Client**
```python
import requests

class MT5APIClient:
    def __init__(self, static_ip, port=8080):
        self.base_url = f"http://{static_ip}:{port}"
        self.session = requests.Session()
    
    def connect(self, server, login, password):
        data = {
            "server": server,
            "login": login,
            "password": password
        }
        response = self.session.post(f"{self.base_url}/api/connect", json=data)
        return response.json()
    
    def get_all_users(self):
        response = self.session.get(f"{self.base_url}/api/users")
        return response.json()

# Usage
client = MT5APIClient("192.168.1.100")  # Your static IP
result = client.connect("your-server.com:443", 12345, "password")
users = client.get_all_users()
```

### **Mobile App (React Native)**
```javascript
const API_CONFIG = {
    baseURL: 'http://YOUR_STATIC_IP:8080',
    timeout: 30000,
};

// Use with axios or fetch
import axios from 'axios';

const apiClient = axios.create(API_CONFIG);

export const mt5Api = {
    connect: (server, login, password) =>
        apiClient.post('/api/connect', { server, login, password }),
    
    getAllUsers: () =>
        apiClient.get('/api/users'),
    
    getUserStats: () =>
        apiClient.get('/api/users/stats'),
};
```

## 🚨 **Troubleshooting**

### **Common Issues and Solutions**

#### **1. "Connection Refused" Error**
```
❌ Error: Connection refused
```
**Solutions:**
- Check if server is running: `netstat -an | findstr :8080`
- Verify host binding: Use `0.0.0.0` instead of `localhost`
- Check firewall: Add rule for port 8080

#### **2. "Access Denied" Error**
```
❌ Error: HTTP 403 Forbidden
```
**Solutions:**
- Run as Administrator (for ports < 1024)
- Check Windows Defender Firewall
- Verify CORS configuration

#### **3. "Timeout" Error**
```
❌ Error: Request timeout
```
**Solutions:**
- Check network connectivity: `ping YOUR_STATIC_IP`
- Verify router configuration
- Check if port is blocked by ISP

#### **4. "Port Already in Use" Error**
```
❌ Error: Port 8080 is already in use
```
**Solutions:**
- Use different port: `--port 8081`
- Find what's using the port: `netstat -ano | findstr :8080`
- Kill the process or use different port

## 📋 **Quick Reference Commands**

### **Start Server Commands**
```cmd
# Local access only
MT5WebAPI.exe --host localhost --port 8080

# Network access (recommended)
MT5WebAPI.exe --host 0.0.0.0 --port 8080

# Specific IP binding
MT5WebAPI.exe --host 192.168.1.100 --port 8080

# Different port
MT5WebAPI.exe --host 0.0.0.0 --port 80
```

### **Firewall Commands**
```cmd
# Add firewall rule
netsh advfirewall firewall add rule name="MT5WebAPI" dir=in action=allow protocol=TCP localport=8080

# Remove firewall rule
netsh advfirewall firewall delete rule name="MT5WebAPI"

# Show firewall rules
netsh advfirewall firewall show rule name="MT5WebAPI"
```

### **Network Testing Commands**
```cmd
# Check if port is open
telnet YOUR_STATIC_IP 8080

# Check what's listening on port
netstat -an | findstr :8080

# Test HTTP endpoint
curl http://YOUR_STATIC_IP:8080/api/status
```

## 🎉 **Success Checklist**

After configuration, verify these work:

✅ **Local Access**
- `http://localhost:8080/api/status`
- `http://127.0.0.1:8080/api/status`

✅ **Static IP Access**
- `http://YOUR_STATIC_IP:8080/api/status`

✅ **Network Access**
- From another computer: `http://YOUR_STATIC_IP:8080/api/status`

✅ **API Functionality**
- Connect: `POST /api/connect`
- Get users: `GET /api/users`
- Get stats: `GET /api/users/stats`

## 🚀 **Next Steps**

1. **Start server with static IP binding**
2. **Configure firewall rules**
3. **Test from another machine**
4. **Update client applications with new URL**
5. **Consider security measures for production**

Your MT5 Web API will now be accessible from anywhere on your network using your static IP address! 🌐
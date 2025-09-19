# 🌐 Network Access Solutions - "The request is not supported" Error

## ❌ **Error Explanation**
The error "The request is not supported" occurs when trying to bind to `0.0.0.0` without Administrator privileges on Windows.

## ✅ **Multiple Solutions Available**

### **Solution 1: Use Your Specific Static IP (Easiest)**
```cmd
# Find your IP address first
ipconfig

# Look for your IPv4 Address (example: 192.168.1.100)
# Then start with your specific IP:
MT5WebAPI.exe --host 192.168.1.100 --port 8080
```

**Advantages:**
- ✅ No Administrator privileges required
- ✅ Still accessible from network
- ✅ More secure (only binds to your IP)

### **Solution 2: Run as Administrator**
```cmd
# Right-click Command Prompt → "Run as administrator"
# Then run:
MT5WebAPI.exe --host 0.0.0.0 --port 8080
```

**Or use the provided batch file:**
```cmd
# Right-click → "Run as administrator"
start-api-admin.bat
```

### **Solution 3: One-time URL Reservation (Recommended)**
```cmd
# Run once as Administrator to set up permissions
setup-url-reservation.bat

# After this, you can run normally without admin:
MT5WebAPI.exe --host 0.0.0.0 --port 8080
```

### **Solution 4: Smart Auto-Detection**
```powershell
# PowerShell script that auto-detects best approach
.\start-api-network.ps1
```

## 🎯 **Recommended Approach**

### **For Testing/Development:**
1. **Find your static IP:**
   ```cmd
   ipconfig
   ```
   Look for "IPv4 Address" (e.g., 192.168.1.100)

2. **Edit and use the static IP batch file:**
   ```cmd
   # Edit start-with-static-ip.bat
   # Replace YOUR_STATIC_IP with your actual IP
   # Then run:
   start-with-static-ip.bat
   ```

### **For Production:**
1. **Set up URL reservation once:**
   ```cmd
   # Run as Administrator (one-time setup)
   setup-url-reservation.bat
   ```

2. **Then run normally:**
   ```cmd
   MT5WebAPI.exe --host 0.0.0.0 --port 8080
   ```

## 🔧 **Step-by-Step Fix**

### **Step 1: Find Your Static IP**
```cmd
ipconfig

# Example output:
# Ethernet adapter Ethernet:
#    IPv4 Address. . . . . . . . . . . : 192.168.1.100  ← This is your static IP
```

### **Step 2: Start with Your Static IP**
```cmd
# Use your actual IP address
MT5WebAPI.exe --host 192.168.1.100 --port 8080
```

### **Step 3: Test Access**
```bash
# Test from same machine
curl http://localhost:8080/api/status
curl http://192.168.1.100:8080/api/status

# Test from another machine on network
curl http://192.168.1.100:8080/api/status
```

## 🛠️ **Helper Scripts Available**

| Script | Purpose | Admin Required |
|--------|---------|----------------|
| `start-api-local.bat` | Local access only | ❌ No |
| `start-with-static-ip.bat` | Static IP access | ❌ No |
| `start-api-admin.bat` | All interfaces (0.0.0.0) | ✅ Yes |
| `start-api-network.ps1` | Smart auto-detection | ❌ No |
| `setup-url-reservation.bat` | One-time permission setup | ✅ Yes |

## 🧪 **Quick Test**

### **Method 1: Manual with Your IP**
```cmd
# Replace 192.168.1.100 with your actual static IP
cd MT5WebAPI\bin\Debug
MT5WebAPI.exe --host 192.168.1.100 --port 8080
```

### **Method 2: Use PowerShell Script**
```powershell
# Auto-detects your IP and starts server
.\start-api-network.ps1
```

## 🔍 **Troubleshooting**

### **If you still get errors:**

#### **"Port already in use"**
```cmd
# Check what's using port 8080
netstat -ano | findstr :8080

# Use different port
MT5WebAPI.exe --host YOUR_STATIC_IP --port 8081
```

#### **"Access denied"**
```cmd
# Try higher port number
MT5WebAPI.exe --host YOUR_STATIC_IP --port 8080
```

#### **"Cannot connect from other machines"**
```cmd
# Configure firewall
configure-firewall.bat
```

## 🎉 **Expected Success Output**

When working correctly, you should see:
```
=== MT5 Manager Web API Server ===

✓ Web server started successfully
  Listening on: http://192.168.1.100:8080/

Available endpoints:
  GET  /api/users          - Get all users (enhanced discovery)
  GET  /api/users/real     - Get users from your real groups
  GET  /api/users/stats    - Get user discovery statistics
  ...

Press 'q' to quit the server...
```

## 🚀 **Quick Fix Right Now**

1. **Find your IP:** Run `ipconfig`
2. **Start with your IP:** 
   ```cmd
   MT5WebAPI.exe --host YOUR_ACTUAL_IP --port 8080
   ```
3. **Test:** `curl http://YOUR_ACTUAL_IP:8080/api/status`

This will work immediately without Administrator privileges! 🎯
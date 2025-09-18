# 🔧 "The request is not supported" Error Fix

## ❌ **Error Encountered**
```
Error starting server: The request is not supported
Press any key to exit...
```

## 🔍 **Root Cause**
This error occurs when trying to bind to `0.0.0.0` (all interfaces) without Administrator privileges on Windows. The HttpListener requires elevated permissions to bind to all network interfaces.

## ✅ **Solutions**

### **Solution 1: Run as Administrator (Recommended)**
1. **Right-click** on Command Prompt
2. Select **"Run as administrator"**
3. Navigate to your project directory
4. Run the server:
   ```cmd
   cd MT5WebAPI\bin\Debug
   MT5WebAPI.exe
   ```

### **Solution 2: Use Your Specific Static IP**
Instead of `0.0.0.0`, use your actual static IP address:
```cmd
# Find your static IP first
ipconfig

# Then use your specific IP (example: 192.168.1.100)
MT5WebAPI.exe --host 192.168.1.100 --port 8080
```

### **Solution 3: Use a Higher Port Number**
Ports below 1024 require Administrator privileges. Use a higher port:
```cmd
MT5WebAPI.exe --host 0.0.0.0 --port 8080
```

### **Solution 4: Grant URL Reservation (One-time Setup)**
Grant permission to bind to the URL without Administrator privileges:
```cmd
# Run as Administrator (one-time setup)
netsh http add urlacl url=http://0.0.0.0:8080/ user=Everyone

# Then you can run normally
MT5WebAPI.exe --host 0.0.0.0 --port 8080
```
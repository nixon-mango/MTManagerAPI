# 🚀 Quick Setup Guide for MT5 Manager API

> **📚 Need more details?** Check out the [Complete Setup Guide](docs/SETUP.md) for comprehensive instructions, troubleshooting, and production deployment.

## ⚠️ Build Error Resolution

The build is failing because the MT5 Manager API DLL files are not in the correct location. Here's how to fix it:

## 📋 Step-by-Step Solution

### 1. **Create DLLs Directory**
In your project root (`C:\Users\Administrator\Documents\MTManagerAPI\`), create a folder called `DLLs`:
```
C:\Users\Administrator\Documents\MTManagerAPI\DLLs\
```

### 2. **Copy Your MT5 Manager API DLLs**
Place your MT5 Manager API DLL files in the `DLLs` folder. You need these files:

**Required .NET Wrapper DLLs:**
- `MetaQuotes.MT5CommonAPI.dll` (or `MetaQuotes.MT5CommonAPI64.dll`)
- `MetaQuotes.MT5GatewayAPI.dll` (or `MetaQuotes.MT5GatewayAPI64.dll`) 
- `MetaQuotes.MT5ManagerAPI.dll` (or `MetaQuotes.MT5ManagerAPI64.dll`)
- `MetaQuotes.MT5WebAPI.dll`

**Required Native DLLs:**
- `MT5APIGateway.dll` (or `MT5APIGateway64.dll`)
- `MT5APIManager.dll` (or `MT5APIManager64.dll`)

**Important:** Use either ALL 32-bit OR ALL 64-bit versions consistently!

### 3. **Run Setup Script**
```cmd
setup-dlls.bat
```
This will:
- ✅ Check if all required DLLs are present
- ✅ Copy DLLs to all project directories
- ✅ Verify the setup is correct

### 4. **Build the Solution**
```cmd
build.bat
```
This will use the Visual Studio 2022 Build Tools path you specified.

## 📁 Expected Directory Structure
After setup, your structure should look like this:
```
C:\Users\Administrator\Documents\MTManagerAPI\
├── DLLs\                           # ← Your MT5 DLLs go here
│   ├── MetaQuotes.MT5CommonAPI.dll
│   ├── MetaQuotes.MT5GatewayAPI.dll
│   ├── MetaQuotes.MT5ManagerAPI.dll
│   ├── MetaQuotes.MT5WebAPI.dll
│   ├── MT5APIGateway.dll
│   └── MT5APIManager.dll
├── MT5ManagerAPI\
│   └── bin\Debug\                  # ← DLLs copied here
├── MT5ConsoleApp\
│   └── bin\Debug\                  # ← DLLs copied here
├── MT5WebAPI\
│   └── bin\Debug\                  # ← DLLs copied here
├── build.bat
├── setup-dlls.bat
└── MT5ManagerAPI.sln
```

## 🔍 Troubleshooting

### If you get "MetaQuotes.MT5CommonAPI does not exist" error:
1. ✅ Verify DLLs are in the `DLLs` folder
2. ✅ Run `setup-dlls.bat` to copy them to project directories
3. ✅ Ensure you're using consistent architecture (32-bit vs 64-bit)

### If you get "Assembly not found" error:
1. ✅ Check that both .NET wrapper DLLs AND native DLLs are present
2. ✅ Verify all DLLs are the same version/architecture
3. ✅ Make sure DLLs are not blocked (right-click → Properties → Unblock)

### Architecture Selection:
- **32-bit**: Use `MetaQuotes.MT5CommonAPI.dll`, `MT5APIManager.dll`, etc.
- **64-bit**: Use `MetaQuotes.MT5CommonAPI64.dll`, `MT5APIManager64.dll`, etc.

## 🎯 Quick Commands

```cmd
# 1. Setup DLLs (after placing them in DLLs folder)
setup-dlls.bat

# 2. Build solution
build.bat

# 3. Test console app
MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe

# 4. Run Web API
MT5WebAPI\bin\Debug\MT5WebAPI.exe
```

## ✅ Success Indicators

After successful setup and build:
- ✅ No "MetaQuotes.MT5CommonAPI does not exist" errors
- ✅ Build completes with "Build completed successfully!"
- ✅ Console app starts and prompts for MT5 server details
- ✅ Web API starts and shows "Web server started successfully"

## 📞 Need Help?

If you're still getting errors:
1. Check the exact DLL files you have
2. Verify they match the required file names
3. Ensure consistent 32-bit or 64-bit usage
4. Make sure all files are unblocked in Windows

## 📚 Next Steps

- **[Try the Interactive Playground](docs/PLAYGROUND.md)** - Hands-on examples and scenarios
- **[Read the API Documentation](docs/API.md)** - Complete API reference
- **[Complete Setup Guide](docs/SETUP.md)** - Comprehensive setup and production deployment
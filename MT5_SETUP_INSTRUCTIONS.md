# MT5 Manager API Setup Instructions

> **📚 For comprehensive documentation:** Visit the [Complete Documentation](docs/README.md) including [Setup Guide](docs/SETUP.md), [API Reference](docs/API.md), and [Interactive Playground](docs/PLAYGROUND.md).

## Overview

This project provides a modern C# wrapper for the MetaQuotes MT5 Manager API, including:
- **MT5ManagerAPI**: Core library with simplified API wrapper
- **MT5ConsoleApp**: Interactive console application for testing
- **MT5WebAPI**: REST API server for remote access
- **Original MT5 Forms App**: Your existing Windows Forms application

## Prerequisites

1. **Windows Environment**: The MT5 Manager API requires Windows
2. **Visual Studio or MSBuild**: For building the projects
3. **MT5 Manager API DLLs**: You mentioned you have these files
4. **Manager Account Credentials**: You need MT5 Manager (not trader) credentials

## Required DLL Files

You need to place the following MT5 Manager API DLL files in the `bin\Debug` directories of each project:

### .NET DLLs (for C# reference):
- `MetaQuotes.MT5CommonAPI.dll` (or `MetaQuotes.MT5CommonAPI64.dll`)
- `MetaQuotes.MT5GatewayAPI.dll` (or `MetaQuotes.MT5GatewayAPI64.dll`)
- `MetaQuotes.MT5ManagerAPI.dll` (or `MetaQuotes.MT5ManagerAPI64.dll`)
- `MetaQuotes.MT5WebAPI.dll`

### Native DLLs (must be in same directory):
- `MT5APIGateway.dll` (or `MT5APIGateway64.dll`)
- `MT5APIManager.dll` (or `MT5APIManager64.dll`)

## Setup Steps

### 1. Create DLL Directories
```bash
mkdir -p MT5ManagerAPI/bin/Debug
mkdir -p MT5ConsoleApp/bin/Debug
mkdir -p MT5WebAPI/bin/Debug
```

### 2. Copy DLL Files
Copy all your MT5 Manager API DLL files to each of the `bin/Debug` directories:
- `MT5ManagerAPI/bin/Debug/`
- `MT5ConsoleApp/bin/Debug/`
- `MT5WebAPI/bin/Debug/`

### 3. Build the Solution
```bash
# Using the provided build script (recommended)
build.bat

# Or using MSBuild directly
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" MT5ManagerAPI.sln /p:Configuration=Debug

# Or using Visual Studio
# Open MT5ManagerAPI.sln in Visual Studio and build
```

## Running the Applications

### Console Application
```bash
cd MT5ConsoleApp/bin/Debug
./MT5ConsoleApp.exe
```

The console app will prompt you for:
- MT5 Server address
- Manager login ID
- Manager password

Then provide an interactive menu to:
1. Get user information
2. Get account information
3. Get users in a group
4. Get user's group
5. Perform balance operations
6. Get user deals

### Web API Server
```bash
cd MT5WebAPI/bin/Debug
./MT5WebAPI.exe --host localhost --port 8080
```

The web server provides REST endpoints:
- `POST /api/connect` - Connect to MT5 server
- `GET /api/status` - Check connection status
- `GET /api/user/{login}` - Get user info
- `GET /api/account/{login}` - Get account info
- `GET /api/group/{name}/users` - Get users in group
- `POST /api/balance` - Perform balance operation

### Original Windows Forms App
```bash
cd MT5/WindowsFormsApp1/WindowsFormsApp1/bin/Debug
./WindowsFormsApp1.exe
```

## API Usage Examples

### Using the Library Directly
```csharp
using MT5ManagerAPI;

using (var api = new MT5ApiWrapper())
{
    // Initialize
    if (!api.Initialize())
        throw new Exception("Failed to initialize");
    
    // Connect
    if (!api.Connect("your-server", 12345, "password"))
        throw new Exception("Failed to connect");
    
    // Get user info
    var user = api.GetUser(67890);
    Console.WriteLine($"User: {user.Name}, Group: {user.Group}");
    
    // Get account info
    var account = api.GetAccount(67890);
    Console.WriteLine($"Balance: {account.Balance}, Equity: {account.Equity}");
}
```

### Using the Web API
```bash
# Connect to MT5
curl -X POST http://localhost:8080/api/connect \
  -H "Content-Type: application/json" \
  -d '{"server":"your-server","login":12345,"password":"your-password"}'

# Get user information
curl http://localhost:8080/api/user/67890

# Get account information
curl http://localhost:8080/api/account/67890

# Perform balance operation
curl -X POST http://localhost:8080/api/balance \
  -H "Content-Type: application/json" \
  -d '{"login":67890,"amount":100.0,"comment":"Deposit"}'
```

## Project Structure

```
/workspace/
├── MT5ManagerAPI/              # Core API wrapper library
│   ├── MT5Manager.cs           # Original manager class
│   ├── MT5ApiWrapper.cs        # Modern wrapper with error handling
│   └── Models/                 # Data models
├── MT5ConsoleApp/              # Interactive console application
├── MT5WebAPI/                  # REST API server
├── MT5/                        # Your original Windows Forms app
└── MT5ManagerAPI.sln          # Visual Studio solution
```

## Important Notes

1. **Manager Credentials Required**: This API requires MT5 Manager credentials, not regular trader accounts
2. **Windows Only**: The MT5 Manager API is Windows-specific
3. **DLL Architecture**: Use 32-bit or 64-bit DLLs consistently based on your target platform
4. **Error Handling**: All wrapper methods include proper exception handling
5. **Resource Management**: Always use `using` statements or call `Dispose()` to clean up resources

## Troubleshooting

### Common Issues:

1. **"Failed to initialize MT5 Manager API"**
   - Ensure all DLL files are in the bin/Debug directory
   - Check that you have the correct architecture (32-bit vs 64-bit)

2. **"Failed to connect to MT5 server"**
   - Verify server address and port
   - Ensure you're using Manager credentials, not trader credentials
   - Check network connectivity

3. **"Assembly not found" errors**
   - Make sure all MetaQuotes.*.dll files are referenced properly
   - Ensure native DLLs are in the same directory as .NET DLLs

### Getting Help:
- Check the console output for detailed error messages
- Verify your MT5 server settings and credentials
- Ensure all required DLL files are present and the correct version

## Security Considerations

- Never hardcode credentials in your applications
- Use environment variables or secure configuration files
- Consider implementing authentication for the Web API
- Limit network access to the Web API server
- Use HTTPS in production environments

## Next Steps

1. Place your DLL files in the required directories
2. Build the solution
3. Test with the console application first
4. Deploy the Web API for remote access if needed
5. Integrate the library into your existing applications

## 📚 Additional Resources

- **[Interactive Playground](docs/PLAYGROUND.md)** - Try hands-on examples and scenarios
- **[Complete API Reference](docs/API.md)** - Detailed documentation with examples
- **[Advanced Setup Guide](docs/SETUP.md)** - Production deployment and optimization
- **[Quick Setup](QUICK_SETUP.md)** - Get started in 5 minutes
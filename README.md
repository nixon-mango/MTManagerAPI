# MT Manager API - C# Sample Code (MT4 & MT5)

A comprehensive C# wrapper library and sample applications for MetaQuotes MT4 and MT5 Manager APIs, providing modern, easy-to-use interfaces for broker management operations.

## 🚀 Quick Start

```bash
# 1. Place your MT5 Manager API DLLs in the DLLs folder
# 2. Run setup script
setup-dlls.bat

# 3. Build the solution
build.bat

# 4. Test with console app
MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
```

## 📋 What's Included

- **🔧 MT5ManagerAPI**: Modern C# wrapper library with error handling
- **💻 MT5ConsoleApp**: Interactive console application for testing
- **🌐 MT5WebAPI**: REST API server for remote access
- **📱 Windows Forms Apps**: Original MT4 and MT5 GUI applications
- **📚 Comprehensive Documentation**: Setup guides and API reference

## ⚠️ Important Notice

**This is a BROKER API, not a TRADER API!**

The MT Manager API is designed for brokers to manage their MetaTrader servers, not for individual traders. You cannot use regular MetaTrader login credentials with this API. You need **Manager** account credentials provided by your broker or MetaTrader server administrator.

## 🏗️ Project Structure

```
/workspace/
├── MT5ManagerAPI/              # 🔧 Core API wrapper library
│   ├── MT5Manager.cs           # Original manager class
│   ├── MT5ApiWrapper.cs        # Modern wrapper with error handling
│   └── Models/                 # Data models (UserInfo, AccountInfo)
├── MT5ConsoleApp/              # 💻 Interactive console application
├── MT5WebAPI/                  # 🌐 REST API server
├── MT4/                        # 📱 MT4 Windows Forms application
├── MT5/                        # 📱 MT5 Windows Forms application
└── docs/                       # 📚 Documentation and playground
```

## 🔧 Core Features

### MT4 Manager API
- **Native DLL Wrapper**: Uses P23.MetaTrader4.Manager.Wrapper for easy .NET integration
- **Bulk User Retrieval**: Get all users with a single `UsersRequest()` call
- **Simple Architecture**: Straightforward API design

### MT5 Manager API
- **Modern .NET DLLs**: Direct integration with MetaQuotes .NET libraries
- **Granular Control**: Get specific users or groups
- **Rich Functionality**: Advanced account and trading operations

## 🚀 Usage Examples

### Basic Connection and User Retrieval

```csharp
using MT5ManagerAPI;

// Initialize and connect
using (var api = new MT5ApiWrapper())
{
    if (!api.Initialize())
        throw new Exception("Failed to initialize MT5 API");
    
    if (!api.Connect("your-mt5-server", 12345, "manager-password"))
        throw new Exception("Failed to connect to MT5 server");
    
    // Get user information
    var user = api.GetUser(67890);
    Console.WriteLine($"User: {user.Name}, Group: {user.Group}");
    
    // Get account information
    var account = api.GetAccount(67890);
    Console.WriteLine($"Balance: {account.Balance}, Equity: {account.Equity}");
    
    // Get users in a group
    var users = api.GetUsersInGroup("demo");
    foreach (var u in users)
    {
        Console.WriteLine($"User: {u.Login} - {u.Name}");
    }
}
```

### Using the Web API

```bash
# Connect to MT5 server
curl -X POST http://localhost:8080/api/connect \
  -H "Content-Type: application/json" \
  -d '{"server":"your-server","login":12345,"password":"your-password"}'

# Get user information
curl http://localhost:8080/api/user/67890

# Perform balance operation
curl -X POST http://localhost:8080/api/balance \
  -H "Content-Type: application/json" \
  -d '{"login":67890,"amount":100.0,"comment":"Deposit"}'
```

## 📊 API Comparison: MT4 vs MT5

| Feature | MT4 | MT5 |
|---------|-----|-----|
| **User Retrieval** | All users at once (`UsersRequest()`) | Individual or by group |
| **Performance** | Slower (bulk data) | Faster (targeted queries) |
| **Flexibility** | Limited | High (granular control) |
| **Complexity** | Simple | More complex |
| **Data Access** | Properties | Methods (e.g., `user.Name()`) |

### MT4 Advantages
- ✅ Simple API design
- ✅ Get all users with one call
- ✅ No need to know group names

### MT5 Advantages
- ✅ Better performance for targeted queries
- ✅ More granular control
- ✅ Modern architecture
- ✅ Rich functionality

## 🛠️ Applications Included

### 1. Console Application (`MT5ConsoleApp`)
Interactive command-line tool with menu-driven interface:
- Connect to MT5 server
- Get user and account information
- Manage user groups
- Perform balance operations
- View trading history

### 2. Web API Server (`MT5WebAPI`)
RESTful API server providing HTTP endpoints:
- `POST /api/connect` - Server connection
- `GET /api/user/{login}` - User information
- `GET /api/account/{login}` - Account details
- `POST /api/balance` - Balance operations
- `GET /api/status` - Connection status

### 3. Windows Forms Applications
Original GUI applications for both MT4 and MT5 with form-based interfaces.

## 📚 Documentation

- **[Quick Setup Guide](QUICK_SETUP.md)** - Get started in 5 minutes
- **[Detailed Setup Instructions](MT5_SETUP_INSTRUCTIONS.md)** - Complete setup guide
- **[API Documentation](docs/API.md)** - Full API reference
- **[Swagger/OpenAPI Spec](swagger.yaml)** - Machine-readable API specification
- **[Interactive Swagger UI](docs/swagger-ui.html)** - Browse and test the API
- **[Interactive Playground](docs/PLAYGROUND.md)** - Try the API online

## 🔧 Prerequisites

1. **Windows Environment** - MT Manager API is Windows-only
2. **Visual Studio or MSBuild** - For building the projects
3. **MT5 Manager API DLLs** - From MetaQuotes or your broker
4. **Manager Credentials** - Broker-provided manager account

## 📦 Required DLL Files

### .NET Wrapper DLLs
- `MetaQuotes.MT5CommonAPI.dll` (or `MetaQuotes.MT5CommonAPI64.dll`)
- `MetaQuotes.MT5GatewayAPI.dll` (or `MetaQuotes.MT5GatewayAPI64.dll`)
- `MetaQuotes.MT5ManagerAPI.dll` (or `MetaQuotes.MT5ManagerAPI64.dll`)
- `MetaQuotes.MT5WebAPI.dll`

### Native DLLs
- `MT5APIGateway.dll` (or `MT5APIGateway64.dll`)
- `MT5APIManager.dll` (or `MT5APIManager64.dll`)

**Important**: Use either ALL 32-bit OR ALL 64-bit versions consistently!

## 🔐 Security Best Practices

- 🔒 Never hardcode credentials in source code
- 🌍 Use environment variables for sensitive data
- 🔐 Implement authentication for Web API endpoints
- 🛡️ Use HTTPS in production environments
- 📝 Log access attempts for security monitoring

## 🐛 Troubleshooting

### Common Issues

**"Failed to initialize MT5 Manager API"**
- ✅ Ensure all DLL files are in the correct directory
- ✅ Check architecture consistency (32-bit vs 64-bit)
- ✅ Verify DLL files are not blocked by Windows

**"Failed to connect to MT5 server"**
- ✅ Verify server address and port
- ✅ Ensure you're using Manager credentials
- ✅ Check network connectivity and firewall settings

**"Assembly not found" errors**
- ✅ Run `setup-dlls.bat` to copy DLLs to all projects
- ✅ Ensure both .NET and native DLLs are present
- ✅ Check DLL version compatibility

## 📞 Getting Help

1. Check the [troubleshooting section](MT5_SETUP_INSTRUCTIONS.md#troubleshooting)
2. Review console output for detailed error messages
3. Verify your MT5 server settings and credentials
4. Ensure all required DLL files are present and correct version

## 📄 License

This project is provided as-is for educational and development purposes. Please ensure you have proper licensing for the MetaQuotes MT Manager API DLLs from MetaQuotes or your broker.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests to improve this wrapper library.

---

**Made with ❤️ for the MetaTrader development community**
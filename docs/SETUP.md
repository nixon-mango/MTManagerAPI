# Complete Setup Guide - MT Manager API

This comprehensive guide will walk you through setting up the MT Manager API from scratch, including troubleshooting common issues and best practices.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Quick Setup (5 minutes)](#quick-setup-5-minutes)
- [Detailed Setup](#detailed-setup)
- [Verification](#verification)
- [Troubleshooting](#troubleshooting)
- [Advanced Configuration](#advanced-configuration)
- [Production Deployment](#production-deployment)

## Prerequisites

### System Requirements
- **Operating System**: Windows 7/8/10/11 or Windows Server 2012/2016/2019/2022
- **Architecture**: 32-bit or 64-bit (must be consistent across all components)
- **.NET Framework**: 4.7.2 or later
- **Visual Studio**: 2019 or later, OR Visual Studio Build Tools
- **Memory**: Minimum 2GB RAM, recommended 4GB+
- **Disk Space**: 500MB for the project and dependencies

### Required Files
You need the MT5 Manager API DLL files from MetaQuotes. These are typically provided by your broker or obtained from MetaQuotes directly.

#### .NET Wrapper DLLs (Required)
- `MetaQuotes.MT5CommonAPI.dll` (32-bit) or `MetaQuotes.MT5CommonAPI64.dll` (64-bit)
- `MetaQuotes.MT5GatewayAPI.dll` (32-bit) or `MetaQuotes.MT5GatewayAPI64.dll` (64-bit)
- `MetaQuotes.MT5ManagerAPI.dll` (32-bit) or `MetaQuotes.MT5ManagerAPI64.dll` (64-bit)
- `MetaQuotes.MT5WebAPI.dll`

#### Native DLLs (Required)
- `MT5APIGateway.dll` (32-bit) or `MT5APIGateway64.dll` (64-bit)
- `MT5APIManager.dll` (32-bit) or `MT5APIManager64.dll` (64-bit)

### Access Requirements
- **Manager Account Credentials**: You need MT5 Manager account credentials (not regular trader account)
- **Server Access**: Your MT5 server must be accessible from your development machine
- **Network**: Ensure firewall allows connections to your MT5 server

## Quick Setup (5 minutes)

If you want to get started quickly and have all prerequisites:

### Step 1: Create DLL Directory
```cmd
mkdir DLLs
```

### Step 2: Copy Your DLLs
Place all your MT5 Manager API DLL files in the `DLLs` folder.

### Step 3: Run Setup
```cmd
setup-dlls.bat
```

### Step 4: Build
```cmd
build.bat
```

### Step 5: Test
```cmd
MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
```

That's it! If everything works, you're ready to go. If you encounter issues, continue with the detailed setup below.

## Detailed Setup

### Step 1: Environment Preparation

#### 1.1 Verify Windows Version
```cmd
ver
```
Ensure you're running a supported Windows version.

#### 1.2 Check .NET Framework
```cmd
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" /v Release
```
The release number should be 461808 or higher for .NET Framework 4.7.2.

#### 1.3 Install Visual Studio Build Tools (if needed)
If you don't have Visual Studio installed:

1. Download Visual Studio Build Tools from Microsoft
2. Install with C++ build tools and .NET Framework targeting packs
3. Verify installation:
```cmd
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" -version
```

### Step 2: Project Structure Setup

#### 2.1 Create Directory Structure
```cmd
mkdir DLLs
mkdir docs
mkdir logs
mkdir config
```

Your directory structure should look like:
```
/workspace/
├── DLLs/                       # MT5 Manager API DLLs
├── docs/                       # Documentation
├── logs/                       # Application logs
├── config/                     # Configuration files
├── MT5ManagerAPI/             # Core library
├── MT5ConsoleApp/             # Console application
├── MT5WebAPI/                 # Web API server
├── MT4/                       # MT4 Forms app
├── MT5/                       # MT5 Forms app
├── build.bat                  # Build script
├── setup-dlls.bat            # DLL setup script
└── README.md
```

#### 2.2 Verify Project Files
Ensure all project files are present:
```cmd
dir *.sln
dir *.bat
dir MT5ManagerAPI\*.csproj
dir MT5ConsoleApp\*.csproj
dir MT5WebAPI\*.csproj
```

### Step 3: DLL Management

#### 3.1 Organize Your DLLs
Place your MT5 Manager API DLL files in the `DLLs` folder. Ensure you have:

**For 32-bit:**
- MetaQuotes.MT5CommonAPI.dll
- MetaQuotes.MT5GatewayAPI.dll
- MetaQuotes.MT5ManagerAPI.dll
- MetaQuotes.MT5WebAPI.dll
- MT5APIGateway.dll
- MT5APIManager.dll

**For 64-bit:**
- MetaQuotes.MT5CommonAPI64.dll
- MetaQuotes.MT5GatewayAPI64.dll
- MetaQuotes.MT5ManagerAPI64.dll
- MetaQuotes.MT5WebAPI.dll
- MT5APIGateway64.dll
- MT5APIManager64.dll

#### 3.2 Verify DLL Architecture
```cmd
# Check if DLLs are 32-bit or 64-bit
powershell -Command "Get-Item DLLs\*.dll | ForEach-Object { Write-Host $_.Name; [System.Reflection.AssemblyName]::GetAssemblyName($_.FullName).ProcessorArchitecture }"
```

#### 3.3 Unblock DLLs (Important!)
Windows may block downloaded DLLs. Unblock them:
```cmd
powershell -Command "Get-ChildItem -Path DLLs\*.dll | Unblock-File"
```

#### 3.4 Run DLL Setup Script
```cmd
setup-dlls.bat
```

This script will:
- Verify all required DLLs are present
- Copy DLLs to all project output directories
- Set up proper directory structure
- Verify the setup

### Step 4: Build Process

#### 4.1 Clean Previous Builds
```cmd
for /d %d in (MT5ManagerAPI\bin MT5ManagerAPI\obj MT5ConsoleApp\bin MT5ConsoleApp\obj MT5WebAPI\bin MT5WebAPI\obj) do if exist "%d" rmdir /s /q "%d"
```

#### 4.2 Restore NuGet Packages (if applicable)
```cmd
nuget restore MT5ManagerAPI.sln
```

#### 4.3 Build Solution
```cmd
build.bat
```

Or manually:
```cmd
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" MT5ManagerAPI.sln /p:Configuration=Debug /p:Platform="Any CPU"
```

#### 4.4 Verify Build Output
Check that all executables were created:
```cmd
dir MT5ManagerAPI\bin\Debug\MT5ManagerAPI.dll
dir MT5ConsoleApp\bin\Debug\MT5ConsoleApp.exe
dir MT5WebAPI\bin\Debug\MT5WebAPI.exe
```

### Step 5: Configuration

#### 5.1 Create Configuration Files

**MT5ConsoleApp Configuration** (`MT5ConsoleApp\App.config`):
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="DefaultServer" value="your-server.com:443" />
        <add key="ConnectionTimeout" value="30" />
        <add key="RetryAttempts" value="3" />
        <add key="LogLevel" value="Info" />
    </appSettings>
    <startup>
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
    </startup>
</configuration>
```

**MT5WebAPI Configuration** (`MT5WebAPI\App.config`):
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="DefaultPort" value="8080" />
        <add key="DefaultHost" value="localhost" />
        <add key="EnableCors" value="true" />
        <add key="LogRequests" value="true" />
    </appSettings>
    <startup>
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
    </startup>
</configuration>
```

#### 5.2 Set Up Logging
Create a simple logging configuration in `config\logging.config`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="LogPath" value="logs\" />
        <add key="LogLevel" value="Info" />
        <add key="MaxLogFiles" value="10" />
    </appSettings>
</configuration>
```

## Verification

### Step 1: Basic Functionality Test
```cmd
cd MT5ConsoleApp\bin\Debug
MT5ConsoleApp.exe
```

Expected output:
```
=== MT5 Manager API Console Application ===

1. Connect to MT5 Server
2. Get User Information
3. Get Account Information
...

Enter your choice (1-9):
```

### Step 2: Web API Test
```cmd
cd MT5WebAPI\bin\Debug
MT5WebAPI.exe --host localhost --port 8080
```

Expected output:
```
MT5 Web API Server
Starting server on http://localhost:8080
Server started successfully!
Press Ctrl+C to stop...
```

### Step 3: API Endpoint Test
In another terminal:
```cmd
curl http://localhost:8080/api/status
```

Expected response:
```json
{
  "status": "running",
  "version": "1.0.0",
  "connected": false
}
```

### Step 4: Connection Test
```cmd
curl -X POST http://localhost:8080/api/connect -H "Content-Type: application/json" -d "{\"server\":\"your-server\",\"login\":12345,\"password\":\"your-password\"}"
```

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: "MetaQuotes.MT5CommonAPI does not exist"

**Cause**: DLL files are not in the correct location or not referenced properly.

**Solution**:
1. Verify DLLs are in the `DLLs` folder
2. Run `setup-dlls.bat` again
3. Check that DLLs were copied to `bin\Debug` folders
4. Ensure DLLs are unblocked:
   ```cmd
   powershell -Command "Get-ChildItem -Recurse -Path . -Name *.dll | ForEach-Object { Unblock-File $_ }"
   ```

#### Issue 2: "Failed to initialize MT5 Manager API"

**Cause**: Architecture mismatch between DLLs and application, or missing native DLLs.

**Solution**:
1. Ensure all DLLs are the same architecture (32-bit or 64-bit)
2. Verify both .NET and native DLLs are present
3. Check Windows Event Log for detailed error messages
4. Try running as Administrator

#### Issue 3: "Failed to connect to MT5 server"

**Cause**: Network issues, incorrect credentials, or server configuration.

**Solution**:
1. Verify server address and port
2. Test network connectivity: `telnet your-server.com 443`
3. Ensure you're using Manager credentials (not trader)
4. Check firewall settings
5. Verify server allows Manager API connections

#### Issue 4: Build Errors

**Cause**: Missing dependencies, incorrect .NET Framework version, or corrupted project files.

**Solution**:
1. Clean and rebuild:
   ```cmd
   "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" MT5ManagerAPI.sln /t:Clean
   "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" MT5ManagerAPI.sln /t:Rebuild
   ```
2. Check .NET Framework version in project files
3. Restore NuGet packages if needed
4. Verify Visual Studio Build Tools installation

#### Issue 5: Access Denied Errors

**Cause**: Insufficient permissions or antivirus interference.

**Solution**:
1. Run Command Prompt as Administrator
2. Add project folder to antivirus exclusions
3. Check folder permissions
4. Disable real-time protection temporarily for testing

### Diagnostic Commands

#### Check DLL Dependencies
```cmd
# Using PowerShell
powershell -Command "Add-Type -AssemblyName System.Reflection; [System.Reflection.Assembly]::LoadFile('$(pwd)\DLLs\MetaQuotes.MT5CommonAPI.dll').GetReferencedAssemblies() | Format-Table"
```

#### Verify .NET Framework
```cmd
powershell -Command "[System.Runtime.InteropServices.RuntimeInformation]::FrameworkDescription"
```

#### Check Process Architecture
```cmd
powershell -Command "[System.Environment]::Is64BitProcess"
```

#### Test Network Connectivity
```cmd
telnet your-mt5-server.com 443
# Or using PowerShell
powershell -Command "Test-NetConnection -ComputerName your-mt5-server.com -Port 443"
```

## Advanced Configuration

### Performance Optimization

#### 1. Connection Pooling
For high-frequency applications, implement connection pooling:

```csharp
public class MT5ConnectionPool
{
    private readonly ConcurrentQueue<MT5ApiWrapper> _connections = new();
    private readonly SemaphoreSlim _semaphore;
    private readonly string _server;
    private readonly ulong _login;
    private readonly string _password;
    
    public MT5ConnectionPool(string server, ulong login, string password, int maxConnections = 5)
    {
        _server = server;
        _login = login;
        _password = password;
        _semaphore = new SemaphoreSlim(maxConnections, maxConnections);
    }
    
    public async Task<T> ExecuteAsync<T>(Func<MT5ApiWrapper, T> operation)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            if (!_connections.TryDequeue(out var api))
            {
                api = new MT5ApiWrapper();
                api.Initialize();
                api.Connect(_server, _login, _password);
            }
            
            var result = operation(api);
            _connections.Enqueue(api);
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

#### 2. Caching Configuration
```csharp
// Add to App.config
<appSettings>
    <add key="CacheEnabled" value="true" />
    <add key="CacheExpirationMinutes" value="5" />
    <add key="MaxCacheSize" value="1000" />
</appSettings>
```

#### 3. Logging Configuration
```csharp
// Enhanced logging setup
public static class LoggingConfig
{
    public static void Setup()
    {
        var logPath = ConfigurationManager.AppSettings["LogPath"] ?? "logs\\";
        var logLevel = ConfigurationManager.AppSettings["LogLevel"] ?? "Info";
        
        // Configure your preferred logging framework
        // Example: NLog, Serilog, or built-in logging
    }
}
```

### Security Configuration

#### 1. Credential Management
Never store credentials in code. Use secure configuration:

```xml
<!-- In App.config -->
<appSettings>
    <add key="CredentialsSource" value="Environment" />
    <!-- or "Registry", "File", "KeyVault" -->
</appSettings>
```

```csharp
public static class CredentialManager
{
    public static (string server, ulong login, string password) GetCredentials()
    {
        var source = ConfigurationManager.AppSettings["CredentialsSource"];
        
        switch (source)
        {
            case "Environment":
                return (
                    Environment.GetEnvironmentVariable("MT5_SERVER"),
                    ulong.Parse(Environment.GetEnvironmentVariable("MT5_LOGIN")),
                    Environment.GetEnvironmentVariable("MT5_PASSWORD")
                );
            // Add other sources as needed
        }
    }
}
```

#### 2. Network Security
```xml
<system.net>
    <settings>
        <httpWebRequest useUnsafeHeaderParsing="false" />
    </settings>
    <connectionManagement>
        <add address="*" maxconnection="10" />
    </connectionManagement>
</system.net>
```

### Monitoring and Health Checks

#### 1. Health Check Implementation
```csharp
public class MT5HealthChecker
{
    private readonly MT5ApiWrapper _api;
    private DateTime _lastCheck;
    private bool _lastResult;
    
    public async Task<HealthStatus> CheckHealthAsync()
    {
        try
        {
            // Perform a lightweight operation to test connectivity
            var testResult = _api.GetUser(1); // Use a known test user
            _lastResult = true;
            _lastCheck = DateTime.Now;
            
            return new HealthStatus
            {
                IsHealthy = true,
                Message = "MT5 API is responsive",
                LastCheck = _lastCheck
            };
        }
        catch (Exception ex)
        {
            _lastResult = false;
            _lastCheck = DateTime.Now;
            
            return new HealthStatus
            {
                IsHealthy = false,
                Message = $"MT5 API error: {ex.Message}",
                LastCheck = _lastCheck,
                Exception = ex
            };
        }
    }
}
```

#### 2. Performance Monitoring
```csharp
public class PerformanceMonitor
{
    private readonly Dictionary<string, List<TimeSpan>> _operationTimes = new();
    
    public T MonitorOperation<T>(string operationName, Func<T> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = operation();
            stopwatch.Stop();
            
            RecordOperationTime(operationName, stopwatch.Elapsed);
            return result;
        }
        catch
        {
            stopwatch.Stop();
            RecordOperationTime(operationName + "_Failed", stopwatch.Elapsed);
            throw;
        }
    }
    
    private void RecordOperationTime(string operation, TimeSpan duration)
    {
        if (!_operationTimes.ContainsKey(operation))
            _operationTimes[operation] = new List<TimeSpan>();
        
        _operationTimes[operation].Add(duration);
        
        // Keep only last 100 measurements
        if (_operationTimes[operation].Count > 100)
            _operationTimes[operation].RemoveAt(0);
    }
    
    public PerformanceStats GetStats(string operation)
    {
        if (!_operationTimes.ContainsKey(operation))
            return null;
        
        var times = _operationTimes[operation];
        return new PerformanceStats
        {
            Operation = operation,
            Count = times.Count,
            Average = TimeSpan.FromMilliseconds(times.Average(t => t.TotalMilliseconds)),
            Min = times.Min(),
            Max = times.Max()
        };
    }
}
```

## Production Deployment

### Prerequisites for Production
1. **Dedicated Server**: Use a dedicated Windows server for production
2. **Service Account**: Create a dedicated Windows service account
3. **SSL/TLS**: Configure secure connections
4. **Monitoring**: Implement comprehensive monitoring
5. **Backup**: Set up configuration and log backups

### Deployment Steps

#### 1. Server Preparation
```cmd
# Install Windows Server roles
dism /online /enable-feature /featurename:IIS-WebServerRole
dism /online /enable-feature /featurename:IIS-WebServer
dism /online /enable-feature /featurename:IIS-CommonHttpFeatures
dism /online /enable-feature /featurename:IIS-HttpErrors
dism /online /enable-feature /featurename:IIS-HttpLogging
```

#### 2. Service Installation
Create a Windows Service for the Web API:

```csharp
// In Program.cs for MT5WebAPI
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--service")
        {
            ServiceBase.Run(new MT5WebAPIService());
        }
        else
        {
            // Run as console application
            RunAsConsole();
        }
    }
}

public class MT5WebAPIService : ServiceBase
{
    private WebServer _server;
    
    protected override void OnStart(string[] args)
    {
        _server = new WebServer();
        _server.Start();
    }
    
    protected override void OnStop()
    {
        _server?.Stop();
    }
}
```

#### 3. Install as Windows Service
```cmd
sc create "MT5WebAPI" binPath="C:\Path\To\MT5WebAPI.exe --service" start=auto
sc description "MT5WebAPI" "MT5 Manager API Web Service"
sc start "MT5WebAPI"
```

#### 4. Configure Firewall
```cmd
netsh advfirewall firewall add rule name="MT5WebAPI" dir=in action=allow protocol=TCP localport=8080
```

#### 5. Set up SSL Certificate (for HTTPS)
```cmd
# Using IIS Manager or PowerShell
New-SelfSignedCertificate -DnsName "your-server.com" -CertStoreLocation "cert:\LocalMachine\My"
```

### Production Configuration

#### 1. Production App.config
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="Environment" value="Production" />
        <add key="DefaultPort" value="443" />
        <add key="EnableHttps" value="true" />
        <add key="CertificateThumbprint" value="YOUR_CERT_THUMBPRINT" />
        <add key="LogLevel" value="Warning" />
        <add key="MaxConcurrentConnections" value="100" />
        <add key="ConnectionTimeout" value="30" />
        <add key="HealthCheckInterval" value="60" />
    </appSettings>
</configuration>
```

#### 2. Production Monitoring
Set up monitoring for:
- API response times
- Connection success rates
- Memory usage
- CPU usage
- Disk space
- Network connectivity
- Error rates

#### 3. Backup Strategy
```cmd
# Daily backup script
@echo off
set BACKUP_DIR=C:\Backups\MT5API\%date:~-4,4%-%date:~-10,2%-%date:~-7,2%
mkdir "%BACKUP_DIR%"

# Backup configuration
xcopy /s /y "C:\MT5API\config\*" "%BACKUP_DIR%\config\"

# Backup logs (last 7 days)
forfiles /p "C:\MT5API\logs" /m *.log /d -7 /c "cmd /c copy @path %BACKUP_DIR%\logs\"

# Compress backup
powershell -Command "Compress-Archive -Path '%BACKUP_DIR%' -DestinationPath '%BACKUP_DIR%.zip'"
```

This completes the comprehensive setup guide. You should now have a fully functional MT Manager API setup that's ready for both development and production use.
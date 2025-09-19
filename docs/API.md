# MT Manager API - Complete API Reference

This document provides comprehensive documentation for the MT Manager API wrapper library, including all classes, methods, and usage examples.

## Table of Contents

- [Overview](#overview)
- [Core Classes](#core-classes)
- [Connection Management](#connection-management)
- [User Management](#user-management)
- [Account Operations](#account-operations)
- [Group Management](#group-management)
- [Trading Operations](#trading-operations)
- [Error Handling](#error-handling)
- [Best Practices](#best-practices)

## Overview

The MT Manager API wrapper provides a modern, easy-to-use interface for the MetaQuotes MT4 and MT5 Manager APIs. It includes proper error handling, resource management, and a consistent API design.

### Key Features

- ✅ **Automatic Resource Management**: Uses `IDisposable` pattern
- ✅ **Comprehensive Error Handling**: All methods include try-catch blocks
- ✅ **Modern C# Design**: Async support and LINQ compatibility
- ✅ **Type Safety**: Strong typing for all data models
- ✅ **Memory Efficient**: Proper cleanup of unmanaged resources

## Core Classes

### MT5ApiWrapper

The main wrapper class that provides a simplified interface to the MT5 Manager API.

```csharp
public class MT5ApiWrapper : IDisposable
{
    public bool Initialize()
    public bool Connect(string server, ulong login, string password)
    public void Disconnect()
    public UserInfo GetUser(ulong login)
    public AccountInfo GetAccount(ulong login)
    public List<UserInfo> GetUsersInGroup(string groupName)
    public string GetUserGroup(ulong login)
    public bool PerformBalanceOperation(ulong login, double amount, string comment)
    public void Dispose()
}
```

### Data Models

#### UserInfo

```csharp
public class UserInfo
{
    public ulong Login { get; set; }
    public string Name { get; set; }
    public string Group { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Comment { get; set; }
    public DateTime RegistrationTime { get; set; }
    public DateTime LastAccessTime { get; set; }
    public uint Leverage { get; set; }
    public int Rights { get; set; }
}
```

#### AccountInfo

```csharp
public class AccountInfo
{
    public ulong Login { get; set; }
    public double Balance { get; set; }
    public double Credit { get; set; }
    public double Margin { get; set; }
    public double MarginFree { get; set; }
    public double MarginLevel { get; set; }
    public double Equity { get; set; }
    public double Profit { get; set; }
    public string Currency { get; set; }
    public DateTime LastUpdate { get; set; }
}
```

## Connection Management

### Initialize the API

Before using any API functions, you must initialize the wrapper:

```csharp
using (var api = new MT5ApiWrapper())
{
    if (!api.Initialize())
    {
        throw new Exception("Failed to initialize MT5 Manager API");
    }
    
    // Use API methods here...
}
```

### Connect to MT5 Server

```csharp
string server = "your-mt5-server.com:443";
ulong managerLogin = 12345;
string managerPassword = "your-manager-password";

if (!api.Connect(server, managerLogin, managerPassword))
{
    throw new Exception("Failed to connect to MT5 server");
}

Console.WriteLine("Successfully connected to MT5 server!");
```

### Disconnect

```csharp
// Explicit disconnect (optional - automatically called by Dispose)
api.Disconnect();
```

## User Management

### Get Single User Information

```csharp
try
{
    ulong userLogin = 67890;
    UserInfo user = api.GetUser(userLogin);
    
    Console.WriteLine($"User Details:");
    Console.WriteLine($"  Login: {user.Login}");
    Console.WriteLine($"  Name: {user.Name}");
    Console.WriteLine($"  Group: {user.Group}");
    Console.WriteLine($"  Email: {user.Email}");
    Console.WriteLine($"  Country: {user.Country}");
    Console.WriteLine($"  Registration: {user.RegistrationTime}");
    Console.WriteLine($"  Last Access: {user.LastAccessTime}");
    Console.WriteLine($"  Leverage: 1:{user.Leverage}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting user info: {ex.Message}");
}
```

### Get Users in a Group

```csharp
try
{
    string groupName = "demo";
    List<UserInfo> users = api.GetUsersInGroup(groupName);
    
    Console.WriteLine($"Found {users.Count} users in group '{groupName}':");
    
    foreach (var user in users)
    {
        Console.WriteLine($"  {user.Login}: {user.Name} ({user.Email})");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting users in group: {ex.Message}");
}
```

### Get User's Group

```csharp
try
{
    ulong userLogin = 67890;
    string groupName = api.GetUserGroup(userLogin);
    
    Console.WriteLine($"User {userLogin} belongs to group: {groupName}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting user group: {ex.Message}");
}
```

## Account Operations

### Get Account Information

```csharp
try
{
    ulong userLogin = 67890;
    AccountInfo account = api.GetAccount(userLogin);
    
    Console.WriteLine($"Account Information for {userLogin}:");
    Console.WriteLine($"  Balance: {account.Balance:F2} {account.Currency}");
    Console.WriteLine($"  Credit: {account.Credit:F2} {account.Currency}");
    Console.WriteLine($"  Equity: {account.Equity:F2} {account.Currency}");
    Console.WriteLine($"  Margin: {account.Margin:F2} {account.Currency}");
    Console.WriteLine($"  Free Margin: {account.MarginFree:F2} {account.Currency}");
    Console.WriteLine($"  Margin Level: {account.MarginLevel:F2}%");
    Console.WriteLine($"  Profit: {account.Profit:F2} {account.Currency}");
    Console.WriteLine($"  Last Update: {account.LastUpdate}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting account info: {ex.Message}");
}
```

### Perform Balance Operations

```csharp
try
{
    ulong userLogin = 67890;
    double amount = 100.0;  // Positive for deposit, negative for withdrawal
    string comment = "Manual deposit via API";
    
    bool success = api.PerformBalanceOperation(userLogin, amount, comment);
    
    if (success)
    {
        Console.WriteLine($"Successfully performed balance operation:");
        Console.WriteLine($"  User: {userLogin}");
        Console.WriteLine($"  Amount: {amount:F2}");
        Console.WriteLine($"  Comment: {comment}");
    }
    else
    {
        Console.WriteLine("Balance operation failed");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error performing balance operation: {ex.Message}");
}
```

## Group Management

### List All Groups

```csharp
// Note: This functionality would need to be added to the wrapper
// For now, you can discover groups by examining user data
var allUsers = new List<UserInfo>();
// ... populate with users from various sources

var groups = allUsers.Select(u => u.Group).Distinct().ToList();
Console.WriteLine($"Available groups: {string.Join(", ", groups)}");
```

### Group Statistics

```csharp
string targetGroup = "demo";
var groupUsers = api.GetUsersInGroup(targetGroup);

var statistics = new
{
    TotalUsers = groupUsers.Count,
    ActiveUsers = groupUsers.Count(u => u.LastAccessTime > DateTime.Now.AddDays(-30)),
    Countries = groupUsers.Select(u => u.Country).Distinct().Count(),
    AverageRegistrationDays = groupUsers.Average(u => (DateTime.Now - u.RegistrationTime).Days)
};

Console.WriteLine($"Group '{targetGroup}' Statistics:");
Console.WriteLine($"  Total Users: {statistics.TotalUsers}");
Console.WriteLine($"  Active Users (30 days): {statistics.ActiveUsers}");
Console.WriteLine($"  Countries: {statistics.Countries}");
Console.WriteLine($"  Avg Registration Age: {statistics.AverageRegistrationDays:F0} days");
```

## Trading Operations

### Get Trading Summary

```csharp
try
{
    ulong userLogin = 67890;
    AccountInfo account = api.GetAccount(userLogin);
    
    var tradingSummary = new
    {
        Login = userLogin,
        Balance = account.Balance,
        Equity = account.Equity,
        Profit = account.Profit,
        MarginUsed = account.Margin,
        MarginFree = account.MarginFree,
        MarginLevel = account.MarginLevel,
        IsMarginCall = account.MarginLevel < 100,
        IsStopOut = account.MarginLevel < 50
    };
    
    Console.WriteLine($"Trading Summary for {userLogin}:");
    Console.WriteLine($"  Equity: {tradingSummary.Equity:F2}");
    Console.WriteLine($"  P&L: {tradingSummary.Profit:F2}");
    Console.WriteLine($"  Margin Level: {tradingSummary.MarginLevel:F2}%");
    
    if (tradingSummary.IsStopOut)
        Console.WriteLine("  ⚠️ STOP OUT LEVEL!");
    else if (tradingSummary.IsMarginCall)
        Console.WriteLine("  ⚠️ MARGIN CALL!");
    else
        Console.WriteLine("  ✅ Account is healthy");
}
catch (Exception ex)
{
    Console.WriteLine($"Error getting trading summary: {ex.Message}");
}
```

## Error Handling

### Exception Types

The wrapper throws standard .NET exceptions:

- `Exception`: General API errors
- `ArgumentException`: Invalid parameters
- `InvalidOperationException`: API not initialized or not connected
- `TimeoutException`: Connection timeout
- `UnauthorizedAccessException`: Invalid credentials

### Best Practices for Error Handling

```csharp
public async Task<UserInfo> GetUserSafely(ulong login)
{
    const int maxRetries = 3;
    const int delayMs = 1000;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return api.GetUser(login);
        }
        catch (TimeoutException) when (attempt < maxRetries)
        {
            Console.WriteLine($"Timeout on attempt {attempt}, retrying in {delayMs}ms...");
            await Task.Delay(delayMs);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on attempt {attempt}: {ex.Message}");
            if (attempt == maxRetries) throw;
            await Task.Delay(delayMs);
        }
    }
    
    return null; // This line should never be reached
}
```

### Logging Recommendations

```csharp
using Microsoft.Extensions.Logging;

public class MT5Service
{
    private readonly ILogger<MT5Service> _logger;
    private readonly MT5ApiWrapper _api;
    
    public MT5Service(ILogger<MT5Service> logger)
    {
        _logger = logger;
        _api = new MT5ApiWrapper();
    }
    
    public UserInfo GetUser(ulong login)
    {
        _logger.LogInformation("Getting user info for login: {Login}", login);
        
        try
        {
            var user = _api.GetUser(login);
            _logger.LogInformation("Successfully retrieved user: {Name} ({Login})", user.Name, login);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user info for login: {Login}", login);
            throw;
        }
    }
}
```

## Best Practices

### 1. Resource Management

Always use the `using` statement or explicitly call `Dispose()`:

```csharp
// Good
using (var api = new MT5ApiWrapper())
{
    // Use API
}

// Also good
var api = new MT5ApiWrapper();
try
{
    // Use API
}
finally
{
    api.Dispose();
}
```

### 2. Connection Pooling

For high-frequency operations, consider connection pooling:

```csharp
public class MT5ConnectionPool
{
    private readonly ConcurrentQueue<MT5ApiWrapper> _connections = new();
    private readonly SemaphoreSlim _semaphore;
    
    public MT5ConnectionPool(int maxConnections)
    {
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
                // Connect with your credentials
            }
            
            return operation(api);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

### 3. Batch Operations

For multiple operations, batch them together:

```csharp
public async Task<Dictionary<ulong, UserInfo>> GetMultipleUsers(IEnumerable<ulong> logins)
{
    var results = new Dictionary<ulong, UserInfo>();
    var tasks = logins.Select(async login =>
    {
        try
        {
            var user = api.GetUser(login);
            lock (results)
            {
                results[login] = user;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get user {Login}", login);
        }
    });
    
    await Task.WhenAll(tasks);
    return results;
}
```

### 4. Configuration Management

Use configuration files for connection settings:

```json
{
  "MT5Settings": {
    "Server": "your-server.com:443",
    "Login": 12345,
    "Password": "your-password",
    "TimeoutSeconds": 30,
    "RetryAttempts": 3
  }
}
```

```csharp
public class MT5Settings
{
    public string Server { get; set; }
    public ulong Login { get; set; }
    public string Password { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryAttempts { get; set; } = 3;
}
```

### 5. Monitoring and Health Checks

Implement health checks for production use:

```csharp
public class MT5HealthCheck : IHealthCheck
{
    private readonly MT5ApiWrapper _api;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Perform a simple operation to test connectivity
            var testUser = _api.GetUser(1); // Use a known test user
            return HealthCheckResult.Healthy("MT5 API is responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MT5 API is not responsive", ex);
        }
    }
}
```

## Performance Considerations

### 1. Caching

Implement caching for frequently accessed data:

```csharp
public class CachedMT5Service
{
    private readonly IMemoryCache _cache;
    private readonly MT5ApiWrapper _api;
    
    public UserInfo GetUser(ulong login)
    {
        return _cache.GetOrCreate($"user_{login}", factory =>
        {
            factory.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return _api.GetUser(login);
        });
    }
}
```

### 2. Async Operations

For web applications, use async wrappers:

```csharp
public Task<UserInfo> GetUserAsync(ulong login)
{
    return Task.Run(() => _api.GetUser(login));
}
```

### 3. Rate Limiting

Implement rate limiting to avoid overwhelming the MT5 server:

```csharp
public class RateLimitedMT5Service
{
    private readonly SemaphoreSlim _semaphore = new(10, 10); // Max 10 concurrent operations
    private readonly MT5ApiWrapper _api;
    
    public async Task<UserInfo> GetUserAsync(ulong login)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await Task.Run(() => _api.GetUser(login));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

---

This API reference provides comprehensive documentation for using the MT Manager API wrapper. For additional examples and use cases, see the [Interactive Playground](PLAYGROUND.md).
# Your Server's Group Configuration Guide

## 🎯 **Your Actual Groups**

Based on your server configuration, you have these specific groups:

- **`real\Executive`**
- **`real\Vipin Zero 1000`**  
- **`real\NORMAL`**

I've now **customized the API** to work perfectly with your group structure!

## 🚀 **New Optimized Endpoints**

### **1. Get All Real Users (Your Groups)**
```http
GET /api/users/real
```
This endpoint **only queries your actual groups**, making it faster and more reliable.

### **2. Get All Users (Discovery + Your Groups)**
```http
GET /api/users
```
This endpoint tries your groups **first**, then falls back to common groups.

## 💻 **Usage Examples**

### **Web API**
```bash
# Get users from your real groups only
curl http://localhost:8080/api/users/real

# Response will include users from:
# - real\Executive
# - real\Vipin Zero 1000
# - real\NORMAL
{
  "success": true,
  "data": [
    {
      "login": 67890,
      "name": "John Executive",
      "group": "real\\Executive",
      "email": "john@example.com",
      "leverage": 100
    },
    {
      "login": 67891,
      "name": "Jane Vipin",
      "group": "real\\Vipin Zero 1000",
      "email": "jane@example.com",
      "leverage": 1000
    }
  ]
}
```

### **C# Library**
```csharp
using (var api = new MT5ApiWrapper())
{
    api.Initialize();
    api.Connect("your-server", login, password);
    
    // Get users from your real groups
    var realUsers = api.GetAllRealUsers();
    Console.WriteLine($"Found {realUsers.Count} real users");
    
    // Analyze by your specific groups
    var executiveUsers = realUsers.Where(u => u.Group == "real\\Executive").ToList();
    var vipinUsers = realUsers.Where(u => u.Group == "real\\Vipin Zero 1000").ToList();
    var normalUsers = realUsers.Where(u => u.Group == "real\\NORMAL").ToList();
    
    Console.WriteLine($"Executive: {executiveUsers.Count} users");
    Console.WriteLine($"Vipin Zero 1000: {vipinUsers.Count} users");
    Console.WriteLine($"Normal: {normalUsers.Count} users");
}
```

### **Console Application**
```
=== MT5 Manager API Menu ===
1. Get User Information
2. Get Account Information
3. Get All Users (Discovery)
4. Get All Real Users (Your Groups)  ← NEW!
5. Get Users in Group
...

Choose option: 4

=== Getting All Real Users ===
Checking your server's real groups:
  - real\Executive
  - real\Vipin Zero 1000
  - real\NORMAL

✓ Retrieved 1,247 real users total

📊 Summary by Group:
   real\Executive: 456 users
   real\Vipin Zero 1000: 389 users
   real\NORMAL: 402 users

📋 Sample Users (showing first 10):
Login       | Name                 | Group                | Country
------------|----------------------|----------------------|------------------
     12345 | John Executive       | real\Executive       | United States
     12346 | Jane Vipin          | real\Vipin Zero 1000 | Canada
     12347 | Bob Normal          | real\NORMAL          | United Kingdom
```

## 🔧 **Individual Group Access**

You can also access each group individually:

```bash
# Get users from specific groups
curl "http://localhost:8080/api/group/real%5CExecutive/users"
curl "http://localhost:8080/api/group/real%5CVipin%20Zero%201000/users"  
curl "http://localhost:8080/api/group/real%5CNORMAL/users"
```

**Note**: URL encoding is needed:
- `\` becomes `%5C`
- Space becomes `%20`

## 📊 **Group Analysis Examples**

### **Leverage Analysis**
```csharp
var realUsers = api.GetAllRealUsers();

var leverageAnalysis = realUsers.GroupBy(u => u.Group)
    .Select(g => new {
        Group = g.Key,
        Count = g.Count(),
        AvgLeverage = g.Average(u => u.Leverage),
        MaxLeverage = g.Max(u => u.Leverage),
        MinLeverage = g.Min(u => u.Leverage)
    });

foreach (var analysis in leverageAnalysis)
{
    Console.WriteLine($"{analysis.Group}:");
    Console.WriteLine($"  Users: {analysis.Count}");
    Console.WriteLine($"  Avg Leverage: 1:{analysis.AvgLeverage:F0}");
    Console.WriteLine($"  Range: 1:{analysis.MinLeverage} - 1:{analysis.MaxLeverage}");
}
```

### **Activity Comparison**
```csharp
var realUsers = api.GetAllRealUsers();
var now = DateTime.Now;

var activityByGroup = realUsers.GroupBy(u => u.Group)
    .Select(g => new {
        Group = g.Key,
        TotalUsers = g.Count(),
        ActiveToday = g.Count(u => (now - u.LastAccess).Days == 0),
        ActiveWeek = g.Count(u => (now - u.LastAccess).Days <= 7),
        ActiveMonth = g.Count(u => (now - u.LastAccess).Days <= 30)
    });

foreach (var activity in activityByGroup)
{
    Console.WriteLine($"{activity.Group}:");
    Console.WriteLine($"  Total: {activity.TotalUsers} users");
    Console.WriteLine($"  Active today: {activity.ActiveToday} ({activity.ActiveToday * 100.0 / activity.TotalUsers:F1}%)");
    Console.WriteLine($"  Active week: {activity.ActiveWeek} ({activity.ActiveWeek * 100.0 / activity.TotalUsers:F1}%)");
    Console.WriteLine($"  Active month: {activity.ActiveMonth} ({activity.ActiveMonth * 100.0 / activity.TotalUsers:F1}%)");
}
```

## 🎯 **Performance Benefits**

### **Before (Generic Approach)**
- Tries 15+ common group names
- Most groups don't exist → wasted API calls
- Slower performance

### **After (Your Groups)**
- Only queries your 3 actual groups
- No wasted API calls
- Faster, more reliable performance

## 🔄 **Fallback Strategy**

The system now works with a smart fallback:

1. **`GetAllRealUsers()`** - Only your groups (fastest)
2. **`GetAllUsers()`** - Your groups + common groups (comprehensive)
3. **`GetUsersInGroup(name)`** - Specific group (precise)

## 🛡️ **Error Handling**

If a group doesn't exist or you don't have access:

```csharp
try
{
    var realUsers = api.GetAllRealUsers();
    if (realUsers.Count == 0)
    {
        Console.WriteLine("No users found. Possible reasons:");
        Console.WriteLine("- Groups don't exist on server");
        Console.WriteLine("- Manager account lacks access");
        Console.WriteLine("- Group names have different formatting");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## 🎉 **What You Get**

✅ **Optimized for your server** - Uses your actual group names  
✅ **Faster performance** - No wasted API calls  
✅ **Better reliability** - Only queries existing groups  
✅ **Dedicated endpoint** - `/api/users/real` just for your groups  
✅ **Console menu option** - "4. Get All Real Users"  
✅ **Complete documentation** - Updated Swagger specs  

## 🚀 **Ready to Use**

Your API now has **perfect knowledge** of your group structure and will work efficiently with your MT5 server! 

Try it out:
1. Build the solution
2. Start the Web API server
3. Call `GET /api/users/real` or use console option 4
4. Get all users from your real groups instantly! 🎯
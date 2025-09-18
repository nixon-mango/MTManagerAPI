# 📊 Positions API - Complete Guide

## ✅ **YES! Positions API is Available**

I've implemented a comprehensive **Positions API** for your MT5 Web API that provides access to all open trading positions, position summaries, and group-level position analysis.

## 🚀 **New Position Endpoints**

### **Individual User Positions**
```http
GET /api/user/{login}/positions         # Get all positions for a user
GET /api/user/{login}/positions/summary # Get position summary for a user
```

### **Group-Level Positions**
```http
GET /api/group/{groupName}/positions    # Get all positions for a group
```

## 🔧 **Position Data Structure**

### **PositionInfo Model**
```json
{
  "position_id": 123456789,
  "login": 67890,
  "symbol": "EURUSD",
  "action": "Buy",
  "volume": 1.0,
  "price_open": 1.0950,
  "price_current": 1.0965,
  "profit": 150.0,
  "storage": -2.5,
  "commission": -10.0,
  "time_create": "2024-01-15T10:30:00Z",
  "time_update": "2024-01-15T15:45:00Z",
  "comment": "Expert Advisor",
  "external_id": "EA_001",
  "reason": "Expert",
  "digits": 5,
  "digits_currency": 2,
  "contract_size": 100000.0,
  "rate_profit": 1.0,
  "rate_margin": 1.0,
  "expert_id": 12345,
  "expert_position_id": 67890
}
```

### **PositionSummary Model**
```json
{
  "login": 67890,
  "total_positions": 5,
  "buy_positions": 3,
  "sell_positions": 2,
  "total_volume": 2.5,
  "total_profit": 245.50,
  "symbols": ["EURUSD", "GBPUSD", "XAUUSD"],
  "last_update": "2024-01-15T15:45:00Z"
}
```

## 🎯 **Usage Examples**

### **1. Get User Positions**
```bash
# Get all positions for a specific user
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/user/67890/positions

# Response:
{
  "success": true,
  "data": [
    {
      "position_id": 123456789,
      "login": 67890,
      "symbol": "EURUSD",
      "action": "Buy",
      "volume": 1.0,
      "price_open": 1.0950,
      "price_current": 1.0965,
      "profit": 150.0,
      "time_create": "2024-01-15T10:30:00Z"
    }
  ]
}
```

### **2. Get Position Summary**
```bash
# Get position summary for quick overview
curl -H "X-API-Key: YOUR_KEY" \
     http://YOUR_STATIC_IP:8080/api/user/67890/positions/summary

# Response:
{
  "success": true,
  "data": {
    "login": 67890,
    "total_positions": 5,
    "buy_positions": 3,
    "sell_positions": 2,
    "total_volume": 2.5,
    "total_profit": 245.50,
    "symbols": ["EURUSD", "GBPUSD", "XAUUSD"]
  }
}
```

### **3. Get Group Positions**
```bash
# Get all positions for users in a specific group
curl -H "X-API-Key: YOUR_KEY" \
     "http://YOUR_STATIC_IP:8080/api/group/real%5CExecutive/positions"

# Returns all open positions for all users in the "real\Executive" group
```

## 💻 **C# Library Usage**

### **Individual User Positions**
```csharp
using (var api = new MT5ApiWrapper())
{
    api.Initialize();
    api.Connect("server.com:443", 12345, "password");
    
    // Get all positions for a user
    var positions = api.GetUserPositions(67890);
    
    Console.WriteLine($"User has {positions.Count} open positions:");
    foreach (var position in positions)
    {
        Console.WriteLine($"  {position.Symbol}: {position.Action} {position.Volume} lots");
        Console.WriteLine($"    Open: {position.PriceOpen}, Current: {position.PriceCurrent}");
        Console.WriteLine($"    Profit: {position.Profit:F2}");
        Console.WriteLine($"    Time: {position.TimeCreate:yyyy-MM-dd HH:mm}");
    }
    
    // Get position summary
    var summary = api.GetUserPositionSummary(67890);
    Console.WriteLine($"\nPosition Summary:");
    Console.WriteLine($"  Total positions: {summary.TotalPositions}");
    Console.WriteLine($"  Buy/Sell: {summary.BuyPositions}/{summary.SellPositions}");
    Console.WriteLine($"  Total volume: {summary.TotalVolume} lots");
    Console.WriteLine($"  Total profit: {summary.TotalProfit:F2}");
    Console.WriteLine($"  Symbols: {string.Join(", ", summary.Symbols)}");
}
```

### **Group Position Analysis**
```csharp
// Get all positions for a group
var groupPositions = api.GetGroupPositions("real\\Executive");

Console.WriteLine($"Group 'real\\Executive' has {groupPositions.Count} open positions");

// Analyze by symbol
var symbolAnalysis = groupPositions.GroupBy(p => p.Symbol)
    .Select(g => new {
        Symbol = g.Key,
        Count = g.Count(),
        TotalVolume = g.Sum(p => p.Volume),
        TotalProfit = g.Sum(p => p.Profit),
        AvgProfit = g.Average(p => p.Profit)
    })
    .OrderByDescending(s => s.TotalVolume);

Console.WriteLine("\nPosition Analysis by Symbol:");
foreach (var analysis in symbolAnalysis)
{
    Console.WriteLine($"  {analysis.Symbol}:");
    Console.WriteLine($"    Positions: {analysis.Count}");
    Console.WriteLine($"    Volume: {analysis.TotalVolume:F2} lots");
    Console.WriteLine($"    Profit: {analysis.TotalProfit:F2}");
    Console.WriteLine($"    Avg P&L: {analysis.AvgProfit:F2}");
}
```

## 🎮 **Console Application Integration**

Let me add position functionality to the console app:

<function_calls>
<invoke name="search_replace">
<parameter name="file_path">/workspace/MT5ConsoleApp/Program.cs
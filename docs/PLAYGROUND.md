# MT Manager API - Interactive Playground

Welcome to the MT Manager API Interactive Playground! This document provides hands-on examples and interactive scenarios to help you learn and test the API functionality.

## 🎮 Getting Started with the Playground

### Prerequisites
- ✅ MT5 Manager API DLLs installed
- ✅ Solution built successfully
- ✅ Manager account credentials available
- ✅ MT5 server accessible

### Quick Test Scenarios

Choose from these interactive scenarios to explore the API:

1. **[Basic Connection Test](#scenario-1-basic-connection-test)** - Verify your setup
2. **[User Information Explorer](#scenario-2-user-information-explorer)** - Browse user data
3. **[Account Balance Manager](#scenario-3-account-balance-manager)** - Manage account balances
4. **[Group Analytics Dashboard](#scenario-4-group-analytics-dashboard)** - Analyze user groups
5. **[Risk Management Monitor](#scenario-5-risk-management-monitor)** - Monitor account risks
6. **[Batch Operations Demo](#scenario-6-batch-operations-demo)** - Process multiple accounts

---

## Scenario 1: Basic Connection Test

**Goal**: Verify your API setup and establish a connection to the MT5 server.

### Interactive Code

```csharp
using System;
using MT5ManagerAPI;

class ConnectionTest
{
    static void Main()
    {
        Console.WriteLine("=== MT5 Manager API Connection Test ===\n");
        
        // Step 1: Get connection details from user
        Console.Write("Enter MT5 Server (e.g., server.com:443): ");
        string server = Console.ReadLine();
        
        Console.Write("Enter Manager Login: ");
        ulong login = ulong.Parse(Console.ReadLine());
        
        Console.Write("Enter Manager Password: ");
        string password = ReadPassword();
        
        // Step 2: Test connection
        using (var api = new MT5ApiWrapper())
        {
            Console.WriteLine("\n🔄 Initializing API...");
            if (!api.Initialize())
            {
                Console.WriteLine("❌ Failed to initialize API");
                Console.WriteLine("💡 Check that all DLL files are in the correct location");
                return;
            }
            Console.WriteLine("✅ API initialized successfully");
            
            Console.WriteLine("\n🔄 Connecting to MT5 server...");
            if (!api.Connect(server, login, password))
            {
                Console.WriteLine("❌ Failed to connect to server");
                Console.WriteLine("💡 Check your server address and credentials");
                return;
            }
            Console.WriteLine("✅ Connected successfully!");
            
            Console.WriteLine("\n🎉 Connection test completed successfully!");
            Console.WriteLine("You're ready to use the MT5 Manager API!");
        }
    }
    
    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password;
    }
}
```

### Expected Output
```
=== MT5 Manager API Connection Test ===

Enter MT5 Server (e.g., server.com:443): demo.mt5server.com:443
Enter Manager Login: 12345
Enter Manager Password: ********

🔄 Initializing API...
✅ API initialized successfully

🔄 Connecting to MT5 server...
✅ Connected successfully!

🎉 Connection test completed successfully!
You're ready to use the MT5 Manager API!
```

---

## Scenario 2: User Information Explorer

**Goal**: Explore user data and understand the user information structure.

### Interactive Code

```csharp
using System;
using System.Linq;
using MT5ManagerAPI;

class UserExplorer
{
    static void Main()
    {
        Console.WriteLine("=== User Information Explorer ===\n");
        
        using (var api = new MT5ApiWrapper())
        {
            // Initialize and connect (use your credentials)
            if (!ConnectToServer(api)) return;
            
            while (true)
            {
                Console.WriteLine("\n📋 User Explorer Menu:");
                Console.WriteLine("1. Get single user info");
                Console.WriteLine("2. Explore user group");
                Console.WriteLine("3. Find users by country");
                Console.WriteLine("4. User activity analysis");
                Console.WriteLine("5. Exit");
                Console.Write("\nSelect option (1-5): ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        ExploreUser(api);
                        break;
                    case "2":
                        ExploreGroup(api);
                        break;
                    case "3":
                        FindUsersByCountry(api);
                        break;
                    case "4":
                        AnalyzeUserActivity(api);
                        break;
                    case "5":
                        Console.WriteLine("Goodbye! 👋");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
    
    static void ExploreUser(MT5ApiWrapper api)
    {
        Console.Write("\nEnter user login ID: ");
        if (ulong.TryParse(Console.ReadLine(), out ulong login))
        {
            try
            {
                var user = api.GetUser(login);
                
                Console.WriteLine($"\n👤 User Information:");
                Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine($"📋 Login: {user.Login}");
                Console.WriteLine($"👤 Name: {user.Name}");
                Console.WriteLine($"👥 Group: {user.Group}");
                Console.WriteLine($"📧 Email: {user.Email}");
                Console.WriteLine($"🌍 Country: {user.Country}");
                Console.WriteLine($"🏙️ City: {user.City}");
                Console.WriteLine($"📍 Address: {user.Address}");
                Console.WriteLine($"📞 Phone: {user.Phone}");
                Console.WriteLine($"📊 Leverage: 1:{user.Leverage}");
                Console.WriteLine($"📅 Registered: {user.RegistrationTime:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"🕐 Last Access: {user.LastAccessTime:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"💬 Comment: {user.Comment}");
                
                // Calculate account age
                var accountAge = DateTime.Now - user.RegistrationTime;
                Console.WriteLine($"⏰ Account Age: {accountAge.Days} days");
                
                // Check activity
                var lastActivity = DateTime.Now - user.LastAccessTime;
                if (lastActivity.Days == 0)
                    Console.WriteLine("🟢 Status: Active today");
                else if (lastActivity.Days <= 7)
                    Console.WriteLine($"🟡 Status: Last active {lastActivity.Days} days ago");
                else if (lastActivity.Days <= 30)
                    Console.WriteLine($"🟠 Status: Last active {lastActivity.Days} days ago");
                else
                    Console.WriteLine($"🔴 Status: Inactive ({lastActivity.Days} days)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("❌ Invalid login ID format");
        }
    }
    
    static void ExploreGroup(MT5ApiWrapper api)
    {
        Console.Write("\nEnter group name (e.g., 'demo', 'real'): ");
        string groupName = Console.ReadLine();
        
        try
        {
            var users = api.GetUsersInGroup(groupName);
            
            Console.WriteLine($"\n👥 Group '{groupName}' Analysis:");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📊 Total Users: {users.Count}");
            
            if (users.Count > 0)
            {
                var countries = users.GroupBy(u => u.Country)
                                   .OrderByDescending(g => g.Count())
                                   .Take(5);
                
                Console.WriteLine($"\n🌍 Top Countries:");
                foreach (var country in countries)
                {
                    Console.WriteLine($"   {country.Key}: {country.Count()} users");
                }
                
                var activeUsers = users.Count(u => (DateTime.Now - u.LastAccessTime).Days <= 30);
                Console.WriteLine($"\n📈 Active Users (30 days): {activeUsers} ({activeUsers * 100.0 / users.Count:F1}%)");
                
                var avgLeverage = users.Average(u => u.Leverage);
                Console.WriteLine($"📊 Average Leverage: 1:{avgLeverage:F0}");
                
                Console.WriteLine($"\n📋 Sample Users:");
                foreach (var user in users.Take(5))
                {
                    var lastActivity = DateTime.Now - user.LastAccessTime;
                    Console.WriteLine($"   {user.Login}: {user.Name} ({lastActivity.Days}d ago)");
                }
                
                if (users.Count > 5)
                    Console.WriteLine($"   ... and {users.Count - 5} more users");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static void FindUsersByCountry(MT5ApiWrapper api)
    {
        Console.Write("\nEnter country name: ");
        string country = Console.ReadLine();
        
        Console.Write("Enter group to search in (or press Enter for 'demo'): ");
        string group = Console.ReadLine();
        if (string.IsNullOrEmpty(group)) group = "demo";
        
        try
        {
            var users = api.GetUsersInGroup(group);
            var countryUsers = users.Where(u => 
                u.Country.Contains(country, StringComparison.OrdinalIgnoreCase)).ToList();
            
            Console.WriteLine($"\n🌍 Users from '{country}' in group '{group}':");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📊 Found: {countryUsers.Count} users");
            
            foreach (var user in countryUsers.Take(10))
            {
                Console.WriteLine($"   {user.Login}: {user.Name} - {user.City}, {user.Country}");
            }
            
            if (countryUsers.Count > 10)
                Console.WriteLine($"   ... and {countryUsers.Count - 10} more users");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static void AnalyzeUserActivity(MT5ApiWrapper api)
    {
        Console.Write("\nEnter group name to analyze: ");
        string group = Console.ReadLine();
        
        try
        {
            var users = api.GetUsersInGroup(group);
            
            Console.WriteLine($"\n📈 Activity Analysis for '{group}':");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            var now = DateTime.Now;
            var activeToday = users.Count(u => (now - u.LastAccessTime).Days == 0);
            var activeWeek = users.Count(u => (now - u.LastAccessTime).Days <= 7);
            var activeMonth = users.Count(u => (now - u.LastAccessTime).Days <= 30);
            var inactive = users.Count(u => (now - u.LastAccessTime).Days > 30);
            
            Console.WriteLine($"🟢 Active today: {activeToday}");
            Console.WriteLine($"🟡 Active this week: {activeWeek}");
            Console.WriteLine($"🟠 Active this month: {activeMonth}");
            Console.WriteLine($"🔴 Inactive (>30 days): {inactive}");
            
            Console.WriteLine($"\n📊 Activity Percentages:");
            Console.WriteLine($"   Today: {activeToday * 100.0 / users.Count:F1}%");
            Console.WriteLine($"   Week: {activeWeek * 100.0 / users.Count:F1}%");
            Console.WriteLine($"   Month: {activeMonth * 100.0 / users.Count:F1}%");
            Console.WriteLine($"   Inactive: {inactive * 100.0 / users.Count:F1}%");
            
            // Registration trends
            var registrationsByMonth = users
                .GroupBy(u => new { u.RegistrationTime.Year, u.RegistrationTime.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Take(6);
            
            Console.WriteLine($"\n📅 Registration Trends (Last 6 months):");
            foreach (var month in registrationsByMonth)
            {
                Console.WriteLine($"   {month.Key.Year}-{month.Key.Month:D2}: {month.Count()} new users");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static bool ConnectToServer(MT5ApiWrapper api)
    {
        // Add your connection logic here
        // This is a placeholder - replace with actual connection code
        Console.WriteLine("🔄 Connecting to server...");
        if (!api.Initialize())
        {
            Console.WriteLine("❌ Failed to initialize API");
            return false;
        }
        
        // Replace with your actual server credentials
        // if (!api.Connect("your-server", 12345, "password"))
        // {
        //     Console.WriteLine("❌ Failed to connect");
        //     return false;
        // }
        
        Console.WriteLine("✅ Connected successfully!");
        return true;
    }
}
```

---

## Scenario 3: Account Balance Manager

**Goal**: Manage account balances and perform financial operations.

### Interactive Code

```csharp
using System;
using System.Collections.Generic;
using MT5ManagerAPI;

class BalanceManager
{
    static void Main()
    {
        Console.WriteLine("=== Account Balance Manager ===\n");
        
        using (var api = new MT5ApiWrapper())
        {
            if (!ConnectToServer(api)) return;
            
            while (true)
            {
                Console.WriteLine("\n💰 Balance Manager Menu:");
                Console.WriteLine("1. View account balance");
                Console.WriteLine("2. Deposit funds");
                Console.WriteLine("3. Withdraw funds");
                Console.WriteLine("4. Balance history simulation");
                Console.WriteLine("5. Bulk balance operations");
                Console.WriteLine("6. Exit");
                Console.Write("\nSelect option (1-6): ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        ViewAccountBalance(api);
                        break;
                    case "2":
                        DepositFunds(api);
                        break;
                    case "3":
                        WithdrawFunds(api);
                        break;
                    case "4":
                        SimulateBalanceHistory(api);
                        break;
                    case "5":
                        BulkBalanceOperations(api);
                        break;
                    case "6":
                        Console.WriteLine("Goodbye! 👋");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
    
    static void ViewAccountBalance(MT5ApiWrapper api)
    {
        Console.Write("\nEnter user login ID: ");
        if (ulong.TryParse(Console.ReadLine(), out ulong login))
        {
            try
            {
                var account = api.GetAccount(login);
                var user = api.GetUser(login);
                
                Console.WriteLine($"\n💰 Account Balance for {user.Name} ({login}):");
                Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine($"💵 Balance: {account.Balance:F2} {account.Currency}");
                Console.WriteLine($"🏦 Credit: {account.Credit:F2} {account.Currency}");
                Console.WriteLine($"💎 Equity: {account.Equity:F2} {account.Currency}");
                Console.WriteLine($"📊 Profit/Loss: {account.Profit:F2} {account.Currency}");
                Console.WriteLine($"🔒 Margin Used: {account.Margin:F2} {account.Currency}");
                Console.WriteLine($"🆓 Free Margin: {account.MarginFree:F2} {account.Currency}");
                Console.WriteLine($"📈 Margin Level: {account.MarginLevel:F2}%");
                
                // Risk assessment
                if (account.MarginLevel < 50)
                    Console.WriteLine("🚨 CRITICAL: Stop-out risk!");
                else if (account.MarginLevel < 100)
                    Console.WriteLine("⚠️ WARNING: Margin call level!");
                else if (account.MarginLevel < 200)
                    Console.WriteLine("🟡 CAUTION: Low margin level");
                else
                    Console.WriteLine("✅ HEALTHY: Good margin level");
                
                Console.WriteLine($"🕐 Last Update: {account.LastUpdate:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("❌ Invalid login ID format");
        }
    }
    
    static void DepositFunds(MT5ApiWrapper api)
    {
        Console.Write("\nEnter user login ID: ");
        if (!ulong.TryParse(Console.ReadLine(), out ulong login)) return;
        
        Console.Write("Enter deposit amount: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("❌ Invalid amount");
            return;
        }
        
        Console.Write("Enter comment (optional): ");
        string comment = Console.ReadLine();
        if (string.IsNullOrEmpty(comment))
            comment = $"Deposit via API - {DateTime.Now:yyyy-MM-dd HH:mm}";
        
        try
        {
            // Show before balance
            var beforeAccount = api.GetAccount(login);
            var user = api.GetUser(login);
            
            Console.WriteLine($"\n💰 Deposit Preview for {user.Name}:");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"Current Balance: {beforeAccount.Balance:F2} {beforeAccount.Currency}");
            Console.WriteLine($"Deposit Amount: +{amount:F2} {beforeAccount.Currency}");
            Console.WriteLine($"New Balance: {beforeAccount.Balance + amount:F2} {beforeAccount.Currency}");
            Console.WriteLine($"Comment: {comment}");
            
            Console.Write("\nConfirm deposit? (y/N): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                bool success = api.PerformBalanceOperation(login, amount, comment);
                
                if (success)
                {
                    Console.WriteLine("✅ Deposit completed successfully!");
                    
                    // Show after balance
                    var afterAccount = api.GetAccount(login);
                    Console.WriteLine($"Updated Balance: {afterAccount.Balance:F2} {afterAccount.Currency}");
                }
                else
                {
                    Console.WriteLine("❌ Deposit failed");
                }
            }
            else
            {
                Console.WriteLine("❌ Deposit cancelled");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static void WithdrawFunds(MT5ApiWrapper api)
    {
        Console.Write("\nEnter user login ID: ");
        if (!ulong.TryParse(Console.ReadLine(), out ulong login)) return;
        
        Console.Write("Enter withdrawal amount: ");
        if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
        {
            Console.WriteLine("❌ Invalid amount");
            return;
        }
        
        try
        {
            var beforeAccount = api.GetAccount(login);
            var user = api.GetUser(login);
            
            // Check if sufficient funds
            if (amount > beforeAccount.Balance)
            {
                Console.WriteLine($"❌ Insufficient funds! Available: {beforeAccount.Balance:F2} {beforeAccount.Currency}");
                return;
            }
            
            Console.Write("Enter comment (optional): ");
            string comment = Console.ReadLine();
            if (string.IsNullOrEmpty(comment))
                comment = $"Withdrawal via API - {DateTime.Now:yyyy-MM-dd HH:mm}";
            
            Console.WriteLine($"\n💸 Withdrawal Preview for {user.Name}:");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"Current Balance: {beforeAccount.Balance:F2} {beforeAccount.Currency}");
            Console.WriteLine($"Withdrawal Amount: -{amount:F2} {beforeAccount.Currency}");
            Console.WriteLine($"New Balance: {beforeAccount.Balance - amount:F2} {beforeAccount.Currency}");
            Console.WriteLine($"Comment: {comment}");
            
            Console.Write("\nConfirm withdrawal? (y/N): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                bool success = api.PerformBalanceOperation(login, -amount, comment);
                
                if (success)
                {
                    Console.WriteLine("✅ Withdrawal completed successfully!");
                    
                    var afterAccount = api.GetAccount(login);
                    Console.WriteLine($"Updated Balance: {afterAccount.Balance:F2} {afterAccount.Currency}");
                }
                else
                {
                    Console.WriteLine("❌ Withdrawal failed");
                }
            }
            else
            {
                Console.WriteLine("❌ Withdrawal cancelled");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static void SimulateBalanceHistory(MT5ApiWrapper api)
    {
        Console.Write("\nEnter user login ID: ");
        if (!ulong.TryParse(Console.ReadLine(), out ulong login)) return;
        
        try
        {
            var account = api.GetAccount(login);
            var user = api.GetUser(login);
            
            Console.WriteLine($"\n📊 Balance History Simulation for {user.Name}:");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            var random = new Random();
            var currentBalance = account.Balance;
            var history = new List<(DateTime Date, double Balance, string Operation)>();
            
            // Simulate 30 days of history
            for (int i = 30; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i);
                
                // Randomly generate operations
                if (random.Next(0, 4) == 0) // 25% chance of operation
                {
                    var operationType = random.Next(0, 3);
                    double amount = 0;
                    string operation = "";
                    
                    switch (operationType)
                    {
                        case 0: // Deposit
                            amount = random.Next(100, 1000);
                            currentBalance += amount;
                            operation = $"Deposit +{amount:F2}";
                            break;
                        case 1: // Withdrawal
                            amount = Math.Min(random.Next(50, 500), currentBalance * 0.1);
                            currentBalance -= amount;
                            operation = $"Withdrawal -{amount:F2}";
                            break;
                        case 2: // Trading P&L
                            amount = random.Next(-200, 300);
                            currentBalance += amount;
                            operation = amount >= 0 ? $"Profit +{amount:F2}" : $"Loss {amount:F2}";
                            break;
                    }
                    
                    history.Add((date, currentBalance, operation));
                }
            }
            
            // Display history
            Console.WriteLine($"Current Balance: {account.Balance:F2} {account.Currency}\n");
            Console.WriteLine("Date       | Balance    | Operation");
            Console.WriteLine("-----------|------------|------------------");
            
            foreach (var entry in history.TakeLast(10))
            {
                Console.WriteLine($"{entry.Date:MM-dd} | {entry.Balance,8:F2} | {entry.Operation}");
            }
            
            if (history.Count > 10)
                Console.WriteLine($"... and {history.Count - 10} more entries");
            
            // Statistics
            var deposits = history.Where(h => h.Operation.StartsWith("Deposit")).Sum(h => 
                double.Parse(h.Operation.Split('+')[1]));
            var withdrawals = history.Where(h => h.Operation.StartsWith("Withdrawal")).Sum(h => 
                double.Parse(h.Operation.Split('-')[1]));
            
            Console.WriteLine($"\n📈 Summary (simulated 30 days):");
            Console.WriteLine($"   Total Deposits: +{deposits:F2} {account.Currency}");
            Console.WriteLine($"   Total Withdrawals: -{withdrawals:F2} {account.Currency}");
            Console.WriteLine($"   Net Flow: {deposits - withdrawals:F2} {account.Currency}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static void BulkBalanceOperations(MT5ApiWrapper api)
    {
        Console.WriteLine("\n🔄 Bulk Balance Operations");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        Console.Write("Enter group name: ");
        string group = Console.ReadLine();
        
        Console.Write("Enter operation amount (+ for deposit, - for withdrawal): ");
        if (!double.TryParse(Console.ReadLine(), out double amount))
        {
            Console.WriteLine("❌ Invalid amount");
            return;
        }
        
        Console.Write("Enter comment: ");
        string comment = Console.ReadLine();
        if (string.IsNullOrEmpty(comment))
            comment = $"Bulk operation - {DateTime.Now:yyyy-MM-dd HH:mm}";
        
        try
        {
            var users = api.GetUsersInGroup(group);
            
            Console.WriteLine($"\n📊 Bulk Operation Preview:");
            Console.WriteLine($"   Group: {group}");
            Console.WriteLine($"   Users affected: {users.Count}");
            Console.WriteLine($"   Amount per user: {amount:F2}");
            Console.WriteLine($"   Total amount: {amount * users.Count:F2}");
            Console.WriteLine($"   Comment: {comment}");
            
            Console.Write("\nProceed with bulk operation? (y/N): ");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("❌ Operation cancelled");
                return;
            }
            
            int successful = 0;
            int failed = 0;
            
            Console.WriteLine("\n🔄 Processing...");
            foreach (var user in users)
            {
                try
                {
                    bool success = api.PerformBalanceOperation(user.Login, amount, comment);
                    if (success)
                    {
                        successful++;
                        Console.Write("✅");
                    }
                    else
                    {
                        failed++;
                        Console.Write("❌");
                    }
                }
                catch
                {
                    failed++;
                    Console.Write("❌");
                }
                
                if ((successful + failed) % 50 == 0)
                    Console.WriteLine($" {successful + failed}/{users.Count}");
            }
            
            Console.WriteLine($"\n\n📊 Bulk Operation Results:");
            Console.WriteLine($"   ✅ Successful: {successful}");
            Console.WriteLine($"   ❌ Failed: {failed}");
            Console.WriteLine($"   📊 Success Rate: {successful * 100.0 / users.Count:F1}%");
            Console.WriteLine($"   💰 Total Processed: {successful * amount:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    static bool ConnectToServer(MT5ApiWrapper api)
    {
        // Placeholder for connection logic
        Console.WriteLine("🔄 Connecting to server...");
        return api.Initialize();
    }
}
```

---

## Scenario 4: Group Analytics Dashboard

**Goal**: Analyze user groups and generate comprehensive reports.

### Interactive Code

```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using MT5ManagerAPI;

class GroupAnalytics
{
    static void Main()
    {
        Console.WriteLine("=== Group Analytics Dashboard ===\n");
        
        using (var api = new MT5ApiWrapper())
        {
            if (!ConnectToServer(api)) return;
            
            while (true)
            {
                Console.WriteLine("\n📊 Analytics Dashboard Menu:");
                Console.WriteLine("1. Group overview report");
                Console.WriteLine("2. User distribution analysis");
                Console.WriteLine("3. Activity trends");
                Console.WriteLine("4. Geographic distribution");
                Console.WriteLine("5. Risk assessment");
                Console.WriteLine("6. Comparative analysis");
                Console.WriteLine("7. Export data to CSV");
                Console.WriteLine("8. Exit");
                Console.Write("\nSelect option (1-8): ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        GroupOverviewReport(api);
                        break;
                    case "2":
                        UserDistributionAnalysis(api);
                        break;
                    case "3":
                        ActivityTrends(api);
                        break;
                    case "4":
                        GeographicDistribution(api);
                        break;
                    case "5":
                        RiskAssessment(api);
                        break;
                    case "6":
                        ComparativeAnalysis(api);
                        break;
                    case "7":
                        ExportToCSV(api);
                        break;
                    case "8":
                        Console.WriteLine("Goodbye! 👋");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
    
    static void GroupOverviewReport(MT5ApiWrapper api)
    {
        Console.Write("\nEnter group name: ");
        string group = Console.ReadLine();
        
        try
        {
            var users = api.GetUsersInGroup(group);
            
            Console.WriteLine($"\n📊 Group Overview Report: {group}");
            Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📅 Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"👥 Total Users: {users.Count}");
            
            if (users.Count == 0)
            {
                Console.WriteLine("❌ No users found in this group");
                return;
            }
            
            // Basic statistics
            var now = DateTime.Now;
            var activeToday = users.Count(u => (now - u.LastAccessTime).Days == 0);
            var activeWeek = users.Count(u => (now - u.LastAccessTime).Days <= 7);
            var activeMonth = users.Count(u => (now - u.LastAccessTime).Days <= 30);
            var newThisMonth = users.Count(u => (now - u.RegistrationTime).Days <= 30);
            
            Console.WriteLine($"\n📈 Activity Overview:");
            Console.WriteLine($"   🟢 Active today: {activeToday} ({activeToday * 100.0 / users.Count:F1}%)");
            Console.WriteLine($"   🟡 Active this week: {activeWeek} ({activeWeek * 100.0 / users.Count:F1}%)");
            Console.WriteLine($"   🟠 Active this month: {activeMonth} ({activeMonth * 100.0 / users.Count:F1}%)");
            Console.WriteLine($"   🆕 New this month: {newThisMonth} ({newThisMonth * 100.0 / users.Count:F1}%)");
            
            // Geographic distribution
            var countries = users.GroupBy(u => u.Country)
                                .Where(g => !string.IsNullOrEmpty(g.Key))
                                .OrderByDescending(g => g.Count())
                                .Take(5);
            
            Console.WriteLine($"\n🌍 Top Countries:");
            foreach (var country in countries)
            {
                var percentage = country.Count() * 100.0 / users.Count;
                Console.WriteLine($"   {country.Key}: {country.Count()} users ({percentage:F1}%)");
            }
            
            // Leverage distribution
            var leverageGroups = users.GroupBy(u => u.Leverage)
                                    .OrderByDescending(g => g.Count())
                                    .Take(5);
            
            Console.WriteLine($"\n📊 Leverage Distribution:");
            foreach (var leverage in leverageGroups)
            {
                var percentage = leverage.Count() * 100.0 / users.Count;
                Console.WriteLine($"   1:{leverage.Key}: {leverage.Count()} users ({percentage:F1}%)");
            }
            
            // Registration trends (last 6 months)
            var registrationTrends = users
                .Where(u => (now - u.RegistrationTime).Days <= 180)
                .GroupBy(u => new { u.RegistrationTime.Year, u.RegistrationTime.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();
            
            Console.WriteLine($"\n📅 Registration Trends (Last 6 months):");
            foreach (var month in registrationTrends.TakeLast(6))
            {
                Console.WriteLine($"   {month.Key.Year}-{month.Key.Month:D2}: {month.Count()} new registrations");
            }
            
            // Account age analysis
            var avgAccountAge = users.Average(u => (now - u.RegistrationTime).Days);
            var oldestAccount = users.Min(u => u.RegistrationTime);
            var newestAccount = users.Max(u => u.RegistrationTime);
            
            Console.WriteLine($"\n⏰ Account Age Analysis:");
            Console.WriteLine($"   Average age: {avgAccountAge:F0} days");
            Console.WriteLine($"   Oldest account: {oldestAccount:yyyy-MM-dd}");
            Console.WriteLine($"   Newest account: {newestAccount:yyyy-MM-dd}");
            
            // Get sample account balances (for demonstration)
            Console.WriteLine($"\n💰 Sample Account Information:");
            foreach (var user in users.Take(3))
            {
                try
                {
                    var account = api.GetAccount(user.Login);
                    Console.WriteLine($"   {user.Login}: {account.Balance:F2} {account.Currency} " +
                                    $"(Equity: {account.Equity:F2})");
                }
                catch
                {
                    Console.WriteLine($"   {user.Login}: Balance unavailable");
                }
            }
            
            if (users.Count > 3)
                Console.WriteLine($"   ... and {users.Count - 3} more accounts");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
    
    // Additional methods would continue here...
    // For brevity, I'll show the structure for the remaining methods
    
    static void UserDistributionAnalysis(MT5ApiWrapper api)
    {
        // Implementation for user distribution charts and statistics
        Console.WriteLine("📊 User Distribution Analysis");
        Console.WriteLine("This would show detailed distribution charts and statistics");
    }
    
    static void ActivityTrends(MT5ApiWrapper api)
    {
        // Implementation for activity trend analysis
        Console.WriteLine("📈 Activity Trends Analysis");
        Console.WriteLine("This would show login patterns and activity trends over time");
    }
    
    static void GeographicDistribution(MT5ApiWrapper api)
    {
        // Implementation for geographic analysis
        Console.WriteLine("🌍 Geographic Distribution Analysis");
        Console.WriteLine("This would show detailed geographic distribution and maps");
    }
    
    static void RiskAssessment(MT5ApiWrapper api)
    {
        // Implementation for risk assessment
        Console.WriteLine("⚠️ Risk Assessment");
        Console.WriteLine("This would analyze account risks and margin levels");
    }
    
    static void ComparativeAnalysis(MT5ApiWrapper api)
    {
        // Implementation for comparing multiple groups
        Console.WriteLine("🔍 Comparative Analysis");
        Console.WriteLine("This would compare metrics between different groups");
    }
    
    static void ExportToCSV(MT5ApiWrapper api)
    {
        // Implementation for CSV export
        Console.WriteLine("📄 Export to CSV");
        Console.WriteLine("This would export group data to CSV files");
    }
    
    static bool ConnectToServer(MT5ApiWrapper api)
    {
        Console.WriteLine("🔄 Connecting to server...");
        return api.Initialize();
    }
}
```

---

## Quick Test Commands

Here are some quick commands you can run to test specific functionality:

### Test Connection
```bash
cd MT5ConsoleApp/bin/Debug
MT5ConsoleApp.exe
# Choose option 1 to test connection
```

### Test Web API
```bash
# Start the web server
cd MT5WebAPI/bin/Debug
MT5WebAPI.exe --host localhost --port 8080

# In another terminal, test the API
curl -X POST http://localhost:8080/api/connect -H "Content-Type: application/json" -d '{"server":"your-server","login":12345,"password":"your-password"}'
curl http://localhost:8080/api/user/67890
```

### Test Batch Operations
```bash
# Use PowerShell script for testing
./test-api.ps1
```

---

## 🎯 Learning Path

### Beginner
1. Start with **Scenario 1** - Basic Connection Test
2. Try **Scenario 2** - User Information Explorer
3. Practice with **Scenario 3** - Account Balance Manager

### Intermediate
4. Explore **Scenario 4** - Group Analytics Dashboard
5. Test the Web API endpoints
6. Implement error handling patterns

### Advanced
7. Create custom batch operations
8. Implement caching and performance optimization
9. Build monitoring and alerting systems
10. Integrate with external systems

---

## 🛠️ Customization Tips

### Adding New Features
```csharp
// Example: Add a custom method to the wrapper
public class ExtendedMT5ApiWrapper : MT5ApiWrapper
{
    public List<UserInfo> GetHighRiskUsers(string group)
    {
        var users = GetUsersInGroup(group);
        var highRiskUsers = new List<UserInfo>();
        
        foreach (var user in users)
        {
            try
            {
                var account = GetAccount(user.Login);
                if (account.MarginLevel < 100)
                {
                    highRiskUsers.Add(user);
                }
            }
            catch
            {
                // Handle individual user errors
            }
        }
        
        return highRiskUsers;
    }
}
```

### Creating Custom Reports
```csharp
public class ReportGenerator
{
    public void GenerateUserReport(List<UserInfo> users, string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            writer.WriteLine("Login,Name,Group,Country,Registration,LastAccess");
            foreach (var user in users)
            {
                writer.WriteLine($"{user.Login},{user.Name},{user.Group}," +
                               $"{user.Country},{user.RegistrationTime:yyyy-MM-dd}," +
                               $"{user.LastAccessTime:yyyy-MM-dd}");
            }
        }
    }
}
```

---

This playground provides hands-on examples to help you learn and master the MT Manager API. Start with the scenarios that match your experience level and gradually work your way up to more advanced features!
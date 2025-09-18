using System;
using System.Collections.Generic;
using System.Linq;
using MT5ManagerAPI;
using MT5ManagerAPI.Models;
using Newtonsoft.Json;

namespace MT5ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MT5 Manager API Console Application ===");
            Console.WriteLine();

            // Initialize the API wrapper
            using (var api = new MT5ApiWrapper())
            {
                try
                {
                    // Initialize the MT5 Manager API
                    Console.WriteLine("Initializing MT5 Manager API...");
                    if (!api.Initialize())
                    {
                        Console.WriteLine("Failed to initialize MT5 Manager API");
                        return;
                    }
                    Console.WriteLine("✓ MT5 Manager API initialized successfully");
                    Console.WriteLine();

                    // Get connection parameters
                    Console.Write("Enter MT5 Server: ");
                    string server = Console.ReadLine();

                    Console.Write("Enter Manager Login: ");
                    if (!ulong.TryParse(Console.ReadLine(), out ulong login))
                    {
                        Console.WriteLine("Invalid login format");
                        return;
                    }

                    Console.Write("Enter Manager Password: ");
                    string password = ReadPassword();
                    Console.WriteLine();

                    // Connect to MT5 server
                    Console.WriteLine("Connecting to MT5 server...");
                    if (!api.Connect(server, login, password))
                    {
                        Console.WriteLine("Failed to connect to MT5 server");
                        return;
                    }
                    Console.WriteLine("✓ Connected to MT5 server successfully");
                    Console.WriteLine();

                    // Main menu loop
                    bool running = true;
                    while (running)
                    {
                        ShowMenu();
                        var choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                GetUserInfo(api);
                                break;
                            case "2":
                                GetAccountInfo(api);
                                break;
                            case "3":
                                GetAllUsers(api);
                                break;
                            case "4":
                                GetAllRealUsers(api);
                                break;
                            case "5":
                                GetUsersInGroup(api);
                                break;
                            case "6":
                                GetUserGroup(api);
                                break;
                            case "7":
                                PerformBalanceOperation(api);
                                break;
                            case "8":
                                GetUserDeals(api);
                                break;
                            case "0":
                                running = false;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }

                        if (running)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
                finally
                {
                    Console.WriteLine("Disconnecting from MT5 server...");
                }
            }

            Console.WriteLine("Application finished. Press any key to exit...");
            Console.ReadKey();
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== MT5 Manager API Menu ===");
            Console.WriteLine("1. Get User Information");
            Console.WriteLine("2. Get Account Information");
            Console.WriteLine("3. Get All Users (Discovery)");
            Console.WriteLine("4. Get All Real Users (Your Groups)");
            Console.WriteLine("5. Get Users in Group");
            Console.WriteLine("6. Get User Group");
            Console.WriteLine("7. Perform Balance Operation");
            Console.WriteLine("8. Get User Deals");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("Choose an option: ");
        }

        static void GetUserInfo(MT5ApiWrapper api)
        {
            Console.Write("Enter user login: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong userLogin))
            {
                Console.WriteLine("Invalid login format");
                return;
            }

            try
            {
                var user = api.GetUser(userLogin);
                if (user != null)
                {
                    Console.WriteLine("User Information:");
                    Console.WriteLine(JsonConvert.SerializeObject(user, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine("User not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void GetAccountInfo(MT5ApiWrapper api)
        {
            Console.Write("Enter user login: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong userLogin))
            {
                Console.WriteLine("Invalid login format");
                return;
            }

            try
            {
                var account = api.GetAccount(userLogin);
                if (account != null)
                {
                    Console.WriteLine("Account Information:");
                    Console.WriteLine(JsonConvert.SerializeObject(account, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine("Account not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void GetAllUsers(MT5ApiWrapper api)
        {
            try
            {
                Console.WriteLine("\n=== Getting All Users (Enhanced Discovery) ===");
                Console.WriteLine("🔍 Step 1: Getting users from your real groups...");
                
                // First, get the real users to show progress
                var realUsers = api.GetAllRealUsers();
                Console.WriteLine($"✓ Found {realUsers.Count} users in your real groups");
                
                Console.WriteLine("🔍 Step 2: Expanding discovery using login ID patterns...");
                Console.WriteLine("⚠️  This may take some time as we search for additional users...");
                
                var allUsers = api.GetAllUsers();
                
                Console.WriteLine($"\n✅ Discovery Complete!");
                Console.WriteLine($"📊 Total Users Found: {allUsers.Count}");
                Console.WriteLine($"   - From real groups: {realUsers.Count}");
                Console.WriteLine($"   - Additional discovered: {allUsers.Count - realUsers.Count}");
                
                if (allUsers.Count > 0)
                {
                    Console.WriteLine("\n📊 Summary by Group:");
                    var groupSummary = allUsers.GroupBy(u => u.Group)
                                              .OrderByDescending(g => g.Count())
                                              .Take(10);
                    
                    foreach (var group in groupSummary)
                    {
                        Console.WriteLine($"   {group.Key}: {group.Count()} users");
                    }
                    
                    Console.WriteLine($"\n📋 Sample Users (showing first 15):");
                    Console.WriteLine("Login       | Name                 | Group                | Country");
                    Console.WriteLine("------------|----------------------|----------------------|------------------");
                    
                    foreach (var user in allUsers.Take(15))
                    {
                        Console.WriteLine($"{user.Login,10} | {user.Name,-20} | {user.Group,-20} | {user.Country}");
                    }
                    
                    if (allUsers.Count > 15)
                    {
                        Console.WriteLine($"... and {allUsers.Count - 15} more users");
                    }
                    
                    // Discovery analysis
                    var loginRanges = allUsers.Select(u => u.Login).OrderBy(l => l).ToList();
                    var minLogin = loginRanges.First();
                    var maxLogin = loginRanges.Last();
                    
                    Console.WriteLine($"\n🔍 Discovery Analysis:");
                    Console.WriteLine($"   Login ID range: {minLogin} - {maxLogin}");
                    Console.WriteLine($"   Groups discovered: {allUsers.Select(u => u.Group).Distinct().Count()}");
                    
                    // Activity analysis
                    var now = DateTime.Now;
                    var activeToday = allUsers.Count(u => (now - u.LastAccess).Days == 0);
                    var activeWeek = allUsers.Count(u => (now - u.LastAccess).Days <= 7);
                    var activeMonth = allUsers.Count(u => (now - u.LastAccess).Days <= 30);
                    
                    Console.WriteLine($"\n📈 Activity Summary:");
                    Console.WriteLine($"   Active today: {activeToday} ({activeToday * 100.0 / allUsers.Count:F1}%)");
                    Console.WriteLine($"   Active this week: {activeWeek} ({activeWeek * 100.0 / allUsers.Count:F1}%)");
                    Console.WriteLine($"   Active this month: {activeMonth} ({activeMonth * 100.0 / allUsers.Count:F1}%)");
                }
                else
                {
                    Console.WriteLine("❌ No users found.");
                    Console.WriteLine("💡 Try using option 4 (Get All Real Users) to see if your groups are accessible.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting all users: {ex.Message}");
                Console.WriteLine("💡 Try using option 4 (Get All Real Users) which is known to work.");
            }
        }

        static void GetAllRealUsers(MT5ApiWrapper api)
        {
            try
            {
                Console.WriteLine("\n=== Getting All Real Users ===");
                Console.WriteLine("Checking your server's real groups:");
                Console.WriteLine("  - real\\Executive");
                Console.WriteLine("  - real\\Vipin Zero 1000");
                Console.WriteLine("  - real\\NORMAL");
                Console.WriteLine();
                
                var users = api.GetAllRealUsers();
                
                Console.WriteLine($"✓ Retrieved {users.Count} real users total");
                
                if (users.Count > 0)
                {
                    Console.WriteLine("\n📊 Summary by Group:");
                    var groupSummary = users.GroupBy(u => u.Group)
                                           .OrderByDescending(g => g.Count());
                    
                    foreach (var group in groupSummary)
                    {
                        Console.WriteLine($"   {group.Key}: {group.Count()} users");
                    }
                    
                    Console.WriteLine($"\n📋 Sample Users (showing first 10):");
                    Console.WriteLine("Login       | Name                 | Group                | Country");
                    Console.WriteLine("------------|----------------------|----------------------|------------------");
                    
                    foreach (var user in users.Take(10))
                    {
                        Console.WriteLine($"{user.Login,10} | {user.Name,-20} | {user.Group,-20} | {user.Country}");
                    }
                    
                    if (users.Count > 10)
                    {
                        Console.WriteLine($"... and {users.Count - 10} more users");
                    }
                    
                    // Activity analysis
                    var now = DateTime.Now;
                    var activeToday = users.Count(u => (now - u.LastAccess).Days == 0);
                    var activeWeek = users.Count(u => (now - u.LastAccess).Days <= 7);
                    var activeMonth = users.Count(u => (now - u.LastAccess).Days <= 30);
                    
                    Console.WriteLine($"\n📈 Activity Summary:");
                    Console.WriteLine($"   Active today: {activeToday} ({activeToday * 100.0 / users.Count:F1}%)");
                    Console.WriteLine($"   Active this week: {activeWeek} ({activeWeek * 100.0 / users.Count:F1}%)");
                    Console.WriteLine($"   Active this month: {activeMonth} ({activeMonth * 100.0 / users.Count:F1}%)");
                }
                else
                {
                    Console.WriteLine("No users found in real groups.");
                    Console.WriteLine("💡 This might mean:");
                    Console.WriteLine("   - The groups don't exist on your server");
                    Console.WriteLine("   - Your manager account doesn't have access to these groups");
                    Console.WriteLine("   - The group names have different formatting");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting real users: {ex.Message}");
            }
        }

        static void GetUsersInGroup(MT5ApiWrapper api)
        {
            Console.Write("Enter group name: ");
            string groupName = Console.ReadLine();

            try
            {
                var users = api.GetUsersInGroup(groupName);
                if (users.Any())
                {
                    Console.WriteLine($"Found {users.Count} users in group '{groupName}':");
                    foreach (var user in users.Take(10)) // Show first 10 users
                    {
                        Console.WriteLine($"- {user.Login}: {user.Name} ({user.Group})");
                    }
                    if (users.Count > 10)
                    {
                        Console.WriteLine($"... and {users.Count - 10} more users");
                    }
                }
                else
                {
                    Console.WriteLine($"No users found in group '{groupName}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void GetUserGroup(MT5ApiWrapper api)
        {
            Console.Write("Enter user login: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong userLogin))
            {
                Console.WriteLine("Invalid login format");
                return;
            }

            try
            {
                var group = api.GetUserGroup(userLogin);
                if (group != null)
                {
                    Console.WriteLine($"User {userLogin} belongs to group: {group}");
                }
                else
                {
                    Console.WriteLine("Group not found for user");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void PerformBalanceOperation(MT5ApiWrapper api)
        {
            Console.Write("Enter user login: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong userLogin))
            {
                Console.WriteLine("Invalid login format");
                return;
            }

            Console.Write("Enter amount (positive for deposit, negative for withdrawal): ");
            if (!double.TryParse(Console.ReadLine(), out double amount))
            {
                Console.WriteLine("Invalid amount format");
                return;
            }

            Console.Write("Enter comment (optional): ");
            string comment = Console.ReadLine();

            try
            {
                bool success = api.BalanceOperation(userLogin, amount, comment);
                if (success)
                {
                    Console.WriteLine($"Balance operation successful: {amount:F2} for user {userLogin}");
                }
                else
                {
                    Console.WriteLine("Balance operation failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void GetUserDeals(MT5ApiWrapper api)
        {
            Console.Write("Enter user login: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong userLogin))
            {
                Console.WriteLine("Invalid login format");
                return;
            }

            Console.Write("Enter start date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime fromDate))
            {
                Console.WriteLine("Invalid date format");
                return;
            }

            Console.Write("Enter end date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime toDate))
            {
                Console.WriteLine("Invalid date format");
                return;
            }

            try
            {
                var deals = api.GetUserDeals(userLogin, fromDate, toDate);
                if (deals != null && deals.Total() > 0)
                {
                    Console.WriteLine($"Found {deals.Total()} deals for user {userLogin}:");
                    for (uint i = 0; i < Math.Min(deals.Total(), 10); i++)
                    {
                        var deal = deals.Next(i);
                        Console.WriteLine($"- Deal {deal.Deal()}: {deal.Volume()} @ {deal.Price():F5} ({deal.Action()})");
                    }
                    if (deals.Total() > 10)
                    {
                        Console.WriteLine($"... and {deals.Total() - 10} more deals");
                    }
                }
                else
                {
                    Console.WriteLine("No deals found for the specified period");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
                    password = password.Substring(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
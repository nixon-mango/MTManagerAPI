using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MT5ManagerAPI;
using MT5WebAPI.Models;
using Newtonsoft.Json;

namespace MT5WebAPI.Controllers
{
    public class MT5Controller : IDisposable
    {
        private MT5ApiWrapper _api;
        private bool _disposed = false;

        public MT5Controller()
        {
            _api = new MT5ApiWrapper();
            _api.Initialize();
        }

        public string Connect(string requestBody)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<ConnectRequest>(requestBody);
                if (request == null)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid request body"));

                if (string.IsNullOrEmpty(request.Server))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Server is required"));

                if (string.IsNullOrEmpty(request.Password))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Password is required"));

                bool success = _api.Connect(request.Server, request.Login, request.Password);
                if (success)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new 
                    { 
                        message = "Connected successfully",
                        server = request.Server,
                        login = request.Login
                    }));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Failed to connect to MT5 server"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Connection error: {ex.Message}"));
            }
        }

        public string Disconnect()
        {
            try
            {
                _api.Disconnect();
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new { message = "Disconnected successfully" }));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Disconnect error: {ex.Message}"));
            }
        }

        public string GetStatus()
        {
            try
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new 
                { 
                    connected = _api.IsConnected,
                    timestamp = DateTime.UtcNow
                }));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Status error: {ex.Message}"));
            }
        }

        public string GetUser(string loginStr)
        {
            try
            {
                if (!ulong.TryParse(loginStr, out ulong login))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid login format"));

                var user = _api.GetUser(login);
                if (user != null)
                {
                    return JsonConvert.SerializeObject(ApiResponse<MT5ManagerAPI.Models.UserInfo>.CreateSuccess(user));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("User not found"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get user error: {ex.Message}"));
            }
        }

        public string GetAccount(string loginStr)
        {
            try
            {
                if (!ulong.TryParse(loginStr, out ulong login))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid login format"));

                var account = _api.GetAccount(login);
                if (account != null)
                {
                    return JsonConvert.SerializeObject(ApiResponse<MT5ManagerAPI.Models.AccountInfo>.CreateSuccess(account));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Account not found"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get account error: {ex.Message}"));
            }
        }

        public string GetAllUsers()
        {
            try
            {
                var users = _api.GetAllUsers();
                
                // Get additional discovery statistics
                var realUsers = _api.GetAllRealUsers();
                var additionalUsers = users.Count - realUsers.Count;
                var groupsFound = users.Select(u => u.Group).Distinct().Count();
                var loginRange = users.Count > 0 ? 
                    $"{users.Min(u => u.Login)} - {users.Max(u => u.Login)}" : "N/A";
                
                var response = new
                {
                    users = users,
                    discovery_stats = new
                    {
                        total_users = users.Count,
                        from_real_groups = realUsers.Count,
                        additional_discovered = additionalUsers,
                        groups_found = groupsFound,
                        login_range = loginRange,
                        discovery_method = "Enhanced discovery using real groups + login ID patterns"
                    }
                };
                
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(response));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get all users error: {ex.Message}"));
            }
        }

        public string GetAllRealUsers()
        {
            try
            {
                var users = _api.GetAllRealUsers();
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.UserInfo>>.CreateSuccess(users));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get all real users error: {ex.Message}"));
            }
        }

        public string GetAllDemoUsers()
        {
            try
            {
                var users = _api.GetAllDemoUsers();
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.UserInfo>>.CreateSuccess(users));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get all demo users error: {ex.Message}"));
            }
        }

        public string GetAllVIPUsers()
        {
            try
            {
                var users = _api.GetAllVIPUsers();
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.UserInfo>>.CreateSuccess(users));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get all VIP users error: {ex.Message}"));
            }
        }

        public string GetAllManagerUsers()
        {
            try
            {
                var users = _api.GetAllManagerUsers();
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.UserInfo>>.CreateSuccess(users));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get all manager users error: {ex.Message}"));
            }
        }

        public string TestBalanceOperation(string requestBody)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<BalanceRequest>(requestBody);
                if (request == null)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid request body"));

                var testResults = new Dictionary<string, object>();
                
                // Request validation
                testResults["request_validation"] = new Dictionary<string, object>
                {
                    ["login_valid"] = request.Login > 0,
                    ["amount_valid"] = request.Amount != 0,
                    ["type_valid"] = request.Type >= 0,
                    ["comment_provided"] = !string.IsNullOrEmpty(request.Comment)
                };

                testResults["api_connection"] = _api.IsConnected;
                testResults["test_timestamp"] = DateTime.UtcNow;

                // Check if user exists
                try
                {
                    var user = _api.GetUser(request.Login);
                    if (user != null)
                    {
                        testResults["user_check"] = new Dictionary<string, object>
                        {
                            ["exists"] = true,
                            ["name"] = user.Name,
                            ["group"] = user.Group,
                            ["rights"] = user.Rights,
                            ["leverage"] = user.Leverage,
                            ["registration"] = user.Registration,
                            ["last_access"] = user.LastAccess
                        };

                        // Check account info
                        try
                        {
                            var account = _api.GetAccount(request.Login);
                            if (account != null)
                            {
                                testResults["account_check"] = new Dictionary<string, object>
                                {
                                    ["exists"] = true,
                                    ["balance"] = account.Balance,
                                    ["currency"] = account.Currency,
                                    ["margin_level"] = account.MarginLevel,
                                    ["equity"] = account.Equity,
                                    ["credit"] = account.Credit
                                };
                            }
                            else
                            {
                                testResults["account_check"] = new Dictionary<string, object>
                                {
                                    ["exists"] = false,
                                    ["error"] = "Account not found"
                                };
                            }
                        }
                        catch (Exception accEx)
                        {
                            testResults["account_check"] = new Dictionary<string, object>
                            {
                                ["error"] = accEx.Message
                            };
                        }
                    }
                    else
                    {
                        testResults["user_check"] = new Dictionary<string, object>
                        {
                            ["exists"] = false,
                            ["error"] = "User not found"
                        };
                    }
                }
                catch (Exception userEx)
                {
                    testResults["user_check"] = new Dictionary<string, object>
                    {
                        ["error"] = userEx.Message
                    };
                }

                // Add recommendations based on findings
                var recommendations = new List<string>();
                
                if (!(bool)((Dictionary<string, object>)testResults["request_validation"])["login_valid"])
                    recommendations.Add("Login ID must be greater than 0");
                
                if (!(bool)((Dictionary<string, object>)testResults["request_validation"])["amount_valid"])
                    recommendations.Add("Amount cannot be zero");

                if (!_api.IsConnected)
                    recommendations.Add("API is not connected to MT5 server");

                if (testResults.ContainsKey("user_check"))
                {
                    var userCheck = (Dictionary<string, object>)testResults["user_check"];
                    if (userCheck.ContainsKey("exists") && !(bool)userCheck["exists"])
                        recommendations.Add("User does not exist - check login ID");
                    
                    if (userCheck.ContainsKey("rights") && (uint)userCheck["rights"] == 0)
                        recommendations.Add("User has no trading rights - contact broker to enable");
                }

                testResults["recommendations"] = recommendations;

                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(testResults));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Test balance operation error: {ex.Message}"));
            }
        }

        public string GetUserDiscoveryStats()
        {
            try
            {
                var allUsers = _api.GetAllUsers();
                var realUsers = _api.GetAllRealUsers();
                
                var stats = new
                {
                    total_users = allUsers.Count,
                    from_real_groups = realUsers.Count,
                    additional_discovered = allUsers.Count - realUsers.Count,
                    groups_found = allUsers.Select(u => u.Group).Distinct().ToList(),
                    groups_count = allUsers.Select(u => u.Group).Distinct().Count(),
                    login_range = allUsers.Count > 0 ? new
                    {
                        min = allUsers.Min(u => u.Login),
                        max = allUsers.Max(u => u.Login),
                        range_text = $"{allUsers.Min(u => u.Login)} - {allUsers.Max(u => u.Login)}"
                    } : null,
                    discovery_method = "Enhanced discovery using real groups + login ID patterns",
                    group_breakdown = allUsers.GroupBy(u => u.Group)
                        .Select(g => new { group = g.Key, count = g.Count() })
                        .OrderByDescending(g => g.count)
                        .ToList(),
                    activity_stats = new
                    {
                        active_today = allUsers.Where(u => (DateTime.Now - u.LastAccess).Days == 0).Count(),
                        active_week = allUsers.Where(u => (DateTime.Now - u.LastAccess).Days <= 7).Count(),
                        active_month = allUsers.Where(u => (DateTime.Now - u.LastAccess).Days <= 30).Count()
                    }
                };
                
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(stats));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get discovery stats error: {ex.Message}"));
            }
        }

        public string GetUsersInGroup(string groupName)
        {
            try
            {
                if (string.IsNullOrEmpty(groupName))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Group name is required"));

                var users = _api.GetUsersInGroup(groupName);
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.UserInfo>>.CreateSuccess(users));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get users in group error: {ex.Message}"));
            }
        }

        public string GetUserGroup(string loginStr)
        {
            try
            {
                if (!ulong.TryParse(loginStr, out ulong login))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid login format"));

                var group = _api.GetUserGroup(login);
                if (group != null)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new { group = group }));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Group not found"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get user group error: {ex.Message}"));
            }
        }

        public string PerformBalanceOperation(string requestBody)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<BalanceRequest>(requestBody);
                if (request == null)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid request body"));

                // Validate the request
                if (request.Login == 0)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Login ID is required"));
                
                if (request.Amount == 0)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Amount cannot be zero"));

                // Check if user exists before attempting balance operation
                try
                {
                    var user = _api.GetUser(request.Login);
                    if (user == null)
                    {
                        return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"User with login {request.Login} not found"));
                    }
                    Console.WriteLine($"  User found: {user.Name} ({user.Group})");
                }
                catch (Exception userEx)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Failed to verify user: {userEx.Message}"));
                }

                // Log the operation attempt
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Balance operation attempt:");
                Console.WriteLine($"  Login: {request.Login}");
                Console.WriteLine($"  Amount: {request.Amount}");
                Console.WriteLine($"  Type: {request.Type}");
                Console.WriteLine($"  Comment: {request.Comment}");

                bool success = _api.BalanceOperation(request.Login, request.Amount, request.Comment ?? "", request.Type);
                
                if (success)
                {
                    Console.WriteLine($"  Result: SUCCESS");
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new 
                    { 
                        message = "Balance operation successful",
                        login = request.Login,
                        amount = request.Amount,
                        comment = request.Comment,
                        type = request.Type,
                        timestamp = DateTime.UtcNow
                    }));
                }
                else
                {
                    Console.WriteLine($"  Result: FAILED");
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Balance operation failed - check MT5 server logs for details"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Result: EXCEPTION - {ex.Message}");
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Balance operation error: {ex.Message}"));
            }
        }

        public string GetUserDeals(string loginStr, Dictionary<string, string> queryParams)
        {
            try
            {
                if (!ulong.TryParse(loginStr, out ulong login))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid login format"));

                DateTime fromDate = DateTime.Today.AddDays(-7); // Default: last 7 days
                DateTime toDate = DateTime.Today.AddDays(1);    // Default: until tomorrow

                if (queryParams.ContainsKey("from"))
                    DateTime.TryParse(queryParams["from"], out fromDate);

                if (queryParams.ContainsKey("to"))
                    DateTime.TryParse(queryParams["to"], out toDate);

                var deals = _api.GetUserDeals(login, fromDate, toDate);
                if (deals != null)
                {
                    var dealsList = new List<object>();
                    for (uint i = 0; i < Math.Min(deals.Total(), 100); i++) // Limit to 100 deals
                    {
                        var deal = deals.Next(i);
                        dealsList.Add(new
                        {
                            deal_id = deal.Deal(),
                            login = deal.Login(),
                            symbol = deal.Symbol(),
                            action = deal.Action().ToString(),
                            volume = deal.Volume(),
                            price = deal.Price(),
                            profit = deal.Profit(),
                            commission = deal.Commission(),
                            swap = deal.Storage(),
                            time = deal.Time(),
                            comment = deal.Comment()
                        });
                    }

                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new 
                    { 
                        total = deals.Total(),
                        returned = dealsList.Count,
                        deals = dealsList
                    }));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Failed to retrieve deals"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get user deals error: {ex.Message}"));
            }
        }

        public string GetUserPositions(string loginStr)
        {
            try
            {
                if (!ulong.TryParse(loginStr, out ulong login))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid login format"));

                var positions = _api.GetUserPositions(login);
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.PositionInfo>>.CreateSuccess(positions));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get user positions error: {ex.Message}"));
            }
        }

        public string GetUserPositionSummary(string loginStr)
        {
            try
            {
                if (!ulong.TryParse(loginStr, out ulong login))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid login format"));

                var summary = _api.GetUserPositionSummary(login);
                return JsonConvert.SerializeObject(ApiResponse<MT5ManagerAPI.Models.PositionSummary>.CreateSuccess(summary));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get user position summary error: {ex.Message}"));
            }
        }

        public string GetGroupPositions(string groupName)
        {
            try
            {
                if (string.IsNullOrEmpty(groupName))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Group name is required"));

                var positions = _api.GetGroupPositions(groupName);
                return JsonConvert.SerializeObject(ApiResponse<List<MT5ManagerAPI.Models.PositionInfo>>.CreateSuccess(positions));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get group positions error: {ex.Message}"));
            }
        }

        public string GetAllGroups()
        {
            try
            {
                var groups = _api.GetAllGroups();
                
                // Create a response with summary statistics
                var response = new
                {
                    groups = groups.Select(g => g.ToSimpleObject()).ToList(),
                    summary = new
                    {
                        total_groups = groups.Count,
                        real_groups = groups.Count(g => !g.IsDemo && !g.Name.ToLower().Contains("manager")),
                        demo_groups = groups.Count(g => g.IsDemo),
                        manager_groups = groups.Count(g => g.Name.ToLower().Contains("manager")),
                        total_users = groups.Sum(g => g.UserCount),
                        groups_by_type = groups.GroupBy(g => 
                        {
                            if (g.IsDemo) return "demo";
                            else if (g.Name.ToLower().Contains("manager")) return "manager";
                            else return "real";
                        }).Select(g => new { type = g.Key, count = g.Count() }).ToList(),
                        last_update = DateTime.UtcNow
                    }
                };

                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(response));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get all groups error: {ex.Message}"));
            }
        }

        public string GetGroup(string groupName)
        {
            try
            {
                if (string.IsNullOrEmpty(groupName))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Group name is required"));

                var groups = _api.GetAllGroups();
                var group = groups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                
                if (group == null)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Group '{groupName}' not found"));
                }

                // Get additional statistics for this specific group
                var users = _api.GetUsersInGroup(groupName);
                var response = new
                {
                    group = group.ToSimpleObject(),
                    statistics = new
                    {
                        user_count = users.Count,
                        active_users = users.Count(u => (DateTime.Now - u.LastAccess).Days <= 30),
                        avg_leverage = users.Count > 0 ? users.Where(u => u.Leverage > 0).Average(u => u.Leverage) : 0,
                        registration_range = users.Count > 0 ? new
                        {
                            earliest = users.Min(u => u.Registration),
                            latest = users.Max(u => u.Registration)
                        } : null,
                        countries = users.Where(u => !string.IsNullOrEmpty(u.Country))
                                        .GroupBy(u => u.Country)
                                        .Select(g => new { country = g.Key, count = g.Count() })
                                        .OrderByDescending(x => x.count)
                                        .Take(10)
                                        .ToList()
                    }
                };

                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(response));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Get group error: {ex.Message}"));
            }
        }

        public string CreateGroup(string requestBody)
        {
            try
            {
                var createRequest = JsonConvert.DeserializeObject<GroupCreateRequest>(requestBody);
                if (createRequest == null)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid request body"));

                if (string.IsNullOrEmpty(createRequest.Name))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Group name is required"));

                // Validate group name format
                if (!IsValidGroupName(createRequest.Name))
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError(
                        "Invalid group name format. Use format like 'real\\GroupName' or 'demo\\GroupName'"));
                }

                // Determine if this is a demo group
                bool isDemo = createRequest.IsDemo ?? createRequest.Name.ToLower().Contains("demo");

                // Create new group info with defaults
                var newGroup = new MT5ManagerAPI.Models.GroupInfo
                {
                    Name = createRequest.Name,
                    Description = createRequest.Description ?? $"New group: {createRequest.Name}",
                    Company = createRequest.Company ?? "MT5 Trading Company",
                    Currency = createRequest.Currency ?? "USD",
                    Leverage = createRequest.Leverage ?? (isDemo ? 500U : 100U),
                    DepositMin = createRequest.DepositMin ?? (isDemo ? 0 : 100),
                    DepositMax = createRequest.DepositMax ?? 1000000,
                    CreditLimit = createRequest.CreditLimit ?? 0,
                    MarginCall = createRequest.MarginCall ?? (createRequest.Name.ToLower().Contains("vip") ? 70 : 80),
                    MarginStopOut = createRequest.MarginStopOut ?? (createRequest.Name.ToLower().Contains("vip") ? 40 : 50),
                    InterestRate = createRequest.InterestRate ?? 0,
                    Commission = createRequest.Commission ?? DetermineCommissionForNewGroup(createRequest.Name),
                    CommissionType = createRequest.CommissionType ?? 0U,
                    AgentCommission = createRequest.AgentCommission ?? 0,
                    FreeMarginMode = 0U,
                    Rights = createRequest.Rights ?? DetermineRightsForNewGroup(createRequest.Name, isDemo),
                    CheckPassword = true,
                    Timeout = createRequest.Timeout ?? (createRequest.Name.ToLower().Contains("manager") ? 0U : 60U),
                    OHLCMaxCount = 65000U,
                    NewsMode = createRequest.NewsMode ?? 2U,
                    ReportsMode = createRequest.ReportsMode ?? 1U,
                    EmailFrom = createRequest.EmailFrom ?? "noreply@mt5trading.com",
                    SMTPServer = "",
                    SMTPLogin = "",
                    SMTPPassword = "",
                    SupportPage = createRequest.SupportPage ?? "https://support.mt5trading.com",
                    SupportEmail = createRequest.SupportEmail ?? "support@mt5trading.com",
                    Templates = "templates\\",
                    CopyQuotes = false,
                    Reports = true,
                    DefaultDeposit = createRequest.DefaultDeposit ?? (isDemo ? 10000 : 0),
                    DefaultCredit = createRequest.DefaultCredit ?? 0,
                    ArchivePeriod = createRequest.ArchivePeriod ?? 90U,
                    ArchiveMaxRecords = createRequest.ArchiveMaxRecords ?? 100000U,
                    MarginFreeMode = 0U,
                    IsDemo = isDemo,
                    UserCount = 0,
                    LastUpdate = DateTime.UtcNow,
                    CustomProperties = new Dictionary<string, object>()
                };

                // Attempt to create the group
                bool success = _api.CreateGroup(newGroup);

                if (success)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new
                    {
                        message = "Group created successfully",
                        group = newGroup.ToSimpleObject(),
                        created_at = DateTime.UtcNow,
                        group_type = isDemo ? "demo" : (createRequest.Name.ToLower().Contains("manager") ? "manager" : "real")
                    }));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Failed to create group"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Create group error: {ex.Message}"));
            }
        }

        public string UpdateGroup(string groupName, string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(groupName))
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Group name is required"));

                var updateRequest = JsonConvert.DeserializeObject<GroupUpdateRequest>(requestBody);
                if (updateRequest == null)
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Invalid request body"));

                // Get the existing group first
                var groups = _api.GetAllGroups();
                var existingGroup = groups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                
                if (existingGroup == null)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Group '{groupName}' not found"));
                }

                // Create updated group info
                var updatedGroup = new MT5ManagerAPI.Models.GroupInfo
                {
                    Name = groupName,
                    Description = updateRequest.Description ?? existingGroup.Description,
                    Company = updateRequest.Company ?? existingGroup.Company,
                    Currency = updateRequest.Currency ?? existingGroup.Currency,
                    Leverage = updateRequest.Leverage ?? existingGroup.Leverage,
                    DepositMin = updateRequest.DepositMin ?? existingGroup.DepositMin,
                    DepositMax = updateRequest.DepositMax ?? existingGroup.DepositMax,
                    CreditLimit = updateRequest.CreditLimit ?? existingGroup.CreditLimit,
                    MarginCall = updateRequest.MarginCall ?? existingGroup.MarginCall,
                    MarginStopOut = updateRequest.MarginStopOut ?? existingGroup.MarginStopOut,
                    InterestRate = updateRequest.InterestRate ?? existingGroup.InterestRate,
                    Commission = updateRequest.Commission ?? existingGroup.Commission,
                    CommissionType = updateRequest.CommissionType ?? existingGroup.CommissionType,
                    AgentCommission = updateRequest.AgentCommission ?? existingGroup.AgentCommission,
                    Rights = updateRequest.Rights ?? existingGroup.Rights,
                    Timeout = updateRequest.Timeout ?? existingGroup.Timeout,
                    NewsMode = updateRequest.NewsMode ?? existingGroup.NewsMode,
                    ReportsMode = updateRequest.ReportsMode ?? existingGroup.ReportsMode,
                    EmailFrom = updateRequest.EmailFrom ?? existingGroup.EmailFrom,
                    SupportEmail = updateRequest.SupportEmail ?? existingGroup.SupportEmail,
                    SupportPage = updateRequest.SupportPage ?? existingGroup.SupportPage,
                    DefaultDeposit = updateRequest.DefaultDeposit ?? existingGroup.DefaultDeposit,
                    DefaultCredit = updateRequest.DefaultCredit ?? existingGroup.DefaultCredit,
                    LastUpdate = DateTime.UtcNow
                };

                // Attempt to update the group
                bool success = _api.UpdateGroup(groupName, updatedGroup);
                
                if (success)
                {
                    var response = new
                    {
                        message = "Group updated successfully",
                        group_name = groupName,
                        updated_properties = GetUpdatedProperties(existingGroup, updatedGroup),
                        timestamp = DateTime.UtcNow
                    };
                    
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(response));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Failed to update group"));
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Update group error: {ex.Message}"));
            }
        }

        private object GetUpdatedProperties(MT5ManagerAPI.Models.GroupInfo original, MT5ManagerAPI.Models.GroupInfo updated)
        {
            var changes = new Dictionary<string, object>();

            if (original.Description != updated.Description)
                changes["description"] = new { from = original.Description, to = updated.Description };
            
            if (original.Company != updated.Company)
                changes["company"] = new { from = original.Company, to = updated.Company };
                
            if (original.Currency != updated.Currency)
                changes["currency"] = new { from = original.Currency, to = updated.Currency };
                
            if (original.Leverage != updated.Leverage)
                changes["leverage"] = new { from = original.Leverage, to = updated.Leverage };
                
            if (original.MarginCall != updated.MarginCall)
                changes["margin_call"] = new { from = original.MarginCall, to = updated.MarginCall };
                
            if (original.MarginStopOut != updated.MarginStopOut)
                changes["margin_stop_out"] = new { from = original.MarginStopOut, to = updated.MarginStopOut };
                
            if (original.Commission != updated.Commission)
                changes["commission"] = new { from = original.Commission, to = updated.Commission };
                
            if (original.Rights != updated.Rights)
                changes["rights"] = new { from = original.Rights, to = updated.Rights };

            return changes;
        }

        private bool IsValidGroupName(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return false;

            // Check for valid group name format (should contain a backslash for category)
            // Examples: "real\MyGroup", "demo\TestGroup", "managers\admin"
            return groupName.Contains("\\") || groupName.Contains("/");
        }

        private double DetermineCommissionForNewGroup(string groupName)
        {
            string name = groupName.ToLower();
            
            if (name.Contains("zero"))
                return 0.0;
            else if (name.Contains("vip") || name.Contains("executive"))
                return 0.0;
            else if (name.Contains("demo"))
                return 0.0;
            else
                return 7.0; // $7 per lot for standard groups
        }

        private uint DetermineRightsForNewGroup(string groupName, bool isDemo)
        {
            bool isManager = groupName.ToLower().Contains("manager");
            
            if (isManager)
                return 127U; // Full rights for managers
            else if (isDemo)
                return 71U;  // Demo rights
            else
                return 67U;  // Standard real trading rights
        }

        public string GetGroupsDebugInfo()
        {
            try
            {
                var debugInfo = new
                {
                    loaded_groups_count = _api.GetLoadedGroupsCount(),
                    loaded_group_names = _api.GetLoadedGroupNames(20),
                    api_connected = _api.IsConnected,
                    timestamp = DateTime.UtcNow
                };

                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(debugInfo));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Debug info error: {ex.Message}"));
            }
        }

        public string ReloadGroups()
        {
            try
            {
                _api.ReloadGroupsFromFile();
                
                var reloadInfo = new
                {
                    message = "Groups reloaded from file",
                    loaded_groups_count = _api.GetLoadedGroupsCount(),
                    sample_groups = _api.GetLoadedGroupNames(10),
                    timestamp = DateTime.UtcNow
                };

                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(reloadInfo));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"Reload groups error: {ex.Message}"));
            }
        }

        public string GetUserDiscoveryInfo()
        {
            try
            {
                var discoveryInfo = new
                {
                    loaded_groups_count = _api.GetLoadedGroupsCount(),
                    real_groups_for_discovery = _api.GetLoadedGroupNames().Where(name => !name.ToLower().Contains("demo") && !name.ToLower().Contains("manager")).Count(),
                    demo_groups_for_discovery = _api.GetLoadedGroupNames().Where(name => name.ToLower().Contains("demo")).Count(),
                    manager_groups_for_discovery = _api.GetLoadedGroupNames().Where(name => name.ToLower().Contains("manager")).Count(),
                    sample_real_groups = _api.GetLoadedGroupNames().Where(name => !name.ToLower().Contains("demo") && !name.ToLower().Contains("manager")).Take(10).ToList(),
                    sample_demo_groups = _api.GetLoadedGroupNames().Where(name => name.ToLower().Contains("demo")).Take(5).ToList(),
                    sample_manager_groups = _api.GetLoadedGroupNames().Where(name => name.ToLower().Contains("manager")).Take(5).ToList(),
                    api_connected = _api.IsConnected,
                    timestamp = DateTime.UtcNow
                };

                return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(discoveryInfo));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResponse<object>.CreateError($"User discovery info error: {ex.Message}"));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _api?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
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

                bool success = _api.BalanceOperation(request.Login, request.Amount, request.Comment ?? "", request.Type);
                if (success)
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateSuccess(new 
                    { 
                        message = "Balance operation successful",
                        login = request.Login,
                        amount = request.Amount,
                        comment = request.Comment
                    }));
                }
                else
                {
                    return JsonConvert.SerializeObject(ApiResponse<object>.CreateError("Balance operation failed"));
                }
            }
            catch (Exception ex)
            {
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
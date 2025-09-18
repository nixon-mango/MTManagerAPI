using System;
using System.Collections.Generic;
using System.Linq;
using MetaQuotes.MT5CommonAPI;
using MT5ManagerAPI.Models;

namespace MT5ManagerAPI
{
    /// <summary>
    /// Modern wrapper for MT5 Manager API with simplified interface
    /// </summary>
    public class MT5ApiWrapper : IDisposable
    {
        private MT5Manager.CManager _manager;
        private bool _isConnected = false;
        private bool _disposed = false;

        /// <summary>
        /// Gets whether the API is currently connected to MT5 server
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Initialize the MT5 API wrapper
        /// </summary>
        public MT5ApiWrapper()
        {
            _manager = new MT5Manager.CManager();
        }

        /// <summary>
        /// Initialize the MT5 Manager API
        /// </summary>
        /// <returns>True if initialization successful</returns>
        public bool Initialize()
        {
            try
            {
                return _manager.Initialize();
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to initialize MT5 Manager API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Connect to MT5 server with manager credentials
        /// </summary>
        /// <param name="server">MT5 server address</param>
        /// <param name="login">Manager login ID</param>
        /// <param name="password">Manager password</param>
        /// <returns>True if connection successful</returns>
        public bool Connect(string server, ulong login, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(server))
                    throw new ArgumentException("Server address cannot be empty", nameof(server));
                
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Password cannot be empty", nameof(password));

                _isConnected = _manager.Login(server, login, password);
                return _isConnected;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to connect to MT5 server: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Disconnect from MT5 server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_isConnected)
                {
                    _manager.Logout();
                    _isConnected = false;
                }
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to disconnect from MT5 server: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user information by login ID
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <returns>User information</returns>
        public UserInfo GetUser(ulong login)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                var user = _manager.GetUserInfo(login);
                if (user == null)
                    return null;

                return new UserInfo
                {
                    Login = user.Login(),
                    Name = user.Name(),
                    Group = user.Group(),
                    Email = user.EMail(),
                    Country = user.Country(),
                    City = user.City(),
                    State = user.State(),
                    ZipCode = user.ZIPCode(),
                    Address = user.Address(),
                    Phone = user.Phone(),
                    Comment = user.Comment(),
                    Registration = SMTTime.ToDateTime(user.Registration()),
                    LastAccess = SMTTime.ToDateTime(user.LastAccess()),
                    Leverage = user.Leverage(),
                    Rights = (uint)user.Rights()
                };
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get user information for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get account information by login ID
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <returns>Account information</returns>
        public AccountInfo GetAccount(ulong login)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                var account = _manager.GetAccountInfo(login);
                if (account == null)
                    return null;

                return new AccountInfo
                {
                    Login = account.Login(),
                    Balance = account.Balance(),
                    Credit = account.Credit(),
                    Margin = account.Margin(),
                    MarginFree = account.MarginFree(),
                    MarginLevel = account.MarginLevel(),
                    MarginSOCall = 0.0, // Method not available in this API version
                    MarginSOSO = 0.0, // Method not available in this API version
                    Profit = account.Profit(),
                    Storage = account.Storage(),
                    Commission = 0.0, // Method not available in this API version
                    Floating = account.Floating(),
                    Equity = account.Equity(),
                    Currency = "", // Method not available in this API version
                    CurrencyDigits = 2 // Default value
                };
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get account information for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all users from specified groups
        /// </summary>
        /// <param name="groupNames">Array of group names to query. If null, uses common group names.</param>
        /// <returns>List of all users from the specified groups</returns>
        public List<UserInfo> GetAllUsers(string[] groupNames = null)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                List<UserInfo> allUsers = new List<UserInfo>();
                HashSet<ulong> seenLogins = new HashSet<ulong>();

                if (groupNames != null && groupNames.Length > 0)
                {
                    // Use specified group names
                    foreach (string groupName in groupNames)
                    {
                        if (string.IsNullOrEmpty(groupName)) continue;

                        try
                        {
                            var users = GetUsersInGroup(groupName);
                            foreach (var user in users)
                            {
                                if (!seenLogins.Contains(user.Login))
                                {
                                    allUsers.Add(user);
                                    seenLogins.Add(user.Login);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error but continue with other groups
                            System.Diagnostics.Debug.WriteLine($"Error getting users for group {groupName}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    // Use common group discovery
                    return GetAllUsersFromCommonGroups();
                }

                return allUsers;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get all users: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all users from all discoverable common groups
        /// </summary>
        /// <returns>List of all users from common groups</returns>
        private List<UserInfo> GetAllUsersFromCommonGroups()
        {
            try
            {
                // Get users from common groups
                var userArrays = _manager.GetUsersFromCommonGroups();
                if (userArrays == null || userArrays.Count == 0)
                    return new List<UserInfo>();

                var allUsers = new List<UserInfo>();
                var seenLogins = new HashSet<ulong>(); // To avoid duplicates

                // Process each group's users
                foreach (var userArray in userArrays)
                {
                    if (userArray != null)
                    {
                        for (uint i = 0; i < userArray.Total(); i++)
                        {
                            var user = userArray.Next(i);
                            if (user != null && !seenLogins.Contains(user.Login()))
                            {
                                allUsers.Add(new UserInfo
                                {
                                    Login = user.Login(),
                                    Name = user.Name(),
                                    Group = user.Group(),
                                    Email = user.EMail(),
                                    Country = user.Country(),
                                    City = user.City(),
                                    State = user.State(),
                                    ZipCode = user.ZIPCode(),
                                    Address = user.Address(),
                                    Phone = user.Phone(),
                                    Comment = user.Comment(),
                                    Registration = SMTTime.ToDateTime(user.Registration()),
                                    LastAccess = SMTTime.ToDateTime(user.LastAccess()),
                                    Leverage = user.Leverage(),
                                    Rights = (uint)user.Rights()
                                });
                                seenLogins.Add(user.Login());
                            }
                        }
                    }
                }

                return allUsers;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get users from common groups: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all users in a specific group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>List of users in the group</returns>
        public List<UserInfo> GetUsersInGroup(string groupName)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException("Group name cannot be empty", nameof(groupName));

            try
            {
                var users = _manager.GetUsers(groupName);
                if (users == null)
                    return new List<UserInfo>();

                var result = new List<UserInfo>();
                for (uint i = 0; i < users.Total(); i++)
                {
                    var user = users.Next(i);
                    if (user != null)
                    {
                        result.Add(new UserInfo
                        {
                            Login = user.Login(),
                            Name = user.Name(),
                            Group = user.Group(),
                            Email = user.EMail(),
                            Country = user.Country(),
                            City = user.City(),
                            State = user.State(),
                            ZipCode = user.ZIPCode(),
                            Address = user.Address(),
                            Phone = user.Phone(),
                            Comment = user.Comment(),
                            Registration = SMTTime.ToDateTime(user.Registration()),
                            LastAccess = SMTTime.ToDateTime(user.LastAccess()),
                            Leverage = user.Leverage(),
                            Rights = (uint)user.Rights()
                        });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get users in group {groupName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user's group name by login ID
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <returns>Group name</returns>
        public string GetUserGroup(ulong login)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                string group;
                if (_manager.GetGroup(login, out group))
                    return group;
                return null;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get group for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Perform balance operation (deposit/withdrawal)
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <param name="amount">Amount (positive for deposit, negative for withdrawal)</param>
        /// <param name="comment">Operation comment</param>
        /// <param name="type">Operation type</param>
        /// <returns>True if operation successful</returns>
        public bool BalanceOperation(ulong login, double amount, string comment = "", uint type = 2)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                return _manager.DealerBalance(login, Math.Abs(amount), type, comment ?? "", amount > 0);
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to perform balance operation for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get user deals within date range
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns>Deal array</returns>
        public CIMTDealArray GetUserDeals(ulong login, DateTime from, DateTime to)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                CIMTDealArray deals;
                if (_manager.GetUserDeal(out deals, login, from, to))
                    return deals;
                return null;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get deals for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
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
                    try
                    {
                        Disconnect();
                        _manager?.Shutdown();
                        _manager?.Dispose();
                    }
                    catch (Exception)
                    {
                        // Ignore exceptions during disposal
                    }
                }
                _disposed = true;
            }
        }

        ~MT5ApiWrapper()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// Custom exception for MT5 API errors
    /// </summary>
    public class MT5ApiException : Exception
    {
        public MT5ApiException(string message) : base(message) { }
        public MT5ApiException(string message, Exception innerException) : base(message, innerException) { }
    }
}
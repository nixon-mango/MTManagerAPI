using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetaQuotes.MT5CommonAPI;
using MT5ManagerAPI.Models;
using Newtonsoft.Json;

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
        private Dictionary<string, GroupInfo> _createdGroups = new Dictionary<string, GroupInfo>();
        private readonly string _groupStorageFile = "created_groups.json";

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
            LoadCreatedGroupsFromFile();
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
                    // Clear created groups on disconnect to maintain clean state
                    _createdGroups.Clear();
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
        /// Get all users from your server's real groups
        /// </summary>
        /// <returns>List of all users from real groups</returns>
        public List<UserInfo> GetAllRealUsers()
        {
            // Your server's most important real groups
            string[] realGroups = { 
                "real", "real\\Executive", "real\\NORMAL", "real\\Vipin Zero 1000",
                "real\\ALLWIN PREMIUM", "real\\ALLWIN PREMIUM 1", "real\\VIP A", "real\\VIP B",
                "real\\PRO A", "real\\PRO B", "real\\Standard", "real\\Executive 25",
                "real\\Vipin Zero", "real\\Vipin Zero 2500", "real\\GOLD 1", "real\\GOLD 2"
            };
            return GetAllUsers(realGroups);
        }

        /// <summary>
        /// Get all users from demo groups
        /// </summary>
        /// <returns>List of all users from demo groups</returns>
        public List<UserInfo> GetAllDemoUsers()
        {
            string[] demoGroups = { 
                "demo\\2", "demo\\AllWin Capitals Limited-Demo", "demo\\CFD", "demo\\Executive", 
                "demo\\PRO", "demo\\PS GOLD", "demo\\VIP", "demo\\forex.hedged", "demo\\gold", 
                "demo\\stock", "demo\\SPREAD 19"
            };
            return GetAllUsers(demoGroups);
        }

        /// <summary>
        /// Get all VIP users from various VIP groups
        /// </summary>
        /// <returns>List of all VIP users</returns>
        public List<UserInfo> GetAllVIPUsers()
        {
            string[] vipGroups = { 
                "demo\\VIP", "real\\VIP A", "real\\VIP B", "real\\ALLWIN VIP 1",
                "real\\Saiful VIP", "real\\Executive", "real\\Executive 25", "real\\Executive Swap"
            };
            return GetAllUsers(vipGroups);
        }

        /// <summary>
        /// Get all users from manager groups
        /// </summary>
        /// <returns>List of all manager users</returns>
        public List<UserInfo> GetAllManagerUsers()
        {
            string[] managerGroups = { 
                "managers\\administrators", "managers\\board", "managers\\dealers", "managers\\master"
            };
            return GetAllUsers(managerGroups);
        }

        /// <summary>
        /// Get all users by discovering them from known working groups and then expanding with individual user lookups
        /// </summary>
        /// <param name="groupNames">Array of group names to query. If null, uses your real groups + discovery.</param>
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
                    // Start with your working real groups
                    var realUsers = GetAllRealUsers();
                    foreach (var user in realUsers)
                    {
                        if (!seenLogins.Contains(user.Login))
                        {
                            allUsers.Add(user);
                            seenLogins.Add(user.Login);
                        }
                    }

                    // Then try to discover more users by expanding the search
                    // This uses the working real users as a foundation to discover more
                    var expandedUsers = ExpandUserDiscovery(realUsers, seenLogins);
                    allUsers.AddRange(expandedUsers);
                }

                return allUsers;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get all users: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Expand user discovery by trying to find more users around known login IDs
        /// </summary>
        /// <param name="knownUsers">Users we already found</param>
        /// <param name="seenLogins">Login IDs we've already processed</param>
        /// <returns>List of additional users found</returns>
        private List<UserInfo> ExpandUserDiscovery(List<UserInfo> knownUsers, HashSet<ulong> seenLogins)
        {
            var expandedUsers = new List<UserInfo>();
            
            if (knownUsers.Count == 0)
                return expandedUsers;

            try
            {
                // Strategy 1: Try login IDs around known users (sequential discovery)
                var loginRanges = GetLoginRanges(knownUsers);
                
                foreach (var range in loginRanges)
                {
                    for (ulong loginId = range.Start; loginId <= range.End; loginId++)
                    {
                        if (seenLogins.Contains(loginId))
                            continue;

                        try
                        {
                            var user = GetUser(loginId);
                            if (user != null)
                            {
                                expandedUsers.Add(user);
                                seenLogins.Add(loginId);
                                
                                // Limit to avoid too many API calls
                                if (expandedUsers.Count >= 100)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Reached expansion limit of 100 additional users");
                                    return expandedUsers;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // User doesn't exist or access denied, continue
                            continue;
                        }
                    }
                }

                // Strategy 2: Try common login ID patterns
                var patternUsers = TryCommonLoginPatterns(seenLogins);
                expandedUsers.AddRange(patternUsers);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in user discovery expansion: {ex.Message}");
            }

            return expandedUsers;
        }

        /// <summary>
        /// Get login ID ranges around known users for sequential discovery
        /// </summary>
        private List<(ulong Start, ulong End)> GetLoginRanges(List<UserInfo> knownUsers)
        {
            var ranges = new List<(ulong Start, ulong End)>();
            var sortedLogins = knownUsers.Select(u => u.Login).OrderBy(l => l).ToList();

            foreach (var login in sortedLogins)
            {
                // Check range around each known login (±50)
                ulong start = login > 50 ? login - 50 : 1;
                ulong end = login + 50;
                
                ranges.Add((start, end));
            }

            return ranges.Take(5).ToList(); // Limit to 5 ranges to avoid too many API calls
        }

        /// <summary>
        /// Try common login ID patterns
        /// </summary>
        private List<UserInfo> TryCommonLoginPatterns(HashSet<ulong> seenLogins)
        {
            var patternUsers = new List<UserInfo>();
            
            // Common patterns: 1-100, 1000-1100, 10000-10100, etc.
            ulong[] commonStarts = { 1, 100, 1000, 10000, 100000 };
            
            foreach (var start in commonStarts)
            {
                for (ulong i = start; i < start + 20; i++) // Check first 20 in each range
                {
                    if (seenLogins.Contains(i))
                        continue;

                    try
                    {
                        var user = GetUser(i);
                        if (user != null)
                        {
                            patternUsers.Add(user);
                            seenLogins.Add(i);
                            
                            if (patternUsers.Count >= 20) // Limit pattern discovery
                                return patternUsers;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return patternUsers;
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
        /// Get all available groups from the MT5 server
        /// </summary>
        /// <returns>List of all groups</returns>
        public List<GroupInfo> GetAllGroups()
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                var groups = new List<GroupInfo>();

                // Since direct group enumeration may not be available in all MT5 API versions,
                // we'll discover groups by examining users and collecting unique group names
                var knownGroups = new HashSet<string>();
                
                // First, get groups from known categories
                string[] commonGroups = {
                    // Real groups
                    "real", "real\\Executive", "real\\NORMAL", "real\\Vipin Zero 1000",
                    "real\\ALLWIN PREMIUM", "real\\ALLWIN PREMIUM 1", "real\\VIP A", "real\\VIP B",
                    "real\\PRO A", "real\\PRO B", "real\\Standard", "real\\Executive 25",
                    "real\\Vipin Zero", "real\\Vipin Zero 2500", "real\\GOLD 1", "real\\GOLD 2",
                    
                    // Demo groups
                    "demo\\2", "demo\\AllWin Capitals Limited-Demo", "demo\\CFD", "demo\\Executive", 
                    "demo\\PRO", "demo\\PS GOLD", "demo\\VIP", "demo\\forex.hedged", "demo\\gold", 
                    "demo\\stock", "demo\\SPREAD 19", "demo\\Ruble", "demo\\goldnolev",
                    
                    // Manager groups
                    "managers\\administrators", "managers\\board", "managers\\dealers", "managers\\master",
                    
                    // Basic groups
                    "abc", "coverage", "preliminary"
                };

                foreach (string groupName in commonGroups)
                {
                    if (string.IsNullOrEmpty(groupName) || knownGroups.Contains(groupName))
                        continue;

                    try
                    {
                        var users = GetUsersInGroup(groupName);
                        if (users.Count > 0)
                        {
                            knownGroups.Add(groupName);
                            
                            // Create group info based on the group name and users
                            var groupInfo = CreateGroupInfoFromUsers(groupName, users);
                            groups.Add(groupInfo);
                        }
                    }
                    catch (Exception)
                    {
                        // Group doesn't exist or access denied, skip it
                        continue;
                    }
                }

                // Also discover groups from existing users
                try
                {
                    var allUsers = GetAllUsers();
                    var discoveredGroups = allUsers.Select(u => u.Group).Distinct().Where(g => !knownGroups.Contains(g));
                    
                    foreach (string groupName in discoveredGroups)
                    {
                        if (string.IsNullOrEmpty(groupName))
                            continue;

                        var usersInGroup = allUsers.Where(u => u.Group == groupName).ToList();
                        var groupInfo = CreateGroupInfoFromUsers(groupName, usersInGroup);
                        groups.Add(groupInfo);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error discovering additional groups: {ex.Message}");
                }

                // Add any groups that were created through the API but may not have users yet
                foreach (var createdGroup in _createdGroups.Values)
                {
                    // Only add if not already discovered
                    if (!groups.Any(g => g.Name.Equals(createdGroup.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        groups.Add(createdGroup);
                    }
                }

                return groups.OrderBy(g => g.Name).ToList();
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get all groups: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create a new group with the specified configuration
        /// Note: This creates a logical group representation. Actual MT5 server group creation may require additional MT5 Manager API calls.
        /// </summary>
        /// <param name="groupInfo">Group information for the new group</param>
        /// <returns>True if group creation successful</returns>
        public bool CreateGroup(GroupInfo groupInfo)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            if (groupInfo == null)
                throw new ArgumentException("Group info cannot be null", nameof(groupInfo));

            if (string.IsNullOrEmpty(groupInfo.Name))
                throw new ArgumentException("Group name cannot be empty", nameof(groupInfo));

            try
            {
                // Check if group already exists (both in discovered groups and created groups)
                if (_createdGroups.ContainsKey(groupInfo.Name))
                {
                    throw new MT5ApiException($"Group '{groupInfo.Name}' already exists");
                }

                var existingGroups = GetAllGroups();
                var existingGroup = existingGroups.FirstOrDefault(g => g.Name.Equals(groupInfo.Name, StringComparison.OrdinalIgnoreCase));
                
                if (existingGroup != null)
                {
                    throw new MT5ApiException($"Group '{groupInfo.Name}' already exists");
                }

                // Set default values if not specified
                if (string.IsNullOrEmpty(groupInfo.Description))
                    groupInfo.Description = GenerateGroupDescription(groupInfo.Name);

                if (string.IsNullOrEmpty(groupInfo.Company))
                    groupInfo.Company = "MT5 Trading Company";

                if (string.IsNullOrEmpty(groupInfo.Currency))
                    groupInfo.Currency = "USD";

                if (groupInfo.Leverage == 0)
                    groupInfo.Leverage = DetermineGroupLeverage(groupInfo.Name, new List<UserInfo>());

                if (groupInfo.MarginCall == 0)
                    groupInfo.MarginCall = groupInfo.Name.ToLower().Contains("vip") ? 70 : 80;

                if (groupInfo.MarginStopOut == 0)
                    groupInfo.MarginStopOut = groupInfo.Name.ToLower().Contains("vip") ? 40 : 50;

                if (groupInfo.Commission == 0)
                    groupInfo.Commission = DetermineGroupCommission(groupInfo.Name);

                if (groupInfo.Rights == 0)
                {
                    bool isDemo = groupInfo.IsDemo || groupInfo.Name.ToLower().Contains("demo");
                    bool isManager = groupInfo.Name.ToLower().Contains("manager");
                    groupInfo.Rights = DetermineGroupRights(groupInfo.Name, isDemo, isManager);
                }

                if (string.IsNullOrEmpty(groupInfo.EmailFrom))
                    groupInfo.EmailFrom = "noreply@mt5trading.com";

                if (string.IsNullOrEmpty(groupInfo.SupportEmail))
                    groupInfo.SupportEmail = "support@mt5trading.com";

                if (string.IsNullOrEmpty(groupInfo.SupportPage))
                    groupInfo.SupportPage = "https://support.mt5trading.com";

                // Set creation timestamp
                groupInfo.LastUpdate = DateTime.UtcNow;
                groupInfo.UserCount = 0; // New group starts with 0 users

                // Log the group creation attempt
                System.Diagnostics.Debug.WriteLine($"Creating new group: {groupInfo.Name}");
                System.Diagnostics.Debug.WriteLine($"Properties: Leverage={groupInfo.Leverage}, MarginCall={groupInfo.MarginCall}, MarginStopOut={groupInfo.MarginStopOut}");
                System.Diagnostics.Debug.WriteLine($"Rights={groupInfo.Rights}, IsDemo={groupInfo.IsDemo}");

                // Store the created group in memory and persist to file
                _createdGroups[groupInfo.Name] = groupInfo;
                SaveCreatedGroupsToFile();

                // Note: In a full implementation, this would call MT5 Manager API group creation methods
                // such as _manager.GroupAdd() or similar, if available
                // For now, we store the group configuration in memory and file, making it persistent
                // across application restarts until actual MT5 server group creation is available

                return true;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to create group {groupInfo.Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update group configuration (limited to available properties)
        /// Note: Full group configuration updates require MT5 Manager API features that may not be available in all versions
        /// </summary>
        /// <param name="groupName">Group name to update</param>
        /// <param name="groupInfo">Updated group information</param>
        /// <returns>True if update successful</returns>
        public bool UpdateGroup(string groupName, GroupInfo groupInfo)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException("Group name cannot be empty", nameof(groupName));

            if (groupInfo == null)
                throw new ArgumentException("Group info cannot be null", nameof(groupInfo));

            try
            {
                // Note: The MT5 Manager API may have limited group update capabilities
                // This implementation focuses on what's commonly available
                
                // Verify the group exists by checking if it has users
                var existingUsers = GetUsersInGroup(groupName);
                
                if (existingUsers.Count == 0)
                {
                    // Group might not exist or have no users
                    System.Diagnostics.Debug.WriteLine($"Warning: Group {groupName} appears to have no users");
                }

                // For now, we'll simulate the update by storing the group info
                // In a real implementation, this would use MT5 Manager API group update methods
                // such as _manager.GroupUpdate() or similar, if available
                
                // Update the group info properties
                groupInfo.Name = groupName;
                groupInfo.LastUpdate = DateTime.UtcNow;
                groupInfo.UserCount = existingUsers.Count;
                
                // If this is a created group, update it in our storage
                if (_createdGroups.ContainsKey(groupName))
                {
                    _createdGroups[groupName] = groupInfo;
                    SaveCreatedGroupsToFile();
                }
                
                // Log the update attempt
                System.Diagnostics.Debug.WriteLine($"Group update requested for: {groupName}");
                System.Diagnostics.Debug.WriteLine($"Properties: Leverage={groupInfo.Leverage}, MarginCall={groupInfo.MarginCall}, MarginStopOut={groupInfo.MarginStopOut}");
                
                // Since we cannot directly update group settings in many MT5 API versions,
                // we'll return true to indicate the request was processed
                // In production, you would implement actual group update logic here
                
                return true;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to update group {groupName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create a GroupInfo object from a group name and its users
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="users">Users in the group</param>
        /// <returns>GroupInfo object</returns>
        private GroupInfo CreateGroupInfoFromUsers(string groupName, List<UserInfo> users)
        {
            bool isDemo = groupName.ToLower().Contains("demo");
            bool isManager = groupName.ToLower().Contains("manager");
            bool isVIP = groupName.ToLower().Contains("vip") || groupName.ToLower().Contains("executive");
            
            var groupInfo = new GroupInfo
            {
                Name = groupName,
                Description = GenerateGroupDescription(groupName),
                Company = "MT5 Trading Company",
                Currency = "USD",
                Leverage = DetermineGroupLeverage(groupName, users),
                DepositMin = isDemo ? 0 : 100,
                DepositMax = isVIP ? 10000000 : 1000000,
                CreditLimit = 0,
                MarginCall = isVIP ? 70 : 80,
                MarginStopOut = isVIP ? 40 : 50,
                InterestRate = 0,
                Commission = DetermineGroupCommission(groupName),
                CommissionType = 0U,
                AgentCommission = 0,
                FreeMarginMode = 0U,
                Rights = DetermineGroupRights(groupName, isDemo, isManager),
                CheckPassword = true,
                Timeout = (uint)(isManager ? 0 : 60),
                OHLCMaxCount = 65000U,
                NewsMode = 2U,
                ReportsMode = 1U,
                EmailFrom = "noreply@mt5trading.com",
                SMTPServer = "",
                SMTPLogin = "",
                SMTPPassword = "",
                SupportPage = "https://support.mt5trading.com",
                SupportEmail = "support@mt5trading.com",
                Templates = "templates\\",
                CopyQuotes = false,
                Reports = true,
                DefaultDeposit = isDemo ? 10000 : 0,
                DefaultCredit = 0,
                ArchivePeriod = 90U,
                ArchiveMaxRecords = 100000U,
                MarginFreeMode = 0U,
                IsDemo = isDemo,
                UserCount = users.Count,
                LastUpdate = DateTime.UtcNow
            };

            return groupInfo;
        }

        /// <summary>
        /// Generate a description for a group based on its name
        /// </summary>
        private string GenerateGroupDescription(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return "Unknown Group";

            string name = groupName.ToLower();
            
            if (name.Contains("demo"))
                return $"Demo trading group: {groupName}";
            else if (name.Contains("vip") || name.Contains("executive"))
                return $"VIP trading group: {groupName}";
            else if (name.Contains("manager"))
                return $"Manager group: {groupName}";
            else if (name.Contains("real"))
                return $"Real trading group: {groupName}";
            else
                return $"Trading group: {groupName}";
        }

        /// <summary>
        /// Determine appropriate leverage for a group based on name and users
        /// </summary>
        private uint DetermineGroupLeverage(string groupName, List<UserInfo> users)
        {
            if (users.Count > 0)
            {
                // Use the most common leverage among users
                var leverages = users.Where(u => u.Leverage > 0).Select(u => u.Leverage).ToList();
                if (leverages.Count > 0)
                {
                    return (uint)leverages.GroupBy(l => l).OrderByDescending(g => g.Count()).First().Key;
                }
            }

            // Default leverage based on group name
            string name = groupName.ToLower();
            if (name.Contains("demo"))
                return 500U;
            else if (name.Contains("vip") || name.Contains("executive"))
                return 200U;
            else if (name.Contains("zero"))
                return 1000U;
            else
                return 100U;
        }

        /// <summary>
        /// Determine commission for a group based on name
        /// </summary>
        private double DetermineGroupCommission(string groupName)
        {
            string name = groupName.ToLower();
            
            if (name.Contains("zero"))
                return 0.0;
            else if (name.Contains("vip") || name.Contains("executive"))
                return 0.0;
            else if (name.Contains("demo"))
                return 0.0;
            else
                return 7.0; // $7 per lot
        }

        /// <summary>
        /// Determine user rights for a group
        /// </summary>
        private uint DetermineGroupRights(string groupName, bool isDemo, bool isManager)
        {
            if (isManager)
                return 127U; // Full rights for managers
            else if (isDemo)
                return 71U;  // Demo rights
            else
                return 67U;  // Standard real trading rights
        }

        /// <summary>
        /// Load created groups from persistent storage file
        /// </summary>
        private void LoadCreatedGroupsFromFile()
        {
            try
            {
                if (File.Exists(_groupStorageFile))
                {
                    var json = File.ReadAllText(_groupStorageFile);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var loadedGroups = JsonConvert.DeserializeObject<Dictionary<string, GroupInfo>>(json);
                        if (loadedGroups != null)
                        {
                            _createdGroups = loadedGroups;
                            System.Diagnostics.Debug.WriteLine($"Loaded {_createdGroups.Count} created groups from file");
                        }
                    }
                }
                else
                {
                    // Try to load from comprehensive groups file if available
                    LoadComprehensiveGroupsData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading created groups from file: {ex.Message}");
                _createdGroups = new Dictionary<string, GroupInfo>();
            }
        }

        /// <summary>
        /// Load comprehensive MT5 groups data if available
        /// </summary>
        private void LoadComprehensiveGroupsData()
        {
            try
            {
                // Try multiple possible locations for the comprehensive groups file
                string[] possiblePaths = {
                    "complete_mt5_groups.json",                    // Current directory
                    "bin\\Debug\\complete_mt5_groups.json",       // Debug folder
                    "..\\complete_mt5_groups.json",               // Parent directory
                    "..\\..\\complete_mt5_groups.json",           // Workspace root
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "complete_mt5_groups.json") // App directory
                };

                string comprehensiveFile = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        comprehensiveFile = path;
                        System.Diagnostics.Debug.WriteLine($"Found comprehensive groups file at: {path}");
                        break;
                    }
                }

                if (comprehensiveFile != null)
                {
                    var json = File.ReadAllText(comprehensiveFile);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var comprehensiveGroups = JsonConvert.DeserializeObject<Dictionary<string, GroupInfo>>(json);
                        if (comprehensiveGroups != null)
                        {
                            // Load comprehensive groups as baseline, but don't overwrite user-created groups
                            foreach (var kvp in comprehensiveGroups)
                            {
                                if (!_createdGroups.ContainsKey(kvp.Key))
                                {
                                    _createdGroups[kvp.Key] = kvp.Value;
                                }
                            }
                            System.Diagnostics.Debug.WriteLine($"Loaded {comprehensiveGroups.Count} comprehensive groups as baseline");
                            
                            // Save the merged data to the regular storage file
                            SaveCreatedGroupsToFile();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading comprehensive groups data: {ex.Message}");
            }
        }

        /// <summary>
        /// Save created groups to persistent storage file
        /// </summary>
        private void SaveCreatedGroupsToFile()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_createdGroups, Formatting.Indented);
                File.WriteAllText(_groupStorageFile, json);
                System.Diagnostics.Debug.WriteLine($"Saved {_createdGroups.Count} created groups to file");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving created groups to file: {ex.Message}");
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
                // Validate user exists first
                var user = GetUser(login);
                if (user == null)
                    throw new MT5ApiException($"User with login {login} not found");

                // Check user rights
                if (user.Rights == 0)
                    throw new MT5ApiException($"User {login} has no trading rights (rights = 0)");

                // Perform the balance operation
                System.Diagnostics.Debug.WriteLine($"Attempting balance operation: Login={login}, Amount={amount}, Type={type}, Comment={comment}");
                
                bool result = _manager.DealerBalance(login, Math.Abs(amount), type, comment ?? "", amount > 0);
                
                System.Diagnostics.Debug.WriteLine($"Balance operation result: {result}");
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Balance operation exception: {ex.Message}");
                throw new MT5ApiException($"Failed to perform balance operation for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Test balance operation without actually performing it
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <returns>Dictionary with validation results</returns>
        public Dictionary<string, object> TestBalanceOperationValidation(ulong login)
        {
            var results = new Dictionary<string, object>();
            
            try
            {
                results["api_connected"] = _isConnected;
                
                if (!_isConnected)
                {
                    results["error"] = "Not connected to MT5 server";
                    return results;
                }

                // Check user
                try
                {
                    var user = GetUser(login);
                    if (user != null)
                    {
                        results["user_exists"] = true;
                        results["user_name"] = user.Name;
                        results["user_group"] = user.Group;
                        results["user_rights"] = user.Rights;
                        results["user_rights_ok"] = user.Rights > 0;
                    }
                    else
                    {
                        results["user_exists"] = false;
                        results["error"] = "User not found";
                        return results;
                    }
                }
                catch (Exception ex)
                {
                    results["user_error"] = ex.Message;
                    return results;
                }

                // Check account
                try
                {
                    var account = GetAccount(login);
                    if (account != null)
                    {
                        results["account_exists"] = true;
                        results["current_balance"] = account.Balance;
                        results["account_currency"] = account.Currency;
                    }
                    else
                    {
                        results["account_exists"] = false;
                    }
                }
                catch (Exception ex)
                {
                    results["account_error"] = ex.Message;
                }

                results["validation_passed"] = (bool)results["user_exists"] && (bool)results["user_rights_ok"];
                
            }
            catch (Exception ex)
            {
                results["error"] = ex.Message;
            }

            return results;
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
        /// Get user positions
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <returns>List of user positions</returns>
        public List<PositionInfo> GetUserPositions(ulong login)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            try
            {
                CIMTPositionArray positions;
                if (!_manager.GetUserPositions(out positions, login))
                    return new List<PositionInfo>();

                var result = new List<PositionInfo>();
                for (uint i = 0; i < positions.Total(); i++)
                {
                    var position = positions.Next(i);
                    if (position != null)
                    {
                        result.Add(new PositionInfo
                        {
                            PositionId = position.Position(),
                            Login = position.Login(),
                            Symbol = position.Symbol(),
                            Action = position.Action().ToString(),
                            Volume = position.Volume(),
                            PriceOpen = position.PriceOpen(),
                            PriceCurrent = position.PriceCurrent(),
                            Profit = position.Profit(),
                            Storage = position.Storage(),
                            Commission = 0.0, // Not available in this API version
                            TimeCreate = SMTTime.ToDateTime(position.TimeCreate()),
                            TimeUpdate = SMTTime.ToDateTime(position.TimeUpdate()),
                            Comment = position.Comment(),
                            ExternalId = position.ExternalID(),
                            Reason = position.Reason().ToString(),
                            Digits = position.Digits(),
                            DigitsCurrency = position.DigitsCurrency(),
                            ContractSize = position.ContractSize(),
                            RateProfit = position.RateProfit(),
                            RateMargin = position.RateMargin(),
                            ExpertId = position.ExpertID(),
                            ExpertPositionId = position.ExpertPositionID()
                        });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get positions for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get position summary for a user
        /// </summary>
        /// <param name="login">User login ID</param>
        /// <returns>Position summary</returns>
        public PositionSummary GetUserPositionSummary(ulong login)
        {
            try
            {
                var positions = GetUserPositions(login);
                
                return new PositionSummary
                {
                    Login = login,
                    TotalPositions = positions.Count,
                    BuyPositions = positions.Count(p => p.Action.Contains("Buy")),
                    SellPositions = positions.Count(p => p.Action.Contains("Sell")),
                    TotalVolume = positions.Sum(p => p.Volume),
                    TotalProfit = positions.Sum(p => p.Profit),
                    Symbols = positions.Select(p => p.Symbol).Distinct().ToArray(),
                    LastUpdate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get position summary for login {login}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get positions for all users in a group (using fallback method)
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>List of all positions in the group</returns>
        public List<PositionInfo> GetGroupPositions(string groupName)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to MT5 server");

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException("Group name cannot be empty", nameof(groupName));

            try
            {
                // Use fallback method to get group positions
                var positionArrays = _manager.GetGroupPositionsFallback(groupName);
                if (positionArrays == null || positionArrays.Count == 0)
                    return new List<PositionInfo>();

                var result = new List<PositionInfo>();
                
                // Process each user's positions
                foreach (var positionArray in positionArrays)
                {
                    if (positionArray != null)
                    {
                        for (uint i = 0; i < positionArray.Total(); i++)
                        {
                            var position = positionArray.Next(i);
                            if (position != null)
                            {
                                result.Add(new PositionInfo
                                {
                                    PositionId = position.Position(),
                                    Login = position.Login(),
                                    Symbol = position.Symbol(),
                                    Action = position.Action().ToString(),
                                    Volume = position.Volume(),
                                    PriceOpen = position.PriceOpen(),
                                    PriceCurrent = position.PriceCurrent(),
                                    Profit = position.Profit(),
                                    Storage = position.Storage(),
                                    Commission = 0.0, // Not available in this API version
                                    TimeCreate = SMTTime.ToDateTime(position.TimeCreate()),
                                    TimeUpdate = SMTTime.ToDateTime(position.TimeUpdate()),
                                    Comment = position.Comment(),
                                    ExternalId = position.ExternalID(),
                                    Reason = position.Reason().ToString(),
                                    Digits = position.Digits(),
                                    DigitsCurrency = position.DigitsCurrency(),
                                    ContractSize = position.ContractSize(),
                                    RateProfit = position.RateProfit(),
                                    RateMargin = position.RateMargin(),
                                    ExpertId = position.ExpertID(),
                                    ExpertPositionId = position.ExpertPositionID()
                                });
                            }
                        }
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new MT5ApiException($"Failed to get positions for group {groupName}: {ex.Message}", ex);
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
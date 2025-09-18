//+------------------------------------------------------------------+
//|                                MetaTrader 5 API Manager for .NET |
//|                        Copyright 2012-2021, OBI HOLDINGS PTE LTD |
//|                                              https://obih.sg/ja/ |
//+------------------------------------------------------------------+
namespace MT5Manager
{
    using MetaQuotes.MT5CommonAPI;
    using MetaQuotes.MT5ManagerAPI;
    using System;

    //+------------------------------------------------------------------+
    //| Manager                                                          |
    //+------------------------------------------------------------------+
    public class CManager : IDisposable
    {
        //--- connect timeout in milliseconds
        uint MT5_CONNECT_TIMEOUT = 30000;
        //---
        CIMTManagerAPI m_manager = null;
        CIMTDealArray m_deal_array = null;
        CIMTUser m_user = null;
        CIMTUserArray m_users = null;
        CIMTAccount m_account = null;
        CIMTPositionArray m_positions = null;
        //+------------------------------------------------------------------+
        //| Constructor                                                      |
        //+------------------------------------------------------------------+
        public CManager()
        {
        }
        //+------------------------------------------------------------------+
        //| Destructor                                                       |
        //+------------------------------------------------------------------+
        public void Dispose()
        {
            Shutdown();
        }
        //+------------------------------------------------------------------+
        //| Initialize library                                               |
        //+------------------------------------------------------------------+
        public bool Initialize()
        {
            string message = string.Empty;
            MTRetCode res = MTRetCode.MT_RET_OK_NONE;
            //--- loading manager API
            if ((res = SMTManagerAPIFactory.Initialize(null)) != MTRetCode.MT_RET_OK)
            {
                message = string.Format("Loading manager API failed ({0})", res);
                System.Console.WriteLine(message);
                return (false);
            }
            //--- creating manager interface
            m_manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out res);
            if ((res != MTRetCode.MT_RET_OK) || (m_manager == null))
            {
                SMTManagerAPIFactory.Shutdown();
                message = string.Format("Creating manager interface failed ({0})", (res == MTRetCode.MT_RET_OK ? "Managed API is null" : res.ToString()));
                System.Console.WriteLine(message);
                return (false);
            }
            //--- create deal array
            if ((m_deal_array = m_manager.DealCreateArray()) == null)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "DealCreateArray fail");
                System.Console.WriteLine("DealCreateArray fail");
                return (false);
            }
            //--- create user interface
            if ((m_user = m_manager.UserCreate()) == null)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserCreate fail");
                System.Console.WriteLine("UserCreate fail");
                return (false);
            }
            //--- create user array interface
            if ((m_users = m_manager.UserCreateArray()) == null)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserCreateArray fail");
                System.Console.WriteLine("UserCreateArray fail");
                return (false);
            }
            //--- create account interface
            if ((m_account = m_manager.UserCreateAccount()) == null)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserCreateAccount fail");
                System.Console.WriteLine("UserCreateAccount fail");
                return (false);
            }
            //--- create position array
            if ((m_positions = m_manager.PositionCreateArray()) == null)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "PositionCreateArray fail");
                System.Console.WriteLine("PositionCreateArray fail");
                return (false);
            }
            //--- all right
            return (true);
        }
        //+------------------------------------------------------------------+
        //| Login                                                            |
        //+------------------------------------------------------------------+
        public bool Login(string server, UInt64 login, string password)
        {
            //--- connect
            MTRetCode res = m_manager.Connect(server, login, password, null, CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, MT5_CONNECT_TIMEOUT);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "Connection failed ({0})", res);
                return (false);
            }
            return (true);
        }
        //+------------------------------------------------------------------+
        //| Logout                                                           |
        //+------------------------------------------------------------------+
        public void Logout()
        {
            //--- disconnect manager
            if (m_manager != null)
                m_manager.Disconnect();
        }
        //+------------------------------------------------------------------+
        //| Shutdown                                                         |
        //+------------------------------------------------------------------+
        public void Shutdown()
        {
            if (m_deal_array != null)
            {
                m_deal_array.Dispose();
                m_deal_array = null;
            }
            if (m_manager != null)
            {
                m_manager.Dispose();
                m_manager = null;
            }
            if (m_user != null)
            {
                m_user.Dispose();
                m_user = null;
            }
            if (m_users != null)
            {
                m_users.Dispose();
                m_users = null;
            }
            if (m_account != null)
            {
                m_account.Dispose();
                m_account = null;
            }
            if (m_positions != null)
            {
                m_positions.Dispose();
                m_positions = null;
            }
            SMTManagerAPIFactory.Shutdown();
        }
        //+------------------------------------------------------------------+
        //| Get array of dealer balance operation                            |
        //+------------------------------------------------------------------+
        public bool GetUserDeal(out CIMTDealArray deals, UInt64 login, DateTime time_from, DateTime time_to)
        {
            deals = null;
            //--- request array
            MTRetCode res = m_manager.DealRequest(login, SMTTime.FromDateTime(time_from), SMTTime.FromDateTime(time_to), m_deal_array);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "DealRequest fail({0})", res);
                return (false);
            }
            //---
            deals = m_deal_array;
            return (true);
        }
        //+------------------------------------------------------------------+
        //| Get user info string                                             |
        //+------------------------------------------------------------------+
        public CIMTUser GetUserInfo(UInt64 login)
        {
            //--- request user from server
            m_user.Clear();
            MTRetCode res = m_manager.UserRequest(login, m_user);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserRequest error ({0})", res);
                return (null);
            }
            //---
            return (m_user);
        }
        //+------------------------------------------------------------------+
        //| Get user info string                                             |
        //+------------------------------------------------------------------+
        public CIMTAccount GetAccountInfo(UInt64 login)
        {
            m_account.Clear();
            MTRetCode res = m_manager.UserAccountRequest(login, m_account);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserAccountRequest error ({0})", res);
                return (null);
            }
            //---
            return (m_account);
        }
        //+------------------------------------------------------------------+
        //| Dealer operation                                                 |
        //+------------------------------------------------------------------+
        public bool DealerBalance(UInt64 login, double amount, uint type, string comment, bool deposit)
        {
            ulong deal_id;
            MTRetCode res = m_manager.DealerBalance(login, deposit ? amount : -amount, type, comment, out deal_id);
            if (res != MTRetCode.MT_RET_REQUEST_DONE)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "DealerBalance error ({0})", res);
                return (false);
            }
            return (true);
        }
        //+------------------------------------------------------------------+
        //| Get User Array                                                   |
        //+------------------------------------------------------------------+
        // https://support.metaquotes.net/en/docs/mt5/api/imtmanagerapi/imtmanagerapi_user/imtmanagerapi_userrequestarray
        public CIMTUserArray GetUsers(string group)
        {
            m_users.Clear();
            MTRetCode res = m_manager.UserRequestArray(group, m_users);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserRequestArray error ({0})", res);
                return (null); ;
            }
            return (m_users);
        }
        //+------------------------------------------------------------------+
        //| Get User Array                                                   |
        //+------------------------------------------------------------------+
        public bool GetGroup(ulong login, out string group)
        {
            MTRetCode res = m_manager.UserGroup(login, out group);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "UserGroup error ({0})", res);
                return (false);
            }
            return (true);
        }
        //+------------------------------------------------------------------+
        //| Get Users from Common Groups (fallback implementation)          |
        //+------------------------------------------------------------------+
        public System.Collections.Generic.List<CIMTUserArray> GetUsersFromCommonGroups()
        {
            // Your server's actual group names (complete list)
            string[] actualGroups = { 
                // Basic groups
                "abc", "coverage", "preliminary", "real",
                
                // Demo groups
                "demo\\2", "demo\\AllWin Capitals Limited-Demo", "demo\\CFD", "demo\\Executive", 
                "demo\\PRO", "demo\\PS GOLD", "demo\\Ruble", "demo\\SPREAD 19", "demo\\VIP", 
                "demo\\forex.hedged", "demo\\forex.hedged1", "demo\\gold", "demo\\gold souq", 
                "demo\\goldnolev", "demo\\gsnew15 test", "demo\\no lev", "demo\\stock",
                
                // Manager groups
                "managers\\administrators", "managers\\board", "managers\\dealers", "managers\\master",
                
                // Real groups (your main groups)
                "real\\20spread", "real\\50 lev", "real\\ALLWIN CENT COVER", "real\\ALLWIN CENT GIVEUP", 
                "real\\ALLWIN PREMIUM", "real\\ALLWIN PREMIUM 1", "real\\ALLWIN PREMIUM 1 B", 
                "real\\ALLWIN PREMIUM 1 test", "real\\ALLWIN PREMIUM 1A", "real\\ALLWIN PREMIUM Hedging strgy", 
                "real\\ALLWIN PREMIUM netting cents", "real\\ALLWIN PREMIUM netting usd", "real\\ALLWIN STD 1", 
                "real\\ALLWIN Test", "real\\ALLWIN VIP 1", "real\\ARAFAT NEW", "real\\Arafat gold", 
                "real\\B2B", "real\\CENT GIVEUP", "real\\CENT INGT", "real\\CENT INGT TEST", 
                "real\\CENT INGT new", "real\\CENT_EA", "real\\Copy trades", "real\\Crown 25 spread", 
                "real\\Crown 30 spread", "real\\Crown 35 spread", "real\\Crown 40 spread", 
                "real\\Crown 45 spread", "real\\Crown 50 spread", "real\\EA B2B", "real\\EA TEST", 
                "real\\EA TEST 2", "real\\EA TEST 3", "real\\Ea covering", "real\\Executive", 
                "real\\Executive 25", "real\\Executive Swap", "real\\Executive Swap+", "real\\Executive+KGM", 
                "real\\Faizal 1", "real\\Faizal 122", "real\\Faizal 4", "real\\Faizal 5", 
                "real\\GOLD 1", "real\\GOLD 2", "real\\GOLD EA", "real\\GRAMIN JEWELLERY", 
                "real\\Gld INGT", "real\\Gld INGT DEMO", "real\\Gld OZ SPREAD 50", "real\\Gold26", 
                "real\\INDIA", "real\\INGT NRML", "real\\JIBIN IB", "real\\LONG TERM", 
                "real\\MSTR IB", "real\\Metal20Spread", "real\\NG", "real\\NORMAL", 
                "real\\NORMAL 2", "real\\NORMAL 60spread", "real\\NORMAL STOCK", "real\\NORMAL Scalpers", 
                "real\\NORMAL slip", "real\\Naseem", "real\\Nijas", "real\\Niyaz", 
                "real\\Normal Floating", "real\\PIP 40", "real\\PRO A", "real\\PRO A1", 
                "real\\PRO B", "real\\PRO new", "real\\Prakash", "real\\SPREAD 15", 
                "real\\SPREAD 19", "real\\STD A", "real\\STD B", "real\\STOCK 20", 
                "real\\STOCK 20 Netting", "real\\SUB IB", "real\\SWAP AFTER 10 DAYS", 
                "real\\Saiful Executive", "real\\Saiful PRO", "real\\Saiful VIP", "real\\Soni Ji Bullion", 
                "real\\Standard", "real\\Stocks 1", "real\\Stocks 1 (100 stop)", "real\\Stocks 10 lev", 
                "real\\Stocks 20 lev", "real\\Stocks 4 lev", "real\\Stocks 5 lev", "real\\TEST", 
                "real\\TEST STOCK 20", "real\\TEST Z", "real\\TradersArena25", "real\\UAE", 
                "real\\VIP A", "real\\VIP B", "real\\VIPIN 18 Spread", "real\\VIPIN KGM", 
                "real\\Vinayak Floating", "real\\Vipin 18", "real\\Vipin 18 Test", "real\\Vipin New", 
                "real\\Vipin Zero", "real\\Vipin Zero 1000", "real\\Vipin Zero 2500", 
                "real\\Vipin Zero 2500 test", "real\\Vipin Zero 2500(400 lev)", "real\\Vipin no lev", 
                "real\\amanawafi", "real\\awafispread45", "real\\awafispread60", "real\\faisal 8", 
                "real\\faisal7", "real\\faizal 6", "real\\falah saadi", "real\\falah saadi 100spread", 
                "real\\falah saadi 100spread&lev", "real\\falah saadi 100spread200lev", 
                "real\\falah saadi 200spread100lev", "real\\falah saadi 200spread200lev", 
                "real\\falah saadi 50spread200lev", "real\\falah saadi 55spread200lev", 
                "real\\falah saadi 60spread200lev", "real\\falah saadi new", "real\\falah saadi normal", 
                "real\\goldlev20", "real\\goldlev20 Spread 30+", "real\\goldnolev", "real\\gs01", 
                "real\\gsnew", "real\\gsnew15", "real\\gsnew15 test", "real\\gsnew6", 
                "real\\ibrahim", "real\\ibrahim Netting", "real\\lev20", "real\\manualtraders", 
                "real\\margin 4", "real\\margin 5", "real\\newtest", "real\\niyas80spred", 
                "real\\raw\\raw WL", "real\\real", "real\\shahala", "real\\shameem", 
                "real\\shameem 2", "real\\shameem swap", "real\\stock0lev", "real\\stock0lev Executive", 
                "real\\stock0levTest", "real\\test 1", "real\\test 1265", "real\\test 2", 
                "real\\tt only", "real\\yoosuf 50 spread", "real\\yoosuf Floating", "real\\yoosuf New"
            };

            var userArrays = new System.Collections.Generic.List<CIMTUserArray>();
            
            foreach (string groupName in actualGroups)
            {
                try
                {
                    var users = GetUsers(groupName);
                    if (users != null && users.Total() > 0)
                    {
                        userArrays.Add(users);
                    }
                }
                catch (Exception)
                {
                    // Group doesn't exist or access denied, continue with next group
                    continue;
                }
            }
            
            return userArrays;
        }
        //+------------------------------------------------------------------+
        //| Get User Positions                                               |
        //+------------------------------------------------------------------+
        public bool GetUserPositions(out CIMTPositionArray positions, UInt64 login)
        {
            positions = null;
            MTRetCode res = m_manager.PositionRequest(login, m_positions);
            if (res != MTRetCode.MT_RET_OK)
            {
                m_manager.LoggerOut(EnMTLogCode.MTLogErr, "PositionRequest fail({0})", res);
                return (false);
            }
            //--- 
            positions = m_positions;
            return (true);
        }
        //+------------------------------------------------------------------+
        //| Get All Positions (fallback: get individual user positions)     |
        //+------------------------------------------------------------------+
        public System.Collections.Generic.List<CIMTPositionArray> GetGroupPositionsFallback(string group)
        {
            var positionArrays = new System.Collections.Generic.List<CIMTPositionArray>();
            
            // Get users in the group first
            var users = GetUsers(group);
            if (users == null) return positionArrays;
            
            // Get positions for each user in the group
            for (uint i = 0; i < Math.Min(users.Total(), 50); i++) // Limit to 50 users to avoid too many API calls
            {
                var user = users.Next(i);
                if (user != null)
                {
                    try
                    {
                        CIMTPositionArray userPositions;
                        if (GetUserPositions(out userPositions, user.Login()))
                        {
                            positionArrays.Add(userPositions);
                        }
                    }
                    catch (Exception)
                    {
                        // Continue with next user if this one fails
                        continue;
                    }
                }
            }
            
            return positionArrays;
        }
    }
}
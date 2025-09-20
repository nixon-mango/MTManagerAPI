using System;
using System.Collections.Generic;

namespace MT5ManagerAPI.Models
{
    /// <summary>
    /// Represents MT5 group information and configuration
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        /// Group name (e.g., "real\Executive", "demo\VIP")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Group description/comment
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Company name for this group
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Group currency (USD, EUR, etc.)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Default leverage for new accounts in this group
        /// </summary>
        public uint Leverage { get; set; }

        /// <summary>
        /// Minimum deposit amount
        /// </summary>
        public double DepositMin { get; set; }

        /// <summary>
        /// Maximum deposit amount
        /// </summary>
        public double DepositMax { get; set; }

        /// <summary>
        /// Credit limit for the group
        /// </summary>
        public double CreditLimit { get; set; }

        /// <summary>
        /// Margin call level (percentage)
        /// </summary>
        public double MarginCall { get; set; }

        /// <summary>
        /// Stop out level (percentage)
        /// </summary>
        public double MarginStopOut { get; set; }

        /// <summary>
        /// Interest rate for credit
        /// </summary>
        public double InterestRate { get; set; }

        /// <summary>
        /// Commission settings
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        /// Commission type (0=money, 1=pips, 2=percent)
        /// </summary>
        public uint CommissionType { get; set; }

        /// <summary>
        /// Agent commission
        /// </summary>
        public double AgentCommission { get; set; }

        /// <summary>
        /// Free margin calculation mode
        /// </summary>
        public uint FreeMarginMode { get; set; }

        /// <summary>
        /// User rights flags
        /// </summary>
        public uint Rights { get; set; }

        /// <summary>
        /// Check password flag
        /// </summary>
        public bool CheckPassword { get; set; }

        /// <summary>
        /// Timeout in seconds for inactive connections
        /// </summary>
        public uint Timeout { get; set; }

        /// <summary>
        /// OHLC history access
        /// </summary>
        public uint OHLCMaxCount { get; set; }

        /// <summary>
        /// News mode (0=disabled, 1=headers only, 2=full news)
        /// </summary>
        public uint NewsMode { get; set; }

        /// <summary>
        /// Reports mode
        /// </summary>
        public uint ReportsMode { get; set; }

        /// <summary>
        /// Email settings
        /// </summary>
        public string EmailFrom { get; set; }

        /// <summary>
        /// SMTP server
        /// </summary>
        public string SMTPServer { get; set; }

        /// <summary>
        /// SMTP login
        /// </summary>
        public string SMTPLogin { get; set; }

        /// <summary>
        /// SMTP password
        /// </summary>
        public string SMTPPassword { get; set; }

        /// <summary>
        /// Support page URL
        /// </summary>
        public string SupportPage { get; set; }

        /// <summary>
        /// Support email
        /// </summary>
        public string SupportEmail { get; set; }

        /// <summary>
        /// Templates path
        /// </summary>
        public string Templates { get; set; }

        /// <summary>
        /// Copy quotes to file flag
        /// </summary>
        public bool CopyQuotes { get; set; }

        /// <summary>
        /// Reports flag
        /// </summary>
        public bool Reports { get; set; }

        /// <summary>
        /// Default deposit amount
        /// </summary>
        public double DefaultDeposit { get; set; }

        /// <summary>
        /// Default credit amount
        /// </summary>
        public double DefaultCredit { get; set; }

        /// <summary>
        /// Archive period in days
        /// </summary>
        public uint ArchivePeriod { get; set; }

        /// <summary>
        /// Archive max records
        /// </summary>
        public uint ArchiveMaxRecords { get; set; }

        /// <summary>
        /// Margin free calculation mode
        /// </summary>
        public uint MarginFreeMode { get; set; }

        /// <summary>
        /// Demo group flag
        /// </summary>
        public bool IsDemo { get; set; }

        /// <summary>
        /// Number of users currently in this group
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Additional custom properties for group configuration
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }

        public GroupInfo()
        {
            CustomProperties = new Dictionary<string, object>();
            LastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// Get a simplified representation of the group for API responses
        /// </summary>
        public object ToSimpleObject()
        {
            return new
            {
                name = Name,
                description = Description,
                company = Company,
                currency = Currency,
                leverage = Leverage,
                margin_call = MarginCall,
                margin_stop_out = MarginStopOut,
                commission = Commission,
                commission_type = CommissionType,
                rights = Rights,
                is_demo = IsDemo,
                user_count = UserCount,
                last_update = LastUpdate
            };
        }

        /// <summary>
        /// Create a group info from basic parameters (for testing/demo purposes)
        /// </summary>
        public static GroupInfo CreateSampleGroup(string name, string description = null, bool isDemo = false)
        {
            return new GroupInfo
            {
                Name = name,
                Description = description ?? $"Sample group: {name}",
                Company = "MT5 Trading Company",
                Currency = "USD",
                Leverage = isDemo ? 500 : 100,
                DepositMin = isDemo ? 0 : 100,
                DepositMax = 1000000,
                CreditLimit = 0,
                MarginCall = 80,
                MarginStopOut = 50,
                InterestRate = 0,
                Commission = 0,
                CommissionType = 0,
                AgentCommission = 0,
                FreeMarginMode = 0,
                Rights = isDemo ? 71 : 67, // Different rights for demo vs real
                CheckPassword = true,
                Timeout = 60,
                OHLCMaxCount = 65000,
                NewsMode = 2,
                ReportsMode = 1,
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
                ArchivePeriod = 90,
                ArchiveMaxRecords = 100000,
                MarginFreeMode = 0,
                IsDemo = isDemo,
                UserCount = 0,
                LastUpdate = DateTime.UtcNow
            };
        }
    }
}
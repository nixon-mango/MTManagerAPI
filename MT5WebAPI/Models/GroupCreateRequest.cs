using System;
using System.ComponentModel.DataAnnotations;

namespace MT5WebAPI.Models
{
    /// <summary>
    /// Request model for creating a new group
    /// </summary>
    public class GroupCreateRequest
    {
        /// <summary>
        /// Group name (required) - e.g., "real\\MyNewGroup", "demo\\TestGroup"
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Group description/comment
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Company name for this group (default: "MT5 Trading Company")
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Group currency (default: "USD")
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Default leverage for new accounts in this group (default: 100)
        /// </summary>
        public uint? Leverage { get; set; }

        /// <summary>
        /// Minimum deposit amount (default: 0 for demo, 100 for real)
        /// </summary>
        public double? DepositMin { get; set; }

        /// <summary>
        /// Maximum deposit amount (default: 1000000)
        /// </summary>
        public double? DepositMax { get; set; }

        /// <summary>
        /// Credit limit for the group (default: 0)
        /// </summary>
        public double? CreditLimit { get; set; }

        /// <summary>
        /// Margin call level percentage (default: 80)
        /// </summary>
        public double? MarginCall { get; set; }

        /// <summary>
        /// Stop out level percentage (default: 50)
        /// </summary>
        public double? MarginStopOut { get; set; }

        /// <summary>
        /// Interest rate for credit (default: 0)
        /// </summary>
        public double? InterestRate { get; set; }

        /// <summary>
        /// Commission settings (default: 0 for VIP/Zero groups, 7.0 for others)
        /// </summary>
        public double? Commission { get; set; }

        /// <summary>
        /// Commission type: 0=money, 1=pips, 2=percent (default: 0)
        /// </summary>
        public uint? CommissionType { get; set; }

        /// <summary>
        /// Agent commission (default: 0)
        /// </summary>
        public double? AgentCommission { get; set; }

        /// <summary>
        /// User rights flags (default: auto-determined by group type)
        /// </summary>
        public uint? Rights { get; set; }

        /// <summary>
        /// Timeout in seconds for inactive connections (default: 60, 0 for managers)
        /// </summary>
        public uint? Timeout { get; set; }

        /// <summary>
        /// News mode: 0=disabled, 1=headers only, 2=full news (default: 2)
        /// </summary>
        public uint? NewsMode { get; set; }

        /// <summary>
        /// Reports mode (default: 1)
        /// </summary>
        public uint? ReportsMode { get; set; }

        /// <summary>
        /// Email from address (default: "noreply@mt5trading.com")
        /// </summary>
        public string EmailFrom { get; set; }

        /// <summary>
        /// Support page URL (default: "https://support.mt5trading.com")
        /// </summary>
        public string SupportPage { get; set; }

        /// <summary>
        /// Support email (default: "support@mt5trading.com")
        /// </summary>
        public string SupportEmail { get; set; }

        /// <summary>
        /// Default deposit amount (default: 10000 for demo, 0 for real)
        /// </summary>
        public double? DefaultDeposit { get; set; }

        /// <summary>
        /// Default credit amount (default: 0)
        /// </summary>
        public double? DefaultCredit { get; set; }

        /// <summary>
        /// Archive period in days (default: 90)
        /// </summary>
        public uint? ArchivePeriod { get; set; }

        /// <summary>
        /// Archive max records (default: 100000)
        /// </summary>
        public uint? ArchiveMaxRecords { get; set; }

        /// <summary>
        /// Is this a demo group? (auto-determined from group name if not specified)
        /// </summary>
        public bool? IsDemo { get; set; }
    }
}
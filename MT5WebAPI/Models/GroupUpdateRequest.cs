using System;

namespace MT5WebAPI.Models
{
    /// <summary>
    /// Request model for updating group configuration
    /// </summary>
    public class GroupUpdateRequest
    {
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
        public uint? Leverage { get; set; }

        /// <summary>
        /// Minimum deposit amount
        /// </summary>
        public double? DepositMin { get; set; }

        /// <summary>
        /// Maximum deposit amount
        /// </summary>
        public double? DepositMax { get; set; }

        /// <summary>
        /// Credit limit for the group
        /// </summary>
        public double? CreditLimit { get; set; }

        /// <summary>
        /// Margin call level (percentage)
        /// </summary>
        public double? MarginCall { get; set; }

        /// <summary>
        /// Stop out level (percentage)
        /// </summary>
        public double? MarginStopOut { get; set; }

        /// <summary>
        /// Interest rate for credit
        /// </summary>
        public double? InterestRate { get; set; }

        /// <summary>
        /// Commission settings
        /// </summary>
        public double? Commission { get; set; }

        /// <summary>
        /// Commission type (0=money, 1=pips, 2=percent)
        /// </summary>
        public uint? CommissionType { get; set; }

        /// <summary>
        /// Agent commission
        /// </summary>
        public double? AgentCommission { get; set; }

        /// <summary>
        /// User rights flags
        /// </summary>
        public uint? Rights { get; set; }

        /// <summary>
        /// Timeout in seconds for inactive connections
        /// </summary>
        public uint? Timeout { get; set; }

        /// <summary>
        /// News mode (0=disabled, 1=headers only, 2=full news)
        /// </summary>
        public uint? NewsMode { get; set; }

        /// <summary>
        /// Reports mode
        /// </summary>
        public uint? ReportsMode { get; set; }

        /// <summary>
        /// Email settings
        /// </summary>
        public string EmailFrom { get; set; }

        /// <summary>
        /// Support page URL
        /// </summary>
        public string SupportPage { get; set; }

        /// <summary>
        /// Support email
        /// </summary>
        public string SupportEmail { get; set; }

        /// <summary>
        /// Default deposit amount
        /// </summary>
        public double? DefaultDeposit { get; set; }

        /// <summary>
        /// Default credit amount
        /// </summary>
        public double? DefaultCredit { get; set; }
    }
}
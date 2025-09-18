using System;
using Newtonsoft.Json;

namespace MT5ManagerAPI.Models
{
    /// <summary>
    /// Represents account information from MT5
    /// </summary>
    public class AccountInfo
    {
        [JsonProperty("login")]
        public ulong Login { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("credit")]
        public double Credit { get; set; }

        [JsonProperty("margin")]
        public double Margin { get; set; }

        [JsonProperty("margin_free")]
        public double MarginFree { get; set; }

        [JsonProperty("margin_level")]
        public double MarginLevel { get; set; }

        [JsonProperty("margin_so_call")]
        public double MarginSOCall { get; set; }

        [JsonProperty("margin_so_so")]
        public double MarginSOSO { get; set; }

        [JsonProperty("profit")]
        public double Profit { get; set; }

        [JsonProperty("storage")]
        public double Storage { get; set; }

        [JsonProperty("commission")]
        public double Commission { get; set; }

        [JsonProperty("floating")]
        public double Floating { get; set; }

        [JsonProperty("equity")]
        public double Equity { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("currency_digits")]
        public uint CurrencyDigits { get; set; }
    }
}
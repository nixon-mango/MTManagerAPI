using System;
using Newtonsoft.Json;

namespace MT5ManagerAPI.Models
{
    /// <summary>
    /// Represents position information from MT5
    /// </summary>
    public class PositionInfo
    {
        [JsonProperty("position_id")]
        public ulong PositionId { get; set; }

        [JsonProperty("login")]
        public ulong Login { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("price_open")]
        public double PriceOpen { get; set; }

        [JsonProperty("price_current")]
        public double PriceCurrent { get; set; }

        [JsonProperty("profit")]
        public double Profit { get; set; }

        [JsonProperty("storage")]
        public double Storage { get; set; }

        [JsonProperty("commission")]
        public double Commission { get; set; }

        [JsonProperty("time_create")]
        public DateTime TimeCreate { get; set; }

        [JsonProperty("time_update")]
        public DateTime TimeUpdate { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("digits")]
        public uint Digits { get; set; }

        [JsonProperty("digits_currency")]
        public uint DigitsCurrency { get; set; }

        [JsonProperty("contract_size")]
        public double ContractSize { get; set; }

        [JsonProperty("rate_profit")]
        public double RateProfit { get; set; }

        [JsonProperty("rate_margin")]
        public double RateMargin { get; set; }

        [JsonProperty("expert_id")]
        public ulong ExpertId { get; set; }

        [JsonProperty("expert_position_id")]
        public ulong ExpertPositionId { get; set; }
    }

    /// <summary>
    /// Position summary for a user
    /// </summary>
    public class PositionSummary
    {
        [JsonProperty("login")]
        public ulong Login { get; set; }

        [JsonProperty("total_positions")]
        public int TotalPositions { get; set; }

        [JsonProperty("buy_positions")]
        public int BuyPositions { get; set; }

        [JsonProperty("sell_positions")]
        public int SellPositions { get; set; }

        [JsonProperty("total_volume")]
        public double TotalVolume { get; set; }

        [JsonProperty("total_profit")]
        public double TotalProfit { get; set; }

        [JsonProperty("symbols")]
        public string[] Symbols { get; set; }

        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }
    }
}
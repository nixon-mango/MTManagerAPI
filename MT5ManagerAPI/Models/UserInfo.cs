using System;
using Newtonsoft.Json;

namespace MT5ManagerAPI.Models
{
    /// <summary>
    /// Represents user information from MT5
    /// </summary>
    public class UserInfo
    {
        [JsonProperty("login")]
        public ulong Login { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zipcode")]
        public string ZipCode { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("registration")]
        public DateTime Registration { get; set; }

        [JsonProperty("last_access")]
        public DateTime LastAccess { get; set; }

        [JsonProperty("leverage")]
        public uint Leverage { get; set; }

        [JsonProperty("rights")]
        public uint Rights { get; set; }
    }
}
using Newtonsoft.Json;

namespace MT5WebAPI.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("timestamp")]
        public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;

        public static ApiResponse<T> CreateSuccess(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data
            };
        }

        public static ApiResponse<T> CreateError(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = error
            };
        }
    }

    public class ConnectRequest
    {
        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("login")]
        public ulong Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class BalanceRequest
    {
        [JsonProperty("login")]
        public ulong Login { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("type")]
        public uint Type { get; set; } = 2;
    }
}
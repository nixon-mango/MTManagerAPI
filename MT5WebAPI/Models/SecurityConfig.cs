using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace MT5WebAPI.Models
{
    /// <summary>
    /// Security configuration for the MT5 Web API
    /// </summary>
    public class SecurityConfig
    {
        private static readonly Lazy<SecurityConfig> _instance = new Lazy<SecurityConfig>(() => new SecurityConfig());
        public static SecurityConfig Instance => _instance.Value;

        public bool RequireApiKey { get; private set; }
        public string ApiKeyHeader { get; private set; }
        public HashSet<string> ValidApiKeys { get; private set; }
        public bool LogSecurityEvents { get; private set; }
        public List<string> AllowedOrigins { get; private set; }

        private SecurityConfig()
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            // Load from App.config or use defaults
            RequireApiKey = bool.Parse(ConfigurationManager.AppSettings["RequireApiKey"] ?? "false");
            ApiKeyHeader = ConfigurationManager.AppSettings["ApiKeyHeader"] ?? "X-API-Key";
            LogSecurityEvents = bool.Parse(ConfigurationManager.AppSettings["LogSecurityEvents"] ?? "true");

            // Load API keys
            ValidApiKeys = new HashSet<string>();
            var apiKeysConfig = ConfigurationManager.AppSettings["ApiKeys"];
            if (!string.IsNullOrEmpty(apiKeysConfig))
            {
                var keys = apiKeysConfig.Split(',');
                foreach (var key in keys)
                {
                    ValidApiKeys.Add(key.Trim());
                }
            }

            // If no keys configured but authentication required, generate a default key
            if (RequireApiKey && ValidApiKeys.Count == 0)
            {
                var defaultKey = GenerateApiKey();
                ValidApiKeys.Add(defaultKey);
                Console.WriteLine($"⚠️  No API keys configured. Generated default key: {defaultKey}");
                Console.WriteLine("   Add this to your App.config for permanent use.");
            }

            // Load allowed origins
            AllowedOrigins = new List<string>();
            var originsConfig = ConfigurationManager.AppSettings["AllowedOrigins"];
            if (!string.IsNullOrEmpty(originsConfig))
            {
                var origins = originsConfig.Split(',');
                foreach (var origin in origins)
                {
                    AllowedOrigins.Add(origin.Trim());
                }
            }
            else
            {
                AllowedOrigins.Add("*"); // Default: allow all origins
            }
        }

        /// <summary>
        /// Validate an API key
        /// </summary>
        public bool IsValidApiKey(string apiKey)
        {
            if (!RequireApiKey)
                return true;

            return !string.IsNullOrEmpty(apiKey) && ValidApiKeys.Contains(apiKey);
        }

        /// <summary>
        /// Generate a new API key
        /// </summary>
        public static string GenerateApiKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            }
        }

        /// <summary>
        /// Check if origin is allowed
        /// </summary>
        public bool IsOriginAllowed(string origin)
        {
            if (AllowedOrigins.Contains("*"))
                return true;

            return !string.IsNullOrEmpty(origin) && AllowedOrigins.Contains(origin);
        }

        /// <summary>
        /// Enable API key authentication (used by command line --secure flag)
        /// </summary>
        public void EnableSecurity()
        {
            RequireApiKey = true;
            
            // If no keys are configured, generate a default one
            if (ValidApiKeys.Count == 0)
            {
                var defaultKey = GenerateApiKey();
                ValidApiKeys.Add(defaultKey);
                Console.WriteLine($"⚠️  Security enabled but no API keys configured. Generated default key: {defaultKey}");
                Console.WriteLine("   Add this to your App.config for permanent use:");
                Console.WriteLine($"   <add key=\"ApiKeys\" value=\"{defaultKey}\" />");
                Console.WriteLine($"   <add key=\"RequireApiKey\" value=\"true\" />");
            }
        }
    }

    /// <summary>
    /// Authentication result
    /// </summary>
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public string ErrorMessage { get; set; }
        public string ApiKey { get; set; }

        public static AuthenticationResult Success(string apiKey)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = true,
                ApiKey = apiKey
            };
        }

        public static AuthenticationResult Failure(string errorMessage)
        {
            return new AuthenticationResult
            {
                IsAuthenticated = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
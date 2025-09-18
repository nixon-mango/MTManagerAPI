using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using MT5WebAPI.Controllers;
using MT5WebAPI.Models;
using Newtonsoft.Json;

namespace MT5WebAPI
{
    public class WebServer
    {
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private readonly MT5Controller _controller;
        private volatile bool _stop;

        public WebServer(string prefix)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _listenerThread = new Thread(HandleRequests);
            _controller = new MT5Controller();
        }

        public void Start()
        {
            _listener.Start();
            _listenerThread.Start();
        }

        public void Stop()
        {
            _stop = true;
            _listener?.Stop();
            _controller?.Dispose();
        }

        private void HandleRequests()
        {
            while (!_stop)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
                catch (Exception ex)
                {
                    if (!_stop)
                        Console.WriteLine($"Error handling request: {ex.Message}");
                }
            }
        }

        private void ProcessRequest(object state)
        {
            HttpListenerContext context = (HttpListenerContext)state;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            try
            {
                // Enable CORS
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                string responseText = "";
                response.ContentType = "application/json";

                // Route the request
                string path = request.Url.AbsolutePath.ToLower();
                string method = request.HttpMethod.ToUpper();

                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {method} {path}");

                // Authentication check
                var authResult = AuthenticateRequest(request);
                if (!authResult.IsAuthenticated)
                {
                    response.StatusCode = 401;
                    responseText = JsonConvert.SerializeObject(new { 
                        success = false, 
                        error = authResult.ErrorMessage,
                        timestamp = DateTime.UtcNow
                    });
                    
                    byte[] authErrorBuffer = Encoding.UTF8.GetBytes(responseText);
                    response.ContentLength64 = authErrorBuffer.Length;
                    response.OutputStream.Write(authErrorBuffer, 0, authErrorBuffer.Length);
                    response.Close();
                    return;
                }

                try
                {
                    if (path == "/api/connect" && method == "POST")
                    {
                        responseText = _controller.Connect(GetRequestBody(request));
                    }
                    else if (path == "/api/disconnect" && method == "POST")
                    {
                        responseText = _controller.Disconnect();
                    }
                    else if (path == "/api/status" && method == "GET")
                    {
                        responseText = _controller.GetStatus();
                    }
                    else if (path.StartsWith("/api/user/") && path.EndsWith("/group") && method == "GET")
                    {
                        string loginStr = ExtractFromPath(path, "/api/user/", "/group");
                        responseText = _controller.GetUserGroup(loginStr);
                    }
                    else if (path.StartsWith("/api/user/") && path.EndsWith("/deals") && method == "GET")
                    {
                        string loginStr = ExtractFromPath(path, "/api/user/", "/deals");
                        var queryParams = ParseQueryString(request.Url.Query);
                        responseText = _controller.GetUserDeals(loginStr, queryParams);
                    }
                    else if (path.StartsWith("/api/user/") && path.EndsWith("/positions") && method == "GET")
                    {
                        string loginStr = ExtractFromPath(path, "/api/user/", "/positions");
                        responseText = _controller.GetUserPositions(loginStr);
                    }
                    else if (path.StartsWith("/api/user/") && path.EndsWith("/positions/summary") && method == "GET")
                    {
                        string loginStr = ExtractFromPath(path, "/api/user/", "/positions/summary");
                        responseText = _controller.GetUserPositionSummary(loginStr);
                    }
                    else if (path.StartsWith("/api/user/") && method == "GET")
                    {
                        string loginStr = ExtractFromPath(path, "/api/user/", "");
                        responseText = _controller.GetUser(loginStr);
                    }
                    else if (path.StartsWith("/api/account/") && method == "GET")
                    {
                        string loginStr = ExtractFromPath(path, "/api/account/", "");
                        responseText = _controller.GetAccount(loginStr);
                    }
                    else if (path == "/api/users" && method == "GET")
                    {
                        responseText = _controller.GetAllUsers();
                    }
                    else if (path == "/api/users/real" && method == "GET")
                    {
                        responseText = _controller.GetAllRealUsers();
                    }
                    else if (path == "/api/users/demo" && method == "GET")
                    {
                        responseText = _controller.GetAllDemoUsers();
                    }
                    else if (path == "/api/users/vip" && method == "GET")
                    {
                        responseText = _controller.GetAllVIPUsers();
                    }
                    else if (path == "/api/users/managers" && method == "GET")
                    {
                        responseText = _controller.GetAllManagerUsers();
                    }
                    else if (path == "/api/users/stats" && method == "GET")
                    {
                        responseText = _controller.GetUserDiscoveryStats();
                    }
                    else if (path.StartsWith("/api/group/") && path.EndsWith("/users") && method == "GET")
                    {
                        string groupName = ExtractFromPath(path, "/api/group/", "/users");
                        responseText = _controller.GetUsersInGroup(groupName);
                    }
                    else if (path.StartsWith("/api/group/") && path.EndsWith("/positions") && method == "GET")
                    {
                        string groupName = ExtractFromPath(path, "/api/group/", "/positions");
                        responseText = _controller.GetGroupPositions(groupName);
                    }
                    else if (path == "/api/balance" && method == "POST")
                    {
                        responseText = _controller.PerformBalanceOperation(GetRequestBody(request));
                    }
                    else
                    {
                        response.StatusCode = 404;
                        responseText = JsonConvert.SerializeObject(new { error = "Endpoint not found" });
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = 500;
                    responseText = JsonConvert.SerializeObject(new { error = ex.Message });
                    Console.WriteLine($"Error processing request: {ex.Message}");
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessRequest: {ex.Message}");
            }
            finally
            {
                try
                {
                    response.Close();
                }
                catch { }
            }
        }

        private string GetRequestBody(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        private string ExtractFromPath(string path, string prefix, string suffix)
        {
            if (!path.StartsWith(prefix))
                return "";

            string result = path.Substring(prefix.Length);
            if (!string.IsNullOrEmpty(suffix) && result.EndsWith(suffix))
                result = result.Substring(0, result.Length - suffix.Length);

            return Uri.UnescapeDataString(result);
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query))
                return result;

            if (query.StartsWith("?"))
                query = query.Substring(1);

            foreach (string pair in query.Split('&'))
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    result[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
                }
            }
            return result;
        }

        private Models.AuthenticationResult AuthenticateRequest(HttpListenerRequest request)
        {
            var config = Models.SecurityConfig.Instance;
            
            // If API key authentication is not required, allow all requests
            if (!config.RequireApiKey)
            {
                return Models.AuthenticationResult.Success(null);
            }

            // Check for API key in headers
            string apiKey = request.Headers[config.ApiKeyHeader];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                // Also check query parameter as fallback
                var queryParams = ParseQueryString(request.Url.Query);
                apiKey = queryParams.ContainsKey("api_key") ? queryParams["api_key"] : null;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                if (config.LogSecurityEvents)
                {
                    Console.WriteLine($"⚠️  Authentication failed: Missing API key from {request.RemoteEndPoint}");
                }
                return Models.AuthenticationResult.Failure($"Missing API key. Include '{config.ApiKeyHeader}' header or 'api_key' query parameter.");
            }

            if (!config.IsValidApiKey(apiKey))
            {
                if (config.LogSecurityEvents)
                {
                    Console.WriteLine($"⚠️  Authentication failed: Invalid API key from {request.RemoteEndPoint}");
                }
                return Models.AuthenticationResult.Failure("Invalid API key.");
            }

            // Check origin if configured
            string origin = request.Headers["Origin"];
            if (!config.IsOriginAllowed(origin))
            {
                if (config.LogSecurityEvents)
                {
                    Console.WriteLine($"⚠️  Authentication failed: Origin not allowed: {origin} from {request.RemoteEndPoint}");
                }
                return Models.AuthenticationResult.Failure("Origin not allowed.");
            }

            if (config.LogSecurityEvents)
            {
                Console.WriteLine($"✅ Authentication successful from {request.RemoteEndPoint}");
            }

            return Models.AuthenticationResult.Success(apiKey);
        }
    }
}
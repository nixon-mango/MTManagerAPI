using System;
using System.Threading;

namespace MT5WebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MT5 Manager Web API Server ===");
            Console.WriteLine();

            // Parse command line arguments
            string host = "localhost";  // Default to localhost (no admin required)
            int port = 8080;
            bool generateKey = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--host" && i + 1 < args.Length)
                    host = args[i + 1];
                else if (args[i] == "--port" && i + 1 < args.Length)
                    int.TryParse(args[i + 1], out port);
                else if (args[i] == "--secure")
                {
                    // Enable security by updating the configuration
                    Models.SecurityConfig.Instance.RequireApiKey = true;
                }
                else if (args[i] == "--generate-key")
                    generateKey = true;
                else if (args[i] == "--help")
                {
                    ShowHelp();
                    return;
                }
            }

            // Handle generate key command
            if (generateKey)
            {
                var newKey = Models.SecurityConfig.GenerateApiKey();
                Console.WriteLine("🔑 Generated new API key:");
                Console.WriteLine($"   {newKey}");
                Console.WriteLine();
                Console.WriteLine("To use this key:");
                Console.WriteLine("1. Add it to App.config:");
                Console.WriteLine($"   <add key=\"ApiKeys\" value=\"{newKey}\" />");
                Console.WriteLine("2. Enable security:");
                Console.WriteLine($"   <add key=\"RequireApiKey\" value=\"true\" />");
                Console.WriteLine("3. Restart the server with --secure flag");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // Start the web server
            var server = new WebServer($"http://{host}:{port}/");
            
            try
            {
                server.Start();
                Console.WriteLine($"✓ Web server started successfully");
                Console.WriteLine($"  Listening on: http://{host}:{port}/");
                
                // Show security status
                var securityConfig = Models.SecurityConfig.Instance;
                if (securityConfig.RequireApiKey)
                {
                    Console.WriteLine($"🔒 Security: API Key authentication ENABLED");
                    Console.WriteLine($"   Header: {securityConfig.ApiKeyHeader}");
                    Console.WriteLine($"   Valid keys: {securityConfig.ValidApiKeys.Count}");
                }
                else
                {
                    Console.WriteLine($"⚠️  Security: API Key authentication DISABLED");
                    Console.WriteLine($"   Use --secure flag or set RequireApiKey=true in App.config to enable");
                }
                Console.WriteLine();
                Console.WriteLine("Available endpoints:");
                Console.WriteLine("  POST /api/connect        - Connect to MT5 server");
                Console.WriteLine("  POST /api/disconnect     - Disconnect from MT5 server");
                Console.WriteLine("  GET  /api/users          - Get all users (complete discovery)");
                Console.WriteLine("  GET  /api/users/real     - Get users from real groups");
                Console.WriteLine("  GET  /api/users/demo     - Get users from demo groups");
                Console.WriteLine("  GET  /api/users/vip      - Get users from VIP groups");
                Console.WriteLine("  GET  /api/users/managers - Get users from manager groups");
                Console.WriteLine("  GET  /api/users/stats    - Get user discovery statistics");
                Console.WriteLine("  GET  /api/user/{login}   - Get user information");
                Console.WriteLine("  GET  /api/account/{login} - Get account information");
                Console.WriteLine("  GET  /api/group/{name}/users - Get users in group");
                Console.WriteLine("  GET  /api/user/{login}/group - Get user's group");
                Console.WriteLine("  GET  /api/user/{login}/positions - Get user positions");
                Console.WriteLine("  GET  /api/user/{login}/positions/summary - Get user position summary");
                Console.WriteLine("  GET  /api/group/{name}/positions - Get group positions");
                Console.WriteLine("  POST /api/balance        - Perform balance operation");
                Console.WriteLine("  GET  /api/user/{login}/deals - Get user deals");
                Console.WriteLine("  GET  /api/status         - Get connection status");
                Console.WriteLine();
                Console.WriteLine("Press 'q' to quit the server...");

                // Wait for quit command
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting server: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server stopped.");
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("MT5 Manager Web API Server");
            Console.WriteLine("==========================");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  MT5WebAPI.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --host <address>    Bind to specific IP address (default: localhost)");
            Console.WriteLine("  --port <number>     Port number (default: 8080)");
            Console.WriteLine("  --secure            Enable API key authentication");
            Console.WriteLine("  --generate-key      Generate a new API key and exit");
            Console.WriteLine("  --help              Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  MT5WebAPI.exe                                    # Local access only");
            Console.WriteLine("  MT5WebAPI.exe --host 0.0.0.0 --port 8080       # Network access (requires admin)");
            Console.WriteLine("  MT5WebAPI.exe --host 192.168.1.100 --port 8080 # Static IP access");
            Console.WriteLine("  MT5WebAPI.exe --secure                          # Enable API key auth");
            Console.WriteLine("  MT5WebAPI.exe --generate-key                    # Generate new API key");
            Console.WriteLine();
            Console.WriteLine("Security:");
            Console.WriteLine("  To enable API key authentication:");
            Console.WriteLine("  1. Generate a key: MT5WebAPI.exe --generate-key");
            Console.WriteLine("  2. Add key to App.config");
            Console.WriteLine("  3. Set RequireApiKey=true in App.config");
            Console.WriteLine("  4. Restart server");
            Console.WriteLine();
        }
    }
}
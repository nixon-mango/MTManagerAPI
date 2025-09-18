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
            string host = "0.0.0.0";  // Changed default to bind to all interfaces
            int port = 8080;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--host" && i + 1 < args.Length)
                    host = args[i + 1];
                else if (args[i] == "--port" && i + 1 < args.Length)
                    int.TryParse(args[i + 1], out port);
            }

            // Start the web server
            var server = new WebServer($"http://{host}:{port}/");
            
            try
            {
                server.Start();
                Console.WriteLine($"✓ Web server started successfully");
                Console.WriteLine($"  Listening on: http://{host}:{port}/");
                Console.WriteLine();
                Console.WriteLine("Available endpoints:");
                Console.WriteLine("  POST /api/connect        - Connect to MT5 server");
                Console.WriteLine("  POST /api/disconnect     - Disconnect from MT5 server");
                Console.WriteLine("  GET  /api/users          - Get all users (enhanced discovery)");
                Console.WriteLine("  GET  /api/users/real     - Get users from your real groups");
                Console.WriteLine("  GET  /api/users/stats    - Get user discovery statistics");
                Console.WriteLine("  GET  /api/user/{login}   - Get user information");
                Console.WriteLine("  GET  /api/account/{login} - Get account information");
                Console.WriteLine("  GET  /api/group/{name}/users - Get users in group");
                Console.WriteLine("  GET  /api/user/{login}/group - Get user's group");
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
    }
}
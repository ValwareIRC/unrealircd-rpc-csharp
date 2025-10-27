using System;
using System.Threading.Tasks;
using System.Net.Http;
using UnrealIRCdRPC;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("UnrealIRCd RPC C# Library Demo");
        Console.WriteLine("===============================");

        // Get connection details from environment variables
        var username = Environment.GetEnvironmentVariable("UNREALIRCD_API_USERNAME");
        var password = Environment.GetEnvironmentVariable("UNREALIRCD_API_PASSWORD");
        var wsURL = Environment.GetEnvironmentVariable("UNREALIRCD_WS_URL") ?? "wss://127.0.0.1:8600/";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("ERROR: Please set UNREALIRCD_API_USERNAME and UNREALIRCD_API_PASSWORD environment variables");
            Console.WriteLine("Example:");
            Console.WriteLine("  export UNREALIRCD_API_USERNAME=your_username");
            Console.WriteLine("  export UNREALIRCD_API_PASSWORD=your_password");
            Console.WriteLine("  export UNREALIRCD_WS_URL=wss://your-server:8600/");
            return;
        }

        Console.WriteLine($"Connecting to: {wsURL}");
        Console.WriteLine($"Using credentials: {username}:[PASSWORD]");
        Console.WriteLine();

        try
        {
            // Test basic connectivity first
            Console.WriteLine("Testing basic connectivity...");
            await TestConnectivity(wsURL);

            // Create connection with timeout
            Console.WriteLine("Connecting to UnrealIRCd...");
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // 10 second timeout
            using var connection = await Connection.NewConnectionAsync(wsURL, $"{username}:{password}",
                new Connection.Options { TlsVerify = false }, cts.Token);

            Console.WriteLine("Connected successfully!");
            Console.WriteLine();

            // Test the strongly typed API
            await TestServerAPI(connection);
            await TestUserAPI(connection);
            await TestChannelAPI(connection);
            await TestServerBanAPI(connection);

            Console.WriteLine();
            Console.WriteLine("All API tests completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            Console.WriteLine();
            Console.WriteLine("Troubleshooting steps:");
            Console.WriteLine("1. Verify UnrealIRCd is running with JSON-RPC enabled");
            Console.WriteLine("2. Check the WebSocket URL is correct (try http vs https)");
            Console.WriteLine("3. Ensure your credentials are valid");
            Console.WriteLine("4. Verify the server is accessible from your network");
            Console.WriteLine("5. Check firewall settings");
            Console.WriteLine($"6. Try testing the connection manually: curl -I {wsURL.Replace("wss://", "https://").Replace("ws://", "http://")}");
        }
    }

    static async Task TestServerAPI(Connection connection)
    {
        Console.WriteLine("Testing Server API...");

        try
        {
            // Get list of servers
            var servers = await connection.Server().GetAllAsync();
            Console.WriteLine($"  Found {servers.Count} servers:");
            foreach (var serverName in servers)
            {
                Console.WriteLine($"    - {serverName}");
            }

            // Get details of first server if available
            if (servers.Count > 0)
            {
                var serverDetails = await connection.Server().GetAsync(servers[0]);
                if (serverDetails != null)
                {
                    Console.WriteLine($"  Server details for {servers[0]}:");
                    Console.WriteLine($"    Name: {serverDetails.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Server API error: {ex.Message}");

            // Try to get raw response for debugging
            try
            {
                Console.WriteLine("  Attempting to get raw server.list response...");
                var rawResult = await connection.QueryAsync("server.list", null, false);
                Console.WriteLine($"  Raw response type: {rawResult?.GetType()?.Name ?? "null"}");
                if (rawResult != null)
                {
                    Console.WriteLine($"  Raw response: {System.Text.Json.JsonSerializer.Serialize(rawResult, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
                }
            }
            catch (Exception debugEx)
            {
                Console.WriteLine($"  Could not get raw response: {debugEx.Message}");
            }
        }

        Console.WriteLine();
    }

    static async Task TestUserAPI(Connection connection)
    {
        Console.WriteLine("Testing User API...");

        try
        {
            // Get list of users (with minimal detail level)
            var users = await connection.User().GetAllAsync(0);
            Console.WriteLine($"  Found {users.Count} users online");

            // Show first few users
            var displayCount = Math.Min(5, users.Count);
            for (int i = 0; i < displayCount; i++)
            {
                Console.WriteLine($"    - {users[i]}");
            }

            if (users.Count > 5)
            {
                Console.WriteLine($"    ... and {users.Count - 5} more");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  User API error: {ex.Message}");

            // Try to get raw response for debugging
            try
            {
                Console.WriteLine("  Attempting to get raw user.list response...");
                var rawResult = await connection.QueryAsync("user.list", new { object_detail_level = 0 }, false);
                Console.WriteLine($"  Raw response type: {rawResult?.GetType()?.Name ?? "null"}");
                if (rawResult != null)
                {
                    Console.WriteLine($"  Raw response: {System.Text.Json.JsonSerializer.Serialize(rawResult, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
                }
            }
            catch (Exception debugEx)
            {
                Console.WriteLine($"  Could not get raw response: {debugEx.Message}");
            }
        }

        Console.WriteLine();
    }

    static async Task TestChannelAPI(Connection connection)
    {
        Console.WriteLine("Testing Channel API...");

        try
        {
            // Get list of channels
            var channels = await connection.Channel().GetAllAsync(0);
            Console.WriteLine($"  Found {channels.Count} channels");

            // Show first few channels
            var displayCount = Math.Min(5, channels.Count);
            for (int i = 0; i < displayCount; i++)
            {
                Console.WriteLine($"    - {channels[i]}");
            }

            if (channels.Count > 5)
            {
                Console.WriteLine($"    ... and {channels.Count - 5} more");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Channel API error: {ex.Message}");

            // Try to get raw response for debugging
            try
            {
                Console.WriteLine("  Attempting to get raw channel.list response...");
                var rawResult = await connection.QueryAsync("channel.list", new { object_detail_level = 0 }, false);
                Console.WriteLine($"  Raw response type: {rawResult?.GetType()?.Name ?? "null"}");
                if (rawResult != null)
                {
                    Console.WriteLine($"  Raw response: {System.Text.Json.JsonSerializer.Serialize(rawResult, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
                }
            }
            catch (Exception debugEx)
            {
                Console.WriteLine($"  Could not get raw response: {debugEx.Message}");
            }
        }

        Console.WriteLine();
    }

    static async Task TestServerBanAPI(Connection connection)
    {
        Console.WriteLine("Testing Server Ban API...");

        try
        {
            // Get list of bans
            var bans = await connection.ServerBan().GetAllAsync();
            Console.WriteLine($"  Found {bans.Count} server bans");

            // Show first few bans
            var displayCount = Math.Min(3, bans.Count);
            for (int i = 0; i < displayCount; i++)
            {
                Console.WriteLine($"    - {bans[i]}");
            }

            if (bans.Count > 3)
            {
                Console.WriteLine($"    ... and {bans.Count - 3} more");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Server Ban API error: {ex.Message}");

            // Try to get raw response for debugging
            try
            {
                Console.WriteLine("  Attempting to get raw server_ban.list response...");
                var rawResult = await connection.QueryAsync("server_ban.list", null, false);
                Console.WriteLine($"  Raw response type: {rawResult?.GetType()?.Name ?? "null"}");
                if (rawResult != null)
                {
                    Console.WriteLine($"  Raw response: {System.Text.Json.JsonSerializer.Serialize(rawResult, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
                }
            }
            catch (Exception debugEx)
            {
                Console.WriteLine($"  Could not get raw response: {debugEx.Message}");
            }
        }

        Console.WriteLine();
    }

    static async Task TestConnectivity(string wsUrl)
    {
        try
        {
            // Convert WebSocket URL to HTTP URL for basic connectivity test
            var httpUrl = wsUrl.Replace("wss://", "https://").Replace("ws://", "http://");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var response = await client.GetAsync(httpUrl);
            Console.WriteLine($"  HTTP connectivity test: {(int)response.StatusCode} {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("  Basic connectivity OK");
            }
            else
            {
                Console.WriteLine($"  Warning: HTTP returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  HTTP connectivity test failed: {ex.Message}");
            Console.WriteLine("  This might indicate network/firewall issues");
        }
        Console.WriteLine();
    }
}
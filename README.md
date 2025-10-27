# UnrealIRCd RPC

[![Build and Test](https://github.com/ValwareIRC/unrealircd-rpc-csharp/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/ValwareIRC/unrealircd-rpc-csharp/actions/workflows/build-and-test.yml)

This allows .NET programs to control [UnrealIRCd](https://www.unrealircd.org/)
via the [JSON-RPC interface](https://www.unrealircd.org/docs/JSON-RPC).

If you are interested in helping out or would like to discuss API
capabilities, join us at `#unreal-webpanel` at `irc.unrealircd.org`
(IRC with TLS on port 6697).

## Installation

Clone the repository and reference it in your project:

```bash
git clone https://github.com/ValwareIRC/unrealircd-rpc-csharp.git
```

Then add a project reference to `UnrealIRCdRPC/UnrealIRCdRPC.csproj`:

```bash
dotnet add reference ../path/to/unrealircd-rpc-csharp/UnrealIRCdRPC/UnrealIRCdRPC.csproj
```

Or add it as a Git dependency in your `.csproj` file:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/unrealircd-rpc-csharp/UnrealIRCdRPC/UnrealIRCdRPC.csproj" />
</ItemGroup>
```

Alternatively, you can build and pack the library locally:

```bash
git clone https://github.com/ValwareIRC/unrealircd-rpc-csharp.git
cd unrealircd-rpc-csharp/UnrealIRCdRPC
dotnet pack -c Release
dotnet nuget push bin/Release/*.nupkg -s Local
```

Then reference it from your local NuGet source.

## UnrealIRCd setup

UnrealIRCd 6.0.6 or later is needed and you need to enable
[JSON-RPC](https://www.unrealircd.org/docs/JSON-RPC) in it.
After doing that, be sure to rehash the IRCd.

## Usage

For this example, create a file like `Program.cs` with:

```csharp
using System;
using System.Threading.Tasks;
using UnrealIRCdRPC;

class Program
{
    static async Task Main(string[] args)
    {
        var username = Environment.GetEnvironmentVariable("UNREALIRCD_API_USERNAME");
        var password = Environment.GetEnvironmentVariable("UNREALIRCD_API_PASSWORD");
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("UNREALIRCD_API_USERNAME and UNREALIRCD_API_PASSWORD environment variables must be set");
            return;
        }
        var apiLogin = $"{username}:{password}";

        var wsURL = Environment.GetEnvironmentVariable("UNREALIRCD_WS_URL");
        if (string.IsNullOrEmpty(wsURL))
        {
            wsURL = "wss://127.0.0.1:8600/"; // default
        }

        using var conn = await Connection.NewConnectionAsync(wsURL, apiLogin, new Connection.Options { TlsVerify = false });

        // Get list of servers (returns IReadOnlyList<string>)
        var servers = await conn.Server().GetAllAsync();
        foreach (var serverName in servers)
        {
            Console.WriteLine($"Server: {serverName}");
        }

        // Get server details (returns ServerInfo object)
        var server = await conn.Server().GetAsync("irc.example.com");
        if (server != null)
        {
            Console.WriteLine($"Server name: {server.Name}");
        }

        // Get list of users (returns IReadOnlyList<string>)
        var users = await conn.User().GetAllAsync(1);
        foreach (var nick in users)
        {
            Console.WriteLine($"User: {nick}");
        }

        // Get user details (returns ClientInfo object)
        var user = await conn.User().GetAsync("someuser", 2);
        if (user != null)
        {
            Console.WriteLine($"User nick: {user.Nick}");
        }

        // Get list of bans (returns IReadOnlyList<string>)
        var bans = await conn.ServerBan().GetAllAsync();
        foreach (var ban in bans)
        {
            Console.WriteLine($"Ban: {ban}");
        }

        // Get ban details (returns TklInfo object)
        var banInfo = await conn.ServerBan().GetAsync("*@badhost", "G");
        if (banInfo != null)
        {
            Console.WriteLine($"Ban type: {banInfo.Type}, mask: {banInfo.Name}");
        }

        var users = await conn.User().GetAllAsync(2);
        // Process users...

        var channels = await conn.Channel().GetAllAsync(1);
        // Process channels...
    }
}
```

Then, run it with `dotnet run`

If the example does not work, then make sure you have configured your
UnrealIRCd correctly, with the same API username and password you use
here, with an allowed IP, and changing the `wss://127.0.0.1:8600/` too
if needed.

## Environment Variables

The library supports configuration via environment variables:

- `UNREALIRCD_API_USERNAME`: API username (required)
- `UNREALIRCD_API_PASSWORD`: API password (required)
- `UNREALIRCD_WS_URL`: WebSocket URL for the UnrealIRCd RPC server (optional, defaults to `wss://127.0.0.1:8600/`)

Example usage:
```bash
export UNREALIRCD_API_USERNAME="api"
export UNREALIRCD_API_PASSWORD="mySecurePassword"
export UNREALIRCD_WS_URL="wss://irc.example.com:8600/"
dotnet run
```

## Custom Queries

All the convenience methods (like `conn.User().GetAllAsync()`) internally use the `Connection.QueryAsync()` method, which is the main wrapper for sending JSON-RPC requests to UnrealIRCd.

You can use `Connection.QueryAsync()` directly for any RPC method that isn't covered by the convenience methods, or for custom implementations:

```csharp
// Example: Get server uptime (custom query)
var uptime = await conn.QueryAsync("server.get", new { server = "irc.example.com" }, false);

// Example: Send a raw JSON-RPC request
var result = await conn.QueryAsync("stats.get", new { object_detail_level = 2 }, false);

// Example: Asynchronous query (no wait for response)
await conn.QueryAsync("log.subscribe", new { sources = new[] { "opers", "errors" } }, true); // true = noWait
```

The `QueryAsync` method parameters are:
- `method`: The JSON-RPC method name (string)
- `parameters`: Parameters for the method (object or null)
- `noWait`: If true, sends the request but doesn't wait for a response (bool)

All responses are returned as `object`, which you can cast to the expected type (usually `Dictionary<string, object>` for objects or arrays for lists).
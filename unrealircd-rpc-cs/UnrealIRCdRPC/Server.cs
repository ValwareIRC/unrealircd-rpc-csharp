using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using UnrealIRCdRPC.Models;

namespace UnrealIRCdRPC
{
    // Server handles server operations
    public class Server
    {
        private readonly IQuerier _querier;

        public Server(IQuerier querier)
        {
            _querier = querier;
        }

        // GetAll returns a list of all servers
        public async Task<IReadOnlyList<string>> GetAllAsync()
        {
            var result = await _querier.QueryAsync("server.list", null, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object)
            {
                if (result.Value.TryGetProperty("list", out var listElement) && listElement.ValueKind == JsonValueKind.Array)
                {
                    var serverNames = new List<string>();
                    foreach (var serverElement in listElement.EnumerateArray())
                    {
                        if (serverElement.ValueKind == JsonValueKind.Object &&
                            serverElement.TryGetProperty("name", out var nameElement) &&
                            nameElement.ValueKind == JsonValueKind.String)
                        {
                            serverNames.Add(nameElement.GetString()!);
                        }
                    }
                    return serverNames;
                }
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get gets a server object
        public async Task<ServerInfo?> GetAsync(string? server = null)
        {
            var parameters = server != null ? new { server } : null;
            var result = await _querier.QueryAsync("server.get", parameters, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("server", out var serverElement))
            {
                // Convert the JsonElement to ServerInfo using JSON deserialization
                return serverElement.Deserialize<ServerInfo>();
            }
            return null; // not found
        }
    }
}
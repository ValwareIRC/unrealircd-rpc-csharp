using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using UnrealIRCdRPC.Models;

namespace UnrealIRCdRPC
{
    // ServerBan handles server ban operations
    public class ServerBan
    {
        private readonly IQuerier _querier;

        public ServerBan(IQuerier querier)
        {
            _querier = querier;
        }

        // Add adds a ban
        public async Task<TklInfo?> AddAsync(string name, string banType, string duration, string reason)
        {
            var result = await _querier.QueryAsync("server_ban.add", new
            {
                name,
                type = banType,
                reason,
                duration_string = duration
            }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                // Convert the JsonElement to TklInfo using JSON deserialization
                return tklElement.Deserialize<TklInfo>();
            }
            return null;
        }

        // Delete deletes a ban
        public async Task<JsonElement?> DeleteAsync(string name, string banType)
        {
            var result = await _querier.QueryAsync("server_ban.del", new { name, type = banType }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // GetAll returns a list of all bans
        public async Task<IReadOnlyList<string>> GetAllAsync()
        {
            var result = await _querier.QueryAsync("server_ban.list", null, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object)
            {
                if (result.Value.TryGetProperty("list", out var listElement) && listElement.ValueKind == JsonValueKind.Array)
                {
                    var banNames = new List<string>();
                    foreach (var banElement in listElement.EnumerateArray())
                    {
                        if (banElement.ValueKind == JsonValueKind.Object &&
                            banElement.TryGetProperty("name", out var nameElement) &&
                            nameElement.ValueKind == JsonValueKind.String)
                        {
                            banNames.Add(nameElement.GetString()!);
                        }
                    }
                    return banNames;
                }
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get gets a specific ban
        public async Task<TklInfo?> GetAsync(string name, string banType)
        {
            var result = await _querier.QueryAsync("server_ban.get", new { name, type = banType }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                // Convert the JsonElement to TklInfo using JSON deserialization
                return tklElement.Deserialize<TklInfo>();
            }
            return null; // not found
        }
    }
}
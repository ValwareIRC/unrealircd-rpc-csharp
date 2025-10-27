using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace UnrealIRCdRPC
{
    // NameBan handles name ban operations
    public class NameBan
    {
        private readonly IQuerier _querier;

        public NameBan(IQuerier querier)
        {
            _querier = querier;
        }

        // Add adds a name ban
        public async Task<JsonElement?> AddAsync(string name, string reason, string? duration = null, string? setBy = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "name", name },
                { "reason", reason }
            };
            if (duration != null) parameters["duration_string"] = duration;
            if (setBy != null) parameters["set_by"] = setBy;
            var result = await _querier.QueryAsync("name_ban.add", parameters, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // Delete deletes a name ban
        public async Task<JsonElement?> DeleteAsync(string name)
        {
            var result = await _querier.QueryAsync("name_ban.del", new { name }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // GetAll returns a list of all name bans
        public async Task<JsonElement?> GetAllAsync()
        {
            var result = await _querier.QueryAsync("name_ban.list", null, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("list", out var listElement))
            {
                return listElement;
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get gets a specific name ban
        public async Task<JsonElement?> GetAsync(string name)
        {
            var result = await _querier.QueryAsync("name_ban.get", new { name }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null; // not found
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace UnrealIRCdRPC
{
    // ServerBanException handles server ban exception operations
    public class ServerBanException
    {
        private readonly IQuerier _querier;

        public ServerBanException(IQuerier querier)
        {
            _querier = querier;
        }

        // Add adds a server ban exception
        public async Task<JsonElement?> AddAsync(string name, string exceptionTypes, string reason, string? setBy = null, string? duration = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "name", name },
                { "exception_types", exceptionTypes },
                { "reason", reason }
            };
            if (setBy != null) parameters["set_by"] = setBy;
            if (duration != null) parameters["duration_string"] = duration;
            var result = await _querier.QueryAsync("server_ban_exception.add", parameters, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // Delete deletes a server ban exception
        public async Task<JsonElement?> DeleteAsync(string name)
        {
            var result = await _querier.QueryAsync("server_ban_exception.del", new { name }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // GetAll returns a list of all server ban exceptions
        public async Task<JsonElement?> GetAllAsync()
        {
            var result = await _querier.QueryAsync("server_ban_exception.list", null, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("list", out var listElement))
            {
                return listElement;
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get gets a specific server ban exception
        public async Task<JsonElement?> GetAsync(string name)
        {
            var result = await _querier.QueryAsync("server_ban_exception.get", new { name }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null; // not found
        }
    }
}
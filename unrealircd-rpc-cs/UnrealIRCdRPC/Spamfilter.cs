using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace UnrealIRCdRPC
{
    // Spamfilter handles spamfilter operations
    public class Spamfilter
    {
        private readonly IQuerier _querier;

        public Spamfilter(IQuerier querier)
        {
            _querier = querier;
        }

        // Add adds a spamfilter
        public async Task<JsonElement?> AddAsync(string name, string matchType, string spamfilterTargets, string banAction, string banDuration, string reason)
        {
            var result = await _querier.QueryAsync("spamfilter.add", new
            {
                name,
                match_type = matchType,
                spamfilter_targets = spamfilterTargets,
                ban_action = banAction,
                ban_duration = banDuration,
                reason
            }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // Delete deletes a spamfilter
        public async Task<JsonElement?> DeleteAsync(string name, string matchType, string spamfilterTargets, string banAction)
        {
            var result = await _querier.QueryAsync("spamfilter.del", new
            {
                name,
                match_type = matchType,
                spamfilter_targets = spamfilterTargets,
                ban_action = banAction
            }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null;
        }

        // GetAll returns a list of all spamfilters
        public async Task<JsonElement?> GetAllAsync()
        {
            var result = await _querier.QueryAsync("spamfilter.list", null, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("list", out var listElement))
            {
                return listElement;
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get gets a specific spamfilter
        public async Task<JsonElement?> GetAsync(string name, string matchType, string spamfilterTargets, string banAction)
        {
            var result = await _querier.QueryAsync("spamfilter.get", new
            {
                name,
                match_type = matchType,
                spamfilter_targets = spamfilterTargets,
                ban_action = banAction
            }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("tkl", out var tklElement))
            {
                return tklElement;
            }
            return null; // not found
        }
    }
}
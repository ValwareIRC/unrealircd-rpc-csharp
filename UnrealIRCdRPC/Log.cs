using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace UnrealIRCdRPC
{
    // Log handles log operations
    public class Log
    {
        private readonly IQuerier _querier;

        public Log(IQuerier querier)
        {
            _querier = querier;
        }

        // Subscribe subscribes to log events
        public async Task<JsonElement?> SubscribeAsync(IEnumerable<string> sources)
        {
            return await _querier.QueryAsync("log.subscribe", new { sources }, false);
        }

        // Unsubscribe unsubscribes from log events
        public async Task<JsonElement?> UnsubscribeAsync()
        {
            return await _querier.QueryAsync("log.unsubscribe", null, false);
        }

        // GetAll gets log entries
        public async Task<JsonElement?> GetAllAsync(IEnumerable<string> sources)
        {
            return await _querier.QueryAsync("log.get", new { sources }, false);
        }
    }
}
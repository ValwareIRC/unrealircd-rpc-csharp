using System.Threading.Tasks;
using System.Text.Json;

namespace UnrealIRCdRPC
{
    // Stats handles statistical information
    public class Stats
    {
        private readonly IQuerier _querier;

        public Stats(IQuerier querier)
        {
            _querier = querier;
        }

        // Get gets basic statistical information: user counts, channel counts, etc.
        public async Task<JsonElement?> GetAsync(int objectDetailLevel)
        {
            return await _querier.QueryAsync("stats.get", new { object_detail_level = objectDetailLevel }, false);
        }
    }
}
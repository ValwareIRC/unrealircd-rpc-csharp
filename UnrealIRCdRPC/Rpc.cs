using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace UnrealIRCdRPC
{
    // Rpc handles RPC meta operations
    public class Rpc
    {
        private readonly IQuerier _querier;

        public Rpc(IQuerier querier)
        {
            _querier = querier;
        }

        // Info gets information on all RPC modules loaded
        public async Task<JsonElement?> InfoAsync()
        {
            return await _querier.QueryAsync("rpc.info", null, false);
        }

        // SetIssuer sets the name of the issuer (requires UnrealIRCd 6.0.8+)
        public async Task<JsonElement?> SetIssuerAsync(string name)
        {
            return await _querier.QueryAsync("rpc.set_issuer", new { name }, false);
        }

        // AddTimer adds a timer (requires UnrealIRCd 6.1.0+)
        public async Task<JsonElement?> AddTimerAsync(string timerId, int everyMsec, string method, object? parameters, object? id = null)
        {
            if (id == null)
            {
                // Generate a random ID above regular query IDs
                var random = new Random();
                id = 100000 + random.Next(900000);
            }

            var request = new
            {
                jsonrpc = "2.0",
                method,
                @params = parameters,
                id
            };

            return await _querier.QueryAsync("rpc.add_timer", new
            {
                timer_id = timerId,
                every_msec = everyMsec,
                request
            }, false);
        }

        // DelTimer deletes a timer (requires UnrealIRCd 6.1.0+)
        public async Task<JsonElement?> DelTimerAsync(string timerId)
        {
            return await _querier.QueryAsync("rpc.del_timer", new { timer_id = timerId }, false);
        }
    }
}
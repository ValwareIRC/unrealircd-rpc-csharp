using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using UnrealIRCdRPC.Models;

namespace UnrealIRCdRPC
{
    // User handles user-related operations
    public class User
    {
        private readonly IQuerier _querier;

        public User(IQuerier querier)
        {
            _querier = querier;
        }

        // GetAll returns a list of all users
        public async Task<IReadOnlyList<string>> GetAllAsync(int objectDetailLevel)
        {
            var result = await _querier.QueryAsync("user.list", new { object_detail_level = objectDetailLevel }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object)
            {
                if (result.Value.TryGetProperty("list", out var listElement) && listElement.ValueKind == JsonValueKind.Array)
                {
                    var userNames = new List<string>();
                    foreach (var userElement in listElement.EnumerateArray())
                    {
                        if (userElement.ValueKind == JsonValueKind.String)
                        {
                            // Server returns simple strings
                            userNames.Add(userElement.GetString()!);
                        }
                        else if (userElement.ValueKind == JsonValueKind.Object &&
                        userElement.TryGetProperty("name", out var nameElement) &&
                        nameElement.ValueKind == JsonValueKind.String)
                {
                    // Server returns user objects with name field
                    userNames.Add(nameElement.GetString()!);
                }
                    }
                    return userNames;
                }
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get returns a user object
        public async Task<ClientInfo?> GetAsync(string nick, int objectDetailLevel)
        {
            var result = await _querier.QueryAsync("user.get", new { nick, object_detail_level = objectDetailLevel }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("client", out var clientElement))
            {
                // Convert the JsonElement to ClientInfo using JSON deserialization
                return clientElement.Deserialize<ClientInfo>();
            }
            return null; // not found
        }

                // SetNick sets the nickname of a user (changes the nick)
        public async Task<JsonElement?> SetNickAsync(string nick, string newnick)
        {
            return await _querier.QueryAsync("user.set_nick", new { nick, newnick }, false);
        }

        // SetUsername sets the username/ident of a user
        public async Task<JsonElement?> SetUsernameAsync(string nick, string username)
        {
            return await _querier.QueryAsync("user.set_username", new { nick, username }, false);
        }

        // SetRealname sets the realname/gecos of a user
        public async Task<JsonElement?> SetRealnameAsync(string nick, string realname)
        {
            return await _querier.QueryAsync("user.set_realname", new { nick, realname }, false);
        }

        // SetVhost sets a virtual host (vhost) on the user
        public async Task<JsonElement?> SetVhostAsync(string nick, string vhost)
        {
            return await _querier.QueryAsync("user.set_vhost", new { nick, vhost }, false);
        }

        // SetMode sets and unsets modes on a user
        public async Task<JsonElement?> SetModeAsync(string nick, string mode, bool hidden)
        {
            return await _querier.QueryAsync("user.set_mode", new { nick, mode, hidden }, false);
        }

        // SetSnoMask sets and unsets snomasks on a user
        public async Task<JsonElement?> SetSnoMaskAsync(string nick, string snomask, bool hidden)
        {
            return await _querier.QueryAsync("user.set_snomask", new { nick, snomask, hidden }, false);
        }

        // SetOper sets oper status on a user
        public async Task<JsonElement?> SetOperAsync(string nick, string operAccount, string operClass, string? @class = null, string? modes = null, string? snomask = null, string? vhost = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "nick", nick },
                { "oper_account", operAccount },
                { "oper_class", operClass }
            };
            if (@class != null) parameters["class"] = @class;
            if (modes != null) parameters["modes"] = modes;
            if (snomask != null) parameters["snomask"] = snomask;
            if (vhost != null) parameters["vhost"] = vhost;
            return await _querier.QueryAsync("user.set_oper", parameters, false);
        }
        public async Task<JsonElement?> JoinAsync(string nick, string channel, string? key = null, bool force = false)
        {
            var parameters = new Dictionary<string, object>
            {
                { "nick", nick },
                { "channel", channel },
                { "force", force }
            };
            if (key != null) parameters["key"] = key;
            return await _querier.QueryAsync("user.join", parameters, false);
        }

        // Part parts a user from a channel
        public async Task<JsonElement?> PartAsync(string nick, string channel, bool force = false)
        {
            return await _querier.QueryAsync("user.part", new { nick, channel, force }, false);
        }

        // Quit quits a user from IRC. Pretends it is a normal QUIT
        public async Task<JsonElement?> QuitAsync(string nick, string reason)
        {
            return await _querier.QueryAsync("user.quit", new { nick, reason }, false);
        }

        // Kill kills a user from IRC. Shows that the user is forcefully removed
        public async Task<JsonElement?> KillAsync(string nick, string reason)
        {
            return await _querier.QueryAsync("user.kill", new { nick, reason }, false);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using UnrealIRCdRPC.Models;

namespace UnrealIRCdRPC
{
    // Channel handles channel-related operations
    public class Channel
    {
        private readonly IQuerier _querier;

        public Channel(IQuerier querier)
        {
            _querier = querier;
        }

        // GetAll returns a list of channels users
        public async Task<IReadOnlyList<string>> GetAllAsync(int objectDetailLevel)
        {
            var result = await _querier.QueryAsync("channel.list", new { object_detail_level = objectDetailLevel }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object)
            {
                if (result.Value.TryGetProperty("list", out var listElement) && listElement.ValueKind == JsonValueKind.Array)
                {
                    var channelNames = new List<string>();
                    foreach (var channelElement in listElement.EnumerateArray())
                    {
                        if (channelElement.ValueKind == JsonValueKind.String)
                        {
                            // Server returns simple strings
                            channelNames.Add(channelElement.GetString()!);
                        }
                        else if (channelElement.ValueKind == JsonValueKind.Object &&
                                channelElement.TryGetProperty("name", out var nameElement) &&
                                nameElement.ValueKind == JsonValueKind.String)
                        {
                            // Server returns channel objects with name field
                            channelNames.Add(nameElement.GetString()!);
                        }
                    }
                    return channelNames;
                }
            }
            throw new Exception("Invalid JSON Response from UnrealIRCd RPC");
        }

        // Get gets a channel object
        public async Task<ChannelInfo?> GetAsync(string channel, int objectDetailLevel)
        {
            var result = await _querier.QueryAsync("channel.get", new { channel, object_detail_level = objectDetailLevel }, false);
            if (result.HasValue && result.Value.ValueKind == JsonValueKind.Object &&
                result.Value.TryGetProperty("channel", out var channelElement))
            {
                // Convert the JsonElement to ChannelInfo using JSON deserialization
                return channelElement.Deserialize<ChannelInfo>();
            }
            return null; // not found
        }

        // SetMode sets and unsets modes on a channel
        public async Task<JsonElement?> SetModeAsync(string channel, string modes, string parameters)
        {
            return await _querier.QueryAsync("channel.set_mode", new { channel, modes, parameters }, false);
        }

        // SetTopic sets the channel topic
        public async Task<JsonElement?> SetTopicAsync(string channel, string topic, string? setBy = null, string? setAt = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "channel", channel },
                { "topic", topic }
            };
            if (setBy != null) parameters["set_by"] = setBy;
            if (setAt != null) parameters["set_at"] = setAt;
            return await _querier.QueryAsync("channel.set_topic", parameters, false);
        }

        // Kick kicks a user from the channel
        public async Task<JsonElement?> KickAsync(string channel, string nick, string reason)
        {
            return await _querier.QueryAsync("channel.kick", new { nick, channel, reason }, false);
        }
    }
}
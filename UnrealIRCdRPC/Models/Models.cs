using System.Text.Json.Serialization;

namespace UnrealIRCdRPC.Models
{
    /// <summary>
    /// Represents a server in the IRC network
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// The server name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Additional server properties based on detail level
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }

    /// <summary>
    /// Represents a client/user connected to the IRC network
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// The user's nickname
        /// </summary>
        [JsonPropertyName("nick")]
        public string? Nick { get; set; }

        /// <summary>
        /// Additional client properties based on detail level
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }

    /// <summary>
    /// Represents a channel on the IRC network
    /// </summary>
    public class ChannelInfo
    {
        /// <summary>
        /// The channel name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Additional channel properties based on detail level
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }

    /// <summary>
    /// Represents a TKL (ban/exception) entry
    /// </summary>
    public class TklInfo
    {
        /// <summary>
        /// The type of TKL (G = gline, Z = gzline, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// The ban mask or name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Additional TKL properties
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }
    }
}
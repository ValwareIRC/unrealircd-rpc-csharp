using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UnrealIRCdRPC
{
    // Querier interface for RPC calls
    public interface IQuerier
    {
        Task<JsonElement?> QueryAsync(string method, object? parameters, bool noWait);
    }

    // Connection represents a connection to the UnrealIRCd RPC server
    public class Connection : IQuerier, IDisposable
    {
        private readonly ClientWebSocket _webSocket;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private int _errno;
        private Exception? _error;
        private int _nextId;
        private readonly object _idLock = new object();

        // Options for connecting to the RPC server
        public class Options
        {
            public bool TlsVerify { get; set; } = true;
            public string? Issuer { get; set; }
        }

        // Constructor
        private Connection(ClientWebSocket webSocket)
        {
            _webSocket = webSocket;
            _nextId = 1;
        }

        // NewConnection creates a new connection to the UnrealIRCd RPC server
        public static async Task<Connection> NewConnectionAsync(string uri, string apiLogin, Options? options = null, CancellationToken cancellationToken = default)
        {
            options ??= new Options();

            var webSocket = new ClientWebSocket();

            // Set up headers for authentication
            webSocket.Options.SetRequestHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(apiLogin)));

            if (!options.TlsVerify)
            {
                // Note: In .NET, skipping TLS verification requires a custom handler, but for simplicity, we'll assume it's handled elsewhere
                // For production, you might need to use HttpClient with custom validation
            }

            var uriObj = new Uri(uri);
            await webSocket.ConnectAsync(uriObj, cancellationToken);

            var connection = new Connection(webSocket);

            if (!string.IsNullOrEmpty(options.Issuer))
            {
                // Set issuer asynchronously
                _ = connection.QueryAsync("rpc.set_issuer", new { name = options.Issuer }, true);
            }
            // Ping-pong is handled automatically by ClientWebSocket

            return connection;
        }

        // Query sends a JSON-RPC request and waits for the response
        public async Task<JsonElement?> QueryAsync(string method, object? parameters, bool noWait)
        {
            int id;
            lock (_idLock)
            {
                id = _nextId++;
            }

            var request = new
            {
                jsonrpc = "2.0",
                method,
                @params = parameters,
                id
            };

            var json = JsonSerializer.Serialize(request);
            var data = Encoding.UTF8.GetBytes(json);

            await _sendLock.WaitAsync();
            try
            {
                await _webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            finally
            {
                _sendLock.Release();
            }

            if (noWait)
            {
                return null;
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var buffer = new byte[4096];
            var segment = new ArraySegment<byte>(buffer);

            while (!cts.Token.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                await _sendLock.WaitAsync();
                try
                {
                    result = await _webSocket.ReceiveAsync(segment, cts.Token);
                }
                finally
                {
                    _sendLock.Release();
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new Exception("WebSocket closed");
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var response = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

                if (response != null && response.TryGetValue("id", out var respIdObj) && respIdObj is JsonElement respIdElement && respIdElement.TryGetInt32(out var respId) && respId == id)
                {
                    if (response.TryGetValue("result", out var resultObj))
                    {
                        _errno = 0;
                        _error = null;
                        return (JsonElement)resultObj;
                    }
                    if (response.TryGetValue("error", out var errorObj) && errorObj is JsonElement errorElement)
                    {
                        var errorDict = JsonSerializer.Deserialize<Dictionary<string, object>>(errorElement.GetRawText());
                        if (errorDict != null)
                        {
                            if (errorDict.TryGetValue("code", out var codeObj) && codeObj is JsonElement codeElement && codeElement.TryGetInt32(out var code))
                            {
                                _errno = code;
                            }
                            if (errorDict.TryGetValue("message", out var msgObj) && msgObj is JsonElement msgElement)
                            {
                                _error = new Exception(msgElement.GetString());
                            }
                        }
                        throw _error ?? new Exception("Unknown error");
                    }
                }
                // Not our response, continue (for streaming events)
            }

            throw new TimeoutException("RPC request timed out");
        }

        // EventLoop waits for the next event (used for log streaming)
        public async Task<object?> EventLoopAsync()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var buffer = new byte[4096];
            var segment = new ArraySegment<byte>(buffer);

            WebSocketReceiveResult result;
            await _sendLock.WaitAsync();
            try
            {
                result = await _webSocket.ReceiveAsync(segment, cts.Token);
            }
            catch (OperationCanceledException)
            {
                return null; // timeout
            }
            finally
            {
                _sendLock.Release();
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                throw new Exception("WebSocket closed");
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

            if (response != null)
            {
                if (response.TryGetValue("result", out var resultObj))
                {
                    _errno = 0;
                    _error = null;
                    return resultObj;
                }
                if (response.TryGetValue("error", out var errorObj) && errorObj is JsonElement errorElement)
                {
                    var errorDict = JsonSerializer.Deserialize<Dictionary<string, object>>(errorElement.GetRawText());
                    if (errorDict != null)
                    {
                        if (errorDict.TryGetValue("code", out var codeObj) && codeObj is JsonElement codeElement && codeElement.TryGetInt32(out var code))
                        {
                            _errno = code;
                        }
                        if (errorDict.TryGetValue("message", out var msgObj) && msgObj is JsonElement msgElement)
                        {
                            _error = new Exception(msgElement.GetString());
                        }
                    }
                    throw _error ?? new Exception("Unknown error");
                }
            }

            throw new Exception("Invalid JSON-RPC data from UnrealIRCd: not an error and not a result");
        }

        // Errno returns the last error code
        public int Errno => _errno;

        // Error returns the last error
        public Exception? Error => _error;

        // Dispose
        public void Dispose()
        {
            _webSocket?.Dispose();
            _sendLock?.Dispose();
        }

        // Module handlers
        public Rpc Rpc() => new Rpc(this);
        public Stats Stats() => new Stats(this);
        public User User() => new User(this);
        public Channel Channel() => new Channel(this);
        public ServerBan ServerBan() => new ServerBan(this);
        public Spamfilter Spamfilter() => new Spamfilter(this);
        public NameBan NameBan() => new NameBan(this);
        public Server Server() => new Server(this);
        public Log Log() => new Log(this);
        public ServerBanException ServerBanException() => new ServerBanException(this);
    }
}
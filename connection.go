package unrealircd

import (
	"crypto/tls"
	"encoding/base64"
	"encoding/json"
	"errors"
	"net/http"
	"sync"
	"time"

	"github.com/gorilla/websocket"
)

// Querier interface for RPC calls
type Querier interface {
	Query(method string, params interface{}, noWait bool) (interface{}, error)
}

// Connection represents a connection to the UnrealIRCd RPC server
type Connection struct {
	conn     *websocket.Conn
	mu       sync.Mutex
	errno    int
	err      error
	nextID   int
	idMu     sync.Mutex
}

// Options for connecting to the RPC server
type Options struct {
	Context   *http.Client
	TLSVerify bool
	Issuer    string
}

// NewConnection creates a new connection to the UnrealIRCd RPC server
func NewConnection(uri, apiLogin string, options *Options) (*Connection, error) {
	if options == nil {
		options = &Options{}
	}

	dialer := websocket.Dialer{
		HandshakeTimeout: 10 * time.Second,
	}

	if options.Context != nil {
		dialer.NetDialContext = options.Context.Transport.(*http.Transport).DialContext
	}

	if !options.TLSVerify {
		dialer.TLSClientConfig = &tls.Config{
			InsecureSkipVerify: true,
		}
	}

	header := http.Header{}
	header.Set("Authorization", "Basic "+base64.StdEncoding.EncodeToString([]byte(apiLogin)))

	conn, _, err := dialer.Dial(uri, header)
	if err != nil {
		return nil, err
	}

	c := &Connection{
		conn:   conn,
		nextID: 1,
	}

	if options.Issuer != "" {
		// Set issuer asynchronously
		go c.Query("rpc.set_issuer", map[string]interface{}{"name": options.Issuer}, true)
	} else {
		// Ping-pong
		conn.WriteMessage(websocket.PingMessage, []byte{})
	}

	return c, nil
}

// Query sends a JSON-RPC request and waits for the response
func (c *Connection) Query(method string, params interface{}, noWait bool) (interface{}, error) {
	c.idMu.Lock()
	id := c.nextID
	c.nextID++
	c.idMu.Unlock()

	req := map[string]interface{}{
		"jsonrpc": "2.0",
		"method":  method,
		"params":  params,
		"id":      id,
	}

	data, err := json.Marshal(req)
	if err != nil {
		return nil, err
	}

	c.mu.Lock()
	err = c.conn.WriteMessage(websocket.TextMessage, data)
	c.mu.Unlock()
	if err != nil {
		return nil, err
	}

	if noWait {
		return true, nil
	}

	timeout := time.After(10 * time.Second)
	for {
		select {
		case <-timeout:
			return nil, errors.New("RPC request timed out")
		default:
			c.mu.Lock()
			c.conn.SetReadDeadline(time.Now().Add(10 * time.Second))
			_, message, err := c.conn.ReadMessage()
			c.mu.Unlock()
			if err != nil {
				return nil, err
			}

			var resp map[string]interface{}
			if err := json.Unmarshal(message, &resp); err != nil {
				return nil, err
			}

			if respID, ok := resp["id"].(float64); ok && int(respID) == id {
				if result, ok := resp["result"]; ok {
					c.errno = 0
					c.err = nil
					return result, nil
				}
				if errObj, ok := resp["error"].(map[string]interface{}); ok {
					if code, ok := errObj["code"].(float64); ok {
						c.errno = int(code)
					}
					if msg, ok := errObj["message"].(string); ok {
						c.err = errors.New(msg)
					}
					return nil, c.err
				}
			}
			// Not our response, continue (for streaming events)
		}
	}
}

// EventLoop waits for the next event (used for log streaming)
func (c *Connection) EventLoop() (interface{}, error) {
	c.mu.Lock()
	c.conn.SetReadDeadline(time.Now().Add(2 * time.Second))
	_, message, err := c.conn.ReadMessage()
	c.mu.Unlock()
	if err != nil {
		if netErr, ok := err.(interface{ Timeout() bool }); ok && netErr.Timeout() {
			return nil, nil // timeout, can retry
		}
		return nil, err
	}

	var resp map[string]interface{}
	if err := json.Unmarshal(message, &resp); err != nil {
		return nil, err
	}

	if result, ok := resp["result"]; ok {
		c.errno = 0
		c.err = nil
		return result, nil
	}

	if errObj, ok := resp["error"].(map[string]interface{}); ok {
		if code, ok := errObj["code"].(float64); ok {
			c.errno = int(code)
		}
		if msg, ok := errObj["message"].(string); ok {
			c.err = errors.New(msg)
		}
		return nil, c.err
	}

	return nil, errors.New("Invalid JSON-RPC data from UnrealIRCd: not an error and not a result")
}

// Errno returns the last error code
func (c *Connection) Errno() int {
	return c.errno
}

// Error returns the last error
func (c *Connection) Error() error {
	return c.err
}

// Rpc returns the RPC handler
func (c *Connection) Rpc() *Rpc {
	return &Rpc{querier: c}
}

// Stats returns the stats handler
func (c *Connection) Stats() *Stats {
	return &Stats{querier: c}
}

// User returns the user handler
func (c *Connection) User() *User {
	return &User{querier: c}
}

// Channel returns the channel handler
func (c *Connection) Channel() *Channel {
	return &Channel{querier: c}
}

// ServerBan returns the server ban handler
func (c *Connection) ServerBan() *ServerBan {
	return &ServerBan{querier: c}
}

// Spamfilter returns the spamfilter handler
func (c *Connection) Spamfilter() *Spamfilter {
	return &Spamfilter{querier: c}
}

// NameBan returns the name ban handler
func (c *Connection) NameBan() *NameBan {
	return &NameBan{querier: c}
}

// Server returns the server handler
func (c *Connection) Server() *Server {
	return &Server{querier: c}
}

// Log returns the log handler
func (c *Connection) Log() *Log {
	return &Log{querier: c}
}

// ServerBanException returns the server ban exception handler
func (c *Connection) ServerBanException() *ServerBanException {
	return &ServerBanException{querier: c}
}
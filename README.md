# UnrealIRCd RPC

This allows Go programs to control [UnrealIRCd](https://www.unrealircd.org/)
via the [JSON-RPC interface](https://www.unrealircd.org/docs/JSON-RPC).

If you are interested in helping out or would like to discuss API
capabilities, join us at `#unreal-webpanel` at `irc.unrealircd.org`
(IRC with TLS on port 6697).

## Installation

```bash
go get github.com/unrealircd/unrealircd-rpc-go
```

Or, if you have a local copy:

```bash
go mod edit -replace=github.com/unrealircd/unrealircd-rpc-go=./path/to/local
go mod tidy
```

## UnrealIRCd setup

UnrealIRCd 6.0.6 or later is needed and you need to enable
[JSON-RPC](https://www.unrealircd.org/docs/JSON-RPC) in it.
After doing that, be sure to rehash the IRCd.

## Usage

For this example, create a file like `main.go` with:

```go
package main

import (
	"fmt"
	"log"
	"os"

	"github.com/unrealircd/unrealircd-rpc-go"
)

func main() {
	apiLogin := os.Getenv("UNREALIRCD_API_LOGIN")
	if apiLogin == "" {
		log.Fatal("UNREALIRCD_API_LOGIN environment variable must be set")
	}

	wsURL := os.Getenv("UNREALIRCD_WS_URL")
	if wsURL == "" {
		wsURL = "wss://127.0.0.1:8600/" // default
	}

	conn, err := unrealircd.NewConnection(wsURL, apiLogin, &unrealircd.Options{
		TLSVerify: false,
	})
	if err != nil {
		log.Fatal(err)
	}

	bans, err := conn.ServerBan().GetAll()
	if err != nil {
		log.Fatal(err)
	}
	for _, ban := range bans.([]interface{}) {
		banMap := ban.(map[string]interface{})
		fmt.Printf("There's a %s on %s\n", banMap["type"], banMap["name"])
	}

	users, err := conn.User().GetAll(2)
	if err != nil {
		log.Fatal(err)
	}
	for _, user := range users.([]interface{}) {
		userMap := user.(map[string]interface{})
		fmt.Printf("User %s\n", userMap["name"])
	}

	channels, err := conn.Channel().GetAll(1)
	if err != nil {
		log.Fatal(err)
	}
	for _, channel := range channels.([]interface{}) {
		channelMap := channel.(map[string]interface{})
		fmt.Printf("Channel %s (%d user[s])\n", channelMap["name"], channelMap["num_users"])
	}
}
```

Then, run it with `go run main.go`

If the example does not work, then make sure you have configured your
UnrealIRCd correctly, with the same API username and password you use
here, with an allowed IP, and changing the `wss://127.0.0.1:8600/` too
if needed.

## Environment Variables

The library supports configuration via environment variables:

- `UNREALIRCD_API_LOGIN`: API credentials in the format `username:password` (required)
- `UNREALIRCD_WS_URL`: WebSocket URL for the UnrealIRCd RPC server (optional, defaults to `wss://127.0.0.1:8600/`)

Example usage:
```bash
export UNREALIRCD_API_LOGIN="api:mySecurePassword"
export UNREALIRCD_WS_URL="wss://irc.example.com:8600/"
go run main.go
```

## Custom Queries

All the convenience methods (like `conn.User().GetAll()`) internally use the `Connection.Query()` method, which is the main wrapper for sending JSON-RPC requests to UnrealIRCd.

You can use `Connection.Query()` directly for any RPC method that isn't covered by the convenience methods, or for custom implementations:

```go
// Example: Get server uptime (custom query)
uptime, err := conn.Query("server.get", map[string]interface{}{
    "server": "irc.example.com",
}, false)
if err != nil {
    log.Fatal(err)
}

// Example: Send a raw JSON-RPC request
result, err := conn.Query("stats.get", map[string]interface{}{
    "object_detail_level": 2,
}, false)

// Example: Asynchronous query (no wait for response)
err = conn.Query("log.subscribe", map[string]interface{}{
    "sources": []string{"opers", "errors"},
}, true) // true = noWait
```

The `Query` method parameters are:
- `method`: The JSON-RPC method name (string)
- `params`: Parameters for the method (map[string]interface{} or nil)
- `noWait`: If true, sends the request but doesn't wait for a response (bool)

All responses are returned as `interface{}`, which you can type-assert to the expected type (usually `map[string]interface{}` for objects or `[]interface{}` for arrays).

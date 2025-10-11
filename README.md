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

	"github.com/unrealircd/unrealircd-rpc-go"
)

func main() {
	apiLogin := "api:apiPASSWORD" // same as in the rpc-user block in UnrealIRCd

	conn, err := unrealircd.NewConnection("wss://127.0.0.1:8600/", apiLogin, &unrealircd.Options{
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

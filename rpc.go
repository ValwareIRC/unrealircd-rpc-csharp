package unrealircd

import (
	"math/rand"
	"time"
)

// Rpc handles RPC meta operations
type Rpc struct {
	querier Querier
}

// Info gets information on all RPC modules loaded
func (r *Rpc) Info() (interface{}, error) {
	return r.querier.Query("rpc.info", nil, false)
}

// SetIssuer sets the name of the issuer (requires UnrealIRCd 6.0.8+)
func (r *Rpc) SetIssuer(name string) (interface{}, error) {
	return r.querier.Query("rpc.set_issuer", map[string]interface{}{
		"name": name,
	}, false)
}

// AddTimer adds a timer (requires UnrealIRCd 6.1.0+)
func (r *Rpc) AddTimer(timerID string, everyMsec int, method string, params interface{}, id interface{}) (interface{}, error) {
	if id == nil {
		// Generate a random ID above regular query IDs
		rand.Seed(time.Now().UnixNano())
		id = 100000 + rand.Intn(900000)
	}

	request := map[string]interface{}{
		"jsonrpc": "2.0",
		"method":  method,
		"params":  params,
		"id":      id,
	}

	return r.querier.Query("rpc.add_timer", map[string]interface{}{
		"timer_id":   timerID,
		"every_msec": everyMsec,
		"request":    request,
	}, false)
}

// DelTimer deletes a timer (requires UnrealIRCd 6.1.0+)
func (r *Rpc) DelTimer(timerID string) (interface{}, error) {
	return r.querier.Query("rpc.del_timer", map[string]interface{}{
		"timer_id": timerID,
	}, false)
}
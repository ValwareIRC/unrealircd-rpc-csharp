package unrealircd

import (
	"errors"
)

// ServerBan handles server ban operations
type ServerBan struct {
	querier Querier
}

// Add adds a ban
func (sb *ServerBan) Add(name, banType, duration, reason string) (interface{}, error) {
	result, err := sb.querier.Query("server_ban.add", map[string]interface{}{
		"name":             name,
		"type":             banType,
		"reason":           reason,
		"duration_string":  duration,
	}, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if tkl, ok := res["tkl"]; ok {
			return tkl, nil
		}
	}

	return nil, nil
}

// Delete deletes a ban
func (sb *ServerBan) Delete(name, banType string) (interface{}, error) {
	result, err := sb.querier.Query("server_ban.del", map[string]interface{}{
		"name": name,
		"type": banType,
	}, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if tkl, ok := res["tkl"]; ok {
			return tkl, nil
		}
	}

	return nil, nil
}

// GetAll returns a list of all bans
func (sb *ServerBan) GetAll() (interface{}, error) {
	result, err := sb.querier.Query("server_ban.list", nil, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if list, ok := res["list"]; ok {
			return list, nil
		}
	}

	return nil, errors.New("Invalid JSON Response from UnrealIRCd RPC")
}

// Get gets a specific ban
func (sb *ServerBan) Get(name, banType string) (interface{}, error) {
	result, err := sb.querier.Query("server_ban.get", map[string]interface{}{
		"name": name,
		"type": banType,
	}, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if tkl, ok := res["tkl"]; ok {
			return tkl, nil
		}
	}

	return nil, nil // not found
}
package unrealircd

import (
	"errors"
)

// NameBan handles name ban (QLine) operations
type NameBan struct {
	querier Querier
}

// Add adds a name ban (QLine)
func (nb *NameBan) Add(name, reason string, duration, setBy *string) (interface{}, error) {
	params := map[string]interface{}{
		"name":    name,
		"reason":  reason,
		"duration_string": "0",
	}
	if duration != nil {
		params["duration_string"] = *duration
	}
	if setBy != nil {
		params["set_by"] = *setBy
	}

	result, err := nb.querier.Query("name_ban.add", params, false)
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
func (nb *NameBan) Delete(name string) (interface{}, error) {
	result, err := nb.querier.Query("name_ban.del", map[string]interface{}{
		"name": name,
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
func (nb *NameBan) GetAll() (interface{}, error) {
	result, err := nb.querier.Query("name_ban.list", nil, false)
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
func (nb *NameBan) Get(name string) (interface{}, error) {
	result, err := nb.querier.Query("name_ban.get", map[string]interface{}{
		"name": name,
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
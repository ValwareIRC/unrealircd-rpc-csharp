package unrealircd

import (
	"errors"
)

// ServerBanException handles server ban exception operations
type ServerBanException struct {
	querier Querier
}

// Add adds a ban exception
func (sbe *ServerBanException) Add(name, exceptionTypes, reason string, setBy, duration *string) (interface{}, error) {
	params := map[string]interface{}{
		"name":             name,
		"exception_types":  exceptionTypes,
		"reason":           reason,
	}
	if setBy != nil {
		params["set_by"] = *setBy
	}
	if duration != nil {
		params["duration_string"] = *duration
	}

	result, err := sbe.querier.Query("server_ban_exception.add", params, false)
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

// Delete deletes a ban exception
func (sbe *ServerBanException) Delete(name string) (interface{}, error) {
	result, err := sbe.querier.Query("server_ban_exception.del", map[string]interface{}{
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

// GetAll returns a list of all exceptions
func (sbe *ServerBanException) GetAll() (interface{}, error) {
	result, err := sbe.querier.Query("server_ban_exception.list", nil, false)
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

// Get gets a specific ban exception
func (sbe *ServerBanException) Get(name string) (interface{}, error) {
	result, err := sbe.querier.Query("server_ban_exception.get", map[string]interface{}{
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
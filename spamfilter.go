package unrealircd

import (
	"errors"
)

// Spamfilter handles spamfilter operations
type Spamfilter struct {
	querier Querier
}

// Add adds a spamfilter
func (sf *Spamfilter) Add(name, matchType, spamfilterTargets, banAction, banDuration, reason string) (interface{}, error) {
	result, err := sf.querier.Query("spamfilter.add", map[string]interface{}{
		"name":                name,
		"match_type":          matchType,
		"spamfilter_targets":  spamfilterTargets,
		"ban_action":          banAction,
		"ban_duration":        banDuration,
		"reason":              reason,
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

// Delete deletes a spamfilter
func (sf *Spamfilter) Delete(name, matchType, spamfilterTargets, banAction string) (interface{}, error) {
	result, err := sf.querier.Query("spamfilter.del", map[string]interface{}{
		"name":               name,
		"match_type":         matchType,
		"spamfilter_targets": spamfilterTargets,
		"ban_action":         banAction,
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

// GetAll returns a list of all spamfilters
func (sf *Spamfilter) GetAll() (interface{}, error) {
	result, err := sf.querier.Query("spamfilter.list", nil, false)
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

// Get gets a specific spamfilter
func (sf *Spamfilter) Get(name, matchType, spamfilterTargets, banAction string) (interface{}, error) {
	result, err := sf.querier.Query("spamfilter.get", map[string]interface{}{
		"name":               name,
		"match_type":         matchType,
		"spamfilter_targets": spamfilterTargets,
		"ban_action":         banAction,
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
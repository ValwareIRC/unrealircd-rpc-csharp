package unrealircd

// Log handles log operations
type Log struct {
	querier Querier
}

// Subscribe subscribes to log events. Any previous subscriptions are overwritten
func (l *Log) Subscribe(sources []string) (interface{}, error) {
	return l.querier.Query("log.subscribe", map[string]interface{}{
		"sources": sources,
	}, false)
}

// Unsubscribe unsubscribes from all log events
func (l *Log) Unsubscribe() (interface{}, error) {
	return l.querier.Query("log.unsubscribe", nil, false)
}

// GetAll gets past log events
func (l *Log) GetAll(sources []string) (interface{}, error) {
	params := map[string]interface{}{}
	if sources != nil {
		params["sources"] = sources
	}

	result, err := l.querier.Query("log.list", params, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if list, ok := res["list"]; ok {
			return list, nil
		}
	}

	return nil, nil
}
package unrealircd

import (
	"errors"
)

// Channel handles channel-related operations
type Channel struct {
	querier Querier
}

// GetAll returns a list of channels users
func (c *Channel) GetAll(objectDetailLevel int) (interface{}, error) {
	result, err := c.querier.Query("channel.list", map[string]interface{}{
		"object_detail_level": objectDetailLevel,
	}, false)
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

// Get gets a channel object
func (c *Channel) Get(channel string, objectDetailLevel int) (interface{}, error) {
	result, err := c.querier.Query("channel.get", map[string]interface{}{
		"channel":             channel,
		"object_detail_level": objectDetailLevel,
	}, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if ch, ok := res["channel"]; ok {
			return ch, nil
		}
	}

	return nil, nil // not found
}

// SetMode sets and unsets modes on a channel
func (c *Channel) SetMode(channel, modes, parameters string) (interface{}, error) {
	return c.querier.Query("channel.set_mode", map[string]interface{}{
		"channel":    channel,
		"modes":      modes,
		"parameters": parameters,
	}, false)
}

// SetTopic sets the channel topic
func (c *Channel) SetTopic(channel, topic string, setBy, setAt *string) (interface{}, error) {
	params := map[string]interface{}{
		"channel": channel,
		"topic":   topic,
	}
	if setBy != nil {
		params["set_by"] = *setBy
	}
	if setAt != nil {
		params["set_at"] = *setAt
	}
	return c.querier.Query("channel.set_topic", params, false)
}

// Kick kicks a user from the channel
func (c *Channel) Kick(channel, nick, reason string) (interface{}, error) {
	return c.querier.Query("channel.kick", map[string]interface{}{
		"nick":    nick,
		"channel": channel,
		"reason":  reason,
	}, false)
}
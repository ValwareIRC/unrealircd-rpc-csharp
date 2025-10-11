package unrealircd

import (
	"errors"
)

// User handles user-related operations
type User struct {
	querier Querier
}

// GetAll returns a list of all users
func (u *User) GetAll(objectDetailLevel int) (interface{}, error) {
	result, err := u.querier.Query("user.list", map[string]interface{}{
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

// Get returns a user object
func (u *User) Get(nick string, objectDetailLevel int) (interface{}, error) {
	result, err := u.querier.Query("user.get", map[string]interface{}{
		"nick":                nick,
		"object_detail_level": objectDetailLevel,
	}, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if client, ok := res["client"]; ok {
			return client, nil
		}
	}

	return nil, nil // not found
}

// SetNick sets the nickname of a user (changes the nick)
func (u *User) SetNick(nick, newnick string) (interface{}, error) {
	return u.querier.Query("user.set_nick", map[string]interface{}{
		"nick":    nick,
		"newnick": newnick,
	}, false)
}

// SetUsername sets the username/ident of a user
func (u *User) SetUsername(nick, username string) (interface{}, error) {
	return u.querier.Query("user.set_username", map[string]interface{}{
		"nick":     nick,
		"username": username,
	}, false)
}

// SetRealname sets the realname/gecos of a user
func (u *User) SetRealname(nick, realname string) (interface{}, error) {
	return u.querier.Query("user.set_realname", map[string]interface{}{
		"nick":     nick,
		"realname": realname,
	}, false)
}

// SetVhost sets a virtual host (vhost) on the user
func (u *User) SetVhost(nick, vhost string) (interface{}, error) {
	return u.querier.Query("user.set_vhost", map[string]interface{}{
		"nick":  nick,
		"vhost": vhost,
	}, false)
}

// SetMode changes the user modes of a user
func (u *User) SetMode(nick, mode string, hidden bool) (interface{}, error) {
	return u.querier.Query("user.set_mode", map[string]interface{}{
		"nick":   nick,
		"modes":  mode,
		"hidden": hidden,
	}, false)
}

// SetSnoMask changes the snomask of a user (oper)
func (u *User) SetSnoMask(nick, snomask string, hidden bool) (interface{}, error) {
	return u.querier.Query("user.set_snomask", map[string]interface{}{
		"nick":    nick,
		"snomask": snomask,
		"hidden":  hidden,
	}, false)
}

// SetOper makes user an IRC Operator (oper)
func (u *User) SetOper(nick, operAccount, operClass string, class, modes, snomask, vhost *string) (interface{}, error) {
	params := map[string]interface{}{
		"nick":         nick,
		"oper_account": operAccount,
		"oper_class":   operClass,
	}
	if class != nil {
		params["class"] = *class
	}
	if modes != nil {
		params["modes"] = *modes
	}
	if snomask != nil {
		params["snomask"] = *snomask
	}
	if vhost != nil {
		params["vhost"] = *vhost
	}
	return u.querier.Query("user.set_oper", params, false)
}

// Join joins a user to a channel
func (u *User) Join(nick, channel string, key *string, force bool) (interface{}, error) {
	params := map[string]interface{}{
		"nick":    nick,
		"channel": channel,
		"force":   force,
	}
	if key != nil {
		params["key"] = *key
	}
	return u.querier.Query("user.join", params, false)
}

// Part parts a user from a channel
func (u *User) Part(nick, channel string, force bool) (interface{}, error) {
	return u.querier.Query("user.part", map[string]interface{}{
		"nick":    nick,
		"channel": channel,
		"force":   force,
	}, false)
}

// Quit quits a user from IRC. Pretends it is a normal QUIT
func (u *User) Quit(nick, reason string) (interface{}, error) {
	return u.querier.Query("user.quit", map[string]interface{}{
		"nick":   nick,
		"reason": reason,
	}, false)
}

// Kill kills a user from IRC. Shows that the user is forcefully removed
func (u *User) Kill(nick, reason string) (interface{}, error) {
	return u.querier.Query("user.kill", map[string]interface{}{
		"nick":   nick,
		"reason": reason,
	}, false)
}
package unrealircd

import (
	"errors"
)

// Server handles server-related operations
type Server struct {
	querier Querier
}

// GetAll returns a list of all servers
func (s *Server) GetAll() (interface{}, error) {
	result, err := s.querier.Query("server.list", nil, false)
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

// Get returns a server object
func (s *Server) Get(server *string) (interface{}, error) {
	params := map[string]interface{}{}
	if server != nil {
		params["server"] = *server
	}

	result, err := s.querier.Query("server.get", params, false)
	if err != nil {
		return nil, err
	}

	if res, ok := result.(map[string]interface{}); ok {
		if srv, ok := res["server"]; ok {
			return srv, nil
		}
	}

	return nil, nil // not found
}
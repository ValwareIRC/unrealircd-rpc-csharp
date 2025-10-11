package unrealircd

import (
	"testing"
)

func TestServer_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server.list",
		response: map[string]interface{}{
			"list": []interface{}{"server1", "server2"},
		},
	}
	s := &Server{querier: mock}

	result, err := s.GetAll()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"server1", "server2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestServer_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server.get",
		response: map[string]interface{}{
			"server": map[string]interface{}{"name": "irc.example.com"},
		},
	}
	s := &Server{querier: mock}

	serverName := "irc.example.com"
	result, err := s.Get(&serverName)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected server object, got nil")
	}
}
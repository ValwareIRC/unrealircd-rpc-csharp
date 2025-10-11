package unrealircd

import (
	"testing"
)

func TestServerBanException_Add(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban_exception.add",
		response: map[string]interface{}{
			"tkl": "added",
		},
	}
	sbe := &ServerBanException{querier: mock}

	setBy := "admin"
	result, err := sbe.Add("goodhost", "G", "reason", &setBy, nil)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "added" {
		t.Errorf("Expected 'added', got %v", result)
	}
}

func TestServerBanException_Delete(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban_exception.del",
		response: map[string]interface{}{
			"tkl": "deleted",
		},
	}
	sbe := &ServerBanException{querier: mock}

	result, err := sbe.Delete("goodhost")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "deleted" {
		t.Errorf("Expected 'deleted', got %v", result)
	}
}

func TestServerBanException_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban_exception.list",
		response: map[string]interface{}{
			"list": []interface{}{"exc1", "exc2"},
		},
	}
	sbe := &ServerBanException{querier: mock}

	result, err := sbe.GetAll()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"exc1", "exc2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestServerBanException_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban_exception.get",
		response: map[string]interface{}{
			"tkl": "exception",
		},
	}
	sbe := &ServerBanException{querier: mock}

	result, err := sbe.Get("goodhost")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "exception" {
		t.Errorf("Expected 'exception', got %v", result)
	}
}
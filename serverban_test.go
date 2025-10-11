package unrealircd

import (
	"testing"
)

func TestServerBan_Add(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban.add",
		response: map[string]interface{}{
			"tkl": map[string]interface{}{"type": "G", "name": "*@badhost"},
		},
	}
	sb := &ServerBan{querier: mock}

	result, err := sb.Add("badhost", "G", "1d", "reason")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected tkl object, got nil")
	}
}

func TestServerBan_Delete(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban.del",
		response: map[string]interface{}{
			"tkl": "deleted",
		},
	}
	sb := &ServerBan{querier: mock}

	result, err := sb.Delete("badhost", "G")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "deleted" {
		t.Errorf("Expected 'deleted', got %v", result)
	}
}

func TestServerBan_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban.list",
		response: map[string]interface{}{
			"list": []interface{}{"ban1", "ban2"},
		},
	}
	sb := &ServerBan{querier: mock}

	result, err := sb.GetAll()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"ban1", "ban2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestServerBan_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "server_ban.get",
		response: map[string]interface{}{
			"tkl": map[string]interface{}{"name": "badhost"},
		},
	}
	sb := &ServerBan{querier: mock}

	result, err := sb.Get("badhost", "G")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected tkl object, got nil")
	}
}
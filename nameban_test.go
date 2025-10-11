package unrealircd

import (
	"testing"
)

func TestNameBan_Add(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "name_ban.add",
		response: map[string]interface{}{
			"tkl": "added",
		},
	}
	nb := &NameBan{querier: mock}

	setBy := "admin"
	result, err := nb.Add("badnick", "reason", &setBy, nil)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "added" {
		t.Errorf("Expected 'added', got %v", result)
	}
}

func TestNameBan_Delete(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "name_ban.del",
		response: map[string]interface{}{
			"tkl": "deleted",
		},
	}
	nb := &NameBan{querier: mock}

	result, err := nb.Delete("badnick")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "deleted" {
		t.Errorf("Expected 'deleted', got %v", result)
	}
}

func TestNameBan_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "name_ban.list",
		response: map[string]interface{}{
			"list": []interface{}{"ban1", "ban2"},
		},
	}
	nb := &NameBan{querier: mock}

	result, err := nb.GetAll()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"ban1", "ban2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestNameBan_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "name_ban.get",
		response: map[string]interface{}{
			"tkl": "ban",
		},
	}
	nb := &NameBan{querier: mock}

	result, err := nb.Get("badnick")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ban" {
		t.Errorf("Expected 'ban', got %v", result)
	}
}
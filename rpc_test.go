package unrealircd

import (
	"testing"
)

func TestRpc_Info(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "rpc.info",
		response:       map[string]interface{}{"modules": []string{"user", "channel"}},
	}
	r := &Rpc{querier: mock}

	result, err := r.Info()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected info object, got nil")
	}
}

func TestRpc_SetIssuer(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "rpc.set_issuer",
		response:       "ok",
	}
	r := &Rpc{querier: mock}

	result, err := r.SetIssuer("admin")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ok" {
		t.Errorf("Expected 'ok', got %v", result)
	}
}

func TestRpc_AddTimer(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "rpc.add_timer",
		response:       "timer_added",
	}
	r := &Rpc{querier: mock}

	result, err := r.AddTimer("test_timer", 1000, "stats.get", map[string]interface{}{"object_detail_level": 1}, nil)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "timer_added" {
		t.Errorf("Expected 'timer_added', got %v", result)
	}
}

func TestRpc_DelTimer(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "rpc.del_timer",
		response:       "timer_deleted",
	}
	r := &Rpc{querier: mock}

	result, err := r.DelTimer("test_timer")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "timer_deleted" {
		t.Errorf("Expected 'timer_deleted', got %v", result)
	}
}
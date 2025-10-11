package unrealircd

import (
	"testing"
)

func TestStats_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "stats.get",
		response: map[string]interface{}{
			"users":    100,
			"channels": 50,
		},
	}
	s := &Stats{querier: mock}

	result, err := s.Get(1)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected stats object, got nil")
	}
}
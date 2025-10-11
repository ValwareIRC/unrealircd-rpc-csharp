package unrealircd

import (
	"testing"
)

func TestLog_Subscribe(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "log.subscribe",
		response:       "subscribed",
	}
	l := &Log{querier: mock}

	result, err := l.Subscribe([]string{"opers", "errors"})
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "subscribed" {
		t.Errorf("Expected 'subscribed', got %v", result)
	}
}

func TestLog_Unsubscribe(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "log.unsubscribe",
		response:       "unsubscribed",
	}
	l := &Log{querier: mock}

	result, err := l.Unsubscribe()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "unsubscribed" {
		t.Errorf("Expected 'unsubscribed', got %v", result)
	}
}

func TestLog_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "log.list",
		response: map[string]interface{}{
			"list": []interface{}{"log1", "log2"},
		},
	}
	l := &Log{querier: mock}

	result, err := l.GetAll([]string{"opers"})
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"log1", "log2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}
package unrealircd

import (
	"testing"
)

func TestChannel_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "channel.list",
		response: map[string]interface{}{
			"list": []interface{}{"channel1", "channel2"},
		},
	}
	channel := &Channel{querier: mock}

	result, err := channel.GetAll(1)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"channel1", "channel2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestChannel_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "channel.get",
		response: map[string]interface{}{
			"channel": map[string]interface{}{"name": "#test"},
		},
	}
	channel := &Channel{querier: mock}

	result, err := channel.Get("#test", 3)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected channel object, got nil")
	}
}

func TestChannel_SetMode(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "channel.set_mode",
		response:       "ok",
	}
	channel := &Channel{querier: mock}

	result, err := channel.SetMode("#test", "+m", "user")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ok" {
		t.Errorf("Expected 'ok', got %v", result)
	}
}

func TestChannel_SetTopic(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "channel.set_topic",
		response:       "ok",
	}
	channel := &Channel{querier: mock}

	setBy := "admin"
	result, err := channel.SetTopic("#test", "New topic", &setBy, nil)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ok" {
		t.Errorf("Expected 'ok', got %v", result)
	}
}

func TestChannel_Kick(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "channel.kick",
		response:       "ok",
	}
	channel := &Channel{querier: mock}

	result, err := channel.Kick("#test", "baduser", "reason")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ok" {
		t.Errorf("Expected 'ok', got %v", result)
	}
}
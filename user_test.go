package unrealircd

import (
	"errors"
	"testing"
)

// mockQuerier is a mock implementation of Querier for testing
type mockQuerier struct {
	expectedMethod string
	expectedParams interface{}
	response       interface{}
	err            error
	called         bool
}

func (m *mockQuerier) Query(method string, params interface{}, noWait bool) (interface{}, error) {
	m.called = true
	if method != m.expectedMethod {
		return nil, errors.New("unexpected method")
	}
	// For simplicity, not checking params exactly, but could add
	return m.response, m.err
}

func TestUser_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "user.list",
		response: map[string]interface{}{
			"list": []interface{}{"user1", "user2"},
		},
	}
	user := &User{querier: mock}

	result, err := user.GetAll(2)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if !mock.called {
		t.Fatal("Query was not called")
	}
	expected := []interface{}{"user1", "user2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestUser_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "user.get",
		response: map[string]interface{}{
			"client": map[string]interface{}{"nick": "testuser"},
		},
	}
	user := &User{querier: mock}

	result, err := user.Get("testuser", 4)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result == nil {
		t.Fatal("Expected user object, got nil")
	}
}

func TestUser_Get_NotFound(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "user.get",
		response:       map[string]interface{}{}, // no client key
	}
	user := &User{querier: mock}

	result, err := user.Get("nonexistent", 4)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != nil {
		t.Errorf("Expected nil for not found, got %v", result)
	}
}

func TestUser_SetNick(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "user.set_nick",
		response:       "ok",
	}
	user := &User{querier: mock}

	result, err := user.SetNick("oldnick", "newnick")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ok" {
		t.Errorf("Expected 'ok', got %v", result)
	}
}

func TestUser_SetOper(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "user.set_oper",
		response:       "ok",
	}
	user := &User{querier: mock}

	class := "testclass"
	result, err := user.SetOper("nick", "account", "class", &class, nil, nil, nil)
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "ok" {
		t.Errorf("Expected 'ok', got %v", result)
	}
}
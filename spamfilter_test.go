package unrealircd

import (
	"testing"
)

func TestSpamfilter_Add(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "spamfilter.add",
		response: map[string]interface{}{
			"tkl": "added",
		},
	}
	sf := &Spamfilter{querier: mock}

	result, err := sf.Add("badword", "simple", "private", "block", "1d", "reason")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "added" {
		t.Errorf("Expected 'added', got %v", result)
	}
}

func TestSpamfilter_Delete(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "spamfilter.del",
		response: map[string]interface{}{
			"tkl": "deleted",
		},
	}
	sf := &Spamfilter{querier: mock}

	result, err := sf.Delete("badword", "simple", "private", "block")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "deleted" {
		t.Errorf("Expected 'deleted', got %v", result)
	}
}

func TestSpamfilter_GetAll(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "spamfilter.list",
		response: map[string]interface{}{
			"list": []interface{}{"filter1", "filter2"},
		},
	}
	sf := &Spamfilter{querier: mock}

	result, err := sf.GetAll()
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	expected := []interface{}{"filter1", "filter2"}
	if result.([]interface{})[0] != expected[0] {
		t.Errorf("Expected %v, got %v", expected, result)
	}
}

func TestSpamfilter_Get(t *testing.T) {
	mock := &mockQuerier{
		expectedMethod: "spamfilter.get",
		response: map[string]interface{}{
			"tkl": "filter",
		},
	}
	sf := &Spamfilter{querier: mock}

	result, err := sf.Get("badword", "simple", "private", "block")
	if err != nil {
		t.Fatalf("Expected no error, got %v", err)
	}
	if result != "filter" {
		t.Errorf("Expected 'filter', got %v", result)
	}
}
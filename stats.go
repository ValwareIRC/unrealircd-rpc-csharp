package unrealircd

// Stats handles statistical information
type Stats struct {
	querier Querier
}

// Get gets basic statistical information: user counts, channel counts, etc.
func (s *Stats) Get(objectDetailLevel int) (interface{}, error) {
	return s.querier.Query("stats.get", map[string]interface{}{
		"object_detail_level": objectDetailLevel,
	}, false)
}
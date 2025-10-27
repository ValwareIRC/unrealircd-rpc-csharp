using Moq;
using Xunit;
using UnrealIRCdRPC;
using System.Text.Json;

namespace UnrealIRCdRPC.Tests
{
    public class StatsTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Stats _stats;

        public StatsTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _stats = new Stats(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { users = 100, channels = 50 };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("stats.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _stats.GetAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("users", out var usersElement));
            Assert.Equal(100, usersElement.GetInt32());
            Assert.True(result.Value.TryGetProperty("channels", out var channelsElement));
            Assert.Equal(50, channelsElement.GetInt32());
            _mockQuerier.Verify(q => q.QueryAsync("stats.get", It.IsAny<object>(), false), Times.Once);
        }
    }
}
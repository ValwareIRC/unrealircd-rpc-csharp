using Moq;
using Xunit;
using UnrealIRCdRPC;
using System.Collections.Generic;
using System.Text.Json;

namespace UnrealIRCdRPC.Tests
{
    public class LogTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Log _log;

        public LogTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _log = new Log(_mockQuerier.Object);
        }

        [Fact]
        public async Task SubscribeAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            var sources = new List<string> { "opers", "errors" };
            _mockQuerier.Setup(q => q.QueryAsync("log.subscribe", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _log.SubscribeAsync(sources);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("success", out var successElement));
            Assert.True(successElement.GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("log.subscribe", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("log.unsubscribe", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _log.UnsubscribeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("success", out var successElement));
            Assert.True(successElement.GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("log.unsubscribe", null, false), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new[] { new { message = "log entry" } };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            var sources = new List<string> { "opers" };
            _mockQuerier.Setup(q => q.QueryAsync("log.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _log.GetAllAsync(sources);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Array, result.Value.ValueKind);
            Assert.Equal(1, result.Value.GetArrayLength());
            _mockQuerier.Verify(q => q.QueryAsync("log.get", It.IsAny<object>(), false), Times.Once);
        }
    }
}
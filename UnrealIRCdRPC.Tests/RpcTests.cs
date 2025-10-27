using Moq;
using Xunit;
using UnrealIRCdRPC;
using System.Text.Json;

namespace UnrealIRCdRPC.Tests
{
    public class RpcTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Rpc _rpc;

        public RpcTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _rpc = new Rpc(_mockQuerier.Object);
        }

        [Fact]
        public async Task InfoAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { version = "6.0.6" };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("rpc.info", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _rpc.InfoAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("version", out var versionElement));
            Assert.Equal("6.0.6", versionElement.GetString());
        }

        [Fact]
        public async Task SetIssuerAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("rpc.set_issuer", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _rpc.SetIssuerAsync("testissuer");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("success", out var successElement));
            Assert.True(successElement.GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("rpc.set_issuer", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task AddTimerAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("rpc.add_timer", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _rpc.AddTimerAsync("timer1", 5000, "user.get", new { nick = "test" }, 12345);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("success", out var successElement));
            Assert.True(successElement.GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("rpc.add_timer", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DelTimerAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            var jsonResponse = JsonSerializer.Serialize(expectedResult);
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("rpc.del_timer", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _rpc.DelTimerAsync("timer1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("success", out var successElement));
            Assert.True(successElement.GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("rpc.del_timer", It.IsAny<object>(), false), Times.Once);
        }
    }
}
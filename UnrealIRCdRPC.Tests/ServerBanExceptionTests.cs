using Moq;
using System.Text.Json;
using Xunit;
using UnrealIRCdRPC;

namespace UnrealIRCdRPC.Tests
{
    public class ServerBanExceptionTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly ServerBanException _serverBanException;

        public ServerBanExceptionTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _serverBanException = new ServerBanException(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { new { name = "exception1" }, new { name = "exception2" } };
            var jsonResponse = JsonSerializer.Serialize(new { list = expectedList });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.list", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBanException.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Array, result.Value.ValueKind);
            Assert.Equal(2, result.Value.GetArrayLength());
        }

        [Fact]
        public async Task AddAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.add", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBanException.AddAsync("gooduser", "kline", "reason", "admin", "1d");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("server_ban_exception.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.del", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBanException.DeleteAsync("gooduser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("server_ban_exception.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBanException.GetAsync("gooduser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
        }
    }
}
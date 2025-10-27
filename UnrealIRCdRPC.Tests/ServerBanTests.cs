using Moq;
using System.Text.Json;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;
using System.Linq;

namespace UnrealIRCdRPC.Tests
{
    public class ServerBanTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly ServerBan _serverBan;

        public ServerBanTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _serverBan = new ServerBan(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { "ban1", "ban2" };
            var banObjects = expectedList.Select(name => new { name }).ToArray();
            var jsonResponse = JsonSerializer.Serialize(new { list = banObjects });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.list", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBan.GetAllAsync();

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task AddAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { type = "G", name = "*@badhost" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.add", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBan.AddAsync("baduser", "kline", "1d", "spam");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("G", result.Type);
            Assert.Equal("*@badhost", result.Name);
            _mockQuerier.Verify(q => q.QueryAsync("server_ban.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { tkl = "deleted" });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.del", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBan.DeleteAsync("baduser", "kline");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.String, result.Value.ValueKind);
            Assert.Equal("deleted", result.Value.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("server_ban.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { type = "G", name = "*@badhost" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _serverBan.GetAsync("baduser", "kline");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("G", result.Type);
            Assert.Equal("*@badhost", result.Name);
        }
    }
}
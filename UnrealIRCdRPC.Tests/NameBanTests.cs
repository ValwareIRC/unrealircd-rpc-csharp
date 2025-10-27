using Moq;
using System.Text.Json;
using Xunit;
using UnrealIRCdRPC;

namespace UnrealIRCdRPC.Tests
{
    public class NameBanTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly NameBan _nameBan;

        public NameBanTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _nameBan = new NameBan(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { new { name = "badname1" }, new { name = "badname2" } };
            var jsonResponse = JsonSerializer.Serialize(new { list = expectedList });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.list", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _nameBan.GetAllAsync();

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
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.add", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _nameBan.AddAsync("badname", "reason", "1d", "admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("name_ban.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.del", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _nameBan.DeleteAsync("badname");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("name_ban.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _nameBan.GetAsync("badname");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
        }
    }
}
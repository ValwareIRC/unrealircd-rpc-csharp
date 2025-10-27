using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;
using System.Text.Json;

namespace UnrealIRCdRPC.Tests
{
    public class UserTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly User _user;

        public UserTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _user = new User(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { "user1", "user2" };
            var jsonResponse = JsonSerializer.Serialize(new { list = expectedList });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.list", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _user.GetAllAsync(2);

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_WhenInvalidResponse()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { invalid = "data" });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.list", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _user.GetAllAsync(2));
        }

        [Fact]
        public async Task GetAsync_ReturnsClient_WhenFound()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { client = new { nick = "testuser" } });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _user.GetAsync("testuser", 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Nick);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { other = "data" });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _user.GetAsync("testuser", 2);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetNickAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { success = true });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.set_nick", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _user.SetNickAsync("oldnick", "newnick");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.GetProperty("success").GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("user.set_nick", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task JoinAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { success = true });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.join", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _user.JoinAsync("nick", "channel", "key", true);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.GetProperty("success").GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("user.join", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task KillAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { success = true });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("user.kill", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));;
            // Act
            var result = await _user.KillAsync("nick", "reason");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.GetProperty("success").GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("user.kill", It.IsAny<object>(), false), Times.Once);
        }
    }
}

using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;

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
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("user.list", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _user.GetAllAsync(2);

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_WhenInvalidResponse()
        {
            // Arrange
            var response = new Dictionary<string, object> { { "invalid", "data" } };
            _mockQuerier.Setup(q => q.QueryAsync("user.list", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _user.GetAllAsync(2));
        }

        [Fact]
        public async Task GetAsync_ReturnsClient_WhenFound()
        {
            // Arrange
            var expectedClient = new { nick = "testuser" };
            var response = new Dictionary<string, object> { { "client", expectedClient } };
            _mockQuerier.Setup(q => q.QueryAsync("user.get", It.IsAny<object>(), false)).ReturnsAsync(response);

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
            var response = new Dictionary<string, object> { { "other", "data" } };
            _mockQuerier.Setup(q => q.QueryAsync("user.get", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _user.GetAsync("testuser", 2);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetNickAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("user.set_nick", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _user.SetNickAsync("oldnick", "newnick");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("user.set_nick", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task JoinAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("user.join", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _user.JoinAsync("nick", "channel", "key", true);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("user.join", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task KillAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("user.kill", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _user.KillAsync("nick", "reason");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("user.kill", It.IsAny<object>(), false), Times.Once);
        }
    }
}
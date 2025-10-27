using Moq;
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
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.list", null, false)).ReturnsAsync(response);

            // Act
            var result = await _serverBanException.GetAllAsync();

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task AddAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.add", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _serverBanException.AddAsync("gooduser", "kline", "reason", "admin", "1d");

            // Assert
            Assert.Equal(expectedTkl, result);
            _mockQuerier.Verify(q => q.QueryAsync("server_ban_exception.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.del", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _serverBanException.DeleteAsync("gooduser");

            // Assert
            Assert.Equal(expectedTkl, result);
            _mockQuerier.Verify(q => q.QueryAsync("server_ban_exception.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban_exception.get", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _serverBanException.GetAsync("gooduser");

            // Assert
            Assert.Equal(expectedTkl, result);
        }
    }
}
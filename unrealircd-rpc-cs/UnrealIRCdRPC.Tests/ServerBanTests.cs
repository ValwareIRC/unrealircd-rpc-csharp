using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;

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
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.list", null, false)).ReturnsAsync(response);

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
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.add", It.IsAny<object>(), false)).ReturnsAsync(response);

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
            var response = new Dictionary<string, object> { { "tkl", "deleted" } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.del", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _serverBan.DeleteAsync("baduser", "kline");

            // Assert
            Assert.Equal("deleted", result);
            _mockQuerier.Verify(q => q.QueryAsync("server_ban.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { type = "G", name = "*@badhost" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("server_ban.get", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _serverBan.GetAsync("baduser", "kline");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("G", result.Type);
            Assert.Equal("*@badhost", result.Name);
        }
    }
}
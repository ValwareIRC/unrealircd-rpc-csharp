using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;

namespace UnrealIRCdRPC.Tests
{
    public class ServerTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Server _server;

        public ServerTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _server = new Server(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { "server1", "server2" };
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("server.list", null, false)).ReturnsAsync(response);

            // Act
            var result = await _server.GetAllAsync();

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task GetAsync_ReturnsServer_WhenFound()
        {
            // Arrange
            var expectedServer = new { name = "testserver" };
            var response = new Dictionary<string, object> { { "server", expectedServer } };
            _mockQuerier.Setup(q => q.QueryAsync("server.get", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _server.GetAsync("testserver");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testserver", result.Name);
        }

        [Fact]
        public async Task GetAsync_CallsQueryWithNullServer_WhenNoParameter()
        {
            // Arrange
            var expectedServer = new { name = "localserver" };
            var response = new Dictionary<string, object> { { "server", expectedServer } };
            _mockQuerier.Setup(q => q.QueryAsync("server.get", null, false)).ReturnsAsync(response);

            // Act
            var result = await _server.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("localserver", result.Name);
            _mockQuerier.Verify(q => q.QueryAsync("server.get", null, false), Times.Once);
        }
    }
}
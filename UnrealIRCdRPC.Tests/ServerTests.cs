using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;
using System.Text.Json;
using System.Linq;

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
            var serverObjects = expectedList.Select(name => new { name }).ToArray();
            var jsonResponse = JsonSerializer.Serialize(new { list = serverObjects });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server.list", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _server.GetAllAsync();

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task GetAsync_ReturnsServer_WhenFound()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { server = new { name = "testserver" } });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

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
            var jsonResponse = JsonSerializer.Serialize(new { server = new { name = "localserver" } });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("server.get", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _server.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("localserver", result.Name);
            _mockQuerier.Verify(q => q.QueryAsync("server.get", null, false), Times.Once);
        }
    }
}
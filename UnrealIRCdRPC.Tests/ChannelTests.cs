using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;
using System.Text.Json;

namespace UnrealIRCdRPC.Tests
{
    public class ChannelTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Channel _channel;

        public ChannelTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _channel = new Channel(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { "#channel1", "#channel2" };
            var jsonResponse = JsonSerializer.Serialize(new { list = expectedList });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("channel.list", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _channel.GetAllAsync(1);

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task GetAsync_ReturnsChannel_WhenFound()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { channel = new { name = "#testchannel" } });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("channel.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _channel.GetAsync("#testchannel", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("#testchannel", result.Name);
        }

        [Fact]
        public async Task SetModeAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { success = true });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("channel.set_mode", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _channel.SetModeAsync("#channel", "+m", "nick");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.GetProperty("success").GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("channel.set_mode", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task SetTopicAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { success = true });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("channel.set_topic", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _channel.SetTopicAsync("#channel", "New topic", "setter", "timestamp");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.GetProperty("success").GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("channel.set_topic", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task KickAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new { success = true });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("channel.kick", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _channel.KickAsync("#channel", "nick", "reason");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.GetProperty("success").GetBoolean());
            _mockQuerier.Verify(q => q.QueryAsync("channel.kick", It.IsAny<object>(), false), Times.Once);
        }
    }
}
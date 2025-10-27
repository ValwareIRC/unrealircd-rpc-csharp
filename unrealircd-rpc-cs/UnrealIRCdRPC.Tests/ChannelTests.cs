using Moq;
using Xunit;
using UnrealIRCdRPC;
using UnrealIRCdRPC.Models;

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
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("channel.list", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _channel.GetAllAsync(1);

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task GetAsync_ReturnsChannel_WhenFound()
        {
            // Arrange
            var expectedChannel = new { name = "#testchannel" };
            var response = new Dictionary<string, object> { { "channel", expectedChannel } };
            _mockQuerier.Setup(q => q.QueryAsync("channel.get", It.IsAny<object>(), false)).ReturnsAsync(response);

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
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("channel.set_mode", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _channel.SetModeAsync("#channel", "+m", "nick");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("channel.set_mode", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task SetTopicAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("channel.set_topic", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _channel.SetTopicAsync("#channel", "New topic", "setter", "timestamp");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("channel.set_topic", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task KickAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("channel.kick", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _channel.KickAsync("#channel", "nick", "reason");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("channel.kick", It.IsAny<object>(), false), Times.Once);
        }
    }
}
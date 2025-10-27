using Moq;
using Xunit;
using UnrealIRCdRPC;
using System.Collections.Generic;

namespace UnrealIRCdRPC.Tests
{
    public class LogTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Log _log;

        public LogTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _log = new Log(_mockQuerier.Object);
        }

        [Fact]
        public async Task SubscribeAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            var sources = new List<string> { "opers", "errors" };
            _mockQuerier.Setup(q => q.QueryAsync("log.subscribe", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _log.SubscribeAsync(sources);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("log.subscribe", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("log.unsubscribe", null, false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _log.UnsubscribeAsync();

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("log.unsubscribe", null, false), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new[] { new { message = "log entry" } };
            var sources = new List<string> { "opers" };
            _mockQuerier.Setup(q => q.QueryAsync("log.get", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _log.GetAllAsync(sources);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("log.get", It.IsAny<object>(), false), Times.Once);
        }
    }
}
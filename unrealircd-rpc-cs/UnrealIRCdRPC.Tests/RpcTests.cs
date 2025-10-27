using Moq;
using Xunit;
using UnrealIRCdRPC;

namespace UnrealIRCdRPC.Tests
{
    public class RpcTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Rpc _rpc;

        public RpcTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _rpc = new Rpc(_mockQuerier.Object);
        }

        [Fact]
        public async Task InfoAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { version = "6.0.6" };
            _mockQuerier.Setup(q => q.QueryAsync("rpc.info", null, false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _rpc.InfoAsync();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task SetIssuerAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("rpc.set_issuer", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _rpc.SetIssuerAsync("testissuer");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("rpc.set_issuer", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task AddTimerAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("rpc.add_timer", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _rpc.AddTimerAsync("timer1", 5000, "user.get", new { nick = "test" }, 12345);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("rpc.add_timer", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DelTimerAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { success = true };
            _mockQuerier.Setup(q => q.QueryAsync("rpc.del_timer", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _rpc.DelTimerAsync("timer1");

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("rpc.del_timer", It.IsAny<object>(), false), Times.Once);
        }
    }
}
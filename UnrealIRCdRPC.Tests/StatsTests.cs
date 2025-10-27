using Moq;
using Xunit;
using UnrealIRCdRPC;

namespace UnrealIRCdRPC.Tests
{
    public class StatsTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Stats _stats;

        public StatsTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _stats = new Stats(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedResult = new { users = 100, channels = 50 };
            _mockQuerier.Setup(q => q.QueryAsync("stats.get", It.IsAny<object>(), false)).ReturnsAsync(expectedResult);

            // Act
            var result = await _stats.GetAsync(2);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockQuerier.Verify(q => q.QueryAsync("stats.get", It.IsAny<object>(), false), Times.Once);
        }
    }
}
using Moq;
using Xunit;
using UnrealIRCdRPC;

namespace UnrealIRCdRPC.Tests
{
    public class SpamfilterTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly Spamfilter _spamfilter;

        public SpamfilterTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _spamfilter = new Spamfilter(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { new { name = "spam1" }, new { name = "spam2" } };
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.list", null, false)).ReturnsAsync(response);

            // Act
            var result = await _spamfilter.GetAllAsync();

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task AddAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.add", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _spamfilter.AddAsync("badword", "simple", "private", "block", "1h", "spam");

            // Assert
            Assert.Equal(expectedTkl, result);
            _mockQuerier.Verify(q => q.QueryAsync("spamfilter.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.del", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _spamfilter.DeleteAsync("badword", "simple", "private", "block");

            // Assert
            Assert.Equal(expectedTkl, result);
            _mockQuerier.Verify(q => q.QueryAsync("spamfilter.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.get", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _spamfilter.GetAsync("badword", "simple", "private", "block");

            // Assert
            Assert.Equal(expectedTkl, result);
        }
    }
}
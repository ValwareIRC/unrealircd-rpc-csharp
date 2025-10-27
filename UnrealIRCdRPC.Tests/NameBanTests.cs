using Moq;
using Xunit;
using UnrealIRCdRPC;

namespace UnrealIRCdRPC.Tests
{
    public class NameBanTests
    {
        private readonly Mock<IQuerier> _mockQuerier;
        private readonly NameBan _nameBan;

        public NameBanTests()
        {
            _mockQuerier = new Mock<IQuerier>();
            _nameBan = new NameBan(_mockQuerier.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList_WhenValidResponse()
        {
            // Arrange
            var expectedList = new[] { new { name = "badname1" }, new { name = "badname2" } };
            var response = new Dictionary<string, object> { { "list", expectedList } };
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.list", null, false)).ReturnsAsync(response);

            // Act
            var result = await _nameBan.GetAllAsync();

            // Assert
            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async Task AddAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.add", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _nameBan.AddAsync("badname", "reason", "1d", "admin");

            // Assert
            Assert.Equal(expectedTkl, result);
            _mockQuerier.Verify(q => q.QueryAsync("name_ban.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.del", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _nameBan.DeleteAsync("badname");

            // Assert
            Assert.Equal(expectedTkl, result);
            _mockQuerier.Verify(q => q.QueryAsync("name_ban.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var response = new Dictionary<string, object> { { "tkl", expectedTkl } };
            _mockQuerier.Setup(q => q.QueryAsync("name_ban.get", It.IsAny<object>(), false)).ReturnsAsync(response);

            // Act
            var result = await _nameBan.GetAsync("badname");

            // Assert
            Assert.Equal(expectedTkl, result);
        }
    }
}
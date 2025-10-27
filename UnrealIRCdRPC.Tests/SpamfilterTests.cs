using Moq;
using System.Text.Json;
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
            var jsonResponse = JsonSerializer.Serialize(new { list = expectedList });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.list", null, false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _spamfilter.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Array, result.Value.ValueKind);
            Assert.Equal(2, result.Value.GetArrayLength());
        }

        [Fact]
        public async Task AddAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.add", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _spamfilter.AddAsync("badword", "simple", "private", "block", "1h", "spam");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("spamfilter.add", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsQueryWithCorrectParameters()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.del", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _spamfilter.DeleteAsync("badword", "simple", "private", "block");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
            _mockQuerier.Verify(q => q.QueryAsync("spamfilter.del", It.IsAny<object>(), false), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsTkl_WhenFound()
        {
            // Arrange
            var expectedTkl = new { id = "123" };
            var jsonResponse = JsonSerializer.Serialize(new { tkl = expectedTkl });
            var response = JsonDocument.Parse(jsonResponse).RootElement;
            _mockQuerier.Setup(q => q.QueryAsync("spamfilter.get", It.IsAny<object>(), false)).Returns(Task.FromResult<JsonElement?>(response));

            // Act
            var result = await _spamfilter.GetAsync("badword", "simple", "private", "block");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(JsonValueKind.Object, result.Value.ValueKind);
            Assert.True(result.Value.TryGetProperty("id", out var idElement));
            Assert.Equal("123", idElement.GetString());
        }
    }
}
using Microsoft.Extensions.Logging;
using Moq;
using LocationFinder.API.Services;
using LocationFinder.API.Data;
using LocationFinder.API.Models;
using LocationFinder.API.Tests.Helpers;

namespace LocationFinder.API.Tests.Services
{
    /// <summary>
    /// Unit tests for LocationService
    /// </summary>
    public class LocationServiceTests : IDisposable
    {
        private readonly Mock<ILogger<LocationService>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly LocationService _service;

        public LocationServiceTests()
        {
            _mockLogger = new Mock<ILogger<LocationService>>();
            _context = TestDbContextHelper.CreateTestDbContextWithData();
            _service = new LocationService(_context, _mockLogger.Object);
        }

        public void Dispose()
        {
            TestDbContextHelper.CleanupTestDbContext(_context);
        }

        [Fact]
        public void CalculateDistance_WithValidCoordinates_ReturnsCorrectDistance()
        {
            // Arrange
            double lat1 = 40.7505; // New York
            double lon1 = -73.9965;
            double lat2 = 34.1030; // Beverly Hills
            double lon2 = -118.4105;

            // Act
            double distance = _service.CalculateDistance(lat1, lon1, lat2, lon2);

            // Assert
            distance.Should().BeGreaterThan(0);
            distance.Should().BeApproximately(2447.0, 50.0); // Approximately 2447 miles
        }

        [Fact]
        public void CalculateDistance_WithSameCoordinates_ReturnsZero()
        {
            // Arrange
            double lat = 40.7505;
            double lon = -73.9965;

            // Act
            double distance = _service.CalculateDistance(lat, lon, lat, lon);

            // Assert
            distance.Should().Be(0);
        }

        [Fact]
        public void CalculateDistance_WithNearbyCoordinates_ReturnsSmallDistance()
        {
            // Arrange
            double lat1 = 40.7505;
            double lon1 = -73.9965;
            double lat2 = 40.7506; // Very close
            double lon2 = -73.9966;

            // Act
            double distance = _service.CalculateDistance(lat1, lon1, lat2, lon2);

            // Assert
            distance.Should().BeGreaterThan(0);
            distance.Should().BeLessThan(1.0); // Less than 1 mile
        }

        [Fact]
        public void ToRadians_WithValidDegrees_ReturnsCorrectRadians()
        {
            // Arrange
            double degrees = 180.0;

            // Act
            double radians = _service.ToRadians(degrees);

            // Assert
            radians.Should().BeApproximately(Math.PI, 0.001);
        }

        [Fact]
        public void ToRadians_WithZeroDegrees_ReturnsZero()
        {
            // Arrange
            double degrees = 0.0;

            // Act
            double radians = _service.ToRadians(degrees);

            // Assert
            radians.Should().Be(0);
        }

        [Theory]
        [InlineData("10001", true)]
        [InlineData("90210", true)]
        [InlineData("12345", true)]
        [InlineData("1234", false)]
        [InlineData("123456", false)]
        [InlineData("abc12", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValidZipCode_WithVariousInputs_ReturnsExpectedResult(string? zipCode, bool expected)
        {
            // Act
            bool result = _service.IsValidZipCode(zipCode);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithValidZipCode_ReturnsLocations()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountGreaterThan(0);
            result.Data.Should().OnlyContain(l => l.DistanceMiles >= 0);
            result.Data.Should().BeInAscendingOrder(l => l.DistanceMiles);
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithValidZipCode_ReturnsOnlyActiveLocations()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // Should not contain inactive locations
            var inactiveLocation = result.Data.FirstOrDefault(l => l.Name == "Inactive Location");
            inactiveLocation.Should().BeNull();
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithValidZipCode_RespectsLimit()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 2;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountLessThanOrEqualTo(limit);
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithInvalidZipCode_ReturnsErrorResponse()
        {
            // Arrange
            string zipCode = "invalid";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("invalid");
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithNonExistentZipCode_ReturnsErrorResponse()
        {
            // Arrange
            string zipCode = "99999";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("not found");
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithEmptyZipCode_ReturnsErrorResponse()
        {
            // Arrange
            string zipCode = "";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("required");
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithNullZipCode_ReturnsErrorResponse()
        {
            // Arrange
            string? zipCode = null;
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("required");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task SearchLocationsByZipCodeAsync_WithInvalidLimit_ReturnsErrorResponse(int limit)
        {
            // Arrange
            string zipCode = "10001";

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("limit");
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithLargeLimit_RespectsMaximumLimit()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 100; // Larger than default maximum

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountLessThanOrEqualTo(20); // Default maximum
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithValidZipCode_CalculatesDistancesCorrectly()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // All locations should have calculated distances
            foreach (var location in result.Data)
            {
                location.DistanceMiles.Should().BeGreaterThanOrEqualTo(0);
            }
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithValidZipCode_ReturnsSortedResults()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // Results should be sorted by distance (closest first)
            var distances = result.Data.Select(l => l.DistanceMiles).ToList();
            distances.Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task SearchLocationsByZipCodeAsync_WithDatabaseException_ReturnsErrorResponse()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;

            // Dispose the context to simulate database error
            _context.Dispose();

            // Act
            var result = await _service.SearchLocationsByZipCodeAsync(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("error");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using LocationFinder.API.Controllers;
using LocationFinder.API.Services;
using LocationFinder.API.Models;
using LocationFinder.API.Tests.Helpers;

namespace LocationFinder.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for LocationsController
    /// </summary>
    public class LocationsControllerTests
    {
        private readonly Mock<ILocationService> _mockLocationService;
        private readonly Mock<ILogger<LocationsController>> _mockLogger;
        private readonly LocationsController _controller;

        public LocationsControllerTests()
        {
            _mockLocationService = new Mock<ILocationService>();
            _mockLogger = new Mock<ILogger<LocationsController>>();
            _controller = new LocationsController(_mockLocationService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Search_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;
            var expectedResponse = TestDataHelper.CreateTestApiResponse(
                TestDataHelper.CreateTestLocationSearchResults(3)
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public void Search_WithValidParameters_CallsServiceCorrectly()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;
            var expectedResponse = TestDataHelper.CreateTestApiResponse(
                TestDataHelper.CreateTestLocationSearchResults(3)
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(expectedResponse);

            // Act
            _controller.Search(zipCode, limit);

            // Assert
            _mockLocationService.Verify(
                x => x.SearchLocationsByZipCodeAsync(zipCode, limit),
                Times.Once
            );
        }

        [Fact]
        public void Search_WithInvalidZipCode_ReturnsBadRequest()
        {
            // Arrange
            string zipCode = "invalid";
            int limit = 10;
            var errorResponse = TestDataHelper.CreateTestApiResponse<List<LocationSearchResult>>(
                null, false, "Invalid zip code format"
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(errorResponse);

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("Invalid zip code");
        }

        [Fact]
        public void Search_WithNonExistentZipCode_ReturnsNotFound()
        {
            // Arrange
            string zipCode = "99999";
            int limit = 10;
            var errorResponse = TestDataHelper.CreateTestApiResponse<List<LocationSearchResult>>(
                null, false, "Zip code not found"
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(errorResponse);

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            var response = notFoundResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("not found");
        }

        [Fact]
        public void Search_WithServiceException_ReturnsInternalServerError()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;
            var errorResponse = TestDataHelper.CreateTestApiResponse<List<LocationSearchResult>>(
                null, false, "An error occurred while searching for locations"
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(errorResponse);

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            var response = statusCodeResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("error");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Search_WithEmptyOrNullZipCode_ReturnsBadRequest(string? zipCode)
        {
            // Arrange
            int limit = 10;

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("required");
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("abc12")]
        public void Search_WithInvalidZipCodeFormat_ReturnsBadRequest(string zipCode)
        {
            // Arrange
            int limit = 10;

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("format");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Search_WithInvalidLimit_ReturnsBadRequest(int limit)
        {
            // Arrange
            string zipCode = "10001";

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("limit");
        }

        [Fact]
        public void Search_WithLargeLimit_ReturnsBadRequest()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 100; // Larger than maximum allowed

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("limit");
        }

        [Fact]
        public void Search_WithValidParameters_ReturnsCorrectDataStructure()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 5;
            var testResults = TestDataHelper.CreateTestLocationSearchResults(3);
            var expectedResponse = TestDataHelper.CreateTestApiResponse(testResults);

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;

            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.Should().HaveCount(3);

            // Verify each location has required properties
            foreach (var location in response.Data)
            {
                location.Id.Should().BeGreaterThan(0);
                location.Name.Should().NotBeNullOrEmpty();
                location.Address.Should().NotBeNullOrEmpty();
                location.City.Should().NotBeNullOrEmpty();
                location.State.Should().NotBeNullOrEmpty();
                location.ZipCode.Should().NotBeNullOrEmpty();
                location.DistanceMiles.Should().BeGreaterThanOrEqualTo(0);
            }
        }

        [Fact]
        public void Search_WithNoResults_ReturnsEmptyList()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;
            var emptyResponse = TestDataHelper.CreateTestApiResponse(new List<LocationSearchResult>());

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(emptyResponse);

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;

            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.Should().BeEmpty();
        }

        [Fact]
        public void Search_WithServiceThrowingException_ReturnsInternalServerError()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = _controller.Search(zipCode, limit);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            var response = statusCodeResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeFalse();
            response.Message.Should().Contain("error");
        }

        [Fact]
        public void Search_WithDefaultLimit_ReturnsExpectedResults()
        {
            // Arrange
            string zipCode = "10001";
            // No limit parameter - should use default
            var expectedResponse = TestDataHelper.CreateTestApiResponse(
                TestDataHelper.CreateTestLocationSearchResults(10)
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = _controller.Search(zipCode);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ApiResponse<List<LocationSearchResult>>>().Subject;
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public void Search_WithValidParameters_LogsInformation()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 10;
            var expectedResponse = TestDataHelper.CreateTestApiResponse(
                TestDataHelper.CreateTestLocationSearchResults(3)
            );

            _mockLocationService
                .Setup(x => x.SearchLocationsByZipCodeAsync(zipCode, limit))
                .ReturnsAsync(expectedResponse);

            // Act
            _controller.Search(zipCode, limit);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ),
                Times.AtLeastOnce
            );
        }

        [Fact]
        public void Search_WithError_LogsError()
        {
            // Arrange
            string zipCode = "invalid";
            int limit = 10;

            // Act
            _controller.Search(zipCode, limit);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ),
                Times.AtLeastOnce
            );
        }
    }
}

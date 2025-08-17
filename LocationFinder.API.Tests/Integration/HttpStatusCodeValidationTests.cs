using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using LocationFinder.API.Data;
using LocationFinder.API.Models;
using LocationFinder.API.Tests.Helpers;

namespace LocationFinder.API.Tests.Integration
{
    /// <summary>
    /// HTTP status code validation tests for API endpoints
    /// </summary>
    public class HttpStatusCodeValidationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public HttpStatusCodeValidationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
                    });

                    // Create a new service provider
                    var serviceProvider = services.BuildServiceProvider();

                    // Create a scope to get the DbContext
                    using var scope = serviceProvider.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    // Ensure database is created
                    db.Database.EnsureCreated();

                    // Seed test data
                    SeedTestData(db);
                });
            });

            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Fact]
        public async Task Search_WithValidZipCode_Returns200Ok()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task Search_WithInvalidZipCode_Returns400BadRequest()
        {
            // Arrange
            string zipCode = "invalid";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Search_WithNonExistentZipCode_Returns404NotFound()
        {
            // Arrange
            string zipCode = "99999";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Search_WithEmptyZipCode_Returns400BadRequest()
        {
            // Arrange
            string zipCode = "";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Search_WithNullZipCode_Returns400BadRequest()
        {
            // Arrange
            string url = "/api/locations/search?zipcode=&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Search_WithMissingZipCode_Returns400BadRequest()
        {
            // Arrange
            string url = "/api/locations/search?limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("abc12")]
        public async Task Search_WithInvalidZipCodeFormat_Returns400BadRequest(string zipCode)
        {
            // Arrange
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task Search_WithInvalidLimit_Returns400BadRequest(int limit)
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit={limit}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Search_WithLargeLimit_Returns400BadRequest()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 100; // Larger than maximum allowed
            string url = $"/api/locations/search?zipcode={zipCode}&limit={limit}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsCorrectContentType()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            response.Content.Headers.ContentType.CharSet.Should().Contain("utf-8");
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsValidJsonResponse()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();

            // Verify it's valid JSON
            Action parseJson = () => JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            parseJson.Should().NotThrow();
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsExpectedResponseStructure()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Message.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task Search_WithError_ReturnsExpectedResponseStructure()
        {
            // Arrange
            string zipCode = "invalid";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsCorsHeaders()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check for CORS headers (if configured)
            response.Headers.Should().ContainKey("Access-Control-Allow-Origin");
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsCacheHeaders()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check for cache control headers
            response.Headers.Should().ContainKey("Cache-Control");
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsCorrectLocationData()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountGreaterThan(0);

            // Verify each location has required properties
            foreach (var location in result.Data)
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
        public async Task Search_WithValidZipCode_ReturnsOnlyActiveLocations()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // Should not contain inactive locations
            var inactiveLocation = result.Data.FirstOrDefault(l => l.Name == "Inactive Location");
            inactiveLocation.Should().BeNull();
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsSortedResults()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();

            // Results should be sorted by distance (closest first)
            var distances = result.Data.Select(l => l.DistanceMiles).ToList();
            distances.Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task Search_WithValidZipCode_RespectsLimit()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 2;
            string url = $"/api/locations/search?zipcode={zipCode}&limit={limit}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountLessThanOrEqualTo(limit);
        }

        [Fact]
        public async Task Search_WithDefaultLimit_ReturnsExpectedResults()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}"; // No limit parameter

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountLessThanOrEqualTo(20); // Default maximum
        }

        private void SeedTestData(ApplicationDbContext context)
        {
            // Clear existing data
            context.ZipCodes.RemoveRange(context.ZipCodes);
            context.Locations.RemoveRange(context.Locations);
            context.SaveChanges();

            // Add test zip codes
            var zipCodes = TestDataHelper.CreateTestZipCodes();
            context.ZipCodes.AddRange(zipCodes);

            // Add test locations
            var locations = TestDataHelper.CreateTestLocations();
            context.Locations.AddRange(locations);

            context.SaveChanges();
        }
    }
}

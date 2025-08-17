using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using LocationFinder.API.Data;
using LocationFinder.API.Models;
using LocationFinder.API.Tests.Helpers;

namespace LocationFinder.API.Tests.Integration
{
    /// <summary>
    /// Integration tests for LocationsController using in-memory database
    /// </summary>
    public class LocationsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;

        public LocationsControllerIntegrationTests(WebApplicationFactory<Program> factory)
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

            // Get the DbContext from the service provider
            using var scope = _factory.Services.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public void Dispose()
        {
            _context?.Dispose();
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsOkWithLocations()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.MediaType.Should().Be("application/json");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCountGreaterThan(0);
            result.Data.Should().OnlyContain(l => l.DistanceMiles >= 0);
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
        public async Task Search_WithInvalidZipCode_ReturnsBadRequest()
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
            result.Message.Should().Contain("invalid");
        }

        [Fact]
        public async Task Search_WithNonExistentZipCode_ReturnsNotFound()
        {
            // Arrange
            string zipCode = "99999";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=10";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Contain("not found");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Search_WithEmptyOrNullZipCode_ReturnsBadRequest(string? zipCode)
        {
            // Arrange
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
            result.Message.Should().Contain("required");
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("abc12")]
        public async Task Search_WithInvalidZipCodeFormat_ReturnsBadRequest(string zipCode)
        {
            // Arrange
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
            result.Message.Should().Contain("format");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task Search_WithInvalidLimit_ReturnsBadRequest(int limit)
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit={limit}";

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
            result.Message.Should().Contain("limit");
        }

        [Fact]
        public async Task Search_WithLargeLimit_ReturnsBadRequest()
        {
            // Arrange
            string zipCode = "10001";
            int limit = 100; // Larger than maximum allowed
            string url = $"/api/locations/search?zipcode={zipCode}&limit={limit}";

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
            result.Message.Should().Contain("limit");
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

        [Fact]
        public async Task Search_WithValidZipCode_ReturnsCorrectDataStructure()
        {
            // Arrange
            string zipCode = "10001";
            string url = $"/api/locations/search?zipcode={zipCode}&limit=5";

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
        public async Task Search_WithDifferentZipCodes_ReturnsDifferentResults()
        {
            // Arrange
            string zipCode1 = "10001";
            string zipCode2 = "90210";
            string url1 = $"/api/locations/search?zipcode={zipCode1}&limit=10";
            string url2 = $"/api/locations/search?zipcode={zipCode2}&limit=10";

            // Act
            var response1 = await _client.GetAsync(url1);
            var response2 = await _client.GetAsync(url2);

            // Assert
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.OK);

            var content1 = await response1.Content.ReadAsStringAsync();
            var content2 = await response2.Content.ReadAsStringAsync();

            var result1 = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content1, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var result2 = JsonSerializer.Deserialize<ApiResponse<List<LocationSearchResult>>>(content2, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            result1.Success.Should().BeTrue();
            result2.Success.Should().BeTrue();

            // Results should be different for different zip codes
            result1.Data.Should().NotBeEquivalentTo(result2.Data);
        }

        [Fact]
        public async Task Search_WithMissingZipCode_ReturnsBadRequest()
        {
            // Arrange
            string url = "/api/locations/search?limit=10"; // Missing zipcode parameter

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
        }

        [Fact]
        public async Task Search_WithValidZipCode_CalculatesDistancesCorrectly()
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

            // All locations should have calculated distances
            foreach (var location in result.Data)
            {
                location.DistanceMiles.Should().BeGreaterThanOrEqualTo(0);

                // Locations in the same zip code should have very small distances
                if (location.ZipCode == zipCode)
                {
                    location.DistanceMiles.Should().BeLessThan(1.0);
                }
            }
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

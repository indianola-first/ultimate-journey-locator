using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LocationFinder.API.Data;
using LocationFinder.API.Models;
using LocationFinder.API.Tests.Helpers;

namespace LocationFinder.API.Tests.Integration
{
    /// <summary>
    /// Database integration tests for data persistence and operations
    /// </summary>
    public class DatabaseIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public DatabaseIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
            });

            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public void Database_CanBeCreated()
        {
            // Assert
            _context.Database.CanConnect().Should().BeTrue();
        }

        [Fact]
        public void Database_TablesExist()
        {
            // Act
            var zipCodesTable = _context.ZipCodes.ToList();
            var locationsTable = _context.Locations.ToList();

            // Assert
            zipCodesTable.Should().NotBeNull();
            locationsTable.Should().NotBeNull();
        }

        [Fact]
        public void Database_CanSeedZipCodes()
        {
            // Arrange
            var zipCodes = TestDataHelper.CreateTestZipCodes();

            // Act
            _context.ZipCodes.AddRange(zipCodes);
            _context.SaveChanges();

            // Assert
            var savedZipCodes = _context.ZipCodes.ToList();
            savedZipCodes.Should().HaveCount(zipCodes.Count);
            savedZipCodes.Should().BeEquivalentTo(zipCodes, options => options.Excluding(z => z.Id));
        }

        [Fact]
        public void Database_CanSeedLocations()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();

            // Act
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Assert
            var savedLocations = _context.Locations.ToList();
            savedLocations.Should().HaveCount(locations.Count);
            savedLocations.Should().BeEquivalentTo(locations, options => options.Excluding(l => l.Id));
        }

        [Fact]
        public void Database_CanQueryZipCodesByZipCode()
        {
            // Arrange
            var zipCodes = TestDataHelper.CreateTestZipCodes();
            _context.ZipCodes.AddRange(zipCodes);
            _context.SaveChanges();

            // Act
            var foundZipCode = _context.ZipCodes.FirstOrDefault(z => z.ZipCode == "10001");

            // Assert
            foundZipCode.Should().NotBeNull();
            foundZipCode.ZipCode.Should().Be("10001");
            foundZipCode.City.Should().Be("New York");
            foundZipCode.State.Should().Be("NY");
        }

        [Fact]
        public void Database_CanQueryLocationsByZipCode()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act
            var foundLocations = _context.Locations.Where(l => l.ZipCode == "10001").ToList();

            // Assert
            foundLocations.Should().NotBeEmpty();
            foundLocations.Should().OnlyContain(l => l.ZipCode == "10001");
        }

        [Fact]
        public void Database_CanQueryActiveLocations()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act
            var activeLocations = _context.Locations.Where(l => l.IsActive).ToList();
            var inactiveLocations = _context.Locations.Where(l => !l.IsActive).ToList();

            // Assert
            activeLocations.Should().NotBeEmpty();
            inactiveLocations.Should().NotBeEmpty();
            activeLocations.Should().OnlyContain(l => l.IsActive);
            inactiveLocations.Should().OnlyContain(l => !l.IsActive);
        }

        [Fact]
        public void Database_CanQueryLocationsByCoordinates()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act
            var locationsWithCoordinates = _context.Locations
                .Where(l => l.Latitude.HasValue && l.Longitude.HasValue)
                .ToList();

            // Assert
            locationsWithCoordinates.Should().NotBeEmpty();
            locationsWithCoordinates.Should().OnlyContain(l => l.Latitude.HasValue && l.Longitude.HasValue);
        }

        [Fact]
        public void Database_CanUpdateLocation()
        {
            // Arrange
            var location = TestDataHelper.CreateTestLocation();
            _context.Locations.Add(location);
            _context.SaveChanges();

            var originalName = location.Name;
            var newName = "Updated Location Name";

            // Act
            location.Name = newName;
            _context.SaveChanges();

            // Assert
            var updatedLocation = _context.Locations.Find(location.Id);
            updatedLocation.Should().NotBeNull();
            updatedLocation.Name.Should().Be(newName);
            updatedLocation.Name.Should().NotBe(originalName);
        }

        [Fact]
        public void Database_CanDeleteLocation()
        {
            // Arrange
            var location = TestDataHelper.CreateTestLocation();
            _context.Locations.Add(location);
            _context.SaveChanges();

            var locationId = location.Id;

            // Act
            _context.Locations.Remove(location);
            _context.SaveChanges();

            // Assert
            var deletedLocation = _context.Locations.Find(locationId);
            deletedLocation.Should().BeNull();
        }

        [Fact]
        public void Database_CanQueryLocationsWithInclude()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act
            var locationsWithDetails = _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToList();

            // Assert
            locationsWithDetails.Should().NotBeEmpty();
            locationsWithDetails.Should().BeInAscendingOrder(l => l.Name);
            locationsWithDetails.Should().OnlyContain(l => l.IsActive);
        }

        [Fact]
        public void Database_CanQueryZipCodesWithFiltering()
        {
            // Arrange
            var zipCodes = TestDataHelper.CreateTestZipCodes();
            _context.ZipCodes.AddRange(zipCodes);
            _context.SaveChanges();

            // Act
            var nyZipCodes = _context.ZipCodes.Where(z => z.State == "NY").ToList();
            var caZipCodes = _context.ZipCodes.Where(z => z.State == "CA").ToList();

            // Assert
            nyZipCodes.Should().NotBeEmpty();
            caZipCodes.Should().NotBeEmpty();
            nyZipCodes.Should().OnlyContain(z => z.State == "NY");
            caZipCodes.Should().OnlyContain(z => z.State == "CA");
        }

        [Fact]
        public void Database_CanQueryLocationsWithComplexFiltering()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act
            var activeLocationsInNY = _context.Locations
                .Where(l => l.IsActive && l.State == "NY")
                .OrderBy(l => l.Name)
                .ToList();

            // Assert
            activeLocationsInNY.Should().NotBeEmpty();
            activeLocationsInNY.Should().OnlyContain(l => l.IsActive && l.State == "NY");
            activeLocationsInNY.Should().BeInAscendingOrder(l => l.Name);
        }

        [Fact]
        public void Database_CanQueryLocationsWithPagination()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            int pageSize = 2;
            int pageNumber = 1;

            // Act
            var pagedLocations = _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Assert
            pagedLocations.Should().HaveCountLessThanOrEqualTo(pageSize);
            pagedLocations.Should().OnlyContain(l => l.IsActive);
        }

        [Fact]
        public void Database_CanQueryLocationsWithDistanceCalculation()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            double searchLat = 40.7505;
            double searchLon = -73.9965;

            // Act
            var locationsWithDistance = _context.Locations
                .Where(l => l.IsActive && l.Latitude.HasValue && l.Longitude.HasValue)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.Latitude,
                    l.Longitude,
                    Distance = Math.Sqrt(
                        Math.Pow((double)(l.Latitude - searchLat), 2) +
                        Math.Pow((double)(l.Longitude - searchLon), 2)
                    )
                })
                .OrderBy(x => x.Distance)
                .ToList();

            // Assert
            locationsWithDistance.Should().NotBeEmpty();
            locationsWithDistance.Should().BeInAscendingOrder(x => x.Distance);
            locationsWithDistance.Should().OnlyContain(x => x.Latitude.HasValue && x.Longitude.HasValue);
        }

        [Fact]
        public void Database_CanHandleConcurrentAccess()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act & Assert
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using var context = CreateNewContext();
                    var count = context.Locations.Count();
                    count.Should().BeGreaterThan(0);
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        [Fact]
        public void Database_CanHandleTransactionRollback()
        {
            // Arrange
            var location = TestDataHelper.CreateTestLocation();
            var originalCount = _context.Locations.Count();

            // Act
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Locations.Add(location);
                _context.SaveChanges();

                // Simulate an error
                throw new Exception("Simulated error");
            }
            catch
            {
                transaction.Rollback();
            }

            // Assert
            var finalCount = _context.Locations.Count();
            finalCount.Should().Be(originalCount);
        }

        [Fact]
        public void Database_CanHandleTransactionCommit()
        {
            // Arrange
            var location = TestDataHelper.CreateTestLocation();
            var originalCount = _context.Locations.Count();

            // Act
            using var transaction = _context.Database.BeginTransaction();
            _context.Locations.Add(location);
            _context.SaveChanges();
            transaction.Commit();

            // Assert
            var finalCount = _context.Locations.Count();
            finalCount.Should().Be(originalCount + 1);
        }

        [Fact]
        public void Database_CanQueryWithRawSql()
        {
            // Arrange
            var locations = TestDataHelper.CreateTestLocations();
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Act
            var activeLocationsCount = _context.Locations
                .FromSqlRaw("SELECT * FROM Locations WHERE IsActive = 1")
                .Count();

            // Assert
            activeLocationsCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Database_CanHandleBulkOperations()
        {
            // Arrange
            var locations = new List<Location>();
            for (int i = 1; i <= 100; i++)
            {
                locations.Add(TestDataHelper.CreateTestLocation(i));
            }

            // Act
            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            // Assert
            var count = _context.Locations.Count();
            count.Should().BeGreaterThanOrEqualTo(100);
        }

        private ApplicationDbContext CreateNewContext()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using LocationFinder.API.Data;
using LocationFinder.API.Models;

namespace LocationFinder.API.Tests.Helpers
{
    /// <summary>
    /// Helper class for creating test database contexts
    /// </summary>
    public static class TestDbContextHelper
    {
        /// <summary>
        /// Creates an in-memory database context for testing
        /// </summary>
        public static ApplicationDbContext CreateTestDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates an in-memory database context with test data
        /// </summary>
        public static ApplicationDbContext CreateTestDbContextWithData()
        {
            var context = CreateTestDbContext();
            SeedTestData(context);
            return context;
        }

        /// <summary>
        /// Seeds test data into the database context
        /// </summary>
        public static void SeedTestData(ApplicationDbContext context)
        {
            // Add test zip codes
            var zipCodes = TestDataHelper.CreateTestZipCodes();
            context.ZipCodes.AddRange(zipCodes);

            // Add test locations
            var locations = TestDataHelper.CreateTestLocations();
            context.Locations.AddRange(locations);

            context.SaveChanges();
        }

        /// <summary>
        /// Creates a test database context with specific zip codes
        /// </summary>
        public static ApplicationDbContext CreateTestDbContextWithZipCodes(List<ZipCode> zipCodes)
        {
            var context = CreateTestDbContext();
            context.ZipCodes.AddRange(zipCodes);
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Creates a test database context with specific locations
        /// </summary>
        public static ApplicationDbContext CreateTestDbContextWithLocations(List<Location> locations)
        {
            var context = CreateTestDbContext();
            context.Locations.AddRange(locations);
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Creates a test database context with both zip codes and locations
        /// </summary>
        public static ApplicationDbContext CreateTestDbContextWithZipCodesAndLocations(
            List<ZipCode> zipCodes,
            List<Location> locations)
        {
            var context = CreateTestDbContext();
            context.ZipCodes.AddRange(zipCodes);
            context.Locations.AddRange(locations);
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Cleans up the test database context
        /// </summary>
        public static void CleanupTestDbContext(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}

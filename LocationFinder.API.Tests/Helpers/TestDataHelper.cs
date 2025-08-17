using LocationFinder.API.Models;

namespace LocationFinder.API.Tests.Helpers
{
    /// <summary>
    /// Helper class for generating test data
    /// </summary>
    public static class TestDataHelper
    {
        /// <summary>
        /// Creates a list of test zip codes
        /// </summary>
        public static List<ZipCode> CreateTestZipCodes()
        {
            return new List<ZipCode>
            {
                new ZipCode
                {
                    Id = 1,
                    ZipCode = "10001",
                    City = "New York",
                    State = "NY",
                    Latitude = 40.7505,
                    Longitude = -73.9965
                },
                new ZipCode
                {
                    Id = 2,
                    ZipCode = "90210",
                    City = "Beverly Hills",
                    State = "CA",
                    Latitude = 34.1030,
                    Longitude = -118.4105
                },
                new ZipCode
                {
                    Id = 3,
                    ZipCode = "33101",
                    City = "Miami",
                    State = "FL",
                    Latitude = 25.7743,
                    Longitude = -80.1937
                },
                new ZipCode
                {
                    Id = 4,
                    ZipCode = "60601",
                    City = "Chicago",
                    State = "IL",
                    Latitude = 41.8857,
                    Longitude = -87.6228
                },
                new ZipCode
                {
                    Id = 5,
                    ZipCode = "77001",
                    City = "Houston",
                    State = "TX",
                    Latitude = 29.7604,
                    Longitude = -95.3698
                }
            };
        }

        /// <summary>
        /// Creates a list of test locations
        /// </summary>
        public static List<Location> CreateTestLocations()
        {
            return new List<Location>
            {
                new Location
                {
                    Id = 1,
                    Name = "Downtown Store",
                    Address = "123 Main St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Phone = "212-555-0100",
                    BusinessHours = "Mon-Fri 9AM-6PM, Sat 10AM-4PM",
                    Latitude = 40.7505,
                    Longitude = -73.9965,
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = 2,
                    Name = "Westside Branch",
                    Address = "456 Oak Ave",
                    City = "Beverly Hills",
                    State = "CA",
                    ZipCode = "90210",
                    Phone = "310-555-0200",
                    BusinessHours = "Mon-Sat 10AM-8PM, Sun 12PM-6PM",
                    Latitude = 34.1030,
                    Longitude = -118.4105,
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = 3,
                    Name = "Beach Location",
                    Address = "789 Ocean Blvd",
                    City = "Miami",
                    State = "FL",
                    ZipCode = "33101",
                    Phone = "305-555-0300",
                    BusinessHours = "Daily 8AM-10PM",
                    Latitude = 25.7743,
                    Longitude = -80.1937,
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = 4,
                    Name = "Loop Office",
                    Address = "321 State St",
                    City = "Chicago",
                    State = "IL",
                    ZipCode = "60601",
                    Phone = "312-555-0400",
                    BusinessHours = "Mon-Fri 8AM-7PM",
                    Latitude = 41.8857,
                    Longitude = -87.6228,
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = 5,
                    Name = "Downtown Houston",
                    Address = "654 Texas Ave",
                    City = "Houston",
                    State = "TX",
                    ZipCode = "77001",
                    Phone = "713-555-0500",
                    BusinessHours = "Mon-Sat 9AM-6PM",
                    Latitude = 29.7604,
                    Longitude = -95.3698,
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Location
                {
                    Id = 6,
                    Name = "Inactive Location",
                    Address = "999 Closed St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Phone = "212-555-9999",
                    BusinessHours = "Closed",
                    Latitude = 40.7505,
                    Longitude = -73.9965,
                    IsActive = false,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };
        }

        /// <summary>
        /// Creates a test zip code
        /// </summary>
        public static ZipCode CreateTestZipCode(string zipCode = "10001")
        {
            return new ZipCode
            {
                Id = 1,
                ZipCode = zipCode,
                City = "Test City",
                State = "TS",
                Latitude = 40.7505,
                Longitude = -73.9965
            };
        }

        /// <summary>
        /// Creates a test location
        /// </summary>
        public static Location CreateTestLocation(int id = 1, bool isActive = true)
        {
            return new Location
            {
                Id = id,
                Name = $"Test Location {id}",
                Address = $"{id}23 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "10001",
                Phone = $"555-555-{id:D4}",
                BusinessHours = "Mon-Fri 9AM-6PM",
                Latitude = 40.7505,
                Longitude = -73.9965,
                IsActive = isActive,
                CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
        }

        /// <summary>
        /// Creates a test location search result
        /// </summary>
        public static LocationSearchResult CreateTestLocationSearchResult(int id = 1, double distanceMiles = 1.5)
        {
            return new LocationSearchResult
            {
                Id = id,
                Name = $"Test Location {id}",
                Address = $"{id}23 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "10001",
                Phone = $"555-555-{id:D4}",
                BusinessHours = "Mon-Fri 9AM-6PM",
                DistanceMiles = distanceMiles
            };
        }

        /// <summary>
        /// Creates a list of test location search results
        /// </summary>
        public static List<LocationSearchResult> CreateTestLocationSearchResults(int count = 5)
        {
            var results = new List<LocationSearchResult>();
            for (int i = 1; i <= count; i++)
            {
                results.Add(CreateTestLocationSearchResult(i, i * 0.5));
            }
            return results;
        }

        /// <summary>
        /// Creates a test API response
        /// </summary>
        public static ApiResponse<T> CreateTestApiResponse<T>(T data, bool success = true, string message = "")
        {
            return new ApiResponse<T>
            {
                Success = success,
                Data = data,
                Message = message
            };
        }
    }
}

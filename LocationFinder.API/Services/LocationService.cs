using LocationFinder.API.Data;
using LocationFinder.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace LocationFinder.API.Services
{
    /// <summary>
    /// Service implementation for location search operations
    /// </summary>
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocationService> _logger;

        public LocationService(ApplicationDbContext context, ILogger<LocationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Searches for locations near a specified zip code, sorted by distance
        /// </summary>
        /// <param name="zipCode">The 5-digit US zip code to search from</param>
        /// <param name="limit">Maximum number of locations to return (default: 10)</param>
        /// <returns>
        /// ApiResponse containing a list of LocationSearchResult objects with distance calculations,
        /// or an error response if the zip code is not found
        /// </returns>
        public async Task<ApiResponse<List<LocationSearchResult>>> SearchLocationsByZipCodeAsync(string zipCode, int limit = 10)
        {
            try
            {
                // Validate input parameters
                if (string.IsNullOrWhiteSpace(zipCode))
                {
                    _logger.LogWarning("Search attempted with null or empty zip code");
                    return ApiResponse<List<LocationSearchResult>>.CreateError("Zip code is required");
                }

                if (!IsValidZipCode(zipCode))
                {
                    _logger.LogWarning("Search attempted with invalid zip code format: {ZipCode}", zipCode);
                    return ApiResponse<List<LocationSearchResult>>.CreateError("Please enter a valid 5-digit zip code");
                }

                if (limit <= 0 || limit > 100)
                {
                    _logger.LogWarning("Invalid limit parameter: {Limit}", limit);
                    return ApiResponse<List<LocationSearchResult>>.CreateError("Limit must be between 1 and 100");
                }

                _logger.LogInformation("Searching for locations near zip code: {ZipCode}, limit: {Limit}", zipCode, limit);

                // Find the zip code coordinates
                var zipCodeEntity = await _context.ZipCodes
                    .FirstOrDefaultAsync(z => z.ZipCodeValue == zipCode);

                if (zipCodeEntity == null)
                {
                    _logger.LogWarning("Zip code not found in database: {ZipCode}", zipCode);
                    return ApiResponse<List<LocationSearchResult>>.CreateError("Zip code not found");
                }

                // Get all active locations
                var locations = await _context.Locations
                    .Where(l => l.IsActive)
                    .ToListAsync();

                if (!locations.Any())
                {
                    _logger.LogInformation("No active locations found in database");
                    return ApiResponse<List<LocationSearchResult>>.CreateSuccess(new List<LocationSearchResult>(), "No locations found");
                }

                // Calculate distances and create results
                var results = locations
                    .Select(location => new LocationSearchResult
                    {
                        Id = location.Id,
                        Name = location.Name,
                        Address = location.Address,
                        City = location.City,
                        State = location.State,
                        ZipCode = location.ZipCode,
                        Phone = location.Phone,
                        BusinessHours = location.BusinessHours,
                        DistanceMiles = CalculateDistance(
                            zipCodeEntity.Latitude,
                            zipCodeEntity.Longitude,
                            location.Latitude,
                            location.Longitude)
                    })
                    .OrderBy(r => r.DistanceMiles)
                    .Take(limit)
                    .ToList();

                _logger.LogInformation("Found {Count} locations near zip code {ZipCode}", results.Count, zipCode);

                return ApiResponse<List<LocationSearchResult>>.CreateSuccess(
                    results,
                    $"Found {results.Count} location(s) near {zipCodeEntity.City}, {zipCodeEntity.State}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching locations for zip code: {ZipCode}", zipCode);
                return ApiResponse<List<LocationSearchResult>>.CreateError("An error occurred while searching for locations. Please try again.");
            }
        }

        /// <summary>
        /// Calculates the distance between two points using the Haversine formula
        /// </summary>
        /// <param name="lat1">Latitude of the first point</param>
        /// <param name="lon1">Longitude of the first point</param>
        /// <param name="lat2">Latitude of the second point</param>
        /// <param name="lon2">Longitude of the second point</param>
        /// <returns>Distance in miles</returns>
        public static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 3959; // Earth's radius in miles
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees">Angle in degrees</param>
        /// <returns>Angle in radians</returns>
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        /// Validates that a zip code is in the correct 5-digit format
        /// </summary>
        /// <param name="zipCode">The zip code to validate</param>
        /// <returns>True if the zip code is valid, false otherwise</returns>
        private static bool IsValidZipCode(string zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
                return false;

            // Remove any whitespace
            zipCode = zipCode.Trim();

            // Check if it's exactly 5 digits
            return Regex.IsMatch(zipCode, @"^\d{5}$");
        }
    }
}

using LocationFinder.API.Models;

namespace LocationFinder.API.Services
{
    /// <summary>
    /// Service interface for location search operations
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Searches for locations near a specified zip code, sorted by distance
        /// </summary>
        /// <param name="zipCode">The 5-digit US zip code to search from</param>
        /// <param name="limit">Maximum number of locations to return (default: 10)</param>
        /// <returns>
        /// ApiResponse containing a list of LocationSearchResult objects with distance calculations,
        /// or an error response if the zip code is not found
        /// </returns>
        /// <remarks>
        /// This method performs the following operations:
        /// 1. Validates the zip code format (5 digits)
        /// 2. Looks up the zip code coordinates in the database
        /// 3. Calculates distances to all active locations using the Haversine formula
        /// 4. Sorts results by proximity (closest first)
        /// 5. Returns up to the specified limit of locations
        /// 
        /// Distance calculations are performed in miles using latitude/longitude coordinates.
        /// Only active locations (IsActive = true) are included in the search results.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when zipCode is null, empty, or invalid format</exception>
        Task<ApiResponse<List<LocationSearchResult>>> SearchLocationsByZipCodeAsync(string zipCode, int limit = 10);
    }
}

using Microsoft.AspNetCore.Mvc;
using LocationFinder.API.Models;
using LocationFinder.API.Services;
using System.ComponentModel.DataAnnotations;

namespace LocationFinder.API.Controllers
{
    /// <summary>
    /// Controller for location search operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService locationService, ILogger<LocationsController> logger)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Searches for locations near a specified zip code, sorted by distance
        /// </summary>
        /// <param name="zipcode">The 5-digit US zip code to search from</param>
        /// <param name="limit">Maximum number of locations to return (default: 10, max: 100)</param>
        /// <returns>
        /// A list of locations sorted by distance from the specified zip code.
        /// Returns 200 OK with locations, 400 Bad Request for invalid zip code, or 404 Not Found if zip code doesn't exist.
        /// </returns>
        /// <response code="200">Successfully found locations. Returns ApiResponse with LocationSearchResult array.</response>
        /// <response code="400">Invalid zip code format or limit parameter.</response>
        /// <response code="404">Zip code not found in database.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<LocationSearchResult>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<LocationSearchResult>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<LocationSearchResult>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<LocationSearchResult>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchLocations(
            [Required(ErrorMessage = "Zip code is required")]
            [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip code must be exactly 5 digits")]
            [FromQuery] string zipcode,
            [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
            [FromQuery] int limit = 10)
        {
            try
            {
                _logger.LogInformation("Location search requested for zip code: {ZipCode}, limit: {Limit}", zipcode, limit);

                // Additional validation for zip code format
                if (string.IsNullOrWhiteSpace(zipcode))
                {
                    _logger.LogWarning("Search attempted with null or empty zip code");
                    return BadRequest(ApiResponse<List<LocationSearchResult>>.CreateError("Zip code is required"));
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(zipcode.Trim(), @"^\d{5}$"))
                {
                    _logger.LogWarning("Search attempted with invalid zip code format: {ZipCode}", zipcode);
                    return BadRequest(ApiResponse<List<LocationSearchResult>>.CreateError("Please enter a valid 5-digit zip code"));
                }

                // Call the service to search for locations
                var result = await _locationService.SearchLocationsByZipCodeAsync(zipcode.Trim(), limit);

                // Return appropriate HTTP status based on the result
                if (!result.Success)
                {
                    if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Zip code not found: {ZipCode}", zipcode);
                        return NotFound(result);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid request for zip code: {ZipCode}, message: {Message}", zipcode, result.Message);
                        return BadRequest(result);
                    }
                }

                _logger.LogInformation("Successfully found {Count} locations for zip code: {ZipCode}",
                    result.Data?.Count ?? 0, zipcode);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while searching locations for zip code: {ZipCode}", zipcode);

                var errorResponse = ApiResponse<List<LocationSearchResult>>.CreateError(
                    "An error occurred while searching for locations. Please try again.");

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}

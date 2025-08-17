using LocationFinder.API.Models;
using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for reading location data from JSON files
/// </summary>
public interface ILocationDataReader
{
    /// <summary>
    /// Reads locations from a JSON file
    /// </summary>
    /// <param name="filePath">Path to the JSON file</param>
    /// <returns>List of locations</returns>
    Task<List<Location>> ReadLocationsAsync(string filePath);

    /// <summary>
    /// Validates location data
    /// </summary>
    /// <param name="locations">List of locations to validate</param>
    /// <returns>Validation result with errors and warnings</returns>
    Task<ValidationResult> ValidateLocationsAsync(List<Location> locations);
}

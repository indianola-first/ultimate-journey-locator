using LocationFinder.API.Models;
using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for reading zip code data from JSON files
/// </summary>
public interface IZipCodeDataReader
{
    /// <summary>
    /// Reads zip codes from a JSON file
    /// </summary>
    /// <param name="filePath">Path to the JSON file</param>
    /// <returns>List of zip codes</returns>
    Task<List<ZipCode>> ReadZipCodesAsync(string filePath);

    /// <summary>
    /// Validates zip code data
    /// </summary>
    /// <param name="zipCodes">List of zip codes to validate</param>
    /// <returns>Validation result with errors and warnings</returns>
    Task<ValidationResult> ValidateZipCodesAsync(List<ZipCode> zipCodes);
}

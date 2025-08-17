using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for importing data into the LocationFinder database
/// </summary>
public interface IDataImportService
{
    /// <summary>
    /// Imports zip codes from a JSON file
    /// </summary>
    /// <param name="filePath">Path to the JSON file</param>
    /// <param name="batchSize">Number of records to process in each batch</param>
    /// <param name="progressCallback">Optional callback for progress reporting</param>
    /// <returns>Import result with statistics</returns>
    Task<ImportResult> ImportZipCodesAsync(string filePath, int batchSize = 1000, Action<ImportProgress>? progressCallback = null);

    /// <summary>
    /// Imports locations from a JSON file
    /// </summary>
    /// <param name="filePath">Path to the JSON file</param>
    /// <param name="batchSize">Number of records to process in each batch</param>
    /// <param name="progressCallback">Optional callback for progress reporting</param>
    /// <returns>Import result with statistics</returns>
    Task<ImportResult> ImportLocationsAsync(string filePath, int batchSize = 1000, Action<ImportProgress>? progressCallback = null);

    /// <summary>
    /// Clears all existing data from the database
    /// </summary>
    /// <returns>Number of records deleted</returns>
    Task<int> ClearAllDataAsync();

    /// <summary>
    /// Clears only zip codes from the database
    /// </summary>
    /// <returns>Number of zip codes deleted</returns>
    Task<int> ClearZipCodesAsync();

    /// <summary>
    /// Clears only locations from the database
    /// </summary>
    /// <returns>Number of locations deleted</returns>
    Task<int> ClearLocationsAsync();
}

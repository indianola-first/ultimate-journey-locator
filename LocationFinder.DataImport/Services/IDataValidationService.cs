using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for validating data in the LocationFinder database
/// </summary>
public interface IDataValidationService
{
    /// <summary>
    /// Validates all data in the database
    /// </summary>
    /// <returns>Validation result with statistics and issues</returns>
    Task<ValidationResult> ValidateDataAsync();

    /// <summary>
    /// Validates zip codes data specifically
    /// </summary>
    /// <returns>Validation result for zip codes</returns>
    Task<ValidationResult> ValidateZipCodesAsync();

    /// <summary>
    /// Validates locations data specifically
    /// </summary>
    /// <returns>Validation result for locations</returns>
    Task<ValidationResult> ValidateLocationsAsync();

    /// <summary>
    /// Checks for orphaned records (locations without corresponding zip codes)
    /// </summary>
    /// <returns>List of orphaned location zip codes</returns>
    Task<List<string>> FindOrphanedLocationZipCodesAsync();

    /// <summary>
    /// Checks for unused zip codes (zip codes not referenced by any location)
    /// </summary>
    /// <returns>List of unused zip codes</returns>
    Task<List<string>> FindUnusedZipCodesAsync();
}

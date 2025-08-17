using System.Text.Json;
using Microsoft.Extensions.Logging;
using LocationFinder.API.Models;
using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for reading zip code data from JSON files
/// </summary>
public class ZipCodeDataReader : IZipCodeDataReader
{
    private readonly ILogger<ZipCodeDataReader> _logger;

    public ZipCodeDataReader(ILogger<ZipCodeDataReader> logger)
    {
        _logger = logger;
    }

    public async Task<List<ZipCode>> ReadZipCodesAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Zip codes file not found: {filePath}");
        }

        _logger.LogInformation("Reading zip codes from {FilePath}", filePath);

        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var zipCodes = JsonSerializer.Deserialize<List<ZipCode>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (zipCodes == null)
            {
                throw new InvalidOperationException("Failed to deserialize zip codes from JSON file");
            }

            _logger.LogInformation("Successfully read {Count} zip codes from file", zipCodes.Count);

            // Validate the data
            var validationResult = await ValidateZipCodesAsync(zipCodes);
            if (validationResult.ValidationErrors.Any())
            {
                _logger.LogWarning("Zip codes validation found {ErrorCount} errors", validationResult.ValidationErrors.Count);
                foreach (var error in validationResult.ValidationErrors.Take(5))
                {
                    _logger.LogWarning("Validation error: {Error}", error);
                }
            }

            if (validationResult.ValidationWarnings.Any())
            {
                _logger.LogInformation("Zip codes validation found {WarningCount} warnings", validationResult.ValidationWarnings.Count);
                foreach (var warning in validationResult.ValidationWarnings.Take(5))
                {
                    _logger.LogInformation("Validation warning: {Warning}", warning);
                }
            }

            return zipCodes;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON file: {FilePath}", filePath);
            throw new InvalidOperationException($"Invalid JSON format in file: {filePath}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading zip codes file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<ValidationResult> ValidateZipCodesAsync(List<ZipCode> zipCodes)
    {
        var result = new ValidationResult();
        var errors = new List<string>();
        var warnings = new List<string>();

        if (zipCodes == null || !zipCodes.Any())
        {
            errors.Add("No zip codes provided for validation");
            result.ValidationErrors = errors;
            return result;
        }

        // Check for duplicates
        var duplicateZipCodes = zipCodes
            .GroupBy(z => z.ZipCodeValue)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateZipCodes.Any())
        {
            errors.Add($"Found {duplicateZipCodes.Count} duplicate zip codes: {string.Join(", ", duplicateZipCodes.Take(10))}");
        }

        // Validate each zip code
        for (int i = 0; i < zipCodes.Count; i++)
        {
            var zipCode = zipCodes[i];
            var recordNumber = i + 1;

            // Validate zip code format
            if (string.IsNullOrWhiteSpace(zipCode.ZipCodeValue))
            {
                errors.Add($"Record {recordNumber}: Zip code is null or empty");
                continue;
            }

            if (!IsValidZipCodeFormat(zipCode.ZipCodeValue))
            {
                errors.Add($"Record {recordNumber}: Invalid zip code format '{zipCode.ZipCodeValue}'");
            }

            // Validate city
            if (string.IsNullOrWhiteSpace(zipCode.City))
            {
                warnings.Add($"Record {recordNumber}: City is null or empty for zip code '{zipCode.ZipCodeValue}'");
            }

            // Validate state
            if (string.IsNullOrWhiteSpace(zipCode.State))
            {
                warnings.Add($"Record {recordNumber}: State is null or empty for zip code '{zipCode.ZipCodeValue}'");
            }
            else if (zipCode.State.Length != 2)
            {
                warnings.Add($"Record {recordNumber}: State should be 2 characters, found '{zipCode.State}' for zip code '{zipCode.ZipCodeValue}'");
            }

            // Validate coordinates
            if (zipCode.Latitude < -90 || zipCode.Latitude > 90)
            {
                errors.Add($"Record {recordNumber}: Invalid latitude {zipCode.Latitude} for zip code '{zipCode.ZipCodeValue}'");
            }

            if (zipCode.Longitude < -180 || zipCode.Longitude > 180)
            {
                errors.Add($"Record {recordNumber}: Invalid longitude {zipCode.Longitude} for zip code '{zipCode.ZipCodeValue}'");
            }

            // Check for extreme coordinates (likely data errors)
            if (zipCode.Latitude == 0 && zipCode.Longitude == 0)
            {
                warnings.Add($"Record {recordNumber}: Zip code '{zipCode.ZipCodeValue}' has coordinates (0,0) which may indicate missing data");
            }
        }

        // Check for missing coordinates
        var zipCodesWithoutCoordinates = zipCodes.Where(z => z.Latitude == 0 && z.Longitude == 0).Count();
        if (zipCodesWithoutCoordinates > 0)
        {
            warnings.Add($"{zipCodesWithoutCoordinates} zip codes have missing coordinates (0,0)");
        }

        // Check for unusual coordinate patterns
        var avgLatitude = zipCodes.Average(z => z.Latitude);
        var avgLongitude = zipCodes.Average(z => z.Longitude);
        
        if (Math.Abs(avgLatitude) > 60)
        {
            warnings.Add($"Average latitude ({avgLatitude:F2}) seems unusual - check data source");
        }

        if (Math.Abs(avgLongitude) > 150)
        {
            warnings.Add($"Average longitude ({avgLongitude:F2}) seems unusual - check data source");
        }

        result.ValidationErrors = errors;
        result.ValidationWarnings = warnings;

        return result;
    }

    private bool IsValidZipCodeFormat(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
            return false;

        // Remove any whitespace
        zipCode = zipCode.Trim();

        // Check for 5-digit format
        if (zipCode.Length == 5 && zipCode.All(char.IsDigit))
            return true;

        // Check for ZIP+4 format (5 digits + hyphen + 4 digits)
        if (zipCode.Length == 10 && zipCode[5] == '-' && 
            zipCode.Substring(0, 5).All(char.IsDigit) && 
            zipCode.Substring(6, 4).All(char.IsDigit))
            return true;

        return false;
    }
}

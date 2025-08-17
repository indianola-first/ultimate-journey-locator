using System.Text.Json;
using Microsoft.Extensions.Logging;
using LocationFinder.API.Models;
using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for reading location data from JSON files
/// </summary>
public class LocationDataReader : ILocationDataReader
{
    private readonly ILogger<LocationDataReader> _logger;

    public LocationDataReader(ILogger<LocationDataReader> logger)
    {
        _logger = logger;
    }

    public async Task<List<Location>> ReadLocationsAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Locations file not found: {filePath}");
        }

        _logger.LogInformation("Reading locations from {FilePath}", filePath);

        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var locations = JsonSerializer.Deserialize<List<Location>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (locations == null)
            {
                throw new InvalidOperationException("Failed to deserialize locations from JSON file");
            }

            _logger.LogInformation("Successfully read {Count} locations from file", locations.Count);

            // Validate the data
            var validationResult = await ValidateLocationsAsync(locations);
            if (validationResult.ValidationErrors.Any())
            {
                _logger.LogWarning("Locations validation found {ErrorCount} errors", validationResult.ValidationErrors.Count);
                foreach (var error in validationResult.ValidationErrors.Take(5))
                {
                    _logger.LogWarning("Validation error: {Error}", error);
                }
            }

            if (validationResult.ValidationWarnings.Any())
            {
                _logger.LogInformation("Locations validation found {WarningCount} warnings", validationResult.ValidationWarnings.Count);
                foreach (var warning in validationResult.ValidationWarnings.Take(5))
                {
                    _logger.LogInformation("Validation warning: {Warning}", warning);
                }
            }

            return locations;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON file: {FilePath}", filePath);
            throw new InvalidOperationException($"Invalid JSON format in file: {filePath}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading locations file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<ValidationResult> ValidateLocationsAsync(List<Location> locations)
    {
        var result = new ValidationResult();
        var errors = new List<string>();
        var warnings = new List<string>();

        if (locations == null || !locations.Any())
        {
            errors.Add("No locations provided for validation");
            result.ValidationErrors = errors;
            return result;
        }

        // Check for duplicates (same name and address)
        var duplicateLocations = locations
            .GroupBy(l => new { l.Name, l.Address })
            .Where(g => g.Count() > 1)
            .Select(g => new { g.Key.Name, g.Key.Address })
            .ToList();

        if (duplicateLocations.Any())
        {
            errors.Add($"Found {duplicateLocations.Count} duplicate locations (same name and address)");
            foreach (var duplicate in duplicateLocations.Take(5))
            {
                errors.Add($"  - {duplicate.Name} at {duplicate.Address}");
            }
        }

        // Validate each location
        for (int i = 0; i < locations.Count; i++)
        {
            var location = locations[i];
            var recordNumber = i + 1;

            // Validate required fields
            if (string.IsNullOrWhiteSpace(location.Name))
            {
                errors.Add($"Record {recordNumber}: Location name is null or empty");
            }

            if (string.IsNullOrWhiteSpace(location.Address))
            {
                errors.Add($"Record {recordNumber}: Address is null or empty");
            }

            if (string.IsNullOrWhiteSpace(location.City))
            {
                warnings.Add($"Record {recordNumber}: City is null or empty for location '{location.Name}'");
            }

            if (string.IsNullOrWhiteSpace(location.State))
            {
                warnings.Add($"Record {recordNumber}: State is null or empty for location '{location.Name}'");
            }
            else if (location.State.Length != 2)
            {
                warnings.Add($"Record {recordNumber}: State should be 2 characters, found '{location.State}' for location '{location.Name}'");
            }

            if (string.IsNullOrWhiteSpace(location.ZipCode))
            {
                warnings.Add($"Record {recordNumber}: Zip code is null or empty for location '{location.Name}'");
            }
            else if (!IsValidZipCodeFormat(location.ZipCode))
            {
                warnings.Add($"Record {recordNumber}: Invalid zip code format '{location.ZipCode}' for location '{location.Name}'");
            }

            // Validate phone number format
            if (!string.IsNullOrWhiteSpace(location.Phone) && !IsValidPhoneFormat(location.Phone))
            {
                warnings.Add($"Record {recordNumber}: Phone number format may be invalid '{location.Phone}' for location '{location.Name}'");
            }

            // Validate coordinates
            if (location.Latitude < -90 || location.Latitude > 90)
            {
                errors.Add($"Record {recordNumber}: Invalid latitude {location.Latitude} for location '{location.Name}'");
            }

            if (location.Longitude < -180 || location.Longitude > 180)
            {
                errors.Add($"Record {recordNumber}: Invalid longitude {location.Longitude} for location '{location.Name}'");
            }

            // Check for extreme coordinates (likely data errors)
            if (location.Latitude == 0 && location.Longitude == 0)
            {
                warnings.Add($"Record {recordNumber}: Location '{location.Name}' has coordinates (0,0) which may indicate missing data");
            }

            // Validate business hours format
            if (!string.IsNullOrWhiteSpace(location.BusinessHours))
            {
                if (location.BusinessHours.Length > 500)
                {
                    warnings.Add($"Record {recordNumber}: Business hours seem very long ({location.BusinessHours.Length} characters) for location '{location.Name}'");
                }
            }

            // Check for very long text fields
            if (location.Name.Length > 255)
            {
                errors.Add($"Record {recordNumber}: Location name is too long ({location.Name.Length} characters) for location '{location.Name.Substring(0, 50)}...'");
            }

            if (location.Address.Length > 255)
            {
                errors.Add($"Record {recordNumber}: Address is too long ({location.Address.Length} characters) for location '{location.Name}'");
            }

            if (location.City.Length > 100)
            {
                errors.Add($"Record {recordNumber}: City name is too long ({location.City.Length} characters) for location '{location.Name}'");
            }
        }

        // Check for missing coordinates
        var locationsWithoutCoordinates = locations.Where(l => l.Latitude == 0 || l.Longitude == 0).Count();
        if (locationsWithoutCoordinates > 0)
        {
            warnings.Add($"{locationsWithoutCoordinates} locations have missing or zero coordinates");
        }

        // Check for inactive locations
        var inactiveLocations = locations.Where(l => !l.IsActive).Count();
        if (inactiveLocations > 0)
        {
            warnings.Add($"{inactiveLocations} locations are marked as inactive");
        }

        // Check for unusual coordinate patterns
        var locationsWithCoordinates = locations.Where(l => l.Latitude != 0 && l.Longitude != 0).ToList();
        
        if (locationsWithCoordinates.Any())
        {
            var avgLatitude = locationsWithCoordinates.Average(l => l.Latitude);
            var avgLongitude = locationsWithCoordinates.Average(l => l.Longitude);
            
            if (Math.Abs(avgLatitude) > 60)
            {
                warnings.Add($"Average latitude ({avgLatitude:F2}) seems unusual - check data source");
            }

            if (Math.Abs(avgLongitude) > 150)
            {
                warnings.Add($"Average longitude ({avgLongitude:F2}) seems unusual - check data source");
            }
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

    private bool IsValidPhoneFormat(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Remove common phone number characters
        var cleaned = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace(".", "");

        // Check for 10 or 11 digit format
        if (cleaned.Length == 10 && cleaned.All(char.IsDigit))
            return true;

        if (cleaned.Length == 11 && cleaned.StartsWith("1") && cleaned.Substring(1).All(char.IsDigit))
            return true;

        return false;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LocationFinder.API.Data;
using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for validating data in the LocationFinder database
/// </summary>
public class DataValidationService : IDataValidationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataValidationService> _logger;

    public DataValidationService(ApplicationDbContext context, ILogger<DataValidationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateDataAsync()
    {
        _logger.LogInformation("Starting comprehensive data validation");

        var result = new ValidationResult();
        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            // Get basic counts
            result.ZipCodesCount = await _context.ZipCodes.CountAsync();
            result.LocationsCount = await _context.Locations.CountAsync();
            result.ActiveLocationsCount = await _context.Locations.Where(l => l.IsActive).CountAsync();

            _logger.LogInformation("Database contains {ZipCodesCount} zip codes, {LocationsCount} locations ({ActiveLocationsCount} active)", 
                result.ZipCodesCount, result.LocationsCount, result.ActiveLocationsCount);

            // Validate zip codes
            var zipCodeValidation = await ValidateZipCodesAsync();
            errors.AddRange(zipCodeValidation.ValidationErrors);
            warnings.AddRange(zipCodeValidation.ValidationWarnings);

            // Validate locations
            var locationValidation = await ValidateLocationsAsync();
            errors.AddRange(locationValidation.ValidationErrors);
            warnings.AddRange(locationValidation.ValidationWarnings);

            // Check for orphaned records
            var orphanedZipCodes = await FindOrphanedLocationZipCodesAsync();
            if (orphanedZipCodes.Any())
            {
                errors.Add($"Found {orphanedZipCodes.Count} locations with zip codes not in the zip codes table");
                foreach (var zipCode in orphanedZipCodes.Take(10))
                {
                    errors.Add($"  - Orphaned zip code: {zipCode}");
                }
                if (orphanedZipCodes.Count > 10)
                {
                    errors.Add($"  - ... and {orphanedZipCodes.Count - 10} more orphaned zip codes");
                }
            }

            // Check for unused zip codes
            var unusedZipCodes = await FindUnusedZipCodesAsync();
            if (unusedZipCodes.Any())
            {
                warnings.Add($"Found {unusedZipCodes.Count} zip codes not referenced by any location");
                if (unusedZipCodes.Count <= 20)
                {
                    foreach (var zipCode in unusedZipCodes)
                    {
                        warnings.Add($"  - Unused zip code: {zipCode}");
                    }
                }
                else
                {
                    warnings.Add($"  - First 20 unused zip codes: {string.Join(", ", unusedZipCodes.Take(20))}");
                    warnings.Add($"  - ... and {unusedZipCodes.Count - 20} more unused zip codes");
                }
            }

            // Check for data consistency issues
            var locationsWithoutCoordinates = await _context.Locations
                .Where(l => l.Latitude == 0 || l.Longitude == 0)
                .CountAsync();

            if (locationsWithoutCoordinates > 0)
            {
                warnings.Add($"{locationsWithoutCoordinates} locations have missing or zero coordinates");
            }

            var zipCodesWithoutCoordinates = await _context.ZipCodes
                .Where(z => z.Latitude == 0 || z.Longitude == 0)
                .CountAsync();

            if (zipCodesWithoutCoordinates > 0)
            {
                warnings.Add($"{zipCodesWithoutCoordinates} zip codes have zero coordinates");
            }

            // Check for duplicate locations
            var duplicateLocations = await _context.Locations
                .GroupBy(l => new { l.Name, l.Address })
                .Where(g => g.Count() > 1)
                .Select(g => new { g.Key.Name, g.Key.Address, Count = g.Count() })
                .ToListAsync();

            if (duplicateLocations.Any())
            {
                errors.Add($"Found {duplicateLocations.Count} duplicate location entries");
                foreach (var duplicate in duplicateLocations.Take(5))
                {
                    errors.Add($"  - {duplicate.Name} at {duplicate.Address} (appears {duplicate.Count} times)");
                }
            }

            // Check for duplicate zip codes
            var duplicateZipCodes = await _context.ZipCodes
                .GroupBy(z => z.ZipCodeValue)
                .Where(g => g.Count() > 1)
                .Select(g => new { ZipCode = g.Key, Count = g.Count() })
                .ToListAsync();

            if (duplicateZipCodes.Any())
            {
                errors.Add($"Found {duplicateZipCodes.Count} duplicate zip code entries");
                foreach (var duplicate in duplicateZipCodes.Take(5))
                {
                    errors.Add($"  - Zip code {duplicate.ZipCode} appears {duplicate.Count} times");
                }
            }

            result.ValidationErrors = errors;
            result.ValidationWarnings = warnings;

            _logger.LogInformation("Data validation completed: {ErrorCount} errors, {WarningCount} warnings", 
                errors.Count, warnings.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data validation");
            errors.Add($"Validation failed: {ex.Message}");
            result.ValidationErrors = errors;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateZipCodesAsync()
    {
        var result = new ValidationResult();
        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            var zipCodes = await _context.ZipCodes.ToListAsync();

            foreach (var zipCode in zipCodes)
            {
                // Validate zip code format
                if (string.IsNullOrWhiteSpace(zipCode.ZipCodeValue))
                {
                    errors.Add($"Zip code ID {zipCode.Id}: Zip code value is null or empty");
                    continue;
                }

                if (!IsValidZipCodeFormat(zipCode.ZipCodeValue))
                {
                    errors.Add($"Zip code ID {zipCode.Id}: Invalid format '{zipCode.ZipCodeValue}'");
                }

                // Validate coordinates
                if (zipCode.Latitude < -90 || zipCode.Latitude > 90)
                {
                    errors.Add($"Zip code {zipCode.ZipCodeValue}: Invalid latitude {zipCode.Latitude}");
                }

                if (zipCode.Longitude < -180 || zipCode.Longitude > 180)
                {
                    errors.Add($"Zip code {zipCode.ZipCodeValue}: Invalid longitude {zipCode.Longitude}");
                }

                // Check for zero coordinates
                if (zipCode.Latitude == 0 && zipCode.Longitude == 0)
                {
                    warnings.Add($"Zip code {zipCode.ZipCodeValue}: Has zero coordinates (0,0)");
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(zipCode.City))
                {
                    warnings.Add($"Zip code {zipCode.ZipCodeValue}: Missing city name");
                }

                if (string.IsNullOrWhiteSpace(zipCode.State))
                {
                    warnings.Add($"Zip code {zipCode.ZipCodeValue}: Missing state");
                }
                else if (zipCode.State.Length != 2)
                {
                    warnings.Add($"Zip code {zipCode.ZipCodeValue}: State should be 2 characters, found '{zipCode.State}'");
                }
            }

            result.ValidationErrors = errors;
            result.ValidationWarnings = warnings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating zip codes");
            errors.Add($"Zip code validation failed: {ex.Message}");
            result.ValidationErrors = errors;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateLocationsAsync()
    {
        var result = new ValidationResult();
        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            var locations = await _context.Locations.ToListAsync();

            foreach (var location in locations)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(location.Name))
                {
                    errors.Add($"Location ID {location.Id}: Missing name");
                }

                if (string.IsNullOrWhiteSpace(location.Address))
                {
                    errors.Add($"Location ID {location.Id}: Missing address");
                }

                            // Validate coordinates
            if (location.Latitude < -90 || location.Latitude > 90)
            {
                errors.Add($"Location {location.Name}: Invalid latitude {location.Latitude}");
            }

            if (location.Longitude < -180 || location.Longitude > 180)
            {
                errors.Add($"Location {location.Name}: Invalid longitude {location.Longitude}");
            }

            // Check for zero coordinates
            if (location.Latitude == 0 && location.Longitude == 0)
            {
                warnings.Add($"Location {location.Name}: Has zero coordinates (0,0)");
            }

                // Validate zip code format
                if (!string.IsNullOrWhiteSpace(location.ZipCode) && !IsValidZipCodeFormat(location.ZipCode))
                {
                    warnings.Add($"Location {location.Name}: Invalid zip code format '{location.ZipCode}'");
                }

                // Validate phone number format
                if (!string.IsNullOrWhiteSpace(location.Phone) && !IsValidPhoneFormat(location.Phone))
                {
                    warnings.Add($"Location {location.Name}: Phone number format may be invalid '{location.Phone}'");
                }

                // Check for very long text fields
                if (location.Name.Length > 255)
                {
                    errors.Add($"Location {location.Name}: Name is too long ({location.Name.Length} characters)");
                }

                if (location.Address.Length > 255)
                {
                    errors.Add($"Location {location.Name}: Address is too long ({location.Address.Length} characters)");
                }

                if (location.City.Length > 100)
                {
                    errors.Add($"Location {location.Name}: City name is too long ({location.City.Length} characters)");
                }

                // Validate state format
                if (!string.IsNullOrWhiteSpace(location.State) && location.State.Length != 2)
                {
                    warnings.Add($"Location {location.Name}: State should be 2 characters, found '{location.State}'");
                }
            }

            result.ValidationErrors = errors;
            result.ValidationWarnings = warnings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating locations");
            errors.Add($"Location validation failed: {ex.Message}");
            result.ValidationErrors = errors;
        }

        return result;
    }

    public async Task<List<string>> FindOrphanedLocationZipCodesAsync()
    {
        try
        {
            var orphanedZipCodes = await _context.Locations
                .Where(l => !string.IsNullOrWhiteSpace(l.ZipCode))
                .Select(l => l.ZipCode)
                .Distinct()
                .Where(zipCode => !_context.ZipCodes.Any(z => z.ZipCodeValue == zipCode))
                .ToListAsync();

            return orphanedZipCodes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding orphaned location zip codes");
            return new List<string>();
        }
    }

    public async Task<List<string>> FindUnusedZipCodesAsync()
    {
        try
        {
            var unusedZipCodes = await _context.ZipCodes
                .Where(z => !_context.Locations.Any(l => l.ZipCode == z.ZipCodeValue))
                .Select(z => z.ZipCodeValue)
                .ToListAsync();

            return unusedZipCodes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding unused zip codes");
            return new List<string>();
        }
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

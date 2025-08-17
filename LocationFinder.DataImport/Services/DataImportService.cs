using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LocationFinder.API.Data;
using LocationFinder.API.Models;
using LocationFinder.DataImport.Models;
using LocationFinder.DataImport.Services;

namespace LocationFinder.DataImport.Services;

/// <summary>
/// Service for importing data into the LocationFinder database
/// </summary>
public class DataImportService : IDataImportService
{
    private readonly ApplicationDbContext _context;
    private readonly IZipCodeDataReader _zipCodeReader;
    private readonly ILocationDataReader _locationReader;
    private readonly ILogger<DataImportService> _logger;

    public DataImportService(
        ApplicationDbContext context,
        IZipCodeDataReader zipCodeReader,
        ILocationDataReader locationReader,
        ILogger<DataImportService> logger)
    {
        _context = context;
        _zipCodeReader = zipCodeReader;
        _locationReader = locationReader;
        _logger = logger;
    }

    public async Task<ImportResult> ImportZipCodesAsync(string filePath, int batchSize = 1000, Action<ImportProgress>? progressCallback = null)
    {
        var result = new ImportResult();
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation("Starting zip codes import from {FilePath}", filePath);

            // Read zip codes from JSON file
            var zipCodes = await _zipCodeReader.ReadZipCodesAsync(filePath);
            result.TotalProcessed = zipCodes.Count;

            _logger.LogInformation("Read {Count} zip codes from file", zipCodes.Count);

            // Process in batches
            var batches = zipCodes.Chunk(batchSize).ToList();
            var progress = new ImportProgress
            {
                TotalRecords = zipCodes.Count,
                TotalBatches = batches.Count
            };

            foreach (var (batch, batchIndex) in batches.Select((batch, index) => (batch, index)))
            {
                progress.CurrentBatch = batchIndex + 1;
                progress.CurrentRecord = (batchIndex * batchSize) + batch.Count();

                try
                {
                    // Check for existing zip codes to avoid duplicates
                    var existingZipCodes = await _context.ZipCodes
                        .Where(z => batch.Select(b => b.ZipCodeValue).Contains(z.ZipCodeValue))
                        .Select(z => z.ZipCodeValue)
                        .ToListAsync();

                    var newZipCodes = batch.Where(z => !existingZipCodes.Contains(z.ZipCodeValue)).ToList();
                    var skippedCount = batch.Count() - newZipCodes.Count;

                    if (newZipCodes.Any())
                    {
                        await _context.ZipCodes.AddRangeAsync(newZipCodes);
                        await _context.SaveChangesAsync();

                        result.SuccessCount += newZipCodes.Count;
                        progress.SuccessCount += newZipCodes.Count;

                        _logger.LogDebug("Batch {BatchNumber}: Added {AddedCount} zip codes, skipped {SkippedCount} duplicates", 
                            batchIndex + 1, newZipCodes.Count, skippedCount);
                    }
                    else
                    {
                        _logger.LogDebug("Batch {BatchNumber}: All {Count} zip codes already exist, skipped", 
                            batchIndex + 1, batch.Count());
                    }

                    // Report progress
                    progressCallback?.Invoke(progress);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Error processing batch {batchIndex + 1}: {ex.Message}";
                    result.Errors.Add(errorMessage);
                    result.FailedCount += batch.Count();
                    progress.FailedCount += batch.Count();
                    _logger.LogError(ex, "Error processing zip codes batch {BatchNumber}", batchIndex + 1);
                }
            }

            result.Duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Zip codes import completed: {SuccessCount} imported, {FailedCount} failed, duration: {Duration}", 
                result.SuccessCount, result.FailedCount, result.Duration);
        }
        catch (Exception ex)
        {
            result.Duration = DateTime.UtcNow - startTime;
            result.Errors.Add($"Import failed: {ex.Message}");
            _logger.LogError(ex, "Zip codes import failed");
        }

        return result;
    }

    public async Task<ImportResult> ImportLocationsAsync(string filePath, int batchSize = 1000, Action<ImportProgress>? progressCallback = null)
    {
        var result = new ImportResult();
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogInformation("Starting locations import from {FilePath}", filePath);

            // Read locations from JSON file
            var locations = await _locationReader.ReadLocationsAsync(filePath);
            result.TotalProcessed = locations.Count;

            _logger.LogInformation("Read {Count} locations from file", locations.Count);

            // Process in batches
            var batches = locations.Chunk(batchSize).ToList();
            var progress = new ImportProgress
            {
                TotalRecords = locations.Count,
                TotalBatches = batches.Count
            };

            foreach (var (batch, batchIndex) in batches.Select((batch, index) => (batch, index)))
            {
                progress.CurrentBatch = batchIndex + 1;
                progress.CurrentRecord = (batchIndex * batchSize) + batch.Count();

                try
                {
                    // Check for existing locations to avoid duplicates
                    var existingLocations = await _context.Locations
                        .Where(l => batch.Select(b => b.Name).Contains(l.Name) && 
                                   batch.Select(b => b.Address).Contains(l.Address))
                        .Select(l => new { l.Name, l.Address })
                        .ToListAsync();

                    var newLocations = batch.Where(l => 
                        !existingLocations.Any(existing => 
                            existing.Name == l.Name && existing.Address == l.Address)).ToList();
                    var skippedCount = batch.Count() - newLocations.Count;

                    if (newLocations.Any())
                    {
                        await _context.Locations.AddRangeAsync(newLocations);
                        await _context.SaveChangesAsync();

                        result.SuccessCount += newLocations.Count;
                        progress.SuccessCount += newLocations.Count;

                        _logger.LogDebug("Batch {BatchNumber}: Added {AddedCount} locations, skipped {SkippedCount} duplicates", 
                            batchIndex + 1, newLocations.Count, skippedCount);
                    }
                    else
                    {
                        _logger.LogDebug("Batch {BatchNumber}: All {Count} locations already exist, skipped", 
                            batchIndex + 1, batch.Count());
                    }

                    // Report progress
                    progressCallback?.Invoke(progress);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Error processing batch {batchIndex + 1}: {ex.Message}";
                    result.Errors.Add(errorMessage);
                    result.FailedCount += batch.Count();
                    progress.FailedCount += batch.Count();
                    _logger.LogError(ex, "Error processing locations batch {BatchNumber}", batchIndex + 1);
                }
            }

            result.Duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Locations import completed: {SuccessCount} imported, {FailedCount} failed, duration: {Duration}", 
                result.SuccessCount, result.FailedCount, result.Duration);
        }
        catch (Exception ex)
        {
            result.Duration = DateTime.UtcNow - startTime;
            result.Errors.Add($"Import failed: {ex.Message}");
            _logger.LogError(ex, "Locations import failed");
        }

        return result;
    }

    public async Task<int> ClearAllDataAsync()
    {
        _logger.LogWarning("Clearing all data from database");
        
        var locationsDeleted = await _context.Locations.ExecuteDeleteAsync();
        var zipCodesDeleted = await _context.ZipCodes.ExecuteDeleteAsync();
        
        var totalDeleted = locationsDeleted + zipCodesDeleted;
        _logger.LogInformation("Cleared {TotalDeleted} records ({LocationsDeleted} locations, {ZipCodesDeleted} zip codes)", 
            totalDeleted, locationsDeleted, zipCodesDeleted);
        
        return totalDeleted;
    }

    public async Task<int> ClearZipCodesAsync()
    {
        _logger.LogWarning("Clearing all zip codes from database");
        
        var deleted = await _context.ZipCodes.ExecuteDeleteAsync();
        _logger.LogInformation("Cleared {Count} zip codes", deleted);
        
        return deleted;
    }

    public async Task<int> ClearLocationsAsync()
    {
        _logger.LogWarning("Clearing all locations from database");
        
        var deleted = await _context.Locations.ExecuteDeleteAsync();
        _logger.LogInformation("Cleared {Count} locations", deleted);
        
        return deleted;
    }
}

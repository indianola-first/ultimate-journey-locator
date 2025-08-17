namespace LocationFinder.DataImport.Models;

/// <summary>
/// Result of a data import operation
/// </summary>
public class ImportResult
{
    /// <summary>
    /// Total number of records processed
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Number of records successfully imported
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of records that failed to import
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Duration of the import operation
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// List of error messages from failed imports
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of warning messages
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Whether the import was successful
    /// </summary>
    public bool IsSuccess => Errors.Count == 0;

    /// <summary>
    /// Success rate as a percentage
    /// </summary>
    public double SuccessRate => TotalProcessed > 0 ? (double)SuccessCount / TotalProcessed * 100 : 0;
}

/// <summary>
/// Result of data validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Number of zip codes in the database
    /// </summary>
    public int ZipCodesCount { get; set; }

    /// <summary>
    /// Number of locations in the database
    /// </summary>
    public int LocationsCount { get; set; }

    /// <summary>
    /// Number of active locations
    /// </summary>
    public int ActiveLocationsCount { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> ValidationWarnings { get; set; } = new();

    /// <summary>
    /// Whether the data is valid
    /// </summary>
    public bool IsValid => ValidationErrors.Count == 0;
}

/// <summary>
/// Progress information for import operations
/// </summary>
public class ImportProgress
{
    /// <summary>
    /// Current record number being processed
    /// </summary>
    public int CurrentRecord { get; set; }

    /// <summary>
    /// Total number of records to process
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Number of records successfully processed
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of records that failed
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Current batch number
    /// </summary>
    public int CurrentBatch { get; set; }

    /// <summary>
    /// Total number of batches
    /// </summary>
    public int TotalBatches { get; set; }

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public double ProgressPercentage => TotalRecords > 0 ? (double)CurrentRecord / TotalRecords * 100 : 0;

    /// <summary>
    /// Success rate as a percentage
    /// </summary>
    public double SuccessRate => CurrentRecord > 0 ? (double)SuccessCount / CurrentRecord * 100 : 0;
}

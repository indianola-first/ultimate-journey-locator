using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using LocationFinder.DataImport.Services;
using LocationFinder.DataImport.Models;

namespace LocationFinder.DataImport;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("LocationFinder Data Import Tool");

        // Import zip codes command
        var zipCodesCommand = new Command("zipcodes", "Import zip codes from JSON file");
        var zipCodesFileOption = new Option<FileInfo>(
            "--file",
            "JSON file containing zip codes data")
        {
            IsRequired = true
        };
        zipCodesFileOption.AddAlias("-f");
        zipCodesCommand.AddOption(zipCodesFileOption);

        var zipCodesBatchSizeOption = new Option<int>(
            "--batch-size",
            () => 1000,
            "Number of records to process in each batch");
        zipCodesBatchSizeOption.AddAlias("-b");
        zipCodesCommand.AddOption(zipCodesBatchSizeOption);

        zipCodesCommand.SetHandler(async (file, batchSize) =>
        {
            await ImportZipCodes(file, batchSize);
        }, zipCodesFileOption, zipCodesBatchSizeOption);

        // Import locations command
        var locationsCommand = new Command("locations", "Import locations from JSON file");
        var locationsFileOption = new Option<FileInfo>(
            "--file",
            "JSON file containing locations data")
        {
            IsRequired = true
        };
        locationsFileOption.AddAlias("-f");
        locationsCommand.AddOption(locationsFileOption);

        var locationsBatchSizeOption = new Option<int>(
            "--batch-size",
            () => 1000,
            "Number of records to process in each batch");
        locationsBatchSizeOption.AddAlias("-b");
        locationsCommand.AddOption(locationsBatchSizeOption);

        locationsCommand.SetHandler(async (file, batchSize) =>
        {
            await ImportLocations(file, batchSize);
        }, locationsFileOption, locationsBatchSizeOption);

        // Validate data command
        var validateCommand = new Command("validate", "Validate existing data in database");
        validateCommand.SetHandler(async () =>
        {
            await ValidateData();
        });

        rootCommand.AddCommand(zipCodesCommand);
        rootCommand.AddCommand(locationsCommand);
        rootCommand.AddCommand(validateCommand);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task ImportZipCodes(FileInfo file, int batchSize)
    {
        try
        {
            var services = ConfigureServices();
            var importService = services.GetRequiredService<IDataImportService>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting zip codes import from {File}", file.FullName);
            logger.LogInformation("Batch size: {BatchSize}", batchSize);

            var result = await importService.ImportZipCodesAsync(file.FullName, batchSize);

            logger.LogInformation("Zip codes import completed:");
            logger.LogInformation("- Total records processed: {TotalProcessed}", result.TotalProcessed);
            logger.LogInformation("- Successfully imported: {SuccessCount}", result.SuccessCount);
            logger.LogInformation("- Failed records: {FailedCount}", result.FailedCount);
            logger.LogInformation("- Duration: {Duration}", result.Duration);

            if (result.Errors.Any())
            {
                logger.LogWarning("Import completed with {ErrorCount} errors:", result.Errors.Count);
                foreach (var error in result.Errors.Take(10))
                {
                    logger.LogWarning("- {Error}", error);
                }
                if (result.Errors.Count > 10)
                {
                    logger.LogWarning("- ... and {MoreErrors} more errors", result.Errors.Count - 10);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing zip codes: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static async Task ImportLocations(FileInfo file, int batchSize)
    {
        try
        {
            var services = ConfigureServices();
            var importService = services.GetRequiredService<IDataImportService>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting locations import from {File}", file.FullName);
            logger.LogInformation("Batch size: {BatchSize}", batchSize);

            var result = await importService.ImportLocationsAsync(file.FullName, batchSize);

            logger.LogInformation("Locations import completed:");
            logger.LogInformation("- Total records processed: {TotalProcessed}", result.TotalProcessed);
            logger.LogInformation("- Successfully imported: {SuccessCount}", result.SuccessCount);
            logger.LogInformation("- Failed records: {FailedCount}", result.FailedCount);
            logger.LogInformation("- Duration: {Duration}", result.Duration);

            if (result.Errors.Any())
            {
                logger.LogWarning("Import completed with {ErrorCount} errors:", result.Errors.Count);
                foreach (var error in result.Errors.Take(10))
                {
                    logger.LogWarning("- {Error}", error);
                }
                if (result.Errors.Count > 10)
                {
                    logger.LogWarning("- ... and {MoreErrors} more errors", result.Errors.Count - 10);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing locations: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static async Task ValidateData()
    {
        try
        {
            var services = ConfigureServices();
            var validationService = services.GetRequiredService<IDataValidationService>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting data validation...");

            var result = await validationService.ValidateDataAsync();

            logger.LogInformation("Data validation completed:");
            logger.LogInformation("- Zip codes count: {ZipCodesCount}", result.ZipCodesCount);
            logger.LogInformation("- Locations count: {LocationsCount}", result.LocationsCount);
            logger.LogInformation("- Active locations: {ActiveLocationsCount}", result.ActiveLocationsCount);
            logger.LogInformation("- Validation errors: {ValidationErrorsCount}", result.ValidationErrors.Count);

            if (result.ValidationErrors.Any())
            {
                logger.LogWarning("Validation found {ErrorCount} issues:", result.ValidationErrors.Count);
                foreach (var error in result.ValidationErrors.Take(10))
                {
                    logger.LogWarning("- {Error}", error);
                }
                if (result.ValidationErrors.Count > 10)
                {
                    logger.LogWarning("- ... and {MoreErrors} more issues", result.ValidationErrors.Count - 10);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating data: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static IServiceProvider ConfigureServices()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<IConfiguration>(configuration);
        services.AddScoped<IDataImportService, DataImportService>();
        services.AddScoped<IDataValidationService, DataValidationService>();
        services.AddScoped<IZipCodeDataReader, ZipCodeDataReader>();
        services.AddScoped<ILocationDataReader, LocationDataReader>();

        // Configure Entity Framework
        services.AddDbContext<LocationFinder.API.Data.ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        });

        return services.BuildServiceProvider();
    }
}

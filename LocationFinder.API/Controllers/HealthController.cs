using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LocationFinder.API.Data;
using System.Diagnostics;

namespace LocationFinder.API.Controllers;

/// <summary>
/// Health check endpoints for monitoring application status
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        ApplicationDbContext context,
        ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Application status</returns>
    [HttpGet]
    public IActionResult Get()
    {
        var healthInfo = new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        };

        _logger.LogInformation("Health check requested at {Timestamp}", healthInfo.timestamp);
        return Ok(healthInfo);
    }

    /// <summary>
    /// Detailed health check with database connectivity
    /// </summary>
    /// <returns>Detailed health status including database connectivity</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        var stopwatch = Stopwatch.StartNew();
        var checks = new Dictionary<string, object>();

        try
        {
            // Database connectivity check
            var dbCheck = await CheckDatabaseHealth();
            checks["database"] = dbCheck;

            // Memory usage check
            var memoryCheck = CheckMemoryHealth();
            checks["memory"] = memoryCheck;

            // Disk space check
            var diskCheck = CheckDiskHealth();
            checks["disk"] = diskCheck;

            // Overall status determination
            var allChecks = new[] { dbCheck, memoryCheck, diskCheck };
            var overallStatus = "healthy";
            if (allChecks.Any(c => GetStatusFromCheck(c) == "unhealthy"))
            {
                overallStatus = "unhealthy";
                Response.StatusCode = 503; // Service Unavailable
            }

            stopwatch.Stop();
            checks["responseTime"] = new { value = stopwatch.ElapsedMilliseconds, unit = "ms" };

            var healthStatus = new
            {
                status = overallStatus,
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                checks = checks
            };

            _logger.LogInformation("Detailed health check completed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            checks["error"] = new { message = ex.Message };

            var healthStatus = new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                checks = checks
            };

            Response.StatusCode = 503;
            return Ok(healthStatus);
        }
    }

    /// <summary>
    /// Database-specific health check
    /// </summary>
    /// <returns>Database health status</returns>
    [HttpGet("database")]
    public async Task<IActionResult> GetDatabaseHealth()
    {
        var result = await CheckDatabaseHealth();

        if (GetStatusFromCheck(result) == "healthy")
        {
            return Ok(result);
        }
        else
        {
            Response.StatusCode = 503;
            return Ok(result);
        }
    }

    /// <summary>
    /// System resources health check
    /// </summary>
    /// <returns>System resources status</returns>
    [HttpGet("system")]
    public IActionResult GetSystemHealth()
    {
        var checks = new Dictionary<string, object>
        {
            ["memory"] = CheckMemoryHealth(),
            ["disk"] = CheckDiskHealth(),
            ["cpu"] = CheckCpuHealth()
        };

        var overallStatus = "healthy";
        if (checks.Values.Any(c => GetStatusFromCheck(c) == "unhealthy"))
        {
            overallStatus = "unhealthy";
            Response.StatusCode = 503;
        }

        var systemHealth = new
        {
            status = overallStatus,
            timestamp = DateTime.UtcNow,
            checks = checks
        };

        return Ok(systemHealth);
    }

    /// <summary>
    /// Ready check for load balancers
    /// </summary>
    /// <returns>Application readiness status</returns>
    [HttpGet("ready")]
    public async Task<IActionResult> GetReady()
    {
        try
        {
            // Check if database is accessible
            await _context.Database.CanConnectAsync();

            return Ok(new
            {
                status = "ready",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Application not ready");
            Response.StatusCode = 503;
            return Ok(new
            {
                status = "not_ready",
                timestamp = DateTime.UtcNow,
                reason = "Database connection failed"
            });
        }
    }

    /// <summary>
    /// Live check for load balancers
    /// </summary>
    /// <returns>Application liveness status</returns>
    [HttpGet("live")]
    public IActionResult GetLive()
    {
        return Ok(new
        {
            status = "alive",
            timestamp = DateTime.UtcNow
        });
    }

    private async Task<object> CheckDatabaseHealth()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            // Test database connection
            var canConnect = await _context.Database.CanConnectAsync();

            // Get basic statistics
            var zipCodeCount = await _context.ZipCodes.CountAsync();
            var locationCount = await _context.Locations.CountAsync();
            var activeLocationCount = await _context.Locations.Where(l => l.IsActive).CountAsync();

            stopwatch.Stop();

            return new
            {
                status = canConnect ? "healthy" : "unhealthy",
                connection = canConnect ? "connected" : "disconnected",
                responseTime = stopwatch.ElapsedMilliseconds,
                statistics = new
                {
                    zipCodes = zipCodeCount,
                    locations = locationCount,
                    activeLocations = activeLocationCount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return new
            {
                status = "unhealthy",
                connection = "error",
                error = ex.Message
            };
        }
    }

    private object CheckMemoryHealth()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var memoryUsageMB = process.WorkingSet64 / 1024 / 1024;
            var memoryLimitMB = 512; // 512MB limit for health check

            return new
            {
                status = memoryUsageMB < memoryLimitMB ? "healthy" : "warning",
                usageMB = memoryUsageMB,
                limitMB = memoryLimitMB,
                percentage = Math.Round((double)memoryUsageMB / memoryLimitMB * 100, 2)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Memory health check failed");
            return new
            {
                status = "unhealthy",
                error = ex.Message
            };
        }
    }

    private object CheckDiskHealth()
    {
        try
        {
            var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory) ?? "C:\\");
            var freeSpaceGB = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
            var totalSpaceGB = drive.TotalSize / 1024 / 1024 / 1024;
            var usedSpaceGB = totalSpaceGB - freeSpaceGB;
            var usagePercentage = Math.Round((double)usedSpaceGB / totalSpaceGB * 100, 2);

            return new
            {
                status = usagePercentage < 90 ? "healthy" : "warning",
                freeSpaceGB = freeSpaceGB,
                totalSpaceGB = totalSpaceGB,
                usedSpaceGB = usedSpaceGB,
                usagePercentage = usagePercentage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Disk health check failed");
            return new
            {
                status = "unhealthy",
                error = ex.Message
            };
        }
    }

    private object CheckCpuHealth()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var cpuTime = process.TotalProcessorTime;
            var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            var cpuPercentage = Math.Round((cpuTime.TotalMilliseconds / (Environment.ProcessorCount * uptime.TotalMilliseconds)) * 100, 2);

            return new
            {
                status = cpuPercentage < 80 ? "healthy" : "warning",
                cpuPercentage = cpuPercentage,
                processorCount = Environment.ProcessorCount,
                uptime = uptime.TotalMinutes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CPU health check failed");
            return new
            {
                status = "unhealthy",
                error = ex.Message
            };
        }
    }

    private string GetStatusFromCheck(object check)
    {
        // Use reflection to get the status property from the anonymous type
        var statusProperty = check.GetType().GetProperty("status");
        return statusProperty?.GetValue(check)?.ToString() ?? "unknown";
    }
}

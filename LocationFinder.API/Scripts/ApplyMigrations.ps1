# Location Finder Database Migration Script
# This script applies Entity Framework migrations for different environments

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Development", "Production")]
    [string]$Environment = "Development"
)

Write-Host "Applying migrations for environment: $Environment" -ForegroundColor Green

# Set the environment variable for the migration
$env:ASPNETCORE_ENVIRONMENT = $Environment

# Navigate to the API project directory
Set-Location $PSScriptRoot\..

try {
    # Update database with migrations
    Write-Host "Applying Entity Framework migrations..." -ForegroundColor Yellow
    dotnet ef database update --verbose
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Migrations applied successfully!" -ForegroundColor Green
        
        # Verify the database setup
        Write-Host "Verifying database setup..." -ForegroundColor Yellow
        
        # Check if we can connect to the database
        dotnet ef dbcontext info
        
        Write-Host "✓ Database setup verification complete!" -ForegroundColor Green
    } else {
        Write-Host "✗ Migration failed!" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "Error applying migrations: $_" -ForegroundColor Red
    exit 1
}
finally {
    # Reset environment variable
    $env:ASPNETCORE_ENVIRONMENT = ""
}

Write-Host "Migration process completed!" -ForegroundColor Green

# Entity Framework Migrations

This directory contains Entity Framework migrations for the Location Finder database.

## Migration Files

- **20250816215448_InitialCreate.cs** - Initial migration creating the database schema
- **ApplicationDbContextModelSnapshot.cs** - Current model snapshot
- **20250816215448_InitialCreate.Designer.cs** - Migration designer file

## Database Schema

### Tables Created

1. **ZipCodes** - Stores zip code coordinates and location information
   - Primary key: Id (auto-increment)
   - Unique index: ZipCode (5 characters)
   - Columns: ZipCode, Latitude, Longitude, City, State

2. **Locations** - Stores business location information
   - Primary key: Id (auto-increment)
   - Indexes: ZipCode, IsActive, Latitude_Longitude (composite)
   - Columns: Name, Address, City, State, ZipCode, Phone, Latitude, Longitude, BusinessHours, IsActive, CreatedDate

### Indexes (Performance Optimization)

As specified in the README.md:

```sql
-- ZipCodes table indexes
CREATE INDEX IX_ZipCodes_ZipCode ON ZipCodes(ZipCode);

-- Locations table indexes  
CREATE INDEX IX_Locations_ZipCode ON Locations(ZipCode);
CREATE INDEX IX_Locations_IsActive ON Locations(IsActive);
CREATE INDEX IX_Locations_Latitude_Longitude ON Locations(Latitude, Longitude);
```

## Sample Data

The migration includes seeded data:

- **15 Zip Codes**: Major US cities (NY, LA, Chicago, Houston, Miami, etc.)
- **10 Locations**: Sample business locations with realistic addresses and coordinates

## Applying Migrations

### Development Environment

```bash
# Using bash script (Unix/macOS)
./Scripts/apply-migrations.sh

# Using PowerShell script (Windows)
.\Scripts\ApplyMigrations.ps1

# Using dotnet CLI directly
dotnet ef database update
```

### Production Environment

```bash
# Using bash script
./Scripts/apply-migrations.sh Production

# Using PowerShell script
.\Scripts\ApplyMigrations.ps1 -Environment Production

# Using dotnet CLI directly
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
```

### Verification

After applying migrations, verify the setup:

```bash
# Check database context info
dotnet ef dbcontext info

# Run verification SQL script
sqlcmd -S localhost -d LocationFinderDB -i Scripts/ApplyMigrations.sql
```

## Connection Strings

### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LocationFinderDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Production (SmarterASP.Net)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SQL_SERVER_ADDRESS;Initial Catalog=DATABASE_NAME;User ID=DB_USERNAME;Password=DB_PASSWORD;TrustServerCertificate=true;"
  }
}
```

## Migration Commands

### Create New Migration
```bash
dotnet ef migrations add MigrationName
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

### Update Database
```bash
dotnet ef database update
```

### Generate SQL Script
```bash
dotnet ef migrations script
```

### Reset Database
```bash
dotnet ef database drop
dotnet ef database update
```

## Troubleshooting

### Common Issues

1. **Design-time factory not found**: Ensure `DesignTimeDbContextFactory.cs` exists
2. **Connection string issues**: Verify appsettings.json configuration
3. **Permission errors**: Ensure database user has appropriate permissions
4. **Migration conflicts**: Use `dotnet ef migrations remove` to revert changes

### Verification Steps

1. Check if tables exist: `SELECT * FROM sys.tables`
2. Verify indexes: `SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('TableName')`
3. Check sample data: `SELECT COUNT(*) FROM ZipCodes; SELECT COUNT(*) FROM Locations`
4. Test coordinates: `SELECT TOP 5 ZipCode, Latitude, Longitude FROM ZipCodes`

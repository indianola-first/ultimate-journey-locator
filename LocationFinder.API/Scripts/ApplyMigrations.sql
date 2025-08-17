-- Location Finder Database Migration Script
-- This script applies the initial migration and verifies the setup

-- Verify database exists, create if not
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'LocationFinderDB')
BEGIN
    CREATE DATABASE LocationFinderDB;
END
GO

USE LocationFinderDB;
GO

-- Verify tables exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ZipCodes')
BEGIN
    PRINT 'ZipCodes table does not exist. Please run Entity Framework migration first.';
    RETURN;
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Locations')
BEGIN
    PRINT 'Locations table does not exist. Please run Entity Framework migration first.';
    RETURN;
END

-- Verify indexes exist (as specified in README)
PRINT 'Verifying database indexes...';

-- Check ZipCodes indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ZipCodes_ZipCode' AND object_id = OBJECT_ID('ZipCodes'))
BEGIN
    PRINT 'WARNING: IX_ZipCodes_ZipCode index missing';
END
ELSE
BEGIN
    PRINT '✓ IX_ZipCodes_ZipCode index exists';
END

-- Check Locations indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Locations_ZipCode' AND object_id = OBJECT_ID('Locations'))
BEGIN
    PRINT 'WARNING: IX_Locations_ZipCode index missing';
END
ELSE
BEGIN
    PRINT '✓ IX_Locations_ZipCode index exists';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Locations_IsActive' AND object_id = OBJECT_ID('Locations'))
BEGIN
    PRINT 'WARNING: IX_Locations_IsActive index missing';
END
ELSE
BEGIN
    PRINT '✓ IX_Locations_IsActive index exists';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Locations_Latitude_Longitude' AND object_id = OBJECT_ID('Locations'))
BEGIN
    PRINT 'WARNING: IX_Locations_Latitude_Longitude index missing';
END
ELSE
BEGIN
    PRINT '✓ IX_Locations_Latitude_Longitude index exists';
END

-- Verify sample data
PRINT 'Verifying sample data...';
SELECT COUNT(*) as ZipCodeCount FROM ZipCodes;
SELECT COUNT(*) as LocationCount FROM Locations;

-- Show sample data
PRINT 'Sample ZipCodes:';
SELECT TOP 5 ZipCode, City, State, Latitude, Longitude FROM ZipCodes;

PRINT 'Sample Locations:';
SELECT TOP 5 Name, City, State, ZipCode, IsActive FROM Locations WHERE IsActive = 1;

PRINT 'Database setup verification complete.';

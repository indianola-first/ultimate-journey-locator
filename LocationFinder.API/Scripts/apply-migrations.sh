#!/bin/bash

# Location Finder Database Migration Script
# This script applies Entity Framework migrations for different environments

# Default to Development environment
ENVIRONMENT=${1:-Development}

echo "Applying migrations for environment: $ENVIRONMENT"

# Set the environment variable for the migration
export ASPNETCORE_ENVIRONMENT=$ENVIRONMENT

# Navigate to the API project directory
cd "$(dirname "$0")/.."

# Apply Entity Framework migrations
echo "Applying Entity Framework migrations..."
dotnet ef database update --verbose

if [ $? -eq 0 ]; then
    echo "✓ Migrations applied successfully!"
    
    # Verify the database setup
    echo "Verifying database setup..."
    dotnet ef dbcontext info
    
    echo "✓ Database setup verification complete!"
else
    echo "✗ Migration failed!"
    exit 1
fi

# Reset environment variable
unset ASPNETCORE_ENVIRONMENT

echo "Migration process completed!"

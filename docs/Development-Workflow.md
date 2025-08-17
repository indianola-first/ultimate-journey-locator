# LocationFinder Development Workflow

## Overview

This document outlines the development workflow, best practices, and collaboration guidelines for the LocationFinder project. It covers the complete development lifecycle from initial setup to deployment.

## 🏗️ Development Environment Setup

### Prerequisites Installation

#### 1. Install Required Software

```bash
# Install .NET 9 SDK
# Download from: https://dotnet.microsoft.com/download/dotnet/9.0

# Install Node.js 18+
# Download from: https://nodejs.org/

# Install Git
# Download from: https://git-scm.com/

# Install Visual Studio 2022 or VS Code
# Download from: https://visualstudio.microsoft.com/ or https://code.visualstudio.com/
```

#### 2. Verify Installation

```bash
# Verify .NET installation
dotnet --version

# Verify Node.js installation
node --version
npm --version

# Verify Git installation
git --version
```

### Project Setup

#### 1. Clone Repository

```bash
git clone <repository-url>
cd LocationFinder
```

#### 2. Install Dependencies

```bash
# Install .NET dependencies
dotnet restore

# Install Angular dependencies
cd LocationFinder.Client
npm install
cd ..
```

#### 3. Database Setup

```bash
# Update connection string in appsettings.Development.json
# Run database migrations
cd LocationFinder.API
dotnet ef database update
cd ..

# Import sample data
cd LocationFinder.DataImport
dotnet run -- zipcodes --file Data/sample-zipcodes.json
dotnet run -- locations --file Data/sample-locations.json
cd ..
```

## 🔄 Development Workflow

### 1. Feature Development Process

#### Step 1: Create Feature Branch

```bash
# Ensure you're on main branch and it's up to date
git checkout main
git pull origin main

# Create feature branch
git checkout -b feature/your-feature-name

# Example: git checkout -b feature/add-distance-filter
```

#### Step 2: Development

```bash
# Start development servers
# Terminal 1: API
cd LocationFinder.API
dotnet watch run

# Terminal 2: Angular (if needed)
cd LocationFinder.Client
ng serve
```

#### Step 3: Testing

```bash
# Run backend tests
cd LocationFinder.API.Tests
dotnet test

# Run frontend tests
cd LocationFinder.Client
ng test
ng e2e
```

#### Step 4: Commit Changes

```bash
# Stage changes
git add .

# Commit with descriptive message
git commit -m "feat: add distance filter to location search

- Add maxDistance parameter to search endpoint
- Update frontend to include distance filter
- Add validation for distance parameter
- Update tests for new functionality"

# Push to remote
git push origin feature/your-feature-name
```

#### Step 5: Create Pull Request

1. Go to GitHub/GitLab repository
2. Create new Pull Request
3. Fill in PR template
4. Request code review
5. Address review comments
6. Merge when approved

### 2. Bug Fix Process

#### Step 1: Create Bug Branch

```bash
git checkout main
git pull origin main
git checkout -b bugfix/describe-the-bug

# Example: git checkout -b bugfix/fix-zipcode-validation
```

#### Step 2: Fix and Test

```bash
# Make necessary changes
# Write/update tests
# Test thoroughly

# Run all tests
dotnet test
ng test
```

#### Step 3: Commit Fix

```bash
git add .
git commit -m "fix: correct zip code validation regex

- Update regex pattern to handle ZIP+4 format
- Add test cases for edge cases
- Fix validation error message

Fixes #123"
```

### 3. Database Changes

#### Creating Migrations

```bash
# Make changes to models
# Create migration
cd LocationFinder.API
dotnet ef migrations add DescriptiveMigrationName

# Example: dotnet ef migrations add AddLocationPhoneNumber
```

#### Applying Migrations

```bash
# Apply to development database
dotnet ef database update

# Generate SQL script for production
dotnet ef migrations script
```

#### Rolling Back Migrations

```bash
# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove
```

## 🧪 Testing Strategy

### 1. Backend Testing

#### Unit Tests

```bash
# Run all unit tests
cd LocationFinder.API.Tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~LocationServiceTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

#### Integration Tests

```bash
# Run integration tests
dotnet test --filter "Category=Integration"
```

#### Test Structure

```
LocationFinder.API.Tests/
├── Unit/
│   ├── Services/
│   │   ├── LocationServiceTests.cs
│   │   └── ValidationServiceTests.cs
│   └── Controllers/
│       └── LocationsControllerTests.cs
├── Integration/
│   ├── ApiIntegrationTests.cs
│   └── DatabaseIntegrationTests.cs
└── Helpers/
    ├── TestDataBuilder.cs
    └── MockHelper.cs
```

### 2. Frontend Testing

#### Unit Tests

```bash
# Run unit tests
cd LocationFinder.Client
ng test

# Run with coverage
ng test --code-coverage
```

#### E2E Tests

```bash
# Run E2E tests
ng e2e

# Run specific E2E test
ng e2e --specs="src/app/search/search.e2e-spec.ts"
```

#### Test Structure

```
LocationFinder.Client/src/
├── app/
│   ├── components/
│   │   ├── search-form/
│   │   │   └── search-form.component.spec.ts
│   │   └── location-list/
│   │       └── location-list.component.spec.ts
│   └── services/
│       └── location.service.spec.ts
└── e2e/
    └── search.e2e-spec.ts
```

### 3. Data Import Testing

```bash
# Test data import functionality
cd LocationFinder.DataImport

# Test zip code import
dotnet run -- zipcodes --file Data/sample-zipcodes.json --batch-size 100

# Test location import
dotnet run -- locations --file Data/sample-locations.json --batch-size 100

# Test validation
dotnet run -- validate
```

## 📝 Code Quality Standards

### 1. C# Coding Standards

#### Naming Conventions

```csharp
// Classes and methods: PascalCase
public class LocationService
{
    public async Task<List<Location>> SearchLocationsAsync(string zipCode)
    {
        // Implementation
    }
}

// Private fields: camelCase with underscore prefix
private readonly ILogger<LocationService> _logger;

// Constants: PascalCase
public const int DefaultMaxDistance = 50;
```

#### Code Organization

```csharp
// File structure
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LocationFinder.API.Services
{
    public class LocationService : ILocationService
    {
        // Private fields
        private readonly ILogger<LocationService> _logger;
        
        // Constructor
        public LocationService(ILogger<LocationService> logger)
        {
            _logger = logger;
        }
        
        // Public methods
        public async Task<List<Location>> SearchLocationsAsync(string zipCode)
        {
            // Implementation
        }
        
        // Private methods
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Implementation
        }
    }
}
```

### 2. TypeScript/Angular Standards

#### Naming Conventions

```typescript
// Components: PascalCase
export class SearchFormComponent {
  // Properties: camelCase
  public searchForm: FormGroup;
  
  // Methods: camelCase
  public onSubmit(): void {
    // Implementation
  }
  
  // Private methods: camelCase with underscore prefix
  private _validateZipCode(zipCode: string): boolean {
    // Implementation
  }
}

// Interfaces: PascalCase with 'I' prefix
export interface ILocation {
  id: number;
  name: string;
  address: string;
}
```

#### File Organization

```typescript
// Component file structure
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-search-form',
  templateUrl: './search-form.component.html',
  styleUrls: ['./search-form.component.css']
})
export class SearchFormComponent implements OnInit {
  // Properties
  public searchForm: FormGroup;
  
  // Constructor
  constructor(private formBuilder: FormBuilder) {}
  
  // Lifecycle hooks
  ngOnInit(): void {
    this.initializeForm();
  }
  
  // Public methods
  public onSubmit(): void {
    // Implementation
  }
  
  // Private methods
  private initializeForm(): void {
    // Implementation
  }
}
```

### 3. Commit Message Standards

#### Conventional Commits

```bash
# Format: type(scope): description

# Feature
git commit -m "feat(search): add distance filter to location search"

# Bug fix
git commit -m "fix(validation): correct zip code regex pattern"

# Documentation
git commit -m "docs(api): update endpoint documentation"

# Refactor
git commit -m "refactor(service): extract distance calculation to utility"

# Test
git commit -m "test(service): add unit tests for location service"

# Chore
git commit -m "chore(deps): update Angular to version 17"
```

#### Commit Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (formatting, etc.)
- **refactor**: Code refactoring
- **test**: Adding or updating tests
- **chore**: Maintenance tasks

## 🔄 Continuous Integration

### 1. Build Pipeline

```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Install Angular CLI
      run: npm install -g @angular/cli
    
    - name: Install frontend dependencies
      run: |
        cd LocationFinder.Client
        npm install
    
    - name: Build frontend
      run: |
        cd LocationFinder.Client
        ng build --configuration production
    
    - name: Run frontend tests
      run: |
        cd LocationFinder.Client
        ng test --watch=false --browsers=ChromeHeadless
```

### 2. Code Quality Checks

```bash
# Run code analysis
dotnet build --verbosity normal

# Run style analysis
dotnet format --verify-no-changes

# Run security analysis
dotnet list package --vulnerable
```

## 🚀 Deployment Workflow

### 1. Development Deployment

```bash
# Build for development
dotnet build --configuration Debug

# Run locally
dotnet run --project LocationFinder.API
```

### 2. Staging Deployment

```bash
# Build for staging
dotnet build --configuration Release

# Deploy to staging environment
dotnet publish --configuration Release --output ./publish
```

### 3. Production Deployment

```bash
# Build for production
dotnet build --configuration Release

# Deploy to production
dotnet publish --configuration Release --output ./publish
```

## 📊 Performance Monitoring

### 1. Backend Performance

```csharp
// Add performance logging
public class LocationService : ILocationService
{
    public async Task<List<Location>> SearchLocationsAsync(string zipCode)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await _repository.SearchLocationsAsync(zipCode);
            return result;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Location search completed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
```

### 2. Frontend Performance

```typescript
// Add performance monitoring
export class LocationService {
  public searchLocations(zipCode: string): Observable<Location[]> {
    const startTime = performance.now();
    
    return this.http.get<ApiResponse<Location[]>>(`${this.apiUrl}/locations/search?zipCode=${zipCode}`)
      .pipe(
        map(response => response.data),
        tap(() => {
          const endTime = performance.now();
          console.log(`Search completed in ${endTime - startTime}ms`);
        })
      );
  }
}
```

## 🔍 Code Review Process

### 1. Review Checklist

#### Backend Review
- [ ] Code follows C# conventions
- [ ] Proper error handling implemented
- [ ] Unit tests added/updated
- [ ] Integration tests pass
- [ ] No security vulnerabilities
- [ ] Performance considerations addressed
- [ ] Documentation updated

#### Frontend Review
- [ ] Code follows TypeScript/Angular conventions
- [ ] Component logic is clean and maintainable
- [ ] Unit tests added/updated
- [ ] E2E tests pass
- [ ] Accessibility considerations
- [ ] Performance optimizations
- [ ] UI/UX improvements

### 2. Review Process

1. **Self-Review**: Review your own code before submitting
2. **Peer Review**: Request review from team member
3. **Address Feedback**: Make necessary changes
4. **Final Approval**: Get approval from reviewer
5. **Merge**: Merge to main branch

## 🐛 Debugging Guide

### 1. Backend Debugging

```bash
# Enable detailed logging
# Update appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}

# Use debugger
[DebuggerStepThrough]
public async Task<List<Location>> SearchLocationsAsync(string zipCode)
{
    // Set breakpoints here
    var locations = await _repository.GetLocationsAsync();
    return locations;
}
```

### 2. Frontend Debugging

```typescript
// Enable Angular debugging
// In main.ts
if (!environment.production) {
  enableDebugTools();
}

// Use browser dev tools
console.log('Debug info:', data);
debugger; // Set breakpoint
```

### 3. Database Debugging

```bash
# Enable SQL logging
# In appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}

# Use SQL Server Profiler
# Monitor database queries and performance
```

## 📚 Learning Resources

### 1. .NET Core
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)

### 2. Angular
- [Angular Documentation](https://angular.io/docs)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Angular Style Guide](https://angular.io/guide/styleguide)

### 3. Testing
- [xUnit Documentation](https://xunit.net/)
- [Jasmine Testing Framework](https://jasmine.github.io/)
- [Angular Testing Guide](https://angular.io/guide/testing)

## 🤝 Team Collaboration

### 1. Communication
- Use GitHub Issues for bug reports and feature requests
- Use Pull Request comments for code review discussions
- Regular team meetings for project updates

### 2. Knowledge Sharing
- Document new features and changes
- Share learnings and best practices
- Conduct code reviews and pair programming sessions

### 3. Code Ownership
- Take ownership of your code
- Be available for questions and support
- Help maintain code quality and standards

---

**Last Updated**: January 2024  
**Version**: 1.0.0

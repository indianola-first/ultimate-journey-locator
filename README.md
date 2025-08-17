# Ultimate Journey Locator
An application that helps users discover nearby in-person or online Ultimate Journey groups using their ZIP code.

# What is the Ultimate Journey?
The Ultimate Journey is a Christ-centered, small-group discipleship program designed to facilitate emotional healing and life transformation, helping participants deepen their relationship with God and embrace their God-given purpose ([The Ultimate Journey](https://www.theultimatejourney.org/)).

# Technical Description
A complete location finder solution consisting of an ASP.NET Core Web API backend and Angular 17 frontend, designed to be embedded in WordPress via iframe. Users can find the closest predefined locations by entering their zip code, with distance calculations using zip code coordinates.

## 🏗️ Architecture Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   WordPress     │    │  Angular 17 SPA  │    │ ASP.NET Core    │
│   (Parent Site) │◄──►│  (Iframe)        │◄──►│  Web API        │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                │                        │
                                │                        │
                                ▼                        ▼
                       ┌──────────────────┐    ┌─────────────────┐
                       │  Data Import     │    │ SQL Server      │
                       │  Console App     │    │  Database       │
                       └──────────────────┘    └─────────────────┘
```

## 📁 Solution Structure

```
LocationFinder/
├── LocationFinder.API/                 # ASP.NET Core Web API
│   ├── Controllers/                    # API endpoints
│   ├── Data/                          # Entity Framework context
│   ├── Models/                        # Data models
│   ├── Services/                      # Business logic
│   ├── Properties/                    # Configuration
│   └── wwwroot/                       # Angular build output
├── LocationFinder.Client/             # Angular 17 SPA
│   ├── src/
│   │   ├── app/                       # Angular components
│   │   ├── assets/                    # Static assets
│   │   └── environments/              # Environment configs
│   └── dist/                          # Build output
├── LocationFinder.DataImport/         # Data import console app
│   ├── Services/                      # Import services
│   ├── Models/                        # Import models
│   └── Data/                          # Sample data files
├── LocationFinder.API.Tests/          # Backend unit tests
└── LocationFinder.Client.Tests/       # Frontend unit tests
```

## 🚀 Quick Start

### Prerequisites

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **SQL Server** (LocalDB, Express, or Azure)
- **Visual Studio 2022** or **VS Code**

### 1. Clone and Setup

```bash
git clone <repository-url>
cd LocationFinder
```

### 2. Database Setup

#### Option A: Using LocalDB (Development)
```bash
# Override placeholder connection string in LocationFinder.API/appsettings.Development.json
cd LocationFinder.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=your-database;Trusted_Connection=true;MultipleActiveResultSets=true"
```

#### Option B: Using SQL Server Express/Azure
```bash
# Override placeholder connection string in LocationFinder.API/appsettings.Development.json
cd LocationFinder.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=true;"
```

### 3. Database Migration

```bash
cd LocationFinder.API
dotnet ef database update
```

### 4. Import Sample Data

```bash
cd LocationFinder.DataImport
dotnet run -- zipcodes --file Data/sample-zipcodes.json
dotnet run -- locations --file Data/sample-locations.json
```

### 5. Build and Run

#### Backend (API)
```bash
cd LocationFinder.API
dotnet restore
dotnet build
dotnet run
```

#### Frontend (Angular)
```bash
cd LocationFinder.Client
npm install
ng serve
```

### 6. Access the Application

- **API**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger
- **Angular**: http://localhost:4200

## 📚 Detailed Setup Instructions

### Backend Configuration

#### Environment Configuration

**Development (`appsettings.Development.json`)**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LocationFinder;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "http://localhost:3000"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Production (`appsettings.Production.json`)**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-production-server;Database=LocationFinder;User ID=your-user;Password=your-password;TrustServerCertificate=true;"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "LocationFinder.API": "Information"
    }
  }
}
```

#### Entity Framework Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update

# Generate SQL script (optional)
dotnet ef migrations script
```

### Frontend Configuration

#### Environment Files

**Development (`src/environments/environment.ts`)**:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'
};
```

**Production (`src/environments/environment.prod.ts`)**:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com/api'
};
```

#### Angular Configuration

**angular.json** - Configured for iframe embedding:
```json
{
  "projects": {
    "LocationFinder.Client": {
      "architect": {
        "build": {
          "options": {
            "outputPath": "../LocationFinder.API/wwwroot",
            "baseHref": "./",
            "deployUrl": "./"
          }
        }
      }
    }
  }
}
```

### Data Import Tool

#### Configuration

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=LocationFinder;User ID=your-user;Password=your-password;TrustServerCertificate=true;"
  },
  "DataImport": {
    "DefaultBatchSize": 1000,
    "MaxBatchSize": 5000,
    "ProgressReportInterval": 100
  }
}
```

#### Usage Examples

```bash
# Import zip codes
dotnet run -- zipcodes --file Data/zipcodes.json --batch-size 500

# Import locations
dotnet run -- locations --file Data/locations.json --batch-size 1000

# Validate data
dotnet run -- validate

# Clear all data
dotnet run -- clear-all

# Show help
dotnet run -- --help
```

## 🔧 Development Workflow

### 1. Development Environment Setup

```bash
# Clone repository
git clone <repository-url>
cd LocationFinder

# Install .NET dependencies
dotnet restore

# Install Node.js dependencies
cd LocationFinder.Client
npm install
cd ..

# Setup database
cd LocationFinder.API
dotnet ef database update
cd ..

# Import sample data
cd LocationFinder.DataImport
dotnet run -- zipcodes --file Data/sample-zipcodes.json
dotnet run -- locations --file Data/sample-locations.json
cd ..
```

### 2. Running the Application

#### Development Mode
```bash
# Terminal 1: Run API
cd LocationFinder.API
dotnet watch run

# Terminal 2: Run Angular (if needed)
cd LocationFinder.Client
ng serve
```

#### Production Build
```bash
# Build Angular and copy to API wwwroot
cd LocationFinder.Client
ng build --configuration production

# Build and run API
cd ../LocationFinder.API
dotnet build --configuration Release
dotnet run --configuration Release
```

### 3. Testing

#### Backend Tests
```bash
cd LocationFinder.API.Tests
dotnet test
```

#### Frontend Tests
```bash
cd LocationFinder.Client
ng test
ng e2e
```

### 4. Database Management

#### Creating Migrations
```bash
cd LocationFinder.API
dotnet ef migrations add MigrationName
```

#### Applying Migrations
```bash
dotnet ef database update
```

#### Rolling Back Migrations
```bash
dotnet ef database update PreviousMigrationName
```

## 📊 Data Models

### ZipCode Model
```csharp
public class ZipCode
{
    public int Id { get; set; }
    public string ZipCodeValue { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
}
```

### Location Model
```csharp
public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? BusinessHours { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
}
```

## 🔌 API Endpoints

### Locations Controller

#### GET /api/locations/search
Search for locations near a zip code.

**Parameters:**
- `zipCode` (string, required): The zip code to search from
- `maxDistance` (int, optional): Maximum distance in miles (default: 50)

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Downtown Office",
      "address": "123 Main Street",
      "city": "New York",
      "state": "NY",
      "zipCode": "10001",
      "phone": "(212) 555-0100",
      "latitude": 40.7505,
      "longitude": -73.9965,
      "businessHours": "Monday-Friday: 9:00 AM - 5:00 PM",
      "isActive": true,
      "distance": 2.5
    }
  ],
  "message": "Locations found successfully"
}
```

## 🎨 Frontend Components

### AppComponent
Main application component that orchestrates the search functionality.

### SearchFormComponent
Handles zip code input and validation with reactive forms.

### LocationListComponent
Displays search results with loading and error states.

### LocationCardComponent
Individual location display with click-to-call functionality.

## 🧪 Testing

### Backend Testing
- **Unit Tests**: Service and controller logic
- **Integration Tests**: Full API endpoint testing
- **Database Tests**: In-memory database testing

### Frontend Testing
- **Unit Tests**: Component and service testing
- **E2E Tests**: End-to-end user workflows

## 📦 Build and Deployment

### Development Build
```bash
# Build entire solution
dotnet build

# Build Angular and copy to API
cd LocationFinder.Client
ng build
cd ../LocationFinder.API
dotnet build
```

### Production Build
```bash
# Build for production
dotnet build --configuration Release
```

### Docker Support
```bash
# Build Docker image
docker build -t locationfinder .

# Run Docker container
docker run -p 5000:5000 locationfinder
```

## 🔒 Security Considerations

### CORS Configuration
Configured for WordPress iframe embedding with specific domain allowlist.

### Input Validation
- Zip code format validation
- SQL injection prevention via Entity Framework
- XSS protection via Angular sanitization

### API Security
- HTTPS enforcement in production
- Security headers configuration
- Rate limiting (configurable)

## 🐛 Troubleshooting

### Common Issues

#### Database Connection
```bash
# Check connection string
# Verify SQL Server is running
# Test connection with SQL Server Management Studio
```

#### Angular Build Issues
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Clear Angular cache
ng cache clean
```

#### API Not Starting
```bash
# Check port availability
netstat -an | grep 5001

# Verify configuration files
# Check database connection
```

### Logging
- **Development**: Console and Debug output
- **Production**: Structured logging with configurable levels

## 📈 Performance Optimization

### Database
- Indexed zip code and location tables
- Optimized distance calculation queries
- Connection pooling

### Frontend
- Lazy loading of components
- Optimized bundle size
- CDN-ready static assets

### API
- Response caching
- Compression middleware
- Async/await patterns

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the troubleshooting section
- Review the API documentation

---

**LocationFinder** - Find locations near you with ease! 🗺️

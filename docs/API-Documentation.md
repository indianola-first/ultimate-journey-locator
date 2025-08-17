# LocationFinder API Documentation

## Overview

The LocationFinder API is a RESTful service built with ASP.NET Core that provides location search functionality. The API calculates distances between zip codes and predefined locations using the Haversine formula.

**Base URL**: `https://localhost:5001/api` (Development)
**Base URL**: `https://your-api-domain.com/api` (Production)

## Authentication

Currently, the API does not require authentication. All endpoints are publicly accessible.

## Response Format

All API responses follow a consistent format:

```json
{
  "success": true|false,
  "data": object|array|null,
  "message": "string"
}
```

### Response Fields

- **success**: Boolean indicating if the request was successful
- **data**: The actual response data (object, array, or null)
- **message**: Human-readable message describing the result

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "data": null,
  "message": "Invalid zip code format. Please enter a valid 5-digit zip code."
}
```

### 404 Not Found
```json
{
  "success": false,
  "data": null,
  "message": "No locations found within the specified distance."
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "data": null,
  "message": "An unexpected error occurred. Please try again later."
}
```

## Endpoints

### Search Locations

#### GET /api/locations/search

Search for locations near a specified zip code.

**URL Parameters:**
- `zipCode` (string, required): 5-digit zip code
- `maxDistance` (integer, optional): Maximum distance in miles (default: 50)

**Example Request:**
```
GET /api/locations/search?zipCode=10001&maxDistance=25
```

**Success Response (200 OK):**
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
    },
    {
      "id": 2,
      "name": "Brooklyn Branch",
      "address": "456 Brooklyn Ave",
      "city": "New York",
      "state": "NY",
      "zipCode": "10002",
      "phone": "(212) 555-0200",
      "latitude": 40.7168,
      "longitude": -73.9861,
      "businessHours": "Monday-Saturday: 8:00 AM - 6:00 PM",
      "isActive": true,
      "distance": 4.2
    }
  ],
  "message": "Found 2 locations within 25 miles of 10001"
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "data": null,
  "message": "Invalid zip code format. Please enter a valid 5-digit zip code."
}
```

**Error Response (404 Not Found):**
```json
{
  "success": false,
  "data": null,
  "message": "No locations found within 50 miles of 99999"
}
```

## Data Models

### Location Model

```json
{
  "id": 1,
  "name": "string",
  "address": "string",
  "city": "string",
  "state": "string",
  "zipCode": "string",
  "phone": "string|null",
  "latitude": 40.7505,
  "longitude": -73.9965,
  "businessHours": "string|null",
  "isActive": true,
  "distance": 2.5
}
```

**Field Descriptions:**
- **id**: Unique identifier for the location
- **name**: Location name
- **address**: Street address
- **city**: City name
- **state**: State abbreviation (2 characters)
- **zipCode**: 5-digit zip code
- **phone**: Phone number (optional)
- **latitude**: Latitude coordinate (decimal degrees)
- **longitude**: Longitude coordinate (decimal degrees)
- **businessHours**: Business hours description (optional)
- **isActive**: Whether the location is currently active
- **distance**: Distance from search zip code in miles (calculated)

### ZipCode Model

```json
{
  "id": 1,
  "zipCodeValue": "10001",
  "latitude": 40.7505,
  "longitude": -73.9965,
  "city": "New York",
  "state": "NY"
}
```

**Field Descriptions:**
- **id**: Unique identifier for the zip code
- **zipCodeValue**: 5-digit zip code
- **latitude**: Latitude coordinate (decimal degrees)
- **longitude**: Longitude coordinate (decimal degrees)
- **city**: City name
- **state**: State abbreviation (2 characters)

## Distance Calculation

The API uses the Haversine formula to calculate distances between geographic coordinates:

```csharp
// Haversine formula implementation
public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
{
    const double earthRadius = 3959; // Earth's radius in miles
    
    var dLat = ToRadians(lat2 - lat1);
    var dLon = ToRadians(lon2 - lon1);
    
    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
    
    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    
    return earthRadius * c;
}
```

## Rate Limiting

Currently, the API does not implement rate limiting. Consider implementing rate limiting for production use.

## CORS Configuration

The API is configured to allow requests from specific origins:

**Development:**
- http://localhost:4200
- http://localhost:3000

**Production:**
- https://yourdomain.com
- https://www.yourdomain.com

## Health Check

### GET /health

Check the health status of the API.

**Example Request:**
```
GET /health
```

**Success Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "1.0.0"
}
```

## Swagger Documentation

Interactive API documentation is available at:
- **Development**: https://localhost:5001/swagger
- **Production**: https://your-api-domain.com/api-docs

## Testing the API

### Using curl

```bash
# Search for locations near zip code 10001
curl -X GET "https://localhost:5001/api/locations/search?zipCode=10001&maxDistance=25" \
  -H "Accept: application/json"

# Search with default distance (50 miles)
curl -X GET "https://localhost:5001/api/locations/search?zipCode=10001" \
  -H "Accept: application/json"
```

### Using Postman

1. Create a new GET request
2. Set URL: `https://localhost:5001/api/locations/search`
3. Add query parameters:
   - `zipCode`: 10001
   - `maxDistance`: 25
4. Send request

### Using JavaScript/Fetch

```javascript
async function searchLocations(zipCode, maxDistance = 50) {
  try {
    const response = await fetch(
      `https://localhost:5001/api/locations/search?zipCode=${zipCode}&maxDistance=${maxDistance}`,
      {
        method: 'GET',
        headers: {
          'Accept': 'application/json'
        }
      }
    );
    
    const result = await response.json();
    
    if (result.success) {
      console.log('Locations found:', result.data);
      return result.data;
    } else {
      console.error('Error:', result.message);
      return null;
    }
  } catch (error) {
    console.error('Network error:', error);
    return null;
  }
}

// Usage
searchLocations('10001', 25);
```

## Error Codes

| HTTP Status | Description | Common Causes |
|-------------|-------------|---------------|
| 200 | Success | Request completed successfully |
| 400 | Bad Request | Invalid zip code format, missing parameters |
| 404 | Not Found | No locations found within specified distance |
| 500 | Internal Server Error | Database connection issues, server errors |

## Best Practices

### Request Optimization

1. **Use appropriate maxDistance**: Don't request unnecessarily large distances
2. **Cache results**: Implement client-side caching for repeated searches
3. **Handle errors gracefully**: Always check the `success` field in responses

### Response Handling

1. **Check success field**: Always verify the `success` field before processing data
2. **Handle empty results**: Be prepared for empty location arrays
3. **Display user-friendly messages**: Use the `message` field for user feedback

### Performance Considerations

1. **Database indexing**: Ensure zip codes and coordinates are properly indexed
2. **Connection pooling**: Use connection pooling for database connections
3. **Response compression**: Enable gzip compression for faster responses

## Versioning

The API currently uses version 1.0. Future versions will be available at:
- `/api/v2/locations/search`

## Support

For API support and questions:
- Check the troubleshooting section in the main README
- Review the Swagger documentation
- Create an issue in the repository

---

**API Version**: 1.0.0  
**Last Updated**: January 2024

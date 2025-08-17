import { HttpErrorResponse } from '@angular/common/http';
import { LocationSearchResult, LocationSearchResponse } from '@app/models';

/**
 * Test utilities for Angular testing
 */
export class TestUtils {
  /**
   * Creates a mock location search result
   */
  static createMockLocationSearchResult(
    overrides: Partial<LocationSearchResult> = {}
  ): LocationSearchResult {
    return {
      id: 1,
      name: 'Test Location',
      address: '123 Test St',
      city: 'Test City',
      state: 'TS',
      zipCode: '10001',
      phone: '555-555-0100',
      businessHours: 'Mon-Fri 9AM-6PM',
      distanceMiles: 1.5,
      ...overrides,
    };
  }

  /**
   * Creates multiple mock location search results
   */
  static createMockLocationSearchResults(count: number = 3): LocationSearchResult[] {
    return Array.from({ length: count }, (_, index) =>
      this.createMockLocationSearchResult({
        id: index + 1,
        name: `Test Location ${index + 1}`,
        address: `${index + 1}23 Test St`,
        phone: `555-555-${(index + 1).toString().padStart(4, '0')}`,
        distanceMiles: (index + 1) * 0.5,
      })
    );
  }

  /**
   * Creates a mock successful API response
   */
  static createMockSuccessResponse(
    data: LocationSearchResult[] = [this.createMockLocationSearchResult()],
    message: string = ''
  ): LocationSearchResponse {
    return {
      success: true,
      data,
      message,
    };
  }

  /**
   * Creates a mock error API response
   */
  static createMockErrorResponse(message: string = 'Test error message'): LocationSearchResponse {
    return {
      success: false,
      data: null,
      message,
    };
  }

  /**
   * Creates a mock HTTP error response
   */
  static createMockHttpErrorResponse(
    status: number = 400,
    statusText: string = 'Bad Request',
    error: any = { message: 'Test error message' }
  ): HttpErrorResponse {
    return new HttpErrorResponse({
      error,
      status,
      statusText,
    });
  }

  /**
   * Creates a mock network error
   */
  static createMockNetworkError(): HttpErrorResponse {
    return new HttpErrorResponse({
      error: new ErrorEvent('Network error', { message: 'Network connection failed' }),
      status: 0,
      statusText: 'Network Error',
    });
  }

  /**
   * Creates a mock timeout error
   */
  static createMockTimeoutError(): HttpErrorResponse {
    return new HttpErrorResponse({
      error: new ErrorEvent('Timeout error', { message: 'Request timeout' }),
      status: 0,
      statusText: 'Timeout',
    });
  }

  /**
   * Creates a mock 400 Bad Request error
   */
  static createMockBadRequestError(message: string = 'Invalid zip code'): HttpErrorResponse {
    return this.createMockHttpErrorResponse(400, 'Bad Request', { message });
  }

  /**
   * Creates a mock 404 Not Found error
   */
  static createMockNotFoundError(message: string = 'Zip code not found'): HttpErrorResponse {
    return this.createMockHttpErrorResponse(404, 'Not Found', { message });
  }

  /**
   * Creates a mock 500 Internal Server Error
   */
  static createMockServerError(message: string = 'Internal server error'): HttpErrorResponse {
    return this.createMockHttpErrorResponse(500, 'Internal Server Error', { message });
  }

  /**
   * Creates a mock 403 Forbidden error
   */
  static createMockForbiddenError(message: string = 'Access denied'): HttpErrorResponse {
    return this.createMockHttpErrorResponse(403, 'Forbidden', { message });
  }

  /**
   * Creates a mock 401 Unauthorized error
   */
  static createMockUnauthorizedError(
    message: string = 'Authentication required'
  ): HttpErrorResponse {
    return this.createMockHttpErrorResponse(401, 'Unauthorized', { message });
  }

  /**
   * Creates a mock 429 Too Many Requests error
   */
  static createMockRateLimitError(message: string = 'Too many requests'): HttpErrorResponse {
    return this.createMockHttpErrorResponse(429, 'Too Many Requests', { message });
  }

  /**
   * Creates a mock 503 Service Unavailable error
   */
  static createMockServiceUnavailableError(
    message: string = 'Service temporarily unavailable'
  ): HttpErrorResponse {
    return this.createMockHttpErrorResponse(503, 'Service Unavailable', { message });
  }

  /**
   * Creates a mock location with specific distance
   */
  static createMockLocationWithDistance(distanceMiles: number): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles });
  }

  /**
   * Creates mock locations with varying distances
   */
  static createMockLocationsWithDistances(distances: number[]): LocationSearchResult[] {
    return distances.map((distance, index) =>
      this.createMockLocationSearchResult({
        id: index + 1,
        name: `Location ${index + 1}`,
        distanceMiles: distance,
      })
    );
  }

  /**
   * Creates a mock location without phone number
   */
  static createMockLocationWithoutPhone(): LocationSearchResult {
    return this.createMockLocationSearchResult({ phone: '' });
  }

  /**
   * Creates a mock location without business hours
   */
  static createMockLocationWithoutHours(): LocationSearchResult {
    return this.createMockLocationSearchResult({ businessHours: '' });
  }

  /**
   * Creates a mock location with long text fields
   */
  static createMockLocationWithLongText(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'This is a very long location name that should be handled properly by the component and should not cause any layout issues or overflow problems',
      address:
        'This is a very long address that should be handled properly by the component and should not cause any layout issues or overflow problems',
      businessHours:
        'Monday through Friday 9:00 AM to 6:00 PM, Saturday 10:00 AM to 4:00 PM, Sunday Closed, Holidays: Closed on major holidays',
    });
  }

  /**
   * Creates a mock location with special characters
   */
  static createMockLocationWithSpecialChars(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'Test Location & Company, Inc.',
      address: '123 Test St, Apt #5, Floor 2',
      city: 'Test City',
      state: 'TS',
      zipCode: '10001',
      phone: '555-555-0100 ext 123',
      businessHours: 'Mon-Fri 9AM-6PM, Sat 10AM-4PM, Sun Closed',
    });
  }

  /**
   * Creates a mock location with international phone number
   */
  static createMockLocationWithInternationalPhone(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      phone: '+1-555-555-0100',
    });
  }

  /**
   * Creates a mock location with different phone formats
   */
  static createMockLocationWithPhoneFormat(format: string): LocationSearchResult {
    return this.createMockLocationSearchResult({
      phone: format,
    });
  }

  /**
   * Creates a mock location with zero distance
   */
  static createMockLocationWithZeroDistance(): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles: 0 });
  }

  /**
   * Creates a mock location with very large distance
   */
  static createMockLocationWithLargeDistance(): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles: 999.9 });
  }

  /**
   * Creates a mock location with decimal distance
   */
  static createMockLocationWithDecimalDistance(): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles: 2.75 });
  }

  /**
   * Creates a mock location with negative distance (edge case)
   */
  static createMockLocationWithNegativeDistance(): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles: -1.5 });
  }

  /**
   * Creates a mock location with very small distance
   */
  static createMockLocationWithSmallDistance(): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles: 0.1 });
  }

  /**
   * Creates a mock location with very large distance
   */
  static createMockLocationWithVeryLargeDistance(): LocationSearchResult {
    return this.createMockLocationSearchResult({ distanceMiles: 1000.0 });
  }

  /**
   * Creates a mock location with null values (edge case)
   */
  static createMockLocationWithNullValues(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      phone: null as any,
      businessHours: null as any,
    });
  }

  /**
   * Creates a mock location with undefined values (edge case)
   */
  static createMockLocationWithUndefinedValues(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      phone: undefined as any,
      businessHours: undefined as any,
    });
  }

  /**
   * Creates a mock location with empty string values
   */
  static createMockLocationWithEmptyValues(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      phone: '',
      businessHours: '',
    });
  }

  /**
   * Creates a mock location with whitespace-only values
   */
  static createMockLocationWithWhitespaceValues(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      phone: '   ',
      businessHours: '   ',
    });
  }

  /**
   * Creates a mock location with leading/trailing whitespace
   */
  static createMockLocationWithWhitespace(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: '  Test Location  ',
      address: '  123 Test St  ',
      city: '  Test City  ',
      state: '  TS  ',
      zipCode: '  10001  ',
      phone: '  555-555-0100  ',
      businessHours: '  Mon-Fri 9AM-6PM  ',
    });
  }

  /**
   * Creates a mock location with HTML entities (edge case)
   */
  static createMockLocationWithHtmlEntities(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'Test & Location <Company> "Inc."',
      address: '123 Test St & Ave, Apt #5',
      businessHours: 'Mon-Fri 9AM-6PM & Sat 10AM-4PM',
    });
  }

  /**
   * Creates a mock location with unicode characters
   */
  static createMockLocationWithUnicode(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'Test Location Café & Restaurant',
      address: '123 Test St, 2nd Floor',
      city: 'Test City',
      businessHours: 'Mon-Fri 9AM-6PM, Sat 10AM-4PM',
    });
  }

  /**
   * Creates a mock location with emoji characters
   */
  static createMockLocationWithEmoji(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'Test Location 🏢',
      address: '123 Test St 📍',
      businessHours: 'Mon-Fri 9AM-6PM 🕒',
    });
  }

  /**
   * Creates a mock location with very long individual fields
   */
  static createMockLocationWithVeryLongFields(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: "This is an extremely long location name that should test the component's ability to handle very long text without breaking the layout or causing any visual issues. The name should be truncated or wrapped appropriately.",
      address:
        "This is an extremely long address that should test the component's ability to handle very long text without breaking the layout or causing any visual issues. The address should be truncated or wrapped appropriately.",
      businessHours:
        "This is an extremely long business hours description that should test the component's ability to handle very long text without breaking the layout or causing any visual issues. The hours should be truncated or wrapped appropriately.",
    });
  }

  /**
   * Creates a mock location with all fields at maximum length
   */
  static createMockLocationWithMaxLengthFields(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'A'.repeat(255),
      address: 'A'.repeat(255),
      city: 'A'.repeat(100),
      state: 'A'.repeat(10),
      zipCode: 'A'.repeat(10),
      phone: 'A'.repeat(20),
      businessHours: 'A'.repeat(500),
    });
  }

  /**
   * Creates a mock location with minimum valid data
   */
  static createMockLocationWithMinimalData(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'A',
      address: 'A',
      city: 'A',
      state: 'A',
      zipCode: 'A',
      phone: '',
      businessHours: '',
      distanceMiles: 0,
    });
  }

  /**
   * Creates a mock location with maximum valid data
   */
  static createMockLocationWithMaximumData(): LocationSearchResult {
    return this.createMockLocationSearchResult({
      name: 'Z'.repeat(255),
      address: 'Z'.repeat(255),
      city: 'Z'.repeat(100),
      state: 'Z'.repeat(10),
      zipCode: 'Z'.repeat(10),
      phone: 'Z'.repeat(20),
      businessHours: 'Z'.repeat(500),
      distanceMiles: 999999.99,
    });
  }
}

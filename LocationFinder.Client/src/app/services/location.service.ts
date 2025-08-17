import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { LocationSearchResult, ApiResponse, LocationSearchResponse } from '@app/models';

@Injectable({
  providedIn: 'root',
})
export class LocationService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Searches for locations near a specified zip code
   * @param zipCode - The 5-digit US zip code to search from
   * @param limit - Maximum number of locations to return (default: 10)
   * @returns Observable of ApiResponse containing LocationSearchResult array
   */
  searchLocations(zipCode: string, limit: number = 10): Observable<LocationSearchResponse> {
    // Validate input parameters
    if (!zipCode || zipCode.trim().length === 0) {
      return throwError(() => new Error('Zip code is required'));
    }

    if (!this.isValidZipCode(zipCode)) {
      return throwError(() => new Error('Please enter a valid 5-digit zip code'));
    }

    if (limit <= 0 || limit > 100) {
      return throwError(() => new Error('Limit must be between 1 and 100'));
    }

    const url = `${this.apiUrl}/locations/search`;
    const params = {
      zipcode: zipCode.trim(),
      limit: limit.toString(),
    };

    return this.http.get<LocationSearchResponse>(url, { params }).pipe(
      map(response => {
        // Ensure the response has the expected structure
        if (!response || typeof response.success !== 'boolean') {
          throw new Error('Invalid response format from server');
        }
        return response;
      }),
      catchError(this.handleError.bind(this))
    );
  }

  /**
   * Handles HTTP errors and converts them to user-friendly messages
   * @param error - The HTTP error response
   * @returns Observable that throws an error with a user-friendly message
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred while searching for locations. Please try again.';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Network error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          errorMessage =
            error.error?.message || 'Invalid request. Please check your zip code format.';
          break;
        case 404:
          errorMessage =
            error.error?.message || 'Zip code not found. Please try a different zip code.';
          break;
        case 500:
          errorMessage = 'Server error. Please try again later.';
          break;
        case 0:
          errorMessage = 'Unable to connect to the server. Please check your internet connection.';
          break;
        default:
          errorMessage =
            error.error?.message || `Server error (${error.status}). Please try again.`;
      }
    }

    console.error('LocationService error:', error);
    return throwError(() => new Error(errorMessage));
  }

  /**
   * Validates that a zip code is in the correct 5-digit format
   * @param zipCode - The zip code to validate
   * @returns True if the zip code is valid, false otherwise
   */
  private isValidZipCode(zipCode: string): boolean {
    if (!zipCode || zipCode.trim().length === 0) {
      return false;
    }

    // Remove any whitespace and check if it's exactly 5 digits
    const trimmedZipCode = zipCode.trim();
    return /^\d{5}$/.test(trimmedZipCode);
  }

  /**
   * Gets the current API URL being used by the service
   * @returns The current API URL
   */
  getApiUrl(): string {
    return this.apiUrl;
  }

  /**
   * Checks if the service is running in production mode
   * @returns True if in production, false otherwise
   */
  isProduction(): boolean {
    return environment.production;
  }
}

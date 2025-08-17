import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { LocationService } from '@app/services';
import { LocationSearchResult, LocationSearchResponse } from '@app/models';
import { SearchFormComponent } from './components/search-form/search-form.component';
import {
  LocationListComponent,
  LocationListState,
} from './components/location-list/location-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SearchFormComponent, LocationListComponent],
  providers: [LocationService],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'Location Finder';
  version = '1.0.0';

  // Component state
  locationListState: LocationListState = {
    isLoading: false,
    hasError: false,
    errorMessage: '',
    locations: [],
    totalCount: 0,
  };

  // Search configuration
  readonly defaultSearchLimit = 10;
  readonly maxSearchLimit = 20;

  constructor(private locationService: LocationService) {}

  ngOnInit(): void {
    // Initialize component state
    this.resetSearchState();
  }

  /**
   * Handle search form submissions
   */
  onSearchRequested(zipCode: string): void {
    if (!zipCode || zipCode.trim().length === 0) {
      this.handleError('Please enter a valid zip code.');
      return;
    }

    this.performLocationSearch(zipCode.trim());
  }

  /**
   * Perform location search using the service
   */
  private performLocationSearch(zipCode: string): void {
    // Set loading state
    this.setLoadingState(true);

    // Call the location service
    this.locationService.searchLocations(zipCode, this.defaultSearchLimit).subscribe({
      next: (response: LocationSearchResponse) => {
        this.handleSearchSuccess(response);
      },
      error: (error: any) => {
        this.handleSearchError(error);
      },
    });
  }

  /**
   * Handle successful search results
   */
  private handleSearchSuccess(response: LocationSearchResponse): void {
    if (response.success && response.data) {
      this.locationListState = {
        isLoading: false,
        hasError: false,
        errorMessage: '',
        locations: response.data,
        totalCount: response.data.length,
      };
    } else {
      // Handle API success but no data
      this.handleError(response.message || 'No locations found for this zip code.');
    }
  }

  /**
   * Handle search errors
   */
  private handleSearchError(error: any): void {
    console.error('Location search error:', error);

    let errorMessage = 'An error occurred while searching for locations.';

    if (error && typeof error === 'string') {
      errorMessage = error;
    } else if (error && error.message) {
      errorMessage = error.message;
    }

    this.handleError(errorMessage);
  }

  /**
   * Handle general errors
   */
  private handleError(message: string): void {
    this.locationListState = {
      isLoading: false,
      hasError: true,
      errorMessage: message,
      locations: [],
      totalCount: 0,
    };
  }

  /**
   * Set loading state
   */
  private setLoadingState(isLoading: boolean): void {
    this.locationListState = {
      ...this.locationListState,
      isLoading,
      hasError: false,
      errorMessage: '',
    };
  }

  /**
   * Reset search state to initial
   */
  private resetSearchState(): void {
    this.locationListState = {
      isLoading: false,
      hasError: false,
      errorMessage: '',
      locations: [],
      totalCount: 0,
    };
  }

  /**
   * Get current year for footer
   */
  get currentYear(): number {
    return new Date().getFullYear();
  }

  /**
   * Check if there are search results to display
   */
  get hasSearchResults(): boolean {
    return this.locationListState.locations && this.locationListState.locations.length > 0;
  }

  /**
   * Check if component is in loading state
   */
  get isLoading(): boolean {
    return this.locationListState.isLoading;
  }

  /**
   * Check if there's an error to display
   */
  get hasError(): boolean {
    return this.locationListState.hasError;
  }
}

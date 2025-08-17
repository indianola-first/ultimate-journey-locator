import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocationSearchResult } from '@app/models';
import { LocationCardComponent } from '../location-card/location-card.component';

export interface LocationListState {
  isLoading: boolean;
  hasError: boolean;
  errorMessage: string;
  locations: LocationSearchResult[];
  totalCount: number;
}

@Component({
  selector: 'app-location-list',
  standalone: true,
  imports: [CommonModule, LocationCardComponent],
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.css'],
})
export class LocationListComponent {
  @Input() state: LocationListState = {
    isLoading: false,
    hasError: false,
    errorMessage: '',
    locations: [],
    totalCount: 0,
  };

  @Input() showDistance: boolean = true;

  /**
   * Check if there are any locations to display
   */
  get hasLocations(): boolean {
    return this.state.locations && this.state.locations.length > 0;
  }

  /**
   * Check if the list is in a loading state
   */
  get isLoading(): boolean {
    return this.state.isLoading;
  }

  /**
   * Check if there's an error to display
   */
  get hasError(): boolean {
    return this.state.hasError;
  }

  /**
   * Get the error message to display
   */
  get errorMessage(): string {
    return this.state.errorMessage || 'An error occurred while searching for locations.';
  }

  /**
   * Get the total count of locations
   */
  get totalCount(): number {
    return this.state.totalCount;
  }

  /**
   * Get the locations array
   */
  get locations(): LocationSearchResult[] {
    return this.state.locations || [];
  }

  /**
   * Get a user-friendly message for empty results
   */
  getEmptyResultsMessage(): string {
    if (this.isLoading) {
      return 'Searching for locations...';
    }

    if (this.hasError) {
      return this.errorMessage;
    }

    if (!this.hasLocations) {
      return 'No locations found near this zip code. Please try a different zip code.';
    }

    return '';
  }

  /**
   * Get the appropriate CSS class for the results container
   */
  getResultsContainerClass(): string {
    if (this.isLoading) {
      return 'location-list-container loading';
    }

    if (this.hasError) {
      return 'location-list-container error';
    }

    if (!this.hasLocations) {
      return 'location-list-container empty';
    }

    return 'location-list-container has-results';
  }

  /**
   * Track locations by their ID for better performance
   */
  trackByLocationId(index: number, location: LocationSearchResult): number {
    return location.id;
  }
}

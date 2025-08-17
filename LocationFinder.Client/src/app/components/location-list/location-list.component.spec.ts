import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { LocationListComponent, LocationListState } from './location-list.component';
import { LocationSearchResult } from '@app/models';

describe('LocationListComponent', () => {
  let component: LocationListComponent;
  let fixture: ComponentFixture<LocationListComponent>;

  const mockLocationSearchResult: LocationSearchResult = {
    id: 1,
    name: 'Test Location',
    address: '123 Test St',
    city: 'Test City',
    state: 'TS',
    zipCode: '10001',
    phone: '555-555-0100',
    businessHours: 'Mon-Fri 9AM-6PM',
    distanceMiles: 1.5,
  };

  const mockState: LocationListState = {
    isLoading: false,
    hasError: false,
    errorMessage: '',
    locations: [mockLocationSearchResult],
    totalCount: 1,
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocationListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(LocationListComponent);
    component = fixture.componentInstance;
    component.state = mockState;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default state', () => {
    const defaultState: LocationListState = {
      isLoading: false,
      hasError: false,
      errorMessage: '',
      locations: [],
      totalCount: 0,
    };

    component.state = defaultState;
    fixture.detectChanges();

    expect(component.hasLocations).toBeFalsy();
    expect(component.isLoading).toBeFalsy();
    expect(component.hasError).toBeFalsy();
    expect(component.totalCount).toBe(0);
  });

  it('should display loading state when isLoading is true', () => {
    component.state = {
      ...mockState,
      isLoading: true,
    };
    fixture.detectChanges();

    const loadingElement = fixture.debugElement.query(By.css('.loading-state'));
    expect(loadingElement).toBeTruthy();
    expect(loadingElement.nativeElement.textContent).toContain('Searching for locations');
  });

  it('should not display loading state when isLoading is false', () => {
    component.state = {
      ...mockState,
      isLoading: false,
    };
    fixture.detectChanges();

    const loadingElement = fixture.debugElement.query(By.css('.loading-state'));
    expect(loadingElement).toBeFalsy();
  });

  it('should display error state when hasError is true', () => {
    const errorMessage = 'Test error message';
    component.state = {
      ...mockState,
      hasError: true,
      errorMessage: errorMessage,
    };
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.error-state'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent).toContain(errorMessage);
  });

  it('should not display error state when hasError is false', () => {
    component.state = {
      ...mockState,
      hasError: false,
    };
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.error-state'));
    expect(errorElement).toBeFalsy();
  });

  it('should display empty state when no locations and not loading and no error', () => {
    component.state = {
      isLoading: false,
      hasError: false,
      errorMessage: '',
      locations: [],
      totalCount: 0,
    };
    fixture.detectChanges();

    const emptyElement = fixture.debugElement.query(By.css('.empty-state'));
    expect(emptyElement).toBeTruthy();
    expect(emptyElement.nativeElement.textContent).toContain('No Locations Found');
  });

  it('should display results when locations are available', () => {
    component.state = {
      ...mockState,
      isLoading: false,
      hasError: false,
    };
    fixture.detectChanges();

    const resultsHeader = fixture.debugElement.query(By.css('.results-header'));
    expect(resultsHeader).toBeTruthy();
    expect(resultsHeader.nativeElement.textContent).toContain('Nearby Locations');

    const resultsGrid = fixture.debugElement.query(By.css('.results-grid'));
    expect(resultsGrid).toBeTruthy();
  });

  it('should display correct location count', () => {
    component.state = {
      ...mockState,
      totalCount: 5,
    };
    fixture.detectChanges();

    const badgeElement = fixture.debugElement.query(By.css('.badge'));
    expect(badgeElement).toBeTruthy();
    expect(badgeElement.nativeElement.textContent).toContain('5 locations found');
  });

  it('should display singular form for single location', () => {
    component.state = {
      ...mockState,
      totalCount: 1,
    };
    fixture.detectChanges();

    const badgeElement = fixture.debugElement.query(By.css('.badge'));
    expect(badgeElement.nativeElement.textContent).toContain('1 location found');
  });

  it('should display location cards for each location', () => {
    const multipleLocations = [
      { ...mockLocationSearchResult, id: 1, name: 'Location 1' },
      { ...mockLocationSearchResult, id: 2, name: 'Location 2' },
      { ...mockLocationSearchResult, id: 3, name: 'Location 3' },
    ];

    component.state = {
      ...mockState,
      locations: multipleLocations,
      totalCount: 3,
    };
    fixture.detectChanges();

    const locationCards = fixture.debugElement.queryAll(By.css('app-location-card'));
    expect(locationCards.length).toBe(3);
  });

  it('should pass correct props to location cards', () => {
    component.state = {
      ...mockState,
      locations: [mockLocationSearchResult],
    };
    component.showDistance = true;
    fixture.detectChanges();

    const locationCard = fixture.debugElement.query(By.css('app-location-card'));
    expect(locationCard.componentInstance.location).toEqual(mockLocationSearchResult);
    expect(locationCard.componentInstance.showDistance).toBe(true);
  });

  it('should display results footer', () => {
    component.state = {
      ...mockState,
      totalCount: 10,
    };
    fixture.detectChanges();

    const footerElement = fixture.debugElement.query(By.css('.results-footer'));
    expect(footerElement).toBeTruthy();
    expect(footerElement.nativeElement.textContent).toContain('Showing up to 10 closest locations');
  });

  it('should get correct empty results message for loading state', () => {
    component.state = {
      ...mockState,
      isLoading: true,
    };

    const message = component.getEmptyResultsMessage();
    expect(message).toBe('Searching for locations...');
  });

  it('should get correct empty results message for error state', () => {
    const errorMessage = 'Test error';
    component.state = {
      ...mockState,
      hasError: true,
      errorMessage: errorMessage,
    };

    const message = component.getEmptyResultsMessage();
    expect(message).toBe(errorMessage);
  });

  it('should get correct empty results message for no locations', () => {
    component.state = {
      ...mockState,
      isLoading: false,
      hasError: false,
      locations: [],
    };

    const message = component.getEmptyResultsMessage();
    expect(message).toBe('No locations found near this zip code. Please try a different zip code.');
  });

  it('should get correct container class for loading state', () => {
    component.state = {
      ...mockState,
      isLoading: true,
    };

    const className = component.getResultsContainerClass();
    expect(className).toBe('location-list-container loading');
  });

  it('should get correct container class for error state', () => {
    component.state = {
      ...mockState,
      hasError: true,
    };

    const className = component.getResultsContainerClass();
    expect(className).toBe('location-list-container error');
  });

  it('should get correct container class for empty state', () => {
    component.state = {
      ...mockState,
      locations: [],
    };

    const className = component.getResultsContainerClass();
    expect(className).toBe('location-list-container empty');
  });

  it('should get correct container class for results state', () => {
    component.state = {
      ...mockState,
      locations: [mockLocationSearchResult],
    };

    const className = component.getResultsContainerClass();
    expect(className).toBe('location-list-container has-results');
  });

  it('should track locations by ID', () => {
    const location1 = { ...mockLocationSearchResult, id: 1 };
    const location2 = { ...mockLocationSearchResult, id: 2 };

    const trackResult1 = component.trackByLocationId(0, location1);
    const trackResult2 = component.trackByLocationId(1, location2);

    expect(trackResult1).toBe(1);
    expect(trackResult2).toBe(2);
  });

  it('should handle null locations array', () => {
    component.state = {
      ...mockState,
      locations: null as any,
    };

    expect(component.hasLocations).toBeFalsy();
    expect(component.locations).toEqual([]);
  });

  it('should handle undefined locations array', () => {
    component.state = {
      ...mockState,
      locations: undefined as any,
    };

    expect(component.hasLocations).toBeFalsy();
    expect(component.locations).toEqual([]);
  });

  it('should handle empty error message', () => {
    component.state = {
      ...mockState,
      hasError: true,
      errorMessage: '',
    };

    expect(component.errorMessage).toBe('An error occurred while searching for locations.');
  });

  it('should handle null error message', () => {
    component.state = {
      ...mockState,
      hasError: true,
      errorMessage: null as any,
    };

    expect(component.errorMessage).toBe('An error occurred while searching for locations.');
  });

  it('should display subtitle text', () => {
    component.state = {
      ...mockState,
      locations: [mockLocationSearchResult],
    };
    fixture.detectChanges();

    const subtitleElement = fixture.debugElement.query(By.css('.results-subtitle'));
    expect(subtitleElement).toBeTruthy();
    expect(subtitleElement.nativeElement.textContent).toContain('Locations are sorted by distance');
  });

  it('should not display results when loading', () => {
    component.state = {
      ...mockState,
      isLoading: true,
    };
    fixture.detectChanges();

    const resultsHeader = fixture.debugElement.query(By.css('.results-header'));
    const resultsGrid = fixture.debugElement.query(By.css('.results-grid'));
    const resultsFooter = fixture.debugElement.query(By.css('.results-footer'));

    expect(resultsHeader).toBeFalsy();
    expect(resultsGrid).toBeFalsy();
    expect(resultsFooter).toBeFalsy();
  });

  it('should not display results when error', () => {
    component.state = {
      ...mockState,
      hasError: true,
    };
    fixture.detectChanges();

    const resultsHeader = fixture.debugElement.query(By.css('.results-header'));
    const resultsGrid = fixture.debugElement.query(By.css('.results-grid'));
    const resultsFooter = fixture.debugElement.query(By.css('.results-footer'));

    expect(resultsHeader).toBeFalsy();
    expect(resultsGrid).toBeFalsy();
    expect(resultsFooter).toBeFalsy();
  });

  it('should handle showDistance input property', () => {
    component.showDistance = false;
    fixture.detectChanges();

    expect(component.showDistance).toBe(false);
  });

  it('should have proper accessibility attributes', () => {
    const loadingElement = fixture.debugElement.query(By.css('.loading-state'));
    if (loadingElement) {
      const spinner = loadingElement.query(By.css('.spinner-border'));
      expect(spinner.nativeElement.getAttribute('role')).toBe('status');
    }
  });

  it('should handle multiple rapid state changes', () => {
    // Test rapid state changes
    component.state = { ...mockState, isLoading: true };
    fixture.detectChanges();

    component.state = { ...mockState, hasError: true };
    fixture.detectChanges();

    component.state = { ...mockState, locations: [] };
    fixture.detectChanges();

    component.state = { ...mockState, locations: [mockLocationSearchResult] };
    fixture.detectChanges();

    expect(component.hasLocations).toBeTruthy();
    expect(component.isLoading).toBeFalsy();
    expect(component.hasError).toBeFalsy();
  });
});

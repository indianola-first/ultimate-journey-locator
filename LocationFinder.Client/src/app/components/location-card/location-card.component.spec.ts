import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { LocationCardComponent } from './location-card.component';
import { LocationSearchResult } from '@app/models';

describe('LocationCardComponent', () => {
  let component: LocationCardComponent;
  let fixture: ComponentFixture<LocationCardComponent>;

  const mockLocation: LocationSearchResult = {
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

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocationCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(LocationCardComponent);
    component = fixture.componentInstance;
    component.location = mockLocation;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display location name', () => {
    const nameElement = fixture.debugElement.query(By.css('.location-name'));
    expect(nameElement).toBeTruthy();
    expect(nameElement.nativeElement.textContent).toContain('Test Location');
  });

  it('should display location address', () => {
    const addressElement = fixture.debugElement.query(By.css('.location-address'));
    expect(addressElement).toBeTruthy();
    expect(addressElement.nativeElement.textContent).toContain('123 Test St');
    expect(addressElement.nativeElement.textContent).toContain('Test City, TS 10001');
  });

  it('should display location phone number', () => {
    const phoneElement = fixture.debugElement.query(By.css('.location-phone'));
    expect(phoneElement).toBeTruthy();
    expect(phoneElement.nativeElement.textContent).toContain('555-555-0100');
  });

  it('should display business hours', () => {
    const hoursElement = fixture.debugElement.query(By.css('.location-hours'));
    expect(hoursElement).toBeTruthy();
    expect(hoursElement.nativeElement.textContent).toContain('Mon-Fri 9AM-6PM');
  });

  it('should display distance badge when showDistance is true', () => {
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge'));
    expect(distanceBadge).toBeTruthy();
    expect(distanceBadge.nativeElement.textContent).toContain('1.5 miles');
  });

  it('should not display distance badge when showDistance is false', () => {
    component.showDistance = false;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge'));
    expect(distanceBadge).toBeFalsy();
  });

  it('should format distance correctly', () => {
    component.location = { ...mockLocation, distanceMiles: 0.5 };
    expect(component.formatDistance(0.5)).toBe('0.5 miles');

    component.location = { ...mockLocation, distanceMiles: 1.0 };
    expect(component.formatDistance(1.0)).toBe('1.0 miles');

    component.location = { ...mockLocation, distanceMiles: 2.5 };
    expect(component.formatDistance(2.5)).toBe('2.5 miles');
  });

  it('should format phone number correctly', () => {
    const formattedPhone = component.formatPhone('555-555-0100');
    expect(formattedPhone).toBe('(555) 555-0100');
  });

  it('should generate correct phone URL', () => {
    const phoneUrl = component.getPhoneUrl('555-555-0100');
    expect(phoneUrl).toBe('tel:555-555-0100');
  });

  it('should generate correct maps URL', () => {
    const mapsUrl = component.getMapsUrl();
    expect(mapsUrl).toContain('https://maps.google.com');
    expect(mapsUrl).toContain('123 Test St, Test City, TS 10001');
  });

  it('should have clickable phone number', () => {
    const phoneElement = fixture.debugElement.query(By.css('.location-phone'));
    expect(phoneElement.nativeElement.href).toContain('tel:555-555-0100');
  });

  it('should have call button', () => {
    const callButton = fixture.debugElement.query(By.css('.btn-call'));
    expect(callButton).toBeTruthy();
    expect(callButton.nativeElement.textContent).toContain('Call');
    expect(callButton.nativeElement.href).toContain('tel:555-555-0100');
  });

  it('should have directions button', () => {
    const directionsButton = fixture.debugElement.query(By.css('.btn-directions'));
    expect(directionsButton).toBeTruthy();
    expect(directionsButton.nativeElement.textContent).toContain('Directions');
    expect(directionsButton.nativeElement.href).toContain('https://maps.google.com');
  });

  it('should handle location without phone number', () => {
    component.location = { ...mockLocation, phone: '' };
    fixture.detectChanges();

    expect(component.hasPhone).toBeFalsy();

    const phoneElement = fixture.debugElement.query(By.css('.location-phone'));
    expect(phoneElement).toBeFalsy();

    const callButton = fixture.debugElement.query(By.css('.btn-call'));
    expect(callButton).toBeFalsy();
  });

  it('should handle location without business hours', () => {
    component.location = { ...mockLocation, businessHours: '' };
    fixture.detectChanges();

    expect(component.hasBusinessHours).toBeFalsy();

    const hoursElement = fixture.debugElement.query(By.css('.location-hours'));
    expect(hoursElement).toBeFalsy();
  });

  it('should handle location with null phone number', () => {
    component.location = { ...mockLocation, phone: null as any };
    fixture.detectChanges();

    expect(component.hasPhone).toBeFalsy();
  });

  it('should handle location with null business hours', () => {
    component.location = { ...mockLocation, businessHours: null as any };
    fixture.detectChanges();

    expect(component.hasBusinessHours).toBeFalsy();
  });

  it('should display distance badge with correct color for close distance', () => {
    component.location = { ...mockLocation, distanceMiles: 0.5 };
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge.close'));
    expect(distanceBadge).toBeTruthy();
  });

  it('should display distance badge with correct color for medium distance', () => {
    component.location = { ...mockLocation, distanceMiles: 5.0 };
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge.medium'));
    expect(distanceBadge).toBeTruthy();
  });

  it('should display distance badge with correct color for far distance', () => {
    component.location = { ...mockLocation, distanceMiles: 15.0 };
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge.far'));
    expect(distanceBadge).toBeTruthy();
  });

  it('should have proper card structure', () => {
    const cardElement = fixture.debugElement.query(By.css('.location-card'));
    expect(cardElement).toBeTruthy();

    const headerElement = fixture.debugElement.query(By.css('.location-card-header'));
    expect(headerElement).toBeTruthy();

    const bodyElement = fixture.debugElement.query(By.css('.location-card-body'));
    expect(bodyElement).toBeTruthy();

    const footerElement = fixture.debugElement.query(By.css('.location-card-footer'));
    expect(footerElement).toBeTruthy();
  });

  it('should display location icon in name', () => {
    const nameElement = fixture.debugElement.query(By.css('.location-name'));
    expect(nameElement.nativeElement.textContent).toContain('🏢');
  });

  it('should display address icon', () => {
    const addressElement = fixture.debugElement.query(By.css('.location-address'));
    expect(addressElement.nativeElement.textContent).toContain('📍');
  });

  it('should display phone icon', () => {
    const phoneElement = fixture.debugElement.query(By.css('.location-phone'));
    expect(phoneElement.nativeElement.textContent).toContain('📞');
  });

  it('should display hours icon', () => {
    const hoursElement = fixture.debugElement.query(By.css('.location-hours'));
    expect(hoursElement.nativeElement.textContent).toContain('🕒');
  });

  it('should display distance icon in badge', () => {
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge'));
    expect(distanceBadge.nativeElement.textContent).toContain('📍');
  });

  it('should display call icon in call button', () => {
    const callButton = fixture.debugElement.query(By.css('.btn-call'));
    expect(callButton.nativeElement.textContent).toContain('📞');
  });

  it('should display directions icon in directions button', () => {
    const directionsButton = fixture.debugElement.query(By.css('.btn-directions'));
    expect(directionsButton.nativeElement.textContent).toContain('🗺️');
  });

  it('should handle special characters in address', () => {
    component.location = {
      ...mockLocation,
      address: '123 Test St, Apt #5',
      city: 'Test City',
      state: 'TS',
      zipCode: '10001',
    };
    fixture.detectChanges();

    const mapsUrl = component.getMapsUrl();
    expect(mapsUrl).toContain('123 Test St, Apt #5, Test City, TS 10001');
  });

  it('should handle phone number with extensions', () => {
    const formattedPhone = component.formatPhone('555-555-0100 ext 123');
    expect(formattedPhone).toBe('(555) 555-0100 ext 123');
  });

  it('should handle phone number with parentheses', () => {
    const formattedPhone = component.formatPhone('(555) 555-0100');
    expect(formattedPhone).toBe('(555) 555-0100');
  });

  it('should handle phone number with spaces', () => {
    const formattedPhone = component.formatPhone('555 555 0100');
    expect(formattedPhone).toBe('(555) 555-0100');
  });

  it('should handle phone number with dots', () => {
    const formattedPhone = component.formatPhone('555.555.0100');
    expect(formattedPhone).toBe('(555) 555-0100');
  });

  it('should have proper accessibility attributes', () => {
    const callButton = fixture.debugElement.query(By.css('.btn-call'));
    expect(callButton.nativeElement.getAttribute('type')).toBe('button');

    const directionsButton = fixture.debugElement.query(By.css('.btn-directions'));
    expect(directionsButton.nativeElement.getAttribute('type')).toBe('button');
  });

  it('should handle very long location names', () => {
    component.location = {
      ...mockLocation,
      name: 'This is a very long location name that should be handled properly by the component',
    };
    fixture.detectChanges();

    const nameElement = fixture.debugElement.query(By.css('.location-name'));
    expect(nameElement).toBeTruthy();
    expect(nameElement.nativeElement.textContent).toContain('This is a very long location name');
  });

  it('should handle very long addresses', () => {
    component.location = {
      ...mockLocation,
      address: 'This is a very long address that should be handled properly by the component',
    };
    fixture.detectChanges();

    const addressElement = fixture.debugElement.query(By.css('.location-address'));
    expect(addressElement).toBeTruthy();
    expect(addressElement.nativeElement.textContent).toContain('This is a very long address');
  });

  it('should handle very long business hours', () => {
    component.location = {
      ...mockLocation,
      businessHours:
        'Monday through Friday 9:00 AM to 6:00 PM, Saturday 10:00 AM to 4:00 PM, Sunday Closed',
    };
    fixture.detectChanges();

    const hoursElement = fixture.debugElement.query(By.css('.location-hours'));
    expect(hoursElement).toBeTruthy();
    expect(hoursElement.nativeElement.textContent).toContain('Monday through Friday');
  });

  it('should handle zero distance', () => {
    component.location = { ...mockLocation, distanceMiles: 0 };
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge'));
    expect(distanceBadge).toBeTruthy();
    expect(distanceBadge.nativeElement.textContent).toContain('0.0 miles');
  });

  it('should handle very large distances', () => {
    component.location = { ...mockLocation, distanceMiles: 999.9 };
    component.showDistance = true;
    fixture.detectChanges();

    const distanceBadge = fixture.debugElement.query(By.css('.distance-badge'));
    expect(distanceBadge).toBeTruthy();
    expect(distanceBadge.nativeElement.textContent).toContain('999.9 miles');
  });

  it('should handle rapid input changes', () => {
    // Test rapid changes to input properties
    component.location = { ...mockLocation, name: 'Location 1' };
    fixture.detectChanges();

    component.location = { ...mockLocation, name: 'Location 2' };
    fixture.detectChanges();

    component.showDistance = true;
    fixture.detectChanges();

    component.showDistance = false;
    fixture.detectChanges();

    const nameElement = fixture.debugElement.query(By.css('.location-name'));
    expect(nameElement.nativeElement.textContent).toContain('Location 2');
  });
});

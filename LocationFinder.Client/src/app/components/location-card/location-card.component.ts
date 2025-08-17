import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocationSearchResult } from '@app/models';

@Component({
  selector: 'app-location-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './location-card.component.html',
  styleUrls: ['./location-card.component.css'],
})
export class LocationCardComponent {
  @Input() location!: LocationSearchResult;
  @Input() showDistance: boolean = true;

  /**
   * Format distance with appropriate units and precision
   * @param distance - Distance in miles
   * @returns Formatted distance string
   */
  formatDistance(distance: number): string {
    if (distance < 0.1) {
      return 'Less than 0.1 miles';
    } else if (distance < 1) {
      return `${distance.toFixed(1)} miles`;
    } else if (distance < 10) {
      return `${distance.toFixed(1)} miles`;
    } else {
      return `${Math.round(distance)} miles`;
    }
  }

  /**
   * Format phone number for display
   * @param phone - Raw phone number string
   * @returns Formatted phone number
   */
  formatPhone(phone: string): string {
    if (!phone) return '';

    // Remove all non-digit characters
    const digits = phone.replace(/\D/g, '');

    // Format based on length
    if (digits.length === 10) {
      return `(${digits.slice(0, 3)}) ${digits.slice(3, 6)}-${digits.slice(6)}`;
    } else if (digits.length === 11 && digits[0] === '1') {
      return `(${digits.slice(1, 4)}) ${digits.slice(4, 7)}-${digits.slice(7)}`;
    }

    // Return original if can't format
    return phone;
  }

  /**
   * Get click-to-call phone number URL
   * @param phone - Phone number string
   * @returns tel: URL for click-to-call
   */
  getPhoneUrl(phone: string): string {
    if (!phone) return '';

    // Remove all non-digit characters and ensure it starts with 1 for US numbers
    const digits = phone.replace(/\D/g, '');
    const formattedNumber = digits.length === 10 ? `1${digits}` : digits;

    return `tel:+${formattedNumber}`;
  }

  /**
   * Get Google Maps URL for the location
   * @returns Google Maps URL
   */
  getMapsUrl(): string {
    const address = `${this.location.address}, ${this.location.city}, ${this.location.state} ${this.location.zipCode}`;
    const encodedAddress = encodeURIComponent(address);
    return `https://www.google.com/maps/search/?api=1&query=${encodedAddress}`;
  }

  /**
   * Check if location has business hours
   * @returns True if business hours are available
   */
  hasBusinessHours(): boolean {
    return !!(this.location.businessHours && this.location.businessHours.trim());
  }

  /**
   * Check if location has phone number
   * @returns True if phone number is available
   */
  hasPhone(): boolean {
    return !!(this.location.phone && this.location.phone.trim());
  }

  /**
   * Get distance badge color class based on distance
   * @returns Bootstrap color class
   */
  getDistanceBadgeClass(): string {
    const distance = this.location.distanceMiles;

    if (distance < 1) return 'bg-success';
    if (distance < 5) return 'bg-primary';
    if (distance < 10) return 'bg-warning';
    return 'bg-secondary';
  }
}

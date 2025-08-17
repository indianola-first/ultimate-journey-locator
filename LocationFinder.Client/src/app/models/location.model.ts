/**
 * Represents a business location in the system
 */
export interface Location {
  /** Unique identifier for the location */
  id: number;

  /** Name of the business location */
  name: string;

  /** Street address of the location */
  address: string;

  /** City where the location is situated */
  city: string;

  /** State or province where the location is situated */
  state: string;

  /** 5-digit zip code of the location */
  zipCode: string;

  /** Phone number for the location (optional) */
  phone?: string;

  /** Latitude coordinate of the location */
  latitude: number;

  /** Longitude coordinate of the location */
  longitude: number;

  /** Business hours information (optional) */
  businessHours?: string;

  /** Whether the location is currently active */
  isActive: boolean;

  /** Date when the location was created */
  createdDate: Date;
}

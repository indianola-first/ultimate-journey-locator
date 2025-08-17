/**
 * Represents a location search result with distance calculation
 */
export interface LocationSearchResult {
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

  /** Business hours information (optional) */
  businessHours?: string;

  /** Distance from the search zip code in miles */
  distanceMiles: number;
}

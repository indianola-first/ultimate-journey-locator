import { LocationSearchResult } from './location-search-result.model';

/**
 * Generic API response wrapper for consistent response format
 */
export interface ApiResponse<T> {
  /** Indicates whether the operation was successful */
  success: boolean;

  /** The actual data payload (null if operation failed) */
  data: T | null;

  /** Human-readable message describing the result or error */
  message: string;
}

/**
 * Type alias for location search API response
 */
export type LocationSearchResponse = ApiResponse<LocationSearchResult[]>;

/**
 * Type alias for single location API response
 */
export type LocationResponse = ApiResponse<LocationSearchResult>;

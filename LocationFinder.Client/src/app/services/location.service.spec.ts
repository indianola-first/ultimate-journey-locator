import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LocationService } from './location.service';
import { LocationSearchResult, LocationSearchResponse } from '@app/models';
import { environment } from '@env/environment';

describe('LocationService', () => {
  let service: LocationService;
  let httpMock: HttpTestingController;

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

  const mockSuccessResponse: LocationSearchResponse = {
    success: true,
    data: [mockLocationSearchResult],
    message: '',
  };

  const mockErrorResponse = {
    success: false,
    data: null,
    message: 'Test error message',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [LocationService],
    });
    service = TestBed.inject(LocationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('searchLocations', () => {
    it('should return locations when API call is successful', done => {
      const zipCode = '10001';
      const limit = 10;
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;

      service.searchLocations(zipCode, limit).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          expect(response.success).toBe(true);
          expect(response.data).toEqual([mockLocationSearchResult]);
          expect(response.message).toBe('');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockSuccessResponse);
    });

    it('should use default limit when limit is not provided', done => {
      const zipCode = '10001';
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`;

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockSuccessResponse);
    });

    it('should handle API error response', done => {
      const zipCode = 'invalid';
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`;

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('invalid');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush(mockErrorResponse, { status: 400, statusText: 'Bad Request' });
    });

    it('should handle network error', done => {
      const zipCode = '10001';
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`;

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('network');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.error(new ErrorEvent('Network error'));
    });

    it('should handle 404 error', done => {
      const zipCode = '99999';
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`;

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('not found');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush({ message: 'Zip code not found' }, { status: 404, statusText: 'Not Found' });
    });

    it('should handle 500 error', done => {
      const zipCode = '10001';
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`;

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('server');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush(
        { message: 'Internal server error' },
        { status: 500, statusText: 'Internal Server Error' }
      );
    });

    it('should validate zip code format before making request', () => {
      const invalidZipCode = 'invalid';

      service.searchLocations(invalidZipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('valid 5-digit zip code');
        },
      });

      // Should not make HTTP request for invalid zip code
      httpMock.expectNone(`${environment.apiUrl}/locations/search`);
    });

    it('should handle empty zip code', () => {
      const emptyZipCode = '';

      service.searchLocations(emptyZipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('required');
        },
      });

      // Should not make HTTP request for empty zip code
      httpMock.expectNone(`${environment.apiUrl}/locations/search`);
    });

    it('should handle null zip code', () => {
      const nullZipCode = null as any;

      service.searchLocations(nullZipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('required');
        },
      });

      // Should not make HTTP request for null zip code
      httpMock.expectNone(`${environment.apiUrl}/locations/search`);
    });

    it('should handle whitespace in zip code', done => {
      const zipCodeWithWhitespace = '  10001  ';
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=10001&limit=20`;

      service.searchLocations(zipCodeWithWhitespace).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush(mockSuccessResponse);
    });

    it('should handle multiple concurrent requests', done => {
      const zipCode1 = '10001';
      const zipCode2 = '90210';
      const expectedUrl1 = `${environment.apiUrl}/locations/search?zipcode=${zipCode1}&limit=20`;
      const expectedUrl2 = `${environment.apiUrl}/locations/search?zipcode=${zipCode2}&limit=20`;

      let completedRequests = 0;
      const totalRequests = 2;

      service.searchLocations(zipCode1).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          completedRequests++;
          if (completedRequests === totalRequests) done();
        },
        error: done.fail,
      });

      service.searchLocations(zipCode2).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          completedRequests++;
          if (completedRequests === totalRequests) done();
        },
        error: done.fail,
      });

      const req1 = httpMock.expectOne(expectedUrl1);
      const req2 = httpMock.expectOne(expectedUrl2);

      req1.flush(mockSuccessResponse);
      req2.flush(mockSuccessResponse);
    });

    it('should handle large limit values', done => {
      const zipCode = '10001';
      const largeLimit = 50;
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${largeLimit}`;

      service.searchLocations(zipCode, largeLimit).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush(mockSuccessResponse);
    });

    it('should handle zero limit', done => {
      const zipCode = '10001';
      const zeroLimit = 0;
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${zeroLimit}`;

      service.searchLocations(zipCode, zeroLimit).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush(mockSuccessResponse);
    });

    it('should handle negative limit', done => {
      const zipCode = '10001';
      const negativeLimit = -5;
      const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${negativeLimit}`;

      service.searchLocations(zipCode, negativeLimit).subscribe({
        next: response => {
          expect(response).toEqual(mockSuccessResponse);
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(expectedUrl);
      req.flush(mockSuccessResponse);
    });
  });

  describe('isValidZipCode', () => {
    it('should validate correct zip code format', () => {
      expect(service['isValidZipCode']('10001')).toBe(true);
      expect(service['isValidZipCode']('90210')).toBe(true);
      expect(service['isValidZipCode']('12345')).toBe(true);
    });

    it('should reject invalid zip code format', () => {
      expect(service['isValidZipCode']('1234')).toBe(false);
      expect(service['isValidZipCode']('123456')).toBe(false);
      expect(service['isValidZipCode']('abc12')).toBe(false);
      expect(service['isValidZipCode']('')).toBe(false);
      expect(service['isValidZipCode'](null as any)).toBe(false);
      expect(service['isValidZipCode'](undefined as any)).toBe(false);
    });

    it('should handle whitespace in zip code', () => {
      expect(service['isValidZipCode']('  10001  ')).toBe(true);
      expect(service['isValidZipCode']('  1234  ')).toBe(false);
    });
  });

  describe('handleError', () => {
    it('should handle HttpErrorResponse with status 400', done => {
      const zipCode = 'invalid';

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('invalid');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(
        `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`
      );
      req.flush({ message: 'Invalid zip code' }, { status: 400, statusText: 'Bad Request' });
    });

    it('should handle HttpErrorResponse with status 404', done => {
      const zipCode = '99999';

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('not found');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(
        `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`
      );
      req.flush({ message: 'Zip code not found' }, { status: 404, statusText: 'Not Found' });
    });

    it('should handle HttpErrorResponse with status 500', done => {
      const zipCode = '10001';

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('server');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(
        `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`
      );
      req.flush(
        { message: 'Internal server error' },
        { status: 500, statusText: 'Internal Server Error' }
      );
    });

    it('should handle network error', done => {
      const zipCode = '10001';

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('network');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(
        `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`
      );
      req.error(new ErrorEvent('Network error'));
    });

    it('should handle unknown error', done => {
      const zipCode = '10001';

      service.searchLocations(zipCode).subscribe({
        next: response => {
          expect(response.success).toBe(false);
          expect(response.data).toBeNull();
          expect(response.message).toContain('unexpected');
          done();
        },
        error: done.fail,
      });

      const req = httpMock.expectOne(
        `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=20`
      );
      req.flush({ message: 'Unknown error' }, { status: 418, statusText: "I'm a teapot" });
    });
  });

  describe('getApiUrl', () => {
    it('should return correct API URL', () => {
      const apiUrl = service['getApiUrl']();
      expect(apiUrl).toBe(environment.apiUrl);
    });
  });

  describe('isProduction', () => {
    it('should return production status', () => {
      const isProd = service['isProduction']();
      expect(typeof isProd).toBe('boolean');
    });
  });
});

import { HttpTestingController } from '@angular/common/http/testing';
import { environment } from '@env/environment';
import { TestUtils } from './test-utils';

/**
 * HTTP testing helpers for common testing patterns
 */
export class HttpTestingHelpers {
  /**
   * Expects a GET request to the locations search endpoint
   */
  static expectLocationSearchRequest(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');
    req.flush(response);
    return req;
  }

  /**
   * Expects a GET request to the locations search endpoint with error response
   */
  static expectLocationSearchRequestWithError(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    errorResponse: any = TestUtils.createMockErrorResponse(),
    status: number = 400,
    statusText: string = 'Bad Request'
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');
    req.flush(errorResponse, { status, statusText });
    return req;
  }

  /**
   * Expects a GET request to the locations search endpoint with network error
   */
  static expectLocationSearchRequestWithNetworkError(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');
    req.error(new ErrorEvent('Network error'));
    return req;
  }

  /**
   * Expects a GET request to the locations search endpoint with timeout error
   */
  static expectLocationSearchRequestWithTimeoutError(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');
    req.error(new ErrorEvent('Timeout error'));
    return req;
  }

  /**
   * Expects a GET request to the locations search endpoint with 404 error
   */
  static expectLocationSearchRequestWith404Error(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    return this.expectLocationSearchRequestWithError(
      httpMock,
      zipCode,
      limit,
      { message: 'Zip code not found' },
      404,
      'Not Found'
    );
  }

  /**
   * Expects a GET request to the locations search endpoint with 500 error
   */
  static expectLocationSearchRequestWith500Error(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    return this.expectLocationSearchRequestWithError(
      httpMock,
      zipCode,
      limit,
      { message: 'Internal server error' },
      500,
      'Internal Server Error'
    );
  }

  /**
   * Expects a GET request to the locations search endpoint with 403 error
   */
  static expectLocationSearchRequestWith403Error(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    return this.expectLocationSearchRequestWithError(
      httpMock,
      zipCode,
      limit,
      { message: 'Access denied' },
      403,
      'Forbidden'
    );
  }

  /**
   * Expects a GET request to the locations search endpoint with 401 error
   */
  static expectLocationSearchRequestWith401Error(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    return this.expectLocationSearchRequestWithError(
      httpMock,
      zipCode,
      limit,
      { message: 'Authentication required' },
      401,
      'Unauthorized'
    );
  }

  /**
   * Expects a GET request to the locations search endpoint with 429 error
   */
  static expectLocationSearchRequestWith429Error(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    return this.expectLocationSearchRequestWithError(
      httpMock,
      zipCode,
      limit,
      { message: 'Too many requests' },
      429,
      'Too Many Requests'
    );
  }

  /**
   * Expects a GET request to the locations search endpoint with 503 error
   */
  static expectLocationSearchRequestWith503Error(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20
  ) {
    return this.expectLocationSearchRequestWithError(
      httpMock,
      zipCode,
      limit,
      { message: 'Service temporarily unavailable' },
      503,
      'Service Unavailable'
    );
  }

  /**
   * Expects no HTTP request to be made (for validation failures)
   */
  static expectNoHttpRequest(httpMock: HttpTestingController) {
    httpMock.expectNone(`${environment.apiUrl}/locations/search`);
  }

  /**
   * Expects multiple concurrent requests
   */
  static expectMultipleLocationSearchRequests(
    httpMock: HttpTestingController,
    requests: Array<{
      zipCode: string;
      limit?: number;
      response?: any;
    }>
  ) {
    const reqs = requests.map(req => {
      const limit = req.limit || 20;
      const response = req.response || TestUtils.createMockSuccessResponse();
      return this.expectLocationSearchRequest(httpMock, req.zipCode, limit, response);
    });
    return reqs;
  }

  /**
   * Expects multiple concurrent requests with different responses
   */
  static expectMultipleLocationSearchRequestsWithMixedResponses(
    httpMock: HttpTestingController,
    requests: Array<{
      zipCode: string;
      limit?: number;
      response?: any;
      status?: number;
      statusText?: string;
      isError?: boolean;
    }>
  ) {
    const reqs = requests.map(req => {
      const limit = req.limit || 20;

      if (req.isError) {
        const errorResponse = req.response || TestUtils.createMockErrorResponse();
        const status = req.status || 400;
        const statusText = req.statusText || 'Bad Request';
        return this.expectLocationSearchRequestWithError(
          httpMock,
          req.zipCode,
          limit,
          errorResponse,
          status,
          statusText
        );
      } else {
        const response = req.response || TestUtils.createMockSuccessResponse();
        return this.expectLocationSearchRequest(httpMock, req.zipCode, limit, response);
      }
    });
    return reqs;
  }

  /**
   * Expects a request with specific query parameters
   */
  static expectLocationSearchRequestWithQueryParams(
    httpMock: HttpTestingController,
    params: {
      zipCode: string;
      limit?: number;
      [key: string]: any;
    },
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const queryParams = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        queryParams.append(key, value.toString());
      }
    });

    const expectedUrl = `${environment.apiUrl}/locations/search?${queryParams.toString()}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');
    req.flush(response);
    return req;
  }

  /**
   * Expects a request with custom headers
   */
  static expectLocationSearchRequestWithHeaders(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    headers: { [key: string]: string } = {},
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    // Verify headers
    Object.entries(headers).forEach(([key, value]) => {
      expect(req.request.headers.get(key)).toBe(value);
    });

    req.flush(response);
    return req;
  }

  /**
   * Expects a request with specific timeout
   */
  static expectLocationSearchRequestWithTimeout(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    timeoutMs: number = 1000
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    // Simulate timeout
    setTimeout(() => {
      req.error(new ErrorEvent('Timeout error'));
    }, timeoutMs);

    return req;
  }

  /**
   * Expects a request with retry logic
   */
  static expectLocationSearchRequestWithRetry(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    retryCount: number = 3
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const reqs = [];

    for (let i = 0; i < retryCount; i++) {
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('GET');

      if (i < retryCount - 1) {
        // Fail all but the last request
        req.error(new ErrorEvent('Network error'));
      } else {
        // Succeed on the last request
        req.flush(TestUtils.createMockSuccessResponse());
      }

      reqs.push(req);
    }

    return reqs;
  }

  /**
   * Expects a request with rate limiting
   */
  static expectLocationSearchRequestWithRateLimit(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    retryAfterSeconds: number = 60
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    req.flush(
      { message: 'Too many requests' },
      {
        status: 429,
        statusText: 'Too Many Requests',
        headers: { 'Retry-After': retryAfterSeconds.toString() },
      }
    );

    return req;
  }

  /**
   * Expects a request with CORS headers
   */
  static expectLocationSearchRequestWithCorsHeaders(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    req.flush(response, {
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
        'Access-Control-Allow-Headers': 'Content-Type, Authorization',
      },
    });

    return req;
  }

  /**
   * Expects a request with cache headers
   */
  static expectLocationSearchRequestWithCacheHeaders(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    req.flush(response, {
      headers: {
        'Cache-Control': 'public, max-age=300',
        ETag: '"abc123"',
        'Last-Modified': 'Wed, 21 Oct 2023 07:28:00 GMT',
      },
    });

    return req;
  }

  /**
   * Expects a request with compression headers
   */
  static expectLocationSearchRequestWithCompressionHeaders(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    req.flush(response, {
      headers: {
        'Content-Encoding': 'gzip',
        'Content-Length': '1024',
        Vary: 'Accept-Encoding',
      },
    });

    return req;
  }

  /**
   * Expects a request with security headers
   */
  static expectLocationSearchRequestWithSecurityHeaders(
    httpMock: HttpTestingController,
    zipCode: string,
    limit: number = 20,
    response: any = TestUtils.createMockSuccessResponse()
  ) {
    const expectedUrl = `${environment.apiUrl}/locations/search?zipcode=${zipCode}&limit=${limit}`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    req.flush(response, {
      headers: {
        'X-Content-Type-Options': 'nosniff',
        'X-Frame-Options': 'DENY',
        'X-XSS-Protection': '1; mode=block',
        'Strict-Transport-Security': 'max-age=31536000; includeSubDomains',
      },
    });

    return req;
  }
}

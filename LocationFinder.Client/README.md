# Location Finder Client

This is the Angular 17 frontend application for the Location Finder project.

## Prerequisites

- Node.js version 18.13.0 or higher
- npm version 8.0.0 or higher

## Setup

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start development server:
   ```bash
   npm start
   ```

3. Build for production:
   ```bash
   npm run build:prod
   ```

4. Build for development (with source maps):
   ```bash
   npm run build:dev
   ```

5. Run tests:
   ```bash
   npm test
   ```

6. Run tests in headless mode:
   ```bash
   npm run test:headless
   ```

## Environment Configuration

The application uses environment-specific configuration files:

- **Development**: `src/environments/environment.ts`
  - API URL: `https://localhost:5001/api`
  - Production mode: `false`

- **Production**: `src/environments/environment.prod.ts`
  - API URL: `https://yourdomain.smarterasp.net/api`
  - Production mode: `true`

## Build Scripts

- `npm run build:prod` - Builds for production and outputs to `../LocationFinder.API/wwwroot`
- `npm run build:dev` - Builds for development with source maps
- `npm run watch` - Watches for changes and rebuilds automatically

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── search-form/
│   │   ├── location-list/
│   │   ├── location-card/
│   │   └── loading-spinner/
│   ├── services/
│   ├── models/
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
├── assets/
└── styles/
```

## Note

This project requires Node.js v18.13+ for Angular 17 compatibility. If you're using an older version, please update Node.js before proceeding.

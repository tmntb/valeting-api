# Valeting API

## Overview

Valeting API is a .NET-based application designed to manage vehicle valeting services. This project includes database configuration, environment variable setup, and migration handling.

## Installation

### Prerequisites

- .NET SDK 10.0
- Docker & Docker Compose

### Setup

Clone the repository and navigate to the project directory:

```sh
git clone https://github.com/tmb37/valeting-api.git
cd Api
```

Install dependencies:

```sh
dotnet restore
```

## Environment Configuration

### Steps to Set the Environment Variable

Since your connection string is defined as "ConnectionStrings": { "ValetingConnection": ... }, you must set the environment variable correctly so that .NET can read it.

#### 1. On macOS/Linux (Terminal)

```sh
export ConnectionStrings__ValetingConnection="Server=localhost\sa,1433;Database=Valeting;User ID=sa;Password=MySecretPassword;Integrated Security=False;Trusted_Connection=False;TrustServerCertificate=True;"
```

#### 2. On Windows (PowerShell)

```powershell
$env:ConnectionStrings__ValetingConnection="Server=localhost\sa,1433;Database=Valeting;User ID=sa;Password=MySecretPassword;Integrated Security=False;Trusted_Connection=False;TrustServerCertificate=True;"
```

#### 3. On Windows (CMD)

```cmd
set ConnectionStrings__ValetingConnection=Server=localhost\sa,1433;Database=Valeting;User ID=sa;
```

### Verify if the Variable is Set Correctly

You can check it in the terminal before running the application:

#### macOS/Linux

```sh
echo $ConnectionStrings__ValetingConnection
```

#### Windows (PowerShell)

```powershell
echo $env:ConnectionStrings__ValetingConnection
```

Windows (CMD)

```cmd
echo %ConnectionStrings__ValetingConnection%
```

### How to pass the SA_PASSWORD variable to docker compose

Before running ==docker-compose up==, set the ==SA_PASSWORD== variable in the terminal:

#### macOS/Linux

```sh
export SA_PASSWORD=MySecretPassword
docker-compose up -d
```

#### Windows

```powershell
$env:SA_PASSWORD="MySecretPassword"
docker-compose up -d
```

```cmd
set SA_PASSWORD=MySecretPassword
docker-compose up -d
```

### How to pass the JWT_KEY to docker compose

#### macOS/Linux

```sh
openssl rand -base64 32
```

#### Windows

```powershell
$bytes = New-Object byte[] 32
[Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
[Convert]::ToBase64String($bytes)
```

Alternatively, you can create a .env file with:

```env
SA_PASSWORD=MySecretPassword
JWT_KEY=MyJwtKey
PK_RPID=example.com
PK_RPNAME=Valeting
PK_ORIGIN=https://app.example.com
```

Docker Compose will automatically load the values.

### Passkey relying-party configuration

Passkey registration and authentication require the relying-party ID and browser origin to match the web client. For local development, `appsettings.Development.json` uses `localhost` and `https://localhost:44376`. For Docker or production, set `PK_RP_ID`, `PK_RP_NAME`, and `PK_ORIGIN` in `.env` to the host domain, display name, and exact HTTPS origin of the client. Do not use a wildcard origin. If the client is served from multiple origins, configure each origin in the API's `Passkeys:Origins` array.

The RP ID must be a registrable domain or a valid local development host, and each origin must use HTTPS except for localhost. Challenges expire after five minutes and can only be used once. The current challenge store is process-local; configure shared atomic storage before running multiple API instances.

### Apply Migrations in Order

```sh
dotnet ef database update 20221011202720_InitialCreate --project Repository --startup-project Api

dotnet ef database update 20250305163544_ReferenceData --project Repository --startup-project Api
```

### Running the Application

To start the application, run:

```sh
dotnet run --project Api
```

### API rate limiting

The API uses Redis-backed fixed-window limits shared by all instances. Docker Compose starts an internal Redis service without publishing its port, and the containerized API connects to it. When running the API directly on the host, start a Redis server at `localhost:6379` or override `RateLimiting:RedisConnection`. If Redis is unavailable, the API logs the outage and allows requests until Redis recovers.

All 28 API endpoints have a general limit of 120 requests per minute, partitioned by authenticated user ID or client IP. Sensitive operations also have stricter limits, initially configured as follows: login and token generation 10 requests/minute per IP plus 5/15 minutes per account; password recovery 10/minute per IP plus 5/15 minutes per account; registration 5/hour per IP; token refresh 30/minute per IP; authenticated security operations 10/5 minutes per user; security verification 5/5 minutes per user or IP; public WebAuthn options 15/minute per IP. Rejected requests receive HTTP 429 and a `Retry-After` header. Values are configurable under `RateLimiting` in `Api/appsettings.json` and may be overridden with environment variables.

The stricter policies apply to:

- `POST /api/v1/auth/login`, `/auth/forgot-password`, `/auth/register`, `/auth/recovery-codes`
- `POST /api/v1/token/generate`, `/token/refresh`
- `POST /api/v1/security/mfa/setup`, `/mfa/verify`
- `POST /api/v1/security/webauthn/authenticate/options`, `/authenticate/verify`, `/register/options`, `/register/verify`

The general policy also covers:

- `GET /api/v1/security/webauthns`; `DELETE /api/v1/security/webauthn/{id}`
- `PATCH /api/v1/user/admin-settings`, `/email`, `/password`, `/profile`
- `POST /api/v1/booking`; `PATCH /api/v1/booking/{id}`, `/booking/{id}/status`
- `GET /api/v1/booking/{id}`, `/booking/customer`, `/booking`
- `GET /api/v1/flexibility/{id}`, `/flexibility`
- `GET /api/v1/vehiclesize/{id}`, `/vehiclesize`

The routes are versioned; `v1` above is the current version. When the API is behind a reverse proxy, set `RateLimiting:TrustedProxies` to the proxy IP addresses (for example, `RateLimiting__TrustedProxies__0=10.0.0.10`). Forwarded client IP headers are only accepted from configured trusted proxies. Keep Redis on a private network and configure authentication/TLS in deployments where the network is not fully trusted.

### Redis cache

`ICacheHandler` is implemented by `RedisCacheHandler`; cache entries are shared by all API instances through the same Redis connection used for rate limiting (`RateLimiting:RedisConnection`). `Cache:KeyPrefix` (default `valeting-api:cache`) isolates cache data from rate-limit counters and can be overridden with `Cache__KeyPrefix`. Docker Compose provides an internal Redis service without publishing the Redis port. For local `dotnet run`, start Redis at `localhost:6379` or override `RateLimiting:RedisConnection`.

On a cache miss the handler executes the supplied data-source callback, serializes the result as JSON and stores it in Redis. Cache keys are deterministic hashes of the request and its request/response types. Absolute expiration defaults to one minute when omitted; a sliding expiration refreshes the entry on reads but never extends it beyond its absolute expiration. Current service settings are:

- Flexibility by ID: one day; filtered flexibility results: five days.
- Vehicle size by ID: one day; filtered vehicle-size results: five minutes.

The handler supports distributed invalidation by entity ID, list type, and application cache namespace. Invalidation methods are asynchronous (`InvalidateCacheByIdAsync`, `InvalidateCacheByListTypeAsync`, and `InvalidateAllCacheAsync`). If Redis is unavailable, cache reads and writes are bypassed: reads execute their source callback and still return its result; invalidation is best-effort. Errors from the data-source callback are not suppressed. Do not use global Redis flush operations; invalidating all application cache entries only removes keys under the configured cache prefix.

### Using Docker:

#### Start containers
```sh
./docker-compose.sh up
./docker-compose.sh up -d --build --force-recreate api
```

#### Start in detached mode
```sh
./docker-compose.sh up -d
```

#### Stop containers
```sh
./docker-compose.sh down
```

#### View logs
```sh
./docker-compose.sh logs -f
```

#### Rebuild
```sh
./docker-compose.sh up --build
```

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository

2. Create a new branch (git checkout -b feature-branch)

3. Commit changes (git commit -m "Add new feature")

4. Push to the branch (git push origin feature-branch)

5. Open a pull request

## License

This project is licensed under the MIT License.
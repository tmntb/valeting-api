# Valeting API

## Overview

Valeting API is a .NET-based application designed to manage vehicle valeting services. This project includes database configuration, environment variable setup, and migration handling.

## Installation

### Prerequisites

- .NET SDK 9.0
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
```

Docker Compose will automatically load the values.

### Apply Migrations in Order

```sh
dotnet ef database update 20221011202720_InitialCreate \
  --project Repository \
  --startup-project Api \
  --connection "Server=localhost,1433;Database=Valeting;User ID=sa;Password=MySecretPassword;Integrated Security=False;Trusted_Connection=False;TrustServerCertificate=True;"

dotnet ef database update 20250305163544_ReferenceData \
  --project Repository \
  --startup-project Api \
  --connection "Server=localhost,1433;Database=Valeting;User ID=sa;Password=MySecretPassword;Integrated Security=False;Trusted_Connection=False;TrustServerCertificate=True;"
```

### Running the Application

To start the application, run:

```sh
dotnet run --project Api
```

### Using Docker:

#### Start containers
```sh
./docker-compose.sh up
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
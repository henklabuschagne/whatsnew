# What's New API - Backend Setup

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Developer Edition)
- Visual Studio 2022 or VS Code with C# extension

## Setup Instructions

### 1. Install .NET 8.0 SDK

Download and install from: https://dotnet.microsoft.com/download/dotnet/8.0

### 2. Setup SQL Server Database

#### Option A: Using SQL Server Management Studio (SSMS)

1. Open SSMS and connect to your SQL Server instance
2. Run the scripts in order:
   - `Database/01_CreateTables.sql`
   - `Database/02_SeedData.sql`
   - `Database/03_StoredProcedures_Auth.sql`

#### Option B: Using sqlcmd (Command Line)

```bash
sqlcmd -S localhost -E -i Database/01_CreateTables.sql
sqlcmd -S localhost -E -i Database/02_SeedData.sql
sqlcmd -S localhost -E -i Database/03_StoredProcedures_Auth.sql
```

### 3. Update Connection String

Edit `WhatsNewAPI/appsettings.json` and update the connection string if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WhatsNewDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server Express, use:
```
Server=localhost\\SQLEXPRESS;Database=WhatsNewDB;Trusted_Connection=True;TrustServerCertificate=True;
```

### 4. Restore NuGet Packages

```bash
cd WhatsNewAPI
dotnet restore
```

### 5. Build the Project

```bash
dotnet build
```

### 6. Run the API

```bash
dotnet run
```

The API will start at: `http://localhost:5000`

Swagger UI will be available at: `http://localhost:5000/swagger`

## Testing the API

### Using Swagger

1. Navigate to `http://localhost:5000/swagger`
2. Test the endpoints directly from the browser

### Using curl or Postman

#### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"viewer@example.com","password":"any"}'
```

Response:
```json
{
  "token": "eyJhbGci...",
  "user": {
    "userId": "...",
    "name": "John Viewer",
    "email": "viewer@example.com",
    "role": "viewer"
  }
}
```

#### Get Current User (with token)
```bash
curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Demo Users

After running the seed script, these users will be available:

- **Viewer**: viewer@example.com (password: any - for demo)
- **Admin**: admin@example.com (password: any - for demo)

⚠️ **Note**: For demo purposes, any password will work. In production, implement proper password hashing!

## Project Structure

```
WhatsNewAPI/
├── Controllers/        # API Controllers
├── DTOs/              # Data Transfer Objects
├── Models/            # Domain Models
├── Repositories/      # Data Access Layer
├── Services/          # Business Logic
├── Program.cs         # Application Entry Point
├── appsettings.json   # Configuration
└── WhatsNewAPI.csproj # Project File
```

## Troubleshooting

### Cannot connect to SQL Server

1. Verify SQL Server is running:
   ```bash
   sc query MSSQLSERVER
   ```

2. Check your connection string in `appsettings.json`

3. Enable TCP/IP in SQL Server Configuration Manager

### Port already in use

Change the port in `Properties/launchSettings.json` or use:
```bash
dotnet run --urls "http://localhost:5001"
```

### CORS errors

Ensure the frontend URL is added to the CORS policy in `Program.cs`:
```csharp
policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
```

## Next Steps

Continue to **Phase 2: Tag Management** to implement tag CRUD operations.

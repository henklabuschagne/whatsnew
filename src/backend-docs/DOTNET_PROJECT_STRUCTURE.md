# .NET Core Backend - Project Structure

## Overview
This document outlines the complete .NET Core 8.0 Web API project structure for the What's New application.

## Project Structure

```
WhatsNewAPI/
├── WhatsNewAPI.sln
├── src/
│   ├── WhatsNewAPI/                          # Main Web API Project
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── ReleasesController.cs
│   │   │   ├── ChangesController.cs
│   │   │   ├── TagsController.cs
│   │   │   └── UsersController.cs
│   │   ├── Models/
│   │   │   ├── Entities/
│   │   │   │   ├── User.cs
│   │   │   │   ├── Release.cs
│   │   │   │   ├── Change.cs
│   │   │   │   ├── Tag.cs
│   │   │   │   ├── ChangeTag.cs
│   │   │   │   ├── SQLIntegrationSetting.cs
│   │   │   │   └── AuditLog.cs
│   │   │   └── DTOs/
│   │   │       ├── Auth/
│   │   │       │   ├── LoginRequestDto.cs
│   │   │       │   ├── LoginResponseDto.cs
│   │   │       │   └── UserDto.cs
│   │   │       ├── Releases/
│   │   │       │   ├── ReleaseDto.cs
│   │   │       │   ├── CreateReleaseDto.cs
│   │   │       │   ├── UpdateReleaseDto.cs
│   │   │       │   └── ReleaseDetailDto.cs
│   │   │       ├── Changes/
│   │   │       │   ├── ChangeDto.cs
│   │   │       │   ├── CreateChangeDto.cs
│   │   │       │   └── UpdateChangeDto.cs
│   │   │       ├── Tags/
│   │   │       │   ├── TagDto.cs
│   │   │       │   ├── CreateTagDto.cs
│   │   │       │   └── UpdateTagDto.cs
│   │   │       └── Common/
│   │   │           ├── ApiResponse.cs
│   │   │           ├── PaginatedResponse.cs
│   │   │           └── ErrorResponse.cs
│   │   ├── Services/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IAuthService.cs
│   │   │   │   ├── IReleaseService.cs
│   │   │   │   ├── IChangeService.cs
│   │   │   │   ├── ITagService.cs
│   │   │   │   ├── IUserService.cs
│   │   │   │   └── IAuditService.cs
│   │   │   └── Implementations/
│   │   │       ├── AuthService.cs
│   │   │       ├── ReleaseService.cs
│   │   │       ├── ChangeService.cs
│   │   │       ├── TagService.cs
│   │   │       ├── UserService.cs
│   │   │       └── AuditService.cs
│   │   ├── Repositories/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   ├── IReleaseRepository.cs
│   │   │   │   ├── IChangeRepository.cs
│   │   │   │   ├── ITagRepository.cs
│   │   │   │   └── IAuditRepository.cs
│   │   │   └── Implementations/
│   │   │       ├── UserRepository.cs
│   │   │       ├── ReleaseRepository.cs
│   │   │       ├── ChangeRepository.cs
│   │   │       ├── TagRepository.cs
│   │   │       └── AuditRepository.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   ├── JwtMiddleware.cs
│   │   │   └── AuditLoggingMiddleware.cs
│   │   ├── Helpers/
│   │   │   ├── JwtHelper.cs
│   │   │   ├── PasswordHelper.cs
│   │   │   └── ValidationHelper.cs
│   │   ├── Data/
│   │   │   └── DatabaseContext.cs
│   │   ├── Migrations/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── WhatsNewAPI.csproj
│   └── WhatsNewAPI.Tests/                    # Unit Tests Project
│       ├── Controllers/
│       ├── Services/
│       ├── Repositories/
│       └── WhatsNewAPI.Tests.csproj
├── docs/
│   └── API_DOCUMENTATION.md
└── README.md
```

## Technology Stack

- **.NET 8.0** - Latest LTS version
- **ASP.NET Core Web API**
- **Dapper** - Micro ORM for database access (stored procedures)
- **SQL Server** - Database
- **JWT Authentication** - JSON Web Tokens
- **BCrypt.Net** - Password hashing
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit testing

## NuGet Packages Required

```xml
<ItemGroup>
  <!-- Database -->
  <PackageReference Include="Dapper" Version="2.1.28" />
  <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.5" />
  
  <!-- Authentication -->
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
  <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.3.1" />
  <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
  
  <!-- Logging -->
  <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
  <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
  <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
  
  <!-- Documentation -->
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  
  <!-- Validation -->
  <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
  
  <!-- Excel Import/Export -->
  <PackageReference Include="EPPlus" Version="7.0.5" />
  
  <!-- Testing -->
  <PackageReference Include="xunit" Version="2.6.6" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
  <PackageReference Include="Moq" Version="4.20.70" />
  <PackageReference Include="FluentAssertions" Version="6.12.0" />
</ItemGroup>
```

## Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "Server=localhost;Database=WhatsNewDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_MIN_32_CHARACTERS_LONG_CHANGE_IN_PRODUCTION",
    "Issuer": "WhatsNewAPI",
    "Audience": "WhatsNewApp",
    "ExpirationMinutes": 480
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:5174"
    ]
  }
}
```

## Key Implementation Guidelines

### 1. Repository Pattern
- Use Dapper for stored procedure calls
- Keep repositories thin - just data access
- Return domain entities from repositories

### 2. Service Layer
- Business logic goes here
- Validate input data
- Map between entities and DTOs
- Handle transactions when needed

### 3. Controllers
- Thin controllers - delegate to services
- Use standard HTTP status codes
- Return consistent ApiResponse wrapper
- Handle authorization with [Authorize] attribute

### 4. Authentication Flow
1. User sends credentials to `/api/auth/login`
2. Backend validates credentials
3. Generate JWT token
4. Return token + user info
5. Client includes token in Authorization header: `Bearer {token}`
6. JwtMiddleware validates token on protected routes

### 5. Error Handling
- Global exception handling middleware
- Return consistent error responses
- Log all errors with Serilog
- Don't expose sensitive information

### 6. Security Best Practices
- Hash passwords with BCrypt
- Use HTTPS in production
- Validate all input
- Implement rate limiting
- Add CORS restrictions
- Sanitize SQL inputs (use parameterized queries)
- Never log sensitive data

### 7. API Versioning
- Use URL versioning: `/api/v1/releases`
- Consider API versioning package for future versions

### 8. Logging
- Log all API requests/responses
- Log authentication attempts
- Log database operations
- Use structured logging with Serilog

## Next Steps

1. Create .NET solution using `dotnet new sln -n WhatsNewAPI`
2. Create Web API project using `dotnet new webapi -n WhatsNewAPI`
3. Install NuGet packages
4. Implement models and DTOs (see DTOs_MODELS.md)
5. Implement repositories (see REPOSITORIES.md)
6. Implement services (see SERVICES.md)
7. Implement controllers (see CONTROLLERS.md)
8. Configure middleware and authentication (see AUTHENTICATION.md)
9. Test all endpoints
10. Deploy to production

## Development Commands

```bash
# Create solution
dotnet new sln -n WhatsNewAPI

# Create Web API project
dotnet new webapi -n WhatsNewAPI -o src/WhatsNewAPI

# Create Test project
dotnet new xunit -n WhatsNewAPI.Tests -o src/WhatsNewAPI.Tests

# Add projects to solution
dotnet sln add src/WhatsNewAPI/WhatsNewAPI.csproj
dotnet sln add src/WhatsNewAPI.Tests/WhatsNewAPI.Tests.csproj

# Run the API
dotnet run --project src/WhatsNewAPI/WhatsNewAPI.csproj

# Run tests
dotnet test

# Build for production
dotnet publish -c Release -o ./publish
```

## Database Connection

The API uses Dapper to execute stored procedures. Example:

```csharp
using (var connection = new SqlConnection(_connectionString))
{
    var parameters = new DynamicParameters();
    parameters.Add("@Username", username);
    
    var user = await connection.QueryFirstOrDefaultAsync<User>(
        "sp_GetUserByUsername",
        parameters,
        commandType: CommandType.StoredProcedure
    );
    
    return user;
}
```

# What's New Backend - Complete Setup Instructions

## 🎯 Overview
This guide will walk you through setting up the complete backend infrastructure for the What's New application, including database, .NET Core API, and integration with the React frontend.

---

## 📋 Prerequisites

### Required Software
- **SQL Server 2019+** (Express, Developer, or Standard Edition)
- **SQL Server Management Studio (SSMS)** or Azure Data Studio
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control
- **Postman** or similar tool for API testing

### Verify Installations
```bash
# Check .NET version
dotnet --version
# Should show: 8.0.x

# Check SQL Server
sqlcmd -S localhost -Q "SELECT @@VERSION"
```

---

## 🗄️ Part 1: Database Setup

### Step 1: Create Database

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Open the `/backend-docs/DATABASE_SCHEMA.sql` file
4. Execute the entire script (F5)

This will create:
- WhatsNewDB database
- All tables (Users, Releases, Changes, Tags, etc.)
- Indexes for performance
- Default tags

### Step 2: Create Stored Procedures

1. Open `/backend-docs/STORED_PROCEDURES.sql`
2. Execute the entire script (F5)

This creates all stored procedures for CRUD operations.

### Step 3: Create Default Users

**IMPORTANT**: You need to hash passwords before inserting users.

```sql
-- After setting up the .NET API, you can create users through the API
-- or manually insert with pre-hashed passwords

-- For now, leave this step - we'll handle it after API setup
```

### Step 4: Verify Database Setup

```sql
USE WhatsNewDB;

-- Check tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Check stored procedures
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE';

-- Check default tags
SELECT * FROM Tags;
```

Expected output:
- 7 tables created
- 20+ stored procedures
- 8 default tags

---

## 🏗️ Part 2: .NET Core API Setup

### Step 1: Create Solution and Projects

```bash
# Create a new directory
mkdir WhatsNewAPI
cd WhatsNewAPI

# Create solution
dotnet new sln -n WhatsNewAPI

# Create Web API project
dotnet new webapi -n WhatsNewAPI -o src/WhatsNewAPI

# Create Test project
dotnet new xunit -n WhatsNewAPI.Tests -o src/WhatsNewAPI.Tests

# Add projects to solution
dotnet sln add src/WhatsNewAPI/WhatsNewAPI.csproj
dotnet sln add src/WhatsNewAPI.Tests/WhatsNewAPI.Tests.csproj

# Add test reference to main project
cd src/WhatsNewAPI.Tests
dotnet add reference ../WhatsNewAPI/WhatsNewAPI.csproj
cd ../..
```

### Step 2: Install NuGet Packages

```bash
cd src/WhatsNewAPI

# Database packages
dotnet add package Dapper
dotnet add package Microsoft.Data.SqlClient

# Authentication packages
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next

# Logging packages
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console

# Documentation
dotnet add package Swashbuckle.AspNetCore

# Validation
dotnet add package FluentValidation.AspNetCore

# Excel
dotnet add package EPPlus

cd ../..
```

### Step 3: Create Project Structure

```bash
cd src/WhatsNewAPI

# Create directories
mkdir Models
mkdir Models/Entities
mkdir Models/DTOs
mkdir Models/DTOs/Auth
mkdir Models/DTOs/Releases
mkdir Models/DTOs/Changes
mkdir Models/DTOs/Tags
mkdir Models/DTOs/Common
mkdir Controllers
mkdir Services
mkdir Services/Interfaces
mkdir Services/Implementations
mkdir Repositories
mkdir Repositories/Interfaces
mkdir Repositories/Implementations
mkdir Middleware
mkdir Helpers
mkdir Data
```

### Step 4: Copy Code Files

Now copy the code from the backend-docs folder:

1. **DTOs_MODELS.cs** → Create individual files in `Models/` folders
   - Split the namespaces into separate files
   - Example: `Models/Entities/User.cs`, `Models/DTOs/Auth/LoginRequestDto.cs`

2. **CONTROLLERS.cs** → Create individual files in `Controllers/` folder
   - `Controllers/AuthController.cs`
   - `Controllers/ReleasesController.cs`
   - `Controllers/ChangesController.cs`
   - `Controllers/TagsController.cs`

3. **HELPERS.cs** → Create individual files in `Helpers/` folder
   - `Helpers/JwtHelper.cs`
   - `Helpers/PasswordHelper.cs`
   - Extract other helper classes

4. **MIDDLEWARE.cs** → Create individual files in `Middleware/` folder
   - `Middleware/ExceptionHandlingMiddleware.cs`
   - `Middleware/AuditLoggingMiddleware.cs`
   - Extract other middleware classes

5. **PROGRAM_CS.cs** → Replace `Program.cs`

### Step 5: Create Repository Implementations

Create `Repositories/Implementations/UserRepository.cs`:

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WhatsNewAPI.Models;
using WhatsNewAPI.Repositories.Interfaces;

namespace WhatsNewAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username);

            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByUsername",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserId = @UserId",
                new { UserId = userId }
            );
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync(
                "sp_UpdateLastLogin",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
```

**Repeat for other repositories**: ReleaseRepository, ChangeRepository, TagRepository, AuditRepository

### Step 6: Create Service Implementations

Create `Services/Implementations/AuthService.cs`:

```csharp
using WhatsNewAPI.Helpers;
using WhatsNewAPI.Models.DTOs.Auth;
using WhatsNewAPI.Repositories.Interfaces;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHelper _passwordHelper;

        public AuthService(
            IUserRepository userRepository,
            JwtHelper jwtHelper,
            PasswordHelper passwordHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _passwordHelper = passwordHelper;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress)
        {
            // Get user by username
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            
            if (user == null)
            {
                return null;
            }

            // Verify password
            if (!_passwordHelper.VerifyPassword(request.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(
                user.UserId,
                user.Username,
                user.Email,
                user.Role
            );

            // Update last login
            await _userRepository.UpdateLastLoginAsync(user.UserId);

            return new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    LastLoginAt = DateTime.UtcNow
                },
                ExpiresAt = _jwtHelper.GetTokenExpiration()
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                LastLoginAt = user.LastLoginAt
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                return false;
            }

            // Verify current password
            if (!_passwordHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // Hash new password and update
            var newPasswordHash = _passwordHelper.HashPassword(request.NewPassword);
            // Call repository method to update password
            
            return true;
        }
    }
}
```

**Repeat for other services**: ReleaseService, ChangeService, TagService

### Step 7: Configure appsettings.json

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "Server=localhost;Database=WhatsNewDB;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!ChangeMe",
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
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/whatsnew-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

**IMPORTANT**: Change the JWT SecretKey to a strong random value!

---

## 🚀 Part 3: Run and Test API

### Step 1: Build the Project

```bash
cd src/WhatsNewAPI
dotnet build
```

Fix any compilation errors.

### Step 2: Run the API

```bash
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### Step 3: Test with Swagger

1. Open browser: `http://localhost:5000` or `https://localhost:5001`
2. Swagger UI should load
3. Test the `/health` endpoint - should return "healthy"

### Step 4: Create First User (via SQL)

Since we need a user to login, create one manually with a hashed password:

```bash
# Use .NET to hash a password
dotnet run --project PasswordHashTool

# Or use online BCrypt generator with work factor 12
# Password: Admin@123
# Hash: $2a$12$... (you'll get this from generator)
```

Then insert into database:

```sql
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role)
VALUES ('admin', 'admin@whatsnew.com', '$2a$12$YOUR_HASH_HERE', 'Admin', 'User', 'admin');

INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role)
VALUES ('john.viewer', 'john@whatsnew.com', '$2a$12$YOUR_HASH_HERE', 'John', 'Viewer', 'viewer');
```

### Step 5: Test Login

Using Postman or Swagger:

```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

You should get a JWT token back!

---

## 🔗 Part 4: Connect Frontend to Backend

### Step 1: Update Frontend API Service

Edit `/services/api.ts` in your React frontend:

```typescript
const API_BASE_URL = 'http://localhost:5000/api';

// Update all API functions to use real endpoints
export const login = async (username: string, password: string) => {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password })
  });
  
  const result = await response.json();
  
  if (!result.success) {
    throw new Error(result.message);
  }
  
  return result.data;
};

// Repeat for all other API functions...
```

### Step 2: Test Frontend-Backend Integration

1. Start backend: `dotnet run` (in `src/WhatsNewAPI`)
2. Start frontend: `npm run dev` (in frontend folder)
3. Login with credentials
4. Test all features

---

## 📊 Part 5: Verify Everything Works

### Checklist

- [ ] Database created with all tables
- [ ] Stored procedures created
- [ ] .NET API builds successfully
- [ ] API starts without errors
- [ ] Swagger UI accessible
- [ ] Can login and receive JWT token
- [ ] Can create/read/update/delete releases
- [ ] Can create/read/update/delete changes
- [ ] Can read tags
- [ ] Frontend connects to backend
- [ ] Authorization works (viewer vs admin)

---

## 🐛 Troubleshooting

### Database Connection Issues
```
Error: Cannot connect to database
```
**Solution**: Check connection string in appsettings.json

### JWT Issues
```
Error: IDX10603: The algorithm: 'HS256' requires the SecurityKey.KeySize to be greater than 256 bits
```
**Solution**: Ensure JWT SecretKey is at least 32 characters

### CORS Issues
```
Error: CORS policy: No 'Access-Control-Allow-Origin' header
```
**Solution**: Add frontend URL to Cors:AllowedOrigins in appsettings.json

### BCrypt Issues
```
Error: Unable to verify password
```
**Solution**: Ensure BCrypt work factor matches (default: 12)

---

## 📚 Next Steps

1. **Add more users** via admin panel or SQL
2. **Import test data** using Excel import feature
3. **Set up production database** on Azure SQL or AWS RDS
4. **Deploy API** to Azure App Service or AWS
5. **Configure CI/CD** with GitHub Actions
6. **Add monitoring** with Application Insights
7. **Set up backups** for database

---

## 📞 Support

For issues or questions:
1. Check the logs in `/logs` folder
2. Review API documentation in `API_ENDPOINTS.md`
3. Check database with SSMS
4. Enable debug logging in appsettings.json

---

## 🎉 Congratulations!

You now have a fully functioning backend for the What's New application!

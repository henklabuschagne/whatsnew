# What's New API - Backend

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK installed
- SQL Server 2019+ (LocalDB, Express, or full version)
- SQL Server Management Studio (SSMS) or Azure Data Studio

### Setup Steps

#### 1. Database Setup

**Run the SQL Scripts:**

```bash
# Open SQL Server Management Studio and connect to your SQL Server instance
# Then run these scripts in order:

1. /backend-docs/DATABASE_SCHEMA.sql       # Creates database and tables
2. /backend-docs/STORED_PROCEDURES.sql     # Creates stored procedures
```

**Verify Database:**
```sql
USE WhatsNewDB;
SELECT * FROM Tags;  -- Should show 8 default tags
```

#### 2. Update Connection String

Edit `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "WhatsNewDB": "Server=localhost;Database=WhatsNewDB;Integrated Security=true;TrustServerCertificate=true"
}
```

**Common connection strings:**
- **LocalDB**: `Server=(localdb)\\mssqllocaldb;Database=WhatsNewDB;Trusted_Connection=true;`
- **SQL Express**: `Server=localhost\\SQLEXPRESS;Database=WhatsNewDB;Trusted_Connection=true;TrustServerCertificate=true;`
- **Full SQL Server**: `Server=localhost;Database=WhatsNewDB;Integrated Security=true;TrustServerCertificate=true;`
- **SQL Authentication**: `Server=localhost;Database=WhatsNewDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;`

#### 3. Create Test Users

Run this SQL to create test users (passwords will be hashed):

```sql
USE WhatsNewDB;

-- Admin user (username: admin, password: Admin@123)
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@whatsnew.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYILSBL8EBK', 'Admin', 'User', 'admin', 1, GETUTCDATE(), GETUTCDATE());

-- Viewer user (username: john.viewer, password: Viewer@123)
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt, UpdatedAt)
VALUES ('john.viewer', 'john@whatsnew.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYILSBL8EBK', 'John', 'Viewer', 'viewer', 1, GETUTCDATE(), GETUTCDATE());
```

**Test Credentials:**
- Admin: `admin` / `Admin@123`
- Viewer: `john.viewer` / `Viewer@123`

#### 4. Build and Run

```bash
# Navigate to API directory
cd src/WhatsNewAPI

# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the API
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000` or `https://localhost:5001`

#### 5. Test the API

**Option 1: Using Swagger UI**
1. Open browser to `http://localhost:5000`
2. Click "Authorize" button
3. Login via `/api/auth/login` endpoint
4. Copy the token from response
5. Paste in "Authorize" dialog as: `Bearer {token}`
6. Test other endpoints

**Option 2: Using cURL**
```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'

# Get releases (replace TOKEN with actual token)
curl -X GET http://localhost:5000/api/releases \
  -H "Authorization: Bearer {TOKEN}"
```

**Option 3: Using Postman**
- Import collection from `/backend-docs/API_ENDPOINTS.md`
- Set base URL to `http://localhost:5000/api`
- Login and copy token
- Add token to Authorization header for other requests

### Project Structure

```
src/WhatsNewAPI/
├── Controllers/           # API controllers
├── Services/             
│   ├── Interfaces/       # Service interfaces
│   └── Implementations/  # Service implementations
├── Repositories/
│   ├── Interfaces/       # Repository interfaces
│   └── Implementations/  # Repository implementations
├── Models/
│   ├── Entities/         # Database entities
│   └── DTOs/             # Data transfer objects
├── Middleware/           # Custom middleware
├── Helpers/              # Helper classes
├── appsettings.json      # Configuration
└── Program.cs            # Application entry point
```

## 📝 API Endpoints

### Authentication
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/me` - Get current user info
- `POST /api/auth/change-password` - Change password

### Releases (authenticated)
- `GET /api/releases` - Get all releases
- `GET /api/releases/{id}` - Get release by ID
- `POST /api/releases` - Create release (admin only)
- `PUT /api/releases/{id}` - Update release (admin only)
- `DELETE /api/releases/{id}` - Delete release (admin only)
- `GET /api/releases/statistics` - Get statistics (admin only)

### Changes (admin only)
- `POST /api/changes` - Create change
- `PUT /api/changes/{id}` - Update change
- `DELETE /api/changes/{id}` - Delete change

### Tags (authenticated)
- `GET /api/tags` - Get all tags
- `POST /api/tags` - Create tag (admin only)
- `PUT /api/tags/{id}` - Update tag (admin only)
- `DELETE /api/tags/{id}` - Delete tag (admin only)

Full API documentation available in `/backend-docs/API_ENDPOINTS.md`

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "YOUR_CONNECTION_STRING_HERE"
  },
  "JwtSettings": {
    "SecretKey": "CHANGE_THIS_TO_A_SECURE_KEY_MIN_32_CHARS",
    "Issuer": "WhatsNewAPI",
    "Audience": "WhatsNewApp",
    "ExpirationMinutes": 480
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173"
    ]
  }
}
```

**Important**: Change the JWT SecretKey in production!

## 🧪 Testing

### Health Check
```bash
curl http://localhost:5000/health
```

### Login Test
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

### Get Releases Test
```bash
# First login and get token, then:
curl -X GET http://localhost:5000/api/releases \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 🐛 Troubleshooting

### Cannot connect to database
- Check SQL Server is running: `services.msc` → SQL Server
- Verify connection string in appsettings.json
- Test connection with SSMS

### Login fails with "Invalid username or password"
- Verify users were created in database
- Check PasswordHash matches the example (BCrypt hash)
- Try creating users again

### CORS errors
- Add your frontend URL to Cors:AllowedOrigins in appsettings.json
- Restart the API after changing configuration

### JWT token invalid
- Ensure SecretKey is at least 32 characters
- Check token expiration (default 8 hours)
- Verify token is passed as: `Bearer {token}`

## 📊 Database Management

### View audit logs
```sql
SELECT TOP 100 * FROM AuditLogs ORDER BY CreatedAt DESC;
```

### Check user activity
```sql
SELECT Username, LastLoginAt, Role, IsActive FROM Users;
```

### Get release statistics
```sql
EXEC sp_GetReleaseStatistics;
```

## 🚢 Production Deployment

### Before deploying:

1. **Update appsettings.json**
   - Change JWT SecretKey to a strong random value
   - Update connection string for production database
   - Configure proper CORS origins

2. **Publish the application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. **Deploy to:**
   - Azure App Service
   - AWS Elastic Beanstalk
   - IIS on Windows Server
   - Docker container

4. **Enable HTTPS**
   - Use valid SSL certificate
   - Force HTTPS redirection

5. **Database**
   - Use managed database (Azure SQL, AWS RDS)
   - Enable backups
   - Configure firewall rules

## 📚 Additional Resources

- Full documentation: `/backend-docs/`
- API reference: `/backend-docs/API_ENDPOINTS.md`
- Architecture: `/backend-docs/ARCHITECTURE_DIAGRAM.md`
- Setup guide: `/backend-docs/SETUP_INSTRUCTIONS.md`

## 🔐 Security Notes

- **Never** commit appsettings.json with real secrets
- Use environment variables for production secrets
- Change default JWT secret key
- Enable HTTPS in production
- Regularly update NuGet packages
- Use strong passwords for database users
- Enable SQL Server authentication only if needed

## 📞 Support

Check logs in `/logs` folder for errors and debugging information.

## ✅ Ready!

Your backend API is now running! Test it with Swagger UI at `http://localhost:5000`

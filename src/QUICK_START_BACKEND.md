# 🚀 Quick Start - Backend Only (5 Minutes!)

Want to test the backend API first? Follow these steps:

---

## Step 1: Setup Database (3 minutes)

### Open SQL Server Management Studio (SSMS)

1. **Connect** to your SQL Server instance
   - LocalDB: `(localdb)\mssqllocaldb`
   - Express: `localhost\SQLEXPRESS`
   - Full: `localhost`

2. **Run Script 1** - Create Database & Tables
   - Open: `/backend-docs/DATABASE_SCHEMA.sql`
   - Click Execute (F5)
   - ✅ Should create WhatsNewDB database with 6 tables and 8 default tags

3. **Run Script 2** - Create Stored Procedures
   - Open: `/backend-docs/STORED_PROCEDURES.sql`
   - Click Execute (F5)
   - ✅ Should create 20+ stored procedures

4. **Create Test Users** - Run this query:
```sql
USE WhatsNewDB;

-- Admin user (username: admin, password: Admin@123)
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt, UpdatedAt)
VALUES ('admin', 'admin@whatsnew.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYILSBL8EBK', 'Admin', 'User', 'admin', 1, GETUTCDATE(), GETUTCDATE());

-- Viewer user (username: john.viewer, password: Viewer@123)
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt, UpdatedAt)
VALUES ('john.viewer', 'john@whatsnew.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYILSBL8EBK', 'John', 'Viewer', 'viewer', 1, GETUTCDATE(), GETUTCDATE());

-- Verify setup
SELECT * FROM Tags;  -- Should show 8 tags
SELECT * FROM Users; -- Should show 2 users
```

✅ **Database ready!**

---

## Step 2: Configure Connection String (30 seconds)

Edit `/src/WhatsNewAPI/appsettings.json`:

**For LocalDB:**
```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "Server=(localdb)\\mssqllocaldb;Database=WhatsNewDB;Trusted_Connection=true;"
  }
}
```

**For SQL Express:**
```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "Server=localhost\\SQLEXPRESS;Database=WhatsNewDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**For Full SQL Server:**
```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "Server=localhost;Database=WhatsNewDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**For SQL Authentication:**
```json
{
  "ConnectionStrings": {
    "WhatsNewDB": "Server=localhost;Database=WhatsNewDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

---

## Step 3: Run the API (1 minute)

Open terminal in project root:

```bash
cd src/WhatsNewAPI
dotnet restore
dotnet build
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

✅ **API is running!**

---

## Step 4: Test with Swagger (1 minute)

1. **Open browser** to: `http://localhost:5000/swagger`

2. **Test login**:
   - Click on `POST /api/auth/login`
   - Click "Try it out"
   - Enter:
     ```json
     {
       "username": "admin",
       "password": "Admin@123"
     }
     ```
   - Click "Execute"

3. **You should see**:
   ```json
   {
     "success": true,
     "data": {
       "token": "eyJhbGciOiJIUzI1NiIs...",
       "user": {
         "userId": 1,
         "username": "admin",
         "email": "admin@whatsnew.com",
         "firstName": "Admin",
         "lastName": "User",
         "role": "admin"
       },
       "expiresAt": "2024-12-05T08:00:00Z"
     },
     "message": "Login successful"
   }
   ```

4. **Copy the token** (the long string after "token":)

5. **Authorize Swagger**:
   - Click the "Authorize" button at top
   - Paste: `Bearer YOUR_TOKEN_HERE`
   - Click "Authorize"

6. **Test other endpoints**:
   - Try `GET /api/tags` - Should return 8 default tags
   - Try `GET /api/releases/statistics` - Should return stats
   - Try `POST /api/releases` - Create a test release

✅ **Backend is working perfectly!**

---

## Step 5: Test with cURL (Optional)

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

### Get Tags (replace {TOKEN} with your token)
```bash
curl -X GET http://localhost:5000/api/tags \
  -H "Authorization: Bearer {TOKEN}"
```

### Get Releases
```bash
curl -X GET http://localhost:5000/api/releases \
  -H "Authorization: Bearer {TOKEN}"
```

### Create a Test Release
```bash
curl -X POST http://localhost:5000/api/releases \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "1.0.0",
    "releaseDate": "2024-12-04",
    "description": "Initial release",
    "isPublished": true
  }'
```

---

## Troubleshooting

### "Cannot connect to database"
**Error**: `A network-related or instance-specific error...`

**Solutions**:
1. Check SQL Server is running:
   - Press `Win + R`, type `services.msc`
   - Find "SQL Server (MSSQLSERVER)" or "SQL Server (SQLEXPRESS)"
   - Ensure it's "Running"

2. Verify connection string matches your SQL Server type

3. Test connection with SSMS first

### "Login fails with 401"
**Error**: `Invalid username or password`

**Solutions**:
1. Verify users exist:
   ```sql
   SELECT * FROM Users;
   ```

2. Re-run the INSERT statements for test users

3. Check username and password are correct:
   - Admin: `admin` / `Admin@123`
   - Viewer: `john.viewer` / `Viewer@123`

### "Port 5000 already in use"
**Error**: `Unable to bind to http://localhost:5000...`

**Solutions**:
1. Kill the process using port 5000
2. Or change the port in `/src/WhatsNewAPI/Properties/launchSettings.json`

### "NuGet packages not restored"
**Error**: Package restore errors

**Solution**:
```bash
cd src/WhatsNewAPI
dotnet restore --force
dotnet build
```

---

## Verify Backend is Working

Run these checks:

### ✅ Database
```sql
USE WhatsNewDB;
SELECT COUNT(*) FROM Tags;    -- Should return 8
SELECT COUNT(*) FROM Users;   -- Should return 2
```

### ✅ API Endpoints
- [ ] POST /api/auth/login - Returns token
- [ ] GET /api/tags - Returns 8 tags
- [ ] GET /api/releases - Returns empty array (no releases yet)
- [ ] POST /api/releases - Creates new release (admin only)

### ✅ Authentication
- [ ] Login with admin works
- [ ] Token is returned
- [ ] Token works in Authorization header
- [ ] Endpoints require valid token

### ✅ Authorization
- [ ] Admin can access all endpoints
- [ ] GET requests work for both roles
- [ ] POST/PUT/DELETE require admin role

---

## Next Steps

Now that backend is running:

1. **Test all endpoints** in Swagger UI
2. **Create test data**:
   - Create a few releases
   - Add changes to releases
   - Create custom tags
   - Test delete operations

3. **Start the frontend**:
   ```bash
   # In new terminal
   npm run dev
   ```

4. **Login from frontend** with `admin` / `Admin@123`

5. **Verify frontend connects to backend**:
   - Open browser DevTools > Network tab
   - Login
   - Should see POST to `http://localhost:5000/api/auth/login`

---

## API is Ready! 🎉

Your backend API is now:
- ✅ Running on `http://localhost:5000`
- ✅ Connected to SQL Server database
- ✅ Authenticated with JWT
- ✅ Documented with Swagger
- ✅ Ready for frontend integration

**Total time**: ~5 minutes
**Next**: Start the frontend and connect it!

For frontend setup, see `/INTEGRATION_COMPLETE.md`

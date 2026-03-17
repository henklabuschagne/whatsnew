# What's New Application - Architecture Diagrams

## 🏗️ System Architecture Overview

```
┌───────────────────────────────────────────────────────────────────────┐
│                          CLIENT LAYER                                  │
├───────────────────────────────────────────────────────────────────────┤
│  React Frontend (Port 5173)                                           │
│  ┌─────────────────┐  ┌──────────────┐  ┌─────────────────┐         │
│  │  What's New Page│  │ Release Mgmt │  │  Tag Management │         │
│  │  (Read-only)    │  │  (Admin)     │  │    (Admin)      │         │
│  └─────────────────┘  └──────────────┘  └─────────────────┘         │
│           │                    │                   │                   │
│           └────────────────────┼───────────────────┘                   │
│                                │                                       │
│                          JWT Token Bearer                              │
│                                │                                       │
└────────────────────────────────┼───────────────────────────────────────┘
                                 │
                          HTTPS/HTTP
                                 │
┌────────────────────────────────▼───────────────────────────────────────┐
│                      API GATEWAY LAYER                                 │
├───────────────────────────────────────────────────────────────────────┤
│  .NET Core Web API (Port 5000/5001)                                   │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │  Middleware Pipeline                                            │  │
│  │  ┌─────────────┐  ┌───────────┐  ┌──────────────┐            │  │
│  │  │  Exception  │→ │   CORS    │→ │     JWT      │            │  │
│  │  │  Handling   │  │           │  │ Validation   │            │  │
│  │  └─────────────┘  └───────────┘  └──────────────┘            │  │
│  │  ┌─────────────┐  ┌───────────┐  ┌──────────────┐            │  │
│  │  │   Audit     │→ │  Rate     │→ │   Security   │            │  │
│  │  │  Logging    │  │ Limiting  │  │   Headers    │            │  │
│  │  └─────────────┘  └───────────┘  └──────────────┘            │  │
│  └─────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘
                                 │
┌────────────────────────────────▼───────────────────────────────────────┐
│                      CONTROLLER LAYER                                  │
├───────────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌───────────────┐  ┌──────────────┐              │
│  │    Auth      │  │   Releases    │  │   Changes    │              │
│  │  Controller  │  │  Controller   │  │  Controller  │              │
│  └──────┬───────┘  └───────┬───────┘  └──────┬───────┘              │
│  ┌──────▼───────┐  ┌───────▼───────┐                                 │
│  │    Tags      │  │    Users      │                                 │
│  │  Controller  │  │  Controller   │                                 │
│  └──────────────┘  └───────────────┘                                 │
└────────────────────────────────────────────────────────────────────────┘
                                 │
┌────────────────────────────────▼───────────────────────────────────────┐
│                       SERVICE LAYER                                    │
├───────────────────────────────────────────────────────────────────────┤
│  Business Logic & Validation                                          │
│  ┌──────────────┐  ┌───────────────┐  ┌──────────────┐              │
│  │    Auth      │  │   Release     │  │   Change     │              │
│  │   Service    │  │   Service     │  │   Service    │              │
│  └──────┬───────┘  └───────┬───────┘  └──────┬───────┘              │
│  ┌──────▼───────┐  ┌───────▼───────┐  ┌──────▼───────┐              │
│  │    Tag       │  │    User       │  │    Audit     │              │
│  │   Service    │  │   Service     │  │   Service    │              │
│  └──────────────┘  └───────────────┘  └──────────────┘              │
└────────────────────────────────────────────────────────────────────────┘
                                 │
┌────────────────────────────────▼───────────────────────────────────────┐
│                     REPOSITORY LAYER                                   │
├───────────────────────────────────────────────────────────────────────┤
│  Data Access with Dapper ORM                                          │
│  ┌──────────────┐  ┌───────────────┐  ┌──────────────┐              │
│  │    User      │  │   Release     │  │   Change     │              │
│  │  Repository  │  │  Repository   │  │  Repository  │              │
│  └──────┬───────┘  └───────┬───────┘  └──────┬───────┘              │
│  ┌──────▼───────┐  ┌───────▼───────┐                                 │
│  │    Tag       │  │    Audit      │                                 │
│  │  Repository  │  │  Repository   │                                 │
│  └──────────────┘  └───────────────┘                                 │
└────────────────────────────────────────────────────────────────────────┘
                                 │
                    Stored Procedures (Dapper)
                                 │
┌────────────────────────────────▼───────────────────────────────────────┐
│                       DATABASE LAYER                                   │
├───────────────────────────────────────────────────────────────────────┤
│  SQL Server 2019+                                                     │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │  Tables:                                                        │  │
│  │  • Users          • Releases        • Changes                  │  │
│  │  • Tags           • Change_Tags     • AuditLogs                │  │
│  │  • SQLIntegrationSettings                                      │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │  Stored Procedures:                                            │  │
│  │  • sp_GetAllReleases      • sp_CreateRelease                   │  │
│  │  • sp_UpdateRelease       • sp_DeleteRelease                   │  │
│  │  • sp_CreateChange        • sp_GetUserByUsername               │  │
│  │  • sp_GetReleaseStatistics (+ 15 more)                        │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │  Indexes:                                                       │  │
│  │  • IX_Releases_Version    • IX_Changes_ReleaseId              │  │
│  │  • IX_Tags_TagValue       • IX_Users_Email                    │  │
│  └─────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Data Flow Diagram

### User Login Flow

```
┌─────────┐
│ Browser │
└────┬────┘
     │ 1. POST /api/auth/login
     │    { username, password }
     │
┌────▼─────────────┐
│ AuthController   │
└────┬─────────────┘
     │ 2. LoginAsync()
     │
┌────▼─────────────┐
│  AuthService     │──────┐ 3. GetByUsernameAsync()
└────┬─────────────┘      │
     │                ┌───▼──────────────┐
     │                │ UserRepository   │
     │                └───┬──────────────┘
     │                    │ 4. sp_GetUserByUsername
     │                ┌───▼──────────────┐
     │                │  SQL Server      │
     │                └───┬──────────────┘
     │                    │ 5. Return User
     │                ┌───▼──────────────┐
     │ 6. Verify      │ PasswordHelper   │
     │    Password    └───┬──────────────┘
     │                    │ BCrypt.Verify()
     │◄───────────────────┘
     │
     │ 7. Generate    ┌──────────────────┐
     │    JWT Token   │   JwtHelper      │
     │────────────────►└──────────────────┘
     │                    
     │ 8. Return { token, user, expiresAt }
     │
┌────▼────┐
│ Browser │ 9. Store token in localStorage
└─────────┘    Future requests: Authorization: Bearer {token}
```

---

### Create Release Flow (Admin Only)

```
┌─────────┐
│ Browser │ Authorization: Bearer {token}
└────┬────┘
     │ 1. POST /api/releases
     │    { version, releaseDate, description, isPublished }
     │
┌────▼──────────────┐
│ JwtMiddleware     │ 2. Validate token & role
└────┬──────────────┘    Extract userId from token
     │
┌────▼──────────────┐
│ReleasesController │ 3. [Authorize(Roles="admin")]
└────┬──────────────┘
     │ 4. CreateReleaseAsync()
     │
┌────▼──────────────┐
│ ReleaseService    │ 5. Validate input
└────┬──────────────┘    Map DTO to Entity
     │ 6. CreateAsync()
     │
┌────▼──────────────┐
│ReleaseRepository  │ 7. Call stored procedure
└────┬──────────────┘
     │ 8. sp_CreateRelease(@Version, @ReleaseDate, ...)
     │
┌────▼──────────────┐
│  SQL Server       │ 9. BEGIN TRANSACTION
│                   │    INSERT INTO Releases
│                   │    INSERT INTO AuditLogs
│                   │    COMMIT TRANSACTION
└────┬──────────────┘
     │ 10. Return releaseId
     │
┌────▼──────────────┐
│ReleaseRepository  │ 11. Return Release entity
└────┬──────────────┘
     │
┌────▼──────────────┐
│ ReleaseService    │ 12. Map entity to DTO
└────┬──────────────┘
     │
┌────▼──────────────┐
│ReleasesController │ 13. Return ApiResponse<ReleaseDto>
└────┬──────────────┘     201 Created
     │
┌────▼────┐
│ Browser │ 14. Update UI with new release
└─────────┘
```

---

## 🔐 Authentication & Authorization Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    REQUEST PIPELINE                          │
└─────────────────────────────────────────────────────────────┘

   HTTP Request
        │
        ▼
   ┌────────────────┐
   │ CORS Middleware│  Allow frontend origin
   └────────┬───────┘
            │
            ▼
   ┌────────────────┐
   │  JWT Middleware│  Extract & validate token
   └────────┬───────┘
            │
            ├─── Token Valid ───┐
            │                   │
            │                   ▼
            │          ┌────────────────┐
            │          │ Extract Claims │
            │          │ • UserId       │
            │          │ • Username     │
            │          │ • Role         │
            │          │ • Email        │
            │          └────────┬───────┘
            │                   │
            │                   ▼
            │          ┌────────────────┐
            │          │Set User Context│
            │          │HttpContext.User│
            │          └────────┬───────┘
            │                   │
            ├───────────────────┘
            │
            ▼
   ┌────────────────┐
   │   Controller   │  [Authorize(Roles = "admin")]
   │   Action       │  Check role from claims
   └────────┬───────┘
            │
            ├─── Role Match ───► Process Request
            │
            └─── Role Mismatch ─► 403 Forbidden
```

---

## 📊 Database Schema Diagram

```
┌────────────────────┐
│      Users         │
├────────────────────┤
│ PK UserId          │──┐
│    Username        │  │
│    Email           │  │  Created By
│    PasswordHash    │  │
│    Role            │  │
│    IsActive        │  │
└────────────────────┘  │
         │              │
         │ 1:N          │
         │              │
┌────────▼───────────┐  │
│     Releases       │◄─┘
├────────────────────┤
│ PK ReleaseId       │──┐
│    Version         │  │
│    ReleaseDate     │  │
│    Description     │  │  1:N
│    IsPublished     │  │
│ FK CreatedBy       │  │
└────────────────────┘  │
                        │
┌───────────────────────▼─┐
│       Changes           │
├─────────────────────────┤
│ PK ChangeId             │──┐
│ FK ReleaseId            │  │
│    Description          │  │
│    ChangeType           │  │  M:N
│ FK CreatedBy            │  │
└─────────────────────────┘  │
                             │
                    ┌────────▼────────┐
                    │   Change_Tags   │
                    ├─────────────────┤
                    │ PK,FK ChangeId  │
                    │ PK,FK TagId     │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │      Tags       │
                    ├─────────────────┤
                    │ PK TagId        │
                    │    TagValue     │
                    │    TagLabel     │
                    │    TagType      │
                    │    IsActive     │
                    └─────────────────┘

┌────────────────────┐
│    AuditLogs       │
├────────────────────┤
│ PK AuditId         │
│ FK UserId          │◄──── Links to Users
│    Action          │
│    EntityType      │
│    EntityId        │
│    OldValue        │
│    NewValue        │
│    IpAddress       │
│    CreatedAt       │
└────────────────────┘
```

---

## 🚀 Deployment Architecture

### Development Environment
```
┌─────────────────────────────────────────┐
│         Developer Machine                │
├─────────────────────────────────────────┤
│  React Dev Server (Port 5173)           │
│  .NET API (Port 5000)                   │
│  SQL Server LocalDB/Express             │
└─────────────────────────────────────────┘
```

### Production Environment (Azure)
```
┌─────────────────────────────────────────────────┐
│              Azure Cloud                         │
├─────────────────────────────────────────────────┤
│  ┌──────────────────────────────────────────┐   │
│  │  Azure CDN                               │   │
│  │  (Static React Build)                    │   │
│  └────────────┬─────────────────────────────┘   │
│               │                                  │
│  ┌────────────▼─────────────────────────────┐   │
│  │  Azure App Service                       │   │
│  │  (.NET Core API)                         │   │
│  │  • Auto-scaling                          │   │
│  │  • HTTPS enabled                         │   │
│  │  • App Insights monitoring               │   │
│  └────────────┬─────────────────────────────┘   │
│               │                                  │
│  ┌────────────▼─────────────────────────────┐   │
│  │  Azure SQL Database                      │   │
│  │  • Geo-replication                       │   │
│  │  • Automated backups                     │   │
│  │  • Firewall rules                        │   │
│  └──────────────────────────────────────────┘   │
│                                                  │
│  ┌──────────────────────────────────────────┐   │
│  │  Azure Key Vault                         │   │
│  │  (JWT Secret, Connection Strings)        │   │
│  └──────────────────────────────────────────┘   │
└─────────────────────────────────────────────────┘
```

---

## 🔄 Request/Response Flow

### Example: Get All Releases (Viewer)

```
1. Frontend Request
   ───────────────────────────────────────────
   GET http://localhost:5000/api/releases
   Authorization: Bearer eyJhbGciOiJIUzI1NiI...
   
2. API Receives Request
   ───────────────────────────────────────────
   • CORS check (origin allowed?)
   • JWT validation (token valid?)
   • Extract user claims (role: viewer)
   
3. Controller Action
   ───────────────────────────────────────────
   ReleasesController.GetAllReleases()
   • includeUnpublished = false (viewer)
   • Call ReleaseService
   
4. Service Layer
   ───────────────────────────────────────────
   ReleaseService.GetAllReleasesAsync(false)
   • Call ReleaseRepository
   
5. Repository Layer
   ───────────────────────────────────────────
   ReleaseRepository.GetAllAsync(false)
   • Execute sp_GetAllReleases
   • Parameter: @IncludeUnpublished = 0
   
6. Database
   ───────────────────────────────────────────
   sp_GetAllReleases executes:
   SELECT * FROM Releases 
   WHERE IsPublished = 1
   ORDER BY ReleaseDate DESC
   
7. Map Results
   ───────────────────────────────────────────
   Repository: Entity → Service
   Service: Entity → DTO
   
8. API Response
   ───────────────────────────────────────────
   200 OK
   {
     "success": true,
     "message": "Success",
     "data": [
       {
         "releaseId": 1,
         "version": "2.1.0",
         "releaseDate": "2024-01-15",
         ...
       }
     ]
   }
   
9. Frontend Updates
   ───────────────────────────────────────────
   • Parse response
   • Update React state
   • Render UI
```

---

## 📦 Package Dependencies

```
.NET Core Web API
├── Core Packages
│   ├── Microsoft.AspNetCore.App (8.0)
│   └── Microsoft.NETCore.App (8.0)
│
├── Database
│   ├── Dapper (2.1.28)
│   └── Microsoft.Data.SqlClient (5.1.5)
│
├── Authentication
│   ├── Microsoft.AspNetCore.Authentication.JwtBearer (8.0)
│   ├── System.IdentityModel.Tokens.Jwt (7.3.1)
│   └── BCrypt.Net-Next (4.0.3)
│
├── Logging
│   ├── Serilog.AspNetCore (8.0.0)
│   ├── Serilog.Sinks.File (5.0.0)
│   └── Serilog.Sinks.Console (5.0.1)
│
├── Documentation
│   └── Swashbuckle.AspNetCore (6.5.0)
│
├── Validation
│   └── FluentValidation.AspNetCore (11.3.0)
│
└── Excel
    └── EPPlus (7.0.5)
```

---

## 🎯 Component Responsibilities

| Layer | Responsibility | Example |
|-------|---------------|---------|
| **Controller** | HTTP routing, request/response handling | Validate input, call service, return status code |
| **Service** | Business logic, orchestration | Validate business rules, coordinate repositories |
| **Repository** | Data access, SQL execution | Execute stored procedures, map results |
| **Helper** | Utility functions | JWT generation, password hashing |
| **Middleware** | Cross-cutting concerns | Logging, error handling, authentication |
| **DTO** | Data transfer | API request/response models |
| **Entity** | Domain models | Database table representations |

---

This architecture follows **Clean Architecture** and **SOLID principles** for maintainability and testability!

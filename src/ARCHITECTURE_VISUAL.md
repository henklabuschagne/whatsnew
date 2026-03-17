# 🏗️ Complete Application Architecture

## 📊 System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                         USER INTERFACE                              │
│                      (Browser / Mobile)                             │
└────────────────────────┬────────────────────────────────────────────┘
                         │ HTTP/HTTPS
                         │ Port 5173
                         ↓
┌─────────────────────────────────────────────────────────────────────┐
│                    FRONTEND (React 18)                              │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Pages & Components                                         │   │
│  │  ├── LoginPage.tsx          (Authentication)                │   │
│  │  ├── WhatsNew.tsx           (View releases)                 │   │
│  │  ├── ReleaseManagement.tsx  (CRUD releases)                 │   │
│  │  ├── TagManagement.tsx      (CRUD tags)                     │   │
│  │  └── AdminDashboard.tsx     (Dashboard navigation)          │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Data Hooks (Custom React Hooks)                            │   │
│  │  ├── useReleases()          (Release operations)            │   │
│  │  ├── useTags()              (Tag operations)                │   │
│  │  └── useChanges()           (Change operations)             │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  API Service Layer (Axios)                                  │   │
│  │  ├── JWT Token Management                                   │   │
│  │  ├── Request Interceptors   (Add Authorization header)      │   │
│  │  ├── Response Interceptors  (Handle 401, errors)            │   │
│  │  └── Error Handling                                         │   │
│  └─────────────────────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────────────────────┘
                         │ REST API
                         │ JSON
                         │ JWT Token
                         ↓
┌─────────────────────────────────────────────────────────────────────┐
│                  BACKEND (.NET Core 8.0 API)                        │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Middleware                                                  │   │
│  │  ├── Exception Handling     (Global error handling)         │   │
│  │  ├── Audit Logging          (Track all actions)             │   │
│  │  └── JWT Authentication     (Validate tokens)               │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Controllers (API Endpoints)                                │   │
│  │  ├── AuthController         (Login, GetMe, ChangePassword)  │   │
│  │  ├── ReleasesController     (CRUD + Statistics)             │   │
│  │  ├── ChangesController      (CRUD changes)                  │   │
│  │  └── TagsController         (CRUD tags)                     │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Services (Business Logic)                                  │   │
│  │  ├── AuthService            (JWT generation, validation)    │   │
│  │  ├── ReleaseService         (Release business logic)        │   │
│  │  ├── ChangeService          (Change business logic)         │   │
│  │  ├── TagService             (Tag business logic)            │   │
│  │  └── AuditService           (Audit logging logic)           │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Repositories (Data Access)                                 │   │
│  │  ├── UserRepository         (User CRUD)                     │   │
│  │  ├── ReleaseRepository      (Release CRUD)                  │   │
│  │  ├── ChangeRepository       (Change CRUD)                   │   │
│  │  ├── TagRepository          (Tag CRUD)                      │   │
│  │  └── AuditRepository        (Audit log CRUD)                │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Helpers                                                     │   │
│  │  ├── JwtHelper              (Token generation)              │   │
│  │  └── PasswordHelper         (BCrypt hashing)                │   │
│  └─────────────────────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────────────────────┘
                         │ Dapper ORM
                         │ ADO.NET
                         │ SQL Queries
                         ↓
┌─────────────────────────────────────────────────────────────────────┐
│                   DATABASE (SQL Server)                             │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Tables                                                      │   │
│  │  ├── Users              (Authentication & authorization)     │   │
│  │  ├── Releases           (Version management)                │   │
│  │  ├── Changes            (Features, bugs, enhancements)       │   │
│  │  ├── Tags               (Categorization)                    │   │
│  │  ├── Change_Tags        (Many-to-many relationship)         │   │
│  │  └── AuditLogs          (Activity tracking)                 │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Stored Procedures (20+ procedures)                         │   │
│  │  ├── sp_GetAllUsers, sp_GetUserByUsername                   │   │
│  │  ├── sp_GetAllReleases, sp_CreateRelease, sp_UpdateRelease  │   │
│  │  ├── sp_GetAllChanges, sp_CreateChange, sp_UpdateChange     │   │
│  │  ├── sp_GetAllTags, sp_CreateTag, sp_UpdateTag              │   │
│  │  └── sp_CreateAuditLog                                      │   │
│  └─────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Indexes & Constraints                                       │   │
│  │  ├── Primary Keys (ID columns)                              │   │
│  │  ├── Foreign Keys (Relationships)                           │   │
│  │  ├── Unique Constraints (Username, Tag Value)               │   │
│  │  └── Performance Indexes                                    │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 🔐 Authentication Flow

```
┌──────────┐                                    ┌──────────┐
│  User    │                                    │ Database │
└────┬─────┘                                    └────┬─────┘
     │                                                │
     │ 1. Enter username/password                    │
     │─────────────────────────────────────────────→ │
     │                                                │
     │ 2. POST /api/auth/login                       │
     │────────────────────────────────→ ┌──────────┐ │
     │                                  │ Backend  │ │
     │                                  └────┬─────┘ │
     │                                       │       │
     │                                       │ 3. Query user
     │                                       │──────→│
     │                                       │       │
     │                                       │ 4. User data
     │                                       │←──────│
     │                                       │       │
     │                                       │ 5. Verify password (BCrypt)
     │                                       │       │
     │                                       │ 6. Generate JWT token
     │                                       │       │
     │ 7. Return { token, user, expiresAt }  │       │
     │←────────────────────────────────────  │       │
     │                                                │
     │ 8. Store token in localStorage                │
     │                                                │
     │ 9. Attach token to all requests               │
     │    Authorization: Bearer {token}              │
     │─────────────────────────────────────────────→ │
     │                                                │
```

---

## 📊 Data Flow - Create Release

```
┌──────────┐                                    ┌──────────┐
│  User    │                                    │ Database │
└────┬─────┘                                    └────┬─────┘
     │                                                │
     │ 1. Fill release form (version, date)          │
     │                                                │
     │ 2. Click "Create"                             │
     │────────────────────────────────→ ┌──────────┐ │
     │                                  │ Frontend │ │
     │                                  └────┬─────┘ │
     │                                       │       │
     │                                       │ 3. Validate form
     │                                       │       │
     │                                       │ 4. POST /api/releases
     │                                       │       │
     │                                       │       │
     │                                  ┌────▼─────┐ │
     │                                  │ Backend  │ │
     │                                  │Controller│ │
     │                                  └────┬─────┘ │
     │                                       │       │
     │                                       │ 5. Check authorization (admin?)
     │                                       │       │
     │                                       │ 6. Call ReleaseService
     │                                  ┌────▼─────┐ │
     │                                  │ Service  │ │
     │                                  └────┬─────┘ │
     │                                       │       │
     │                                       │ 7. Validate business rules
     │                                       │       │
     │                                       │ 8. Call Repository
     │                                  ┌────▼─────┐ │
     │                                  │Repository│ │
     │                                  └────┬─────┘ │
     │                                       │       │
     │                                       │ 9. Execute sp_CreateRelease
     │                                       │──────→│
     │                                       │       │
     │                                       │10. Insert into Releases table
     │                                       │       │
     │                                       │11. Return ReleaseID
     │                                       │←──────│
     │                                       │       │
     │                                       │12. Log to AuditLogs
     │                                       │──────→│
     │                                       │       │
     │ 13. Return success response           │       │
     │←────────────────────────────────────  │       │
     │                                                │
     │ 14. Show success toast                        │
     │                                                │
     │ 15. Refresh release list                      │
     │─────────────────────────────────────────────→ │
     │                                                │
```

---

## 🔄 Request/Response Flow

```
Frontend Request:
┌────────────────────────────────────────────────────┐
│ POST /api/releases                                 │
│ Headers:                                           │
│   Authorization: Bearer eyJhbGciOiJIUzI1NiIs...    │
│   Content-Type: application/json                   │
│ Body:                                              │
│   {                                                │
│     "version": "1.0.0",                            │
│     "releaseDate": "2024-12-04",                   │
│     "description": "Initial release",              │
│     "isPublished": true                            │
│   }                                                │
└────────────────────────────────────────────────────┘
                         ↓
                    Middleware
                         ↓
                  JWT Validation
                         ↓
                    Controller
                         ↓
                     Service
                         ↓
                   Repository
                         ↓
                  Stored Procedure
                         ↓
Backend Response:
┌────────────────────────────────────────────────────┐
│ HTTP 201 Created                                   │
│ {                                                  │
│   "success": true,                                 │
│   "data": {                                        │
│     "releaseId": 1,                                │
│     "version": "1.0.0",                            │
│     "releaseDate": "2024-12-04T00:00:00Z",         │
│     "description": "Initial release",              │
│     "isPublished": true,                           │
│     "changeCount": 0,                              │
│     "createdByUsername": "admin",                  │
│     "createdAt": "2024-12-04T10:30:00Z"            │
│   },                                               │
│   "message": "Release created successfully"        │
│ }                                                  │
└────────────────────────────────────────────────────┘
```

---

## 🗄️ Database Schema Relationships

```
┌─────────────────────┐
│       Users         │
│─────────────────────│
│ UserId (PK)         │
│ Username (UNIQUE)   │
│ Email               │
│ PasswordHash        │
│ FirstName           │
│ LastName            │
│ Role                │◄────┐
│ IsActive            │     │
│ CreatedAt           │     │
│ UpdatedAt           │     │
└─────────────────────┘     │
                            │ CreatedBy
                            │
┌─────────────────────┐     │
│      Releases       │     │
│─────────────────────│     │
│ ReleaseId (PK)      │     │
│ Version (UNIQUE)    │     │
│ ReleaseDate         │     │
│ Description         │     │
│ IsPublished         │     │
│ CreatedBy (FK)      │─────┘
│ CreatedAt           │
│ UpdatedAt           │
└──────┬──────────────┘
       │ 1:N
       │
       │
┌──────▼──────────────┐
│      Changes        │
│─────────────────────│
│ ChangeId (PK)       │
│ ReleaseId (FK)      │◄────┐
│ Description         │     │
│ ChangeType          │     │
│ CreatedAt           │     │
│ UpdatedAt           │     │
└──────┬──────────────┘     │
       │ M:N                │
       │                    │
       │              ┌─────┴─────────────┐
       │              │   Change_Tags     │
       │              │───────────────────│
       │              │ ChangeId (FK)     │
       └─────────────►│ TagId (FK)        │◄───┐
                      └───────────────────┘    │
                                               │
                                               │
                      ┌────────────────────────┘
                      │
                ┌─────▼─────────────┐
                │       Tags         │
                │────────────────────│
                │ TagId (PK)         │
                │ Value (UNIQUE)     │
                │ Label              │
                │ Type               │
                │ IsActive           │
                │ CreatedAt          │
                │ UpdatedAt          │
                └────────────────────┘

┌─────────────────────┐
│    AuditLogs        │
│─────────────────────│
│ AuditLogId (PK)     │
│ UserId (FK)         │
│ Action              │
│ EntityType          │
│ EntityId            │
│ OldValue            │
│ NewValue            │
│ Timestamp           │
│ IPAddress           │
└─────────────────────┘
```

---

## 📦 Technology Stack

```
┌─────────────────────────────────────────────────────────┐
│                    FRONTEND                             │
├─────────────────────────────────────────────────────────┤
│ React 18               │ UI Framework                   │
│ TypeScript             │ Type Safety                    │
│ React Router           │ Routing                        │
│ Axios                  │ HTTP Client                    │
│ TailwindCSS            │ Styling                        │
│ ShadCN UI              │ Component Library              │
│ Sonner                 │ Toast Notifications            │
│ Lucide React           │ Icons                          │
│ Vite                   │ Build Tool                     │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                    BACKEND                              │
├─────────────────────────────────────────────────────────┤
│ .NET Core 8.0          │ Framework                      │
│ ASP.NET Core Web API   │ Web Framework                  │
│ Dapper                 │ Micro-ORM                      │
│ BCrypt.Net             │ Password Hashing               │
│ JWT Bearer             │ Authentication                 │
│ Swashbuckle            │ Swagger/OpenAPI                │
│ Serilog (optional)     │ Logging                        │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                   DATABASE                              │
├─────────────────────────────────────────────────────────┤
│ SQL Server             │ Database Engine                │
│ T-SQL                  │ Query Language                 │
│ Stored Procedures      │ Business Logic                 │
│ Indexes                │ Performance                    │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                   SECURITY                              │
├─────────────────────────────────────────────────────────┤
│ JWT Tokens             │ Stateless Authentication       │
│ BCrypt                 │ Password Hashing (10 rounds)   │
│ HTTPS                  │ Encrypted Communication        │
│ CORS                   │ Cross-Origin Protection        │
│ Role-Based Access      │ Authorization                  │
│ SQL Parameters         │ SQL Injection Prevention       │
└─────────────────────────────────────────────────────────┘
```

---

## 🚦 Application State Management

```
Frontend State:
┌────────────────────────────────────────┐
│ React Component State                  │
│ ├── useState (local state)             │
│ ├── useEffect (side effects)           │
│ └── Custom Hooks (shared logic)        │
└────────────────────────────────────────┘

┌────────────────────────────────────────┐
│ LocalStorage                           │
│ ├── auth_token (JWT)                   │
│ └── whats-new-current-user (User info) │
└────────────────────────────────────────┘

Backend State:
┌────────────────────────────────────────┐
│ Stateless (except JWT validation)      │
│ └── All state in SQL Server database   │
└────────────────────────────────────────┘
```

---

## ⚡ Performance Optimizations

```
Frontend:
├── Lazy loading components (React.lazy)
├── Debounced search
├── Skeleton loaders
├── Optimistic UI updates
└── React memoization (useMemo, useCallback)

Backend:
├── Stored procedures (pre-compiled)
├── Connection pooling
├── Dapper (fast micro-ORM)
├── Database indexes
└── Async/await (non-blocking)

Database:
├── Clustered indexes on primary keys
├── Non-clustered indexes on foreign keys
├── Filtered indexes on active records
└── Query optimization
```

---

## 📈 Scalability Path

```
Current (Single Server):
Frontend → Backend → Database

Next (Load Balanced):
Frontend → Load Balancer → Backend 1
                        → Backend 2
                        → Backend 3
                              ↓
                        Shared Database

Future (Distributed):
Frontend → CDN → API Gateway → Microservices → Redis Cache
                                              → Message Queue
                                              → Read Replicas
                                              → Write Master
```

---

This architecture provides a solid foundation for:
- ✅ Easy maintenance
- ✅ Testability
- ✅ Scalability
- ✅ Security
- ✅ Performance
- ✅ Extensibility

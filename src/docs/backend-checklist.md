# Backend Module Implementation Checklist

## 📋 **Quick Reference: Complete Module Implementation**

This is your **GO-TO** checklist for implementing ANY backend module. Print this and check off each item!

---

## 🎯 **Phase 1: Analysis & Planning**

### Step 1: Analyze Frontend Requirements

- [ ] Review frontend TypeScript DTOs in `/types/[module].ts`
- [ ] Review frontend API calls in `/services/api.ts`
- [ ] List all API endpoints needed
- [ ] List all CRUD operations needed
- [ ] List all custom actions needed
- [ ] Document all DTO fields with data types
- [ ] Identify computed/derived fields
- [ ] Identify related entities (JOINs needed)

### Step 2: Design Database Schema

- [ ] Design main table(s)
- [ ] Design lookup tables (if needed)
- [ ] Design junction tables for many-to-many (if needed)
- [ ] Plan foreign key relationships
- [ ] Plan indexes for performance
- [ ] Document cascade delete strategy

---

## 🗄️ **Phase 2: Database Layer**

### Step 1: Create SQL Tables

- [ ] Main table with all required columns
- [ ] Primary key (UNIQUEIDENTIFIER DEFAULT NEWID())
- [ ] All business fields
- [ ] Status field (if applicable with CHECK constraint)
- [ ] Audit fields: CreatedAt, UpdatedAt
- [ ] Foreign key constraints
- [ ] Indexes on commonly queried fields
- [ ] Indexes on foreign keys
- [ ] UNIQUE constraints where needed (Version, Code, etc.)
- [ ] Test table creation script

**Template Location:** See `/docs/backend-standards.md` Step 2

### Step 2: Create Stored Procedures

- [ ] `sp_[Module]_Create` - INSERT with validation
- [ ] `sp_[Module]_GetById` - SELECT with JOINs
- [ ] `sp_[Module]_GetAll` - SELECT with filters/ordering
- [ ] `sp_[Module]_Update` - UPDATE with validation
- [ ] `sp_[Module]_Delete` - DELETE (hard or CASCADE)
- [ ] Custom SPs (e.g., GetByRelease, GetByClient)
- [ ] All SPs have BEGIN TRANSACTION
- [ ] All SPs have TRY/CATCH error handling
- [ ] All SPs validate inputs
- [ ] All SPs return full object after write operations
- [ ] Test all stored procedures

**Template Location:** See `/docs/backend-standards.md` Step 3

---

## 📦 **Phase 3: Data Access Layer**

### Step 1: Create Repository Interface

**Location:** `/Backend/WhatsNewAPI/Repositories/I[Module]Repository.cs`

- [ ] Interface created
- [ ] CreateAsync method
- [ ] GetByIdAsync method
- [ ] GetAllAsync method
- [ ] UpdateAsync method
- [ ] DeleteAsync method
- [ ] ExistsAsync method (if needed)
- [ ] Custom methods (e.g., GetByReleaseAsync)

**Example:**

```csharp
public interface IReleaseRepository
{
    Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto dto);
    Task<ReleaseDto?> GetReleaseByIdAsync(Guid id);
    Task<List<ReleaseDto>> GetAllReleasesAsync();
    Task<ReleaseDto?> UpdateReleaseAsync(Guid id, UpdateReleaseDto dto);
    Task<bool> DeleteReleaseAsync(Guid id);
}
```

### Step 2: Create Repository Implementation

**Location:** `/Backend/WhatsNewAPI/Repositories/[Module]Repository.cs`

- [ ] Implementation class created
- [ ] Connection string injected from IConfiguration
- [ ] All interface methods implemented using Dapper
- [ ] All methods call correct stored procedures
- [ ] Parameters mapped correctly
- [ ] Exception handling for SQL errors
- [ ] Using statements for SqlConnection
- [ ] Test all repository methods

**Example:**

```csharp
public class ReleaseRepository : IReleaseRepository
{
    private readonly string _connectionString;

    public ReleaseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Version", dto.Version);
        parameters.Add("@ReleaseDate", dto.ReleaseDate);
        parameters.Add("@NewId", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_Releases_Create",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var newId = parameters.Get<Guid>("@NewId");
        return await GetReleaseByIdAsync(newId);
    }
}
```

---

## 💼 **Phase 4: Business Logic Layer (Optional)**

### Step 1: Create DTOs

**Location:** `/Backend/WhatsNewAPI/DTOs/`

- [ ] **CreateDto** - For POST requests
  - [ ] All required fields
  - [ ] Data Annotations for validation
  - [ ] No computed fields
  - [ ] No ID field
- [ ] **UpdateDto** - For PUT requests
  - [ ] All updateable fields
  - [ ] Validation rules
  - [ ] May or may not include ID
- [ ] **ResponseDto** - For GET responses
  - [ ] All table fields
  - [ ] Computed fields (counts, etc.)
  - [ ] Related entity data (JOINed)
  - [ ] Created/Updated timestamps

**Example:**

```csharp
public class CreateReleaseDto
{
    [Required]
    [StringLength(50)]
    public string Version { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }
}

public class UpdateReleaseDto
{
    [Required]
    [StringLength(50)]
    public string Version { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }
}

public class ReleaseDto
{
    public Guid ReleaseId { get; set; }
    public string Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<ChangeDto> Changes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

---

## 🌐 **Phase 5: API Layer**

### Step 1: Create Controller

**Location:** `/Backend/WhatsNewAPI/Controllers/[Module]Controller.cs`

- [ ] Controller class created
- [ ] `[ApiController]` attribute
- [ ] `[Route("api/[controller]")]` attribute
- [ ] `[Authorize]` attribute for JWT auth
- [ ] Dependencies injected:
  - [ ] ILogger<T>
  - [ ] I[Module]Repository

**Example:**

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReleasesController : ControllerBase
{
    private readonly ILogger<ReleasesController> _logger;
    private readonly IReleaseRepository _releaseRepository;

    public ReleasesController(
        ILogger<ReleasesController> logger,
        IReleaseRepository releaseRepository)
    {
        _logger = logger;
        _releaseRepository = releaseRepository;
    }
}
```

### Step 2: Implement CRUD Endpoints

**CREATE Endpoint:**

- [ ] `[HttpPost]` attribute
- [ ] `[ProducesResponseType(typeof(ResponseDto), 201)]`
- [ ] `[ProducesResponseType(400)]` for validation errors
- [ ] Accepts CreateDto from body
- [ ] Calls repository.CreateAsync
- [ ] Returns CreatedAtAction with location header
- [ ] Exception handling with appropriate status codes
- [ ] Logging

**READ BY ID Endpoint:**

- [ ] `[HttpGet("{id}")]` attribute
- [ ] `[ProducesResponseType(typeof(ResponseDto), 200)]`
- [ ] `[ProducesResponseType(404)]`
- [ ] Accepts id from route
- [ ] Calls repository.GetByIdAsync
- [ ] Returns 404 if not found
- [ ] Returns 200 with data if found
- [ ] Logging

**READ ALL Endpoint:**

- [ ] `[HttpGet]` attribute
- [ ] `[ProducesResponseType(typeof(List<ResponseDto>), 200)]`
- [ ] Calls repository.GetAllAsync
- [ ] Returns 200 with list (even if empty)
- [ ] Logging

**UPDATE Endpoint:**

- [ ] `[HttpPut("{id}")]` attribute
- [ ] `[ProducesResponseType(typeof(ResponseDto), 200)]`
- [ ] `[ProducesResponseType(404)]`
- [ ] `[ProducesResponseType(400)]`
- [ ] Accepts id from route
- [ ] Accepts UpdateDto from body
- [ ] Calls repository.UpdateAsync
- [ ] Returns 404 if not found
- [ ] Returns 200 with updated data
- [ ] Logging

**DELETE Endpoint:**

- [ ] `[HttpDelete("{id}")]` attribute
- [ ] `[ProducesResponseType(204)]`
- [ ] `[ProducesResponseType(404)]`
- [ ] Accepts id from route
- [ ] Calls repository.DeleteAsync
- [ ] Returns 404 if not found
- [ ] Returns 204 No Content if successful
- [ ] Logging

### Step 3: Implement Custom Action Endpoints

**For each custom action:**

- [ ] Appropriate HTTP verb (`[HttpGet]`, `[HttpPost]`, or `[HttpPut]`)
- [ ] Route attribute (e.g., `[HttpGet("release/{releaseId}")]`)
- [ ] ProducesResponseType attributes
- [ ] Extract parameters from route/query/body
- [ ] Call appropriate repository method
- [ ] Return appropriate status code
- [ ] Exception handling
- [ ] Logging

### Step 4: Add XML Documentation Comments

**For the controller:**

```csharp
/// <summary>
/// Manages release operations for the What's New application
/// </summary>
[ApiController]
public class ReleasesController : ControllerBase
```

**For each endpoint:**

```csharp
/// <summary>
/// Creates a new software release
/// </summary>
/// <param name="dto">The release creation data</param>
/// <returns>The created release</returns>
/// <response code="201">Successfully created</response>
/// <response code="400">Invalid input</response>
[HttpPost]
```

---

## ⚙️ **Phase 6: Configuration & Registration**

### Step 1: Register Services in DI

**Location:** `/Backend/WhatsNewAPI/Program.cs`

- [ ] Repository interface and implementation registered:
  ```csharp
  builder.Services.AddScoped<I[Module]Repository, [Module]Repository>();
  ```

**Example:**

```csharp
// Add repositories
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IChangeRepository, ChangeRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
```

---

## 🔍 **Phase 7: Verification & Testing**

### Step 1: Verify Endpoint URLs Match Frontend

- [ ] Open `/services/api.ts`
- [ ] List all frontend API calls
- [ ] For each call, verify:
  - [ ] HTTP method matches
  - [ ] URL path matches
  - [ ] Route parameters match
  - [ ] Query parameters match
- [ ] Document any mismatches
- [ ] Update backend OR frontend to match

**Template Location:** See `/docs/backend-standards.md` Step 4

### Step 2: Verify Field Matching

**Create a spreadsheet:**

| Field Name       | Frontend DTO | Backend DTO | DB Table | Stored Proc | Match? |
| ---------------- | ------------ | ----------- | -------- | ----------- | ------ |
| id / ReleaseId   | ✅ string    | ✅ Guid     | ✅ UNIQUEIDENTIFIER | ✅ Selected | ✅ |
| version / Version | ✅ string   | ✅ string   | ✅ NVARCHAR(50) | ✅ Selected | ✅ |
| releaseDate      | ✅ string    | ✅ DateTime | ✅ DATE  | ✅ Selected | ✅ |

- [ ] All frontend DTO fields verified
- [ ] All backend DTO fields verified
- [ ] All table columns verified
- [ ] All SP SELECTs include required fields
- [ ] Data types compatible across layers
- [ ] Nullable consistency verified
- [ ] Computed fields have SQL logic
- [ ] Related entity names JOINed in SPs

**Template Location:** See `/docs/backend-standards.md` Step 5

### Step 3: Update Backend to Match Frontend

- [ ] Add any missing computed fields to SPs
- [ ] Add any missing JOINs for related names
- [ ] Add any missing fields to backend DTOs
- [ ] Verify all changes tested

**Template Location:** See `/docs/backend-standards.md` Step 6

---

## 🧪 **Phase 8: Testing**

### Step 1: Manual API Testing

- [ ] Test POST endpoint (create)
  - [ ] With valid data
  - [ ] With missing required fields
  - [ ] With duplicate unique values
- [ ] Test GET by ID endpoint
  - [ ] With existing ID
  - [ ] With non-existent ID
- [ ] Test GET all endpoint
  - [ ] With data
  - [ ] With empty database
- [ ] Test PUT endpoint (update)
  - [ ] With valid data
  - [ ] With non-existent ID
  - [ ] With duplicate unique values
- [ ] Test DELETE endpoint
  - [ ] With existing ID
  - [ ] With non-existent ID
  - [ ] With ID that has dependent records
- [ ] Test custom action endpoints
- [ ] Test without authentication token (should return 401)
- [ ] Test with invalid token (should return 401)

### Step 2: Frontend Integration Testing

- [ ] Switch frontend to use real API
- [ ] Test CREATE operation from frontend
- [ ] Test READ operations from frontend
- [ ] Test UPDATE operation from frontend
- [ ] Test DELETE operation from frontend
- [ ] Test custom actions from frontend
- [ ] Verify error handling in frontend
- [ ] Verify loading states work
- [ ] Verify validation errors display correctly

---

## 📚 **Phase 9: Documentation**

### Step 1: API Documentation

- [ ] XML comments on all controller actions
- [ ] Example requests/responses documented

### Step 2: Database Documentation

- [ ] Table schema documented
- [ ] Foreign key relationships documented
- [ ] Indexes documented
- [ ] Stored procedures documented with parameters

### Step 3: Update README

- [ ] Module added to completed list
- [ ] API endpoints documented
- [ ] Database tables documented
- [ ] Known issues documented (if any)

---

## ✅ **Phase 10: Final Verification**

### Security Checklist

- [ ] JWT authentication enforced on all endpoints (except login)
- [ ] Input validation working (Data Annotations)
- [ ] SQL injection prevention (parameterized queries)
- [ ] Authorization policies applied where needed (admin vs viewer)

### Performance Checklist

- [ ] Database indexes created on frequently queried columns
- [ ] N+1 query problems avoided (proper JOINs in SPs)
- [ ] Connection pooling configured

### Operations Checklist

- [ ] Structured logging throughout
- [ ] Exception handling middleware catching errors
- [ ] CORS configured properly

### Code Quality Checklist

- [ ] Code follows naming conventions
- [ ] No code duplication
- [ ] All async methods properly awaited
- [ ] Using statements for disposable resources
- [ ] Constants used instead of magic strings/numbers
- [ ] Error messages clear and helpful

---

## 🎯 **Module Status Tracking**

| Module             | Phase 2 | Phase 3 | Phase 4 | Phase 5 | Phase 6 | Phase 7 | Phase 8 | Phase 9 | Complete |
| ------------------ | ------- | ------- | ------- | ------- | ------- | ------- | ------- | ------- | -------- |
| **Authentication** | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **Releases**       | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **Changes**        | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **Tags**           | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **Clients**        | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **SQL Integration**| ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **Analytics**      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |
| **Import/Export**  | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅      | ✅       |

---

## 🚀 **Quick Start: Implementing a New Module**

1. **Print this checklist**
2. **Start with Phase 1** - Analyze frontend requirements
3. **Work through phases sequentially** - Don't skip ahead
4. **Check off each item** as you complete it
5. **Test thoroughly** before moving to next phase
6. **Update status tracking table** as you complete each phase
7. **Don't mark module "complete"** until ALL phases done

---

## 📖 **Reference Documents**

- **Database & Stored Procedures:** `/docs/backend-standards.md`
- **Frontend Standards:** `/docs/development-standards.md`
- **Development Checklist:** `/docs/development-checklist.md`

---

## 📋 **What's New Application - Specific Modules**

### Core Modules

1. **Authentication (`/api/auth`)**
   - Login, Logout, Get Current User
   - Tables: Users
   - Roles: viewer, admin

2. **Releases (`/api/releases`)**
   - CRUD for software releases
   - Tables: Releases
   - Fields: ReleaseId, Version, ReleaseDate

3. **Changes (`/api/changes`)**
   - CRUD for individual changes
   - Tables: Changes
   - Fields: ChangeId, ReleaseId, Description, ChangeType, ClientId, TicketNumber, DevOpsNumber
   - Types: bug-fix, new-feature, enhancement

4. **Tags (`/api/tags`)**
   - CRUD for tags
   - Tables: Tags, ChangeTags (junction)
   - Fields: TagId, Label, Value, Type
   - Types: module, changeType
   - Modules: import, export, packs, systems, security, reports, publisher, dashboard

5. **Clients (`/api/clients`)**
   - CRUD for client tracking
   - Tables: Clients
   - Fields: ClientId, Name, Code, ContactEmail, ContactPhone, IsActive

6. **SQL Integration (`/api/sqlintegration`)**
   - CRUD for SQL connections
   - Tables: Integrations
   - Fields: IntegrationId, Name, Server, Database, Query, IsEnabled

7. **Analytics (`/api/analytics`)**
   - Read-only analytics endpoints
   - No dedicated tables (uses views/aggregations)

8. **Import/Export (`/api/importexport`)**
   - Excel import/export functionality
   - No dedicated tables (uses ExcelService)

---

## ✅ **Definition of "Complete"**

A module is **ONLY** considered complete when:

1. ✅ All 10 phases completed
2. ✅ All checklist items verified
3. ✅ All tests passing
4. ✅ Frontend integration working
5. ✅ Documentation complete
6. ✅ Code reviewed
7. ✅ Security verified
8. ✅ Performance verified

**No exceptions. No shortcuts.** 🎯

This ensures a **fully operational, production-ready application**.

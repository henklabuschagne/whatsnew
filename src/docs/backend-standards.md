# .NET Backend Implementation Standards

## 📋 **Mandatory Checklist for ALL Backend Modules**

Before marking any module as "complete", ALL of the following must be implemented and verified:

- [ ] **Step 1:** .NET Core Controllers created
- [ ] **Step 2:** SQL Tables designed and created
- [ ] **Step 3:** SQL Stored Procedures for all CRUD operations
- [ ] **Step 4:** Endpoints match frontend URLs exactly
- [ ] **Step 5:** DTOs, table fields, and stored procedures all match
- [ ] **Step 6:** Backend updated to match frontend requirements
- [ ] **Step 7:** Integration testing completed
- [ ] **Step 8:** Authentication/authorization verified

---

## 🏗️ **ARCHITECTURE DECISION: Service Layer Pattern**

**Date:** February 2, 2026  
**Decision:** Hybrid approach - Use service layer only where business logic exists

### When to Use Service Layer

**USE SERVICE LAYER WHEN:**
- Password hashing/validation required (e.g., AuthService)
- Token generation/validation required (e.g., AuthService)
- Complex data transformation required
- External system integration required (e.g., SqlIntegrationService)
- Business rules beyond simple CRUD
- Multiple repository coordination needed
- Data validation spanning multiple entities

**Example:**
```csharp
// NEEDS SERVICE: Authentication (complex logic)
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        // Service handles: password validation, token generation, user mapping
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }
}
```

### When to Skip Service Layer

**SKIP SERVICE LAYER WHEN:**
- Pure CRUD operations (Create, Read, Update, Delete)
- Single repository, no coordination needed
- No business logic beyond data access
- Direct data mapping (Repository handles it all)
- Simple validation (can be done in controller)

**Example:**
```csharp
// NO SERVICE NEEDED: Releases (simple CRUD)
[ApiController]
[Route("api/[controller]")]
public class ReleasesController : ControllerBase
{
    private readonly IReleaseRepository _releaseRepository;
    
    public ReleasesController(IReleaseRepository releaseRepository)
    {
        _releaseRepository = releaseRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ReleaseDto>>> GetAll()
    {
        // Direct repository call - no business logic needed
        var results = await _releaseRepository.GetAllReleasesAsync();
        return Ok(results);
    }
}
```

### Current Implementation Pattern

**Modules WITH Service Layer:**
- ✅ Authentication (AuthService) - Password hashing, token generation
- ✅ SQL Integration (SqlIntegrationService) - Connection testing, data sync

**Modules WITHOUT Service Layer:**
- ✅ Releases (direct to ReleaseRepository) - Simple CRUD
- ✅ Tags (direct to TagRepository) - Simple CRUD
- ✅ Changes (direct to ChangeRepository) - Simple CRUD
- ✅ Clients (direct to ClientRepository) - Simple CRUD
- ✅ Analytics (direct to AnalyticsRepository) - Read-only queries

**This is intentional and follows best practices - don't add services just for consistency.**

---

## 🎯 **Step 1: Build .NET Core Controllers**

### Controller Structure Requirements

**Namespace Convention:**

```csharp
namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT authentication required
    public class ReleasesController : ControllerBase
    {
        // Controller implementation
    }
}
```

### Required Controller Elements

**1. Dependency Injection:**

```csharp
private readonly ILogger<ReleasesController> _logger;
private readonly IReleaseRepository _releaseRepository;

public ReleasesController(
    ILogger<ReleasesController> logger,
    IReleaseRepository releaseRepository)
{
    _logger = logger;
    _releaseRepository = releaseRepository;
}
```

**2. Standard CRUD Endpoints:**

Every controller MUST implement these at minimum:

```csharp
// CREATE
[HttpPost]
[ProducesResponseType(typeof(ReleaseDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<ReleaseDto>> Create([FromBody] CreateReleaseDto dto)
{
    try
    {
        var result = await _releaseRepository.CreateReleaseAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.ReleaseId }, result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating release");
        return StatusCode(500, "An error occurred");
    }
}

// READ BY ID
[HttpGet("{id}")]
[ProducesResponseType(typeof(ReleaseDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ReleaseDto>> GetById(Guid id)
{
    var result = await _releaseRepository.GetReleaseByIdAsync(id);
    if (result == null) return NotFound();
    return Ok(result);
}

// READ LIST
[HttpGet]
[ProducesResponseType(typeof(List<ReleaseDto>), StatusCodes.Status200OK)]
public async Task<ActionResult<List<ReleaseDto>>> GetAll()
{
    var results = await _releaseRepository.GetAllReleasesAsync();
    return Ok(results);
}

// UPDATE
[HttpPut("{id}")]
[ProducesResponseType(typeof(ReleaseDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ReleaseDto>> Update(Guid id, [FromBody] UpdateReleaseDto dto)
{
    try
    {
        var result = await _releaseRepository.UpdateReleaseAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating release");
        return StatusCode(500, "An error occurred");
    }
}

// DELETE
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(Guid id)
{
    var success = await _releaseRepository.DeleteReleaseAsync(id);
    if (!success) return NotFound();
    return NoContent();
}
```

### Controller Checklist

Before completing a controller:

- [ ] All CRUD endpoints implemented
- [ ] JWT authentication applied (`[Authorize]` attribute)
- [ ] Proper HTTP status codes returned
- [ ] Exception handling with logging
- [ ] ProducesResponseType attributes for documentation
- [ ] Route attributes match frontend API calls

---

## 🗄️ **Step 2: Build SQL Tables**

### Table Design Standards

**Naming Convention:**

- Use PascalCase for table names
- Singular names (e.g., `Release` not `Releases`)
- Descriptive and specific names

**Required Columns for ALL Tables:**

```sql
CREATE TABLE [dbo].[Release]
(
    -- Primary Key
    [ReleaseId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    -- Your module-specific fields here
    [Version] NVARCHAR(50) NOT NULL UNIQUE,
    [ReleaseDate] DATE NOT NULL,

    -- Audit Fields (REQUIRED)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Indexes (REQUIRED for performance)
CREATE NONCLUSTERED INDEX [IX_Release_ReleaseDate]
    ON [dbo].[Release]([ReleaseDate] DESC);

CREATE NONCLUSTERED INDEX [IX_Release_Version]
    ON [dbo].[Release]([Version]);
```

### Table Design Checklist

- [ ] Primary key defined (UNIQUEIDENTIFIER with NEWID())
- [ ] All audit fields included (CreatedAt, UpdatedAt)
- [ ] Proper foreign key constraints
- [ ] Indexes on commonly queried fields
- [ ] NOT NULL constraints on required fields
- [ ] Appropriate data types matching frontend DTOs
- [ ] NVARCHAR for text fields (Unicode support)
- [ ] DATETIME2 for dates (better precision)
- [ ] UNIQUE constraints where needed (e.g., Version, Code)

### Common Table Patterns

**Main Entity Table:**

```sql
CREATE TABLE [dbo].[Release]
(
    [ReleaseId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Version] NVARCHAR(50) NOT NULL UNIQUE,
    [ReleaseDate] DATE NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

**Lookup Tables:**

```sql
CREATE TABLE [dbo].[Tag]
(
    [TagId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Label] NVARCHAR(100) NOT NULL,
    [Value] NVARCHAR(100) NOT NULL UNIQUE,
    [Type] NVARCHAR(50) NOT NULL CHECK (Type IN ('module', 'changeType')),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

**Junction Tables (Many-to-Many):**

```sql
CREATE TABLE [dbo].[ChangeTag]
(
    [ChangeTagId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [ChangeId] UNIQUEIDENTIFIER NOT NULL,
    [TagId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [FK_ChangeTag_Change]
        FOREIGN KEY ([ChangeId]) REFERENCES [dbo].[Change]([ChangeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChangeTag_Tag]
        FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag]([TagId]) ON DELETE CASCADE,
    CONSTRAINT [UQ_ChangeTag_ChangeTag]
        UNIQUE ([ChangeId], [TagId])
);
```

---

## 📜 **Step 3: Build SQL Stored Procedures**

### Naming Convention

```
sp_[ModuleName]_[Action]
```

Examples:

- `sp_Releases_Create`
- `sp_Releases_GetById`
- `sp_Releases_GetAll`
- `sp_Releases_Update`
- `sp_Releases_Delete`
- `sp_Changes_GetByRelease`

### Required Stored Procedures (Minimum)

**1. CREATE:**

```sql
CREATE PROCEDURE [dbo].[sp_Releases_Create]
    @Version NVARCHAR(50),
    @ReleaseDate DATE,
    @NewId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validate inputs
        IF @Version IS NULL OR @Version = ''
            THROW 50001, 'Version is required', 1;

        IF @ReleaseDate IS NULL
            THROW 50002, 'Release date is required', 1;

        -- Check for duplicate version
        IF EXISTS (SELECT 1 FROM [dbo].[Release] WHERE [Version] = @Version)
            THROW 50003, 'Version already exists', 1;

        -- Insert record
        SET @NewId = NEWID();

        INSERT INTO [dbo].[Release]
        (
            [ReleaseId],
            [Version],
            [ReleaseDate],
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES
        (
            @NewId,
            @Version,
            @ReleaseDate,
            GETUTCDATE(),
            GETUTCDATE()
        );

        COMMIT TRANSACTION;

        -- Return the created record
        EXEC [dbo].[sp_Releases_GetById] @NewId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
```

**2. READ BY ID:**

```sql
CREATE PROCEDURE [dbo].[sp_Releases_GetById]
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.[ReleaseId],
        r.[Version],
        r.[ReleaseDate],
        r.[CreatedAt],
        r.[UpdatedAt]
    FROM [dbo].[Release] r
    WHERE r.[ReleaseId] = @ReleaseId;
END
GO
```

**3. READ LIST:**

```sql
CREATE PROCEDURE [dbo].[sp_Releases_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.[ReleaseId],
        r.[Version],
        r.[ReleaseDate],
        r.[CreatedAt],
        r.[UpdatedAt]
    FROM [dbo].[Release] r
    ORDER BY r.[ReleaseDate] DESC, r.[Version] DESC;
END
GO
```

**4. UPDATE:**

```sql
CREATE PROCEDURE [dbo].[sp_Releases_Update]
    @ReleaseId UNIQUEIDENTIFIER,
    @Version NVARCHAR(50),
    @ReleaseDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if record exists
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[Release]
            WHERE [ReleaseId] = @ReleaseId
        )
            THROW 50404, 'Release not found', 1;

        -- Check for duplicate version
        IF EXISTS (
            SELECT 1 FROM [dbo].[Release]
            WHERE [Version] = @Version AND [ReleaseId] <> @ReleaseId
        )
            THROW 50003, 'Version already exists', 1;

        -- Update record
        UPDATE [dbo].[Release]
        SET
            [Version] = @Version,
            [ReleaseDate] = @ReleaseDate,
            [UpdatedAt] = GETUTCDATE()
        WHERE [ReleaseId] = @ReleaseId;

        COMMIT TRANSACTION;

        -- Return updated record
        EXEC [dbo].[sp_Releases_GetById] @ReleaseId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
```

**5. DELETE (Hard Delete):**

```sql
CREATE PROCEDURE [dbo].[sp_Releases_Delete]
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if record exists
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[Release]
            WHERE [ReleaseId] = @ReleaseId
        )
            THROW 50404, 'Release not found', 1;

        -- Delete record (CASCADE will delete related changes)
        DELETE FROM [dbo].[Release]
        WHERE [ReleaseId] = @ReleaseId;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
```

### Stored Procedure Checklist

- [ ] All CRUD operations have stored procedures
- [ ] Transaction management (BEGIN/COMMIT/ROLLBACK)
- [ ] Error handling (TRY/CATCH blocks)
- [ ] Input validation
- [ ] Return full object after CREATE/UPDATE
- [ ] Proper NULL handling with COALESCE
- [ ] Performance optimized (proper indexes used)
- [ ] UNIQUE constraint checks before INSERT/UPDATE

---

## 🔗 **Step 4: Verify Endpoint URLs Match Frontend**

### Frontend API File Location

Check the `/services/api.ts` file for frontend API calls.

### URL Matching Process

**1. Extract Frontend URLs:**

Example from `/services/api.ts`:

```typescript
// Frontend expects these URLs:
GET    /api/releases                      // Get all
GET    /api/releases/{id}                 // Get by ID
POST   /api/releases                      // Create
PUT    /api/releases/{id}                 // Update
DELETE /api/releases/{id}                 // Delete

GET    /api/changes/release/{releaseId}   // Get changes by release
POST   /api/changes                       // Create change
PUT    /api/changes/{id}                  // Update change
DELETE /api/changes/{id}                  // Delete change

GET    /api/tags                          // Get all tags
POST   /api/tags                          // Create tag
PUT    /api/tags/{id}                     // Update tag
DELETE /api/tags/{id}                     // Delete tag

GET    /api/clients                       // Get all clients
POST   /api/clients                       // Create client
PUT    /api/clients/{id}                  // Update client
DELETE /api/clients/{id}                  // Delete client
```

**2. Match Backend Routes:**

```csharp
[ApiController]
[Route("api/[controller]")] // This creates /api/releases
public class ReleasesController : ControllerBase
{
    [HttpGet]                           // GET /api/releases ✅
    public async Task<ActionResult> GetAll() { }

    [HttpGet("{id}")]                    // GET /api/releases/{id} ✅
    public async Task<ActionResult> GetById(Guid id) { }

    [HttpPost]                           // POST /api/releases ✅
    public async Task<ActionResult> Create() { }

    [HttpPut("{id}")]                    // PUT /api/releases/{id} ✅
    public async Task<ActionResult> Update(Guid id) { }

    [HttpDelete("{id}")]                 // DELETE /api/releases/{id} ✅
    public async Task<ActionResult> Delete(Guid id) { }
}
```

### Endpoint Verification Checklist

For each module:

- [ ] List all frontend API calls from `/services/api.ts`
- [ ] Extract HTTP method and URL for each call
- [ ] Verify controller route matches exactly
- [ ] Verify HTTP method attribute matches (GET/POST/PUT/DELETE)
- [ ] Verify route parameters match (`{id}`, `{releaseId}`, etc.)
- [ ] Verify query parameters match
- [ ] Document any discrepancies and fix backend OR frontend

### Common URL Patterns

```
GET    /api/module                       // Get all
GET    /api/module/{id}                  // Get by ID
GET    /api/module/parent/{parentId}     // Get by parent entity
POST   /api/module                       // Create
PUT    /api/module/{id}                  // Update
DELETE /api/module/{id}                  // Delete
POST   /api/module/{id}/action           // Special action
```

---

## 🔄 **Step 5: Verify DTO, Table, and Stored Procedure Field Matching**

### Three-Way Field Matching Process

For EVERY field in your module, verify it exists in all three places:

**Example: Release Module**

#### 1. Frontend DTO (`/types/release.ts`):

```typescript
export interface Release {
  id: string;
  version: string;
  releaseDate: string;
  changes: Change[];
  createdAt?: string;
  updatedAt?: string;
}
```

#### 2. Backend DTO (`/Backend/WhatsNewAPI/DTOs/ReleaseDto.cs`):

```csharp
public class ReleaseDto
{
    public Guid ReleaseId { get; set; }
    public string Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<ChangeDto> Changes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### 3. Database Table:

```sql
CREATE TABLE [dbo].[Release]
(
    [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
    [Version] NVARCHAR(50) NOT NULL,
    [ReleaseDate] DATE NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL
);
```

#### 4. Stored Procedure SELECT:

```sql
CREATE PROCEDURE [dbo].[sp_Releases_GetById]
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT
        r.[ReleaseId],
        r.[Version],
        r.[ReleaseDate],
        r.[CreatedAt],
        r.[UpdatedAt]
    FROM [dbo].[Release] r
    WHERE r.[ReleaseId] = @ReleaseId;
END
```

### Field Matching Checklist

Create a spreadsheet for each module:

| Field Name              | Frontend DTO | Backend DTO | Database Table | Stored Proc | Match? |
| ----------------------- | ------------ | ----------- | -------------- | ----------- | ------ |
| id / ReleaseId          | ✅ string    | ✅ Guid     | ✅ UNIQUEIDENTIFIER | ✅ Selected | ✅     |
| version / Version       | ✅ string    | ✅ string   | ✅ NVARCHAR(50) | ✅ Selected | ✅     |
| releaseDate / ReleaseDate | ✅ string  | ✅ DateTime | ✅ DATE        | ✅ Selected | ✅     |

**Verification Steps:**

- [ ] List all fields from frontend DTO
- [ ] Verify each field exists in backend DTO
- [ ] Verify each field exists in database table
- [ ] Verify each field is selected in stored procedures
- [ ] Check data type compatibility (TypeScript ↔ C# ↔ SQL)
- [ ] Check nullable consistency (null | undefined ↔ ? ↔ NULL)
- [ ] Verify computed fields are calculated correctly
- [ ] Verify JOIN fields are included (names, etc.)

### Common Data Type Mappings

| TypeScript        | C#           | SQL Server         |
| ----------------- | ------------ | ------------------ |
| string            | Guid         | UNIQUEIDENTIFIER   |
| string            | string       | NVARCHAR(n)        |
| string            | string       | NVARCHAR(MAX)      |
| boolean           | bool         | BIT                |
| string (ISO date) | DateTime     | DATETIME2          |
| string (ISO date) | DateTime     | DATE               |
| number            | int          | INT                |
| null \| undefined | ? (nullable) | NULL               |

### Naming Convention Conversion

**Frontend (camelCase) ↔ Backend (PascalCase):**

```
id               ↔ ReleaseId
version          ↔ Version
releaseDate      ↔ ReleaseDate
changeType       ↔ ChangeType
moduleTags       ↔ ModuleTags
clientId         ↔ ClientId
ticketNumber     ↔ TicketNumber
devopsNumber     ↔ DevOpsNumber
```

**Backend uses PascalCase for:**

- DTOs
- Properties
- Method names
- Controller actions

**Database uses PascalCase for:**

- Table names
- Column names
- Stored procedure names
- Parameter names

---

## 🔧 **Step 6: Update Backend to Match Frontend**

### When to Update Backend vs Frontend

**Update Backend When:**

- Frontend DTO has additional fields needed by UI
- Frontend expects different data structure
- Frontend requires computed/derived fields

**Update Frontend When:**

- Backend constraints are more restrictive
- Database has additional required fields
- Business logic requires additional data

### Common Updates Needed

**1. Add Computed Fields:**

Frontend expects `changeCount`, but it's not a table column:

```sql
-- Add to stored procedure SELECT:
(SELECT COUNT(*)
 FROM [dbo].[Change]
 WHERE [ReleaseId] = r.[ReleaseId]) AS [ChangeCount]
```

```csharp
// Add to backend DTO:
public int ChangeCount { get; set; }
```

**2. Add JOIN Fields:**

Frontend expects tag labels from related table:

```sql
-- Add JOIN to stored procedure:
LEFT JOIN [dbo].[Tag] t ON ct.[TagId] = t.[TagId]

-- Add to SELECT:
STRING_AGG(t.[Label], ', ') AS [TagLabels]
```

```csharp
// Add to backend DTO:
public string TagLabels { get; set; }
```

**3. Handle Nullable Fields:**

```csharp
// Backend DTO:
public string? Description { get; set; }      // Nullable
public Guid? ClientId { get; set; }           // Nullable
public string? TicketNumber { get; set; }     // Nullable
```

```typescript
// Frontend interface:
description?: string | null;
clientId?: string | null;
ticketNumber?: string | null;
```

---

## ✅ **Final Verification Checklist**

Before marking module as complete:

- [ ] All tables created with proper structure
- [ ] All stored procedures created and tested
- [ ] All DTOs match frontend interfaces
- [ ] All controller endpoints implemented
- [ ] All repository methods implemented
- [ ] All services registered in Program.cs
- [ ] All endpoints secured with [Authorize]
- [ ] All field mappings verified
- [ ] All URL routes match frontend
- [ ] All error handling implemented
- [ ] All logging added
- [ ] Integration testing completed
- [ ] Authentication/authorization working

---

## 📖 **What's New Application Modules**

### Current Modules

| Module             | Controllers | Tables    | Stored Procedures | Complete |
| ------------------ | ----------- | --------- | ----------------- | -------- |
| **Authentication** | ✅ Auth     | ✅ Users  | ✅ Auth SPs       | ✅       |
| **Releases**       | ✅ Releases | ✅ Releases | ✅ Release SPs   | ✅       |
| **Changes**        | ✅ Changes  | ✅ Changes | ✅ Change SPs    | ✅       |
| **Tags**           | ✅ Tags     | ✅ Tags, ChangeTags | ✅ Tag SPs | ✅  |
| **Clients**        | ✅ Clients  | ✅ Clients | ✅ Client SPs    | ✅       |
| **SQL Integration**| ✅ SqlIntegration | ✅ Integrations | ✅ Integration SPs | ✅ |
| **Analytics**      | ✅ Analytics | ❌ Views  | ✅ Analytics SPs | ✅       |
| **Import/Export**  | ✅ ImportExport | ❌     | ❌               | ✅       |

### Core Entities

1. **Users** - Authentication and authorization
   - Fields: UserId, Name, Email, PasswordHash, Role
   - Roles: 'viewer', 'admin'

2. **Releases** - Software versions
   - Fields: ReleaseId, Version, ReleaseDate, CreatedAt, UpdatedAt
   - Relationships: One-to-Many with Changes

3. **Changes** - Bug fixes, features, enhancements
   - Fields: ChangeId, ReleaseId, Description, ChangeType, ClientId, TicketNumber, DevOpsNumber
   - Types: 'bug-fix', 'new-feature', 'enhancement'
   - Relationships: Many-to-One with Release, Many-to-Many with Tags, Many-to-One with Client

4. **Tags** - Module and change type categorization
   - Fields: TagId, Label, Value, Type
   - Types: 'module', 'changeType'
   - Modules: import, export, packs, systems, security, reports, publisher, dashboard

5. **Clients** - Customer/client tracking
   - Fields: ClientId, Name, Code, ContactEmail, ContactPhone, IsActive

6. **Integrations** - SQL connection configurations
   - Fields: IntegrationId, Name, Server, Database, Query, IsEnabled

---

## 🚀 **Quick Start Guide**

### Creating a New Module

1. **Design Database Schema**
   - Create table with proper fields
   - Add foreign keys and indexes
   - Document relationships

2. **Create Stored Procedures**
   - sp_[Module]_Create
   - sp_[Module]_GetById
   - sp_[Module]_GetAll
   - sp_[Module]_Update
   - sp_[Module]_Delete

3. **Create DTOs**
   - Create[Module]Dto
   - Update[Module]Dto
   - [Module]Dto

4. **Create Repository**
   - Interface: I[Module]Repository
   - Implementation: [Module]Repository

5. **Create Controller**
   - [Module]Controller with CRUD endpoints

6. **Register Services**
   - Add to Program.cs

7. **Test**
   - Test all endpoints
   - Verify authentication
   - Check error handling

---

## ✅ **Definition of Complete**

A module is ONLY complete when:

1. ✅ All tables created with proper structure
2. ✅ All stored procedures implemented
3. ✅ All DTOs match frontend interfaces
4. ✅ All endpoints secured and tested
5. ✅ All field mappings verified
6. ✅ Integration testing passed
7. ✅ Documentation complete

**No exceptions. No shortcuts.** 🎯

This ensures a **fully operational, production-ready application**.
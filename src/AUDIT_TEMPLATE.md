# MODULE AUDIT TEMPLATE

## MODULE: [Module Name]
**Date:** [Date]  
**Auditor:** [Name]  
**Status:** 🔄 IN PROGRESS / ✅ PASS / ❌ FAIL / ⚠️ NEEDS FIXES

---

## DATA FLOW DIAGRAM

```
┌─────────────────────────────────────────────────────────────────────┐
│ FRONTEND LAYER                                                       │
├─────────────────────────────────────────────────────────────────────┤
│ 1. User Interface Component                                         │
│    File: /pages/[module]/[ComponentName].tsx                        │
│    Route: /[route-path]                                             │
│    Data Needed: [List what data component expects]                  │
│    ↓                                                                 │
│ 2. API Call                                                          │
│    Function: [apiName].[methodName](params)                         │
│    File: /utils/api.ts or /utils/apiWithMock.ts                     │
│    Parameters: { param1, param2, ... }                              │
│    ↓                                                                 │
│ 3. Type Definition                                                   │
│    File: /types/[module].ts                                         │
│    Interface: [InterfaceName]                                       │
└─────────────────────────────────────────────────────────────────────┘
                              ↓ HTTP REQUEST
┌─────────────────────────────────────────────────────────────────────┐
│ API UTILITIES LAYER                                                  │
├─────────────────────────────────────────────────────────────────────┤
│ 4. API Utility Function                                              │
│    File: /utils/api.ts                                              │
│    Endpoint: [HTTP_METHOD] /api/[controller]/[action]              │
│    Request Body: { ... }                                            │
│    Expected Response: { data: { ... } }                             │
└─────────────────────────────────────────────────────────────────────┘
                              ↓ HTTP REQUEST
┌─────────────────────────────────────────────────────────────────────┐
│ BACKEND - CONTROLLER LAYER                                          │
├─────────────────────────────────────────────────────────────────────┤
│ 5. Controller Endpoint                                               │
│    File: /Backend/Controllers/[Module]Controller.cs                 │
│    Route: [Route("[api/controller]")]                              │
│    Method: [HttpGet/Post/Put/Delete("[action]")]                   │
│    Parameters: (type param1, type param2, ...)                     │
│    Authorization: [Authorize(Roles = "role1,role2")]               │
│    ↓                                                                 │
│ 6. Service Call                                                      │
│    Call: await _service.MethodName(params)                          │
│    Returns: DTO or ResponseDTO<T>                                   │
└─────────────────────────────────────────────────────────────────────┘
                              ↓ METHOD CALL
┌─────────────────────────────────────────────────────────────────────┐
│ BACKEND - SERVICE LAYER                                             │
├─────────────────────────────────────────────────────────────────────┤
│ 7. Service Interface                                                 │
│    File: /Backend/Services/I[Module]Service.cs                      │
│    Method Signature: Task<ReturnType> MethodName(params)            │
│    ↓                                                                 │
│ 8. Service Implementation                                            │
│    File: /Backend/Services/[Module]Service.cs                       │
│    Implements: I[Module]Service                                     │
│    ↓                                                                 │
│ 9. Stored Procedure Call                                             │
│    SP Name: sp_[Module]_[Action]                                    │
│    Parameters: @Param1, @Param2, ...                                │
│    ↓                                                                 │
│ 10. Data Mapping                                                     │
│     Maps SQL Result → DTO Object                                     │
│     Returns: DTO / List<DTO> / ResponseDTO<DTO>                     │
└─────────────────────────────────────────────────────────────────────┘
                              ↓ SQL EXECUTION
┌─────────────────────────────────────────────────────────────────────┐
│ BACKEND - DTO LAYER                                                 │
├─────────────────────────────────────────────────────────────────────┤
│ 11. DTO Definition                                                   │
│     File: /Backend/DTOs/[Module]Dtos.cs                            │
│     Class: [EntityName]Dto                                          │
│     Properties: { SchoolId, Name, Status, ... }                     │
│     Naming: PascalCase (SchoolId NOT KindergartenId)               │
└─────────────────────────────────────────────────────────────────────┘
                              ↓ DATA STRUCTURE
┌─────────────────────────────────────────────────────────────────────┐
│ DATABASE - TABLE LAYER                                              │
├─────────────────────────────────────────────────────────────────────┤
│ 12. Table Schema                                                     │
│     File: /files/database/database-schema.sql                       │
│     Table: [dbo].[TableName]                                        │
│     Columns: SchoolId, ChildId, Status, CreatedDate, ...           │
│     Primary Key: [PK_TableName]                                     │
│     Foreign Keys: [FK_Table_Reference]                              │
│     Constraints: CHECK, UNIQUE, DEFAULT                             │
└─────────────────────────────────────────────────────────────────────┘
                              ↓ QUERIES DATA
┌─────────────────────────────────────────────────────────────────────┐
│ DATABASE - STORED PROCEDURE LAYER                                   │
├─────────────────────────────────────────────────────────────────────┤
│ 13. Stored Procedure                                                 │
│     File: /files/database/phase[X]/02-[module]-storedprocs.sql     │
│     Name: sp_[Module]_[Action]                                      │
│     Parameters: @SchoolId INT, @Status VARCHAR(50), ...            │
│     Tables Referenced: [dbo].[Schools], [dbo].[Children], ...     │
│     Columns Selected: SchoolId, ChildId, FirstName, ...            │
│     Result Set: Returns data matching DTO structure                 │
└─────────────────────────────────────────────────────────────────────┘
```

---

## DETAILED AUDIT CHECKLIST

### ✅ 1. FRONTEND - ROUTES
- [ ] Route exists in `/utils/routes.tsx`
- [ ] Route path: `/[path]`
- [ ] Component import path correct
- [ ] Role permissions defined: `roles: ['role1', 'role2']`
- [ ] Route accessible in navigation menu

**Route Definition:**
```tsx
{
  path: '/[path]',
  element: <ComponentName />,
  roles: ['role1', 'role2'],
}
```

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 2. FRONTEND - COMPONENT DATA NEEDS

**Component File:** `/pages/[module]/[ComponentName].tsx`

**Data Requirements:**
| Data Item | Type | Source | Used For |
|-----------|------|--------|----------|
| [field1] | [type] | [API call] | [purpose] |
| [field2] | [type] | [API call] | [purpose] |

**State Management:**
- [ ] useState hooks for local state
- [ ] useEffect for data fetching
- [ ] Error handling implemented
- [ ] Loading states implemented

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 3. FRONTEND - API CALLS

**API Calls Made by Component:**

| API Function | File | Parameters | Expected Response | Used In Component |
|--------------|------|------------|-------------------|-------------------|
| [apiName].[method]() | /utils/api.ts | { param1, param2 } | { data: { ... } } | useEffect/handler |

**API Call Example:**
```typescript
// From component
const response = await apiName.methodName({ param1, param2 });
// Expected: { data: { field1, field2, ... } }
```

**Checks:**
- [ ] API function exists in `/utils/api.ts`
- [ ] Parameters match component usage
- [ ] Response structure matches component expectations
- [ ] Error handling present (try/catch)

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 4. FRONTEND - TYPE DEFINITIONS

**Types File:** `/types/[module].ts`

**Interfaces/Types Defined:**
```typescript
export interface [EntityName] {
  [field1]: [type];
  [field2]: [type];
  // ...
}
```

**Checks:**
- [ ] Type file exists
- [ ] All component props typed
- [ ] API response types defined
- [ ] Naming convention: camelCase (schoolId, NOT KindergartenId)
- [ ] Types match API response structure

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 5. API UTILITIES - ENDPOINT DEFINITION

**API File:** `/utils/api.ts` or `/utils/apiWithMock.ts`

**Endpoint Definition:**
```typescript
export const [moduleName]Api = {
  [methodName]: (params) => api.get('/api/[controller]/[action]', { params }),
};
```

**Checks:**
- [ ] Function exists in API utilities
- [ ] HTTP method correct (GET/POST/PUT/DELETE)
- [ ] URL path matches backend route
- [ ] Parameters passed correctly (query params vs body)
- [ ] Response handling correct
- [ ] Mock API version exists (if using apiWithMock.ts)

**URL Pattern:**
- Frontend calls: `/api/[controller]/[action]`
- Backend route: `[Route("api/[controller]")]` + `[HttpMethod("[action]")]`
- [ ] URLs match exactly

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 6. BACKEND - CONTROLLER

**Controller File:** `/Backend/Controllers/[Module]Controller.cs`

**Controller Structure:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class [Module]Controller : ControllerBase
{
    private readonly I[Module]Service _service;
    
    [HttpGet("[action]")]
    [Authorize(Roles = "role1,role2")]
    public async Task<IActionResult> [Action](params)
    {
        var result = await _service.Method(params);
        return Ok(result);
    }
}
```

**Checks:**
- [ ] Controller file exists
- [ ] `[Route("api/[controller]")]` matches frontend API call
- [ ] `[HttpMethod("[action]")]` matches frontend API call
- [ ] `[Authorize]` attribute present
- [ ] Role restrictions appropriate
- [ ] Parameters match frontend API call
- [ ] Service injected via constructor
- [ ] Service method called (NOT direct SQL)
- [ ] Returns proper status codes (Ok, BadRequest, NotFound, etc.)
- [ ] Returns DTO or ResponseDTO<T>

**Endpoint Signature:**
| Attribute | Value |
|-----------|-------|
| Route | api/[controller] |
| Action | [action] |
| HTTP Method | GET/POST/PUT/DELETE |
| Parameters | (type param1, type param2) |
| Return Type | Task<IActionResult> |
| Authorization | Roles = "role1,role2" |

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 7. BACKEND - SERVICE INTERFACE

**Service Interface File:** `/Backend/Services/I[Module]Service.cs`

**Interface Structure:**
```csharp
public interface I[Module]Service
{
    Task<[ReturnType]> [MethodName]([Params]);
}
```

**Checks:**
- [ ] Interface file exists
- [ ] Method signatures match controller calls
- [ ] Return types are DTOs (not raw database types)
- [ ] Parameters match controller parameters
- [ ] Async methods return Task<T>
- [ ] All DTOs used are defined

**Methods Defined:**
| Method Name | Return Type | Parameters | Called By Controller |
|-------------|-------------|------------|---------------------|
| [Method1] | Task<[Type]> | (params) | [ControllerAction] |

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 8. BACKEND - SERVICE IMPLEMENTATION

**Service Implementation File:** `/Backend/Services/[Module]Service.cs`

**Service Structure:**
```csharp
public class [Module]Service : I[Module]Service
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    
    public [Module]Service(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<[ReturnType]> [MethodName]([Params])
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var result = await connection.QueryAsync<[DTO]>(
                "sp_[Module]_[Action]",
                new { Param1, Param2 },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}
```

**Checks:**
- [ ] Service file exists
- [ ] Implements interface (: I[Module]Service)
- [ ] All interface methods implemented
- [ ] Uses Dapper for SQL calls
- [ ] Calls stored procedures (NOT inline SQL)
- [ ] SP names match database SPs
- [ ] Parameters match SP parameters
- [ ] Maps result to DTOs
- [ ] Proper error handling (try/catch)
- [ ] Connection string from configuration

**Stored Procedure Calls:**
| Service Method | SP Name | Parameters | Returns |
|----------------|---------|------------|---------|
| [Method1] | sp_[Module]_[Action] | @Param1, @Param2 | [DTO] |

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 9. BACKEND - SERVICE REGISTRATION

**File:** `/Backend/Program.cs`

**Registration:**
```csharp
builder.Services.AddScoped<I[Module]Service, [Module]Service>();
```

**Checks:**
- [ ] Service registered in Program.cs
- [ ] Uses AddScoped (per-request lifetime)
- [ ] Interface and implementation mapped correctly

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 10. BACKEND - DTO DEFINITIONS

**DTO File:** `/Backend/DTOs/[Module]Dtos.cs`

**DTO Structure:**
```csharp
public class [Entity]Dto
{
    public int [Entity]Id { get; set; }
    public int SchoolId { get; set; }  // NOT KindergartenId
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

**Checks:**
- [ ] DTO file exists in `/Backend/DTOs/` folder
- [ ] DTO class names match usage in services
- [ ] Properties use PascalCase
- [ ] Property names match database columns EXACTLY
- [ ] Uses `SchoolId` (NOT KindergartenId)
- [ ] Data types match database column types:
  - SQL `INT` → C# `int`
  - SQL `VARCHAR/NVARCHAR` → C# `string`
  - SQL `DATETIME` → C# `DateTime`
  - SQL `BIT` → C# `bool`
  - SQL `DECIMAL` → C# `decimal`
- [ ] Nullable types for nullable database columns (`int?`, `DateTime?`)
- [ ] All DTOs used by service exist

**DTOs Defined:**
| DTO Name | Properties | Used By Service Method |
|----------|------------|------------------------|
| [Dto1] | { prop1, prop2 } | [Method1] |

**Property Mapping (DTO ↔ Database):**
| DTO Property | C# Type | Database Column | SQL Type | Nullable |
|--------------|---------|-----------------|----------|----------|
| [Property1] | [type] | [ColumnName] | [sqltype] | Yes/No |

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 11. DATABASE - TABLE SCHEMA

**Schema File:** `/files/database/database-schema.sql`

**Table Structure:**
```sql
CREATE TABLE [dbo].[TableName] (
    [PrimaryKeyId] INT IDENTITY(1,1) PRIMARY KEY,
    [SchoolId] INT NOT NULL,
    [ColumnName] VARCHAR(100) NOT NULL,
    [CreatedDate] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK_Table_School] FOREIGN KEY ([SchoolId]) 
        REFERENCES [dbo].[Schools]([SchoolId])
);
```

**Checks:**
- [ ] Table exists in database-schema.sql
- [ ] Table name correct (plural: Schools, Children, Classrooms)
- [ ] Primary key defined
- [ ] Columns match DTO properties
- [ ] Column names use PascalCase
- [ ] Uses `SchoolId` (NOT KindergartenId)
- [ ] Foreign keys reference correct tables:
  - [ ] `[dbo].[Schools]` (NOT [dbo].[Kindergarten])
  - [ ] `[dbo].[Users]` (NOT [dbo].[User])
  - [ ] `[dbo].[Classrooms]` (NOT [dbo].[Classroom])
  - [ ] `[dbo].[Children]` (NOT [dbo].[Child])
- [ ] Data types match DTO types
- [ ] NOT NULL constraints appropriate
- [ ] DEFAULT values appropriate
- [ ] CHECK constraints for enums/statuses
- [ ] Indexes for performance

**Table Definition:**
| Column Name | Data Type | Nullable | Default | Constraint | FK Reference |
|-------------|-----------|----------|---------|------------|--------------|
| [Column1] | [type] | Yes/No | [value] | PK/FK/CHECK | [Table]([Column]) |

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 12. DATABASE - STORED PROCEDURES

**SP File:** `/files/database/phase[X]/02-[module]-storedprocs.sql`

**SP Structure:**
```sql
CREATE PROCEDURE [dbo].[sp_Module_Action]
    @SchoolId INT,
    @Param1 VARCHAR(100),
    @Param2 INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.[PrimaryKeyId],
        t.[SchoolId],
        t.[ColumnName],
        t.[CreatedDate]
    FROM [dbo].[TableName] t
    WHERE t.[SchoolId] = @SchoolId
        AND t.[Status] = 'Active';
END
```

**Checks:**
- [ ] SP file exists in `/files/database/phase[X]/` folder
- [ ] SP name matches service call
- [ ] SP name follows convention: `sp_[Module]_[Action]`
- [ ] Parameters match service call
- [ ] Parameter names use PascalCase with @ prefix
- [ ] Uses `@SchoolId` (NOT @KindergartenId)
- [ ] Table references correct:
  - [ ] `[dbo].[Schools]` (NOT [dbo].[Kindergarten])
  - [ ] `[dbo].[Users]` (NOT [dbo].[User])
  - [ ] Proper table aliases used
- [ ] Column names match table schema
- [ ] SELECT columns match DTO properties
- [ ] SET NOCOUNT ON present
- [ ] Proper error handling (TRY/CATCH if needed)
- [ ] Returns result set matching DTO structure

**Stored Procedures:**
| SP Name | Parameters | Tables Used | Returns | Called By Service |
|---------|------------|-------------|---------|-------------------|
| sp_[Module]_[Action] | @Param1, @Param2 | [Table1], [Table2] | [Columns] | [ServiceMethod] |

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 13. CROSS-LAYER VALIDATION

**Data Flow Validation:**

#### Component → API → Controller
- [ ] Component calls: `[apiName].[method]({ param1, param2 })`
- [ ] API calls: `GET /api/[controller]/[action]`
- [ ] Controller route: `[Route("api/[controller]")] + [HttpGet("[action]")]`
- [ ] Parameters match across all layers

#### Controller → Service → DTO
- [ ] Controller calls: `await _service.Method(param1, param2)`
- [ ] Service method exists: `Task<[DTO]> Method(param1, param2)`
- [ ] Service returns DTO: `[EntityName]Dto`
- [ ] Controller returns DTO in response

#### Service → SP → Table
- [ ] Service calls: `sp_[Module]_[Action]`
- [ ] SP exists with exact name
- [ ] SP parameters: `@Param1, @Param2`
- [ ] SP queries table: `[dbo].[TableName]`
- [ ] SP SELECT columns match DTO properties

#### DTO → Table Mapping
- [ ] DTO property: `public int SchoolId { get; set; }`
- [ ] Table column: `[SchoolId] INT NOT NULL`
- [ ] Types compatible: `int` ↔ `INT`
- [ ] Names match exactly (case-sensitive)

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 14. NAMING CONVENTION AUDIT

**Critical Checks:**
- [ ] Frontend uses `schoolId` (camelCase)
- [ ] Backend DTOs use `SchoolId` (PascalCase)
- [ ] Database columns use `SchoolId` (PascalCase)
- [ ] SP parameters use `@SchoolId` (PascalCase with @)
- [ ] NO references to `KindergartenId` anywhere
- [ ] NO references to `[dbo].[Kindergarten]` table
- [ ] FK references use `[dbo].[Schools]` (NOT [dbo].[Kindergarten])
- [ ] FK references use `[dbo].[Users]` (NOT [dbo].[User])
- [ ] FK references use `[dbo].[Classrooms]` (NOT [dbo].[Classroom])
- [ ] FK references use `[dbo].[Children]` (NOT [dbo].[Child])

**Search Results:**
```bash
# Run these searches to verify:
grep -r "KindergartenId" /pages /types /utils  # Should be 0 results
grep -r "kindergartenId" /pages /types /utils  # Should be 0 results
grep -r "KindergartenId" /Backend              # Should be 0 results
grep -r "Kindergarten\]" /files/database       # Should be 0 results (except in comments)
```

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 15. AUTHORIZATION & SECURITY

**Role-Based Access:**
- [ ] Controller has `[Authorize]` attribute
- [ ] Specific roles defined: `[Authorize(Roles = "role1,role2")]`
- [ ] Frontend route has role restrictions
- [ ] Navigation menu respects roles
- [ ] Appropriate roles for actions:
  - SuperAdmin: Full access
  - SchoolAdmin: School-level management
  - Teacher: Classroom operations
  - Parent: Read-only child data

**Data Isolation:**
- [ ] Queries filter by SchoolId for multi-tenant isolation
- [ ] Parents can only see their own children
- [ ] Teachers can only see their classrooms
- [ ] SchoolAdmins can only see their school data

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

### ✅ 16. ERROR HANDLING

**Frontend:**
- [ ] API calls wrapped in try/catch
- [ ] Error messages displayed to user (toast/alert)
- [ ] Loading states during async operations
- [ ] Graceful degradation on errors

**Backend:**
- [ ] Controller validates input
- [ ] Service handles SQL exceptions
- [ ] Appropriate HTTP status codes:
  - 200 OK: Success
  - 400 Bad Request: Invalid input
  - 401 Unauthorized: Not authenticated
  - 403 Forbidden: Not authorized
  - 404 Not Found: Resource not found
  - 500 Internal Server Error: Unexpected errors

**Issues Found:**
- [ ] None
- [ ] Issue: [Description]

---

## SUMMARY

### MODULE STATUS: [PASS/FAIL/NEEDS FIXES]

### LAYERS VALIDATED:
- [ ] ✅ Frontend Routes
- [ ] ✅ Frontend Components
- [ ] ✅ Frontend Types
- [ ] ✅ API Utilities
- [ ] ✅ Backend Controller
- [ ] ✅ Backend Service Interface
- [ ] ✅ Backend Service Implementation
- [ ] ✅ Backend DTOs
- [ ] ✅ Database Tables
- [ ] ✅ Stored Procedures

### CRITICAL ISSUES (Blocking):
1. [Issue description]

### WARNINGS (Non-blocking):
1. [Issue description]

### RECOMMENDATIONS:
1. [Recommendation]

---

## NEXT STEPS

**If PASS:**
- [x] Module audit complete
- [ ] Move to next module

**If FAIL:**
- [ ] Fix critical issues listed above
- [ ] Re-run audit
- [ ] Document fixes applied

---

## AUDIT NOTES

**Additional Observations:**
- [Any other notes or observations]

**Performance Considerations:**
- [Database indexing, query optimization, etc.]

**Future Enhancements:**
- [Suggestions for future improvements]

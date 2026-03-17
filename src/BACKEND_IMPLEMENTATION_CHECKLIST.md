# Backend Module Implementation Checklist

## 📋 **Quick Reference: Complete Module Implementation**

This is your **GO-TO** checklist for implementing ANY backend module. Print this and check off each item!

---

## 🎯 **Phase 1: Analysis & Planning**

### Step 1: Analyze Frontend Requirements
- [ ] Review frontend TypeScript DTOs in `/types/[module].ts`
- [ ] Review frontend API calls in `/utils/[module]Api.ts`
- [ ] List all API endpoints needed
- [ ] List all CRUD operations needed
- [ ] List all custom actions needed (submit, approve, reject, etc.)
- [ ] Document all DTO fields with data types
- [ ] Identify computed/derived fields
- [ ] Identify related entities (JOINs needed)

### Step 2: Design Database Schema
- [ ] Design main table(s)
- [ ] Design lookup tables (if needed)
- [ ] Design junction tables for many-to-many (if needed)
- [ ] Plan foreign key relationships
- [ ] Plan indexes for performance
- [ ] Document soft delete strategy

---

## 🗄️ **Phase 2: Database Layer**

### Step 1: Create SQL Tables
- [ ] Main table with all required columns
- [ ] Primary key (INT IDENTITY)
- [ ] KindergartenId foreign key (multi-tenant)
- [ ] All business fields
- [ ] Status field (if applicable)
- [ ] Audit fields: CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId
- [ ] Soft delete fields: IsDeleted, DeletedAt, DeletedByUserId
- [ ] Foreign key constraints
- [ ] Index on KindergartenId
- [ ] Indexes on commonly queried fields
- [ ] Indexes on foreign keys
- [ ] Test table creation script

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 2

### Step 2: Create Stored Procedures
- [ ] `sp_[Module]_Create` - INSERT with validation
- [ ] `sp_[Module]_GetById` - SELECT with JOINs for names
- [ ] `sp_[Module]_GetAll` - SELECT with filters
- [ ] `sp_[Module]_GetPaginated` - SELECT with pagination
- [ ] `sp_[Module]_Update` - UPDATE with validation
- [ ] `sp_[Module]_Delete` - Soft delete
- [ ] Custom SPs (e.g., Submit, Approve, Reject)
- [ ] All SPs have BEGIN TRANSACTION
- [ ] All SPs have TRY/CATCH error handling
- [ ] All SPs validate KindergartenId
- [ ] All SPs include audit logging
- [ ] All SPs return full object after write operations
- [ ] Test all stored procedures

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 3

---

## 📦 **Phase 3: Data Access Layer**

### Step 1: Create Repository Interface
**Location:** `/KindergartenManagement.Data/Interfaces/I[Module]Repository.cs`

- [ ] Interface created
- [ ] CreateAsync method
- [ ] GetByIdAsync method
- [ ] GetAllAsync method (with filters)
- [ ] GetPaginatedAsync method
- [ ] UpdateAsync method
- [ ] DeleteAsync method (soft delete)
- [ ] ExistsAsync method
- [ ] Custom methods (SubmitAsync, ApproveAsync, etc.)

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1B

### Step 2: Create Repository Implementation
**Location:** `/KindergartenManagement.Data/Repositories/[Module]Repository.cs`

- [ ] Implementation class created
- [ ] Connection string injected from IConfiguration
- [ ] All interface methods implemented using Dapper
- [ ] All methods call correct stored procedures
- [ ] Parameters mapped correctly
- [ ] Multi-result queries handled (pagination)
- [ ] Exception handling for SQL errors
- [ ] Using statements for SqlConnection
- [ ] Test all repository methods

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1B

---

## 💼 **Phase 4: Business Logic Layer**

### Step 1: Create DTOs
**Location:** `/KindergartenManagement.Core/DTOs/`

- [ ] **CreateDto** - For POST requests
  - [ ] All required fields
  - [ ] Data Annotations or FluentValidation
  - [ ] No computed fields
- [ ] **UpdateDto** - For PUT requests
  - [ ] Id field included
  - [ ] All updateable fields
  - [ ] Validation rules
- [ ] **ResponseDto (full)** - For GET by ID
  - [ ] All table fields
  - [ ] Computed fields (counts, etc.)
  - [ ] Related entity names (JOINed)
  - [ ] Created/Updated by names
- [ ] **ListDto (lighter)** - For GET all/paginated
  - [ ] Essential fields only
  - [ ] Optimized for list views
- [ ] **FilterDto** - Query parameters
  - [ ] Status filter
  - [ ] Date range filters
  - [ ] Search term
  - [ ] Sort by/order

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1C

### Step 2: Create FluentValidation Validators
**Location:** `/KindergartenManagement.Core/Validators/`

- [ ] `Create[Module]DtoValidator` created
- [ ] `Update[Module]DtoValidator` created
- [ ] Required fields validated
- [ ] String length limits enforced
- [ ] Email format validated (if applicable)
- [ ] Date range validated (if applicable)
- [ ] Custom business rules validated
- [ ] Error messages clear and user-friendly

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1C

### Step 3: Create AutoMapper Profile
**Location:** `/KindergartenManagement.Core/Mappings/[Module]Profile.cs`

- [ ] Profile class created
- [ ] Entity → ResponseDto mapping
- [ ] Entity → ListDto mapping
- [ ] CreateDto → Entity mapping
- [ ] UpdateDto → Entity mapping (null values ignored)
- [ ] Custom mappings for computed fields
- [ ] Test all mappings

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1D

### Step 4: Create Service Interface
**Location:** `/KindergartenManagement.Core/Interfaces/I[Module]Service.cs`

- [ ] Interface created
- [ ] CreateAsync method
- [ ] GetByIdAsync method
- [ ] GetAllAsync method
- [ ] GetPaginatedAsync method
- [ ] UpdateAsync method
- [ ] DeleteAsync method
- [ ] ExistsAsync method
- [ ] Custom action methods (Submit, Approve, etc.)
- [ ] All methods return Task<T>
- [ ] All methods include kindergartenId parameter
- [ ] All write methods include userId parameter

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1A

### Step 5: Create Service Implementation
**Location:** `/KindergartenManagement.Core/Services/[Module]Service.cs`

- [ ] Service class created
- [ ] Dependencies injected:
  - [ ] ILogger<T>
  - [ ] I[Module]Repository
  - [ ] IMapper
  - [ ] ICacheService
  - [ ] IAuditService
- [ ] All interface methods implemented
- [ ] Business logic validation in each method
- [ ] Cache checking before database calls (GET operations)
- [ ] Cache invalidation after writes (CREATE/UPDATE/DELETE)
- [ ] Audit logging after write operations
- [ ] Structured logging throughout
- [ ] Exception handling with logging
- [ ] Multi-tenant validation
- [ ] Test all service methods

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 1A

---

## 🌐 **Phase 5: API Layer**

### Step 1: Create Controller
**Location:** `/KindergartenManagement.API/Controllers/[Module]Controller.cs`

- [ ] Controller class created
- [ ] `[ApiController]` attribute
- [ ] `[Route("api/[controller]")]` attribute
- [ ] `[Authorize]` attribute for JWT auth
- [ ] Dependencies injected:
  - [ ] ILogger<T>
  - [ ] I[Module]Service
  - [ ] IMapper (if needed)
- [ ] Helper method to extract KindergartenId from claims
- [ ] Helper method to extract UserId from claims

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 1

### Step 2: Implement CRUD Endpoints

**CREATE Endpoint:**
- [ ] `[HttpPost]` attribute
- [ ] `[ProducesResponseType(typeof(ResponseDto), 201)]`
- [ ] `[ProducesResponseType(400)]` for validation errors
- [ ] Accepts CreateDto from body
- [ ] Extracts kindergartenId from claims
- [ ] Extracts userId from claims
- [ ] Calls service.CreateAsync
- [ ] Returns CreatedAtAction with location header
- [ ] Exception handling with appropriate status codes
- [ ] Logging

**READ BY ID Endpoint:**
- [ ] `[HttpGet("{id}")]` attribute
- [ ] `[ProducesResponseType(typeof(ResponseDto), 200)]`
- [ ] `[ProducesResponseType(404)]`
- [ ] Accepts id from route
- [ ] Accepts kindergartenId from query string
- [ ] Calls service.GetByIdAsync
- [ ] Returns 404 if not found
- [ ] Returns 200 with data if found
- [ ] Logging

**READ ALL Endpoint:**
- [ ] `[HttpGet]` attribute
- [ ] `[ProducesResponseType(typeof(List<ListDto>), 200)]`
- [ ] Accepts kindergartenId from query string
- [ ] Accepts FilterDto from query string
- [ ] Calls service.GetAllAsync with filters
- [ ] Returns 200 with list (even if empty)
- [ ] Logging

**READ PAGINATED Endpoint:**
- [ ] `[HttpGet("paginated")]` attribute
- [ ] `[ProducesResponseType(typeof(PaginatedResult<ListDto>), 200)]`
- [ ] Accepts page, pageSize, kindergartenId from query
- [ ] Accepts FilterDto from query
- [ ] Calls service.GetPaginatedAsync
- [ ] Returns paginated result with metadata
- [ ] Logging

**UPDATE Endpoint:**
- [ ] `[HttpPut("{id}")]` attribute
- [ ] `[ProducesResponseType(typeof(ResponseDto), 200)]`
- [ ] `[ProducesResponseType(404)]`
- [ ] `[ProducesResponseType(400)]`
- [ ] Accepts id from route
- [ ] Accepts UpdateDto from body
- [ ] Extracts kindergartenId from claims
- [ ] Extracts userId from claims
- [ ] Calls service.UpdateAsync
- [ ] Returns 404 if not found
- [ ] Returns 200 with updated data
- [ ] Logging

**DELETE Endpoint:**
- [ ] `[HttpDelete("{id}")]` attribute
- [ ] `[ProducesResponseType(204)]`
- [ ] `[ProducesResponseType(404)]`
- [ ] Accepts id from route
- [ ] Accepts kindergartenId from query string
- [ ] Extracts userId from claims
- [ ] Calls service.DeleteAsync
- [ ] Returns 404 if not found
- [ ] Returns 204 No Content if successful
- [ ] Logging

### Step 3: Implement Custom Action Endpoints

**For each custom action (e.g., Submit, Approve, Reject):**
- [ ] Appropriate HTTP verb (`[HttpPost]` or `[HttpPut]`)
- [ ] Route attribute (e.g., `[HttpPost("{id}/submit")]`)
- [ ] ProducesResponseType attributes
- [ ] Extract parameters from route/query/body
- [ ] Extract kindergartenId and userId from claims
- [ ] Call appropriate service method
- [ ] Return appropriate status code
- [ ] Exception handling
- [ ] Logging

### Step 4: Add XML Documentation Comments

**For the controller:**
```csharp
/// <summary>
/// Manages [Module] operations
/// </summary>
[ApiController]
public class ModuleController : ControllerBase
```

**For each endpoint:**
```csharp
/// <summary>
/// Creates a new [module item]
/// </summary>
/// <param name="dto">The creation data</param>
/// <returns>The created [module item]</returns>
/// <response code="201">Successfully created</response>
/// <response code="400">Invalid input</response>
[HttpPost]
```

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 1

---

## ⚙️ **Phase 6: Configuration & Registration**

### Step 1: Register Services in DI
**Location:** `/KindergartenManagement.API/Extensions/ServiceExtensions.cs`

- [ ] Service interface and implementation registered:
  ```csharp
  services.AddScoped<I[Module]Service, [Module]Service>();
  ```
- [ ] Repository interface and implementation registered:
  ```csharp
  services.AddScoped<I[Module]Repository, [Module]Repository>();
  ```

**Template Location:** See `/BACKEND_COMPLETE_ARCHITECTURE.md` Step 6

### Step 2: Register FluentValidation Validators
**Location:** `/KindergartenManagement.API/Program.cs`

- [ ] Validators registered from assembly:
  ```csharp
  builder.Services.AddValidatorsFromAssemblyContaining<Create[Module]DtoValidator>();
  ```
- [ ] Auto-validation enabled:
  ```csharp
  builder.Services.AddFluentValidationAutoValidation();
  ```

### Step 3: Register AutoMapper Profiles
**Location:** `/KindergartenManagement.API/Program.cs`

- [ ] AutoMapper registered:
  ```csharp
  builder.Services.AddAutoMapper(typeof([Module]Profile).Assembly);
  ```

---

## 🔍 **Phase 7: Verification & Testing**

### Step 1: Verify Endpoint URLs Match Frontend
- [ ] Open `/utils/[module]Api.ts`
- [ ] List all frontend API calls
- [ ] For each call, verify:
  - [ ] HTTP method matches
  - [ ] URL path matches
  - [ ] Route parameters match
  - [ ] Query parameters match
- [ ] Document any mismatches
- [ ] Update backend OR frontend to match

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 4

### Step 2: Verify Field Matching

**Create a spreadsheet:**

| Field Name | Frontend DTO | Backend DTO | DB Table | Stored Proc | Match? |
|------------|-------------|-------------|----------|-------------|--------|
| [field1]   | ✅ type     | ✅ type     | ✅ type  | ✅ Selected | ✅     |
| [field2]   | ✅ type     | ✅ type     | ✅ type  | ✅ Selected | ✅     |

- [ ] All frontend DTO fields verified
- [ ] All backend DTO fields verified
- [ ] All table columns verified
- [ ] All SP SELECTs include required fields
- [ ] Data types compatible across layers
- [ ] Nullable consistency verified
- [ ] Computed fields have SQL logic
- [ ] Related entity names JOINed in SPs

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 5

### Step 3: Update Backend to Match Frontend
- [ ] Add any missing computed fields to SPs
- [ ] Add any missing JOINs for related names
- [ ] Add any missing fields to backend DTOs
- [ ] Update AutoMapper profiles for new fields
- [ ] Verify all changes tested

**Template Location:** See `/BACKEND_IMPLEMENTATION_STANDARDS.md` Step 6

---

## 🧪 **Phase 8: Testing**

### Step 1: Unit Tests for Repository
**Location:** `/KindergartenManagement.Tests/Unit/Repositories/`

- [ ] Test CreateAsync
- [ ] Test GetByIdAsync (found)
- [ ] Test GetByIdAsync (not found)
- [ ] Test GetAllAsync (with filters)
- [ ] Test UpdateAsync
- [ ] Test DeleteAsync
- [ ] Test ExistsAsync
- [ ] Test SQL exceptions handled
- [ ] Test multi-tenant isolation

### Step 2: Unit Tests for Service
**Location:** `/KindergartenManagement.Tests/Unit/Services/`

- [ ] Test CreateAsync (success)
- [ ] Test CreateAsync (validation failure)
- [ ] Test GetByIdAsync (from cache)
- [ ] Test GetByIdAsync (from database)
- [ ] Test UpdateAsync (success)
- [ ] Test UpdateAsync (not found)
- [ ] Test DeleteAsync
- [ ] Test business logic validation
- [ ] Test caching behavior
- [ ] Test audit logging called

### Step 3: Integration Tests for API
**Location:** `/KindergartenManagement.Tests/Integration/Controllers/`

- [ ] Test POST endpoint (success)
- [ ] Test POST endpoint (validation error)
- [ ] Test POST endpoint (unauthorized)
- [ ] Test GET by ID endpoint (found)
- [ ] Test GET by ID endpoint (not found)
- [ ] Test GET all endpoint
- [ ] Test PUT endpoint (success)
- [ ] Test PUT endpoint (not found)
- [ ] Test DELETE endpoint (success)
- [ ] Test DELETE endpoint (not found)
- [ ] Test custom action endpoints
- [ ] Test multi-tenant isolation
- [ ] Test JWT authentication

### Step 4: Create Postman Collection
- [ ] Collection created for module
- [ ] Environment variables configured
- [ ] Authentication token setup
- [ ] All endpoints added
- [ ] Request examples provided
- [ ] Test scripts added
- [ ] Collection exported and saved

### Step 5: Frontend Integration Testing
- [ ] Switch frontend to use real API (`USE_MOCK_API = false`)
- [ ] Update `API_BASE_URL` to backend URL
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
- [ ] Swagger UI tested and working
- [ ] Example requests/responses in Swagger
- [ ] Authentication documented in Swagger

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
- [ ] JWT authentication enforced on all endpoints
- [ ] KindergartenId validated from JWT claims
- [ ] Multi-tenant isolation working (users can't access other kindergartens)
- [ ] Input validation working (FluentValidation)
- [ ] SQL injection prevention (parameterized queries)
- [ ] Soft delete implemented (no hard deletes)
- [ ] Authorization policies applied where needed

### Performance Checklist
- [ ] Caching implemented for GET operations
- [ ] Database indexes created on frequently queried columns
- [ ] Pagination implemented for large lists
- [ ] N+1 query problems avoided (proper JOINs in SPs)
- [ ] Connection pooling configured

### Operations Checklist
- [ ] Structured logging throughout
- [ ] Exception handling middleware catching errors
- [ ] Audit logging working
- [ ] Health checks passing
- [ ] Swagger documentation accessible

### Code Quality Checklist
- [ ] Code follows naming conventions
- [ ] No code duplication
- [ ] All async methods properly awaited
- [ ] Using statements for disposable resources
- [ ] Constants used instead of magic strings/numbers
- [ ] Error messages clear and helpful

---

## 🎯 **Module Status Tracking**

| Module | Phase 2 | Phase 3 | Phase 4 | Phase 5 | Phase 6 | Phase 7 | Phase 8 | Phase 9 | Complete |
|--------|---------|---------|---------|---------|---------|---------|---------|---------|----------|
| **Lesson Plans** | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Activity Logs** | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Meal Plans | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Enrollments | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Attendance | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Health Records | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Incidents | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Communications | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Assessments | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Staff | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Classrooms | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Children | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

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

- **Database & Stored Procedures:** `/BACKEND_IMPLEMENTATION_STANDARDS.md`
- **Complete Architecture:** `/BACKEND_COMPLETE_ARCHITECTURE.md`
- **Frontend Standards:** `/guidelines/Guidelines.md`
- **AlertDialog Standards:** `/ALERTDIALOG_STANDARDS_AUDIT.md`
- **API Status:** `/BACKEND_API_STATUS.md`

---

## ✅ **Definition of "Complete"**

A module is **ONLY** considered complete when:

1. ✅ All 10 phases completed
2. ✅ All checklist items verified
3. ✅ All tests passing (unit + integration)
4. ✅ Frontend integration working
5. ✅ Documentation complete
6. ✅ Code reviewed
7. ✅ Security verified
8. ✅ Performance verified

**No exceptions. No shortcuts.** 🎯

This ensures a **fully operational, production-ready application**.

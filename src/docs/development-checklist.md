# What's New Application - Development Checklist

## Purpose

This file serves as a comprehensive quality assurance checklist for all new features and functionality. Use this to ensure consistency, prevent errors, and maintain architectural patterns across the entire application.

---

## 📋 Table of Contents

1. [Backend Implementation Checklist](#backend-implementation-checklist)
2. [Frontend Implementation Checklist](#frontend-implementation-checklist)
3. [Database Design Checklist](#database-design-checklist)
4. [API Integration Checklist](#api-integration-checklist)
5. [Common Errors & Solutions](#common-errors--solutions)
6. [Code Review Checklist](#code-review-checklist)

---

## 🔧 Backend Implementation Checklist

### .NET Core Structure

- [ ] **Controllers** placed in `/Backend/WhatsNewAPI/Controllers` folder
  - [ ] Inherits from `ControllerBase`
  - [ ] Uses `[ApiController]` attribute
  - [ ] Uses `[Route("api/[controller]")]` attribute
  - [ ] Uses `[Authorize]` attribute with appropriate roles
  - [ ] All endpoints return `IActionResult` or `ActionResult<T>`
  - [ ] Proper HTTP method attributes (`[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`)
  - [ ] Route parameters use `[FromRoute]`, body parameters use `[FromBody]`, query parameters use `[FromQuery]`
  - [ ] Proper error handling with try-catch blocks
  - [ ] Returns appropriate status codes (200, 201, 400, 404, 500)

- [ ] **Services** placed in `/Backend/WhatsNewAPI/Services` folder
  - [ ] Interface and implementation pattern (e.g., `IReleaseService` and `ReleaseService`)
  - [ ] Service registered in `Program.cs` with appropriate lifetime (Scoped, Transient, Singleton)
  - [ ] Uses dependency injection for repositories
  - [ ] All methods have clear return types
  - [ ] Business logic separated from controller logic
  - [ ] Uses async/await pattern with `Task<T>` return types

- [ ] **Repositories** placed in `/Backend/WhatsNewAPI/Repositories` folder
  - [ ] Interface and implementation pattern (e.g., `IReleaseRepository` and `ReleaseRepository`)
  - [ ] All database operations use Dapper
  - [ ] All stored procedure calls use parameterized queries
  - [ ] Proper connection management with `using` statements

- [ ] **DTOs** (Data Transfer Objects) placed in `/Backend/WhatsNewAPI/DTOs` folder
  - [ ] Clear naming convention: `[Entity][Purpose]Dto` (e.g., `ReleaseDto`, `CreateReleaseDto`, `UpdateReleaseDto`)
  - [ ] Separate DTOs for different operations:
    - `Create[Entity]Dto` - for creation (no ID)
    - `Update[Entity]Dto` - for updates (includes ID)
    - `[Entity]Dto` - for responses
  - [ ] Use data annotations for validation:
    - `[Required]`
    - `[StringLength(max, MinimumLength = min)]`
    - `[EmailAddress]`
    - `[Range(min, max)]`
  - [ ] Property names match frontend TypeScript interfaces
  - [ ] Use proper data types (DateTime, Guid, int, string, etc.)

### Database Integration

- [ ] **Stored Procedures** called correctly
  - [ ] Use parameterized queries (prevents SQL injection)
  - [ ] Parameters match stored procedure signature exactly
  - [ ] Parameter names use `@` prefix (e.g., `@ReleaseId`, `@Version`)
  - [ ] Output parameters handled correctly
  - [ ] Result sets mapped to DTOs properly
  - [ ] Connection properly disposed (use `using` statements)

---

## 🗄️ Database Design Checklist

### Table Design

- [ ] **Naming Conventions**
  - [ ] Table names are PascalCase and singular (e.g., `Release`, `Change`, `Tag`, `Client`)
  - [ ] Column names are PascalCase (e.g., `Version`, `ReleaseDate`, `CreatedAt`)
  - [ ] Primary keys named `[TableName]Id` (e.g., `ReleaseId`, `ChangeId`, `TagId`, `ClientId`)
  - [ ] Foreign keys named `[ReferencedTable]Id` (e.g., `ReleaseId`, `ClientId`)

- [ ] **Standard Fields** (include where appropriate)
  - [ ] `[Entity]Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID()` - Primary key
  - [ ] `CreatedAt DATETIME2 DEFAULT GETUTCDATE()` - Audit trail
  - [ ] `UpdatedAt DATETIME2 DEFAULT GETUTCDATE()` - Audit trail
  - [ ] `IsActive BIT DEFAULT 1` - Status flag (for Clients, Tags, etc.)

- [ ] **Status Fields**
  - [ ] Use `NVARCHAR(50)` for status fields
  - [ ] Define clear status values (e.g., 'bug-fix', 'new-feature', 'enhancement')
  - [ ] Add CHECK constraints for status values where possible

- [ ] **Foreign Keys**
  - [ ] All foreign keys have proper FOREIGN KEY constraints
  - [ ] Cascade behavior defined (ON DELETE, ON UPDATE)
  - [ ] Consider using `ON DELETE NO ACTION` for important references
  - [ ] Use `ON DELETE CASCADE` for junction tables (e.g., ChangeTags)

- [ ] **Indexes**
  - [ ] Primary key automatically indexed
  - [ ] Foreign keys indexed for query performance
  - [ ] Common query fields indexed (e.g., Version, Code, ReleaseDate)
  - [ ] Unique constraints where needed (e.g., UNIQUE on Version, Code)

### Stored Procedure Design

- [ ] **Naming Conventions**
  - [ ] Format: `sp_[Module]_[Action]`
  - [ ] Examples: `sp_Releases_GetAll`, `sp_Releases_Create`, `sp_Changes_GetByRelease`
  - [ ] Use descriptive action verbs: Get, Create, Update, Delete, List, Search

- [ ] **Parameter Naming**
  - [ ] All parameters start with `@` (e.g., `@ReleaseId`, `@Version`)
  - [ ] Match table column names for clarity
  - [ ] Use `@UserId` for the current user (for audit trails)
  - [ ] Use `@SearchTerm` for search functionality

- [ ] **Standard Patterns**
  - [ ] Include `SET NOCOUNT ON;` at the beginning
  - [ ] Use `BEGIN TRY ... END TRY BEGIN CATCH ... END CATCH` for error handling
  - [ ] Return result sets, not scalar values (unless COUNT or EXISTS check)
  - [ ] For updates: Set `UpdatedAt = GETUTCDATE()`
  - [ ] JOIN related tables for comprehensive results (e.g., Changes with Tags)

- [ ] **CRUD Operations**
  - [ ] **Create**: Return the newly created record with generated ID
  - [ ] **Read (Get)**: Return single record or NULL if not found
  - [ ] **Read (List)**: Return all records matching criteria
  - [ ] **Update**: Return updated record or indicate success/failure
  - [ ] **Delete**: Hard delete with CASCADE consideration

- [ ] **Common Stored Procedures per Module**
  - [ ] `sp_[Module]_Create` - Create new record
  - [ ] `sp_[Module]_GetById` - Get single record by ID
  - [ ] `sp_[Module]_GetAll` - List all records
  - [ ] `sp_[Module]_Update` - Update existing record
  - [ ] `sp_[Module]_Delete` - Delete record
  - [ ] `sp_[Module]_Search` - Search with filters

### Example Table Structure

```sql
CREATE TABLE Releases (
    ReleaseId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Version NVARCHAR(50) NOT NULL UNIQUE,
    ReleaseDate DATE NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

CREATE INDEX IX_Releases_ReleaseDate ON Releases(ReleaseDate);
CREATE INDEX IX_Releases_Version ON Releases(Version);
```

### Example Stored Procedure

```sql
CREATE PROCEDURE sp_Releases_GetById
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            r.ReleaseId,
            r.Version,
            r.ReleaseDate,
            r.CreatedAt,
            r.UpdatedAt
        FROM Releases r
        WHERE r.ReleaseId = @ReleaseId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

## 🎨 Frontend Implementation Checklist

### Project Structure

- [ ] **Components** organized by feature/module
  - [ ] Place in `/components` folder (e.g., `WhatsNew.tsx`, `ReleaseManagement.tsx`)
  - [ ] Use PascalCase for component files
  - [ ] Export components using named exports: `export function ComponentName() { ... }`

- [ ] **Services** in `/services` folder
  - [ ] `api.ts` - Real API integration with mock data fallback

- [ ] **Hooks** in `/hooks` folder
  - [ ] Custom hooks for data fetching (e.g., `useReleases.ts`, `useChanges.ts`, `useTags.ts`)
  - [ ] Reusable logic extracted into custom hooks

- [ ] **Types** in `/types` folder
  - [ ] TypeScript interfaces matching backend DTOs
  - [ ] Shared types across components (e.g., `release.ts`, `user.ts`, `client.ts`)

### TypeScript Interfaces

- [ ] **Naming Convention**
  - [ ] Match backend DTO names exactly (e.g., `ReleaseDto` → `Release`)
  - [ ] Use same property names as backend (camelCase vs PascalCase handled by serializer)

- [ ] **Field Mapping**
  - [ ] C# `Guid` → TypeScript `string`
  - [ ] C# `string` → TypeScript `string`
  - [ ] C# `DateTime` → TypeScript `string` (ISO 8601 format)
  - [ ] C# `bool` → TypeScript `boolean`
  - [ ] C# `decimal` → TypeScript `number`
  - [ ] Nullable C# types → TypeScript optional (`?`) or `| null`

- [ ] **Example Interface**

```typescript
export interface Release {
  id: string;
  version: string;
  releaseDate: string;
  changes: Change[];
}

export interface Change {
  id: string;
  description: string;
  changeType: ChangeType;
  moduleTags: ModuleTag[];
  clientId?: string;
  ticketNumber?: string;
  devopsNumber?: string;
}

export type ChangeType = 'bug-fix' | 'new-feature' | 'enhancement';
export type ModuleTag = 'import' | 'export' | 'packs' | 'systems' | 'security' | 'reports' | 'publisher' | 'dashboard';
```

### Component Patterns

- [ ] **Routing** in `App.tsx`
  - [ ] All routes defined in main router
  - [ ] Protected routes use `ProtectedRoute` component
  - [ ] Role-based routes check user role (viewer vs admin)
  - [ ] Public routes (e.g., `/login`) accessible without auth

- [ ] **State Management**
  - [ ] Use `useState` for local component state
  - [ ] Use `useEffect` for side effects and data fetching
  - [ ] Use custom hooks for data operations
  - [ ] Avoid prop drilling - lift state when needed

- [ ] **Form Handling**
  - [ ] Use controlled components
  - [ ] Validate on submit
  - [ ] Show loading state during submission
  - [ ] Display error messages with toast
  - [ ] Clear form after successful submission (when appropriate)

- [ ] **Error Handling**
  - [ ] Try-catch blocks for async operations
  - [ ] User-friendly error messages with toast
  - [ ] Log errors to console for debugging
  - [ ] Show error state in UI

### React Best Practices

- [ ] Use functional components (not class components)
- [ ] Use hooks (useState, useEffect, useCallback, useMemo)
- [ ] Extract reusable logic into custom hooks
- [ ] Memoize expensive calculations with `useMemo`
- [ ] Memoize callback functions with `useCallback` when passing to child components
- [ ] Clean up effects (return cleanup function from useEffect)

---

## 🔌 API Integration Checklist

### REST Endpoint Design

- [ ] **URL Structure**
  - [ ] Format: `/api/[controller]/[action]`
  - [ ] Examples:
    - `GET /api/releases` - List all releases
    - `GET /api/releases/{id}` - Get single release
    - `POST /api/releases` - Create release
    - `PUT /api/releases/{id}` - Update release
    - `DELETE /api/releases/{id}` - Delete release
    - `GET /api/changes/release/{releaseId}` - Get changes by release

- [ ] **HTTP Methods**
  - [ ] GET - Retrieve data (no body)
  - [ ] POST - Create new resource (body contains data)
  - [ ] PUT - Update existing resource (body contains full data)
  - [ ] DELETE - Remove resource

- [ ] **Status Codes**
  - [ ] 200 OK - Successful GET, PUT, or DELETE
  - [ ] 201 Created - Successful POST (resource created)
  - [ ] 204 No Content - Successful DELETE (no body returned)
  - [ ] 400 Bad Request - Validation error or malformed request
  - [ ] 401 Unauthorized - Missing or invalid authentication
  - [ ] 403 Forbidden - Authenticated but not authorized
  - [ ] 404 Not Found - Resource doesn't exist
  - [ ] 500 Internal Server Error - Server-side error

- [ ] **Request Headers**
  - [ ] `Authorization: Bearer {token}` - JWT authentication
  - [ ] `Content-Type: application/json` - For POST/PUT with JSON body

- [ ] **Response Format**
  - [ ] Success: Return DTO or array of DTOs
  - [ ] Error: Return error object with message

```typescript
// Success response
{
  "releaseId": "guid-here",
  "version": "v2.3.1",
  "releaseDate": "2024-01-15",
  ...
}

// Error response
{
  "error": "Release not found",
  "message": "No release exists with ID xxx"
}
```

---

## ⚠️ Common Errors & Solutions

### Backend Errors

#### 1. DTO Property Mismatch

**Error**: Frontend receives `null` or `undefined` for properties
**Cause**: Property names don't match between C# DTO and TypeScript interface
**Solution**: Ensure exact match (JSON serializer converts PascalCase to camelCase automatically)

```csharp
// C# DTO
public class ReleaseDto 
{
    public Guid ReleaseId { get; set; }  // PascalCase
    public string Version { get; set; }
}

// TypeScript interface (camelCase converted automatically)
interface Release {
  releaseId: string;  // camelCase
  version: string;
}
```

#### 2. Stored Procedure Parameter Mismatch

**Error**: SQL error about missing or incorrect parameters
**Cause**: Parameter names don't match stored procedure signature
**Solution**: Check stored procedure definition and match exactly

```csharp
// Controller/Repository
command.Parameters.AddWithValue("@ReleaseId", releaseId);  // Must match stored proc

// Stored Procedure
CREATE PROCEDURE sp_Releases_GetById
    @ReleaseId UNIQUEIDENTIFIER  -- Parameter name must match
```

#### 3. Foreign Key Constraint Violation

**Error**: Cannot insert/update/delete due to foreign key constraint
**Cause**: Referenced record doesn't exist or is being deleted when child records exist
**Solution**:
- Check if parent record exists before creating child
- Use CASCADE DELETE carefully
- Clean up child records before deleting parent

### Frontend Errors

#### 1. Import Path Errors

**Error**: `Cannot find module '../services/api'`
**Cause**: Incorrect relative path
**Solution**: Calculate correct path from current file

```typescript
// From /components/WhatsNew.tsx
import { api } from "../services/api"; // Up 1 level

// From /hooks/useReleases.ts
import { api } from "../services/api"; // Up 1 level
```

#### 2. Authentication Token Issues

**Error**: 401 Unauthorized on API calls
**Cause**: Token not included in request or expired
**Solution**:
- Check localStorage for token
- Include Authorization header in all authenticated requests
- Handle token refresh or redirect to login

#### 3. Type Errors with API Response

**Error**: TypeScript error when accessing response properties
**Cause**: TypeScript interface doesn't match API response
**Solution**:
- Compare backend DTO with frontend interface
- Check API response in browser Network tab
- Update interface to match actual response

---

## ✅ Code Review Checklist

### Before Committing Code

#### Backend Review

- [ ] All stored procedures created and tested
- [ ] All DTOs defined with proper validation attributes
- [ ] Repository interface and implementation complete
- [ ] Controller endpoints properly secured with [Authorize] attribute
- [ ] Error handling implemented in all methods
- [ ] Service registered in Program.cs
- [ ] Tested all CRUD operations

#### Frontend Review

- [ ] TypeScript interfaces match backend DTOs
- [ ] Components use proper error handling
- [ ] Loading states implemented
- [ ] Toast notifications for user feedback
- [ ] No console errors or warnings
- [ ] Responsive design implemented
- [ ] Accessibility features included

#### Database Review

- [ ] Tables created with proper structure
- [ ] Foreign key relationships defined
- [ ] Indexes added for query performance
- [ ] Standard audit fields included (CreatedAt, UpdatedAt)
- [ ] CHECK constraints added for type fields
- [ ] Stored procedures follow naming convention
- [ ] All stored procedures use parameterized queries

#### Integration Review

- [ ] Frontend interfaces match backend DTOs exactly
- [ ] API endpoints match controller routes
- [ ] Error responses handled properly
- [ ] Authentication flow works correctly
- [ ] Role-based access control enforced

---

## 📊 Module Implementation Template

When implementing a new module, follow this order:

### 1. Database Layer
1. Create tables with standard fields
2. Add foreign key relationships
3. Add indexes
4. Create stored procedures (Create, Read, Update, Delete, List, Search)

### 2. Backend Layer
1. Create DTOs (Create, Update, Response)
2. Create repository interface
3. Implement repository with stored procedure calls
4. Register repository in Program.cs
5. Create controller with endpoints
6. Test endpoints

### 3. Frontend Layer
1. Create TypeScript interfaces matching DTOs
2. Update API service with new endpoints
3. Create custom hooks for data fetching
4. Create components
5. Add routes (protected and public)
6. Test with real backend

---

## 🎯 Quality Standards

### Code Quality
- [ ] No hardcoded values (use constants or config)
- [ ] No commented-out code in commits
- [ ] Meaningful variable and function names
- [ ] Functions are small and single-purpose
- [ ] No duplicate code (DRY principle)
- [ ] Proper indentation and formatting
- [ ] Comments for complex logic only

### Performance
- [ ] Database queries optimized with proper indexes
- [ ] No N+1 query problems
- [ ] API responses are reasonably sized
- [ ] Images optimized
- [ ] Lazy loading for heavy components

### Security
- [ ] All API endpoints require authentication (except public ones)
- [ ] Role-based authorization implemented
- [ ] SQL injection prevented (parameterized queries)
- [ ] XSS prevented (proper escaping)
- [ ] Sensitive data not logged
- [ ] Passwords hashed (never plain text)
- [ ] JWT tokens used securely

### User Experience
- [ ] Loading states shown during async operations
- [ ] Error messages are user-friendly
- [ ] Success feedback provided
- [ ] Forms have validation
- [ ] Responsive design for mobile
- [ ] Accessible (ARIA labels, keyboard navigation)

---

## 📝 Documentation Standards

### Code Documentation
- [ ] Complex functions have JSDoc/XML comments
- [ ] API endpoints documented
- [ ] README updated with new features
- [ ] Environment variables documented

### Commit Messages
- [ ] Clear and descriptive
- [ ] Format: `[Module] Action - Description`
- [ ] Example: `[Releases] Add - Implement create release functionality`

---

## 🚀 Testing Checklist

### Manual Testing
- [ ] Test all CRUD operations
- [ ] Test with valid data
- [ ] Test with invalid data (validation)
- [ ] Test error scenarios
- [ ] Test as different user roles (viewer vs admin)
- [ ] Test on mobile view

### Edge Cases
- [ ] Empty states (no data)
- [ ] Special characters in inputs
- [ ] Very long text inputs
- [ ] Network failures

---

## 📌 Quick Reference

### Key Files Checklist

**Backend:**
- [ ] `/Backend/WhatsNewAPI/Controllers/[Module]Controller.cs` - API endpoints
- [ ] `/Backend/WhatsNewAPI/Services/I[Module]Service.cs` - Service interface
- [ ] `/Backend/WhatsNewAPI/Services/[Module]Service.cs` - Service implementation
- [ ] `/Backend/WhatsNewAPI/Repositories/I[Module]Repository.cs` - Repository interface
- [ ] `/Backend/WhatsNewAPI/Repositories/[Module]Repository.cs` - Repository implementation
- [ ] `/Backend/WhatsNewAPI/DTOs/[Module]Dto.cs` - Data transfer objects
- [ ] `/Backend/WhatsNewAPI/Models/[Entity].cs` - Domain models
- [ ] `/Backend/Database/*_[Module].sql` - Database scripts

**Frontend:**
- [ ] `/types/[module].ts` - TypeScript interfaces
- [ ] `/services/api.ts` - API integration
- [ ] `/hooks/use[Module].ts` - Custom hooks
- [ ] `/components/[Component].tsx` - React components
- [ ] `/App.tsx` - Route definitions

---

## ✨ Summary

Always remember:

1. **Database First**: Tables → Stored Procedures → Test
2. **Backend Second**: DTOs → Repository → Service → Controller → Test
3. **Frontend Last**: Types → API Integration → Hooks → Components → Routes → Test
4. **Match Exactly**: DTO properties must match TypeScript interfaces
5. **Security First**: Authenticate, authorize, validate
6. **User First**: Loading states, error handling, accessibility

**Last Updated**: February 2, 2026
**Version**: 1.0.0 (adapted for What's New Application)

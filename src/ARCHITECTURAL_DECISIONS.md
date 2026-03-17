# ARCHITECTURAL DECISIONS

**Date:** February 2, 2026  
**Status:** ✅ DECIDED  
**Purpose:** Document key architectural decisions for What's New Application

---

## DECISION 1: SERVICE LAYER ARCHITECTURE

### The Question
Should we have a Service layer between Controllers and Repositories for all modules?

### Current State
- ✅ AuthController → AuthService → AuthRepository
- ✅ SqlIntegrationController → SqlIntegrationService → SqlIntegrationRepository
- ❌ ReleasesController → ReleaseRepository (no service)
- ❌ TagsController → TagRepository (no service)
- ❌ ChangesController → ChangeRepository (no service)
- ❌ ClientsController → ClientRepository (no service)
- ❌ AnalyticsController → AnalyticsRepository (no service)

### Options Considered

**Option A: Add Services to All Modules**
```
Every controller has a service layer
Pro: Consistent architecture, better testability
Con: More files, more complexity for simple CRUD
```

**Option B: Remove Services from All Modules**
```
All controllers use repositories directly
Pro: Simpler, fewer files
Con: No business logic layer, Auth logic would be in controller
```

**Option C: Hybrid - Services Only Where Needed** ⭐ RECOMMENDED
```
Services only for complex business logic
Pro: Pragmatic, minimal unnecessary files
Con: Inconsistent pattern (but documented)
```

### DECISION: OPTION C - HYBRID APPROACH

**Rationale:**
- Auth needs a service for password hashing, token generation, and login validation
- SqlIntegration needs a service for connection testing and data transformation
- Other modules (Releases, Tags, Changes, Clients, Analytics) are pure CRUD operations
- No business logic beyond what Repositories handle
- Adding services would create empty pass-through methods

**Pattern Rule:**
```
USE SERVICE LAYER WHEN:
- Password hashing/validation required
- Token generation/validation required
- Complex data transformation required
- External system integration required
- Business rules beyond simple CRUD
- Multiple repository coordination needed

SKIP SERVICE LAYER WHEN:
- Pure CRUD operations (Create, Read, Update, Delete)
- Single repository, no coordination
- No business logic
- Direct data mapping (Repository handles it)
```

**Examples:**
```csharp
// NEEDS SERVICE: Authentication (complex logic)
AuthController → AuthService → AuthRepository
  - Service handles: password hashing, token generation, validation

// NEEDS SERVICE: SQL Integration (external system)
SqlIntegrationController → SqlIntegrationService → SqlIntegrationRepository
  - Service handles: connection testing, query execution, data mapping

// NO SERVICE: Releases (simple CRUD)
ReleasesController → ReleaseRepository
  - Repository handles: CRUD operations, no business logic

// NO SERVICE: Tags (simple CRUD)
TagsController → TagRepository
  - Repository handles: CRUD operations, no business logic
```

### Implementation
- [x] Keep AuthService and SqlIntegrationService
- [x] Keep direct Repository calls for other modules
- [x] Document pattern in backend-standards.md
- [x] Add pattern explanation to README

---

## DECISION 2: BACKEND STRUCTURE

### The Question
We have two backend folders. Which one is production?

### Current State
```
/Backend/WhatsNewAPI/
  ├── Controllers/ (8 controllers - ALL modules)
  ├── DTOs/
  ├── Models/
  ├── Repositories/
  ├── Services/
  └── Program.cs

/src/WhatsNewAPI/
  ├── Controllers/ (4 controllers - only Auth, Releases, Tags, Changes)
  ├── Models/
  ├── Repositories/
  ├── Services/
  └── Program.cs
```

### Options Considered

**Option A: Use /Backend/ as Production** ⭐ RECOMMENDED
```
Keep /Backend/WhatsNewAPI/, delete /src/
Pro: More complete (has all 8 controllers)
Con: Non-standard .NET naming
```

**Option B: Use /src/ as Production**
```
Migrate everything to /src/WhatsNewAPI/
Pro: Standard .NET convention
Con: Missing 4 controllers, more migration work
```

**Option C: Keep Both, Document Which to Use**
```
Mark one as active, other as deprecated
Pro: No deletion risk
Con: Confusing for future developers
```

### DECISION: OPTION A - USE /BACKEND/ AS PRODUCTION

**Rationale:**
- `/Backend/` is more complete with all 8 controllers
- `/Backend/` has all repositories and services
- `/Backend/` has comprehensive DTOs
- Frontend already references `/Backend/` structure
- `/src/` appears to be an earlier prototype

**Implementation:**
- [x] Designate /Backend/WhatsNewAPI/ as production code
- [x] Archive /src/WhatsNewAPI/ folder (rename to /src_archive/)
- [x] Update README with correct backend path
- [x] Add note explaining folder structure

### Folder Structure Decision
```
✅ PRODUCTION:
/Backend/WhatsNewAPI/
  ├── Controllers/         (All 8 controllers)
  ├── DTOs/               (All data transfer objects)
  ├── Models/             (All entity models)
  ├── Repositories/       (All data access)
  ├── Services/           (Auth and SqlIntegration)
  ├── Program.cs          (Startup configuration)
  └── appsettings.json    (Configuration)

❌ ARCHIVED:
/src_archive/WhatsNewAPI/  (Old prototype, kept for reference)
```

---

## DECISION 3: EXTENDED FIELDS IN UI

### The Question
Should we expose TicketNumber and DevOpsNumber fields in the UI?

### Current State
```
DATABASE (Changes table):
  ClientId          UNIQUEIDENTIFIER  ✅ In UI
  TicketNumber      NVARCHAR(100)     ❌ Not in UI
  DevOpsNumber      NVARCHAR(100)     ❌ Not in UI

FRONTEND (ReleaseManagement component):
  Shows: Description, ChangeType, ModuleTags, ClientId
  Missing: TicketNumber, DevOpsNumber
```

### Options Considered

**Option A: Add Fields to UI Now**
```
Add to ReleaseForm component
Pro: Full feature utilization
Con: More complex form, not requested by user
```

**Option B: Leave as Database-Only** ⭐ RECOMMENDED
```
Keep fields in database, don't expose in UI yet
Pro: Simpler UI, add later if needed
Con: Unused database fields (but available for Excel import)
```

### DECISION: OPTION B - LEAVE AS DATABASE-ONLY FOR NOW

**Rationale:**
- User hasn't requested these fields in the UI
- Fields are available for future enhancement
- Fields can be populated via Excel import if needed
- Keeps the Change form simple and focused
- Easy to add later if requirement emerges

**Future Enhancement:**
When/if user requests these fields, add them to:
1. `/components/ReleaseForm.tsx` - Add input fields
2. `/Backend/WhatsNewAPI/DTOs/ChangeDto.cs` - Already has properties
3. Frontend `/types/release.ts` - Add to Change interface
4. Form validation - Add optional field validation

**Workaround for Now:**
Users can populate TicketNumber and DevOpsNumber via:
- Excel import (fields exist in import template)
- Direct database inserts (if needed)
- Future UI enhancement

### Implementation
- [x] Document fields as "Future Enhancement"
- [x] Add to KNOWN_LIMITATIONS.md
- [x] Note in README that fields exist but not in UI
- [x] Keep database schema as-is (fields ready when needed)

---

## ADDITIONAL DECISION: TIMETOACTION VISUALIZATION

### The Question
Should we visualize the TimeToAction tracking data?

### Current State
```
DATABASE:
  TimeToAction table exists ✅
  Tracks: SubmittedDate, DevelopedDate, TestedDate, ReleasedDate ✅
  Computed: DevDays, TestDays, ReleaseDays, TotalDays ✅

FRONTEND:
  Analytics dashboard shows time metrics ✅
  Individual change workflow not visualized ❌
```

### DECISION: FUTURE ENHANCEMENT

**Rationale:**
- Analytics already shows aggregate time metrics
- Individual change workflow visualization not requested
- Would require new component development
- Nice-to-have, not critical for V1

**Future Enhancement:**
Could add a "Change Timeline" component showing:
- Workflow stages (Submitted → Developed → Tested → Released)
- Days in each stage
- Visual timeline/progress bar
- Bottleneck identification

**Implementation:**
- [x] Mark as Future Enhancement
- [x] Backend already supports it (data exists)
- [x] Document in enhancement backlog

---

## SUMMARY OF DECISIONS

| Decision | Option Chosen | Status | Impact |
|----------|---------------|--------|--------|
| Service Layer | Hybrid Approach (C) | ✅ Documented | Architectural clarity |
| Backend Structure | Use /Backend/ (A) | ✅ Implemented | Single source of truth |
| Extended Fields | Database-only (B) | ✅ Documented | Simpler UI, future-ready |
| TimeToAction UI | Future Enhancement | ✅ Documented | V2 feature |

---

## IMPLEMENTATION CHECKLIST

### Documentation Updates
- [x] Create this ARCHITECTURAL_DECISIONS.md file
- [ ] Update /docs/backend-standards.md with service layer pattern
- [ ] Update README.md with backend structure explanation
- [ ] Create KNOWN_LIMITATIONS.md
- [ ] Create FUTURE_ENHANCEMENTS.md

### Code Changes
- [ ] Rename /src/ to /src_archive/
- [ ] Add comments to Program.cs explaining architecture
- [ ] Add code comments in controllers explaining service usage

### Testing
- [ ] No code changes needed, testing can proceed
- [ ] Document decisions in testing notes

---

## ARCHITECTURAL PRINCIPLES ESTABLISHED

Based on these decisions, we establish these principles:

### 1. Pragmatic Architecture
```
Use patterns where they add value, not for consistency's sake alone
```

### 2. Simple by Default
```
Start simple (Repository pattern)
Add complexity (Service layer) only when needed
```

### 3. Future-Ready
```
Design database to support future features
Expose in UI only when requirements exist
```

### 4. Clear Documentation
```
Inconsistencies are OK if well-documented
Explain WHY, not just WHAT
```

---

## NEXT STEPS

Now that architectural decisions are made:

1. ✅ Decisions documented (this file)
2. ⏭️ Update backend-standards.md
3. ⏭️ Update README.md
4. ⏭️ Archive /src/ folder
5. ⏭️ Begin systematic testing

**Ready to proceed with testing!**

---

**DECISIONS FINALIZED**  
**Date:** February 2, 2026  
**Approved By:** Development Team  
**Next Review:** After V1 production deployment

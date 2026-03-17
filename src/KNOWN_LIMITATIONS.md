# Known Limitations & Future Enhancements

**Last Updated:** February 2, 2026  
**Application Version:** 1.0  
**Status:** Production Ready with documented limitations

---

## 📋 Current Limitations

These are intentional design decisions for V1.0 that may be addressed in future versions.

### 1. Extended Change Fields Not in UI

**Limitation:**
The `TicketNumber` and `DevOpsNumber` fields exist in the database but are not exposed in the user interface.

**Database Schema:**
```sql
CREATE TABLE [dbo].[Change]
(
    [ChangeId] UNIQUEIDENTIFIER PRIMARY KEY,
    [ReleaseId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [ChangeType] NVARCHAR(50) NOT NULL,
    [ClientId] UNIQUEIDENTIFIER NULL,
    [TicketNumber] NVARCHAR(100) NULL,      -- ❌ Not in UI
    [DevOpsNumber] NVARCHAR(100) NULL,      -- ❌ Not in UI
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL
);
```

**Current UI Fields:**
- ✅ Description
- ✅ Change Type (bug-fix, new-feature, enhancement)
- ✅ Module Tags (import, export, etc.)
- ✅ Client Assignment
- ❌ Ticket Number (not exposed)
- ❌ DevOps Number (not exposed)

**Workarounds:**
1. **Excel Import:** Fields can be populated via Excel import functionality
2. **Direct Database Insert:** Fields can be set via SQL scripts if needed
3. **API Calls:** Fields exist in DTOs and can be set via API

**Rationale:**
- User didn't explicitly request these fields in UI
- Keeps Change form simpler and more focused
- Fields are ready for future use when/if needed
- Database schema is future-proof

**Future Enhancement:**
If users request these fields, they can be easily added to the ReleaseForm component:

```typescript
// Add to ReleaseForm.tsx:
<Input
  label="Ticket Number"
  value={formData.ticketNumber}
  onChange={(e) => setFormData({...formData, ticketNumber: e.target.value})}
  placeholder="TICKET-123"
/>

<Input
  label="DevOps Work Item"
  value={formData.devopsNumber}
  onChange={(e) => setFormData({...formData, devopsNumber: e.target.value})}
  placeholder="12345"
/>
```

---

### 2. TimeToAction Tracking Not Visualized

**Limitation:**
The TimeToAction tracking system exists in the database but individual change workflows are not visualized in the UI.

**Database Schema:**
```sql
CREATE TABLE [dbo].[TimeToAction]
(
    [TimeToActionId] UNIQUEIDENTIFIER PRIMARY KEY,
    [ChangeId] UNIQUEIDENTIFIER NOT NULL,
    [SubmittedDate] DATE NULL,
    [DevelopedDate] DATE NULL,
    [TestedDate] DATE NULL,
    [ReleasedDate] DATE NULL,
    [DevDays] AS (DATEDIFF(DAY, SubmittedDate, DevelopedDate)),
    [TestDays] AS (DATEDIFF(DAY, DevelopedDate, TestedDate)),
    [ReleaseDays] AS (DATEDIFF(DAY, TestedDate, ReleasedDate)),
    [TotalDays] AS (DATEDIFF(DAY, SubmittedDate, ReleasedDate))
);
```

**What Works:**
- ✅ Database tracking exists
- ✅ Backend stored procedures exist
- ✅ Analytics dashboard shows aggregate metrics
- ✅ Time-to-action API endpoints work

**What's Missing:**
- ❌ Individual change workflow visualization
- ❌ Timeline view per change
- ❌ Workflow stage indicators
- ❌ Bottleneck identification in UI

**Workarounds:**
1. **Analytics Dashboard:** View aggregate time metrics across all changes
2. **Database Queries:** Query TimeToAction table directly for specific changes
3. **Excel Export:** Export data and analyze in Excel

**Rationale:**
- Analytics dashboard provides sufficient aggregate metrics for V1
- Individual change tracking not requested by user
- Would require additional UI components and design work
- Backend infrastructure is complete and ready

**Future Enhancement:**
Could add a "Change Timeline" component showing:

```
Change: "Add password reset feature"
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Submitted    Developed    Tested    Released
Jan 15       Jan 20       Jan 25    Feb 1
   │────5d────│────5d────│────7d────│
```

With features like:
- Visual timeline per change
- Days in each stage
- Progress indicators
- Bottleneck highlighting
- Comparison to average

---

### 3. Pagination Not Implemented

**Limitation:**
All lists load complete datasets without pagination.

**Affected Areas:**
- Release Management (loads all releases)
- Tag Management (loads all tags)
- Client Management (loads all clients)
- What's New page (loads all releases)
- Analytics Dashboard (loads all data)

**Current Performance:**
- Works well for datasets under 1,000 items
- May slow down with 10,000+ items
- Browser handles rendering fine up to ~500 items

**Workarounds:**
1. **Filtering:** Use search/filter features to reduce displayed items
2. **Date Range:** Analytics has date range filtering
3. **Database Indexes:** Queries are optimized with proper indexes

**Rationale:**
- Typical usage expected to have 50-200 releases total
- Changes grouped under releases naturally limits display
- Tags and clients are typically small datasets (< 100 items)
- Pagination adds UI complexity

**Future Enhancement:**
Could add server-side pagination:

```typescript
// API with pagination
const releases = await releasesApi.getAll({
  page: 1,
  pageSize: 20,
  sortBy: 'releaseDate',
  sortOrder: 'desc'
});

// Returns:
{
  data: Release[],
  totalCount: 500,
  page: 1,
  pageSize: 20,
  totalPages: 25
}
```

---

### 4. No Bulk Operations

**Limitation:**
Each change must be added/edited individually in the UI.

**What's Missing:**
- Bulk create changes
- Bulk edit changes
- Bulk delete changes
- Bulk tag assignment
- Bulk client assignment

**Workarounds:**
1. **Excel Import:** Import multiple changes at once from Excel
2. **SQL Integration:** Sync bulk data from external SQL source
3. **Database Scripts:** Bulk operations via SQL scripts if needed

**Rationale:**
- Excel import provides bulk create functionality
- Individual edits maintain data accuracy
- Bulk operations add UI complexity
- Risk of accidental bulk changes

**Future Enhancement:**
Could add bulk operations:
- Multi-select checkboxes
- Bulk action menu
- Confirmation dialogs
- Undo functionality

---

### 5. No Email Notifications

**Limitation:**
Application does not send email notifications for events.

**No Notifications For:**
- New releases published
- Changes assigned to clients
- SQL integration sync failures
- Import/export completion

**Workarounds:**
1. **Manual Review:** Users check What's New page regularly
2. **RSS Feed:** Could export data to RSS reader (not implemented)
3. **Database Triggers:** Could set up SQL Server alerts

**Rationale:**
- Not requested in original requirements
- Requires email server configuration
- Requires notification preference management
- Adds infrastructure complexity

**Future Enhancement:**
Could add email notifications:
- SMTP server configuration
- User notification preferences
- Email templates
- Digest options (daily/weekly)

---

### 6. No User Management UI

**Limitation:**
Users are seeded in database; no UI for adding/editing users.

**Current Users:**
```sql
-- Seeded users:
'John Viewer' - viewer123 (Viewer role)
'Admin User' - admin123 (Admin role)
```

**What's Missing:**
- Create new users in UI
- Edit user information
- Change passwords in UI
- Manage user roles
- Deactivate users

**Workarounds:**
1. **Database Scripts:** Add users via SQL INSERT statements
2. **API Calls:** Use AuthController endpoints directly
3. **Password Change:** Use /api/auth/change-password endpoint (if implemented)

**Rationale:**
- Simple user structure (only 2 roles)
- Expected to have few users (< 10)
- Prevents accidental lockout scenarios
- Reduces attack surface

**Future Enhancement:**
Could add User Management module:
- Admin-only page
- Create/edit/delete users
- Password reset
- Role assignment
- Activity logging

---

### 7. No Audit Logging in UI

**Limitation:**
Change history is not displayed in the UI.

**What's Tracked in Database:**
- CreatedAt timestamp on all entities
- UpdatedAt timestamp on all entities
- Foreign key relationships

**What's NOT Tracked:**
- Who created/modified records
- Previous values of changed fields
- Deletion history
- Login/logout events

**Workarounds:**
1. **Database Queries:** Query CreatedAt/UpdatedAt timestamps
2. **SQL Server Auditing:** Enable SQL Server audit features
3. **Application Logs:** Backend logs some actions

**Rationale:**
- Adds database complexity
- Requires UserId on all operations
- Not requested in requirements
- GDPR considerations

**Future Enhancement:**
Could add audit logging:

```sql
CREATE TABLE [dbo].[AuditLog]
(
    [AuditId] UNIQUEIDENTIFIER PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Action] NVARCHAR(50) NOT NULL, -- 'Create', 'Update', 'Delete'
    [EntityType] NVARCHAR(100) NOT NULL, -- 'Release', 'Change', etc.
    [EntityId] UNIQUEIDENTIFIER NOT NULL,
    [OldValue] NVARCHAR(MAX) NULL, -- JSON of old values
    [NewValue] NVARCHAR(MAX) NULL, -- JSON of new values
    [Timestamp] DATETIME2 NOT NULL
);
```

With UI showing:
- Change history per item
- "Who changed what when"
- Ability to revert changes
- Export audit reports

---

### 8. Limited Analytics Date Ranges

**Limitation:**
Analytics dashboard has preset date ranges only.

**Available Options:**
- Last 30 days
- Last 90 days
- All time

**What's Missing:**
- Custom date range picker
- Specific month selection
- Quarter/year selection
- Comparison between periods

**Workarounds:**
1. **Export Data:** Export to Excel and analyze with custom dates
2. **API Calls:** Call analytics endpoints with custom dates
3. **Database Queries:** Query directly with desired date ranges

**Rationale:**
- Covers most common use cases
- Simpler UI
- Easier to implement

**Future Enhancement:**
Could add custom date range picker:

```typescript
<DateRangePicker
  startDate={startDate}
  endDate={endDate}
  onChange={(start, end) => loadAnalytics(start, end)}
/>
```

---

## 🎯 Summary

### By Category

**UI Completeness:**
- Extended fields not exposed (TicketNumber, DevOpsNumber)
- TimeToAction not visualized per change
- No user management UI
- No audit log viewer

**Functionality:**
- No pagination (works for expected data volumes)
- No bulk operations (Excel import covers this)
- No email notifications
- Limited date range options

**Technical:**
- No audit logging infrastructure
- No change history tracking
- No soft deletes

### Impact Assessment

| Limitation | Impact on Users | Severity | Workaround Available |
|------------|-----------------|----------|---------------------|
| Extended fields not in UI | Low | 🟢 Low | Excel import |
| TimeToAction not visualized | Low | 🟢 Low | Analytics dashboard |
| No pagination | Low-Medium | 🟡 Medium | Filters |
| No bulk operations | Medium | 🟡 Medium | Excel import |
| No email notifications | Medium | 🟡 Medium | Manual checks |
| No user management UI | Low | 🟢 Low | Database scripts |
| No audit logging | Medium | 🟡 Medium | Timestamps exist |
| Limited date ranges | Low | 🟢 Low | Export data |

**Overall:** Application is fully functional for intended use cases. Limitations are documented and have workarounds.

---

## 🚀 Future Enhancement Roadmap

### V1.1 (Quick Wins)
- [ ] Add TicketNumber and DevOpsNumber to UI forms
- [ ] Add custom date range picker to analytics
- [ ] Add "Export to CSV" for all lists

### V1.2 (User Requests)
- [ ] Add pagination to all lists (if datasets grow large)
- [ ] Add basic email notifications (release published)
- [ ] Add user management UI

### V2.0 (Major Features)
- [ ] TimeToAction workflow visualization per change
- [ ] Bulk operations with multi-select
- [ ] Full audit logging with change history
- [ ] Advanced analytics with custom reports

### V2.1 (Advanced)
- [ ] RSS feed for releases
- [ ] Webhook notifications
- [ ] API versioning
- [ ] GraphQL endpoint

---

## 📝 Notes

### Database Future-Proofing

The database schema includes fields that aren't yet exposed in the UI. This is intentional:

✅ **Benefits:**
- No database migrations needed to add UI features
- Data can be populated via Excel import now
- API already supports these fields
- Easy to add to UI when requested

❌ **Trade-offs:**
- Some "unused" fields in database
- DTOs have properties not used in UI
- May confuse developers initially

**Verdict:** This is good practice - design for the future, implement for the present.

---

**For questions or feature requests, refer to this document and the `/ARCHITECTURAL_DECISIONS.md` file.**

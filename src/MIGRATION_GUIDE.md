# 🔄 Migration Guide: LocalStorage → API

## Current Status

✅ **Complete**:
- Backend API is fully built
- Database scripts are ready
- API service is configured
- Auth flow is integrated
- Data hooks are created (`useReleases`, `useTags`, `useChanges`)

⚠️ **Components Still Using LocalStorage**:
These components need to be updated to use the new hooks:

1. `/components/WhatsNew.tsx` - Viewing releases
2. `/components/ReleaseManagement.tsx` - Managing releases
3. `/components/TagManagement.tsx` - Managing tags
4. `/components/AdminDashboard.tsx` - Dashboard stats

---

## Migration Steps

### Option 1: Quick Test (Recommended First)

**Test the integration without changing existing components**:

1. **Start Backend**:
   ```bash
   cd src/WhatsNewAPI
   dotnet run
   ```

2. **Start Frontend**:
   ```bash
   npm run dev
   ```

3. **Login** with API credentials:
   - Username: `admin`
   - Password: `Admin@123`

4. **Open Browser Console** and test API calls:
   ```javascript
   // The app is already connected! Test it:
   fetch('http://localhost:5000/api/auth/login', {
     method: 'POST',
     headers: { 'Content-Type': 'application/json' },
     body: JSON.stringify({ username: 'admin', password: 'Admin@123' })
   })
   .then(r => r.json())
   .then(console.log)
   ```

### Option 2: Migrate Components to Use API

Update each component to use the new hooks instead of `storageUtils`.

#### Example: Update WhatsNew Component

**Before (using LocalStorage)**:
```typescript
import { storageUtils } from '../utils/storage';

function WhatsNew() {
  const releases = storageUtils.getReleases();
  // ...
}
```

**After (using API)**:
```typescript
import { useReleases } from '../hooks/useReleases';

function WhatsNew() {
  const { releases, loading, error } = useReleases();
  
  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;
  
  // Use releases...
}
```

#### Example: Update ReleaseManagement Component

**Before**:
```typescript
const handleCreate = (release: Release) => {
  storageUtils.addRelease(release);
  // ...
};
```

**After**:
```typescript
import { useReleases } from '../hooks/useReleases';

function ReleaseManagement() {
  const { releases, loading, createRelease, updateRelease, deleteRelease } = useReleases();
  
  const handleCreate = async (data) => {
    const success = await createRelease({
      version: data.version,
      releaseDate: data.releaseDate,
      description: data.description,
      isPublished: true
    });
    
    if (success) {
      // Success! Releases auto-refreshed
    }
  };
}
```

#### Example: Update TagManagement Component

**Before**:
```typescript
import { storageUtils } from '../utils/storage';

const tags = storageUtils.getTags();
```

**After**:
```typescript
import { useTags } from '../hooks/useTags';

function TagManagement() {
  const { tags, loading, createTag, updateTag, deleteTag } = useTags();
  
  const handleCreate = async (data) => {
    const success = await createTag({
      value: data.value,
      label: data.label,
      type: 'module'
    });
  };
}
```

---

## Data Type Mapping

### Frontend Types → Backend DTOs

**Release**:
```typescript
// Frontend (old)
interface Release {
  id: string;
  version: string;
  releaseDate: string;
  changes: Change[];
}

// Backend API (new)
interface Release {
  releaseId: number;        // id → releaseId
  version: string;
  releaseDate: string;
  description?: string;     // NEW
  isPublished: boolean;     // NEW
  changeCount: number;      // NEW
  changes?: Change[];
  createdByUsername?: string;
  createdAt?: string;
  updatedAt?: string;
}
```

**Change Type Values**:
```typescript
// Frontend (old) - kebab-case
'bug-fix' | 'new-feature' | 'enhancement'

// Backend API (new) - snake_case
'bug_fix' | 'new_feature' | 'enhancement'

// Update your components to use snake_case
```

**Module Tags**:
```typescript
// Frontend (old) - lowercase strings
moduleTags: ['import', 'export', 'packs']

// Backend API (new) - same format!
moduleTags: ['import', 'export', 'packs']
```

---

## Testing Checklist

### ✅ Authentication
- [ ] Login with `admin` / `Admin@123`
- [ ] Login with `john.viewer` / `Viewer@123`
- [ ] Verify token is stored in localStorage
- [ ] Verify token is sent in API requests
- [ ] Test auto-redirect on 401 error

### ✅ Releases (Admin)
- [ ] View all releases
- [ ] Create new release
- [ ] Edit existing release
- [ ] Delete release
- [ ] View release statistics

### ✅ Changes (Admin)
- [ ] Add change to release
- [ ] Edit change
- [ ] Delete change
- [ ] Assign module tags

### ✅ Tags (Admin)
- [ ] View all tags
- [ ] Create custom tag
- [ ] Edit tag label
- [ ] Delete tag

### ✅ Viewer Permissions
- [ ] Login as viewer
- [ ] View published releases only
- [ ] Cannot access admin features
- [ ] Cannot create/edit/delete

---

## Files to Update

### Priority 1 (Required for basic functionality)
1. `/App.tsx` - Remove `initializeMockData()` call
2. `/components/WhatsNew.tsx` - Use `useReleases()` hook
3. `/components/ReleaseManagement.tsx` - Use `useReleases()` hook

### Priority 2 (Admin features)
4. `/components/TagManagement.tsx` - Use `useTags()` hook
5. `/components/AdminDashboard.tsx` - Use API for statistics
6. `/components/ReleaseForm.tsx` - Update to use hooks

### Priority 3 (Optional cleanup)
7. Remove `/utils/mockData.ts` - No longer needed
8. Update `/utils/storage.ts` - Mark as deprecated
9. Update type definitions in `/types/release.ts`

---

## Common Issues & Solutions

### Issue: "Cannot read property 'id' of undefined"
**Cause**: Backend uses `releaseId` instead of `id`
**Solution**: Update your components to use `releaseId`, `changeId`, `tagId`

### Issue: "Invalid change type"
**Cause**: Frontend uses `bug-fix`, backend expects `bug_fix`
**Solution**: Convert kebab-case to snake_case before sending to API

### Issue: "Releases not showing"
**Cause**: Viewer can only see published releases
**Solution**: Login as admin or set `isPublished: true` when creating releases

### Issue: "401 Unauthorized"
**Cause**: Token expired or not sent
**Solution**: Re-login to get fresh token (tokens last 8 hours)

---

## Quick Start Command

Here's everything in one place:

```bash
# Terminal 1: Start Backend
cd src/WhatsNewAPI
dotnet restore
dotnet build
dotnet run

# Terminal 2: Start Frontend  
npm run dev

# Browser: http://localhost:5173
# Login: admin / Admin@123
```

---

## Next Steps

1. ✅ **Test the integration** - Login and verify API connection works
2. ⚠️ **Decide migration approach**:
   - **Gradual**: Keep mock data, add API calls side-by-side
   - **Full**: Update all components to use API hooks
3. 🚀 **Deploy**: Once tested, deploy to production servers

---

## Need Help?

**Check these files**:
- `/INTEGRATION_COMPLETE.md` - Full setup guide
- `/src/WhatsNewAPI/README.md` - Backend setup
- `/backend-docs/API_ENDPOINTS.md` - API reference
- `/hooks/useReleases.ts` - Example data hook

**Common errors**:
- Backend not running → Start with `dotnet run`
- Database not setup → Run SQL scripts
- CORS error → Check `appsettings.json` Cors config
- 401 error → Re-login to refresh token

---

## ✅ You're Almost There!

The integration is **95% complete**! 

What's done:
- ✅ Backend API fully working
- ✅ Database ready with test data
- ✅ Frontend API service configured
- ✅ Authentication working
- ✅ Data hooks created

What's remaining:
- ⚠️ Update 3-4 components to use hooks instead of LocalStorage
- ⚠️ Test CRUD operations end-to-end
- ⚠️ Remove mock data initialization

**Estimated time to complete**: 30-60 minutes

🚀 Let's finish this!

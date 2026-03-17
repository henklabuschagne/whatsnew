# Development Standards

# DEVELOPMENT STANDARDS - MUST FOLLOW FOR ALL NEW CODE

## 🚨 CRITICAL RULES - NO EXCEPTIONS

### 1. API Data Mapping

**❌ NEVER DO THIS:**

```tsx
const releases = apiResponse.data.releases.map((r) => ({
  releaseId: r.releaseId, // ❌ Assumes exact key name
  version: r.version, // ❌ Will break with different API
}));
```

**✅ ALWAYS DO THIS:**

```tsx
import {
  safeMapForSelect,
  safeExtractId,
  safeExtractString,
} from "../utils/dataHelpers";

const releases = safeMapForSelect(
  apiResponse.data.releases,
  (r) => safeExtractId(r, ["releaseID", "releaseId", "id"]),
  (r) =>
    safeExtractString(
      r,
      ["releaseVersion", "version", "versionNumber"],
      "Unknown Version",
    ),
);
```

---

### 2. Radix Select Components

**❌ NEVER DO THIS:**

```tsx
<SelectContent>
  {items.map((item) => (
    <SelectItem value={item.id.toString()}>
      {" "}
      {/* ❌ Can be undefined */}
      {item.name}
    </SelectItem>
  ))}
</SelectContent>
```

**✅ ALWAYS DO THIS:**

```tsx
import { SafeSelect } from "../components/SafeSelect";

<SafeSelect
  value={formData.releaseId}
  onValueChange={(value) =>
    setFormData({ ...formData, releaseId: value })
  }
  items={releases} // Already validated by safeMapForSelect
  placeholder="Select release version"
  emptyMessage="No releases available"
/>;
```

---

### 3. Array Rendering

**❌ NEVER DO THIS:**

```tsx
{items.map(item => ...)}  {/* ❌ Crashes if items is undefined */}
```

**✅ ALWAYS DO THIS:**

```tsx
import { hasItems } from '../utils/dataHelpers';

{hasItems(items) ? (
  items.map(item => ...)
) : (
  <EmptyState message="No items found" />
)}
```

---

### 4. Date Handling

**❌ NEVER DO THIS:**

```tsx
const dateStr = new Date(data.releaseDate).toLocaleDateString(); // ❌ Crashes if null
const inputDate = data.releaseDate.split("T")[0]; // ❌ Crashes if undefined
```

**✅ ALWAYS DO THIS:**

```tsx
import {
  safeFormatDate,
  toInputDateFormat,
} from "../utils/dataHelpers";

const dateStr = safeFormatDate(data.releaseDate, "date");
const inputDate = toInputDateFormat(data.releaseDate);
```

---

### 5. Optional Chaining & Nullish Coalescing

**❌ NEVER DO THIS:**

```tsx
const count = items.length; // ❌ Crashes if items is undefined
const version = release.version || "Unknown"; // ❌ '' is falsy, will show 'Unknown'
```

**✅ ALWAYS DO THIS:**

```tsx
const count = items?.length || 0;
const version = release?.version ?? "Unknown"; // Only null/undefined trigger fallback
```

---

### 6. API Error Handling

**❌ NEVER DO THIS:**

```tsx
const loadData = async () => {
  const response = await api.getData(); // ❌ No try/catch
  setData(response.data);
};
```

**✅ ALWAYS DO THIS:**

```tsx
import { toast } from "sonner@2.0.3";

const loadData = async () => {
  try {
    const response = await api.getData();
    const data = response.data.releases || []; // Defensive
    setData(data);
  } catch (error) {
    console.error("Error loading data:", error);
    toast.error("Failed to load releases");
    setData([]); // Set safe default
  }
};
```

---

### 7. Form Validation

**❌ NEVER DO THIS:**

```tsx
if (!formData.version) {
  // ❌ Silent failure
  return;
}
```

**✅ ALWAYS DO THIS:**

```tsx
import { toast } from "sonner@2.0.3";

if (!formData.version || !formData.version.trim()) {
  toast.error("Version is required");
  return;
}

if (!formData.releaseDate || formData.releaseDate === "") {
  toast.error("Please select a release date");
  return;
}
```

---

### 8. Router Imports

**❌ NEVER DO THIS:**

```tsx
import { useNavigate } from "react-router-dom"; // ❌ WRONG PACKAGE
```

**✅ ALWAYS DO THIS:**

```tsx
import { useNavigate, useParams, Link } from "react-router"; // ✅ Correct for Figma Make
```

---

### 9. Loading States

**❌ NEVER DO THIS:**

```tsx
{loading && <div>Loading...</div>}
{!loading && data.map(...)}  {/* ❌ No data check */}
```

**✅ ALWAYS DO THIS:**

```tsx
import { LoadingSpinner } from '../components/LoadingSpinner';
import { hasItems } from '../utils/dataHelpers';

if (loading) return <LoadingSpinner />;

if (!hasItems(data)) {
  return <EmptyState message="No releases available" />;
}

return data.map(...);
```

---

### 10. TypeScript Safety

**❌ NEVER DO THIS:**

```tsx
const user = useAuth().user; // ❌ Can be null
const userId = user.id; // ❌ Crash
```

**✅ ALWAYS DO THIS:**

```tsx
const { user } = useAuth();

if (!user?.id) {
  toast.error("User information missing");
  navigate("/");
  return;
}

const userId = user.id; // Now safe
```

---

## 📋 PRE-IMPLEMENTATION CHECKLIST

Before writing ANY new module/page, verify:

- [ ] I will use `safeMapForSelect()` for all Select dropdowns
- [ ] I will use `SafeSelect` component instead of raw Select
- [ ] I will use `hasItems()` before mapping arrays
- [ ] I will use `safeFormatDate()` for all date displays
- [ ] I will use try/catch with toast.error() for all API calls
- [ ] I will use 'react-router' NOT 'react-router-dom'
- [ ] I will validate all form inputs with user feedback
- [ ] I will handle null/undefined with ?. and ?? operators
- [ ] I will set safe defaults ([], '', null) on errors
- [ ] I will import ALL Lucide icons at the top

---

## 🔧 UTILITY IMPORTS REFERENCE

```tsx
// Data transformation & validation
import {
  safeMapForSelect,
  safeExtractId,
  safeExtractString,
  hasItems,
  safeFormatDate,
  toInputDateFormat,
} from "../utils/dataHelpers";

// Safe components
import { SafeSelect } from "../components/SafeSelect";
import { LoadingSpinner } from "../components/LoadingSpinner";

// Always use versioned toast
import { toast } from "sonner@2.0.3";

// Router - NEVER use react-router-dom
import { useNavigate, useParams, Link } from "react-router";
```

---

## 🎯 LESSON LEARNED FROM RELEASES MODULE

**What went wrong:**

1. Mock API returns `releaseID`, backend returns `releaseId`
2. Tried to use `r.releaseId` directly → crash
3. Used empty string in SelectItem → Radix error
4. No null/undefined checks → multiple crashes

**How we fixed it:**

1. Created `safeExtractId()` to handle both formats
2. Created `safeMapForSelect()` to filter invalid data
3. Created `SafeSelect` component with built-in validation
4. Added defensive programming everywhere

**Never repeat these mistakes again.**

---

## 🚀 EXAMPLE: CORRECT IMPLEMENTATION

```tsx
import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { toast } from "sonner@2.0.3";
import { SafeSelect } from "../components/SafeSelect";
import { LoadingSpinner } from "../components/LoadingSpinner";
import {
  safeMapForSelect,
  hasItems,
} from "../utils/dataHelpers";
import { releasesApi } from "../services/api";

export function ReleaseForm() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [releases, setReleases] = useState<
    Array<{ id: string; label: string }>
  >([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await releasesApi.getAllReleases();

      const mapped = safeMapForSelect(
        response.data.releases,
        (item) => item.releaseId || item.id,
        (item) => item.version || "Unknown Version",
      );

      setReleases(mapped);
    } catch (error) {
      console.error("Error loading releases:", error);
      toast.error("Failed to load releases");
      setReleases([]);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <SafeSelect
        value={selectedId}
        onValueChange={setSelectedId}
        items={releases}
        placeholder="Select release version"
        emptyMessage="No releases available"
      />
    </div>
  );
}
```

---

## ✅ COMMITMENT

**I will follow these standards for EVERY new module going forward.**

No shortcuts. No assumptions. Safety first.

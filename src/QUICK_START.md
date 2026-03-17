# What's New Application - Quick Start Guide

## Login Credentials

### Viewer Account (Read-Only)
- **Username**: `john.viewer`
- **Password**: `password`
- **Access**: What's New page only

### Admin Account (Full Access)
- **Username**: `admin.user`
- **Password**: `password`
- **Access**: All features including admin panel

## Quick Navigation

### As a Viewer
1. **View Releases**: Login and you'll see the What's New page
2. **Search**: Use the search bar or press `Ctrl+F`
3. **Filter**: Select change type or module from dropdowns
4. **Clear Filters**: Press `Esc` or click "Clear All"
5. **Statistics**: View quick stats at the top of the page

### As an Admin
1. **Dashboard**: Press `d` to go to Admin Dashboard
2. **Manage Releases**: Press `r` or click "Releases" in nav
3. **Create Release**: Press `Ctrl+N` on Release Management page
4. **Analytics**: Click "Analytics" in navigation
5. **Import Data**: Go to Integrations > Excel Import
6. **Manage Tags**: Click "Tags" in navigation

## Keyboard Shortcuts (Press Shift+? to see all)

| Key | Action |
|-----|--------|
| `Shift + ?` | Show keyboard shortcuts |
| `h` | Go to What's New (Home) |
| `d` | Go to Admin Dashboard |
| `r` | Go to Release Management |
| `Ctrl + F` | Focus search input |
| `Ctrl + N` | Create new release |
| `Esc` | Close dialogs / Clear filters |

## Common Tasks

### Create a New Release (Admin)
1. Navigate to Release Management (`r` key or nav)
2. Click "New Release" or press `Ctrl+N`
3. Enter version (e.g., "2.1.0") and date
4. Click "Create"
5. Click "Add Change" to add items to the release

### Import from Excel (Admin)
1. Go to Integrations page
2. Click "Excel Import/Export" tab
3. Click "Download Template" (first time)
4. Fill in the Excel file with your releases
5. Click "Choose File" and select your Excel
6. Click "Import from Excel"

### Filter Releases
1. On What's New page, click "Show Filters"
2. Select change type (Bug Fix, New Feature, Enhancement)
3. Select module (Import, Export, Packs, etc.)
4. Click "Apply Filters"
5. Press `Esc` to clear all filters quickly

### View Analytics (Admin)
1. Press `d` for Admin Dashboard, then click "Analytics"
2. Or click "Analytics" in the navigation
3. View summary cards, charts, and metrics
4. Scroll down for detailed breakdowns

## Features Overview

### What's New Page (All Users)
- ✅ Browse all releases
- ✅ Search functionality
- ✅ Filter by type and module
- ✅ View statistics
- ✅ Responsive design
- ✅ Keyboard shortcuts

### Release Management (Admin)
- ✅ Create/edit/delete releases
- ✅ Add/edit/delete changes
- ✅ Assign tags to changes
- ✅ Organize by version
- ✅ Expandable release cards

### Analytics (Admin)
- ✅ Total releases and changes
- ✅ Change type breakdown
- ✅ Release timeline chart
- ✅ Distribution charts
- ✅ Activity metrics

### Integrations (Admin)
- ✅ Excel import/export
- ✅ SQL Server connection
- ✅ Bulk data operations
- ✅ Data validation

### Tag Management (Admin)
- ✅ Module tags
- ✅ Change type tags
- ✅ Custom categories

## Tips & Tricks

### Efficiency Tips
1. **Use Keyboard Shortcuts**: Much faster than clicking
2. **Press Esc**: Quick way to close dialogs or clear filters
3. **Search + Filter**: Combine both for precise results
4. **Ctrl+F**: Instantly focus search from anywhere
5. **Take the Tour**: Click through the onboarding on first visit

### Best Practices
1. **Version Format**: Use semantic versioning (e.g., 2.1.0)
2. **Clear Descriptions**: Write detailed change descriptions
3. **Tag Appropriately**: Use relevant module tags
4. **Categorize Correctly**: Choose accurate change types
5. **Regular Updates**: Keep releases current

### Accessibility
- **Tab Navigation**: Use Tab to navigate, Enter to select
- **Skip Link**: Press Tab on page load to skip to content
- **Screen Reader**: Fully compatible with screen readers
- **High Contrast**: Supports high contrast modes
- **Keyboard Only**: Entire app usable without mouse

## Getting Help

### In-App Help
- **User Guide**: Click "Guide" button in navigation
- **Keyboard Shortcuts**: Click "Shortcuts" or press `Shift+?`
- **Onboarding Tour**: Available on first visit to What's New
- **Contextual Hints**: Look for hints in UI (e.g., placeholders)

### Common Issues

**Q: I can't see the Admin menu**
- A: Make sure you're logged in as `admin.user`

**Q: My filters aren't working**
- A: Click "Apply Filters" after selecting options
- Or press Esc to clear and try again

**Q: Excel import failed**
- A: Check that your Excel file matches the template format
- Ensure version numbers are in X.Y or X.Y.Z format

**Q: How do I reset everything?**
- A: Clear your browser's localStorage and refresh

## Sample Data

The application comes with pre-loaded sample data including:
- **3 Releases**: Versions 2.0.0, 2.1.0, and 2.2.0
- **Multiple Changes**: Bug fixes, features, and enhancements
- **8 Module Tags**: Import, Export, Packs, Systems, Security, Reports, Publisher, Dashboard
- **3 Change Types**: Bug Fix, New Feature, Enhancement

## Development Mode

This application runs in **local development mode** with:
- Mock API (no real backend)
- LocalStorage for data persistence
- Sample credentials (not secure)
- All data stored in browser

**For Production**: Integrate with real .NET Core backend and SQL Server database.

## Support

### Documentation
- `IMPLEMENTATION_SUMMARY.md` - Complete technical overview
- `FEATURES.md` - Detailed feature list
- This file - Quick start guide

### Keyboard Shortcuts Reference
Press `Shift + ?` anytime to see the complete shortcuts dialog.

---

**Ready to Start?**
1. Login with viewer or admin credentials
2. Take the onboarding tour
3. Explore the features
4. Press `Shift+?` to see all shortcuts

Enjoy using What's New! 🎉

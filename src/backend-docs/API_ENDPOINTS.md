# What's New API - Endpoint Documentation

## Base URL
```
Development: http://localhost:5000/api
Production: https://api.whatsnew.com/api
```

## Authentication
All endpoints except `/auth/login` require a JWT token in the Authorization header:
```
Authorization: Bearer {your_jwt_token}
```

---

## 📋 Table of Contents
1. [Authentication Endpoints](#authentication-endpoints)
2. [Release Endpoints](#release-endpoints)
3. [Change Endpoints](#change-endpoints)
4. [Tag Endpoints](#tag-endpoints)
5. [User Endpoints](#user-endpoints)
6. [Statistics Endpoints](#statistics-endpoints)

---

## 🔐 Authentication Endpoints

### POST /api/auth/login
Login and receive JWT token.

**Request Body:**
```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 1,
      "username": "admin",
      "email": "admin@whatsnew.com",
      "firstName": "Admin",
      "lastName": "User",
      "role": "admin",
      "lastLoginAt": "2024-01-15T10:30:00Z"
    },
    "expiresAt": "2024-01-15T18:30:00Z"
  }
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid username or password",
  "data": null,
  "errors": []
}
```

---

### GET /api/auth/me
Get current user information.

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "userId": 1,
    "username": "admin",
    "email": "admin@whatsnew.com",
    "firstName": "Admin",
    "lastName": "User",
    "role": "admin",
    "lastLoginAt": "2024-01-15T10:30:00Z"
  }
}
```

---

### POST /api/auth/change-password
Change current user's password.

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "currentPassword": "OldPassword@123",
  "newPassword": "NewPassword@123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully",
  "data": null
}
```

---

## 📦 Release Endpoints

### GET /api/releases
Get all releases.

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
- `includeUnpublished` (boolean, optional): Include unpublished releases (admin only). Default: false

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "releaseId": 1,
      "version": "2.1.0",
      "releaseDate": "2024-01-15",
      "description": "Major update with new features",
      "isPublished": true,
      "changeCount": 5,
      "createdByUsername": "admin",
      "createdAt": "2024-01-10T09:00:00Z",
      "updatedAt": "2024-01-10T09:00:00Z"
    }
  ]
}
```

---

### GET /api/releases/{id}
Get release by ID with all changes.

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "releaseId": 1,
    "version": "2.1.0",
    "releaseDate": "2024-01-15",
    "description": "Major update with new features",
    "isPublished": true,
    "changes": [
      {
        "changeId": 1,
        "releaseId": 1,
        "description": "Fixed login bug",
        "changeType": "bug_fix",
        "moduleTags": ["security", "dashboard"],
        "createdAt": "2024-01-10T09:00:00Z",
        "updatedAt": "2024-01-10T09:00:00Z"
      }
    ],
    "createdByUsername": "admin",
    "createdAt": "2024-01-10T09:00:00Z",
    "updatedAt": "2024-01-10T09:00:00Z"
  }
}
```

---

### POST /api/releases
Create new release (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "version": "2.1.0",
  "releaseDate": "2024-01-15",
  "description": "Major update with new features",
  "isPublished": false
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Release created successfully",
  "data": {
    "releaseId": 1,
    "version": "2.1.0",
    "releaseDate": "2024-01-15",
    "description": "Major update with new features",
    "isPublished": false,
    "changeCount": 0,
    "createdByUsername": "admin",
    "createdAt": "2024-01-10T09:00:00Z",
    "updatedAt": "2024-01-10T09:00:00Z"
  }
}
```

---

### PUT /api/releases/{id}
Update release (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "version": "2.1.0",
  "releaseDate": "2024-01-15",
  "description": "Updated description",
  "isPublished": true
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Release updated successfully",
  "data": null
}
```

---

### DELETE /api/releases/{id}
Delete release (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Release deleted successfully",
  "data": null
}
```

---

### GET /api/releases/statistics
Get release statistics (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "totalReleases": 10,
    "publishedReleases": 8,
    "totalChanges": 45,
    "bugFixes": 20,
    "newFeatures": 15,
    "enhancements": 10,
    "moduleStats": [
      {
        "moduleName": "Dashboard",
        "changeCount": 12
      },
      {
        "moduleName": "Security",
        "changeCount": 8
      }
    ]
  }
}
```

---

### POST /api/releases/import/excel
Import releases from Excel file (admin only).

**Headers:**
```
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

**Request Body:**
```
file: [Excel file]
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Import completed",
  "data": {
    "successCount": 25,
    "errorCount": 2,
    "errors": [
      "Row 5: Invalid version format",
      "Row 12: Missing required field 'description'"
    ],
    "warnings": [
      "Row 8: Tag 'custom_tag' was created automatically"
    ]
  }
}
```

---

### GET /api/releases/export/excel
Export releases to Excel file (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
- `includeUnpublished` (boolean, optional): Include unpublished releases. Default: true

**Response:**
Excel file download (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)

---

## 🔄 Change Endpoints

### POST /api/changes
Create new change (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "releaseId": 1,
  "description": "Fixed critical security vulnerability",
  "changeType": "bug_fix",
  "moduleTags": ["security", "dashboard"]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Change created successfully",
  "data": {
    "changeId": 1,
    "releaseId": 1,
    "description": "Fixed critical security vulnerability",
    "changeType": "bug_fix",
    "moduleTags": ["security", "dashboard"],
    "createdAt": "2024-01-10T09:00:00Z",
    "updatedAt": "2024-01-10T09:00:00Z"
  }
}
```

---

### PUT /api/changes/{id}
Update change (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "description": "Updated description",
  "changeType": "enhancement",
  "moduleTags": ["security", "reports"]
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Change updated successfully",
  "data": null
}
```

---

### DELETE /api/changes/{id}
Delete change (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Change deleted successfully",
  "data": null
}
```

---

## 🏷️ Tag Endpoints

### GET /api/tags
Get all tags.

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
- `activeOnly` (boolean, optional): Return only active tags. Default: true

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Success",
  "data": [
    {
      "tagId": 1,
      "value": "security",
      "label": "Security",
      "type": "module",
      "isActive": true
    },
    {
      "tagId": 2,
      "value": "dashboard",
      "label": "Dashboard",
      "type": "module",
      "isActive": true
    }
  ]
}
```

---

### POST /api/tags
Create tag (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "value": "custom_module",
  "label": "Custom Module",
  "type": "module"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Tag created successfully",
  "data": {
    "tagId": 9,
    "value": "custom_module",
    "label": "Custom Module",
    "type": "module",
    "isActive": true
  }
}
```

---

### PUT /api/tags/{id}
Update tag (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "label": "Updated Label",
  "isActive": true
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Tag updated successfully",
  "data": null
}
```

---

### DELETE /api/tags/{id}
Delete tag (admin only).

**Headers:**
```
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Tag deleted successfully",
  "data": null
}
```

---

## 📊 Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Invalid request",
  "data": null,
  "errors": [
    "Version is required",
    "Release date must be a valid date"
  ]
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Unauthorized access",
  "data": null,
  "errors": []
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "Access denied",
  "data": null,
  "errors": ["You don't have permission to access this resource"]
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found",
  "data": null,
  "errors": []
}
```

### 500 Internal Server Error
```json
{
  "message": "An internal server error occurred",
  "details": "Please contact support if the problem persists",
  "errors": [],
  "traceId": "0HMVFE42N93M5:00000001",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## 🔒 Authorization Matrix

| Endpoint | Viewer | Admin |
|----------|--------|-------|
| POST /api/auth/login | ✅ | ✅ |
| GET /api/auth/me | ✅ | ✅ |
| POST /api/auth/change-password | ✅ | ✅ |
| GET /api/releases | ✅ (published only) | ✅ (all) |
| GET /api/releases/{id} | ✅ (published only) | ✅ (all) |
| POST /api/releases | ❌ | ✅ |
| PUT /api/releases/{id} | ❌ | ✅ |
| DELETE /api/releases/{id} | ❌ | ✅ |
| GET /api/releases/statistics | ❌ | ✅ |
| POST /api/releases/import/excel | ❌ | ✅ |
| GET /api/releases/export/excel | ❌ | ✅ |
| POST /api/changes | ❌ | ✅ |
| PUT /api/changes/{id} | ❌ | ✅ |
| DELETE /api/changes/{id} | ❌ | ✅ |
| GET /api/tags | ✅ | ✅ |
| POST /api/tags | ❌ | ✅ |
| PUT /api/tags/{id} | ❌ | ✅ |
| DELETE /api/tags/{id} | ❌ | ✅ |

---

## 📝 Notes

1. **Date Format**: All dates should be in ISO 8601 format (YYYY-MM-DD or YYYY-MM-DDTHH:mm:ssZ)
2. **Change Types**: Valid values are `bug_fix`, `new_feature`, `enhancement`
3. **Tag Values**: Must be lowercase with underscores only (e.g., `custom_module`)
4. **Token Expiration**: JWT tokens expire after 8 hours (480 minutes) by default
5. **Rate Limiting**: Maximum 100 requests per minute per IP address
6. **File Upload**: Maximum file size for Excel import is 10MB

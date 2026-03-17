# What's New Application - Implementation Plan

## Features Overview

### Phase 1: Core Infrastructure & Authentication
- Database setup and connection
- User authentication and authorization
- User roles (Viewer/Admin)
- JWT token authentication

### Phase 2: Tag Management
- Create, Read, Update, Delete tags
- Tag types (module tags)
- Tag listing and filtering

### Phase 3: Release & Change Management
- Create, Read, Update, Delete releases
- Create, Read, Update, Delete changes within releases
- Release-Change relationships
- Change types (Bug Fix, New Feature, Enhancement)
- Module tag associations with changes

### Phase 4: Filtering & Display
- Get releases with filters
- Get changes by release
- Search and filter functionality

### Phase 5: Import & Integration (Future Enhancement)
- Excel/CSV import
- SQL Server integration for data import
- Bulk operations

---

## Implementation Steps per Phase

Each phase follows these steps:
1. Database Tables and Stored Procedures
2. DTOs (Data Transfer Objects)
3. Repository
4. Controller
5. Update Program.cs
6. Update Frontend API Service
7. Frontend Components

---

## Technology Stack

### Backend
- .NET Core 8.0 Web API
- SQL Server
- Entity Framework Core (optional, using ADO.NET for stored procedures)
- JWT Authentication

### Frontend
- React with TypeScript
- Axios for HTTP calls
- React Router for navigation

### Database
- SQL Server (LocalDB or Express for development)
- Stored Procedures for all data operations

---

## Database Schema

### Users Table
- UserId (PK)
- Name
- Email
- PasswordHash
- Role (Viewer/Admin)
- CreatedAt

### Tags Table
- TagId (PK)
- Label
- Value
- Type (module/changeType)
- CreatedAt

### Releases Table
- ReleaseId (PK)
- Version
- ReleaseDate
- CreatedAt
- UpdatedAt

### Changes Table
- ChangeId (PK)
- ReleaseId (FK)
- Description
- ChangeType (bug-fix/new-feature/enhancement)
- CreatedAt

### ChangeTags Table (Junction Table)
- ChangeTagId (PK)
- ChangeId (FK)
- TagId (FK)

### Integrations Table
- IntegrationId (PK)
- Name
- Host
- Port
- Database
- Username
- PasswordEncrypted
- Query
- Enabled
- LastSync
- CreatedAt

---

## API Endpoints

### Authentication
- POST /api/auth/login
- POST /api/auth/logout
- GET /api/auth/me

### Tags
- GET /api/tags
- GET /api/tags/{id}
- POST /api/tags
- PUT /api/tags/{id}
- DELETE /api/tags/{id}

### Releases
- GET /api/releases
- GET /api/releases/{id}
- POST /api/releases
- PUT /api/releases/{id}
- DELETE /api/releases/{id}

### Changes
- GET /api/changes/release/{releaseId}
- POST /api/changes
- PUT /api/changes/{id}
- DELETE /api/changes/{id}

### Integrations
- GET /api/integrations
- GET /api/integrations/{id}
- POST /api/integrations
- PUT /api/integrations/{id}
- DELETE /api/integrations/{id}
- POST /api/integrations/{id}/sync

---

## Current Status: Ready to implement Phase 1

-- ============================================
-- What's New Application - Database Schema
-- SQL Server 2019+
-- ============================================

-- ============================================
-- 1. CREATE DATABASE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WhatsNewDB')
BEGIN
    CREATE DATABASE WhatsNewDB;
END
GO

USE WhatsNewDB;
GO

-- ============================================
-- 2. TABLES
-- ============================================

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FirstName NVARCHAR(100),
        LastName NVARCHAR(100),
        Role NVARCHAR(50) NOT NULL DEFAULT 'viewer', -- 'admin' or 'viewer'
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        LastLoginAt DATETIME2 NULL,
        CONSTRAINT CK_User_Role CHECK (Role IN ('admin', 'viewer'))
    );
END
GO

-- Releases Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Releases')
BEGIN
    CREATE TABLE Releases (
        ReleaseId INT IDENTITY(1,1) PRIMARY KEY,
        Version NVARCHAR(50) NOT NULL UNIQUE,
        ReleaseDate DATE NOT NULL,
        Description NVARCHAR(MAX) NULL,
        IsPublished BIT NOT NULL DEFAULT 0,
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Releases_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
    );
END
GO

-- Changes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Changes')
BEGIN
    CREATE TABLE Changes (
        ChangeId INT IDENTITY(1,1) PRIMARY KEY,
        ReleaseId INT NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        ChangeType NVARCHAR(50) NOT NULL, -- 'bug_fix', 'new_feature', 'enhancement'
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Changes_ReleaseId FOREIGN KEY (ReleaseId) REFERENCES Releases(ReleaseId) ON DELETE CASCADE,
        CONSTRAINT FK_Changes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
        CONSTRAINT CK_Change_Type CHECK (ChangeType IN ('bug_fix', 'new_feature', 'enhancement'))
    );
END
GO

-- Tags Table (Module Tags)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tags')
BEGIN
    CREATE TABLE Tags (
        TagId INT IDENTITY(1,1) PRIMARY KEY,
        TagValue NVARCHAR(100) NOT NULL UNIQUE,
        TagLabel NVARCHAR(100) NOT NULL,
        TagType NVARCHAR(50) NOT NULL DEFAULT 'module', -- 'module' or 'custom'
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Change_Tags Junction Table (Many-to-Many)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Change_Tags')
BEGIN
    CREATE TABLE Change_Tags (
        ChangeId INT NOT NULL,
        TagId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        PRIMARY KEY (ChangeId, TagId),
        CONSTRAINT FK_ChangeTags_ChangeId FOREIGN KEY (ChangeId) REFERENCES Changes(ChangeId) ON DELETE CASCADE,
        CONSTRAINT FK_ChangeTags_TagId FOREIGN KEY (TagId) REFERENCES Tags(TagId) ON DELETE CASCADE
    );
END
GO

-- SQL Integration Settings Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SQLIntegrationSettings')
BEGIN
    CREATE TABLE SQLIntegrationSettings (
        SettingId INT IDENTITY(1,1) PRIMARY KEY,
        ConnectionString NVARCHAR(MAX) NOT NULL, -- Encrypted in production
        DatabaseName NVARCHAR(255) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        LastSyncAt DATETIME2 NULL,
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_SQLSettings_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
    );
END
GO

-- Audit Log Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
BEGIN
    CREATE TABLE AuditLogs (
        AuditId BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NULL,
        Action NVARCHAR(100) NOT NULL, -- 'CREATE', 'UPDATE', 'DELETE', 'LOGIN', etc.
        EntityType NVARCHAR(50) NOT NULL, -- 'Release', 'Change', 'Tag', 'User', etc.
        EntityId INT NULL,
        OldValue NVARCHAR(MAX) NULL,
        NewValue NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_AuditLogs_UserId FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

-- ============================================
-- 3. INDEXES FOR PERFORMANCE
-- ============================================

-- Users indexes
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Role ON Users(Role) WHERE IsActive = 1;

-- Releases indexes
CREATE NONCLUSTERED INDEX IX_Releases_Version ON Releases(Version);
CREATE NONCLUSTERED INDEX IX_Releases_ReleaseDate ON Releases(ReleaseDate DESC);
CREATE NONCLUSTERED INDEX IX_Releases_IsPublished ON Releases(IsPublished) WHERE IsPublished = 1;

-- Changes indexes
CREATE NONCLUSTERED INDEX IX_Changes_ReleaseId ON Changes(ReleaseId);
CREATE NONCLUSTERED INDEX IX_Changes_ChangeType ON Changes(ChangeType);

-- Tags indexes
CREATE NONCLUSTERED INDEX IX_Tags_TagValue ON Tags(TagValue);
CREATE NONCLUSTERED INDEX IX_Tags_IsActive ON Tags(IsActive) WHERE IsActive = 1;

-- Audit logs indexes
CREATE NONCLUSTERED INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_EntityType ON AuditLogs(EntityType, EntityId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt DESC);

GO

-- ============================================
-- 4. INSERT DEFAULT DATA
-- ============================================

-- Insert default tags
IF NOT EXISTS (SELECT * FROM Tags WHERE TagValue = 'import')
BEGIN
    INSERT INTO Tags (TagValue, TagLabel, TagType) VALUES
    ('import', 'Import', 'module'),
    ('export', 'Export', 'module'),
    ('packs', 'Packs', 'module'),
    ('systems', 'Systems', 'module'),
    ('security', 'Security', 'module'),
    ('reports', 'Reports', 'module'),
    ('publisher', 'Publisher', 'module'),
    ('dashboard', 'Dashboard', 'module');
END
GO

-- Insert default users (passwords need to be hashed in application)
-- Note: These are example passwords - MUST be hashed with BCrypt/PBKDF2 in .NET
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    -- Password: Admin@123 (MUST BE HASHED IN APPLICATION)
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role) VALUES
    ('admin', 'admin@whatsnew.com', 'HASH_ME_IN_APPLICATION', 'Admin', 'User', 'admin');
END
GO

IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'john.viewer')
BEGIN
    -- Password: Viewer@123 (MUST BE HASHED IN APPLICATION)
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role) VALUES
    ('john.viewer', 'john@whatsnew.com', 'HASH_ME_IN_APPLICATION', 'John', 'Viewer', 'viewer');
END
GO

PRINT 'Database schema created successfully!';

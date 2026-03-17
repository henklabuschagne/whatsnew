-- =============================================
-- What's New Application - Database Tables
-- =============================================

USE master;
GO

-- Create Database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'WhatsNewDB')
BEGIN
    CREATE DATABASE WhatsNewDB;
END
GO

USE WhatsNewDB;
GO

-- =============================================
-- Users Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL CHECK (Role IN ('viewer', 'admin')),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CONSTRAINT CK_Users_Email CHECK (Email LIKE '%@%')
    );
END
GO

-- =============================================
-- Tags Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tags')
BEGIN
    CREATE TABLE Tags (
        TagId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Label NVARCHAR(100) NOT NULL,
        Value NVARCHAR(100) NOT NULL UNIQUE,
        Type NVARCHAR(50) NOT NULL CHECK (Type IN ('module', 'changeType')),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- =============================================
-- Releases Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Releases')
BEGIN
    CREATE TABLE Releases (
        ReleaseId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Version NVARCHAR(50) NOT NULL UNIQUE,
        ReleaseDate DATE NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- =============================================
-- Changes Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Changes')
BEGIN
    CREATE TABLE Changes (
        ChangeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ReleaseId UNIQUEIDENTIFIER NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        ChangeType NVARCHAR(50) NOT NULL CHECK (ChangeType IN ('bug-fix', 'new-feature', 'enhancement')),
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Changes_Releases FOREIGN KEY (ReleaseId) REFERENCES Releases(ReleaseId) ON DELETE CASCADE
    );
END
GO

-- =============================================
-- ChangeTags Junction Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChangeTags')
BEGIN
    CREATE TABLE ChangeTags (
        ChangeTagId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ChangeId UNIQUEIDENTIFIER NOT NULL,
        TagId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT FK_ChangeTags_Changes FOREIGN KEY (ChangeId) REFERENCES Changes(ChangeId) ON DELETE CASCADE,
        CONSTRAINT FK_ChangeTags_Tags FOREIGN KEY (TagId) REFERENCES Tags(TagId) ON DELETE CASCADE,
        CONSTRAINT UQ_ChangeTags_ChangeTag UNIQUE (ChangeId, TagId)
    );
END
GO

-- =============================================
-- Integrations Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Integrations')
BEGIN
    CREATE TABLE Integrations (
        IntegrationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        Host NVARCHAR(255) NOT NULL,
        Port NVARCHAR(10) NOT NULL DEFAULT '1433',
        DatabaseName NVARCHAR(100) NOT NULL,
        Username NVARCHAR(100),
        PasswordEncrypted NVARCHAR(500),
        Query NVARCHAR(MAX),
        Enabled BIT DEFAULT 1,
        LastSync DATETIME2,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- =============================================
-- Create Indexes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Changes_ReleaseId')
    CREATE INDEX IX_Changes_ReleaseId ON Changes(ReleaseId);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChangeTags_ChangeId')
    CREATE INDEX IX_ChangeTags_ChangeId ON ChangeTags(ChangeId);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChangeTags_TagId')
    CREATE INDEX IX_ChangeTags_TagId ON ChangeTags(TagId);
GO

PRINT 'Database tables created successfully';
GO

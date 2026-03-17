-- =============================================
-- SQL Integration Tables
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- Table: SqlConnections
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SqlConnections]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SqlConnections] (
        [ConnectionId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL,
        [Server] NVARCHAR(255) NOT NULL,
        [Database] NVARCHAR(100) NOT NULL,
        [Username] NVARCHAR(100),
        [Password] NVARCHAR(255),
        [UseIntegratedSecurity] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
    
    PRINT 'Table SqlConnections created successfully';
END
ELSE
BEGIN
    PRINT 'Table SqlConnections already exists';
END
GO

-- =============================================
-- Table: SqlQueries
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SqlQueries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SqlQueries] (
        [QueryId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [ConnectionId] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(MAX),
        [QueryText] NVARCHAR(MAX) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_SqlQueries_SqlConnections FOREIGN KEY (ConnectionId) 
            REFERENCES SqlConnections(ConnectionId) ON DELETE CASCADE
    );
    
    PRINT 'Table SqlQueries created successfully';
END
ELSE
BEGIN
    PRINT 'Table SqlQueries already exists';
END
GO

PRINT 'SQL Integration tables setup completed';
GO

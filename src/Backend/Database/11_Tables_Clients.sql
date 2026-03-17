-- =============================================
-- Clients Table - Store client/customer information
-- =============================================

-- Create Clients table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clients')
BEGIN
    CREATE TABLE Clients (
        ClientId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(255) NOT NULL,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        ContactEmail NVARCHAR(255) NULL,
        ContactPhone NVARCHAR(50) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UK_Clients_Code UNIQUE (Code)
    );

    PRINT 'Clients table created successfully';
END
ELSE
BEGIN
    PRINT 'Clients table already exists';
END
GO

-- Create index on Code for quick lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_Code')
BEGIN
    CREATE INDEX IX_Clients_Code ON Clients(Code);
    PRINT 'Index IX_Clients_Code created successfully';
END
GO

-- Create index on IsActive for filtering
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_IsActive')
BEGIN
    CREATE INDEX IX_Clients_IsActive ON Clients(IsActive);
    PRINT 'Index IX_Clients_IsActive created successfully';
END
GO

-- =============================================
-- Add Client tracking columns to Changes table
-- =============================================

-- Add ClientId column to Changes table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Changes') AND name = 'ClientId')
BEGIN
    ALTER TABLE Changes
    ADD ClientId UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_Changes_Clients FOREIGN KEY (ClientId) REFERENCES Clients(ClientId);
    
    PRINT 'ClientId column added to Changes table';
END
ELSE
BEGIN
    PRINT 'ClientId column already exists in Changes table';
END
GO

-- Add TicketNumber column to Changes table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Changes') AND name = 'TicketNumber')
BEGIN
    ALTER TABLE Changes
    ADD TicketNumber NVARCHAR(100) NULL;
    
    PRINT 'TicketNumber column added to Changes table';
END
ELSE
BEGIN
    PRINT 'TicketNumber column already exists in Changes table';
END
GO

-- Add DevOpsNumber column to Changes table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Changes') AND name = 'DevOpsNumber')
BEGIN
    ALTER TABLE Changes
    ADD DevOpsNumber NVARCHAR(100) NULL;
    
    PRINT 'DevOpsNumber column added to Changes table';
END
ELSE
BEGIN
    PRINT 'DevOpsNumber column already exists in Changes table';
END
GO

-- Create index on ClientId for filtering
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Changes_ClientId')
BEGIN
    CREATE INDEX IX_Changes_ClientId ON Changes(ClientId);
    PRINT 'Index IX_Changes_ClientId created successfully';
END
GO

-- Create index on TicketNumber for quick lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Changes_TicketNumber')
BEGIN
    CREATE INDEX IX_Changes_TicketNumber ON Changes(TicketNumber);
    PRINT 'Index IX_Changes_TicketNumber created successfully';
END
GO

-- Create index on DevOpsNumber for quick lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Changes_DevOpsNumber')
BEGIN
    CREATE INDEX IX_Changes_DevOpsNumber ON Changes(DevOpsNumber);
    PRINT 'Index IX_Changes_DevOpsNumber created successfully';
END
GO

-- =============================================
-- Time To Action Tracking Table
-- =============================================

-- Create TimeToAction table to track workflow stages
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TimeToAction')
BEGIN
    CREATE TABLE TimeToAction (
        TimeToActionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        ChangeId UNIQUEIDENTIFIER NOT NULL,
        SubmittedDate DATETIME2 NULL,
        DevelopedDate DATETIME2 NULL,
        TestedDate DATETIME2 NULL,
        ReleasedDate DATETIME2 NULL,
        TotalDays AS DATEDIFF(DAY, SubmittedDate, ReleasedDate) PERSISTED,
        DevDays AS DATEDIFF(DAY, SubmittedDate, DevelopedDate) PERSISTED,
        TestDays AS DATEDIFF(DAY, DevelopedDate, TestedDate) PERSISTED,
        ReleaseDays AS DATEDIFF(DAY, TestedDate, ReleasedDate) PERSISTED,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_TimeToAction_Changes FOREIGN KEY (ChangeId) REFERENCES Changes(ChangeId) ON DELETE CASCADE,
        CONSTRAINT UK_TimeToAction_ChangeId UNIQUE (ChangeId)
    );

    PRINT 'TimeToAction table created successfully';
END
ELSE
BEGIN
    PRINT 'TimeToAction table already exists';
END
GO

-- Create index on ChangeId for quick lookup
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TimeToAction_ChangeId')
BEGIN
    CREATE INDEX IX_TimeToAction_ChangeId ON TimeToAction(ChangeId);
    PRINT 'Index IX_TimeToAction_ChangeId created successfully';
END
GO

-- Create index on SubmittedDate for date range queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TimeToAction_SubmittedDate')
BEGIN
    CREATE INDEX IX_TimeToAction_SubmittedDate ON TimeToAction(SubmittedDate);
    PRINT 'Index IX_TimeToAction_SubmittedDate created successfully';
END
GO

PRINT 'All client tracking tables and columns created successfully';
GO

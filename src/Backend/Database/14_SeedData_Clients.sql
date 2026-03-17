-- =============================================
-- Seed Data for Clients and Time To Action
-- =============================================

-- Insert sample clients
PRINT 'Inserting sample clients...';

DECLARE @Client1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Client3Id UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM Clients WHERE Code = 'ACME')
BEGIN
    INSERT INTO Clients (ClientId, Name, Code, ContactEmail, ContactPhone, IsActive, CreatedAt, UpdatedAt)
    VALUES 
    (@Client1Id, 'Acme Corporation', 'ACME', 'contact@acmecorp.com', '+1-555-0100', 1, GETUTCDATE(), GETUTCDATE());
    PRINT 'Acme Corporation inserted';
END

IF NOT EXISTS (SELECT 1 FROM Clients WHERE Code = 'GTS')
BEGIN
    INSERT INTO Clients (ClientId, Name, Code, ContactEmail, ContactPhone, IsActive, CreatedAt, UpdatedAt)
    VALUES 
    (@Client2Id, 'Global Tech Solutions', 'GTS', 'info@globaltech.com', '+1-555-0200', 1, GETUTCDATE(), GETUTCDATE());
    PRINT 'Global Tech Solutions inserted';
END

IF NOT EXISTS (SELECT 1 FROM Clients WHERE Code = 'INNOVLAB')
BEGIN
    INSERT INTO Clients (ClientId, Name, Code, ContactEmail, ContactPhone, IsActive, CreatedAt, UpdatedAt)
    VALUES 
    (@Client3Id, 'Innovation Labs', 'INNOVLAB', 'hello@innovationlabs.io', '+1-555-0300', 1, GETUTCDATE(), GETUTCDATE());
    PRINT 'Innovation Labs inserted';
END

-- Update existing changes with client tracking information
PRINT 'Updating changes with client information...';

-- Get client IDs
DECLARE @AcmeId UNIQUEIDENTIFIER = (SELECT ClientId FROM Clients WHERE Code = 'ACME');
DECLARE @GTSId UNIQUEIDENTIFIER = (SELECT ClientId FROM Clients WHERE Code = 'GTS');
DECLARE @InnovLabId UNIQUEIDENTIFIER = (SELECT ClientId FROM Clients WHERE Code = 'INNOVLAB');

-- Update some existing changes with client info
UPDATE TOP (5) Changes 
SET 
    ClientId = @AcmeId,
    TicketNumber = 'TICKET-' + CAST(10000 + ABS(CHECKSUM(NEWID()) % 10000) AS NVARCHAR),
    DevOpsNumber = 'DEVOPS-' + CAST(10000 + ABS(CHECKSUM(NEWID()) % 10000) AS NVARCHAR)
WHERE ClientId IS NULL AND ChangeType = 'bug-fix';

UPDATE TOP (4) Changes 
SET 
    ClientId = @GTSId,
    TicketNumber = 'TICKET-' + CAST(10000 + ABS(CHECKSUM(NEWID()) % 10000) AS NVARCHAR),
    DevOpsNumber = 'DEVOPS-' + CAST(10000 + ABS(CHECKSUM(NEWID()) % 10000) AS NVARCHAR)
WHERE ClientId IS NULL AND ChangeType = 'enhancement';

UPDATE TOP (3) Changes 
SET 
    ClientId = @InnovLabId,
    TicketNumber = 'TICKET-' + CAST(10000 + ABS(CHECKSUM(NEWID()) % 10000) AS NVARCHAR),
    DevOpsNumber = 'DEVOPS-' + CAST(10000 + ABS(CHECKSUM(NEWID()) % 10000) AS NVARCHAR)
WHERE ClientId IS NULL AND ChangeType = 'new-feature';

PRINT 'Changes updated with client information';

-- Insert Time To Action data for existing changes
PRINT 'Inserting Time To Action data...';

-- Insert time tracking for all changes
INSERT INTO TimeToAction (TimeToActionId, ChangeId, SubmittedDate, DevelopedDate, TestedDate, ReleasedDate, CreatedAt, UpdatedAt)
SELECT 
    NEWID() AS TimeToActionId,
    c.ChangeId,
    DATEADD(DAY, -CAST(RAND(CHECKSUM(NEWID())) * 30 AS INT), c.CreatedAt) AS SubmittedDate,
    DATEADD(DAY, -CAST(RAND(CHECKSUM(NEWID())) * 20 AS INT), c.CreatedAt) AS DevelopedDate,
    DATEADD(DAY, -CAST(RAND(CHECKSUM(NEWID())) * 10 AS INT), c.CreatedAt) AS TestedDate,
    c.CreatedAt AS ReleasedDate,
    GETUTCDATE() AS CreatedAt,
    GETUTCDATE() AS UpdatedAt
FROM Changes c
WHERE NOT EXISTS (SELECT 1 FROM TimeToAction WHERE ChangeId = c.ChangeId);

PRINT 'Time To Action data inserted';

-- Generate realistic time progression data
UPDATE TimeToAction
SET 
    SubmittedDate = CASE 
        WHEN SubmittedDate > ReleasedDate THEN DATEADD(DAY, -30, ReleasedDate)
        ELSE SubmittedDate 
    END,
    DevelopedDate = CASE 
        WHEN DevelopedDate > ReleasedDate THEN DATEADD(DAY, -20, ReleasedDate)
        WHEN DevelopedDate < SubmittedDate THEN DATEADD(DAY, 3, SubmittedDate)
        ELSE DevelopedDate 
    END,
    TestedDate = CASE 
        WHEN TestedDate > ReleasedDate THEN DATEADD(DAY, -5, ReleasedDate)
        WHEN TestedDate < DevelopedDate THEN DATEADD(DAY, 2, DevelopedDate)
        ELSE TestedDate 
    END;

PRINT 'Time To Action dates normalized';

-- Display summary
SELECT 
    'Clients' AS TableName,
    COUNT(*) AS RecordCount
FROM Clients
UNION ALL
SELECT 
    'Changes with Client Info' AS TableName,
    COUNT(*) AS RecordCount
FROM Changes
WHERE ClientId IS NOT NULL
UNION ALL
SELECT 
    'Time To Action Records' AS TableName,
    COUNT(*) AS RecordCount
FROM TimeToAction;

PRINT 'Seed data for clients and time tracking completed successfully';
GO

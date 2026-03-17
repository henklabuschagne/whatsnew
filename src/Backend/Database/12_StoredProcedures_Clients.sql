-- =============================================
-- Stored Procedures for Client Management
-- =============================================

-- =============================================
-- SP: Get All Clients
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllClients')
    DROP PROCEDURE sp_GetAllClients;
GO

CREATE PROCEDURE sp_GetAllClients
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ClientId,
        Name,
        Code,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE (@IncludeInactive = 1 OR IsActive = 1)
    ORDER BY Name;
END
GO

-- =============================================
-- SP: Get Client By Id
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetClientById')
    DROP PROCEDURE sp_GetClientById;
GO

CREATE PROCEDURE sp_GetClientById
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ClientId,
        Name,
        Code,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE ClientId = @ClientId;
END
GO

-- =============================================
-- SP: Get Client By Code
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetClientByCode')
    DROP PROCEDURE sp_GetClientByCode;
GO

CREATE PROCEDURE sp_GetClientByCode
    @Code NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ClientId,
        Name,
        Code,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE Code = @Code;
END
GO

-- =============================================
-- SP: Create Client
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateClient')
    DROP PROCEDURE sp_CreateClient;
GO

CREATE PROCEDURE sp_CreateClient
    @Name NVARCHAR(255),
    @Code NVARCHAR(50),
    @ContactEmail NVARCHAR(255) = NULL,
    @ContactPhone NVARCHAR(50) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ClientId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME2 = GETUTCDATE();

    -- Check if code already exists
    IF EXISTS (SELECT 1 FROM Clients WHERE Code = @Code)
    BEGIN
        RAISERROR('Client code already exists', 16, 1);
        RETURN;
    END

    INSERT INTO Clients (ClientId, Name, Code, ContactEmail, ContactPhone, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ClientId, @Name, @Code, @ContactEmail, @ContactPhone, @IsActive, @Now, @Now);

    SELECT 
        ClientId,
        Name,
        Code,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE ClientId = @ClientId;
END
GO

-- =============================================
-- SP: Update Client
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_UpdateClient')
    DROP PROCEDURE sp_UpdateClient;
GO

CREATE PROCEDURE sp_UpdateClient
    @ClientId UNIQUEIDENTIFIER,
    @Name NVARCHAR(255),
    @Code NVARCHAR(50),
    @ContactEmail NVARCHAR(255) = NULL,
    @ContactPhone NVARCHAR(50) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if client exists
    IF NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = @ClientId)
    BEGIN
        RAISERROR('Client not found', 16, 1);
        RETURN;
    END

    -- Check if code already exists for a different client
    IF EXISTS (SELECT 1 FROM Clients WHERE Code = @Code AND ClientId != @ClientId)
    BEGIN
        RAISERROR('Client code already exists', 16, 1);
        RETURN;
    END

    UPDATE Clients
    SET 
        Name = @Name,
        Code = @Code,
        ContactEmail = @ContactEmail,
        ContactPhone = @ContactPhone,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE ClientId = @ClientId;

    SELECT 
        ClientId,
        Name,
        Code,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE ClientId = @ClientId;
END
GO

-- =============================================
-- SP: Delete Client
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_DeleteClient')
    DROP PROCEDURE sp_DeleteClient;
GO

CREATE PROCEDURE sp_DeleteClient
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if client exists
    IF NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = @ClientId)
    BEGIN
        RAISERROR('Client not found', 16, 1);
        RETURN;
    END

    -- Check if client has associated changes
    IF EXISTS (SELECT 1 FROM Changes WHERE ClientId = @ClientId)
    BEGIN
        -- Soft delete by setting IsActive to 0 and nullifying references
        UPDATE Changes SET ClientId = NULL WHERE ClientId = @ClientId;
        UPDATE Clients SET IsActive = 0, UpdatedAt = GETUTCDATE() WHERE ClientId = @ClientId;
    END
    ELSE
    BEGIN
        -- Hard delete if no references
        DELETE FROM Clients WHERE ClientId = @ClientId;
    END
END
GO

-- =============================================
-- SP: Get Client Statistics
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetClientStatistics')
    DROP PROCEDURE sp_GetClientStatistics;
GO

CREATE PROCEDURE sp_GetClientStatistics
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ClientId,
        c.Name,
        c.Code,
        COUNT(ch.ChangeId) AS TotalChanges,
        SUM(CASE WHEN ch.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixes,
        SUM(CASE WHEN ch.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS Enhancements,
        SUM(CASE WHEN ch.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatures,
        MIN(ch.CreatedAt) AS FirstChangeDate,
        MAX(ch.CreatedAt) AS LastChangeDate
    FROM Clients c
    LEFT JOIN Changes ch ON c.ClientId = ch.ClientId
    WHERE c.ClientId = @ClientId
    GROUP BY c.ClientId, c.Name, c.Code;
END
GO

PRINT 'Client stored procedures created successfully';
GO

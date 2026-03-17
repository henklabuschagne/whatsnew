-- =============================================
-- MIGRATION: Add Description Field to Clients Table
-- Date: February 4, 2026
-- Purpose: Fix alignment issue - Frontend and DTOs have Description but table doesn't
-- =============================================

USE WhatsNewDB;
GO

PRINT '========================================';
PRINT 'MIGRATION: Adding Description to Clients';
PRINT '========================================';
PRINT '';

-- =============================================
-- STEP 1: Add Description Column to Clients Table
-- =============================================
PRINT 'Step 1: Adding Description column to Clients table...';

IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') 
    AND name = 'Description'
)
BEGIN
    ALTER TABLE Clients 
    ADD Description NVARCHAR(MAX) NULL;
    
    PRINT '✓ Description column added successfully';
END
ELSE
BEGIN
    PRINT '⚠ Description column already exists - skipping';
END
GO

PRINT '';

-- =============================================
-- STEP 2: Update sp_GetAllClients
-- =============================================
PRINT 'Step 2: Updating sp_GetAllClients...';

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
        Description,          -- ✓ NOW INCLUDED
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE (@IncludeInactive = 1 OR IsActive = 1)
    ORDER BY Name ASC;
END
GO

PRINT '✓ sp_GetAllClients updated successfully';
PRINT '';

-- =============================================
-- STEP 3: Update sp_GetClientById
-- =============================================
PRINT 'Step 3: Updating sp_GetClientById...';

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
        Description,          -- ✓ NOW INCLUDED
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE ClientId = @ClientId;
END
GO

PRINT '✓ sp_GetClientById updated successfully';
PRINT '';

-- =============================================
-- STEP 4: Update sp_GetClientByCode
-- =============================================
PRINT 'Step 4: Updating sp_GetClientByCode...';

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
        Description,          -- ✓ NOW INCLUDED
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE Code = @Code;
END
GO

PRINT '✓ sp_GetClientByCode updated successfully';
PRINT '';

-- =============================================
-- STEP 5: Update sp_CreateClient
-- =============================================
PRINT 'Step 5: Updating sp_CreateClient...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateClient')
    DROP PROCEDURE sp_CreateClient;
GO

CREATE PROCEDURE sp_CreateClient
    @Name NVARCHAR(255),
    @Code NVARCHAR(50),
    @Description NVARCHAR(MAX) = NULL,        -- ✓ NOW INCLUDED
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
        RAISERROR('A client with this code already exists', 16, 1);
        RETURN;
    END

    INSERT INTO Clients (
        ClientId,
        Name,
        Code,
        Description,          -- ✓ NOW INCLUDED
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    )
    VALUES (
        @ClientId,
        @Name,
        @Code,
        @Description,         -- ✓ NOW INCLUDED
        @ContactEmail,
        @ContactPhone,
        @IsActive,
        @Now,
        @Now
    );

    -- Return the created client
    SELECT 
        ClientId,
        Name,
        Code,
        Description,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE ClientId = @ClientId;
END
GO

PRINT '✓ sp_CreateClient updated successfully';
PRINT '';

-- =============================================
-- STEP 6: Update sp_UpdateClient
-- =============================================
PRINT 'Step 6: Updating sp_UpdateClient...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_UpdateClient')
    DROP PROCEDURE sp_UpdateClient;
GO

CREATE PROCEDURE sp_UpdateClient
    @ClientId UNIQUEIDENTIFIER,
    @Name NVARCHAR(255),
    @Code NVARCHAR(50),
    @Description NVARCHAR(MAX) = NULL,        -- ✓ NOW INCLUDED
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

    -- Check if code already exists for another client
    IF EXISTS (SELECT 1 FROM Clients WHERE Code = @Code AND ClientId != @ClientId)
    BEGIN
        RAISERROR('A client with this code already exists', 16, 1);
        RETURN;
    END

    UPDATE Clients
    SET 
        Name = @Name,
        Code = @Code,
        Description = @Description,           -- ✓ NOW INCLUDED
        ContactEmail = @ContactEmail,
        ContactPhone = @ContactPhone,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE ClientId = @ClientId;

    -- Return the updated client
    SELECT 
        ClientId,
        Name,
        Code,
        Description,
        ContactEmail,
        ContactPhone,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Clients
    WHERE ClientId = @ClientId;
END
GO

PRINT '✓ sp_UpdateClient updated successfully';
PRINT '';

-- =============================================
-- VERIFICATION
-- =============================================
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';

-- Check column exists
IF EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') 
    AND name = 'Description'
)
BEGIN
    PRINT '✓ Description column exists in Clients table';
END
ELSE
BEGIN
    PRINT '✗ ERROR: Description column not found!';
END

-- Check stored procedures
DECLARE @SPCount INT = 0;

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllClients')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetClientById')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetClientByCode')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateClient')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_UpdateClient')
    SET @SPCount = @SPCount + 1;

PRINT '✓ ' + CAST(@SPCount AS VARCHAR) + '/5 stored procedures updated';

PRINT '';
PRINT '========================================';
PRINT 'MIGRATION COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Summary:';
PRINT '- Added Description column to Clients table';
PRINT '- Updated 5 stored procedures to include Description';
PRINT '- Frontend and backend now fully aligned';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Test Client creation with description';
PRINT '2. Test Client update with description';
PRINT '3. Verify description appears in UI';
GO

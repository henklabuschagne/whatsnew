-- =============================================
-- Change Management Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get Changes by Release ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetChangesByReleaseId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetChangesByReleaseId]
GO

CREATE PROCEDURE [dbo].[sp_GetChangesByReleaseId]
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ChangeId,
        c.ReleaseId,
        c.Description,
        c.ChangeType,
        c.CreatedAt,
        c.ClientId,
        c.TicketNumber,
        c.DevOpsNumber,
        STRING_AGG(ct.TagId, ',') AS TagIds
    FROM Changes c
    LEFT JOIN ChangeTags ct ON c.ChangeId = ct.ChangeId
    WHERE c.ReleaseId = @ReleaseId
    GROUP BY c.ChangeId, c.ReleaseId, c.Description, c.ChangeType, c.CreatedAt, c.ClientId, c.TicketNumber, c.DevOpsNumber
    ORDER BY c.CreatedAt DESC;
END
GO

-- =============================================
-- SP: Get Change by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetChangeById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetChangeById]
GO

CREATE PROCEDURE [dbo].[sp_GetChangeById]
    @ChangeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ChangeId,
        c.ReleaseId,
        c.Description,
        c.ChangeType,
        c.CreatedAt,
        c.ClientId,
        c.TicketNumber,
        c.DevOpsNumber,
        STRING_AGG(ct.TagId, ',') AS TagIds
    FROM Changes c
    LEFT JOIN ChangeTags ct ON c.ChangeId = ct.ChangeId
    WHERE c.ChangeId = @ChangeId
    GROUP BY c.ChangeId, c.ReleaseId, c.Description, c.ChangeType, c.CreatedAt, c.ClientId, c.TicketNumber, c.DevOpsNumber;
END
GO

-- =============================================
-- SP: Create Change
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateChange]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CreateChange]
GO

CREATE PROCEDURE [dbo].[sp_CreateChange]
    @ChangeId UNIQUEIDENTIFIER OUTPUT,
    @ReleaseId UNIQUEIDENTIFIER,
    @Description NVARCHAR(MAX),
    @ChangeType NVARCHAR(50),
    @TagIds NVARCHAR(MAX) = NULL,
    @ClientId UNIQUEIDENTIFIER = NULL,
    @TicketNumber NVARCHAR(100) = NULL,
    @DevOpsNumber NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if release exists
    IF NOT EXISTS (SELECT 1 FROM Releases WHERE ReleaseId = @ReleaseId)
    BEGIN
        RAISERROR('Release not found', 16, 1);
        RETURN;
    END
    
    SET @ChangeId = NEWID();
    
    -- Insert change with client tracking fields
    INSERT INTO Changes (ChangeId, ReleaseId, Description, ChangeType, ClientId, TicketNumber, DevOpsNumber)
    VALUES (@ChangeId, @ReleaseId, @Description, @ChangeType, @ClientId, @TicketNumber, @DevOpsNumber);
    
    -- Insert tags if provided
    IF @TagIds IS NOT NULL AND LEN(@TagIds) > 0
    BEGIN
        INSERT INTO ChangeTags (ChangeId, TagId)
        SELECT @ChangeId, value
        FROM STRING_SPLIT(@TagIds, ',')
        WHERE TRIM(value) != '';
    END
    
    -- Return the created change
    EXEC sp_GetChangeById @ChangeId;
END
GO

-- =============================================
-- SP: Update Change
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateChange]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateChange]
GO

CREATE PROCEDURE [dbo].[sp_UpdateChange]
    @ChangeId UNIQUEIDENTIFIER,
    @Description NVARCHAR(MAX),
    @ChangeType NVARCHAR(50),
    @TagIds NVARCHAR(MAX) = NULL,
    @ClientId UNIQUEIDENTIFIER = NULL,
    @TicketNumber NVARCHAR(100) = NULL,
    @DevOpsNumber NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if change exists
    IF NOT EXISTS (SELECT 1 FROM Changes WHERE ChangeId = @ChangeId)
    BEGIN
        RAISERROR('Change not found', 16, 1);
        RETURN;
    END
    
    -- Update change with client tracking fields
    UPDATE Changes
    SET 
        Description = @Description,
        ChangeType = @ChangeType,
        ClientId = @ClientId,
        TicketNumber = @TicketNumber,
        DevOpsNumber = @DevOpsNumber
    WHERE ChangeId = @ChangeId;
    
    -- Remove existing tags
    DELETE FROM ChangeTags WHERE ChangeId = @ChangeId;
    
    -- Insert new tags if provided
    IF @TagIds IS NOT NULL AND LEN(@TagIds) > 0
    BEGIN
        INSERT INTO ChangeTags (ChangeId, TagId)
        SELECT @ChangeId, value
        FROM STRING_SPLIT(@TagIds, ',')
        WHERE TRIM(value) != '';
    END
    
    -- Return the updated change
    EXEC sp_GetChangeById @ChangeId;
END
GO

-- =============================================
-- SP: Delete Change
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteChange]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteChange]
GO

CREATE PROCEDURE [dbo].[sp_DeleteChange]
    @ChangeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if change exists
    IF NOT EXISTS (SELECT 1 FROM Changes WHERE ChangeId = @ChangeId)
    BEGIN
        RAISERROR('Change not found', 16, 1);
        RETURN;
    END
    
    -- Delete change (cascade will delete associated tags)
    DELETE FROM Changes
    WHERE ChangeId = @ChangeId;
    
    SELECT 1 AS Success;
END
GO

PRINT 'Change stored procedures created successfully';
GO
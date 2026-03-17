-- =============================================
-- Tag Management Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get All Tags
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllTags]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetAllTags]
GO

CREATE PROCEDURE [dbo].[sp_GetAllTags]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TagId,
        Label,
        Value,
        Type,
        CreatedAt
    FROM Tags
    ORDER BY Type, Label;
END
GO

-- =============================================
-- SP: Get Tag by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetTagById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetTagById]
GO

CREATE PROCEDURE [dbo].[sp_GetTagById]
    @TagId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TagId,
        Label,
        Value,
        Type,
        CreatedAt
    FROM Tags
    WHERE TagId = @TagId;
END
GO

-- =============================================
-- SP: Get Tags by Type
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetTagsByType]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetTagsByType]
GO

CREATE PROCEDURE [dbo].[sp_GetTagsByType]
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TagId,
        Label,
        Value,
        Type,
        CreatedAt
    FROM Tags
    WHERE Type = @Type
    ORDER BY Label;
END
GO

-- =============================================
-- SP: Create Tag
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateTag]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CreateTag]
GO

CREATE PROCEDURE [dbo].[sp_CreateTag]
    @TagId UNIQUEIDENTIFIER OUTPUT,
    @Label NVARCHAR(100),
    @Value NVARCHAR(100),
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if value already exists
    IF EXISTS (SELECT 1 FROM Tags WHERE Value = @Value)
    BEGIN
        RAISERROR('Tag with this value already exists', 16, 1);
        RETURN;
    END
    
    SET @TagId = NEWID();
    
    INSERT INTO Tags (TagId, Label, Value, Type)
    VALUES (@TagId, @Label, @Value, @Type);
    
    SELECT 
        TagId,
        Label,
        Value,
        Type,
        CreatedAt
    FROM Tags
    WHERE TagId = @TagId;
END
GO

-- =============================================
-- SP: Update Tag
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateTag]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateTag]
GO

CREATE PROCEDURE [dbo].[sp_UpdateTag]
    @TagId UNIQUEIDENTIFIER,
    @Label NVARCHAR(100),
    @Value NVARCHAR(100),
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if tag exists
    IF NOT EXISTS (SELECT 1 FROM Tags WHERE TagId = @TagId)
    BEGIN
        RAISERROR('Tag not found', 16, 1);
        RETURN;
    END
    
    -- Check if value already exists for a different tag
    IF EXISTS (SELECT 1 FROM Tags WHERE Value = @Value AND TagId != @TagId)
    BEGIN
        RAISERROR('Tag with this value already exists', 16, 1);
        RETURN;
    END
    
    UPDATE Tags
    SET 
        Label = @Label,
        Value = @Value,
        Type = @Type
    WHERE TagId = @TagId;
    
    SELECT 
        TagId,
        Label,
        Value,
        Type,
        CreatedAt
    FROM Tags
    WHERE TagId = @TagId;
END
GO

-- =============================================
-- SP: Delete Tag
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteTag]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteTag]
GO

CREATE PROCEDURE [dbo].[sp_DeleteTag]
    @TagId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if tag exists
    IF NOT EXISTS (SELECT 1 FROM Tags WHERE TagId = @TagId)
    BEGIN
        RAISERROR('Tag not found', 16, 1);
        RETURN;
    END
    
    -- Check if tag is in use
    IF EXISTS (SELECT 1 FROM ChangeTags WHERE TagId = @TagId)
    BEGIN
        RAISERROR('Cannot delete tag that is in use by changes', 16, 1);
        RETURN;
    END
    
    DELETE FROM Tags
    WHERE TagId = @TagId;
    
    SELECT 1 AS Success;
END
GO

PRINT 'Tag stored procedures created successfully';
GO

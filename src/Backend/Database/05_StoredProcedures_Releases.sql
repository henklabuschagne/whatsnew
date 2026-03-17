-- =============================================
-- Release Management Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get All Releases
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllReleases]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetAllReleases]
GO

CREATE PROCEDURE [dbo].[sp_GetAllReleases]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ReleaseId,
        Version,
        ReleaseDate,
        CreatedAt,
        UpdatedAt
    FROM Releases
    ORDER BY ReleaseDate DESC, Version DESC;
END
GO

-- =============================================
-- SP: Get Release by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetReleaseById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetReleaseById]
GO

CREATE PROCEDURE [dbo].[sp_GetReleaseById]
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ReleaseId,
        Version,
        ReleaseDate,
        CreatedAt,
        UpdatedAt
    FROM Releases
    WHERE ReleaseId = @ReleaseId;
END
GO

-- =============================================
-- SP: Create Release
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateRelease]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CreateRelease]
GO

CREATE PROCEDURE [dbo].[sp_CreateRelease]
    @ReleaseId UNIQUEIDENTIFIER OUTPUT,
    @Version NVARCHAR(50),
    @ReleaseDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if version already exists
    IF EXISTS (SELECT 1 FROM Releases WHERE Version = @Version)
    BEGIN
        RAISERROR('Release with this version already exists', 16, 1);
        RETURN;
    END
    
    SET @ReleaseId = NEWID();
    
    INSERT INTO Releases (ReleaseId, Version, ReleaseDate)
    VALUES (@ReleaseId, @Version, @ReleaseDate);
    
    SELECT 
        ReleaseId,
        Version,
        ReleaseDate,
        CreatedAt,
        UpdatedAt
    FROM Releases
    WHERE ReleaseId = @ReleaseId;
END
GO

-- =============================================
-- SP: Update Release
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateRelease]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateRelease]
GO

CREATE PROCEDURE [dbo].[sp_UpdateRelease]
    @ReleaseId UNIQUEIDENTIFIER,
    @Version NVARCHAR(50),
    @ReleaseDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if release exists
    IF NOT EXISTS (SELECT 1 FROM Releases WHERE ReleaseId = @ReleaseId)
    BEGIN
        RAISERROR('Release not found', 16, 1);
        RETURN;
    END
    
    -- Check if version already exists for a different release
    IF EXISTS (SELECT 1 FROM Releases WHERE Version = @Version AND ReleaseId != @ReleaseId)
    BEGIN
        RAISERROR('Release with this version already exists', 16, 1);
        RETURN;
    END
    
    UPDATE Releases
    SET 
        Version = @Version,
        ReleaseDate = @ReleaseDate,
        UpdatedAt = GETUTCDATE()
    WHERE ReleaseId = @ReleaseId;
    
    SELECT 
        ReleaseId,
        Version,
        ReleaseDate,
        CreatedAt,
        UpdatedAt
    FROM Releases
    WHERE ReleaseId = @ReleaseId;
END
GO

-- =============================================
-- SP: Delete Release
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteRelease]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteRelease]
GO

CREATE PROCEDURE [dbo].[sp_DeleteRelease]
    @ReleaseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if release exists
    IF NOT EXISTS (SELECT 1 FROM Releases WHERE ReleaseId = @ReleaseId)
    BEGIN
        RAISERROR('Release not found', 16, 1);
        RETURN;
    END
    
    -- Delete release (cascade will delete associated changes)
    DELETE FROM Releases
    WHERE ReleaseId = @ReleaseId;
    
    SELECT 1 AS Success;
END
GO

PRINT 'Release stored procedures created successfully';
GO

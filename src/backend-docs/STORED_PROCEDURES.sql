-- ============================================
-- What's New Application - Stored Procedures
-- SQL Server 2019+
-- ============================================

USE WhatsNewDB;
GO

-- ============================================
-- USER PROCEDURES
-- ============================================

-- Get user by username
CREATE OR ALTER PROCEDURE sp_GetUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Username,
        Email,
        PasswordHash,
        FirstName,
        LastName,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt,
        LastLoginAt
    FROM Users
    WHERE Username = @Username AND IsActive = 1;
END
GO

-- Get user by email
CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Username,
        Email,
        PasswordHash,
        FirstName,
        LastName,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt,
        LastLoginAt
    FROM Users
    WHERE Email = @Email AND IsActive = 1;
END
GO

-- Update last login
CREATE OR ALTER PROCEDURE sp_UpdateLastLogin
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Users
    SET LastLoginAt = GETUTCDATE()
    WHERE UserId = @UserId;
END
GO

-- Get all users (admin only)
CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Username,
        Email,
        FirstName,
        LastName,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt,
        LastLoginAt
    FROM Users
    ORDER BY CreatedAt DESC;
END
GO

-- ============================================
-- RELEASE PROCEDURES
-- ============================================

-- Get all releases with change counts
CREATE OR ALTER PROCEDURE sp_GetAllReleases
    @IncludeUnpublished BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.ReleaseId,
        r.Version,
        r.ReleaseDate,
        r.Description,
        r.IsPublished,
        r.CreatedBy,
        r.CreatedAt,
        r.UpdatedAt,
        u.Username as CreatedByUsername,
        COUNT(c.ChangeId) as ChangeCount
    FROM Releases r
    LEFT JOIN Users u ON r.CreatedBy = u.UserId
    LEFT JOIN Changes c ON r.ReleaseId = c.ReleaseId
    WHERE (@IncludeUnpublished = 1 OR r.IsPublished = 1)
    GROUP BY 
        r.ReleaseId,
        r.Version,
        r.ReleaseDate,
        r.Description,
        r.IsPublished,
        r.CreatedBy,
        r.CreatedAt,
        r.UpdatedAt,
        u.Username
    ORDER BY r.ReleaseDate DESC, r.Version DESC;
END
GO

-- Get release by ID with changes and tags
CREATE OR ALTER PROCEDURE sp_GetReleaseById
    @ReleaseId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get release details
    SELECT 
        r.ReleaseId,
        r.Version,
        r.ReleaseDate,
        r.Description,
        r.IsPublished,
        r.CreatedBy,
        r.CreatedAt,
        r.UpdatedAt,
        u.Username as CreatedByUsername
    FROM Releases r
    LEFT JOIN Users u ON r.CreatedBy = u.UserId
    WHERE r.ReleaseId = @ReleaseId;
    
    -- Get changes for this release
    SELECT 
        c.ChangeId,
        c.ReleaseId,
        c.Description,
        c.ChangeType,
        c.CreatedBy,
        c.CreatedAt,
        c.UpdatedAt
    FROM Changes c
    WHERE c.ReleaseId = @ReleaseId;
    
    -- Get tags for each change
    SELECT 
        ct.ChangeId,
        t.TagId,
        t.TagValue,
        t.TagLabel
    FROM Change_Tags ct
    INNER JOIN Tags t ON ct.TagId = t.TagId
    WHERE ct.ChangeId IN (SELECT ChangeId FROM Changes WHERE ReleaseId = @ReleaseId);
END
GO

-- Create new release
CREATE OR ALTER PROCEDURE sp_CreateRelease
    @Version NVARCHAR(50),
    @ReleaseDate DATE,
    @Description NVARCHAR(MAX) = NULL,
    @IsPublished BIT = 0,
    @CreatedBy INT,
    @ReleaseId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO Releases (Version, ReleaseDate, Description, IsPublished, CreatedBy)
        VALUES (@Version, @ReleaseDate, @Description, @IsPublished, @CreatedBy);
        
        SET @ReleaseId = SCOPE_IDENTITY();
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, NewValue)
        VALUES (@CreatedBy, 'CREATE', 'Release', @ReleaseId, 
                CONCAT('Version: ', @Version, ', Date: ', CONVERT(NVARCHAR, @ReleaseDate, 120)));
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Update release
CREATE OR ALTER PROCEDURE sp_UpdateRelease
    @ReleaseId INT,
    @Version NVARCHAR(50),
    @ReleaseDate DATE,
    @Description NVARCHAR(MAX) = NULL,
    @IsPublished BIT,
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Store old values for audit
        DECLARE @OldValue NVARCHAR(MAX);
        SELECT @OldValue = CONCAT('Version: ', Version, ', Date: ', CONVERT(NVARCHAR, ReleaseDate, 120), 
                                  ', Published: ', CAST(IsPublished AS NVARCHAR))
        FROM Releases WHERE ReleaseId = @ReleaseId;
        
        -- Update release
        UPDATE Releases
        SET 
            Version = @Version,
            ReleaseDate = @ReleaseDate,
            Description = @Description,
            IsPublished = @IsPublished,
            UpdatedAt = GETUTCDATE()
        WHERE ReleaseId = @ReleaseId;
        
        -- Log audit
        DECLARE @NewValue NVARCHAR(MAX);
        SET @NewValue = CONCAT('Version: ', @Version, ', Date: ', CONVERT(NVARCHAR, @ReleaseDate, 120), 
                               ', Published: ', CAST(@IsPublished AS NVARCHAR));
        
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, OldValue, NewValue)
        VALUES (@UpdatedBy, 'UPDATE', 'Release', @ReleaseId, @OldValue, @NewValue);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Delete release
CREATE OR ALTER PROCEDURE sp_DeleteRelease
    @ReleaseId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Store old values for audit
        DECLARE @OldValue NVARCHAR(MAX);
        SELECT @OldValue = CONCAT('Version: ', Version, ', Date: ', CONVERT(NVARCHAR, ReleaseDate, 120))
        FROM Releases WHERE ReleaseId = @ReleaseId;
        
        -- Delete release (cascade will delete changes and change_tags)
        DELETE FROM Releases WHERE ReleaseId = @ReleaseId;
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, OldValue)
        VALUES (@DeletedBy, 'DELETE', 'Release', @ReleaseId, @OldValue);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================
-- CHANGE PROCEDURES
-- ============================================

-- Create new change
CREATE OR ALTER PROCEDURE sp_CreateChange
    @ReleaseId INT,
    @Description NVARCHAR(MAX),
    @ChangeType NVARCHAR(50),
    @ModuleTags NVARCHAR(MAX), -- Comma-separated tag values
    @CreatedBy INT,
    @ChangeId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Insert change
        INSERT INTO Changes (ReleaseId, Description, ChangeType, CreatedBy)
        VALUES (@ReleaseId, @Description, @ChangeType, @CreatedBy);
        
        SET @ChangeId = SCOPE_IDENTITY();
        
        -- Insert tags
        IF @ModuleTags IS NOT NULL AND LEN(@ModuleTags) > 0
        BEGIN
            INSERT INTO Change_Tags (ChangeId, TagId)
            SELECT @ChangeId, t.TagId
            FROM Tags t
            WHERE t.TagValue IN (SELECT value FROM STRING_SPLIT(@ModuleTags, ','));
        END
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, NewValue)
        VALUES (@CreatedBy, 'CREATE', 'Change', @ChangeId, 
                CONCAT('Type: ', @ChangeType, ', Release: ', @ReleaseId));
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Update change
CREATE OR ALTER PROCEDURE sp_UpdateChange
    @ChangeId INT,
    @Description NVARCHAR(MAX),
    @ChangeType NVARCHAR(50),
    @ModuleTags NVARCHAR(MAX), -- Comma-separated tag values
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Update change
        UPDATE Changes
        SET 
            Description = @Description,
            ChangeType = @ChangeType,
            UpdatedAt = GETUTCDATE()
        WHERE ChangeId = @ChangeId;
        
        -- Delete existing tags
        DELETE FROM Change_Tags WHERE ChangeId = @ChangeId;
        
        -- Insert new tags
        IF @ModuleTags IS NOT NULL AND LEN(@ModuleTags) > 0
        BEGIN
            INSERT INTO Change_Tags (ChangeId, TagId)
            SELECT @ChangeId, t.TagId
            FROM Tags t
            WHERE t.TagValue IN (SELECT value FROM STRING_SPLIT(@ModuleTags, ','));
        END
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, NewValue)
        VALUES (@UpdatedBy, 'UPDATE', 'Change', @ChangeId, 
                CONCAT('Type: ', @ChangeType));
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Delete change
CREATE OR ALTER PROCEDURE sp_DeleteChange
    @ChangeId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Delete change (cascade will delete change_tags)
        DELETE FROM Changes WHERE ChangeId = @ChangeId;
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId)
        VALUES (@DeletedBy, 'DELETE', 'Change', @ChangeId);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================
-- TAG PROCEDURES
-- ============================================

-- Get all tags
CREATE OR ALTER PROCEDURE sp_GetAllTags
    @ActiveOnly BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TagId,
        TagValue,
        TagLabel,
        TagType,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Tags
    WHERE (@ActiveOnly = 0 OR IsActive = 1)
    ORDER BY TagLabel;
END
GO

-- Create tag
CREATE OR ALTER PROCEDURE sp_CreateTag
    @TagValue NVARCHAR(100),
    @TagLabel NVARCHAR(100),
    @TagType NVARCHAR(50) = 'module',
    @CreatedBy INT,
    @TagId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO Tags (TagValue, TagLabel, TagType)
        VALUES (@TagValue, @TagLabel, @TagType);
        
        SET @TagId = SCOPE_IDENTITY();
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, NewValue)
        VALUES (@CreatedBy, 'CREATE', 'Tag', @TagId, 
                CONCAT('Value: ', @TagValue, ', Label: ', @TagLabel));
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Update tag
CREATE OR ALTER PROCEDURE sp_UpdateTag
    @TagId INT,
    @TagLabel NVARCHAR(100),
    @IsActive BIT,
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE Tags
        SET 
            TagLabel = @TagLabel,
            IsActive = @IsActive,
            UpdatedAt = GETUTCDATE()
        WHERE TagId = @TagId;
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, NewValue)
        VALUES (@UpdatedBy, 'UPDATE', 'Tag', @TagId, 
                CONCAT('Label: ', @TagLabel, ', Active: ', CAST(@IsActive AS NVARCHAR)));
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Delete tag
CREATE OR ALTER PROCEDURE sp_DeleteTag
    @TagId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Delete tag (cascade will delete change_tags)
        DELETE FROM Tags WHERE TagId = @TagId;
        
        -- Log audit
        INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId)
        VALUES (@DeletedBy, 'DELETE', 'Tag', @TagId);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================
-- REPORTING PROCEDURES
-- ============================================

-- Get release statistics
CREATE OR ALTER PROCEDURE sp_GetReleaseStatistics
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(DISTINCT r.ReleaseId) as TotalReleases,
        COUNT(DISTINCT CASE WHEN r.IsPublished = 1 THEN r.ReleaseId END) as PublishedReleases,
        COUNT(c.ChangeId) as TotalChanges,
        COUNT(CASE WHEN c.ChangeType = 'bug_fix' THEN 1 END) as BugFixes,
        COUNT(CASE WHEN c.ChangeType = 'new_feature' THEN 1 END) as NewFeatures,
        COUNT(CASE WHEN c.ChangeType = 'enhancement' THEN 1 END) as Enhancements
    FROM Releases r
    LEFT JOIN Changes c ON r.ReleaseId = c.ReleaseId;
    
    -- Get changes by module
    SELECT 
        t.TagLabel as ModuleName,
        COUNT(ct.ChangeId) as ChangeCount
    FROM Tags t
    INNER JOIN Change_Tags ct ON t.TagId = ct.TagId
    GROUP BY t.TagLabel
    ORDER BY ChangeCount DESC;
END
GO

-- Get audit log
CREATE OR ALTER PROCEDURE sp_GetAuditLog
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @UserId INT = NULL,
    @EntityType NVARCHAR(50) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.AuditId,
        a.UserId,
        u.Username,
        a.Action,
        a.EntityType,
        a.EntityId,
        a.OldValue,
        a.NewValue,
        a.IpAddress,
        a.CreatedAt
    FROM AuditLogs a
    LEFT JOIN Users u ON a.UserId = u.UserId
    WHERE 
        (@StartDate IS NULL OR a.CreatedAt >= @StartDate) AND
        (@EndDate IS NULL OR a.CreatedAt <= @EndDate) AND
        (@UserId IS NULL OR a.UserId = @UserId) AND
        (@EntityType IS NULL OR a.EntityType = @EntityType)
    ORDER BY a.CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) as TotalRecords
    FROM AuditLogs a
    WHERE 
        (@StartDate IS NULL OR a.CreatedAt >= @StartDate) AND
        (@EndDate IS NULL OR a.CreatedAt <= @EndDate) AND
        (@UserId IS NULL OR a.UserId = @UserId) AND
        (@EntityType IS NULL OR a.EntityType = @EntityType);
END
GO

PRINT 'Stored procedures created successfully!';

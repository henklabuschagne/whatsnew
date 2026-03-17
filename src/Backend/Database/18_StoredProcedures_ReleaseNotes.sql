-- =============================================
-- Stored Procedures for Release Notes Management
-- Purpose: CRUD operations for file attachments on changes
-- =============================================

USE WhatsNewDB;
GO

PRINT '========================================';
PRINT 'Creating Release Notes Stored Procedures';
PRINT '========================================';
PRINT '';

-- =============================================
-- SP: Get Release Notes by Change ID
-- =============================================
PRINT 'Creating sp_GetReleaseNotesByChangeId...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetReleaseNotesByChangeId')
    DROP PROCEDURE sp_GetReleaseNotesByChangeId;
GO

CREATE PROCEDURE sp_GetReleaseNotesByChangeId
    @ChangeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        rn.ReleaseNoteId,
        rn.ChangeId,
        rn.FileName,
        rn.FileSize,
        rn.FileType,
        rn.FileExtension,
        -- Don't return FileData in list (too large)
        rn.UploadedBy,
        u.Name AS UploadedByName,
        rn.UploadedAt,
        rn.CreatedAt,
        rn.UpdatedAt
    FROM ReleaseNotes rn
    LEFT JOIN Users u ON rn.UploadedBy = u.UserId
    WHERE rn.ChangeId = @ChangeId
    ORDER BY rn.UploadedAt DESC;
END
GO

PRINT '✓ sp_GetReleaseNotesByChangeId created';
PRINT '';

-- =============================================
-- SP: Get Release Note by ID (with file data)
-- =============================================
PRINT 'Creating sp_GetReleaseNoteById...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetReleaseNoteById')
    DROP PROCEDURE sp_GetReleaseNoteById;
GO

CREATE PROCEDURE sp_GetReleaseNoteById
    @ReleaseNoteId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        rn.ReleaseNoteId,
        rn.ChangeId,
        rn.FileName,
        rn.FileSize,
        rn.FileType,
        rn.FileExtension,
        rn.FileData,              -- Include file data for download
        rn.UploadedBy,
        u.Name AS UploadedByName,
        rn.UploadedAt,
        rn.CreatedAt,
        rn.UpdatedAt
    FROM ReleaseNotes rn
    LEFT JOIN Users u ON rn.UploadedBy = u.UserId
    WHERE rn.ReleaseNoteId = @ReleaseNoteId;
END
GO

PRINT '✓ sp_GetReleaseNoteById created';
PRINT '';

-- =============================================
-- SP: Create Release Note
-- =============================================
PRINT 'Creating sp_CreateReleaseNote...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateReleaseNote')
    DROP PROCEDURE sp_CreateReleaseNote;
GO

CREATE PROCEDURE sp_CreateReleaseNote
    @ChangeId UNIQUEIDENTIFIER,
    @FileName NVARCHAR(255),
    @FileSize BIGINT,
    @FileType NVARCHAR(100),
    @FileExtension NVARCHAR(50),
    @FileData VARBINARY(MAX),
    @UploadedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ReleaseNoteId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME2 = GETUTCDATE();

    -- Check if change exists
    IF NOT EXISTS (SELECT 1 FROM Changes WHERE ChangeId = @ChangeId)
    BEGIN
        RAISERROR('Change not found', 16, 1);
        RETURN;
    END

    -- Check if user exists (if provided)
    IF @UploadedBy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UploadedBy)
    BEGIN
        RAISERROR('User not found', 16, 1);
        RETURN;
    END

    -- Validate file size (max 50MB)
    IF @FileSize > 52428800  -- 50MB in bytes
    BEGIN
        RAISERROR('File size exceeds maximum allowed size of 50MB', 16, 1);
        RETURN;
    END

    -- Insert release note
    INSERT INTO ReleaseNotes (
        ReleaseNoteId,
        ChangeId,
        FileName,
        FileSize,
        FileType,
        FileExtension,
        FileData,
        UploadedBy,
        UploadedAt,
        CreatedAt,
        UpdatedAt
    )
    VALUES (
        @ReleaseNoteId,
        @ChangeId,
        @FileName,
        @FileSize,
        @FileType,
        @FileExtension,
        @FileData,
        @UploadedBy,
        @Now,
        @Now,
        @Now
    );

    -- Return the created release note (without file data for performance)
    SELECT 
        rn.ReleaseNoteId,
        rn.ChangeId,
        rn.FileName,
        rn.FileSize,
        rn.FileType,
        rn.FileExtension,
        rn.UploadedBy,
        u.Name AS UploadedByName,
        rn.UploadedAt,
        rn.CreatedAt,
        rn.UpdatedAt
    FROM ReleaseNotes rn
    LEFT JOIN Users u ON rn.UploadedBy = u.UserId
    WHERE rn.ReleaseNoteId = @ReleaseNoteId;
END
GO

PRINT '✓ sp_CreateReleaseNote created';
PRINT '';

-- =============================================
-- SP: Delete Release Note
-- =============================================
PRINT 'Creating sp_DeleteReleaseNote...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_DeleteReleaseNote')
    DROP PROCEDURE sp_DeleteReleaseNote;
GO

CREATE PROCEDURE sp_DeleteReleaseNote
    @ReleaseNoteId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if release note exists
    IF NOT EXISTS (SELECT 1 FROM ReleaseNotes WHERE ReleaseNoteId = @ReleaseNoteId)
    BEGIN
        RAISERROR('Release note not found', 16, 1);
        RETURN;
    END

    -- Delete the release note
    DELETE FROM ReleaseNotes 
    WHERE ReleaseNoteId = @ReleaseNoteId;

    -- Return success
    SELECT 1 AS Success;
END
GO

PRINT '✓ sp_DeleteReleaseNote created';
PRINT '';

-- =============================================
-- SP: Get All Release Notes (Admin)
-- =============================================
PRINT 'Creating sp_GetAllReleaseNotes...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllReleaseNotes')
    DROP PROCEDURE sp_GetAllReleaseNotes;
GO

CREATE PROCEDURE sp_GetAllReleaseNotes
    @TopN INT = 100  -- Limit for performance
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@TopN)
        rn.ReleaseNoteId,
        rn.ChangeId,
        c.Description AS ChangeDescription,
        r.Version AS ReleaseVersion,
        rn.FileName,
        rn.FileSize,
        rn.FileType,
        rn.FileExtension,
        rn.UploadedBy,
        u.Name AS UploadedByName,
        rn.UploadedAt,
        rn.CreatedAt,
        rn.UpdatedAt
    FROM ReleaseNotes rn
    INNER JOIN Changes c ON rn.ChangeId = c.ChangeId
    INNER JOIN Releases r ON c.ReleaseId = r.ReleaseId
    LEFT JOIN Users u ON rn.UploadedBy = u.UserId
    ORDER BY rn.UploadedAt DESC;
END
GO

PRINT '✓ sp_GetAllReleaseNotes created';
PRINT '';

-- =============================================
-- SP: Get Release Notes Count by Change
-- =============================================
PRINT 'Creating sp_GetReleaseNotesCount...';

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetReleaseNotesCount')
    DROP PROCEDURE sp_GetReleaseNotesCount;
GO

CREATE PROCEDURE sp_GetReleaseNotesCount
    @ChangeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS NoteCount
    FROM ReleaseNotes
    WHERE ChangeId = @ChangeId;
END
GO

PRINT '✓ sp_GetReleaseNotesCount created';
PRINT '';

-- =============================================
-- Verification
-- =============================================
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';

DECLARE @SPCount INT = 0;

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetReleaseNotesByChangeId')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetReleaseNoteById')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateReleaseNote')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_DeleteReleaseNote')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllReleaseNotes')
    SET @SPCount = @SPCount + 1;
    
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetReleaseNotesCount')
    SET @SPCount = @SPCount + 1;

PRINT '✓ ' + CAST(@SPCount AS VARCHAR) + '/6 stored procedures created';

PRINT '';
PRINT '========================================';
PRINT 'STORED PROCEDURES COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'Created Procedures:';
PRINT '1. sp_GetReleaseNotesByChangeId - Get notes for a change';
PRINT '2. sp_GetReleaseNoteById - Get single note with file data';
PRINT '3. sp_CreateReleaseNote - Upload a new note';
PRINT '4. sp_DeleteReleaseNote - Delete a note';
PRINT '5. sp_GetAllReleaseNotes - Admin view of all notes';
PRINT '6. sp_GetReleaseNotesCount - Count notes for a change';
PRINT '';
PRINT 'Features:';
PRINT '- File size limit: 50MB';
PRINT '- File data returned only when downloading';
PRINT '- User tracking with name lookup';
PRINT '- Validation for change and user existence';
PRINT '';
GO

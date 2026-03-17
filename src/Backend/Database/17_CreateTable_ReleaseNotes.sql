-- =============================================
-- Create ReleaseNotes Table
-- Purpose: Store file attachments (release notes) for changes
-- =============================================

USE WhatsNewDB;
GO

PRINT '========================================';
PRINT 'Creating ReleaseNotes Table';
PRINT '========================================';
PRINT '';

-- =============================================
-- Drop table if exists (for clean reinstall)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReleaseNotes]') AND type in (N'U'))
BEGIN
    PRINT 'Dropping existing ReleaseNotes table...';
    DROP TABLE [dbo].[ReleaseNotes];
    PRINT '✓ Table dropped';
END
GO

-- =============================================
-- Create ReleaseNotes Table
-- =============================================
PRINT 'Creating ReleaseNotes table...';

CREATE TABLE [dbo].[ReleaseNotes] (
    -- Primary Key
    ReleaseNoteId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Foreign Key to Changes
    ChangeId UNIQUEIDENTIFIER NOT NULL,
    
    -- File Metadata
    FileName NVARCHAR(255) NOT NULL,
    FileSize BIGINT NOT NULL,                    -- Size in bytes
    FileType NVARCHAR(100) NOT NULL,             -- MIME type (e.g., application/pdf)
    FileExtension NVARCHAR(50) NOT NULL,         -- File extension (e.g., .pdf, .docx)
    
    -- File Storage
    FileData VARBINARY(MAX) NOT NULL,            -- Store file as binary data
    
    -- Metadata
    UploadedBy UNIQUEIDENTIFIER NULL,            -- Optional: User who uploaded
    UploadedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Audit
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Foreign Key Constraint
    CONSTRAINT FK_ReleaseNotes_Changes 
        FOREIGN KEY (ChangeId) 
        REFERENCES Changes(ChangeId) 
        ON DELETE CASCADE,  -- Delete notes when change is deleted
        
    CONSTRAINT FK_ReleaseNotes_Users 
        FOREIGN KEY (UploadedBy) 
        REFERENCES Users(UserId) 
        ON DELETE SET NULL  -- Keep note even if user is deleted
);
GO

PRINT '✓ ReleaseNotes table created successfully';
PRINT '';

-- =============================================
-- Create Indexes for Performance
-- =============================================
PRINT 'Creating indexes...';

-- Index on ChangeId for fast lookup of notes by change
CREATE NONCLUSTERED INDEX IX_ReleaseNotes_ChangeId 
    ON ReleaseNotes(ChangeId);
    
PRINT '✓ Index IX_ReleaseNotes_ChangeId created';

-- Index on UploadedBy for user activity tracking
CREATE NONCLUSTERED INDEX IX_ReleaseNotes_UploadedBy 
    ON ReleaseNotes(UploadedBy);
    
PRINT '✓ Index IX_ReleaseNotes_UploadedBy created';

-- Index on UploadedAt for chronological queries
CREATE NONCLUSTERED INDEX IX_ReleaseNotes_UploadedAt 
    ON ReleaseNotes(UploadedAt DESC);
    
PRINT '✓ Index IX_ReleaseNotes_UploadedAt created';

PRINT '';

-- =============================================
-- Verification
-- =============================================
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';

-- Check table exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReleaseNotes]') AND type in (N'U'))
BEGIN
    PRINT '✓ ReleaseNotes table exists';
    
    -- Count columns
    DECLARE @ColCount INT;
    SELECT @ColCount = COUNT(*) 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[ReleaseNotes]');
    
    PRINT '✓ Column count: ' + CAST(@ColCount AS VARCHAR);
    
    -- Count indexes
    DECLARE @IdxCount INT;
    SELECT @IdxCount = COUNT(*) 
    FROM sys.indexes 
    WHERE object_id = OBJECT_ID(N'[dbo].[ReleaseNotes]') 
    AND name IS NOT NULL;
    
    PRINT '✓ Index count: ' + CAST(@IdxCount AS VARCHAR);
    
    -- Check foreign keys
    DECLARE @FKCount INT;
    SELECT @FKCount = COUNT(*) 
    FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID(N'[dbo].[ReleaseNotes]');
    
    PRINT '✓ Foreign key count: ' + CAST(@FKCount AS VARCHAR);
END
ELSE
BEGIN
    PRINT '✗ ERROR: ReleaseNotes table not found!';
END

PRINT '';
PRINT '========================================';
PRINT 'TABLE CREATION COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'Notes:';
PRINT '- ReleaseNotes stores file attachments for changes';
PRINT '- Files stored as VARBINARY(MAX) in database';
PRINT '- CASCADE DELETE: Notes deleted when change is deleted';
PRINT '- SET NULL: Notes kept if uploader user is deleted';
PRINT '- Indexes created for performance';
PRINT '';
GO

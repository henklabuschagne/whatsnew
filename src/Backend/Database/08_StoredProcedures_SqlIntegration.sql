-- =============================================
-- SQL Integration Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get All SQL Connections
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllSqlConnections]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetAllSqlConnections]
GO

CREATE PROCEDURE [dbo].[sp_GetAllSqlConnections]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ConnectionId,
        Name,
        Server,
        [Database],
        Username,
        UseIntegratedSecurity,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SqlConnections
    ORDER BY Name;
END
GO

-- =============================================
-- SP: Get SQL Connection by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetSqlConnectionById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetSqlConnectionById]
GO

CREATE PROCEDURE [dbo].[sp_GetSqlConnectionById]
    @ConnectionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ConnectionId,
        Name,
        Server,
        [Database],
        Username,
        Password,
        UseIntegratedSecurity,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SqlConnections
    WHERE ConnectionId = @ConnectionId;
END
GO

-- =============================================
-- SP: Create SQL Connection
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateSqlConnection]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CreateSqlConnection]
GO

CREATE PROCEDURE [dbo].[sp_CreateSqlConnection]
    @ConnectionId UNIQUEIDENTIFIER OUTPUT,
    @Name NVARCHAR(100),
    @Server NVARCHAR(255),
    @Database NVARCHAR(100),
    @Username NVARCHAR(100) = NULL,
    @Password NVARCHAR(255) = NULL,
    @UseIntegratedSecurity BIT = 0,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @ConnectionId = NEWID();
    
    INSERT INTO SqlConnections (
        ConnectionId,
        Name,
        Server,
        [Database],
        Username,
        Password,
        UseIntegratedSecurity,
        IsActive
    )
    VALUES (
        @ConnectionId,
        @Name,
        @Server,
        @Database,
        @Username,
        @Password,
        @UseIntegratedSecurity,
        @IsActive
    );
    
    SELECT 
        ConnectionId,
        Name,
        Server,
        [Database],
        Username,
        UseIntegratedSecurity,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SqlConnections
    WHERE ConnectionId = @ConnectionId;
END
GO

-- =============================================
-- SP: Update SQL Connection
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateSqlConnection]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateSqlConnection]
GO

CREATE PROCEDURE [dbo].[sp_UpdateSqlConnection]
    @ConnectionId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Server NVARCHAR(255),
    @Database NVARCHAR(100),
    @Username NVARCHAR(100) = NULL,
    @Password NVARCHAR(255) = NULL,
    @UseIntegratedSecurity BIT = 0,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM SqlConnections WHERE ConnectionId = @ConnectionId)
    BEGIN
        RAISERROR('SQL Connection not found', 16, 1);
        RETURN;
    END
    
    UPDATE SqlConnections
    SET 
        Name = @Name,
        Server = @Server,
        [Database] = @Database,
        Username = @Username,
        Password = ISNULL(@Password, Password),
        UseIntegratedSecurity = @UseIntegratedSecurity,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE ConnectionId = @ConnectionId;
    
    SELECT 
        ConnectionId,
        Name,
        Server,
        [Database],
        Username,
        UseIntegratedSecurity,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SqlConnections
    WHERE ConnectionId = @ConnectionId;
END
GO

-- =============================================
-- SP: Delete SQL Connection
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteSqlConnection]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteSqlConnection]
GO

CREATE PROCEDURE [dbo].[sp_DeleteSqlConnection]
    @ConnectionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM SqlConnections WHERE ConnectionId = @ConnectionId)
    BEGIN
        RAISERROR('SQL Connection not found', 16, 1);
        RETURN;
    END
    
    DELETE FROM SqlConnections
    WHERE ConnectionId = @ConnectionId;
    
    SELECT 1 AS Success;
END
GO

-- =============================================
-- SP: Get All SQL Queries
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllSqlQueries]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetAllSqlQueries]
GO

CREATE PROCEDURE [dbo].[sp_GetAllSqlQueries]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        q.QueryId,
        q.ConnectionId,
        q.Name,
        q.Description,
        q.QueryText,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt,
        c.Name AS ConnectionName
    FROM SqlQueries q
    INNER JOIN SqlConnections c ON q.ConnectionId = c.ConnectionId
    ORDER BY q.Name;
END
GO

-- =============================================
-- SP: Get SQL Query by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetSqlQueryById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetSqlQueryById]
GO

CREATE PROCEDURE [dbo].[sp_GetSqlQueryById]
    @QueryId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        q.QueryId,
        q.ConnectionId,
        q.Name,
        q.Description,
        q.QueryText,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt,
        c.Name AS ConnectionName
    FROM SqlQueries q
    INNER JOIN SqlConnections c ON q.ConnectionId = c.ConnectionId
    WHERE q.QueryId = @QueryId;
END
GO

-- =============================================
-- SP: Create SQL Query
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateSqlQuery]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CreateSqlQuery]
GO

CREATE PROCEDURE [dbo].[sp_CreateSqlQuery]
    @QueryId UNIQUEIDENTIFIER OUTPUT,
    @ConnectionId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX) = NULL,
    @QueryText NVARCHAR(MAX),
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM SqlConnections WHERE ConnectionId = @ConnectionId)
    BEGIN
        RAISERROR('SQL Connection not found', 16, 1);
        RETURN;
    END
    
    SET @QueryId = NEWID();
    
    INSERT INTO SqlQueries (
        QueryId,
        ConnectionId,
        Name,
        Description,
        QueryText,
        IsActive
    )
    VALUES (
        @QueryId,
        @ConnectionId,
        @Name,
        @Description,
        @QueryText,
        @IsActive
    );
    
    SELECT 
        q.QueryId,
        q.ConnectionId,
        q.Name,
        q.Description,
        q.QueryText,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt,
        c.Name AS ConnectionName
    FROM SqlQueries q
    INNER JOIN SqlConnections c ON q.ConnectionId = c.ConnectionId
    WHERE q.QueryId = @QueryId;
END
GO

-- =============================================
-- SP: Update SQL Query
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateSqlQuery]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UpdateSqlQuery]
GO

CREATE PROCEDURE [dbo].[sp_UpdateSqlQuery]
    @QueryId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX) = NULL,
    @QueryText NVARCHAR(MAX),
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM SqlQueries WHERE QueryId = @QueryId)
    BEGIN
        RAISERROR('SQL Query not found', 16, 1);
        RETURN;
    END
    
    UPDATE SqlQueries
    SET 
        Name = @Name,
        Description = @Description,
        QueryText = @QueryText,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE QueryId = @QueryId;
    
    SELECT 
        q.QueryId,
        q.ConnectionId,
        q.Name,
        q.Description,
        q.QueryText,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt,
        c.Name AS ConnectionName
    FROM SqlQueries q
    INNER JOIN SqlConnections c ON q.ConnectionId = c.ConnectionId
    WHERE q.QueryId = @QueryId;
END
GO

-- =============================================
-- SP: Delete SQL Query
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteSqlQuery]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_DeleteSqlQuery]
GO

CREATE PROCEDURE [dbo].[sp_DeleteSqlQuery]
    @QueryId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM SqlQueries WHERE QueryId = @QueryId)
    BEGIN
        RAISERROR('SQL Query not found', 16, 1);
        RETURN;
    END
    
    DELETE FROM SqlQueries
    WHERE QueryId = @QueryId;
    
    SELECT 1 AS Success;
END
GO

PRINT 'SQL Integration stored procedures created successfully';
GO

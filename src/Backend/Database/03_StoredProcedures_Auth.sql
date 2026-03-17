-- =============================================
-- Authentication Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get User by Email
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetUserByEmail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetUserByEmail]
GO

CREATE PROCEDURE [dbo].[sp_GetUserByEmail]
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Name,
        Email,
        PasswordHash,
        Role,
        CreatedAt
    FROM Users
    WHERE Email = @Email;
END
GO

-- =============================================
-- SP: Get User by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetUserById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetUserById]
GO

CREATE PROCEDURE [dbo].[sp_GetUserById]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Name,
        Email,
        Role,
        CreatedAt
    FROM Users
    WHERE UserId = @UserId;
END
GO

-- =============================================
-- SP: Get All Users
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllUsers]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetAllUsers]
GO

CREATE PROCEDURE [dbo].[sp_GetAllUsers]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        UserId,
        Name,
        Email,
        Role,
        CreatedAt
    FROM Users
    ORDER BY Name;
END
GO

-- =============================================
-- SP: Create User
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CreateUser]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CreateUser]
GO

CREATE PROCEDURE [dbo].[sp_CreateUser]
    @UserId UNIQUEIDENTIFIER OUTPUT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(255),
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @UserId = NEWID();
    
    INSERT INTO Users (UserId, Name, Email, PasswordHash, Role)
    VALUES (@UserId, @Name, @Email, @PasswordHash, @Role);
    
    SELECT 
        UserId,
        Name,
        Email,
        Role,
        CreatedAt
    FROM Users
    WHERE UserId = @UserId;
END
GO

PRINT 'Authentication stored procedures created successfully';
GO

-- =============================================
-- Stored Procedures for Analytics with Client Tracking
-- =============================================

-- =============================================
-- SP: Get Client Distribution Analytics
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetClientDistribution')
    DROP PROCEDURE sp_GetClientDistribution;
GO

CREATE PROCEDURE sp_GetClientDistribution
AS
BEGIN
    SET NOCOUNT ON;

    -- Get total changes count for percentage calculation
    DECLARE @TotalChanges INT;
    SELECT @TotalChanges = COUNT(*) FROM Changes;

    SELECT 
        ISNULL(c.ClientId, '00000000-0000-0000-0000-000000000000') AS ClientId,
        ISNULL(c.Name, 'Internal') AS ClientName,
        ISNULL(c.Code, 'INT') AS ClientCode,
        COUNT(ch.ChangeId) AS ChangeCount,
        SUM(CASE WHEN ch.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixes,
        SUM(CASE WHEN ch.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS Enhancements,
        SUM(CASE WHEN ch.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatures,
        COUNT(ch.ChangeId) AS Count,
        CASE 
            WHEN @TotalChanges > 0 THEN CAST(ROUND((COUNT(ch.ChangeId) * 100.0 / @TotalChanges), 0) AS INT)
            ELSE 0 
        END AS Percentage
    FROM Changes ch
    LEFT JOIN Clients c ON ch.ClientId = c.ClientId
    GROUP BY c.ClientId, c.Name, c.Code
    ORDER BY ChangeCount DESC;
END
GO

-- =============================================
-- SP: Get Time To Action Metrics
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetTimeToActionMetrics')
    DROP PROCEDURE sp_GetTimeToActionMetrics;
GO

CREATE PROCEDURE sp_GetTimeToActionMetrics
AS
BEGIN
    SET NOCOUNT ON;

    -- Get metrics by change type
    SELECT 
        ch.ChangeType,
        CASE 
            WHEN ch.ChangeType = 'bug-fix' THEN 'Bug Fix'
            WHEN ch.ChangeType = 'enhancement' THEN 'Enhancement'
            WHEN ch.ChangeType = 'new-feature' THEN 'New Feature'
            ELSE ch.ChangeType
        END AS Label,
        AVG(CAST(tta.TotalDays AS FLOAT)) AS AverageTotalTime,
        AVG(CAST(tta.DevDays AS FLOAT)) AS AverageDevTime,
        AVG(CAST(tta.TestDays AS FLOAT)) AS AverageTestTime,
        AVG(CAST(tta.ReleaseDays AS FLOAT)) AS AverageReleaseTime,
        AVG(CAST(tta.DevDays AS FLOAT)) AS SubmittedToDeveloped,
        AVG(CAST(tta.TestDays AS FLOAT)) AS DevelopedToTested,
        AVG(CAST(tta.ReleaseDays AS FLOAT)) AS TestedToReleased,
        COUNT(ch.ChangeId) AS Count
    FROM Changes ch
    INNER JOIN TimeToAction tta ON ch.ChangeId = tta.ChangeId
    WHERE tta.SubmittedDate IS NOT NULL AND tta.ReleasedDate IS NOT NULL
    GROUP BY ch.ChangeType
    ORDER BY ch.ChangeType;

    -- Get overall statistics
    SELECT 
        AVG(CAST(TotalDays AS FLOAT)) AS AverageTotalTime,
        MIN(TotalDays) AS FastestCompletion,
        MAX(TotalDays) AS SlowestCompletion,
        (
            SELECT TOP 1 TotalDays 
            FROM TimeToAction 
            WHERE SubmittedDate IS NOT NULL AND ReleasedDate IS NOT NULL
            ORDER BY TotalDays 
            OFFSET (SELECT COUNT(*)/2 FROM TimeToAction WHERE SubmittedDate IS NOT NULL AND ReleasedDate IS NOT NULL) ROWS
            FETCH NEXT 1 ROWS ONLY
        ) AS MedianTime
    FROM TimeToAction
    WHERE SubmittedDate IS NOT NULL AND ReleasedDate IS NOT NULL;

    -- Get timeline data (last 6 months)
    WITH MonthlyData AS (
        SELECT 
            FORMAT(tta.SubmittedDate, 'yyyy-MM') AS YearMonth,
            DATENAME(MONTH, tta.SubmittedDate) AS MonthName,
            LEFT(DATENAME(MONTH, tta.SubmittedDate), 3) AS Month,
            ch.ChangeType,
            AVG(CAST(tta.TotalDays AS FLOAT)) AS AvgDays
        FROM TimeToAction tta
        INNER JOIN Changes ch ON tta.ChangeId = ch.ChangeId
        WHERE tta.SubmittedDate >= DATEADD(MONTH, -6, GETUTCDATE())
            AND tta.SubmittedDate IS NOT NULL 
            AND tta.ReleasedDate IS NOT NULL
        GROUP BY FORMAT(tta.SubmittedDate, 'yyyy-MM'), 
                 DATENAME(MONTH, tta.SubmittedDate),
                 ch.ChangeType
    )
    SELECT 
        YearMonth,
        Month,
        MonthName,
        MAX(CASE WHEN ChangeType = 'bug-fix' THEN AvgDays ELSE NULL END) AS BugFix,
        MAX(CASE WHEN ChangeType = 'enhancement' THEN AvgDays ELSE NULL END) AS Enhancement,
        MAX(CASE WHEN ChangeType = 'new-feature' THEN AvgDays ELSE NULL END) AS NewFeature
    FROM MonthlyData
    GROUP BY YearMonth, Month, MonthName
    ORDER BY YearMonth;
END
GO

-- =============================================
-- SP: Update Time To Action for Change
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_UpdateTimeToAction')
    DROP PROCEDURE sp_UpdateTimeToAction;
GO

CREATE PROCEDURE sp_UpdateTimeToAction
    @ChangeId UNIQUEIDENTIFIER,
    @SubmittedDate DATETIME2 = NULL,
    @DevelopedDate DATETIME2 = NULL,
    @TestedDate DATETIME2 = NULL,
    @ReleasedDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if record exists
    IF EXISTS (SELECT 1 FROM TimeToAction WHERE ChangeId = @ChangeId)
    BEGIN
        -- Update existing record
        UPDATE TimeToAction
        SET 
            SubmittedDate = ISNULL(@SubmittedDate, SubmittedDate),
            DevelopedDate = ISNULL(@DevelopedDate, DevelopedDate),
            TestedDate = ISNULL(@TestedDate, TestedDate),
            ReleasedDate = ISNULL(@ReleasedDate, ReleasedDate),
            UpdatedAt = GETUTCDATE()
        WHERE ChangeId = @ChangeId;
    END
    ELSE
    BEGIN
        -- Create new record
        INSERT INTO TimeToAction (TimeToActionId, ChangeId, SubmittedDate, DevelopedDate, TestedDate, ReleasedDate, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @ChangeId, @SubmittedDate, @DevelopedDate, @TestedDate, @ReleasedDate, GETUTCDATE(), GETUTCDATE());
    END

    -- Return updated record
    SELECT 
        TimeToActionId,
        ChangeId,
        SubmittedDate,
        DevelopedDate,
        TestedDate,
        ReleasedDate,
        TotalDays,
        DevDays,
        TestDays,
        ReleaseDays,
        CreatedAt,
        UpdatedAt
    FROM TimeToAction
    WHERE ChangeId = @ChangeId;
END
GO

-- =============================================
-- SP: Get Time To Action By Change
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetTimeToActionByChange')
    DROP PROCEDURE sp_GetTimeToActionByChange;
GO

CREATE PROCEDURE sp_GetTimeToActionByChange
    @ChangeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        TimeToActionId,
        ChangeId,
        SubmittedDate,
        DevelopedDate,
        TestedDate,
        ReleasedDate,
        TotalDays,
        DevDays,
        TestDays,
        ReleaseDays,
        CreatedAt,
        UpdatedAt
    FROM TimeToAction
    WHERE ChangeId = @ChangeId;
END
GO

-- =============================================
-- Update existing sp_GetAllChanges to include client tracking
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllChanges')
    DROP PROCEDURE sp_GetAllChanges;
GO

CREATE PROCEDURE sp_GetAllChanges
    @ReleaseId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ChangeId,
        c.ReleaseId,
        c.Title,
        c.Description,
        c.ChangeType,
        c.ClientId,
        c.TicketNumber,
        c.DevOpsNumber,
        c.CreatedAt,
        c.UpdatedAt,
        -- Get client info
        cl.Name AS ClientName,
        cl.Code AS ClientCode,
        -- Get tags as JSON array
        (
            SELECT t.TagId, t.Label, t.Value, t.Type
            FROM Tags t
            INNER JOIN ChangeTags ct ON t.TagId = ct.TagId
            WHERE ct.ChangeId = c.ChangeId
            FOR JSON PATH
        ) AS Tags
    FROM Changes c
    LEFT JOIN Clients cl ON c.ClientId = cl.ClientId
    WHERE (@ReleaseId IS NULL OR c.ReleaseId = @ReleaseId)
    ORDER BY c.CreatedAt DESC;
END
GO

-- =============================================
-- Update existing sp_GetChangeById to include client tracking
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetChangeById')
    DROP PROCEDURE sp_GetChangeById;
GO

CREATE PROCEDURE sp_GetChangeById
    @ChangeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ChangeId,
        c.ReleaseId,
        c.Title,
        c.Description,
        c.ChangeType,
        c.ClientId,
        c.TicketNumber,
        c.DevOpsNumber,
        c.CreatedAt,
        c.UpdatedAt,
        -- Get client info
        cl.Name AS ClientName,
        cl.Code AS ClientCode,
        -- Get tags as JSON array
        (
            SELECT t.TagId, t.Label, t.Value, t.Type
            FROM Tags t
            INNER JOIN ChangeTags ct ON t.TagId = ct.TagId
            WHERE ct.ChangeId = c.ChangeId
            FOR JSON PATH
        ) AS Tags
    FROM Changes c
    LEFT JOIN Clients cl ON c.ClientId = cl.ClientId
    WHERE c.ChangeId = @ChangeId;
END
GO

-- =============================================
-- Update existing sp_CreateChange to include client tracking
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_CreateChange')
    DROP PROCEDURE sp_CreateChange;
GO

CREATE PROCEDURE sp_CreateChange
    @ReleaseId UNIQUEIDENTIFIER,
    @Title NVARCHAR(500),
    @Description NVARCHAR(MAX),
    @ChangeType NVARCHAR(50),
    @TagIds NVARCHAR(MAX) = NULL, -- JSON array of TagIds
    @ClientId UNIQUEIDENTIFIER = NULL,
    @TicketNumber NVARCHAR(100) = NULL,
    @DevOpsNumber NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ChangeId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME2 = GETUTCDATE();

    -- Validate ReleaseId
    IF NOT EXISTS (SELECT 1 FROM Releases WHERE ReleaseId = @ReleaseId)
    BEGIN
        RAISERROR('Release not found', 16, 1);
        RETURN;
    END

    -- Validate ClientId if provided
    IF @ClientId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = @ClientId)
    BEGIN
        RAISERROR('Client not found', 16, 1);
        RETURN;
    END

    -- Insert change
    INSERT INTO Changes (ChangeId, ReleaseId, Title, Description, ChangeType, ClientId, TicketNumber, DevOpsNumber, CreatedAt, UpdatedAt)
    VALUES (@ChangeId, @ReleaseId, @Title, @Description, @ChangeType, @ClientId, @TicketNumber, @DevOpsNumber, @Now, @Now);

    -- Insert tags if provided
    IF @TagIds IS NOT NULL AND @TagIds != '[]'
    BEGIN
        INSERT INTO ChangeTags (ChangeTagId, ChangeId, TagId, CreatedAt)
        SELECT NEWID(), @ChangeId, value, @Now
        FROM OPENJSON(@TagIds) WITH (value UNIQUEIDENTIFIER '$');
    END

    -- Create initial TimeToAction record
    INSERT INTO TimeToAction (TimeToActionId, ChangeId, SubmittedDate, CreatedAt, UpdatedAt)
    VALUES (NEWID(), @ChangeId, @Now, @Now, @Now);

    -- Return created change
    EXEC sp_GetChangeById @ChangeId;
END
GO

-- =============================================
-- Update existing sp_UpdateChange to include client tracking
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_UpdateChange')
    DROP PROCEDURE sp_UpdateChange;
GO

CREATE PROCEDURE sp_UpdateChange
    @ChangeId UNIQUEIDENTIFIER,
    @Title NVARCHAR(500),
    @Description NVARCHAR(MAX),
    @ChangeType NVARCHAR(50),
    @TagIds NVARCHAR(MAX) = NULL, -- JSON array of TagIds
    @ClientId UNIQUEIDENTIFIER = NULL,
    @TicketNumber NVARCHAR(100) = NULL,
    @DevOpsNumber NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate ChangeId
    IF NOT EXISTS (SELECT 1 FROM Changes WHERE ChangeId = @ChangeId)
    BEGIN
        RAISERROR('Change not found', 16, 1);
        RETURN;
    END

    -- Validate ClientId if provided
    IF @ClientId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Clients WHERE ClientId = @ClientId)
    BEGIN
        RAISERROR('Client not found', 16, 1);
        RETURN;
    END

    -- Update change
    UPDATE Changes
    SET 
        Title = @Title,
        Description = @Description,
        ChangeType = @ChangeType,
        ClientId = @ClientId,
        TicketNumber = @TicketNumber,
        DevOpsNumber = @DevOpsNumber,
        UpdatedAt = GETUTCDATE()
    WHERE ChangeId = @ChangeId;

    -- Update tags
    DELETE FROM ChangeTags WHERE ChangeId = @ChangeId;
    
    IF @TagIds IS NOT NULL AND @TagIds != '[]'
    BEGIN
        INSERT INTO ChangeTags (ChangeTagId, ChangeId, TagId, CreatedAt)
        SELECT NEWID(), @ChangeId, value, GETUTCDATE()
        FROM OPENJSON(@TagIds) WITH (value UNIQUEIDENTIFIER '$');
    END

    -- Return updated change
    EXEC sp_GetChangeById @ChangeId;
END
GO

PRINT 'Analytics stored procedures with client tracking created successfully';
GO

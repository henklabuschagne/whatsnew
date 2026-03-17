-- =============================================
-- Analytics Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get Release Timeline
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetReleaseTimeline]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetReleaseTimeline]
GO

CREATE PROCEDURE [dbo].[sp_GetReleaseTimeline]
    @Months INT = 12
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StartDate DATETIME = DATEADD(MONTH, -@Months, GETUTCDATE());
    
    SELECT 
        YEAR(r.ReleaseDate) AS Year,
        MONTH(r.ReleaseDate) AS Month,
        DATENAME(MONTH, r.ReleaseDate) AS MonthName,
        COUNT(DISTINCT r.ReleaseId) AS ReleaseCount,
        COUNT(c.ChangeId) AS TotalChanges,
        SUM(CASE WHEN c.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixes,
        SUM(CASE WHEN c.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatures,
        SUM(CASE WHEN c.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS Enhancements
    FROM Releases r
    LEFT JOIN Changes c ON r.ReleaseId = c.ReleaseId
    WHERE r.ReleaseDate >= @StartDate
    GROUP BY YEAR(r.ReleaseDate), MONTH(r.ReleaseDate), DATENAME(MONTH, r.ReleaseDate)
    ORDER BY YEAR(r.ReleaseDate), MONTH(r.ReleaseDate);
END
GO

-- =============================================
-- SP: Get Module Distribution
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetModuleDistribution]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetModuleDistribution]
GO

CREATE PROCEDURE [dbo].[sp_GetModuleDistribution]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.TagId,
        t.Label AS ModuleName,
        t.Value AS ModuleValue,
        COUNT(DISTINCT ct.ChangeId) AS ChangeCount,
        SUM(CASE WHEN c.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixes,
        SUM(CASE WHEN c.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatures,
        SUM(CASE WHEN c.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS Enhancements
    FROM Tags t
    INNER JOIN ChangeTags ct ON t.TagId = ct.TagId
    INNER JOIN Changes c ON ct.ChangeId = c.ChangeId
    WHERE t.Type = 'module'
    GROUP BY t.TagId, t.Label, t.Value
    ORDER BY COUNT(DISTINCT ct.ChangeId) DESC;
END
GO

-- =============================================
-- SP: Get Change Type Distribution
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetChangeTypeDistribution]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetChangeTypeDistribution]
GO

CREATE PROCEDURE [dbo].[sp_GetChangeTypeDistribution]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ChangeType,
        COUNT(*) AS Count,
        CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Changes) AS DECIMAL(5,2)) AS Percentage
    FROM Changes c
    GROUP BY c.ChangeType
    ORDER BY COUNT(*) DESC;
END
GO

-- =============================================
-- SP: Get Recent Activity
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetRecentActivity]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetRecentActivity]
GO

CREATE PROCEDURE [dbo].[sp_GetRecentActivity]
    @TopN INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopN)
        'Release' AS ActivityType,
        r.ReleaseId AS EntityId,
        r.Version AS EntityName,
        NULL AS Description,
        r.CreatedAt AS ActivityDate
    FROM Releases r
    
    UNION ALL
    
    SELECT TOP (@TopN)
        'Change' AS ActivityType,
        c.ChangeId AS EntityId,
        r.Version AS EntityName,
        c.Description AS Description,
        c.CreatedAt AS ActivityDate
    FROM Changes c
    INNER JOIN Releases r ON c.ReleaseId = r.ReleaseId
    
    ORDER BY ActivityDate DESC;
END
GO

-- =============================================
-- SP: Get Release Velocity
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetReleaseVelocity]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetReleaseVelocity]
GO

CREATE PROCEDURE [dbo].[sp_GetReleaseVelocity]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Last 30 days
    DECLARE @Last30Days INT = (
        SELECT COUNT(*) FROM Releases 
        WHERE ReleaseDate >= DATEADD(DAY, -30, GETUTCDATE())
    );
    
    -- Last 90 days
    DECLARE @Last90Days INT = (
        SELECT COUNT(*) FROM Releases 
        WHERE ReleaseDate >= DATEADD(DAY, -90, GETUTCDATE())
    );
    
    -- Last 365 days
    DECLARE @Last365Days INT = (
        SELECT COUNT(*) FROM Releases 
        WHERE ReleaseDate >= DATEADD(DAY, -365, GETUTCDATE())
    );
    
    -- Average days between releases
    DECLARE @AvgDaysBetween DECIMAL(10,2) = (
        SELECT AVG(DATEDIFF(DAY, PrevDate, ReleaseDate))
        FROM (
            SELECT 
                ReleaseDate,
                LAG(ReleaseDate) OVER (ORDER BY ReleaseDate) AS PrevDate
            FROM Releases
        ) AS DateDiffs
        WHERE PrevDate IS NOT NULL
    );
    
    SELECT 
        @Last30Days AS ReleasesLast30Days,
        @Last90Days AS ReleasesLast90Days,
        @Last365Days AS ReleasesLast365Days,
        @AvgDaysBetween AS AvgDaysBetweenReleases;
END
GO

-- =============================================
-- SP: Get Top Contributors (Changes per Release)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetTopReleases]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetTopReleases]
GO

CREATE PROCEDURE [dbo].[sp_GetTopReleases]
    @TopN INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopN)
        r.ReleaseId,
        r.Version,
        r.ReleaseDate,
        COUNT(c.ChangeId) AS ChangeCount,
        SUM(CASE WHEN c.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixes,
        SUM(CASE WHEN c.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatures,
        SUM(CASE WHEN c.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS Enhancements
    FROM Releases r
    LEFT JOIN Changes c ON r.ReleaseId = c.ReleaseId
    GROUP BY r.ReleaseId, r.Version, r.ReleaseDate
    ORDER BY COUNT(c.ChangeId) DESC;
END
GO

-- =============================================
-- SP: Get Dashboard Summary
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetDashboardSummary]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetDashboardSummary]
GO

CREATE PROCEDURE [dbo].[sp_GetDashboardSummary]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Overall statistics
    SELECT 
        (SELECT COUNT(*) FROM Releases) AS TotalReleases,
        (SELECT COUNT(*) FROM Changes) AS TotalChanges,
        (SELECT COUNT(*) FROM Tags WHERE Type = 'module') AS TotalModules,
        (SELECT COUNT(*) FROM Releases WHERE ReleaseDate >= DATEADD(DAY, -30, GETUTCDATE())) AS ReleasesThisMonth,
        (SELECT COUNT(*) FROM Changes WHERE CreatedAt >= DATEADD(DAY, -30, GETUTCDATE())) AS ChangesThisMonth,
        (SELECT MAX(ReleaseDate) FROM Releases) AS LatestReleaseDate,
        (SELECT TOP 1 Version FROM Releases ORDER BY ReleaseDate DESC) AS LatestVersion;
END
GO

-- =============================================
-- SP: Get Change Trends
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetChangeTrends]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetChangeTrends]
GO

CREATE PROCEDURE [dbo].[sp_GetChangeTrends]
    @Days INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StartDate DATETIME = DATEADD(DAY, -@Days, GETUTCDATE());
    
    SELECT 
        CAST(c.CreatedAt AS DATE) AS Date,
        COUNT(*) AS TotalChanges,
        SUM(CASE WHEN c.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixes,
        SUM(CASE WHEN c.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatures,
        SUM(CASE WHEN c.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS Enhancements
    FROM Changes c
    WHERE c.CreatedAt >= @StartDate
    GROUP BY CAST(c.CreatedAt AS DATE)
    ORDER BY CAST(c.CreatedAt AS DATE);
END
GO

PRINT 'Analytics stored procedures created successfully';
GO

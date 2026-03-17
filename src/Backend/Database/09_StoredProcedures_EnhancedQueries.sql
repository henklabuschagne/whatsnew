-- =============================================
-- Enhanced Query Stored Procedures
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- SP: Get Releases with Filters
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetReleasesWithFilters]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetReleasesWithFilters]
GO

CREATE PROCEDURE [dbo].[sp_GetReleasesWithFilters]
    @SearchTerm NVARCHAR(255) = NULL,
    @ChangeType NVARCHAR(50) = NULL,
    @ModuleTagId UNIQUEIDENTIFIER = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all releases first
    SELECT DISTINCT
        r.ReleaseId,
        r.Version,
        r.ReleaseDate,
        r.CreatedAt,
        r.UpdatedAt
    INTO #FilteredReleases
    FROM Releases r
    WHERE 
        (@FromDate IS NULL OR r.ReleaseDate >= @FromDate)
        AND (@ToDate IS NULL OR r.ReleaseDate <= @ToDate)
        AND (
            @SearchTerm IS NULL 
            OR r.Version LIKE '%' + @SearchTerm + '%'
            OR EXISTS (
                SELECT 1 FROM Changes c 
                WHERE c.ReleaseId = r.ReleaseId 
                AND c.Description LIKE '%' + @SearchTerm + '%'
            )
        )
        AND (
            @ChangeType IS NULL
            OR EXISTS (
                SELECT 1 FROM Changes c
                WHERE c.ReleaseId = r.ReleaseId
                AND c.ChangeType = @ChangeType
            )
        )
        AND (
            @ModuleTagId IS NULL
            OR EXISTS (
                SELECT 1 FROM Changes c
                INNER JOIN ChangeTags ct ON c.ChangeId = ct.ChangeId
                WHERE c.ReleaseId = r.ReleaseId
                AND ct.TagId = @ModuleTagId
            )
        )
    ORDER BY r.ReleaseDate DESC;
    
    -- Return the filtered releases
    SELECT * FROM #FilteredReleases;
    
    DROP TABLE #FilteredReleases;
END
GO

-- =============================================
-- SP: Get Release Statistics
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetReleaseStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetReleaseStatistics]
GO

CREATE PROCEDURE [dbo].[sp_GetReleaseStatistics]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(DISTINCT r.ReleaseId) AS TotalReleases,
        COUNT(c.ChangeId) AS TotalChanges,
        SUM(CASE WHEN c.ChangeType = 'bug-fix' THEN 1 ELSE 0 END) AS BugFixCount,
        SUM(CASE WHEN c.ChangeType = 'new-feature' THEN 1 ELSE 0 END) AS NewFeatureCount,
        SUM(CASE WHEN c.ChangeType = 'enhancement' THEN 1 ELSE 0 END) AS EnhancementCount,
        MIN(r.ReleaseDate) AS FirstReleaseDate,
        MAX(r.ReleaseDate) AS LatestReleaseDate
    FROM Releases r
    LEFT JOIN Changes c ON r.ReleaseId = c.ReleaseId;
END
GO

-- =============================================
-- SP: Get Popular Tags
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetPopularTags]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetPopularTags]
GO

CREATE PROCEDURE [dbo].[sp_GetPopularTags]
    @TopN INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopN)
        t.TagId,
        t.Label,
        t.Value,
        t.Type,
        COUNT(ct.ChangeId) AS UsageCount
    FROM Tags t
    INNER JOIN ChangeTags ct ON t.TagId = ct.TagId
    WHERE t.Type = 'module'
    GROUP BY t.TagId, t.Label, t.Value, t.Type
    ORDER BY COUNT(ct.ChangeId) DESC;
END
GO

-- =============================================
-- SP: Search Changes
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SearchChanges]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SearchChanges]
GO

CREATE PROCEDURE [dbo].[sp_SearchChanges]
    @SearchTerm NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ChangeId,
        c.ReleaseId,
        c.Description,
        c.ChangeType,
        c.CreatedAt,
        c.UpdatedAt,
        r.Version,
        r.ReleaseDate
    FROM Changes c
    INNER JOIN Releases r ON c.ReleaseId = r.ReleaseId
    WHERE 
        c.Description LIKE '%' + @SearchTerm + '%'
        OR r.Version LIKE '%' + @SearchTerm + '%'
    ORDER BY r.ReleaseDate DESC, c.CreatedAt DESC;
END
GO

-- =============================================
-- SP: Get Version List
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetVersionList]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetVersionList]
GO

CREATE PROCEDURE [dbo].[sp_GetVersionList]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ReleaseId,
        Version,
        ReleaseDate,
        (SELECT COUNT(*) FROM Changes WHERE ReleaseId = r.ReleaseId) AS ChangeCount
    FROM Releases r
    ORDER BY ReleaseDate DESC;
END
GO

PRINT 'Enhanced query stored procedures created successfully';
GO

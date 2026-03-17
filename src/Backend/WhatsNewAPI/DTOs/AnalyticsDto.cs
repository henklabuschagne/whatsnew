using System;
using System.Collections.Generic;

namespace WhatsNewAPI.DTOs
{
    public class ReleaseTimelineDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int ReleaseCount { get; set; }
        public int TotalChanges { get; set; }
        public int BugFixes { get; set; }
        public int NewFeatures { get; set; }
        public int Enhancements { get; set; }
    }

    public class ModuleDistributionDto
    {
        public Guid TagId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleValue { get; set; }
        public int ChangeCount { get; set; }
        public int BugFixes { get; set; }
        public int NewFeatures { get; set; }
        public int Enhancements { get; set; }
    }

    public class ChangeTypeDistributionDto
    {
        public string ChangeType { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RecentActivityDto
    {
        public string ActivityType { get; set; }
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public string Description { get; set; }
        public DateTime ActivityDate { get; set; }
    }

    public class ReleaseVelocityDto
    {
        public int ReleasesLast30Days { get; set; }
        public int ReleasesLast90Days { get; set; }
        public int ReleasesLast365Days { get; set; }
        public decimal? AvgDaysBetweenReleases { get; set; }
    }

    public class TopReleaseDto
    {
        public Guid ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ChangeCount { get; set; }
        public int BugFixes { get; set; }
        public int NewFeatures { get; set; }
        public int Enhancements { get; set; }
    }

    public class DashboardSummaryDto
    {
        public int TotalReleases { get; set; }
        public int TotalChanges { get; set; }
        public int TotalModules { get; set; }
        public int ReleasesThisMonth { get; set; }
        public int ChangesThisMonth { get; set; }
        public DateTime? LatestReleaseDate { get; set; }
        public string LatestVersion { get; set; }
    }

    public class ChangeTrendDto
    {
        public DateTime Date { get; set; }
        public int TotalChanges { get; set; }
        public int BugFixes { get; set; }
        public int NewFeatures { get; set; }
        public int Enhancements { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace WhatsNewAPI.DTOs
{
    public class ReleaseFilterDto
    {
        public string SearchTerm { get; set; }
        public string ChangeType { get; set; }
        public Guid? ModuleTagId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class ReleaseStatisticsDto
    {
        public int TotalReleases { get; set; }
        public int TotalChanges { get; set; }
        public int BugFixCount { get; set; }
        public int NewFeatureCount { get; set; }
        public int EnhancementCount { get; set; }
        public DateTime? FirstReleaseDate { get; set; }
        public DateTime? LatestReleaseDate { get; set; }
    }

    public class PopularTagDto
    {
        public Guid TagId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public int UsageCount { get; set; }
    }

    public class VersionListItemDto
    {
        public Guid ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ChangeCount { get; set; }
    }

    public class ChangeSearchResultDto
    {
        public Guid ChangeId { get; set; }
        public Guid ReleaseId { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}

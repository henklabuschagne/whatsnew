using System;
using System.ComponentModel.DataAnnotations;

namespace WhatsNewAPI.DTOs
{
    // =============================================
    // Client DTOs
    // =============================================

    public class ClientDto
    {
        public Guid ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ClientCreateDto
    {
        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Code must contain only uppercase letters, numbers, underscores, and hyphens")]
        public string Code { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? ContactEmail { get; set; }

        [Phone]
        [StringLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ClientUpdateDto
    {
        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Code must contain only uppercase letters, numbers, underscores, and hyphens")]
        public string Code { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? ContactEmail { get; set; }

        [Phone]
        [StringLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; }
    }

    public class ClientStatisticsDto
    {
        public Guid ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int TotalChanges { get; set; }
        public int BugFixes { get; set; }
        public int Enhancements { get; set; }
        public int NewFeatures { get; set; }
        public DateTime? FirstChangeDate { get; set; }
        public DateTime? LastChangeDate { get; set; }
    }

    // =============================================
    // Enhanced Analytics DTOs
    // =============================================

    public class ClientDistributionDto
    {
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
        public int ChangeCount { get; set; }
        public int BugFixes { get; set; }
        public int Enhancements { get; set; }
        public int NewFeatures { get; set; }
        public int Count { get; set; }
        public int Percentage { get; set; }
    }

    public class TimeToActionMetricsDto
    {
        public List<ChangeTypeMetricDto> ByChangeType { get; set; } = new();
        public List<TimelineDataDto> Timeline { get; set; } = new();
        public OverallMetricsDto Overall { get; set; } = new();
    }

    public class ChangeTypeMetricDto
    {
        public string ChangeType { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public double AverageTotalTime { get; set; }
        public double AverageDevTime { get; set; }
        public double AverageTestTime { get; set; }
        public double AverageReleaseTime { get; set; }
        public double SubmittedToDeveloped { get; set; }
        public double DevelopedToTested { get; set; }
        public double TestedToReleased { get; set; }
        public int Count { get; set; }
    }

    public class TimelineDataDto
    {
        public string Month { get; set; } = string.Empty;
        public string MonthName { get; set; } = string.Empty;
        public double? BugFix { get; set; }
        public double? Enhancement { get; set; }
        public double? NewFeature { get; set; }
    }

    public class OverallMetricsDto
    {
        public double AverageTotalTime { get; set; }
        public double FastestCompletion { get; set; }
        public double SlowestCompletion { get; set; }
        public double MedianTime { get; set; }
    }

    public class TimeToActionDto
    {
        public Guid TimeToActionId { get; set; }
        public Guid ChangeId { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? DevelopedDate { get; set; }
        public DateTime? TestedDate { get; set; }
        public DateTime? ReleasedDate { get; set; }
        public int? TotalDays { get; set; }
        public int? DevDays { get; set; }
        public int? TestDays { get; set; }
        public int? ReleaseDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TimeToActionUpdateDto
    {
        public Guid ChangeId { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? DevelopedDate { get; set; }
        public DateTime? TestedDate { get; set; }
        public DateTime? ReleasedDate { get; set; }
    }

    // =============================================
    // Enhanced Change DTOs with Client Tracking
    // =============================================

    public class EnhancedChangeDto
    {
        public Guid ChangeId { get; set; }
        public Guid ReleaseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientCode { get; set; }
        public string? TicketNumber { get; set; }
        public string? DevOpsNumber { get; set; }
        public List<string> ModuleTags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class EnhancedChangeCreateDto
    {
        [Required]
        public Guid ReleaseId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(5000, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(bug-fix|enhancement|new-feature)$")]
        public string ChangeType { get; set; } = string.Empty;

        public List<Guid> TagIds { get; set; } = new();

        public Guid? ClientId { get; set; }

        [StringLength(100)]
        public string? TicketNumber { get; set; }

        [StringLength(100)]
        public string? DevOpsNumber { get; set; }
    }

    public class EnhancedChangeUpdateDto
    {
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(5000, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(bug-fix|enhancement|new-feature)$")]
        public string ChangeType { get; set; } = string.Empty;

        public List<Guid> TagIds { get; set; } = new();

        public Guid? ClientId { get; set; }

        [StringLength(100)]
        public string? TicketNumber { get; set; }

        [StringLength(100)]
        public string? DevOpsNumber { get; set; }
    }
}

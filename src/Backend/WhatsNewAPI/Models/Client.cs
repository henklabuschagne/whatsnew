using System;

namespace WhatsNewAPI.Models
{
    public class Client
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

    public class TimeToAction
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
}
